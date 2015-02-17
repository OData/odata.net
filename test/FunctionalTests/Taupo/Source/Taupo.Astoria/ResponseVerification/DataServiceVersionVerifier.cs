//---------------------------------------------------------------------
// <copyright file="DataServiceVersionVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// A response verifier that compares the HTTP status code of the response to an expected value
    /// </summary>
    public class DataServiceVersionVerifier : ResponseVerifierBase
    {
        private DataServiceProtocolVersion maxProtocolVersion;
        private EntityModelSchema model;

        /// <summary>
        /// Initializes a new instance of the DataServiceVersionVerifier class
        /// </summary>
        /// <param name="model">Entity Model Schema</param>
        /// <param name="maxProtocolVersion">Max Protocol Version</param>
        public DataServiceVersionVerifier(EntityModelSchema model,  DataServiceProtocolVersion maxProtocolVersion)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.Assert(maxProtocolVersion != DataServiceProtocolVersion.Unspecified, "Max protocol version cannot be unspecified");

            this.maxProtocolVersion = maxProtocolVersion;
            this.model = model;
        }

        /// <summary>
        /// Gets or sets the version calculator for the payload
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataPayloadElementVersionCalculator PayloadElementVersionCalculator { get; set; }

        /// <summary>
        /// Gets or sets the version calculator for the uri
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriVersionCalculator UriVersionCalculator { get; set; }

        /// <summary>
        /// Gets or sets the version calculator for the model
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IEntityModelVersionCalculator ModelVersionCalculator { get; set; }

        /// <summary>
        /// Checks that the response's status code matches the expected value
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            if (request.Uri.IsBatch())
            {
                throw new TaupoNotSupportedException("Batching isn't supported yet, when it is we need to iterate over sets of requests in batch and calculate the version");
            }

            var expectedDataServiceProtocolVersion = this.CalculateDataServiceProtocolVersion(request, response);
            
            string expectedProtocolVersion = expectedDataServiceProtocolVersion.ToString().Replace("V", string.Empty) + ".0;";

            string actualDataServiceVersionHeaderValue = response.Headers[HttpHeaders.DataServiceVersion];

            this.Assert(expectedProtocolVersion.Equals(actualDataServiceVersionHeaderValue), string.Format(CultureInfo.InvariantCulture, "Data Service Version Header error, Expected '{0}' Actual '{1}'", expectedProtocolVersion, actualDataServiceVersionHeaderValue), request, response);
        }

        private DataServiceProtocolVersion CalculateDataServiceProtocolVersion(ODataRequest request, ODataResponse response)
        {
            DataServiceProtocolVersion dataServiceVersion = VersionHelper.ConvertToDataServiceProtocolVersion(request.GetHeaderValueIfExists(HttpHeaders.DataServiceVersion));
            DataServiceProtocolVersion maxDataServiceVersion = VersionHelper.ConvertToDataServiceProtocolVersion(request.GetHeaderValueIfExists(HttpHeaders.MaxDataServiceVersion));

            var responseContentType = response.GetHeaderValueIfExists(HttpHeaders.ContentType);
            if (responseContentType != null)
            {
                if (responseContentType.StartsWith(MimeTypes.ApplicationJsonODataLightNonStreaming, StringComparison.OrdinalIgnoreCase) ||
                    responseContentType.StartsWith(MimeTypes.ApplicationJsonODataLightStreaming, StringComparison.OrdinalIgnoreCase))
                {
                    return DataServiceProtocolVersion.V4;
                }
            }

            if (response.StatusCode.IsError())
            {
                return DataServiceProtocolVersion.V4;
            }

            DataServiceProtocolVersion expectedVersion = DataServiceProtocolVersion.V4;
            
            // Apply minDsv if MPV > V2
            if (maxDataServiceVersion != DataServiceProtocolVersion.Unspecified && this.maxProtocolVersion >= maxDataServiceVersion)
            {
                expectedVersion = maxDataServiceVersion;
            }
            else
            {
                expectedVersion = this.maxProtocolVersion;
            }

            // If body of a response is empty, the version is V1 unless it has prefer header.
            if (!this.IsResponseBodyEmpty(response))
            {
                if (request.Uri.IsMetadata())
                {
                    // metadata payloads are not handled by the normal payload element visitor, but the response header will match the model version exactly
                    expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, this.ModelVersionCalculator.CalculateProtocolVersion(this.model));
                }
                else
                {
                    // GET requests are versioned based on the URI because type information is not known until serialization
                    if (request.GetEffectiveVerb() == HttpVerb.Get || request.Uri.IsServiceOperation() || request.Uri.IsAction())
                    {
                        expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, this.UriVersionCalculator.CalculateProtocolVersion(request.Uri, response.Headers[HttpHeaders.ContentType], this.maxProtocolVersion, dataServiceVersion, maxDataServiceVersion));
                    }

                    // Post and update requests are versioned based on the specific instance
                    if (response.RootElement != null)
                    {
                        expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, this.PayloadElementVersionCalculator.CalculateProtocolVersion(response.RootElement, response.Headers[HttpHeaders.ContentType], this.maxProtocolVersion, maxDataServiceVersion));
                    }
                }
            }
            else
            {
                if (request.Uri.IsAction())
                {
                    expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, this.UriVersionCalculator.CalculateProtocolVersion(request.Uri, response.GetHeaderValueIfExists(HttpHeaders.ContentType), this.maxProtocolVersion, dataServiceVersion, maxDataServiceVersion));
                }
            }

            // NOTE: the prefer verifier will ensure that this header is present if it should be, so our only concern here
            // is that the version is >= V3 if it is present
            if (response.Headers.ContainsKey(HttpHeaders.PreferenceApplied))
            {
                expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, DataServiceProtocolVersion.V4);
            }

            return expectedVersion;
        } 
   
        private bool IsResponseBodyEmpty(ODataResponse response)
        {
            return response.Body == null || response.Body.Length == 0;
        }
    }
}
