using Application.Contracts;
using Domain.Contracts;

namespace Application.Services;

/// <summary>
/// Service zur Prüfung von Überschneidungen bei Gerätenutzungen.
/// </summary>
public class UsageOverlapChecker(IUnitOfWork uow) : IUsageOverlapChecker
{
    public async Task<bool> HasOverlapAsync(int id, int deviceId, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        return await uow.Usages.HasOverlapAsync(id, deviceId, from, to, ct);
    }
}

