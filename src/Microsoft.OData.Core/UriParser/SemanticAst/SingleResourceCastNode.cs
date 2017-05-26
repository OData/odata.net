//---------------------------------------------------------------------
// <copyright file="SingleResourceCastNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Node representing a type segment that casts a single entity/complex parent node.
    /// </summary>
    public sealed class SingleResourceCastNode : SingleResourceNode
    {
        /// <summary>
        /// The resource that we're casting to a different type.
        /// </summary>
        private readonly SingleResourceNode source;

        /// <summary>
        /// The target type that the source is cast to.
        /// </summary>
        private readonly IEdmStructuredTypeReference structuredTypeReference;

        /// <summary>
        /// The navigation source containing the source entity.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Created a SingleResourceCastNode with the given source node and the given type to cast to.
        /// </summary>
        /// <param name="source"> Source <see cref="SingleValueNode"/> that is being cast.</param>
        /// <param name="structuredType">Type to cast to.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input entityType is null.</exception>
        public SingleResourceCastNode(SingleResourceNode source, IEdmStructuredType structuredType)
        {
            ExceptionUtils.CheckArgumentNotNull(structuredType, "structuredType");
            this.source = source;
            this.navigationSource = source != null ? source.NavigationSource : null;
            this.structuredTypeReference = structuredType.GetTypeReference();
        }

        /// <summary>
        /// Gets the resource that we're casting to a different type.
        /// </summary>
        public SingleResourceNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the target type that the source is cast to.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.structuredTypeReference; }
        }

        /// <summary>
        /// Gets the navigation source containing the source entity..
        /// </summary>
        public override IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Gets the target type that the source is cast to.
        /// </summary>
        public override IEdmStructuredTypeReference StructuredTypeReference
        {
            get { return this.structuredTypeReference; }
        }

        /// <summary>
        /// Gets the kind of this query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.SingleResourceCast;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walks a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}