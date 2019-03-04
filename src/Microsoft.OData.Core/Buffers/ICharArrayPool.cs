//---------------------------------------------------------------------
// <copyright file="ICharArrayPool.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Buffers
{
    /// <summary>
    /// Provides an interface for using character arrays.
    /// </summary>
    public interface ICharArrayPool
    {
        /// <summary>
        /// Rents a character array from the pool.
        /// This character array must be returned when it is no longer used.
        /// </summary>
        /// <param name="minSize">The min required size of the character array.</param>
        /// <returns>The character array from the pool.</returns>
        char[] Rent(int minSize);

        /// <summary>
        /// Returns a character array to the pool.
        /// </summary>
        /// <param name="array">The character array should be returned.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Return")]
        void Return(char[] array);
    }
}