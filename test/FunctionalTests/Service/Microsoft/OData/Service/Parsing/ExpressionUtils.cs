//---------------------------------------------------------------------
// <copyright file="ExpressionUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Parsing
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Service.Providers;
    #endregion Namespaces

    /// <summary>Utility methods to work with the Expression type.</summary>
    internal static class ExpressionUtils
    {
        /// <summary>Constant for "null" literal.</summary>
        internal static readonly ConstantExpression NullLiteral = Expression.Constant(null);

        #region Private Fields

        /// <summary>Constant for "false" literal.</summary>
        private static readonly ConstantExpression falseLiteral = Expression.Constant(false);

        /// <summary>
        /// MethodInfo for Queryable.Where
        /// </summary>
        private static MethodInfo queryableWhereMethodInfo;

        /// <summary>
        /// MethodInfo for Queryable.OfType
        /// </summary>
        private static MethodInfo queryableOfTypeMethodInfo;

        /// <summary>
        /// MethodInfo for Queryable.Select
        /// </summary>
        private static MethodInfo queryableSelectMethodInfo;

        /// <summary>
        /// MethodInfo for Queryable.SelectMany
        /// </summary>
        private static MethodInfo queryableSelectManyMethodInfo;

        /// <summary>
        /// MethodInfo for Queryable.OrderBy
        /// </summary>
        private static MethodInfo queryableOrderByMethodInfo;

        /// <summary>
        /// MethodInfo for Queryable.OrderByDescending
        /// </summary>
        private static MethodInfo queryableOrderByDescendingMethodInfo;

        /// <summary>
        /// MethodInfo for Queryable.ThenBy
        /// </summary>
        private static MethodInfo queryableThenByMethodInfo;

        /// <summary>
        /// MethodInfo for Queryable.ThenByDescending
        /// </summary>
        private static MethodInfo queryableThenByDescendingMethodInfo;

        /// <summary>
        /// MethodInfo for Queryable.Take
        /// </summary>
        private static MethodInfo queryableTakeMethodInfo;

        /// <summary>
        /// MethodInfo for Queryable.Skip
        /// </summary>
        private static MethodInfo queryableSkipMethodInfo;

        /// <summary>
        /// MethodInfo for Queryable.LongCount
        /// </summary>
        private static MethodInfo queryableLongCountMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.Where
        /// </summary>
        private static MethodInfo enumerableWhereMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.OfType
        /// </summary>
        private static MethodInfo enumerableOfTypeMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.Select
        /// </summary>
        private static MethodInfo enumerableSelectMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.SelectMany
        /// </summary>
        private static MethodInfo enumerableSelectManyMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.OrderBy
        /// </summary>
        private static MethodInfo enumerableOrderByMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.OrderByDescending
        /// </summary>
        private static MethodInfo enumerableOrderByDescendingMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.ThenBy
        /// </summary>
        private static MethodInfo enumerableThenByMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.ThenByDescending
        /// </summary>
        private static MethodInfo enumerableThenByDescendingMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.Take
        /// </summary>
        private static MethodInfo enumerableTakeMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.Skip
        /// </summary>
        private static MethodInfo enumerableSkipMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.Cast
        /// </summary>
        private static MethodInfo enumerableCastMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.All
        /// </summary>
        private static MethodInfo enumerableAllMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.Any()
        /// </summary>
        private static MethodInfo enumerableAnyWithNoPredicateMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.Any(predicate)
        /// </summary>
        private static MethodInfo enumerableAnyWithPredicateMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.Empty
        /// </summary>
        private static MethodInfo enumerableEmptyMethodInfo;

        /// <summary>
        /// MethodInfo for IQueryProvider.CreateQuery
        /// </summary>
        private static MethodInfo createQueryMethodInfo;

        #endregion Private Fields

        #region Private Properties

        /// <summary>
        /// MethodInfo for Queryable.Where
        /// </summary>
        private static MethodInfo QueryableWhereMethodInfo
        {
            get { return queryableWhereMethodInfo ?? (queryableWhereMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsQueryable().Where(o => true))); }
        }

        /// <summary>
        /// MethodInfo for Queryable.OfType
        /// </summary>
        private static MethodInfo QueryableOfTypeMethodInfo
        {
            get { return queryableOfTypeMethodInfo ?? (queryableOfTypeMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsQueryable().OfType<object>())); }
        }

        /// <summary>
        /// MethodInfo for Queryable.Select
        /// </summary>
        private static MethodInfo QueryableSelectMethodInfo
        {
            get { return queryableSelectMethodInfo ?? (queryableSelectMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsQueryable().Select(o => o.ToString()))); }
        }

        /// <summary>
        /// MethodInfo for Queryable.SelectMany
        /// </summary>
        private static MethodInfo QueryableSelectManyMethodInfo
        {
            get { return queryableSelectManyMethodInfo ?? (queryableSelectManyMethodInfo = GetMethodInfoFromLambdaBody(() => (new Type[0]).AsQueryable().SelectMany(t => t.GetMembers()))); }
        }

        /// <summary>
        /// MethodInfo for Queryable.OrderBy
        /// </summary>
        private static MethodInfo QueryableOrderByMethodInfo
        {
            get { return queryableOrderByMethodInfo ?? (queryableOrderByMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsQueryable().OrderBy(o => o.GetHashCode()))); }
        }

        /// <summary>
        /// MethodInfo for Queryable.OrderByDescending
        /// </summary>
        private static MethodInfo QueryableOrderByDescendingMethodInfo
        {
            get { return queryableOrderByDescendingMethodInfo ?? (queryableOrderByDescendingMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsQueryable().OrderByDescending(o => o.GetHashCode()))); }
        }

        /// <summary>
        /// MethodInfo for Queryable.ThenBy
        /// </summary>
        private static MethodInfo QueryableThenByMethodInfo
        {
            get { return queryableThenByMethodInfo ?? (queryableThenByMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsQueryable().OrderBy(o => o.GetHashCode()).ThenBy(o => o.ToString()))); }
        }

        /// <summary>
        /// MethodInfo for Queryable.ThenByDescending
        /// </summary>
        private static MethodInfo QueryableThenByDescendingMethodInfo
        {
            get { return queryableThenByDescendingMethodInfo ?? (queryableThenByDescendingMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsQueryable().OrderBy(o => o.GetHashCode()).ThenByDescending(o => o.ToString()))); }
        }

        /// <summary>
        /// MethodInfo for Queryable.Take
        /// </summary>
        private static MethodInfo QueryableTakeMethodInfo
        {
            get { return queryableTakeMethodInfo ?? (queryableTakeMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsQueryable().Take(1))); }
        }

        /// <summary>
        /// MethodInfo for Queryable.Skip
        /// </summary>
        private static MethodInfo QueryableSkipMethodInfo
        {
            get { return queryableSkipMethodInfo ?? (queryableSkipMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsQueryable().Skip(1))); }
        }

        /// <summary>
        /// MethodInfo for Queryable.LongCount
        /// </summary>
        private static MethodInfo QueryableLongCountMethodInfo
        {
            get { return queryableLongCountMethodInfo ?? (queryableLongCountMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsQueryable().LongCount())); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.Where
        /// </summary>
        private static MethodInfo EnumerableWhereMethodInfo
        {
            get { return enumerableWhereMethodInfo ?? (enumerableWhereMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().Where(o => true))); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.OfType
        /// </summary>
        private static MethodInfo EnumerableOfTypeMethodInfo
        {
            get { return enumerableOfTypeMethodInfo ?? (enumerableOfTypeMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().OfType<object>())); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.Select
        /// </summary>
        private static MethodInfo EnumerableSelectMethodInfo
        {
            get { return enumerableSelectMethodInfo ?? (enumerableSelectMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().Select(o => o.ToString()))); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.SelectMany
        /// </summary>
        private static MethodInfo EnumerableSelectManyMethodInfo
        {
            get { return enumerableSelectManyMethodInfo ?? (enumerableSelectManyMethodInfo = GetMethodInfoFromLambdaBody(() => (new Type[0]).AsEnumerable().SelectMany(t => t.GetMembers()))); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.OrderBy
        /// </summary>
        private static MethodInfo EnumerableOrderByMethodInfo
        {
            get { return enumerableOrderByMethodInfo ?? (enumerableOrderByMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().OrderBy(o => o.GetHashCode()))); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.OrderByDescending
        /// </summary>
        private static MethodInfo EnumerableOrderByDescendingMethodInfo
        {
            get { return enumerableOrderByDescendingMethodInfo ?? (enumerableOrderByDescendingMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().OrderByDescending(o => o.GetHashCode()))); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.ThenBy
        /// </summary>
        private static MethodInfo EnumerableThenByMethodInfo
        {
            get { return enumerableThenByMethodInfo ?? (enumerableThenByMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().OrderBy(o => o.GetHashCode()).ThenBy(o => o.ToString()))); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.ThenByDescending
        /// </summary>
        private static MethodInfo EnumerableThenByDescendingMethodInfo
        {
            get { return enumerableThenByDescendingMethodInfo ?? (enumerableThenByDescendingMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().OrderBy(o => o.GetHashCode()).ThenByDescending(o => o.ToString()))); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.Take
        /// </summary>
        private static MethodInfo EnumerableTakeMethodInfo
        {
            get { return enumerableTakeMethodInfo ?? (enumerableTakeMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().Take(1))); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.Skip
        /// </summary>
        private static MethodInfo EnumerableSkipMethodInfo
        {
            get { return enumerableSkipMethodInfo ?? (enumerableSkipMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().Skip(1))); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.Cast
        /// </summary>
        private static MethodInfo EnumerableCastMethodInfo
        {
            get { return enumerableCastMethodInfo ?? (enumerableCastMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().Cast<object>())); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.All
        /// </summary>
        private static MethodInfo EnumerableAllMethodInfo
        {
            get { return enumerableAllMethodInfo ?? (enumerableAllMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().All(o => true))); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.Any()
        /// </summary>
        private static MethodInfo EnumerableAnyWithNoPredicateMethodInfo
        {
            get { return enumerableAnyWithNoPredicateMethodInfo ?? (enumerableAnyWithNoPredicateMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().Any())); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.Any(predicate)
        /// </summary>
        private static MethodInfo EnumerableAnyWithPredicateMethodInfo
        {
            get { return enumerableAnyWithPredicateMethodInfo ?? (enumerableAnyWithPredicateMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().Any(o => true))); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.Empty
        /// </summary>
        private static MethodInfo EnumerableEmptyMethodInfo
        {
            get { return enumerableEmptyMethodInfo ?? (enumerableEmptyMethodInfo = GetMethodInfoFromLambdaBody(() => Enumerable.Empty<object>())); }
        }

        /// <summary>
        /// MethodInfo for IQueryProvider.CreateQuery
        /// </summary>
        private static MethodInfo CreateQueryMethodInfo
        {
            get { return createQueryMethodInfo ?? (createQueryMethodInfo = GetMethodInfoFromLambdaBody(() => (new object[0]).AsQueryable().Provider.CreateQuery<object>(Expression.Constant(0)))); }
        }

        #endregion Private Properties

        /// <summary>Checks whether <paramref name="expression"/> is a null constant.</summary>
        /// <param name="expression">Expression to check.</param>
        /// <returns>true if <paramref name="expression"/> is a null constant; false otherwise.</returns>
        internal static bool IsNullConstant(Expression expression)
        {
            Debug.Assert(expression != null, "expression != null");
            return
                expression == NullLiteral ||
                (expression.NodeType == ExpressionType.Constant && ((ConstantExpression)expression).Value == null);
        }

        /// <summary>Prepare the given expression for passing as a predicate to a filter function 
        ///  i.e. Queryable.Where()/Enumerable.Any()/Enumerable.All().
        /// </summary>
        /// <param name="expr">Input expression.</param>
        /// <returns>Expression converted to boolean expression.</returns>
        internal static Expression EnsurePredicateExpressionIsBoolean(Expression expr)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(expr))
            {
                expr = OpenTypeMethods.EqualExpression(expr, Expression.Constant(true, typeof(object)));
                expr = Expression.Convert(expr, typeof(bool));
            }
            else if (IsNullConstant(expr))
            {
                expr = falseLiteral;
            }
            else if (expr.Type == typeof(bool?))
            {
                Expression test = Expression.Equal(expr, Expression.Constant(null, typeof(bool?)));
                expr = Expression.Condition(test, falseLiteral, Expression.Property(expr, "Value"));
            }

            if (expr.Type != typeof(bool))
            {
                string message = Strings.RequestQueryParser_ExpressionTypeMismatch(WebUtil.GetTypeName(typeof(bool)));
                throw DataServiceException.CreateSyntaxError(message);
            }

            return expr;
        }

        /// <summary>
        /// Returns a method call expression to the Empty method
        /// </summary>
        /// <param name="targetType">Target type</param>
        /// <returns>Expression calling the Empty method</returns>
        internal static Expression EnumerableEmpty(Type targetType)
        {
            Debug.Assert(targetType != null, "targetType != null");

            MethodInfo emptyMethod = EnumerableEmptyMethodInfo.MakeGenericMethod(targetType);
            return Expression.Call(null, emptyMethod);
        }

        /// <summary>Rewrites an expression to propagate null values if necessary.</summary>
        /// <param name='element'>Expression to check for null.</param>
        /// <param name='notNullExpression'>Expression to yield if <paramref name='element' /> does not yield null.</param>
        /// <returns>The possibly rewriteen <paramref name='notNullExpression' />.</returns>
        internal static Expression AddNullPropagationIfNeeded(Expression element, Expression notNullExpression)
        {
            if (element is ParameterExpression || !WebUtil.TypeAllowsNull(element.Type))
            {
                return notNullExpression;
            }

            // Tiny optimization: remove the check on constants which are known not to be null.
            // Otherwise every string literal propagates out, which is correct but unnecessarily messy.
            if (element is ConstantExpression && element != NullLiteral)
            {
                return notNullExpression;
            }

            // ifTrue and ifFalse expressions must match exactly. We need to ensure that the 'false'
            // side is nullable, and the 'true' side is a null of the correct type.
            Expression test = Expression.Equal(element, Expression.Constant(null, element.Type));
            Expression ifFalse = notNullExpression;
            if (!WebUtil.TypeAllowsNull(ifFalse.Type))
            {
                ifFalse = Expression.Convert(ifFalse, typeof(Nullable<>).MakeGenericType(ifFalse.Type));
            }

            Expression ifTrue = Expression.Constant(null, ifFalse.Type);
            return Expression.Condition(test, ifTrue, ifFalse);
        }

        /// <summary>
        /// Composes a property navigation with the appropriate filter lamba, as appropriate.
        /// </summary>
        /// <param name="expression">Member access expression to compose.</param>
        /// <param name="filterLambda">Lambda expression used for the filter.</param>
        /// <param name="propagateNull">Whether null propagation is required on the <paramref name="expression"/>.</param>
        /// <param name="isSingleResult">Whether <paramref name="expression"/> represent a single resource navigation.</param>
        /// <returns>The composed expression.</returns>
        internal static Expression ComposePropertyNavigation(
            Expression expression,
            LambdaExpression filterLambda,
            bool propagateNull,
            bool isSingleResult)
        {
            Debug.Assert(expression != null, "expression != null");
            Debug.Assert(filterLambda != null, "filterLambda != null");

            Expression nullConstant = NullLiteral;
            if (isSingleResult)
            {
                Expression fixedFilter = ParameterReplacerVisitor.Replace(
                    filterLambda.Body,
                    filterLambda.Parameters[0],
                    expression);

                Expression test = propagateNull
                                      ? Expression.AndAlso(Expression.NotEqual(expression, nullConstant), fixedFilter)
                                      : fixedFilter;

                Expression conditionTrue = expression;
                Expression conditionFalse = Expression.Constant(null, conditionTrue.Type);
                return Expression.Condition(test, conditionTrue, conditionFalse);
            }

            Type elementType = filterLambda.Parameters[0].Type;
            Expression filterExpression = expression.EnumerableWhere(filterLambda);

            if (propagateNull)
            {
                Expression test = Expression.Equal(expression, nullConstant);
                Expression falseIf = filterExpression;
                Expression trueIf = EnumerableEmpty(elementType);
                return Expression.Condition(test, trueIf, falseIf, trueIf.Type);
            }

            return filterExpression;
        }

        #region Expression Extension Methods

        /// <summary>
        /// Returns the element type of the expression.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <returns>Returns the element type of the expression.</returns>
        internal static Type ElementType(this Expression source)
        {
            Debug.Assert(source != null, "source != null");
            return BaseServiceProvider.GetIEnumerableElement(source.Type) ?? source.Type;
        }

        #region Queryable Methods.

        /// <summary>
        /// Applies Queryable.Where() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="predicate">Predicate to pass to Queryable.Where().</param>
        /// <returns>New expression with Queryable.Where() applied.</returns>
        internal static Expression QueryableWhere(this Expression source, LambdaExpression predicate)
        {
            return Where(QueryableWhereMethodInfo, source, predicate);
        }

        /// <summary>
        /// Applies Queryable.Select() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="selector">Selector to pass to Queryable.Select().</param>
        /// <returns>New expression with Queryable.Select() applied.</returns>
        internal static Expression QueryableSelect(this Expression source, LambdaExpression selector)
        {
            return CallMethodWithSelector(QueryableSelectMethodInfo, source, selector);
        }

        /// <summary>
        /// Applies Queryable.SelectMany() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="selector">Selector to pass to Queryable.SelectMany().</param>
        /// <returns>New expression with Queryable.SelectMany() applied.</returns>
        internal static Expression QueryableSelectMany(this Expression source, LambdaExpression selector)
        {
            return SelectMany(QueryableSelectManyMethodInfo, source, selector);
        }

        /// <summary>
        /// Applies Queryable.OfType() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="targetType">Target type to pass to Queryable.OfType().</param>
        /// <returns>New expression with Queryable.OfType() applied.</returns>
        internal static Expression QueryableOfType(this Expression source, Type targetType)
        {
            return CallMethodWithTypeParam(QueryableOfTypeMethodInfo, source, targetType);
        }

        /// <summary>
        /// Applies Queryable.OrderBy() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="keySelector">KeySelector to pass to Queryable.OrderBy().</param>
        /// <returns>New expression with Queryable.OrderBy() applied.</returns>
        internal static Expression QueryableOrderBy(this Expression source, LambdaExpression keySelector)
        {
            return CallMethodWithSelector(QueryableOrderByMethodInfo, source, keySelector);
        }

        /// <summary>
        /// Applies Queryable.OrderByDescending() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="keySelector">KeySelector to pass to Queryable.OrderByDescending().</param>
        /// <returns>New expression with Queryable.OrderByDescending() applied.</returns>
        internal static Expression QueryableOrderByDescending(this Expression source, LambdaExpression keySelector)
        {
            return CallMethodWithSelector(QueryableOrderByDescendingMethodInfo, source, keySelector);
        }

        /// <summary>
        /// Applies Queryable.ThenBy() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="keySelector">KeySelector to pass to Queryable.ThenBy().</param>
        /// <returns>New expression with Queryable.ThenBy() applied.</returns>
        internal static Expression QueryableThenBy(this Expression source, LambdaExpression keySelector)
        {
            return CallMethodWithSelector(QueryableThenByMethodInfo, source, keySelector);
        }

        /// <summary>
        /// Applies Queryable.ThenByDescending() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="keySelector">KeySelector to pass to Queryable.ThenByDescending().</param>
        /// <returns>New expression with Queryable.ThenByDescending() applied.</returns>
        internal static Expression QueryableThenByDescending(this Expression source, LambdaExpression keySelector)
        {
            return CallMethodWithSelector(QueryableThenByDescendingMethodInfo, source, keySelector);
        }

        /// <summary>
        /// Applies Queryable.Take() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="count">Take count.</param>
        /// <returns>New expression with Queryable.Take() applied.</returns>
        internal static Expression QueryableTake(this Expression source, int count)
        {
            return CallMethodWithCount(QueryableTakeMethodInfo, source, count);
        }

        /// <summary>
        /// Applies Queryable.Skip() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="count">Skip count.</param>
        /// <returns>New expression with Queryable.Skip() applied.</returns>
        internal static Expression QueryableSkip(this Expression source, int count)
        {
            return CallMethodWithCount(QueryableSkipMethodInfo, source, count);
        }

        /// <summary>
        /// Applies Queryable.LongCount() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <returns>New expression with Queryable.LongCount() applied.</returns>
        internal static Expression QueryableLongCount(this Expression source)
        {
            return CallMethodWithNoParam(QueryableLongCountMethodInfo, source);
        }

        /// <summary>
        /// Invokes the generic version of CreateQuery on the <paramref name="provider"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="provider"><see cref="IQueryProvider"/> instance.</param>
        /// <returns><see cref="IQueryable"/> instance.</returns>
        internal static IQueryable CreateQuery(this Expression source, IQueryProvider provider)
        {
            Debug.Assert(source != null, "source != null");
            Debug.Assert(provider != null, "provider != null");

            return (IQueryable)CreateQueryMethodInfo.MakeGenericMethod(source.ElementType()).Invoke(provider, new object[] { source });
        }
        #endregion Queryable Methods.

        #region Enumerable Methods.

        /// <summary>
        /// Applies Enumerable.Where() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="predicate">Predicate to pass to Enumerable.Where().</param>
        /// <returns>New expression with Enumerable.Where() applied.</returns>
        internal static Expression EnumerableWhere(this Expression source, LambdaExpression predicate)
        {
            return Where(EnumerableWhereMethodInfo, source, predicate);
        }

        /// <summary>
        /// Applies Enumerable.Select() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="selector">Selector to pass to Enumerable.Select().</param>
        /// <returns>New expression with Enumerable.Select() applied.</returns>
        internal static Expression EnumerableSelect(this Expression source, LambdaExpression selector)
        {
            return CallMethodWithSelector(EnumerableSelectMethodInfo, source, selector);
        }

        /// <summary>
        /// Applies Enumerable.SelectMany() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="selector">Selector to pass to Enumerable.SelectMany().</param>
        /// <returns>New expression with Enumerable.SelectMany() applied.</returns>
        internal static Expression EnumerableSelectMany(this Expression source, LambdaExpression selector)
        {
            return SelectMany(EnumerableSelectManyMethodInfo, source, selector);
        }

        /// <summary>
        /// Applies Enumerable.OfType() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="targetType">Target type to pass to Enumerable.OfType().</param>
        /// <returns>New expression with Enumerable.OfType() applied.</returns>
        internal static Expression EnumerableOfType(this Expression source, Type targetType)
        {
            return CallMethodWithTypeParam(EnumerableOfTypeMethodInfo, source, targetType);
        }

        /// <summary>
        /// Applies Enumerable.OrderBy() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="keySelector">KeySelector to pass to Enumerable.OrderBy().</param>
        /// <returns>New expression with Enumerable.OrderBy() applied.</returns>
        internal static Expression EnumerableOrderBy(this Expression source, LambdaExpression keySelector)
        {
            return CallMethodWithSelector(EnumerableOrderByMethodInfo, source, keySelector);
        }

        /// <summary>
        /// Applies Enumerable.OrderByDescending() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="keySelector">KeySelector to pass to Enumerable.OrderByDescending().</param>
        /// <returns>New expression with Enumerable.OrderByDescending() applied.</returns>
        internal static Expression EnumerableOrderByDescending(this Expression source, LambdaExpression keySelector)
        {
            return CallMethodWithSelector(EnumerableOrderByDescendingMethodInfo, source, keySelector);
        }

        /// <summary>
        /// Applies Enumerable.ThenBy() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="keySelector">KeySelector to pass to Enumerable.ThenBy().</param>
        /// <returns>New expression with Enumerable.ThenBy() applied.</returns>
        internal static Expression EnumerableThenBy(this Expression source, LambdaExpression keySelector)
        {
            return CallMethodWithSelector(EnumerableThenByMethodInfo, source, keySelector);
        }

        /// <summary>
        /// Applies Enumerable.ThenByDescending() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="keySelector">KeySelector to pass to Enumerable.ThenByDescending().</param>
        /// <returns>New expression with Enumerable.ThenByDescending() applied.</returns>
        internal static Expression EnumerableThenByDescending(this Expression source, LambdaExpression keySelector)
        {
            return CallMethodWithSelector(EnumerableThenByDescendingMethodInfo, source, keySelector);
        }

        /// <summary>
        /// Applies Enumerable.Take() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="count">Take count.</param>
        /// <returns>New expression with Enumerable.Take() applied.</returns>
        internal static Expression EnumerableTake(this Expression source, int count)
        {
            return CallMethodWithCount(EnumerableTakeMethodInfo, source, count);
        }

        /// <summary>
        /// Applies Enumerable.Skip() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="count">Skip count.</param>
        /// <returns>New expression with Enumerable.Skip() applied.</returns>
        internal static Expression EnumerableSkip(this Expression source, int count)
        {
            return CallMethodWithCount(EnumerableSkipMethodInfo, source, count);
        }

        /// <summary>
        /// Applies Enumerable.Cast to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression</param>
        /// <param name="targetType">Target type</param>
        /// <returns>Expression with Enumerable.Cast() applied.</returns>
        internal static Expression EnumerableCast(this Expression source, Type targetType)
        {
            return CallMethodWithTypeParam(EnumerableCastMethodInfo, source, targetType);
        }

        /// <summary>
        /// Applies Enumerable.All to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression</param>
        /// <param name="predicate">Predicate to pass to Enumerable.All()</param>
        /// <returns>Expression with Enumerable.All() applied.</returns>
        internal static Expression EnumerableAll(this Expression source, LambdaExpression predicate)
        {
            return CallMethodWithPredicate(EnumerableAllMethodInfo, source, predicate);
        }

        /// <summary>
        /// Applies Enumerable.Any to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression</param>
        /// <returns>Expression with Enumerable.Any() applied.</returns>
        internal static Expression EnumerableAny(this Expression source)
        {
            return CallMethodWithNoParam(EnumerableAnyWithNoPredicateMethodInfo, source);
        }

        /// <summary>
        /// Applies Enumerable.Any() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression</param>
        /// <param name="predicate">Predicate to pass to Enumerable.Any()</param>
        /// <returns>Expression with Enumerable.Any() applied.</returns>
        internal static Expression EnumerableAny(this Expression source, LambdaExpression predicate)
        {
            return CallMethodWithPredicate(EnumerableAnyWithPredicateMethodInfo, source, predicate);
        }

        #endregion Enumerable Methods.

        #region Private Extension Implementations

        /// <summary>
        /// Compose Where() to expression
        /// </summary>
        /// <param name="genericMethodInfo">Where MethodInfo</param>
        /// <param name="source">Source expression</param>
        /// <param name="predicate">Predicate expression</param>
        /// <returns>Expression with Where()</returns>
        private static Expression Where(MethodInfo genericMethodInfo, Expression source, LambdaExpression predicate)
        {
            Debug.Assert(genericMethodInfo != null && genericMethodInfo.IsGenericMethod, "genericMethodInfo != null && genericMethodInfo.IsGenericMethod");
            Debug.Assert(source != null, "source != null");
            Debug.Assert(predicate != null, "predicate != null");

            Type elementType = source.ElementType();
            Debug.Assert(elementType != null, "elementType != null");

            MethodInfo whereMethod = genericMethodInfo.MakeGenericMethod(elementType);

            // If the type of predicate is not sourceType, we need to replace the parameter with
            // a downcasted parameter type if predicate's input is a base class of sourceType.
            if (predicate.Parameters[0].Type != elementType &&
                predicate.Parameters[0].Type.IsAssignableFrom(elementType))
            {
                predicate = ReplaceParameterTypeForLambda(predicate, elementType);
            }

            // Note the ParameterType on an IQueryable mehtod is Expression<Func<>> where as the ParameterType
            // on an IEnumerable method is Func<>.
            Debug.Assert(
                whereMethod.GetParameters()[1].ParameterType == predicate.GetType() ||
                whereMethod.GetParameters()[1].ParameterType == predicate.Type,
                "predicate should be of type Expression<Func<TSource, bool>>");
            return Expression.Call(null, whereMethod, source, predicate);
        }

        /// <summary>
        /// Compose SelectMany() to expression
        /// </summary>
        /// <param name="genericMethodInfo">SelectMany MethodInfo</param>
        /// <param name="source">Source expression</param>
        /// <param name="selector">Selector expression</param>
        /// <returns>Expression with SelectMany()</returns>
        private static Expression SelectMany(MethodInfo genericMethodInfo, Expression source, LambdaExpression selector)
        {
            Debug.Assert(genericMethodInfo != null && genericMethodInfo.IsGenericMethod, "genericMethodInfo != null && genericMethodInfo.IsGenericMethod");
            Debug.Assert(source != null, "source != null");
            Debug.Assert(selector != null, "selector != null");

            Type elementType = source.ElementType();
            Debug.Assert(elementType != null, "elementType != null");

            Type resultElementType = BaseServiceProvider.GetIEnumerableElement(selector.Body.Type);
            MethodInfo selectManyMethod = genericMethodInfo.MakeGenericMethod(elementType, resultElementType);

            // Note the ParameterType on an IQueryable mehtod is Expression<Func<>> where as the ParameterType
            // on an IEnumerable method is Func<>.
            Debug.Assert(
                selectManyMethod.GetParameters()[1].ParameterType == selector.GetType() ||
                selectManyMethod.GetParameters()[1].ParameterType == selector.Type,
                "selector should be of type Expression<Func<TSource, IEnumerable<TResult>>>");
            return Expression.Call(null, selectManyMethod, source, selector);
        }

        /// <summary>
        /// Call generic method
        /// </summary>
        /// <param name="genericMethodInfo">MethodInfo</param>
        /// <param name="source">Source expression</param>
        /// <param name="targetType">Target type</param>
        /// <returns>New expression with method call.</returns>
        private static Expression CallMethodWithTypeParam(MethodInfo genericMethodInfo, Expression source, Type targetType)
        {
            Debug.Assert(genericMethodInfo != null && genericMethodInfo.IsGenericMethod, "genericMethodInfo != null && genericMethodInfo.IsGenericMethod");
            Debug.Assert(source != null, "source != null");
            Debug.Assert(targetType != null, "targetType != null");            
            Debug.Assert(source.ElementType() != null, "source.ElementType() != null");

            MethodInfo method = genericMethodInfo.MakeGenericMethod(targetType);
            return Expression.Call(null, method, source);
        }

        /// <summary>
        /// Call generic method
        /// </summary>
        /// <param name="genericMethodInfo">MethodInfo</param>
        /// <param name="source">Source expression</param>
        /// <param name="selector">selector expression</param>
        /// <returns>New expression with method call.</returns>
        private static Expression CallMethodWithSelector(MethodInfo genericMethodInfo, Expression source, LambdaExpression selector)
        {
            Debug.Assert(genericMethodInfo != null && genericMethodInfo.IsGenericMethod, "genericMethodInfo != null && genericMethodInfo.IsGenericMethod");
            Debug.Assert(source != null, "source != null");
            Debug.Assert(selector != null, "selector != null");

            Type elementType = source.ElementType();
            Debug.Assert(elementType != null, "elementType != null");

            MethodInfo methodInfo = genericMethodInfo.MakeGenericMethod(elementType, selector.Body.Type);

            // Note the ParameterType on an IQueryable mehtod is Expression<Func<>> where as the ParameterType
            // on an IEnumerable method is Func<>.
            Debug.Assert(
                methodInfo.GetParameters()[1].ParameterType == selector.GetType() ||
                methodInfo.GetParameters()[1].ParameterType == selector.Type,
                "selector type should match the method parameter type, which is Expression<Func<T1, T2>>");
            Debug.Assert(selector.Type == typeof(Func<,>).MakeGenericType(elementType, selector.Body.Type), "selector should be of type Expression<Func<T1, T2>>");
            return Expression.Call(null, methodInfo, source, selector);
        }

        /// <summary>
        /// Call generic method
        /// </summary>
        /// <param name="genericMethodInfo">MethodInfo</param>
        /// <param name="source">Source expression</param>
        /// <param name="count">Take count</param>
        /// <returns>New expression with method call.</returns>
        private static Expression CallMethodWithCount(MethodInfo genericMethodInfo, Expression source, int count)
        {
            Debug.Assert(genericMethodInfo != null && genericMethodInfo.IsGenericMethod, "genericMethodInfo != null && genericMethodInfo.IsGenericMethod");
            Debug.Assert(source != null, "source != null");

            Type elementType = source.ElementType();
            Debug.Assert(elementType != null, "elementType != null");

            MethodInfo methodInfo = genericMethodInfo.MakeGenericMethod(elementType);
            return Expression.Call(null, methodInfo, source, Expression.Constant(count));
        }

        /// <summary>
        /// Call generic method
        /// </summary>
        /// <param name="genericMethodInfo">MethodInfo</param>
        /// <param name="source">Source expression</param>
        /// <returns>New expression with method call.</returns>
        private static Expression CallMethodWithNoParam(MethodInfo genericMethodInfo, Expression source)
        {
            Debug.Assert(genericMethodInfo != null && genericMethodInfo.IsGenericMethod, "genericMethodInfo != null && genericMethodInfo.IsGenericMethod");
            Debug.Assert(source != null, "source != null");

            Type elementType = source.ElementType();
            Debug.Assert(elementType != null, "elementType != null");

            MethodInfo methodInfo = genericMethodInfo.MakeGenericMethod(elementType);
            return Expression.Call(null, methodInfo, source);
        }

        /// <summary>
        /// Call generic method
        /// </summary>
        /// <param name="genericMethodInfo">MethodInfo</param>
        /// <param name="source">Source expression</param>
        /// <param name="predicate">Predicate to pass to the method</param>
        /// <returns>New expression with method call.</returns>
        private static Expression CallMethodWithPredicate(MethodInfo genericMethodInfo, Expression source, LambdaExpression predicate)
        {
            Debug.Assert(genericMethodInfo != null && genericMethodInfo.IsGenericMethod, "genericMethodInfo != null && genericMethodInfo.IsGenericMethod");
            Debug.Assert(source != null, "source != null");
            Debug.Assert(predicate != null, "predicate != null");

            Type elementType = source.ElementType();
            Debug.Assert(elementType != null, "elementType != null");

            MethodInfo allMethod = genericMethodInfo.MakeGenericMethod(elementType);

            // Note the ParameterType on an IQueryable mehtod is Expression<Func<>> where as the ParameterType
            // on an IEnumerable method is Func<>.
            Debug.Assert(
                allMethod.GetParameters()[1].ParameterType == predicate.GetType() ||
                allMethod.GetParameters()[1].ParameterType == predicate.Type,
                "predicate should be of type Expression<Func<TSource, bool>>");
            return Expression.Call(null, allMethod, source, predicate);
        }

        #endregion Private Extension Implementations

        #endregion Expression Extension Methods

        /// <summary>Replaced the type of input parameter with the given <paramref name="targetType"/></summary>
        /// <param name="input">Input lambda expression.</param>
        /// <param name="targetType">Type of the new parameter that will be replaced.</param>
        /// <returns>New lambda expression with parameter of new type.</returns>
        private static LambdaExpression ReplaceParameterTypeForLambda(LambdaExpression input, Type targetType)
        {
            Debug.Assert(input.Parameters.Count == 1, "Assuming a single parameter for input lambda expression in this function.");

            ParameterExpression p = Expression.Parameter(targetType, input.Parameters[0].Name);

            return Expression.Lambda(
                    ParameterReplacerVisitor.Replace(input.Body, input.Parameters[0], p),
                    p);
        } 
        
        /// <summary>
        /// Helper method to get the MethodInfo from the body of the given lambda expression.
        /// </summary>
        /// <typeparam name="TResult">Result type of <paramref name="lambda"/>.</typeparam>
        /// <param name="lambda">Lambda expression.</param>
        /// <returns>Returns the MethodInfo from the body of the given lambda expression.</returns>
        private static MethodInfo GetMethodInfoFromLambdaBody<TResult>(Expression<Func<TResult>> lambda)
        {
            Debug.Assert(lambda != null, "lambda != null");
            return ((MethodCallExpression)lambda.Body).Method.GetGenericMethodDefinition();
        }
    }
}
