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
#pragma warning disable CA1305 // Specify IFormatProvider
        state.JsonWriter.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ssZ")); // Ensure OData format is applied
#pragma warning restore CA1305 // Specify IFormatProvider
        return ValueTask.CompletedTask;
    }
}
