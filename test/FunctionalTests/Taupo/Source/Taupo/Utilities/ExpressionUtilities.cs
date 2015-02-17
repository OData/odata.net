//---------------------------------------------------------------------
// <copyright file="ExpressionUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides utilities for working with lambda expressions.
    /// </summary>
    public static class ExpressionUtilities
    {
        /// <summary>
        /// Helper method to return the name of property accessed in the lambda <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">The expression which has a single simple property access.</param>
        /// <returns>Name of the property</returns>
        /// <remarks>This method is for generating refactoring-friendly strings, so when we rename a property 
        /// or event there's no need to update strings as well.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Expression<T> needed for the compiler to resolve the call to this method with an inline lambda expression argument.")]
        public static string NameOf(Expression<Func<object>> expression)
        {
            return ((MemberExpression)RemoveConvert(expression.Body)).Member.Name;
        }

        private static Expression RemoveConvert(Expression expression)
        {
            var unaryExpression = expression as UnaryExpression;

            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
            {
                return RemoveConvert(unaryExpression.Operand);
            }

            return expression;
        }
    }
}