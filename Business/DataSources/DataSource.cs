using System;
using System.Collections.Generic;
using System.Linq;
using Business.DataStores;
using Business.Models;
using Business.Validators;
using Leupold.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace Business.DataSources;

public interface IDataSource<T> where T : class
{
    IEnumerable<T> Get();
    T Get(ShortGuid id);
    IEnumerable<T> Get(IEnumerable<ShortGuid> ids);
    IEnumerable<T> Get(DataStoreConfig config);
    void Insert(T data);
    void Insert(IEnumerable<T> data);
    void Update(ShortGuid id, JsonPatchDocument<T> patch);

    void Delete(string id);
    //bool Patch(DBPacket packet, string userID);
}

public class DataSource<T> : IDataSource<T> where T : class
{
    //public bool Patch(DBPacket packet, string userID)
    //{
    //    var packetData = JsonConvert.DeserializeObject<Dictionary<string, object>>(packet.Data);
    //    if (packet.TableName == nameof(User))
    //    {
    //        if (packet.ID != userID) return false;
    //        if (packetData.Any(x => x.Key == "Hash" || x.Key == "Salt" || x.Key == "Attempts")) return false; //can't change other user's data or change password here
    //    }
    //    switch (packet.Action)
    //    {
    //        case DBPacketAction.Insert:
    //            packetData.AddPropertyToDictionary("CreatedBy", userID);
    //            packetData.AddPropertyToDictionary("CreatedDate", DateTime.Now);
    //            packetData.AddPropertyToDictionary("UpdatedBy", userID);
    //            packetData.AddPropertyToDictionary("UpdatedDate", DateTime.Now);
    //            Insert(new DataStoreConfig { Container = packet.TableName }, packetData);
    //            break;
    //        case DBPacketAction.Update:
    //            packetData.AddPropertyToDictionary("UpdatedBy", userID);
    //            packetData.AddPropertyToDictionary("UpdatedDate", DateTime.Now);
    //            var dbItem = GetSingle(DataStoreConfig.CreateGet("ID", packet.ID, packet.TableName));
    //            if (dbItem == null) return false;

    //            foreach (var sourceKey in dbItem.Keys.ToList())                    //compare data
    //                if (packetData.ContainsKey(sourceKey))
    //                    dbItem[sourceKey] = packetData[sourceKey];

    //            Update(DataStoreConfig.CreateUpdate(dbItem.Keys.ToArray(), "ID", packet.ID, packet.TableName), new[] { dbItem });
    //            break;
    //        case DBPacketAction.Delete:
    //            packetData.AddPropertyToDictionary("RemovedBy", userID);
    //            packetData.AddPropertyToDictionary("RemovedDate", DateTime.Now);
    //            var dbItemDelete = GetSingle(DataStoreConfig.CreateGet("ID", packet.ID, packet.TableName));
    //            if (dbItemDelete == null) return true;
    //            Delete(DataStoreConfig.CreateDelete("ID", packet.ID, packet.TableName));
    //            break;
    //    }
    //    return true;
    //}

    public readonly IDatabase _DB;
    public readonly IApiSettings _Settings;

    public DataSource(IValidator<T> v, IApiSettings settings, IDatabase db)
    {
        _DB = db;
        _Settings = settings;
        Validator = v;
    }

    public IValidator<T> Validator { get; }

    public IEnumerable<T> Get()
    {
        var config = new DataStoreConfig(typeof(T).Name)
        {
            WhereStatements = new[]
            {
                //new DataStoreWhere("CreatedBy", WhereAction.EqualTo, _Settings.CurrentUser.ID, WhereJoinAction.And),
                new DataStoreWhere("RemovedBy", WhereAction.Is)
            }
        };
        return Get(config);
    }

    public T Get(ShortGuid id)
    {
        var config = new DataStoreConfig(typeof(T).Name)
        {
            WhereStatements = new[]
            {
                new DataStoreWhere("ID", WhereAction.EqualTo, id, WhereJoinAction.And),
                new DataStoreWhere("RemovedBy", WhereAction.Is)
            } //TODO add when Auth0 is applied - new DataStoreWhere("CreatedBy", WhereAction.EqualTo, _Settings.CurrentUser.ID, WhereJoinAction.And),
        };
        return Get(config).FirstOrDefault();
    }

    public IEnumerable<T> Get(IEnumerable<ShortGuid> ids)
    {
        var config = new DataStoreConfig(typeof(T).Name)
        {
            WhereStatements = new[]
            {
                new DataStoreWhere("ID", WhereAction.In, ids.Select(x => x.ToString()).ToArray(), WhereJoinAction.And),
                new DataStoreWhere("CreatedBy", WhereAction.EqualTo, _Settings.CurrentUser.ID, WhereJoinAction.And),
                new DataStoreWhere("RemovedBy", WhereAction.Is)
            }
        };
        return Get(config);
    }

    public IEnumerable<T> Get(DataStoreConfig config)
    {
        return _DB.Query(config).ToObject<T>();
    }

    public void Insert(T data)
    {
        if (Validator.CanAdd(data)) _DB.Insert(data);
    }

    public void Insert(IEnumerable<T> data)
    {
        _DB.Insert(data);
    }

    public void Update(ShortGuid id, JsonPatchDocument<T> patch)
    {
        var entity = Get(id);
        if (entity == null)
            return;
        if (Validator.CanUpdate(entity))
        {
            patch.ApplyTo(entity);
            _DB.Update<T>(id, entity.ToDictionary());
        }
    }

    public void Delete(string id)
    {
        if (!Validator.CanDelete(id)) return;
        _DB.Delete<T>(id);
    }
}