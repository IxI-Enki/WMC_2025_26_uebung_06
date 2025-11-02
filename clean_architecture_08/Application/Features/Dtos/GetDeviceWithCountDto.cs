using Domain.Entities;

namespace Application.Features.Dtos;

/// <summary>
/// Datenübertragungsobjekt (DTO) für Devices inkl. Anzahl der Nutzungen.
/// </summary>
public sealed record GetDeviceWithCountDto(
    int Id,
    string SerialNumber,
    string DeviceName,
    DeviceType DeviceType,
    int NumberOfUsages);

