//---------------------------------------------------------------------
// <copyright file="ODataMessageReaderDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Base class for all deserializers using the ODataMessageReader.
    /// </summary>
    internal abstract class ODataMessageReaderDeserializer : Deserializer
    {
        /// <summary>
        /// The message reader being used.
        /// </summary>
        private readonly ODataMessageReader messageReader;

        /// <summary>
        /// Cached value indicating whether the request is JSON Light
        /// </summary>
        private bool? isJsonLightRequest = null;

        /// <summary>
        /// Initializes a new instance of <see cref="ODataMessageReaderDeserializer"/>.
        /// </summary>
        /// <param name="update">true if we're reading an update operation; false if not.</param>
        /// <param name="dataService">Data service for which the deserializer will act.</param>
        /// <param name="tracker">Tracker to use for modifications.</param>
        /// <param name="requestDescription">The request description to use.</param>
        /// <param name="enableODataServerBehavior">If true, the message reader settings will use the ODataServer behavior;
        /// if false, the message reader settings will use the default behavior.</param>
        internal ODataMessageReaderDeserializer(bool update, IDataService dataService, UpdateTracker tracker, RequestDescription requestDescription, bool enableODataServerBehavior)
            : base(update, dataService, tracker, requestDescription)
        {
            AstoriaRequestMessage requestMessage = dataService.OperationContext.RequestMessage;

            // WCF DS needs to treat content type */* as ATOM payload, so check for it here and override the content type header
            if (ContentTypeUtil.CompareMimeType(requestMessage.ContentType, XmlConstants.MimeAny))
            {
                requestMessage.ContentType = XmlConstants.MimeApplicationAtom;
            }

            this.messageReader = new ODataMessageReader(
                requestMessage,
                WebUtil.CreateMessageReaderSettings(dataService, enableODataServerBehavior),
                dataService.Provider.GetMetadataProviderEdmModel());
        }

        /// <summary>
        /// The message reader to use for reading the request body.
        /// </summary>
        protected ODataMessageReader MessageReader
        {
            get
            {
                return this.messageReader;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the request is json light
        /// </summary>
        protected override bool IsJsonLightRequest
        {
            get
            {
                if (!this.isJsonLightRequest.HasValue)
                {
                    this.isJsonLightRequest = ODataUtils.GetReadFormat(this.messageReader) == ODataFormat.Json;
                }

                return this.isJsonLightRequest.Value;
            }
        }

        public static object ParseJsonToPrimitiveValue(string rawValue)
        {
            Debug.Assert(rawValue != null && rawValue.Length > 0, "");
            ODataCollectionValue collectionValue = (ODataCollectionValue)
                Microsoft.OData.ODataUriUtils.ConvertFromUriLiteral(string.Format("[{0}]", rawValue), ODataVersion.V4);
            foreach (object item in collectionValue.Items)
            {
                return item;
            }

            return null;
        }

        /// <summary>
        /// Converts a primitive value read by ODataMessageReader into a primitive value acceptable by WCF DS.
        /// </summary>
        /// <param name="value">The value reported by ODataMessageReader.</param>
        /// <param name="resourceType">The resource type of the value to read, null if it is an open property value.</param>
        /// <returns>The value converted to the WCF DS value space.</returns>
        protected static object ConvertPrimitiveValue(object value, ref ResourceType resourceType)
        {
            Debug.Assert(resourceType == null || resourceType.ResourceTypeKind == ResourceTypeKind.Primitive, "Only primitive types are supported by this method.");

            // System.Xml.Linq.XElement and System.Data.Linq.Binaries primitive types are not supported by ODataLib directly,
            // so if the property is of one of those types, we need to convert the value to that type here.
            if (value != null)
            {
                Type instanceType;
                if (resourceType == null)
                {
                    resourceType = ResourceType.GetPrimitiveResourceType(value.GetType());
                    instanceType = resourceType.InstanceType;
                }
                else
                {
                    instanceType = resourceType.InstanceType;
                    if (instanceType == typeof(System.Xml.Linq.XElement))
                    {
                        string stringValue = value as string;
                        Debug.Assert(stringValue != null, "If the property is of type System.Xml.Linq.XElement then ODataLib should ahve read it as string.");
                        return System.Xml.Linq.XElement.Parse(stringValue, System.Xml.Linq.LoadOptions.PreserveWhitespace);
                    }
                    else if (instanceType == typeof(System.Data.Linq.Binary))
                    {
                        byte[] byteArray = value as byte[];
                        Debug.Assert(byteArray != null, "If the property is of type System.Data.Linq.Binary then ODataLib should have read it as byte[].");
                        return new System.Data.Linq.Binary(byteArray);
                    }
                }

                // It might be the case that store type is DateTime/DateTime? in which case the ResourceType is DateTimeOffset/DateTimeOffset?.
                if (instanceType == typeof(DateTime) || instanceType == typeof(DateTime?))
                {
                    Debug.Assert(value is DateTimeOffset, "Expecting DateTimeOffset for DateTime properties.");
                    value = WebUtil.ConvertDateTimeOffsetToDateTime((DateTimeOffset)value);
                }
            }

            return value;
        }

        /// <summary>
        /// Asserts that the format of the given reader is one of the given expected formats
        /// </summary>
        /// <param name="reader">The current reader</param>
        /// <param name="expectedFormats">The expected formats</param>
        [Conditional("DEBUG")]
        protected static void AssertReaderFormatIsExpected(ODataMessageReader reader, params ODataFormat[] expectedFormats)
        {
#if DEBUG
            Debug.Assert(expectedFormats.Contains(ODataUtils.GetReadFormat(reader)), "unexpected format encountered");
#endif
        }

        /// <summary>
        /// Reads the input request payload and returns the WCF DS value representation of it.
        /// </summary>
        /// <param name="segmentInfo">Info about the request to read.</param>
        /// <returns>The WCF DS representation of the value read.</returns>
        protected abstract object Read(SegmentInfo segmentInfo);

        /// <summary>
        /// Reads the given payload and return the top level object.
        /// </summary>
        /// <param name="segmentInfo">Info about the object being created.</param>
        /// <returns>Instance of the object created.</returns>
        protected override sealed object Deserialize(SegmentInfo segmentInfo)
        {
            try
            {
                return this.Read(segmentInfo);
            }
            catch (System.Xml.XmlException exception)
            {
                throw DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.DataServiceException_GeneralError, exception);
            }
            catch (ODataContentTypeException exception)
            {
                throw new DataServiceException(
                    415,
                    null,
                    Microsoft.OData.Service.Strings.DataServiceException_UnsupportedMediaType,
                    null,
                    exception);
            }
            catch (ODataException exception)
            {
                throw DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.DataServiceException_GeneralError, exception);
            }
        }

        /// <summary>Provides an opportunity to clean-up resources.</summary>
        /// <param name="disposing">
        /// Whether the call is being made from an explicit call to 
        /// IDisposable.Dispose() rather than through the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.messageReader.Dispose();
            }
        }

        /// <summary>
        /// Converts the value reported by OData reader into WCF DS resource.
        /// </summary>
        /// <param name="odataValue">The value reported by the reader.</param>
        /// <param name="resourceType">The expected resource type of the value. This should be null if we were reading open value.</param>
        /// <returns>The converted WCF DS resource. In case of complex or collection this will be a newly created instance.</returns>
        protected object ConvertValue(object odataValue, ref ResourceType resourceType)
        {
            if (odataValue == null)
            {
                return null;
            }

            ODataCollectionValue collection = odataValue as ODataCollectionValue;
            if (collection != null)
            {
                return this.ConvertCollection(collection, resourceType);
            }

            if (odataValue is ODataUntypedValue)
            {
                object primitiveVal = ParseJsonToPrimitiveValue((odataValue as ODataUntypedValue).RawValue);
                return ConvertPrimitiveValue(primitiveVal, ref resourceType);
            }

            Debug.Assert(!(odataValue is ODataStreamReferenceValue), "We should never get here for stream values.");
            return ConvertPrimitiveValue(odataValue, ref resourceType);
        }

        /// <summary>Applies a property from the reader to the specified resource.</summary>
        /// <param name='property'>The OData property to apply.</param>
        /// <param name='resourceType'>Type of resource.</param>
        /// <param name='resource'>Resource to set value on.</param>
        protected void ApplyProperty(ODataProperty property, ResourceType resourceType, object resource)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resource != null, "resource != null");

            string propertyName = property.Name;

            // Note that we include even stream properties in this lookup
            // This is not needed for the purposes of validation, since that has already been done by ODataLib reader.
            ResourceProperty resourceProperty = resourceType.TryResolvePropertyName(propertyName);
            ResourceType propertyResourceType;
            if (resourceProperty == null)
            {
                Debug.Assert(resourceType.IsOpenType, "ODataLib reader should have already failed on undefined properties on non-open types.");
                propertyResourceType = null;
            }
            else if (resourceProperty.Kind == ResourcePropertyKind.Stream)
            {
                // Ignore stream properties in update operations.
                return;
            }
            else
            {
                // Ignore key properties.
                if (this.Update && resourceProperty.IsOfKind(ResourcePropertyKind.Key))
                {
                    return;
                }

                propertyResourceType = resourceProperty.ResourceType;
            }

            object propertyValue = this.ConvertValue(property.Value, ref propertyResourceType);
            if (resourceProperty == null)
            {
                Deserializer.SetOpenPropertyValue(resource, propertyName, propertyValue, this.Service);
            }
            else
            {
                Deserializer.SetPropertyValue(resourceProperty, resource, propertyValue, this.Service);
            }

            return;
        }

        /// <summary>
        /// Gets the IEdmModel type reference for a specified <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="resourceType">The resource type to get the type reference for.</param>
        /// <param name="customAnnotations">Custom annotations to use when creating type reference.</param>
        /// <returns>The type reference.</returns>
        protected IEdmTypeReference GetTypeReference(ResourceType resourceType, List<KeyValuePair<string, object>> customAnnotations)
        {
            return this.Service.Provider.GetMetadataProviderEdmModel().EnsureTypeReference(resourceType, customAnnotations);
        }

        /// <summary>
        /// Gets the IEdmModel schema type for the specified <paramref name="resourceType"/>
        /// </summary>
        /// <param name="resourceType">The resource type to get the schema type for.</param>
        /// <returns>The schema type.</returns>
        protected IEdmSchemaType GetSchemaType(ResourceType resourceType)
        {
            return this.Service.Provider.GetMetadataProviderEdmModel().EnsureSchemaType(resourceType);
        }

        /// <summary>
        /// Gets the IEdmOperationI for a specified <paramref name="serviceOperation"/>.
        /// </summary>
        /// <param name="serviceOperation">The service action or function to get the function import for.</param>
        /// <returns>The function import.</returns>
        protected IEdmOperation GetOperation(OperationWrapper serviceOperation)
        {
            var model = this.Service.Provider.GetMetadataProviderEdmModel();
            model.EnsureDefaultEntityContainer().EnsureOperationImport(serviceOperation);
            return model.GetRelatedOperation(serviceOperation);
        }

        /// <summary>
        /// Gets the entity set for the specified <paramref name="resourceSet"/>
        /// </summary>
        /// <param name="resourceSet">ResourceSet instance.</param>
        /// <returns>an IEdmEntitySet instance for the given resource set.</returns>
        protected IEdmEntitySet GetEntitySet(ResourceSetWrapper resourceSet)
        {
            return this.Service.Provider.GetMetadataProviderEdmModel().EnsureEntitySet(resourceSet);
        }

        /// <summary>
        /// Converts the collection reported by OData reader into WCF DS collection resource.
        /// </summary>
        /// <param name="collection">The collection reported by the reader.</param>
        /// <param name="resourceType">The expected collection resource type.</param>
        /// <returns>THe newly created WCF DS collection resource.</returns>
        private object ConvertCollection(ODataCollectionValue collection, ResourceType resourceType)
        {
            Debug.Assert(collection != null, "collection != null");
            if (resourceType == null)
            {
                // Open collection property value - read the type from the value
                Debug.Assert(!string.IsNullOrEmpty(collection.TypeName), "ODataLib should have verified that open collection property value has a type name since we provided metadata.");
                string itemTypeName = CommonUtil.GetCollectionItemTypeName(collection.TypeName, false);
                var itemType = PrimitiveResourceTypeMap.TypeMap.GetPrimitive(itemTypeName);
                itemType = itemType ?? this.Service.Provider.TryResolveResourceType(itemTypeName);
                Debug.Assert(itemType != null, "The item Type in open collection property can not be resolved.");

                resourceType = CollectionResourceType.GetCollectionResourceType(itemType);
                Debug.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.Collection, "ODataLib should have verified that collection value has a collection resource type.");
            }

            Debug.Assert(
                collection.TypeName == null || collection.TypeName == resourceType.FullName,
                "We don't support any inheritance in collections, so the type of the collection should exactly match the type in the metadata.");

            CollectionResourceType collectionResourceType = resourceType as CollectionResourceType;
            Debug.Assert(collectionResourceType != null, "The resource type for collection must be a CollectionResourceType.");

            IList collectionList = Deserializer.CreateNewCollection();

            this.RecurseEnter();

            Debug.Assert(collection.Items != null, "The ODataLib reader should always populate the ODataCollectionValue.Items collection.");
            foreach (object odataItem in collection.Items)
            {
                ResourceType itemType = collectionResourceType.ItemType;
                collectionList.Add(this.ConvertValue(odataItem, ref itemType));
            }

            this.RecurseLeave();

            return Deserializer.GetReadOnlyCollection(collectionList, collectionResourceType);
        }
    }
}
