using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using Mapster;
using MediatR;

namespace Application.Features.Devices.Commands.UpdateDevice;

/// <summary>
/// Command-Handler zum Aktualisieren eines vorhandenen Geräts.
/// </summary>
public sealed class UpdateDeviceCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateDeviceCommand, Result<GetDeviceDto>>
{
    public async Task<Result<GetDeviceDto>> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        var entity = await uow.Devices.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return Result<GetDeviceDto>.NotFound($"Device mit ID {request.Id} nicht gefunden.");

        entity.Update(request.SerialNumber, request.DeviceName, request.DeviceType);
        uow.Devices.Update(entity);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<GetDeviceDto>.Success(entity.Adapt<GetDeviceDto>());
    }
}

