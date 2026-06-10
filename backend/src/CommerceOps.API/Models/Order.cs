namespace CommerceOps.API.Models;

public class Order
{
    public int Id { get; set; }
    public string ExternalOrderId { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class OrderStatus
{
    public const string Pending = "Pending";
    public const string Processing = "Processing";
    public const string Delivered = "Delivered";
    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlyList<string> All = [Pending, Processing, Delivered, Cancelled];

    public static bool IsValid(string status) => All.Contains(status);
}
