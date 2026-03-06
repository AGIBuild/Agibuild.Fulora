using System.Diagnostics;
using Agibuild.Fulora;
using Avalonia.Controls;
using AvaloniVue.Bridge.Services;

namespace AvaloniVue.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        WebView.EnvironmentOptions = new WebViewEnvironmentOptions { EnableDevTools = true };

        Loaded += async (_, _) =>
        {
            try
            {
#if DEBUG
                await WebView.NavigateAsync(new Uri("http://localhost:5174"));
#else
                // In Release: use SPA hosting with embedded resources via app:// scheme.
                WebView.EnableSpaHosting(new SpaHostingOptions
                {
                    EmbeddedResourcePrefix = "wwwroot",
                    ResourceAssembly = typeof(MainWindow).Assembly,
                });
                await WebView.NavigateAsync(new Uri("app://localhost/index.html"));
#endif
            }
            catch (WebViewNavigationException ex)
            {
                Debug.WriteLine($"Navigation failed: {ex.Message}");
                await WebView.NavigateToStringAsync(
                    "<html><body style='font-family:system-ui;padding:2em;color:#333'>" +
                    "<h2>Navigation failed</h2>" +
                    $"<p>{ex.Message}</p>" +
#if DEBUG
                    "<p>Make sure the Vite dev server is running:<br>" +
                    "<code>cd AvaloniVue.Web && npm run dev</code></p>" +
#endif
                    "</body></html>");
                return;
            }

            // ── 2. Expose Bridge Services ([JsExport] — C# → JS) ───────
            // Must be called AFTER navigation completes so the RPC JS stubs
            // are injected into the actual page (Vue app polls for window.agWebView.rpc).
            WebView.Bridge.Expose<IAppShellService>(new AppShellService());
            WebView.Bridge.Expose<ISystemInfoService>(new SystemInfoService());
            WebView.Bridge.Expose<IChatService>(new ChatService());
            WebView.Bridge.Expose<IFileService>(new FileService());
            WebView.Bridge.Expose<ISettingsService>(new SettingsService());
        };
    }
}
