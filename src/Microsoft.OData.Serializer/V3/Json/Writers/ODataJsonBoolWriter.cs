using Microsoft.OData.Serializer.V3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonBoolWriter<TCustomState> : ODataJsonWriter<bool, TCustomState>
{
    public override bool Write(bool value, ODataJsonWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteBooleanValue(value);
        return true;
    }
}
