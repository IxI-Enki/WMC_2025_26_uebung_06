using Application.Contracts.Repositories;

namespace Application.Contracts;

/// <summary>
/// Aggregiert Repositories und speichert Änderungen. Sicherer Umgang mit Transaktionen.
/// </summary>
public interface IUnitOfWork
{
    IDeviceRepository Devices { get; }
    IPersonRepository People { get; }
    IUsageRepository Usages { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

