namespace Agibuild.Fulora;

public sealed class ServiceWorkerOptions
{
    public string ScriptPath { get; set; } = "/sw.js";
    public ServiceWorkerCacheStrategy CacheStrategy { get; set; } = ServiceWorkerCacheStrategy.NetworkFirst;
    public string[] PrecacheUrls { get; set; } = [];
    public string CacheName { get; set; } = "agibuild-offline-v1";
    public TimeSpan? MaxAge { get; set; }
}

public enum ServiceWorkerCacheStrategy
{
    CacheFirst,
    NetworkFirst,
    StaleWhileRevalidate,
    NetworkOnly,
    CacheOnly
}
