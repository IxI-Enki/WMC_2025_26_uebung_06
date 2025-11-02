using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using MediatR;

namespace Application.Features.Devices.Queries.GetDevicesWithCounts;

public sealed class GetDevicesWithCountsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetDevicesWithCountsQuery, Result<IReadOnlyCollection<GetDeviceWithCountDto>>>
{
    public async Task<Result<IReadOnlyCollection<GetDeviceWithCountDto>>> Handle(GetDevicesWithCountsQuery request,
        CancellationToken cancellationToken)
    {
        var devices = await uow.Devices.GetAllWithCountAsync(cancellationToken);
        return Result<IReadOnlyCollection<GetDeviceWithCountDto>>.Success(devices);
    }
}

