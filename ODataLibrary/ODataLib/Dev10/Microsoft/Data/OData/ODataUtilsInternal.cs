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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Internal utility methods used in the OData library.
    /// </summary>
    internal static class ODataUtilsInternal
    {
        /// <summary>
        /// Converts a given <paramref name="version"/> to its <see cref="System.Version"/> representation.
        /// </summary>
        /// <param name="version">The <see cref="ODataVersion"/> instance to convert.</param>
        /// <returns>The <see cref="System.Version"/> representation of the <paramref name="version"/>.</returns>
        internal static Version ToDataServiceVersion(this ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();

            switch (version)
            {
                case ODataVersion.V1:
                    return new Version(1, 0);

                case ODataVersion.V2:
                    return new Version(2, 0);

                case ODataVersion.V3:
                    return new Version(3, 0);

                default:
                    string errorMessage = Strings.General_InternalError(InternalErrorCodes.ODataUtilsInternal_ToDataServiceVersion_UnreachableCodePath);
                    Debug.Assert(false, errorMessage);
                    throw new ODataException(errorMessage);
            }
        }

        /// <summary>
        /// Sets the 'DataServiceVersion' HTTP header on the message based on the protocol version specified in the settings.
        /// </summary>
        /// <param name="message">The message to set the data service version header on.</param>
        /// <param name="settings">The <see cref="ODataMessageWriterSettings"/> determining the protocol version to use.</param>
        internal static void SetDataServiceVersion(ODataMessage message, ODataMessageWriterSettings settings)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(message != null, "message != null");
            Debug.Assert(settings != null, "settings != null");
            Debug.Assert(settings.Version.HasValue, "settings.Version.HasValue");

            string userAgentString = string.Empty;
            string dataServiceVersionString = ODataUtils.ODataVersionToString(settings.Version.Value) + ";" + userAgentString;
            message.SetHeader(ODataHttpHeaders.DataServiceVersion, dataServiceVersionString);
        }

        /// <summary>
        /// Reads the DataServiceVersion header from the <paramref name="message"/> and parses it.
        /// If no DataServiceVersion header is found it sets the default version to be used for reading.
        /// </summary>
        /// <param name="message">The message to get the data service version header from.</param>
        /// <returns>
        /// The <see cref="ODataVersion"/> retrieved from the DataServiceVersion header of the message.
        /// The default version if none is specified in the header.
        /// </returns>
        internal static ODataVersion GetDataServiceVersion(ODataMessage message)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(message != null, "message != null");

            string originalHeaderValue = message.GetHeader(ODataHttpHeaders.DataServiceVersion);
            string headerValue = originalHeaderValue;

            // TODO: once we have a MaxDataServiceVersion setting on the message reader settings
            //       we should use that one (if specified)
            return string.IsNullOrEmpty(headerValue) ? ODataConstants.ODataDefaultProtocolVersion : ODataUtils.StringToODataVersion(headerValue);
        }
    }
}
