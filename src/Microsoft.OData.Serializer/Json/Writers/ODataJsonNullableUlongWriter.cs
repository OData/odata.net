namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonNullableUlongWriter<TCustomState> : ODataJsonWriter<ulong?, TCustomState>
{
    public override bool Write(ulong? value, ODataWriterState<TCustomState> state)
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
