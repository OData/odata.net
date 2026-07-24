
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
        if (!reader.Read())
        {
            throw new Exception("Unexpected end of JSON input.");
        }
        value = reader.GetInt32();
        state.SaveJsonReaderState(ref reader);
        return true;
    }
}
