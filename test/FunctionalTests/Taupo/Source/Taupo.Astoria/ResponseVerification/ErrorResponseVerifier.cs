//---------------------------------------------------------------------
// <copyright file="ErrorResponseVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// A response verifier that compares the HTTP status code of the response to an expected value
    /// </summary>
    public class ErrorResponseVerifier : ResponseVerifierBase
    {
        /// <summary>
        /// Initializes a new instance of the ErrorResponseVerifier class
        /// </summary>
        /// <param name="expectedErrorMessage">Expected error message to compare against</param>
        /// <param name="resourceVerifier">resource verifier for to use for the error message. Can be null if the expected errors provide specific verifiers.</param>
        public ErrorResponseVerifier(ExpectedErrorMessage expectedErrorMessage, IStringResourceVerifier resourceVerifier)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(expectedErrorMessage, "expectedErrorMessage");
            this.ExpectedErrorMessage = expectedErrorMessage;
            this.ResourceVerifier = resourceVerifier;
        }

        /// <summary>
        /// Gets the expected status code
        /// </summary>
        public ExpectedErrorMessage ExpectedErrorMessage { get; private set; }

        /// <summary>
        /// Gets or sets the format selector to use if an error response needs to be deserialized. 
        /// This is only needed in the case of errors from media-resource operations which are not automatically deserialized.
        /// </summary>
        [InjectDependency]
        public IProtocolFormatStrategySelector FormatSelector { get; set; }

        /// <summary>
        /// Gets the string resource verifier
        /// </summary>
        internal IStringResourceVerifier ResourceVerifier { get; private set; }

        /// <summary>
        /// Checks that the response's status code matches the expected value
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            ODataErrorPayload errorPayload;
            this.Assert(TryGetErrorPayloadFromResponse(response, this.FormatSelector, out errorPayload), "Expected an error payload", request, response);

            this.ExpectedErrorMessage.VerifyExceptionMessage(
                this.ResourceVerifier, 
                errorPayload, 
                (assertion, errorMessage) =>
                {
                    this.Assert(assertion, errorMessage, request, response);
                });
        }

        /// <summary>
        /// Helper method for getting the error payload from the response. Handles a special case for media-resource operations which may contain errors despite not normally
        /// being deserialized as such.
        /// </summary>
        /// <param name="response">The current response</param>
        /// <param name="formatSelector">The format selector to use if the response needs to be deserialized</param>
        /// <param name="errorPayload">The error payload if one was found</param>
        /// <returns>Whether or not an error payload was found</returns>
        internal static bool TryGetErrorPayloadFromResponse(ODataResponse response, IProtocolFormatStrategySelector formatSelector, out ODataErrorPayload errorPayload)
        {
            ExceptionUtilities.CheckArgumentNotNull(response, "response");

            errorPayload = null;
            var payload = response.RootElement;
            if (payload == null)
            {
                return false;
            }

            if (payload.ElementType == ODataPayloadElementType.ODataErrorPayload)
            {
                errorPayload = (ODataErrorPayload)payload;
                return true;
            }

            // From here out, try to handle special case for streams which come back with error payloads and are not interpreted
            if (payload.ElementType != ODataPayloadElementType.PrimitiveValue)
            {
                return false;
            }

            var body = ((PrimitiveValue)payload).ClrValue as byte[];
            if (body == null)
            {
                return false;
            }

            var contentType = response.GetHeaderValueIfExists(HttpHeaders.ContentType);
            if (contentType == null)
            {
                return false;
            }

            // deserialize it
            ExceptionUtilities.CheckArgumentNotNull(formatSelector, "formatSelector");
            var formatForContentType = formatSelector.GetStrategy(contentType, null);
            var deserializer = formatForContentType.GetDeserializer();
            payload = deserializer.DeserializeFromBinary(body, new ODataPayloadContext { EncodingName = HttpUtilities.GetContentTypeCharsetOrNull(contentType) });

            errorPayload = payload as ODataErrorPayload;
            return errorPayload != null;
        }
    }
}
