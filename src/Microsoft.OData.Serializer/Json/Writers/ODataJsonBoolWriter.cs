
namespace Microsoft.OData.Serializer;

internal class ODataJsonBoolWriter<TCustomState> : ODataJsonWriter<bool, TCustomState>
{
    public override bool Write(bool value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteBooleanValue(value);
        return true;
    }

    public override bool Read(ODataReaderState<TCustomState> state, out bool value)
    {
        using var reader = state.GetJsonReaderScope();
        value = reader.Reader.GetBoolean();
        return true;
    }
}
