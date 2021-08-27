//---------------------------------------------------------------------
// <copyright file="ArithmeticEvaluationHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq.Expressions;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Helper methods for evaluation of arithmetic operations against untyped typed values.
    /// </summary>
    public static class ArithmeticEvaluationHelper
    {
        private static Dictionary<string, Func<object, object>> unaryDelegateCache = new Dictionary<string, Func<object, object>>();
        private static Dictionary<string, Func<object, object, object>> binaryDelegateCache = new Dictionary<string, Func<object, object, object>>();

        /// <summary>
        /// Negate the untyped value and returns result
        /// </summary>
        /// <param name="argument">Argument of the operation</param>
        /// <param name="resultType">The clr type of result</param>
        /// <returns>Result of operation.</returns>
        public static object Negate(object argument, Type resultType)
        {
            return EvaluateUnaryOperation(argument, Expression.NegateChecked, resultType);
        }

        /// <summary>
        /// Performs bitwise not on the untyped value and returns result
        /// </summary>
        /// <param name="argument">Argument of the operation</param>
        /// <param name="resultType">The clr type of result</param>
        /// <returns>Result of operation.</returns>
        public static object BitwiseNot(object argument, Type resultType)
        {
            return EvaluateUnaryOperation(argument, Expression.Not, resultType);
        }

        /// <summary>
        /// Adds two untyped values and returns their result.
        /// </summary>
        /// <param name="leftArgument">Left argument to the operation.</param>
        /// <param name="rightArgument">Right argument to the operation.</param>
        /// <param name="commonType">Common CLR type for both input types (will be the type of the result of the operation)</param>
        /// <returns>Result of arithmetic operation.</returns>
        public static object Add(object leftArgument, object rightArgument, Type commonType)
        {
            return EvaluateBinaryOperation(leftArgument, rightArgument, Expression.AddChecked, commonType);
        }

        /// <summary>
        /// Subtracts two untyped values and returns their result.
        /// </summary>
        /// <param name="leftArgument">Left argument to the operation.</param>
        /// <param name="rightArgument">Right argument to the operation.</param>
        /// <param name="commonType">Common CLR type for both input types (will be the type of the result of the operation)</param>
        /// <returns>Result of arithmetic operation.</returns>
        public static object Subtract(object leftArgument, object rightArgument, Type commonType)
        {
            return EvaluateBinaryOperation(leftArgument, rightArgument, Expression.SubtractChecked, commonType);
        }

        /// <summary>
        /// Multiplies two untyped values and returns their result.
        /// </summary>
        /// <param name="leftArgument">Left argument to the operation.</param>
        /// <param name="rightArgument">Right argument to the operation.</param>
        /// <param name="commonType">Common CLR type for both input types (will be the type of the result of the operation)</param>
        /// <returns>Result of arithmetic operation.</returns>
        public static object Multiply(object leftArgument, object rightArgument, Type commonType)
        {
            return EvaluateBinaryOperation(leftArgument, rightArgument, Expression.MultiplyChecked, commonType);
        }

        /// <summary>
        /// Divides two untyped values and returns their result.
        /// </summary>
        /// <param name="leftArgument">Left argument to the operation.</param>
        /// <param name="rightArgument">Right argument to the operation.</param>
        /// <param name="commonType">Common CLR type for both input types (will be the type of the result of the operation)</param>
        /// <returns>Result of arithmetic operation.</returns>
        public static object Divide(object leftArgument, object rightArgument, Type commonType)
        {
            return EvaluateBinaryOperation(leftArgument, rightArgument, Expression.Divide, commonType);
        }

        /// <summary>
        /// Performs modulo operation on two untyped values and returns their result.
        /// </summary>
        /// <param name="leftArgument">Left argument to the operation.</param>
        /// <param name="rightArgument">Right argument to the operation.</param>
        /// <param name="commonType">Common CLR type for both input types (will be the type of the result of the operation)</param>
        /// <returns>Result of arithmetic operation.</returns>
        public static object Modulo(object leftArgument, object rightArgument, Type commonType)
        {
            return EvaluateBinaryOperation(leftArgument, rightArgument, Expression.Modulo, commonType);
        }

        /// <summary>
        /// Performs bitwise AND on two untyped values and returns their result.
        /// </summary>
        /// <param name="leftArgument">Left argument to the operation.</param>
        /// <param name="rightArgument">Right argument to the operation.</param>
        /// <param name="commonType">Common CLR type for both input types (will be the type of the result of the operation)</param>
        /// <returns>Result of arithmetic operation.</returns>
        public static object BitwiseAnd(object leftArgument, object rightArgument, Type commonType)
        {
            return EvaluateBinaryOperation(leftArgument, rightArgument, Expression.And, commonType);
        }

        /// <summary>
        /// Performs bitwise OR on two untyped values and returns their result.
        /// </summary>
        /// <param name="leftArgument">Left argument to the operation.</param>
        /// <param name="rightArgument">Right argument to the operation.</param>
        /// <param name="commonType">Common CLR type for both input types (will be the type of the result of the operation)</param>
        /// <returns>Result of arithmetic operation.</returns>
        public static object BitwiseOr(object leftArgument, object rightArgument, Type commonType)
        {
            return EvaluateBinaryOperation(leftArgument, rightArgument, Expression.Or, commonType);
        }

        /// <summary>
        /// Performs exclusive OR on two untyped values and returns their result.
        /// </summary>
        /// <param name="leftArgument">Left argument to the operation.</param>
        /// <param name="rightArgument">Right argument to the operation.</param>
        /// <param name="commonType">Common CLR type for both input types (will be the type of the result of the operation)</param>
        /// <returns>Result of arithmetic operation.</returns>
        public static object ExclusiveOr(object leftArgument, object rightArgument, Type commonType)
        {
            return EvaluateBinaryOperation(leftArgument, rightArgument, Expression.ExclusiveOr, commonType);
        }

        /// <summary>
        /// Changes an object's value into the required type
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <param name="typeToConvertTo">The type to convert the value into</param>
        /// <returns>The value cast into the required type</returns>
        public static object ChangeType(object value, Type typeToConvertTo)
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");
            ExceptionUtilities.CheckArgumentNotNull(typeToConvertTo, "typeToConvertTo");

            TypeConverter tc = TypeDescriptor.GetConverter(typeToConvertTo);
            if (tc.CanConvertTo(typeToConvertTo))
            {
                return tc.ConvertTo(value, typeToConvertTo);
            }
            else
            {
                return Convert.ChangeType(value, typeToConvertTo, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Applies given arithmetic operation to two arguments and returns the result of the operation.
        /// </summary>
        /// <param name="leftArgument">First argument to the operation.</param>
        /// <param name="rightArgument">Second argument to the operation.</param>
        /// <param name="expressionBuilderMethod">Linq Expression builder method that describes the operation.</param>
        /// <param name="commonType">Common CLR type for both input types (will be the type of the result of the operation)</param>
        /// <returns>Result of arithmetic operation.</returns>
        private static object EvaluateBinaryOperation(object leftArgument, object rightArgument, Func<Expression, Expression, Expression> expressionBuilderMethod, Type commonType)
        {
            string cacheKey = expressionBuilderMethod.Method().Name + "_" + commonType.Name;
            Func<object, object, object> lambda;

            lock (binaryDelegateCache)
            {
                if (!binaryDelegateCache.TryGetValue(cacheKey, out lambda))
                {
                    lambda = GetBinaryEvaluationDelegate(expressionBuilderMethod, commonType);
                    binaryDelegateCache.Add(cacheKey, lambda);
                }
            }

            leftArgument = ChangeType(leftArgument, commonType);
            rightArgument = ChangeType(rightArgument, commonType);

            return lambda(leftArgument, rightArgument);
        }

        private static Func<object, object, object> GetBinaryEvaluationDelegate(Func<Expression, Expression, Expression> expressionBuilderMethod, Type commonClrType)
        {
            ParameterExpression leftParam = Expression.Parameter(typeof(object), "left");
            ParameterExpression rightParam = Expression.Parameter(typeof(object), "right");

            var lambda = Expression.Lambda<Func<object, object, object>>(
                Expression.Convert(
                    expressionBuilderMethod(
                        Expression.Convert(leftParam, commonClrType),
                        Expression.Convert(rightParam, commonClrType)),
                    typeof(object)),
                leftParam,
                rightParam);

            return lambda.Compile();
        }

        private static object EvaluateUnaryOperation(object argument, Func<Expression, Expression> expressionBuilderMethod, Type resultType)
        {
            string cacheKey = expressionBuilderMethod.Method().Name + "_" + resultType.Name;
            Func<object, object> lambda;

            lock (unaryDelegateCache)
            {
                if (!unaryDelegateCache.TryGetValue(cacheKey, out lambda))
                {
                    lambda = GetUnaryEvaluationDelegate(expressionBuilderMethod, resultType);
                    unaryDelegateCache.Add(cacheKey, lambda);
                }
            }

            argument = ChangeType(argument, resultType);

            return lambda(argument);
        }

        private static Func<object, object> GetUnaryEvaluationDelegate(Func<Expression, Expression> expressionBuilderMethod, Type clrType)
        {
            ParameterExpression param = Expression.Parameter(typeof(object), "param");

            var lambda = Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    expressionBuilderMethod(Expression.Convert(param, clrType)),
                    typeof(object)),
                param);

            return lambda.Compile();
        }
    }
}