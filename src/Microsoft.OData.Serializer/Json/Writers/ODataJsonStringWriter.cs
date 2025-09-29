
namespace Microsoft.OData.Serializer;

internal class ODataJsonStringWriter<TCustomState> : ODataJsonWriter<string, TCustomState>
{
    public override bool Write(string value, ODataWriterState<TCustomState> state)
    {
        if (value == null)
        {
            state.JsonWriter.WriteNullValue();
            return true;
        }

        // We use a fixed threshold here to ensure that the condition always returns the same result
        // for the same input value. Ideally, we should write value if it can fit the buffer capacity,
        // but if's possibly that the buffer capacity changes between calls such that the if statement
        // is not taken the first iteration, but taken the second and lead to invalid values.
        // Perhaps we should hold some state that tells us whether we are in chunking mode or not.
        if (value.Length < 1024 * 16) //state.FreeBufferCapacity)
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
