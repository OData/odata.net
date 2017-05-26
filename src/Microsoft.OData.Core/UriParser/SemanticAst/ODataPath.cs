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
        private readonly IList<ODataPathSegment> segments;

        /// <summary>
        /// Creates a new instance of <see cref="ODataPath"/> containing the given segments.
        /// </summary>
        /// <param name="segments">The segments that make up the path.</param>
        /// <exception cref="ArgumentNullException">Throws if input segments is null.</exception>
        public ODataPath(IEnumerable<ODataPathSegment> segments)
        {
            ExceptionUtils.CheckArgumentNotNull(segments, "segments");
            this.segments = segments.ToList();
            if (this.segments.Any(s => s == null))
            {
                throw Error.ArgumentNull("segments");
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
        /// Gets the first segment in the path. Returns null if the path is empty.
        /// </summary>
        public ODataPathSegment FirstSegment
        {
            get
            {
                return this.segments.Count == 0 ? null : this.segments[0];
            }
        }

        /// <summary>
        /// Get the last segment in the path. Returns null if the path is empty.
        /// </summary>
        public ODataPathSegment LastSegment
        {
            get
            {
                return this.segments.Count == 0 ? null : this.segments[this.segments.Count - 1];
            }
        }

        /// <summary>
        /// Get the number of segments in this path.
        /// </summary>
        public int Count
        {
            get { return this.segments.Count; }
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
            foreach (ODataPathSegment segment in this.segments)
            {
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

        /// <summary>
        /// Add a segment to this path.
        /// </summary>
        /// <param name="newSegment">the segment to add</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input newSegment is null.</exception>
        internal void Add(ODataPathSegment newSegment)
        {
            ExceptionUtils.CheckArgumentNotNull(newSegment, "newSegment");
            this.segments.Add(newSegment);
        }
    }
}
