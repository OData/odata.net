//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Serializers
{
    using System;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;

    /// <summary>
    /// Implements deserializer for entity reference links (the $links payloads).
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
            : base(update, dataService, tracker, requestDescription, true /*enableWcfDataServicesServerBehavior*/)
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
            Debug.Assert(this.RequestDescription.LinkUri, "The EntityReferenceLinkDeserializer only supports $link payloads.");

            // Get the entity type on which the navigation property exists
            int entityResourceIndex = Deserializer.GetIndexOfEntityResourceToModify(this.RequestDescription);
            Debug.Assert(entityResourceIndex != -1, "This method should never be called for request that doesn't have a parent resource");
            var entityType = (IEdmEntityType)this.GetSchemaType(this.RequestDescription.SegmentInfos[entityResourceIndex].TargetResourceType);

            // Get the target navigation property
            Debug.Assert(this.RequestDescription.Property != null, "this.RequestDescription.Property != null");
            Debug.Assert(this.RequestDescription.Property.ResourceType.ResourceTypeKind == Providers.ResourceTypeKind.EntityType, "Must be navigation property");
            var navigationProperty = (IEdmNavigationProperty)entityType.FindProperty(this.RequestDescription.Property.Name);

            ODataEntityReferenceLink entityReferenceLink = this.MessageReader.ReadEntityReferenceLink(navigationProperty);
            Debug.Assert(entityReferenceLink != null, "ReadEntityReferenceLink should never return null.");
            Uri entityReferenceUri = entityReferenceLink.Url;
            Debug.Assert(entityReferenceUri != null, "The Url of the entity reference link should never be null.");
            AssertReaderFormatIsExpected(this.MessageReader, ODataFormat.Atom, ODataFormat.VerboseJson, ODataFormat.Json);

            // We must fail on empty URI
            string entityReferenceUriAsString = UriUtil.UriToString(entityReferenceUri);
            if (string.IsNullOrEmpty(entityReferenceUriAsString))
            {
                throw DataServiceException.CreateBadRequestError(System.Data.Services.Strings.BadRequest_MissingUriForLinkOperation);
            }

            // Resolve the URI against the service
            return RequestUriProcessor.GetAbsoluteUriFromReference(entityReferenceUri, this.Service.OperationContext.AbsoluteServiceUri, this.RequestDescription.RequestVersion);
        }
    }
}
