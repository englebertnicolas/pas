namespace PAS.Common.Application.Queries;

public record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    bool HasNextPage
);