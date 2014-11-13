//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
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

            string dataServiceVersionString = ODataUtils.ODataVersionToString(settings.Version.Value) + ";";
            message.SetHeader(ODataConstants.DataServiceVersionHeader, dataServiceVersionString);
        }

        /// <summary>
        /// Reads the DataServiceVersion header from the <paramref name="message"/> and parses it.
        /// If no DataServiceVersion header is found it sets the default version to be used for reading.
        /// </summary>
        /// <param name="message">The message to get the data service version header from.</param>
        /// <param name="defaultVersion">The default version to use if the header was not specified.</param>
        /// <returns>
        /// The <see cref="ODataVersion"/> retrieved from the DataServiceVersion header of the message.
        /// The default version if none is specified in the header.
        /// </returns>
        internal static ODataVersion GetDataServiceVersion(ODataMessage message, ODataVersion defaultVersion)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(message != null, "message != null");

            string originalHeaderValue = message.GetHeader(ODataConstants.DataServiceVersionHeader);
            string headerValue = originalHeaderValue;

            return string.IsNullOrEmpty(headerValue) 
                ? defaultVersion
                : ODataUtils.StringToODataVersion(headerValue);
        }

        /// <summary>
        /// Checks whether a payload kind is supported in a request or a response.
        /// </summary>
        /// <param name="payloadKind">The <see cref="ODataPayloadKind"/> to check.</param>
        /// <param name="inRequest">true if the check is for a request; false for a response.</param>
        /// <returns>true if the <paramref name="payloadKind"/> is valid in a request or response respectively based on <paramref name="inRequest"/>.</returns>
        internal static bool IsPayloadKindSupported(ODataPayloadKind payloadKind, bool inRequest)
        {
            DebugUtils.CheckNoExternalCallers();

            switch (payloadKind)
            {
                // These payload kinds are valid in requests and responses
                case ODataPayloadKind.Value:
                case ODataPayloadKind.BinaryValue:
                case ODataPayloadKind.Batch:
                case ODataPayloadKind.Entry:
                case ODataPayloadKind.Property:
                case ODataPayloadKind.EntityReferenceLink:
                    return true;

                // These payload kinds are only valid in responses
                case ODataPayloadKind.Feed:
                case ODataPayloadKind.EntityReferenceLinks:
                case ODataPayloadKind.Collection:
                case ODataPayloadKind.ServiceDocument:
                case ODataPayloadKind.MetadataDocument:
                case ODataPayloadKind.Error:
                    return !inRequest;

                // These payload kidns are only valid in requests
                case ODataPayloadKind.Parameter:
                    return inRequest;

                // Anything else should never show up
                default:
                    Debug.Assert(false, "Unsupported payload kind found: " + payloadKind.ToString());
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataUtilsInternal_IsPayloadKindSupported_UnreachableCodePath));
            }
        }

        /// <summary>
        /// Concats two enumerables.
        /// </summary>
        /// <typeparam name="T">Element type of the enumerable.</typeparam>
        /// <param name="enumerable1">Enumerable 1 to concat.</param>
        /// <param name="enumerable2">Enumerable 2 to concat.</param>
        /// <returns>Returns the combined enumerable.</returns>
        internal static IEnumerable<T> ConcatEnumerables<T>(IEnumerable<T> enumerable1, IEnumerable<T> enumerable2)
        {
            DebugUtils.CheckNoExternalCallers();

            // Note that we allow null, instead of empty enumerable, to be returned here because the OData OM can return null for IE<T>.
            if (enumerable1 == null)
            {
                return enumerable2;
            }

            if (enumerable2 == null)
            {
                return enumerable1;
            }

            return enumerable1.Concat(enumerable2);
        }

        /// <summary>
        /// Gets the selected properties from the given <paramref name="metadataDocumentUri"/>.
        /// </summary>
        /// <param name="metadataDocumentUri">The <see cref="ODataMetadataDocumentUri"/> instance to get the selected properties node from.</param>
        /// <returns>The selected properties node instance.</returns>
        /// <remarks>This can be a property on <see cref="ODataMetadataDocumentUri"/>. Having it as an extension method here so we don't have to do the null check at the call site.</remarks>
        internal static SelectedPropertiesNode SelectedProperties(this ODataMetadataDocumentUri metadataDocumentUri)
        {
            DebugUtils.CheckNoExternalCallers();

            if (metadataDocumentUri == null)
            {
                return SelectedPropertiesNode.Create(null);
            }

            return SelectedPropertiesNode.Create(metadataDocumentUri.SelectClause);
        }
    }
}
