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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using Microsoft.Data.OData.Query.SemanticAst;

    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a unary operator.
    /// </summary>
    internal sealed class UnaryOperatorToken : QueryToken
    {
        /// <summary>
        /// The operator represented by this node.
        /// </summary>
        private readonly UnaryOperatorKind operatorKind;

        /// <summary>
        /// The operand.
        /// </summary>
        private readonly QueryToken operand;

        /// <summary>
        /// Create a new UnaryOperatorToken given the operator and operand
        /// </summary>
        /// <param name="operatorKind">The operator represented by this node.</param>
        /// <param name="operand">The operand.</param>
        public UnaryOperatorToken(UnaryOperatorKind operatorKind, QueryToken operand)
        {
            ExceptionUtils.CheckArgumentNotNull(operand, "operand");

            this.operatorKind = operatorKind;
            this.operand = operand;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.UnaryOperator; }
        }

        /// <summary>
        /// The operator represented by this node.
        /// </summary>
        public UnaryOperatorKind OperatorKind
        {
            get { return this.operatorKind; }
        }

        /// <summary>
        /// The operand.
        /// </summary>
        public QueryToken Operand
        {
            get { return this.operand; }
        }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
