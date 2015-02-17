//---------------------------------------------------------------------
// <copyright file="ILinqExpressionVisitor`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq
{
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Visits a QueryExpression tree following the double dispatch visitor pattern. 
    /// </summary>
    /// <typeparam name="TResult">The type which the visitor will return.</typeparam>
    public interface ILinqExpressionVisitor<TResult> : ICommonExpressionVisitor<TResult>
    {
        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqAllExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqAnyExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqAsEnumerableExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqBitwiseAndExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqBitwiseOrExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqBuiltInFunctionCallExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqCastExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqConcatExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqContainsExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqCountExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqDefaultIfEmptyExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqDistinctExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqExceptExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqExclusiveOrExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqFirstExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqFirstOrDefaultExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqFreeVariableExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqGroupByExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqGroupJoinExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqJoinExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqLambdaExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqLengthPropertyExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqLongCountExpression expression);

        /// <summary>
        /// Visits the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqMaxExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqMemberMethodExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqMemberMethodExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqMinExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqMinExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqNewArrayExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqNewArrayExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqNewExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqNewExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqNewInstanceExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqNewInstanceExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqOrderByExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqOrderByExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqParameterExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqParameterExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSelectExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqSelectExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSelectManyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqSelectManyExpression expression);
        
        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSingleExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqSingleExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSingleOrDefaultExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqSingleOrDefaultExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSkipExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqSkipExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqTakeExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqTakeExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqUnionExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqUnionExpression expression);

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqWhereExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        TResult Visit(LinqWhereExpression expression);
    }
}
