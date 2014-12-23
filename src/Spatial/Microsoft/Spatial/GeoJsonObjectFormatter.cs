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

namespace Microsoft.Spatial
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.Spatial;

    /// <summary>Represents a formatter for Json object.</summary>
    public abstract class GeoJsonObjectFormatter
    {
        /// <summary>Creates the implementation of the formatter.</summary>
        /// <returns>The created <see cref="T:Microsoft.Spatial.GeoJsonObjectFormatter" /> implementation.</returns>
        public static GeoJsonObjectFormatter Create()
        {
            return SpatialImplementation.CurrentImplementation.CreateGeoJsonObjectFormatter();
        }

        /// <summary>Reads from the source.</summary>
        /// <returns>The <see cref="T:Microsoft.Spatial.GeoJsonObjectFormatter" /> object that was read.</returns>
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
