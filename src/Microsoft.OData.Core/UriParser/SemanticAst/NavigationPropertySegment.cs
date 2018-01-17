//---------------------------------------------------------------------
// <copyright file="NavigationPropertySegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// A segment representing a navigation property
    /// </summary>
    public sealed class NavigationPropertySegment : ODataPathSegment
    {
        /// <summary>
        /// The navigation property this segment represents.
        /// </summary>
        private readonly IEdmNavigationProperty navigationProperty;

        /// <summary>
        /// Build a segment representing a navigation property.
        /// </summary>
        /// <param name="navigationProperty">The navigation property this segment represents.</param>
        /// <param name="navigationSource">The navigation source of the entities targetted by this navigation property. This can be null.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input navigationProperty is null.</exception>
        public NavigationPropertySegment(IEdmNavigationProperty navigationProperty, IEdmNavigationSource navigationSource)
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
        /// Gets the navigation property represented by this NavigationPropertySegment.
        /// </summary>
        public IEdmNavigationProperty NavigationProperty
        {
            get { return this.navigationProperty; }
        }

        /// <summary>
        /// Gets the navigation source of the entities targetted by this Navigation Property.
        /// This can be null.
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get { return this.TargetEdmNavigationSource; }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="NavigationPropertySegment"/>.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.navigationProperty.Type.Definition; }
        }

        /// <summary>
        /// Translate a <see cref="PathSegmentTranslator{T}"/>.
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
        /// Checks if this segment is equal to another segment.
        /// </summary>
        /// <param name="other">the other segment to check.</param>
        /// <returns>true if the other segment is equal.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input other is null.</exception>
        internal override bool Equals(ODataPathSegment other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            NavigationPropertySegment otherNavPropSegment = other as NavigationPropertySegment;
            return otherNavPropSegment != null && otherNavPropSegment.NavigationProperty == this.NavigationProperty;
        }
    }
}
