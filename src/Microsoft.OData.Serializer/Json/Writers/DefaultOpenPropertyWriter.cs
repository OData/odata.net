
namespace Microsoft.OData.Serializer;

internal class DefaultOpenPropertyWriter<TCustomState> : IOpenPropertyHandler<TCustomState>
{
    internal static readonly DefaultOpenPropertyWriter<TCustomState> Instance = new();

    public void WriteOpenProperty<TValue>(ReadOnlySpan<char> name, TValue value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WritePropertyName(name);

        // TODO: consider supporting resumability in dynamic properties
        while (!state.WriteValue(value)) { } // Write to completion
    }

    public void WriteOpenProperty<TValue>(ReadOnlySpan<byte> name, TValue value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WritePropertyName(name);
        while (!state.WriteValue(value)) { } // Write to completion
    }
}
