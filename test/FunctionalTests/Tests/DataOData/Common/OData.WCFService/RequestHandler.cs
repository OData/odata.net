//---------------------------------------------------------------------
// <copyright file="RequestHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.OData;

    /// <summary>
    /// Base class for processing and responding to a client request.
    /// </summary>
    public abstract class RequestHandler
    {
        private readonly WebHeaderCollection incomingHeaders;
        private readonly string incomingMethod;
        private readonly string incomingAccept;

        /// <summary>
        /// RequestHandler constructor.
        /// </summary>
        protected RequestHandler()
        {
            var incomingContext = WebOperationContext.Current.IncomingRequest;
            this.incomingAccept = incomingContext.Accept;
            this.incomingHeaders = incomingContext.Headers;
            this.incomingMethod = incomingContext.Method;

            this.IncomingRequestUri = OperationContext.Current.RequestContext.RequestMessage.Properties.Via;
        }

        /// <summary>
        /// Gets or sets the data access context.
        /// </summary>
        public DataContext DataContext { get; set; }

        /// <summary>
        /// Gets or sets the service model.
        /// </summary>
        public IEdmModel Model { get; set; }

        /// <summary>
        /// Gets or sets the URI for the incoming request.
        /// </summary>
        public Uri IncomingRequestUri { get; private set; }

        /// <summary>
        /// Wraps the incoming message in a IODataRequestMessage implementation.
        /// </summary>
        /// <param name="messageBody">The incoming message body.</param>
        /// <returns>Wrapped request message.</returns>
        protected IncomingRequestMessage GetIncomingRequestMessage(Stream messageBody)
        {
            return new IncomingRequestMessage(messageBody, this.incomingHeaders, this.IncomingRequestUri, this.incomingMethod);
        }

        /// <summary>
        /// Creates the QueryContext based on the incoming request URI.
        /// </summary>
        /// <returns>The QueryContext for the incoming request URI.</returns>
        protected QueryContext GetDefaultQueryContext()
        {
            return QueryContext.ParseUri(this.IncomingRequestUri, this.Model);
        }

        /// <summary>
        /// Creates ODataMessageReaderSettings to use when reading incoming client messages.
        /// </summary>
        /// <returns>The default ODataMessageReaderSettings for this client request.</returns>
        protected ODataMessageReaderSettings GetDefaultReaderSettings()
        {
            return new ODataMessageReaderSettings();
        }

        /// <summary>
        /// Creates ODataMessageWriterSettings to use when writing responses to this client request.
        /// </summary>
        /// <returns>The default ODataMessageWriterSettings for this client request.</returns>
        protected ODataMessageWriterSettings GetDefaultWriterSettings()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { BaseUri = ServiceConstants.ServiceBaseUri };
            settings.SetServiceDocumentUri(new Uri(ServiceConstants.ServiceBaseUri.OriginalString));
            return settings;
        }

        /// <summary>
        /// Writes an OData response for this client request.
        /// </summary>
        /// <param name="status">The HTTP status code for the outgoing response.</param>
        /// <param name="writeAction">Delegate that writes the actual OData response.</param>
        /// <returns>A Stream containing the outgoing response.</returns>
        protected Stream WriteResponse(int status, Action<ODataMessageWriter, ODataMessageWriterSettings, InMemoryTestResponseMessage> writeAction)
        {
            var writerSettings = this.GetDefaultWriterSettings();
            writerSettings.SetContentType(this.incomingAccept, Encoding.UTF8.WebName);

            MemoryStream responseStream = new NonClosingStream();
            var responseMessage = new InMemoryTestResponseMessage(responseStream, status);

            using (var messageWriter = new ODataMessageWriter(responseMessage, writerSettings, this.Model))
            {
                try
                {
                    writeAction(messageWriter, writerSettings, responseMessage);
                }
                catch (Exception error)
                {
                    return this.WriteErrorResponse(500, error);
                }
            }

            responseStream.Seek(0, SeekOrigin.Begin);
            return responseStream;
        }

        /// <summary>
        /// Writes an OData error for this client request.
        /// </summary>
        /// <param name="errorCode">The HTTP status code for the outgoing response.</param>
        /// <param name="error">The exception that will be used to create the OData error.</param>
        /// <returns>A Stream containing the outgoing response.</returns>
        protected Stream WriteErrorResponse(int errorCode, Exception error)
        {
            return this.WriteResponse(
                errorCode,
                (writer, writerSettings, message) =>
                    writer.WriteError(
                         new ODataError
                         {
                             ErrorCode = errorCode.ToString(CultureInfo.InvariantCulture),
                             Message = error.Message,
                             InnerError = new ODataInnerError
                             {
                                 Message = error.Message,
                                 TypeName = error.GetType().FullName,
                                 StackTrace = error.StackTrace
                             }
                         },
                         /*includeDebugInformation*/true));
        }
    }
}