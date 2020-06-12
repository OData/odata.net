//---------------------------------------------------------------------
// <copyright file="ODataMessageWritingHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System.Diagnostics;
    using Microsoft.OData;

    /// <summary>
    /// Helper class for creating ODataLib writers, settings, and other write-related classes based on an instance of <see cref="RequestInfo"/>.
    /// </summary>
    internal class ODataMessageWritingHelper
    {
        /// <summary>The current request info.</summary>
        private readonly RequestInfo requestInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataMessageWritingHelper"/> class.
        /// </summary>
        /// <param name="requestInfo">The request info.</param>
        internal ODataMessageWritingHelper(RequestInfo requestInfo)
        {
            Debug.Assert(requestInfo != null, "requestInfo != null");
            this.requestInfo = requestInfo;
        }

        /// <summary>
        /// Create message writer settings for producing requests.
        /// </summary>
        /// <param name="isBatchPartRequest">if set to <c>true</c> indicates that this is a part of a batch request.</param>
        /// <param name="enableWritingODataAnnotationWithoutPrefix">Whether to enable writing odata annotation without prefix.</param>
        /// <returns>Newly created message writer settings.</returns>
        internal ODataMessageWriterSettings CreateSettings(bool isBatchPartRequest, bool enableWritingODataAnnotationWithoutPrefix)
        {
            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings
            {
                EnableCharactersCheck = false,

                // For operations inside batch, we need to dispose the stream. For top level requests,
                // we do not need to dispose the stream. Since for inner batch requests, the request
                // message is an internal implementation of IODataRequestMessage in ODataLib,
                // we can do this here.
                EnableMessageStreamDisposal = isBatchPartRequest,

                // When using relative URIs in batch requests, we need to set the baseUri in ODataWriter
                BaseUri = this.requestInfo.BaseUriResolver.BaseUriOrNull == null ? null : this.requestInfo.BaseUriResolver.GetBaseUriWithSlash()
            };

            // [#623] As client does not support DI currently, odata simplifiedoptions cannot be customize pre request.
            // Now, we just change the global options.
            // TODO: After finish the issue #623, need add the customize code of ODataAnnotationWithoutPrefix for each request
            CommonUtil.SetDefaultMessageQuotas(writerSettings.MessageQuotas);

            this.requestInfo.Configurations.RequestPipeline.ExecuteWriterSettingsConfiguration(writerSettings);

            return writerSettings;
        }

        /// <summary>
        /// Creates a writer for the given request message and settings.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="writerSettings">The writer settings.</param>
        /// <param name="isParameterPayload">true if the writer is intended to for a parameter payload, false otherwise.</param>
        /// <returns>Newly created writer.</returns>
        internal ODataMessageWriter CreateWriter(IODataRequestMessage requestMessage, ODataMessageWriterSettings writerSettings, bool isParameterPayload)
        {
            Debug.Assert(requestMessage != null, "requestMessage != null");
            Debug.Assert(writerSettings != null, "writerSettings != null");

            DataServiceClientFormat.ValidateCanWriteRequestFormat(requestMessage);

            // When calling Execute() to invoke an Action, the client doesn't support parsing the target url
            // to determine which IEdmOperationImport to pass to the ODL writer. So the ODL writer is
            // serializing the parameter payload without metadata. Setting the model to null so ODL doesn't
            // do unnecessary validations when writing without metadata.
            var model = isParameterPayload ? null : this.requestInfo.Model;
            return new ODataMessageWriter(requestMessage, writerSettings, model);
        }

        /// <summary>
        /// Creates a request message with the given arguments.
        /// </summary>
        /// <param name="requestMessageArgs">The request message args.</param>
        /// <returns>Newly created request message.</returns>
        internal ODataRequestMessageWrapper CreateRequestMessage(BuildingRequestEventArgs requestMessageArgs)
        {
            return ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, this.requestInfo);
        }
    }
}