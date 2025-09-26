
namespace Microsoft.OData.Serializer;

internal partial class StreamValueWriter<TCustomState> : IStreamValueWriter<TCustomState>
{
    public static StreamValueWriter<TCustomState> Instance { get; } = new StreamValueWriter<TCustomState>();
    public ValueTask FlushAsync(ODataWriterState<TCustomState> state)
    {
        return state.FlushAsync();
    }

    public ValueTask FlushIfBufferFullAsync(ODataWriterState<TCustomState> state)
    {
        if (state.ShouldFlush())
        {
            return state.FlushAsync();
        }

        return ValueTask.CompletedTask;
    }

    public bool ShouldFlush(ODataWriterState<TCustomState> state)
    {
        return state.ShouldFlush();
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
