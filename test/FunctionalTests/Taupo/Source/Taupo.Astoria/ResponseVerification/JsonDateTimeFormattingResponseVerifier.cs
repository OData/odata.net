//---------------------------------------------------------------------
// <copyright file="JsonDateTimeFormattingResponseVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Response verifier for JSON datetime formatting
    /// </summary>
    public class JsonDateTimeFormattingResponseVerifier : ResponseVerifierBase, ISelectiveResponseVerifier
    {
        /// <summary>
        /// Gets or sets the validator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataJsonDateTimeFormattingValidator Validator { get; set; }

        /// <summary>
        /// Returns a value indicating whether this verifier applies to the given request
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>
        /// True if it applies, false otherwise
        /// </returns>
        public bool Applies(ODataRequest request)
        {
            return true;
        }

        /// <summary>
        /// Returns a value indicating whether this verifier applies to the given response
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>
        /// True if it applies, false otherwise
        /// </returns>
        public bool Applies(ODataResponse response)
        {
            return response.GetHeaderValueIfExists(HttpHeaders.ContentType).IfValid(false, c => c.StartsWith(MimeTypes.ApplicationJsonLight, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Verifies the given OData request/response pair
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            if (response.RootElement != null)
            {
                this.Validator.ValidateDateTimeFormatting(response.RootElement);
            }
        }
    }
}