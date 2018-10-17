//---------------------------------------------------------------------
// <copyright file="EachSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// A segment representing $each in a path.
    /// </summary>
    public sealed class EachSegment : ODataPathSegment
    {
        /// <summary>
        /// The <see cref="IEdmType"/> of the resource that this <see cref="EachSegment"/> represents.
        /// </summary>
        private readonly IEdmType edmType;

        /// <summary>
        /// Build a segment representing $each.
        /// </summary>
        /// <param name="navigationSource">The entity collection that this set-based operation applies to.</param>
        /// <param name="targetEdmType">Target type for the entity being referenced.</param>
        /// <exception cref="System.ArgumentNullException">Throws if any input parameter is null.</exception>
        /// <remarks>$each cannot be applied on singletons.</remarks>
        public EachSegment(IEdmNavigationSource navigationSource, IEdmType targetEdmType)
        {
            ExceptionUtils.CheckArgumentNotNull(navigationSource, "navigationSource");
            ExceptionUtils.CheckArgumentNotNull(targetEdmType, "targetEdmType");

            this.Identifier = UriQueryConstants.EachSegment;
            this.SingleResult = false;
            this.TargetEdmNavigationSource = navigationSource;
            this.TargetEdmType = targetEdmType;
            this.TargetKind = targetEdmType.GetTargetKindFromType();

            this.edmType = navigationSource.Type;
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> to which this <see cref="EachSegment"/> applies.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.edmType; }
        }

        /// <summary>
        /// Translate a <see cref="EachSegment"/> using an instance of <see cref="PathSegmentTranslator{T}"/>.
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
        /// Handle a <see cref="EachSegment"/> using an instance of <see cref="PathSegmentHandler"/>.
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
            EachSegment otherSegment = other as EachSegment;

            return otherSegment != null &&
                otherSegment.TargetEdmNavigationSource == this.TargetEdmNavigationSource;
        }
    }
}
