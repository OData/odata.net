namespace Microsoft.OData.Serializer;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ODataIgnoreAttribute(ODataIgnoreCondition condition = ODataIgnoreCondition.Always) : Attribute
{
    public ODataIgnoreCondition Condition { get; } = condition;
}
