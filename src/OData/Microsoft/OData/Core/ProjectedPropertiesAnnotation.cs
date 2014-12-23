//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Annotation which stores a list of projected properties for an entry.
    /// </summary>
    public sealed class ProjectedPropertiesAnnotation
    {
        /// <summary>The special '*' segment indicating that all properties are selected.</summary>
        internal const string StarSegment = "*";

        /// <summary>
        /// Cached projected properties annotation with no properties projected.
        /// </summary>
        private static readonly ProjectedPropertiesAnnotation emptyProjectedPropertiesMarker = new ProjectedPropertiesAnnotation(new string[0]);

        /// <summary>
        /// Cached projected properties annotation with all properties projected.
        /// </summary>
        private static readonly ProjectedPropertiesAnnotation allProjectedPropertiesMarker = new ProjectedPropertiesAnnotation(new string[] { StarSegment });
        
        /// <summary>
        /// A hash set with the projected property names.
        /// </summary>
        private readonly HashSet<string> projectedProperties;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Core.ProjectedPropertiesAnnotation" /> class.</summary>
        /// <param name="projectedPropertyNames">The enumeration of projected property names.</param>
        public ProjectedPropertiesAnnotation(IEnumerable<string> projectedPropertyNames)
        {
            ExceptionUtils.CheckArgumentNotNull(projectedPropertyNames, "projectedPropertyNames");

            this.projectedProperties = new HashSet<string>(projectedPropertyNames, StringComparer.Ordinal);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ProjectedPropertiesAnnotation()
        {
            this.projectedProperties = new HashSet<string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Projected properties annotation with no properties projected.
        /// </summary>
        internal static ProjectedPropertiesAnnotation EmptyProjectedPropertiesInstance
        {
            get
            {
                return emptyProjectedPropertiesMarker;
            }
        }

        /// <summary>
        /// Projected properties annotation with all properties projected.
        /// </summary>
        internal static ProjectedPropertiesAnnotation AllProjectedPropertiesInstance
        {
            get
            {
                return allProjectedPropertiesMarker;
            }
        }

        /// <summary>
        /// The set of projected property names.
        /// </summary>
        internal IEnumerable<string> ProjectedProperties
        {
            get
            {
                return this.projectedProperties;
            }
        }

        /// <summary>
        /// Determines if a property is in a list of projected properties.
        /// </summary>
        /// <param name="propertyName">The name of the property to lookup.</param>
        /// <returns>true if the property is projected; false otherwise.</returns>
        /// <remarks>Note that we allow null and empty property names here for the lookup just so that
        /// we don't have to validate before we skip them. If we would not skip them, the writing would fail later on anyway.</remarks>
        internal bool IsPropertyProjected(string propertyName)
        {
            return this.projectedProperties.Contains(propertyName);
        }

        /// <summary>
        /// Adds the specified property name to the set of projected properties (if it is not already included).
        /// </summary>
        /// <param name="propertyName">The name of the property to include in the set of projected properties.</param>
        internal void Add(string propertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(!object.ReferenceEquals(ProjectedPropertiesAnnotation.EmptyProjectedPropertiesInstance, this), "Must not add properties to the empty singleton instance.");

            if (object.ReferenceEquals(ProjectedPropertiesAnnotation.AllProjectedPropertiesInstance, this))
            {
                return;
            }

            if (!this.projectedProperties.Contains(propertyName))
            {
                this.projectedProperties.Add(propertyName);
            }
        }

        /// <summary>
        /// Removes the specified property name from the set of projected properties.
        /// </summary>
        /// <param name="propertyName">The name of the property to remove from the set of projected properties.</param>
        internal void Remove(string propertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(this.projectedProperties.Contains(propertyName), "this.projectedProperties.Contains(propertyName)");

            this.projectedProperties.Remove(propertyName);
        }
    }
}
