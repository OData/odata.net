//---------------------------------------------------------------------
// <copyright file="JsonLightObjectModelToPayloadElementConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Converts OData object model to ODataPayloadElement model.
    /// </summary>
    public class JsonLightObjectModelToPayloadElementConverter : ObjectModelToPayloadElementConverter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public JsonLightObjectModelToPayloadElementConverter()
        {
        }

        /// <summary>
        /// Virtual method to create the visitor to perform the conversion.
        /// </summary>
        /// <param name="response">true if payload represents a response payload, false if it's a request payload.</param>
        /// <param name="payloadContainsId">Whether or not the payload contains identity values for entries.</param>
        /// <param name="payloadContainsEtagForType">A function for determining whether the payload contains etag property values for a given type.</param>
        /// <returns>The newly created visitor.</returns>
        protected override ObjectModelToPayloadElementConverterVisitor CreateVisitor(bool response, bool payloadContainsId, Func<string, bool> payloadContainsEtagForType)
        {
            return new JsonLightObjectModelToPayloadElementConverterVisitor(response, this.RequestManager);
        }

        /// <summary>
        /// The inner visitor which performs the conversion.
        /// </summary>
        protected class JsonLightObjectModelToPayloadElementConverterVisitor : ObjectModelToPayloadElementConverterVisitor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="response">true if payload represents a response payload, false if it's a request payload.</param>
            /// <param name="requestManager">The request manager used to convert batch payloads.</param>
            public JsonLightObjectModelToPayloadElementConverterVisitor(bool response, IODataRequestManager requestManager)
                : base(response, requestManager, true, (t) => true)
            {
            }

            /// <summary>
            /// Visits a stream reference value (named stream).
            /// </summary>
            /// <param name="streamReferenceValue">The stream reference value to visit.</param>
            protected override ODataPayloadElement VisitStreamReferenceValue(ODataStreamReferenceValue streamReferenceValue)
            {
                ExceptionUtilities.CheckArgumentNotNull(streamReferenceValue, "streamReferenceValue");

                // Note that we're creating a named stream instance without a name here - name will be supplied by the caller if necessary.
                return new NamedStreamInstance()
                {
                    SourceLink = streamReferenceValue.ReadLink == null ? null : streamReferenceValue.ReadLink.OriginalString,
                    EditLink = streamReferenceValue.EditLink == null ? null : streamReferenceValue.EditLink.OriginalString,
                    ETag = streamReferenceValue.ETag,
                    SourceLinkContentType = streamReferenceValue.ContentType,
                    EditLinkContentType = streamReferenceValue.ContentType,
                };
            }
        }
    }
}
