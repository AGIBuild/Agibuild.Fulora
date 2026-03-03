using Agibuild.Fulora.Plugin.Notifications;
using Xunit;

namespace Agibuild.Fulora.UnitTests.Plugins;

public class NotificationServiceTests
{
    private readonly InMemoryNotificationProvider _provider = new();
    private NotificationService CreateService() => new(_provider);

    [Fact]
    public async Task Show_ReturnsNotificationId()
    {
        var svc = CreateService();
        var id = await svc.Show("Test", "Body");
        Assert.NotNull(id);
        Assert.StartsWith("notif-", id);
        Assert.True(_provider.HasNotification(id));
    }

    [Fact]
    public async Task RequestPermission_ReturnsTrue()
    {
        var svc = CreateService();
        var granted = await svc.RequestPermission();
        Assert.True(granted);
    }

    [Fact]
    public async Task ClearAll_ClearsAll()
    {
        var svc = CreateService();
        await svc.Show("A", "1");
        await svc.Show("B", "2");
        Assert.Equal(2, _provider.Count);
        await svc.ClearAll();
        Assert.Equal(0, _provider.Count);
    }

    [Fact]
    public async Task Clear_ById_RemovesNotification()
    {
        var svc = CreateService();
        var id = await svc.Show("X", "Y");
        Assert.True(_provider.HasNotification(id));
        await svc.Clear(id);
        Assert.False(_provider.HasNotification(id));
    }

    [Fact]
    public async Task Show_WithOptions_Stores()
    {
        var svc = CreateService();
        var id = await svc.Show("Title", "Body", new NotificationOptions { Icon = "icon.png", Tag = "tag1", Silent = true });
        Assert.NotNull(id);
        Assert.True(_provider.HasNotification(id));
    }
}
