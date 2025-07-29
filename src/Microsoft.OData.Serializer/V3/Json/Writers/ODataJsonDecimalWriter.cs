using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonDecimalWriter : ODataJsonWriter<decimal>
{
    public override ValueTask Write(decimal value, ODataJsonWriterState state)
    {
        // TODO: support write as string
        state.JsonWriter.WriteNumberValue(value);
        return ValueTask.CompletedTask;
    }
}
