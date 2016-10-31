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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Microsoft.Data.OData.Query.SyntacticAst;

    #endregion Namespaces

    /// <summary>
    /// Class to handle writing an BinaryOperatorToken.
    /// This class optimizes on reducing parentheses as base on operator precedence.
    /// </summary>
    internal sealed class BinaryOperatorUriBuilder
    {
        /// <summary>
        /// The parent ODataUriBuilder that invokes this binary operator builder.
        /// </summary>
        private readonly ODataUriBuilder builder;

        /// <summary>
        /// Create a new BinaryOperatorToken for the given Uri builder to write BinaryOperatorToken.
        /// </summary>
        /// <param name="builder">The parent builder of this builder.</param>
        public BinaryOperatorUriBuilder(ODataUriBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Write the given binary token as Uri part.
        /// </summary>
        /// <param name="binary">To write as Uri part.</param>
        public void Write(BinaryOperatorToken binary)
        {
            Write(false, binary);
        }

        /// <summary>
        /// Determine whether parentheses are needed around the left subtree base on the current operator.
        /// </summary>
        /// <param name="currentOperator">The current binary node's operator.</param>
        /// <param name="leftSubtree">The left binary subtree.</param>
        /// <returns>True if need parenthese, false if not.</returns>
        private static bool NeedParenthesesLeft(BinaryOperator currentOperator, BinaryOperatorToken leftSubtree)
        {
            BinaryOperator leftOperator = BinaryOperator.GetOperator(leftSubtree.OperatorKind);
            return leftOperator.Precedence < currentOperator.Precedence;
        }

        /// <summary>
        /// Determine whether parentheses are needed around the right subtree base on the current operator.
        /// </summary>
        /// <param name="currentOperator">The current binary node's operator.</param>
        /// <param name="rightSubtree">The right binary subtree.</param>
        /// <returns>True if need parentheses, false if not.</returns>
        private static bool NeedParenthesesRight(BinaryOperator currentOperator, BinaryOperatorToken rightSubtree)
        {
            BinaryOperator rightOperator = BinaryOperator.GetOperator(rightSubtree.OperatorKind);
            if (currentOperator.Precedence < rightOperator.Precedence)
            {
                return false;
            }

            if (currentOperator.Precedence > rightOperator.Precedence)
            {
                return true;
            }

            // They are equal in precedence, 
            return currentOperator.NeedParenEvenWhenTheSame;
        }

        /// <summary>
        /// Write the given binary token as Uri part.
        /// </summary>
        /// <param name="needParenthesis">Whether parentheses are needed around the written expression.</param>
        /// <param name="binary">To write as Uri part.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Data.OData.Query.ODataUriBuilder.Append(System.String)", Justification = "Non-localizable symbol")]
        private void Write(bool needParenthesis, BinaryOperatorToken binary)
        {
            if (needParenthesis)
            {
                this.builder.Append(ExpressionConstants.SymbolOpenParen);
            }

            BinaryOperator currentOperator = BinaryOperator.GetOperator(binary.OperatorKind);

            // Left binary expression may need paren depending on their operator
            BinaryOperatorToken leftBinaryExpr = binary.Left as BinaryOperatorToken;
            if (leftBinaryExpr != null)
            {
                Write(NeedParenthesesLeft(currentOperator, leftBinaryExpr), leftBinaryExpr);
            }
            else
            {
                this.builder.WriteQuery(binary.Left);
            }

            this.builder.Append(ExpressionConstants.SymbolEscapedSpace);
            this.builder.Append(currentOperator.Text);
            this.builder.Append(ExpressionConstants.SymbolEscapedSpace);

            // Right binary expression may need paren depending on their operator
            BinaryOperatorToken rightBinaryExpr = binary.Right as BinaryOperatorToken;
            if (rightBinaryExpr != null)
            {
                Write(NeedParenthesesRight(currentOperator, rightBinaryExpr), rightBinaryExpr);
            }
            else
            {
                this.builder.WriteQuery(binary.Right);
            }

            if (needParenthesis)
            {
                this.builder.Append(ExpressionConstants.SymbolClosedParen);
            }
        }
    }
}
