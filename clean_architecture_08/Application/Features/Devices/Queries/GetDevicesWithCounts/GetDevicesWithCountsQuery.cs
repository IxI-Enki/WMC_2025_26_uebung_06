using Application.Common.Results;
using Application.Features.Dtos;
using MediatR;

namespace Application.Features.Devices.Queries.GetDevicesWithCounts;

public readonly record struct GetDevicesWithCountsQuery : IRequest<Result<IReadOnlyCollection<GetDeviceWithCountDto>>>;

