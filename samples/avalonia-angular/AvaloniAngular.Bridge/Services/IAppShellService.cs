using Agibuild.Fulora;
using AvaloniAngular.Bridge.Models;

namespace AvaloniAngular.Bridge.Services;

/// <summary>
/// Provides application metadata and dynamic page registry.
/// The Angular frontend queries this service on startup to build navigation.
/// </summary>
[JsExport]
public interface IAppShellService
{
    Task<List<PageDefinition>> GetPages();
    Task<AppInfo> GetAppInfo();
}
