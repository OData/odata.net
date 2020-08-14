//---------------------------------------------------------------------
// <copyright file="DataServiceClientFormat.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region namespaces
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    #endregion
    /// <summary>
    /// Tracks the user-preferred format which the client should use when making requests.
    /// </summary>
    public sealed class DataServiceClientFormat
    {
        /// <summary>MIME type for JSON bodies in light mode (http://www.iana.org/assignments/media-types/application/).</summary>
        private const string MimeApplicationJsonODataLight = "application/json;odata.metadata=minimal";

        /// <summary>MIME type for JSON (https://www.ietf.org/rfc/rfc4627.txt).</summary>
        private const string MimeApplicationJson = "application/json";

        /// <summary>MIME type for JSON bodies in light mode with all metadata.</summary>
        private const string MimeApplicationJsonODataLightWithAllMetadata = "application/json;odata.metadata=full";

        /// <summary>MIME type for changeset multipart/mixed</summary>
        private const string MimeMultiPartMixed = "multipart/mixed";

        /// <summary>text for the utf8 encoding</summary>
        private const string Utf8Encoding = "UTF-8";

        /// <summary>The character set the client wants the response to be in.</summary>
        private const string HttpAcceptCharset = "Accept-Charset";

        /// <summary>The context this format instance is associated with.</summary>
        private readonly DataServiceContext context;

        /// <summary>The service edm model.</summary>
        private IEdmModel serviceModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceClientFormat"/> class.
        /// </summary>
        /// <param name="context">DataServiceContext instance associated with this format.</param>
        internal DataServiceClientFormat(DataServiceContext context)
        {
            Debug.Assert(context != null, "Context cannot be null for DataServiceClientFormat");

            // On V6.0.2, we change the default format to be json for the client
            this.ODataFormat = ODataFormat.Json;
            this.context = context;
        }

        /// <summary>
        /// Gets the current format. Defaults to Atom if nothing else has been specified.
        /// </summary>
        public ODataFormat ODataFormat { get; private set; }

        /// <summary>
        /// Invoked when using the parameterless UseJson method in order to get the service model.
        /// </summary>
        public Func<IEdmModel> LoadServiceModel { get; set; }

        /// <summary>
        /// Gets the service model.
        /// </summary>
        internal IEdmModel ServiceModel
        {
            get
            {
                if (serviceModel != null)
                {
                    return serviceModel;
                }

                if (LoadServiceModel != null)
                {
                    serviceModel = LoadServiceModel();
                }
                else
                {
                    serviceModel = LoadServiceModelFromNetwork();
                }

                PopulateEdmStructuredSchemaElements();
                return serviceModel;
            }
        }

        /// <summary>
        /// Invoked during test cases to fake out network calls to get the metadata
        /// returns a string that is passed to the csdl parser and is used to bypass the network call while testing
        /// </summary>
        internal Func<DataServiceClientRequestMessage> InjectMetadataHttpNetworkRequest { get; set; }

        /// <summary>
        /// Indicates that the client should use the efficient JSON format.
        /// </summary>
        /// <param name="serviceModel">The model of the service.</param>
        public void UseJson(IEdmModel serviceModel)
        {
            Util.CheckArgumentNull(serviceModel, "serviceModel");

            this.ODataFormat = ODataFormat.Json;
            this.serviceModel = serviceModel;
            PopulateEdmStructuredSchemaElements();
        }

        /// <summary>
        /// Indicates that the client should use the efficient JSON format. Will invoke the LoadServiceModel delegate property in order to get the required service model.
        /// </summary>
        public void UseJson()
        {
            if (this.ServiceModel == null)
            {
                throw new InvalidOperationException(Strings.DataServiceClientFormat_LoadServiceModelRequired);
            }

            this.ODataFormat = ODataFormat.Json;
        }

        /// <summary>
        /// Sets the value of the Accept header to the appropriate value for the current format.
        /// </summary>
        /// <param name="headers">The headers to modify.</param>
        internal void SetRequestAcceptHeader(HeaderCollection headers)
        {
            this.SetAcceptHeaderAndCharset(headers, ChooseMediaType(false));
        }

        /// <summary>
        /// Sets the value of the Accept header for a query.
        /// </summary>
        /// <param name="headers">The headers to modify.</param>
        /// <param name="components">The query components for the request.</param>
        internal void SetRequestAcceptHeaderForQuery(HeaderCollection headers, QueryComponents components)
        {
            this.SetAcceptHeaderAndCharset(headers, ChooseMediaType(components.HasSelectQueryOption));
        }

        /// <summary>
        /// Sets the value of the Accept header for a stream request (will set it to '*/*').
        /// </summary>
        /// <param name="headers">The headers to modify.</param>
        internal void SetRequestAcceptHeaderForStream(HeaderCollection headers)
        {
            this.SetAcceptHeaderAndCharset(headers, XmlConstants.MimeAny);
        }

        /// <summary>
        /// Sets the value of the Accept header for a count request (will set it to 'text/plain').
        /// </summary>
        /// <param name="headers">The headers to modify.</param>
        internal void SetRequestAcceptHeaderForCount(HeaderCollection headers)
        {
            this.SetAcceptHeaderAndCharset(headers, XmlConstants.MimeTextPlain);
        }

        /// <summary>
        /// Sets the value of the Accept header for a batch request
        /// Will set it to 'multipart/mixed' for a multipart batch request
        /// Will set it to 'application/json' for a json batch request
        /// </summary>
        /// <param name="headers">The headers to modify.</param>
        internal void SetRequestAcceptHeaderForBatch(HeaderCollection headers)
        {
            bool useJsonBatch = headers.GetHeader(XmlConstants.HttpContentType).Equals(MimeApplicationJson, StringComparison.Ordinal);
            this.SetAcceptHeaderAndCharset(headers, useJsonBatch ? MimeApplicationJson : MimeMultiPartMixed);
        }

        /// <summary>
        /// Sets the value of the ContentType header on the specified entry request to the appropriate value for the current format.
        /// </summary>
        /// <param name="headers">Dictionary of request headers.</param>
        internal void SetRequestContentTypeForEntry(HeaderCollection headers)
        {
            this.SetRequestContentTypeHeader(headers, ChooseMediaType(false));
        }

        /// <summary>
        /// Sets the value of the  Content-Type header a request with operation parameters to the appropriate value for the current format.
        /// </summary>
        /// <param name="headers">Dictionary of request headers.</param>
        internal void SetRequestContentTypeForOperationParameters(HeaderCollection headers)
        {
            // Note: There has never been an atom or xml format for parameters.
            this.SetRequestContentTypeHeader(headers, MimeApplicationJsonODataLight);
        }

        /// <summary>
        /// Sets the value of the ContentType header on the specified links request to the appropriate value for the current format.
        /// </summary>
        /// <param name="headers">Dictionary of request headers.</param>
        internal void SetRequestContentTypeForLinks(HeaderCollection headers)
        {
            this.SetRequestContentTypeHeader(headers, ChooseMediaType(false));
        }

        /// <summary>
        /// Validates that we can write the request format.
        /// </summary>
        /// <param name="requestMessage">The request message to get the format from.</param>
        internal static void ValidateCanWriteRequestFormat(IODataRequestMessage requestMessage)
        {
            string contentType = requestMessage.GetHeader(XmlConstants.HttpContentType);
            ValidateContentType(contentType);
        }

        /// <summary>
        /// Validates that we can read the response format.
        /// </summary>
        /// <param name="responseMessage">The response message to get the format from.</param>
        internal static void ValidateCanReadResponseFormat(IODataResponseMessage responseMessage)
        {
            string contentType = responseMessage.GetHeader(XmlConstants.HttpContentType);
            ValidateContentType(contentType);
        }

        /// <summary>
        /// Loads the metadata and converts it into an EdmModel that is then used by a dataservice context
        /// This allows the user to use the DataServiceContext directly without having to manually pass an IEdmModel in the Format
        /// </summary>
        /// <returns>A service model to be used in format tracking</returns>
        internal IEdmModel LoadServiceModelFromNetwork()
        {
            DataServiceClientRequestMessage httpRequest;
            BuildingRequestEventArgs requestEventArgs = null;

            // test hook for injecting a network request to use instead of the default
            if (InjectMetadataHttpNetworkRequest != null)
            {
                httpRequest = InjectMetadataHttpNetworkRequest();
            }
            else
            {
                requestEventArgs = new BuildingRequestEventArgs(
                    "GET",
                    context.GetMetadataUri(),
                    null,
                    null,
                    context.HttpStack);

                // fire the right events if they exist to allow user to modify the request
                if (context.HasBuildingRequestEventHandlers)
                {
                    requestEventArgs = context.CreateRequestArgsAndFireBuildingRequest(
                        requestEventArgs.Method,
                        requestEventArgs.RequestUri,
                        requestEventArgs.HeaderCollection,
                        requestEventArgs.ClientHttpStack,
                        requestEventArgs.Descriptor);
                }

                DataServiceClientRequestMessageArgs args = new DataServiceClientRequestMessageArgs(
                    requestEventArgs.Method,
                    requestEventArgs.RequestUri,
                    context.UseDefaultCredentials,
                    context.UsePostTunneling,
                    requestEventArgs.Headers);

                httpRequest = new HttpClientRequestMessage(args);
            }

            Descriptor descriptor = requestEventArgs != null ? requestEventArgs.Descriptor : null;

            // fire the right events if they exist
            if (context.HasSendingRequest2EventHandlers)
            {
                SendingRequest2EventArgs eventArgs = new SendingRequest2EventArgs(
                    httpRequest,
                    descriptor,
                    false);

                context.FireSendingRequest2(eventArgs);
            }

            Task<IODataResponseMessage> asyncResponse =
                Task<IODataResponseMessage>.Factory.FromAsync(httpRequest.BeginGetResponse, httpRequest.EndGetResponse,
                    httpRequest);
            IODataResponseMessage response = asyncResponse.GetAwaiter().GetResult();

            ReceivingResponseEventArgs responseEvent = new ReceivingResponseEventArgs(response, descriptor);

            context.FireReceivingResponseEvent(responseEvent);

            using (StreamReader streamReader = new StreamReader(response.GetStream()))
            using (XmlReader xmlReader = XmlReader.Create(streamReader))
            {
                return CsdlReader.Parse(xmlReader);
            }
        }

        /// <summary>
        /// Validates that we can read or write a message with the given content-type value.
        /// </summary>
        /// <param name="contentType">The content-type value in question.</param>
        private static void ValidateContentType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return;
            }

            // Ideally ODataLib should have a public API to get the ODataFormat from the content-type header.
            // Unfortunately since that's not available, we will process the content-type value to determine if the format is JSON Light.
            string mime;
            ContentTypeUtil.ReadContentType(contentType, out mime);
        }

        /// <summary>
        /// Sets the request's content type header.
        /// </summary>
        /// <param name="headers">Dictionary of request headers.</param>
        /// <param name="mediaType">content type</param>
        private void SetRequestContentTypeHeader(HeaderCollection headers, string mediaType)
        {
            if (mediaType == MimeApplicationJsonODataLight)
            {
                // set the request version to 4.0
                headers.SetRequestVersion(Util.ODataVersion4, this.context.MaxProtocolVersionAsVersion);
            }

            headers.SetHeaderIfUnset(XmlConstants.HttpContentType, mediaType);
        }

        /// <summary>
        /// Sets the accept header to the given value and the charset to UTF-8.
        /// </summary>
        /// <param name="headers">The headers to update.</param>
        /// <param name="mediaType">The media type for the accept header.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "If this becomes static, then so do its more visible callers, and we do not want to provide a mix of instance and static methods on this class.")]
        private void SetAcceptHeaderAndCharset(HeaderCollection headers, string mediaType)
        {
            // NOTE: we intentionally do NOT set the DSV header for 'accept' as content-negotiation
            // is primarily about determining how to respond and not how to interpret the request.
            // It is entirely valid to send a V1 request and get a V3 response.
            // (We do set the DSV to 3 for Content-Type above)
            headers.SetHeaderIfUnset(XmlConstants.HttpAccept, mediaType);
            headers.SetHeaderIfUnset(HttpAcceptCharset, Utf8Encoding);
        }

        /// <summary>
        /// Chooses between using JSON-Light and the context-dependent media type for when Atom is selected based on the user-selected format.
        /// </summary>
        /// <param name="hasSelectQueryOption">
        ///   Whether or not the select query option is present in the request URI.
        ///   If true, indicates that the client should ask the server to include all metadata in a JSON-Light response.
        /// </param>
        /// <returns>The media type to use (either JSON-Light or the provided value)</returns>
        private static string ChooseMediaType(bool hasSelectQueryOption)
        {
            if (hasSelectQueryOption)
            {
                return MimeApplicationJsonODataLightWithAllMetadata;
            }

            return MimeApplicationJsonODataLight;
        }

        /// <summary>
        /// Populates the client edm model EdmStructuredSchemaElements dictionary with the schema elements in the service model
        /// </summary>
        private void PopulateEdmStructuredSchemaElements()
        {
            if (serviceModel != null && this.context.Model != null)
            {
                foreach (var element in serviceModel.SchemaElements.Where(el => el is IEdmStructuredType))
                {
                    this.context.Model.EdmStructuredSchemaElements.TryAdd(element.Name, element);
                }
            }
        }
    }
}
