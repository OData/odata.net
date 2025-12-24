
namespace Microsoft.OData.Serializer;

internal sealed class ODataJsonInt32Writer<TCustomState> : ODataJsonWriter<int, TCustomState>
{

    public override bool Write(int value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }

    public override bool Read(ODataReaderState<TCustomState> state, out int value)
    {
        using var scope = state.GetJsonReaderScope();
        value = scope.Reader.GetInt32();
        return true;
    }
}
