using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Still evaluating its utility.")]
internal class PooledByteSegmentBufferWriter(int initialCapacity) : IBufferWriter<byte>, IDisposable
{
    private BufferSegmentCollection _segments = new(initialCapacity);
    public void Advance(int count)
    {
        _segments.Advance(count);
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        return _segments.GetMemory(sizeHint);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        return _segments.GetSpan(sizeHint);
    }

    public ValueTask WriteToStreamAsync(Stream destination, CancellationToken cancellationToken)
    {
        return _segments.WriteToStreamAsync(destination, cancellationToken);
    }

    public void Clear()
    {
        _segments.Clear();
    }

    public void Dispose()
    {
        _segments.Dispose();
    }

    struct BufferSegmentCollection(int initialCapacity)
    {
        InlineSegmentArray _inlineSegments;
        public const int MaxInlineSegments = 64;

        BufferSegment[]? _segments;
        int _numSegments;
        int _allocatedSegments; // number of segments that have been allocated, does not get reset when buffer is cleared.
        int _currSegment;

        public Span<byte> GetSpan(int minSize)
        {
            EnsureCurrentSegmentHasFreeSpace(minSize);

            if (_currSegment < MaxInlineSegments)
            {
                var inlineSpan = _inlineSegments[_currSegment].GetSpan(minSize);
                Debug.Assert(inlineSpan.Length >= minSize, "Not enough free space in the current inline segment");

                return inlineSpan;
            }

            Debug.Assert(_segments != null);
            var span = _segments[_currSegment - MaxInlineSegments].GetSpan(minSize);
            Debug.Assert(span.Length >= minSize, "Not enough free space in the current array segment");
            return span;
        }

        public Memory<byte> GetMemory(int minSize)
        {
            EnsureCurrentSegmentHasFreeSpace(minSize);

            if (_currSegment < MaxInlineSegments)
            {
                var inlineMemory = _inlineSegments[_currSegment].GetMemory(minSize);
                Debug.Assert(inlineMemory.Length >= minSize, "Not enough free space in the current inline segment");

                return inlineMemory;
            }

            Debug.Assert(_segments != null);
            var memory = _segments[_currSegment - MaxInlineSegments].GetMemory(minSize);
            Debug.Assert(memory.Length >= minSize, "Not enough free space in the current array segment");
            return memory;
        }

        private int EnsureCurrentSegmentHasFreeSpace(int minSize)
        {
            if (_numSegments == 0)
            {
                if (_numSegments == _allocatedSegments)
                {
                    _inlineSegments[0] = new BufferSegment(initialCapacity);
                    _allocatedSegments++;
                }

                _currSegment = 0;
                _numSegments = 1;

                return 0;
            }

            if (_currSegment < MaxInlineSegments)
            {
                ref var segment = ref _inlineSegments[_currSegment];
                if (segment.FreeSpace >= minSize)
                {
                    return _currSegment;
                }

                // Not enough space, check next segment
                _currSegment++;
            }

            if (_currSegment < MaxInlineSegments)
            {
                // if we get here, it means we've just incremented
                // _currSegment because it did not have enough space, so let's allocated a new one
                if (_currSegment == _allocatedSegments)
                {
                    _inlineSegments[_currSegment] = new BufferSegment(Math.Max(initialCapacity, minSize));
                    _allocatedSegments++;
                }

                _numSegments++;
                return _currSegment;
            }

            // We have exhausted inline segments,
            // let's check if we have any array segments
            if (_segments == null)
            {
                _segments = ArrayPool<BufferSegment>.Shared.Rent(MaxInlineSegments * 2);
                _segments[0] = new BufferSegment(Math.Max(initialCapacity, minSize));
                _numSegments++;
                _currSegment++;
                return _currSegment;
            }

            if (_currSegment < MaxInlineSegments + _segments.Length)
            {
                ref var segment = ref _segments[_currSegment - MaxInlineSegments];
                if (segment.FreeSpace >= minSize)
                {
                    return _currSegment;
                }

                // Not enough space, check next segment, resize if necessary
                _currSegment++;
                EnsureSufficientSegments();
            }

            Debug.Assert(_currSegment < MaxInlineSegments + _segments.Length);

            // if we get here, it means we've just incremented
            // _currSegment because it did not have enough space, so let's allocated a new
            // buffer segment in the heap array
            if (_currSegment == _allocatedSegments)
            {
                _segments[_currSegment - MaxInlineSegments] = new BufferSegment(Math.Max(initialCapacity, minSize));
                _allocatedSegments++;
            }

            _numSegments++;
            return _currSegment;
        }
            

        public void Advance(int count)
        {
            Debug.Assert(count >= 0, "Count must be non-negative");
            Debug.Assert(_currSegment < _numSegments, "Current segment index is out of bounds");
            if (_currSegment < MaxInlineSegments)
            {
                _inlineSegments[_currSegment].Advance(count);
            }
            else
            {
                _segments![_currSegment - MaxInlineSegments].Advance(count);
            }
        }

        public async ValueTask WriteToStreamAsync(Stream destination, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(destination);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            for (int i = 0; i < _numSegments && i < MaxInlineSegments; i++)
            {
                var memory = _inlineSegments[i].WrittenMemory;
                if (memory.Length > 0)
                {
                    await destination.WriteAsync(memory, cancellationToken);
                }
            }

            if (_numSegments < MaxInlineSegments || _segments == null)
            {
                // No array segments to write
                return;
            }

            int numArraySegments = _numSegments - MaxInlineSegments;

            for (int i = 0; i < numArraySegments; i++)
            {
                var memory = _segments![i].WrittenMemory;
                if (memory.Length > 0)
                {
                    await destination.WriteAsync(memory, cancellationToken);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < MaxInlineSegments; i++)
            {
                _inlineSegments[i].Clear();
            }

            if (_segments != null)
            {
                foreach (var segment in _segments)
                {
                    segment.Clear();
                }
            }

            _numSegments = 0;
        }

        public void Dispose()
        {
            foreach (var segment in _inlineSegments)
            {
                segment.Dispose();
            }

            if (_segments != null)
            {
                foreach (var segment in _segments)
                {
                    segment.Dispose();
                }

                ArrayPool<BufferSegment>.Shared.Return(_segments);
            }
        }

        private void EnsureSufficientSegments()
        {
            if (_currSegment == MaxInlineSegments + _segments!.Length)
            {
                // We have exhausted all inline segments and array segments
                // Let's allocate a new array segment
                var newSegments = ArrayPool<BufferSegment>.Shared.Rent(_segments.Length * 2);
                Array.Copy(_segments, newSegments, _segments.Length);
                ArrayPool<BufferSegment>.Shared.Return(_segments);
                _segments = newSegments;
            }
        }
    }

    struct BufferSegment(int sizeHint)
    {
        byte[] _buffer = ArrayPool<byte>.Shared.Rent(sizeHint);
        int consumed;

        public readonly int FreeSpace => _buffer.Length - consumed;

        public ReadOnlySpan<byte> WrittenSpan
        {
            get
            {
                if (_buffer == null)
                {
                    return default;
                }
                return _buffer.AsSpan(0, consumed);
            }
        }

        public ReadOnlyMemory<byte> WrittenMemory
        {
            get
            {
                if (_buffer == null)
                {
                    return default;
                }
                return _buffer.AsMemory(0, consumed);
            }
        }

        public Span<byte> GetSpan(int minSize)
        {
            Debug.Assert(_buffer != null);
            Debug.Assert(FreeSpace >= minSize, "Not enough free space in the buffer");

            return _buffer.AsSpan(consumed);
        }

        public Memory<byte> GetMemory(int minSize)
        {
            Debug.Assert(_buffer != null);
            Debug.Assert(FreeSpace >= minSize, "Not enough free space in the buffer");
            return _buffer.AsMemory(consumed);
        }

        public void Advance(int count)
        {
            Debug.Assert(_buffer != null);
            Debug.Assert(count >= 0, "Count must be non-negative");
            Debug.Assert(consumed + count <= _buffer.Length, "Cannot advance beyond the buffer length");
            consumed += count;
        }

        public void Clear()
        {
            consumed = 0;
        }

        public void Dispose()
        {
            if (_buffer != null)
            {
                ArrayPool<byte>.Shared.Return(_buffer);
                _buffer = null;
                consumed = 0;
            }
        }
    }

    [InlineArray(BufferSegmentCollection.MaxInlineSegments)]
    struct InlineSegmentArray
    {
        BufferSegment _segment;
    }
}
