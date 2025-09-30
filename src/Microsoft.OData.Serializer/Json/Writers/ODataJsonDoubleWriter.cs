
namespace Microsoft.OData.Serializer;

internal class ODataJsonDoubleWriter<TCustomState> : ODataWriter<double, ODataWriterState<TCustomState>>
{
    public override bool Write(double value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
