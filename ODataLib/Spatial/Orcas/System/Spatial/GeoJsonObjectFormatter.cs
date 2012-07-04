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

namespace System.Spatial
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Formatter for Json Object
    /// </summary>
    public abstract class GeoJsonObjectFormatter
    {
        /// <summary>
        /// Creates the implementation of the formatter
        /// </summary>
        /// <returns>Returns the created GeoJsonFormatter implementation</returns>
        public static GeoJsonObjectFormatter Create()
        {
            return SpatialImplementation.CurrentImplementation.CreateGeoJsonObjectFormatter();
        }

        /// <summary>
        /// Read from the source
        /// </summary>
        /// <typeparam name="T">The spatial type to read</typeparam>
        /// <param name="source">The source json object</param>
        /// <returns>The read instance</returns>
        public abstract T Read<T>(IDictionary<String, Object> source) where T : class, ISpatial;

        /// <summary>
        /// Convert spatial value to a Json Object
        /// </summary>
        /// <param name="value">The spatial value</param>
        /// <returns>The json object</returns>
        public abstract IDictionary<String, Object> Write(ISpatial value);
    }
}
