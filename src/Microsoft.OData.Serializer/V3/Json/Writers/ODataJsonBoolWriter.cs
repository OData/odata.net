using Microsoft.OData.Serializer.V3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonBoolWriter : ODataJsonWriter<bool>
{
    public override ValueTask Write(bool value, ODataJsonWriterState state)
    {
        state.JsonWriter.WriteBooleanValue(value);
        return ValueTask.CompletedTask;
    }
}
