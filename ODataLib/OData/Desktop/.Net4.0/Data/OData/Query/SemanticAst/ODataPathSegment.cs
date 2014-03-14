//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using Microsoft.Data.Edm;

    #endregion Namespaces

    /// <summary>
    /// The semantic representation of a segment in a path.
    /// </summary>
    public abstract class ODataPathSegment : ODataAnnotatable
    {
        /// <summary>Returns the identifier for this segment i.e. string part without the keys.</summary>
        private string identifier;

        /// <summary>Indicates whether this segment targets a single result or not.</summary>
        private bool singleResult;

        /// <summary>The entity set targetted by this segment. Can be null.</summary>
        private IEdmEntitySet targetEdmEntitySet;

        /// <summary>The type targetted by this segment. Can be null.</summary>
        private IEdmType targetEdmType;

        /// <summary>The kind of resource targeted by this segment.</summary>
        private RequestTargetKind targetKind;

        /// <summary>
        /// Creates a new Segment and copies values from another Segment.
        /// </summary>
        /// <param name="other">Segment to copy values from.</param>
        internal ODataPathSegment(ODataPathSegment other)
        {
            DebugUtils.CheckNoExternalCallers();
            this.CopyValuesFrom(other);
        }

        /// <summary>
        /// Creates a new Segment.
        /// </summary>
        protected ODataPathSegment()
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="ODataPathSegment"/>.
        /// </summary>
        /// <remarks>This property can be null. Not all segments have a Type, such as a <see cref="BatchSegment"/>.</remarks>
        public abstract IEdmType EdmType { get; }

#region Temporary Internal Properties
        /// <summary>Returns the identifier for this segment i.e. string part without the keys.</summary>
        internal string Identifier
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.identifier;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.identifier = value;
            }
        }

        /// <summary>Whether the segment targets a single result or not.</summary>
        internal bool SingleResult
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.singleResult;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.singleResult = value;
            }
        }

        /// <summary>The entity set targetted by this segment. Can be null.</summary>
        internal IEdmEntitySet TargetEdmEntitySet
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.targetEdmEntitySet;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.targetEdmEntitySet = value;
            }
        }

        /// <summary>The type targetted by this segment. Can be null.</summary>
        internal IEdmType TargetEdmType
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.targetEdmType;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.targetEdmType = value;
            }
        }

        /// <summary>The kind of resource targeted by this segment.</summary>
        internal RequestTargetKind TargetKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.targetKind;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.targetKind = value;
            }
        }
#endregion

        /// <summary>
        /// Translate a <see cref="ODataPathSegment"/> using an implemntation of<see cref="PathSegmentTranslator{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after visiting this token.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        public abstract T Translate<T>(PathSegmentTranslator<T> translator);

        /// <summary>
        /// Handle a <see cref="ODataPathSegment"/> using an implementation of a <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handler interface.</param>
        public abstract void Handle(PathSegmentHandler handler);

        /// <summary>
        /// Check if this segment is equal to another segment.
        /// </summary>
        /// <param name="other">the other segment to check</param>
        /// <returns>true if the segments are equal.</returns>
        internal virtual bool Equals(ODataPathSegment other)
        {
            DebugUtils.CheckNoExternalCallers();
            return ReferenceEquals(this, other);
        }

#if DEBUG
        /// <summary>In DEBUG builds, ensures that invariants for the class hold.</summary>
        internal void AssertValid()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(Enum.IsDefined(typeof(RequestTargetKind), this.TargetKind), "enum value is not valid");
            Debug.Assert(
                this.TargetKind != RequestTargetKind.Resource ||
                this.TargetEdmEntitySet != null || 
                this.TargetKind == RequestTargetKind.OpenProperty ||
                this is OperationSegment,
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(other != null, "other != null");
            this.Identifier = other.Identifier;
            this.SingleResult = other.SingleResult;
            this.TargetEdmEntitySet = other.TargetEdmEntitySet;
            this.TargetKind = other.TargetKind;
            this.TargetEdmType = other.TargetEdmType;
        }
    }
}
