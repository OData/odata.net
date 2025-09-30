
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

    /// <summary>
    /// When true, signals that we should flush the writer to
    /// avoid overflowing the internal buffer. If we don't
    /// flush, the buffer may grow excessively and cause memory issues.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool ShouldFlush(ODataWriterState<TCustomState> state)
    {
        return state.ShouldFlush();
    }

    /// <summary>
    /// Writes the value entirely in memory. Since this does not
    /// flush the buffer, it should be avoided for large string
    /// or byte array values otherwise it can cause memory issues.
    /// For writing large values, consider <see cref="WriteValueAsync{T}(T, ODataWriterState{TCustomState})"/>
    /// instead.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="state"></param>
    public void WriteValue<T>(T value, ODataWriterState<TCustomState> state)
    {
        // Write to completion. This can overflow the buffer and cause
        // a resize.
        state.WriteValueToCompletionInMemory(value);
    }

    /// <summary>
    /// Writes the specified value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="state"></param>
    /// <returns></returns>
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
