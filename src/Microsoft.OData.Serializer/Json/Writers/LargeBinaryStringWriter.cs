using Microsoft.OData.Serializer.Json.State;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.OData.Serializer.Json.Writers;

internal class LargeBinaryStringWriter<TCustomState>
{

    internal const int ChunkSize = 2048;

    public static bool WriteNextChunkFromByteArray(ReadOnlySpan<byte> value, ODataWriterState<TCustomState> state)
    {
        // TODO: In .NET 10, we should use WriteBase64ValueSegment to write chunks.
        // In < .NET 10 we bypass the Utf8JsonWriter and write directly to the buffer writer.
        // Since Utf8JsonWriter is not aware of this write, it might lead to incorrect or missed
        // placement of the comma separator, particularly in arrays. To get around this,
        // we would need to check whether to manually place the comma before writing any other value.
        // This leads to a lot of bookeeping across before and after use of Utf8JsonWriter. That's
        // what we had to do with the ODataUtf8JsonWriter in the legacy serializer.
        // For convenience, we don't do that bookkeeping here since this is still an early preview
        // and we expect large strings to only exists as field values in objects, no array elements.
        // In the official release we may or may not need the bookkeeping depending on whether or not
        // we support .NET < 10.
        var bufferWriter = state.BufferWriter;
        var encoder = state.JavaScriptEncoder;
        int chunkStart = state.Stack.Current.EnumeratorIndex;
        if (chunkStart == value.Length)
        {
            return true;
        }

        // This is a "safer" option of the commented version below, but if the commented
        // version is faster, we can consider using it and extra checks in debug mode.
        if (state.Stack.Current.ResourceProgress < ResourceWriteProgress.Value)
        {
            state.JsonWriter.Flush(); // Commit pending bytes to the buffer writer before we start writing to it.
            bufferWriter.Write([JsonConstants.DoubleQuote]);
            state.Stack.Current.ResourceProgress = ResourceWriteProgress.Value;
        }

        int chunkEnd = Math.Min(chunkStart + ChunkSize, value.Length);
        ReadOnlySpan<byte> chunk = value[chunkStart..chunkEnd];

        bool isFinalBlock = chunkEnd == value.Length;

        // It's possible that max encoded length could end up larger than the free buffer capacity.
        // In this case the bufferWriter will grow the underlying buffer.
        // If we want to avoid the resizing, we could either return false and signal the caller to flush,
        // but the free capacity is still below the flush threshold, so we would need to add another flag
        // to force the caller to flush. Alternatively, we could adjust the chunk size downwards
        // so that the encoded length fits within the capacity. If we can't do that, then we could fall back to growing the buffer.
        var maxEncodedLength = Base64.GetMaxEncodedToUtf8Length(chunk.Length);
        Span<byte> destination = bufferWriter.GetSpan(maxEncodedLength);
        OperationStatus result = Base64.EncodeToUtf8(chunk, destination, out int bytesConsumed, out int bytesWritten, isFinalBlock);
        Debug.Assert(result == OperationStatus.Done || result == OperationStatus.NeedMoreData, "Base64 encoding should succeed or indicate more data is needed.");

        bufferWriter.Advance(bytesWritten);

        int totalCharsConsumed = chunkStart + bytesConsumed;

        if (totalCharsConsumed < value.Length)
        {
            // Update the enumerator index for the next chunk.
            state.Stack.Current.EnumeratorIndex = totalCharsConsumed;
            return false; // More chunks to write.
        }

        bufferWriter.Write([JsonConstants.DoubleQuote]);

        return true;
    }
}
