//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Parsing
{
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using Microsoft.Data.OData.Query;
    using Microsoft.Data.OData.Query.SemanticAst;

    /// <summary>
    /// Component for building LINQ expressions for skip-tokens.
    /// </summary>
    internal class SkipTokenExpressionBuilder
    {
        /// <summary>
        /// The node to expression translator to use.
        /// </summary>
        private readonly NodeToExpressionTranslator nodeToExpressionTranslator;

        /// <summary>
        /// Initializes a new instance of <see cref="SkipTokenExpressionBuilder"/>.
        /// </summary>
        /// <param name="nodeToExpressionTranslator">The node to expression translator to use</param>
        internal SkipTokenExpressionBuilder(NodeToExpressionTranslator nodeToExpressionTranslator)
        {
            this.nodeToExpressionTranslator = nodeToExpressionTranslator;
        }

        /// <summary>Parse one of the literals of skip token.</summary>
        /// <param name="literal">Input literal.</param>
        /// <returns>Object resulting from conversion of literal.</returns>
        internal static object ParseSkipTokenLiteral(string literal)
        {
            ExpressionLexer l = new ExpressionLexer(literal);
            ConstantNode node;
            if (!TokenToQueryNodeTranslator.TryCreateLiteral(l.CurrentToken, out node))
            {
                throw new InvalidOperationException(Strings.RequsetQueryParser_ExpectingLiteralInSkipToken(literal));
            }

            return node.Value;
        }

        /// <summary>Makes the expression that is used as a filter corresponding to skip token.</summary>
        /// <param name="topLevelOrderingInfo">Ordering expression.</param>
        /// <param name="skipToken">The provided skip token.</param>
        /// <param name="parameterType">The parameter type of the lambda.</param>
        /// <returns>LambdaExpression corresponding to the skip token filter.</returns>
        internal LambdaExpression BuildSkipTokenFilter(OrderingInfo topLevelOrderingInfo, IList<object> skipToken, Type parameterType)
        {
            ParameterExpression element = Expression.Parameter(parameterType, "element");
            Expression lastCondition = Expression.Constant(true, typeof(bool));
            Expression lastMatch = Expression.Constant(false, typeof(bool));

            foreach (var v in WebUtil.Zip(topLevelOrderingInfo.OrderingExpressions, skipToken, (x, y) => new { Order = x, Value = y }))
            {
                BinaryOperatorKind comparisonExp = v.Order.IsAscending ? BinaryOperatorKind.GreaterThan : BinaryOperatorKind.LessThan;

                Expression fixedLambda = ParameterReplacerVisitor.Replace(
                    ((LambdaExpression)v.Order.Expression).Body,
                    ((LambdaExpression)v.Order.Expression).Parameters[0],
                    element);

                ConstantNode node;
                var lexer = new ExpressionLexer((string)v.Value);
                bool success = TokenToQueryNodeTranslator.TryCreateLiteral(lexer.CurrentToken, out node);
                Debug.Assert(success, "Was not a literal");

                Expression right = this.nodeToExpressionTranslator.TranslateNode(node);

                Expression comparison = ExpressionGenerator.GenerateLogicalAnd(
                    lastCondition,
                    this.GenerateNullAwareComparison(fixedLambda, right, comparisonExp));

                lastMatch = ExpressionGenerator.GenerateLogicalOr(lastMatch, comparison);

                lastCondition = ExpressionGenerator.GenerateLogicalAnd(
                    lastCondition,
                    this.GenerateComparisonExpression(fixedLambda, right, BinaryOperatorKind.Equal));
            }

            lastMatch = ExpressionUtils.EnsurePredicateExpressionIsBoolean(lastMatch);

            Debug.Assert(lastMatch.Type == typeof(bool), "Skip token should generate boolean expression.");

            return Expression.Lambda(lastMatch, element);
        }

        /// <summary>
        /// Generates a comparison expression which can handle NULL values for any type.
        /// NULL is always treated as the smallest possible value.
        /// So for example for strings NULL is smaller than any non-NULL string.
        /// For now only GreaterThan and LessThan operators are supported by this method.
        /// </summary>
        /// <param name="left">Left hand side expression</param>
        /// <param name="right">Right hand side expression</param>
        /// <param name="operatorKind">gt or lt operator token</param>
        /// <returns>Resulting comparison expression (has a Boolean value)</returns>
        private Expression GenerateNullAwareComparison(Expression left, Expression right, BinaryOperatorKind operatorKind)
        {
            Debug.Assert(
                operatorKind == BinaryOperatorKind.GreaterThan || operatorKind == BinaryOperatorKind.LessThan,
                "Only GreaterThan or LessThan operators are supported by the GenerateNullAwateComparison method for now.");

            if (WebUtil.TypeAllowsNull(left.Type))
            {
                if (!WebUtil.TypeAllowsNull(right.Type))
                {
                    right = Expression.Convert(right, typeof(Nullable<>).MakeGenericType(right.Type));
                }
            }
            else if (WebUtil.TypeAllowsNull(right.Type))
            {
                left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
            }
            else
            {
                // Can't perform NULL aware comparison on this type. Just let the normal 
                // comparison deal with it. Since the type doesn't allow NULL one should 
                // never appear, so normal comparison is just fine.
                return this.GenerateComparisonExpression(left, right, operatorKind);
            }

            switch (operatorKind)
            {
                case BinaryOperatorKind.GreaterThan:
                    // (left != null) && ((right == null) || Compare(left, right) > 0)
                    if (left == ExpressionUtils.NullLiteral)
                    {
                        return Expression.Constant(false, typeof(bool));
                    }
                    else if (right == ExpressionUtils.NullLiteral)
                    {
                        return ExpressionGenerator.GenerateNotEqual(left, Expression.Constant(null, left.Type));
                    }
                    else
                    {
                        return ExpressionGenerator.GenerateLogicalAnd(
                            ExpressionGenerator.GenerateNotEqual(left, Expression.Constant(null, left.Type)),
                            ExpressionGenerator.GenerateLogicalOr(
                                ExpressionGenerator.GenerateEqual(right, Expression.Constant(null, right.Type)),
                                this.GenerateComparisonExpression(left, right, operatorKind)));
                    }

                case BinaryOperatorKind.LessThan:
                    // (right != null) && ((left == null) || Compare(left, right) < 0)
                    if (right == ExpressionUtils.NullLiteral)
                    {
                        return Expression.Constant(false, typeof(bool));
                    }
                    else if (left == ExpressionUtils.NullLiteral)
                    {
                        return ExpressionGenerator.GenerateNotEqual(right, Expression.Constant(null, right.Type));
                    }
                    else
                    {
                        return ExpressionGenerator.GenerateLogicalAnd(
                            ExpressionGenerator.GenerateNotEqual(right, Expression.Constant(null, left.Type)),
                            ExpressionGenerator.GenerateLogicalOr(
                                ExpressionGenerator.GenerateEqual(left, Expression.Constant(null, right.Type)),
                                this.GenerateComparisonExpression(left, right, operatorKind)));
                    }

                default:
                    // For now only < and > are supported as we use this only from $skiptoken
                    string message = Strings.RequestQueryParser_NullOperatorUnsupported(operatorKind);
                    throw DataServiceException.CreateSyntaxError(message);
            }
        }

        /// <summary>
        /// Generates a comparison expression.
        /// </summary>
        /// <param name="left">The left side of the comparison.</param>
        /// <param name="right">The right side of the comparison.</param>
        /// <param name="op">The comparison operator.</param>
        /// <returns>A comparison expression.</returns>
        private Expression GenerateComparisonExpression(Expression left, Expression right, BinaryOperatorKind op)
        {
            return this.nodeToExpressionTranslator.TranslateComparison(op, left, right);
        }
    }
}
