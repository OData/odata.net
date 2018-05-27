//---------------------------------------------------------------------
// <copyright file="SingleValueCastNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Node representing a type segment that casts a single primitive value node.
    /// </summary>
    public sealed class SingleValueCastNode : SingleValueNode
    {
        /// <summary>
        /// The resource that we're casting to a different type.
        /// </summary>
        private readonly SingleValueNode source;

        /// <summary>
        /// The target type that the source is cast to.
        /// </summary>
        private readonly IEdmPrimitiveTypeReference primitiveTypeReference;

        /// <summary>
        /// Created a SingleValueCastNode with the given source node and the given type to cast to.
        /// </summary>
        /// <param name="source"> Source <see cref="SingleValueNode"/> that is being cast.</param>
        /// <param name="primitiveType">Type to cast to.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input primitiveType is null.</exception>
        public SingleValueCastNode(SingleValueNode source, IEdmPrimitiveType primitiveType)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");
            ExceptionUtils.CheckArgumentNotNull(primitiveType, "primitiveType");
            this.source = source;
            this.primitiveTypeReference = new EdmPrimitiveTypeReference(primitiveType, true);
        }

        /// <summary>
        /// Gets the property that we're casting to a different type.
        /// </summary>
        public SingleValueNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the target type that the source is cast to.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.primitiveTypeReference; }
        }

        /// <summary>
        /// Gets the kind of this query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get { return InternalQueryNodeKind.SingleValueCast; }
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