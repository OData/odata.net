namespace Microsoft.OData.Serializer.IO;

internal interface IBufferedReader<T> : IAsyncDisposable where T : struct
{
    bool TryRead(out BufferedReadResult<T> result);

    ValueTask<BufferedReadResult<T>> ReadAsync();

    void AdvanceTo(in SequencePosition consumed);

    void AdvanceTo(in SequencePosition consumed, in SequencePosition position);
}
