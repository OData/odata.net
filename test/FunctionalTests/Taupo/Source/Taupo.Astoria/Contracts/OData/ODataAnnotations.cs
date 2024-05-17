//---------------------------------------------------------------------
// <copyright file="ODataAnnotations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// A collection of extension methods for working with OData-specific annotations
    /// </summary>
    public static class ODataAnnotations
    {
        /// <summary>
        /// String which holds xml namespace declaration attributes for the standard Atom and Data Services namespaces.
        /// </summary>
        public const string StandardAtomNamespaceDeclarations = 
            "xmlns:" + ODataConstants.DataServicesNamespaceDefaultPrefix + "=\"" + ODataConstants.DataServicesNamespaceName + "\" " +
            "xmlns:" + ODataConstants.DataServicesMetadataNamespaceDefaultPrefix + "=\"" + ODataConstants.DataServicesMetadataNamespaceName + "\" " +
            "xmlns=\"" + ODataConstants.AtomNamespaceName + "\"";

        /// <summary>
        /// Adds an edit link annotation to the given entity instance
        /// </summary>
        /// <param name="instance">the entity instance</param>
        /// <param name="editLinkValue">The value of the edit link</param>
        /// <returns>The same entity instance that was given</returns>
        public static EntityInstance WithEditLink(this EntityInstance instance, string editLinkValue)
        {
            instance.EditLink = editLinkValue;
            return instance;
        }

        /// <summary>
        /// Adds an self link annotation to the given entity instance
        /// </summary>
        /// <param name="instance">the entity instance</param>
        /// <param name="selfLinkValue">The value of the self link</param>
        /// <returns>The same entity instance that was given</returns>
        public static EntityInstance WithSelfLink(this EntityInstance instance, string selfLinkValue)
        {
            instance.AddAnnotationIfNotExist(new SelfLinkAnnotation(selfLinkValue));
            return instance;
        }

        /// <summary>
        /// Returns the edit link annotation value of the entity, if one exists
        /// </summary>
        /// <param name="instance">The entity to get the edit link from</param>
        /// <returns>The edit link, or null if no annotation is found</returns>
        public static string GetEditLink(this EntityInstance instance)
        {
            return instance.EditLink;
        }

        /// <summary>
        /// Returns the self link annotation value of the entity, if one exists
        /// </summary>
        /// <param name="instance">The entity to get the self link from</param>
        /// <returns>The self link, or null if no annotation is found</returns>
        public static string GetSelfLink(this EntityInstance instance)
        {
            return instance.Annotations.OfType<SelfLinkAnnotation>().Select(a => a.Value).SingleOrDefault();
        }

        /// <summary>
        /// Adds a content type annotation to the given deferred link
        /// </summary>
        /// <param name="link">The deferred link</param>
        /// <param name="contentTypeValue">The value for the annotation</param>
        /// <returns>The same deferred link</returns>
        public static DeferredLink WithContentType(this DeferredLink link, string contentTypeValue)
        {
            return link.AddAnnotation(new ContentTypeAnnotation(contentTypeValue));
        }

        /// <summary>
        /// Adds a content type annotation to the given entity instance
        /// </summary>
        /// <param name="instance">The entity instance</param>
        /// <param name="contentTypeValue">The value for the annotation</param>
        /// <returns>The same entity instance</returns>
        public static EntityInstance WithContentType(this EntityInstance instance, string contentTypeValue)
        {
            return instance.AddAnnotation(new ContentTypeAnnotation(contentTypeValue));
        }

        /// <summary>
        /// Adds a title attribute annotation to the given payload
        /// </summary>
        /// <param name="instance">The payload</param>
        /// <param name="value">The value for the annotation</param>
        /// <typeparam name="TPayload">Type which can have a title annotation added</typeparam>
        /// <returns>The same payload</returns>
        public static TPayload WithTitleAttribute<TPayload>(this TPayload instance, string value) 
            where TPayload : ODataPayloadElement
        {
            return instance.AddAnnotation(new TitleAnnotation(value));
        }

        /// <summary>
        /// Returns the value to use for the entity's content element mime type annotation
        /// </summary>
        /// <param name="instance">The entity instance</param>
        /// <param name="defaultValue">The default value to return if no annotation is found</param>
        /// <returns>The value of the annotation, or the default if one does not exist</returns>
        public static string GetContentType(this EntityInstance instance, string defaultValue)
        {
            ContentTypeAnnotation annotation = instance.Annotations.OfType<ContentTypeAnnotation>().SingleOrDefault();
            if (annotation == null)
            {
                return defaultValue;
            }

            return annotation.Value;
        }

        /// <summary>
        /// Adds the media link entry annotation to the given entity instance
        /// </summary>
        /// <param name="instance">The entity instance</param>
        /// <returns>The same entity instance</returns>
        public static EntityInstance AsMediaLinkEntry(this EntityInstance instance)
        {
            return instance.AddAnnotation(new IsMediaLinkEntryAnnotation());
        }

        /// <summary>
        /// Returns whether or not the entity is marked with a media link entry annotation
        /// </summary>
        /// <param name="instance">The entity to check</param>
        /// <returns>Whether or not there is a media link entry annotation on the entity</returns>
        public static bool IsMediaLinkEntry(this EntityInstance instance)
        {
            return instance.StreamSourceLink != null || instance.Annotations.OfType<IsMediaLinkEntryAnnotation>().Any();
        }

        /// <summary>
        /// Adds an annotation to the deferred link to tell the xml serializer to use the metadata namespace instead of the data services namespace
        /// </summary>
        /// <param name="link">The link to add the annotation to</param>
        /// <returns>The annotated link</returns>
        public static DeferredLink UseMetadataNamespace(this DeferredLink link)
        {
            return link.AddAnnotation(new UseMetadataNamespaceAnnotation());
        }

        /// <summary>
        /// Gets the xml name to use for generating items in a collection based on the given element's annotations
        /// </summary>
        /// <param name="element">The element to get a name for</param>
        /// <returns>The xml name to use, or null if one cannot be determined</returns>
        public static XName GetCollectionItemElementName(this ODataPayloadElement element)
        {
            return element.Annotations.OfType<CollectionItemElementNameAnnotation>().Select(a => XName.Get(a.LocalName, a.NamespaceName)).SingleOrDefault();
        }

        /// <summary>
        /// Adds a collection item element name to the given element and returns it
        /// </summary>
        /// <typeparam name="TElement">The type of the element</typeparam>
        /// <param name="element">The element to annotate</param>
        /// <param name="localName">The local name to use when generating individual item's in the xml</param>
        /// <param name="namespaceName">The namespace name to use</param>
        /// <returns>The given element</returns>
        public static TElement WithCollectionItemElementName<TElement>(this TElement element, string localName, string namespaceName) where TElement : ODataPayloadElement
        {
            var existing = element.Annotations.OfType<CollectionItemElementNameAnnotation>().SingleOrDefault();
            if (existing == null)
            {
                existing = new CollectionItemElementNameAnnotation();
                element.Annotations.Add(existing);
            }

            existing.LocalName = localName;
            existing.NamespaceName = namespaceName;

            return element;
        }

        /// <summary>
        /// Adds an XML specific representation annotation to the specified ODataPayloadElement.
        /// </summary>
        /// <typeparam name="TElement">The type of the payload element to work on.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="xmlNodes">The XML nodes to annotate with.</param>
        /// <returns>The annotated payload element, for composability.</returns>
        public static TElement XmlRepresentation<TElement>(this TElement payloadElement, IEnumerable<XNode> xmlNodes) where TElement : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckArgumentNotNull(xmlNodes, "xmlNodes");

            var annotation = new XmlPayloadElementRepresentationAnnotation { XmlNodes = xmlNodes };

            payloadElement.SetAnnotation(annotation);
            return payloadElement;
        }

        /// <summary>
        /// Adds an XML specific representation annotation to the specified ODataPayloadElement.
        /// </summary>
        /// <typeparam name="TElement">The type of the payload element to work on.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="xml">The XML node to annotate with.</param>
        /// <returns>The annotated payload element, for composability.</returns>
        public static TElement XmlRepresentation<TElement>(this TElement payloadElement, XNode xml) where TElement : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckArgumentNotNull(xml, "xml");

            return payloadElement.XmlRepresentation(new[] { xml });
        }

        /// <summary>
        /// Adds an XML specific representation annotation to the specified ODataPayloadElement.
        /// </summary>
        /// <typeparam name="TElement">The type of the payload element to work on.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="xml">The XML string to annotate with. Null means annotate with no XML (an empty parent element).
        /// Empty string means annotate with a single XText with empty string (parent element will have no content, but it will have a full end element).</param>
        /// <returns>The annotated payload element, for composability.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "odata", Justification = "Literal forms part of xml namespace")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "dataservices", Justification = "Literal forms part of xml namespace")]
        public static TElement XmlRepresentation<TElement>(this TElement payloadElement, string xml) where TElement : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            if (xml == null)
            {
                return payloadElement.XmlRepresentation(new XNode[0]);
            }
            else if (xml.Length == 0)
            {
                return payloadElement.XmlRepresentation(new[] { new XText(string.Empty) });
            }
            else
            {
                string tempElementString = string.Format(CultureInfo.InvariantCulture, "<temp {0}>{1}</temp>", StandardAtomNamespaceDeclarations, xml);

                return payloadElement.XmlRepresentation(XElement.Parse(tempElementString, LoadOptions.PreserveWhitespace).Nodes());
            }
        }

        /// <summary>
        /// Adds an XML rewriting annotation to the specified ODataPayloadElement
        /// </summary>
        /// <typeparam name="TElement">The type of the payload element to work on.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="rewriteFunction">The function that will be applied to the XML element that results from serializing the payload element</param>
        /// <returns>The annotated payload element, for composability.</returns>
        /// <returns></returns>
        public static TElement WithXmlRewriteFunction<TElement>(this TElement payloadElement, Func<XElement, XNode> rewriteFunction) where TElement : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var annotation = new XmlRewriteElementAnnotation { RewriteFunction = rewriteFunction };
            payloadElement.SetAnnotation(annotation);
            return payloadElement;
        }

        /// <summary>
        /// Adds a text representation annotation to the payload element and returns it.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="element">The element.</param>
        /// <param name="text">The text.</param>
        /// <returns>
        /// The element with the annotation added.
        /// </returns>
        public static TElement WithTextRepresentation<TElement>(this TElement element, string text) where TElement : ODataPayloadElement
        {
            var annotation = new RawTextPayloadElementRepresentationAnnotation() { Text = text };
            element.SetAnnotation(annotation);
            return element;
        }

        /// <summary>
        /// Adds an annotation to the specified ODataPayloadElement
        /// </summary>
        /// <typeparam name="TElement">The type of the payload element to work on.</typeparam>
        /// <param name="element">The payload element to annotate</param>
        /// <param name="annotation">The annotation</param>
        /// <returns>The annotated payload element, for composability</returns>
        private static TElement AddAnnotation<TElement>(this TElement element, ODataPayloadElementAnnotation annotation) where TElement : ODataPayloadElement
        {
            element.Annotations.Add(annotation);
            return element;
        }
    }
}
