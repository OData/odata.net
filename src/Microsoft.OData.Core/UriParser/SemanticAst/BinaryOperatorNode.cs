//---------------------------------------------------------------------
// <copyright file="BinaryOperatorNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// Query node representing a binary operator.
    /// </summary>
    public sealed class BinaryOperatorNode : SingleValueNode
    {
        /// <summary>
        /// The operator represented by this node.
        /// </summary>
        private readonly BinaryOperatorKind operatorKind;

        /// <summary>
        /// The left operand.
        /// </summary>
        private readonly SingleValueNode left;

        /// <summary>
        /// The right operand.
        /// </summary>
        private readonly SingleValueNode right;

        /// <summary>
        /// Cache for the TypeReference after it has been calculated for the current state of the node.
        /// This can be an expensive calculation so we want to avoid doing it repeatedly.
        /// </summary>
        private IEdmTypeReference typeReference;

        /// <summary>
        /// Create a BinaryOperatorNode
        /// </summary>
        /// <param name="operatorKind">The binary operator type.</param>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the left or right inputs are null.</exception>
        /// <exception cref="ODataException">Throws if the two operands don't have the same type.</exception>
        public BinaryOperatorNode(BinaryOperatorKind operatorKind, SingleValueNode left, SingleValueNode right)
            : this(operatorKind, left, right, /*typeReference*/ null)
        {
        }

        /// <summary>
        /// Create a BinaryOperatorNode
        /// </summary>
        /// <param name="operatorKind">The binary operator type.</param>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <param name="typeReference">The result typeReference.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the left or right inputs are null.</exception>
        internal BinaryOperatorNode(BinaryOperatorKind operatorKind, SingleValueNode left, SingleValueNode right, IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(left, "left");
            ExceptionUtils.CheckArgumentNotNull(right, "right");
            this.operatorKind = operatorKind;
            this.left = left;
            this.right = right;

            // set the TypeReference if explictly given, otherwise based on the Operands.
            if (typeReference != null)
            {
                this.typeReference = typeReference;
            }
            else if (this.Left == null || this.Right == null || this.Left.TypeReference == null || this.Right.TypeReference == null)
            {
                this.typeReference = null;
            }
            else
            {
                // Get a primitive type reference; this must not fail since we checked that the type is of kind 'primitive'.
                IEdmPrimitiveTypeReference leftType = this.Left.TypeReference.AsPrimitive();
                IEdmPrimitiveTypeReference rightType = this.Right.TypeReference.AsPrimitive();

                this.typeReference = QueryNodeUtils.GetBinaryOperatorResultType(leftType, rightType, this.OperatorKind);
            }
        }

        /// <summary>
        /// Gets the operator represented by this node.
        /// </summary>
        public BinaryOperatorKind OperatorKind
        {
            get
            {
                return this.operatorKind;
            }
        }

        /// <summary>
        /// Gets the left operand.
        /// </summary>
        public SingleValueNode Left
        {
            get
            {
                return this.left;
            }
        }

        /// <summary>
        /// Gets the right operand.
        /// </summary>
        public SingleValueNode Right
        {
            get
            {
                return this.right;
            }
        }

        /// <summary>
        /// Gets the resource type of the single value this node represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get
            {
                return this.typeReference;
            }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.BinaryOperator;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walks a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        /// <exception cref="System.ArgumentNullException">throws if the input visitor is null.</exception>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}