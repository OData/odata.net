
namespace Microsoft.OData.Serializer;

internal class DefaultDynamicPropertyWriter<TCustomState> : IDynamicPropertyWriter<TCustomState>
{
    internal static readonly DefaultDynamicPropertyWriter<TCustomState> Instance = new();

    public void WriteDynamicProperty<TValue>(ReadOnlySpan<char> name, TValue value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WritePropertyName(name);

        // TODO: consider supporting resumability in dynamic properties
        while (!state.WriteValue(value)) { } // Write to completion
    }

    public void WriteDynamicProperty<TValue>(ReadOnlySpan<byte> name, TValue value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WritePropertyName(name);
        while (!state.WriteValue(value)) { } // Write to completion
    }
}
