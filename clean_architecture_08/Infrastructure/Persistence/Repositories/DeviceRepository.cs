using Application.Contracts.Repositories;
using Application.Features.Dtos;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Spezifisches Repository für Devices mit zusätzlichen Abfragen.
/// </summary>
public class DeviceRepository(AppDbContext ctx) : GenericRepository<Device>(ctx), IDeviceRepository
{
    /// <summary>
    /// Liefert alle Devices mit Anzahl der Usages.
    /// </summary>
    public async Task<IReadOnlyCollection<GetDeviceWithCountDto>> GetAllWithCountAsync(CancellationToken ct = default)
    {
        var result = await Set
            .AsNoTracking()
            .OrderBy(d => d.DeviceName)
            .Select(d => new GetDeviceWithCountDto(
                d.Id,
                d.SerialNumber,
                d.DeviceName,
                d.DeviceType,
                d.Usages.Count))
            .ToListAsync(ct);
        return result;
    }

    /// <summary>
    /// Sucht ein Device nach Seriennummer.
    /// </summary>
    public async Task<Device?> GetBySerialNumberAsync(string serialNumber, CancellationToken ct = default)
        => await Set.FirstOrDefaultAsync(d => d.SerialNumber == serialNumber, ct);
}
