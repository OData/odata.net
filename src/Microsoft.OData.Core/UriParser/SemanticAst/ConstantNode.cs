//---------------------------------------------------------------------
// <copyright file="ConstantNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Node representing a constant value, can either be primitive, enum literal node.
    /// </summary>
    public sealed class ConstantNode : SingleValueNode
    {
        /// <summary>
        /// Create a ConstantNode
        /// </summary>
        /// <param name="constantValue">This node's primitive value.</param>
        /// <param name="literalText">The literal text for this node's value, formatted according to the OData URI literal formatting rules.</param>
        public ConstantNode(object constantValue, string literalText)
            : this(constantValue)
        {
            ExceptionUtils.CheckArgumentNotNull(literalText, "literalText");

            this.LiteralText = literalText;
        }

        /// <summary>
        /// Create a ConstantNode
        /// </summary>
        /// <param name="constantValue">This node's primitive value.</param>
        /// <param name="literalText">The literal text for this node's value, formatted according to the OData URI literal formatting rules.</param>
        /// <param name="typeReference">The typeReference of this node's value.</param>
        public ConstantNode(object constantValue, string literalText, IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(literalText, "literalText");

            this.Value = constantValue;
            this.LiteralText = literalText;
            this.TypeReference = typeReference;
        }

        /// <summary>
        /// Create a ConstantNode
        /// </summary>
        /// <param name="constantValue">This node's primitive value.</param>
        public ConstantNode(object constantValue)
        {
            this.Value = constantValue;
            this.TypeReference = constantValue == null ? null : EdmLibraryExtensions.GetPrimitiveTypeReference(constantValue.GetType());
        }

        /// <summary>
        /// Create a ConstantNode without providing a literal text.
        /// </summary>
        /// <param name="constantValue">This node's primitive value.</param>
        /// <param name="typeReference">The typeReference of this node's value.</param>
        internal ConstantNode(object constantValue, IEdmTypeReference typeReference)
        {
            this.Value = constantValue;
            this.TypeReference = typeReference;
        }

        /// <summary>
        /// Gets the primitive constant value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Get or Set the literal text for this node's value, formatted according to the OData URI literal formatting rules. May be null if the text was not provided at construction time.
        /// </summary>
        public string LiteralText { get; }

        /// <summary>
        /// Gets the resource type of the single value this node represents. it could be null if the type is not provided at construction time.
        /// </summary>
        public override IEdmTypeReference TypeReference { get; }

        /// <summary>
        /// Gets the kind of the query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.Constant;

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> to walk a tree of <see cref="QueryNode"/>s.
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