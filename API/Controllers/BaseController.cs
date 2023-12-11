using System;
using System.Collections.Generic;
using Business.DataSources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("[controller]")]
public class BaseController<T> : Controller where T : class
{
    private readonly IDataSource<T> _DS;

    public BaseController(IDataSource<T> ds) => _DS = ds;

    [HttpGet]
    [Route("")]
    public IEnumerable<T> Get(IEnumerable<ShortGuid> ids) => _DS.Get(ids);

    [HttpGet]
    [Route("{id}")]
    public T Get(string id) => _DS.Get(id);

    [HttpDelete]
    [Route("{id}")]
    public void Delete(string id) => _DS.Delete(id);

    [HttpPatch]
    [Route("{id}")]
    public void Patch(string id, [FromBody] JsonPatchDocument<T> patchDocument) => _DS.Update(id, patchDocument);

    [HttpPut]
    [Route("{id}")]
    public void Put(string id, [FromBody] JsonPatchDocument<T> patchDocument) => _DS.Update(id, patchDocument);

    [HttpPost]
    [Route("")]
    public void Post([FromBody] T entity)
    {
        if (!ModelState.IsValid)
            throw new ArgumentOutOfRangeException(ModelState.Values.ToString());
        _DS.Insert(entity);
    }
}