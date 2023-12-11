using Business.Models;
using Models;

namespace Business.Validators;

public interface IValidator<T>
{
    bool CanGet();
    bool CanGet(ShortGuid id);
    bool CanGet(ShortGuid[] id);
    bool CanGetAll();
    bool CanAdd(T entity);
    bool CanUpdate(T entity);
    bool CanDelete(ShortGuid id);
}

public class Validator<T> : IValidator<T>
{
    protected readonly IApiSettings AppSettings;

    public Validator(IApiSettings settings)
    {
        AppSettings = settings;
    }

    protected bool IsSiteAdmin => AppSettings.CurrentUser.IsAdmin;

    public virtual bool CanGet()
    {
        return true;
    }

    public virtual bool CanGet(ShortGuid id)
    {
        return true;
    }

    public virtual bool CanGet(ShortGuid[] id)
    {
        return true;
    }

    public virtual bool CanGetAll()
    {
        return true;
    }

    public virtual bool CanAdd(T entity)
    {
        return true;
    }

    public virtual bool CanUpdate(T entity)
    {
        return true;
    }

    public virtual bool CanDelete(ShortGuid id)
    {
        return true;
    }
}