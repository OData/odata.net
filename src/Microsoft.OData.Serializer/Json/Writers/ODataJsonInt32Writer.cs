
namespace Microsoft.OData.Serializer;

internal sealed class ODataJsonInt32Writer<TCustomState> : ODataWriter<int, ODataWriterState<TCustomState>>
{

    public override bool Write(int value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
