//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
