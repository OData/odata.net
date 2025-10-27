
namespace Microsoft.OData.Serializer;

internal class ODataJsonNullableBoolWriter<TCustomState> : ODataJsonWriter<bool?, TCustomState>
{
    public override bool Write(bool? value, ODataWriterState<TCustomState> state)
    {
        if (!value.HasValue)
        {
            state.JsonWriter.WriteNullValue();
            return true;
        }

        state.JsonWriter.WriteBooleanValue(value.Value);
        return true;
    }
}
