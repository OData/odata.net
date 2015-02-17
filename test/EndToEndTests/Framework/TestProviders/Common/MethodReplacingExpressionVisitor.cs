//---------------------------------------------------------------------
// <copyright file="MethodReplacingExpressionVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Common
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Microsoft.Test.OData.Framework.TestProviders.Contracts;

    /// <summary>
    /// Component for replacing method calls in an expression tree given a set of replacement strategies
    /// </summary>
    public class MethodReplacingExpressionVisitor : ExpressionVisitor
    {
        private IEnumerable<IMethodReplacementStrategy> replacementStrategies;

        /// <summary>
        /// Initializes a new instance of the MethodReplacingExpressionVisitor class
        /// </summary>
        /// <param name="replacementStrategies">The strategies to use when replacing methods</param>
        internal MethodReplacingExpressionVisitor(IEnumerable<IMethodReplacementStrategy> replacementStrategies)
        {
            ExceptionUtilities.CheckCollectionNotEmpty(replacementStrategies, "replacementStrategies");
            this.replacementStrategies = replacementStrategies;
        }

        /// <summary>
        /// Visits a method call expression
        /// </summary>
        /// <param name="node">The method call expression</param>
        /// <returns>A new expression after the parameter is visited</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            foreach (var replacementStrategy in this.replacementStrategies)
            {
                Expression replacement;
                if (replacementStrategy.TryGetReplacement(node.Method, this.Visit(node.Arguments), out replacement))
                {
                    return replacement;
                }
            }

            return base.VisitMethodCall(node);
        }

        /// <summary>
        /// UnaryExpression visit method
        /// </summary>
        /// <param name="node">The UnaryExpression expression to visit</param>
        /// <returns>The visited UnaryExpression expression </returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.Method != null)
            {
                foreach (var replacementStrategy in this.replacementStrategies)
                {
                    Expression replacement;
                    if (replacementStrategy.TryGetReplacement(node.Method, new[] { this.Visit(node.Operand) }, out replacement))
                    {
                        return replacement;
                    }
                }
            }

            return base.VisitUnary(node);
        }

        /// <summary>
        /// BinaryExpression visit method
        /// </summary>
        /// <param name="node">The BinaryExpression expression to visit</param>
        /// <returns>The visited BinaryExpression expression </returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.Method != null)
            {
                foreach (var replacementStrategy in this.replacementStrategies)
                {
                    Expression replacement;
                    if (replacementStrategy.TryGetReplacement(node.Method, new[] { this.Visit(node.Left), this.Visit(node.Right) }, out replacement))
                    {
                        return replacement;
                    }
                }
            }

            return base.VisitBinary(node);
        }
    }
}