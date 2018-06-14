//---------------------------------------------------------------------
// <copyright file="BufferUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Buffers
{
    using System;

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
            if (buffer == null)
            {
                return new char[BufferUtils.BufferLength];
            }

            return buffer;
        }
    }
}