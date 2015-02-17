//---------------------------------------------------------------------
// <copyright file="ResponseVerificationServices.cs" company="Microsoft">
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
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Default implementation of the response verification services contract
    /// </summary>
    [ImplementationName(typeof(IResponseVerificationServices), "Default")]
    public class ResponseVerificationServices : IResponseVerificationServices
    {
        /// <summary>
        /// Initializes a new instance of the ResponseVerificationServices class.
        /// </summary>
        public ResponseVerificationServices()
        {
            this.Logger = Logger.Null;
        }

        /// <summary>
        /// Gets or sets the payload element validator
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataPayloadElementValidator Validator { get; set; }

        /// <summary>
        /// Gets or sets the format selector
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProtocolFormatStrategySelector FormatSelector { get; set; }

        /// <summary>
        /// Gets or sets the implementation details of the service
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProtocolImplementationDetails ProtocolImplementationDetails { get; set; }

        /// <summary>
        /// Gets or sets the primitive data type converter to use
        /// </summary>
        [InjectDependency]
        public ODataQueryValueEntityGraphPrettyPrinter ODataQueryValueEntityGraphPrettyPrinter { get; set; }

        /// <summary>
        /// Gets or sets the primitive data type converter to use
        /// </summary>
        [InjectDependency]
        public ODataPayloadElementEntityGraphPrettyPrinter ODataPayloadElementEntityGraphPrettyPrinter { get; set; }

        /// <summary>
        /// Gets or sets the primitive data type converter to use
        /// </summary>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets the primitive data type converter to use
        /// </summary>
        [InjectDependency]
        public IQueryDataSet QueryDataSet { get; set; }

        /// <summary>
        /// Validates the data in the response payload based on the expected query value
        /// </summary>
        /// <param name="requestUri">The request uri</param>
        /// <param name="response">The response</param>
        /// <param name="expected">The expected query value</param>
        /// <param name="maxProtocolVersion">The max protocol version of the service</param>
        public void ValidateResponsePayload(ODataUri requestUri, ODataResponse response, QueryValue expected, DataServiceProtocolVersion maxProtocolVersion)
        {
            ExceptionUtilities.CheckObjectNotNull(requestUri, "requestUri");
            ExceptionUtilities.CheckObjectNotNull(response, "response");
            ExceptionUtilities.CheckObjectNotNull(expected, "expected");

            var expectedVersion = response.GetDataServiceVersion();
            this.Validator.ExpectedProtocolVersion = expectedVersion;
            this.Validator.ExpectedPayloadOptions = ODataPayloadOptions.None;

            string contentType;
            if (response.Headers.TryGetValue(HttpHeaders.ContentType, out contentType))
            {
                var strategy = this.FormatSelector.GetStrategy(contentType, requestUri);
                ExceptionUtilities.CheckObjectNotNull(strategy, "Could not get strategy for content type '{0}'", contentType);
                this.Validator.PrimitiveValueComparer = strategy.GetPrimitiveComparer();

                this.Validator.ExpectedPayloadOptions = this.ProtocolImplementationDetails.GetExpectedPayloadOptions(contentType, expectedVersion, requestUri);
            }

            this.ValidateAndPrintInfoOnError(requestUri, response.RootElement, expected);
        }

        private void ValidateAndPrintInfoOnError(ODataUri requestUri, ODataPayloadElement rootElement, QueryValue expectedValue)
        {
            try
            {
                this.Validator.Validate(rootElement, expectedValue);
            }
            catch (DataComparisonException)
            {
                // Doing this because of unit test failures, will need to fix this later
                if (this.QueryDataSet != null)
                {
                    this.Logger.WriteLine(LogLevel.Info, "Actual Payload Entity Graph:");
                    this.Logger.WriteLine(LogLevel.Info, this.ODataPayloadElementEntityGraphPrettyPrinter.PrettyPrint(rootElement));
                    this.Logger.WriteLine(LogLevel.Info, "Expected Query Entity Graph:");
                    this.Logger.WriteLine(LogLevel.Info, this.ODataQueryValueEntityGraphPrettyPrinter.PrettyPrint(expectedValue, 10));

                    this.Logger.WriteLine(LogLevel.Info, "Query DataSet Data");
                    foreach (var entitySet in requestUri.GetAllEntitySetsIncludingExpands())
                    {
                        this.Logger.WriteLine(LogLevel.Info, entitySet.Name + " DataSet");
                        var results = this.QueryDataSet[entitySet.Name];
                        this.Logger.WriteLine(LogLevel.Info, this.ODataQueryValueEntityGraphPrettyPrinter.PrettyPrint(results, 1));
                    }
                }

                throw;
            }
        }
    }
}
