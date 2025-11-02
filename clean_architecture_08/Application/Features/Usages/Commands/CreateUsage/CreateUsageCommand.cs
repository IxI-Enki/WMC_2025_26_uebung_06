using Application.Common.Results;
using Application.Features.Dtos;
using MediatR;

namespace Application.Features.Usages.Commands.CreateUsage;

public readonly record struct CreateUsageCommand(int DeviceId, int PersonId, DateOnly From, DateOnly To)
    : IRequest<Result<GetUsageDto>>;

