using Domain.Entities;

namespace Application.Features.Dtos;

/// <summary>
/// Datenübertragungsobjekt (DTO) für Devices.
/// </summary>
public sealed record GetDeviceDto(int Id, string SerialNumber, string DeviceName, DeviceType DeviceType);

