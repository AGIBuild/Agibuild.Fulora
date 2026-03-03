namespace Agibuild.Fulora.Plugin.Notifications;

/// <summary>
/// Options for notification display.
/// </summary>
public sealed class NotificationOptions
{
    public string? Icon { get; init; }
    public string? Tag { get; init; }
    public bool Silent { get; init; }
}
