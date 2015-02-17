//---------------------------------------------------------------------
// <copyright file="Int32EqualityComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System.Collections.Generic;

    /// <summary>This class implements IEqualityComparer for System.In32.</summary>
    /// <remarks>
    /// Using this class rather than EqualityComparer&lt;T&gt;.Default 
    /// saves from JIT'ing it in each AppDomain.
    /// </remarks>
    internal class Int32EqualityComparer : IEqualityComparer<int>
    {
        /// <summary>Checks whether two numbers are equal.</summary>
        /// <param name='x'>First number.</param><param name='y'>Second number.</param>
        /// <returns>true if x equals y; false otherwise.</returns>
        public bool Equals(int x, int y)
        {
            return x == y;
        }

        /// <summary>Gets a hash code for the specified number.</summary>
        /// <param name='obj'>Value.</param>
        /// <returns>The hash code for the specified value.</returns>
        public int GetHashCode(int obj)
        {
            return obj;
        }
    }
}
