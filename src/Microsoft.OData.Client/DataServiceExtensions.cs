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
    using System.Runtime.CompilerServices;

    #endregion Namespaces

    public static class DataServiceExtensions
    {
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
            MethodCallExpression countMethodExpr;

            if (source.Provider.GetType().Equals(typeof(DataServiceQueryProvider)))
            {
                var currentMethod = (MethodInfo)MethodBase.GetCurrentMethod();
                MethodInfo methodInfo = currentMethod.MakeGenericMethod(typeof(TSource), typeof(TTarget));

                countMethodExpr = Expression.Call(null,
                    methodInfo,
                    new Expression[] { source.Expression, selector });
            }
            else
            {
                // Provide a default implementation...
                // Catch scenarios like: new List<int> { 1, 2, 1 }.AsQueryable().CountDistinct(d => d)

                // Extract method: Select<TSource,TResult>(IQueryable<TSource>, Expression<Func<TSource,TResult>>)
                MethodInfo selectMethod = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Where(d1 => d1.Name.Equals("Select", StringComparison.Ordinal))
                    .Select(d2 => new { Method = d2, Parameters = d2.GetParameters() })
                    .Where(d3 => d3.Parameters.Length.Equals(2)
                        && d3.Parameters[0].ParameterType.IsGenericType
                        && d3.Parameters[0].ParameterType.GetGenericTypeDefinition().Equals(typeof(IQueryable<>))
                        && d3.Parameters[1].ParameterType.IsGenericType
                        && d3.Parameters[1].ParameterType.GetGenericTypeDefinition().Equals(typeof(Expression<>)))
                    .Select(d4 => new { d4.Method, SelectorArguments = d4.Parameters[1].ParameterType.GetGenericArguments() })
                    .Where(d5 => d5.SelectorArguments.Length.Equals(1)
                        && d5.SelectorArguments[0].IsGenericType
                        && d5.SelectorArguments[0].GetGenericTypeDefinition().Equals(typeof(Func<,>)))
                    .Select(d6 => d6.Method).Single();

                // Extract method: Distinct<TSource>(IQueryable<TSource>)
                MethodInfo distinctMethod = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Where(d1 => d1.Name.Equals("Distinct", StringComparison.Ordinal))
                    .Select(d2 => new { Method = d2, Parameters = d2.GetParameters() })
                    .Where(d3 => d3.Parameters.Length.Equals(1)
                        && d3.Parameters[0].ParameterType.IsGenericType
                        && d3.Parameters[0].ParameterType.GetGenericTypeDefinition().Equals(typeof(IQueryable<>)))
                    .Select(d6 => d6.Method).Single();

                // Extract method: Count<TSource>(IQueryable<TSource>)
                MethodInfo countMethod = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Where(d1 => d1.Name.Equals("Count", StringComparison.Ordinal))
                    .Select(d2 => new { Method = d2, Parameters = d2.GetParameters() })
                    .Where(d3 => d3.Parameters.Length.Equals(1)
                        && d3.Parameters[0].ParameterType.IsGenericType
                        && d3.Parameters[0].ParameterType.GetGenericTypeDefinition().Equals(typeof(IQueryable<>)))
                    .Select(d6 => d6.Method).Single();

                // Select(d => d.Prop)
                MethodCallExpression selectMethodExpr = Expression.Call(null,
                    selectMethod.MakeGenericMethod(new Type[] { source.ElementType, selector.Body.Type }),
                    new[] { source.Expression, Expression.Quote(selector) });

                // Select(d => d.Prop).Distinct()
                MethodCallExpression distinctMethodExpr = Expression.Call(null,
                    distinctMethod.MakeGenericMethod(new Type[] { selector.Body.Type }),
                    new[] { selectMethodExpr });

                // Select(d => d.Prop).Distinct().Count()
                countMethodExpr = Expression.Call(null,
                    countMethod.MakeGenericMethod(new Type[] { selector.Body.Type }),
                    new[] { distinctMethodExpr });
            }

            return source.Provider.Execute<int>(countMethodExpr);
        }

        /// <summary>
        /// Returns the distinct count of elements in a sequence after applying the projection function to each element.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/></typeparam>
        /// <typeparam name="TTarget">The type returned by the projection function represented in <paramref name="selector"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>Distinct count of elements in a sequence after applying the projection function to each element.</returns>
        public static int CountDistinct<TSource, TTarget>(this IEnumerable<TSource> source, Func<TSource, TTarget> selector)
        {
            // Provide a default implementation...
            // Catch scenarios like: new List<int> { 1, 2, 1 }.CountDistinct(d => d)

            // Extract method: Select<TSource,TResult>(IEnumerable<TSource>, Func<TSource,TResult>)
            MethodInfo selectMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(d1 => d1.Name.Equals("Select", StringComparison.Ordinal))
                .Select(d2 => new { Method = d2, Parameters = d2.GetParameters() })
                .Where(d3 => d3.Parameters.Length.Equals(2)
                    && d3.Parameters[0].ParameterType.IsGenericType
                    && d3.Parameters[0].ParameterType.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>))
                    && d3.Parameters[1].ParameterType.IsGenericType
                    && d3.Parameters[1].ParameterType.GetGenericTypeDefinition().Equals(typeof(Func<,>)))
                .Select(d6 => d6.Method).Single();

            IEnumerable<TTarget> transientResult;

            transientResult = (IEnumerable<TTarget>)selectMethod.MakeGenericMethod(
                typeof(TSource), typeof(TTarget)).Invoke(null, new object[] { source, selector });

            // Extract method: Distinct<TSource>(IEnumerable<TSource>)
            MethodInfo distinctMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(d1 => d1.Name.Equals("Distinct", StringComparison.Ordinal))
                .Select(d2 => new { Method = d2, Parameters = d2.GetParameters() })
                .Where(d3 => d3.Parameters.Length.Equals(1)
                    && d3.Parameters[0].ParameterType.IsGenericType
                    && d3.Parameters[0].ParameterType.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                .Select(d6 => d6.Method).Single();

            transientResult = (IEnumerable<TTarget>)distinctMethod.MakeGenericMethod(
                typeof(TTarget)).Invoke(null, new object[] { transientResult });

            MethodInfo countMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(d1 => d1.Name.Equals("Count", StringComparison.Ordinal))
                .Select(d2 => new { Method = d2, Parameters = d2.GetParameters() })
                .Where(d3 => d3.Parameters.Length.Equals(1)
                    && d3.Parameters[0].ParameterType.IsGenericType
                    && d3.Parameters[0].ParameterType.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                .Select(d6 => d6.Method).Single();

            return (int)countMethod.MakeGenericMethod(
                typeof(TTarget)).Invoke(null, new object[] { transientResult });
        }
    }
}
