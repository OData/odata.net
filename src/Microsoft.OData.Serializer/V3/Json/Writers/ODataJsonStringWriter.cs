using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonStringWriter<TCustomState> : ODataJsonWriter<string, TCustomState>
{
    public override bool Write(string value, ODataWriterState<TCustomState> state)
    {
        if (value.Length < state.FreeBufferCapacity)
        {
            state.JsonWriter.WriteStringValue(value);
            return true;
        }

        if (state.ShouldFlush())
        {
            return false;
        }

        state.Stack.Push();
        bool success = LargeStringJsonWriter<TCustomState>.WriteNextChunkFromString(value, state);

        // In .NET 10, we can leverage Utf8JsonWriter.WriteStringSegment
        // to write chunks. But in < .NET 10, we have to handle
        // that manually.

        // Select chunk size.
        // Commit current Utf8JsonWriter contents to buffer writer.
        // Find next chunk to write from the state.
        // Transcode and write chunk to buffer.
        // state.JsonWriter.Flush();


        state.Stack.Pop(success);
        return success;
    }
}
