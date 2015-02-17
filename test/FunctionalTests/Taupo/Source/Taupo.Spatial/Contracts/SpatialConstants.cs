//---------------------------------------------------------------------
// <copyright file="SpatialConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.Contracts
{
    /// <summary>
    /// Set of constants specific to spatial
    /// </summary>
    public static class SpatialConstants
    {
        /// <summary>
        /// Constant for "http://www.opengis.net/gml"
        /// </summary>
        public const string GmlNamespaceName = "http://www.opengis.net/gml";

        /// <summary>
        /// Constant for the default prefix for "http://www.opengis.net/gml" which is "gml"
        /// </summary>
        public const string GmlNamespaceDefaultPrefix = "gml";

        /// <summary>
        /// Constant for "http://www.georss.org/georss"
        /// </summary>
        public const string GeoRssNamespaceName = "http://www.georss.org/georss";

        /// <summary>
        /// Constant for the default prefix for "http://www.georss.org/georss" which is "georss"
        /// </summary>
        public const string GeoRssNamespaceDefaultPrefix = "georss";

        /// <summary>
        /// Constant for the 'SRID=' prefix that appears at the beginning of all well-known-text formatted strings
        /// </summary>
        public const string WellKnownTextSridPrefix = "SRID=";

        /// <summary>
        /// Constant for the Default SRID for Geometry types, i.e., "0"
        /// </summary>
        public const string DefaultSridForGeometry = "0";

        /// <summary>
        /// Constant for the Default SRID for Geography types, i.e., "4326"
        /// </summary>
        public const string DefaultSridForGeography = "4326";

        /// <summary>
        /// Constant for the Default SRID for Geography types, i.e., "4326"
        /// </summary>
        public const string VariableSrid = "Variable";
    }
}