using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal partial class StreamValueWriter<TCustomState> : IStreamValueWriter<TCustomState>
{
    public static StreamValueWriter<TCustomState> Instance { get; } = new StreamValueWriter<TCustomState>();
    public ValueTask FlushAsync(ODataWriterState<TCustomState> state)
    {
        return state.FlushAsync();
    }

    public ValueTask FlushIfBufferGettingFullAsync(ODataWriterState<TCustomState> state)
    {
        if (state.ShouldFlush())
        {
            return state.FlushAsync();
        }

        return ValueTask.CompletedTask;
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
