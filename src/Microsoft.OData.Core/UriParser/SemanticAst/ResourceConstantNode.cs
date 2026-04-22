//---------------------------------------------------------------------
// <copyright file="ResourceConstantNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// The JSON object syntax allows for key-value pairs, where the key is a string literal,
    /// and the value could be primitive, enum, complex, or entity, or collection of them.
    /// For example: { 'name': 'abc', 'address': { 'city': 'Redmond', 'state': 'WA' }, 'phoneNumbers': ['425-882-8080', '425-882-8081'] }
    /// Be noted, the token is single quoted for back-compatibility. For JSON syntax, the token should be double quoted.
    /// Be noted, the node itself is a single value node. It represents such a collection of key-value pairs in the semantic tree.
    /// </summary>
    public sealed class ResourceConstantNode : SingleValueNode
    {
        /// <summary>
        /// Collection of properties.
        /// </summary>
        private IList<KeyValuePair<string, QueryNode>> _properties = new List<KeyValuePair<string, QueryNode>>();

        /// <summary>
        /// Creates a new instance of the <see cref="ResourceConstantNode"/> class.
        /// </summary>
        /// <param name="typeReference">The expected structured type for this resource constant node. It could be null.</param>
        /// <param name="literal">The literal text for this node's value. It could be null.</param>
        public ResourceConstantNode(IEdmStructuredTypeReference typeReference, ReadOnlyMemory<char> literal)
        {
            ExpectedStructuredType = typeReference;
            LiteralText = literal;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ResourceConstantNode"/> class.
        /// </summary>
        /// <param name="typeReference">The expected structured type for this resource constant node. It could be null.</param>
        /// <param name="literal">The literal text for this node's value. It could be null.</param>
        internal ResourceConstantNode(IEdmStructuredType structuredType, ReadOnlyMemory<char> literal)
        {
            if (structuredType != null)
            {
                ExpectedStructuredType = structuredType.ToTypeReference(true) as IEdmStructuredTypeReference;
            }

            LiteralText = literal;
        }

        /// <summary>
        /// Adds a single value constant property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">The property value.</param>
        public void Add(string propertyName, ConstantNode propertyValue)
            => AddProperty(propertyName, propertyValue);

        /// <summary>
        /// Adds a resource constant property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">The property value.</param>
        public void Add(string propertyName, ResourceConstantNode propertyValue)
            => AddProperty(propertyName, propertyValue);

        /// <summary>
        /// Adds a collection constant property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">The property value.</param>
        public void Add(string propertyName, CollectionConstantNode propertyValue)
             => AddProperty(propertyName, propertyValue);

        internal void AddProperty(string propertyName, QueryNode propertyValue)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(propertyName, nameof(propertyName));
            ExceptionUtils.CheckArgumentNotNull(propertyValue, nameof(propertyValue));

            if (propertyValue.Kind == QueryNodeKind.CollectionConstant ||
                propertyValue.Kind == QueryNodeKind.ResourceConstant)
            {
                this._properties.Add(new KeyValuePair<string, QueryNode>(propertyName, propertyValue));
            }

            QueryNode source = propertyValue;
            while (source != null && source.Kind == QueryNodeKind.Convert)
            {
                source = ((ConvertNode)source).Source;
            }

            // Constant or Convert(Constant)
            if (source != null && source.Kind == QueryNodeKind.Constant)
            {
                this._properties.Add(new KeyValuePair<string, QueryNode>(propertyName, propertyValue));
            }

            throw new ODataException("TODO: ");
        }

        /// <summary>
        /// Gets the read only collection of the properties in this resource constant node.
        /// </summary>
        public IReadOnlyCollection<KeyValuePair<string, QueryNode>> Properties => _properties.AsReadOnly();

        /// <summary>
        /// Gets the expected type of this node. This could be null.
        /// </summary>
        public override IEdmTypeReference TypeReference => ExpectedStructuredType;

        /// <summary>
        /// Gets the expected structured type of this resource constant node. This could be null.
        /// </summary>
        public IEdmStructuredTypeReference ExpectedStructuredType { get; }

        /// <summary>
        /// Get or Set the literal text for this node's value. It could be null.
        /// </summary>
        public ReadOnlyMemory<char> LiteralText { get; }

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