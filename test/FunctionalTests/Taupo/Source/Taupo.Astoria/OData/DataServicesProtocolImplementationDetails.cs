//---------------------------------------------------------------------
// <copyright file="DataServicesProtocolImplementationDetails.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Protocol implementation details for the WCF Data Services server implementation
    /// </summary>
    [ImplementationName(typeof(IProtocolImplementationDetails), "DataServices")]
    public class DataServicesProtocolImplementationDetails : IProtocolImplementationDetails
    {
        private static ODataPayloadOptions expectedPayloadOptions =
            ODataPayloadOptions.UseConventionBasedIdentifiers
            | ODataPayloadOptions.IncludeTypeNames
            | ODataPayloadOptions.IncludeEntityIdentifier
            | ODataPayloadOptions.IncludeMediaResourceEditLinks
            | ODataPayloadOptions.IncludeMediaResourceSourceLinks
            | ODataPayloadOptions.IncludeNamedMediaResourceEditLinks
            | ODataPayloadOptions.UseConventionBasedLinks
            | ODataPayloadOptions.OmitTypeNamesForStrings
            | ODataPayloadOptions.OmitTypeNamesWithinMultiValues
            | ODataPayloadOptions.IncludeETags
            | ODataPayloadOptions.IncludeSelfOrEditLink;

        /// <summary>
        /// Gets or sets the uri to string converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriToStringConverter UriConverter { get; set; }

        /// <summary>
        /// Gets the expected options for the given content type and version
        /// </summary>
        /// <param name="contentType">The content type of the payload</param>
        /// <param name="version">The current version</param>
        /// <param name="payloadUri">The payload URI</param>
        /// <returns>The payload options for the given content type and version</returns>
        public ODataPayloadOptions GetExpectedPayloadOptions(string contentType, DataServiceProtocolVersion version, ODataUri payloadUri)
        {
            ExceptionUtilities.CheckArgumentNotNull(contentType, "contentType");
            ExceptionUtilities.Assert(version != DataServiceProtocolVersion.Unspecified, "Version cannot be unspecified");
            
            var expected = expectedPayloadOptions;
            
            if (contentType.StartsWith(MimeTypes.ApplicationJsonODataLightNonStreaming, System.StringComparison.Ordinal) ||
                     contentType.StartsWith(MimeTypes.ApplicationJsonODataLightStreaming, System.StringComparison.Ordinal))
            {
                expected = ODataPayloadOptions.IncludeTypeNames
                           | ODataPayloadOptions.IncludeMediaResourceSourceLinks
                           | ODataPayloadOptions.IncludeMediaResourceEditLinks
                           | ODataPayloadOptions.IncludeNamedMediaResourceSourceLinks
                           | ODataPayloadOptions.IncludeNamedMediaResourceEditLinks
                           | ODataPayloadOptions.IncludeEntityIdentifier
                           | ODataPayloadOptions.ConventionallyProducedNamedStreamSelfLink;

                var selectedPropertyNames = ODataUtilities.GetSelectedPropertyNamesFromUri(payloadUri, this.UriConverter).ToList();
                if (selectedPropertyNames.Any())
                {
                    EntitySet payloadEntitySet = null;
                    EntityType payloadEntityType = null;
                    if (payloadUri.TryGetExpectedEntitySetAndType(out payloadEntitySet, out payloadEntityType) &&
                        !ODataUtilities.ContainsAllIdentityPropertyNames(selectedPropertyNames, payloadEntityType))
                    {
                        // JSON Light projections without identity do not contain enough metadata to deduce 
                        // id and link values.
                        expected = ODataPayloadOptions.IncludeTypeNames;
                    }
                }
            }
            else if (version < DataServiceProtocolVersion.V4)
            {
                // Type names for null values are only supported in V1 and V2.
                expected = expected | ODataPayloadOptions.IncludeTypeNamesForNullValues;
            }

            return expected;
        }
    }
}
