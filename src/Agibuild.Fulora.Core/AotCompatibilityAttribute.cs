namespace Agibuild.Fulora;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class AotCompatibilityAttribute : Attribute
{
    public bool IsAotCompatible { get; }

    public AotCompatibilityAttribute(bool isAotCompatible = true) => IsAotCompatible = isAotCompatible;
}
