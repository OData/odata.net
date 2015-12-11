﻿//---------------------------------------------------------------------
// <copyright file="SingleNavigationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces
    using System;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// Node representing a single navigation property.
    /// </summary>
    public sealed class SingleNavigationNode : SingleEntityNode
    {
        /// <summary>
        /// The navigation source that this NavigationProperty targets.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

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

            EdmMultiplicity multiplicity = navigationProperty.TargetMultiplicity();
            if (multiplicity != EdmMultiplicity.One && multiplicity != EdmMultiplicity.ZeroOrOne)
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity);
            }

            this.source = source;
            this.navigationProperty = navigationProperty;
            this.entityTypeReference = (IEdmEntityTypeReference)this.NavigationProperty.Type;

            this.navigationSource = source.NavigationSource != null ? source.NavigationSource.FindNavigationTarget(navigationProperty) : null;
        }

        /// <summary>
        /// Constructs a SingleNavigationNode.
        /// </summary>
        /// <param name="navigationProperty">The navigation property this node represents.</param>
        /// <param name="sourceNavigationSource">The navigation source that this of the previous segment.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input navigationProperty or source is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input navigationProperty targets more than one entity.</exception>
        public SingleNavigationNode(IEdmNavigationProperty navigationProperty, IEdmNavigationSource sourceNavigationSource)
        {
            ExceptionUtils.CheckArgumentNotNull(navigationProperty, "navigationProperty");

            EdmMultiplicity multiplicity = navigationProperty.TargetMultiplicity();
            if (multiplicity != EdmMultiplicity.One && multiplicity != EdmMultiplicity.ZeroOrOne)
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity);
            }

            this.navigationProperty = navigationProperty;
            this.entityTypeReference = (IEdmEntityTypeReference)this.NavigationProperty.Type;

            this.navigationSource = sourceNavigationSource != null ? sourceNavigationSource.FindNavigationTarget(navigationProperty) : null;
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
            get { return this.NavigationProperty.TargetMultiplicity(); }
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
        /// Gets the navigation source that this NavigationProperty targets.
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
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}