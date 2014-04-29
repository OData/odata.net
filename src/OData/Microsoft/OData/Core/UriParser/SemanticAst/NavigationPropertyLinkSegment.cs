//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using System;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;

    #endregion Namespaces

    /// <summary>
    /// A segment representing $ref
    /// </summary>
    public sealed class NavigationPropertyLinkSegment : ODataPathSegment
    {
        /// <summary>
        /// The navigation property this link or ref acts on.
        /// </summary>
        private readonly IEdmNavigationProperty navigationProperty;

        /// <summary>
        /// Build a segment to represnt $ref on a Nav prop
        /// </summary>
        /// <param name="navigationProperty">The navigaiton property this link or ref acts on</param>
        /// <param name="navigationSource">The navigation source of entities linked to by this <see cref="NavigationPropertyLinkSegment"/>. This can be null.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input navigationProperty is null.</exception>
        public NavigationPropertyLinkSegment(IEdmNavigationProperty navigationProperty, IEdmNavigationSource navigationSource) 
        {
            ExceptionUtils.CheckArgumentNotNull(navigationProperty, "navigationProperty");
            this.navigationProperty = navigationProperty;
            this.TargetEdmNavigationSource = navigationSource;

            this.Identifier = navigationProperty.Name;
            this.TargetEdmType = navigationProperty.Type.Definition;
            this.SingleResult = !navigationProperty.Type.IsCollection();
            this.TargetKind = RequestTargetKind.Resource;
        }

        /// <summary>
        /// Gets the navigation property this link or ref acts on.
        /// </summary>
        public IEdmNavigationProperty NavigationProperty
        {
            get { return this.navigationProperty; }
        }

        /// <summary>
        /// Gets the navigation source of entities linked to by this <see cref="NavigationPropertyLinkSegment"/>.
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get { return this.TargetEdmNavigationSource; }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="NavigationPropertyLinkSegment"/>.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.navigationProperty.Type.Definition; }
        }

        /// <summary>
        /// Translate a <see cref="PathSegmentTranslator{T}"/>
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after visiting this token.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input translator is null.</exception>
        public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
        {
            ExceptionUtils.CheckArgumentNotNull(translator, "translator");
            return translator.Translate(this);
        }

        /// <summary>
        /// Translate a <see cref="PathSegmentHandler"/> to walk a tree of <see cref="ODataPathSegment"/>s.
        /// </summary>
        /// <param name="handler">An implementation of the translator interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void HandleWith(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "handler");
            handler.Handle(this);
        }

        /// <summary>
        /// Check if this segment is equal to another.
        /// </summary>
        /// <param name="other">The other segment to check.</param>
        /// <returns>True if the other segment is equal.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input other is null.</exception>
        internal override bool Equals(ODataPathSegment other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            NavigationPropertyLinkSegment otherLinkSegment = other as NavigationPropertyLinkSegment;
            return otherLinkSegment != null && otherLinkSegment.NavigationProperty == this.navigationProperty;
        }
    }
}
