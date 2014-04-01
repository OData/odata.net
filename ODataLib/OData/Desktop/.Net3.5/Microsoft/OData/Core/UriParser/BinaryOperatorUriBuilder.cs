//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Microsoft.OData.Core.UriParser.Syntactic;

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
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.Core.UriParser.ODataUriBuilder.Append(System.String)", Justification = "Non-localizable symbol")]
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
