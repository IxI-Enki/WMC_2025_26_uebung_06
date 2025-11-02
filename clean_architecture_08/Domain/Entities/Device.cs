using Domain.Common;
using Domain.Exceptions;
using Domain.ValidationSpecifications;

namespace Domain.Entities;

/// <summary>
/// Repräsentiert ein technisches Endgerät (Smartphone, Notebook, Tablet).
/// </summary>
public class Device : BaseEntity
{
    /// <summary>
    /// Seriennummer des Geräts. Muss eindeutig sein und mindestens 3 Zeichen haben.
    /// </summary>
    public string SerialNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Name/Bezeichnung des Geräts. Muss mindestens 2 Zeichen haben.
    /// </summary>
    public string DeviceName { get; private set; } = string.Empty;

    /// <summary>
    /// Gerätetyp (Tablet, SmartPhone, Notebook).
    /// </summary>
    public DeviceType DeviceType { get; private set; }

    /// <summary>
    /// Navigation zu den Nutzungen dieses Geräts.
    /// </summary>
    public ICollection<Usage> Usages { get; private set; } = default!;

    private Device() { } // Für EF Core notwendig (parameterloser Konstruktor)

    /// <summary>
    /// Erstellt ein neues Device mit den angegebenen Eigenschaften.
    /// </summary>
    /// <param name="serialNumber">Seriennummer (mind. 3 Zeichen, eindeutig)</param>
    /// <param name="deviceName">Gerätename (mind. 2 Zeichen)</param>
    /// <param name="deviceType">Gerätetyp</param>
    /// <returns>Neues Device-Objekt</returns>
    public static Device Create(string serialNumber, string deviceName, DeviceType deviceType)
    {
        var trimmedSerialNumber = (serialNumber ?? string.Empty).Trim();
        var trimmedDeviceName = (deviceName ?? string.Empty).Trim();

        ValidateDeviceProperties(trimmedSerialNumber, trimmedDeviceName);

        return new Device
        {
            SerialNumber = trimmedSerialNumber,
            DeviceName = trimmedDeviceName,
            DeviceType = deviceType
        };
    }

    /// <summary>
    /// Aktualisiert die Eigenschaften des Geräts.
    /// </summary>
    public void Update(string serialNumber, string deviceName, DeviceType deviceType)
    {
        var trimmedSerialNumber = (serialNumber ?? string.Empty).Trim();
        var trimmedDeviceName = (deviceName ?? string.Empty).Trim();

        if (SerialNumber == trimmedSerialNumber && DeviceName == trimmedDeviceName && DeviceType == deviceType)
            return; // Keine Änderung

        ValidateDeviceProperties(trimmedSerialNumber, trimmedDeviceName);

        SerialNumber = trimmedSerialNumber;
        DeviceName = trimmedDeviceName;
        DeviceType = deviceType;
    }

    /// <summary>
    /// Validiert die Device-Eigenschaften.
    /// </summary>
    public static void ValidateDeviceProperties(string serialNumber, string deviceName)
    {
        var validationResults = new List<DomainValidationResult>
        {
            DeviceSpecifications.CheckSerialNumber(serialNumber),
            DeviceSpecifications.CheckDeviceName(deviceName)
        };

        foreach (var result in validationResults)
        {
            if (!result.IsValid)
            {
                throw new DomainValidationException(result.Property, result.ErrorMessage!);
            }
        }
    }

    public override string ToString() => $"{DeviceName} ({SerialNumber})";
}

