//---------------------------------------------------------------------
// <copyright file="ResourceConstantNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// The JSON object syntax allows for key-value pairs, where the key is a string literal,
    /// and the value could be primitive, enum, complex, or entity, or collection of them.
    /// For example: { 'name': 'abc', 'address': { 'city': 'Redmond', 'state': 'WA' }, 'phoneNumbers': ['425-882-8080', '425-882-8081'] }
    /// Be noted, the token maybe single quoted allowed for back-compatibility. For JSON syntax, the token should be double quoted.
    /// Be noted, the node itself is a single value node. It represents a single value with a collection of key-value pairs in the semantic tree.
    /// </summary>
    public sealed class ResourceConstantNode : SingleValueNode
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ResourceConstantNode"/> class.
        /// </summary>
        /// <param name="typeReference">The expected structured type for this resource constant node. It could be null.</param>
        public ResourceConstantNode(IEdmStructuredTypeReference typeReference)
        {
            ExpectedStructuredType = typeReference;
        }

        /// <summary>
        /// Gets the read-only collection of the properties in this resource constant node.
        /// </summary>
        public IList<KeyValuePair<string, QueryNode>> Properties { get; } = new List<KeyValuePair<string, QueryNode>>();

        /// <summary>
        /// Gets the expected type of this node. This could be null.
        /// </summary>
        public override IEdmTypeReference TypeReference => ExpectedStructuredType;

        /// <summary>
        /// Gets the structured type of this resource constant node. This could be null.
        /// If have, this type could be from literal "@odata.type" or from the metadata if no "@odata.type" in the literal.
        /// </summary>
        public IEdmStructuredTypeReference ExpectedStructuredType { get; }

        /// <summary>
        /// Get or Set the literal text for this node's value. It could be null.
        /// </summary>
        public ReadOnlyMemory<char> LiteralText { get; set; }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.ResourceConstant;

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