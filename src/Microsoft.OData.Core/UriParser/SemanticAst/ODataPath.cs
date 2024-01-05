//---------------------------------------------------------------------
// <copyright file="ODataPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    #endregion Namespaces

    /// <summary>
    /// A representation of the path portion of an OData URI which is made up of <see cref="ODataPathSegment"/>s.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "ODataPathCollection just doesn't sound right")]
    public class ODataPath : IEnumerable<ODataPathSegment>
    {
        /// <summary>
        /// The segments that make up this path.
        /// </summary>
        private readonly List<ODataPathSegment> segments;

        /// <summary>
        /// Creates a new instance of <see cref="ODataPath"/> containing the given segments.
        /// </summary>
        /// <param name="segments">The segments that make up the path.</param>
        /// <exception cref="ArgumentNullException">Throws if input segments is null.</exception>
        public ODataPath(IEnumerable<ODataPathSegment> segments)
        {
            ExceptionUtils.CheckArgumentNotNull(segments, "segments");
            this.segments = new List<ODataPathSegment>(segments);
            if (this.segments.Contains(null))
            {
                throw Error.ArgumentNull(nameof(segments));
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ODataPath"/> containing the given segments.
        /// </summary>
        /// <param name="segments">The segments that make up the path.</param>
        /// <exception cref="ArgumentNullException">Throws if input segments is null.</exception>
        public ODataPath(params ODataPathSegment[] segments)
            : this((IEnumerable<ODataPathSegment>)segments)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="ODataPath"/> by copying segments
        /// from an existing <see cref="ODataPath"/>.
        /// </summary>
        /// <param name="odataPath">The instance to copy segments from.</param>
        internal ODataPath(ODataPath odataPath)
        {
            ExceptionUtils.CheckArgumentNotNull(odataPath, nameof(odataPath));
            // We mostly clone the path when we want to add a new segment.
            // We allocate additional capacity to ensure the new segment will
            // be added without requiring a resize.
            this.segments = new List<ODataPathSegment>(odataPath.segments.Count + 1);
            this.segments.AddRange(odataPath.segments);

            // We don't need to check for null segments since
            // the input ODataPath must have already validated the segments
        }

        /// <summary>
        /// Creates a new instance of <see cref="ODataPath"/> from the
        /// specified <paramref name="segments"/>. This does not
        /// copy the segments list.
        /// </summary>
        /// <remarks>
        /// This should be used in internal scenarios where we're sure
        /// the input list will not be used by the caller after the ODataPath
        /// is instantiated.
        /// </remarks>
        /// <param name="segments">The segments that make up the path.</param>
        /// <param name="checkNullSegments">Whether to perform validation for null segments. To improve performance, set to false when you're sure
        /// the input does not contain null segments.</param>
        private ODataPath(List<ODataPathSegment> segments, bool checkNullSegments = true)
        {
            ExceptionUtils.CheckArgumentNotNull(segments, "segments");
            this.segments = segments;
            if (checkNullSegments && this.segments.Contains(null))
            {
                throw Error.ArgumentNull(nameof(segments));
            }
        }

        /// <summary>
        /// Efficiently creates a new instance of <see cref="ODataPath"/> from the
        /// specified <paramref name="segments"/> list without creating an internal copy.
        /// </summary>
        /// <remarks>
        /// This should be used in internal scenarios where we're sure
        /// the input list will not be used by the caller after the ODataPath
        /// is instantiated.
        /// </remarks>
        /// <param name="segments">The segments that make up the path.</param>
        /// <param name="verifySegmentsNotNull">Whether to perform validation for null segments. To improve performance, set to false when you're sure
        /// the input does not contain null segments.</param>
        /// <returns>A new instance of <see cref="ODataPath"/></returns>
        internal static ODataPath CreateFromListWithoutCopying(List<ODataPathSegment> segments, bool verifySegmentsNotNull = true)
        {
            var path = new ODataPath(segments, verifySegmentsNotNull);
            return path;
        }

        /// <summary>
        /// Gets the first segment in the path. Returns null if the path is empty.
        /// </summary>
        public ODataPathSegment FirstSegment
        {
            get
            {
                return this.segments.Count > 0 ? this.segments[0] : null;
            }
        }

        /// <summary>
        /// Get the last segment in the path. Returns null if the path is empty.
        /// </summary>
        public ODataPathSegment LastSegment
        {
            get
            {
                return this.segments.Count > 0 ? this.segments[this.segments.Count - 1] : null;
            }
        }

        /// <summary>
        /// Get the segment at the specified index.
        /// </summary>
        /// <param name="index">The index of the segment to retrieve</param>
        public ODataPathSegment this[int index] => this.segments[index];

        /// <summary>
        /// Get the number of segments in this path.
        /// </summary>
        public int Count
        {
            get { return this.segments.Count; }
        }

        /// <summary>
        /// Get the List of ODataPathSegments.
        /// </summary>
        internal IReadOnlyList<ODataPathSegment> Segments
        {
            get { return this.segments; }
        }

        /// <summary>
        /// Get the segments enumerator
        /// </summary>
        /// <returns>The segments enumerator</returns>
        public IEnumerator<ODataPathSegment> GetEnumerator()
        {
            return this.segments.GetEnumerator();
        }

        /// <summary>
        /// get the segments enumerator
        /// </summary>
        /// <returns>The segments enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Walk this path using a translator
        /// </summary>
        /// <typeparam name="T">the return type of the translator</typeparam>
        /// <param name="translator">a user defined translation path</param>
        /// <returns>an enumerable containing user defined objects for each segment</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "We would rather ship the PathSegmentTranslator so that its more extensible later")]
        public IEnumerable<T> WalkWith<T>(PathSegmentTranslator<T> translator)
        {
            return this.segments.Select(segment => segment.TranslateWith(translator));
        }

        /// <summary>
        /// Walk this path using a handler
        /// </summary>
        /// <param name="handler">the handler that will be applied to each segment</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "We would rather ship the PathSegmentHandler so that its more extensible later")]
        public void WalkWith(PathSegmentHandler handler)
        {
            int segmentsCount = this.segments.Count;
            for (var index = 0; index < segmentsCount; index++)
            {
                ODataPathSegment segment = this.segments[index];
                segment.HandleWith(handler);
            }
        }

        /// <summary>
        /// Checks if this path is equal to another path.
        /// </summary>
        /// <param name="other">The other path to compare it to</param>
        /// <returns>True if the two paths are equal</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input other is null.</exception>
        internal bool Equals(ODataPath other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            if (this.segments.Count != other.segments.Count)
            {
                return false;
            }

            return !this.segments.Where((t, i) => !t.Equals(other.segments[i])).Any();
        }
    }
}
