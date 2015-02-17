//---------------------------------------------------------------------
// <copyright file="ContentTypeHeaderVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Linq;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// The response verifier for the Content-Type HTTP header
    /// </summary>
    public class ContentTypeHeaderVerifier : ResponseVerifierBase
    {
        /// <summary>
        /// Initializes a new instance of the ContentTypeHeaderVerifier class
        /// </summary>
        internal ContentTypeHeaderVerifier()
            : base()
        {
        }

        /// <summary>
        /// Verifies response Content-Type header value based on request Accept header and response status 
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Expected content-type values are always lower-case")]
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                this.Assert(
                    !response.Headers.ContainsKey(HttpHeaders.ContentType),
                    "Content-Type header should not be returned for the responses without content.",
                    request,
                    response);
            }
            else
            {
                // Verify the response Content-Type header exists and is consistent with the request Accept header
                string responseContentType;
                this.Assert(
                    response.Headers.TryGetValue(HttpHeaders.ContentType, out responseContentType),
                    "Response Content-Type header does not exist.",
                    request,
                    response);

                string acceptedContentType = this.RetrieveAcceptedContentType(request);
                
                // TODO: strict verification based on the type of the uri
                var acceptedMimeTypes = acceptedContentType.Split(',').Select(type => type.ToLowerInvariant()).ToList();

                this.Assert(
                    acceptedMimeTypes.Any(h => responseContentType.StartsWith(h, StringComparison.Ordinal))
                    || acceptedMimeTypes.Any(acceptType => acceptType == MimeTypes.Any),
                    "Response Content-Type header should be consistent with request Accept header.",
                    request,
                    response);

                if (responseContentType.StartsWith(MimeTypes.ApplicationJson, StringComparison.OrdinalIgnoreCase)) 
                {
                    this.Assert(
                        responseContentType.StartsWith(MimeTypes.ApplicationJsonODataLightNonStreaming, StringComparison.Ordinal) ||
                        responseContentType.StartsWith(MimeTypes.ApplicationJsonODataLightStreaming, StringComparison.Ordinal),
                        "JSON responses should be fully qualified",
                        request,
                        response);
                }
            }
        }

        private string RetrieveAcceptedContentType(ODataRequest request)
        {
            string acceptedContentType = string.Empty;

            if (request.Uri.Format != null)
            {
                switch (request.Uri.Format)
                {
                    case FormatQueryOptions.Atom:
                        acceptedContentType = MimeTypes.ApplicationAtomXml;
                        break;
                    case FormatQueryOptions.Xml:
                        acceptedContentType = MimeTypes.ApplicationXml;
                        break;
                    default:
                        ExceptionUtilities.Assert(request.Uri.Format == FormatQueryOptions.Json, "Unexpected request.Uri.Format.");
                        acceptedContentType = MimeTypes.ApplicationJson;
                        break;
                }
            }
            else if (!request.Headers.TryGetValue(HttpHeaders.Accept, out acceptedContentType))
            {
                // TODO: be strict for the data-services implementation
                acceptedContentType = MimeTypes.Any;
            }

            return acceptedContentType;
        }
    }
}
