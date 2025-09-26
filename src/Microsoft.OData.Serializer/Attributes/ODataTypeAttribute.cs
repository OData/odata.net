namespace Microsoft.OData.Serializer;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = false)]
public sealed class ODataTypeAttribute(string fullTypeName) : Attribute
{
    public string FullTypeName { get; } = fullTypeName;
}
