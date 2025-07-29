using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonByteArrayWriter : ODataJsonWriter<byte[]>
{
    public override ValueTask Write(byte[] value, ODataJsonWriterState state)
    {
        state.JsonWriter.WriteBase64StringValue(value);
        return ValueTask.CompletedTask;
    }
}
