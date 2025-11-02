using Domain.Common;

namespace Domain.ValidationSpecifications;

/// <summary>
/// Validierungsregeln für Usage-Entitäten.
/// </summary>
public static class UsageSpecifications
{
    /// <summary>
    /// Prüft, ob die DeviceId gültig ist (größer 0).
    /// </summary>
    public static DomainValidationResult CheckDeviceId(int deviceId)
    {
        if (deviceId <= 0)
        {
            return DomainValidationResult.Failure("DeviceId", "DeviceId muss größer 0 sein.");
        }
        return DomainValidationResult.Success("DeviceId");
    }

    /// <summary>
    /// Prüft, ob die PersonId gültig ist (größer 0).
    /// </summary>
    public static DomainValidationResult CheckPersonId(int personId)
    {
        if (personId <= 0)
        {
            return DomainValidationResult.Failure("PersonId", "PersonId muss größer 0 sein.");
        }
        return DomainValidationResult.Success("PersonId");
    }

    /// <summary>
    /// Prüft, ob der Datumsbereich gültig ist (To >= From).
    /// </summary>
    public static DomainValidationResult CheckDateRange(DateOnly from, DateOnly to)
    {
        if (to < from)
        {
            return DomainValidationResult.Failure("DateRange", "Das Rückgabedatum darf nicht vor dem Startdatum liegen.");
        }
        return DomainValidationResult.Success("DateRange");
    }

    /// <summary>
    /// Prüft, ob die Daten in der Zukunft liegen (für neue Buchungen).
    /// </summary>
    public static DomainValidationResult CheckFutureDates(DateOnly from, DateOnly to)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (from < today)
        {
            return DomainValidationResult.Failure("From", "Nutzungen können nur für die Zukunft gebucht werden.");
        }
        if (to < today)
        {
            return DomainValidationResult.Failure("To", "Das Rückgabedatum muss in der Zukunft liegen.");
        }
        return DomainValidationResult.Success("FutureDates");
    }
}

