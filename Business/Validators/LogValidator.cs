using Business.Models;
using Leupold.Models;

namespace Business.Validators;

public class LogValidator : Validator<Log>, IValidator<Log>
{
    public LogValidator(IApiSettings settings) : base(settings)
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

    public override bool CanAdd(Log entity)
    {
        return true;
    }

    public override bool CanUpdate(Log entity)
    {
        return false;
    }

    public override bool CanDelete(ShortGuid id)
    {
        return false;
    }
}