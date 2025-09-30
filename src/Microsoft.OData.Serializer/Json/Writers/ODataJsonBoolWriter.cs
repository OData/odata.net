
namespace Microsoft.OData.Serializer;

internal class ODataJsonBoolWriter<TCustomState> : ODataJsonWriter<bool, TCustomState>
{
    public override bool Write(bool value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteBooleanValue(value);
        return true;
    }
}
