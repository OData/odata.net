namespace Microsoft.OData.Serializer.Json.Writers;

using Microsoft.OData.Serializer.Json.State;
using System;
using System.Buffers;
using System.Diagnostics;

internal partial class StreamValueWriter<TCustomState>
{
    public void WriteStringSegment(ReadOnlySpan<char> value, bool isFinalBlock, ODataWriterState<TCustomState> state)
    {
        var bufferWriter = state.BufferWriter;

        if (state.TryStartSegmentedValueScope())
        {
            state.JsonWriter.Flush(); // Commit pending bytes to the buffer writer before we start writing to it.
            bufferWriter.Write([JsonConstants.DoubleQuote]);
        }

        var prevTrailingCharsLength = state.GetTrailingPartialDataLength();
        if (prevTrailingCharsLength > 0)
        {
            Debug.Assert(prevTrailingCharsLength == 1, "String escaping should not leave 2 or more trailing chars.");

            if (value.Length == 0 && !isFinalBlock)
            {
                // Nothing to combine, just return and wait for next segment
                return;
            }

            if (value.Length == 0 && isFinalBlock)
            {
                // We have a trailing char, but this is the final block with no new chars.
                // We need to write out the trailing char now. This an incomplete unicode sequence,
                // but we'll let the encoder deal with that.
                JsonStringChunkWriter.WriteSingleChunk(state.PartialTrailingCharsBuffer[..prevTrailingCharsLength], bufferWriter, state.JavaScriptEncoder, isFinalBlock);
                state.ClearTrailingPartialData();
                state.BufferWriter.Write([JsonConstants.DoubleQuote]);
                state.EndSegmentedValueScope();
                return;
            }

            Span<char> combined = [state.PartialTrailingCharsBuffer[0], value[0]];
            int charsConsumedInPartialData = JsonStringChunkWriter.WriteSingleChunk(combined, bufferWriter, state.JavaScriptEncoder, isFinalBlock && value.Length == 1);
            Debug.Assert(charsConsumedInPartialData == 2);
            state.ClearTrailingPartialData();

            if (isFinalBlock && value.Length == 1)
            {
                // All chars consumed, we're done.
                bufferWriter.Write([JsonConstants.DoubleQuote]);
                state.EndSegmentedValueScope();
                return;
            }

            value = value[1..];
        }

        int charsConsumed = JsonStringChunkWriter.WriteSingleChunk(value, bufferWriter, state.JavaScriptEncoder, isFinalBlock);

        if (charsConsumed < value.Length)
        {
            Debug.Assert(!isFinalBlock, "If not all chars are consumed, it cannot be the final block.");
            var trailingChars = value[charsConsumed..];
            state.SetTrailingChars(trailingChars);
        }
        else
        {
            state.ClearTrailingPartialData();
        }

        if (isFinalBlock)
        {
            bufferWriter.Write([JsonConstants.DoubleQuote]);
            state.EndSegmentedValueScope();
        }
    }
}

