using System.Buffers;

namespace Microsoft.OData.Serializer;

internal readonly struct ReadResult<T>(ReadOnlySequence<T> buffer, bool isCompleted)
{
    public ReadOnlySequence<T> Buffer { get; } = buffer;
    public bool IsCompleted { get; } = isCompleted;
}
