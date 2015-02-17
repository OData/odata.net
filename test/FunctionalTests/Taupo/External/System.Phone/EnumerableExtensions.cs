//---------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling;

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) {
            ExceptionUtilities.CheckArgumentNotNull(first, "first");
            ExceptionUtilities.CheckArgumentNotNull(second, "second");
            ExceptionUtilities.CheckArgumentNotNull(resultSelector, "resultSelector");
            return ZipIterator(first, second, resultSelector);
        }

        private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) {
            using (IEnumerator<TFirst> e1 = first.GetEnumerator())
                using (IEnumerator<TSecond> e2 = second.GetEnumerator())
                    while (e1.MoveNext() && e2.MoveNext())
                        yield return resultSelector(e1.Current, e2.Current);
        }
    }
}
