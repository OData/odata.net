//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Query node representing a binary operator.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class BinaryOperatorQueryNode : SingleValueQueryNode
#else
    public sealed class BinaryOperatorQueryNode : SingleValueQueryNode
#endif
    {
        /// <summary>
        /// The operator represented by this node.
        /// </summary>
        public BinaryOperatorKind OperatorKind
        {
            get;
            set;
        }

        /// <summary>
        /// The left operand.
        /// </summary>
        public SingleValueQueryNode Left
        {
            get;
            set;
        }

        /// <summary>
        /// The right operand.
        /// </summary>
        public SingleValueQueryNode Right
        {
            get;
            set;
        }

        /// <summary>
        /// The resouce type of the single value this node represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get
            {
                if (this.Left == null || this.Right == null || this.Left.TypeReference == null || this.Right.TypeReference == null)
                {
                    return null;
                }
                else
                {
                    // Binary operators only work on primitive types so check that both operands have primitive types
                    if (!this.Left.TypeReference.IsODataPrimitiveTypeKind() || !this.Right.TypeReference.IsODataPrimitiveTypeKind())
                    {
                        throw new ODataException(Strings.BinaryOperatorQueryNode_InvalidOperandType(this.Left.TypeReference.ODataFullName(), this.Right.TypeReference.ODataFullName()));
                    }

                    // Ensure that both operands have the same type
                    if (!this.Left.TypeReference.Definition.IsEquivalentTo(this.Right.TypeReference.Definition))
                    {
                        throw new ODataException(Strings.BinaryOperatorQueryNode_OperandsMustHaveSameTypes(this.Left.TypeReference.ODataFullName(), this.Right.TypeReference.ODataFullName()));
                    }

                    // Get a primitive type reference; this must not fail since we checked that the type is of kind 'primitive'.
                    IEdmPrimitiveTypeReference primitiveOperatorTypeReference = this.Left.TypeReference.AsPrimitive();

                    return QueryNodeUtils.GetBinaryOperatorResultType(primitiveOperatorTypeReference, this.OperatorKind);
                }
            }
        }

        /// <summary>
        /// The kind of the query node.
        /// </summary>
        public override QueryNodeKind Kind
        {
            get
            {
                return QueryNodeKind.BinaryOperator;
            }
        }
    }
}
