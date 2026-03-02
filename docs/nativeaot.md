# NativeAOT Support

## Overview

Agibuild.Fulora supports NativeAOT publishing for improved startup time and smaller binaries.

## Requirements

- .NET 10+ SDK
- Platform-specific AOT toolchain

## Enabling NativeAOT

Add to your `.csproj`:

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
</PropertyGroup>
```

Then publish for your target runtime:

```bash
dotnet publish -r linux-x64
dotnet publish -r win-x64
dotnet publish -r osx-arm64
```

## Compatibility Notes

- All bridge services use source-generated serialization (no reflection)
- `IBridgeServiceRegistration` ensures AOT-safe service registration
- The Bridge.Generator emits trimmer-safe code

## Platform Support Matrix

| Platform   | Status      |
|-----------|-------------|
| Windows x64 | Supported |
| macOS arm64 | Supported |
| Linux x64   | Supported |
| iOS        | Not yet tested |
| Android    | Not supported (use Mono) |

## Known Limitations

- Dynamic assembly loading is not supported
- Reflection-based serialization will fail; use source-generated `JsonSerializer`
- Some third-party libraries may not be AOT-compatible; verify dependencies before enabling
