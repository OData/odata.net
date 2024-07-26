//---------------------------------------------------------------------
// <copyright file="EdmTargetPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
    /// <inheritdoc/>
    public class EdmTargetPath : IEdmTargetPath
    {
        private List<IEdmElement> segments;
        private string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTargetPath"/> class.
        /// </summary>
        /// <param name="segments">Target path segments.</param>
        public EdmTargetPath(IEnumerable<IEdmElement> segments)
        {
            EdmUtil.CheckArgumentNull(segments, nameof(segments));

            this.segments = segments.ToList();

            ValidateSegments();

            this.path = this.GetPathString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTargetPath"/> class.
        /// </summary>
        /// <param name="segments">Target path segments.</param>
        public EdmTargetPath(params IEdmElement[] segments)
            :this(segments.AsEnumerable())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTargetPath"/> class.
        /// </summary>
        /// <param name="segments">Target path segments.</param>
        /// <param name="path">Target path string.</param>
        internal EdmTargetPath(IEnumerable<IEdmElement> segments, string path)
        {
            EdmUtil.CheckArgumentNull(segments, nameof(segments));
            EdmUtil.CheckArgumentNull(path, nameof(path));

            this.segments = segments.ToList();

            ValidateSegments();

            this.path = path;
        }

        /// <inheritdoc/>
        public IReadOnlyList<IEdmElement> Segments
        {
            get { return this.segments; }
        }

        /// <inheritdoc/>
        public string Path
        {
            get
            {
                if (this.path == null)
                {
                    this.path = this.GetPathString();
                }

                return this.path;
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            EdmTargetPath other = obj as EdmTargetPath;

            return string.Equals(this.Path, other.Path, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.path.GetHashCode(StringComparison.Ordinal);
        }

        private void ValidateSegments()
        {
            // Validate no segment is null
            if (this.segments.Contains(null))
            {
                throw new ArgumentException(Strings.TargetPath_SegmentsMustNotContainNullSegment);
            }

            // Validate that the first element is Container.

            if (this.segments.Count > 0 && !(this.segments[0] is IEdmEntityContainer))
            {
                throw new InvalidOperationException(Strings.TargetPath_FirstSegmentMustBeIEdmEntityContainer);
            }

            // Validate the second element is a Container element.
            if (this.segments.Count > 1 && !(this.segments[1] is IEdmEntityContainerElement))
            {
                throw new InvalidOperationException(Strings.TargetPath_SecondSegmentMustBeIEdmEntityContainerElement);
            }
        }
    }
}
