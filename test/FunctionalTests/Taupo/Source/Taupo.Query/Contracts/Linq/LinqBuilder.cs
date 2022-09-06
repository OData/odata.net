//---------------------------------------------------------------------
// <copyright file="LinqBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;
    using CoreLinq = System.Linq.Expressions;

    /// <summary>
    /// Factory methods used to construct QueryExpression tree nodes.
    /// </summary>
    public static class LinqBuilder
    {
        /// <summary>
        /// Factory method to create the <see cref="LinqAllExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The <see cref="LinqAllExpression"/> with the provided arguments.</returns>
        public static QueryExpression All(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(predicate(parameter), parameter);

            return new LinqAllExpression(source, lambda, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqAnyExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="LinqAnyExpression"/> with the provided source.</returns>
        public static QueryExpression Any(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqAnyExpression(source, null, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqAnyExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The <see cref="LinqAnyExpression"/> with the provided arguments.</returns>
        public static QueryExpression Any(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(predicate(parameter), parameter);

            return new LinqAnyExpression(source, lambda, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqAsEnumerableExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="LinqAsEnumerableExpression"/> with the provided argument.</returns>
        public static QueryExpression AsEnumerable(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqAsEnumerableExpression(source, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqBitwiseAndExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="LinqBitwiseAndExpression"/> with the provided arguments.</returns>
        public static QueryExpression BitwiseAnd(this QueryExpression left, QueryExpression right)
        {
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(right, "right");

            return new LinqBitwiseAndExpression(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqBitwiseOrExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="LinqBitwiseOrExpression"/> with the provided arguments.</returns>
        public static QueryExpression BitwiseOr(this QueryExpression left, QueryExpression right)
        {
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(right, "right");

            return new LinqBitwiseOrExpression(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqBuiltInFunctionCallExpression"/>.
        /// </summary>
        /// <param name="builtInFunction">The built-in function to call.</param>
        /// <param name="arguments">The arguments for the function call.</param>
        /// <returns>The <see cref="LinqBuiltInFunctionCallExpression"/> with the provided arguments.</returns>
        public static QueryExpression Call(this LinqBuiltInFunction builtInFunction, params QueryExpression[] arguments)
        {
            ExceptionUtilities.CheckArgumentNotNull(builtInFunction, "builtInFunction");
            ExceptionUtilities.CheckArgumentNotNull(arguments, "arguments");

            return new LinqBuiltInFunctionCallExpression(QueryType.Unresolved, builtInFunction, arguments);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqMemberMethodExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="memberMethod">The linq member method to call.</param>
        /// <param name="arguments">The arguments for the method call.</param>
        /// <returns>The <see cref="LinqMemberMethodExpression"/> with the provided arguments.</returns>
        public static QueryExpression Call(this QueryExpression source, Function memberMethod, params QueryExpression[] arguments)
        {
            return new LinqMemberMethodExpression(source, memberMethod, QueryType.UnresolvedPrimitive, arguments);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqCastExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="type">The type to cast to.</param>
        /// <returns>The <see cref="LinqCastExpression"/> with the provided source.</returns>
        public static QueryExpression CastEnumerable(this QueryExpression source, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.Assert(!type.IsUnresolved, "Type must be resolved.");

            return new LinqCastExpression(source, type, type.CreateCollectionType());
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqConcatExpression"/>
        /// </summary>
        /// <param name="outer">Outer input collection.</param>
        /// <param name="inner">Inner input collection.</param>
        /// <returns>The <see cref="LinqConcatExpression"/> with the provided arguments.</returns>
        public static QueryExpression Concat(
            this QueryExpression outer,
            QueryExpression inner)
        {
            return new LinqConcatExpression(outer, inner, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqContainsExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="value">The value to locate in source.</param>
        /// <returns>The <see cref="LinqContainsExpression"/> with the provided arguments.</returns>
        public static QueryExpression Contains(this QueryExpression source, QueryExpression value)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(value, "value");

            return new LinqContainsExpression(source, value, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqCountExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="LinqCountExpression"/> with the provided source.</returns>
        public static QueryExpression Count(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqCountExpression(source, null, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqCountExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The <see cref="LinqCountExpression"/> with the provided arguments.</returns>
        public static QueryExpression Count(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(predicate(parameter), parameter);

            return new LinqCountExpression(source, lambda, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqDefaultIfEmptyExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="LinqDefaultIfEmptyExpression"/> with the provided source.</returns>
        public static QueryExpression DefaultIfEmpty(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqDefaultIfEmptyExpression(source, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqDefaultIfEmptyExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The <see cref="LinqDefaultIfEmptyExpression"/> with the provided source.</returns>
        public static QueryExpression DefaultIfEmpty(this QueryExpression source, QueryExpression defaultValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqDefaultIfEmptyExpression(source, QueryType.Unresolved, defaultValue);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqDistinctExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="LinqDistinctExpression"/> with the provided argument.</returns>
        public static QueryExpression Distinct(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqDistinctExpression(source, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqExclusiveOrExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="LinqExclusiveOrExpression"/> with the provided arguments.</returns>
        public static QueryExpression ExclusiveOr(this QueryExpression left, QueryExpression right)
        {
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(right, "right");

            return new LinqExclusiveOrExpression(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqFirstExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="LinqFirstExpression"/> with the provided source.</returns>
        public static QueryExpression First(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqFirstExpression(source, null, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqFirstOrDefaultExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="LinqFirstOrDefaultExpression"/> with the provided source.</returns>
        public static QueryExpression FirstOrDefault(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqFirstOrDefaultExpression(source, null, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqFirstExpression"/> with a predicate.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The <see cref="LinqFirstExpression"/> with the provided arguments.</returns>
        public static QueryExpression First(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(predicate(parameter), parameter);

            return new LinqFirstExpression(source, lambda, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqFirstOrDefaultExpression"/> with a predicate.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The <see cref="LinqFirstOrDefaultExpression"/> with the provided arguments.</returns>
        public static QueryExpression FirstOrDefault(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(predicate(parameter), parameter);

            return new LinqFirstOrDefaultExpression(source, lambda, QueryType.Unresolved);
        }

        /// <summary>
        /// Creates the free variable for use in LINQ expression.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="type">Variable type.</param>
        /// <param name="initialValue">The initial value of a variable.</param>
        /// <param name="additionalValues">The additional values.</param>
        /// <returns>Expression representing free variable.</returns>
        public static QueryExpression FreeVariable(string name, QueryType type, QueryExpression initialValue, params QueryExpression[] additionalValues)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.CheckArgumentNotNull(initialValue, "initialValue");
            ExceptionUtilities.CheckArgumentNotNull(additionalValues, "additionalValues");

            var possibleValues = new List<QueryExpression>();
            possibleValues.Add(initialValue);
            possibleValues.AddRange(additionalValues);

            return new LinqFreeVariableExpression(name, type, possibleValues);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqGroupByExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="keySelector">Key selector lambda.</param>
        /// <returns>The <see cref="LinqGroupByExpression"/> with the provided key selector.</returns>
        public static QueryExpression GroupBy(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> keySelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(keySelector, "keySelector");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(keySelector(parameter), parameter);

            return GroupBy(source, lambda, null, null);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqGroupByExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="keySelector">Key selector lambda.</param>
        /// <param name="elementSelector">Element selector lambda.</param>
        /// <returns>The <see cref="LinqGroupByExpression"/> with the provided key and element selectors.</returns>
        public static QueryExpression GroupBy(
            this QueryExpression source, 
            Func<LinqParameterExpression, QueryExpression> keySelector, 
            Func<LinqParameterExpression, QueryExpression> elementSelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(keySelector, "keySelector");
            ExceptionUtilities.CheckArgumentNotNull(elementSelector, "elementSelector");

            var keySelectorParameter = Parameter(QueryType.Unresolved);
            var keySelectorLambda = Lambda(keySelector(keySelectorParameter), keySelectorParameter);

            var elementSelectorParameter = Parameter(QueryType.Unresolved);
            var elementSelectorLambda = Lambda(elementSelector(elementSelectorParameter), elementSelectorParameter);

            return GroupBy(source, keySelectorLambda, elementSelectorLambda, null);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqGroupByExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="keySelector">Key selector lambda.</param>
        /// <param name="resultSelector">Result selector lambda.</param>
        /// <returns>The <see cref="LinqGroupByExpression"/> with the provided key and result selectors.</returns>
        public static QueryExpression GroupBy(
            this QueryExpression source,
            Func<LinqParameterExpression, QueryExpression> keySelector,
            Func<LinqParameterExpression, LinqParameterExpression, QueryExpression> resultSelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(keySelector, "keySelector");
            ExceptionUtilities.CheckArgumentNotNull(resultSelector, "resultSelector");

            var keySelectorParameter = Parameter(QueryType.Unresolved);
            var keySelectorLambda = Lambda(keySelector(keySelectorParameter), keySelectorParameter);

            var resultSelectorKeyParameter = Parameter(QueryType.Unresolved);
            var resultSelectorGroupingParameter = Parameter(QueryType.Unresolved);
            var resulSelectorLambda = Lambda(resultSelector(resultSelectorKeyParameter, resultSelectorGroupingParameter), resultSelectorKeyParameter, resultSelectorGroupingParameter);

            return GroupBy(source, keySelectorLambda, null, resulSelectorLambda);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqGroupByExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="keySelector">Key selector lambda.</param>
        /// <param name="elementSelector">Element selector lambda.</param>
        /// <param name="resultSelector">Result selector lambda.</param>
        /// <returns>The <see cref="LinqGroupByExpression"/> with the provided key element and result selectors.</returns>
        public static QueryExpression GroupBy(
            this QueryExpression source,
            Func<LinqParameterExpression, QueryExpression> keySelector,
            Func<LinqParameterExpression, QueryExpression> elementSelector,
            Func<LinqParameterExpression, LinqParameterExpression, QueryExpression> resultSelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(keySelector, "keySelector");
            ExceptionUtilities.CheckArgumentNotNull(elementSelector, "elementSelector");
            ExceptionUtilities.CheckArgumentNotNull(resultSelector, "resultSelector");

            var keySelectorParameter = Parameter(QueryType.Unresolved);
            var keySelectorLambda = Lambda(keySelector(keySelectorParameter), keySelectorParameter);

            var elementSelectorParameter = Parameter(QueryType.Unresolved);
            var elementSelectorLambda = Lambda(elementSelector(elementSelectorParameter), elementSelectorParameter);

            var resultSelectorKeyParameter = Parameter(QueryType.Unresolved);
            var resultSelectorGroupingParameter = Parameter(QueryType.Unresolved);
            var resulSelectorLambda = Lambda(resultSelector(resultSelectorKeyParameter, resultSelectorGroupingParameter), resultSelectorKeyParameter, resultSelectorGroupingParameter);

            return GroupBy(source, keySelectorLambda, elementSelectorLambda, resulSelectorLambda);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqGroupByExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="keySelector">Key selector lambda expression.</param>
        /// <param name="elementSelector">Element selector lambda expression.</param>
        /// <param name="resultSelector">Result selector lambda expression.</param>
        /// <returns>The <see cref="LinqGroupByExpression"/> with the provided argument.</returns>
        public static QueryExpression GroupBy(
            this QueryExpression source, 
            LinqLambdaExpression keySelector, 
            LinqLambdaExpression elementSelector, 
            LinqLambdaExpression resultSelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqGroupByExpression(source, keySelector, elementSelector, resultSelector, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqGroupJoinExpression"/>.
        /// </summary>
        /// <param name="outer">First input collection.</param>
        /// <param name="inner">Second input collection.</param>
        /// <param name="outerKeySelector">First key selector to the join condition.</param>
        /// <param name="innerKeySelector">Second key selector to the join condition.</param>
        /// <param name="resultSelector">Result selector</param>
        /// <returns>The <see cref="LinqGroupJoinExpression"/> with the provided arguments.</returns>
        public static QueryExpression GroupJoin(
            this QueryExpression outer,
            QueryExpression inner,
            Func<LinqParameterExpression, QueryExpression> outerKeySelector,
            Func<LinqParameterExpression, QueryExpression> innerKeySelector,
            Func<LinqParameterExpression, LinqParameterExpression, QueryExpression> resultSelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(outerKeySelector, "outerKeySelector");
            ExceptionUtilities.CheckArgumentNotNull(innerKeySelector, "innerKeySelector");
            ExceptionUtilities.CheckArgumentNotNull(resultSelector, "resultSelector");

            var outerParameter = Parameter(QueryType.Unresolved);
            var innerParameter = Parameter(QueryType.Unresolved);
            var outerKeySelectorLambda = Lambda(outerKeySelector(outerParameter), outerParameter);
            var innerKeySelectorLambda = Lambda(innerKeySelector(innerParameter), innerParameter);
            var resultSelectorLambda = Lambda(resultSelector(outerParameter, innerParameter), outerParameter, innerParameter);

            return GroupJoin(outer, inner, outerKeySelectorLambda, innerKeySelectorLambda, resultSelectorLambda, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqExceptExpression"/>
        /// </summary>
        /// <param name="outer">Outer collection.</param>
        /// <param name="inner">Inner collection.</param>
        /// <returns>The <see cref="LinqExceptExpression"/> with the provided arguments.</returns>
        public static QueryExpression Except(
            this QueryExpression outer, 
            QueryExpression inner)
        {
            return new LinqExceptExpression(outer, inner, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqGroupJoinExpression"/>.
        /// </summary>
        /// <param name="outer">First input collection.</param>
        /// <param name="inner">Second input collection.</param>
        /// <param name="outerKeySelector">First key selector to the join condition.</param>
        /// <param name="innerKeySelector">Second key selector to the join condition.</param>
        /// <param name="resultSelector">Result selector</param>
        /// <returns>The <see cref="LinqGroupJoinExpression"/> with the provided arguments.</returns>
        public static QueryExpression GroupJoin(
            this QueryExpression outer,
            QueryExpression inner,
            LinqLambdaExpression outerKeySelector,
            LinqLambdaExpression innerKeySelector,
            LinqLambdaExpression resultSelector)
        {
            return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqJoinExpression"/>.
        /// </summary>
        /// <param name="outer">First input collection.</param>
        /// <param name="inner">Second input collection.</param>
        /// <param name="outerKeySelector">First key selector to the join condition.</param>
        /// <param name="innerKeySelector">Second key selector to the join condition.</param>
        /// <param name="resultSelector">Result selector</param>
        /// <returns>The <see cref="LinqJoinExpression"/> with the provided arguments.</returns>
        public static QueryExpression Join(
            this QueryExpression outer,
            QueryExpression inner,
            Func<LinqParameterExpression, QueryExpression> outerKeySelector,
            Func<LinqParameterExpression, QueryExpression> innerKeySelector,
            Func<LinqParameterExpression, LinqParameterExpression, QueryExpression> resultSelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(outerKeySelector, "outerKeySelector");
            ExceptionUtilities.CheckArgumentNotNull(innerKeySelector, "innerKeySelector");
            ExceptionUtilities.CheckArgumentNotNull(resultSelector, "resultSelector");

            var outerParameter = Parameter(QueryType.Unresolved);
            var innerParameter = Parameter(QueryType.Unresolved);
            var outerKeySelectorLambda = Lambda(outerKeySelector(outerParameter), outerParameter);
            var innerKeySelectorLambda = Lambda(innerKeySelector(innerParameter), innerParameter);
            var resultSelectorLambda = Lambda(resultSelector(outerParameter, innerParameter), outerParameter, innerParameter);

            return Join(outer, inner, outerKeySelectorLambda, innerKeySelectorLambda, resultSelectorLambda, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqJoinExpression"/>.
        /// </summary>
        /// <param name="outer">First input collection.</param>
        /// <param name="inner">Second input collection.</param>
        /// <param name="outerKeySelector">First key selector to the join condition.</param>
        /// <param name="innerKeySelector">Second key selector to the join condition.</param>
        /// <param name="resultSelector">Result selector</param>
        /// <returns>The <see cref="LinqJoinExpression"/> with the provided arguments.</returns>
        public static QueryExpression Join(
            this QueryExpression outer,
            QueryExpression inner,
            LinqLambdaExpression outerKeySelector,
            LinqLambdaExpression innerKeySelector,
            LinqLambdaExpression resultSelector)
        {
            return Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqLambdaExpression"/>.
        /// </summary>
        /// <param name="body">The body of lambda.</param>
        /// <param name="parameters">Parameters to the lambda.</param>
        /// <returns>The <see cref="LinqLambdaExpression"/> with the provided arguments.</returns>
        public static LinqLambdaExpression Lambda(QueryExpression body, params LinqParameterExpression[] parameters)
        {
            return Lambda(body, parameters, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqLengthPropertyExpression"/>.
        /// </summary>
        /// <param name="instance">The instance to call property on.</param>
        /// <returns>The <see cref="LinqLengthPropertyExpression"/> with the provided instance.</returns>
        public static QueryExpression LengthProperty(this QueryExpression instance)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");

            return new LinqLengthPropertyExpression(instance, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqLongCountExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="LinqLongCountExpression"/> with the provided source.</returns>
        public static QueryExpression LongCount(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqLongCountExpression(source, null, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqLongCountExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The <see cref="LinqLongCountExpression"/> with the provided arguments.</returns>
        public static QueryExpression LongCount(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(predicate(parameter), parameter);

            return new LinqLongCountExpression(source, lambda, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqMaxExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="LinqMaxExpression"/> with the provided source.</returns>
        public static QueryExpression Max(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqMaxExpression(source, null, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqMaxExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The <see cref="LinqMaxExpression"/> with the provided arguments.</returns>
        public static QueryExpression Max(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(predicate(parameter), parameter);

            return new LinqMaxExpression(source, lambda, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqMinExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="LinqMinExpression"/> with the provided source.</returns>
        public static QueryExpression Min(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqMinExpression(source, null, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqMinExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The <see cref="LinqMinExpression"/> with the provided arguments.</returns>
        public static QueryExpression Min(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(predicate(parameter), parameter);

            return new LinqMinExpression(source, lambda, QueryType.Unresolved);
        }

        /// <summary>
        /// Creates LinqNewExpression from the anonymous object.
        /// </summary>
        /// <param name="anonymousInstance">Anonymous object to create the expression from.</param>
        /// <returns>LinqNewExpression with arguments taken from the given anonymous object.</returns>
        public static QueryExpression New(object anonymousInstance)
        {
            ExceptionUtilities.CheckArgumentNotNull(anonymousInstance, "anonymousInstance");

            if (!anonymousInstance.GetType().GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any())
            {
                throw new TaupoArgumentException("Given argument must be an anonymous object.");
            }

            var memberNames = new List<string>();
            var members = new List<QueryExpression>();

            foreach (var propertyInfo in anonymousInstance.GetType().GetProperties())
            {
                memberNames.Add(propertyInfo.Name);
                members.Add((QueryExpression)propertyInfo.GetValue(anonymousInstance, null));       
            }
            
            return New(memberNames, members, QueryType.Unresolved);
        }

        /// <summary>
        /// Creates a new array of a particular element type and adds in the elements
        /// </summary>
        /// <param name="arrayElementType">Array Element type</param>
        /// <param name="expressions">Expressions to add to the array</param>
        /// <returns>A new array expression</returns>
        public static QueryExpression NewArray(this QueryType arrayElementType, params QueryExpression[] expressions)
        {
            return arrayElementType.NewArray(expressions.AsEnumerable());
        }

        /// <summary>
        /// Creates a new array of a particular element type and adds in the elements
        /// </summary>
        /// <param name="arrayElementType">Array Element type</param>
        /// <param name="expressions">Expressions to add to the array</param>
        /// <returns>A new array expression</returns>
        public static QueryExpression NewArray(this QueryType arrayElementType, IEnumerable<QueryExpression> expressions)
        {
            return new LinqNewArrayExpression(arrayElementType.CreateCollectionType(), expressions);
        }

        /// <summary>
        ///  Creates a new anonymous array with the given the elements.
        /// </summary>
        /// <param name="expressions">The element expressions.</param>
        /// <returns>A new array with an unresolved type.</returns>
        public static QueryExpression AnonymousArray(params QueryExpression[] expressions)
        {
            return new LinqNewArrayExpression(QueryType.UnresolvedCollection, expressions);
        }

        /// <summary>
        /// Creates LinqNewExpression from the list of member names and members.
        /// </summary>
        /// <param name="memberNames">List of member names for the anonymous type.</param>
        /// <param name="members">List of members for the anonymous type.</param>
        /// <returns>LinqNewExpression with arguments taken from the given member names and values.</returns>
        public static QueryExpression New(IEnumerable<string> memberNames, IEnumerable<QueryExpression> members)
        {
            return New(memberNames, members, QueryType.Unresolved);
        }

        /// <summary>
        /// Creates LinqNewExpression from the list of member names and members.
        /// </summary>
        /// <param name="type">The query Entity Type which is selected into.</param>
        /// <param name="memberNames">List of member names for the entity type.</param>
        /// <param name="members">List of members for the entity type.</param>
        /// <typeparam name="TQueryClrType">A Type which is a QueryStructuraltype and implements IQueryClrType</typeparam> 
        /// <returns>LinqNewExpression with arguments taken from the given member names and values.</returns>
        public static QueryExpression NewInstance<TQueryClrType>(this TQueryClrType type, IEnumerable<string> memberNames, IEnumerable<QueryExpression> members) where TQueryClrType : QueryType, IQueryClrType
        {
            return NewInstance(Enumerable.Empty<QueryExpression>(), memberNames, members, type);
        }

        /// <summary>
        /// Creates LinqNewExpression from the list of member names and members.
        /// </summary>
        /// <param name="type">The query Entity Type which is selected into.</param>
        /// <param name="constructorParameters">List of parameters to the constructor of the type.</param>
        /// <param name="memberNames">List of member names for the entity type.</param>
        /// <param name="members">List of members for the entity type.</param>
        /// <typeparam name="TQueryClrType">A Type which is a QueryType and implements IQueryClrType</typeparam> 
        /// <returns>LinqNewExpression with arguments taken from the given member names and values.</returns>
        public static QueryExpression NewInstance<TQueryClrType>(this TQueryClrType type, IEnumerable<QueryExpression> constructorParameters, IEnumerable<string> memberNames, IEnumerable<QueryExpression> members) where TQueryClrType : QueryType, IQueryClrType
        {
            return NewInstance(constructorParameters, memberNames, members, type);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqOrderByExpression"/> with ascending order.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>The <see cref="LinqOrderByExpression"/> with the provided arguments.</returns>
        public static LinqOrderByExpression OrderBy(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> keySelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(keySelector, "keySelector");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(keySelector(parameter), parameter);

            return OrderBy(source, new[] { lambda }, new[] { false }, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqOrderByExpression"/> with descending order.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>The <see cref="LinqOrderByExpression"/> with the provided arguments.</returns>
        public static LinqOrderByExpression OrderByDescending(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> keySelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(keySelector, "keySelector");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(keySelector(parameter), parameter);

            return OrderBy(source, new[] { lambda }, new[] { true }, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqParameterExpression"/>.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <returns>The <see cref="LinqParameterExpression"/> with the provided name and type.</returns>
        public static LinqParameterExpression Parameter(string name, QueryType parameterType)
        {
            // we allow empty and null name when constructing the parameter. The names are resolved later.
            ExceptionUtilities.CheckArgumentNotNull(parameterType, "parameterType");

            return new LinqParameterExpression(name, parameterType);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqSelectExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "Used to model Linq Select method")]
        public static QueryExpression Select(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> selector)
        {
            ExceptionUtilities.CheckArgumentNotNull(selector, "selector");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(selector(parameter), parameter);

            return Select(source, lambda);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqSelectExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "Used to model Linq Select method")]
        public static QueryExpression Select(this QueryExpression source, LinqLambdaExpression selector)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(selector, "selector");

            return new LinqSelectExpression(source, selector, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqSelectManyExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        public static QueryExpression SelectMany(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> selector)
        {
            ExceptionUtilities.CheckArgumentNotNull(selector, "selector");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(selector(parameter), parameter);

            return SelectMany(source, lambda);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqSelectManyExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        public static QueryExpression SelectMany(this QueryExpression source, LinqLambdaExpression selector)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(selector, "selector");

            return new LinqSelectManyExpression(source, selector, null, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqSelectManyExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="collectionSelector">The collection selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        public static QueryExpression SelectMany(
            this QueryExpression source, 
            Func<LinqParameterExpression, QueryExpression> collectionSelector, 
            Func<LinqParameterExpression, LinqParameterExpression, QueryExpression> resultSelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(collectionSelector, "collectionSelector");
            ExceptionUtilities.CheckArgumentNotNull(resultSelector, "resultSelector");

            var sourceParameter = Parameter(QueryType.Unresolved);
            var collectionParameter = Parameter(QueryType.Unresolved);
            var collectionSelectorLambda = Lambda(collectionSelector(sourceParameter), sourceParameter);
            var resultSelectorLambda = Lambda(resultSelector(sourceParameter, collectionParameter), sourceParameter, collectionParameter);

            return SelectMany(source, collectionSelectorLambda, resultSelectorLambda);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqSelectManyExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="collectionSelector">The collection selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        public static QueryExpression SelectMany(this QueryExpression source, LinqLambdaExpression collectionSelector, LinqLambdaExpression resultSelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(collectionSelector, "collectionSelector");
            ExceptionUtilities.CheckArgumentNotNull(resultSelector, "resultSelector");

            return new LinqSelectManyExpression(source, collectionSelector, resultSelector, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqSingleExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="LinqSingleExpression"/> with the provided source.</returns>
        public static QueryExpression Single(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqSingleExpression(source, null, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqSingleExpression"/> with a predicate.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The <see cref="LinqSingleExpression"/> with the provided arguments.</returns>
        public static QueryExpression Single(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(predicate(parameter), parameter);

            return new LinqSingleExpression(source, lambda, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqSingleOrDefaultExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="LinqSingleOrDefaultExpression"/> with the provided source.</returns>
        public static QueryExpression SingleOrDefault(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqSingleOrDefaultExpression(source, null, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqSingleOrDefaultExpression"/> with a predicate.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The <see cref="LinqSingleOrDefaultExpression"/> with the provided arguments.</returns>
        public static QueryExpression SingleOrDefault(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(predicate(parameter), parameter);

            return new LinqSingleOrDefaultExpression(source, lambda, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqSkipExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="skipCount">How many elements to take.</param>
        /// <returns>The <see cref="LinqSkipExpression"/> with the provided arguments.</returns>
        public static QueryExpression Skip(this QueryExpression source, QueryExpression skipCount)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(skipCount, "skipCount");

            return new LinqSkipExpression(source, skipCount, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqSkipExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="skipCount">How many elements to take.</param>
        /// <returns>The <see cref="LinqSkipExpression"/> with the provided arguments.</returns>
        public static QueryExpression Skip(this QueryExpression source, int skipCount)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return source.Skip(CommonQueryBuilder.Constant(skipCount, QueryType.UnresolvedPrimitive));
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqTakeExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="takeCount">How many elements to take.</param>
        /// <returns>The <see cref="LinqTakeExpression"/> with the provided arguments.</returns>
        public static QueryExpression Take(this QueryExpression source, QueryExpression takeCount)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(takeCount, "takeCount");

            return new LinqTakeExpression(source, takeCount, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqTakeExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="takeCount">How many elements to take.</param>
        /// <returns>The <see cref="LinqTakeExpression"/> with the provided arguments.</returns>
        public static QueryExpression Take(this QueryExpression source, int takeCount)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return source.Take(CommonQueryBuilder.Constant(takeCount, QueryType.UnresolvedPrimitive));
        }

        /// <summary>
        /// Factory method to add ordering to the <see cref="LinqOrderByExpression"/> with ascending order.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>The <see cref="LinqOrderByExpression"/> with added ordering.</returns>
        public static LinqOrderByExpression ThenBy(this LinqOrderByExpression source, Func<LinqParameterExpression, QueryExpression> keySelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(keySelector, "keySelector");

            var parameter = Parameter(QueryType.Unresolved);
            var keySelectorLambda = Lambda(keySelector(parameter), parameter);

            return OrderBy(source.Source, source.KeySelectors.Concat(new[] { keySelectorLambda }), source.AreDescending.Concat(new[] { false }), QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to add ordering to the <see cref="LinqOrderByExpression"/> with ascending order.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>The <see cref="LinqOrderByExpression"/> with added ordering.</returns>
        public static LinqOrderByExpression ThenByDescending(this LinqOrderByExpression source, Func<LinqParameterExpression, QueryExpression> keySelector)
        {
            ExceptionUtilities.CheckArgumentNotNull(keySelector, "keySelector");

            var parameter = Parameter(QueryType.Unresolved);
            var keySelectorLambda = Lambda(keySelector(parameter), parameter);

            return OrderBy(source.Source, source.KeySelectors.Concat(new[] { keySelectorLambda }), source.AreDescending.Concat(new[] { true }), QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqUnionExpression"/>.
        /// </summary>
        /// <param name="firstSource">First input collection.</param>
        /// <param name="secondSource">Second input collection.</param>
        /// <returns>The <see cref="LinqUnionExpression"/> with the provided arguments.</returns>
        public static QueryExpression Union(this QueryExpression firstSource, QueryExpression secondSource)
        {
            ExceptionUtilities.CheckArgumentNotNull(firstSource, "firstSource");
            ExceptionUtilities.CheckArgumentNotNull(secondSource, "secondSource");

            return new LinqUnionExpression(firstSource, secondSource, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqWhereExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The <see cref="LinqWhereExpression"/> with the provided arguments.</returns>
        public static QueryExpression Where(this QueryExpression source, Func<LinqParameterExpression, QueryExpression> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            var parameter = Parameter(QueryType.Unresolved);
            var lambda = Lambda(predicate(parameter), parameter);

            return Where(source, lambda);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqWhereExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The <see cref="LinqWhereExpression"/> with the provided arguments.</returns>
        public static QueryExpression Where(this QueryExpression source, LinqLambdaExpression predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            return new LinqWhereExpression(source, predicate, QueryType.Unresolved);
        }

        internal static QueryExpression GroupJoin(
            QueryExpression outer,
            QueryExpression inner,
            LinqLambdaExpression outerKeySelector,
            LinqLambdaExpression innerKeySelector,
            LinqLambdaExpression resultSelector,
            QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(outer, "outer");
            ExceptionUtilities.CheckArgumentNotNull(inner, "inner");
            ExceptionUtilities.CheckArgumentNotNull(outerKeySelector, "outerKeySelector");
            ExceptionUtilities.CheckArgumentNotNull(innerKeySelector, "innerKeySelector");
            ExceptionUtilities.CheckArgumentNotNull(resultSelector, "resultSelector");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new LinqGroupJoinExpression(outer, inner, outerKeySelector, innerKeySelector, resultSelector, type);
        }

        internal static QueryExpression Join(
            QueryExpression outer,
            QueryExpression inner,
            LinqLambdaExpression outerKeySelector,
            LinqLambdaExpression innerKeySelector,
            LinqLambdaExpression resultSelector,
            QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(outer, "outer");
            ExceptionUtilities.CheckArgumentNotNull(inner, "inner");
            ExceptionUtilities.CheckArgumentNotNull(outerKeySelector, "outerKeySelector");
            ExceptionUtilities.CheckArgumentNotNull(innerKeySelector, "innerKeySelector");
            ExceptionUtilities.CheckArgumentNotNull(resultSelector, "resultSelector");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new LinqJoinExpression(outer, inner, outerKeySelector, innerKeySelector, resultSelector, type);
        }

        internal static LinqLambdaExpression Lambda(QueryExpression body, LinqParameterExpression[] parameters, QueryType type)
        {
            ExceptionUtilities.CheckObjectNotNull(body, "body");
            ExceptionUtilities.CheckObjectNotNull(parameters, "parameters");
            ExceptionUtilities.CheckObjectNotNull(type, "type");

            return new LinqLambdaExpression(body, parameters, type);
        }

        internal static QueryExpression New(IEnumerable<string> memberNames, IEnumerable<QueryExpression> members, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(memberNames, "memberNames");
            ExceptionUtilities.CheckArgumentNotNull(members, "members");
            ExceptionUtilities.CheckCollectionNotEmpty(memberNames, "memberNames");
            ExceptionUtilities.CheckCollectionNotEmpty(members, "members");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.Assert(memberNames.Count() == members.Count(), "The count of member names and members should be the same.");

            return new LinqNewExpression(memberNames, members, type);
        }

        internal static QueryExpression NewInstance(IEnumerable<QueryExpression> constructorParameters, IEnumerable<string> memberNames, IEnumerable<QueryExpression> members, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.CheckArgumentNotNull(constructorParameters, "constructorParameters");
            ExceptionUtilities.CheckArgumentNotNull(memberNames, "memberNames");
            ExceptionUtilities.CheckArgumentNotNull(members, "members");
            ExceptionUtilities.Assert(memberNames.Count() == members.Count(), "The count of member names and members should be the same.");

            return new LinqNewInstanceExpression(constructorParameters, memberNames, members, type);
        }

        internal static LinqOrderByExpression OrderBy(QueryExpression source, IEnumerable<LinqLambdaExpression> keySelectors, IEnumerable<bool> areDescending, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckCollectionNotEmpty(keySelectors, "keySelectors");
            ExceptionUtilities.CheckCollectionNotEmpty(areDescending, "areDescending");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new LinqOrderByExpression(source, keySelectors, areDescending, type);
        }

        internal static LinqParameterExpression Parameter(QueryType parameterType)
        {
            return new LinqParameterExpression(null, parameterType);
        }
    }
}
