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
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

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
        {
            ExceptionUtils.CheckArgumentNotNull(left, "left");
            ExceptionUtils.CheckArgumentNotNull(right, "right");
            this.operatorKind = operatorKind;
            this.left = left;
            this.right = right;

            // set the TypeReerence based on the Operands.
            if (this.Left == null || this.Right == null || this.Left.TypeReference == null || this.Right.TypeReference == null)
            {
                this.typeReference = null;
            }
            else
            {
                // Ensure that both operands have the same type
                if (!this.Left.TypeReference.Definition.IsEquivalentTo(this.Right.TypeReference.Definition))
                {
                    throw new ODataException(
                        ODataErrorStrings.BinaryOperatorQueryNode_OperandsMustHaveSameTypes(
                            this.Left.TypeReference.ODataFullName(), this.Right.TypeReference.ODataFullName()));
                }

                // Get a primitive type reference; this must not fail since we checked that the type is of kind 'primitive'.
                IEdmPrimitiveTypeReference primitiveOperatorTypeReference =
                    this.Left.TypeReference.AsPrimitive();

                this.typeReference = QueryNodeUtils.GetBinaryOperatorResultType(primitiveOperatorTypeReference, this.OperatorKind);
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
                DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
