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

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Internal utility methods used in the OData library.
    /// </summary>
    internal static class ODataUtilsInternal
    {
        /// <summary>
        /// Sets the 'OData-Version' HTTP header on the message based on the protocol version specified in the settings.
        /// </summary>
        /// <param name="message">The message to set the OData-Version header on.</param>
        /// <param name="settings">The <see cref="ODataMessageWriterSettings"/> determining the protocol version to use.</param>
        internal static void SetODataVersion(ODataMessage message, ODataMessageWriterSettings settings)
        {
            Debug.Assert(message != null, "message != null");
            Debug.Assert(settings != null, "settings != null");
            Debug.Assert(settings.Version.HasValue, "settings.Version.HasValue");

            string odataVersionString = ODataUtils.ODataVersionToString(settings.Version.Value);
            message.SetHeader(ODataConstants.ODataVersionHeader, odataVersionString);
        }

        /// <summary>
        /// Reads the OData-Version header from the <paramref name="message"/> and parses it.
        /// If no OData-Version header is found it sets the default version to be used for reading.
        /// </summary>
        /// <param name="message">The message to get the OData-Version header from.</param>
        /// <param name="defaultVersion">The default version to use if the header was not specified.</param>
        /// <returns>
        /// The <see cref="ODataVersion"/> retrieved from the OData-Version header of the message.
        /// The default version if none is specified in the header.
        /// </returns>
        internal static ODataVersion GetODataVersion(ODataMessage message, ODataVersion defaultVersion)
        {
            Debug.Assert(message != null, "message != null");

            string originalHeaderValue = message.GetHeader(ODataConstants.ODataVersionHeader);
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
                case ODataPayloadKind.IndividualProperty:
                case ODataPayloadKind.Delta:
                case ODataPayloadKind.Asynchronous:
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
        /// Returns true if this reference refers to a nullable type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a nullable type.</returns>
        internal static bool IsNullable(this IEdmTypeReference type)
        {
            return type == null || type.IsNullable;
        }
    }
}
