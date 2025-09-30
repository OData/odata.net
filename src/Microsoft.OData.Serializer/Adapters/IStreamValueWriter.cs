
namespace Microsoft.OData.Serializer;

public interface IStreamValueWriter<TCustomState>
{
    // Writes a value completely, does not perform I/O. Entire value will be buffered.
    // Suitable to efficiently write values that can fit in the buffer or when
    // you don't mind growing the buffer.
    void WriteValue<T>(T value, ODataWriterState<TCustomState> state);
    // Writes value completely, flushes if buffer fills up to avoid resizing.
    // flushing may perform async I/O depending on the underlying stream implementation.
    ValueTask WriteValueAsync<T>(T value, ODataWriterState<TCustomState> state);
    // The WriteXXSegment APIs give the caller control on how to handle chunking.
    // They're expected to call the API iteratively one chunk at time.
    // They're also responsible for flushing if they want to keep buffer size from growing.
    void WriteStringSegment(ReadOnlySpan<char> value, bool isFinalBlock, ODataWriterState<TCustomState> state);
    void WriteBinarySegment(ReadOnlySpan<byte> value, bool isFinalBlock,ODataWriterState<TCustomState> state);

    // Tentatively, does it make sense to have these "convenience" APIs for common case?
    // They could also be implemented as extension methods on top of the WriteXXSegment APIs
    // ValueTask WriteStringValueAsync(Stream, encoding, state);
    // ValueTask WriteStringValueAsync(PipeReader, encoding, state);
    // ValueTask WriteBinaryValueAsync(Stream, state);
    // ValueTask WriteBinaryValueAsync(PipeReader, state);
    // ValueTask WriteBinaryValueAsync(IAsyncEnumerable<ReadOnlyMemory<byte>>, state);

    /// <summary>
    /// Returns whether it's recommended to flush the internal buffer to the
    /// output stream. If this returns true, then consider calling <see cref="FlushAsync"/>.
    /// If you continue writing to the buffer without flushing, the buffer may grow
    /// which may impact performance and memory use.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    bool ShouldFlush(ODataWriterState<TCustomState> state);

    // Flushes the contents of the buffer to the underlying output stream
    ValueTask FlushAsync(ODataWriterState<TCustomState> state);
    // Flushes the contents of the buffer if it's (almost) full. Otherwise it does nothing.
    // Not sure if it's a good idea to expose this to caller. Code smell.
    ValueTask FlushIfBufferFullAsync(ODataWriterState<TCustomState> state);
}
