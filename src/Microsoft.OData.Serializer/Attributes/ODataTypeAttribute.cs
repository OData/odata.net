namespace Microsoft.OData.Serializer.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ODataTypeAttribute(string fullTypeName) : Attribute
{
    public string FullTypeName { get; } = fullTypeName;
}
