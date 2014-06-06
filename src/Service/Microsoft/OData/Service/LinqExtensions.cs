//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
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
