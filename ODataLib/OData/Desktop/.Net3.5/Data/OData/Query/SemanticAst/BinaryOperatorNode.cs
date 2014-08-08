//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
