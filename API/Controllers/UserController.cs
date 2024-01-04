using Business.DataSources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserDataSource _DS;

    public UserController(IUserDataSource ds) => _DS = ds;

    [HttpGet]
    [Route("{id}")]
    public User Get(string id) => _DS.Get((ShortGuid)id);

    [HttpPost]
    [Route("")]
    public void Post([FromBody] User entity) => _DS.Insert(entity);

    [HttpPut]
    [Route("{id}")]
    public void Put(string id, [FromBody] JsonPatchDocument<User> patchDocument) => _DS.Update(id, patchDocument);
}