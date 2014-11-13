//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
