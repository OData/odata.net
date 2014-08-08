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
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Metadata;

    /// <summary>
    /// Implements deserializer for top level properties.
    /// </summary>
    internal sealed class PropertyDeserializer : ODataMessageReaderDeserializer
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PropertyDeserializer"/>.
        /// </summary>
        /// <param name="update">true if we're reading an update operation; false if not.</param>
        /// <param name="dataService">Data service for which the deserializer will act.</param>
        /// <param name="tracker">Tracker to use for modifications.</param>
        /// <param name="requestDescription">The request description to use.</param>
        internal PropertyDeserializer(bool update, IDataService dataService, UpdateTracker tracker, RequestDescription requestDescription)
            : base(update, dataService, tracker, requestDescription, true /*enableWcfDataServicesServerBehavior*/)
        {
        }

        /// <summary>
        /// Reads the input request payload and returns the WCF DS value representation of it.
        /// </summary>
        /// <param name="segmentInfo">Info about the request to read.</param>
        /// <returns>The WCF DS representation of the value read.</returns>
        protected override object Read(SegmentInfo segmentInfo)
        {
            Debug.Assert(segmentInfo != null, "segmentInfo != null");
            Debug.Assert(
                segmentInfo.TargetKind == RequestTargetKind.Primitive ||
                segmentInfo.TargetKind == RequestTargetKind.ComplexObject ||
                segmentInfo.TargetKind == RequestTargetKind.Collection ||
                segmentInfo.TargetKind == RequestTargetKind.OpenProperty,
                "The PropertyDeserializer only supports Primitive, ComplexObject, Collection or OpenProperty target kinds.");

            ResourceProperty resourceProperty;
            ResourceType propertyResourceType;
            IEdmTypeReference propertyTypeReference;
            if (segmentInfo.TargetKind == RequestTargetKind.OpenProperty)
            {
                resourceProperty = null;
                propertyResourceType = null;
                propertyTypeReference = null;
            }
            else
            {
                resourceProperty = segmentInfo.ProjectedProperty;
                propertyResourceType = resourceProperty.ResourceType;
                propertyTypeReference = this.GetTypeReference(propertyResourceType, resourceProperty.CustomAnnotations.ToList());

                if (resourceProperty.Kind == ResourcePropertyKind.Primitive
                    && MetadataProviderUtils.ShouldDisablePrimitivePropertyNullValidation(resourceProperty, (IEdmPrimitiveTypeReference)propertyTypeReference))
                {
                    propertyTypeReference = this.GetSchemaType(propertyResourceType).ToTypeReference(true);
                }

                if (resourceProperty.Kind == ResourcePropertyKind.ComplexType && this.Service.Provider.HasReflectionOrEFProviderQueryBehavior && !propertyTypeReference.IsNullable)
                {
                    propertyTypeReference = this.GetSchemaType(propertyResourceType).ToTypeReference(true);
                }
            }

            ODataProperty property = this.MessageReader.ReadProperty(propertyTypeReference);
            Debug.Assert(property != null, "property != null");
            AssertReaderFormatIsExpected(this.MessageReader, ODataFormat.Atom, ODataFormat.VerboseJson, ODataFormat.Json);

            if (!(this.IsAtomRequest && (segmentInfo.TargetKind == RequestTargetKind.OpenProperty || property.Value == null)) &&
                !(this.IsVerboseJsonRequest && property.Name.Length == 0) &&
                !this.IsJsonLightRequest &&
                string.CompareOrdinal(segmentInfo.Identifier, property.Name) != 0)
            {
                throw DataServiceException.CreateBadRequestError(
                    System.Data.Services.Strings.PlainXml_IncorrectElementName(segmentInfo.Identifier, property.Name));
            }

            object propertyValue = property.Value;
            if (segmentInfo.TargetKind == RequestTargetKind.OpenProperty && propertyValue is ODataCollectionValue)
            {
                throw DataServiceException.CreateBadRequestError(
                    System.Data.Services.Strings.BadRequest_OpenCollectionProperty(property.Name));
            }

            object convertedValue = this.ConvertValue(propertyValue, ref propertyResourceType);

            if (segmentInfo.TargetKind == RequestTargetKind.OpenProperty)
            {
                // Set the target resource type for open properties so we can reason over the property type later.
                // The target resource type of an open property segment is determined when converting the payload value above.
                Debug.Assert(segmentInfo.TargetResourceType == null, "segmentInfo.TargetResourceType == null");
                segmentInfo.TargetResourceType = propertyResourceType;
            }
            else
            {
                Debug.Assert(segmentInfo.TargetResourceType.FullName == propertyResourceType.FullName, "segmentInfo.TargetResourceType == propertyResourceType");
            }

            return convertedValue;
        }
    }
}
