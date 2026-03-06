using System.Diagnostics;
using Agibuild.Fulora;
using Avalonia.Controls;
using AvaloniAiChat.Bridge.Services;
using Microsoft.Extensions.AI;

namespace AvaloniAiChat.Desktop;

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
                await WebView.NavigateAsync(new Uri("http://localhost:5175"));
#else
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
                    "<code>cd AvaloniAiChat.Web && npm run dev</code></p>" +
#endif
                    "</body></html>");
                return;
            }

            var (chatClient, backendName) = CreateChatClient();
            var chatService = new AiChatService(chatClient, backendName);
            WebView.Bridge.Expose<IAiChatService>(chatService);

            WebView.DropCompleted += (_, e) =>
            {
                var file = e.Payload.Files?.FirstOrDefault();
                if (file is not null)
                    chatService.SetDroppedFile(file.Path);
            };
        };
    }

    private static (IChatClient Client, string Name) CreateChatClient()
    {
        var endpoint = new Uri(Environment.GetEnvironmentVariable("AI__ENDPOINT") ?? "http://localhost:11434");
        var model = Environment.GetEnvironmentVariable("AI__MODEL");

        // Auto-detect: if Ollama is running, use it; otherwise fall back to Echo.
        if (model is null)
        {
            try
            {
                using var http = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                var json = http.GetStringAsync($"{endpoint}api/tags").GetAwaiter().GetResult();
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("models", out var models) && models.GetArrayLength() > 0)
                    model = models[0].GetProperty("name").GetString();
            }
            catch { /* Ollama not available */ }
        }

        if (model is not null)
            return (new OllamaChatClient(endpoint, model), $"Ollama ({model})");

        return (new EchoChatClient(), "Echo (demo mode)");
    }
}
