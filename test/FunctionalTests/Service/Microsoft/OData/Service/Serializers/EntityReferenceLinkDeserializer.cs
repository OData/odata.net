//---------------------------------------------------------------------
// <copyright file="EntityReferenceLinkDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData;

    /// <summary>
    /// Implements deserializer for entity reference links (the $refs payloads).
    /// </summary>
    internal sealed class EntityReferenceLinkDeserializer : ODataMessageReaderDeserializer
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EntityReferenceLinkDeserializer"/>.
        /// </summary>
        /// <param name="update">true if we're reading an update operation; false if not.</param>
        /// <param name="dataService">Data service for which the deserializer will act.</param>
        /// <param name="tracker">Tracker to use for modifications.</param>
        /// <param name="requestDescription">The request description to use.</param>
        internal EntityReferenceLinkDeserializer(bool update, IDataService dataService, UpdateTracker tracker, RequestDescription requestDescription)
            : base(update, dataService, tracker, requestDescription, true /*enableODataServerBehavior*/)
        {
        }

        /// <summary>
        /// Reads the input request payload and returns the WCF DS value representation of it.
        /// </summary>
        /// <param name="segmentInfo">Info about the request to read. For entity reference requests this is null.</param>
        /// <returns>The WCF DS representation of the value read. For entity reference link this is the Uri of the link.</returns>
        protected override object Read(SegmentInfo segmentInfo)
        {
            Debug.Assert(segmentInfo == null, "segmentInfo == null");
            Debug.Assert(this.RequestDescription.LinkUri, "The EntityReferenceLinkDeserializer only supports $ref payloads.");

            ODataEntityReferenceLink entityReferenceLink = this.MessageReader.ReadEntityReferenceLink();
            Debug.Assert(entityReferenceLink != null, "ReadEntityReferenceLink should never return null.");
            Uri entityReferenceUri = entityReferenceLink.Url;
            Debug.Assert(entityReferenceUri != null, "The Url of the entity reference link should never be null.");
            AssertReaderFormatIsExpected(this.MessageReader, ODataFormat.Json);

            // We must fail on empty URI
            string entityReferenceUriAsString = UriUtil.UriToString(entityReferenceUri);
            if (string.IsNullOrEmpty(entityReferenceUriAsString))
            {
                throw DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.BadRequest_MissingUriForLinkOperation);
            }

            // Resolve the URI against the service
            return RequestUriProcessor.GetAbsoluteUriFromReference(entityReferenceUri, this.Service.OperationContext.AbsoluteServiceUri);
        }
    }
}
