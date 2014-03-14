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
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Node representing a single navigation property.
    /// </summary>
    public sealed class SingleNavigationNode : SingleEntityNode
    {
        /// <summary>
        /// The entity set that this NavigationProperty targets.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// The previous node in the path.
        /// </summary>
        private readonly SingleEntityNode source;

        /// <summary>
        /// The navigation property this node represents.
        /// </summary>
        private readonly IEdmNavigationProperty navigationProperty;

        /// <summary>
        /// The type of entity that this NavigationProperty targets.
        /// </summary>
        private readonly IEdmEntityTypeReference entityTypeReference;

        /// <summary>
        /// Constructs a SingleNavigationNode.
        /// </summary>
        /// <param name="navigationProperty">The navigation property this node represents.</param>
        /// <param name="source">The previous node in the path.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input navigationProperty or source is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input navigationProperty targets more than one entity.</exception>
        public SingleNavigationNode(IEdmNavigationProperty navigationProperty, SingleEntityNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(navigationProperty, "navigationProperty");
            ExceptionUtils.CheckArgumentNotNull(source, "source");

            EdmMultiplicity multiplicity = navigationProperty.TargetMultiplicityTemporary();
            if (multiplicity != EdmMultiplicity.One && multiplicity != EdmMultiplicity.ZeroOrOne)
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity);
            }

            this.source = source;
            this.navigationProperty = navigationProperty;
            this.entityTypeReference = (IEdmEntityTypeReference)this.NavigationProperty.Type;
            this.entitySet = source.EntitySet != null ? source.EntitySet.FindNavigationTarget(navigationProperty) : null;
        }

        /// <summary>
        /// Constructs a SingleNavigationNode.
        /// </summary>
        /// <param name="navigationProperty">The navigation property this node represents.</param>
        /// <param name="sourceSet">The entity set that this of the previous segment.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input navigationProperty or source is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input navigationProperty targets more than one entity.</exception>
        public SingleNavigationNode(IEdmNavigationProperty navigationProperty, IEdmEntitySet sourceSet)
        {
            ExceptionUtils.CheckArgumentNotNull(navigationProperty, "navigationProperty");

            EdmMultiplicity multiplicity = navigationProperty.TargetMultiplicityTemporary();
            if (multiplicity != EdmMultiplicity.One && multiplicity != EdmMultiplicity.ZeroOrOne)
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity);
            }

            this.navigationProperty = navigationProperty;
            this.entityTypeReference = (IEdmEntityTypeReference)this.NavigationProperty.Type;
            this.entitySet = sourceSet != null ? sourceSet.FindNavigationTarget(navigationProperty) : null;
        }

        /// <summary>
        /// Gets the previous node in the path.
        /// </summary>
        public SingleEntityNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the navigation property this node represents.
        /// </summary>
        public IEdmNavigationProperty NavigationProperty
        {
            get { return this.navigationProperty; }
        }

        /// <summary>
        /// Gets the target multiplicity.
        /// </summary>
        public EdmMultiplicity TargetMultiplicity
        {
            get { return this.NavigationProperty.TargetMultiplicityTemporary(); }
        }

        /// <summary>
        /// Gets the type of entity that this NavigationProperty targets.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the type of entity that this NavigationProperty targets.
        /// </summary>
        public override IEdmEntityTypeReference EntityTypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the entity set that this NavigationProperty targets.
        /// </summary>
        public override IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the kind of this query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return InternalQueryNodeKind.SingleNavigationNode;
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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
