using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonDateTimeWriter : ODataJsonWriter<DateTime>
{
    public override ValueTask Write(DateTime value, ODataJsonWriterState state)
    {
        state.JsonWriter.WriteStringValue(value);
        return ValueTask.CompletedTask;
    }
}
