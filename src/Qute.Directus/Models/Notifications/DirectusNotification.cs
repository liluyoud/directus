namespace Qute.Directus.Models.Notifications;

/// <summary>
/// Represents a Directus notification.
/// </summary>
public record DirectusNotification
{
    public int? Id { get; init; }
    public DateTime? Timestamp { get; init; }
    public string? Status { get; init; }
    public string? Recipient { get; init; }
    public string? Sender { get; init; }
    public string? Subject { get; init; }
    public string? Message { get; init; }
    public string? Collection { get; init; }
    public string? Item { get; init; }
}
