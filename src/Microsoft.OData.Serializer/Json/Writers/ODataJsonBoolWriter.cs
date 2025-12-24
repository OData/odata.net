
namespace Microsoft.OData.Serializer;

internal class ODataJsonBoolWriter<TCustomState> : ODataJsonWriter<bool, TCustomState>
{
    public override bool Write(bool value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteBooleanValue(value);
        return true;
    }

    public override bool Read(ODataReaderState<TCustomState> state, out bool value)
    {
        var reader = state.GetJsonReader();
        if (!reader.Read())
        {
            throw new Exception("Unexpected end of JSON input.");
        }

        value = reader.GetBoolean();
        state.SaveJsonReaderState(ref reader);
        return true;
    }
}
