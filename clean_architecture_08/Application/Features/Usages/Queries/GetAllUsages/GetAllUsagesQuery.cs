using Application.Common.Results;
using Application.Features.Dtos;
using MediatR;

namespace Application.Features.Usages.Queries.GetAllUsages;

public readonly record struct GetAllUsagesQuery : IRequest<Result<IReadOnlyCollection<GetUsageDto>>>;

