using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal static class LargeStringJsonWriter<TCustomState>
{
    private const byte DoubleQuote = (byte)'"';

    /// <summary>
    /// Represents the threshold value used for determining whether to use stackalloc for char arrays.
    /// </summary>
    private const int StackallocCharThreshold = 128;

    // In the worst case, an ASCII character represented as a single utf-8 byte could expand 6x when escaped.
    // For example: '+' becomes '\u0043'
    // Escaping surrogate pairs (represented by 3 or 4 utf-8 bytes) would expand to 12 bytes (which is still <= 6x).
    // The same factor applies to utf-16 characters.
    private const int MaxExpansionFactorWhileEscaping = 6;

    internal const int ChunkSize = 2048;

    public static bool WriteNextChunkFromString(ReadOnlySpan<char> value, ODataJsonWriterState<TCustomState> state)
    {
        // TODO: In .NET 10, we should use WriteStringValueSegment to write chunks.
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
        if (state.Stack.Current.ResourceProgress != State.ResourceWriteProgress.Value)
        {
            state.JsonWriter.Flush(); // Commit pending bytes to the buffer writer before we start writing to it.
            bufferWriter.Write([DoubleQuote]);
            state.Stack.Current.ResourceProgress = State.ResourceWriteProgress.Value;
        }

        //if (chunkStart == 0)
        //{
        //    // Is it possible to re-enter this method with chunkStart == 0?
        //    // That would only be possible if the first chunk did not contain
        //    // enough data for a full escape. But that should not happen
        //    // if string value has > 6 chars. If string < 6 chars, we should
        //    // not reach this method, whe should write single pass.
        //    // TODO: so we must ensure that 
        //    bufferWriter.Write([DoubleQuote]);
        //}

        int chunkEnd = Math.Min(chunkStart + ChunkSize, value.Length);
        ReadOnlySpan<char> chunk = value[chunkStart..chunkEnd];

        bool isFinalBlock = chunkEnd == value.Length;
        int chunkCharsConsumed = WriteSingleChunk(chunk, bufferWriter, encoder, isFinalBlock);
        int totalCharsConsumed = chunkStart + chunkCharsConsumed;

        if (totalCharsConsumed < value.Length)
        {
            // Update the enumerator index for the next chunk.
            state.Stack.Current.EnumeratorIndex = totalCharsConsumed;
            return false; // More chunks to write.
        }

        bufferWriter.Write([DoubleQuote]);

        return true;
    }

    public static int WriteSingleChunk(ReadOnlySpan<char> chunk, IBufferWriter<byte> bufferWriter, JavaScriptEncoder encoder, bool isFinalBlock)
    {
        // Try to write chunk.
        // return number chars written.
        int firstIndexToEscape = NeedsEscaping(chunk, encoder);

        Debug.Assert(firstIndexToEscape >= -1 && firstIndexToEscape < chunk.Length);

        if (firstIndexToEscape != -1)
        {
            return WriteEscapedStringChunk(chunk, bufferWriter, encoder, firstIndexToEscape, isFinalBlock);
        }

        return WriteStringChunk(chunk, bufferWriter, isFinalBlock);
    }

    private static unsafe int NeedsEscaping(ReadOnlySpan<char> value, JavaScriptEncoder encoder)
    {
        // Some implementations of JavaScriptEncoder.FindFirstCharacterToEncode may not accept
        // null pointers and guard against that. Hence, check up-front to return -1.
        if (value.IsEmpty)
        {
            return -1;
        }

        fixed (char* ptr = value)
        {
            return encoder.FindFirstCharacterToEncode(ptr, value.Length);
        }
    }

    private static int WriteEscapedStringChunk(ReadOnlySpan<char> sourceChunk, IBufferWriter<byte> bufferWriter, JavaScriptEncoder encoder, int firstIndexToEscape, bool isFinalBlock)
    {
        char[]? valueArray = null;

        int maxSize = firstIndexToEscape + MaxExpansionFactorWhileEscaping * (sourceChunk.Length - firstIndexToEscape);

        Span<char> destination = maxSize <= StackallocCharThreshold
            ? stackalloc char[maxSize]
            : valueArray = ArrayPool<char>.Shared.Rent(maxSize);

        OperationStatus status = encoder.Encode(
               sourceChunk,
               destination,
               out int charsConsumed,
               out int charsWritten,
               isFinalBlock
           );

        Debug.Assert(status == OperationStatus.Done || status == OperationStatus.NeedMoreData);

        WriteStringChunk(destination[..charsWritten], bufferWriter, isFinalBlock);

        if (valueArray != null)
        {
            ArrayPool<char>.Shared.Return(valueArray);
        }

        return charsConsumed;
    }

    /// <summary>
    /// Writes a chunk of the string into the buffer, converting it to UTF-8 encoding.
    /// </summary>
    /// <param name="chunk">The chunk of the string to be written into the buffer.</param>
    /// <param name="isFinalBlock">Indicates whether the chunk is the final block of the string.</param>
    /// <remarks>
    /// This method converts the provided chunk of the string from UTF-16 encoding to UTF-8 encoding
    /// and writes it into the buffer. It ensures that the buffer has sufficient space for the encoded
    /// string and notifies the buffer writer of the write operation. This method assumes that the UTF-8
    /// encoding does not exceed the maximum byte count specified by the UTF-8 encoding for the given
    /// chunk of the string.
    /// </remarks>
    private static int WriteStringChunk(ReadOnlySpan<char> chunk, IBufferWriter<byte> bufferWriter, bool isFinalBlock)
    {
        int maxUtf8Length = Encoding.UTF8.GetMaxByteCount(chunk.Length);

        Span<byte> output = bufferWriter.GetSpan(maxUtf8Length);
        OperationStatus status = Utf8.FromUtf16(chunk, output, out int charsRead, out int charsWritten, isFinalBlock);
        Debug.Assert(status == OperationStatus.Done);

        // The charsRead will always be equal to chunk.Length. This is because the characters
        // that would cause utf-8 encoding to result in partial processing is already 
        // taken care of by the JavascriptEncoder when escaping special characters. 
        Debug.Assert(charsRead == chunk.Length);

        // notify the bufferWriter of the write
        bufferWriter.Advance(charsWritten);

        return charsRead;
    }
}
