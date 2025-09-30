namespace Microsoft.OData.Serializer;

/// <summary>
/// This attribute is applied to a property to specify a custom writer for the property's value.
/// The specified custom writer should be a type that inherits from
/// <see cref="ODataAsyncPropertyWriter{TDeclaringType, TValue, TCustomState}"/>.
/// </summary>
/// <param name="writerType">
/// The custom writer type. It should inherit from
/// <see cref="ODataAsyncPropertyWriter{TDeclaringType, TValue, TCustomState}"/>.
/// </param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ODataValueWriterAttribute(Type writerType) : Attribute
{
    public Type WriterType { get; } = writerType;
}
