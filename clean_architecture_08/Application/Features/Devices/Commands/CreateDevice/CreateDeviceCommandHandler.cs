using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Devices.Commands.CreateDevice;

/// <summary>
/// Command-Handler zum Erstellen eines neuen Geräts.
/// </summary>
public sealed class CreateDeviceCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateDeviceCommand, Result<GetDeviceDto>>
{
    public async Task<Result<GetDeviceDto>> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
    {
        var entity = Device.Create(request.SerialNumber, request.DeviceName, request.DeviceType);
        await uow.Devices.AddAsync(entity, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<GetDeviceDto>.Created(entity.Adapt<GetDeviceDto>());
    }
}

