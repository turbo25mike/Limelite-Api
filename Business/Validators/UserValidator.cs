using Business.Models;
using Leupold.Models;

namespace Business.Validators;

public class UserValidator : Validator<User>, IValidator<User>
{
    public UserValidator(IApiSettings settings) : base(settings)
    {
    }

    public override bool CanGet()
    {
        return false;
    }

    public override bool CanGet(ShortGuid id)
    {
        return false;
    }

    public override bool CanGet(ShortGuid[] id)
    {
        return false;
    }

    public override bool CanGetAll()
    {
        return false;
    }

    public override bool CanAdd(User entity)
    {
        return true;
    }

    public override bool CanUpdate(User entity)
    {
        return true;
    }

    public override bool CanDelete(ShortGuid id)
    {
        return false;
    }
}