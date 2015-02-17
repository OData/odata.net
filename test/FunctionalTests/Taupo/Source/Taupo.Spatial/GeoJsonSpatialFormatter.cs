//---------------------------------------------------------------------
// <copyright file="GeoJsonSpatialFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial
{
    using System.Collections.Generic;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Default GeoJSON object formatter for spatial values
    /// </summary>
    [ImplementationName(typeof(IGeoJsonSpatialFormatter), "Default")]
    public class GeoJsonSpatialFormatter : IGeoJsonSpatialFormatter
    {
        /// <summary>
        /// Tries to convert the given spatial instance to the formatted representation
        /// </summary>
        /// <param name="spatialInstance">The spatial instance to format</param>
        /// <param name="formatted">The formatted spatial value</param>
        /// <returns>True if the value was spatial and could be formatted, otherwise false</returns>
        public bool TryConvert(object spatialInstance, out IDictionary<string, object> formatted)
        {
            var spatial = spatialInstance as ISpatial;
            if (spatial != null)
            {
                formatted = GeoJsonObjectFormatter.Create().Write(spatial);
                return true;
            }

            formatted = null;
            return false;
        }

        /// <summary>
        /// Tries to parse the given formatted spatial representation.
        /// </summary>
        /// <param name="formatted">The formatted representation</param>
        /// <param name="expectedSpatialType">The expected spatial type or null if it is unknown</param>
        /// <param name="spatialInstance">The parsed spatial instance</param>
        /// <returns>True if the value could be parsed, false otherwise</returns>
        public bool TryParse(IDictionary<string, object> formatted, SpatialTypeKind? expectedSpatialType, out object spatialInstance)
        {
            if (expectedSpatialType == SpatialTypeKind.Geography)
            {
                return TryRead<Geography>(formatted, out spatialInstance);
            }
            else if (expectedSpatialType == SpatialTypeKind.Geometry)
            {
                return TryRead<Geometry>(formatted, out spatialInstance);
            }
            else
            {
                // try geometry first because it does not have value restrictions like geography does
                return TryRead<Geometry>(formatted, out spatialInstance) || TryRead<Geography>(formatted, out spatialInstance);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exceptions here.")]
        private static bool TryRead<T>(IDictionary<string, object> dictionary, out object value) where T : class, ISpatial
        {
            try
            {
                var formatter = GeoJsonObjectFormatter.Create();
                value = formatter.Read<T>(dictionary);
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }
    }
}