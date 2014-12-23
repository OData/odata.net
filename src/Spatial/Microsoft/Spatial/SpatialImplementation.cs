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
    using Microsoft.Data.Spatial;

    /// <summary>
    /// Class responsible for knowing how to create the Geography and Geometry builders for 
    /// a particular implemenation of Spatial types
    /// </summary>
    public abstract class SpatialImplementation
    {
        /// <summary>Default Spatial Implementation.</summary>
        private static SpatialImplementation spatialImplementation = new DataServicesSpatialImplementation();

        /// <summary> Returns an instance of SpatialImplementation that is currently being used. </summary>
        public static SpatialImplementation CurrentImplementation
        {
            get
            {
                return spatialImplementation;
            }

            // Intended for use by tests and DataContractSerialization
            internal set
            {
                spatialImplementation = value;
            }
        }

        /// <summary>Gets or sets the Spatial operations implementation.</summary>
        public abstract SpatialOperations Operations
        {
            get;
            set;
        }

        /// <summary> Creates a SpatialBuilder for this implementation.</summary>
        /// <returns>The SpatialBuilder created.</returns>
        public abstract SpatialBuilder CreateBuilder();

        /// <summary> Creates a Formatter for Json Object.</summary>
        /// <returns>The JsonObjectFormatter created.</returns>
        public abstract GeoJsonObjectFormatter CreateGeoJsonObjectFormatter();

        /// <summary> Creates a GmlFormatter for this implementation.</summary>
        /// <returns>The GmlFormatter created.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml", Justification = "Gml is the common name for this format")]
        public abstract GmlFormatter CreateGmlFormatter();

        /// <summary> Creates a WellKnownTextSqlFormatter for this implementation.</summary>
        /// <returns>The WellKnownTextSqlFormatter created.</returns>
        public abstract WellKnownTextSqlFormatter CreateWellKnownTextSqlFormatter();

        /// <summary> Creates a WellKnownTextSqlFormatter for this implementation.</summary>
        /// <returns>The WellKnownTextSqlFormatter created.</returns>
        /// <param name="allowOnlyTwoDimensions">Controls the writing and reading of the Z and M dimension.</param>
        public abstract WellKnownTextSqlFormatter CreateWellKnownTextSqlFormatter(bool allowOnlyTwoDimensions);

        /// <summary> Creates a spatial Validator.</summary>
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
