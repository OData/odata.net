using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json.Writers;

internal static class JsonStringChunkWriter
{
    /// <summary>
    /// Represents the threshold value used for determining whether to use stackalloc for char arrays.
    /// </summary>
    private const int StackallocCharThreshold = 128;

    // In the worst case, an ASCII character represented as a single utf-8 byte could expand 6x when escaped.
    // For example: '+' becomes '\u0043'
    // Escaping surrogate pairs (represented by 3 or 4 utf-8 bytes) would expand to 12 bytes (which is still <= 6x).
    // The same factor applies to utf-16 characters.
    private const int MaxExpansionFactorWhileEscaping = 6;

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
