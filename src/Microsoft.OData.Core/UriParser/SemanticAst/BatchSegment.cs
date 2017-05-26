//---------------------------------------------------------------------
// <copyright file="BatchSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// A segment representing $batch
    /// </summary>
    public sealed class BatchSegment : ODataPathSegment
    {
        /// <summary>
        /// Gets the singleton instance of the batch segment.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "BatchSegment is immutable")]
        public static readonly BatchSegment Instance = new BatchSegment();

        /// <summary>
        /// Build a segment to represent $batch.
        /// </summary>
        private BatchSegment()
        {
            this.Identifier = UriQueryConstants.BatchSegment;
            this.TargetKind = RequestTargetKind.Batch;
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="BatchSegment"/>, which is always null.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return null; }
        }

        /// <summary>
        /// Translate a <see cref="BatchSegment"/> into something else using an implementation of <see cref="PathSegmentTranslator{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after translating this segment.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input translator is null.</exception>
        public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
        {
            ExceptionUtils.CheckArgumentNotNull(translator, "translator");
            return translator.Translate(this);
        }

        /// <summary>
        /// Handle a <see cref="BatchSegment"/> using an implementation of <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the Handler interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void HandleWith(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "handler");
            handler.Handle(this);
        }

        /// <summary>
        /// Check if this segment is equal to another segment.
        /// </summary>
        /// <param name="other">The other segment to check.</param>
        /// <returns>True if the other segment is equivalent to this one.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input other is null</exception>
        internal override bool Equals(ODataPathSegment other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            return other is BatchSegment;
        }
    }
}
