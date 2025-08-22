using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.IO;

internal class AsyncEnumerableReader<T>(IAsyncEnumerable<ReadOnlyMemory<T>> source) : IBufferedReader<T> where T : struct
{
    BufferSegment? first = null;
    BufferSegment? last = null;

    private int indexInFirst = 0;
    bool examinedEverything = false;

    IAsyncEnumerator<ReadOnlyMemory<T>> enumerator = source.GetAsyncEnumerator();

    public void AdvanceTo(in SequencePosition consumed)
    {
        AdvanceTo(consumed, consumed);
    }

    public void AdvanceTo(in SequencePosition consumed, in SequencePosition examined)
    {
        var consumedSegment = (BufferSegment)consumed.GetObject();
        var consumedIndex = consumed.GetInteger();

        // We don't have a way to release segments or communicate to the source that
        // we're done with them, so we just move forward.
        first = consumedSegment;
        indexInFirst = consumedIndex;
        var examinedTo = (BufferSegment)examined.GetObject();
        var indexInExamined = examined.GetInteger();

        examinedEverything = examinedTo == last && indexInExamined == last.Memory.Length - 1;
    }

    public ValueTask DisposeAsync()
    {
        first = null;
        last = null;
        indexInFirst = 0;
        return enumerator.DisposeAsync();
    }

    public async ValueTask<ReadResult<T>> ReadAsync()
    {
        // if examined everything, fetch more data, otherwise return current sequence.
        if (await enumerator.MoveNextAsync())
        {
            
            var memory = enumerator.Current;
            AppendSegment(memory);
            return new ReadResult<T>(GetCurrentSequence(), isCompleted: false);
        }

        return default;
    }

    public bool TryRead(out ReadResult<T> result)
    {
        throw new NotImplementedException();
        // if current sequence is empty return false
        // if examined is not null and == last segment, and examinedIndex = last.Length, return false
        // if examined is null, return current sequence
        // if examined is not null, return current sequence
    }

    public ReadOnlySequence<T> GetCurrentSequence()
    {
        if (first == null)
        {
            return ReadOnlySequence<T>.Empty;
        }
        
        Debug.Assert(last != null);
        return new ReadOnlySequence<T>(first, indexInFirst, last, last.Memory.Length);
    }

    private BufferSegment AppendSegment(ReadOnlyMemory<T> memory)
    {
        if (first == null)
        {
            first = new BufferSegment(memory);
            last = first;
        }
        else
        {
            Debug.Assert(last != null);
            last = last.Append(memory);
        }

        return last;
    }

    class BufferSegment : ReadOnlySequenceSegment<T>
    {
        public BufferSegment(ReadOnlyMemory<T> memory)
        {
            Memory = memory;
        }
        public BufferSegment Append(ReadOnlyMemory<T> memory)
        {
            var segment = new BufferSegment(memory)
            {
                RunningIndex = this.RunningIndex + this.Memory.Length
            };
            this.Next = segment;
            return segment;
        }
    }
}
