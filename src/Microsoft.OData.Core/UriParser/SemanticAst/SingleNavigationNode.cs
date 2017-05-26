//---------------------------------------------------------------------
// <copyright file="SingleNavigationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
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
        private readonly SingleResourceNode source;

        /// <summary>
        /// The navigation property this node represents.
        /// </summary>
        private readonly IEdmNavigationProperty navigationProperty;

        /// <summary>
        /// The type of entity that this NavigationProperty targets.
        /// </summary>
        private readonly IEdmEntityTypeReference entityTypeReference;

        /// <summary>
        /// The parsed segments in the path.
        /// </summary>
        private readonly List<ODataPathSegment> parsedSegments;

        /// <summary>
        /// The binding path of current navigation property.
        /// </summary>
        private readonly IEdmPathExpression bindingPath;

        /// <summary>
        /// Constructs a SingleNavigationNode.
        /// </summary>
        /// <param name="source">The previous node in the path.</param>
        /// <param name="navigationProperty">The navigation property this node represents.</param>
        /// <param name="bindingPath">The binding path of navigation property</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input navigationProperty or source is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input navigationProperty targets more than one entity.</exception>
        public SingleNavigationNode(SingleResourceNode source, IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
            : this(ExceptionUtils.CheckArgumentNotNull(source, "source").NavigationSource, navigationProperty, bindingPath)
        {
            this.source = source;
        }

        /// <summary>
        /// Constructs a SingleNavigationNode.
        /// </summary>
        /// <param name="navigationSource">The navigation source that this of the previous segment.</param>
        /// <param name="navigationProperty">The navigation property this node represents.</param>
        /// <param name="bindingPath">The binding path of navigation property</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input navigationProperty.</exception>
        /// <exception cref="ArgumentException">Throws if the input navigationProperty targets more than one entity.</exception>
        internal SingleNavigationNode(IEdmNavigationSource navigationSource, IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
        {
            ExceptionUtils.CheckArgumentNotNull(navigationProperty, "navigationProperty");

            EdmMultiplicity multiplicity = navigationProperty.TargetMultiplicity();
            if (multiplicity != EdmMultiplicity.One && multiplicity != EdmMultiplicity.ZeroOrOne)
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity);
            }

            this.navigationProperty = navigationProperty;
            this.entityTypeReference = (IEdmEntityTypeReference)this.NavigationProperty.Type;
            this.bindingPath = bindingPath;
            this.navigationSource = navigationSource != null ? navigationSource.FindNavigationTarget(navigationProperty, bindingPath) : null;
        }

        /// <summary>
        /// Constructs a SingleNavigationNode.
        /// </summary>
        /// <param name="source">he previous node in the path.</param>
        /// <param name="navigationProperty">The navigation property this node represents.</param>
        /// <param name="segments">The path segments parsed in path and query option.</param>
        internal SingleNavigationNode(SingleResourceNode source, IEdmNavigationProperty navigationProperty, List<ODataPathSegment> segments)
            : this(ExceptionUtils.CheckArgumentNotNull(source, "source").NavigationSource, navigationProperty, segments)
        {
            this.source = source;
        }

        /// <summary>
        /// Constructs a SingleNavigationNode.
        /// </summary>
        /// <param name="navigationSource">The navigation source that this of the previous segment.</param>
        /// <param name="navigationProperty">The navigation property this node represents.</param>
        /// <param name="segments">The path segments parsed in path and query option.</param>
        private SingleNavigationNode(IEdmNavigationSource navigationSource,
            IEdmNavigationProperty navigationProperty, List<ODataPathSegment> segments)
        {
            ExceptionUtils.CheckArgumentNotNull(navigationProperty, "navigationProperty");

            EdmMultiplicity multiplicity = navigationProperty.TargetMultiplicity();
            if (multiplicity != EdmMultiplicity.One && multiplicity != EdmMultiplicity.ZeroOrOne)
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity);
            }

            this.navigationProperty = navigationProperty;
            this.entityTypeReference = (IEdmEntityTypeReference)this.NavigationProperty.Type;
            this.parsedSegments = segments;
            this.navigationSource = navigationSource != null ? navigationSource.FindNavigationTarget(navigationProperty, BindingPathHelper.MatchBindingPath, this.parsedSegments, out this.bindingPath) : null;
        }

        /// <summary>
        /// Gets the previous node in the path.
        /// </summary>
        public SingleResourceNode Source
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
        /// Gets the structured type of entity that this NavigationProperty targets.
        /// </summary>
        public override IEdmStructuredTypeReference StructuredTypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// The binding path of current navigation property.
        /// </summary>
        public IEdmPathExpression BindingPath
        {
            get { return bindingPath; }
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