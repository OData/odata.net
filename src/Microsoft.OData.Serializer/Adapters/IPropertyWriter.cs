
using System.Buffers;

namespace Microsoft.OData.Serializer;

public interface IPropertyWriter<TCustomState>
{
    bool WriteProperty<T>(ReadOnlySpan<char> propertyName, T value, ODataWriterState<TCustomState> state);
    void WritePropertyToBuffer<T>(
        Action<IBufferWriter<byte>, T, ODataWriterState<TCustomState>> writeAction,
        ReadOnlySpan<char> propertyName,
        T value,
        ODataWriterState<TCustomState> state);
}
