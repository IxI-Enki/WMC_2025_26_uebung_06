using Application.Common.Results;
using MediatR;

namespace Application.Features.Usages.Commands.DeleteUsage;

public readonly record struct DeleteUsageCommand(int Id) : IRequest<Result<bool>>;

