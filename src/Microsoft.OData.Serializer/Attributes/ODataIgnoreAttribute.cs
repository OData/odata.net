namespace Microsoft.OData.Serializer;

/// <summary>
/// When applied to a property, specifies whether and under which condition the property should be ignored during OData serialization.
/// </summary>
/// <param name="condition"></param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ODataIgnoreAttribute(ODataIgnoreCondition condition = ODataIgnoreCondition.Always) : Attribute
{
    public ODataIgnoreCondition Condition { get; } = condition;
}
