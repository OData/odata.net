namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonNullableUshortWriter<TCustomState> : ODataJsonWriter<ushort?, TCustomState>
{
    public override bool Write(ushort? value, ODataWriterState<TCustomState> state)
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
