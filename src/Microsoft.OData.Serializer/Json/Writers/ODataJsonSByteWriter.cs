namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonSByteWriter<TCustomState> : ODataJsonWriter<sbyte, TCustomState>
{
    public override bool Write(sbyte value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
