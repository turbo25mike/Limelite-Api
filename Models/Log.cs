namespace Models;

public interface ILog : IModel
{
    string Version { get; set; }
    string Platform { get; set; }
    string Model { get; set; }
    string Manufacturer { get; set; }
    string Data { get; set; }
    string AppVersion { get; set; }
    string GroupID { get; set; }
}

public class Log : Model, ILog
{
    [MySql(ColumnType = "varchar(20)")]
    public string Version { get; set; }

    [MySql(ColumnType = "varchar(20)")]
    public string Platform { get; set; }

    [MySql(ColumnType = "varchar(20)")]
    public string Model { get; set; }

    [MySql(ColumnType = "varchar(20)")]
    public string Manufacturer { get; set; }

    [MySql(ColumnType = "varchar(20)")]
    public string AppVersion { get; set; }

    [MySql(ColumnType = "varchar(8000)")]
    public string Data { get; set; }

    [MySql(ColumnType = "varchar(22)")]
    public string GroupID { get; set; }
}