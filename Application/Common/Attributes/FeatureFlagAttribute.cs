namespace Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class FeatureFlagAttribute(string featureFlag, Type alternativeHandlerType) : Attribute
{
    public string FeatureFlag { get; } = featureFlag;
    public Type AlternativeHandlerType { get; } = alternativeHandlerType;
}