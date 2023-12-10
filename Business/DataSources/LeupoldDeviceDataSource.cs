using Business.DataStores;
using Business.Models;
using Business.Validators;
using Leupold.Models;

namespace Business.DataSources;

public interface ILeupoldDeviceDataSource : IDataSource<LeupoldDevice>
{
}

public class LeupoldDeviceDataSource : DataSource<LeupoldDevice>, ILeupoldDeviceDataSource
{
    public LeupoldDeviceDataSource(IValidator<LeupoldDevice> v, IApiSettings settings, IDatabase ds) : base(v, settings,
        ds)
    {
    }
}