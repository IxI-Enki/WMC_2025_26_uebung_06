using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using Domain.Contracts;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.People.Commands.CreatePerson;

/// <summary>
/// Command-Handler zum Erstellen einer neuen Person.
/// </summary>
public sealed class CreatePersonCommandHandler(IUnitOfWork uow, IPersonUniquenessChecker uniquenessChecker)
    : IRequestHandler<CreatePersonCommand, Result<GetPersonDto>>
{
    public async Task<Result<GetPersonDto>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var entity = await Person.CreateAsync(request.LastName, request.FirstName, request.MailAddress,
            uniquenessChecker, cancellationToken);
        await uow.People.AddAsync(entity, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<GetPersonDto>.Created(entity.Adapt<GetPersonDto>());
    }
}

