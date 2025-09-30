namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonUshortWriter<TCustomState> : ODataJsonWriter<ushort, TCustomState>
{
    public override bool Write(ushort value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
