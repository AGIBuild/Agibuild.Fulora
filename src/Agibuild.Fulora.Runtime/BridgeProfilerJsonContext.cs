using System.Text.Json.Serialization;

namespace Agibuild.Fulora;

/// <summary>
/// Source-generated JSON context for <see cref="MethodProfileStats"/> and <see cref="ServiceProfileStats"/>.
/// </summary>
[JsonSerializable(typeof(MethodProfileStats))]
[JsonSerializable(typeof(ServiceProfileStats))]
[JsonSerializable(typeof(List<ServiceProfileStats>))]
internal partial class BridgeProfilerJsonContext : JsonSerializerContext
{
}
