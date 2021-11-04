//---------------------------------------------------------------------
// <copyright file="ODataExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mime;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    
    /// <summary>
    /// Various useful extensions when working with OData requests or payloads
    /// </summary>
    public static class ODataExtensions
    {
        /// <summary>
        /// Randomizes the capitalization of the media-type portion of the given content type
        /// </summary>
        /// <param name="random">The random number generator to use</param>
        /// <param name="contentType">The content type to capitalize</param>
        /// <returns>The content type, with random capitalization of the media type</returns>
        public static string RandomizeContentTypeCapitalization(this IRandomNumberGenerator random, string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return contentType;
            }

            ContentType type = new ContentType(contentType);
            type.MediaType = random.RandomizeCapitalization(type.MediaType);
            return type.ToString();
        }

        /// <summary>
        /// Replaces the given element, storing the original in an annotation and copying the other annotations over.
        /// </summary>
        /// <typeparam name="TReplace">The type of the replacement element</typeparam>
        /// <param name="toReplace">The original element</param>
        /// <param name="replacement">The replacement element</param>
        /// <returns>The replacement element with the original in an annotation</returns>
        public static TReplace ReplaceWith<TReplace>(this ODataPayloadElement toReplace, TReplace replacement)
            where TReplace : ODataPayloadElement
        {
            return replacement.WithAnnotations(toReplace.Annotations.Select(a => a.Clone()).Concat(new ReplacedElementAnnotation() { Original = toReplace }));
        }

        /// <summary>
        /// Adds the given annotations to the given element
        /// </summary>
        /// <typeparam name="TElement">The type of the element</typeparam>
        /// <param name="element">The element to annotate</param>
        /// <param name="annotations">The annotations to add</param>
        /// <returns>The element with annotations added</returns>
        public static TElement WithAnnotations<TElement>(this TElement element, IEnumerable<ODataPayloadElementAnnotation> annotations)
            where TElement : ODataPayloadElement
        {
            foreach (var annotation in annotations)
            {
                element.Annotations.Add(annotation);
            }

            return element;
        }

        /// <summary>
        /// Adds the given annotations to the given element
        /// </summary>
        /// <typeparam name="TElement">The type of the element</typeparam>
        /// <param name="element">The element to annotate</param>
        /// <param name="annotations">The annotations to add</param>
        /// <returns>The element with annotations added</returns>
        public static TElement WithAnnotations<TElement>(this TElement element, params ODataPayloadElementAnnotation[] annotations)
            where TElement : ODataPayloadElement
        {
            return element.WithAnnotations((IEnumerable<ODataPayloadElementAnnotation>)annotations);
        }

        /// <summary>
        /// Sets the stream source link of the given entity, then returns it
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <param name="link">The stream source link </param>
        /// <returns>The given entity with the updated stream source link</returns>
        public static EntityInstance WithStreamSourceLink(this EntityInstance entity, string link)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            entity.StreamSourceLink = link;
            return entity;
        }

        /// <summary>
        /// Sets the stream edit link of the given entity, then returns it
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <param name="link">The stream edit link </param>
        /// <returns>The given entity with the updated stream edit link</returns>
        public static EntityInstance WithStreamEditLink(this EntityInstance entity, string link)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            entity.StreamEditLink = link;
            return entity;
        }

        /// <summary>
        /// Sets the stream etag of the given entity, then returns it
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <param name="etag">The stream etag </param>
        /// <returns>The given entity with the updated stream etag</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "etag", Justification = "Casing is correct")]
        public static EntityInstance WithStreamETag(this EntityInstance entity, string etag)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            entity.StreamETag = etag;
            return entity;
        }

        /// <summary>
        /// Sets the stream content type of the given entity, then returns it
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <param name="contentType">The stream content type</param>
        /// <returns>The given entity with the updated stream content type</returns>
        public static EntityInstance WithStreamContentType(this EntityInstance entity, string contentType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            entity.StreamContentType = contentType;
            return entity;
        }

        /// <summary>
        /// Uses the content type of the request to get a deserializer, and asserts that the root element is of the expected type
        /// </summary>
        /// <typeparam name="TElement">The expected root element type</typeparam>
        /// <param name="request">The request to deserialize</param>
        /// <param name="selector">The protocol format selector</param>
        /// <returns>The root element of the request</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required to lower case content type header")]
        public static TElement DeserializeAndCast<TElement>(this IHttpRequest request, IProtocolFormatStrategySelector selector) where TElement : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");

            var odataRequest = request as ODataRequest;
            if (odataRequest != null)
            {
                ExceptionUtilities.CheckObjectNotNull(odataRequest.Body, "OData request body unexpectedly null");
                var rootElement = odataRequest.Body.RootElement;
                var afterCast = rootElement as TElement;
                ExceptionUtilities.CheckObjectNotNull(afterCast, "Root element was of unexpected type '{0}'. Expected '{1}'", rootElement.ElementType, ODataPayloadElement.GetElementType<TElement>());
                return afterCast;
            }
            else
            {
                string contentType;
                ExceptionUtilities.Assert(request.TryGetHeaderValueIgnoreHeaderCase(HttpHeaders.ContentType, out contentType), "Cannot deserialize request. 'Content-Type' header not found");

                return DeserializeAndCast<TElement>(selector, null, contentType, request.GetRequestBody());
            }
        }

        /// <summary>
        /// Uses the content type of the response to get a deserializer, and asserts that the root element is of the expected type
        /// </summary>
        /// <typeparam name="TElement">The expected root element type</typeparam>
        /// <param name="response">The response to deserialize</param>
        /// <param name="selector">The protocol format selector</param>
        /// <returns>The root element of the response</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Base type leads to ambiguouosness with method above")]
        public static TElement DeserializeAndCast<TElement>(this HttpResponseData response, IProtocolFormatStrategySelector selector) where TElement : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(response, "response");

            // TODO: parse uri
            string contentType;
            ExceptionUtilities.Assert(response.TryGetHeaderValueIgnoreHeaderCase(HttpHeaders.ContentType, out contentType), "Cannot deserialize response. 'Content-Type' header not found");
            return DeserializeAndCast<TElement>(selector, null, contentType, response.Body);
        }
        
        /// <summary>
        /// Uses the content type and uri to get a deserializer, and asserts that the root element is of the expected type
        /// </summary>
        /// <typeparam name="TElement">The expected root element type</typeparam>
        /// <param name="selector">The protocol format selector</param>
        /// <param name="uri">The uri to use for getting a format strategy</param>
        /// <param name="contentType">The content type to use for getting a format strategy</param>
        /// <param name="body">The body to deserialize</param>
        /// <returns>The root element of the body</returns>
        public static TElement DeserializeAndCast<TElement>(this IProtocolFormatStrategySelector selector, ODataUri uri, string contentType, byte[] body) where TElement : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(selector, "selector");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(contentType, "contentType");
            ExceptionUtilities.CheckArgumentNotNull(body, "body");

            // TODO: force uri to be non-null
            var strategy = selector.GetStrategy(contentType, uri);
            ExceptionUtilities.CheckObjectNotNull(strategy, "Could not find protocol strategy for content-type '{0}'.", contentType);

            var deserializer = strategy.GetDeserializer();
            ExceptionUtilities.CheckObjectNotNull(deserializer, "Strategy returned null deserializer");

            string charset = HttpUtilities.GetContentTypeCharsetOrNull(contentType);

            var rootElement = deserializer.DeserializeFromBinary(body, new ODataPayloadContext { EncodingName = charset });
            ExceptionUtilities.CheckObjectNotNull(rootElement, "Deserializer returned null element");

            var afterCast = rootElement as TElement;
            ExceptionUtilities.CheckObjectNotNull(afterCast, "Root element was of unexpected type '{0}'. Expected '{1}'", rootElement.ElementType, typeof(TElement).Name);

            return afterCast;
        }

        /// <summary>
        /// Returns a value indicating whether the given syndication item property supports extension attributes
        /// </summary>
        /// <param name="itemProperty">The syndication item property</param>
        /// <returns>True if the property is Title, Summary, Rights, or Custom</returns>
        public static bool SupportsExtensionAttributes(this SyndicationItemProperty itemProperty)
        {
            return itemProperty == SyndicationItemProperty.Title
                || itemProperty == SyndicationItemProperty.Summary
                || itemProperty == SyndicationItemProperty.Rights
                || itemProperty == SyndicationItemProperty.CustomProperty;
        }

        /// <summary>
        /// Returns the local attribute or element name for the given syndication item property
        /// </summary>
        /// <param name="itemProperty">the syndication item property</param>
        /// <returns>The local name for the attribute or element</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "need to use lower characters for xml element")]
        public static string ToElementOrAttributeName(this SyndicationItemProperty itemProperty)
        {
            return itemProperty.ToString().ToLowerInvariant()
                .Replace(ODataConstants.LinkElementName, string.Empty)
                .Replace(ODataConstants.CategoryElementName, string.Empty)
                .Replace(ODataConstants.AuthorElementName, string.Empty)
                .Replace(ODataConstants.ContributorElementName, string.Empty);
        }

        /// <summary>
        /// Returns the local attribute or element name for the given syndication item property
        /// </summary>
        /// <param name="itemProperty">the syndication item property</param>
        /// <returns>The local name for the attribute or element</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "need to use lower characters for xml element")]
        public static string ToParentElementName(this SyndicationItemProperty itemProperty)
        {
            string[] parentElements = new[] { ODataConstants.LinkElementName, ODataConstants.CategoryElementName, ODataConstants.AuthorElementName, ODataConstants.ContributorElementName };
            var stringValue = itemProperty.ToString().ToLowerInvariant();
            foreach (var parent in parentElements)
            {
                if (stringValue.StartsWith(parent, StringComparison.Ordinal))
                {
                    return parent;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the value of the attribute for the given text content kind
        /// </summary>
        /// <param name="textKind">the text content kind</param>
        /// <returns>The value of the attribute</returns>
        public static string ToAttributeString(this SyndicationTextContentKind textKind)
        {
            if (textKind == SyndicationTextContentKind.Plaintext)
            {
                return "text";
            }
            else if (textKind == SyndicationTextContentKind.Html)
            {
                return "html";
            }
            else
            {
                ExceptionUtilities.Assert(textKind == SyndicationTextContentKind.Xhtml, "Unrecognized syndication text content kind: '{0}'", textKind);
                return "xhtml";
            }
        }

        /// <summary>
        /// Returns the <see cref="SyndicationTextContentKind"/> value of the given attribute text value
        /// </summary>
        /// <param name="contentKindAttributeValue">the text content kind</param>
        /// <returns>The value of the attribute</returns>
        public static SyndicationTextContentKind FromTextContentKindAttributeString(string contentKindAttributeValue)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(contentKindAttributeValue, "contentKindAttributeValue");

            if (contentKindAttributeValue == "text")
            {
                return SyndicationTextContentKind.Plaintext;
            }
            else if (contentKindAttributeValue == "html")
            {
                return SyndicationTextContentKind.Html;
            }
            else
            {
                ExceptionUtilities.Assert(contentKindAttributeValue == "xhtml", "Unrecognized attribute value for syndication text content kind: '{0}'", contentKindAttributeValue);
                return SyndicationTextContentKind.Xhtml;
            }
        }
       
        /// <summary>
        /// Serializes the given payload element into a form ready to be sent over HTTP using the default encoding.
        /// </summary>
        /// <param name="serializer">The serializer to extend</param>
        /// <param name="rootElement">The root payload element to serialize</param>
        /// <returns>The binary representation of the payload for this format, ready to be sent over HTTP</returns>
        public static byte[] SerializeToBinary(this IPayloadSerializer serializer, ODataPayloadElement rootElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(serializer, "serializer");
            return serializer.SerializeToBinary(rootElement, null);
        }

        /// <summary>
        /// Deserializes the given HTTP payload into a rich representation according to the rules of this format using the default encoding.
        /// </summary>
        /// <param name="deserializer">The deserializer to extend</param>
        /// <param name="serialized">The binary that was sent over HTTP</param>
        /// <returns>The payload element</returns>
        public static ODataPayloadElement DeserializeFromBinary(this IPayloadDeserializer deserializer, byte[] serialized)
        {
            ExceptionUtilities.CheckArgumentNotNull(deserializer, "deserializer");
            return deserializer.DeserializeFromBinary(serialized, new ODataPayloadContext());
        }

        /// <summary>
        /// Specifies the equality function for the xml tree annotation
        /// </summary>
        /// <param name="xmlTree">The xml tree annotation</param>
        /// <param name="func">The equality function</param>
        /// <returns>The annotation with the equality function set</returns>
        public static XmlTreeAnnotation SetValueEqualityFunc(this XmlTreeAnnotation xmlTree, Func<object, object, bool> func)
        {
            ExceptionUtilities.CheckArgumentNotNull(func, "func");
            ExceptionUtilities.CheckArgumentNotNull(xmlTree, "xmlTree");
            xmlTree.ValueEqualityFunc = func;
            return xmlTree;
        }

        /// <summary>
        /// Deserializes the given HTTP payload a error payload using the default encoding or returns null
        /// </summary>
        /// <param name="deserializer">The deserializer to extend</param>
        /// <param name="serialized">Bytes of the Payload</param>
        /// <param name="errorPayload">Error payload that is found</param>
        /// <returns>True if it finds and error, false if not</returns>
        public static bool TryDeserializeErrorPayload(this IPayloadErrorDeserializer deserializer, byte[] serialized, out ODataPayloadElement errorPayload)
        {
            ExceptionUtilities.CheckArgumentNotNull(deserializer, "deserializer");
            return deserializer.TryDeserializeErrorPayload(serialized, null, out errorPayload);
        }

        /// <summary>
        /// Extension method for IPayloadTransformFactory which sets a composite transformed payload.
        /// </summary>
        /// <typeparam name="TPayload">Payload object type.</typeparam>
        /// <param name="factory">Payload transform factory.</param>
        /// <param name="payload">Original payload.</param>
        /// <param name="transformedPayload">Transformed payload.</param>
        /// <returns>True if the transform is applied else returns false.</returns>
        public static bool TryTransform<TPayload>(this IPayloadTransformFactory factory, TPayload payload, out TPayload transformedPayload)
        {
            ExceptionUtilities.CheckArgumentNotNull(factory, "factory");

            IPayloadTransform<TPayload> payloadElementTransform = factory.GetTransform<TPayload>();
            transformedPayload = default(TPayload);

            return payloadElementTransform.TryTransform(payload, out transformedPayload);
        }

        /// <summary>
        /// Converts the given value to a string according to the OData literal formatting rules
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="value">The value to serialize</param>
        /// <returns>
        /// The serialized value
        /// </returns>
        public static string SerializePrimitive(this IODataLiteralConverter converter, object value)
        {
            ExceptionUtilities.CheckArgumentNotNull(converter, "converter");
            return converter.SerializePrimitive(value, false);
        }

        /// <summary>
        /// Add the given annotation if it is not in the payload element.
        /// </summary>
        /// <typeparam name="TElement">type of annotation to add</typeparam>
        /// <param name="payload">payload element to add annotation</param>
        /// <param name="annotationToAdd">the annotation to add</param>
        public static void AddAnnotationIfNotExist<TElement>(this IAnnotatable<ODataPayloadElementAnnotation> payload, TElement annotationToAdd)
            where TElement : ODataPayloadElementEquatableAnnotation
        {
            payload.AddAnnotationIfNotExist<ODataPayloadElementAnnotation, ODataPayloadElementEquatableAnnotation, TElement>(annotationToAdd);
        }

        /// <summary>
        /// Gets the second to last or default.
        /// </summary>
        /// <typeparam name="T">Type of ODataUriSegment</typeparam>
        /// <param name="segments">The segments.</param>
        /// <returns>Second to last segment</returns>
        public static T GetSecondToLastOrDefault<T>(this IEnumerable<ODataUriSegment> segments) where T : ODataUriSegment
        {
            var segmentList = new List<ODataUriSegment>(segments);
            
            var secondToLastIndex = segmentList.Count - 2;
            if (secondToLastIndex > -1)
            {
                return segmentList[secondToLastIndex] as T;
            }

            return null;
        }
    }
}
