using Xunit;

namespace Agibuild.Fulora.UnitTests;

public class ServiceWorkerTests
{
    [Fact]
    public void GenerateRegistrationScript_contains_navigator_serviceWorker_register()
    {
        var options = new ServiceWorkerOptions();
        var script = ServiceWorkerRegistrar.GenerateRegistrationScript(options);
        Assert.Contains("navigator.serviceWorker.register", script);
    }

    [Fact]
    public void GenerateRegistrationScript_uses_configured_ScriptPath()
    {
        var options = new ServiceWorkerOptions { ScriptPath = "/custom-sw.js" };
        var script = ServiceWorkerRegistrar.GenerateRegistrationScript(options);
        Assert.Contains("/custom-sw.js", script);
    }

    [Fact]
    public void GenerateServiceWorkerScript_contains_cache_name()
    {
        var options = new ServiceWorkerOptions { CacheName = "my-cache-v2" };
        var script = ServiceWorkerRegistrar.GenerateServiceWorkerScript(options);
        Assert.Contains("my-cache-v2", script);
    }

    [Fact]
    public void GenerateServiceWorkerScript_contains_precache_urls()
    {
        var options = new ServiceWorkerOptions
        {
            PrecacheUrls = ["/index.html", "/app.js", "/style.css"]
        };
        var script = ServiceWorkerRegistrar.GenerateServiceWorkerScript(options);
        Assert.Contains("/index.html", script);
        Assert.Contains("/app.js", script);
        Assert.Contains("/style.css", script);
        Assert.Contains("cache.addAll", script);
    }

    [Theory]
    [InlineData(ServiceWorkerCacheStrategy.CacheFirst, "caches.match")]
    [InlineData(ServiceWorkerCacheStrategy.NetworkFirst, "fetch(event.request)")]
    [InlineData(ServiceWorkerCacheStrategy.StaleWhileRevalidate, "caches.match")]
    [InlineData(ServiceWorkerCacheStrategy.NetworkOnly, "fetch(event.request)")]
    [InlineData(ServiceWorkerCacheStrategy.CacheOnly, "caches.match")]
    public void Each_CacheStrategy_produces_correct_JS_strategy_code(
        ServiceWorkerCacheStrategy strategy,
        string expectedFragment)
    {
        var options = new ServiceWorkerOptions { CacheStrategy = strategy };
        var script = ServiceWorkerRegistrar.GenerateServiceWorkerScript(options);
        Assert.Contains(expectedFragment, script);
    }

    [Fact]
    public void Default_options_produce_valid_scripts()
    {
        var options = new ServiceWorkerOptions();
        var regScript = ServiceWorkerRegistrar.GenerateRegistrationScript(options);
        var swScript = ServiceWorkerRegistrar.GenerateServiceWorkerScript(options);

        Assert.NotNull(regScript);
        Assert.NotEmpty(regScript);
        Assert.Contains("navigator.serviceWorker.register", regScript);
        Assert.Contains("/sw.js", regScript);

        Assert.NotNull(swScript);
        Assert.NotEmpty(swScript);
        Assert.Contains("agibuild-offline-v1", swScript);
        Assert.Contains("self.addEventListener('install'", swScript);
        Assert.Contains("self.addEventListener('fetch'", swScript);
    }

    [Fact]
    public void SpaHostingOptions_ServiceWorker_integration()
    {
        var swOptions = new ServiceWorkerOptions
        {
            ScriptPath = "/sw-offline.js",
            CacheName = "spa-cache-v1",
            PrecacheUrls = ["/", "/index.html"]
        };
        var spaOptions = new SpaHostingOptions { ServiceWorker = swOptions };

        Assert.NotNull(spaOptions.ServiceWorker);
        Assert.Equal("/sw-offline.js", spaOptions.ServiceWorker.ScriptPath);
        Assert.Equal("spa-cache-v1", spaOptions.ServiceWorker.CacheName);

        var regScript = ServiceWorkerRegistrar.GenerateRegistrationScript(spaOptions.ServiceWorker);
        Assert.Contains("/sw-offline.js", regScript);
        Assert.Contains("navigator.serviceWorker.register", regScript);
    }

    [Fact]
    public void SpaHostingOptions_ServiceWorker_can_be_null()
    {
        var spaOptions = new SpaHostingOptions();
        Assert.Null(spaOptions.ServiceWorker);
    }

    [Fact]
    public void GenerateRegistrationScript_throws_on_null_options()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ServiceWorkerRegistrar.GenerateRegistrationScript(null!));
    }

    [Fact]
    public void GenerateServiceWorkerScript_throws_on_null_options()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ServiceWorkerRegistrar.GenerateServiceWorkerScript(null!));
    }
}
