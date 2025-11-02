namespace Domain.Contracts;

/// <summary>
/// Domain-Service zur Prüfung der fachlichen Eindeutigkeit einer Person (E-Mail-Adresse).
/// </summary>
public interface IPersonUniquenessChecker
{
    /// <summary>
    /// Prüft, ob die E-Mail-Adresse eindeutig ist.
    /// </summary>
    /// <param name="id">ID der Person (0 bei neuer Person)</param>
    /// <param name="mailAddress">E-Mail-Adresse</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>True wenn eindeutig, sonst false</returns>
    Task<bool> IsUniqueAsync(int id, string mailAddress, CancellationToken ct = default);
}

