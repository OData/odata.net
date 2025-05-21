//---------------------------------------------------------------------
// <copyright file="RootPathNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// The $root literal can be used in expressions to refer to resources of the same service. It can be used as a single-valued expression or within complex or collection literals.
    /// </summary>
    public sealed class RootPathNode : SingleValueNode
    {
        /// <summary>
        /// Created a RootPathNode with the given path and the given type.
        /// </summary>
        /// <param name="path">The OData path.</param>
        /// <param name="typeRef">The path type. It could be null if the last segment is dynamic.</param>
        public RootPathNode(ODataPath path, IEdmTypeReference typeRef)
        {
            ExceptionUtils.CheckArgumentNotNull(path, "path");
            Path = path;
            TypeReference = typeRef;
        }

        /// <summary>
        /// Gets the OData path.
        /// </summary>
        public ODataPath Path { get;}

        /// <summary>
        /// Gets the type of the single value this node represents.
        /// </summary>
        public override IEdmTypeReference TypeReference { get; }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.RootPath;

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