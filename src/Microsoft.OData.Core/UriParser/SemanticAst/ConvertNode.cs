//---------------------------------------------------------------------
// <copyright file="ConvertNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Node representing a conversion of primitive type to another type.
    /// </summary>
    public sealed class ConvertNode : SingleValueNode
    {
        /// <summary>
        /// The source value to convert.
        /// </summary>
        private readonly SingleValueNode source;

        /// <summary>
        /// The target type that the source will be converted to.
        /// </summary>
        private readonly IEdmTypeReference typeReference;

        /// <summary>
        /// Constructs a ConvertNode.
        /// </summary>
        /// <param name="source">The node to convert.</param>
        /// <param name="typeReference"> The type to convert the node to</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input source or typeReference is null.</exception>
        public ConvertNode(SingleValueNode source, IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            this.source = source;
            this.typeReference = typeReference;
        }

        /// <summary>
        /// Get the source value to convert.
        /// </summary>
        public SingleValueNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Get the type we're converting to.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.typeReference; }
        }

        /// <summary>
        /// Get the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.Convert;
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