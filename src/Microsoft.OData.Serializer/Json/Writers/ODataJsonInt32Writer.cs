using Microsoft.OData.Serializer.Core;
using Microsoft.OData.Serializer.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json.Writers;

internal sealed class ODataJsonInt32Writer<TCustomState> : ODataWriter<int, ODataWriterState<TCustomState>>
{

    public override bool Write(int value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
