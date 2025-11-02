using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using Domain.Contracts;
using Mapster;
using MediatR;

namespace Application.Features.People.Commands.UpdatePerson;

/// <summary>
/// Command-Handler zum Aktualisieren einer vorhandenen Person.
/// </summary>
public sealed class UpdatePersonCommandHandler(IUnitOfWork uow, IPersonUniquenessChecker uniquenessChecker)
    : IRequestHandler<UpdatePersonCommand, Result<GetPersonDto>>
{
    public async Task<Result<GetPersonDto>> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var entity = await uow.People.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return Result<GetPersonDto>.NotFound($"Person mit ID {request.Id} nicht gefunden.");

        await entity.UpdateAsync(request.LastName, request.FirstName, request.MailAddress,
            uniquenessChecker, cancellationToken);
        uow.People.Update(entity);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<GetPersonDto>.Success(entity.Adapt<GetPersonDto>());
    }
}

