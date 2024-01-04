using Models;

namespace Business.Models;

public interface IApiSettings
{
    User CurrentUser { get; set; }
    string DB_Connection { get; set; }
    string Environment { get; set; }
    string SleepInSeconds { get; set; }
    string Auth0Domain { get; set; }
    string Auth0Audience { get; set; }
    string WeatherAPI { get; set; }
    string WeatherSecret { get; set; }
    string SysAccountID { get; set; }
    string Version { get; set; }
    bool Admin { get; set; }
}

public class ApiSettings : IApiSettings
{
    public User CurrentUser { get; set; }
    public string DB_Connection { get; set; }
    public string Environment { get; set; }
    public string SleepInSeconds { get; set; }
    public string Auth0Domain { get; set; }
    public string Auth0Audience { get; set; }
    public string WeatherAPI { get; set; }
    public string WeatherSecret { get; set; }
    public string SysAccountID { get; set; }
    public string Version { get; set; }
    public bool Admin { get; set; }
}