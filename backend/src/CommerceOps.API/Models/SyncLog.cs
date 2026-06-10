namespace CommerceOps.API.Models;

public class SyncLog
{
    public int Id { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class SyncStatus
{
    public const string Success = "Success";
    public const string Failed = "Failed";
    public const string InProgress = "InProgress";
}
