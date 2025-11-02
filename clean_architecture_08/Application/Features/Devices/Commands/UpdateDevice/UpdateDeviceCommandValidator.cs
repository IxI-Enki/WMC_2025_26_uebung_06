using FluentValidation;

namespace Application.Features.Devices.Commands.UpdateDevice;

public class UpdateDeviceValidator : AbstractValidator<UpdateDeviceCommand>
{
    public UpdateDeviceValidator()
    {
        // Domain validations are handled in the entity
    }
}

