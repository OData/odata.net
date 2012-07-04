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
    using Microsoft.Data.Spatial;

    /// <summary>
    /// Class responsible for knowing how to create the Geography and Geometry builders for 
    /// a particular implemenation of Spatial types
    /// </summary>
    public abstract class SpatialImplementation
    {
        /// <summary>Default Spatial Implementation.</summary>
        private static SpatialImplementation spatialImplementation = new DataServicesSpatialImplementation();

        /// <summary>
        /// Returns an instance of SpatialImplementation that is currently being used.
        /// </summary>
        public static SpatialImplementation CurrentImplementation
        {
            get
            {
                return spatialImplementation;
            }

            // Intended for use by tests only
            internal set
            {
                spatialImplementation = value;
            }
        }

        /// <summary>
        /// Property used to register Spatial operations implementation.
        /// </summary>
        public abstract SpatialOperations Operations
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a SpatialBuilder for this implemenation
        /// </summary>
        /// <returns>The SpatialBuilder created.</returns>
        public abstract SpatialBuilder CreateBuilder();

        /// <summary>
        /// Creates a Formatter for Json Object
        /// </summary>
        /// <returns>The JsonObjectFormatter created</returns>
        public abstract GeoJsonObjectFormatter CreateGeoJsonObjectFormatter();

        /// <summary>
        /// Creates a GmlFormatter for this implementation
        /// </summary>
        /// <returns>The GmlFormatter created.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml", Justification = "Gml is the common name for this format")]
        public abstract GmlFormatter CreateGmlFormatter();

        /// <summary>
        /// Creates a WellKnownTextSqlFormatter for this implementation
        /// </summary>
        /// <returns>The WellKnownTextSqlFormatter created.</returns>
        public abstract WellKnownTextSqlFormatter CreateWellKnownTextSqlFormatter();

        /// <summary>
        /// Creates a WellKnownTextSqlFormatter for this implementation
        /// </summary>
        /// <param name="allowOnlyTwoDimensions">Controls the writing and reading of the Z and M dimension</param>
        /// <returns>The WellKnownTextSqlFormatter created.</returns>
        public abstract WellKnownTextSqlFormatter CreateWellKnownTextSqlFormatter(bool allowOnlyTwoDimensions);

        /// <summary>
        /// Creates a spatial Validator
        /// </summary>
        /// <returns>The SpatialValidator created.</returns>
        public abstract SpatialPipeline CreateValidator();

        /// <summary>
        /// This method throws if the operations instance is null. It returns a non-null operations implementation.
        /// </summary>
        /// <returns>a SpatialOperations implementation.</returns>
        internal SpatialOperations VerifyAndGetNonNullOperations()
        {
            var operations = this.Operations;
            if (operations == null)
            {
                throw new NotImplementedException(Strings.SpatialImplementation_NoRegisteredOperations);
            }

            return operations;
        }
    }
}
