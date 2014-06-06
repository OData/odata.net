//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Internal;

    /// <summary>
    /// Factory for creating and configuring message writers and their settings.
    /// </summary>
    internal class MessageWriterBuilder
    {
        /// <summary>The current writer settings.</summary>
        private readonly ODataMessageWriterSettings writerSettings;

        /// <summary>The current model.</summary>
        private readonly IEdmModel model;

        /// <summary>The current response message.</summary>
        private IODataResponseMessage responseMessage;

        /// <summary>
        /// Prevents a default instance of the <see cref="MessageWriterBuilder"/> class from being created.
        /// </summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="responseVersion">The response version.</param>
        /// <param name="dataService">The data service.</param>
        /// <param name="responseMessage">The response message.</param>
        /// <param name="model">The model to provide to the message writer.</param>
        private MessageWriterBuilder(Uri serviceUri, Version responseVersion, IDataService dataService, IODataResponseMessage responseMessage, IEdmModel model)
        {
            this.writerSettings = CreateMessageWriterSettings();
            ApplyCommonSettings(this.writerSettings, serviceUri, responseVersion, dataService, responseMessage);

            Debug.Assert(responseMessage != null, "responseMessage != null");
            this.responseMessage = responseMessage;

            this.model = model;
        }

        /// <summary>
        /// Gets the writer settings.
        /// </summary>
        internal ODataMessageWriterSettings WriterSettings
        {
            get { return this.writerSettings; }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        internal IEdmModel Model
        {
            get { return this.model; }
        }

        /// <summary>
        /// Create a new instance of ODataMessageWriterSettings for normal requests.
        /// </summary>
        /// <param name="dataService">Data service instance.</param>
        /// <param name="requestDescription">The current request description.</param>
        /// <param name="responseMessage">IODataResponseMessage implementation.</param>
        /// <param name="model">The model to provide to the message writer.</param>
        /// <returns>An instance of a message writer with the appropriate settings.</returns>
        internal static MessageWriterBuilder ForNormalRequest(IDataService dataService, RequestDescription requestDescription, IODataResponseMessage responseMessage, IEdmModel model)
        {
            Debug.Assert(dataService != null, "dataService != null");
            Debug.Assert(dataService.OperationContext != null, "dataService.OperationContext != null");
            Debug.Assert(requestDescription != null, "requestDescription != null");
            Debug.Assert(dataService.OperationContext.RequestMessage != null, "dataService.OperationContext.RequestMessage != null");
            Debug.Assert(responseMessage != null, "responseMessage != null");

            Uri serviceUri = dataService.OperationContext.AbsoluteServiceUri;
            Version responseVersion = requestDescription.ActualResponseVersion;

            MessageWriterBuilder messageWriterBuilder = new MessageWriterBuilder(serviceUri, responseVersion, dataService, responseMessage, model);

            if (requestDescription.ResponsePayloadKind == ODataPayloadKind.Value && !string.IsNullOrEmpty(requestDescription.MimeType))
            {
                messageWriterBuilder.WriterSettings.SetContentType(ODataFormat.RawValue);
            }
            else
            {
                string acceptHeaderValue = dataService.OperationContext.RequestMessage.GetAcceptableContentTypes();

                // In V1/V2 we defaulted to charset=utf-8 for the response when there was no specific Accept-Charset.
                // ODataMessageWriter uses a different default in some cases depending on the media type, so we need to override that here.
                string requestAcceptCharSet = dataService.OperationContext.RequestMessage.GetRequestAcceptCharsetHeader();
                if (string.IsNullOrEmpty(requestAcceptCharSet) || requestAcceptCharSet == "*")
                {
                    requestAcceptCharSet = XmlConstants.Utf8Encoding;
                }

                messageWriterBuilder.WriterSettings.SetContentType(acceptHeaderValue, requestAcceptCharSet);
            }

            // always set the metadata document URI. ODataLib will decide whether or not to write it.
            messageWriterBuilder.WriterSettings.ODataUri = new ODataUri()
            {
                ServiceRoot = serviceUri,
                SelectAndExpand = requestDescription.ExpandAndSelect.Clause,
                Path = requestDescription.Path
            };

            messageWriterBuilder.WriterSettings.JsonPCallback = requestDescription.JsonPaddingFunctionName;

            return messageWriterBuilder;
        }

        /// <summary>
        /// Create a new instance of ODataMessageWriterSettings for batch requests.
        /// </summary>
        /// <param name="dataService">Data service instance.</param>
        /// <returns>An instance of a message writer with the appropriate settings.</returns>
        internal static MessageWriterBuilder ForBatch(IDataService dataService)
        {
            Uri serviceUri = dataService.OperationContext.RequestMessage.AbsoluteServiceUri;
            Version responseVersion = VersionUtil.GetEffectiveMaxResponseVersion(dataService.OperationContext.RequestMessage.RequestMaxVersion, dataService.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion());

            MessageWriterBuilder messageWriterBuilder = new MessageWriterBuilder(serviceUri, responseVersion, dataService, dataService.OperationContext.ResponseMessage, null /*model*/);

            // Astoria does not do content negotiation for the top level batch payload at all in V1/V2
            // Hence passing */* as the accept header value.
            messageWriterBuilder.WriterSettings.SetContentType(XmlConstants.MimeAny, null /*acceptableCharSets*/);

            return messageWriterBuilder;
        }

        /// <summary>
        /// Create a new instance of ODataMessageWriterSettings for errors.
        /// </summary>
        /// <param name="serviceUri">Service base uri.</param>
        /// <param name="dataService">Data service instance.</param>
        /// <param name="responseVersion">Version of the response payload.</param>
        /// <param name="responseMessage">IODataResponseMessage implementation.</param>
        /// <param name="acceptHeaderValue">Accept header value.</param>
        /// <param name="acceptCharSetHeaderValue">Accept charset header value.</param>
        /// <returns>An instance of a message writer with the appropriate settings.</returns>
        internal static MessageWriterBuilder ForError(Uri serviceUri, IDataService dataService, Version responseVersion, IODataResponseMessage responseMessage, string acceptHeaderValue, string acceptCharSetHeaderValue)
        {
            MessageWriterBuilder messageWriterBuilder = new MessageWriterBuilder(serviceUri, responseVersion, dataService, responseMessage, null /*model*/);

            messageWriterBuilder.WriterSettings.SetContentType(acceptHeaderValue, acceptCharSetHeaderValue);

            return messageWriterBuilder;
        }

        /// <summary>
        /// Applies common settings from the parameters to the given writer settings.
        /// </summary>
        /// <param name="writerSettings">The writer settings to modify.</param>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="responseVersion">The response version.</param>
        /// <param name="dataService">The data service.</param>
        /// <param name="responseMessage">The response message.</param>
        internal static void ApplyCommonSettings(ODataMessageWriterSettings writerSettings, Uri serviceUri, Version responseVersion, IDataService dataService, IODataResponseMessage responseMessage)
        {
            writerSettings.Version = CommonUtil.ConvertToODataVersion(responseVersion);
            writerSettings.PayloadBaseUri = serviceUri;

            writerSettings.EnableODataServerBehavior(
                dataService.Configuration.DataServiceBehavior.AlwaysUseDefaultXmlNamespaceForRootElement);
            writerSettings.DisableMessageStreamDisposal = responseMessage is AstoriaResponseMessage;
        }

        /// <summary>
        /// Creates a new message writer settings instance.
        /// </summary>
        /// <returns>A new settings instance.</returns>
        internal static ODataMessageWriterSettings CreateMessageWriterSettings()
        {
            var writerSettings = new ODataMessageWriterSettings { Indent = false, CheckCharacters = false };
            writerSettings.EnableAtomSupport();
            CommonUtil.SetDefaultMessageQuotas(writerSettings.MessageQuotas);
            return writerSettings;
        }

        /// <summary>
        /// Creates a new message writer from the current settings, message, and model.
        /// </summary>
        /// <returns>A new message writer.</returns>
        internal ODataMessageWriter CreateWriter()
        {
            return new ODataMessageWriter(this.responseMessage, this.WriterSettings, this.Model);
        }

        /// <summary>
        /// Erases the current response message, replacing it with a new response messaage specifically
        /// for handling errors that occur while writing errors. The stream is the same as the original
        /// message, but everything else is ignored.
        /// </summary>
        internal void SetMessageForErrorInError()
        {
            this.responseMessage = new PartiallyWrittenStreamMessage(this.responseMessage.GetStream());
        }

        /// <summary>
        /// Get resource path from base uri and result uri
        /// </summary>
        /// <param name="serviceBase">The service base uri.</param>
        /// <param name="resultUri">The result uri for payload, should use canonical uri here.</param>
        /// <returns>The resource path of the payload.</returns>
        private static string GetResourcePath(Uri serviceBase, Uri resultUri)
        {
            string baseUriPath = serviceBase.AbsolutePath;
            string resultUriPath = resultUri.AbsolutePath;
            Debug.Assert(resultUriPath.StartsWith(baseUriPath, StringComparison.OrdinalIgnoreCase), "Result uri should start with service base uri.");
            return resultUriPath.Remove(0, baseUriPath.Length).Trim('/');
        }
    }
}
