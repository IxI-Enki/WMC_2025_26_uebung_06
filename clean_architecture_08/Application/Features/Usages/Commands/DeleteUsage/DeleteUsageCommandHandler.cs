using Application.Common.Results;
using Application.Contracts;
using MediatR;

namespace Application.Features.Usages.Commands.DeleteUsage;

/// <summary>
/// Command-Handler zum Löschen einer Usage.
/// </summary>
public sealed class DeleteUsageCommandHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteUsageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteUsageCommand request, CancellationToken cancellationToken)
    {
        var entity = await uow.Usages.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return Result<bool>.NotFound($"Usage mit ID {request.Id} nicht gefunden.");

        uow.Usages.Remove(entity);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<bool>.NoContent();
    }
}

