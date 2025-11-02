using Application.Common.Results;
using Application.Contracts;
using MediatR;

namespace Application.Features.Devices.Commands.DeleteDevice;

/// <summary>
/// Command-Handler zum Löschen eines Geräts.
/// </summary>
public sealed class DeleteDeviceCommandHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteDeviceCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        var entity = await uow.Devices.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return Result<bool>.NotFound($"Device mit ID {request.Id} nicht gefunden.");

        uow.Devices.Remove(entity);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<bool>.NoContent();
    }
}

