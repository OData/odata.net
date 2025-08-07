using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonByteArrayWriter<TCustomState> : ODataJsonWriter<byte[], TCustomState>
{
    public override bool Write(byte[] value, ODataJsonWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteBase64StringValue(value);
        return true;
    }
}
