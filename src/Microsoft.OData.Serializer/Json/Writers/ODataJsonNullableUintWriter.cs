namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonNullableUintWriter<TCustomState> : ODataJsonWriter<uint?, TCustomState>
{
    public override bool Write(uint? value, ODataWriterState<TCustomState> state)
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
