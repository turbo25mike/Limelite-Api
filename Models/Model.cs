namespace Models;

public interface IModel
{
    string ID { get; set; }
    string CreatedBy { get; set; }
    DateTime CreatedDate { get; set; }
    string UpdatedBy { get; set; }
    DateTime? UpdatedDate { get; set; }
    string RemovedBy { get; set; }
    DateTime? RemovedDate { get; set; }
}

[DBEntity]
public class Model : ObservableObject, IModel
{
    public Model()
    {
        ID = ShortGuid.NewGuid();
        CreatedDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
    }

    [PrimaryKey]
    [MySql(PK = true)]
    public string ID { get; set; }

    [MySql(ColumnType = "varchar(22)")]
    public string CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    [MySql(ColumnType = "varchar(22)")]
    public string UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    [MySql(ColumnType = "varchar(22)")]
    public string RemovedBy { get; set; }

    public DateTime? RemovedDate { get; set; }
}

[AttributeUsage(AttributeTargets.Class)]
public class DBEntityAttribute : Attribute
{
    public DBEntityAttribute(string route = "")
    {
        Route = route;
    }

    public string Route { get; set; }
}