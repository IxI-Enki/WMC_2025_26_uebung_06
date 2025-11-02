using Application.Common.Results;
using Application.Features.Dtos;
using MediatR;

namespace Application.Features.Devices.Queries.GetDeviceById;

public readonly record struct GetDeviceByIdQuery(int Id) : IRequest<Result<GetDeviceDto>>;

