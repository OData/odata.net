using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonUriWriter<TCustomState> : ODataJsonWriter<Uri, TCustomState>
{
    public override ValueTask Write(Uri value, ODataJsonWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteStringValue(value.ToString());
        return ValueTask.CompletedTask;
    }
}
