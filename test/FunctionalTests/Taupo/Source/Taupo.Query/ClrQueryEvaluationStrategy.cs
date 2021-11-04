//---------------------------------------------------------------------
// <copyright file="ClrQueryEvaluationStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Spatial;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Linq;

    /// <summary>
    /// Implementation of <see cref="IQueryEvaluationStrategy"/> that uses CLR/LINQ semantics.
    /// </summary>
    public class ClrQueryEvaluationStrategy : IQueryEvaluationStrategy
    {
        /// <summary>
        /// Initializes a new instance of the ClrQueryEvaluationStrategy class.
        /// </summary>
        public ClrQueryEvaluationStrategy()
        {
            this.BooleanType = new QueryClrPrimitiveType(typeof(bool), this);
            this.DoubleType = new QueryClrPrimitiveType(typeof(double), this);
            this.FloatType = new QueryClrPrimitiveType(typeof(float), this);
            this.IntegerType = new QueryClrPrimitiveType(typeof(int), this);
            this.LongIntegerType = new QueryClrPrimitiveType(typeof(long), this);
        }

        /// <summary>
        /// Gets the default boolean type.
        /// </summary>
        public QueryScalarType BooleanType { get; private set; }

        /// <summary>
        /// Gets the default dobule type.
        /// </summary>
        public QueryScalarType DoubleType { get; private set; }

        /// <summary>
        /// Gets the default float type.
        /// </summary>
        public QueryScalarType FloatType { get; private set; }

        /// <summary>
        /// Gets the default integer type.
        /// </summary>
        /// <value></value>
        public QueryScalarType IntegerType { get; private set; }

        /// <summary>
        /// Gets the default long integer type.
        /// </summary>
        /// <value></value>
        public QueryScalarType LongIntegerType { get; private set; }

        /// <summary>
        /// Gets or sets the spatial clr type resolver
        /// </summary>
        [InjectDependency]
        public ISpatialClrTypeResolver SpatialTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets the spatial equality comparer
        /// </summary>
        [InjectDependency]
        public ISpatialEqualityComparer SpatialEqualityComparer { get; set; }

        /// <summary>
        /// Counts the elements in the collection and returns the result.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// Value representing result of Count() operation on a collection (store specific, for example may or
        /// may not include NULL values depending on store rules).
        /// </returns>
        public QueryScalarValue Count<TElement>(IEnumerable<TElement> collection)
        {
            int? value = null;
            if (collection != null)
            {
                value = collection.Count();
            }

            return this.IntegerType.CreateValue(value);
        }

        /// <summary>
        /// Counts the elements in the collection and returns the result as 64-bit integer.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// Value representing result of Count() operation on a collection (store specific, for example may or
        /// may not include NULL values depending on store rules).
        /// </returns>
        public QueryScalarValue LongCount<TElement>(IEnumerable<TElement> collection)
        {
            long? value = null;
            if (collection != null)
            {
                value = collection.LongCount();
            }

            return this.LongIntegerType.CreateValue(value);
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> indicating whether the source collection contains any elements.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> indicating whether the source collection contains any elements.
        /// </returns>
        /// <remarks>The value can be store-dependent w.r.t. handling of NULL values of the input collection</remarks>
        public QueryScalarValue Any<TElement>(IEnumerable<TElement> source)
        {
            if (source == null)
            {
                return this.BooleanType.CreateValue(false);
            }
            else
            {
                return this.BooleanType.CreateValue(source.Any());
            }
        }

        /// <summary>
        /// Casts a <see cref="QueryScalarValue"/> to a <see cref="QueryScalarType"/>. The cast will return the value type cast to the new type.
        /// </summary>
        /// <param name="source">The source for the cast operation.</param>
        /// <param name="type">The type for the cast operation.</param>
        /// <returns><see cref="QueryScalarValue"/> which is cast to the appropriate type</returns>
        public QueryScalarValue Cast(QueryScalarValue source, QueryScalarType type)
        {
            var targetType = (QueryClrPrimitiveType)type;

            try
            {
                return this.EvaluateCast(source.Value, targetType);
            }
            catch (InvalidCastException)
            {
                return targetType.CreateValue(null);
            }
        }

        /// <summary>
        /// Computes the Maximum value of a collection as a <see cref="QueryScalarValue"/>.
        /// </summary>
        /// <param name="collection">The collection of values.</param>
        /// <returns>The max value.</returns>
        public QueryScalarValue Max(QueryCollectionValue collection)
        {
            ExceptionUtilities.Assert(collection.Type.ElementType is QueryScalarType, "Max can only be applied on scalar types");
            ExceptionUtilities.Assert(collection.Elements.Any(v => !v.IsNull), "Collection must contain at least one non-null value in a Max operation");

            var type = collection.Type.ElementType as QueryScalarType;

            return type.CreateValue(collection.Elements.Cast<QueryScalarValue>().Max(sv => sv.Value));
        }

        /// <summary>
        /// Computes the Maximum value of a collection as a <see cref="QueryScalarValue"/>.
        /// </summary>
        /// <param name="collection">The collection of values.</param>
        /// <returns>The min value.</returns>
        public QueryScalarValue Min(QueryCollectionValue collection)
        {
            ExceptionUtilities.Assert(collection.Type.ElementType is QueryScalarType, "Min can only be applied on scalar types");
            ExceptionUtilities.Assert(collection.Elements.Any(v => !v.IsNull), "Collection must contain at least one non-null value in a Min operation");

            var type = collection.Type.ElementType as QueryScalarType;

            return type.CreateValue(collection.Elements.Cast<QueryScalarValue>().Min(sv => sv.Value));
        }

        /// <summary>
        /// Filters the specified source using given predicate.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// Filtered collection (including elements where the predicate matches).
        /// </returns>
        /// <remarks>The value can be store-dependent w.r.t. handling of NULL values of the input collection
        /// and/or result of the predicate.</remarks>
        public QueryCollectionValue Filter(QueryCollectionValue source, Func<QueryValue, QueryScalarValue> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            IEnumerable<QueryValue> result = null;
            if (!source.IsNull)
            {
                result = source.Elements.Where(
                    c =>
                    {
                        var evaluatedValue = predicate(c);
                        if (evaluatedValue.IsNull)
                        {
                            return false;
                        }

                        return (bool)evaluatedValue.Value;
                    });
            }

            return new QueryCollectionValue(source.Type, this, QueryError.GetErrorFromValues(result), result, source.IsSorted);
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> indicating whether all elements in a source collection
        /// match a given predicate.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> indicating whether all elements in a source collection match
        /// a given predicate.
        /// </returns>
        /// <remarks>The value can be store-dependent w.r.t. handling of NULL values of the input collection
        /// and/or result of the predicate.</remarks>
        public QueryScalarValue All<TElement>(IEnumerable<TElement> source, Func<TElement, QueryScalarValue> predicate)
        {
            if (source == null)
            {
                return this.BooleanType.CreateValue(false);
            }
            else
            {
                return this.BooleanType.CreateValue(source.All(c => (bool)predicate(c).Value));
            }
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> indicating whether any element in a source collection
        /// matches a given predicate.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> indicating whether any element in a source collection matches
        /// a given predicate.
        /// </returns>
        /// <remarks>The value can be store-dependent w.r.t. handling of NULL values of the input collection
        /// and/or result of the predicate.</remarks>
        public QueryScalarValue Any<TElement>(IEnumerable<TElement> source, Func<TElement, QueryScalarValue> predicate)
        {
            if (source == null)
            {
                return this.BooleanType.CreateValue(false);
            }
            else
            {
                return this.BooleanType.CreateValue(source.Any(c => (bool)predicate(c).Value));
            }
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
            return this.Compare(value, otherValue) == 0;
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
            var t1 = (QueryClrPrimitiveType)value.Type;
            var t2 = (QueryClrPrimitiveType)otherValue.Type;
            object v1 = value.Value;
            object v2 = otherValue.Value;

            if (this.IsSpatialType(t1))
            {
                ExceptionUtilities.Assert(this.IsSpatialType(t2), "Because CLR types match, both values should both be spatial. Type was '{0}'", t2);
                ExceptionUtilities.CheckObjectNotNull(this.SpatialEqualityComparer, "Cannot compare spatial values without a spatial equality-comparer");
                return this.SpatialEqualityComparer.Equals(v1, v2) ? 0 : 2;
            }

            if (t1.ClrType != t2.ClrType)
            {
                Type commonClrType = LinqTypeSemantics.GetCommonType(t1.ClrType, t2.ClrType);
                commonClrType = Nullable.GetUnderlyingType(commonClrType) ?? commonClrType;

                if (v1 != null)
                {
                    v1 = ArithmeticEvaluationHelper.ChangeType(v1, commonClrType);
                }

                if (v2 != null)
                {
                    v2 = ArithmeticEvaluationHelper.ChangeType(v2, commonClrType);
                }
            }

            if (v1 == null)
            {
                if (v2 == null)
                {
                    // both null - are equal
                    return 0;
                }
                else
                {
                    // first null, second not-null
                    return -1;
                }
            }
            else
            {
                if (v2 == null)
                {
                    // first not null, second null
                    return 1;
                }
            }

            var bytes1 = v1 as byte[];
            if (bytes1 != null)
            {
                var bytes2 = (byte[])v2;

                return CompareByteArrays(bytes1, bytes2);
            }

            IComparable cv1 = (IComparable)v1;
            return cv1.CompareTo(v2);
        }

        /// <summary>
        /// Gets the common type to which both types can be promoted.
        /// </summary>
        /// <param name="leftType">First type.</param>
        /// <param name="rightType">Second type.</param>
        /// <returns>
        /// Common type to which both types can be promoted. Throws if unable to find a common type.
        /// </returns>
        public QueryScalarType GetCommonType(QueryScalarType leftType, QueryScalarType rightType)
        {
            Type commonClrType = LinqTypeSemantics.GetCommonType(((QueryClrPrimitiveType)leftType).ClrType, ((QueryClrPrimitiveType)rightType).ClrType);

            return new QueryClrPrimitiveType(commonClrType, this);
        }

        /// <summary>
        /// Removes any length constraints from the type
        /// </summary>
        /// <param name="type">The type from which to remove the constraints</param>
        /// <returns>The new type with length constraints removed.</returns>
        public QueryScalarType RemoveLengthConstraints(QueryScalarType type)
        {
            return type;
        }

        /// <summary>
        /// Determines whether a typed data can do certain operation
        /// </summary>
        /// <param name="operation">the operation</param>
        /// <param name="sourceType">the type of data to operate on</param>
        /// <returns>Value <c>true</c> if operation can be performed; otherwise, <c>false</c>.</returns>
        public bool Supports(QueryUnaryOperation operation, QueryScalarType sourceType)
        {
            switch (operation)
            {
                case QueryUnaryOperation.LogicalNegate:
                    return ((IQueryClrType)sourceType).ClrType == typeof(bool);
                default:
                    throw new TaupoNotSupportedException("Unsupported query unary operation.");
            }
        }

        /// <summary>
        /// Determines whether a type supports ordering. 
        /// </summary>
        /// <param name="sourceType">the type of data to operation on</param>
        /// <returns>Value <c>true</c> if ordering can be performed; otherwise, <c>false</c>.</returns>
        public bool SupportsOrderComparison(QueryScalarType sourceType)
        {
            // Ordering is supported for all primitive types in the clr 
            // Note, this is ordering in the sense of "orderby p.ProductName", not Where(p => p.ProductName < "foo" ). 
            var spatialType = sourceType as QueryClrSpatialType;

            if (spatialType != null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether a typed data can do certain operation with another typed data
        /// </summary>
        /// <param name="operation">the operation</param>
        /// <param name="sourceType">the type of data to operation on</param>
        /// <param name="otherType">the other type</param>
        /// <returns>Value <c>true</c> if operation can be performed; otherwise, <c>false</c>.</returns>
        public virtual bool Supports(QueryBinaryOperation operation, QueryScalarType sourceType, QueryScalarType otherType)
        {
            var sourceQueryClrType = (IQueryClrType)sourceType;
            var otherQueryClrType = (IQueryClrType)otherType;
            var sourceClrType = sourceQueryClrType.ClrType;
            var otherClrType = otherQueryClrType.ClrType;

            switch (operation)
            {
                case QueryBinaryOperation.Add:
                case QueryBinaryOperation.Divide:
                case QueryBinaryOperation.Modulo:
                case QueryBinaryOperation.Multiply:
                case QueryBinaryOperation.Subtract:
                    return LinqTypeSemantics.IsNumeric(sourceClrType) &&
                           LinqTypeSemantics.IsNumeric(otherClrType) &&
                           this.HaveCommonType(sourceClrType, otherClrType);

                case QueryBinaryOperation.BitwiseAnd:
                case QueryBinaryOperation.BitwiseExclusiveOr:
                case QueryBinaryOperation.BitwiseOr:
                    return LinqTypeSemantics.IsIntegralType(sourceClrType) &&
                           LinqTypeSemantics.IsIntegralType(otherClrType) &&
                           this.HaveCommonType(sourceClrType, otherClrType);

                case QueryBinaryOperation.LogicalAnd:
                case QueryBinaryOperation.LogicalOr:
                    return sourceClrType == typeof(bool) && otherClrType == typeof(bool);

                case QueryBinaryOperation.EqualTo:
                case QueryBinaryOperation.NotEqualTo:
                    return this.HaveCommonType(sourceClrType, otherClrType);

                case QueryBinaryOperation.LessThan:
                case QueryBinaryOperation.LessThanOrEqualTo:
                case QueryBinaryOperation.GreaterThan:
                case QueryBinaryOperation.GreaterThanOrEqualTo:
                    if (sourceClrType == typeof(byte[]) || otherClrType == typeof(byte[])
                        || sourceClrType == typeof(string) || otherClrType == typeof(string)
                        || sourceClrType == typeof(bool) || otherClrType == typeof(bool)
                        || sourceClrType == typeof(bool?) || otherClrType == typeof(bool?)
                        || this.IsSpatialType(sourceQueryClrType) || this.IsSpatialType(otherQueryClrType))
                    {
                        return false;
                    }
                    else
                    {
                        return this.HaveCommonType(sourceClrType, otherClrType);
                    }

                case QueryBinaryOperation.Concat:
                    return sourceClrType == typeof(string) || otherClrType == typeof(string);

                default:
                    throw new TaupoNotSupportedException("Unsupported query binary operation.");
            }
        }

        /// <summary>
        /// Do the specified UnaryOperation for the value and returns the result.
        /// </summary>
        /// <param name="operation">The binary operation to perform.</param>
        /// <param name="value">The value.</param>
        /// <returns>Result of the operation.</returns>
        public virtual QueryScalarValue Evaluate(QueryUnaryOperation operation, QueryScalarValue value)
        {
            Func<object, Type, object> evaluationMethod = null;

            switch (operation)
            {
                case QueryUnaryOperation.LogicalNegate:
                    return this.BooleanType.CreateValue(!(bool)value.Value);
                case QueryUnaryOperation.Negate:
                    evaluationMethod = ArithmeticEvaluationHelper.Negate;
                    break;
                case QueryUnaryOperation.BitwiseNot:
                    evaluationMethod = ArithmeticEvaluationHelper.BitwiseNot;
                    break;
                default:
                    throw new TaupoNotSupportedException("Unsupported query unary operation.");
            }

            ExceptionUtilities.Assert(evaluationMethod != null, "evaluationMethod should not be null.");
            Type clrType = ((QueryClrPrimitiveType)value.Type).ClrType;
            object result = evaluationMethod(value.Value, clrType);

            return value.Type.CreateValue(result);
        }

        /// <summary>
        /// Do the specified BinaryOperation for the two values and returns the result.
        /// </summary>
        /// <param name="operation">The binary operation to perform.</param>
        /// <param name="firstValue">The first value.</param>
        /// <param name="secondValue">The second value.</param>
        /// <returns>Result of the operation.</returns>
        public virtual QueryScalarValue Evaluate(QueryBinaryOperation operation, QueryScalarValue firstValue, QueryScalarValue secondValue)
        {
            Func<object, object, Type, object> evaluationMethod = null;

            switch (operation)
            {
                case QueryBinaryOperation.Add:
                    evaluationMethod = ArithmeticEvaluationHelper.Add;
                    break;
                case QueryBinaryOperation.Subtract:
                    evaluationMethod = ArithmeticEvaluationHelper.Subtract;
                    break;
                case QueryBinaryOperation.Multiply:
                    evaluationMethod = ArithmeticEvaluationHelper.Multiply;
                    break;
                case QueryBinaryOperation.Divide:
                    evaluationMethod = ArithmeticEvaluationHelper.Divide;
                    break;
                case QueryBinaryOperation.Modulo:
                    evaluationMethod = ArithmeticEvaluationHelper.Modulo;
                    break;
                case QueryBinaryOperation.BitwiseAnd:
                    evaluationMethod = ArithmeticEvaluationHelper.BitwiseAnd;
                    break;
                case QueryBinaryOperation.BitwiseOr:
                    evaluationMethod = ArithmeticEvaluationHelper.BitwiseOr;
                    break;
                case QueryBinaryOperation.BitwiseExclusiveOr:
                    evaluationMethod = ArithmeticEvaluationHelper.ExclusiveOr;
                    break;

                case QueryBinaryOperation.LogicalAnd:
                    return this.BooleanType.CreateValue((bool)firstValue.Value && (bool)secondValue.Value);
                case QueryBinaryOperation.LogicalOr:
                    return this.BooleanType.CreateValue((bool)firstValue.Value || (bool)secondValue.Value);

                case QueryBinaryOperation.EqualTo:
                    return this.BooleanType.CreateValue(this.AreEqual(firstValue, secondValue));
                case QueryBinaryOperation.NotEqualTo:
                    return this.BooleanType.CreateValue(!this.AreEqual(firstValue, secondValue));

                case QueryBinaryOperation.LessThan:
                    return this.BooleanType.CreateValue(this.Compare(firstValue, secondValue) < 0);
                case QueryBinaryOperation.LessThanOrEqualTo:
                    return this.BooleanType.CreateValue(this.Compare(firstValue, secondValue) <= 0);
                case QueryBinaryOperation.GreaterThan:
                    return this.BooleanType.CreateValue(this.Compare(firstValue, secondValue) > 0);
                case QueryBinaryOperation.GreaterThanOrEqualTo:
                    return this.BooleanType.CreateValue(this.Compare(firstValue, secondValue) >= 0);
                
                case QueryBinaryOperation.Concat:
                    return new QueryClrPrimitiveType(typeof(string), this).CreateValue(firstValue.Value.ToString() + secondValue.Value.ToString());

                default:
                    throw new TaupoNotSupportedException("Unsupported query binary operation.");
            }

            ExceptionUtilities.Assert(evaluationMethod != null, "evaluationMethod should not be null.");
            return this.ExecuteBinaryOperation(firstValue, secondValue, evaluationMethod);
        }

        /// <summary>
        /// Evaluates built in function with the given namespace and name.
        /// </summary>
        /// <param name="resultType">The function result type.</param>
        /// <param name="functionNamespace">The function namespace.</param>
        /// <param name="functionName">The function name.</param>
        /// <param name="arguments">Arguments for the function call.</param>
        /// <returns>Query value which is the result of function evaluation.</returns>
        public virtual QueryValue EvaluateBuiltInFunction(QueryType resultType, string functionNamespace, string functionName, params QueryValue[] arguments)
        {
            throw new TaupoInvalidOperationException("Clr Query Evaluation can't evaluate built-in functions. Function: '" + functionName + "'.");
        }

        /// <summary>
        /// Evaluates function.
        /// </summary>
        /// <param name="resultType">The function result type.</param>
        /// <param name="customFunction">The function to evaluate.</param>
        /// <param name="arguments">The arguments for the function call.</param>
        /// <returns>Query value which is the result of function evaluation.</returns>
        public virtual QueryValue EvaluateFunction(QueryType resultType, Function customFunction, params QueryValue[] arguments)
        {
            throw new TaupoInvalidOperationException("Clr Query Evaluation can't evaluate custom functions. Function: '" + customFunction.FullName + "'.");
        }

        /// <summary>
        /// Evaluates function import.
        /// </summary>
        /// <param name="resultType">The function import result type.</param>
        /// <param name="functionImport">The function import to evaluate.</param>
        /// <param name="arguments">The arguments for the function import call.</param>
        /// <returns>Query value which is the result of function import evaluation.</returns>
        public virtual QueryValue EvaluateFunctionImport(QueryType resultType, FunctionImport functionImport, params QueryValue[] arguments)
        {
            throw new TaupoInvalidOperationException("Clr Query Evaluation can't evaluate function imports. Function import: '" + functionImport.Name + "'.");
        }

        /// <summary>
        /// Evaluates member property of a spatial instance.
        /// </summary>
        /// <param name="instance">The instance of query value object</param>
        /// <param name="resultType">The property result type.</param>
        /// <param name="memberPropertyName">The member property name to evaluate.</param>
        /// <returns>Query value which is the result of method evaluation.</returns>
        public virtual QueryValue EvaluateMemberProperty(QueryValue instance, QueryType resultType, string memberPropertyName)
        {
            var clrInstance = ((QueryScalarValue)instance).Value;
            var clrType = clrInstance.GetType();
            return QueryType.UnresolvedPrimitive.CreateValue(
                        clrType.GetProperty(memberPropertyName).GetValue(clrInstance, null)); 
        }

        /// <summary>
        /// Evaluates member method of a spatial instance.
        /// </summary>
        /// <param name="instance">The instance of query value object</param>
        /// <param name="resultType">The function result type.</param>
        /// <param name="methodName">The member method to evaluate.</param>
        /// <param name="arguments">Arguments for the function call.</param>
        /// <returns>Query value which is the result of function evaluation.</returns>
        public virtual QueryValue EvaluateMemberMethod(QueryValue instance, QueryType resultType, string methodName, params QueryValue[] arguments)
        {
            var primitiveResultType = resultType as QueryScalarType;
            ExceptionUtilities.CheckObjectNotNull(primitiveResultType, "Only query primitive type is supported as member method's result type!");

            var clrInstance = ((QueryScalarValue)instance).Value;
            var instanceType = ((IQueryClrType)instance.Type).ClrType;

            var argTypes = arguments.Select(a => a.Type).Cast<IQueryClrType>().Select(t => t.ClrType);
            var argValues = arguments.Cast<QueryScalarValue>().Select(a => a.Value).ToArray();
            
            var methodToInvoke = this.FindMemberMethod(instanceType, methodName, argTypes);
            object returnValue = this.InvokeMemberMethod(clrInstance, methodToInvoke, argValues);

            return primitiveResultType.CreateValue(returnValue);
        }

        /// <summary>
        /// Compares the byte arrays lexicographically.
        /// </summary>
        /// <param name="array1">The first array.</param>
        /// <param name="array2">The second array.</param>
        /// <returns>-1 if first array is lexicographically before second array, +1 if it's after
        /// and 0 if they are equal.</returns>
        internal static int CompareByteArrays(byte[] array1, byte[] array2)
        {
            ExceptionUtilities.CheckArgumentNotNull(array1, "array1");
            ExceptionUtilities.CheckArgumentNotNull(array2, "array2");

            int compareLength = Math.Min(array1.Length, array2.Length);
            for (int i = 0; i < compareLength; ++i)
            {
                if (array1[i] < array2[i])
                {
                    return -1;
                }
                else if (array1[i] > array2[i])
                {
                    return 1;
                }
            }

            if (array1.Length < array2.Length)
            {
                return -1;
            }

            if (array1.Length > array2.Length)
            {
                return 1;
            }

            return 0;
        }
        
        /// <summary>
        /// Helper method for determining if a type is spatial
        /// </summary>
        /// <param name="queryType">The query type to check</param>
        /// <returns>Whether or not the type is spatial</returns>
        protected internal bool IsSpatialType(IQueryClrType queryType)
        {
            if (queryType is QueryClrSpatialType)
            {
                return true;
            }

            if (this.SpatialTypeResolver != null)
            {
                return this.SpatialTypeResolver.IsSpatial(queryType.ClrType);
            }

            return false;
        }

        /// <summary>
        /// Finds the member method. This can be over-ridden to support extension methods.
        /// </summary>
        /// <param name="instanceType">Type of the instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="argTypes">The argument types.</param>
        /// <returns>The member method info.</returns>
        protected virtual MethodInfo FindMemberMethod(Type instanceType, string methodName, IEnumerable<Type> argTypes)
        {
            ExceptionUtilities.CheckArgumentNotNull(instanceType, "instanceType");
            ExceptionUtilities.CheckArgumentNotNull(methodName, "methodName");
            ExceptionUtilities.CheckArgumentNotNull(argTypes, "argTypes");

            return instanceType.GetMethod(methodName, argTypes.ToArray());
        }

        /// <summary>
        /// Invokes the member method.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <param name="method">The method.</param>
        /// <param name="argValues">The argument values.</param>
        /// <returns>The result of invoking the method</returns>
        protected virtual object InvokeMemberMethod(object instance, MethodInfo method, object[] argValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(method, "method");
            ExceptionUtilities.CheckArgumentNotNull(argValues, "argValues");

            if (method.IsStatic)
            {
                return method.Invoke(null, argValues);
            }
            else
            {
                return method.Invoke(instance, argValues);
            }
        }
        
        private bool HaveCommonType(Type t1, Type t2)
        {
            Type commonClrType;
            return LinqTypeSemantics.TryGetCommonType(t1, t2, out commonClrType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exceptions.")]
        private QueryScalarValue ExecuteBinaryOperation(QueryScalarValue firstValue, QueryScalarValue secondValue, Func<object, object, Type, object> evaluationMethod)
        {
            var t1 = (QueryClrPrimitiveType)firstValue.Type;
            var t2 = (QueryClrPrimitiveType)secondValue.Type;
            object v1 = firstValue.Value;
            object v2 = secondValue.Value;

            QueryClrPrimitiveType commonType = t1;

            try
            {
                commonType = (QueryClrPrimitiveType)this.GetCommonType(t1, t2);

                if (v1 == null || v2 == null)
                {
                    return commonType.CreateErrorValue(new QueryError("Null values are not supported for the operation."));
                }

                var result = evaluationMethod(v1, v2, commonType.ClrType);
                return commonType.CreateValue(result);
            }
            catch (Exception ex)
            {
                return commonType.CreateErrorValue(new QueryError(ex.Message));
            }
        }

        private QueryScalarValue EvaluateCast(object value, QueryClrPrimitiveType targetType)
        {
            object result = null;

            if (value == null)
            {
                return targetType.NullValue;
            }

            if (targetType.ClrType == typeof(bool))
            {
                result = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(byte[]))
            {
                // TODO: figure out what's the intended behavior here
                ////result = Convert.ToBase64String(value as byte[], Base64FormattingOptions.None);                
                result = value as byte[];
            }

            if (targetType.ClrType == typeof(byte))
            {
                result = Convert.ToByte(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(sbyte))
            {
                result = Convert.ToSByte(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(char))
            {
                result = Convert.ToChar(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(decimal))
            {
                result = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(double))
            {
                result = Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(float))
            {
                result = Convert.ToSingle(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(int))
            {
                result = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(uint))
            {
                result = Convert.ToUInt32(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(long))
            {
                result = Convert.ToInt64(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(ulong))
            {
                result = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(short))
            {
                result = Convert.ToInt16(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(ushort))
            {
                result = Convert.ToUInt16(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(string))
            {
                result = Convert.ToString(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(DateTime))
            {
                result = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
            }

            if (targetType.ClrType == typeof(Guid))
            {
                Type valueType = null;
                if (value != null)
                {
                    valueType = value.GetType();
                }

                if (valueType == typeof(Guid) || valueType == typeof(Guid?))
                {
                    result = (Guid)value;
                }
                else if (value != null)
                {
                    throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "Cannot cast value of type '{0}' to guid", value.GetType()));
                }
            }

            return targetType.CreateValue(result);
        }
    }
}