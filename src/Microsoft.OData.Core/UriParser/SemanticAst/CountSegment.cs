//---------------------------------------------------------------------
// <copyright file="CountSegment.cs" company="Microsoft">
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
    /// A segment representing $count in a path
    /// </summary>
    public sealed class CountSegment : ODataPathSegment
    {
        /// <summary>
        /// Return the singleton instance of Count
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "CountSegment is immutable")]
        public static readonly CountSegment Instance = new CountSegment();

        /// <summary>
        /// Build a segment representing $count
        /// </summary>
        private CountSegment()
        {
            this.Identifier = UriQueryConstants.CountSegment;
            this.SingleResult = true;
            this.TargetKind = RequestTargetKind.PrimitiveValue;
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="CountSegment"/>, which is always Edm.Int32.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return EdmCoreModel.Instance.GetInt32(false).Definition; }
        }

        /// <summary>
        /// Translate a <see cref="CountSegment"/> using an instance of <see cref="PathSegmentTranslator{T}"/>.
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
        /// Handle a <see cref="CountSegment"/> using an instance of <see cref="PathSegmentHandler"/>.
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
            return other is CountSegment;
        }
    }
}
