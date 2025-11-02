using FluentValidation;

namespace Application.Features.People.Commands.CreatePerson;

public class CreatePersonValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonValidator()
    {
        // Domain validations are handled in the entity
    }
}

