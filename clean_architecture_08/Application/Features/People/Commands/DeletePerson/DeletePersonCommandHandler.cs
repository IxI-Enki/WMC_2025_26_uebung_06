using Application.Common.Results;
using Application.Contracts;
using MediatR;

namespace Application.Features.People.Commands.DeletePerson;

/// <summary>
/// Command-Handler zum Löschen einer Person.
/// </summary>
public sealed class DeletePersonCommandHandler(IUnitOfWork uow)
    : IRequestHandler<DeletePersonCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        var entity = await uow.People.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return Result<bool>.NotFound($"Person mit ID {request.Id} nicht gefunden.");

        uow.People.Remove(entity);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<bool>.NoContent();
    }
}

