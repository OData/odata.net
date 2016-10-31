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

namespace System.Data.Services.Parsing
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>This class implements IEqualityComparer for UnicodeCategory</summary>
    /// <remarks>
    /// Using this class rather than EqualityComparer&lt;T&gt;.Default 
    /// saves from JIT'ing it in each AppDomain.
    /// </remarks>
    internal class UnicodeCategoryEqualityComparer : IEqualityComparer<UnicodeCategory>
    {
        /// <summary>
        /// Checks whether two unicode categories are equal
        /// </summary>
        /// <param name="x">first unicode category</param>
        /// <param name="y">second unicode category</param>
        /// <returns>true if they are equal, false otherwise</returns>
        public bool Equals(UnicodeCategory x, UnicodeCategory y)
        {
            return x == y;
        }

        /// <summary>
        /// Gets a hash code for the specified unicode category
        /// </summary>
        /// <param name="obj">the input value</param>
        /// <returns>The hash code for the given input unicode category, the underlying int</returns>
        public int GetHashCode(UnicodeCategory obj)
        {
            return (int)obj;
        }
    }
}
