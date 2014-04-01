//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Syntactic
#endif
{
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;

    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a binary operator.
    /// </summary>
    internal sealed class BinaryOperatorToken : QueryToken
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
