using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using Mapster;
using MediatR;

namespace Application.Features.Devices.Queries.GetDeviceById;

public sealed class GetDeviceByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetDeviceByIdQuery, Result<GetDeviceDto>>
{
    public async Task<Result<GetDeviceDto>> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken)
    {
        var device = await uow.Devices.GetByIdAsync(request.Id, cancellationToken);
        if (device is null) return Result<GetDeviceDto>.NotFound($"Device mit ID {request.Id} nicht gefunden.");

        return Result<GetDeviceDto>.Success(device.Adapt<GetDeviceDto>());
    }
}

