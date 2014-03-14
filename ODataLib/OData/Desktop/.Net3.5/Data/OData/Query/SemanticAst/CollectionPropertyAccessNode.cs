//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces

    using System;
    using Microsoft.Data.Edm;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// Node representing an access to a collection property value.
    /// </summary>
    public sealed class CollectionPropertyAccessNode : CollectionNode
    {
        /// <summary>
        /// The value containing the property.
        /// </summary>
        private readonly SingleValueNode source;

        /// <summary>
        /// The EDM property which is to be accessed.
        /// </summary>
        /// <remarks>Only non-entity, collection properties are supported by this node.</remarks>
        private readonly IEdmProperty property;

        /// <summary>
        /// The resouce type of a single item from the collection represented by this node.
        /// </summary>
        private readonly IEdmTypeReference itemType;

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        private readonly IEdmCollectionTypeReference collectionTypeReference;

        /// <summary>
        /// Constructs a new <see cref="CollectionPropertyAccessNode"/>.
        /// </summary>
        /// <param name="source">The value containing the property.</param>
        /// <param name="property">The EDM property which is to be accessed.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input source or property is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input property is not a collection of structural properties</exception>
        public CollectionPropertyAccessNode(SingleValueNode source, IEdmProperty property)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            if (property.PropertyKind != EdmPropertyKind.Structural)
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_PropertyAccessShouldBeNonEntityProperty(property.Name));
            }

            if (!property.Type.IsCollection())
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_PropertyAccessTypeMustBeCollection(property.Name));
            }

            this.source = source;
            this.property = property;
            this.collectionTypeReference = property.Type.AsCollection();
            this.itemType = this.collectionTypeReference.ElementType();
        }

        /// <summary>
        /// Gets the value containing the property.
        /// </summary>
        public SingleValueNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the EDM property which is to be accessed.
        /// </summary>
        /// <remarks>Only non-entity, collection properties are supported by this node.</remarks>
        public IEdmProperty Property
        {
            get { return this.property; }
        }

        /// <summary>
        /// Gets the resouce type of a single item from the collection represented by this node.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get
            {
                return this.itemType;
            }
        }

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        public override IEdmCollectionTypeReference CollectionType
        {
            get { return this.collectionTypeReference; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return InternalQueryNodeKind.CollectionPropertyAccess;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> to walk a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input visitor is null.</exception>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
