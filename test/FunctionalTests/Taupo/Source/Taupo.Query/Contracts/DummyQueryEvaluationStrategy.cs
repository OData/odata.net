//---------------------------------------------------------------------
// <copyright file="DummyQueryEvaluationStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Dummy query evaluation strategy, used by the QueryUnresolved type. Does not have any functionality, just informs user that he/she should resolve types
    /// before using it's QueryEvaluationStrategy
    /// </summary>
    internal sealed class DummyQueryEvaluationStrategy : IQueryEvaluationStrategy
    {
        private DummyQueryEvaluationStrategy()
        {
        }

        /// <summary>
        /// Gets the instance of the DummyQueryEvaluationStrategy.
        /// </summary>
        public static DummyQueryEvaluationStrategy Instance
        {
            get
            {
                return Nested.Instance;
            }
        }

        /// <summary>
        /// Gets the default boolean type.
        /// </summary>
        public QueryScalarType BooleanType
        {
            get
            {
                throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
            }
        }

        /// <summary>
        /// Gets the default double type.
        /// </summary>
        public QueryScalarType DoubleType
        {
            get
            {
                throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
            }
        }

        /// <summary>
        /// Gets the default float type.
        /// </summary>
        public QueryScalarType FloatType
        {
            get
            {
                throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
            }
        }

        /// <summary>
        /// Gets the default integer type.
        /// </summary>
        public QueryScalarType IntegerType
        {
            get 
            {
                throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
            }
        }

        /// <summary>
        /// Gets the default long integer type.
        /// </summary>
        public QueryScalarType LongIntegerType
        {
            get
            {
                throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
            }
        }

        /// <summary>
        /// Counts the elements in the collection and returns the result.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>Value representing result of Count() operation on a collection (store specific, for example may or 
        /// may not include NULL values depending on store rules).</returns>
        public QueryScalarValue Count<TElement>(IEnumerable<TElement> collection)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Counts the elements in the collection and returns the result as 64-bit integer.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>Value representing result of Count() operation on a collection (store specific, for example may or 
        /// may not include NULL values depending on store rules).</returns>
        public QueryScalarValue LongCount<TElement>(IEnumerable<TElement> collection)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Filters the specified source using given predicate.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Filtered collection (including elements where the predicate matches).</returns>
        /// <remarks>The value can be store-dependent w.r.t. handling of NULL values of the input collection
        /// and/or result of the predicate.</remarks>
        public QueryCollectionValue Filter(QueryCollectionValue source, Func<QueryValue, QueryScalarValue> predicate)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> indicating whether all elements in a source collection
        /// match a given predicate.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Instance of <see cref="QueryScalarValue"/> indicating whether all elements in a source collection match
        /// a given predicate.</returns>
        /// <remarks>The value can be store-dependent w.r.t. handling of NULL values of the input collection
        /// and/or result of the predicate.</remarks>
        public QueryScalarValue All<TElement>(IEnumerable<TElement> source, Func<TElement, QueryScalarValue> predicate)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> indicating whether there is any element in a the collection.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> indicating whether there is any element in a the collection.
        /// </returns>
        /// <remarks>The value can be store-dependent w.r.t. handling of NULL values of the input collection
        /// and/or result of the predicate.</remarks>
        public QueryScalarValue Any<TElement>(IEnumerable<TElement> source)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> indicating whether any element in a source collection
        /// matches a given predicate.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Instance of <see cref="QueryScalarValue"/> indicating whether any element in a source collection matches
        /// a given predicate.</returns>
        /// <remarks>The value can be store-dependent w.r.t. handling of NULL values of the input collection
        /// and/or result of the predicate.</remarks>
        public QueryScalarValue Any<TElement>(IEnumerable<TElement> source, Func<TElement, QueryScalarValue> predicate)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Casts a <see cref="QueryScalarValue"/> to a <see cref="QueryScalarType"/>. The cast will return the value type cast to the new type.
        /// </summary>
        /// <param name="source">The source for the cast operation.</param>
        /// <param name="type">The type for the cast operation.</param>
        /// <returns><see cref="QueryScalarValue"/> which is cast to the appropriate type</returns>
        public QueryScalarValue Cast(QueryScalarValue source, QueryScalarType type)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Computes the Maximum value of a collection as a <see cref="QueryScalarValue"/>.
        /// </summary>
        /// <param name="collection">The collection of values.</param>
        /// <returns>The max value.</returns>
        public QueryScalarValue Max(QueryCollectionValue collection)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Computes the Minimum value of a collection as a <see cref="QueryScalarValue"/>.
        /// </summary>
        /// <param name="collection">The collection of values.</param>
        /// <returns>The min value.</returns>
        public QueryScalarValue Min(QueryCollectionValue collection)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Compares the primitive value to another value and returns their relative ordering.
        /// </summary>
        /// <param name="value">First value.</param>
        /// <param name="otherValue">Second value.</param>
        /// <returns>
        /// True if the values are equal.
        /// </returns>
        public bool AreEqual(QueryScalarValue value, QueryScalarValue otherValue)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Compares the primitive value to another value and returns their relative ordering.
        /// </summary>
        /// <param name="value">First value.</param>
        /// <param name="otherValue">Second value.</param>
        /// <returns>
        /// Integer which is less than zero if this value is less than the other value, 0 if they are equal,
        /// greater than zero if this value is greater than the other value
        /// </returns>
        public int Compare(QueryScalarValue value, QueryScalarValue otherValue)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Gets the common type to which both types can be promoted.
        /// </summary>
        /// <param name="leftType">First type.</param>
        /// <param name="rightType">Second type.</param>
        /// <returns>Common type to which both types can be promoted. Throws if unable to find a common type.</returns>
        public QueryScalarType GetCommonType(QueryScalarType leftType, QueryScalarType rightType)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Removes any length constraints from the type
        /// </summary>
        /// <param name="type">The type from which to remove the constraints</param>
        /// <returns>The new type with length constraints removed.</returns>
        public QueryScalarType RemoveLengthConstraints(QueryScalarType type)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Determines whether a unary operation on a type is supported
        /// </summary>
        /// <param name="operation">the operation</param>
        /// <param name="sourceType">the type of data to operate on</param>
        /// <returns>Value <c>true</c> if operation can be performed; otherwise, <c>false</c>.</returns>
        public bool Supports(QueryUnaryOperation operation, QueryScalarType sourceType)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Determines whether an operation between two types is supported
        /// </summary>
        /// <param name="operation">the operation</param>
        /// <param name="sourceType">the type of data to operation on</param>
        /// <param name="otherType">the other type</param>
        /// <returns>Value <c>true</c> if operation can be performed; otherwise, <c>false</c>.</returns>
        public bool Supports(QueryBinaryOperation operation, QueryScalarType sourceType, QueryScalarType otherType)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Determines whether a type supports ordering. 
        /// </summary>
        /// <param name="sourceType">the type of data to operation on</param>
        /// <returns>Value <c>true</c> if ordering can be performed; otherwise, <c>false</c>.</returns>
        public bool SupportsOrderComparison(QueryScalarType sourceType)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Do the specified UnaryOperation for the value and returns the result.
        /// </summary>
        /// <param name="operation">The binary operation to perform.</param>
        /// <param name="value">The value.</param>
        /// <returns>Result of the operation.</returns>
        public QueryScalarValue Evaluate(QueryUnaryOperation operation, QueryScalarValue value)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Do the specified BinaryOperation for the two values and returns the result.
        /// </summary>
        /// <param name="operation">The binary operation to perform.</param>
        /// <param name="firstValue">The first value.</param>
        /// <param name="secondValue">The second value.</param>
        /// <returns>Result of the operation.</returns>
        public QueryScalarValue Evaluate(QueryBinaryOperation operation, QueryScalarValue firstValue, QueryScalarValue secondValue)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Evaluates built in function with the given namespace and name.
        /// </summary>
        /// <param name="resultType">The function result type.</param>
        /// <param name="functionNamespace">The function namespace.</param>
        /// <param name="functionName">The function name.</param>
        /// <param name="arguments">Arguments for the function call.</param>
        /// <returns>Query value which is the result of function evaluation.</returns>
        public QueryValue EvaluateBuiltInFunction(QueryType resultType, string functionNamespace, string functionName, params QueryValue[] arguments)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Evaluates function.
        /// </summary>
        /// <param name="resultType">The function result type.</param>
        /// <param name="customFunction">The function to evaluate.</param>
        /// <param name="arguments">The arguments for the function call.</param>
        /// <returns>Query value which is the result of function evaluation.</returns>
        public QueryValue EvaluateFunction(QueryType resultType, Function customFunction, params QueryValue[] arguments)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Evaluates function import.
        /// </summary>
        /// <param name="resultType">The function import result type.</param>
        /// <param name="functionImport">The function import to evaluate.</param>
        /// <param name="arguments">The arguments for the function import call.</param>
        /// <returns>Query value which is the result of function import evaluation.</returns>
        public QueryValue EvaluateFunctionImport(QueryType resultType, FunctionImport functionImport, params QueryValue[] arguments)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Evaluates member property of a spatial instance.
        /// </summary>
        /// <param name="instance">The instance of query value object</param>
        /// <param name="resultType">The proeprty result type.</param>
        /// <param name="memberPropertyName">The member property name to evaluate.</param>
        /// <returns>Query value which is the result of method evaluation.</returns>
        public QueryValue EvaluateMemberProperty(QueryValue instance, QueryType resultType, string memberPropertyName)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Evaluates member method of a spatial instance.
        /// </summary>
        /// <param name="instance">The instance of query value object</param>
        /// <param name="resultType">The function result type.</param>
        /// <param name="methodName">The member method to evaluate.</param>
        /// <param name="arguments">Arguments for the function call.</param>
        /// <returns>Query value which is the result of function evaluation.</returns>
        public QueryValue EvaluateMemberMethod(QueryValue instance, QueryType resultType, string methodName, params QueryValue[] arguments)
        {
            throw new TaupoNotSupportedException("Attempt to use evaluation strategy from " + typeof(QueryUnresolvedType).Name + ". Please resolve expression first.");
        }

        /// <summary>
        /// Nested class for lazy instantiation
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used for lazily instantiated singleton.")]
        private class Nested
        {
            internal static readonly DummyQueryEvaluationStrategy Instance = new DummyQueryEvaluationStrategy();
        }
    }
}