//---------------------------------------------------------------------
// <copyright file="UnaryOperatorNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;
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
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}