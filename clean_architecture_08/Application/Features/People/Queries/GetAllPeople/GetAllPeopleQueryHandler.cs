using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using Mapster;
using MediatR;

namespace Application.Features.People.Queries.GetAllPeople;

public sealed class GetAllPeopleQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetAllPeopleQuery, Result<IReadOnlyCollection<GetPersonDto>>>
{
    public async Task<Result<IReadOnlyCollection<GetPersonDto>>> Handle(GetAllPeopleQuery request,
        CancellationToken cancellationToken)
    {
        var people = await uow.People.GetAllAsync(
            orderBy: q => q.OrderBy(p => p.LastName).ThenBy(p => p.FirstName), ct: cancellationToken);
        var dtos = people.Adapt<IReadOnlyCollection<GetPersonDto>>();
        return Result<IReadOnlyCollection<GetPersonDto>>.Success(dtos);
    }
}

