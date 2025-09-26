using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonDecimalWriter<TCustomState> : ODataJsonWriter<decimal, TCustomState>
{
    public override bool Write(decimal value, ODataWriterState<TCustomState> state)
    {
        // TODO: support write as string
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
