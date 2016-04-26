//---------------------------------------------------------------------
// <copyright file="ResponseContentTypeNegotiator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// Standalone component for doing content-type negotiation which wraps ODataLib's APIs
    /// </summary>
    internal class ResponseContentTypeNegotiator
    {
        /// <summary>
        /// The response version.
        /// </summary>
        private readonly ODataVersion responseVersion;

        /// <summary>
        /// Whether or not to throw if no match is found.
        /// </summary>
        private readonly bool throwIfNoMatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseContentTypeNegotiator"/> class.
        /// </summary>
        /// <param name="responseVersion">The version of the response.</param>
        /// <param name="throwIfNoMatch">if set to <c>true</c> then the method should throw an exception if no match is found. Otherwise null should be returned in this case.</param>
        internal ResponseContentTypeNegotiator(ODataVersion responseVersion, bool throwIfNoMatch)
        {
            this.responseVersion = responseVersion;
            this.throwIfNoMatch = throwIfNoMatch;
        }

        /// <summary>
        /// Determines the response format based on the results of content negotiation.
        /// </summary>
        /// <param name="payloadKind">The payload kind of the response.</param>
        /// <param name="acceptableMediaTypes">
        /// The acceptable media types used to determine the content type of the message.
        /// This is a comma separated list of content types as specified in RFC 2616, Section 14.1
        /// </param>
        /// <param name="acceptableCharSets">
        /// The acceptable charsets to use to the determine the encoding of the message.
        /// This is a comma separated list of charsets as specified in RFC 2616, Section 14.2
        /// </param>
        /// <returns>The format the response should use. </returns>
        internal ODataFormatWithParameters DetermineResponseFormat(ODataPayloadKind payloadKind, string acceptableMediaTypes, string acceptableCharSets)
        {
            Debug.Assert(payloadKind != ODataPayloadKind.Unsupported, "kind != ODataPayloadKind.Unsupported");

            ContentNegotiationResponseMessage responseMessage = new ContentNegotiationResponseMessage();

            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { Version = this.responseVersion };
            // settings.EnableAtomSupport();
            settings.SetContentType(acceptableMediaTypes, acceptableCharSets);

            try
            {
                using (ODataMessageWriter writer = new ODataMessageWriter(responseMessage, settings))
                {
                    ODataFormat format = ODataUtils.SetHeadersForPayload(writer, payloadKind);
                    return new ODataFormatWithParameters(format, responseMessage.ContentType);
                }
            }
            catch (ODataContentTypeException exception)
            {
                if (this.throwIfNoMatch)
                {
                    throw new DataServiceException(415, null, Microsoft.OData.Service.Strings.DataServiceException_UnsupportedMediaType, null, exception);
                }

                return null;
            }
        }

        /// <summary>
        /// A <see cref="IODataResponseMessage"/> implementation that is only used for content-negotation and doesn't support anything other than SetHeader for 'Content-Type'.
        /// </summary>
        private class ContentNegotiationResponseMessage : IODataResponseMessage
        {
            /// <summary>
            /// Getting headers from this implementation is not supported.
            /// </summary>
            public IEnumerable<KeyValuePair<string, string>> Headers
            {
                get { throw new NotSupportedException(); }
            }

            /// <summary>
            /// Getting/setting the status code is not supported for this implementation.
            /// </summary>
            public int StatusCode
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            /// <summary>
            /// Gets the content type which was set via SetHeader, if any.
            /// </summary>
            internal string ContentType { get; private set; }

            /// <summary>
            /// Getting arbitrary header values is not supported for this implementation.
            /// </summary>
            /// <param name="headerName">The name of the header to get.</param>
            /// <returns>
            /// The value of the HTTP header, or null if no such header was present on the message.
            /// </returns>
            public string GetHeader(string headerName)
            {
                if (headerName == "Preference-Applied")
                {
                    return null;
                }

                throw new NotSupportedException();
            }

            /// <summary>
            /// Sets the value of an HTTP header. Only certain headers are allowed to be set in this implementation.
            /// </summary>
            /// <param name="headerName">The name of the header to set.</param>
            /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
            public void SetHeader(string headerName, string headerValue)
            {
                Debug.Assert(
                    headerName == XmlConstants.HttpContentType || headerName == XmlConstants.HttpODataVersion,
                    "Only 'Content-Type' and 'OData-Version' headers are supported");
                this.ContentType = headerValue;
            }

            /// <summary>
            /// Getting the stream is not supported for this implementation.
            /// </summary>
            /// <returns>
            /// The stream for this message.
            /// </returns>
            public Stream GetStream()
            {
                throw new NotSupportedException();
            }
        }
    }
}