//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
