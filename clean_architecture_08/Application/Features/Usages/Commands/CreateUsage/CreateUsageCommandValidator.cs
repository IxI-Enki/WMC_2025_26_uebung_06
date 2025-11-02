using FluentValidation;

namespace Application.Features.Usages.Commands.CreateUsage;

public class CreateUsageValidator : AbstractValidator<CreateUsageCommand>
{
    public CreateUsageValidator()
    {
        // Domain validations are handled in the entity
    }
}

