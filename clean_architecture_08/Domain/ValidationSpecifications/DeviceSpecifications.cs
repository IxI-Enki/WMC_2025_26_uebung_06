using Domain.Common;

namespace Domain.ValidationSpecifications;

/// <summary>
/// Validierungsregeln für Device-Entitäten.
/// </summary>
public static class DeviceSpecifications
{
    public const int SerialNumberMinLength = 3;
    public const int DeviceNameMinLength = 2;

    /// <summary>
    /// Prüft, ob die Seriennummer gültig ist (nicht leer, mind. 3 Zeichen).
    /// </summary>
    public static DomainValidationResult CheckSerialNumber(string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            return DomainValidationResult.Failure("SerialNumber", "Seriennummer darf nicht leer sein.");
        }
        if (serialNumber.Trim().Length < SerialNumberMinLength)
        {
            return DomainValidationResult.Failure("SerialNumber", $"Seriennummer muss mindestens {SerialNumberMinLength} Zeichen haben.");
        }
        return DomainValidationResult.Success("SerialNumber");
    }

    /// <summary>
    /// Prüft, ob der Gerätename gültig ist (nicht leer, mind. 2 Zeichen).
    /// </summary>
    public static DomainValidationResult CheckDeviceName(string deviceName)
    {
        if (string.IsNullOrWhiteSpace(deviceName))
        {
            return DomainValidationResult.Failure("DeviceName", "Gerätename darf nicht leer sein.");
        }
        if (deviceName.Trim().Length < DeviceNameMinLength)
        {
            return DomainValidationResult.Failure("DeviceName", $"Gerätename muss mindestens {DeviceNameMinLength} Zeichen haben.");
        }
        return DomainValidationResult.Success("DeviceName");
    }
}

