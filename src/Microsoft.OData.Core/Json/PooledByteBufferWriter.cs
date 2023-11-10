#if NETCOREAPP3_1_OR_GREATER
using Microsoft.OData;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json
{
    /// <summary>
    /// An implementation of <see cref="IBufferWriter{byte}"/> that rents
    /// buffers from an array pool instead of allocating a new array every time.
    /// </summary>
    internal sealed class PooledByteBufferWriter : IBufferWriter<byte>, IDisposable
    {
        // This class allows two possible configurations: if rentedBuffer is not null then
        // it can be used as an IBufferWriter and holds a buffer that should eventually be
        // returned to the shared pool. If rentedBuffer is null, then the instance is in a
        // cleared/disposed state and it must re-rent a buffer before it can be used again.
        private byte[]? _rentedBuffer;
        private int _index;

        private const int MinimumBufferSize = 256;

        // Value copied from Array.MaxLength in System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Array.cs.
        public const int MaximumBufferSize = 0X7FFFFFC7;

        private PooledByteBufferWriter()
        {
        }

        public PooledByteBufferWriter(int initialCapacity) : this()
        {
            Debug.Assert(initialCapacity > 0);

            _rentedBuffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
            _index = 0;
        }

        public ReadOnlyMemory<byte> WrittenMemory
        {
            get
            {
                Debug.Assert(_rentedBuffer != null);
                Debug.Assert(_index <= _rentedBuffer.Length);
                return _rentedBuffer.AsMemory(0, _index);
            }
        }

        public int WrittenCount
        {
            get
            {
                Debug.Assert(_rentedBuffer != null);
                return _index;
            }
        }

        public int Capacity
        {
            get
            {
                Debug.Assert(_rentedBuffer != null);
                return _rentedBuffer.Length;
            }
        }

        public int FreeCapacity
        {
            get
            {
                Debug.Assert(_rentedBuffer != null);
                return _rentedBuffer.Length - _index;
            }
        }

        public void Clear()
        {
            ClearHelper();
        }

        public void ClearAndReturnBuffers()
        {
            Debug.Assert(_rentedBuffer != null);

            ClearHelper();
            byte[] toReturn = _rentedBuffer;
            _rentedBuffer = null;
            ArrayPool<byte>.Shared.Return(toReturn);
        }

        private void ClearHelper()
        {
            Debug.Assert(_rentedBuffer != null);
            Debug.Assert(_index <= _rentedBuffer.Length);

            _rentedBuffer.AsSpan(0, _index).Clear();
            _index = 0;
        }

        // Returns the rented buffer back to the pool
        public void Dispose()
        {
            if (_rentedBuffer == null)
            {
                return;
            }

            ClearHelper();
            byte[] toReturn = _rentedBuffer;
            _rentedBuffer = null;
            ArrayPool<byte>.Shared.Return(toReturn);
        }

        public void InitializeEmptyInstance(int initialCapacity)
        {
            Debug.Assert(initialCapacity > 0);
            Debug.Assert(_rentedBuffer is null);

            _rentedBuffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
            _index = 0;
        }

        public static PooledByteBufferWriter CreateEmptyInstanceForCaching() => new PooledByteBufferWriter();

        public void Advance(int count)
        {
            Debug.Assert(_rentedBuffer != null);
            Debug.Assert(count >= 0);
            Debug.Assert(_index <= _rentedBuffer.Length - count);
            _index += count;
        }

        public Memory<byte> GetMemory(int sizeHint = MinimumBufferSize)
        {
            CheckAndResizeBuffer(sizeHint);
            return _rentedBuffer.AsMemory(_index);
        }

        public Span<byte> GetSpan(int sizeHint = MinimumBufferSize)
        {
            // TODO: this condition is here cause without it the sizeHint is getting set to 0
            // instead of the MinimumBufferSize when the method is called without params.
            // I don't understand why this is happening, will investigate later
            if (sizeHint == 0)
            {
                sizeHint = MinimumBufferSize;
            }

            CheckAndResizeBuffer(sizeHint);
            return _rentedBuffer.AsSpan(_index);
        }

        internal ValueTask WriteToStreamAsync(Stream destination, CancellationToken cancellationToken)
        {
            return destination.WriteAsync(WrittenMemory, cancellationToken);
        }

        internal void WriteToStream(Stream destination)
        {
            destination.Write(WrittenMemory.Span);
        }

        private void CheckAndResizeBuffer(int sizeHint)
        {
            Debug.Assert(_rentedBuffer != null);
            Debug.Assert(sizeHint > 0);

            int currentLength = _rentedBuffer.Length;
            int availableSpace = currentLength - _index;

            // If we've reached ~1GB written, grow to the maximum buffer
            // length to avoid incessant minimal growths causing perf issues.
            if (_index >= MaximumBufferSize / 2)
            {
                sizeHint = Math.Max(sizeHint, MaximumBufferSize - currentLength);
            }

            if (sizeHint > availableSpace)
            {
                int growBy = Math.Max(sizeHint, currentLength);

                int newSize = currentLength + growBy;

                if ((uint)newSize > MaximumBufferSize)
                {
                    newSize = currentLength + sizeHint;
                    if ((uint)newSize > MaximumBufferSize)
                    {
                        throw new OutOfMemoryException(Strings.ODataMessageWriter_Buffer_Maximum_Size_Exceeded(newSize));
                    }
                }

                byte[] oldBuffer = _rentedBuffer;

                _rentedBuffer = ArrayPool<byte>.Shared.Rent(newSize);

                Debug.Assert(oldBuffer.Length >= _index);
                Debug.Assert(_rentedBuffer.Length >= _index);

                Span<byte> oldBufferAsSpan = oldBuffer.AsSpan(0, _index);
                oldBufferAsSpan.CopyTo(_rentedBuffer);
                oldBufferAsSpan.Clear();
                ArrayPool<byte>.Shared.Return(oldBuffer);
            }

            Debug.Assert(_rentedBuffer.Length - _index > 0);
            Debug.Assert(_rentedBuffer.Length - _index >= sizeHint);
        }
    }
}
#endif