namespace Microsoft.OData.Serializer;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ODataTypeAttribute(string fullTypeName) : Attribute
{
    public string FullTypeName { get; } = fullTypeName;
}
