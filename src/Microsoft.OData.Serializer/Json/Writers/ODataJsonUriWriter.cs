using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonUriWriter<TCustomState> : ODataJsonWriter<Uri, TCustomState>
{
    public override bool Write(Uri value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteStringValue(value.ToString());
        return true;
    }
}
