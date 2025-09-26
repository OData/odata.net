namespace Microsoft.OData.Serializer.Attributes;


[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ODataValueWriterAttribute(Type writerType) : Attribute
{
    public Type WriterType { get; } = writerType;
}
