using Microsoft.OData.Serializer.V3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal sealed class ODataJsonInt32Writer<TCustomState> : ODataWriter<int, ODataJsonWriterState<TCustomState>>
{

    public override bool Write(int value, ODataJsonWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
