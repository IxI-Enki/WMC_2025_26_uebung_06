using Application.Contracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Spezifisches Repository für Usages mit zusätzlichen Abfragen.
/// </summary>
public class UsageRepository(AppDbContext ctx) : GenericRepository<Usage>(ctx), IUsageRepository
{
    /// <summary>
    /// Prüft, ob sich eine Usage mit anderen Usages desselben Geräts überlappt.
    /// Wichtig: Usages werden auf Datumsbasis verwaltet, nicht auf Zeitbasis.
    /// Es darf auch nicht am Tag der Rückgabe eine neue Usage beginnen.
    /// </summary>
    public async Task<bool> HasOverlapAsync(int id, int deviceId, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        // Prüfe ob eine andere Usage existiert, die:
        // 1. Das gleiche Device betrifft
        // 2. Eine andere ID hat (bei Update)
        // 3. Sich im Datumsbereich überlappt
        // 
        // Überlappung existiert wenn:
        // - Die neue Usage startet während einer bestehenden Usage (from zwischen existing.From und existing.To)
        // - Die neue Usage endet während einer bestehenden Usage (to zwischen existing.From und existing.To)
        // - Die neue Usage umschließt eine bestehende Usage (from <= existing.From UND to >= existing.To)
        // 
        // Wichtig: Es darf auch nicht am Tag der Rückgabe eine neue Usage beginnen!
        // Das bedeutet: from darf nicht gleich existing.To sein
        
        var hasOverlap = await Set
            .AsNoTracking()
            .AnyAsync(u =>
                u.DeviceId == deviceId &&
                u.Id != id &&
                (
                    // Neue Usage startet während bestehender Usage (inkl. Rückgabetag)
                    (from >= u.From && from <= u.To) ||
                    // Neue Usage endet während bestehender Usage
                    (to >= u.From && to <= u.To) ||
                    // Neue Usage umschließt bestehende Usage
                    (from <= u.From && to >= u.To)
                ),
                ct);

        return hasOverlap;
    }
}
