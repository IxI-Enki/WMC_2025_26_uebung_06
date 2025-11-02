using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using Mapster;
using MediatR;

namespace Application.Features.Usages.Queries.GetAllUsages;

public sealed class GetAllUsagesQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetAllUsagesQuery, Result<IReadOnlyCollection<GetUsageDto>>>
{
    public async Task<Result<IReadOnlyCollection<GetUsageDto>>> Handle(GetAllUsagesQuery request,
        CancellationToken cancellationToken)
    {
        var usages = await uow.Usages.GetAllAsync(
            orderBy: q => q.OrderBy(u => u.From).ThenBy(u => u.To), ct: cancellationToken);
        var dtos = usages.Adapt<IReadOnlyCollection<GetUsageDto>>();
        return Result<IReadOnlyCollection<GetUsageDto>>.Success(dtos);
    }
}

