using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using Domain.Contracts;
using Mapster;
using MediatR;

namespace Application.Features.Usages.Commands.UpdateUsage;

/// <summary>
/// Command-Handler zum Aktualisieren einer vorhandenen Usage.
/// </summary>
public sealed class UpdateUsageCommandHandler(IUnitOfWork uow, IUsageOverlapChecker overlapChecker)
    : IRequestHandler<UpdateUsageCommand, Result<GetUsageDto>>
{
    public async Task<Result<GetUsageDto>> Handle(UpdateUsageCommand request, CancellationToken cancellationToken)
    {
        var entity = await uow.Usages.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return Result<GetUsageDto>.NotFound($"Usage mit ID {request.Id} nicht gefunden.");

        // Lade Device und Person
        var device = await uow.Devices.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device is null) return Result<GetUsageDto>.NotFound($"Device mit ID {request.DeviceId} nicht gefunden.");

        var person = await uow.People.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null) return Result<GetUsageDto>.NotFound($"Person mit ID {request.PersonId} nicht gefunden.");

        await entity.UpdateAsync(device, person, request.From, request.To,
            overlapChecker, allowPastDates: false, cancellationToken);

        uow.Usages.Update(entity);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<GetUsageDto>.Success(entity.Adapt<GetUsageDto>());
    }
}

