using Application.Common.Results;
using Application.Contracts;
using Application.Features.Dtos;
using Domain.Contracts;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Usages.Commands.CreateUsage;

/// <summary>
/// Command-Handler zum Erstellen einer neuen Usage.
/// </summary>
public sealed class CreateUsageCommandHandler(IUnitOfWork uow, IUsageOverlapChecker overlapChecker)
    : IRequestHandler<CreateUsageCommand, Result<GetUsageDto>>
{
    public async Task<Result<GetUsageDto>> Handle(CreateUsageCommand request, CancellationToken cancellationToken)
    {
        // Lade Device und Person
        var device = await uow.Devices.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device is null) return Result<GetUsageDto>.NotFound($"Device mit ID {request.DeviceId} nicht gefunden.");

        var person = await uow.People.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null) return Result<GetUsageDto>.NotFound($"Person mit ID {request.PersonId} nicht gefunden.");

        // Erstelle Usage (mit Overlap-Check, allowPastDates = false für neue Buchungen)
        var entity = await Usage.CreateAsync(device, person, request.From, request.To,
            overlapChecker, allowPastDates: false, cancellationToken);

        await uow.Usages.AddAsync(entity, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<GetUsageDto>.Created(entity.Adapt<GetUsageDto>());
    }
}

