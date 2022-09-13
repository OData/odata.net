//---------------------------------------------------------------------
// <copyright file="InNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// Query node representing an In operator.
    /// </summary>
    public sealed class InNode : SingleValueNode
    {
        /// <summary>
        /// Bool reference, as In always returns boolean.
        /// </summary>
        private readonly IEdmTypeReference boolTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(typeof(bool));

        /// <summary>
        /// The left operand.
        /// </summary>
        private readonly SingleValueNode left;

        /// <summary>
        /// The right operand.
        /// </summary>
        private readonly CollectionNode right;

        /// <summary>
        /// Create a InNode
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the left or right inputs are null.</exception>
        /// <exception cref="ODataException">Throws if the right operand single item type isn't the same type as the left operand.</exception>
        public InNode(SingleValueNode left, CollectionNode right)
        {
            ExceptionUtils.CheckArgumentNotNull(left, "left");
            ExceptionUtils.CheckArgumentNotNull(right, "right");
            this.left = left;
            this.right = right;

            if (!right.GetEdmTypeReference().IsUntyped())
            {
                if (!this.left.GetEdmTypeReference().IsAssignableFrom(this.right.ItemType) &&
                    !this.right.ItemType.IsAssignableFrom(this.left.GetEdmTypeReference()))
                {
                    throw new ArgumentException(ODataErrorStrings.Nodes_InNode_CollectionItemTypeMustBeSameAsSingleItemType(
                        this.right.ItemType.FullName(), this.left.GetEdmTypeReference().FullName()));
                }
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
        public CollectionNode Right
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
                return this.boolTypeReference;
            }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.In;
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