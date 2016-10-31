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
    using System.Linq;

    /// <summary>
    /// LINQ extension methods that are not provided by .NET itself.
    /// </summary>
    internal static class LinqExtensions
    {
        /// <summary>
        /// Creates a new hash-set from the given enumerable.
        /// </summary>
        /// <typeparam name="TElement">The type of the elements.</typeparam>
        /// <param name="elements">The elements to put in the hash-set.</param>
        /// <param name="comparer">The comparer for the hash-set to use.</param>
        /// <returns>A new hash-set with the given elements and comparer.</returns>
        internal static HashSet<TElement> ToHashSet<TElement>(this IEnumerable<TElement> elements, IEqualityComparer<TElement> comparer)
        {
            return new HashSet<TElement>(elements, comparer);
        }

        /// <summary>
        /// Filters the given elements to only return the elements that appear more than once. Will return them in the order of their second appearance. 
        /// If items appear 3 or more times in the original set, they will appear multiple times in the result.
        /// </summary>
        /// <typeparam name="TElement">The type of the elemenens.</typeparam>
        /// <param name="elements">The elements filter for duplicates.</param>
        /// <param name="comparer">The comparer to use for determining whether an element is a duplicate of an earlier item.</param>
        /// <returns>The set of elements that appear in the original set more than once. May contain duplicates itself.</returns>
        internal static IEnumerable<TElement> Duplicates<TElement>(this IEnumerable<TElement> elements, IEqualityComparer<TElement> comparer)
        {
            HashSet<TElement> elementsSeen = new HashSet<TElement>(comparer);
            return elements.Where(element => !elementsSeen.Add(element));
        }
    }
}
