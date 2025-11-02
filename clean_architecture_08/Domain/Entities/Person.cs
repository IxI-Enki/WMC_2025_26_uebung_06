using Domain.Common;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.ValidationSpecifications;

namespace Domain.Entities;

/// <summary>
/// Repräsentiert eine Person, die Geräte nutzen kann.
/// </summary>
public class Person : BaseEntity
{
    /// <summary>
    /// Nachname der Person. Darf nicht leer sein.
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// Vorname der Person. Darf nicht leer sein.
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// E-Mail-Adresse der Person. Muss syntaktisch korrekt und eindeutig sein.
    /// </summary>
    public string MailAddress { get; private set; } = string.Empty;

    /// <summary>
    /// Navigation zu den Nutzungen dieser Person.
    /// </summary>
    public ICollection<Usage> Usages { get; private set; } = default!;

    private Person() { } // Für EF Core notwendig (parameterloser Konstruktor)

    /// <summary>
    /// Erstellt asynchron eine neue Person mit den angegebenen Eigenschaften.
    /// </summary>
    /// <param name="lastName">Nachname (nicht leer)</param>
    /// <param name="firstName">Vorname (nicht leer)</param>
    /// <param name="mailAddress">E-Mail-Adresse (syntaktisch korrekt, eindeutig)</param>
    /// <param name="uniquenessChecker">Service zur Prüfung der E-Mail-Eindeutigkeit</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Neue Person</returns>
    public static async Task<Person> CreateAsync(string lastName, string firstName, string mailAddress,
        IPersonUniquenessChecker uniquenessChecker, CancellationToken ct = default)
    {
        var trimmedLastName = (lastName ?? string.Empty).Trim();
        var trimmedFirstName = (firstName ?? string.Empty).Trim();
        var trimmedMailAddress = (mailAddress ?? string.Empty).Trim();

        ValidatePersonProperties(trimmedLastName, trimmedFirstName, trimmedMailAddress);
        await ValidatePersonUniqueness(0, trimmedMailAddress, uniquenessChecker, ct);

        return new Person
        {
            LastName = trimmedLastName,
            FirstName = trimmedFirstName,
            MailAddress = trimmedMailAddress
        };
    }

    /// <summary>
    /// Aktualisiert die Eigenschaften der Person.
    /// </summary>
    public async Task UpdateAsync(string lastName, string firstName, string mailAddress,
        IPersonUniquenessChecker uniquenessChecker, CancellationToken ct = default)
    {
        var trimmedLastName = (lastName ?? string.Empty).Trim();
        var trimmedFirstName = (firstName ?? string.Empty).Trim();
        var trimmedMailAddress = (mailAddress ?? string.Empty).Trim();

        if (LastName == trimmedLastName && FirstName == trimmedFirstName && MailAddress == trimmedMailAddress)
            return; // Keine Änderung

        ValidatePersonProperties(trimmedLastName, trimmedFirstName, trimmedMailAddress);
        await ValidatePersonUniqueness(Id, trimmedMailAddress, uniquenessChecker, ct);

        LastName = trimmedLastName;
        FirstName = trimmedFirstName;
        MailAddress = trimmedMailAddress;
    }

    /// <summary>
    /// Validiert die Person-Eigenschaften.
    /// </summary>
    public static void ValidatePersonProperties(string lastName, string firstName, string mailAddress)
    {
        var validationResults = new List<DomainValidationResult>
        {
            PersonSpecifications.CheckLastName(lastName),
            PersonSpecifications.CheckFirstName(firstName),
            PersonSpecifications.CheckMailAddress(mailAddress)
        };

        foreach (var result in validationResults)
        {
            if (!result.IsValid)
            {
                throw new DomainValidationException(result.Property, result.ErrorMessage!);
            }
        }
    }

    /// <summary>
    /// Validiert die Eindeutigkeit der E-Mail-Adresse.
    /// </summary>
    public static async Task ValidatePersonUniqueness(int id, string mailAddress,
        IPersonUniquenessChecker uniquenessChecker, CancellationToken ct = default)
    {
        if (!await uniquenessChecker.IsUniqueAsync(id, mailAddress, ct))
            throw new DomainValidationException("MailAddress", "Eine Person mit dieser E-Mail-Adresse existiert bereits.");
    }

    public override string ToString() => $"{FirstName} {LastName} ({MailAddress})";
}

