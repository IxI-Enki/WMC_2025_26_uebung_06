using Application.Contracts;
using Application.Contracts.Repositories;

namespace Infrastructure.Persistence;

/// <summary>
/// Unit of Work aggregiert Repositories und speichert Änderungen transaktional.
/// </summary>
public class UnitOfWork(AppDbContext dbContext,
        IDeviceRepository devices, IPersonRepository people, IUsageRepository usages) : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _dbContext = dbContext;
    private bool _disposed;

    /// <summary>
    /// Zugriff auf Device-Repository.
    /// </summary>
    public IDeviceRepository Devices { get; } = devices;

    /// <summary>
    /// Zugriff auf Person-Repository.
    /// </summary>
    public IPersonRepository People { get; } = people;

    /// <summary>
    /// Zugriff auf Usage-Repository.
    /// </summary>
    public IUsageRepository Usages { get; } = usages;

    /// <summary>
    /// Persistiert alle Änderungen in die DB. Gibt die Anzahl der betroffenen Zeilen zurück.
    /// </summary>
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _dbContext.SaveChangesAsync(ct);

    /// <summary>
    /// Gibt verwaltete Ressourcen frei. Der DbContext gehört zum Scope dieser UoW und wird hier entsorgt.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _dbContext.Dispose();
        }
        _disposed = true;
    }
}
