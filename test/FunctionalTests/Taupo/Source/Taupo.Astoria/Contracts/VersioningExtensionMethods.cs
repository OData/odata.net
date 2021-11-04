//---------------------------------------------------------------------
// <copyright file="VersioningExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using DC = Microsoft.OData.Client;

    /// <summary>
    /// Extension methods that make writing astoria tests easier with respect to versioning
    /// </summary>
    public static class VersioningExtensionMethods
    {
        /// <summary>
        /// Gets the value of the 'DataServiceVersion' header for the given request or response
        /// </summary>
        /// <param name="part">The request to get the version header value from</param>
        /// <returns>The value of the 'DataServiceVersion' header from the request or response</returns>
        public static DataServiceProtocolVersion GetDataServiceVersion(this IMimePart part)
        {
            return part.GetProtocolVersionFromHeader(HttpHeaders.DataServiceVersion);
        }

        /// <summary>
        /// Gets the value of the 'MaxDataServiceVersion' header for the given request
        /// </summary>
        /// <param name="request">The request to get the version header value from</param>
        /// <returns>The value of the 'MaxDataServiceVersion' header from the request</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Header only used on requests")]
        public static DataServiceProtocolVersion GetMaxDataServiceVersion(this IHttpRequest request)
        {
            return request.GetProtocolVersionFromHeader(HttpHeaders.MaxDataServiceVersion);
        }

        /// <summary>
        /// Converts the given product enum value to the equivalent test enum value
        /// </summary>
        /// <param name="version">The value to convert</param>
        /// <returns>The converted value</returns>
        public static DataServiceProtocolVersion ToTestEnum(this DC.ODataProtocolVersion version)
        {
            return ExtensionMethods.ConvertEnum<DC.ODataProtocolVersion, DataServiceProtocolVersion>(version);
        }

        /// <summary>
        /// Converts the given test enum value to the equivalent product enum value
        /// </summary>
        /// <param name="version">The value to convert</param>
        /// <returns>The converted value</returns>
        public static DC.ODataProtocolVersion ToProductEnum(this DataServiceProtocolVersion version)
        {
            ExceptionUtilities.Assert(version != DataServiceProtocolVersion.Unspecified, "Cannot convert unspecified version value");
            ExceptionUtilities.Assert(version != DataServiceProtocolVersion.LatestVersionPlusOne, "Cannot convert latest +1 version value");
            return ExtensionMethods.ConvertEnum<DataServiceProtocolVersion, DC.ODataProtocolVersion>(version);
        }

        /// <summary>
        /// Gets the value of the given header for the given request or response and converts it into a DataServiceProtocolVersion
        /// </summary>
        /// <param name="part">The request or response to get the version header value from</param>
        /// <param name="header">The header to convert</param>
        /// <returns>The converted value of the header from the request</returns>
        private static DataServiceProtocolVersion GetProtocolVersionFromHeader(this IMimePart part, string header)
        {
            string version;
            if (!part.Headers.TryGetValue(header, out version))
            {
                return DataServiceProtocolVersion.Unspecified;
            }
            else
            {
                return VersionHelper.ConvertToDataServiceProtocolVersion(version);
            }
        }
    }
}
