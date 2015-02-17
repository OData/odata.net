//---------------------------------------------------------------------
// <copyright file="DSPLinqQueryProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>Implementation of <see cref="IQueryProvider"/> which allows running the DSP queries against a LINQ to Objects.</summary>
    internal class DSPLinqQueryProvider : IQueryProvider
    {
        /// <summary>The underlying query provider (the LINQ to Objects provider) determined from the source query.</summary>
        private IQueryProvider underlyingQueryProvider;

        /// <summary>Private constructor.</summary>
        /// <param name="underlyingQueryProvider">The underlying provider to run the translated query on.</param>
        private DSPLinqQueryProvider(IQueryProvider underlyingQueryProvider)
        {
            this.underlyingQueryProvider = underlyingQueryProvider;
        }

        /// <summary>Wraps a query in a new query which will translate the DSP query into a LINQ to Objects runnable query
        /// and run it on the provided <paramref name="underlyingQuery"/>.</summary>
        /// <param name="underlyingQuery">The underlying (LINQ to Objects) query to wrap.</param>
        /// <returns>A new query which can handle the DSP expressions and run them on top of the <pararef name="underlyingQuery"/>.</returns>
        public static IQueryable CreateQuery(IQueryable underlyingQuery)
        {
            DSPLinqQueryProvider provider = new DSPLinqQueryProvider(underlyingQuery.Provider);
            return CreateQuery(provider, underlyingQuery.Expression);
        }

        /// <summary>
        /// Invokes the generic version of CreateQuery on the <paramref name="provider"/>.
        /// </summary>
        /// <param name="provider"><see cref="IQueryProvider"/> instance.</param>
        /// <param name="expression"><see cref="Expression"/> instance.</param>
        /// <returns><see cref="IQueryable"/> instance.</returns>
        internal static IQueryable CreateQuery(IQueryProvider provider, Expression expression)
        {
            MethodInfo methodInfo = typeof(IQueryProvider)
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(mi => mi.Name.Split('.').Last() == "CreateQuery")
                    .FirstOrDefault(m => m.IsGenericMethod);

            Type queryType = TypeSystem.GetIEnumerableElementType(expression.Type);
            return (IQueryable)methodInfo.MakeGenericMethod(queryType).Invoke(provider, new object[] { expression });
        }

        /// <summary>Executes the specified DSP expression on the underlying LINQ to Objects provider.</summary>
        /// <typeparam name="TElement">The type of the result.</typeparam>
        /// <param name="expression">The expression (the DSP version) to run.</param>
        /// <returns>Enumerator with the results of the query.</returns>
        internal IEnumerator<TElement> ExecuteQuery<TElement>(Expression expression)
        {
            expression = this.ProcessExpression(expression);
            return this.underlyingQueryProvider.CreateQuery<TElement>(expression).GetEnumerator();
        }

        #region IQueryProvider Members

        /// <summary>Creates a query for the specified <paramref name="expression"/>.</summary>
        /// <typeparam name="TElement">The type of the result of the query.</typeparam>
        /// <param name="expression">The expression to create a query for.</param>
        /// <returns>The new query using the specified <paramref name="expression"/>.</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DSPLinqQuery<TElement>(this, expression);
        }

        /// <summary>Creates a query for the specified <paramref name="expression"/>.</summary>
        /// <param name="expression">The expression to create a query for.</param>
        /// <returns>The new query using the specified <paramref name="expression"/>.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            // WCF DS V2 Server always calls the generic version of IQueryProvider.CreateQuery.
            // To ensure we have the same behavior in future versions, if someone calls the non-generic version of IQueryProvider.CreateQuery, throw.
            throw new Exception("To avoid breaking changes, WCF DS Server should never invoke the non-generic version of IQueryProvider.CreateQuery.");
        }

        /// <summary>Executes an expression which returns a single result.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The result of the expression.</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            expression = this.ProcessExpression(expression);
            return this.underlyingQueryProvider.Execute<TResult>(expression);
        }

        /// <summary>Executes an expression which returns a single result.</summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The result of the expression.</returns>
        public object Execute(Expression expression)
        {
            expression = this.ProcessExpression(expression);
            return this.underlyingQueryProvider.Execute(expression);
        }

        #endregion

        /// <summary>Method which converts expressions from the DSP "syntax" into the LINQ to Objects "syntax".</summary>
        /// <param name="expression">The expression to process.</param>
        /// <returns>A new expression which is the result of the conversion.</returns>
        private Expression ProcessExpression(Expression expression)
        {
            if (DSPServiceDefinition.Current != null && DSPServiceDefinition.Current.ExpressionTreeInterceptor != null)
            {
                expression = DSPServiceDefinition.Current.ExpressionTreeInterceptor(expression);
            }

            return DSPMethodTranslatingVisitor.TranslateExpression(expression);
        }
    }
}
