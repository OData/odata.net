using Microsoft.OData.Serializer.Core;
using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonDoubleWriter<TCustomState> : ODataWriter<double, ODataWriterState<TCustomState>>
{
    public override bool Write(double value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
