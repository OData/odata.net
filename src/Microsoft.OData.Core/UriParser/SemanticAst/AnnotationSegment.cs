//---------------------------------------------------------------------
// <copyright file="AnnotationSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Edm.Vocabularies;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// A segment representing an Annotation
    /// </summary>
    public sealed class AnnotationSegment : ODataPathSegment
    {
        /// <summary>
        /// The annotation term referred to by this segment
        /// </summary>
        private readonly IEdmTerm term;

        /// <summary>
        /// Build a segment based on an annotation term
        /// </summary>
        /// <param name="term">The annotation term that this segment represents.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input annotation term is null.</exception>
        public AnnotationSegment(IEdmTerm term)
        {
            ExceptionUtils.CheckArgumentNotNull(term, "term");

            this.term = term;

            this.Identifier = term.Name;
            this.TargetEdmType = term.Type.Definition;
            this.SingleResult = !term.Type.IsCollection();
        }

        /// <summary>
        /// Gets the annotation term that this segment represents.
        /// </summary>
        public IEdmTerm Term
        {
            get { return this.term; }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="AnnotationSegment"/>.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.term.Type.Definition; }
        }

        /// <summary>
        /// Translate a <see cref="AnnotationSegment"/> using an instance of <see cref="PathSegmentTranslator{T}"/>/>.
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
        /// Handle a <see cref="AnnotationSegment"/> using an instance of <see cref="PathSegmentHandler"/>.
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
            AnnotationSegment otherTerm = other as AnnotationSegment;
            return otherTerm != null && otherTerm.term == this.term;
        }
    }
}
