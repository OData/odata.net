namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonFloatWriter<TCustomState> : ODataJsonWriter<float, TCustomState>
{
    public override bool Write(float value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
