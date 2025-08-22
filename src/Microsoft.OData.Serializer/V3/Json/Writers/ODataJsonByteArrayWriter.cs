using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonByteArrayWriter<TCustomState> : ODataJsonWriter<byte[], TCustomState>
{
    public override bool Write(byte[] value, ODataJsonWriterState<TCustomState> state)
    {
        // Base64 encoding expands length by 4/3
        if (value.Length * 4.0 / 3 < state.FreeBufferCapacity)
        {
            state.JsonWriter.WriteBase64StringValue(value);
            return true;
        }

        if (state.ShouldFlush())
        {
            return false;
        }

        state.Stack.Push();
        bool success = LargeBinaryStringWriter<TCustomState>.WriteNextChunkFromByteArray(value, state);

        // In .NET 10, we can leverage Utf8JsonWriter.WriteBase64ValueSegment
        // to write chunks. But in < .NET 10, we have to handle
        // that manually.


        state.Stack.Pop(success);
        return success;
    }
}
