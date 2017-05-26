//---------------------------------------------------------------------
// <copyright file="AnnotatedPayloadElementToJsonLightConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.Spatial;
    using System.Text;
    using System.Xml;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.Contracts.JsonLight;
    using Microsoft.Test.Taupo.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// The converter from ODataPayloadElement representation (with possible annotations) to JSON Light.
    /// </summary>
    [ImplementationName(typeof(IPayloadElementToJsonLightConverter), "AnnotatedPayloadElementToJsonLightConverter", HelpText = "Payload to JSON Light converter which allows JSON specific annotations to be used.")]
    public class AnnotatedPayloadElementToJsonLightConverter : IPayloadElementToJsonLightConverter
    {
        // The goal for the payload converters is that they are very simple. No clever things in here.
        // They should not even know about versions and such up front. Everything should be guided by the input (ODataPayloadElement).
        // Any specific changes to the serialization should be done through annotations.

        /// <summary>
        /// Gets or sets the payload transform factory
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IPayloadTransformFactory TransformFactory { get; set; }

        /// <summary>
        /// Gets or sets the spatial primitive to odata json converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public SpatialPrimitiveToODataJsonLightValueConverter SpatialConverter { get; set; }

        /// <summary>
        /// Converts the given payload element into a Json Light representation.
        /// </summary>
        /// <param name="rootElement">The root payload element to convert.</param>        
        /// <returns>The Json Light representation of the payload.</returns>
        public string ConvertToJsonLight(ODataPayloadElement rootElement)
        {
            return ConvertToJsonLight(rootElement, /*newline*/null);
        }

        /// <summary>
        /// Converts the given payload element into a Json Light representation.
        /// </summary>
        /// <param name="rootElement">The root payload element to convert.</param>   
        /// <param name="newline">Newline character(s) to use when writing the returned string.
        /// A null strings results in the use of the default.</param>
        /// <returns>The Json Light representation of the payload.</returns>
        public string ConvertToJsonLight(ODataPayloadElement rootElement, string newline)
        {
            JsonValue jsonValue = this.ConvertToJsonLightValue(rootElement);

            StringBuilder builder = new StringBuilder();
            using (StringWriter strWriter = new StringWriter(builder, CultureInfo.CurrentCulture))
            {
                if (newline != null)
                {
                    strWriter.NewLine = newline;
                }

                JsonTextPreservingWriter jsonWriter = new JsonTextPreservingWriter(strWriter, writingJsonLight: true);
                jsonWriter.WriteValue(jsonValue);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts the given payload element into a Json Light representation.
        /// </summary>
        /// <param name="rootElement">The root payload element to convert.</param>        
        /// <returns>The Json Light representation of the payload.</returns>
        public JsonValue ConvertToJsonLightValue(ODataPayloadElement rootElement)
        {
            JsonValue jsonValue = new SerializeODataPayloadElementVisitor(this).Serialize(rootElement);

            if (this.TransformFactory != null)
            {
                IPayloadTransform<JsonValue> jsonTransform = this.TransformFactory.GetTransform<JsonValue>();

                JsonValue transformedJsonPayload = null;
                if (jsonTransform.TryTransform(jsonValue, out transformedJsonPayload))
                {
                    jsonValue = transformedJsonPayload;
                }
            }

            return jsonValue;
        }
        
        /// <summary>
        /// Visitor for serializing an ODataPayloadElement to JSON Light.
        /// </summary>
        private sealed class SerializeODataPayloadElementVisitor : IODataPayloadElementVisitor<JsonValue>
        {
            private Stack<ODataPayloadElement> payloadStack = new Stack<ODataPayloadElement>();
            private readonly AnnotatedPayloadElementToJsonLightConverter parent;
            private JsonValue parentElementValue;

            /// <summary>
            /// Initializes a new instance of the SerializeODataPayloadElementVisitor class
            /// </summary>
            internal SerializeODataPayloadElementVisitor(AnnotatedPayloadElementToJsonLightConverter parent)
            {
                ExceptionUtilities.CheckArgumentNotNull(parent, "parent");
                this.parent = parent;
                this.parentElementValue = null;
            }

            /// <summary>
            /// Serialize the current payload element
            /// </summary>
            /// <param name="rootElement">Element to be converted</param>            
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Serialize(ODataPayloadElement rootElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");

                return this.Recurse(rootElement);
            }

            /// <summary>
            /// This method should never be called.
            /// </summary>
            /// <param name="payloadElement">Batch Payload</param>
            /// <returns>Throws TaupoNotSupportedException</returns>
            public JsonValue Visit(BatchRequestPayload payloadElement)
            {
                throw new TaupoNotSupportedException("A batch payload cannot be converted to Json Light.");
            }

            /// <summary>
            /// This method should never be called.
            /// </summary>
            /// <param name="payloadElement">Batch Payload</param>
            /// <returns>Throws TaupoNotSupportedException</returns>
            public JsonValue Visit(BatchResponsePayload payloadElement)
            {
                throw new TaupoNotSupportedException("A batch payload cannot be converted to Json Light.");
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexMultiValueProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ComplexMultiValueProperty payloadElement)
            {
                // This is the MultiValue property of complex types.
                bool needsWrapping = this.CurrentElementIsRoot();
                string propertyName = needsWrapping ? JsonLightConstants.ODataValuePropertyName : payloadElement.Name;

                var jsonProperty = new JsonProperty(propertyName, null);
                jsonProperty.Value = this.Recurse(payloadElement.Value, jsonProperty);

                var jsonValue = this.WrapTopLevelProperty(payloadElement, jsonProperty, needsWrapping);
                this.AddPropertyAnnotations(payloadElement, jsonValue);
                return jsonValue;
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload the element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ComplexInstance payloadElement)
            {
                // TODO: ODataLib test item: Add new ODataPayloadElement for parameters payload
                //ExceptionUtilities.Assert(!this.isRootElement, "Json serialization does not allow a root element of type: " + payloadElement.ElementType.ToString());

                JsonObject complexValue = new JsonObject();

                if (this.CurrentElementIsRoot())
                {
                    AddContextUriProperty(payloadElement, complexValue);
                }

                if (payloadElement.FullTypeName != null)
                {
                    complexValue.Add(this.CreateTypeName(payloadElement.FullTypeName));
                }

                foreach (var property in payloadElement.Properties)
                {
                    complexValue.Add((JsonProperty)this.Recurse(property, complexValue));
                }

                return complexValue;
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexInstanceCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ComplexInstanceCollection payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                ExceptionUtilities.Assert(this.CurrentElementIsRoot(), "Complex collections should only appear at the top level.");

                JsonArray complexCollectionValue = new JsonArray();
                foreach (var item in payloadElement)
                {
                    complexCollectionValue.Add(this.Recurse(item, complexCollectionValue));
                }

                return this.WrapTopLevelProperty(payloadElement, new JsonProperty(JsonLightConstants.ODataValuePropertyName, complexCollectionValue), /*needsWrapping*/true);
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexMultiValue.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ComplexMultiValue payloadElement)
            {
                // This is a MultiValue of complex types.
                JsonArray resultsValue = new JsonArray();
                foreach (var item in payloadElement)
                {
                    resultsValue.Add(this.Recurse(item, resultsValue));
                }

                return resultsValue;
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ComplexProperty payloadElement)
            {
                bool isTopLevel = this.CurrentElementIsRoot();

                if (isTopLevel)
                {
                    if (payloadElement.Value.IsNull)
                    {
                        // TODO: Change the payload of null top-level properties #645
                    }

                    JsonValue complexValue = this.Recurse(payloadElement.Value);

                    // NOTE: top-level complex properties don't use a wrapper object.
                    //       Inject a context URI if the value is an object (and not an invalid other kind)
                    JsonObject complexObject = complexValue as JsonObject;
                    if (complexObject != null)
                    {
                        InsertContextUriProperty(payloadElement, complexObject);
                        if (!payloadElement.Annotations.OfType<JsonLightMaintainPropertyOrderAnnotation>().Any())
                        {
                            this.ReorderPropertyAnnotations(complexObject);
                        }
                    }

                    return complexValue;
                }
                else
                {
                    var result = new JsonProperty(payloadElement.Name, null);
                    result.Value = this.Recurse(payloadElement.Value, result);
                    return this.WrapTopLevelProperty(payloadElement, result, isTopLevel);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a DeferredLink.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(DeferredLink payloadElement)
            {
                // In responses we produce an entity reference link at the top-level or inside a collection of entity reference links
                // In requests we produce bind operation (propName@odata.bind property annotation)

                if (this.CurrentElementIsRoot())
                {
                    // Create an entity reference link
                    return this.WrapTopLevelProperty(payloadElement, new JsonProperty(JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName, new JsonPrimitiveValue(payloadElement.UriString)), /*needsWrapping*/true);
                }

                bool isResponse = IsResponse(payloadElement);
                ODataPayloadElement parent = this.payloadStack.ElementAt(1);

                // NOTE: a deferred singleton link for a nav prop (request or response) is already processed when creating the nav prop itself.
                ExceptionUtilities.Assert(
                    parent.ElementType == ODataPayloadElementType.LinkCollection, 
                    "Non top-level deferred links should only appear in link collections here.");

                // Entity reference link in a collection of entity reference links.
                if (!isResponse)
                {
                    // Just write the URI itself. The array for it has been already written.
                    return new JsonPrimitiveValue(payloadElement.UriString);
                }

                // In a response, write an object with the link. The array for it has already been written.
                return new JsonObject()
                {
                    new JsonProperty(JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName, new JsonPrimitiveValue(payloadElement.UriString))
                };
            }

            /// <summary>
            /// Visits a payload element whose root is an EmptyCollectionProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(EmptyCollectionProperty payloadElement)
            {
                bool needsWrapping = this.CurrentElementIsRoot();

                // The JSON deserializer uses this if it find and empty array.
                // The XML deserializer will never use it.
                // So basically this is a non-metadata version of an empty entity set.

                string propertyName = needsWrapping ? JsonLightConstants.ODataValuePropertyName : payloadElement.Name;
                var result = new JsonProperty(propertyName, null);
                result.Value = this.Recurse(payloadElement.Value, result);
                return this.WrapTopLevelProperty(payloadElement, result, needsWrapping);
            }

            /// <summary>
            /// Visits a payload element whose root is an EmptyPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(EmptyPayload payloadElement)
            {
                throw new TaupoNotSupportedException("Json Light serialization does not allow element of type: " + payloadElement.ElementType.ToString());
            }

            /// <summary>
            /// Visits a payload element whose root is an EmptyUntypedCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(EmptyUntypedCollection payloadElement)
            {
                throw new TaupoNotSupportedException("We always need metadata for a top-level collection.");
            }

            /// <summary>
            /// Visits a payload element whose root is an EntityInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(EntityInstance payloadElement)
            {
                JsonObject entityValue = new JsonObject();

                if (this.CurrentElementIsRoot())
                {
                    AddContextUriProperty(payloadElement, entityValue);
                }

                AddMetadataProperties(payloadElement, entityValue);

                foreach (var property in payloadElement.Properties)
                {
                    // Special handling of named streams as they add multiple properties to the entry
                    // and thus the single JsonValue return type of the Visit method does not work.
                    NamedStreamInstance namedStreamProperty = property as NamedStreamInstance;
                    if (namedStreamProperty != null)
                    {
                        this.AddNamedStreamProperty(namedStreamProperty, entityValue);
                    }
                    else
                    {
                        NavigationPropertyInstance navigationProperty = property as NavigationPropertyInstance;
                        if (navigationProperty != null)
                        {
                            this.AddNavigationProperty(navigationProperty, entityValue);
                        }
                        else
                        {
                            JsonProperty jsonProperty = (JsonProperty)this.Recurse(property, entityValue);
                            if (jsonProperty != null)
                            {
                                entityValue.Add(jsonProperty);
                            }
                        }
                    }
                }

                if (!payloadElement.Annotations.OfType<JsonLightMaintainPropertyOrderAnnotation>().Any())
                {
                    this.ReorderPropertyAnnotations(entityValue);
                }

                return entityValue;
            }

            /// <summary>
            /// Visits a payload element whose root is an EntitySetInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(EntitySetInstance payloadElement)
            {
                JsonArray entitySetValue = new JsonArray();
                for (int i = 0; i < payloadElement.Count(); i++)
                {
                    entitySetValue.Add(this.Recurse(payloadElement[i], entitySetValue));
                }

                if (this.CurrentElementIsRoot())
                {
                    JsonObject wrapper = new JsonObject();
                    AddContextUriProperty(payloadElement, wrapper);

                    if (payloadElement.InlineCount.HasValue)
                    {
                        wrapper.Add(new JsonProperty(JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataCountAnnotationName, new JsonPrimitiveValue(payloadElement.InlineCount.Value)));
                    }

                    wrapper.Add(new JsonProperty(JsonLightConstants.ODataValuePropertyName, entitySetValue));

                    AddPropertyIfNotNull(wrapper, JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNextLinkAnnotationName, payloadElement.NextLink);
                    return wrapper;
                }

                // For nested feeds (expanded nav properties), we only return the feed value (i.e., the array) here.
                // The inline count, next link, URLs, etc. are written when we process the parent navigation property.
                return entitySetValue;
            }

            /// <summary>
            /// Visits a payload element whose root is an ExpandedLink.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ExpandedLink payloadElement)
            {
                throw new TaupoNotSupportedException("The expanded link should have already been processed when creating the navigation property.");
            }

            /// <summary>
            /// Visits a payload element whose root is a LinkCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(LinkCollection payloadElement)
            {
                // In requests, this is a set of binding operations for a parent nav prop.
                // In responses, this represents a $ref collection response (must be top-level)
                JsonArray linkCollectionValue = new JsonArray();
                foreach (Link link in payloadElement)
                {
                    linkCollectionValue.Add(this.Recurse(link, linkCollectionValue));
                }

                if (!IsResponse(payloadElement))
                {
                    return linkCollectionValue;
                }

                ExceptionUtilities.Assert(this.CurrentElementIsRoot(), "Link collections in responses should only appear at the top level.");
                JsonObject wrapper = new JsonObject();
                AddContextUriProperty(payloadElement, wrapper);

                if (payloadElement.InlineCount.HasValue)
                {
                    wrapper.Add(new JsonProperty(JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataCountAnnotationName, new JsonPrimitiveValue(payloadElement.InlineCount.Value)));
                }

                wrapper.Add(new JsonProperty(JsonLightConstants.ODataValuePropertyName, linkCollectionValue));
                AddPropertyIfNotNull(wrapper, JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNextLinkAnnotationName, payloadElement.NextLink);
                return wrapper;
            }

            /// <summary>
            /// Visits a payload element whose root is a NamedStreamProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(NamedStreamInstance payloadElement)
            {
                throw new TaupoNotSupportedException("Named streams should have been processed when writing the entit instance.");
            }

            /// <summary>
            /// Visits a payload element whose root is a NavigationProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(NavigationPropertyInstance payloadElement)
            {
                throw new TaupoNotSupportedException("Navigation properties should have been processed when writing the entity instance.");
            }

            /// <summary>
            /// Visits a payload element whose root is a NullPropertyInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(NullPropertyInstance payloadElement)
            {
                bool needsWrapping = this.CurrentElementIsRoot();
                string propertyName = needsWrapping ? JsonLightConstants.ODataValuePropertyName : payloadElement.Name;
                JsonValue result = new JsonProperty(propertyName, new JsonPrimitiveValue(null));
                return this.WrapTopLevelProperty(payloadElement, result, needsWrapping);
            }

            /// <summary>
            /// Visits a payload element whose root is an ODataErrorPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public JsonValue Visit(ODataErrorPayload payloadElement)
            {
                ExceptionUtilities.Assert(this.CurrentElementIsRoot(), "Json Light serialization requires an element of type {0} to be at the root level.", payloadElement.ElementType.ToString());

                // the error code
                JsonObject errorObject = new JsonObject();
                AddPropertyIfNotNull(errorObject, JsonConstants.ODataErrorCodeName, payloadElement.Code);

                // the message
                AddPropertyIfNotNull(errorObject, JsonConstants.ODataErrorMessageName, payloadElement.Message);

                if (payloadElement.InnerError != null)
                {
                    JsonValue innerErrorValue = this.Recurse(payloadElement.InnerError, errorObject);
                    errorObject.Add(new JsonProperty(JsonConstants.ODataErrorInnerErrorName, innerErrorValue));
                }

                return new JsonObject()
                {
                    new JsonProperty(JsonLightConstants.ODataErrorPropertyName, errorObject)
                };
            }

            /// <summary>
            /// Visits a payload element whose root is an ODataInternalExceptionPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public JsonValue Visit(ODataInternalExceptionPayload payloadElement)
            {
                ExceptionUtilities.Assert(!this.CurrentElementIsRoot(), "Json Lite serialization does not support an element of type {0} at the root level.", payloadElement.ElementType.ToString());

                JsonObject innerErrorObject = new JsonObject();

                // the inner error message
                AddPropertyIfNotNull(innerErrorObject, JsonConstants.ODataErrorInnerErrorMessageName, payloadElement.Message);

                // the inner error type name
                AddPropertyIfNotNull(innerErrorObject, JsonConstants.ODataErrorInnerErrorTypeNameName, payloadElement.TypeName);

                // the inner stack trace
                AddPropertyIfNotNull(innerErrorObject, JsonConstants.ODataErrorInnerErrorStackTraceName, payloadElement.StackTrace);

                // the nested inner error
                if (payloadElement.InternalException != null)
                {
                    JsonValue nestedInnerError = this.Recurse(payloadElement.InternalException, innerErrorObject);
                    innerErrorObject.Add(new JsonProperty(JsonConstants.ODataErrorInnerErrorInnerErrorName, nestedInnerError));
                }

                return innerErrorObject;
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(PrimitiveCollection payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                JsonArray collectionValue = new JsonArray();
                foreach (var item in payloadElement)
                {
                    collectionValue.Add(this.Recurse(item, collectionValue));
                }

                JsonObject wrapper = new JsonObject();
                AddContextUriProperty(payloadElement, wrapper);
                wrapper.Add(new JsonProperty(JsonLightConstants.ODataValuePropertyName, collectionValue));
                return wrapper;
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveMultiValue.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(PrimitiveMultiValue payloadElement)
            {
                // This is the value of the MultiValue of primitive values.
                JsonArray primitiveMultiValueItems = new JsonArray();
                foreach (var item in payloadElement)
                {
                    primitiveMultiValueItems.Add(this.Recurse(item, primitiveMultiValueItems));
                }

                return primitiveMultiValueItems;
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveMultiValueProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(PrimitiveMultiValueProperty payloadElement)
            {
                // This is the property with value MultiValue of primitive values.
                bool needsWrapping = this.CurrentElementIsRoot();
                string propertyName = needsWrapping ? JsonLightConstants.ODataValuePropertyName : payloadElement.Name;

                var jsonProperty = new JsonProperty(propertyName, null);
                jsonProperty.Value = this.Recurse(payloadElement.Value, jsonProperty);

                var jsonValue = this.WrapTopLevelProperty(payloadElement, jsonProperty, needsWrapping);
                this.AddPropertyAnnotations(payloadElement, jsonValue);
                return jsonValue;
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(PrimitiveProperty payloadElement)
            {
                bool needsWrapping = this.CurrentElementIsRoot();

                string propertyName = needsWrapping ? JsonLightConstants.ODataValuePropertyName : payloadElement.Name;
                // TODO: Change the payload of null top-level properties #645
                JsonProperty jsonProperty = new JsonProperty(propertyName, null);
                jsonProperty.Value = this.Recurse(payloadElement.Value, jsonProperty);

                var jsonValue = this.WrapTopLevelProperty(payloadElement, jsonProperty, needsWrapping);
                this.AddPropertyAnnotations(payloadElement, jsonValue);
                return jsonValue;
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveValue.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(PrimitiveValue payloadElement)
            {
                JsonObject converted;
                if (this.parent.SpatialConverter.TryConvertIfSpatial(payloadElement, out converted))
                {
                    return converted;
                }

                object clrValue = payloadElement.ClrValue;
                if (clrValue is DateTime)
                {
                    return new JsonPrimitiveValue(XmlConvert.ToString((DateTime)clrValue, XmlDateTimeSerializationMode.RoundtripKind));
                }
                else if (clrValue is DateTimeOffset)
                {
                    return new JsonPrimitiveValue(XmlConvert.ToString((DateTimeOffset)clrValue));
                }

                return new JsonPrimitiveValue(payloadElement.ClrValue);
            }

            /// <summary>
            /// Visits a payload element whose root is a ServiceDocumentInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ServiceDocumentInstance payloadElement)
            {
                ExceptionUtilities.Assert(this.CurrentElementIsRoot(), "Json serialization allows an element of type '{0}' only at the root level.", payloadElement.ElementType.ToString());

                // NOTE: ODataLib currently only supports a single workspace in a service document
                ExceptionUtilities.Assert(payloadElement.Workspaces.Count == 1, "Expected exactly one workspace in a service document.");

                JsonObject serviceDocObject = new JsonObject();
                AddContextUriProperty(payloadElement, serviceDocObject);

                JsonValue workspaceArray = this.Recurse(payloadElement.Workspaces[0], serviceDocObject);
                serviceDocObject.Add(new JsonProperty(JsonLightConstants.ODataValuePropertyName, workspaceArray));
                return serviceDocObject;
            }

            /// <summary>
            /// Serialize the current payload element
            /// </summary>
            /// <param name="serviceOperationDescriptor">Element to be converted</param>            
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ServiceOperationDescriptor serviceOperationDescriptor)
            {
                JsonObject operationValue = new JsonObject();
                AddPropertyIfNotNull(operationValue, JsonConstants.ODataOperationTitleName, serviceOperationDescriptor.Title);
                AddPropertyIfNotNull(operationValue, JsonConstants.ODataOperationTargetName, serviceOperationDescriptor.Target);
                return operationValue;
            }

            /// <summary>
            /// Visits a payload element whose root is a WorkspaceInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(WorkspaceInstance payloadElement)
            {
                ExceptionUtilities.Assert(!this.CurrentElementIsRoot(), "Json serialization does not allow a root element of type: {0}", payloadElement.ElementType.ToString());

                JsonArray collectionArray = new JsonArray();

                if (payloadElement.ResourceCollections != null)
                {
                    foreach (ResourceCollectionInstance collection in payloadElement.ResourceCollections)
                    {
                        collectionArray.Add(this.Recurse(collection, collectionArray));
                    }
                }

                return collectionArray;
            }

            /// <summary>
            /// Visits a payload element whose root is a ResourceCollectionInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ResourceCollectionInstance payloadElement)
            {
                ExceptionUtilities.Assert(!this.CurrentElementIsRoot(), "Json serialization does not allow a root element of type: {0}", payloadElement.ElementType.ToString());

                JsonObject collectionObject = new JsonObject();
                collectionObject.Add(new JsonProperty(JsonLightConstants.ODataWorkspaceCollectionNameName, new JsonPrimitiveValue(payloadElement.Name)));
                collectionObject.Add(new JsonProperty(JsonLightConstants.ODataWorkspaceCollectionUrlName, new JsonPrimitiveValue(payloadElement.Href)));
                return collectionObject;
            }

            /// <summary>
            /// Adds the context URI property to a JSON object if the payload element has the proper annotation.
            /// </summary>
            /// <param name="payloadElement">The payload element to get the context URI from (if present).</param>
            /// <param name="jsonObject">The JSON object to add the context URI property to.</param>
            private static void AddContextUriProperty(ODataPayloadElement payloadElement, JsonObject jsonObject)
            {
                ExceptionUtilities.Assert(payloadElement != null, "payloadElement != null");
                ExceptionUtilities.Assert(jsonObject != null, "jsonObject != null");

                string contextUri = payloadElement.ContextUri();
                AddPropertyIfNotNull(jsonObject, JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName, contextUri);
            }

            /// <summary>
            /// Adds the context URI property to a JSON object if the payload element has the proper annotation.
            /// </summary>
            /// <param name="payloadElement">The payload element to get the context URI from (if present).</param>
            /// <param name="jsonObject">The JSON object to add the context URI property to.</param>
            private static void InsertContextUriProperty(ODataPayloadElement payloadElement, JsonObject jsonObject)
            {
                ExceptionUtilities.Assert(payloadElement != null, "payloadElement != null");
                ExceptionUtilities.Assert(jsonObject != null, "jsonObject != null");

                string contextUri = payloadElement.ContextUri();
                if (contextUri != null)
                {
                    jsonObject.Insert(0, new JsonProperty(JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName, new JsonPrimitiveValue(contextUri)));
                }
            }

            /// <summary>
            /// Adds a new property with the specified type to the provided JSON object if the property value is not null.
            /// </summary>
            /// <param name="jsonObject">The JSON object to add the property to.</param>
            /// <param name="propertyName">The property name to use.</param>
            /// <param name="propertyValue">The property value.</param>
            private static void AddPropertyIfNotNull(JsonObject jsonObject, string propertyName, string propertyValue)
            {
                if (propertyValue != null)
                {
                    jsonObject.Add(new JsonProperty(propertyName, new JsonPrimitiveValue(propertyValue)));
                }
            }

            /// <summary>
            /// Creates the metadata properties for the specified entity.
            /// </summary>
            /// <param name="payloadElement">Entity payload element.</param>
            /// <param name="entityObject">The entity object to create the metadata properties for.</param>
            /// <returns>The metadata property.</returns>
            private void AddMetadataProperties(EntityInstance payloadElement, JsonObject entityObject)
            {
                AddPropertyIfNotNull(entityObject, JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName, payloadElement.FullTypeName);
                AddPropertyIfNotNull(entityObject, JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName, payloadElement.Id);
                AddPropertyIfNotNull(entityObject, JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataETagAnnotationName, payloadElement.ETag);

                string editLink = payloadElement.GetEditLink();
                AddPropertyIfNotNull(entityObject, JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataEditLinkAnnotationName, editLink);

                string readLink = payloadElement.GetSelfLink();
                AddPropertyIfNotNull(entityObject, JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName, readLink);

                // write all the default stream properties if the entry is an MLE
                if (payloadElement.IsMediaLinkEntry())
                {
                    AddPropertyIfNotNull(entityObject, JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaEditLinkAnnotationName, payloadElement.StreamEditLink);
                    AddPropertyIfNotNull(entityObject, JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaReadLinkAnnotationName, payloadElement.StreamSourceLink);
                    AddPropertyIfNotNull(entityObject, JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaContentTypeAnnotationName, payloadElement.StreamContentType);
                    AddPropertyIfNotNull(entityObject, JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaETagAnnotationName, payloadElement.StreamETag);
                }

                var actionDescriptors = payloadElement.ServiceOperationDescriptors.Where(s => s.IsAction).ToList();
                if (actionDescriptors.Count > 0)
                {
                    JsonProperty actionsProperty = this.CreateOperationsObject(JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataActionsAnnotationName, actionDescriptors);
                    entityObject.Add(actionsProperty);
                }

                var functionsDescriptors = payloadElement.ServiceOperationDescriptors.Where(s => s.IsFunction).ToList();
                if (functionsDescriptors.Count > 0)
                {
                    JsonProperty functionsProperty = this.CreateOperationsObject(JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataFunctionsAnnotationName, functionsDescriptors);
                    entityObject.Add(functionsProperty);
                }
            }

            /// <summary>
            /// Processes a named stream and adds all JSON properties related to the named stream into the current entity object.
            /// </summary>
            /// <param name="namedStreamInstance">The named stream to process.</param>
            /// <param name="entityObject">The entity object to add JSON properties to.</param>
            private void AddNamedStreamProperty(NamedStreamInstance namedStreamInstance, JsonObject entityObject)
            {
                ExceptionUtilities.Assert(namedStreamInstance != null, "namedStreamInstance != null");
                ExceptionUtilities.Assert(entityObject != null, "jsonObject != null");

                JsonValue result = GetJsonRepresentation(namedStreamInstance);
                if (result != null)
                {
                    entityObject.Add((JsonProperty)result);
                    return;
                }

                string propertyName = namedStreamInstance.Name;
                AddPropertyIfNotNull(entityObject, JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataMediaEditLinkAnnotationName), namedStreamInstance.EditLink);
                AddPropertyIfNotNull(entityObject, JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataMediaReadLinkAnnotationName), namedStreamInstance.SourceLink);

                // NOTE the named stream instance stores two content types; one for the read link and
                //      one for the edit link. If an edit link is present, the edit link's value will win.
                string contentTypeString = null;
                if (namedStreamInstance.EditLink != null && namedStreamInstance.EditLinkContentType != null)
                {
                    contentTypeString = namedStreamInstance.EditLinkContentType;
                }
                else if (namedStreamInstance.SourceLink != null && namedStreamInstance.SourceLinkContentType != null)
                {
                    contentTypeString = namedStreamInstance.SourceLinkContentType;
                }
                else
                {
                    // If no links are present or there are mismatches, use the edit link's content type, if not available use source link's content type.
                    contentTypeString = namedStreamInstance.EditLinkContentType ?? namedStreamInstance.SourceLinkContentType;
                }

                AddPropertyIfNotNull(entityObject, JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataMediaContentTypeAnnotationName), contentTypeString);
                AddPropertyIfNotNull(entityObject, JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataMediaETagAnnotationName), namedStreamInstance.ETag);
            }

            /// <summary>
            /// Processes a navigation property and adds all JSON properties related to the navigation property into the current entity object.
            /// </summary>
            /// <param name="navigationProperty">The navigation property to process.</param>
            /// <param name="entityObject">The entity object to add JSON properties to.</param>
            private void AddNavigationProperty(NavigationPropertyInstance navigationProperty, JsonObject entityObject)
            {
                ExceptionUtilities.Assert(navigationProperty != null, "navigationProperty != null");
                ExceptionUtilities.Assert(entityObject != null, "jsonObject != null");

                JsonValue result = GetJsonRepresentation(navigationProperty);
                if (result != null)
                {
                    entityObject.Add((JsonProperty)result);
                    return;
                }

                ODataPayloadElement propertyValue = navigationProperty.Value;
                if (propertyValue != null)
                {
                    result = GetJsonRepresentation(propertyValue);
                    if (result != null)
                    {
                        entityObject.Add(new JsonProperty(navigationProperty.Name, result));
                        return;
                    }
                }

                bool isRequest = !IsResponse(navigationProperty);
                LinkCollection linkCollection = propertyValue as LinkCollection;
                if (linkCollection != null)
                {
                    ExceptionUtilities.Assert(isRequest, "Link collection in a navigation property is only allowed in requests.");
                    this.AddNavigationLinkCollection(navigationProperty, linkCollection, entityObject);
                    return;
                }

                DeferredLink deferredLink = propertyValue as DeferredLink;
                ExpandedLink expandedLink = propertyValue as ExpandedLink;
                if (deferredLink != null && deferredLink.UriString != null)
                {
                    JsonValue navigationPropertyValue = new JsonPrimitiveValue(deferredLink.UriString);

                    // Apply cardinality if we have it and it's a request
                    NavigationPropertyCardinalityAnnotation cardinalityAnnotation =
                        (NavigationPropertyCardinalityAnnotation)navigationProperty.GetAnnotation(typeof(NavigationPropertyCardinalityAnnotation));
                    if (isRequest && cardinalityAnnotation != null && cardinalityAnnotation.IsCollection == true)
                    {
                        // In requests, the deferred link must be wrapped in an array if the navigation property is a collection
                        navigationPropertyValue = new JsonArray() { navigationPropertyValue };
                    }

                    // Singleton bind in request or deferred link in response
                    string suffix = isRequest ? JsonLightConstants.ODataBindAnnotationName : JsonLightConstants.ODataNavigationLinkUrlAnnotationName;
                    string annotationName = JsonLightUtils.GetPropertyAnnotationName(navigationProperty.Name, suffix);
                    entityObject.Add(new JsonProperty(annotationName, navigationPropertyValue));
                }
                else if (expandedLink != null && !isRequest)
                {
                    // The navigation URL for an expanded link is only written in responses.
                    AddPropertyIfNotNull(entityObject, JsonLightUtils.GetPropertyAnnotationName(navigationProperty.Name, JsonLightConstants.ODataNavigationLinkUrlAnnotationName), expandedLink.UriString);
                }

                // Then write the association link if any (respones only)
                DeferredLink associationLink = navigationProperty.AssociationLink;
                if (associationLink != null && !isRequest)
                {
                    AddPropertyIfNotNull(entityObject, JsonLightUtils.GetPropertyAnnotationName(navigationProperty.Name, JsonLightConstants.ODataAssociationLinkUrlAnnotationName), associationLink.UriString);
                }

                // Then write the expanded content if any
                if (expandedLink != null)
                {
                    ODataPayloadElement expandedContent = expandedLink.ExpandedElement;
                    if (expandedContent == null)
                    {
                        // Expanded null entry
                        entityObject.Add(new JsonProperty(navigationProperty.Name, new JsonPrimitiveValue(null)));
                    }
                    else if (expandedContent.ElementType == ODataPayloadElementType.EntityInstance)
                    {
                        // Write the expanded entry
                        JsonValue expandedJsonEntry = this.Recurse(expandedContent);
                        entityObject.Add(new JsonProperty(navigationProperty.Name, expandedJsonEntry));
                    }
                    else if (expandedContent.ElementType == ODataPayloadElementType.EntitySetInstance)
                    {
                        // Write the expanded feed
                        EntitySetInstance expandedFeed = (EntitySetInstance)expandedContent;
                        if (!isRequest && expandedFeed.InlineCount.HasValue)
                        {
                            entityObject.Add(new JsonProperty(JsonLightUtils.GetPropertyAnnotationName(navigationProperty.Name, JsonLightConstants.ODataCountAnnotationName), new JsonPrimitiveValue(expandedFeed.InlineCount.Value)));
                        }

                        JsonValue expandedJsonFeed = this.Recurse(expandedFeed);
                        entityObject.Add(new JsonProperty(navigationProperty.Name, expandedJsonFeed));

                        if (!isRequest)
                        {
                            AddPropertyIfNotNull(entityObject, JsonLightUtils.GetPropertyAnnotationName(navigationProperty.Name, JsonLightConstants.ODataNextLinkAnnotationName), expandedFeed.NextLink);
                        }
                    }
                    else
                    {
                        throw new TaupoNotSupportedException("Unsupported expanded content of kind '" + expandedContent.ElementType.ToString() + "'.");
                    }
                }
            }

            /// <summary>
            /// Processes a link collection and adds all JSON properties related to the link collection into the current entity object.
            /// </summary>
            /// <param name="navigationProperty">The parent navigation property of the <paramref name="linkCollection"/>.</param>
            /// <param name="linkCollection">The link collection to process</param>
            /// <param name="entityObject">The entity object to add JSON properties to.</param>
            private void AddNavigationLinkCollection(NavigationPropertyInstance navigationProperty, LinkCollection linkCollection, JsonObject entityObject)
            {
                ExceptionUtilities.Assert(navigationProperty != null, "(navigationProperty != null");
                ExceptionUtilities.Assert(linkCollection != null, "(linkCollection != null");
                ExceptionUtilities.Assert(!IsResponse(navigationProperty), "Link collections in a navigation property are only allowed in requests.");

                // Check that all the deferred links are at the beginning of the list; otherwise fail since this is an invalid
                // payload that we would not be able to consume. For duplicate property name checkin we can produce such payloads
                // manually.
                int expandedLinkIndex = -1;
                for (int i=0; i<linkCollection.Count; ++i)
                {
                    ExpandedLink expandedLink = linkCollection[i] as ExpandedLink;
                    if (expandedLink == null)
                    {
                        if (expandedLinkIndex >= 0)
                        {
                            throw new TaupoInvalidOperationException("Must not have a deferred link after an expanded link in a link collection in a request payload.");
                        }
                    }
                    else
                    {
                        if (expandedLinkIndex < 0)
                        {
                            expandedLinkIndex = i;
                        }
                    }
                }

                // Write the binding operations first
                if (expandedLinkIndex != 0)
                {
                    int index = expandedLinkIndex < 0 ? linkCollection.Count : expandedLinkIndex;
                    JsonArray navigationPropertyValue = new JsonArray();
                    for (int i = 0; i < index; ++i)
                    {
                        navigationPropertyValue.Add(new JsonPrimitiveValue(linkCollection[i].UriString));
                    }

                    entityObject.Add(new JsonProperty(JsonLightUtils.GetPropertyAnnotationName(navigationProperty.Name, JsonLightConstants.ODataBindAnnotationName), navigationPropertyValue));
                }

                // Write the insert operations next
                if (expandedLinkIndex >= 0)
                {
                    JsonArray navigationPropertyValue = new JsonArray();
                    for (int i = expandedLinkIndex; i < linkCollection.Count; ++i)
                    {
                        ODataPayloadElement expandedContent = ((ExpandedLink)linkCollection[i]).ExpandedElement;
                        if (expandedContent == null)
                        {
                            // Sending an expanded entry with a null value in requests is not supported; we do it here
                            // anyways so we can test our readers.
                            // Expanded null entry
                            navigationPropertyValue.Add(new JsonPrimitiveValue(null));
                        }
                        else if (expandedContent.ElementType == ODataPayloadElementType.EntityInstance)
                        {
                            // Sending an expanded entry in requests is not supported for collection properties; we do it
                            // here anyways so we can test our readers.
                            // Write the expanded entry
                            JsonValue expandedJsonEntry = this.Recurse(expandedContent);
                            navigationPropertyValue.Add(expandedJsonEntry);
                        }
                        else if (expandedContent.ElementType == ODataPayloadElementType.EntitySetInstance)
                        {
                            // Write the expanded feed
                            EntitySetInstance expandedFeed = (EntitySetInstance)expandedContent;
                            for (int j = 0; j < expandedFeed.Count(); j++)
                            {
                                navigationPropertyValue.Add(this.Recurse(expandedFeed[j]));
                            }
                        }
                        else
                        {
                            throw new TaupoNotSupportedException("Unsupported expanded content of kind '" + expandedContent.ElementType.ToString() + "'.");
                        }
                    }

                    entityObject.Add(new JsonProperty(navigationProperty.Name, navigationPropertyValue));
                }
            }

            /// <summary>
            /// Create 'actions' or 'functions' metadata property with given list of operations
            /// </summary>
            /// <param name="headerName">"actions" or "functions"</param>
            /// <param name="serviceOperationDescriptors">The list of ServiceOperationDescriptor</param>
            /// <returns></returns>
            private JsonProperty CreateOperationsObject(string headerName, IEnumerable<ServiceOperationDescriptor> serviceOperationDescriptors)
            {
                JsonObject operationsMetadataValue = new JsonObject();

                var groups = serviceOperationDescriptors.GroupBy(o => o.Metadata);

                foreach (var relGroup in groups)
                {
                    JsonProperty relGroupProperty = CreateOperationRelProperty(relGroup);

                    operationsMetadataValue.Add(relGroupProperty);
                }

                return new JsonProperty(headerName, operationsMetadataValue);
            }

            private JsonProperty CreateOperationRelProperty(IEnumerable<ServiceOperationDescriptor> relGroup)
            {
                ExceptionUtilities.CheckArgumentNotNull(relGroup, "relGroup"); 
                ExceptionUtilities.CheckCollectionNotEmpty(relGroup, "relGroup");
                JsonArray array = new JsonArray();

                foreach (ServiceOperationDescriptor operation in relGroup)
                {
                    JsonValue value = this.Visit(operation);
                    array.Add(value);
                }

                return new JsonProperty(relGroup.First().Metadata, array);
            }

            /// <summary>
            /// Creates the odata.type property for a specified type name.
            /// </summary>
            /// <param name="fullTypeName">The type name to use.</param>
            /// <returns>The odata.type property.</returns>
            private JsonProperty CreateTypeName(string fullTypeName)
            {
                return new JsonProperty(JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName, new JsonPrimitiveValue(fullTypeName));
            }

            /// <summary>
            /// Determines if the current element being processed is the root of the element tree.
            /// </summary>
            /// <returns>true if the current element is the root, false otherwise.</returns>
            private bool CurrentElementIsRoot()
            {
                return this.payloadStack.Count == 1;
            }

            /// <summary>
            /// The main Visit method which is called to visit every single element in the payload.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            /// <returns>The serialized element as JSON.</returns>
            private JsonValue Recurse(ODataPayloadElement payloadElement, JsonValue parentValue = null)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result != null)
                {
                    return result;
                }

                // Check for 'IsNull'
                ITypedValue typedValue = payloadElement as ITypedValue;
                if (typedValue != null && typedValue.IsNull)
                {
                    return new JsonPrimitiveValue(null);
                }

                JsonValue originalParent = this.parentElementValue;
                if (parentValue != null)
                {
                    this.parentElementValue = parentValue;
                }

                this.payloadStack.Push(payloadElement);
                try
                {
                    return payloadElement.Accept(this);
                }
                finally
                {
                    this.payloadStack.Pop();
                    if (parentValue != null)
                    {
                        this.parentElementValue = originalParent;
                    }
                }
            }

            /// <summary>
            /// Wraps a top level property in an JSON object.
            /// </summary>
            /// <param name="payloadElement">The payload element which represents the top-level value.</param>
            /// <param name="value">The value to wrap (must be a JsonProperty to wrap).</param>
            /// <param name="needsWrapping">true if the value should be wrapped, false otherwise.</param>
            /// <returns>The original or wrapped value.</returns>
            private JsonValue WrapTopLevelProperty(ODataPayloadElement payloadElement, JsonValue value, bool needsWrapping)
            {
                if (!needsWrapping)
                {
                    return value;
                }

                JsonObject wrapper = new JsonObject();
                AddContextUriProperty(payloadElement, wrapper);
                wrapper.Add((JsonProperty)value);
                return wrapper;
            }

            /// <summary>
            /// Returns the JSON represenation for the payload element from its annotation.
            /// </summary>
            /// <param name="payloadElement">The payload element to inspect.</param>
            /// <returns>The JSON representation or null if the payload element should be serialized as usual.</returns>
            private static JsonValue GetJsonRepresentation(ODataPayloadElement payloadElement)
            {
                JsonPayloadElementRepresentationAnnotation jsonRepresentation =
                    (JsonPayloadElementRepresentationAnnotation)payloadElement.GetAnnotation(typeof(JsonPayloadElementRepresentationAnnotation));

                if (jsonRepresentation == null)
                {
                    return null;
                }
                else
                {
                    return jsonRepresentation.Json;
                }
            }

            /// <summary>
            /// Returns the response/request to use for writing the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element being written.</param>
            /// <returns>true if the element should be written as in response, false if it should be written as in request.</returns>
            private static bool IsResponse(ODataPayloadElement payloadElement)
            {
                PayloadFormatVersionAnnotation payloadFormatVersion =
                    (PayloadFormatVersionAnnotation)payloadElement.GetAnnotation(typeof(PayloadFormatVersionAnnotation));

                if (payloadFormatVersion == null || !payloadFormatVersion.Response.HasValue)
                {
                    // If we don't find an annotation we assume we are writing a request.
                    return false;
                }
                else
                {
                    return payloadFormatVersion.Response.Value;
                }
            }

            private void AddPropertyAnnotations(PropertyInstance property, JsonValue jsonValue) 
            {
                string annotationPrefix = string.Empty;
                JsonObject targetObject = null;
                if (jsonValue is JsonObject)
                {
                    targetObject = (JsonObject)jsonValue;
                }
                else
                {
                    var jsonProperty = jsonValue as JsonProperty;
                    ExceptionUtilities.CheckObjectNotNull(jsonProperty, "Json value is neither object nor property");

                    if (property is ComplexProperty)
                    {
                        targetObject = jsonProperty.Value as JsonObject;
                    }
                    else
                    {
                        targetObject = this.parentElementValue as JsonObject;
                        annotationPrefix = jsonProperty.Name;
                    }
                    
                    ExceptionUtilities.CheckObjectNotNull(targetObject, "Neither property nor parent are Json Objects.");
                }

                var metadataPropertyIndex = targetObject.Properties
                    .Select((prop, index) => new { prop, index })
                    .LastOrDefault(p => p.prop.Name.StartsWith("@odata.") || (!string.IsNullOrEmpty(annotationPrefix) && p.prop.Name.StartsWith(annotationPrefix)));

                int annotationInsertIndex = metadataPropertyIndex == null ? 0 : metadataPropertyIndex.index + 1;

                foreach (var propertyAnnotation in property.Annotations.OfType<JsonLightPropertyAnnotationAnnotation>())
                {
                    string annotationPropertyName = string.IsNullOrEmpty(annotationPrefix) ? JsonLightConstants.ODataPropertyAnnotationSeparator + propertyAnnotation.AnnotationName : JsonLightUtils.GetPropertyAnnotationName(annotationPrefix, propertyAnnotation.AnnotationName);
                    var annotationProperty = new JsonProperty(annotationPropertyName, new JsonPrimitiveValue(propertyAnnotation.AnnotationValue));
                    targetObject.Insert(annotationInsertIndex, annotationProperty);
                }
            }

            private void ReorderPropertyAnnotations(JsonObject jsonObject)
            {
                var properties = jsonObject.Properties.ToList();
                var propertyNames = properties.Select(p => p.Name).ToArray();
                var firstAnnotationNames = new[]
                {
                    JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName,
                    JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName,
                    JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName,
                    JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataETagAnnotationName
                };

                var odataAnnotations = properties.Where(p => firstAnnotationNames.Contains(p.Name));

                // IndexOf('@') > 0 is suitable here, it says that only property annotation is filtered.
                var reorderTheseAnnotations = properties.Except(odataAnnotations).Where(p => p.Name.IndexOf(JsonLightConstants.ODataPropertyAnnotationSeparator, StringComparison.Ordinal) > 0 && 
                    propertyNames.Contains(JsonLightConstants.ODataPropertyAnnotationSeparator + p.Name.Split(new[] { '@' }).ElementAt(1)));

                var remainingProperties = properties.Except(odataAnnotations).Except(reorderTheseAnnotations).ToList();

                foreach (var annotationProperty in reorderTheseAnnotations)
                {
                    string prefix = annotationProperty.Name.Split(new[] { '@' }).ElementAt(0);
                    var correspondingPropertyIndex = remainingProperties
                        .Select((prop, index) => new { prop, index })
                        .FirstOrDefault(p => p.prop.Name == prefix);

                    if (correspondingPropertyIndex != null)
                    {
                        remainingProperties.Insert(correspondingPropertyIndex.index, annotationProperty);
                    }
                    else
                    {
                        remainingProperties.Add(annotationProperty);
                    }
                }

                jsonObject.Clear();
                odataAnnotations.Concat(remainingProperties).ToList().ForEach(jsonObject.Add);
            }
        }
    }
}
