using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonUriWriter : ODataJsonWriter<Uri>
{
    public override ValueTask Write(Uri value, ODataJsonWriterState state)
    {
        state.JsonWriter.WriteStringValue(value.ToString());
        return ValueTask.CompletedTask;
    }
}
