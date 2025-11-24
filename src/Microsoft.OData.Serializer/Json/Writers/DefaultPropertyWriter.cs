
using System.Buffers;

namespace Microsoft.OData.Serializer;

internal class DefaultPropertyWriter<TCustomState> : IPropertyWriter<TCustomState>
{
    public static readonly DefaultPropertyWriter<TCustomState> Instance = new();

    public bool WriteProperty<T>(ReadOnlySpan<char> propertyName, T value, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        if (state.Stack.Current.PropertyProgress < PropertyProgress.Name)
        {
            jsonWriter.WritePropertyName(propertyName);
            state.Stack.Current.PropertyProgress = PropertyProgress.Name;
        }

        return state.WriteValue(value);
    }

    public void WritePropertyToBuffer<T>(
        Action<IBufferWriter<byte>, T, ODataWriterState<TCustomState>> writeAction,
        ReadOnlySpan<char> propertyName,
        T value,
        ODataWriterState<TCustomState> state)
    {

        state.JsonWriter.WritePropertyName(propertyName);
        state.WriteValueToBufferWriter(writeAction, value, state);
    }
}
