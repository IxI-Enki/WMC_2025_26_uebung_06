using Domain.Entities;

namespace Application.Contracts.Repositories;

/// <summary>
/// Person-spezifische Abfragen zusätzlich zu den generischen CRUDs.
/// </summary>
public interface IPersonRepository : IGenericRepository<Person>
{
    /// <summary>
    /// Sucht eine Person nach E-Mail-Adresse.
    /// </summary>
    Task<Person?> GetByEmailAsync(string mailAddress, CancellationToken ct = default);
}

