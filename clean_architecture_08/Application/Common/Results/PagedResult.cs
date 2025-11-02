namespace Application.Common.Results;

/// <summary>
/// Represents a paginated result.
/// </summary>
public sealed record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int PageNumber, int PageSize);

