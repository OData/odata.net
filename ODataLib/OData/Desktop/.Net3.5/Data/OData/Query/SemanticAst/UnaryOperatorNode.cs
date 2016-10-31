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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// Node representing a unary operator.
    /// </summary>
    public sealed class UnaryOperatorNode : SingleValueNode
    {
        /// <summary>
        /// The operand of the unary operator.
        /// </summary>
        private readonly SingleValueNode operand;

        /// <summary>
        /// The operator represented by this node.
        /// </summary>
        private readonly UnaryOperatorKind operatorKind;

        /// <summary>
        /// Cache for the TypeReference after it has been calculated for the current state of the node.
        /// This can be an expensive calculation so we want to avoid doing it repeatedly.
        /// </summary>
        private IEdmTypeReference typeReference;

        /// <summary>
        /// Creates a UnaryOperatorNode
        /// </summary>
        /// <param name="operatorKind">the kind of operator this node represents</param>
        /// <param name="operand">the operand that this operator modifies</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input operand is null.</exception>
        public UnaryOperatorNode(UnaryOperatorKind operatorKind, SingleValueNode operand)
        {
            ExceptionUtils.CheckArgumentNotNull(operand, "operand");
            this.operand = operand;
            this.operatorKind = operatorKind;

            if (operand == null || operand.TypeReference == null)
            {
                this.typeReference = null;
            }
            else
            {
                this.typeReference = operand.TypeReference;
            }
        }

        /// <summary>
        /// Gets the operator represented by this node.
        /// </summary>
        public UnaryOperatorKind OperatorKind
        {
            get { return this.operatorKind; }
        }

        /// <summary>
        /// Gets the operand of the unary operator.
        /// </summary>
        public SingleValueNode Operand
        {
            get { return this.operand; }
        }

        /// <summary>
        /// Gets the type of the single value this node represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get
            {
                return this.typeReference;
            }
        }

        /// <summary>
        /// Gets the kind of this query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return InternalQueryNodeKind.UnaryOperator;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walks a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input visitor is null.</exception>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
