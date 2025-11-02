using FluentValidation;

namespace Application.Features.People.Commands.UpdatePerson;

public class UpdatePersonValidator : AbstractValidator<UpdatePersonCommand>
{
    public UpdatePersonValidator()
    {
        // Domain validations are handled in the entity
    }
}

