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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Internal utility methods used in the OData library.
    /// </summary>
    internal static class ODataUtilsInternal
    {
        /// <summary>String representation of the version 1.0 of the OData protocol.</summary>
        private const string Version1NumberString = "1.0";

        /// <summary>String representation of the version 2.0 of the OData protocol.</summary>
        private const string Version2NumberString = "2.0";

        /// <summary>String representation of the version 3.0 of the OData protocol.</summary>
        private const string Version3NumberString = "3.0";

        /// <summary>
        /// Converts a given version enumeration instance to its string representation.
        /// </summary>
        /// <param name="version">The version instance to convert.</param>
        /// <returns>The string representation of the version.</returns>
        internal static string VersionString(this ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();

            switch (version)
            {
                case ODataVersion.V1:
                    return Version1NumberString;
                case ODataVersion.V2:
                    return Version2NumberString;
                case ODataVersion.V3:
                    return Version3NumberString;
                default:
                    string errorMessage = Strings.General_InternalError(InternalErrorCodes.ODataUtils_VersionString_UnreachableCodePath);
                    Debug.Assert(false, errorMessage);
                    throw new ODataException(errorMessage);
            }
        }

        /// <summary>
        /// Extracts error details from an <see cref="ODataError"/>.
        /// </summary>
        /// <param name="error">The ODataError instance to extract the error details from.</param>
        /// <param name="code">A data service-defined string which serves as a substatus to the HTTP response code.</param>
        /// <param name="message">A human readable message describing the error.</param>
        /// <param name="messageLanguage">The language identifier representing the language the error message is in.</param>
        internal static void GetErrorDetails(ODataError error, out string code, out string message, out string messageLanguage)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(error != null, "error != null");

            code = error.ErrorCode ?? string.Empty;
            message = error.Message ?? string.Empty;
            messageLanguage = error.MessageLanguage ?? ODataConstants.ODataErrorMessageDefaultLanguage;
        }

        /// <summary>
        /// Sets the 'DataServiceVersion' HTTP header on the message based on the protocol version specified in the settings.
        /// </summary>
        /// <param name="message">The message to set the data service version header on.</param>
        /// <param name="settings">The <see cref="ODataWriterSettings"/> determining the protocol version to use.</param>
        internal static void SetDataServiceVersion(ODataMessage message, ODataWriterSettings settings)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(message != null, "message != null");
            Debug.Assert(settings != null, "settings != null");

            // TODO: Bug 112260: What to do with the optional user agent string?
            string userAgentString = string.Empty;
            string dataServiceVersionString = settings.Version.VersionString() + ";" + userAgentString;
            message.SetHeader(ODataHttpHeaders.DataServiceVersion, dataServiceVersionString);
        }
    }
}
