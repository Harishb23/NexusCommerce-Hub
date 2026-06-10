namespace CommerceOps.API.DTOs;

public record IntegrationHealthDto(
    int Id,
    string Channel,
    string Status,
    DateTime LastChecked,
    int FailureCount,
    double ResponseTime
);
