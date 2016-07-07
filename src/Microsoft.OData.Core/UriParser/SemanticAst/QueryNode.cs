//---------------------------------------------------------------------
// <copyright file="QueryNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;

    /// <summary>
    /// Base class for all semantic metadata bound nodes.
    /// </summary>
    public abstract class QueryNode
    {
        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        public abstract QueryNodeKind Kind { get; }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal virtual InternalQueryNodeKind InternalKind
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walks a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public virtual T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}