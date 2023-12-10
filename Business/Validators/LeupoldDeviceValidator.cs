using Business.Models;
using Leupold.Models;

namespace Business.Validators;

public class LeupoldDeviceValidator : Validator<LeupoldDevice>, IValidator<LeupoldDevice>
{
    public LeupoldDeviceValidator(IApiSettings settings) : base(settings)
    {
    }

    public override bool CanGet()
    {
        return false;
    }

    public override bool CanGet(ShortGuid id)
    {
        return true;
    }

    public override bool CanGet(ShortGuid[] id)
    {
        return false;
    }

    public override bool CanGetAll()
    {
        return false;
    }

    public override bool CanAdd(LeupoldDevice entity)
    {
        return true;
    }

    public override bool CanUpdate(LeupoldDevice entity)
    {
        return true;
    }

    public override bool CanDelete(ShortGuid id)
    {
        return false;
    }
}