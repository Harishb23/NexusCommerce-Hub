namespace CommerceOps.API.Models;

public class IntegrationHealth
{
    public int Id { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Status { get; set; } = HealthStatus.Healthy;
    public DateTime LastChecked { get; set; } = DateTime.UtcNow;
    public int FailureCount { get; set; }
    public double ResponseTime { get; set; }
}

public static class HealthStatus
{
    public const string Healthy = "Healthy";
    public const string Warning = "Warning";
    public const string Failed = "Failed";
}
