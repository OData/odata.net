
namespace Microsoft.OData.Serializer;

internal class ODataJsonDecimalWriter<TCustomState> : ODataJsonWriter<decimal, TCustomState>
{
    public override bool Write(decimal value, ODataWriterState<TCustomState> state)
    {
        // TODO: support write as string
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
