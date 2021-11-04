//---------------------------------------------------------------------
// <copyright file="ProtocolQueryVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Linq;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Builds and verifies queries using raw protocol requests
    /// </summary>
    public abstract class ProtocolQueryVerifier : IQueryVerifier
    {
        /// <summary>
        /// Initializes a new instance of the ProtocolQueryVerifier class.
        /// </summary>
        /// <param name="defaultAcceptType">The mime type to use in the 'accept' header</param>
        /// <param name="serviceRoot">The root uri of the service</param>
        /// <param name="maxProtocolVersion">The max protocol version of the service</param>
        /// <param name="resourceVerifier">The resource string verifier to use for errors</param>
        protected ProtocolQueryVerifier(string defaultAcceptType, Uri serviceRoot, DataServiceProtocolVersion maxProtocolVersion, IStringResourceVerifier resourceVerifier)
        {
            ExceptionUtilities.CheckArgumentNotNull(defaultAcceptType, "defaultAcceptType");
            ExceptionUtilities.CheckArgumentNotNull(serviceRoot, "serviceRoot");
            ExceptionUtilities.CheckArgumentNotNull(resourceVerifier, "resourceVerifier");

            this.DefaultAcceptType = defaultAcceptType;
            this.ServiceRoot = serviceRoot;
            this.Logger = Logger.Null;
            this.MaxProtocolVersion = maxProtocolVersion;
            this.DataServiceVersion = DataServiceProtocolVersion.Unspecified;
            this.MaxDataServiceVersion = DataServiceProtocolVersion.Unspecified;
            this.ResourceVerifier = resourceVerifier;
        }

        /// <summary>
        /// Gets or sets the logger used to print diagnostic messages.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets the protocol test services
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ProtocolTestServices ProtocolTestServices { get; set; }

        /// <summary>
        /// Gets or sets the query resolver to resolve untyped expressions to runtime-type bound expression
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQueryResolver QueryResolver { get; set; }

        /// <summary>
        /// Gets or sets the ODataRequest ResourceStringInformation Error Calculator
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataRequestVersionResourceErrorCalculator ODataRequestVersionResourceErrorCalculator { get; set; }

        /// <summary>
        /// Gets or sets a expression to payload converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryToODataPayloadConverter QueryToODataPayloadConverter { get; set; }

        /// <summary>
        /// Gets or sets the MaxDataServiceVersion
        /// </summary>
        [InjectTestParameter("DataServiceVersion", DefaultValueDescription = "NotSpecified", HelpText = "DataServiceVersion to assign to Protocol Query Tests")]
        public DataServiceProtocolVersion DataServiceVersion { get; set; }

        /// <summary>
        /// Gets or sets the MaxDataServiceVersion
        /// </summary>
        [InjectTestParameter("MaxDataServiceVersion", DefaultValueDescription = "NotSpecified", HelpText = "MaxDataServiceVersion to assign to Protocol Query Tests")]
        public DataServiceProtocolVersion MaxDataServiceVersion { get; set; }

        /// <summary>
        /// Gets or sets the odata uri evaluator
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriEvaluator UriEvaluator { get; set; }

        /// <summary>
        /// Gets the default accept type to use on protocol requests
        /// </summary>
        internal string DefaultAcceptType { get; private set; }

        /// <summary>
        /// Gets the root uri of the service
        /// </summary>
        internal Uri ServiceRoot { get; private set; }

        /// <summary>
        /// Gets the max protocol version of the service
        /// </summary>
        internal DataServiceProtocolVersion MaxProtocolVersion { get; private set; }

        /// <summary>
        /// Gets the resource string verifier to use for errors
        /// </summary>
        internal IStringResourceVerifier ResourceVerifier { get; private set; }

        /// <summary>
        /// Constructs, sends, and verifies a raw protcol request for the given query expression
        /// </summary>
        /// <param name="expression">The expression to verify</param>
        public virtual void Verify(QueryExpression expression)
        {
            ODataRequest request;
            ODataResponse response;
            expression = this.ResolveExpressionAndGetResponse(expression, out request, out response);

            IResponseVerifier verifier = null;
            ExpectedErrorMessage errorInformation = null;
            if (this.ODataRequestVersionResourceErrorCalculator.TryCalculateError(request, this.MaxProtocolVersion, out errorInformation))
            {
                verifier = this.ProtocolTestServices.ResponseVerifierFactory.GetErrorVerifier(GetExpectedErrorStatusCode(errorInformation), errorInformation, this.ResourceVerifier);
            }
            else
            {
                verifier = this.ProtocolTestServices.ResponseVerifierFactory.GetStandardVerifier(GetExpectedStatusCode(request.Uri, this.UriEvaluator));
            }

            verifier.Verify(request, response);
        }

        internal static HttpStatusCode GetExpectedErrorStatusCode(ExpectedErrorMessage errorInformation)
        {
            if (errorInformation.ResourceIdentifier == "DataServiceException_GeneralError" ||
                errorInformation.ResourceIdentifier == "DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion")
            {
                return HttpStatusCode.InternalServerError;
            }

            return HttpStatusCode.BadRequest;
        }

        internal static HttpStatusCode GetExpectedStatusCode(ODataUri uri, IODataUriEvaluator evaluator)
        {
            bool specialStatusCodeIfNull = false;

            specialStatusCodeIfNull |= uri.IsNamedStream();
            specialStatusCodeIfNull |= uri.IsMediaResource();
            specialStatusCodeIfNull |= uri.IsEntity();
            specialStatusCodeIfNull |= uri.IsEntityReferenceLink();
            specialStatusCodeIfNull |= uri.IsPropertyValue();
            
            bool uriIsValue = uri.IsNamedStream() || uri.IsMediaResource();

            // For an action it is evaluated specially via the actionresponse verifier, skip eval here
            if (!uri.IsAction() && specialStatusCodeIfNull && evaluator.Evaluate(uri).IsNull)
            {
                if (uriIsValue)
                {
                    return HttpStatusCode.NoContent;
                }
                else
                {
                    return HttpStatusCode.NotFound;
                }
            }

            return HttpStatusCode.OK;
        }

        internal static void SetupProtocolRequest(QueryExpression expression, IODataRequestManager requestManager, IQueryToODataPayloadConverter queryToPayloadConverter, ODataUri uri, HttpHeaderCollection headers, string actionContentType, out Contracts.OData.ODataRequest request)
        {
            HttpVerb requestVerb = HttpVerb.Get;
            if (uri.IsAction())
            {
                requestVerb = HttpVerb.Post;
            }

            if (uri.IsWebInvokeServiceOperation())
            {
                requestVerb = HttpVerb.Post;
            }

            request = requestManager.BuildRequest(uri, requestVerb, headers);
            if (uri.IsAction())
            {
                var procedurePayload = queryToPayloadConverter.ComputePayload(expression) as ComplexInstance;

                if (procedurePayload != null)
                {
                    request.Headers.Add(HttpHeaders.ContentType, actionContentType);

                    FixupAddingResultWrappers(actionContentType, procedurePayload);

                    // TODO: Need to understand if product allow an Html form even if no parameters specified
                    request.Body = requestManager.BuildBody(actionContentType, uri, procedurePayload);
                }
            }
        }

        internal static void FixupAddingResultWrappers(string actionContentType, ComplexInstance procedurePayload)
        {
            if (actionContentType.Contains("odata.metadata=verbose"))
            {
                // Fixup to ensure payload is as expected for actions
                foreach (var primitiveMultiValueProperty in procedurePayload.Properties.OfType<PrimitiveMultiValueProperty>())
                {
                    primitiveMultiValueProperty.Value.Annotations.Add(new JsonCollectionResultWrapperAnnotation(false));
                    primitiveMultiValueProperty.Value.FullTypeName = null;
                }

                foreach (var complexMultiValueProperty in procedurePayload.Properties.OfType<ComplexMultiValueProperty>())
                {
                    complexMultiValueProperty.Value.Annotations.Add(new JsonCollectionResultWrapperAnnotation(false));
                    complexMultiValueProperty.Value.FullTypeName = null;
                }
            }
        }

        internal static string DetermineAcceptType(ODataUri uri, string defaultAcceptType)
        {
            if (uri.IsNamedStream() || uri.IsMediaResource())
            {
                return MimeTypes.Any;
            }

            if (uri.IsCount())
            {
                return MimeTypes.TextPlain;
            }

            if (uri.IsPropertyValue())
            {
                string propertyAcceptType = MimeTypes.Any;
                var propertySegment = uri.Segments[uri.Segments.Count - 2] as PropertySegment;
                ExceptionUtilities.CheckObjectNotNull(propertySegment, "Cannot get Property segment from uri");
                var mimeTypeAnnotation = propertySegment.Property.Annotations.OfType<MimeTypeAnnotation>().SingleOrDefault();
                if (mimeTypeAnnotation != null)
                {
                    propertyAcceptType = mimeTypeAnnotation.MimeTypeValue;
                }

                return propertyAcceptType;
            }

            return defaultAcceptType;
        }

        /// <summary>
        /// Resolves the Query expression, Construct request from Query Expression, send request and get response
        /// </summary>
        /// <param name="expression">query expression</param>
        /// <param name="request">OData request</param>
        /// <param name="response">OData response</param>
        /// <returns>returns the resolved query expression</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Need to return more than one result.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Need to return more than one result.")]
        protected QueryExpression ResolveExpressionAndGetResponse(QueryExpression expression, out Contracts.OData.ODataRequest request, out Contracts.OData.ODataResponse response)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            expression = this.QueryResolver.Resolve(expression);

            var uri = this.ProtocolTestServices.QueryToUriConverter.GenerateUriFromQuery(expression, this.ServiceRoot);

            var headers = new HttpHeaderCollection();

            headers.Accept = DetermineAcceptType(uri, this.DefaultAcceptType);

            if (this.DataServiceVersion != DataServiceProtocolVersion.Unspecified)
            {
                headers.DataServiceVersion = this.DataServiceVersion.ConvertToHeaderFormat();
            }

            if (this.MaxDataServiceVersion != DataServiceProtocolVersion.Unspecified)
            {
                headers.MaxDataServiceVersion = this.MaxDataServiceVersion.ConvertToHeaderFormat();
            }

            SetupProtocolRequest(expression, this.ProtocolTestServices.RequestManager, this.QueryToODataPayloadConverter, uri, headers, this.GetActionContentType(), out request);

            response = this.GetResponse(expression, request);
            return expression;
        }

        /// <summary>
        /// Sends request and gets response
        /// </summary>
        /// <param name="expression">The expected expression</param>
        /// <param name="request">The request to send</param>
        /// <returns>The response</returns>
        protected virtual ODataResponse GetResponse(QueryExpression expression, ODataRequest request)
        {
            return this.ProtocolTestServices.RequestManager.GetResponse(request);
        }

        /// <summary>
        /// Gets the value of the content type to be provided for the action payload, defaults to application/json
        /// </summary>
        /// <returns>Action Content Type</returns>
        protected virtual string GetActionContentType()
        {
            return MimeTypes.ApplicationJsonODataLightStreaming;
        }
    }
}
