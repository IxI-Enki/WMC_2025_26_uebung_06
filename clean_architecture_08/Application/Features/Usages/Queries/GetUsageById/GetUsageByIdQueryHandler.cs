using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using Mapster;
using MediatR;

namespace Application.Features.Usages.Queries.GetUsageById;

public sealed class GetUsageByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetUsageByIdQuery, Result<GetUsageDto>>
{
    public async Task<Result<GetUsageDto>> Handle(GetUsageByIdQuery request, CancellationToken cancellationToken)
    {
        var usage = await uow.Usages.GetByIdAsync(request.Id, cancellationToken);
        if (usage is null) return Result<GetUsageDto>.NotFound($"Usage mit ID {request.Id} nicht gefunden.");

        return Result<GetUsageDto>.Success(usage.Adapt<GetUsageDto>());
    }
}

