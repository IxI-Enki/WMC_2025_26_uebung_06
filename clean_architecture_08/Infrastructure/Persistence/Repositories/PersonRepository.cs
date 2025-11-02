using Application.Contracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Spezifisches Repository für Persons mit zusätzlichen Abfragen.
/// </summary>
public class PersonRepository(AppDbContext ctx) : GenericRepository<Person>(ctx), IPersonRepository
{
    /// <summary>
    /// Sucht eine Person nach E-Mail-Adresse.
    /// </summary>
    public async Task<Person?> GetByEmailAsync(string mailAddress, CancellationToken ct = default)
        => await Set.FirstOrDefaultAsync(p => p.MailAddress == mailAddress, ct);
}
