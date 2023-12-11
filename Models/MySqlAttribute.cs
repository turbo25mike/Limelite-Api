namespace Models;

[AttributeUsage(AttributeTargets.Property)]
public class MySqlAttribute : Attribute
{
    //https://docs.microsoft.com/en-us/dotnet/standard/attributes/writing-custom-attributes
    public string Name { get; set; }
    public bool Ignore { get; set; }
    public bool PK { get; set; }
    public string ColumnType { get; set; }
}