//---------------------------------------------------------------------
// <copyright file="IndexSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Globalization;
using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// A segment representing an index to the collection (Primitive, Enum, Complex).
    /// </summary>
    public sealed class IndexSegment : ODataPathSegment
    {
        /// <summary>
        /// Build a segment representing an index.
        /// </summary>
        /// <param name="index">The index value.</param>
        /// <param name="targetEdmType">The target type of the segment.</param>
        public IndexSegment(long index, IEdmType targetEdmType)
        {
            ExceptionUtils.CheckArgumentNotNull(targetEdmType, "targetEdmType");

            Index = index;
            this.Identifier = index.ToString(CultureInfo.InvariantCulture);
            this.TargetEdmType = targetEdmType;
            this.TargetKind = targetEdmType.IsODataPrimitiveTypeKind() ?
                RequestTargetKind.Primitive :
                (targetEdmType.IsODataEnumTypeKind() ? RequestTargetKind.Enum : RequestTargetKind.Resource);
            this.SingleResult = true;
            EdmType = targetEdmType;
        }

        /// <summary>
        /// Gets the index value.
        /// </summary>
        public long Index { get; }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> to which this <see cref="IndexSegment"/> applies.
        /// </summary>
        public override IEdmType EdmType { get; }

        /// <summary>
        /// Translate an <see cref="IndexSegment"/> into another type using an instance of <see cref="PathSegmentTranslator{T}"/>.
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
        /// Handle an <see cref="IndexSegment"/> using the an instance of the <see cref="PathSegmentHandler"/>.
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
            IndexSegment otherIndex = other as IndexSegment;
            return otherIndex != null && otherIndex.Index == this.Index && otherIndex.EdmType.IsEquivalentTo(this.EdmType);
        }
    }
}
