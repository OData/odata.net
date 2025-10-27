namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonNullableSByteWriter<TCustomState> : ODataJsonWriter<sbyte?, TCustomState>
{
    public override bool Write(sbyte? value, ODataWriterState<TCustomState> state)
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
