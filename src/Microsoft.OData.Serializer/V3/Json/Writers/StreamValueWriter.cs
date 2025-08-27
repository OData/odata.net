using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class StreamValueWriter<TCustomState> : IStreamValueWriter<TCustomState>
{
    public static StreamValueWriter<TCustomState> Instance { get; } = new StreamValueWriter<TCustomState>();
    public ValueTask FlushAsync(ODataWriterState<TCustomState> state)
    {
        return state.FlushAsync();
    }

    public ValueTask FlushIfFull(ODataWriterState<TCustomState> state)
    {
        if (state.ShouldFlush())
        {
            return state.FlushAsync();
        }

        return ValueTask.CompletedTask;
    }

    public void WriteBinarySegment(ReadOnlySpan<byte> value, bool isFinalBlock, ODataWriterState<TCustomState> state)
    {
        var bufferWriter = state.BufferWriter;

        // TODO how to tell if first segment
        if (state.TryStartBase64SegmentScope())
        {
            state.JsonWriter.Flush(); // Commit pending bytes to the buffer writer before we start writing to it.
            bufferWriter.Write([JsonConstants.DoubleQuote]);
        }

        int numTrailingBytes = state.GetTrailingBase64BytesLength();
        // If we have trailing bytes from previous segment, we need to prepend them to this segment.
        if (numTrailingBytes > 0)
        {
            // Instead of allocating a new segment, let's move some bytes from this segment
            // into the temp buffer. We need at least 3 bytes to encode without trailing.
            // State 1: trailingBytes.Length + segment.Length >= 3, so we can encode without trailing
            if (numTrailingBytes + value.Length >= 3)
            {
                int bytesNeeded = 3 - numTrailingBytes;

                value[..bytesNeeded].CopyTo(state.trailingBase64BytesBuffer[numTrailingBytes..]);
                var trailingDestination = bufferWriter.GetSpan(Base64.GetMaxEncodedToUtf8Length(3));

                OperationStatus encodingResult = Base64.EncodeToUtf8(
                    state.trailingBase64BytesBuffer,
                    trailingDestination,
                    out int trailingBytesConsumed,
                    out int trailingBytesWritten
                    );

                Debug.Assert(encodingResult == OperationStatus.Done, "Encoding 3 bytes should always succeed.");
                Debug.Assert(trailingBytesConsumed == 3, "All 3 bytes should be consumed.");
                state.BufferWriter.Advance(trailingBytesWritten);
                value = value[bytesNeeded..];
            }
            else
            {
                // Not enough bytes to complete a 3-byte block, just store them and return.
                Debug.Assert(value.Length <= 2);
                value.CopyTo(state.trailingBase64BytesBuffer[numTrailingBytes..]);
                state.SetTrailingBase64Bytes(state.trailingBase64BytesBuffer[..(numTrailingBytes + value.Length)]);
                return;
            }
        }

        // It's possible that max encoded length could end up larger than the free buffer capacity.
        // In this case the bufferWriter will grow the underlying buffer.
        // If we want to avoid the resizing, we could either return false and signal the caller to flush,
        // but the free capacity is still below the flush threshold, so we would need to add another flag
        // to force the caller to flush. Alternatively, we could adjust the chunk size downwards
        // so that the encoded length fits within the capacity. If we can't do that, then we could fall back to growing the buffer.
        var maxEncodedLength = Base64.GetMaxEncodedToUtf8Length(value.Length);
        Span<byte> destination = bufferWriter.GetSpan(maxEncodedLength);
        OperationStatus result = Base64.EncodeToUtf8(value, destination, out int bytesConsumed, out int bytesWritten, isFinalBlock);
        Debug.Assert(result == OperationStatus.Done || result == OperationStatus.NeedMoreData, "Base64 encoding should succeed or indicate more data is needed.");

        bufferWriter.Advance(bytesWritten);

        if (bytesConsumed < value.Length)
        {
            Debug.Assert(value.Length - bytesConsumed < 3, "Base64 encoding should only leave up to 2 trailing bytes.");
            state.SetTrailingBase64Bytes(value[bytesConsumed..]);
        }

        if (isFinalBlock)
        {
            bufferWriter.Write([JsonConstants.DoubleQuote]);
            state.EndBase64SegmentScope();
        }
    }

    public void WriteStringSegment(ReadOnlySpan<char> value, bool isFinalBlock, ODataWriterState<TCustomState> state)
    {
        throw new NotImplementedException();
    }

    public void WriteStringSegment(ReadOnlySpan<byte> value, bool isFinalBlock, ODataWriterState<TCustomState> state)
    {
        throw new NotImplementedException();
    }

    public void WriteValue<T>(T value, ODataWriterState<TCustomState> state)
    {
        // Write to completion. This can overflow the buffer and cause
        // a resize.
        while (!state.WriteValue(value)) { }
    }

    public ValueTask WriteValueAsync<T>(T value, ODataWriterState<TCustomState> state)
    {
        while (!state.WriteValue(value))
        {
            if (state.ShouldFlush())
            {
                return FlushAndWriteRemainderAsync(value, state);
            }
        }

        return ValueTask.CompletedTask;

        static async ValueTask FlushAndWriteRemainderAsync(T value, ODataWriterState<TCustomState> state)
        {
            await state.FlushAsync();
            while (!state.WriteValue(value))
            {
                if (state.ShouldFlush())
                {
                    await state.FlushAsync();
                }
            }
        }
    }
}
