using Business.DataSources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class UserDeviceController : ControllerBase
{
    private readonly IUserDeviceDataSource _DS;

    public UserDeviceController(IUserDeviceDataSource ds) => _DS = ds;

    [HttpGet]
    [Route("{id}")]
    public UserDevice Get(string id) => _DS.Get((ShortGuid)id);

    [HttpPost]
    [Route("")]
    public void Post([FromBody] UserDevice entity) => _DS.Insert(entity);

    [HttpPut]
    [Route("{id}")]
    public void Put(string id, [FromBody] JsonPatchDocument<UserDevice> patchDocument) => _DS.Update(id, patchDocument);
}