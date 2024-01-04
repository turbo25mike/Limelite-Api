using System;
using System.Net.Http;
using System.Threading.Tasks;
using Business.DataSources;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("")]
public class HealthController : Controller
{
    private readonly IApiSettings _Settings;

    public HealthController(IApiSettings appSettings) => _Settings = appSettings;

    [HttpGet]
    [Route("")]
    public AppStatus Get() => new AppStatus() { Status = $"ver: {_Settings.Version}" };
}

[Authorize]
[Route("[controller]")]
[ApiController]
public class AppController : ControllerBase
{
    private readonly IAppDataSource _DS;
    private readonly IApiSettings _Settings;

    public AppController(IApiSettings appSettings, IAppDataSource ds)
    {
        _DS = ds;
        _Settings = appSettings;
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("environment")]
    public AppStatus Get() => new AppStatus() { Status = _Settings.Environment };

    [HttpGet]
    [AllowAnonymous]
    [Route("db/status")]
    public AppStatus GetDbStatus() => new AppStatus() { Status = _DS.IsDatabaseOnline() ? "online" : "offline" };

    [HttpGet]
    [AllowAnonymous]
    [Route("db/details")]
    public AppStatus GetDbDetails() => new AppStatus() { Status = _DS.DatabaseStatus().ToString() };

    //[AllowAnonymous]
    [HttpGet("ping")]
    public string Ping() => "{ \"status\" : \"healthy\" }";

    [AllowAnonymous]
    [HttpGet("ping/ext")]
    public async Task<string> PingAddress([FromQuery] string address)
    {
        try
        {
            var client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(address);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return $"Ping: was successful";
            }

            return $"Ping failed: {response.StatusCode}";
        }
        catch (Exception ex)
        {
            return $"Ping failed with exception: {ex.Message}";
        }
    }
}