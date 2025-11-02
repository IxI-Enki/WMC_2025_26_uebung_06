namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found.
/// </summary>
public class NotFoundException(string entityName, object key)
    : Exception($"{entityName} mit ID '{key}' wurde nicht gefunden.")
{
    public string EntityName { get; } = entityName;
    public object Key { get; } = key;
}

