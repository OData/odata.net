//---------------------------------------------------------------------
// <copyright file="PooledByteBufferWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Buffers;
using System.Diagnostics;

namespace Microsoft.OData.Json
{
    // This class has been adapted from https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/Text/Json/PooledByteBufferWriter.cs
    /// <summary>
    /// An implementation of <see cref="IBufferWriter{T}"/> that rents
    /// buffers from an array pool instead of allocating a new array every time.
    /// </summary>
    internal sealed class PooledByteBufferWriter : IBufferWriter<byte>, IDisposable
    {
        // This class allows two possible configurations: if rentedBuffer is not null then
        // it can be used as an IBufferWriter and holds a buffer that should eventually be
        // returned to the shared pool. If rentedBuffer is null, then the instance is in a
        // cleared/disposed state and it must re-rent a buffer before it can be used again.
        private byte[] rentedBuffer;
        private int index;

        private const int MinimumBufferSize = 256;

        // Value copied from Array.MaxLength in https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Array.cs
        public const int MaximumBufferSize = 0X7FFFFFC7; // 2147483591 (approx 2GB)

        /// <summary>
        /// Creates an instance of <see cref="PooledByteBufferWriter"/> to which data can be written,
        /// with a specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity"></param>
        public PooledByteBufferWriter(int initialCapacity)
        {
            Debug.Assert(initialCapacity > 0);

            this.rentedBuffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
            this.index = 0;
        }

        /// <summary>
        /// Gets a <see cref="ReadOnlyMemory{T}"/> that contains the data
        /// written to the underlying buffer so far.
        /// </summary>
        public ReadOnlyMemory<byte> WrittenMemory
        {
            get
            {
                Debug.Assert(this.rentedBuffer != null);
                Debug.Assert(this.index <= this.rentedBuffer.Length);
                return this.rentedBuffer.AsMemory(0, this.index);
            }
        }

        /// <summary>
        /// Gets the amount of data written to the underlying buffer.
        /// </summary>
        public int WrittenCount
        {
            get
            {
                Debug.Assert(rentedBuffer != null);
                return this.index;
            }
        }

        /// <summary>
        /// Gets the total amount of space within the underlying buffer.
        /// </summary>
        public int Capacity
        {
            get
            {
                Debug.Assert(this.rentedBuffer != null);
                return this.rentedBuffer.Length;
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
                Debug.Assert(this.rentedBuffer != null);
                return this.rentedBuffer.Length - this.index;
            }
        }

        /// <inheritdoc/>
        public void Advance(int count)
        {
            Debug.Assert(this.rentedBuffer != null);
            Debug.Assert(count >= 0);
            Debug.Assert(this.index <= this.rentedBuffer.Length - count);
            this.index += count;
        }

        /// <inheritdoc/>
        public Memory<byte> GetMemory(int sizeHint = MinimumBufferSize)
        {
            // When called through an IBufferWriter interface,
            // the sizeHint will be initialized to 0.
            // IBufferWriter expects a non-empty buffer to be returned
            // when a 0 sizeHint is provided.
            if (sizeHint == 0)
            {
                sizeHint = MinimumBufferSize;
            }

            CheckAndResizeBuffer(sizeHint);
            return this.rentedBuffer.AsMemory(index);
        }

        /// <inheritdoc/>
        public Span<byte> GetSpan(int sizeHint = MinimumBufferSize)
        {
            // When called through an IBufferWriter interface,
            // the sizeHint will be initialized to 0.
            // IBufferWriter expects a non-empty buffer to be returned
            // when a 0 sizeHint is provided.
            if (sizeHint == 0)
            {
                sizeHint = MinimumBufferSize;
            }

            CheckAndResizeBuffer(sizeHint);
            return this.rentedBuffer.AsSpan(index);
        }

        /// <summary>
        /// Clears the data written to the underlying buffer.
        /// </summary>
        public void Clear()
        {
            ClearInternal();
        }

        /// <summary>
        /// Returns the rented memory back to the pool.
        /// </summary>
        public void Dispose()
        {
            if (this.rentedBuffer == null)
            {
                return;
            }

            ClearInternal();
            byte[] toReturn = this.rentedBuffer;
            this.rentedBuffer = null;
            ArrayPool<byte>.Shared.Return(toReturn);
        }

        private void ClearInternal()
        {
            Debug.Assert(this.rentedBuffer != null);
            Debug.Assert(this.index <= this.rentedBuffer.Length);

            this.rentedBuffer.AsSpan(0, index).Clear();
            this.index = 0;
        }

        private void CheckAndResizeBuffer(int sizeHint)
        {
            Debug.Assert(rentedBuffer != null);
            Debug.Assert(sizeHint > 0);

            int currentLength = this.rentedBuffer.Length;
            int availableSpace = currentLength - this.index;

            // If we've reached ~1GB written, grow to the maximum buffer
            // length to avoid incessant minimal growths causing perf issues.
            if (this.index >= MaximumBufferSize / 2)
            {
                sizeHint = Math.Max(sizeHint, MaximumBufferSize - currentLength);
            }

            if (sizeHint > availableSpace)
            {
                // If the current size buffer of buffer is relatively small,
                // we'll grow it by the size hint requested by the caller
                // otherwise we'll attempt to double the buffer size.
                // This avoids repeatedly growing the buffer by small
                // chunks.
                int growBy = Math.Max(sizeHint, currentLength);

                int newSize = currentLength + growBy;

                if ((uint)newSize > MaximumBufferSize)
                {
                    // If the new size exceeds by the maximum buffer size,
                    // then we fall back and attempt to grow it only by
                    // the size requested by the caller.
                    newSize = currentLength + sizeHint;

                    // If the size still exceeds the max size, then give up and throw an OOM.
                    if ((uint)newSize > MaximumBufferSize)
                    {
                        throw new OutOfMemoryException(Strings.ODataMessageWriter_Buffer_Maximum_Size_Exceeded(newSize));
                    }
                }

                byte[] oldBuffer = this.rentedBuffer;

                this.rentedBuffer = ArrayPool<byte>.Shared.Rent(newSize);

                Debug.Assert(oldBuffer.Length >= this.index);
                Debug.Assert(this.rentedBuffer.Length >= this.index);

                Span<byte> oldBufferAsSpan = oldBuffer.AsSpan(0, this.index);
                oldBufferAsSpan.CopyTo(this.rentedBuffer);
                oldBufferAsSpan.Clear();
                ArrayPool<byte>.Shared.Return(oldBuffer);
            }

            Debug.Assert(this.rentedBuffer.Length - this.index > 0);
            Debug.Assert(this.rentedBuffer.Length - this.index >= sizeHint);
        }
    }
}
