
namespace Microsoft.OData.Serializer;

internal sealed class ODataJsonNullableInt32Writer<TCustomState> : ODataWriter<int?, ODataWriterState<TCustomState>>
{

    public override bool Write(int? value, ODataWriterState<TCustomState> state)
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
