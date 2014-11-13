//   OData .NET Libraries ver. 6.8.1
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
    using System;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.UriParser.Semantic;
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

            EdmMultiplicity multiplicity = navigationProperty.TargetMultiplicityTemporary();
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

            EdmMultiplicity multiplicity = navigationProperty.TargetMultiplicityTemporary();
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
