# Fulora

Web-first product velocity with Avalonia-native performance, control, and security.

## Documentation Hub

This project targets a **framework-grade C# + web development model** while preserving **standalone WebView control integration flexibility**.
One runtime core supports both paths.

Use this page as the entry point based on what you want to do next.

## Start Here

- **Build your first app**: [Getting Started](articles/getting-started.md)
- **Understand architecture decisions**: [Architecture](articles/architecture.md)
- **See a real sample experience**: [Demo: Avalonia + React Hybrid App](demo/index.md)
- **Check product direction and phases**: [Roadmap](../openspec/ROADMAP.md)
- **Review goals and positioning**: [Project Vision & Goals](../openspec/PROJECT.md)

## Developer Resources

- **[CLI Reference](cli.md)** — `fulora new`, `dev`, `generate types`, `add service`
- **[Bridge DevTools Panel](bridge-devtools-panel.md)** — In-app debug overlay for bridge call inspection
- **[Plugin Authoring Guide](plugin-authoring-guide.md)** — Create and publish bridge plugins (NuGet + npm)
- **[Documentation Site Deployment](docs-site-deploy.md)** — How the docs site is built and deployed
- **[Release Checklist](release-checklist.md)** — Steps for publishing a new release

## Features

- **Type-Safe Bridge**: `[JsExport]` / `[JsImport]` attributes with Roslyn Source Generator for AOT-compatible C# ↔ JS interop
- **SPA Hosting**: Embedded resource serving with custom `app://` scheme, SPA router fallback, dev server proxy
- **Cross-Platform**: Windows (WebView2), macOS/iOS (WKWebView), Android (WebView), Linux (WebKitGTK)
- **Testable**: `MockBridgeService` for unit testing without a real browser
- **Secure**: Origin-based policy, rate limiting, protocol versioning

## Current Product Objective

Current roadmap focus is **Phase 10: Production Operations & Ecosystem Maturity**:

- Auto-update framework with policy-governed check/download/apply lifecycle
- DI integration for all post-1.0 services (config, telemetry, message bus, auto-update)
- OpenTelemetry provider package for bridge call observability
- NativeAOT publish validation and CI enforcement
- GTK/Linux promotion from preview to production-ready

## Roadmap Snapshot

| Phase | Focus | Status |
|---|---|---|
| Phase 0–3 | Foundation, Bridge, SPA, Polish | ✅ Done |
| Phase 4 | Application Shell | ✅ Done |
| Phase 5 | Framework Positioning | ✅ Done |
| Phase 6 | Governance Productization | ✅ Done |
| Phase 7 | Release Orchestration | ✅ Done |
| Phase 8 | Bridge V2 & Platform Parity | ✅ Done |
| Phase 9 | GA Release (1.0.0) | ✅ Done |
| Phase 10 | Production Operations & Ecosystem | 🚧 Active |
| Phase 11 | Cross-Framework & Plugin Ecosystem | Planned |
| Phase 12 | Advanced Runtime & Performance | Planned |
