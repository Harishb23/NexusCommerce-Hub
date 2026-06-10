namespace CommerceOps.API.DTOs;

public record SyncLogDto(
    int Id,
    string Channel,
    string Status,
    string Message,
    DateTime CreatedAt
);

public record SyncRequestDto(string? Channel);

public record SyncResultDto(
    bool Success,
    string Message,
    int OrdersSynced,
    int ProductsSynced,
    IReadOnlyList<string> Errors
);
