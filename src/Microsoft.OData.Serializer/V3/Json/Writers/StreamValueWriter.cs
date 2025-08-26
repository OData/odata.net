using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class StreamValueWriter<TCustomState> : IStreamValueWriter<TCustomState>
{
    public ValueTask FlushAsync(ODataJsonWriterState<TCustomState> state)
    {
        return state.FlushAsync();
    }

    public ValueTask FlushIfFull(ODataJsonWriterState<TCustomState> state)
    {
        if (state.ShouldFlush())
        {
            return state.FlushAsync();
        }

        return ValueTask.CompletedTask;
    }

    public void WriteBinarySegment(ReadOnlySpan<byte> value, ODataJsonWriterState<TCustomState> state)
    {
        throw new NotImplementedException();
    }

    public void WriteStringSegment(ReadOnlySpan<char> value, ODataJsonWriterState<TCustomState> state)
    {
        throw new NotImplementedException();
    }

    public void WriteStringSegment(ReadOnlySpan<byte> value, ODataJsonWriterState<TCustomState> state)
    {
        throw new NotImplementedException();
    }

    public void WriteValue<T>(T value, ODataJsonWriterState<TCustomState> state)
    {
        throw new NotImplementedException();
    }
}
