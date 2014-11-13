//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Node representing an entity set.
    /// TODO: This should be deleted but it is used in many, many tests.
    /// </summary>
    internal sealed class EntitySetNode : EntityCollectionNode
    {
        /// <summary>
        /// The entity set this node represents.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// The resouce type of a single entity in the entity set.
        /// </summary>
        private readonly IEdmEntityTypeReference entityType;

        /// <summary>
        /// the type of the collection returned by this function
        /// </summary>
        private readonly IEdmCollectionTypeReference collectionTypeReference;

        /// <summary>
        /// Creates an <see cref="EntitySetNode"/>
        /// </summary>
        /// <param name="entitySet">The entity set this node represents</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input entitySet is null.</exception>
        public EntitySetNode(IEdmEntitySet entitySet)
        {
            ExceptionUtils.CheckArgumentNotNull(entitySet, "entitySet");
            this.entitySet = entitySet;
            this.entityType = new EdmEntityTypeReference(UriEdmHelpers.GetEntitySetElementType(this.EntitySet), false);
            this.collectionTypeReference = EdmCoreModel.GetCollection(this.entityType);
        }

        /// <summary>
        /// Gets the resouce type of a single entity in the entity set.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get
            {
                return this.entityType;
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
        /// Gets the resouce type of a single entity in the entity set.
        /// </summary>
        public override IEdmEntityTypeReference EntityItemType
        {
            get { return this.entityType; }
        }

        /// <summary>
        /// Gets the entity set this node represents.
        /// </summary>
        public override IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the kind for this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return InternalQueryNodeKind.EntitySet;
            }
        }
    }
}
