
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
        var reader = state.GetJsonReader();
        value = reader.GetInt32();
        state.SaveJsonReaderState(ref reader);
        return true;
    }
}
