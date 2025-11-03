using Domain.Entities;

namespace Application.Contracts.Repositories;

/// <summary>
/// Usage-spezifische Abfragen zusätzlich zu den generischen CRUDs.
/// </summary>
public interface IUsageRepository : IGenericRepository<Usage>
{
    /// <summary>
    /// Prüft, ob sich eine Usage mit anderen Usages desselben Geräts überlappt.
    /// </summary>
    /// <param name="id">ID der Usage (0 bei neuer Usage)</param>
    /// <param name="deviceId">ID des Geräts</param>
    /// <param name="from">Startdatum</param>
    /// <param name="to">Enddatum</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>True wenn Überlappung existiert, sonst false</returns>
    Task<bool> HasOverlapAsync(int id, int deviceId, DateOnly from, DateOnly to, CancellationToken ct = default);
}
