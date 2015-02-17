//---------------------------------------------------------------------
// <copyright file="ILinqToAstoriaExpressionVisitor`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using Microsoft.Test.Taupo.Query.Contracts.Linq;

    /// <summary>
    /// Visits QueryExpression tree nodes specific to Astoria, following the double dispatch visitor pattern. 
    /// </summary>
    /// <typeparam name="TResult">The type which the visitor will return.</typeparam>
    public interface ILinqToAstoriaExpressionVisitor<TResult> : ILinqExpressionVisitor<TResult>
    {
        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqAddQueryOptionExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqToAstoriaAddQueryOptionExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaConditionalExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqToAstoriaConditionalExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqExpandExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqToAstoriaExpandExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaValueExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqToAstoriaExpandLambdaExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaKeyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqToAstoriaKeyExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaLinksExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqToAstoriaLinksExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaValueExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqToAstoriaValueExpression expression);
    }
}
