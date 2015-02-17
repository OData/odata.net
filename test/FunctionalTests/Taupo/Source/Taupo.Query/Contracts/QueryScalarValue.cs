//---------------------------------------------------------------------
// <copyright file="QueryScalarValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Result of a query evaluation which is a scalar value.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes", Justification = "By design.")]
    public class QueryScalarValue : QueryValue
    {
        /// <summary>
        /// Initializes a new instance of the QueryScalarValue class.
        /// </summary>
        /// <param name="type">The type of the value.</param>
        /// <param name="value">The value.</param>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public QueryScalarValue(QueryScalarType type, object value, QueryError evaluationError, IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationError, evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            this.Type = type;
            this.Value = value;
        }

        /// <summary>
        /// Gets the comparer used to order scalar values.
        /// </summary>
        public static IComparer<QueryScalarValue> Comparer
        {
            get { return TheComparer.Instance; }
        }

        /// <summary>
        /// Gets the scalar type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Must be the same as the base class.")]
        public new QueryScalarType Type { get; private set; }

        /// <summary>
        /// Gets the scalar value.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        public override bool IsNull
        {
            get { return this.Value == null /*|| this.Value == DBNull.Value*/; }
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> value indicating whether two values are equal.
        /// </summary>
        /// <param name="otherValue">The second value.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of comparison.
        /// </returns>
        public QueryScalarValue EqualTo(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.EqualTo);
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> value indicating whether two values are not equal.
        /// </summary>
        /// <param name="otherValue">The query scalar value.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of comparison.
        /// </returns>
        public QueryScalarValue NotEqualTo(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.NotEqualTo);
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> value indicating whether first value is less than the second one.
        /// </summary>
        /// <param name="otherValue">The second value.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of comparison.
        /// </returns>
        public QueryScalarValue LessThan(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.LessThan);
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> value indicating whether first value is greater than the second one.
        /// </summary>
        /// <param name="otherValue">The second value.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of comparison.
        /// </returns>
        public QueryScalarValue GreaterThan(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.GreaterThan);
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> value indicating whether first value is less than or equal to the second one.
        /// </summary>
        /// <param name="otherValue">The second value.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of comparison.
        /// </returns>
        public QueryScalarValue LessThanOrEqual(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.LessThanOrEqualTo);
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> value indicating whether first value is greater than or equal to the second one.
        /// </summary>
        /// <param name="otherValue">The second value.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of comparison.
        /// </returns>
        public QueryScalarValue GreaterThanOrEqual(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.GreaterThanOrEqualTo);
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> value representing the result of boolean conjunction (AND) of two values.
        /// </summary>
        /// <param name="otherValue">The second boolean value.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of boolean operation.
        /// </returns>
        public QueryScalarValue AndAlso(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.LogicalAnd);
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> value representing the result of boolean alternative (OR) of two values.
        /// </summary>
        /// <param name="otherValue">The second boolean value.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of boolean operation.
        /// </returns>
        public QueryScalarValue OrElse(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.LogicalOr);
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> value representing the result of boolean negation (NOT) of the value.
        /// </summary>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of boolean operation.
        /// </returns>
        public QueryScalarValue LogicalNegate()
        {
            if (this.EvaluationError != null)
            {
                return this;
            }

            return this.EvaluationStrategy.Evaluate(QueryUnaryOperation.LogicalNegate, this);
        }

        /// <summary>
        /// Adds the value to the specified other value.
        /// </summary>
        /// <param name="otherValue">The other value.</param>
        /// <returns>Result of the arithmetic operation.</returns>
        public QueryScalarValue Add(QueryScalarValue otherValue)
        {
            var evaluationResult = this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.Add);

            // Enums do not exist as a concept at the store level, so whenever we evaluate something on the store side, we have to 
            // simulate the materialization of the result value to an enum.
            var materializedResult = evaluationResult.MaterializeValueIfEnum(this.Type);

            return materializedResult;
        }

        /// <summary>
        /// Divides the value by the the specified other value.
        /// </summary>
        /// <param name="otherValue">The other value.</param>
        /// <returns>Result of the arithmetic operation.</returns>
        public QueryScalarValue Divide(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.Divide);
        }

        /// <summary>
        /// Gets the result of modulo operation of the value by the the specified other value.
        /// </summary>
        /// <param name="otherValue">The other value.</param>
        /// <returns>Result of the arithmetic operation.</returns>
        public QueryScalarValue Modulo(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.Modulo);
        }

        /// <summary>
        /// Multiplies the value by the the specified other value.
        /// </summary>
        /// <param name="otherValue">The other value.</param>
        /// <returns>Result of the arithmetic operation.</returns>
        public QueryScalarValue Multiply(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.Multiply);
        }

        /// <summary>
        /// Subtracts the specified value from this value.
        /// </summary>
        /// <param name="otherValue">The other value.</param>
        /// <returns>Result of the arithmetic operation.</returns>
        public QueryScalarValue Subtract(QueryScalarValue otherValue)
        {
            var evaluationResult = this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.Subtract);

            // Enums do not exist as a concept at the store level, so whenever we evaluate something on the store side, we have to 
            // simulate the materialization of the result value to an enum.
            var materializedResult = evaluationResult.MaterializeValueIfEnum(this.Type);

            return materializedResult;
        }

        /// <summary>
        /// Gets the result of bitwise OR operation on this value and specified value.
        /// </summary>
        /// <param name="otherValue">The other value.</param>
        /// <returns>Result of the arithmetic operation.</returns>
        public QueryValue BitwiseOr(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.BitwiseOr);
        }

        /// <summary>
        /// Gets the result of bitwise AND operation on this value and specified value.
        /// </summary>
        /// <param name="otherValue">The other value.</param>
        /// <returns>Result of the arithmetic operation.</returns>
        public QueryValue BitwiseAnd(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.BitwiseAnd);
        }

        /// <summary>
        /// Gets the result of exclusive OR (XOR) operation on this value and specified value.
        /// </summary>
        /// <param name="otherValue">The other value.</param>
        /// <returns>Result of the arithmetic operation.</returns>
        public QueryValue ExclusiveOr(QueryScalarValue otherValue)
        {
            return this.EvaluateBinaryOperation(otherValue, QueryBinaryOperation.BitwiseExclusiveOr);
        }

        /// <summary>
        /// Casts a <see cref="QueryScalarValue"/> to a <see cref="QueryScalarType"/>. The cast will return the value type cast to the new type.
        /// </summary>
        /// <param name="type">The type for the cast operation.</param>
        /// <returns><see cref="QueryScalarValue"/> which is cast to the appropriate type</returns>
        public override QueryValue Cast(QueryType type)
        {
            bool wasNullBefore = this.IsNull;
            var typeToCastTo = type as QueryScalarType;
            var clrType = ((IQueryClrType)typeToCastTo).ClrType;
            QueryScalarValue result = null;

            if ((clrType.GetGenericArguments().Length == 1 && clrType.GetGenericArguments()[0].IsEnum()) || clrType.IsEnum())
            {
                result = typeToCastTo.CreateValue(this.Value);
            }
            else
            {
                result = this.EvaluationStrategy.Cast(this, typeToCastTo);
            }

            // if the value wasn't null before, but is null now after the cast
            if (result.IsNull && !wasNullBefore)
            {
                return type.CreateErrorValue(new QueryError("Invalid Cast : Cannot perform Cast to the specified type"));
            }

            result = result.MaterializeValueIfEnum(typeToCastTo);

            return result;
        }

        /// <summary>
        /// Checks if a <see cref="QueryValue"/> is of a particular <see cref="QueryType"/>. This operation will return a true if the value is of the specified type.
        /// </summary>
        /// <param name="type">The type for the IsOf operation.</param>
        /// <param name="performExactMatch">Determines if an exact match needs to be performed.</param>
        /// <returns>A <see cref="QueryValue"/> containing true or false depending on whether the value is of the specified type or not.</returns>
        public override QueryValue IsOf(QueryType type, bool performExactMatch)
        {
            QueryValue result = this.EvaluationStrategy.Cast(this, type as QueryScalarType);

            if (result.IsNull)
            {
                return this.Type.EvaluationStrategy.BooleanType.CreateValue(false);
            }

            if (performExactMatch && (this.Type != type))
            {
                return this.Type.EvaluationStrategy.BooleanType.CreateValue(false);
            }

            return this.Type.EvaluationStrategy.BooleanType.CreateValue(true);
        }

        /// <summary>
        /// Converts the <see cref="QueryValue"/> to a particular <see cref="QueryType"/>.
        /// </summary>
        /// <param name="type">The type for the As operation.</param>
        /// <returns>The <see cref="QueryValue"/> converted to the specified type if successful. Returns null if this operation fails.</returns>
        public override QueryValue TreatAs(QueryType type)
        {
            return this.EvaluationStrategy.Cast(this, type as QueryScalarType);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.EvaluationError != null)
            {
                return "Scalar Value Error=" + this.EvaluationError + ", Type=" + this.Type.StringRepresentation;
            }
            else
            {
                return "Scalar Value=" + GetDebugValue(this.Value) + ", Type=" + this.Type.StringRepresentation;
            }
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query value.</param>
        /// <returns>The result of visiting this query value.</returns>
        public override TResult Accept<TResult>(IQueryValueVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        internal static string GetDebugValue(object value)
        {
            if (value != null)
            {
                var valueType = value.GetType();

                if (valueType == typeof(DateTime))
                {
                    var dateTimeValue = (DateTime)value;
                    return string.Format(CultureInfo.InvariantCulture, "({0}, Ticks={1}, Kind={2})", dateTimeValue.ToString(CultureInfo.InvariantCulture), dateTimeValue.Ticks.ToString(CultureInfo.InvariantCulture), dateTimeValue.Kind);
                }
                else if (valueType == typeof(DateTimeOffset))
                {
                    var dateTimeOffsetValue = (DateTimeOffset)value;
                    return string.Format(CultureInfo.InvariantCulture, "({0}, Ticks={1})", dateTimeOffsetValue.ToString(CultureInfo.InvariantCulture), dateTimeOffsetValue.Ticks.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    return value.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <returns>Type of the value.</returns>
        protected override QueryType GetTypeInternal()
        {
            return this.Type;
        }

        /// <summary>
        /// Compares the primitive value to another value and returns their relative ordering.
        /// </summary>
        /// <param name="otherValue">The other scalar value.</param>
        /// <returns>
        /// Integer which is less than zero if this value is less than the other value, 0 if they are equal,
        /// greater than zero if this value is greater than the other value
        /// </returns>
        private int CompareTo(QueryScalarValue otherValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(otherValue, "otherValue");

            return this.EvaluationStrategy.Compare(this, otherValue);
        }

        private QueryScalarValue EvaluateBinaryOperation(QueryScalarValue otherValue, QueryBinaryOperation operation)
        {
            ExceptionUtilities.CheckArgumentNotNull(otherValue, "otherValue");

            var result = this.EvaluationStrategy.Evaluate(operation, this, otherValue);

            var error = QueryError.Combine(this.EvaluationError, otherValue.EvaluationError);
            if (error != null)
            {
                result.EvaluationError = error;
            }

            return result;
        }

        /// <summary>
        /// Comparer for QueryScalarValues
        /// </summary>
        private sealed class TheComparer : IComparer<QueryScalarValue>
        {
            /// <summary>
            /// Initializes static members of the TheComparer class.
            /// </summary>
            static TheComparer()
            {
                Instance = new TheComparer();
            }

            /// <summary>
            /// Gets the singleton instance.
            /// </summary>
            public static IComparer<QueryScalarValue> Instance { get; private set; }

            /// <summary>
            /// Computes the relative ordering of two primitive values.
            /// </summary>
            /// <param name="firstValue">The first value.</param>
            /// <param name="secondValue">The second value.</param>
            /// <returns>
            /// Integer which is less than zero if first value is less than the second value, 0 if they are equal,
            /// greater than zero if the first value is greater than the second value.
            /// </returns>
            public int Compare(QueryScalarValue firstValue, QueryScalarValue secondValue)
            {
                return firstValue.CompareTo(secondValue);
            }
        }
    }
}
