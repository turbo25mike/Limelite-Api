using System;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Linq;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AvengerController : ControllerBase
{
    [HttpGet]
    [Route("")]
    public IEnumerable<ShortAvenger> Get() => Avengers.Team.Assemble();

    [HttpGet]
    [Route("{id}")]
    public Avenger? Get(string id) => Avengers.Team.Get(id);

    [HttpPost]
    [Authorize]
    [Route("")]
    public void Post([FromBody] Avenger user) => Avengers.Team.Add(user);

    [HttpPut]
    [Authorize]
    [Route("{id}")]
    public void Put(string id, [FromBody] JsonPatchDocument<Avenger> data) => Avengers.Team.Update(id, data);

    [HttpDelete]
    [Authorize]
    [Route("{id}")]
    public void Delete(string id) => Avengers.Team.Delete(id);
}

public class ShortAvenger
{
    public static ShortAvenger Create(Avenger user) => new() { ID = user.ID, FullName = $"{user.FirstName} {user.LastName}" };

    [JsonPropertyName("id")]
    public string? ID { get; set; }
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }
}

public partial class Avenger : Model
{
    [JsonPropertyName("acceptedEulaVersion")]
    public int? AcceptedEulaVersion { get; set; }

    [JsonPropertyName("autoMapPinResults")]
    public bool AutoMapPinResults { get; set; } = true;

    [JsonPropertyName("isAdmin")]
    public bool IsAdmin { get; set; }

    [JsonPropertyName("measurementStandard")]
    public MeasurementStandard MeasurementStandard;

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
}

public enum MeasurementStandard { Yards, Meters }

public sealed class Avengers
{
    private Avengers()
    {
        //Default Team
        _Team = new()
        {
            new(){ AcceptedEulaVersion = 1, FirstName = "Thor", LastName = "Odinson" },
            new(){ AcceptedEulaVersion = 2, FirstName = "Natasha", LastName = "Rominoff" },
            new(){ AcceptedEulaVersion = 2, FirstName = "Tony", LastName = "Stark" },
            new(){ AcceptedEulaVersion = 2, FirstName = "Steve", LastName = "Rodgers" },
            new(){ AcceptedEulaVersion = 2, FirstName = "Clint", LastName = "Barton" },
            new(){ AcceptedEulaVersion = 2, FirstName = "Peter", LastName = "Parker" },
            new(){ AcceptedEulaVersion = 2, FirstName = "James", LastName = "Rhodes" },
            new(){ AcceptedEulaVersion = 2, FirstName = "Carol", LastName = "Danvers" }
        };
    }

    private static readonly Lazy<Avengers> _Lazy = new(() => new Avengers());
    public static Avengers Team => _Lazy.Value;

    public IEnumerable<ShortAvenger> Assemble() => _Team.Select(ShortAvenger.Create);

    public Avenger Get(string id)
    {
        var user = _Team.FirstOrDefault(user => user.ID == id);
        return user is null ? throw new ArgumentOutOfRangeException("User not found") : user;
    }

    public void Add(Avenger user) => _Team.Add(user);

    public void Update(string id, JsonPatchDocument<Avenger> patch) => patch.ApplyTo(Get(id));

    public void Delete(string id) => _Team.Remove(Get(id));

    private readonly List<Avenger> _Team;
}

