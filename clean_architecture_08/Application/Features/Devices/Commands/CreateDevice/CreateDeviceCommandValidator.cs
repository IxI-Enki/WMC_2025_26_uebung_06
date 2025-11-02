using FluentValidation;

namespace Application.Features.Devices.Commands.CreateDevice;

public class CreateDeviceValidator : AbstractValidator<CreateDeviceCommand>
{
    public CreateDeviceValidator()
    {
        // Domain validations are handled in the entity
    }
}

