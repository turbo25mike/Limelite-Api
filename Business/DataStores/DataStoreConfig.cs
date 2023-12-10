using System.Collections.Generic;

namespace Business.DataStores;

public enum DataStoreAction
{
    Get,
    Delete,
    Update,
    Insert,
    Execute
}

public class DataStoreConfig
{
    public DataStoreConfig(string table = "")
    {
        Table = table;
    }

    public bool Truncate { get; set; }
    public string Table { get; set; }
    public bool CreateContainer { get; set; }
    public string[] Properties { get; set; }
    public DataStoreWhere[] WhereStatements { get; set; }
    public IDictionary<string, string> Mapping { get; set; }
    public int? Limit { get; set; }
    public int Page { get; set; }

    public static DataStoreConfig CreateGet<T>(string propName, object propVal = null,
        WhereAction action = WhereAction.EqualTo, int? limit = 1)
    {
        return new DataStoreConfig(typeof(T).Name)
        {
            Limit = limit,
            WhereStatements = new[] { new DataStoreWhere(propName, action, propVal) }
        };
    }

    public static DataStoreConfig CreateUpdate<T>(string[] propertiesToUpdate, string propIDName, object ID)
    {
        return new DataStoreConfig(typeof(T).Name)
        {
            Properties = propertiesToUpdate,
            WhereStatements = new[] { new DataStoreWhere(propIDName, WhereAction.EqualTo, ID) }
        };
    }

    public static DataStoreConfig CreateDelete<T>(string propName, object propVal = null)
    {
        return new DataStoreConfig(typeof(T).Name)
        {
            WhereStatements = new[] { new DataStoreWhere(propName, WhereAction.EqualTo, propVal) }
        };
    }
}

public class DataStoreWhere
{
    public DataStoreWhere(string propertyName = null, WhereAction? action = null, object propertyValue = null,
        WhereJoinAction? joinAction = null)
    {
        PropertyName = propertyName;
        PropertyValue = propertyValue;
        if (action != null) Action = action.Value;
        JoinAction = joinAction;
    }

    public string PropertyName { get; set; }
    public object PropertyValue { get; set; }
    public WhereAction Action { get; set; }
    public WhereJoinAction? JoinAction { get; set; }

    public static string ConvertWhereAction(WhereAction action)
    {
        switch (action)
        {
            case WhereAction.EqualTo: return "=";
            case WhereAction.GreaterThan: return ">";
            case WhereAction.GreaterThanOrEqualTo: return ">=";
            case WhereAction.In: return "IN";
            case WhereAction.IsNot: return "IS NOT";
            case WhereAction.Is: return "IS";
            case WhereAction.LessThan: return "<";
            case WhereAction.LessThanOrEqualTo: return "<=";
            case WhereAction.NotEqualTo: return "<>";
            default: return "";
        }
    }
}

public enum WhereAction
{
    EqualTo,
    NotEqualTo,
    GreaterThan,
    GreaterThanOrEqualTo,
    LessThan,
    LessThanOrEqualTo,
    In,
    Is,
    IsNot
}

public enum WhereJoinAction
{
    And,
    Or
}