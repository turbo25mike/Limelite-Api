namespace Models;

// ReSharper disable once PartialTypeWithSinglePart
public partial class User : Model
{
    [ObservableProperty]
    private int? _AcceptedEulaVersion;

    [ObservableProperty]
    private bool _IsAdmin;

    [ObservableProperty]
    private string _CurrentEnvironment;

}

public partial class UserDevice : Model
{
    [MySql(ColumnType = "varchar(20)")]
    public string AppVersion { get; set; }

    [MySql(ColumnType = "varchar(20)")]
    public string Version { get; set; }

    [MySql(ColumnType = "varchar(20)")]
    public string Platform { get; set; }

    [MySql(ColumnType = "varchar(20)")]
    public string Model { get; set; }

    [MySql(ColumnType = "varchar(20)")]
    public string Manufacturer { get; set; }
}

public class Auth0User
{
    public bool email_verified { get; set; }
    public string email { get; set; }
    public string clientID { get; set; }
    public DateTime updated_at { get; set; }
    public string name { get; set; }
    public string picture { get; set; }
    public string user_id { get; set; }
    public string nickname { get; set; }
    public string given_name { get; set; }
    public string family_name { get; set; }
    public DateTime created_at { get; set; }
    public AppMetaData app_metadata { get; set; }
    public string sub { get; set; }
}

public class AppMetaData
{
    public bool AutoMapPinResults { get; set; }
    public int? AcceptedEulaVersion { get; set; }
}