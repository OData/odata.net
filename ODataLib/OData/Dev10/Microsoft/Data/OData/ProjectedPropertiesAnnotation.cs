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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Annotation which stores a list of projected properties for an entry.
    /// </summary>
    public sealed class ProjectedPropertiesAnnotation
    {
        /// <summary>
        /// Cached projected properties annotation with no properties projected.
        /// </summary>
        private static readonly ProjectedPropertiesAnnotation emptyProjectedPropertiesMarker = new ProjectedPropertiesAnnotation(new string[0]);

        /// <summary>
        /// A hash set with the projected property names.
        /// </summary>
        private readonly HashSet<string> projectedProperties;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="projectedPropertyNames">Enumeration of projected property names.</param>
        public ProjectedPropertiesAnnotation(IEnumerable<string> projectedPropertyNames)
        {
            ExceptionUtils.CheckArgumentNotNull(projectedPropertyNames, "projectedPropertyNames");

            this.projectedProperties = new HashSet<string>(projectedPropertyNames, StringComparer.Ordinal);
        }

        /// <summary>
        /// Projected properties annotation with no properties projected.
        /// </summary>
        internal static ProjectedPropertiesAnnotation EmptyProjectedPropertiesMarker
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return emptyProjectedPropertiesMarker;
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
            DebugUtils.CheckNoExternalCallers();

            return this.projectedProperties.Contains(propertyName);
        }
    }
}
