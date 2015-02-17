//---------------------------------------------------------------------
// <copyright file="IEnumerableExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Platforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Extenstion methods to fake platform support for missing IEnumerable(Of T) APIs
    /// </summary>
    public static class IEnumerableExtensionMethods
    {
#if WINDOWS_PHONE_MANGO
        /// <summary>
        /// Merges two sequences by using the specified predicate function.
        /// </summary>
        /// <typeparam name="TFirst">The type of the elements of the first input sequence</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the result sequence.</typeparam>
        /// <param name="first">The first sequence to merge.</param>
        /// <param name="second">The second sequence to merge.</param>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
        /// <returns>An IEnumerable(OF T) that contains merged elements of two input sequences.</returns>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(first, "first");
            ExceptionUtilities.CheckArgumentNotNull(second, "second");
            ExceptionUtilities.Assert(first.Count() == second.Count(), "Both the enumerables should have the same length");

            int length = first.Count();
            List<TResult> results = new List<TResult>();
            for (int index = 0; index < length; index++)
            {
                results.Add(resultSelector(first.ElementAt(index), second.ElementAt(index)));
            }

            return results;
        }
#endif
    }
}