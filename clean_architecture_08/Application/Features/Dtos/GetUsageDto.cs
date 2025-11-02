namespace Application.Features.Dtos;

/// <summary>
/// Datenübertragungsobjekt (DTO) für Usages.
/// </summary>
public sealed record GetUsageDto(
    int Id,
    int DeviceId,
    string DeviceName,
    int PersonId,
    string PersonFirstName,
    string PersonLastName,
    DateOnly From,
    DateOnly To);

