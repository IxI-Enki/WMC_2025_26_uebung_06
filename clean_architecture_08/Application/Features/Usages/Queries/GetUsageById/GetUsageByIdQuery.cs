using Application.Common.Results;
using Application.Features.Dtos;
using MediatR;

namespace Application.Features.Usages.Queries.GetUsageById;

public readonly record struct GetUsageByIdQuery(int Id) : IRequest<Result<GetUsageDto>>;

