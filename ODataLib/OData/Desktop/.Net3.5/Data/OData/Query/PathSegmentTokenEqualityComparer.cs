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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using System.Collections.Generic;

    /// <summary>
    /// Equality comparer for <see cref="PathSegmentToken"/>.
    /// </summary>
    internal sealed class PathSegmentTokenEqualityComparer : EqualityComparer<PathSegmentToken>
    {
        /// <summary>
        /// Determines whether the two paths are equivalent.
        /// </summary>
        /// <param name="first">The first path to compare.</param>
        /// <param name="second">The second path to compare.</param>
        /// <returns>Whether the two paths are equivalent.</returns>
        public override bool Equals(PathSegmentToken first, PathSegmentToken second)
        {
            if (first == null && second == null)
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            return this.ToHashableString(first) == this.ToHashableString(second);
        }

        /// <summary>
        /// Returns a hash code for the given path.
        /// </summary>
        /// <param name="path">The path to hash.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode(PathSegmentToken path)
        {
            if (path == null)
            {
                return 0;
            }

            return this.ToHashableString(path).GetHashCode();
        }

        /// <summary>
        /// Converts the token to a string that is sufficiently unique to be hashed or compared.
        /// </summary>
        /// <param name="token">The path token to convert to a string.</param>
        /// <returns>A string representing the path.</returns>
        private string ToHashableString(PathSegmentToken token)
        {
            if (token.NextToken == null)
            {
                return token.Identifier;
            }

            return token.Identifier + "/" + this.ToHashableString(token.NextToken);
        }
    }
}
