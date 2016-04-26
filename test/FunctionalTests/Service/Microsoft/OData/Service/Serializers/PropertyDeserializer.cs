//---------------------------------------------------------------------
// <copyright file="PropertyDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Providers;

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
            : base(update, dataService, tracker, requestDescription, true /*enableODataServerBehavior*/)
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

                // WCF DS server allows null values for primitive properties. For non top-level properties, custom annotations are added 
                // to ensure ODataLib does not perform any validation. The provider can choose to throw in which case, the response code will be 500. The
                // same applies to top-level primitive properties. So, we ensure the type reference carries the correct value for the nullability facet.
                if (resourceProperty.Kind == ResourcePropertyKind.Primitive
                    && MetadataProviderUtils.ShouldDisablePrimitivePropertyNullValidation(resourceProperty, (IEdmPrimitiveTypeReference)propertyTypeReference))
                {
                    propertyTypeReference = this.GetSchemaType(propertyResourceType).ToTypeReference(true);
                }

                // WCF DS server allows null values for complex properties.  For non top-level properties, custom annotations are added 
                // to ensure ODataLib does not perform any validation. The provider can choose to throw in which case, the response code will be 500. The
                // same applies to top-level complex properties. So, we ensure the type reference carries the correct value for the nullability facet.
                if (resourceProperty.Kind == ResourcePropertyKind.ComplexType && this.Service.Provider.HasReflectionOrEFProviderQueryBehavior && !propertyTypeReference.IsNullable)
                {
                    propertyTypeReference = this.GetSchemaType(propertyResourceType).ToTypeReference(true);
                }
            }

            ODataProperty property = this.MessageReader.ReadProperty(propertyTypeReference);
            Debug.Assert(property != null, "property != null");
            AssertReaderFormatIsExpected(this.MessageReader, ODataFormat.Json);

            // On V4, it seems this checking logic is useless. Will remove it after fully understanding.
            // PlainXmlDeserializer - PUT to a property with m:null='true' doesn't verify the name of the property matches
            //
            // PUT to open property with XML payload ignores the name of the property in the payload.
            //  For backward compat reasons we must not fail if the property name doesn't match and the value is null or it's an open property (in XML case).
            //
            // V2 Server reads JSON complex values without property wrapper, ODataLib will report these as ODataProperty with empty name, so for those
            //  we need to not compare the property name.
            //
            // For Json light, we do not validate the property name since in requests we do not parse the metadata URI and thus
            //   will always report 'value'.
            if (!this.IsJsonLightRequest && string.CompareOrdinal(segmentInfo.Identifier, property.Name) != 0)
            {
                throw DataServiceException.CreateBadRequestError(
                    Microsoft.OData.Service.Strings.PlainXml_IncorrectElementName(segmentInfo.Identifier, property.Name));
            }

            object propertyValue = property.Value;

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
