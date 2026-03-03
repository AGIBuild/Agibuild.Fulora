using Agibuild.Fulora;

namespace Agibuild.Fulora.Plugin.Notifications;

/// <summary>
/// Bridge service for showing native/system notifications.
/// </summary>
[JsExport]
public interface INotificationService
{
    Task<string> Show(string title, string body, NotificationOptions? options = null);
    Task<bool> RequestPermission();
    Task ClearAll();
    Task Clear(string notificationId);
}
