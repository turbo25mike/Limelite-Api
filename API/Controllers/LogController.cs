using System.Collections.Generic;
using Business.DataSources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class LogController : ControllerBase
{
    private readonly ILogDataSource _DS;

    public LogController(ILogDataSource ds) => _DS = ds;

    [AllowAnonymous]
    [HttpGet]
    [Route("list")]
    public IEnumerable<Log> GetAll() => _DS.Get();

    [AllowAnonymous]
    [HttpPost]
    [Route("")]
    public void Post([FromBody] Log entity) => _DS.Insert(entity);
}