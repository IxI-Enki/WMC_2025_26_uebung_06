namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a concurrency conflict occurs.
/// </summary>
public class ConcurrencyException(string message = "Der Datensatz wurde zwischenzeitlich von einem anderen Benutzer geändert.")
    : Exception(message)
{
}

