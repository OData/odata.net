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

namespace System.Data.OData.Query
{
    #region Namespaces.
    using System.Data.Services.Providers;
    using System.Diagnostics;
    #endregion Namespaces.

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
        public override ResourceType ResourceType
        {
            get
            {
                if (this.Left == null || this.Right == null || this.Left.ResourceType == null || this.Right.ResourceType == null)
                {
                    return null;
                }
                else
                {
                    // ensure that both operands have the same type; binary operators only work on primitive types
                    // so check that both operands have primitive types and then use reference equality comparison
                    // since primitive types are always atomized
                    if (this.Left.ResourceType.ResourceTypeKind != ResourceTypeKind.Primitive ||
                        this.Right.ResourceType.ResourceTypeKind != ResourceTypeKind.Primitive)
                    {
                        throw new ODataException(Strings.BinaryOperatorQueryNode_InvalidOperandType(this.Left.ResourceType.FullName, this.Right.ResourceType.FullName));
                    }

                    if (!object.ReferenceEquals(this.Left.ResourceType, this.Right.ResourceType))
                    {
                        throw new ODataException(Strings.BinaryOperatorQueryNode_OperandsMustHaveSameTypes(this.Left.ResourceType.FullName, this.Right.ResourceType.FullName));
                    }

                    return QueryNodeUtils.GetBinaryOperatorResultType(this.Left.ResourceType, this.OperatorKind);
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
