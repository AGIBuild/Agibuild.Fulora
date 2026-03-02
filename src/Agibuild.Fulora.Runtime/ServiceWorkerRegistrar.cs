namespace Agibuild.Fulora;

/// <summary>
/// Generates and injects service worker registration and implementation scripts
/// for offline support in WebView-hosted SPAs.
/// </summary>
public static class ServiceWorkerRegistrar
{
    /// <summary>
    /// Returns a JavaScript script string that registers a service worker with the specified options.
    /// </summary>
    public static string GenerateRegistrationScript(ServiceWorkerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var path = EscapeJsString(options.ScriptPath);

        return $$"""
            (function(){
              if ('serviceWorker' in navigator) {
                navigator.serviceWorker.register('{{path}}', { scope: '/' }).then(function(reg) {
                  console.log('[Agibuild.Fulora] Service worker registered:', '{{path}}');
                }).catch(function(err) {
                  console.warn('[Agibuild.Fulora] Service worker registration failed:', err);
                });
              }
            })();
            """;
    }

    /// <summary>
    /// Returns a JavaScript service worker script with the specified cache strategy and precache URLs.
    /// </summary>
    public static string GenerateServiceWorkerScript(ServiceWorkerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var cacheName = EscapeJsString(options.CacheName);
        var strategy = MapStrategyToJs(options.CacheStrategy);
        var precacheList = options.PrecacheUrls.Select(u => $"'{EscapeJsString(u)}'").ToList();
        var precacheBody = precacheList.Count > 0
            ? "    var precache = [" + string.Join(", ", precacheList) + "];\n    return cache.addAll(precache);"
            : "    return Promise.resolve();";

        var fetchHandler = strategy switch
        {
            "cacheFirst" => GetCacheFirstHandler(cacheName),
            "networkFirst" => GetNetworkFirstHandler(cacheName),
            "staleWhileRevalidate" => GetStaleWhileRevalidateHandler(cacheName),
            "networkOnly" => GetNetworkOnlyHandler(),
            "cacheOnly" => GetCacheOnlyHandler(cacheName),
            _ => GetNetworkFirstHandler(cacheName)
        };

        return $$"""
            const CACHE_NAME = '{{cacheName}}';

            self.addEventListener('install', function(event) {
              event.waitUntil(
                caches.open(CACHE_NAME).then(function(cache) {
            {{precacheBody}}
                })
              );
              self.skipWaiting();
            });

            self.addEventListener('activate', function(event) {
              event.waitUntil(
                caches.keys().then(function(names) {
                  return Promise.all(names.filter(function(n) { return n !== CACHE_NAME; }).map(function(n) { return caches.delete(n); }));
                })
              );
              self.clients.claim();
            });

            self.addEventListener('fetch', function(event) {
            {{fetchHandler}}
            });
            """;
    }

    private static string GetCacheFirstHandler(string cacheName) => $$"""
              if (event.request.mode !== 'navigate' && event.request.method === 'GET') {
                event.respondWith(
                  caches.match(event.request).then(function(cached) {
                    return cached || fetch(event.request).then(function(res) {
                      var clone = res.clone();
                      caches.open('{{cacheName}}').then(function(c) { c.put(event.request, clone); });
                      return res;
                    });
                  })
                );
              }
            """;

    private static string GetNetworkFirstHandler(string cacheName) => $$"""
              if (event.request.mode !== 'navigate' && event.request.method === 'GET') {
                event.respondWith(
                  fetch(event.request).catch(function() {
                    return caches.match(event.request);
                  }).then(function(res) {
                    if (res && res.ok) {
                      var clone = res.clone();
                      caches.open('{{cacheName}}').then(function(c) { c.put(event.request, clone); });
                    }
                    return res || caches.match(event.request);
                  })
                );
              }
            """;

    private static string GetStaleWhileRevalidateHandler(string cacheName) => $$"""
              if (event.request.mode !== 'navigate' && event.request.method === 'GET') {
                event.respondWith(
                  caches.match(event.request).then(function(cached) {
                    var fetchPromise = fetch(event.request).then(function(res) {
                      if (res && res.ok) {
                        var clone = res.clone();
                        caches.open('{{cacheName}}').then(function(c) { c.put(event.request, clone); });
                      }
                      return res;
                    });
                    return cached || fetchPromise;
                  })
                );
              }
            """;

    private static string GetNetworkOnlyHandler() => """
              event.respondWith(fetch(event.request));
            """;

    private static string GetCacheOnlyHandler(string cacheName) => $$"""
              if (event.request.mode !== 'navigate' && event.request.method === 'GET') {
                event.respondWith(caches.match(event.request).then(function(c) { return c || new Response('', { status: 404 }); }));
              }
            """;

    private static string MapStrategyToJs(ServiceWorkerCacheStrategy strategy) => strategy switch
    {
        ServiceWorkerCacheStrategy.CacheFirst => "cacheFirst",
        ServiceWorkerCacheStrategy.NetworkFirst => "networkFirst",
        ServiceWorkerCacheStrategy.StaleWhileRevalidate => "staleWhileRevalidate",
        ServiceWorkerCacheStrategy.NetworkOnly => "networkOnly",
        ServiceWorkerCacheStrategy.CacheOnly => "cacheOnly",
        _ => "networkFirst"
    };

    private static string EscapeJsString(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "\\r");
    }
}
