namespace Microsoft.OData.Serializer;

/// <summary>
/// The attribute is applied on a class to specify a custom writer for a specific property
/// that is not necessarily defined on the class.
/// </summary>
/// <param name="propertyName"></param>
/// <param name="writerType">A custom type that extends one of the ODataPropertyValueWriter variants.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
// TODO: internal because it's not yet supported.
internal sealed class ODataPropertyValueWriterAttribute(string propertyName, Type writerType) : Attribute
{
    public string PropertyName { get; } = propertyName;
    public Type WriterType { get; } = writerType;
}
