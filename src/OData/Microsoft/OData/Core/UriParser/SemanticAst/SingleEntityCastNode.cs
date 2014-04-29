//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm.Library;

    #endregion Namespaces

    /// <summary>
    /// Node representing a type segment that casts a single entity parent node.
    /// </summary>
    public sealed class SingleEntityCastNode : SingleEntityNode
    {
        /// <summary>
        /// The entity that we're casting to a different type.
        /// </summary>
        private readonly SingleEntityNode source;

        /// <summary>
        /// The target type that the source is cast to.
        /// </summary>
        private readonly IEdmEntityTypeReference entityTypeReference;

        /// <summary>
        /// The navigation source containing the source entity.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Created a SingleEntityCastNode with the given source node and the given type to cast to.
        /// </summary>
        /// <param name="source"> Source <see cref="SingleValueNode"/> that is being cast.</param>
        /// <param name="entityType">Type to cast to.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input entityType is null.</exception>
        public SingleEntityCastNode(SingleEntityNode source, IEdmEntityType entityType)
        {
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            this.source = source;
            this.navigationSource = source != null ? source.NavigationSource : null;
            this.entityTypeReference = new EdmEntityTypeReference(entityType, false);
        }

        /// <summary>
        /// Gets the entity that we're casting to a different type.
        /// </summary>
        public SingleEntityNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the target type that the source is cast to.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the target type that the source is cast to.
        /// </summary>
        public override IEdmEntityTypeReference EntityTypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the navigation source containing the source entity..
        /// </summary>
        public override IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Gets the kind of this query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.SingleEntityCast;
            }
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
