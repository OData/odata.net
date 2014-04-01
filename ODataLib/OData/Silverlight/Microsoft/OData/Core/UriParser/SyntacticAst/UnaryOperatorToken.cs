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
