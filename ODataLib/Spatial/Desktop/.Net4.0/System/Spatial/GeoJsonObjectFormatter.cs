//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

    /// <summary>Represents a formatter for Json object.</summary>
    public abstract class GeoJsonObjectFormatter
    {
        /// <summary>Creates the implementation of the formatter.</summary>
        /// <returns>The created <see cref="T:System.Spatial.GeoJsonObjectFormatter" /> implementation.</returns>
        public static GeoJsonObjectFormatter Create()
        {
            return SpatialImplementation.CurrentImplementation.CreateGeoJsonObjectFormatter();
        }

        /// <summary>Reads from the source.</summary>
        /// <returns>The <see cref="T:System.Spatial.GeoJsonObjectFormatter" /> object that was read.</returns>
        /// <param name="source">The source json object.</param>
        /// <typeparam name="T">The spatial type to read.</typeparam>
        public abstract T Read<T>(IDictionary<String, Object> source) where T : class, ISpatial;

        /// <summary>Converts spatial value to a Json object.</summary>
        /// <returns>The json object.</returns>
        /// <param name="value">The spatial value.</param>
        public abstract IDictionary<String, Object> Write(ISpatial value);
    }
}
