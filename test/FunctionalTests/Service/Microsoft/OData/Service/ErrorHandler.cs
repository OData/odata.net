//---------------------------------------------------------------------
// <copyright file="ErrorHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// Provides support for orchestrating error handling at different points in the processing cycle and for
    /// serializing structured errors.
    /// </summary>
    internal class ErrorHandler
    {
#if !EF6Provider
        #region Private fields

        /// <summary>The maximum number of nested inner errors to write.</summary>
        private const int MaxInnerErrorDepth = 100;

        /// <summary>Arguments for the exception being handled.</summary>
        private readonly HandleExceptionArgs exceptionArgs;

        /// <summary>Response version.</summary>
        private readonly Version responseVersion;

        /// <summary>Content type and charset to use when writing the error.</summary>
        private readonly string contentTypeWithCharset;

        /// <summary>Content type to use when writing the error.</summary>
        private readonly string contentType;

        /// <summary>Encoding to use when writing the error.</summary>
        private readonly Encoding encoding;

        /// <summary>
        /// Prevents a default instance of the <see cref="ErrorHandler"/> class from being created.
        /// </summary>
        /// <param name="exception">The exception to be written.</param>
        /// <param name="verbose">if set to <c>true</c> indicates verbose errors should be written.</param>
        /// <param name="responseVersion">The response version.</param>
        /// <param name="acceptableContentTypes">The acceptable content types.</param>
        /// <param name="requestAcceptCharsetHeader">The request accept charset header.</param>
        private ErrorHandler(Exception exception, bool verbose, Version responseVersion, string acceptableContentTypes, string requestAcceptCharsetHeader)
        {
            this.contentType = GetErrorResponseContentType(acceptableContentTypes, responseVersion);
            this.encoding = GetEncodingForError(requestAcceptCharsetHeader);

            // [GQL Failure, ODataLib integration] Server does not write charset in content-type header on error responses
            // [GQL Failure, Astoria-ODataLib Integration] ContentType provided to DataService.HandleException does not match final header value in $batch
            // With the integration of ODataLib into the server-side , batch-parts which have top level error messages in them
            // have the charset header value appended to the content-type of the response.
            // To retain a consistent behavior between top level error messages in the batch and non-batch cases, we will append 
            // the charset header to the response content type before passing it to the HandleException method on the IDataServiceHost.
            this.contentTypeWithCharset = string.Concat(this.contentType, ";", XmlConstants.HttpCharsetParameter, "=", this.encoding.WebName);

            this.exceptionArgs = new HandleExceptionArgs(exception, false, this.contentTypeWithCharset, verbose);

            Debug.Assert(responseVersion != null, "responseVersion != null");
            this.responseVersion = responseVersion;
        }

        #endregion Constructors
#endif
        #region Internal methods.
#if !EF6Provider
        /// <summary>Handles an exception when processing a batch response.</summary>
        /// <param name='service'>Data service doing the processing.</param>
        /// <param name="requestMessage">requestMessage holding information about the request that caused an error</param>
        /// <param name="responseMessage">responseMessage to which we need to write the exception message</param>
        /// <param name='exception'>Exception thrown.</param>
        /// <param name='batchWriter'>Output writer for the batch.</param>
        /// <param name="responseStream">Underlying response stream.</param>
        /// <param name="defaultResponseVersion">The data service version to use for response, if it cannot be computed from the requestMessage.</param>
        internal static void HandleBatchOperationError(IDataService service, AstoriaRequestMessage requestMessage, IODataResponseMessage responseMessage, Exception exception, ODataBatchWriter batchWriter, Stream responseStream, Version defaultResponseVersion)
        {
            Debug.Assert(service != null, "service != null");
            Debug.Assert(exception != null, "exception != null");
            Debug.Assert(batchWriter != null, "batchWriter != null");
            Debug.Assert(service.Configuration != null, "service.Configuration != null");
            Debug.Assert(CommonUtil.IsCatchableExceptionType(exception), "CommonUtil.IsCatchableExceptionType(exception)");

            ErrorHandler handler = CreateHandler(service, requestMessage, exception, defaultResponseVersion);

            service.InternalHandleException(handler.exceptionArgs);

            if (requestMessage != null && responseMessage != null)
            {
                responseMessage.SetHeader(XmlConstants.HttpODataVersion, handler.responseVersion.ToString(2) + ";");
                requestMessage.ProcessException(handler.exceptionArgs);

                // if ProcessBenignException returns anything, we can safely not write to the stream.
                if (ProcessBenignException(exception, service) != null)
                {
                    return;
                }
            }

            if (requestMessage != null)
            {
                responseMessage = requestMessage.BatchServiceHost.GetOperationResponseMessage();
                WebUtil.SetResponseHeadersForBatchRequests(responseMessage, requestMessage.BatchServiceHost);
            }
            else
            {
                responseMessage = batchWriter.CreateOperationResponseMessage(null);
                responseMessage.StatusCode = handler.exceptionArgs.ResponseStatusCode;
            }

            MessageWriterBuilder messageWriterBuilder = MessageWriterBuilder.ForError(
                null,
                service, 
                handler.responseVersion,
                responseMessage,
                handler.contentType,
                null /*acceptCharsetHeaderValue*/);
            using (ODataMessageWriter messageWriter = messageWriterBuilder.CreateWriter())
            {
                ODataError error = handler.exceptionArgs.CreateODataError();
                WriteErrorWithFallbackForXml(messageWriter, handler.encoding, responseStream, handler.exceptionArgs, error, messageWriterBuilder);
            }
        }

        /// <summary>Handles an exception when processing a batch request.</summary>
        /// <param name='service'>Data service doing the processing.</param>
        /// <param name='exception'>Exception thrown.</param>
        /// <param name='batchWriter'>Output writer for the batch.</param>
        /// <param name="responseStream">Underlying response stream.</param>
        internal static void HandleBatchInStreamError(IDataService service, Exception exception, ODataBatchWriter batchWriter, Stream responseStream)
        {
            Debug.Assert(service != null, "service != null");
            Debug.Assert(exception != null, "exception != null");
            Debug.Assert(responseStream != null, "responseStream != null");
            Debug.Assert(service.Configuration != null, "service.Configuration != null - it should have been initialized by now");
            Debug.Assert(CommonUtil.IsCatchableExceptionType(exception), "CommonUtil.IsCatchableExceptionType(exception) - ");

            AstoriaRequestMessage requestMessage = service.OperationContext == null ? null : service.OperationContext.RequestMessage;

            ErrorHandler handler = CreateHandler(service, requestMessage, exception, VersionUtil.DataServiceDefaultResponseVersion);

            service.InternalHandleException(handler.exceptionArgs);

            // Make sure to flush the batch writer before we write anything to the underlying stream
            batchWriter.Flush();

            // Note the OData protocol spec did not defined the behavior when an exception is encountered outside of a batch operation.
            // The batch writer in ODataLib doesn't allow WriteError in this case.
            // Unfortunately the shipped behavior on the server is we serialize out an error payload in XML format. We need to keep the
            // existing behavior. The batch client doesn't know how to deserialize an error payload outside of a batch operation however.
            using (XmlWriter xmlWriter = XmlUtil.CreateXmlWriterAndWriteProcessingInstruction(responseStream, handler.encoding))
            {
                ODataError error = handler.exceptionArgs.CreateODataError();
                ErrorUtils.WriteXmlError(xmlWriter, error, handler.exceptionArgs.UseVerboseErrors, MaxInnerErrorDepth);
            }
        }

        /// <summary>Handles an exception before the response has been written out.</summary>
        /// <param name='exception'>Exception thrown.</param>
        /// <param name='service'>Data service doing the processing.</param>
        /// <returns>An action that can serialize the exception into a stream.</returns>
        internal static Action<Stream> HandleBeforeWritingException(Exception exception, IDataService service)
        {
            Debug.Assert(CommonUtil.IsCatchableExceptionType(exception), "CommonUtil.IsCatchableExceptionType(exception)");
            Debug.Assert(exception != null, "exception != null");
            Debug.Assert(service != null, "service != null");

            AstoriaRequestMessage requestMessage = service.OperationContext.RequestMessage;
            Debug.Assert(requestMessage != null, "requestMessage != null");

            ErrorHandler handler = CreateHandler(service, requestMessage, exception, VersionUtil.DataServiceDefaultResponseVersion);

            service.InternalHandleException(handler.exceptionArgs);

            service.OperationContext.ResponseMessage.SetHeader(XmlConstants.HttpODataVersion, handler.responseVersion.ToString(2) + ";");
            requestMessage.ProcessException(handler.exceptionArgs);
            
            Action<Stream> action = ProcessBenignException(exception, service);
            if (action != null)
            {
                return action;
            }

            MessageWriterBuilder messageWriterBuilder = MessageWriterBuilder.ForError(
                service.OperationContext.RequestMessage.AbsoluteServiceUri,
                service, 
                handler.responseVersion, 
                service.OperationContext.ResponseMessage, 
                handler.contentType,
                service.OperationContext.RequestMessage.GetRequestAcceptCharsetHeader());

            ODataMessageWriter messageWriter = messageWriterBuilder.CreateWriter();
            ODataUtils.SetHeadersForPayload(messageWriter, ODataPayloadKind.Error);

            return stream =>
                   {
                       service.OperationContext.ResponseMessage.SetStream(stream);
                       ODataError error = handler.exceptionArgs.CreateODataError();
                       WriteErrorWithFallbackForXml(messageWriter, handler.encoding, stream, handler.exceptionArgs, error, messageWriterBuilder);
                   };
        }

        /// <summary>
        /// Handles an exception that occurred while writing a response.
        /// </summary>
        /// <param name="service">Data service doing the processing.</param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="responseMessage">The response message.</param>
        /// <param name="messageWriter">The message writer, if null this will fall back to writing a raw XML error to the stream.</param>
        /// <param name="encoding">The encoding to while writing the error.</param>
        /// <param name="responseStream">The response stream to write the error to.</param>
        /// <param name="messageWriterBuilder">MessageWriterBuilder to use in case a new ODataMessageWriter needs to be constructed.</param>
        internal static void HandleExceptionWhileWriting(IDataService service, Exception exception, IODataResponseMessage responseMessage, ODataMessageWriter messageWriter, Encoding encoding, Stream responseStream, MessageWriterBuilder messageWriterBuilder)
        {
            Debug.Assert(service != null, "service != null");
            Debug.Assert(service.Configuration != null, "service.Configuration != null");
            Debug.Assert(exception != null, "exception != null");
            Debug.Assert(CommonUtil.IsCatchableExceptionType(exception), "CommonUtil.IsCatchableExceptionType(exception)");
            Debug.Assert(responseMessage != null, "responseMessage != null");

            string contentType = responseMessage.GetHeader(XmlConstants.HttpContentType);

            HandleExceptionArgs args = new HandleExceptionArgs(exception, true, contentType, service.Configuration.UseVerboseErrors);
            service.InternalHandleException(args);
            service.OperationContext.RequestMessage.ProcessException(args);

            ODataError error = args.CreateODataError();
            WriteErrorWithFallbackForXml(messageWriter, encoding, responseStream, args, error, messageWriterBuilder);
        }
#endif
        /// <summary>Handles the specified <paramref name='exception'/>.</summary>
        /// <param name='exception'>Exception to handle</param>
        /// <remarks>The caller should re-throw the original exception if this method returns normally.</remarks>
        internal static void HandleTargetInvocationException(TargetInvocationException exception)
        {
            Debug.Assert(exception != null, "exception != null");

            DataServiceException dataException = exception.InnerException as DataServiceException;
            if (dataException == null)
            {
                return;
            }

            throw new DataServiceException(
                dataException.StatusCode,
                dataException.ErrorCode,
                dataException.Message,
                dataException.MessageLanguage,
                exception);
        }

        #endregion Internal methods.
#if !EF6Provider
        #region Private methods.

        /// <summary>
        /// Check to see if the given excpetion is a benign one such as statusCode = 304.  If yes we return an action that can
        /// serialize the exception into a stream.  Other wise we return null.
        /// </summary>
        /// <param name="exception">Exception to be processed</param>
        /// <param name="service">Data service instance</param>
        /// <returns>An action that can serialize the exception into a stream.</returns>
        private static Action<Stream> ProcessBenignException(Exception exception, IDataService service)
        {
            DataServiceException dataServiceException = exception as DataServiceException;
            if (dataServiceException != null)
            {
                if (dataServiceException.StatusCode == (int)HttpStatusCode.NotModified)
                {
                    Debug.Assert(service.OperationContext != null, "service.OperationContext != null");
                    service.OperationContext.ResponseStatusCode = (int)HttpStatusCode.NotModified;

                    // For 304, we MUST return an empty message-body.
                    return WebUtil.GetEmptyStreamWriter();
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the content type for error serialization based on the accept header and version.
        /// </summary>
        /// <param name="requestAcceptHeader">The accept header value.</param>
        /// <param name="responseVersion">The response version.</param>
        /// <returns>The content type to use for the error response.</returns>
        private static string GetErrorResponseContentType(string requestAcceptHeader, Version responseVersion)
        {
            string contentType = null;
            if (requestAcceptHeader != null)
            {
                try
                {
                    contentType = ContentTypeUtil.SelectResponseMediaType(requestAcceptHeader, false /*entityTarget*/, responseVersion);
                }
                catch (DataServiceException)
                {
                    // Ignore formatting errors in Accept and rely on text.
                }
            }

            // TODO: change fallback type to JSON
            return contentType ?? XmlConstants.MimeApplicationXml;
        }

        /// <summary>
        /// Gets the encoding for error serialization based on the accept charset header.
        /// </summary>
        /// <param name="requestAcceptCharsetHeader">The request accept charset header.</param>
        /// <returns>The encoding to use.</returns>
        private static Encoding GetEncodingForError(string requestAcceptCharsetHeader)
        {
            Encoding encoding = null;
            if (requestAcceptCharsetHeader != null)
            {
                try
                {
                    encoding = ContentTypeUtil.EncodingFromAcceptCharset(requestAcceptCharsetHeader);
                }
                catch (DataServiceException)
                {
                    // Ignore formatting erros in Accept-Charset and rely on text.
                }
            }

            return encoding ?? ContentTypeUtil.FallbackEncoding;
        }

        /// <summary>
        /// Creates an error handler for the given exception.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="defaultResponseVersion">The default/minimum response version.</param>
        /// <returns>The newly created error handler.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0014:DoNotHandleProhibitedExceptionsRule", Justification = "We're calling this correctly")]
        private static ErrorHandler CreateHandler(IDataService service, AstoriaRequestMessage requestMessage, Exception exception, Version defaultResponseVersion)
        {
            Debug.Assert(service != null, "service != null");
            Debug.Assert(service.Configuration != null, "service.Configuration != null");

            string acceptableContentTypes = null;
            string requestAcceptCharsetHeader = null;
            Version responseVersion = defaultResponseVersion;

            if (requestMessage != null)
            {
                acceptableContentTypes = requestMessage.GetAcceptableContentTypes();
                requestAcceptCharsetHeader = requestMessage.GetRequestAcceptCharsetHeader();

                try
                {
                    Version maxProtocolVersion = service.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion();
                    requestMessage.InitializeRequestVersionHeaders(maxProtocolVersion);
                    responseVersion = VersionUtil.GetResponseVersionForError(requestMessage.GetAcceptableContentTypes(), requestMessage.RequestMaxVersion, maxProtocolVersion);
                }
                catch (Exception e)
                {
                    if (!CommonUtil.IsCatchableExceptionType(e))
                    {
                        throw;
                    }

                    // Ignore exceptions as we should use the default response version.
                }
            }

            return new ErrorHandler(exception, service.Configuration.UseVerboseErrors, responseVersion, acceptableContentTypes, requestAcceptCharsetHeader);
        }

        /// <summary>
        /// Writes the error with fallback logic for XML cases where the writer is in an error state and a new writer must be created.
        /// </summary>
        /// <param name="messageWriter">The message writer.</param>
        /// <param name="encoding">The encoding to use for the error if we have to fallback.</param>
        /// <param name="responseStream">The response stream to write to in the fallback case.</param>
        /// <param name="args">The args for the error.</param>
        /// <param name="error">The error to write.</param>
        /// <param name="messageWriterBuilder">MessageWriterBuilder to use if a new ODataMessageWriter needs to be constructed.</param>
        private static void WriteErrorWithFallbackForXml(ODataMessageWriter messageWriter, Encoding encoding, Stream responseStream, HandleExceptionArgs args, ODataError error, MessageWriterBuilder messageWriterBuilder)
        {
            Debug.Assert(args != null, "args != null");
#if DEBUG
            Debug.Assert(args.ProcessExceptionWasCalled, "ProcessException was not called by the time we tried to serialze this error message with ODataLib.");
#endif

            if (messageWriter != null)
            {
                try
                {
                    // If the XmlWriter inside the ODataMessageWriter had entered Error state, ODataMessageWriter.WriteError would throw an InvalidOperationException
                    // when we try to write to it. Note that XmlWriter doesn't always throw an XmlException when it enters Error state.
                    // The right thing to do is we don't write any more because at this point we don't know what's been written to the underlying
                    // stream. However we still should flush the writer to make sure that all the content that was written but is sitting in the buffers actually appears 
                    // in the stream before writing the instream error. Otherwise the buffer will be flushed when disposing the writer later and we would end up with
                    // either content written after the instream error (this would also result in having the Xml declaration in the middle of the payload -
                    // [Astoria-ODataLib-Integration] In-stream errors due to XmlExceptions are written out backwards (error before partial valid payload)) or, 
                    // hypothetically, the instream error in the middle of the other content that was already partially written. For example we can end up with a payload that 
                    // looks like <element attr="val<m:error... The XmlReader would not be able to parse the error payload in this case. Disposing the writer will flush the buffer. 
                    // It is fine to do it since the writer is not usable at this point anyways. Also note that the writer will be disposed more than once (e.g. in finally block
                    // in ResponseBodySerializer) but only the first call will have any effect.
                    // However since in the versions we shipped we always create a new XmlWriter to serialize the error payload when the existing
                    // one is in error state, we will continue to do the same to avoid introducing any breaking change here.
                    messageWriter.WriteError(error, args.UseVerboseErrors);
                }
                catch (ODataException e)
                {
                    // Yikes, ODataLib threw while writing the error. This tends to happen if the service author did something invalid during custom
                    // error handling, such as add an custom instance annotation to the error payload. In this dire case, we treat it almost like 
                    // an in-stream error, and abort the previous writing. We write out the new error. Note that this will produce an invalid payload like
                    // the situation noted above with XmlWriter errors.
                    WebUtil.Dispose(messageWriter);
                    messageWriterBuilder.SetMessageForErrorInError();
                    var newErrorWriter = messageWriterBuilder.CreateWriter();
                    ODataError errorWhileWritingOtherError = new ODataError()
                    {
                        ErrorCode = "500",
                        InnerError = new ODataInnerError(e),
                        Message = Strings.ErrorHandler_ErrorWhileWritingError
                    };

                    newErrorWriter.WriteError(errorWhileWritingOtherError, args.UseVerboseErrors);
                }
                catch (InvalidOperationException)
                {
                    Debug.Assert(ContentTypeUtil.IsNotJson(args.ResponseContentType), "Should never get here for JSON responses");
                    WebUtil.Dispose(messageWriter);

                    // if either an InvalidOperationException was encountered (see comment above) or the message writer was null, write the error out manually.
                    Debug.Assert(responseStream != null, "responseStream != null");
                    using (XmlWriter xmlWriter = XmlWriter.Create(responseStream, XmlUtil.CreateXmlWriterSettings(encoding)))
                    {
                        ErrorUtils.WriteXmlError(xmlWriter, error, args.UseVerboseErrors, MaxInnerErrorDepth);
                    }
                }
            }
        }

        #endregion Private methods.
#endif
    }
}
