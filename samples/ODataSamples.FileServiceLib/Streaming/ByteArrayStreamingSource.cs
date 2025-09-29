using System;
using System.Collections.Generic;
using System.Text;

namespace ODataSamples.FileServiceLib.Streaming;

internal class ByteArrayStreamingSource(byte[] array, int chunkSize = ByteArrayStreamingSource.DefaultChunkSize) : IStreamingSource, IAsyncEnumerable<ReadOnlyMemory<byte>>
{
    const int DefaultChunkSize = 8192;

    public async IAsyncEnumerator<ReadOnlyMemory<byte>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        int remaining = array.Length;
        while (remaining > 0)
        {
            int currentChunkSize = Math.Min(remaining, chunkSize);
            yield return new ReadOnlyMemory<byte>(array, array.Length - remaining, currentChunkSize);
            remaining -= currentChunkSize;
        }
    }

    public IAsyncEnumerable<ReadOnlyMemory<byte>> GetStreamingBytes()
    {
        return this;
    }
}
