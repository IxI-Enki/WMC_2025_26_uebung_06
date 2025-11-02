using Application.Contracts;
using Domain.Contracts;

namespace Application.Services;

/// <summary>
/// Service zur Prüfung der Eindeutigkeit einer Person (E-Mail-Adresse).
/// </summary>
public class PersonUniquenessChecker(IUnitOfWork uow) : IPersonUniquenessChecker
{
    public async Task<bool> IsUniqueAsync(int id, string mailAddress, CancellationToken ct = default)
    {
        var existing = await uow.People.GetByEmailAsync(mailAddress, ct);
        return existing is null || existing.Id == id;
    }
}

