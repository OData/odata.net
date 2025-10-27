namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonNullableShortWriter<TCustomState> : ODataJsonWriter<short?, TCustomState>
{
    public override bool Write(short? value, ODataWriterState<TCustomState> state)
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
