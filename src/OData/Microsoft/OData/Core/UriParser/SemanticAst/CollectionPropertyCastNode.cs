//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Node representing a type segment that casts an collection property node.
    /// </summary>
    public sealed class CollectionPropertyCastNode : CollectionNode
    {
        /// <summary>
        /// The collection property node that we're casting.
        /// </summary>
        private readonly CollectionPropertyAccessNode source;

        /// <summary>
        /// The target type that we're casting our collection node to.
        /// </summary>
        private readonly IEdmComplexTypeReference edmTypeReference;

        /// <summary>
        /// the type of the collection returned by this function
        /// </summary>
        private readonly IEdmCollectionTypeReference collectionTypeReference;

        /// <summary>
        /// Create a CollectionPropertyCastNode with the given source node and the given target type.
        /// </summary>
        /// <param name="source">Parent <see cref="CollectionPropertyAccessNode"/> that is being cast.</param>
        /// <param name="complexType">Type to cast to.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input source or complexType are null.</exception>
        public CollectionPropertyCastNode(CollectionPropertyAccessNode source, IEdmComplexType complexType)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");
            ExceptionUtils.CheckArgumentNotNull(complexType, "complexType");
            this.source = source;
            this.edmTypeReference = new EdmComplexTypeReference(complexType, false);

            // creating a new collection type here because the type in the request is just the item type, there is no user-provided collection type.
            this.collectionTypeReference = EdmCoreModel.GetCollection(this.edmTypeReference);
        }

        /// <summary>
        /// Gets the collection property node that we're casting.
        /// </summary>
        public CollectionPropertyAccessNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the type that we're casting all items in this collection to.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get { return this.edmTypeReference; }
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
                return InternalQueryNodeKind.CollectionPropertyCast;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walk a tree of <see cref="QueryNode"/>s.
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
