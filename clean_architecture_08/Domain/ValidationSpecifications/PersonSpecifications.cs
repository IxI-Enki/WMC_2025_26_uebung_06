using Domain.Common;
using System.Text.RegularExpressions;

namespace Domain.ValidationSpecifications;

/// <summary>
/// Validierungsregeln für Person-Entitäten.
/// </summary>
public static class PersonSpecifications
{
    // Einfache E-Mail-Regex für grundlegende Syntaxprüfung
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    /// <summary>
    /// Prüft, ob der Nachname gültig ist (nicht leer).
    /// </summary>
    public static DomainValidationResult CheckLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return DomainValidationResult.Failure("LastName", "Nachname darf nicht leer sein.");
        }
        return DomainValidationResult.Success("LastName");
    }

    /// <summary>
    /// Prüft, ob der Vorname gültig ist (nicht leer).
    /// </summary>
    public static DomainValidationResult CheckFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return DomainValidationResult.Failure("FirstName", "Vorname darf nicht leer sein.");
        }
        return DomainValidationResult.Success("FirstName");
    }

    /// <summary>
    /// Prüft, ob die E-Mail-Adresse gültig ist (nicht leer, syntaktisch korrekt).
    /// </summary>
    public static DomainValidationResult CheckMailAddress(string mailAddress)
    {
        if (string.IsNullOrWhiteSpace(mailAddress))
        {
            return DomainValidationResult.Failure("MailAddress", "E-Mail-Adresse darf nicht leer sein.");
        }
        if (!EmailRegex.IsMatch(mailAddress.Trim()))
        {
            return DomainValidationResult.Failure("MailAddress", "E-Mail-Adresse ist syntaktisch nicht korrekt.");
        }
        return DomainValidationResult.Success("MailAddress");
    }
}

