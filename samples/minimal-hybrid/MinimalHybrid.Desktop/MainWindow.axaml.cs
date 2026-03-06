using Agibuild.Fulora;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace MinimalHybrid.Desktop;

public partial class MainWindow : Window
{
    private int _clickCount;
    private Border? _overlayPanel;
    private Border? _reopenBadge;

    public MainWindow()
    {
        InitializeComponent();

        WebView.EnvironmentOptions = new WebViewEnvironmentOptions { EnableDevTools = true };

        BuildOverlayPanel();
        BuildReopenBadge();
        ShowOverlay();

        Loaded += async (_, _) =>
        {
            await WebView.NavigateToStringAsync("""
                <html>
                <body style="margin:0; font-family:system-ui; background:linear-gradient(135deg,#667eea 0%,#764ba2 100%); color:#fff; display:flex; align-items:center; justify-content:center; height:100vh;">
                  <div style="text-align:center;">
                    <h1 style="font-size:2.5em; margin-bottom:0.3em;">Fulora WebView</h1>
                    <p style="font-size:1.2em; opacity:0.85;">Native Avalonia overlay is rendered above this web content.</p>
                    <p style="font-size:0.9em; opacity:0.6; margin-top:2em;">Try dragging / resizing the window — the overlay follows.</p>
                  </div>
                </body>
                </html>
                """);

            WebView.Bridge.Expose<MinimalHybrid.Bridge.IAppService>(new MinimalHybrid.Bridge.AppService());
        };
    }

    private void ShowOverlay()
    {
        WebView.OverlayContent = _overlayPanel;
    }

    private void HideOverlay()
    {
        WebView.OverlayContent = _reopenBadge;
    }

    private void BuildReopenBadge()
    {
        var openBtn = new Button
        {
            Content = "\u2726 Show Overlay",
            FontSize = 13,
            Padding = new Thickness(12, 6),
            Foreground = Brushes.White,
            Background = new SolidColorBrush(Color.Parse("#CC4444CC")),
        };
        openBtn.Click += (_, _) => ShowOverlay();

        _reopenBadge = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(16),
            CornerRadius = new CornerRadius(8),
            Child = openBtn,
        };
    }

    private void BuildOverlayPanel()
    {
        var btn = new Button
        {
            Content = "Click Me",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            FontSize = 16,
            Padding = new Thickness(12, 8),
        };
        btn.Click += (_, _) =>
        {
            _clickCount++;
            btn.Content = $"Clicked {_clickCount}x";
        };

        var toggleBtn = new Button
        {
            Content = "Toggle WebView Zoom",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            FontSize = 14,
            Padding = new Thickness(12, 8),
        };
        var zoomed = false;
        toggleBtn.Click += (_, _) =>
        {
            zoomed = !zoomed;
            WebView.ZoomFactor = zoomed ? 1.5 : 1.0;
            toggleBtn.Content = zoomed ? "Reset Zoom" : "Toggle WebView Zoom";
        };

        var closeBtn = new Button
        {
            Content = "\u2715",
            FontSize = 14,
            Padding = new Thickness(4, 0),
            HorizontalAlignment = HorizontalAlignment.Right,
            Background = Brushes.Transparent,
            Foreground = Brushes.Gray,
        };
        closeBtn.Click += (_, _) => HideOverlay();

        var header = new Grid();
        header.Children.Add(new TextBlock
        {
            Text = "Avalonia Overlay",
            Foreground = Brushes.White,
            FontWeight = FontWeight.Bold,
            FontSize = 18,
            VerticalAlignment = VerticalAlignment.Center,
        });
        header.Children.Add(closeBtn);

        var panel = new StackPanel { Spacing = 10 };
        panel.Children.Add(header);
        panel.Children.Add(new TextBlock
        {
            Text = "These are native Avalonia controls\nrendered above the WebView.",
            Foreground = Brushes.LightGray,
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap,
        });
        panel.Children.Add(btn);
        panel.Children.Add(toggleBtn);

        _overlayPanel = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#DD222222")),
            CornerRadius = new CornerRadius(16),
            Padding = new Thickness(24),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(24),
            MinWidth = 240,
            Child = panel,
        };
    }
}
