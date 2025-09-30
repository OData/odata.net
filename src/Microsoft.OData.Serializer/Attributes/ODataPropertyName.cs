namespace Microsoft.OData.Serializer;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ODataPropertyNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
