//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services
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
