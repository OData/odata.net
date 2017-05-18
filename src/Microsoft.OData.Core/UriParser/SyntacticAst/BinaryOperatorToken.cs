//---------------------------------------------------------------------
// <copyright file="BinaryOperatorToken.cs" company="Microsoft">
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
    /// Lexical token representing a binary operator.
    /// </summary>
    public sealed class BinaryOperatorToken : QueryToken
    {
        /// <summary>
        /// The operator represented by this node.
        /// </summary>
        private readonly BinaryOperatorKind operatorKind;

        /// <summary>
        /// The left operand.
        /// </summary>
        private readonly QueryToken left;

        /// <summary>
        /// The right operand.
        /// </summary>
        private readonly QueryToken right;

        /// <summary>
        /// Create a new BinaryOperatorToken given the operator, left and right query.
        /// </summary>
        /// <param name="operatorKind">The operator represented by this node.</param>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        public BinaryOperatorToken(BinaryOperatorKind operatorKind, QueryToken left, QueryToken right)
        {
            ExceptionUtils.CheckArgumentNotNull(left, "left");
            ExceptionUtils.CheckArgumentNotNull(right, "right");

            this.operatorKind = operatorKind;
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.BinaryOperator; }
        }

        /// <summary>
        /// The operator represented by this node.
        /// </summary>
        public BinaryOperatorKind OperatorKind
        {
            get { return this.operatorKind; }
        }

        /// <summary>
        /// The left operand.
        /// </summary>
        public QueryToken Left
        {
            get { return this.left; }
        }

        /// <summary>
        /// The right operand.
        /// </summary>
        public QueryToken Right
        {
            get { return this.right; }
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