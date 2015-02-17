//---------------------------------------------------------------------
// <copyright file="ODataRequest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Augments an http request with OData-protocol-level information
    /// </summary>
    public class ODataRequest : HttpRequestData<ODataUri, ODataPayloadBody>
    {
        private IODataUriToStringConverter uriToStringConverter;

        /// <summary>
        /// Initializes a new instance of the ODataRequest class
        /// </summary>
        /// <param name="uriToStringConverter">The OData uri to string converter to use when sending the request</param>
        /// <param name="requestManager">The IODataRequestManager used for building the ODataRequest (which is needed for cloning)</param>
        internal ODataRequest(IODataUriToStringConverter uriToStringConverter, IODataRequestManager requestManager = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(uriToStringConverter, "uriToStringConverter");
            this.uriToStringConverter = uriToStringConverter;
            this.RequestManager = requestManager;
        }

        /// <summary>
        /// Gets or sets the request manager
        /// </summary>
        internal IODataRequestManager RequestManager { get; set; }

        /// <summary>
        /// Implicit cast operator to ease adding OData request to batch payloads
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>The request wrapped with default batch-specific headers</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed")]
        public static implicit operator MimePartData<IHttpRequest>(ODataRequest request)
        {
            return ((IHttpRequest)request).AsBatchFragment();
        }

        /// <summary>
        /// Returns the request uri based on the request's OData uri
        /// </summary>
        /// <returns>The raw uri</returns>
        public override Uri GetRequestUri()
        {
            return this.uriToStringConverter.ConvertToUri(this.Uri);
        }

        /// <summary>
        /// Returns the binary body of the request
        /// </summary>
        /// <returns>The binary body of the request</returns>
        public override byte[] GetRequestBody()
        {
            if (this.Body == null)
            {
                return null;
            }

            return this.Body.SerializedValue;
        }
    }
}
