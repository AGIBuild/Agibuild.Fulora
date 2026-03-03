namespace Agibuild.Fulora.Plugin.Notifications;

/// <summary>
/// Platform abstraction for showing native notifications.
/// Implementations can use OS-specific APIs (e.g., Windows Toast, macOS NSUserNotification).
/// </summary>
public interface INativeNotificationProvider
{
    Task<string> ShowNotification(string title, string body, NotificationOptions? options);
    Task<bool> RequestPermission();
    Task ClearAll();
    Task Clear(string notificationId);
}
