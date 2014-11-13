//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.Data.Spatial
{
    using Microsoft.Spatial;

    /// <summary>
    /// Class responsible for knowing how to create the Geography and Geometry builders for 
    /// the data services implemenation of Spatial types
    /// </summary>
    internal class DataServicesSpatialImplementation : SpatialImplementation
    {
        /// <summary>
        /// Property used to register Spatial operations implementation.
        /// </summary>
        public override SpatialOperations Operations
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a SpatialBuilder for this implemenation
        /// </summary>
        /// <returns>
        /// The SpatialBuilder created.
        /// </returns>
        public override SpatialBuilder CreateBuilder()
        {
            var geography = new GeographyBuilderImplementation(this);
            var geometry = new GeometryBuilderImplementation(this);
            var input = new ForwardingSegment(geography, geometry);
            return new SpatialBuilder(input, input, geography, geometry);
        }

        /// <summary>
        /// Creates a GmlFormatter for this implementation
        /// </summary>
        /// <returns>The GmlFormatter created.</returns>
        public override GmlFormatter CreateGmlFormatter()
        {
            return new GmlFormatterImplementation(this);
        }

        /// <summary>
        /// Creates a GeoJsonObjectFormatter for this implementation
        /// </summary>
        /// <returns>The GeoJsonObjectFormatter created.</returns>
        public override GeoJsonObjectFormatter CreateGeoJsonObjectFormatter()
        {
            return new GeoJsonObjectFormatterImplementation(this);
        }

        /// <summary>
        /// Creates a WellKnownTextSqlFormatter for this implementation
        /// </summary>
        /// <returns>The WellKnownTextSqlFormatter created.</returns>
        public override WellKnownTextSqlFormatter CreateWellKnownTextSqlFormatter()
        {
            return new WellKnownTextSqlFormatterImplementation(this);
        }

        /// <summary>
        /// Creates a WellKnownTextSqlFormatter for this implementation
        /// </summary>
        /// <param name="allowOnlyTwoDimensions">Controls the writing and reading of the Z and M dimension</param>
        /// <returns>
        /// The WellKnownTextSqlFormatter created.
        /// </returns>
        public override WellKnownTextSqlFormatter CreateWellKnownTextSqlFormatter(bool allowOnlyTwoDimensions)
        {
            return new WellKnownTextSqlFormatterImplementation(this, allowOnlyTwoDimensions);
        }

        /// <summary>
        /// Creates a SpatialValidator for this implementation
        /// </summary>
        /// <returns>The SpatialValidator created.</returns>
        public override SpatialPipeline CreateValidator()
        {
            return new ForwardingSegment(new SpatialValidatorImplementation());
        }
    }
}
