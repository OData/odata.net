//---------------------------------------------------------------------
// <copyright file="GeoJsonObjectFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Collections.Generic;

    /// <summary>Represents a formatter for Json object.</summary>
    public abstract class GeoJsonObjectFormatter
    {
        /// <summary>Creates the implementation of the formatter.</summary>
        /// <returns>The created <see cref="Microsoft.Spatial.GeoJsonObjectFormatter" /> implementation.</returns>
        public static GeoJsonObjectFormatter Create()
        {
            return SpatialImplementation.CurrentImplementation.CreateGeoJsonObjectFormatter();
        }

        /// <summary>Reads from the source.</summary>
        /// <returns>The <see cref="Microsoft.Spatial.GeoJsonObjectFormatter" /> object that was read.</returns>
        /// <param name="source">The source json object.</param>
        /// <typeparam name="T">The spatial type to read.</typeparam>
        public abstract T Read<T>(IDictionary<String, Object> source) where T : class, ISpatial;

        /// <summary>Converts spatial value to a Json object.</summary>
        /// <returns>The json object.</returns>
        /// <param name="value">The spatial value.</param>
        public abstract IDictionary<String, Object> Write(ISpatial value);

        /// <summary> Creates the writerStream. </summary>
        /// <returns>The writerStream that was created.</returns>
        /// <param name="writer">The actual stream to write Json.</param>
        public abstract SpatialPipeline CreateWriter(IGeoJsonWriter writer);
    }
}
