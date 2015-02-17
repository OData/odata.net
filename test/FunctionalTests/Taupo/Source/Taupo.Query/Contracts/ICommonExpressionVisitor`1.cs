//---------------------------------------------------------------------
// <copyright file="ICommonExpressionVisitor`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Visits a QueryExpression tree following the double dispatch visitor pattern. 
    /// </summary>
    /// <typeparam name="TResult">The type which the visitor will return.</typeparam>
    public interface ICommonExpressionVisitor<TResult>
    {
        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryAddExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryAndExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryAsExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited</param>
        /// <returns>The result of visiting this expression</returns>
        TResult Visit(QueryCastExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryConstantExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryCustomFunctionCallExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryDivideExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryEqualToExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited</param>
        /// <returns>The result of visiting this expression</returns>
        TResult Visit(QueryFunctionImportCallExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryFunctionParameterReferenceExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryGreaterThanExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryGreaterThanOrEqualToExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited</param>
        /// <returns>The result of visiting this expression</returns>
        TResult Visit(QueryIsNotNullExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited</param>
        /// <returns>The result of visiting this expression</returns>
        TResult Visit(QueryIsNullExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited</param>
        /// <returns>The result of visiting this expression</returns>
        TResult Visit(QueryIsOfExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryLessThanExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryLessThanOrEqualToExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryModuloExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryMultiplyExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryNotEqualToExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryNotExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryNullExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited</param>
        /// <returns>The result of visiting this expression</returns>
        TResult Visit(QueryOfTypeExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryOrExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryPropertyExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QueryRootExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(QuerySubtractExpression expression);
    }
}
