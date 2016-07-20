//---------------------------------------------------------------------
// <copyright file="ODataPathSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System.Diagnostics;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// The semantic representation of a segment in a path.
    /// </summary>
    public abstract class ODataPathSegment
    {
        /// <summary>
        /// Creates a new Segment and copies values from another Segment.
        /// </summary>
        /// <param name="other">Segment to copy values from.</param>
        internal ODataPathSegment(ODataPathSegment other)
        {
            this.CopyValuesFrom(other);
        }

        /// <summary>
        /// Creates a new Segment.
        /// </summary>
        protected ODataPathSegment()
        {
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="ODataPathSegment"/>.
        /// </summary>
        /// <remarks>This property can be null. Not all segments have a Type, such as a <see cref="BatchSegment"/>.</remarks>
        public abstract IEdmType EdmType { get; }

        /// <summary>Returns the identifier for this segment i.e. string part without the keys.</summary>
        public string Identifier { get; set; }

        #region Temporary Internal Properties
        /// <summary>Whether the segment targets a single result or not.</summary>
        internal bool SingleResult { get; set; }

        /// <summary>The navigation source targeted by this segment. Can be null.</summary>
        internal IEdmNavigationSource TargetEdmNavigationSource { get; set; }

        /// <summary>The type targeted by this segment. Can be null.</summary>
        internal IEdmType TargetEdmType { get; set; }

        /// <summary>The kind of resource targeted by this segment.</summary>
        internal RequestTargetKind TargetKind { get; set; }
        #endregion

        /// <summary>
        /// Translate a <see cref="ODataPathSegment"/> using an implementation of<see cref="PathSegmentTranslator{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after visiting this token.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        public abstract T TranslateWith<T>(PathSegmentTranslator<T> translator);

        /// <summary>
        /// Handle a <see cref="ODataPathSegment"/> using an implementation of a <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handler interface.</param>
        public abstract void HandleWith(PathSegmentHandler handler);

        /// <summary>
        /// Check if this segment is equal to another segment.
        /// </summary>
        /// <param name="other">the other segment to check</param>
        /// <returns>true if the segments are equal.</returns>
        internal virtual bool Equals(ODataPathSegment other)
        {
            return ReferenceEquals(this, other);
        }

#if DEBUG
        /// <summary>In DEBUG builds, ensures that invariants for the class hold.</summary>
        internal void AssertValid()
        {
            Debug.Assert(Enum.IsDefined(typeof(RequestTargetKind), this.TargetKind), "enum value is not valid");
            Debug.Assert(
                this.TargetKind != RequestTargetKind.Resource ||
                this.TargetEdmNavigationSource != null ||
                this.TargetEdmType != null ||
                this.EdmType != null ||
                this.TargetKind == RequestTargetKind.Dynamic ||
                this is OperationSegment ||
                this is OperationImportSegment,
                "All resource targets (except for some service operations and open properties) should have a container.");
            Debug.Assert(!string.IsNullOrEmpty(this.Identifier), "identifier must not be empty or null.");
        }
#endif

        /// <summary>
        /// Copies over all the values of the internal-only properties from one segment to another.
        /// </summary>
        /// <param name="other">The segment to copy from.</param>
        internal void CopyValuesFrom(ODataPathSegment other)
        {
            Debug.Assert(other != null, "other != null");
            this.Identifier = other.Identifier;
            this.SingleResult = other.SingleResult;
            this.TargetEdmNavigationSource = other.TargetEdmNavigationSource;
            this.TargetKind = other.TargetKind;
            this.TargetEdmType = other.TargetEdmType;
        }
    }
}
