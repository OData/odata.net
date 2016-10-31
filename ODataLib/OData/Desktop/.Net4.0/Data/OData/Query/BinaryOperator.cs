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
    /// <summary>
    /// Class to wrap around BinaryOperatorKind that gives precedent meaning to it.
    /// </summary>
    internal sealed class BinaryOperator
    {
        /// <summary>Wrapper for Add operator kind.</summary>
        private static readonly BinaryOperator Add = new BinaryOperator(ExpressionConstants.KeywordAdd, 4, false);

        /// <summary>Wrapper for And operator kind.</summary>
        private static readonly BinaryOperator And = new BinaryOperator(ExpressionConstants.KeywordAnd, 1, false);

        /// <summary>Wrapper for Divide operator kind.</summary>
        private static readonly BinaryOperator Divide = new BinaryOperator(ExpressionConstants.KeywordDivide, 5, true);

        /// <summary>Wrapper for Equal operator kind.</summary>
        private static readonly BinaryOperator Equal = new BinaryOperator(ExpressionConstants.KeywordEqual, 2, true);

        /// <summary>Wrapper for GreaterThanOrEqual operator kind.</summary>
        private static readonly BinaryOperator GreaterThanOrEqual = new BinaryOperator(ExpressionConstants.KeywordGreaterThanOrEqual, 3, true);

        /// <summary>Wrapper for GreaterThan operator kind.</summary>
        private static readonly BinaryOperator GreaterThan = new BinaryOperator(ExpressionConstants.KeywordGreaterThan, 3, true);

        /// <summary>Wrapper for LessThanOrEqual operator kind.</summary>
        private static readonly BinaryOperator LessThanOrEqual = new BinaryOperator(ExpressionConstants.KeywordLessThanOrEqual, 3, true);

        /// <summary>Wrapper for LessThan operator kind.</summary>
        private static readonly BinaryOperator LessThan = new BinaryOperator(ExpressionConstants.KeywordLessThan, 3, true);

        /// <summary>Wrapper for Modulo operator kind.</summary>
        private static readonly BinaryOperator Modulo = new BinaryOperator(ExpressionConstants.KeywordModulo, 5, true);

        /// <summary>Wrapper for Multiply operator kind.</summary>
        private static readonly BinaryOperator Multiply = new BinaryOperator(ExpressionConstants.KeywordMultiply, 5, false);

        /// <summary>Wrapper for NotEqual operator kind.</summary>
        private static readonly BinaryOperator NotEqual = new BinaryOperator(ExpressionConstants.KeywordNotEqual, 2, true);

        /// <summary>Wrapper for Or operator kind.</summary>
        private static readonly BinaryOperator Or = new BinaryOperator(ExpressionConstants.KeywordOr, 0, false);

        /// <summary>Wrapper for Subtract operator kind.</summary>
        private static readonly BinaryOperator Subtract = new BinaryOperator(ExpressionConstants.KeywordSub, 4, true);

        /// <summary>
        /// The text for this operator.
        /// </summary>
        private readonly string text;

        /// <summary>
        /// The precedence for this operator.
        /// </summary>
        private readonly short precedence;

        /// <summary>
        /// Whether it needs parentheses against other same operator.
        /// </summary>
        private readonly bool needParenEvenWhenTheSame;

        /// <summary>
        /// Create a new BinaryOperator given its text, precedence, 
        /// and whether it needs parentheses against other same operator.
        /// </summary>
        /// <param name="text">The text for this operator.</param>
        /// <param name="precedence">The precedence for this operator in relative to other operators.</param>
        /// <param name="needParenEvenWhenTheSame">
        /// Whether it needs parentheses 
        /// when nesting with other operators of same precedence.
        /// </param>
        private BinaryOperator(string text, short precedence, bool needParenEvenWhenTheSame)
        {
            this.text = text;
            this.precedence = precedence;
            this.needParenEvenWhenTheSame = needParenEvenWhenTheSame;
        }

        /// <summary>
        /// Whether it needs parentheses against other same operator.
        /// </summary>
        public bool NeedParenEvenWhenTheSame 
        { 
            get { return this.needParenEvenWhenTheSame; } 
        }

        /// <summary>
        /// The precedence for this operator.
        /// </summary>
        public short Precedence 
        { 
            get { return this.precedence; } 
        }

        /// <summary>
        /// The text for this operator.
        /// </summary>
        public string Text 
        { 
            get { return this.text; } 
        }

        /// <summary>
        /// Get the BinaryOperator wrapper for the given operatorKind.
        /// </summary>
        /// <param name="operatorKind">The kind to get wrapper for.</param>
        /// <returns>The binary operator for the specified <paramref name="operatorKind"/>.</returns>
        public static BinaryOperator GetOperator(BinaryOperatorKind operatorKind)
        {
            switch (operatorKind)
            {
                case BinaryOperatorKind.Add:
                    return Add;

                case BinaryOperatorKind.And:
                    return And;

                case BinaryOperatorKind.Divide:
                    return Divide;

                case BinaryOperatorKind.Equal:
                    return Equal;

                case BinaryOperatorKind.GreaterThan:
                    return GreaterThan;

                case BinaryOperatorKind.GreaterThanOrEqual:
                    return GreaterThanOrEqual;

                case BinaryOperatorKind.LessThan:
                    return LessThan;

                case BinaryOperatorKind.LessThanOrEqual:
                    return LessThanOrEqual;

                case BinaryOperatorKind.Modulo:
                    return Modulo;

                case BinaryOperatorKind.Multiply:
                    return Multiply;

                case BinaryOperatorKind.NotEqual:
                    return NotEqual;

                case BinaryOperatorKind.Or:
                    return Or;

                case BinaryOperatorKind.Subtract:
                    return Subtract;
            }

            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BinaryOperator_GetOperator_UnreachableCodePath));
        }
    }
}
