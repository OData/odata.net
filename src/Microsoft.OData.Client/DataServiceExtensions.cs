//---------------------------------------------------------------------
// <copyright file="DataServiceExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    #endregion Namespaces

    public static class DataServiceExtensions
    {
        /// <summary>
        /// The single MethodInfo instance of Enumerable.Distinct
        /// </summary>
        private readonly static SimpleLazy<MethodInfo> EnumerableSelectMethod = new SimpleLazy<MethodInfo>(() =>
        {
            return GetEnumerableSelectMethod();
        }, /*isThreadSafe*/ true);

        /// <summary>
        /// The single MethodInfo instance of Enumerable.Distinct
        /// </summary>
        private readonly static SimpleLazy<MethodInfo> EnumerableDistinctMethod = new SimpleLazy<MethodInfo>(() =>
        {
            return GetDistinctMethod(typeof(Enumerable), typeof(IEnumerable<>));
        }, /*isThreadSafe*/ true);

        /// <summary>
        /// The single MethodInfo instance of Enumerable.Count
        /// </summary>
        private readonly static SimpleLazy<MethodInfo> EnumerableCountMethod = new SimpleLazy<MethodInfo>(() =>
        {
            return GetCountMethod(typeof(Enumerable), typeof(IEnumerable<>));
        }, /*isThreadSafe*/ true);

        /// <summary>
        /// Returns the distinct count of elements in a sequence after applying the projection function to each element.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/></typeparam>
        /// <typeparam name="TTarget">The type returned by the projection function represented in <paramref name="selector"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>Distinct count of elements in a sequence after applying the projection function to each element.</returns>
        public static int CountDistinct<TSource, TTarget>(this IQueryable<TSource> source, Expression<Func<TSource, TTarget>> selector)
        {
            Util.CheckArgumentNull(source, "source");
            Util.CheckArgumentNull(selector, "selector");

            MethodInfo currentMethod = (MethodInfo)MethodBase.GetCurrentMethod();
            MethodInfo methodInfo = currentMethod.MakeGenericMethod(typeof(TSource), typeof(TTarget));

            return source.Provider.Execute<int>(
                Expression.Call(null,
                    methodInfo,
                    new Expression[] { source.Expression, selector }));
        }

        /// <summary>
        /// Returns the distinct count of elements in a sequence after applying the projection function to each element.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/></typeparam>
        /// <typeparam name="TTarget">The type returned by the projection function represented in <paramref name="selector"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>Distinct count of elements in a sequence after applying the projection function to each element.</returns>
        public static int CountDistinct<TSource, TTarget>(this IEnumerable<TSource> source, Func<TSource, TTarget> selector)
        {
            Util.CheckArgumentNull(source, "source");
            Util.CheckArgumentNull(selector, "selector");

            // Provide a default implementation...
            // In the event that a developer who adds a reference to the library invokes CountDistinct() as follows:
            // - new List<int> { 1, 2, 1 }.CountDistinct(d => d)

            // Method: Select<TSource,TResult>(IEnumerable<TSource>, Func<TSource,TResult>)
            MethodInfo selectMethod = EnumerableSelectMethod.Value;

            IEnumerable<TTarget> transientResult;

            transientResult = (IEnumerable<TTarget>)selectMethod.MakeGenericMethod(
                typeof(TSource), typeof(TTarget)).Invoke(null, new object[] { source, selector });

            // Method: Distinct<TSource>(IEnumerable<TSource>)
            MethodInfo distinctMethod = EnumerableDistinctMethod.Value;

            transientResult = (IEnumerable<TTarget>)distinctMethod.MakeGenericMethod(
                typeof(TTarget)).Invoke(null, new object[] { transientResult });

            // Method: Count<TSource>(IEnumerable<TSource>)
            MethodInfo countMethod = EnumerableCountMethod.Value;

            return (int)countMethod.MakeGenericMethod(
                typeof(TTarget)).Invoke(null, new object[] { transientResult });
        }

        /// <summary>
        /// Returns Select method defined in Enumerable type.
        /// </summary>
        private static MethodInfo GetEnumerableSelectMethod()
        {
            return typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(d1 => d1.Name.Equals("Select", StringComparison.Ordinal))
                .Select(d2 => new { Method = d2, Parameters = d2.GetParameters() })
                .Where(d3 => d3.Parameters.Length.Equals(2)
                    && d3.Parameters[0].ParameterType.IsGenericType
                    && d3.Parameters[0].ParameterType.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>))
                    && d3.Parameters[1].ParameterType.IsGenericType
                    && d3.Parameters[1].ParameterType.GetGenericTypeDefinition().Equals(typeof(Func<,>)))
                .Select(d6 => d6.Method).Single();
        }

        /// <summary>
        /// Returns Distinct method defined in <paramref name="declaringType"/>.
        /// </summary>
        private static MethodInfo GetDistinctMethod(Type declaringType, Type sourceType)
        {
            return declaringType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(d1 => d1.Name.Equals("Distinct", StringComparison.Ordinal))
                .Select(d2 => new { Method = d2, Parameters = d2.GetParameters() })
                .Where(d3 => d3.Parameters.Length.Equals(1)
                    && d3.Parameters[0].ParameterType.IsGenericType
                    && d3.Parameters[0].ParameterType.GetGenericTypeDefinition().Equals(sourceType))
                .Select(d6 => d6.Method).Single();
        }

        /// <summary>
        /// Returns Count method defined in <paramref name="declaringType"/>.
        /// </summary>
        private static MethodInfo GetCountMethod(Type declaringType, Type sourceType)
        {
            return declaringType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(d1 => d1.Name.Equals("Count", StringComparison.Ordinal))
                .Select(d2 => new { Method = d2, Parameters = d2.GetParameters() })
                .Where(d3 => d3.Parameters.Length.Equals(1)
                    && d3.Parameters[0].ParameterType.IsGenericType
                    && d3.Parameters[0].ParameterType.GetGenericTypeDefinition().Equals(sourceType))
                .Select(d6 => d6.Method).Single();
        }
    }
}
