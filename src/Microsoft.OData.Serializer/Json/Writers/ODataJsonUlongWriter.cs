namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonUlongWriter<TCustomState> : ODataJsonWriter<ulong, TCustomState>
{
    public override bool Write(ulong value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
