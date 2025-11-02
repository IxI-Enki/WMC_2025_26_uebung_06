using Domain.Common;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.ValidationSpecifications;

namespace Domain.Entities;

/// <summary>
/// Repräsentiert die Nutzung eines Geräts durch eine Person in einem Zeitraum.
/// </summary>
public class Usage : BaseEntity
{
    /// <summary>
    /// Fremdschlüssel auf das Gerät.
    /// </summary>
    public int DeviceId { get; private set; }

    /// <summary>
    /// Navigation zum Gerät.
    /// </summary>
    public Device Device { get; private set; } = null!;

    /// <summary>
    /// Fremdschlüssel auf die Person.
    /// </summary>
    public int PersonId { get; private set; }

    /// <summary>
    /// Navigation zur Person.
    /// </summary>
    public Person Person { get; private set; } = null!;

    /// <summary>
    /// Startdatum der Nutzung (inklusiv).
    /// </summary>
    public DateOnly From { get; private set; }

    /// <summary>
    /// Enddatum der Nutzung (inklusiv).
    /// </summary>
    public DateOnly To { get; private set; }

    private Usage() { } // Für EF Core notwendig (parameterloser Konstruktor)

    /// <summary>
    /// Erstellt asynchron eine neue Usage mit den angegebenen Eigenschaften.
    /// </summary>
    /// <param name="device">Gerät (muss existieren)</param>
    /// <param name="person">Person (muss existieren)</param>
    /// <param name="from">Startdatum</param>
    /// <param name="to">Enddatum</param>
    /// <param name="overlapChecker">Service zur Prüfung von Überschneidungen</param>
    /// <param name="allowPastDates">Erlaubt vergangene Daten (für Datenimport)</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Neue Usage</returns>
    public static async Task<Usage> CreateAsync(Device device, Person person, DateOnly from, DateOnly to,
        IUsageOverlapChecker overlapChecker, bool allowPastDates = false, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(person);

        ValidateUsageProperties(device.Id, person.Id, from, to, allowPastDates);
        await ValidateNoOverlap(0, device.Id, from, to, overlapChecker, ct);

        return new Usage
        {
            Device = device,
            DeviceId = device.Id,
            Person = person,
            PersonId = person.Id,
            From = from,
            To = to
        };
    }

    /// <summary>
    /// Aktualisiert die Eigenschaften der Usage.
    /// </summary>
    public async Task UpdateAsync(Device device, Person person, DateOnly from, DateOnly to,
        IUsageOverlapChecker overlapChecker, bool allowPastDates = false, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(person);

        if (DeviceId == device.Id && PersonId == person.Id && From == from && To == to)
            return; // Keine Änderung

        ValidateUsageProperties(device.Id, person.Id, from, to, allowPastDates);
        await ValidateNoOverlap(Id, device.Id, from, to, overlapChecker, ct);

        Device = device;
        DeviceId = device.Id;
        Person = person;
        PersonId = person.Id;
        From = from;
        To = to;
    }

    /// <summary>
    /// Validiert die Usage-Eigenschaften.
    /// </summary>
    public static void ValidateUsageProperties(int deviceId, int personId, DateOnly from, DateOnly to, bool allowPastDates)
    {
        var validationResults = new List<DomainValidationResult>
        {
            UsageSpecifications.CheckDeviceId(deviceId),
            UsageSpecifications.CheckPersonId(personId),
            UsageSpecifications.CheckDateRange(from, to),
        };

        if (!allowPastDates)
        {
            validationResults.Add(UsageSpecifications.CheckFutureDates(from, to));
        }

        foreach (var result in validationResults)
        {
            if (!result.IsValid)
            {
                throw new DomainValidationException(result.Property, result.ErrorMessage!);
            }
        }
    }

    /// <summary>
    /// Validiert, dass sich die Usage nicht mit anderen Usages desselben Geräts überlappt.
    /// </summary>
    public static async Task ValidateNoOverlap(int id, int deviceId, DateOnly from, DateOnly to,
        IUsageOverlapChecker overlapChecker, CancellationToken ct = default)
    {
        if (await overlapChecker.HasOverlapAsync(id, deviceId, from, to, ct))
            throw new DomainValidationException("DateRange", "Die Nutzung überlappt mit einer anderen Buchung für dieses Gerät.");
    }

    public override string ToString() => $"Usage: {Device?.DeviceName} by {Person?.FirstName} {Person?.LastName} ({From} - {To})";
}

