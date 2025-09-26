using Microsoft.OData.Serializer.Core;
using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Json.Writers;

internal sealed class ODataJsonInt32Writer<TCustomState> : ODataWriter<int, ODataWriterState<TCustomState>>
{

    public override bool Write(int value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
