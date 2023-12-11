using Business.DataStores;
using Business.Models;
using Business.Validators;
using Models;

namespace Business.DataSources;

public interface IUserDeviceDataSource : IDataSource<UserDevice>
{
}

public class UserDeviceDataSource : DataSource<UserDevice>, IUserDeviceDataSource
{
    public UserDeviceDataSource(IValidator<UserDevice> v, IApiSettings settings, IDatabase ds) : base(v, settings, ds)
    {
    }
}