//---------------------------------------------------------------------
// <copyright file="ClientQueryVersionErrorCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Calculates the Error for the Query Expression
    /// </summary>
    [ImplementationName(typeof(IClientQueryVersionErrorCalculator), "Default")]
    public class ClientQueryVersionErrorCalculator : IClientQueryVersionErrorCalculator
    {
        /// <summary>
        /// Gets or sets the Query to ODataUri Converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryToODataUriConverter QueryToODataUriConverter { get; set; }

        /// <summary>
        /// Gets or sets the component that determines versioning errors on the server
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataRequestVersionResourceErrorCalculator ODataRequestVersionResourceErrorCalculator { get; set; }

        /// <summary>
        /// Gets or sets the component that helps build ODataUri to a string
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriToStringConverter ODataUriToStringConverter { get; set; }

        /// <summary>
        /// Gets or sets the client side projection replacing visitor.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IClientSideProjectionReplacingVisitor ClientSideProjectionReplacer { get; set; }

        /// <summary>
        /// Returns the error expected for the Query
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="usesClientQueryable">Whether the client uri is build from a Client Linq expression or not</param>
        /// <param name="clientMaxProtocolVersion">Client Max Protocol Version</param>
        /// <param name="maxProtocolVersion">Max Protocol version of the server</param>
        /// <returns> a versioning error at the Client level</returns>
        public ExpectedClientErrorBaseline CalculateExpectedClientVersionError(QueryExpression expression, bool usesClientQueryable, DataServiceProtocolVersion clientMaxProtocolVersion, DataServiceProtocolVersion maxProtocolVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            DataServiceProtocolVersion expectedDataServiceVersion = DataServiceProtocolVersion.Unspecified;
            DataServiceProtocolVersion minRequiredRequestVersion = DataServiceProtocolVersion.Unspecified;

            ExpectedErrorMessage errorInformation;

            // If the Client Queryable interface is not used to build the expression that is run then no error can happen at this stage
            // There can only be a protocol error from the server if there is no client error
            var serverExpression = this.ClientSideProjectionReplacer.ReplaceClientSideProjections(expression);

            ODataUri odataUri = this.QueryToODataUriConverter.ComputeUri(serverExpression);

            // Predict what the Client will generate for the data service version, if we are not using client.linq its unspecified otherwise calculate it
            if (usesClientQueryable)
            {
                ExpectedClientErrorBaseline expectedErrorIfTooLow;
                minRequiredRequestVersion = this.CalculateExpectedClientMinRequestVersion(odataUri, out expectedErrorIfTooLow);
                if (minRequiredRequestVersion > clientMaxProtocolVersion)
                {
                    if (expectedErrorIfTooLow == null)
                    {
                        errorInformation = new ExpectedErrorMessage("Context_RequestVersionIsBiggerThanProtocolVersion", minRequiredRequestVersion.ConvertToHeaderFormat(), clientMaxProtocolVersion.ConvertToHeaderFormat());
                        expectedErrorIfTooLow = new ExpectedClientErrorBaseline(typeof(InvalidOperationException), false, errorInformation);
                    }

                    return expectedErrorIfTooLow;
                }

                EntitySet expectedEntitySet = null;
                if (odataUri.TryGetExpectedEntitySet(out expectedEntitySet))
                {
                    DataServiceProtocolVersion entitySetVersion = expectedEntitySet.CalculateEntitySetProtocolVersion(MimeTypes.ApplicationAtomXml, VersionCalculationType.Request, maxProtocolVersion, clientMaxProtocolVersion);

                    // Client will create a DSV based on the following pieces, metadata of the query based on sets it goes through, uri contructs (select, count, inlinecount, etc)
                    // and headers (like DataServiceResponsePreference). In order to mimic this client behavior I will get the metadata version and a version
                    // from the minrequestVersion and take the max. MinRequest deals with Uri and headers, metadata the metadata.
                    expectedDataServiceVersion = VersionHelper.GetMaximumVersion(minRequiredRequestVersion, entitySetVersion);
                }
            }

            ODataRequest request = new ODataRequest(this.ODataUriToStringConverter)
            {
                Uri = odataUri,
                Verb = HttpVerb.Get,
                Headers =
                {
                    { HttpHeaders.DataServiceVersion, expectedDataServiceVersion.ConvertToHeaderFormat() },
                    { HttpHeaders.MaxDataServiceVersion, clientMaxProtocolVersion.ConvertToHeaderFormat() },
                    { HttpHeaders.Accept, MimeTypes.ApplicationAtomXml }
                }
            };

            if (this.ODataRequestVersionResourceErrorCalculator.TryCalculateError(request, maxProtocolVersion, out errorInformation))
            {
                return new ExpectedClientErrorBaseline(typeof(DSClient.DataServiceQueryException), true, errorInformation);
            }

            return null;
        }

        private DataServiceProtocolVersion CalculateExpectedClientMinRequestVersion(ODataUri odataUri, out ExpectedClientErrorBaseline errorIfTooLow)
        {
            ExceptionUtilities.CheckArgumentNotNull(odataUri, "odataUri");

            DataServiceProtocolVersion expectedVersion = DataServiceProtocolVersion.V4;
            errorIfTooLow = null;

            // Uri specific processing
            if (odataUri.HasAnyOrAllInFilter())
            {
                expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, DataServiceProtocolVersion.V4);
                string anyOrAll = string.Empty;

                if (odataUri.Filter.Contains("/any("))
                {
                    anyOrAll = "Any";
                }
                else
                {
                    anyOrAll = "All";
                }

                var errorInformation = new ExpectedErrorMessage("ALinq_MethodNotSupportedForMaxDataServiceVersionLessThanX", anyOrAll, expectedVersion.ConvertToHeaderFormat());
                errorIfTooLow = new ExpectedClientErrorBaseline(typeof(NotSupportedException), false, errorInformation);
            }

            if (odataUri.HasSignificantTypeSegmentInPath())
            {
                // TODO: this seems like it should be inferred from the query, not the uri
                expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, DataServiceProtocolVersion.V4);
                var errorInformation = new ExpectedErrorMessage("ALinq_MethodNotSupportedForMaxDataServiceVersionLessThanX", "OfType", expectedVersion.ConvertToHeaderFormat());
                errorIfTooLow = new ExpectedClientErrorBaseline(typeof(NotSupportedException), false, errorInformation);
            }
            else if (odataUri.HasTypeSegmentInExpandOrSelect())
            {
                // TODO: this seems like it should be inferred from the query, not the uri
                expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, DataServiceProtocolVersion.V4);
                var errorInformation = new ExpectedErrorMessage("ALinq_TypeAsNotSupportedForMaxDataServiceVersionLessThan3");
                errorIfTooLow = new ExpectedClientErrorBaseline(typeof(NotSupportedException), false, errorInformation);
            }
            
            return expectedVersion;
        }
    }
}