using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using Mapster;
using MediatR;

namespace Application.Features.People.Queries.GetPersonById;

public sealed class GetPersonByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetPersonByIdQuery, Result<GetPersonDto>>
{
    public async Task<Result<GetPersonDto>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await uow.People.GetByIdAsync(request.Id, cancellationToken);
        if (person is null) return Result<GetPersonDto>.NotFound($"Person mit ID {request.Id} nicht gefunden.");

        return Result<GetPersonDto>.Success(person.Adapt<GetPersonDto>());
    }
}

