//---------------------------------------------------------------------
// <copyright file="UnaryOperatorToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    /// <summary>
    /// Lexical token representing a unary operator.
    /// </summary>
    public sealed class UnaryOperatorToken : QueryToken
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