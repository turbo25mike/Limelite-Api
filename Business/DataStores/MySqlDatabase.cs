using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Business.Models;
using Dapper;
using Leupold.Models;
using MySql.Data.MySqlClient;
using Z.BulkOperations;
using MySqlAttribute = Leupold.Models.MySqlAttribute;

namespace Business.DataStores;

public interface IDatabase
{
    int Check();
    IDictionary<string, object> QuerySingle(DataStoreConfig config);
    IDictionary<string, object>[] Query(DataStoreConfig config);
    void Insert<T>(T data);
    void Insert<T>(IEnumerable<T> data);
    void Update<T>(ShortGuid id, object data);
    void Update<T>(ShortGuid id, IDictionary<string, object> data);
    void Update<T>(string id, IDictionary<string, object> data);
    void Delete<T>(ShortGuid id);
}

public class MySqlDatabase : IDatabase
{
    private readonly IDbConnection _DB;
    private readonly IApiSettings _Settings;

    public MySqlDatabase(IApiSettings appSettings)
    {
        _Settings = appSettings;
        _DB = new MySqlConnection(appSettings.DB_Connection);
    }

    public IDictionary<string, object> QuerySingle(DataStoreConfig config)
    {
        return Run(() => RunSelect(config).FirstOrDefault());
    }

    public IDictionary<string, object>[] Query(DataStoreConfig config)
    {
        return Run(() => RunSelect(config));
    }

    public int Check()
    {
        int exception = -1;
        try
        {
            _DB.Open();
        }
        catch (MySqlException ex)
        {
            exception = ex.Number;
            switch (ex.Number)
            {
                case 1042: // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                    break;
                case 0: // Access denied (Check DB name,username,password)
                    break;
            }
        }
        finally
        {
            if (_DB.State == ConnectionState.Open) _DB.Close();
        }

        return exception;
    }


    public void Insert<T>(T data)
    {
        Run(() =>
        {
            CreateTable<T>();
            if (typeof(T).Name != nameof(User) && _Settings.CurrentUser != null)
                data.GetType().GetProperty("CreatedBy").SetValue(data, _Settings.CurrentUser.ID);

            var sql =
                $"INSERT INTO {typeof(T).Name} ({CreateSQLProps(data.ToDictionary())}) VALUES ({CreateSQLValues(data.ToDictionary())})";
            _DB.Execute(sql);
            return true;
        });
    }

    public void Insert<T>(IEnumerable<T> data)
    {
        if (typeof(T).Name != nameof(User) && _Settings.CurrentUser != null)
            foreach (var d in data)
                data.GetType().GetProperty("CreatedBy").SetValue(data, _Settings.CurrentUser?.ID);

        Run(() =>
        {
            CreateTable<T>();
            var dt = DataTableExt.CreateTable(new[] { data.ToDictionary() });
            var bulk = new BulkOperation(_DB as MySqlConnection)
            {
                DestinationTableName = typeof(T).Name
            };
            bulk.BulkInsert(dt, DataRowState.Added);
            return true;
        });
    }

    public void Update<T>(ShortGuid id, object data)
    {
        Update<T>(id.ToString(), data.ToDictionary());
    }

    public void Update<T>(ShortGuid id, IDictionary<string, object> data)
    {
        Update<T>(id.ToString(), data);
    }

    public void Update<T>(string id, IDictionary<string, object> data)
    {
        Run(() =>
        {
            if (!TableExists<T>()) return false;
            if (!data.ContainsKey("UpdatedBy"))
                data.Add("UpdatedBy", _Settings.CurrentUser?.ID);
            if (data.ContainsKey("CreatedBy"))
                data.Remove("CreatedBy");
            if (data.ContainsKey("CreatedDate"))
                data.Remove("CreatedDate");
            var set = string.Join(", ", data.Select(k => $"`{k.Key}` = {ConvertPropValueToSql(k.Value)}"));
            var sql = $"UPDATE {typeof(T).Name} SET {set} WHERE `ID` = '{id}'";
            _DB.Execute(sql);
            return true;
        });
    }

    public void Delete<T>(ShortGuid id)
    {
        var data = new { Deleted = true };
        Update<T>(id, data.ToDictionary());
    }

    protected T Run<T>(Func<T> function)
    {
        try
        {
            _DB.Open();
            return function.Invoke();
        }
        finally
        {
            _DB.Close();
        }
    }

    protected bool TableExists<T>()
    {
        return TableExists(typeof(T).Name);
    }

    protected bool TableExists(string tableName)
    {
        var res = _DB.Query<string>(
            "SELECT table_name FROM information_schema.tables WHERE table_schema = @Database AND table_name = @Name;",
            new { Database = GetValueFromConnectionString("database"), Name = tableName });
        return res != null && res.Any();
    }

    protected void CreateTable<T>()
    {
        if (TableExists<T>()) return;

        var props = new Dictionary<string, MySqlAttribute>();
        foreach (var p in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var att = p.GetCustomAttribute<MySqlAttribute>();
            if (att != null && att.Ignore) continue;

            if (att == null)
                att = new MySqlAttribute();

            if (att.ColumnType == null)
                att.ColumnType = GetMySqlColumnType(p);

            props.Add(p.Name, att);
        }

        var table = $"`{typeof(T).Name}`";
        var columnList = props.Select(p => p.Key).ToArray();
        var columns = string.Join(",", columnList.Select(p => $"`{p}` {props[p].ColumnType} NULL"));

        var sql = $"CREATE TABLE {table} ({columns});";

        _DB.Execute(sql);
    }

    private string GetMySqlColumnType(PropertyInfo p)
    {
        var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

        if (t == typeof(string))
            return "varchar(1000)";
        if (t == typeof(int))
            return "int(11)";
        if (t == typeof(bool))
            return "boolean";
        if (t == typeof(DateTime))
            return "datetime";
        if (t == typeof(double))
            return "decimal(14,2)";

        return "varchar(1000)";
    }

    private string GetConnectionStringPropertyValue(string connectionString, string propName)
    {
        foreach (var prop in connectionString.Split(';'))
            if (prop.Length > 0)
            {
                var p = prop.Trim().Split('=');
                if (p != null && p.Length > 0 && p[0].Trim().ToLower() == propName.ToLower()) return p[1].Trim();
            }

        throw new ArgumentOutOfRangeException();
    }

    private IDictionary<string, object>[] RunSelect(DataStoreConfig config)
    {
        if (!TableExists(config.Table)) return new Dictionary<string, object>[0];
        var props = config.Properties != null ? string.Join(",", config.Properties) : "*";
        var where = GenWhere(config);
        var table = config.Table;
        var mysqlLimit = config.Limit > 0 ? $"Limit {config.Limit}" : "";
        var sql = $"SELECT {props} FROM {table} {where} {mysqlLimit}";
        var data = _DB.Query(sql) as IEnumerable<IDictionary<string, object>>;
        var result = data.Select(r => r.ToDictionary(d => d.Key, d => d.Value)).ToArray();
        return result;
    }

    private string GenWhere(DataStoreConfig config)
    {
        if (config.WhereStatements == null || !config.WhereStatements.Any()) return "";
        var whereStrings = config.WhereStatements.Select(i =>
            $"`{i.PropertyName}` {DataStoreWhere.ConvertWhereAction(i.Action)} {ConvertPropValueToSql(i.PropertyValue)} {i.JoinAction?.ToString()}");
        return "WHERE " + string.Join(" ", whereStrings);
    }

    private string CreateSQLValues(IDictionary<string, object> data)
    {
        return string.Join(",", data.Select(kvp => ConvertPropValueToSql(kvp.Value)));
    }

    private string CreateSQLProps(IDictionary<string, object> data)
    {
        return string.Join(",", data.Select(kvp => $"`{kvp.Key}`"));
    }

    private object ConvertPropValueToSql(object val)
    {
        if (val == null) return "NULL";
        if (val is int || val is long || val is decimal) return val;
        if (val is bool) return (bool)val ? 1 : 0;
        if (val is Array)
        {
            var items = new List<string>();
            foreach (var o in (Array)val)
                items.Add(ConvertPropValueToSql(o).ToString());
            return "(" + string.Join(",", items) + ")";
        }

        if (val is DateTime) return $"'{((DateTime)val).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}'";
        val = val.ToString().Replace("'", "''"); //escaping single quotes
        return $"'{val}'";
    }

    private string GetValueFromConnectionString(string propName)
    {
        if (!_Settings.DB_Connection.Contains(propName)) return "";
        return _Settings.DB_Connection.Split(";").First(x => x.Contains(propName)).Split("=")[1];
    }

    private int GetColumnLength(string colName, IDictionary<string, object>[] data)
    {
        return data.Max(d => d[colName].ToString().Length);
    }

    private string GetPropertyValue(string connectionString, string propName)
    {
        foreach (var prop in connectionString.Split(';'))
            if (prop.Length > 0)
            {
                var p = prop.Trim().Split('=');
                if (p != null && p.Length > 0 && p[0].Trim().ToLower() == propName.ToLower())
                    return p[1].Trim();
            }

        throw new ArgumentOutOfRangeException();
    }
}