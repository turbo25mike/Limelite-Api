using Business.DataSources;
using Leupold.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class LeupoldDeviceController : ControllerBase
{
    private readonly ILeupoldDeviceDataSource _DS;

    public LeupoldDeviceController(ILeupoldDeviceDataSource ds) => _DS = ds;

    [AllowAnonymous]
    [HttpGet]
    [Route("{id}")]
    public LeupoldDevice Get(string id) => _DS.Get((ShortGuid)id);

    [AllowAnonymous]
    [HttpPost]
    [Route("")]
    public void Post([FromBody] LeupoldDevice entity) => _DS.Insert(entity);

    [AllowAnonymous]
    [HttpPut]
    [Route("{id}")]
    public void Put(string id, [FromBody] JsonPatchDocument<LeupoldDevice> patchDocument) => _DS.Update(id, patchDocument);
}