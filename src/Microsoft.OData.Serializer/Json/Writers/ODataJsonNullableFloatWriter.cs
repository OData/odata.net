namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonNullableFloatWriter<TCustomState> : ODataJsonWriter<float?, TCustomState>
{
    public override bool Write(float? value, ODataWriterState<TCustomState> state)
    {
        if (!value.HasValue)
        {
            state.JsonWriter.WriteNullValue();
            return true;
        }

        state.JsonWriter.WriteNumberValue(value.Value);
        return true;
    }
}
