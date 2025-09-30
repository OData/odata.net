namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonShortWriter<TCustomState> : ODataJsonWriter<short, TCustomState>
{
    public override bool Write(short value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
