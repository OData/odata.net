
namespace Microsoft.OData.Serializer;

public interface IPropertyWriter<TCustomState>
{
    bool WriteProperty<T>(ReadOnlySpan<char> propertyName, T value, ODataWriterState<TCustomState> state);
}
