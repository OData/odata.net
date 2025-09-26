using System.Buffers;

namespace Microsoft.OData.Serializer.IO;

internal readonly struct BufferedReadResult<T>(ReadOnlySequence<T> buffer, bool isCompleted)
{
    public ReadOnlySequence<T> Buffer { get; } = buffer;
    public bool IsCompleted { get; } = isCompleted;
}
