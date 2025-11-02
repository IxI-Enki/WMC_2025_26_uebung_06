using Application.Common.Results;
using Application.Features.Dtos;
using MediatR;

namespace Application.Features.Usages.Commands.UpdateUsage;

public readonly record struct UpdateUsageCommand(int Id, int DeviceId, int PersonId, DateOnly From, DateOnly To)
    : IRequest<Result<GetUsageDto>>;

