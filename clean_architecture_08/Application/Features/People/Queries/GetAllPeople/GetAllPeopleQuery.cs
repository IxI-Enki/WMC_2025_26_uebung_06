using Application.Common.Results;
using Application.Features.Dtos;
using MediatR;

namespace Application.Features.People.Queries.GetAllPeople;

public readonly record struct GetAllPeopleQuery : IRequest<Result<IReadOnlyCollection<GetPersonDto>>>;

