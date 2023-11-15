//---------------------------------------------------------------------
// <copyright file="PooledByteBufferWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1_OR_GREATER
using System;
using System.Buffers;
using System.Diagnostics;

namespace Microsoft.OData.Json
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

        /// <summary>
        /// Creates an instance of <see cref="PooledByteBufferWriter"/> to which data can be written,
        /// with a specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity"></param>
        public PooledByteBufferWriter(int initialCapacity) : this()
        {
            Debug.Assert(initialCapacity > 0);

            _rentedBuffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
            _index = 0;
        }

        /// <summary>
        /// Gets a <see cref="ReadOnlyMemory{byte}"/> that contains the data
        /// written to the underlying buffer so far.
        /// </summary>
        public ReadOnlyMemory<byte> WrittenMemory
        {
            get
            {
                Debug.Assert(_rentedBuffer != null);
                Debug.Assert(_index <= _rentedBuffer.Length);
                return _rentedBuffer.AsMemory(0, _index);
            }
        }

        /// <summary>
        /// Gets the amount of data written to the underlying buffer.
        /// </summary>
        public int WrittenCount
        {
            get
            {
                Debug.Assert(_rentedBuffer != null);
                return _index;
            }
        }

        /// <summary>
        /// Gets the total amount of space within the underlying buffer.
        /// </summary>
        public int Capacity
        {
            get
            {
                Debug.Assert(_rentedBuffer != null);
                return _rentedBuffer.Length;
            }
        }

        /// <summary>
        /// Gets the amount of available space that can be written to
        /// without forcing the underlying buffer to grow.
        /// </summary>
        public int FreeCapacity
        {
            get
            {
                Debug.Assert(_rentedBuffer != null);
                return _rentedBuffer.Length - _index;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="count"></param>
        public void Advance(int count)
        {
            Debug.Assert(_rentedBuffer != null);
            Debug.Assert(count >= 0);
            Debug.Assert(_index <= _rentedBuffer.Length - count);
            _index += count;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="sizeHint"></param>
        /// <returns></returns>
        public Memory<byte> GetMemory(int sizeHint = MinimumBufferSize)
        {
            CheckAndResizeBuffer(sizeHint);
            return _rentedBuffer.AsMemory(_index);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="sizeHint"></param>
        /// <returns></returns>
        public Span<byte> GetSpan(int sizeHint = MinimumBufferSize)
        {
            if (sizeHint == 0)
            {
                sizeHint = MinimumBufferSize;
            }

            CheckAndResizeBuffer(sizeHint);
            return _rentedBuffer.AsSpan(_index);
        }

        /// <summary>
        /// Clears the data written to the underlying buffer.
        /// </summary>
        public void Clear()
        {
            ClearHelper();
        }

        /// <summary>
        /// Returns the rented memory back to the pool.
        /// </summary>
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

        private void ClearHelper()
        {
            Debug.Assert(_rentedBuffer != null);
            Debug.Assert(_index <= _rentedBuffer.Length);

            _rentedBuffer.AsSpan(0, _index).Clear();
            _index = 0;
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