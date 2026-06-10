namespace CommerceOps.API.DTOs;

public record OrderDto(
    int Id,
    string ExternalOrderId,
    string Channel,
    string CustomerName,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt
);

public record UpdateOrderStatusDto(string Status);

public record OrderSearchParams(
    string? Search,
    string? Status,
    string? Channel,
    int Page = 1,
    int PageSize = 20
);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Total,
    int Page,
    int PageSize
);
