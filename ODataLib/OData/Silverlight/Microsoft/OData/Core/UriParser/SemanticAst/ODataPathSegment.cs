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
    using System.Diagnostics;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// The semantic representation of a segment in a path.
    /// </summary>
    public abstract class ODataPathSegment : ODataAnnotatable
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

#region Temporary Internal Properties
        /// <summary>Returns the identifier for this segment i.e. string part without the keys.</summary>
        internal string Identifier { get; set; }

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
        /// Translate a <see cref="ODataPathSegment"/> using an implemntation of<see cref="PathSegmentTranslator{T}"/>.
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
                this.TargetKind == RequestTargetKind.OpenProperty ||
                this is OperationSegment ||
                this is OperationImportSegment,
                "All resource targets (except for some service operations and open properties) should have a container.");
            Debug.Assert(!string.IsNullOrEmpty(this.Identifier), "identifier must not be empty or null.");
        }
#endif

        /// <summary>
        /// Copies over all the values of the internal-only properties from one segment to another.
        /// </summary>
        /// <param name="other">Ther segment to copy from.</param>
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
