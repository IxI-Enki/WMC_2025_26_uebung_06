namespace Application.Features.Dtos;

/// <summary>
/// Datenübertragungsobjekt (DTO) für Persons.
/// </summary>
public sealed record GetPersonDto(int Id, string LastName, string FirstName, string MailAddress);

