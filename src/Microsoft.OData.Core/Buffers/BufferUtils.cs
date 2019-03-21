//---------------------------------------------------------------------
// <copyright file="BufferUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Buffers
{
    /// <summary>
    /// Helpers to deal with buffers
    /// </summary>
    internal static class BufferUtils
    {
        /// <summary>
        /// Buffer length
        /// </summary>
        private const int BufferLength = 128;

        /// <summary>
        /// Checks if the buffer is not initialized and if initialized returns the same buffer or creates a new one.
        /// </summary>
        /// <param name="buffer">The buffer to verify</param>
        /// <returns>The initialized buffer</returns>
        public static char[] InitializeBufferIfRequired(char[] buffer)
        {
            return InitializeBufferIfRequired(null, buffer);
        }

        /// <summary>
        /// Checks if the buffer is not initialized and if initialized returns the same buffer or creates a new one.
        /// </summary>
        /// <param name="bufferPool">The character pool.</param>
        /// <param name="buffer">The buffer to verify</param>
        /// <returns>The initialized buffer.</returns>
        public static char[] InitializeBufferIfRequired(ICharArrayPool bufferPool, char[] buffer)
        {
            if (buffer != null)
            {
                return buffer;
            }

            return RentFromBuffer(bufferPool, BufferLength);
        }

        /// <summary>
        /// Rents a character array from the pool.
        /// </summary>
        /// <param name="bufferPool">The character pool.</param>
        /// <param name="minSize">The min required size of the character array.</param>
        /// <returns>The character array from the pool.</returns>
        public static char[] RentFromBuffer(ICharArrayPool bufferPool, int minSize)
        {
            if (bufferPool == null)
            {
                return new char[minSize];
            }

            char[] buffer = bufferPool.Rent(minSize);
            if (buffer == null || buffer.Length < minSize)
            {
                throw new ODataException(Strings.BufferUtils_InvalidBufferOrSize(minSize));
            }

            return buffer;
        }

        /// <summary>
        /// Returns a character array to the pool.
        /// </summary>
        /// <param name="bufferPool">The character pool.</param>
        /// <param name="buffer">The character array should be returned.</param>
        public static void ReturnToBuffer(ICharArrayPool bufferPool, char[] buffer)
        {
            if (bufferPool != null)
            {
                bufferPool.Return(buffer);
            }
        }
    }
}
