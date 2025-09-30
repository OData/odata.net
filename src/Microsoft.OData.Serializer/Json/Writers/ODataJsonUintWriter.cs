namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonUintWriter<TCustomState> : ODataJsonWriter<uint, TCustomState>
{
    public override bool Write(uint value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
