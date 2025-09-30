using Microsoft.OData.Serializer;
using ODataSamples.FileServiceLib.Streaming;
using System.Buffers;
using System.Text;

namespace ODataSamples.FileServiceLib.Serialization.OData;

internal static class StreamingSourceWriter
{
    private const int DefaultBufferSize = 8192;

    public static async ValueTask WriteTextStreamAsync(IStreamingSource source, IStreamValueWriter<ODataCustomState> writer, ODataWriterState<ODataCustomState> state)
    {
        Decoder decoder = Encoding.Unicode.GetDecoder();

        // OData serializer currently does not support writing text data from a byte buffer source,
        // but that's a planned feature.
        // Meanwhile, you need to implement the byte to string transcoding yourself.

        char[] charBuffer = ArrayPool<char>.Shared.Rent(DefaultBufferSize);
        try
        {
            bool firstChunk = true;
            await foreach (var chunk in source.GetStreamingBytes())
            {
                int start = 0;

                // Skip BOM (UTF-16 LE) on the very first chunk to match StreamReader behavior
                if (firstChunk)
                {
                    firstChunk = false;
                    if (chunk.Length >= 2 && chunk.Span[0] == 0xFF && chunk.Span[1] == 0xFE)
                    {
                        start = 2; // skip BOM
                    }
                }

                while (start < chunk.Length)
                {
                    decoder.Convert(chunk.Span.Slice(start), charBuffer.AsSpan(), flush: false, out int bytesUsed, out int charsUsed, out bool completed);
                    if (charsUsed > 0)
                    {
                        writer.WriteStringSegment(charBuffer, isFinalBlock: false, state);
                    }

                    start += bytesUsed;
                    if (bytesUsed == 0 && !completed)
                    {
                        break; // safety
                    }
                }

                // Periodically flush the writer to avoid growing the internal buffer.
                if (writer.ShouldFlush(state))
                {
                    await writer.FlushAsync(state);
                }
            }

            // Flush any remaining state in the decoder
            decoder.Convert(ReadOnlySpan<byte>.Empty, charBuffer.AsSpan(), flush: true, out _, out int finalChars, out _);
            if (finalChars > 0)
            {
                writer.WriteStringSegment(charBuffer, isFinalBlock: true, state);
            }
            else
            {
                writer.WriteStringSegment([], isFinalBlock: true, state);
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(charBuffer);
        }
    }
}
