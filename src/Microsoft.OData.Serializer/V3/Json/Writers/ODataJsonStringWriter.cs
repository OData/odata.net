using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonStringWriter<TCustomState> : ODataJsonWriter<string, TCustomState>
{
    public override ValueTask Write(string value, ODataJsonWriterState<TCustomState> state)
    {
        // TODO: We should handle resumable writes for long strings.
        state.JsonWriter.WriteStringValue(value);
        return ValueTask.CompletedTask;
    }
}
