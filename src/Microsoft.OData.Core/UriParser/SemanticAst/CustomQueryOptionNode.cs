//---------------------------------------------------------------------
// <copyright file="CustomQueryOptionNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Base class for all custom query options.
    /// </summary>
    public class CustomQueryOptionNode : QueryNode
    {
        /// <summary>
        /// Create a NameValueCustomQueryOptionNode
        /// </summary>
        /// <param name="name">This node's primitive value. May be null.</param>
        /// <param name="value">The literal text for this node's value. May be null.</param>
        public CustomQueryOptionNode(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// Gets the Query Option Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the Query Option Value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        public override QueryNodeKind Kind => (QueryNodeKind)this.InternalKind;

        /// <summary>
        /// Gets the kind of the query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.CustomQueryOption;

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