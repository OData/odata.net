//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Class to handle writing an BinaryOperatorQueryToken.
    /// This class optimizes on reducing parentheses as base on operator precedence.
    /// </summary>
    internal sealed class BinaryOperatorUriBuilder
    {
        /// <summary>
        /// The parent ODataUriBuilder that invokes this binary operator builder.
        /// </summary>
        private readonly ODataUriBuilder builder;

        /// <summary>
        /// Create a new BinaryOperatorQueryToken for the given Uri builder to write BinaryOperatorQueryToken.
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
        public void Write(BinaryOperatorQueryToken binary)
        {
            Write(false, binary);
        }

        /// <summary>
        /// Determine whether parentheses are needed around the left subtree base on the current operator.
        /// </summary>
        /// <param name="currentOperator">The current binary node's operator.</param>
        /// <param name="leftSubtree">The left binary subtree.</param>
        /// <returns>True if need parenthese, false if not.</returns>
        private static bool NeedParenthesesLeft(BinaryOperator currentOperator, BinaryOperatorQueryToken leftSubtree)
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
        private static bool NeedParenthesesRight(BinaryOperator currentOperator, BinaryOperatorQueryToken rightSubtree)
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
        private void Write(bool needParenthesis, BinaryOperatorQueryToken binary)
        {
            if (needParenthesis)
            {
                this.builder.Append(ExpressionConstants.SymbolOpenParen);
            }

            BinaryOperator currentOperator = BinaryOperator.GetOperator(binary.OperatorKind);

            // Left binary expression may need paren depending on their operator
            BinaryOperatorQueryToken leftBinaryExpr = binary.Left as BinaryOperatorQueryToken;
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
            BinaryOperatorQueryToken rightBinaryExpr = binary.Right as BinaryOperatorQueryToken;
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
