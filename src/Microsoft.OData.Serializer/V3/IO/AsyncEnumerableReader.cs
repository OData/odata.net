using Microsoft.OData.Serializer.V3.Json.Writers;
using System;
using System.Buffers;
using System.Diagnostics;

namespace Microsoft.OData.Serializer.V3.IO;

internal class AsyncEnumerableReader<T>(
    IAsyncEnumerable<ReadOnlyMemory<T>> source)
    : IBufferedReader<T> where T : struct
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
        var consumedSegment = consumed.GetObject() as BufferSegment;
        Debug.Assert(consumedSegment != null);
        var consumedIndex = consumed.GetInteger();

        first = consumedSegment;
        indexInFirst = consumedIndex;

        var examinedTo = examined.GetObject() as BufferSegment;
        Debug.Assert(examinedTo != null);
        var indexInExamined = examined.GetInteger();

        examinedEverything = examinedTo == last && indexInExamined == last.Memory.Length - 1;
    }

    public ValueTask DisposeAsync()
    {
        return enumerator.DisposeAsync();
    }

    public ValueTask<BufferedReadResult<T>> ReadAsync()
    {
        if (TryRead(out var result))
        {
            return ValueTask.FromResult(result);
        }

        return FetchMoreDataAsync(this);

        static async ValueTask<BufferedReadResult<T>> FetchMoreDataAsync(AsyncEnumerableReader<T> reader)
        {
            do
            {
                var moreData = await reader.enumerator.MoveNextAsync();
                if (!moreData)
                {
                    return new(
                        reader.GetCurrentSequence(),
                        isCompleted: true);
                }
            } while (reader.enumerator.Current.IsEmpty);

            if (reader.first == null)
            {
                reader.first = new BufferSegment(reader.enumerator.Current);
                reader.last = reader.first;
            }
            else
            {
                Debug.Assert(reader.last != null);
                reader.last = reader.last.Append(reader.enumerator.Current);
            }

            return new(
                reader.GetCurrentSequence(),
                isCompleted: false);
        }
    }

    public bool TryRead(out BufferedReadResult<T> result)
    {
        if (examinedEverything)
        {
            // If everything in the current buffer has been examined,
            // consumer should call ReadAsync() before calling TryRead() again.
            result = default;
            return false;
        }

        var curSeq = this.GetCurrentSequence();
        if (curSeq.IsEmpty)
        {
            result = default;
            return false;
        }

        // If not examined everything, we should
        // return everything that has not yet been consumed
        result = new BufferedReadResult<T>(
            curSeq,
            isCompleted: false);

        return true;
    }

    private ReadOnlySequence<T> GetCurrentSequence()
    {
        if (first == null)
        {
            Debug.Assert(last == null);
            return ReadOnlySequence<T>.Empty;
        }

        Debug.Assert(last != null);
        return new ReadOnlySequence<T>(first, indexInFirst, last, last.Memory.Length - 1);
    }

    class BufferSegment : ReadOnlySequenceSegment<T>
    {
        public BufferSegment(ReadOnlyMemory<T> segment)
        {
            Memory = segment;
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
