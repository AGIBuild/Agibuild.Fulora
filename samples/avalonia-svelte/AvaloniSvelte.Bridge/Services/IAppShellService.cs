using Agibuild.Fulora;
using AvaloniSvelte.Bridge.Models;

namespace AvaloniSvelte.Bridge.Services;

/// <summary>
/// Provides application metadata and dynamic page registry.
/// The Svelte frontend queries this service on startup to build navigation.
/// </summary>
[JsExport]
public interface IAppShellService
{
    Task<List<PageDefinition>> GetPages();
    Task<AppInfo> GetAppInfo();
}
