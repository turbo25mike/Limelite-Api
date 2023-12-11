using Business.DataStores;
using Business.Models;
using Business.Validators;
using Models;

namespace Business.DataSources;

public interface ILogDataSource : IDataSource<Log>
{
}

public class LogDataSource : DataSource<Log>, ILogDataSource
{
    public LogDataSource(IValidator<Log> v, IApiSettings settings, IDatabase ds) : base(v, settings, ds)
    {
    }
}