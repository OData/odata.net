using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class StreamValueWriter<TCustomState> : IStreamValueWriter<TCustomState>
{
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

    public void WriteBinarySegment(ReadOnlySpan<byte> value, ODataWriterState<TCustomState> state)
    {
        throw new NotImplementedException();
    }

    public void WriteBinarySegment(ReadOnlySpan<byte> value, bool isFinalBlock, ODataWriterState<TCustomState> state)
    {
        throw new NotImplementedException();
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
