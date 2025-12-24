
namespace Microsoft.OData.Serializer;

internal class ODataJsonDoubleWriter<TCustomState> : ODataJsonWriter<double, TCustomState>
{
    public override bool Write(double value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
