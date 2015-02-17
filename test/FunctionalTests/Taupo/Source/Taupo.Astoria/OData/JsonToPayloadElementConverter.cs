//---------------------------------------------------------------------
// <copyright file="JsonToPayloadElementConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// The converter from an atom/Json into a rich payload element representation.
    /// </summary>
    [ImplementationName(typeof(IJsonToPayloadElementConverter), "Default", HelpText = "The default converter an atom/Json to rich payload element representation.")]
    public class JsonToPayloadElementConverter : IJsonToPayloadElementConverter
    {
        private const string ResultsFieldName = "results";
        private const string CountFieldName = "__count";
        private const string NextLinkFieldName = "__next";
        private const string MetadataFieldName = ODataConstants.JsonMetadataPropertyName;
        private const string DeferredFieldName = "__deferred";
        private const string ETagFieldName = "etag";
        private const string TypeFieldName = "type";
        private const string UriFieldName = "uri";
        private const string IdFieldName = "id";
        private const string SecurityTag = "d";
        private const string AssociationUriFieldName = "associationuri";
        private const string LinkInfoFieldName = "__linkInfo";
        private const string MediaResourceFieldName = "__mediaresource";
        private const string MediaSrcFieldName = "media_src";
        private const string EditMediaFieldName = "edit_media";
        private const string MediaETagFieldName = "media_etag";
        private const string ContentTypeFieldName = "content_type";
        private const string PropertiesFieldName = "properties";
        private bool isRootLevel = true;

        /// <summary>
        /// Gets or sets the clr to data type converter to use
        /// </summary>
        [InjectDependency]
        public IClrToPrimitiveDataTypeConverter PrimitiveDataTypeConverter { get; set; }

        /// <summary>
        /// Gets or sets the spatial converter to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ISpatialPrimitiveToODataJsonValueConverter SpatialConverter { get; set; }

        /// <summary>
        /// Converts the given Json element into a rich payload element representation.
        /// </summary>
        /// <param name="jsonValue">Json element to convert.</param>
        /// <returns>A payload element representing the given element.</returns>
        public ODataPayloadElement ConvertToPayloadElement(JsonValue jsonValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(jsonValue, "jsonValue");
            ExceptionUtilities.Assert(this.isRootLevel, "Should only be called for the root level value. Otherwise use ConvertValue");

            try
            {
                this.isRootLevel = false;

                ODataPayloadElement converted;
                if (jsonValue.JsonType == JsonValueType.JsonObject)
                {
                    converted = this.ConvertRootObject((JsonObject)jsonValue);
                }
                else
                {
                    converted = this.ConvertValue(jsonValue);
                }

                return converted;
            }
            finally
            {
                this.isRootLevel = true;
            }
        }

        internal bool TryGetSpatialValue(JsonObject jsonObject, out ODataPayloadElement elem)
        {
            PrimitiveValue value;
            ExceptionUtilities.CheckObjectNotNull(this.SpatialConverter, "Cannot tell if value was spatial without spatial converter");
            if (this.SpatialConverter.TryConvertIfSpatial(jsonObject, out value))
            {
                elem = value;
                return true;
            }

            elem = null;
            return false;
        }

        private static ODataPayloadElement CreateNullElement(ODataPayloadElementType elementType)
        {
            if (elementType == ODataPayloadElementType.PrimitiveValue)
            {
                return new PrimitiveValue(null, null);
            }
            else if (elementType == ODataPayloadElementType.DeferredLink)
            {
                return new DeferredLink();
            }
            else if (elementType == ODataPayloadElementType.ComplexInstance)
            {
                return new ComplexInstance(null, true);
            }
            else
            {
                ExceptionUtilities.Assert(elementType == ODataPayloadElementType.EntityInstance, "Element type was unexpected type '{0}'", elementType);
                return new EntityInstance(null, true);
            }
        }

        private static ODataPayloadElementCollection CreateCollectionForElementType(ODataPayloadElementType elementType)
        {
            if (elementType == ODataPayloadElementType.PrimitiveValue)
            {
                return new PrimitiveCollection();
            }
            else if (elementType == ODataPayloadElementType.DeferredLink)
            {
                return new LinkCollection();
            }
            else if (elementType == ODataPayloadElementType.ComplexInstance)
            {
                return new ComplexInstanceCollection();
            }
            else
            {
                ExceptionUtilities.Assert(elementType == ODataPayloadElementType.EntityInstance, "Element type was unexpected type '{0}'", elementType);
                return new EntitySetInstance();
            }
        }

        private ODataPayloadElement ConvertRootObject(JsonObject jsonObject)
        {
            JsonValue toConvert;
            if (!this.TryGetSecurityPropertyValue(jsonObject, out toConvert))
            {
                toConvert = jsonObject;
            }
            else if (toConvert.JsonType == JsonValueType.JsonObject)
            {
                // If toConvert is a json object it means it has already been unwrapped of the d: wrapper.
                // This is done to update the jsonObject to be the unwrapped version of the payload.
                jsonObject = (JsonObject)toConvert;
            }

            ODataPayloadElement converted = this.ConvertValue(toConvert);

            // an untyped root-level complex instance with one property should be 'unwrapped'
            if (converted.ElementType == ODataPayloadElementType.ComplexInstance)
            {
                var complex = (ComplexInstance)converted;

                // if there are multiple properties or any type information/metadata, do not unwrap it
                if (complex.Properties.Count() == 1 && jsonObject.Properties.Count() == 1)
                {
                    converted = complex.Properties.Single();
                }
            }

            return converted;
        }

        private ODataPayloadElement ConvertValue(JsonValue jsonValue)
        {
            if (jsonValue.JsonType == JsonValueType.JsonArray)
            {
                return this.ConvertArray((JsonArray)jsonValue, false);
            }
            else if (jsonValue.JsonType == JsonValueType.JsonObject)
            {
                return this.ConvertObject((JsonObject)jsonValue);
            }
            else
            {
                // Assume it is JsonPrimitive
                ExceptionUtilities.Assert(jsonValue.JsonType == JsonValueType.JsonPrimitiveValue, "Unrecognized type");
                return new PrimitiveValue(null, ((JsonPrimitiveValue)jsonValue).Value);
            }
        }

        /// <summary>
        /// Converts the given name/value pair into a property element.
        /// And infers the type of property from the converted value.
        /// </summary>
        /// <param name="jsonProperty">the property value</param>
        /// <returns>the converted property</returns>
        private PropertyInstance ConvertProperty(JsonProperty jsonProperty)
        {
            if (jsonProperty.Value.JsonType == JsonValueType.JsonPrimitiveValue && ((JsonPrimitiveValue)jsonProperty.Value).Value == null)
            {
                return new NullPropertyInstance()
                {
                    Name = jsonProperty.Name
                };
            }
            else
            {
                ODataPayloadElement elem = this.ConvertValue(jsonProperty.Value);
                ExceptionUtilities.CheckObjectNotNull(elem, "Converted property value was null");

                if (elem.ElementType == ODataPayloadElementType.PrimitiveValue)
                {
                    return new PrimitiveProperty(jsonProperty.Name, (PrimitiveValue)elem);
                }
                else if (elem.ElementType == ODataPayloadElementType.ComplexInstance)
                {
                    return new ComplexProperty(jsonProperty.Name, (ComplexInstance)elem);
                }
                else if (elem.ElementType == ODataPayloadElementType.EntityInstance)
                {
                    return new NavigationPropertyInstance(jsonProperty.Name, new ExpandedLink(elem));
                }
                else if (elem.ElementType == ODataPayloadElementType.DeferredLink)
                {
                    DeferredLink deferredLink = (DeferredLink)elem; 
                    return new NavigationPropertyInstance(jsonProperty.Name, deferredLink);
                }
                else if (elem.ElementType == ODataPayloadElementType.EntitySetInstance)
                {            
                    return new NavigationPropertyInstance(jsonProperty.Name, elem);
                }
                else if (elem.ElementType == ODataPayloadElementType.ComplexMultiValue)
                {
                    ComplexMultiValue complexMultiValue = (ComplexMultiValue)elem;
                    return new ComplexMultiValueProperty(jsonProperty.Name, complexMultiValue);
                }
                else if (elem.ElementType == ODataPayloadElementType.PrimitiveMultiValue)
                {
                    PrimitiveMultiValue primitiveMultiValue = (PrimitiveMultiValue)elem;
                    return new PrimitiveMultiValueProperty(jsonProperty.Name, primitiveMultiValue);
                }
                else if (elem.ElementType == ODataPayloadElementType.ComplexInstanceCollection)
                {
                    ComplexInstanceCollection complexCollection = (ComplexInstanceCollection)elem;
                    return new ComplexMultiValueProperty(jsonProperty.Name, new ComplexMultiValue(null, false, complexCollection.ToArray()));
                }
                else if (elem.ElementType == ODataPayloadElementType.PrimitiveCollection)
                {
                    PrimitiveCollection primitiveCollection = (PrimitiveCollection)elem;
                    return new PrimitiveMultiValueProperty(jsonProperty.Name, new PrimitiveMultiValue(null, false, primitiveCollection.ToArray()));
                }
                else if (elem.ElementType == ODataPayloadElementType.NamedStreamInstance)
                {
                    NamedStreamInstance nsi = (NamedStreamInstance)elem;
                    nsi.Name = jsonProperty.Name;
                    return nsi;
                }
                else
                {
                    ExceptionUtilities.Assert(elem.ElementType == ODataPayloadElementType.EmptyUntypedCollection, "Do not know how to handle element of type" + elem.ElementType);
                    return new EmptyCollectionProperty(jsonProperty.Name, (EmptyUntypedCollection)elem);
                }
            }
        }

        /// <summary>
        /// Converts a JSObject into a payload element representation.
        /// The conversion is dependent on the object itself, 
        /// and can result in many different payload elements.
        /// </summary>
        /// <param name="jsonObject">The object to convert</param>
        /// <returns>A payload element equivalent to the given object</returns>
        private ODataPayloadElement ConvertObject(JsonObject jsonObject)
        {
            ODataPayloadElement elem;

            if (this.TryGetSpatialValue(jsonObject, out elem))
            {
                return elem;
            }
            else if (this.TryGetLink(jsonObject, out elem))
            {
                return elem;
            }
            else if (this.TryGetDeferredLink(jsonObject, out elem))
            {
                return elem;
            }
            else if (this.TryGetCollectionType(jsonObject, out elem))
            {
                return elem;
            }
            else if (this.TryGetEntityInstance(jsonObject, out elem))
            {
                return elem;
            }
            else if (this.TryGetNamedStream(jsonObject, out elem))
            {
                return elem;
            }
            else
            {
                ExceptionUtilities.Assert(this.TryGetComplexInstance(jsonObject, out elem), "Do not know how to convert element");
                return elem;
            }
        }

        /// <summary>
        /// Coverts an array of json values into a collection payload elements
        /// Note: if the array is empty, then its type cannot be inferred,
        /// so an UntypedEmptyCollection will be returned
        /// </summary>
        /// <param name="jsonArray">the array to convert</param>
        /// <param name="inResultsWrapper">Whether the array is inside a results wrapper</param>
        /// <returns>
        /// a collection of payload elements
        /// </returns>
        private ODataPayloadElementCollection ConvertArray(JsonArray jsonArray, bool inResultsWrapper)
        {
            ExceptionUtilities.CheckArgumentNotNull(jsonArray, "jsonArray");

            if (jsonArray.Elements.Count == 0)
            {
                return new EmptyUntypedCollection() { Annotations = { new JsonCollectionResultWrapperAnnotation(inResultsWrapper) } };
            }

            var convertedElements = new List<ODataPayloadElement>();
            ODataPayloadElementType? elementType = null;
            foreach (var childObj in jsonArray.Elements)
            {
                ExceptionUtilities.Assert(childObj.JsonType != JsonValueType.JsonArray, "Cannot have nested array");
                var converted = this.ConvertValue(childObj);

                if (converted.ElementType == ODataPayloadElementType.PrimitiveValue)
                {
                    if (((PrimitiveValue)converted).IsNull)
                    {
                        convertedElements.Add(null);
                        continue;
                    }
                }

                if (!elementType.HasValue)
                {
                    elementType = converted.ElementType;
                }
                
                convertedElements.Add(converted);
            }

            if (!elementType.HasValue)
            {
                ExceptionUtilities.Assert(convertedElements.All(e => e == null), "Element type should only be un-initialized if all elements were null");
                elementType = ODataPayloadElementType.PrimitiveValue;
            }

            var collection = CreateCollectionForElementType(elementType.Value);
            foreach (var converted in convertedElements)
            {
                if (converted == null)
                {
                    collection.Add(CreateNullElement(elementType.Value));
                }
                else
                {
                    collection.Add(converted);
                }
            }

            collection.Annotations.Add(new JsonCollectionResultWrapperAnnotation(inResultsWrapper));

            return collection;
        }

        private string GetMetadataPropertyValue(JsonObject jsonObject, string propName)
        {
            string metadataPropValue = null;

            // read the property, if present
            JsonProperty metadataProp = jsonObject.Properties.SingleOrDefault(p => p.Name == MetadataFieldName);
            if (metadataProp != null)
            {
                JsonObject metadataObj = metadataProp.Value as JsonObject;
                ExceptionUtilities.CheckObjectNotNull(metadataObj, "__metadata property did not contain an object. Value was: '{0}'", metadataProp.Value.JsonType);

                JsonProperty jsonProp = metadataObj.Properties.SingleOrDefault(p => p.Name == propName);
                if (jsonProp != null)
                {
                    metadataPropValue = (string)((JsonPrimitiveValue)jsonProp.Value).Value;
                }
            }

            return metadataPropValue;
        }

        private bool ContainsDeferredLink(JsonObject jsonObject)
        {
            if (jsonObject.Properties.Any(p => p.Name == DeferredFieldName))
            {
                return true;
            }

            foreach (JsonProperty prop in jsonObject.Properties.Where(p => p.Value.JsonType == JsonValueType.JsonObject))
            {
                if (prop.Value.JsonType == JsonValueType.JsonObject)
                {
                    if (this.ContainsDeferredLink(prop.Value as JsonObject))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private string GetInlineCount(JsonObject jsonObject)
        {
            string countField = null;
            var countProp = jsonObject.Properties.SingleOrDefault(prop => prop.Name == CountFieldName);
            if (countProp != null)
            {
                countField = ((JsonPrimitiveValue)countProp.Value).Value as string;
            }

            return countField;
        }

        private string GetNextLink(JsonObject jsonObject)
        {
            string nextLink = null;
            var nextLinkProp = jsonObject.Properties.Where(prop => prop.Name == NextLinkFieldName).SingleOrDefault();
            if (nextLinkProp != null)
            {
                nextLink = ((JsonPrimitiveValue)nextLinkProp.Value).Value as string;
            }

            return nextLink;
        }

        private bool TryGetServiceOperationDescriptorAnnotations<T>(JsonObject jsonObject, string collectionName, out List<T> serviceOperationPayloadMetadataAnnotations) where T : ServiceOperationDescriptor, new ()
        {
            serviceOperationPayloadMetadataAnnotations = null;

            // if this object has exactly one property with key "uri"
            // Example:
            // {
            //     "actions||functions": {
            //          "http://metadata": {
            //              [
            //                  {
            //                      "title" : "mytitle",
            //                      "target" : "mytitle",
            //                  }
            //                  <one or more>
            //              ]
            //           } 
            //           <one or more>
            //      }
            // }, 
            JsonProperty metadataProp = jsonObject.Properties.SingleOrDefault(p => p.Name == MetadataFieldName);
            if (metadataProp != null)
            {
                serviceOperationPayloadMetadataAnnotations = new List<T>();

                var metadataObject = metadataProp.Value as JsonObject;
                ExceptionUtilities.CheckObjectNotNull(metadataObject, "Expected to have a metadata JsonObject");

                var collectionPropertyValue = metadataObject.Properties.SingleOrDefault(p => p.Name == collectionName);
                if (collectionPropertyValue != null)
                {
                    var serviceOperationPayloadObject = collectionPropertyValue.Value as JsonObject;
                    ExceptionUtilities.CheckObjectNotNull(serviceOperationPayloadObject, "expected to have an {0} that is convertable to a JsonObject", collectionName);

                    foreach (var metadataProperty in serviceOperationPayloadObject.Properties)
                    {
                        var metadataArray = metadataProperty.Value as JsonArray;
                        ExceptionUtilities.CheckObjectNotNull(metadataArray, "expected to have an {0} that is convertable to a JsonArray", collectionName);

                        foreach (var arrayElement in metadataArray.Elements)
                        {
                            var elementObject = arrayElement as JsonObject;
                            ExceptionUtilities.CheckObjectNotNull(elementObject, "Cannot convert an element in a relation's array from JsonValue to JsonObject");

                            var metadataElementAnnotation = new T();

                            if (collectionName == "actions")
                            {
                                metadataElementAnnotation.IsAction = true;
                            }

                            metadataElementAnnotation.Metadata = metadataProperty.Name;

                            var titleProperty = elementObject.Properties.Where(p => p.Name == "title").SingleOrDefault();
                            if (titleProperty != null)
                            {
                                metadataElementAnnotation.Title = ((JsonPrimitiveValue)titleProperty.Value).Value as string;
                            }

                            var targetProperty = elementObject.Properties.Where(p => p.Name == "target").SingleOrDefault();
                            if (targetProperty != null)
                            {
                                metadataElementAnnotation.Target = ((JsonPrimitiveValue)targetProperty.Value).Value as string;
                            }

                            serviceOperationPayloadMetadataAnnotations.Add(metadataElementAnnotation);
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private bool TryGetLink(JsonObject jsonObject, out ODataPayloadElement elem)
        {
            elem = null;

            // if this object has exactly one property with key "uri"
            // Example:
            // {
            //     "uri": "http://services.odata.org/OData/OData.svc/Products(0)"
            // }, 
            // return DeferredLink
            if (jsonObject.Properties.Count() == 1 && jsonObject.Properties.Single().Name == UriFieldName)
            {
                elem = new DeferredLink()
                {
                    UriString = ((JsonPrimitiveValue)jsonObject.Properties.Single().Value).Value as string,
                };

                return true;
            }

            return false;
        }

        private bool TryGetDeferredLink(JsonObject jsonObject, out ODataPayloadElement elem)
        {
            elem = null;
            bool gotDeferredLink = false;

            // if this object has exactly one property with key "__deferred"
            // Example:
            // {
            //     "__deferred": {
            //     "uri": "http://services.odata.org/OData/OData.svc/Products(0)/Category"
            //     }
            // }
            if (jsonObject.Properties.Count() == 1 && jsonObject.Properties.Single().Name == DeferredFieldName)
            {
                JsonObject deferredObject = jsonObject.Properties.Single().Value as JsonObject;
                ExceptionUtilities.Assert(deferredObject != null, "Property \"_deferred\" should have an object value.");
                ExceptionUtilities.Assert(deferredObject.Properties.Count() == 1, "Property \"_deferred\" should have only one value.");
                ExceptionUtilities.Assert(deferredObject.Properties.Any(p => p.Name == UriFieldName), "Property \"_deferred\" should have only uri value.");
                gotDeferredLink = this.TryGetLink(deferredObject, out elem);
            }

            return gotDeferredLink;
        }

        private bool TryGetCollectionType(JsonObject jsonObject, out ODataPayloadElement elem)
        {
            string typeName = this.GetMetadataPropertyValue(jsonObject, TypeFieldName);
            elem = null;

            // if this object has a property with.Name "results" and it is of type "JsonArray"
            // Example: 
            //    "results": [
            //      { ... },
            //      { ... }
            //     ]
            // if it is EntitySet or ComplexCollection
            // EntitySets have "_Count" and "_Next". So process it before retrning.
            if (jsonObject.Properties.Any(prop => prop.Name == ResultsFieldName)
                && jsonObject.Properties.Where(prop => prop.Name == ResultsFieldName).Single().Value.JsonType == JsonValueType.JsonArray)
            {
                string nextLink = this.GetNextLink(jsonObject);
                string countField = this.GetInlineCount(jsonObject);               
       
                ODataPayloadElementCollection coll = this.ConvertArray((JsonArray)jsonObject.Properties.Where(p => p.Name == ResultsFieldName).Single().Value, true);
                
                if (coll.ElementType == ODataPayloadElementType.EntitySetInstance)
                {
                    EntitySetInstance entitySet = coll as EntitySetInstance;
                    entitySet.NextLink = nextLink;
                    if (countField == null)
                    {
                        entitySet.InlineCount = null;
                    }
                    else
                    {
                        entitySet.InlineCount = Convert.ToInt64(countField, CultureInfo.InvariantCulture);
                    }

                    elem = coll;
                }
                else if (coll.ElementType == ODataPayloadElementType.LinkCollection)
                {
                    LinkCollection linkCollection = (LinkCollection)coll;
                    linkCollection.InlineCount = Convert.ToInt64(countField, CultureInfo.InvariantCulture);
                    linkCollection.NextLink = nextLink;
                    elem = coll;
                }
                else if (coll.ElementType == ODataPayloadElementType.PrimitiveCollection)
                {
                    if (typeName != null)
                    {
                        PrimitiveMultiValue primitiveMultiValue = new PrimitiveMultiValue(typeName, false, ((PrimitiveCollection)coll).ToArray());
                        elem = primitiveMultiValue;
                    }
                    else
                    {
                        elem = coll;
                    }
                }
                else if (coll.ElementType == ODataPayloadElementType.ComplexInstanceCollection)
                {
                    if (typeName != null)
                    {
                        ComplexMultiValue complexMultiValue = new ComplexMultiValue(typeName, false, ((ComplexInstanceCollection)coll).ToArray());
                        elem = complexMultiValue;
                    }
                    else
                    {
                        elem = coll;
                    }
                }
                else
                {
                    ExceptionUtilities.Assert(coll.ElementType == ODataPayloadElementType.EmptyUntypedCollection, "Collection type unhandled: {0}", coll.ElementType);
                    string elementTypeName;
                    if (ODataUtilities.TryGetMultiValueElementTypeName(typeName, out elementTypeName))
                    {
                        ExceptionUtilities.CheckObjectNotNull(this.PrimitiveDataTypeConverter, "Cannot infer clr type from edm type without converter");

                        if (this.PrimitiveDataTypeConverter.ToClrType(elementTypeName) != null)
                        {
                            elem = new PrimitiveMultiValue(typeName, false);
                        }
                        else
                        {
                            elem = new ComplexMultiValue(typeName, false);
                        }
                    }
                    else
                    {
                        EmptyUntypedCollection emptyCollection = (EmptyUntypedCollection)coll;
                        if (countField != null)
                        {
                            emptyCollection.InlineCount = long.Parse(countField, CultureInfo.InvariantCulture);
                        }

                        if (nextLink != null)
                        {
                            emptyCollection.NextLink = nextLink;
                        }

                        elem = emptyCollection;
                    }
                }

                return true;
            }

            return false;
        }

        private bool TryGetEntityInstance(JsonObject jsonObject, out ODataPayloadElement elem)
        {
            string uri = this.GetMetadataPropertyValue(jsonObject, UriFieldName);
            string id = this.GetMetadataPropertyValue(jsonObject, IdFieldName);
            string typeName = this.GetMetadataPropertyValue(jsonObject, TypeFieldName);
            string etag = this.GetMetadataPropertyValue(jsonObject, ETagFieldName);
            string streamSrc = this.GetMetadataPropertyValue(jsonObject, MediaSrcFieldName);
            string streamEdit = this.GetMetadataPropertyValue(jsonObject, EditMediaFieldName);
            string streamEtag = this.GetMetadataPropertyValue(jsonObject, MediaETagFieldName);
            string contentType = this.GetMetadataPropertyValue(jsonObject, ContentTypeFieldName);

            List<ServiceOperationDescriptor> actions = null;
            bool containsActionsMetadata = this.TryGetServiceOperationDescriptorAnnotations(jsonObject, "actions", out actions);

            List<ServiceOperationDescriptor> functions = null;
            bool containsFunctionMetadata = this.TryGetServiceOperationDescriptorAnnotations(jsonObject, "functions", out functions);

            // if this object has any property with key "__deferred"
            // or metadata tag returns a "uri", "id", or "etag" property,
            // or actions, or functions
            // return EntityInstance
            // else return ComplexInstance
            bool isEntity = this.ContainsDeferredLink(jsonObject);
            isEntity |= uri != null;
            isEntity |= id != null;
            isEntity |= etag != null;
            isEntity |= containsActionsMetadata;
            isEntity |= containsFunctionMetadata;

            if (isEntity)
            {
                EntityInstance entity = new EntityInstance(typeName, false, id, etag);

                if (containsActionsMetadata)
                {
                    foreach (var action in actions)
                    {
                        entity.ServiceOperationDescriptors.Add(action);
                    }
                }

                if (containsFunctionMetadata)
                {
                    foreach (var function in functions)
                    {
                        entity.ServiceOperationDescriptors.Add(function);
                    }
                }

                entity.EditLink = uri;
                entity.StreamSourceLink = streamSrc;
                entity.StreamEditLink = streamEdit;
                entity.StreamETag = streamEtag;
                entity.StreamContentType = contentType;

                foreach (JsonProperty prop in jsonObject.Properties)
                {
                    if (prop.Name != MetadataFieldName && prop.Name != LinkInfoFieldName)
                    {
                        entity.Add(this.ConvertProperty(prop) as PropertyInstance);
                    }
                }

                // Update entity navigation properties with association links
                JsonObject metaDataJsonObject = (JsonObject)jsonObject.Properties.Single(p1 => p1.Name == MetadataFieldName).Value;
                JsonProperty properties = metaDataJsonObject.Properties.FirstOrDefault(p2 => p2.Name == PropertiesFieldName);
                
                if (properties != null)
                {
                    foreach (JsonProperty jp in ((JsonObject)properties.Value).Properties)
                    {
                        JsonProperty associationUriJsonProperty = ((JsonObject)jp.Value).Properties.SingleOrDefault(p => p.Name == AssociationUriFieldName);
                        DeferredLink newAssociationLink = new DeferredLink();
                        if (associationUriJsonProperty != null)
                        {
                            newAssociationLink.UriString = ((JsonPrimitiveValue)associationUriJsonProperty.Value).Value as string;
                        }

                        var navigation = entity.Properties.SingleOrDefault(np => np.Name.Equals(jp.Name));

                        NavigationPropertyInstance navigationPropertyInstance = navigation as NavigationPropertyInstance;
                        if (navigationPropertyInstance != null)
                        {
                            navigationPropertyInstance.AssociationLink = newAssociationLink;
                        }
                        else if (navigation is EmptyCollectionProperty)
                        {
                            ExpandedLink newExpandedLink = new ExpandedLink() { ExpandedElement = new EntitySetInstance() };
                            NavigationPropertyInstance newNavigation = new NavigationPropertyInstance(navigation.Name, newExpandedLink, newAssociationLink);
                            entity.Replace(navigation, newNavigation);
                        }
                        else
                        {
                            ExceptionUtilities.Assert(navigation.ElementType == ODataPayloadElementType.NullPropertyInstance, "Invalid type of PropertyInstance : {0}", navigation.ElementType);
                            ExpandedLink newExpandedLink = new ExpandedLink() { ExpandedElement = new EntityInstance() };
                            NavigationPropertyInstance newNavigation = new NavigationPropertyInstance(navigation.Name, newExpandedLink, newAssociationLink);
                            entity.Replace(navigation, newNavigation);
                        }    
                    }
                }

                elem = entity;
                return true;
            }

            elem = null;
            return false;
        }

        private bool TryGetComplexInstance(JsonObject jsonObject, out ODataPayloadElement elem)
        {
            string typeName = this.GetMetadataPropertyValue(jsonObject, TypeFieldName);
            ComplexInstance complex = new ComplexInstance(typeName, false);
            elem = null;

            foreach (JsonProperty prop in jsonObject.Properties)
            {
                // Just parse the object properties. Skip properties like "__metadata", "Count" and "Next"
                if (prop.Name != MetadataFieldName)
                {
                    complex.Add(this.ConvertProperty(prop) as PropertyInstance);
                }
            }

            elem = complex;
            return elem != null;
        }

        private bool TryGetNamedStream(JsonObject jsonObject, out ODataPayloadElement elem)
        {
            if (jsonObject.Properties.Count() == 1 && jsonObject.Properties.Single().Name == MediaResourceFieldName)
            {
                JsonProperty namedStreamProperty = jsonObject.Properties.Single();

                NamedStreamInstance nsi = new NamedStreamInstance(namedStreamProperty.Name);

                JsonObject streamJsonObject = (JsonObject)namedStreamProperty.Value;
                var editLink = streamJsonObject.Properties.SingleOrDefault(a => a.Name.Equals(EditMediaFieldName));
                if (editLink != null)
                {
                    nsi.EditLink = (string)((JsonPrimitiveValue)editLink.Value).Value;
                }

                var sourceLink = streamJsonObject.Properties.SingleOrDefault(a => a.Name.Equals(MediaSrcFieldName));
                if (sourceLink != null)
                {
                    nsi.SourceLink = (string)((JsonPrimitiveValue)sourceLink.Value).Value;
                }

                var etag = streamJsonObject.Properties.SingleOrDefault(a => a.Name.Equals(MediaETagFieldName));
                if (etag != null)
                {
                    nsi.ETag = (string)((JsonPrimitiveValue)etag.Value).Value;
                }

                var contentType = streamJsonObject.Properties.SingleOrDefault(a => a.Name.Equals(ContentTypeFieldName));
                if (contentType != null)
                {
                    nsi.SourceLinkContentType = nsi.EditLinkContentType = (string)((JsonPrimitiveValue)contentType.Value).Value;
                }

                elem = nsi;
                return true;
            }

            elem = null;
            return false;
        }
        
        private bool TryGetSecurityPropertyValue(JsonObject jsonObject, out JsonValue value)
        {
            value = null;

            // if this object has exactly one property with key "d"
            // Example:
            // {
            //  "d" : {
            //      "__metadata" : { ... },
            //      "ID" : 0,
            //      "Name" : "Alice"
            // }
            if (jsonObject.Properties.Select(p => p.Name).SequenceEqual(new[] { SecurityTag }))
            {
                value = jsonObject.Properties.Single().Value;
                return true;
            }

            return false;
        }
    }
}
