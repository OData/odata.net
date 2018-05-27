//---------------------------------------------------------------------
// <copyright file="DynamicPathSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    #endregion Namespaces

    /// <summary>
    /// A segment representing an unknown path or an open property.
    /// </summary>
    public sealed class DynamicPathSegment : ODataPathSegment
    {
        /// <summary>
        /// Build a segment to represent an unknown path or an open property.
        /// </summary>
        /// <param name="identifier">The identifier of the dynamic path segment</param>
        public DynamicPathSegment(string identifier)
        {
            ExceptionUtils.CheckArgumentNotNull(identifier, "identifier");
            this.Identifier = identifier;
            this.TargetEdmType = null;
            this.TargetKind = RequestTargetKind.Dynamic;
            this.SingleResult = true;
        }

        /// <summary>
        /// Build a segment to represent an unknown path or an open property.
        /// </summary>
        /// <param name="identifier">The identifier of the dynamic path segment.</param>
        /// <param name="edmType">the IEdmType of this segment</param>
        /// <param name="navigationSource">The navigation source targeted by this segment. Can be null.</param>
        /// <param name="singleResult">Whether the segment targets a single result or not.</param>
        public DynamicPathSegment(string identifier, IEdmType edmType, IEdmNavigationSource navigationSource, bool singleResult)
        {
            ExceptionUtils.CheckArgumentNotNull(identifier, "identifier");
            this.Identifier = identifier;
            this.TargetEdmType = edmType;
            this.SingleResult = singleResult;
            this.TargetKind = edmType == null ? RequestTargetKind.Dynamic : edmType.GetTargetKindFromType();
            this.TargetEdmNavigationSource = navigationSource;
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="DynamicPathSegment"/>, which might be null.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.TargetEdmType; }
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
        /// Handle a <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handler interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void HandleWith(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "handler");
            handler.Handle(this);
        }

        /// <summary>
        /// Check if this segment is equal to another segment.
        /// </summary>
        /// <param name="other">the other segment to check.</param>
        /// <returns>true if the other segment is equal.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input other is null.</exception>
        internal override bool Equals(ODataPathSegment other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            DynamicPathSegment otherDynmaicPathSegment = other as DynamicPathSegment;
            return otherDynmaicPathSegment != null
                && otherDynmaicPathSegment.Identifier == this.Identifier
                && otherDynmaicPathSegment.EdmType == this.EdmType
                && otherDynmaicPathSegment.TargetEdmNavigationSource == this.TargetEdmNavigationSource
                && otherDynmaicPathSegment.SingleResult == this.SingleResult;
        }
    }
}
