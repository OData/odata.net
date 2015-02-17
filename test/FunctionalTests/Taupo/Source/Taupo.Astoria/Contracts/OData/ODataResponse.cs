//---------------------------------------------------------------------
// <copyright file="ODataResponse.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Augments an HTTP response with OData-protocol-level information
    /// </summary>
    public class ODataResponse : HttpResponseData
    {
        /// <summary>
        /// Initializes a new instance of the ODataResponse class. Copies information from given response, but does not maintain a reference to it.
        /// </summary>
        /// <param name="toCopy">The response to copy information from</param>
        public ODataResponse(HttpResponseData toCopy)
        {
            ExceptionUtilities.CheckArgumentNotNull(toCopy, "toCopy");
            this.StatusCode = toCopy.StatusCode;
            foreach (var header in toCopy.Headers)
            {
                this.Headers.Add(header.Key, header.Value);
            }

            this.Body = toCopy.Body;
        }

        /// <summary>
        /// Gets or sets the root element for the response's deserialized payload
        /// </summary>
        public ODataPayloadElement RootElement { get; set; }

        /// <summary>
        /// Implicit cast operator to ease adding OData responses to batch payloads
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>The response wrapped with default batch-specific headers</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed")]
        public static implicit operator MimePartData<HttpResponseData>(ODataResponse response)
        {
            return ((HttpResponseData)response).AsBatchFragment();
        }
    }
}
