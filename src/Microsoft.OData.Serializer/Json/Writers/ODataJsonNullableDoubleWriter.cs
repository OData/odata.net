
namespace Microsoft.OData.Serializer;

internal class ODataJsonNullableDoubleWriter<TCustomState> : ODataWriter<double?, ODataWriterState<TCustomState>>
{
    public override bool Write(double? value, ODataWriterState<TCustomState> state)
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
