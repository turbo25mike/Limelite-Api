using Business.DataStores;
using Business.Models;
using Business.Validators;
using Models;

namespace Business.DataSources;

public interface IUserDataSource : IDataSource<User>
{
}

public class UserDataSource : DataSource<User>, IUserDataSource
{
    public UserDataSource(IValidator<User> v, IApiSettings settings, IDatabase ds) : base(v, settings, ds)
    {
    }
}