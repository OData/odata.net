//---------------------------------------------------------------------
// <copyright file="AnnotatedPayloadElementToJsonConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    #endregion Namespaces

    /// <summary>
    /// The converter from ODataPayloadElement representation (with possible annotations) to JSON.
    /// </summary>
    [ImplementationName(typeof(IPayloadElementToJsonConverter), "AnnotatedPayloadElementToJsonConverter", HelpText = "Payload to JSON converter which allows JSON specific annotations to be used.")]
    public class AnnotatedPayloadElementToJsonConverter : IPayloadElementToJsonConverter
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
        public ISpatialPrimitiveToODataJsonValueConverter SpatialConverter { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AnnotatedPayloadElementToJsonConverter()
        {
        }

        /// <summary>
        /// Converts the given payload element into a Json representation.
        /// </summary>
        /// <param name="rootElement">The root payload element to convert.</param>        
        /// <returns>The Json representation of the payload.</returns>
        public string ConvertToJson(ODataPayloadElement rootElement)
        {
            return ConvertToJson(rootElement, null);
        }

        /// <summary>
        /// Converts the given payload element into a Json representation.
        /// </summary>
        /// <param name="rootElement">The root payload element to convert.</param>   
        /// <param name="newline">Newline character(s) to use when writing the returned string.
        /// A null strings results in the use of the default.</param>
        /// <returns>The Json representation of the payload.</returns>
        public string ConvertToJson(ODataPayloadElement rootElement, string newline)
        {
            JsonValue jsonValue = this.ConvertToJsonValue(rootElement);

            IPayloadTransform<JsonValue> jsonTransform = this.TransformFactory.GetTransform<JsonValue>();

            JsonValue transformedJsonPayload = null;
            if (jsonTransform.TryTransform(jsonValue, out transformedJsonPayload))
            {
                jsonValue = transformedJsonPayload;
            }

            StringBuilder builder = new StringBuilder();
            using (StringWriter strWriter = new StringWriter(builder, CultureInfo.CurrentCulture))
            {
                if (newline != null)
                {
                    strWriter.NewLine = newline;
                }

                JsonTextPreservingWriter jsonWriter = new JsonTextPreservingWriter(strWriter, writingJsonLight: false);
                jsonWriter.WriteValue(jsonValue);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts the given payload element into a Json representation.
        /// </summary>
        /// <param name="rootElement">The root payload element to convert.</param>        
        /// <returns>The Json representation of the payload.</returns>
        public JsonValue ConvertToJsonValue(ODataPayloadElement rootElement)
        {
            return new SerializeODataPayloadElementVisitor(this).Serialize(rootElement);
        }
        
        /// <summary>
        /// Visitor for Serializing ODataPayload Element
        /// </summary>
        internal class SerializeODataPayloadElementVisitor : IODataPayloadElementVisitor<JsonValue>
        {
            private Stack<ODataPayloadElement> payloadStack = new Stack<ODataPayloadElement>();
            private readonly AnnotatedPayloadElementToJsonConverter parent;

            /// <summary>
            /// Initializes a new instance of the SerializeOdataPayloadElementVisitor class
            /// </summary>
            internal SerializeODataPayloadElementVisitor(AnnotatedPayloadElementToJsonConverter parent)
            {
                ExceptionUtilities.CheckArgumentNotNull(parent, "parent");
                this.parent = parent;
            }

            /// <summary>
            /// Serialize the current payload element
            /// </summary>
            /// <param name="rootElement">Element to be converted</param>            
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Serialize(ODataPayloadElement rootElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");

                return this.VisitElement(rootElement);
            }

            /// <summary>
            /// This method should never be called.
            /// </summary>
            /// <param name="payloadElement">Batch Payload</param>
            /// <returns>Throws InvalidOperationException</returns>
            public JsonValue Visit(BatchRequestPayload payloadElement)
            {
                throw new InvalidOperationException("A batch payload cannot be converted to Json.");
            }

            /// <summary>
            /// This method should never be called.
            /// </summary>
            /// <param name="payloadElement">Batch Payload</param>
            /// <returns>Throws InvalidOperationException</returns>
            public JsonValue Visit(BatchResponsePayload payloadElement)
            {
                throw new InvalidOperationException("A batch payload cannot be converted to Json.");
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

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    result = new JsonProperty(payloadElement.Name, this.VisitElement(payloadElement.Value));
                }

                return this.WrapTopLevelResult(payloadElement, result, needsWrapping);
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

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result != null)
                {
                    return result;
                }
                if (payloadElement.IsNull)
                {
                    return new JsonPrimitiveValue(null);
                }
                else
                {
                    JsonObject complexValue = new JsonObject();
                    if (payloadElement.FullTypeName != null)
                    {
                        complexValue.Add(CreateMetadataProperty(payloadElement.FullTypeName));
                    }

                    foreach (var property in payloadElement.Properties)
                    {
                        complexValue.Add((JsonProperty)this.VisitElement(property));
                    }

                    return complexValue;
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexInstanceCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ComplexInstanceCollection payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    JsonArray complexCollectionValue = new JsonArray();
                    foreach (var item in payloadElement)
                    {
                        complexCollectionValue.Add(this.VisitElement(item));
                    }

                    JsonObject resultsWrapper = new JsonObject();
                    resultsWrapper.Add(new JsonProperty("results", complexCollectionValue));
                    result = resultsWrapper;
                }

                return this.WrapTopLevelResult(payloadElement, result, false);
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexMultiValue.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ComplexMultiValue payloadElement)
            {
                // This is the value of MultiValue of Complex types.
                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result != null)
                {
                    return result;
                }
                else 
                {
                    JsonObject complexMultiValueWrapperObject = new JsonObject();
                    if (payloadElement.FullTypeName != null)
                    {
                        complexMultiValueWrapperObject.Add(CreateMetadataProperty(payloadElement.FullTypeName));
                    }

                    JsonValue resultsValue;
                    if (payloadElement.IsNull)
                    {
                        resultsValue = new JsonPrimitiveValue(null);
                    }
                    else
                    {
                        JsonArray complexMultiValueItems = new JsonArray();
                        foreach (var item in payloadElement)
                        {
                            complexMultiValueItems.Add(this.VisitElement(item));
                        }

                        resultsValue = complexMultiValueItems;
                    }

                    if (payloadElement.Annotations.OfType<JsonCollectionResultWrapperAnnotation>().Any(a => !a.Value))
                    {
                        return resultsValue;
                    }

                    complexMultiValueWrapperObject.Add(new JsonProperty("results", resultsValue));
                    return complexMultiValueWrapperObject;
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ComplexProperty payloadElement)
            {
                bool needsWrapping = this.CurrentElementIsRoot();

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    result = new JsonProperty(payloadElement.Name, this.VisitElement(payloadElement.Value));
                }

                return this.WrapTopLevelResult(payloadElement, result, needsWrapping);
            }

            /// <summary>
            /// Visits a payload element whose root is a DeferredLink.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(DeferredLink payloadElement)
            {
                bool isExpandedReferenceLink = this.payloadStack.Count > 1 && this.payloadStack.ElementAt(1).ElementType == ODataPayloadElementType.LinkCollection;
                bool needsWrapping = isExpandedReferenceLink || this.CurrentElementIsRoot();

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    if (needsWrapping)
                    {
                        result = new JsonObject()
                        {
                            new JsonProperty("uri", new JsonPrimitiveValue(payloadElement.UriString))
                        };
                    }
                    else
                    {
                        string propertyName = IsResponse(payloadElement) == false
                                              ? "__metadata"
                                              : "__deferred";

                        result = new JsonObject()
                        {
                            new JsonProperty(propertyName, new JsonObject() 
                            { 
                                new JsonProperty("uri", new JsonPrimitiveValue(payloadElement.UriString)) 
                            })
                        };
                    }
                }

                return this.WrapTopLevelResult(payloadElement, result, false);
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

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    result = new JsonProperty(payloadElement.Name, this.VisitElement(payloadElement.Value));
                }

                return this.WrapTopLevelResult(payloadElement, result, needsWrapping);
            }

            /// <summary>
            /// Visits a payload element whose root is an EmptyPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(EmptyPayload payloadElement)
            {
                throw new TaupoNotSupportedException("Json serialization does not allow element of type: " + payloadElement.ElementType.ToString());
            }

            /// <summary>
            /// Visits a payload element whose root is an EmptyUntypedCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(EmptyUntypedCollection payloadElement)
            {
                // The JSON deserializer uses this if it finds and empty array.
                // The XML deserializer will never use it.
                // So basically this is a non-metadata version of an empty entity set.

                JsonValue result = GetJsonRepresentation(payloadElement) ?? new JsonArray();
                return this.WrapTopLevelResult(payloadElement, result, false);
            }

            /// <summary>
            /// Visits a payload element whose root is an EntityInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(EntityInstance payloadElement)
            {
                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    if (payloadElement.IsNull)
                    {
                        result = new JsonPrimitiveValue(null);
                    }
                    else
                    {
                        JsonObject entityValue = new JsonObject();

                        JsonProperty metadataProperty = CreateMetadataProperty(payloadElement);
                        if (metadataProperty != null)
                        {
                            entityValue.Add(metadataProperty);
                        }

                        foreach (var property in payloadElement.Properties)
                        {
                            JsonProperty jsonProperty = (JsonProperty)this.VisitElement(property);
                            if (jsonProperty != null)
                            {
                                entityValue.Add(jsonProperty);
                            }
                        }

                        result = entityValue;
                    }
                }

                return this.WrapTopLevelResult(payloadElement, result, false);
            }

            /// <summary>
            /// Visits a payload element whose root is an EntitySetInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(EntitySetInstance payloadElement)
            {
                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    JsonArray entitySetValue = new JsonArray();
                    for (int i = 0; i < payloadElement.Count(); i++)
                    {
                        entitySetValue.Add(this.VisitElement(payloadElement[i]));
                    }

                        JsonObject resultsWrapper = new JsonObject();
                        if (payloadElement.InlineCount.HasValue)
                        {
                            resultsWrapper.Add(new JsonProperty("__count", new JsonPrimitiveValue(payloadElement.InlineCount.Value)));
                        }

                        resultsWrapper.Add(new JsonProperty("results", entitySetValue));

                        if (payloadElement.NextLink != null)
                        {
                            resultsWrapper.Add(new JsonProperty("__next", new JsonPrimitiveValue(payloadElement.NextLink)));
                        }

                        result = resultsWrapper;
                }

                return this.WrapTopLevelResult(payloadElement, result, false);
            }

            /// <summary>
            /// Visits a payload element whose root is an ExpandedLink.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ExpandedLink payloadElement)
            {
                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    if (payloadElement.ExpandedElement == null)
                    {
                        result = new JsonPrimitiveValue(null);
                    }
                    else
                    {
                        result = this.VisitElement(payloadElement.ExpandedElement);
                    }
                }

                return this.WrapTopLevelResult(payloadElement, result, false);
            }

            /// <summary>
            /// Visits a payload element whose root is a LinkCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(LinkCollection payloadElement)
            {
                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    JsonArray linkCollectionValue = new JsonArray();
                    foreach(Link link in payloadElement)
                    {
                        var expandedLink = link as ExpandedLink;
                        if (expandedLink != null)
                        {
                            // Flatten expanded links, and just serialise the entities
                            var expandedFeed = expandedLink.ExpandedElement as EntitySetInstance;
                            if (expandedFeed != null)
                            {
                                foreach (var entity in expandedFeed)
                                {
                                    linkCollectionValue.Add(this.VisitElement(entity));
                                }
                            }
                        }
                        else
                        {
                            linkCollectionValue.Add(this.VisitElement(link));
                        }
                    }

                    if (GetFormatVersion(payloadElement) >= DataServiceProtocolVersion.V4)
                    {
                        JsonObject resultsWrapper = new JsonObject();
                        if (payloadElement.InlineCount.HasValue)
                        {
                            resultsWrapper.Add(new JsonProperty("__count", new JsonPrimitiveValue(payloadElement.InlineCount.Value)));
                        }

                        resultsWrapper.Add(new JsonProperty("results", linkCollectionValue));

                        if (payloadElement.NextLink != null)
                        {
                            resultsWrapper.Add(new JsonProperty("__next", new JsonPrimitiveValue(payloadElement.NextLink)));
                        }

                        result = resultsWrapper;
                    }
                    else
                    {
                        result = linkCollectionValue;
                    }
                }

                return this.WrapTopLevelResult(payloadElement, result, false);
            }

            /// <summary>
            /// Visits a payload element whose root is a NamedStreamProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(NamedStreamInstance payloadElement)
            {
                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    JsonObject namedStream = new JsonObject();

                    if (payloadElement.SourceLink != null)
                    {
                        JsonProperty sourceLink = new JsonProperty("media_src", new JsonPrimitiveValue(payloadElement.SourceLink));
                        namedStream.Add(sourceLink);
                    }

                    if (payloadElement.EditLink != null)
                    {
                        JsonProperty editLink = new JsonProperty("edit_media", new JsonPrimitiveValue(payloadElement.EditLink));
                        namedStream.Add(editLink);
                    }

                    // NOTE the named stream instance stores two content types; one for the read link and
                    //      one for the edit link. If an edit link is present, the edit link's value will win.
                    string contentTypeString = null;
                    if (payloadElement.EditLink != null && payloadElement.EditLinkContentType != null)
                    {
                        contentTypeString = payloadElement.EditLinkContentType;
                    }
                    else if (payloadElement.SourceLink != null && payloadElement.SourceLinkContentType != null)
                    {
                        contentTypeString = payloadElement.SourceLinkContentType;
                    }
                    else
                    {
                        // If no links are present or there are mismatches, use the edit link's content type, if not available use source link's content type.
                        contentTypeString = payloadElement.EditLinkContentType ?? payloadElement.SourceLinkContentType;
                    }

                    if (contentTypeString != null)
                    {
                        JsonProperty contentType = new JsonProperty("content_type", new JsonPrimitiveValue(contentTypeString));
                        namedStream.Add(contentType);
                    }

                    if (payloadElement.ETag != null)
                    {
                        JsonProperty etag = new JsonProperty("media_etag", new JsonPrimitiveValue(payloadElement.ETag));
                        namedStream.Add(etag);
                    }

                    JsonObject mediaResourceWrapper = new JsonObject()
                    {
                        new JsonProperty("__mediaresource", namedStream)
                    };

                    result = new JsonProperty(payloadElement.Name, mediaResourceWrapper);
                }

                return this.WrapTopLevelResult(payloadElement, result, false);
            }

            /// <summary>
            /// Visits a payload element whose root is a NavigationProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(NavigationPropertyInstance payloadElement)
            {
                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    JsonValue value;
                    if (payloadElement.Value == null)
                    {
                        // If there is no value, it might be that the nav. property is here just for the association link
                        // so skip it here.
                        return null;
                    }
                    else
                    {
                        value = this.VisitElement(payloadElement.Value);

                        DeferredLink deferredLink = payloadElement.Value as DeferredLink;
                        if (deferredLink != null)
                        {
                            // Apply cardinality if we have it and it's a request
                            NavigationPropertyCardinalityAnnotation cardinalityAnnotation = 
                                (NavigationPropertyCardinalityAnnotation)payloadElement.GetAnnotation(typeof(NavigationPropertyCardinalityAnnotation));
                            if (IsResponse(deferredLink) == false && cardinalityAnnotation != null && cardinalityAnnotation.IsCollection == true)
                            {
                                // In requests, the deferred link must be wrapped in an array if the navigation property is a collection
                                value = new JsonArray() { value };
                            }
                        }
                    }

                    result = new JsonProperty(payloadElement.Name, value);
                }

                return this.WrapTopLevelResult(payloadElement, result, false);
            }

            /// <summary>
            /// Visits a payload element whose root is a NullPropertyInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(NullPropertyInstance payloadElement)
            {
                bool needsWrapping = this.CurrentElementIsRoot();

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    result = new JsonProperty(payloadElement.Name, new JsonPrimitiveValue(null));
                }

                return this.WrapTopLevelResult(payloadElement, result, needsWrapping);
            }

            /// <summary>
            /// Visits a payload element whose root is an ODataErrorPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public JsonValue Visit(ODataErrorPayload payloadElement)
            {
                ExceptionUtilities.Assert(this.CurrentElementIsRoot(), "Json serialization requires an element of type {0} to be at the root level.", payloadElement.ElementType.ToString());

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    JsonObject errorObject = new JsonObject();

                    // the error code
                    if (payloadElement.Code != null)
                    {
                        errorObject.Add(new JsonProperty("code", new JsonPrimitiveValue(payloadElement.Code)));
                    }

                    // the message
                    if (payloadElement.Message != null)
                    {
                        errorObject.Add(new JsonProperty("message", new JsonPrimitiveValue(payloadElement.Message)));
                    }

                    if (payloadElement.InnerError != null)
                    {
                        JsonValue innerErrorValue = this.VisitElement(payloadElement.InnerError);
                        errorObject.Add(new JsonProperty("innererror", innerErrorValue));
                    }

                    result = new JsonObject()
                    {
                        new JsonProperty("error", errorObject)
                    };
                }

                return this.WrapTopLevelResult(payloadElement, result, false);
            }

            /// <summary>
            /// Visits a payload element whose root is an ODataInternalExceptionPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public JsonValue Visit(ODataInternalExceptionPayload payloadElement)
            {
                ExceptionUtilities.Assert(!this.CurrentElementIsRoot(), "Json serialization does not support an element of type {0} at the root level.", payloadElement.ElementType.ToString());

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    JsonObject innerErrorObject = new JsonObject();

                    // the inner error message
                    if (payloadElement.Message != null)
                    {
                        innerErrorObject.Add(new JsonProperty("message", new JsonPrimitiveValue(payloadElement.Message)));
                    }

                    // the inner error type name
                    if (payloadElement.TypeName != null)
                    {
                        innerErrorObject.Add(new JsonProperty("type", new JsonPrimitiveValue(payloadElement.TypeName)));
                    }

                    // the inner stack trace
                    if (payloadElement.StackTrace != null)
                    {
                        innerErrorObject.Add(new JsonProperty("stacktrace", new JsonPrimitiveValue(payloadElement.StackTrace)));
                    }

                    // the nested inner error
                    if (payloadElement.InternalException != null)
                    {
                        JsonValue nestedInnerError = this.VisitElement(payloadElement.InternalException);
                        innerErrorObject.Add(new JsonProperty("internalexception", nestedInnerError));
                    }

                    result = innerErrorObject;
                }

                return result;
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(PrimitiveCollection payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    JsonArray collectionValue = new JsonArray();
                    foreach (var item in payloadElement)
                    {
                        collectionValue.Add(this.VisitElement(item));
                    }

                    if (GetFormatVersion(payloadElement) >= DataServiceProtocolVersion.V4)
                    {
                        JsonObject resultsWrapper = new JsonObject();
                        resultsWrapper.Add(new JsonProperty("results", collectionValue));
                        result = resultsWrapper;
                    }
                    else
                    {
                        result = collectionValue;
                    }
                }

                return this.WrapTopLevelResult(payloadElement, result, false);
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveMultiValue.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(PrimitiveMultiValue payloadElement)
            {
                // This is the value of the MultiValue of primitive values.
                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    JsonObject primitiveMultiValueWrapperObject = new JsonObject();
                    if (payloadElement.FullTypeName != null)
                    {
                        primitiveMultiValueWrapperObject.Add(CreateMetadataProperty(payloadElement.FullTypeName));
                    }

                    JsonValue resultsValue;
                    if (payloadElement.IsNull)
                    {
                        resultsValue = new JsonPrimitiveValue(null);
                    }
                    else
                    {
                        JsonArray primitiveMultiValueItems = new JsonArray();
                        foreach (var item in payloadElement)
                        {
                            primitiveMultiValueItems.Add(this.VisitElement(item));
                        }

                        resultsValue = primitiveMultiValueItems;
                    }

                    if (payloadElement.Annotations.OfType<JsonCollectionResultWrapperAnnotation>().Any(a => !a.Value))
                    {
                        return resultsValue;
                    }

                    primitiveMultiValueWrapperObject.Add(new JsonProperty("results", resultsValue));
                    return primitiveMultiValueWrapperObject;
                }
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

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    result = new JsonProperty(payloadElement.Name, this.VisitElement(payloadElement.Value));
                }

                return this.WrapTopLevelResult(payloadElement, result, needsWrapping);
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(PrimitiveProperty payloadElement)
            {
                bool needsWrapping = this.CurrentElementIsRoot();

                JsonValue result = GetJsonRepresentation(payloadElement) ?? new JsonProperty(payloadElement.Name, this.VisitElement(payloadElement.Value));
                return this.WrapTopLevelResult(payloadElement, result, needsWrapping);
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveValue.
            /// </summary>
            /// <param name="payloadElement">The root node of the payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(PrimitiveValue payloadElement)
            {
                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result != null)
                {
                    return result;
                }
                else if (payloadElement.IsNull)
                {
                    return new JsonPrimitiveValue(null);
                }
                else
                {
                    JsonObject converted;
                    if (this.parent.SpatialConverter.TryConvertIfSpatial(payloadElement, out converted))
                    {
                        return converted;
                    }

                    return new JsonPrimitiveValue(payloadElement.ClrValue);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ServiceDocumentInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ServiceDocumentInstance payloadElement)
            {
                ExceptionUtilities.Assert(this.CurrentElementIsRoot(), "Json serialization allows an element of type '{0}' only at the root level.", payloadElement.ElementType.ToString());

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    // NOTE: ODataLib currently only supports a single workspace in a service document
                    ExceptionUtilities.Assert(payloadElement.Workspaces.Count == 1, "Expected exactly one workspace in a service document.");
                    result = this.VisitElement(payloadElement.Workspaces[0]);
                }

                return this.WrapTopLevelResult(payloadElement, result, false);
            }

            /// <summary>
            /// Serialize the current payload element
            /// </summary>
            /// <param name="serviceOperationDescriptor">Element to be converted</param>            
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ServiceOperationDescriptor serviceOperationDescriptor)
            {
                JsonObject operationValue = new JsonObject();

                if (serviceOperationDescriptor.Title != null)
                {
                    operationValue.Add(new JsonProperty("title", new JsonPrimitiveValue(serviceOperationDescriptor.Title)));
                }

                if (serviceOperationDescriptor.Target != null)
                {
                    operationValue.Add(new JsonProperty("target", new JsonPrimitiveValue(serviceOperationDescriptor.Target)));
                }

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

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result == null)
                {
                    JsonArray collectionArray = new JsonArray();

                    if (payloadElement.ResourceCollections != null)
                    {
                        foreach (ResourceCollectionInstance collection in payloadElement.ResourceCollections)
                        {
                            collectionArray.Add(this.VisitElement(collection));
                        }
                    }

                    result = new JsonObject()
                    {
                        new JsonProperty("EntitySets", collectionArray)
                    };
                }

                return result;
            }

            /// <summary>
            /// Visits a payload element whose root is a ResourceCollectionInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            /// <returns>The JsonValue representing the payload.</returns>
            public JsonValue Visit(ResourceCollectionInstance payloadElement)
            {
                ExceptionUtilities.Assert(!this.CurrentElementIsRoot(), "Json serialization does not allow a root element of type: {0}", payloadElement.ElementType.ToString());

                JsonValue result = GetJsonRepresentation(payloadElement);
                if (result != null)
                {
                    return result;
                }

                // NOTE: ODataLib currently stores the entity set names for JSON service documents
                //       in the Href property (since the title property is only available in ATOM 
                //       and retrieved from ATOM metadata in that case).
                return new JsonPrimitiveValue(payloadElement.Href);
            }

            /// <summary>
            /// Creates the metadata property for the specified entity.
            /// </summary>
            /// <param name="payloadElement">Entity payload element.</param>
            /// <returns>The metadata property.</returns>
            private JsonProperty CreateMetadataProperty(EntityInstance payloadElement)
            {
                if (payloadElement.ETag == null && payloadElement.FullTypeName == null && payloadElement.Id == null)
                {
                    return null;
                }

                JsonObject metadataValue = new JsonObject();

                string uri = payloadElement.GetEditLink();
                if (uri == null)
                {
                    uri = payloadElement.GetSelfLink();
                }

                if (uri != null)
                {
                    metadataValue.Add(new JsonProperty("uri", new JsonPrimitiveValue(uri)));
                }

                if (payloadElement.Id != null)
                {
                    metadataValue.Add(new JsonProperty("id", new JsonPrimitiveValue(payloadElement.Id)));
                }

                if (payloadElement.FullTypeName != null)
                {
                    metadataValue.Add(new JsonProperty("type", new JsonPrimitiveValue(payloadElement.FullTypeName)));
                }

                if (payloadElement.ETag != null)
                {
                    metadataValue.Add(new JsonProperty("etag", new JsonPrimitiveValue(payloadElement.ETag)));
                }

                // write all the default stream properties if the entry is an MLE
                if (payloadElement.IsMediaLinkEntry())
                {
                    if (payloadElement.StreamETag != null)
                    {
                        metadataValue.Add(new JsonProperty("media_etag", new JsonPrimitiveValue(payloadElement.StreamETag)));
                    }

                    if (payloadElement.StreamContentType != null)
                    {
                        metadataValue.Add(new JsonProperty("content_type", new JsonPrimitiveValue(payloadElement.StreamContentType)));
                    }

                    if (payloadElement.StreamSourceLink != null)
                    {
                        metadataValue.Add(new JsonProperty("media_src", new JsonPrimitiveValue(payloadElement.StreamSourceLink)));
                    }

                    if (payloadElement.StreamEditLink != null)
                    {
                        metadataValue.Add(new JsonProperty("edit_media", new JsonPrimitiveValue(payloadElement.StreamEditLink)));
                    }
                }

                var actionDescriptors = payloadElement.ServiceOperationDescriptors.Where(s => s.IsAction).ToList();
                if (actionDescriptors.Count > 0)
                {
                    JsonProperty actionsProperty = this.CreateOperationsObject("actions", actionDescriptors);
                    metadataValue.Add(actionsProperty);
                }

                var functionsDescriptors = payloadElement.ServiceOperationDescriptors.Where(s => s.IsFunction).ToList();
                if (functionsDescriptors.Count > 0)
                {
                    JsonProperty functionsProperty = this.CreateOperationsObject("functions", functionsDescriptors);
                    metadataValue.Add(functionsProperty);
                }

                // write all the association links
                IList<NavigationPropertyInstance> navPropsWithAssociationLinks = payloadElement
                    .Properties.Where(p => p is NavigationPropertyInstance && ((NavigationPropertyInstance)p).AssociationLink != null)
                    .Cast<NavigationPropertyInstance>().ToList();

                if (navPropsWithAssociationLinks.Count > 0)
                {
                    JsonObject propertiesMetadataValue = new JsonObject();

                    foreach (NavigationPropertyInstance navProp in navPropsWithAssociationLinks)
                    {
                        JsonObject associationUriObject = new JsonObject();

                        associationUriObject.Add(new JsonProperty("associationuri", new JsonPrimitiveValue(navProp.AssociationLink.UriString)));

                        propertiesMetadataValue.Add(new JsonProperty(navProp.Name, associationUriObject));
                    }

                    metadataValue.Add(new JsonProperty("properties", propertiesMetadataValue));
                }

                return new JsonProperty("__metadata", metadataValue);
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
            /// Creates the metadata property for a specified type name.
            /// </summary>
            /// <param name="fullTypeName">The type name to use.</param>
            /// <returns>The metadata property.</returns>
            private static JsonProperty CreateMetadataProperty(string fullTypeName)
            {
                return new JsonProperty("__metadata",
                    new JsonObject()
                        {
                            new JsonProperty("type", new JsonPrimitiveValue(fullTypeName))
                        });
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
            private JsonValue VisitElement(ODataPayloadElement payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                this.payloadStack.Push(payloadElement);
                try
                {
                    return payloadElement.Accept(this);
                }
                finally
                {
                    this.payloadStack.Pop();
                }
            }

            /// <summary>
            /// Wraps top level result in an JSON object.
            /// </summary>
            /// <param name="payloadElement">The payload element which represents the top-level value.</param>
            /// <param name="value">The value to wrap (must be a JsonProperty to wrap).</param>
            /// <param name="needsWrapping">true if the value should be wrapped, false otherwise.</param>
            /// <returns>The original or wrapped value.</returns>
            private JsonValue WrapTopLevelResult(ODataPayloadElement payloadElement, JsonValue value, bool needsWrapping)
            {
                // Wrap the result in an object if asked.
                JsonValue result = needsWrapping ? new JsonObject { (JsonProperty)value } : value;

                // Wrap the result in the "d" wrapper if annotated as such.
                PayloadFormatVersionAnnotation payloadFormatVersionAnnotation =
                    (PayloadFormatVersionAnnotation)payloadElement.GetAnnotation(typeof(PayloadFormatVersionAnnotation));
                if (payloadFormatVersionAnnotation != null && payloadFormatVersionAnnotation.ResponseWrapper && this.CurrentElementIsRoot())
                {
                    result = new JsonObject { new JsonProperty("d", result) };
                }

                return result;
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
            /// Returns the version of the format to use for writing the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element being written.</param>
            /// <returns>The format version to use.</returns>
            private static DataServiceProtocolVersion GetFormatVersion(ODataPayloadElement payloadElement)
            {
                PayloadFormatVersionAnnotation payloadFormatVersion =
                    (PayloadFormatVersionAnnotation)payloadElement.GetAnnotation(typeof(PayloadFormatVersionAnnotation));

                if (payloadFormatVersion == null || payloadFormatVersion.Version == null)
                {
                    return DataServiceProtocolVersion.V4;
                }
                else
                {
                    return payloadFormatVersion.Version.Value;
                }
            }

            /// <summary>
            /// Returns the response/request to use for writing the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element being written.</param>
            /// <returns>true if the element should be written as in response, false if it should be written as in request, null if it was not specified.</returns>
            private static bool? IsResponse(ODataPayloadElement payloadElement)
            {
                PayloadFormatVersionAnnotation payloadFormatVersion =
                    (PayloadFormatVersionAnnotation)payloadElement.GetAnnotation(typeof(PayloadFormatVersionAnnotation));

                if (payloadFormatVersion == null)
                {
                    return null;
                }
                else
                {
                    return payloadFormatVersion.Response;
                }
            }
        }
    }
}
