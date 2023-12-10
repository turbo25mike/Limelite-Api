using Business.DataStores;

namespace Business.DataSources;

public interface IAppDataSource
{
    bool IsDatabaseOnline();
    int DatabaseStatus();
}

public class AppDataSource : IAppDataSource
{
    private readonly IDatabase _DS;

    public AppDataSource(IDatabase ds)
    {
        _DS = ds;
    }

    public bool IsDatabaseOnline()
    {
        return _DS.Check() == -1;
    }

    public int DatabaseStatus()
    {
        return _DS.Check();
    }
}