namespace Agibuild.Fulora.Plugin.Notifications;

/// <summary>
/// Implementation of <see cref="INotificationService"/> that delegates to an <see cref="INativeNotificationProvider"/>.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly INativeNotificationProvider _provider;

    public NotificationService(INativeNotificationProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public Task<string> Show(string title, string body, NotificationOptions? options = null) =>
        _provider.ShowNotification(title, body, options);

    public Task<bool> RequestPermission() =>
        _provider.RequestPermission();

    public Task ClearAll() =>
        _provider.ClearAll();

    public Task Clear(string notificationId) =>
        _provider.Clear(notificationId);
}
