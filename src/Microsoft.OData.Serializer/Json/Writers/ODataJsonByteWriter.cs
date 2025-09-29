using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonByteWriter<TCustomState> : ODataJsonWriter<byte, TCustomState>
{
    public override bool Write(byte value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
