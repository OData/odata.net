//---------------------------------------------------------------------
// <copyright file="ExpectedClientRequest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Data structure for representing an HTTP request that the data services client is expected to send
    /// </summary>
    public class ExpectedClientRequest : HttpRequestData<Uri, ODataPayloadElement>
    {
        /// <summary>
        /// Gets or sets a debugging hint string for this request. Typically used to describe the combination of client-side state leading to the request being generated.
        /// </summary>
        internal string DebugHintString { get; set; }

        /// <summary>
        /// Always throws. Do not use this as an actual IHttpRequest to be sent over the network.
        /// </summary>
        /// <returns>The uri of the request</returns>
        public override Uri GetRequestUri()
        {
            throw new TaupoInvalidOperationException("Used for building client request expectations, should not actually be sent over the network");
        }

        /// <summary>
        /// Always throws. Do not use this as an actual IHttpRequest to be sent over the network.
        /// </summary>
        /// <returns>The binary body of the request</returns>
        public override byte[] GetRequestBody()
        {
            throw new TaupoInvalidOperationException("Used for building client request expectations, should not actually be sent over the network");
        }
    }
}
