using Application.Features.Dtos;
using Domain.Entities;

namespace Application.Contracts.Repositories;

/// <summary>
/// Device-spezifische Abfragen zusätzlich zu den generischen CRUDs.
/// </summary>
public interface IDeviceRepository : IGenericRepository<Device>
{
    /// <summary>
    /// Liefert alle Devices mit Anzahl der Usages.
    /// </summary>
    Task<IReadOnlyCollection<GetDeviceWithCountDto>> GetAllWithCountAsync(CancellationToken ct = default);

    /// <summary>
    /// Sucht ein Device nach Seriennummer.
    /// </summary>
    Task<Device?> GetBySerialNumberAsync(string serialNumber, CancellationToken ct = default);
}

