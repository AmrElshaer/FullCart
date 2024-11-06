namespace Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class FeatureFlagAttribute : Attribute
{
    public FeatureFlagAttribute(string featureFlag, Type alternativeHandlerType)
    {
        FeatureFlag = featureFlag;
        AlternativeHandlerType = alternativeHandlerType;
    }

    public string FeatureFlag { get; }
    public Type AlternativeHandlerType { get; }
}