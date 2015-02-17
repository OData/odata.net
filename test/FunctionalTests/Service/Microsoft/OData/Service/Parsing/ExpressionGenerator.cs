//---------------------------------------------------------------------
// <copyright file="ExpressionGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Parsing
{
    #region Namespaces

    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Service.Providers;
    using Strings = Microsoft.OData.Service.Strings;
    #endregion Namespaces

    /// <summary>
    /// Component for building up LINQ expressions as part of URI parsing.
    /// </summary>
    internal static class ExpressionGenerator
    {
        /// <summary>Method info for byte array comparison.</summary>
        private static readonly MethodInfo AreByteArraysEqualMethodInfo = typeof(DataServiceProviderMethods)
            .GetMethod("AreByteArraysEqual", BindingFlags.Public | BindingFlags.Static);

        /// <summary>Method info for byte array comparison.</summary>
        private static readonly MethodInfo AreByteArraysNotEqualMethodInfo = typeof(DataServiceProviderMethods)
            .GetMethod("AreByteArraysNotEqual", BindingFlags.Public | BindingFlags.Static);

        /// <summary>Generates an Equal expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateEqual(Expression left, Expression right)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.EqualExpression(left, right);
            }

            if (left.Type == typeof(byte[]))
            {
                return Expression.Equal(left, right, false, AreByteArraysEqualMethodInfo);
            }

            return Expression.Equal(left, right);
        }

        /// <summary>Generates a NotEqual expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateNotEqual(Expression left, Expression right)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.NotEqualExpression(left, right);
            }

            if (left.Type == typeof(byte[]))
            {
                return Expression.NotEqual(left, right, false, AreByteArraysNotEqualMethodInfo);
            }

            return Expression.NotEqual(left, right);
        }

        /// <summary>Generates a GreaterThan comparison expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <param name="comparisonMethodInfo">MethodInfo for comparison method used for string, bool, guid types</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateGreaterThan(Expression left, Expression right, MethodInfo comparisonMethodInfo)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.GreaterThanExpression(left, right);
            }

            if (comparisonMethodInfo != null)
            {
                left = Expression.Call(null, comparisonMethodInfo, left, right);
                right = Expression.Constant(0, typeof(int));
            }

            return Expression.GreaterThan(left, right);
        }

        /// <summary>Generates a GreaterThanOrEqual comparsion expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <param name="comparisonMethodInfo">MethodInfo for comparison method used for string, bool, guid types</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateGreaterThanEqual(Expression left, Expression right, MethodInfo comparisonMethodInfo)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.GreaterThanOrEqualExpression(left, right);
            }

            if (comparisonMethodInfo != null)
            {
                left = Expression.Call(null, comparisonMethodInfo, left, right);
                right = Expression.Constant(0, typeof(int));
            }

            return Expression.GreaterThanOrEqual(left, right);
        }

        /// <summary>Generates a LessThan comparsion expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <param name="comparisonMethodInfo">MethodInfo for comparison method used for string, bool, guid types</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateLessThan(Expression left, Expression right, MethodInfo comparisonMethodInfo)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.LessThanExpression(left, right);
            }

            if (comparisonMethodInfo != null)
            {
                left = Expression.Call(null, comparisonMethodInfo, left, right);
                right = Expression.Constant(0, typeof(int));
            }

            return Expression.LessThan(left, right);
        }

        /// <summary>Generates a LessThanOrEqual comparsion expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <param name="comparisonMethodInfo">MethodInfo for comparison method used for string, bool, guid types</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateLessThanEqual(Expression left, Expression right, MethodInfo comparisonMethodInfo)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.LessThanOrEqualExpression(left, right);
            }

            if (comparisonMethodInfo != null)
            {
                left = Expression.Call(null, comparisonMethodInfo, left, right);
                right = Expression.Constant(0, typeof(int));
            }

            return Expression.LessThanOrEqual(left, right);
        }

        /// <summary>Generates an addition expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateAdd(Expression left, Expression right)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.AddExpression(left, right);
            }

            return Expression.Add(left, right);
        }

        /// <summary>Generates a subtract expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateSubtract(Expression left, Expression right)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.SubtractExpression(left, right);
            }

            return Expression.Subtract(left, right);
        }

        /// <summary>Generates a multiplication expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateMultiply(Expression left, Expression right)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.MultiplyExpression(left, right);
            }

            return Expression.Multiply(left, right);
        }

        /// <summary>Generates a negative of expression.</summary>
        /// <param name="expr">Input expression.</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateNegate(Expression expr)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(expr))
            {
                return OpenTypeMethods.NegateExpression(expr);
            }

            return Expression.Negate(expr);
        }

        /// <summary>Generates a not of expression.</summary>
        /// <param name="expr">Input expression.</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateNot(Expression expr)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(expr))
            {
                return OpenTypeMethods.NotExpression(expr);
            }

            if (expr.Type == typeof(bool) || expr.Type == typeof(Nullable<bool>))
            {
                // Expression.Not will take numerics and apply '~' to them, thus the extra check here.
                return Expression.Not(expr);
            }
            else
            {
                string message = Strings.RequestQueryParser_NotDoesNotSupportType(expr.Type);
                throw DataServiceException.CreateSyntaxError(message);
            }
        }

        /// <summary>Generates a divide expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateDivide(Expression left, Expression right)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.DivideExpression(left, right);
            }

            return Expression.Divide(left, right);
        }

        /// <summary>Generates a modulo expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateModulo(Expression left, Expression right)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.ModuloExpression(left, right);
            }

            return Expression.Modulo(left, right);
        }

        /// <summary>Generates a logical And expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateLogicalAnd(Expression left, Expression right)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.AndAlsoExpression(left, right);
            }

            return Expression.AndAlso(left, right);
        }

        /// <summary>Generates a Logical Or expression.</summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The generated expression.</returns>
        internal static Expression GenerateLogicalOr(Expression left, Expression right)
        {
            if (OpenTypeMethods.IsOpenPropertyExpression(left) || OpenTypeMethods.IsOpenPropertyExpression(right))
            {
                return OpenTypeMethods.OrElseExpression(left, right);
            }

            return Expression.OrElse(left, right);
        }

        /// <summary>
        /// Generate TypeAs expression over the given instance expression.
        /// </summary>
        /// <param name="instance">base expression.</param>
        /// <param name="resourceType">resource type to which the as checks needs to be performed on.</param>
        /// <returns>TypeAs expression over the given instance expression.</returns>
        internal static Expression GenerateTypeAs(Expression instance, ResourceType resourceType)
        {
            if (resourceType.CanReflectOnInstanceType)
            {
                return Expression.TypeAs(instance, resourceType.InstanceType);
            }

            return Expression.Call(
                null,
                DataServiceProviderMethods.TypeAsMethodInfo.MakeGenericMethod(resourceType.InstanceType),
                instance,
                Expression.Constant(resourceType));
        }

        /// <summary>
        /// Filters the elements of <paramref name="source"/> based on <paramref name="targetResourceType"/> 
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="targetResourceType">targetResourceType</param>
        /// <returns>An expression for IEnumerable(Of T) that contains elements from the source sequence of type <paramref name="targetResourceType"/>.</returns>
        internal static Expression GenerateOfType(Expression source, ResourceType targetResourceType)
        {
            if (targetResourceType.CanReflectOnInstanceType)
            {
                return source.EnumerableOfType(targetResourceType.InstanceType);
            }

            source = Expression.Call(
                DataServiceProviderMethods.OfTypeIEnumerableMethodInfo.MakeGenericMethod(
                    BaseServiceProvider.GetIEnumerableElement(source.Type),
                    targetResourceType.InstanceType),
                source,
                Expression.Constant(targetResourceType));

            return source.EnumerableCast(targetResourceType.InstanceType);
        }
    }
}
