
namespace Microsoft.OData.Serializer;

public interface IResourcePopertyWriter<TDeclaringType, TCustomState>
{
    // TODO: consider overloads that accept ReadOnlySpan<char> and ReadOnlySpan<byte> as property name
    bool WriteProperty<T>(TDeclaringType resource, string name, T Value, ODataWriterState<TCustomState> state);
    bool WriteProperty<T>(TDeclaringType resource, ODataPropertyInfo<TDeclaringType, TCustomState> propertyInfo, T value, ODataWriterState<TCustomState> state);
    bool WriteProperty(
        TDeclaringType resource,
        ODataPropertyInfo<TDeclaringType, TCustomState> propertyInfo,
        ODataWriterState<TCustomState> state);
}
