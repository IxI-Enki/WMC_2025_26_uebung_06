using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using Mapster;
using MediatR;

namespace Application.Features.Devices.Queries.GetAllDevices;

public sealed class GetAllDevicesQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetAllDevicesQuery, Result<IReadOnlyCollection<GetDeviceDto>>>
{
    public async Task<Result<IReadOnlyCollection<GetDeviceDto>>> Handle(GetAllDevicesQuery request,
        CancellationToken cancellationToken)
    {
        var devices = await uow.Devices.GetAllAsync(
            orderBy: q => q.OrderBy(d => d.DeviceName), ct: cancellationToken);
        var dtos = devices.Adapt<IReadOnlyCollection<GetDeviceDto>>();
        return Result<IReadOnlyCollection<GetDeviceDto>>.Success(dtos);
    }
}

