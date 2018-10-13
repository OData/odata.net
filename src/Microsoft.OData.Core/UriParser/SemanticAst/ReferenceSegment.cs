//---------------------------------------------------------------------
// <copyright file="ReferenceSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// A segment representing $ref in a path.
    /// </summary>
    public sealed class ReferenceSegment : ODataPathSegment
    {
        /// <summary>
        /// Build a segment representing $ref.
        /// </summary>
        /// <param name="navigationSource">The navigation source to which this segment references.</param>
        /// <exception cref="System.ArgumentNullException">Throws if any input parameter is null.</exception>
        public ReferenceSegment(IEdmNavigationSource navigationSource)
        {
            ExceptionUtils.CheckArgumentNotNull(navigationSource, "navigationSource");

            this.Identifier = UriQueryConstants.RefSegment;
            this.SingleResult = navigationSource.Type.TypeKind != EdmTypeKind.Collection;
            this.TargetEdmNavigationSource = navigationSource;
            this.TargetKind = RequestTargetKind.Resource;
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="ReferenceSegment"/>.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return null; }
        }

        /// <summary>
        /// Translate a <see cref="ReferenceSegment"/> using an instance of <see cref="PathSegmentTranslator{T}"/>.
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
        /// Handle a <see cref="ReferenceSegment"/> using an instance of <see cref="PathSegmentHandler"/>.
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
        /// <exception cref="System.ArgumentNullException">throws if the input other is null.</exception>
        internal override bool Equals(ODataPathSegment other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            ReferenceSegment otherSegment = other as ReferenceSegment;

            return otherSegment != null && otherSegment.TargetEdmNavigationSource == this.TargetEdmNavigationSource;
        }
    }
}
