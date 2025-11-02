using FluentValidation;

namespace Application.Features.Usages.Commands.UpdateUsage;

public class UpdateUsageValidator : AbstractValidator<UpdateUsageCommand>
{
    public UpdateUsageValidator()
    {
        // Domain validations are handled in the entity
    }
}

