using Application.Common.Results;
using Application.Features.Dtos;
using MediatR;

namespace Application.Features.People.Queries.GetPersonById;

public readonly record struct GetPersonByIdQuery(int Id) : IRequest<Result<GetPersonDto>>;

