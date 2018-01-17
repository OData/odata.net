//---------------------------------------------------------------------
// <copyright file="ODataVersionUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.OData;
    #endregion Namespaces

    public static class ODataVersionUtils
    {
        /// <summary>
        /// All supported versions
        /// </summary>
        public readonly static IEnumerable<ODataVersion> AllSupportedVersions = new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 };

        /// <summary>
        /// The default version to use for reading and writing OData payloads.
        /// </summary>
        public static readonly ODataVersion DefaultVersion = ODataVersion.V4;

        /// <summary>
        /// Returns the string representation used in error message for example for the specified OData version.
        /// </summary>
        /// <param name="version">The version to process.</param>
        /// <returns>The string representation of the version.</returns>
        public static string ToText(this ODataVersion version)
        {
            switch (version)
            {
                case ODataVersion.V4:
                    return "4.0";
                case ODataVersion.V401:
                    return "4.01";
                default:
                    string errorMessage = "Unsupported version '" + version.ToString() + "' found.";
                    Debug.Assert(false, errorMessage);
                    throw new NotSupportedException(errorMessage);
            }
        }

        /// <summary>
        /// Returns the DataServiceProtocolVersion representation of the specified OData version.
        /// </summary>
        /// <param name="version">The version to process.</param>
        /// <returns>The DataServiceProtocolVersion value.</returns>
        public static DataServiceProtocolVersion ToDataServiceProtocolVersion(this ODataVersion version)
        {
            switch (version)
            {
                case ODataVersion.V4:
                    return DataServiceProtocolVersion.V4;
                case ODataVersion.V401:
                    return DataServiceProtocolVersion.V4_01;
                default:
                    string errorMessage = "Unsupported version '" + version.ToString() + "' found.";
                    Debug.Assert(false, errorMessage);
                    throw new NotSupportedException(errorMessage);
            }
        }

        /// <summary>
        /// Converts an <see cref="ODataVersion"/> into an <see cref="EdmVersion"/>.
        /// </summary>
        /// <param name="odataVersion">The <see cref="ODataVersion"/> to convert.</param>
        /// <returns>An <see cref="EdmVersion"/> to be used for the given <paramref name="odataVersion"/>.</returns>
        public static EdmVersion ToEdmVersion(this ODataVersion odataVersion)
        {
            return EdmVersion.V40;
        }

        /// <summary>
        /// Converts an <see cref="ODataVersion"/> into an <see cref="Version"/>.
        /// </summary>
        /// <param name="odataVersion">The <see cref="ODataVersion"/> to convert.</param>
        /// <returns>An <see cref="Version"/> to be used for the given <paramref name="odataVersion"/>.</returns>
        public static Version ToSystemVersion(this ODataVersion odataVersion)
        {
            switch (odataVersion)
            {
                case ODataVersion.V4: return new Version(4, 0);
                case ODataVersion.V401: return new Version(4, 1);
                default:
                    throw new InvalidOperationException("Unsupported OData version: " + odataVersion.ToString());
            }
        }
    }
}
