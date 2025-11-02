using Application.Common.Results;
using Application.Features.Dtos;
using MediatR;

namespace Application.Features.Devices.Queries.GetAllDevices;

public readonly record struct GetAllDevicesQuery : IRequest<Result<IReadOnlyCollection<GetDeviceDto>>>;

