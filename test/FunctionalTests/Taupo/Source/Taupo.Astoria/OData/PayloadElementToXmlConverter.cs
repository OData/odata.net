//---------------------------------------------------------------------
// <copyright file="PayloadElementToXmlConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;
    
    /// <summary>
    /// The converter from a reach payload elemnt representation to the atom/xml representation.
    /// </summary>
    [ImplementationName(typeof(IPayloadElementToXmlConverter), "Default", HelpText = "The default converter from a reach payload elemnt representation to the atom/xml representation.")]
    public class PayloadElementToXmlConverter : ODataPayloadElementVisitorBase, IPayloadElementToXmlConverter
    {
        private const string DataServicesNamespacePrefix = "d";
        private const string DataServicesMetadataNamespacePrefix = "m";
        private const string DataServicesNamespace = ODataConstants.DataServicesNamespaceName;
        private const string DataServicesMetadataNamespace = ODataConstants.DataServicesMetadataNamespaceName;
        private const string DataServicesRelatedNamespace = ODataConstants.DataServicesRelatedNamespaceName;
        private const string DataServicesRelatedLinksNamespace = ODataConstants.DataServicesRelatedLinksNamespaceName;
        private const string DataServicesSchemeNamespace = ODataConstants.DataServicesSchemeNamespaceName;
        private const string AtomNamespace = ODataConstants.AtomNamespaceName;
        private const string AtomPubNamespace = ODataConstants.AtomPubNamespaceName;
        private const string AtomPubNamespacePrefix = ODataConstants.AtomPubNamespacePrefix;
        private const string AtomValueElement = ODataConstants.ValueElementName;

        private static readonly ODataPayloadElementType[] supportedElementTypes = new[]
        {
            ODataPayloadElementType.EntityInstance,
            ODataPayloadElementType.PrimitiveProperty,
            ODataPayloadElementType.ComplexInstanceCollection,
            ODataPayloadElementType.PrimitiveCollection,
            ODataPayloadElementType.ComplexProperty,
            ODataPayloadElementType.DeferredLink,
            ODataPayloadElementType.LinkCollection,
            ODataPayloadElementType.EntitySetInstance,
            ODataPayloadElementType.ComplexMultiValueProperty,
            ODataPayloadElementType.PrimitiveMultiValueProperty,
            ODataPayloadElementType.NullPropertyInstance,
            ODataPayloadElementType.ServiceDocumentInstance,
            ODataPayloadElementType.ODataErrorPayload,
        };

        // Maintains the current scope of operation ie the element being populated
        private XElement currentXElement;

        // Maintains the payload elements currently being visited
        private Stack<ODataPayloadElement> payloadStack = new Stack<ODataPayloadElement>();

        // Set of namespace attributes to apply to the resultant XElement
        private XAttribute[] namespaceAttributes;

        /// <summary>
        /// Gets or sets the primitive converter to use while serializing
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IXmlPrimitiveConverter PrimitiveConverter { get; set; }

        /// <summary>
        /// Gets or sets the spatial formatter to use while serializing
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IGmlSpatialFormatter SpatialFormatter { get; set; }

        /// <summary>
        /// Gets or sets the payload transform factory
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IPayloadTransformFactory TransformFactory { get; set; }

        /// <summary>
        /// Converts the given payload element into an XML representation.
        /// </summary>
        /// <param name="rootElement">The root payload element to convert.</param>
        /// <returns>The XML representation of the payload.</returns>
        public XElement ConvertToXml(ODataPayloadElement rootElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            AssertIsSupportedElementType(rootElement.ElementType);

            ExceptionUtilities.Assert(this.payloadStack.Count == 0, "Payload element stack is not empty");

            // Default set of namespace attributes to apply
            this.namespaceAttributes = new XAttribute[]
                {
                    // xmlns:d="http://docs.oasis-open.org/odata/ns/data"
                    new XAttribute(XNamespace.Xmlns.GetName(DataServicesNamespacePrefix), DataServicesNamespace),

                    // xmlns:m="http://docs.oasis-open.org/odata/ns/metadata"
                    new XAttribute(XNamespace.Xmlns.GetName(DataServicesMetadataNamespacePrefix), DataServicesMetadataNamespace),

                    // ATOM namespace
                    new XAttribute("xmlns", AtomNamespace),
                };
            
            var tempElement = new XElement("Temporary");

            this.VisitPayloadElement(rootElement, tempElement);

            XElement xmlElement = tempElement.Elements().Single();

            // If XmlRepresentationAnnotation has been used, then these attributes may already be present
            xmlElement.Add(this.namespaceAttributes.Where(a => xmlElement.Attribute(a.Name) == null).ToArray());

            XElement transformedXMLPayload = null;
            if (this.TransformFactory.TryTransform<XElement>(xmlElement, out transformedXMLPayload))
            {
                xmlElement = transformedXMLPayload;
            }

            return xmlElement;
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            this.AttachValueAttributes(this.currentXElement, payloadElement);

            if (!payloadElement.IsNull)
            {
                foreach (PropertyInstance property in payloadElement.Properties)
                {
                    this.VisitPayloadElement(property);
                }
            }

            PostProcessXElement(payloadElement, this.currentXElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexInstanceCollection payloadElement)
        {
            this.SerializeTypedValueCollection(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexMultiValueProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.SerializeMultiValueProperty<ComplexMultiValue, ComplexInstance>(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            // when name is null, it means that it is in the top level. We should use <m:value
            XElement complexXElement = payloadElement.Name == null ? CreateMetadataElement(this.currentXElement, AtomValueElement) : CreateDataServicesElement(this.currentXElement, payloadElement.Name);
            this.VisitPayloadElement(payloadElement.Value, complexXElement);

            PostProcessXElement(payloadElement, complexXElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(DeferredLink payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            XElement linkElement;

            var navPropertyInstance = this.payloadStack.OfType<NavigationPropertyInstance>().FirstOrDefault();
            if (navPropertyInstance == null)
            {
                linkElement = CreateMetadataElement(this.currentXElement, "ref");
                CreateAtomAttribute(linkElement, "id", payloadElement.UriString ?? string.Empty);
            }
            else
            {
                string relAttributeValue;
                if (navPropertyInstance.AssociationLink == payloadElement)
                {
                    relAttributeValue = DataServicesRelatedLinksNamespace + navPropertyInstance.Name;
                }
                else
                {
                    relAttributeValue = DataServicesRelatedNamespace + navPropertyInstance.Name;
                }

                string contentType = payloadElement.Annotations.OfType<ContentTypeAnnotation>().Select(a => a.Value).SingleOrDefault();
                string title = payloadElement.Annotations.OfType<TitleAnnotation>().Select(a => a.Value).SingleOrDefault();

                linkElement = CreateAtomLinkElement(this.currentXElement, relAttributeValue, payloadElement.UriString, contentType, null, title);
            }

            PostProcessXElement(payloadElement, linkElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Unavoidable due to number of potential properties.")]
        public override void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            if (payloadElement.IsNull)
            {
                // Allow null entity as value of expanded navigation properties
                if (this.payloadStack.Count > 1 && this.payloadStack.ElementAt(1).ElementType == ODataPayloadElementType.ExpandedLink)
                {
                    return;
                }

                throw new TaupoInvalidOperationException("Null entities cannot be represented in xml");
            }

            XElement entryElement = CreateAtomElement(this.currentXElement, "entry");

            if (payloadElement.ETag != null)
            {
                CreateMetadataAttribute(entryElement, "etag", payloadElement.ETag);
            }

            if (payloadElement.FullTypeName != null)
            {
                // add type attribute
                XElement typeElement = CreateAtomElement(entryElement, "category");
                typeElement.Add(new XAttribute("scheme", DataServicesSchemeNamespace));
                typeElement.Add(new XAttribute("term", payloadElement.FullTypeName));
            }

            if (payloadElement.Id != null)
            {
                XElement idNode = CreateAtomElement(entryElement, "id");
                idNode.Value = payloadElement.Id;
            }

            foreach (NavigationPropertyInstance navprop in payloadElement.Properties.OfType<NavigationPropertyInstance>())
            {
                this.VisitPayloadElement(navprop, entryElement);
            }

            foreach (var namedStreamInstance in payloadElement.Properties.OfType<NamedStreamInstance>())
            {
                this.VisitPayloadElement(namedStreamInstance, entryElement);
            }

            var selfLinkAnnotation = payloadElement.Annotations.OfType<SelfLinkAnnotation>().SingleOrDefault();
            if (selfLinkAnnotation != null)
            {
                CreateAtomLinkElement(entryElement, "self", selfLinkAnnotation.Value);
            }

            if (payloadElement.EditLink != null)
            {
                CreateAtomLinkElement(entryElement, "edit", payloadElement.EditLink);
            }

            foreach (var actionDescriptor in payloadElement.ServiceOperationDescriptors)
            {
                this.CreateServiceOperationDescriptor(entryElement, actionDescriptor);
            }

            XElement propertyElementParent = entryElement;
            if (payloadElement.IsMediaLinkEntry())
            {
                // Serializing Media Link Entry :
                //  - if stream edit link is defined, create it first (this is not a requirement, just keeps
                //    it organized with any prior link elements)
                //  - if stream content type and/or source link defined, create content element and write attributes
                //  - property element is created as child of entry element (NOT as child of content)
                if (payloadElement.StreamEditLink != null)
                {
                    CreateAtomLinkElement(entryElement, "edit-media", payloadElement.StreamEditLink, null, payloadElement.StreamETag);
                }

                var contentAttributes = new List<XAttribute>();
                if (payloadElement.StreamContentType != null)
                {
                    contentAttributes.Add(new XAttribute("type", payloadElement.StreamContentType));
                }

                if (payloadElement.StreamSourceLink != null)
                {
                    contentAttributes.Add(new XAttribute("src", payloadElement.StreamSourceLink));
                }

                if (contentAttributes.Any())
                {
                    XElement contentNode = CreateAtomElement(entryElement, "content");
                    contentNode.Add(contentAttributes.ToArray());
                }
            }
            else if (payloadElement.Properties.Any() || payloadElement.Annotations.OfType<ContentTypeAnnotation>().Any())
            {
                // Only create content element if there are any properties present or if the content type annotation
                // is present
                XElement contentNode = CreateAtomElement(entryElement, "content");
                string contentType = payloadElement.GetContentType(MimeTypes.ApplicationXml);
                if (contentType != null)
                {
                    CreateAtomAttribute(contentNode, "type", contentType);
                }

                propertyElementParent = contentNode;
            }

            if (payloadElement.Properties.Any())
            {
                XElement propertyElement = CreateMetadataElement(propertyElementParent, "properties");

                // named streams and navigation properties were already serialized
                var unserializedProperties = payloadElement.Properties
                    .Where(p => p.ElementType != ODataPayloadElementType.NavigationPropertyInstance && p.ElementType != ODataPayloadElementType.NamedStreamInstance);

                foreach (PropertyInstance property in unserializedProperties)
                {
                    this.VisitPayloadElement(property, propertyElement);
                }
            }

            PostProcessXElement(payloadElement, entryElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            XElement feed = CreateAtomElement(this.currentXElement, "feed");

            if (payloadElement.InlineCount.HasValue)
            {
                XElement countElement = CreateMetadataElement(feed, "count");
                countElement.Value = payloadElement.InlineCount.Value.ToString(CultureInfo.InvariantCulture);
            }
            
            foreach (EntityInstance entity in payloadElement)
            {
                this.VisitPayloadElement(entity, feed);
            }

            if (payloadElement.NextLink != null)
            {
                CreateAtomLinkElement(feed, "next", payloadElement.NextLink);
            }

            PostProcessXElement(payloadElement, feed);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ExpandedLink payloadElement)
        {
            var navPropertyInstance = this.payloadStack.OfType<NavigationPropertyInstance>().FirstOrDefault();
            ExceptionUtilities.CheckObjectNotNull(navPropertyInstance, "No NavigationPropertyInstance found in stack for ExpandedLink");

            string contentType = null;
            var contentTypeAnnotation = payloadElement.Annotations.OfType<ContentTypeAnnotation>().SingleOrDefault();
            if (contentTypeAnnotation != null)
            {
                contentType = contentTypeAnnotation.Value;
            }
            else if (payloadElement.ExpandedElement != null)
            {
                contentType = MimeTypes.ApplicationAtomXml + ";type=" + (payloadElement.IsSingleEntity ? "entry" : "feed");
            }

            XElement linkElement = CreateAtomLinkElement(
                this.currentXElement, 
                DataServicesRelatedNamespace + navPropertyInstance.Name, 
                payloadElement.UriString,
                contentType);

            XElement inlineElement = CreateMetadataElement(linkElement, "inline");

            if (payloadElement.ExpandedElement != null)
            {
                this.VisitPayloadElement(payloadElement.ExpandedElement, inlineElement);
            }

            PostProcessXElement(payloadElement, linkElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(LinkCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            XElement feed = CreateAtomElement(this.currentXElement, "feed");

            if (payloadElement.InlineCount.HasValue)
            {
                XElement countElement = CreateMetadataElement(feed, "count");
                countElement.Value = payloadElement.InlineCount.Value.ToString(CultureInfo.InvariantCulture);
            }

            foreach (Link link in payloadElement)
            {
                this.VisitPayloadElement(link, feed);
            }

            if (payloadElement.NextLink != null)
            {
                XElement nextLink = CreateElement(feed, null, "next", ODataConstants.DataServicesNamespaceName);
                nextLink.Value = payloadElement.NextLink;
            }

            PostProcessXElement(payloadElement, feed);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(NamedStreamInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            if (payloadElement.SourceLink != null)
            {
                XElement sourceLinkElement = CreateAtomLinkElement(
                    this.currentXElement,
                    ODataConstants.DataServicesMediaResourceNamespaceName + payloadElement.Name,
                    payloadElement.SourceLink,
                    payloadElement.SourceLinkContentType);

                PostProcessXElement(payloadElement, sourceLinkElement);
            }
            
            if (payloadElement.EditLink != null)
            {
                XElement editLinkElement = CreateAtomLinkElement(
                    this.currentXElement,
                    ODataConstants.DataServicesMediaResourceEditNamespaceName + payloadElement.Name,
                    payloadElement.EditLink,
                    payloadElement.EditLinkContentType,
                    payloadElement.ETag);

                PostProcessXElement(payloadElement, editLinkElement);
            }
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(NavigationPropertyInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");
            ExceptionUtilities.Assert(!payloadElement.Annotations.OfType<XmlRewriteElementAnnotation>().Any(), "Xml rewrite is not supported on Nav Prop instances");

            if (payloadElement.Value != null)
            {
                ExceptionUtilities.Assert(
                    payloadElement.Value.ElementType == ODataPayloadElementType.DeferredLink
                    || payloadElement.Value.ElementType == ODataPayloadElementType.LinkCollection
                    || payloadElement.Value.ElementType == ODataPayloadElementType.ExpandedLink,
                    "Navigation property value is invalid");

                this.VisitPayloadElement(payloadElement.Value);
            }
            else if (payloadElement.AssociationLink == null)
            {
                // special case: navigation properties with null values are represented with empty href attributes
                this.VisitPayloadElement(new DeferredLink());
            }

            if (payloadElement.AssociationLink != null)
            {
                this.VisitPayloadElement(payloadElement.AssociationLink);
            }
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(NullPropertyInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            XElement propertyElement = CreateDataServicesElement(this.currentXElement, payloadElement.Name);

            if (payloadElement.FullTypeName != null)
            {
                CreateMetadataAttribute(propertyElement, "type", payloadElement.FullTypeName);
            }

            CreateMetadataAttribute(propertyElement, "null", "true");

            PostProcessXElement(payloadElement, propertyElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ODataErrorPayload payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            XElement errorElement = CreateMetadataElement(this.currentXElement, ODataConstants.ErrorElementName);

            string code = payloadElement.Code;
            if (code != null)
            {
                XElement codeElement = CreateMetadataElement(errorElement, ODataConstants.CodeElementName);
                codeElement.Add(code);
            }

            string message = payloadElement.Message;
            if (message != null)
            {
                XElement messageElement = CreateMetadataElement(errorElement, ODataConstants.MessageElementName);
                messageElement.Add(message);
            }

            ODataInternalExceptionPayload innerError = payloadElement.InnerError;
            if (innerError != null)
            {
                XElement innerErrorElement = CreateMetadataElement(errorElement, ODataConstants.InnerErrorElementName);
                this.VisitPayloadElement(innerError, innerErrorElement);
            }

            PostProcessXElement(payloadElement, errorElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ODataInternalExceptionPayload payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            string innerMessage = payloadElement.Message;
            if (innerMessage != null)
            {
                XElement innerMessageElement = CreateMetadataElement(this.currentXElement, ODataConstants.MessageElementName);
                innerMessageElement.Add(innerMessage);
            }

            string innerTypeName = payloadElement.TypeName;
            if (innerTypeName != null)
            {
                XElement innerTypeNameElement = CreateMetadataElement(this.currentXElement, ODataConstants.TypeNameElementName);
                innerTypeNameElement.Add(innerTypeName);
            }

            string stacktrace = payloadElement.StackTrace;
            if (stacktrace != null)
            {
                XElement stackTraceElement = CreateMetadataElement(this.currentXElement, ODataConstants.StackTraceElementName);
                stackTraceElement.Add(stacktrace);
            }

            ODataInternalExceptionPayload internalException = payloadElement.InternalException;
            if (internalException != null)
            {
                XElement internalExceptionElement = CreateMetadataElement(this.currentXElement, ODataConstants.InternalExceptionElementName);
                this.VisitPayloadElement(internalException, internalExceptionElement);
            }

            PostProcessXElement(payloadElement, this.currentXElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(PrimitiveCollection payloadElement)
        {
            this.SerializeTypedValueCollection(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(PrimitiveMultiValueProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.SerializeMultiValueProperty<PrimitiveMultiValue, PrimitiveValue>(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(PrimitiveProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            // when name is null, it means that it is in the top level. We should use <m:value
            XElement propertyElement = payloadElement.Name == null ? CreateMetadataElement(this.currentXElement, AtomValueElement) : CreateDataServicesElement(this.currentXElement, payloadElement.Name);
            this.VisitPayloadElement(payloadElement.Value, propertyElement);
            PostProcessXElement(payloadElement, propertyElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(PrimitiveValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            this.AttachValueAttributes(this.currentXElement, payloadElement);

            if (!payloadElement.IsNull)
            {
                XElement spatialElement;
                if (this.SpatialFormatter.TryConvert(payloadElement.ClrValue, out spatialElement))
                {
                    this.currentXElement.Add(spatialElement);
                }
                else
                {
                    var serializedValue = this.PrimitiveConverter.SerializePrimitive(payloadElement.ClrValue);
                    if (serializedValue.Trim() != serializedValue)
                    {
                        this.currentXElement.Add(new XAttribute(XNamespace.Xml.GetName("space"), "preserve"));
                    }

                    this.currentXElement.Value = serializedValue;
                }
            }

            PostProcessXElement(payloadElement, this.currentXElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ResourceCollectionInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            XElement collectionElement = CreateAtomPubElement(this.currentXElement, "collection");

            if (payloadElement.Href != null)
            {
                collectionElement.SetAttributeValue("href", payloadElement.Href);
            }

            if (payloadElement.Title != null)
            {
                XElement title = CreateAtomElement(collectionElement, "title");
                title.Value = payloadElement.Title;
            }

            PostProcessXElement(payloadElement, collectionElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ServiceDocumentInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            // Override the namespace attributes for this element
            this.namespaceAttributes = new XAttribute[]
                {
                    // xmlns="http://www.w3.org/2007/app"
                    new XAttribute("xmlns", AtomPubNamespace),

                    // ATOM namespace
                    new XAttribute(XNamespace.Xmlns.GetName("atom"), AtomNamespace),
                };

            XElement serviceDocument = CreateAtomPubElement(this.currentXElement, "service");

            foreach (WorkspaceInstance workspaceInstance in payloadElement.Workspaces)
            {
                this.VisitPayloadElement(workspaceInstance, serviceDocument);
            }

            PostProcessXElement(payloadElement, serviceDocument);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(WorkspaceInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            XElement workspaceElement = CreateAtomPubElement(this.currentXElement, "workspace");

            if (payloadElement.Title != null)
            {
                XElement title = CreateAtomElement(workspaceElement, "title");
                title.Value = payloadElement.Title;
            }

            foreach (ResourceCollectionInstance collection in payloadElement.ResourceCollections)
            {
                this.VisitPayloadElement(collection, workspaceElement);
            }

            PostProcessXElement(payloadElement, workspaceElement);
        }

        private static XAttribute CreateAttribute(XElement parent, string localName, XNamespace xmlNamespace, object value)
        {
            XName name = xmlNamespace != null ? xmlNamespace.GetName(localName) : localName;

            XAttribute attribute = new XAttribute(name, value);
            parent.Add(attribute);
            return attribute;
        }

        private static XElement CreateElement(XElement parent, string prefix, string localName, string namespaceUri)
        {
            XElement element = new XElement(XNamespace.Get(namespaceUri).GetName(localName));
            if (!string.IsNullOrEmpty(prefix))
            {
                XName prefixNamespace = XNamespace.Xmlns.GetName(prefix);

                XAttribute attr = new XAttribute(prefixNamespace, namespaceUri);
                element.Add(attr);
            }

            parent.Add(element);
            return element;
        }

        private static XAttribute CreateAtomAttribute(XElement parent, string name, object value)
        {
            return CreateAttribute(parent, name, null, value);
        }

        private static XElement CreateAtomElement(XElement parent, string name)
        {
            return CreateElement(parent, null, name, AtomNamespace);
        }

        private static XElement CreateAtomPubElement(XElement parent, string name)
        {
            return CreateElement(parent, null, name, AtomPubNamespace);
        }

        private static XAttribute CreateMetadataAttribute(XElement parent, string name, object value)
        {
            return CreateAttribute(parent, name, DataServicesMetadataNamespace, value);
        }

        private static XElement CreateDataServicesElement(XElement parent, string name)
        {
            return CreateElement(parent, null, name, DataServicesNamespace);
        }

        private static XElement CreateMetadataElement(XElement parent, string name)
        {
            return CreateElement(parent, null, name, DataServicesMetadataNamespace);
        }

        private static XElement CreateAtomLinkElement(XElement parent, string relation, string href = null, string contentType = null, string etag = null, string title = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(parent, "parent");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(relation, "relation");

            XElement linkElement = CreateAtomElement(parent, "link");
            CreateAtomAttribute(linkElement, "rel", relation);

            if (etag != null)
            {
                CreateMetadataAttribute(linkElement, "etag", etag);
            }

            if (contentType != null)
            {
                CreateAtomAttribute(linkElement, "type", contentType);
            }

            if (title != null)
            {
                CreateAtomAttribute(linkElement, "title", title);
            }

            if (href != null)
            {
                CreateAtomAttribute(linkElement, "href", href);
            }

            return linkElement;
        }

        private static void AssertIsSupportedElementType(ODataPayloadElementType elementType)
        {
            ExceptionUtilities.Assert(supportedElementTypes.Contains(elementType), "Unsupported element type: '{0}'", elementType);
        }

        private static void PostProcessXElement(ODataPayloadElement payloadElement, XElement xmlElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckArgumentNotNull(xmlElement, "xmlElement");

            foreach (var xmlTree in payloadElement.Annotations.OfType<XmlTreeAnnotation>())
            {
                SerializeXmlTree(xmlTree, xmlElement);
            }

            var rewriteAnnotation = payloadElement.Annotations.OfType<XmlRewriteElementAnnotation>().SingleOrDefault();
            if (rewriteAnnotation != null && rewriteAnnotation.RewriteFunction != null)
            {
                xmlElement.ReplaceWith(rewriteAnnotation.RewriteFunction(xmlElement));
            }
        }

        private static void SerializeXmlTree(XmlTreeAnnotation xmlTree, XElement parent)
        {
            ExceptionUtilities.CheckArgumentNotNull(xmlTree, "xmlTree");

            XNamespace targetNamespace = XNamespace.Get(xmlTree.NamespaceName);

            if (xmlTree.IsAttribute)
            {
                CreateAttribute(parent, xmlTree.LocalName, xmlTree.NamespaceName, xmlTree.PropertyValue);
            }
            else
            {
                XElement element = parent.Element(targetNamespace.GetName(xmlTree.LocalName));

                // create new element for repeatable elements
                if (element == null || IsRepeatableElement(element))
                {
                    element = CreateElement(parent, xmlTree.NamespacePrefix, xmlTree.LocalName, xmlTree.NamespaceName);
                }

                foreach (var child in xmlTree.Children)
                {
                    SerializeXmlTree(child, element);
                }

                if (xmlTree.PropertyValue != null)
                {
                    element.Value = xmlTree.PropertyValue;
                }
            }
        }

        private static bool IsRepeatableElement(XElement element)
        {
            ExceptionUtilities.CheckArgumentNotNull(element, "element");

            return element.Name.LocalName.Equals(ODataConstants.AuthorElementName) || element.Name.LocalName.Equals(ODataConstants.ContributorElementName)
                    || element.Name.LocalName.Equals(ODataConstants.LinkElementName) || element.Name.LocalName.Equals(ODataConstants.ContributorElementName)
                    || element.Name.LocalName.Equals(ODataConstants.CategoryElementName);
        }

        private void CreateServiceOperationDescriptor(XElement entryElement, ServiceOperationDescriptor serviceOperationPayloadAnnotation)
        {
            string elementName = "action";
            if (serviceOperationPayloadAnnotation.IsFunction)
            {
                elementName = "function";
            }

            XElement actionNode = CreateMetadataElement(entryElement, elementName);
            actionNode.Add(new XAttribute("metadata", serviceOperationPayloadAnnotation.Metadata));

            if (serviceOperationPayloadAnnotation.Title != null)
            {
                actionNode.Add(new XAttribute("title", serviceOperationPayloadAnnotation.Title));
            }

            if (serviceOperationPayloadAnnotation.Target != null)
            {
                actionNode.Add(new XAttribute("target", serviceOperationPayloadAnnotation.Target));
            }
        }

        private void VisitPayloadElement(ODataPayloadElement payloadElement, XElement newCurrentElement)
        {
            // Visit the payload within the scope of the new element
            XElement originalElement = this.currentXElement;
            this.currentXElement = newCurrentElement;

            try
            {
                this.VisitPayloadElement(payloadElement);
            }
            finally
            {
                // restore scope to the original element
                this.currentXElement = originalElement;
            }
        }

        private void VisitPayloadElement(ODataPayloadElement payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            if (this.TryAttachXmlRepresentation(payloadElement))
            {
                return;
            }

            this.payloadStack.Push(payloadElement);

            try
            {
                payloadElement.Accept(this);
            }
            finally
            {
                this.payloadStack.Pop();
            }
        }

        private void SerializeMultiValueProperty<TMultiValue, TElement>(TypedProperty<TMultiValue> multiValueProperty)
            where TMultiValue : TypedValueCollection<TElement>, new()
            where TElement : ODataPayloadElement, ITypedValue
        {
            ExceptionUtilities.CheckArgumentNotNull(multiValueProperty, "multiValueProperty");
            ExceptionUtilities.CheckArgumentNotNull(multiValueProperty.Value, "multiValueProperty.Value");

            // when name is null, it means that it is in the top level. We should use <m:value
            XElement propertyElement = multiValueProperty.Name == null ? CreateMetadataElement(this.currentXElement, AtomValueElement) : CreateDataServicesElement(this.currentXElement, multiValueProperty.Name);

            this.AttachValueAttributes(propertyElement, multiValueProperty.Value);

            if (!multiValueProperty.Value.IsNull)
            {
                var collectionElementName = multiValueProperty.Value.GetCollectionItemElementName();
                if (collectionElementName == null)
                {
                    collectionElementName = XName.Get("element", DataServicesMetadataNamespace);
                }

                foreach (var element in multiValueProperty.Value)
                {
                    var elementName = element.GetCollectionItemElementName();
                    if (elementName == null)
                    {
                        elementName = collectionElementName;
                    }

                    var primitiveValue = element as PrimitiveValue;
                    var elementXml = CreateElement(propertyElement, DataServicesMetadataNamespacePrefix, elementName.LocalName, elementName.NamespaceName);
                    if (primitiveValue != null)
                    {
                        this.VisitPayloadElement(primitiveValue, elementXml);
                    }
                    else
                    {
                        ExceptionUtilities.Assert(typeof(TElement) == typeof(ComplexInstance), "Unsupported collection element type: {0}", typeof(TElement));
                        this.VisitPayloadElement(element, elementXml);
                    }
                }
            }

            PostProcessXElement(multiValueProperty.Value, propertyElement);
            PostProcessXElement(multiValueProperty, propertyElement);
        }

        private void SerializeTypedValueCollection<T>(ODataPayloadElementCollection<T> payloadElement) where T : TypedValue
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckObjectNotNull(this.currentXElement, "Current XElement is not defined");

            var collectionNameAnnotation = payloadElement.Annotations.OfType<CollectionNameAnnotation>().SingleOrDefault();

            XElement collectionElement = (collectionNameAnnotation == null || collectionNameAnnotation.Name == null) ? CreateMetadataElement(this.currentXElement, AtomValueElement) : CreateDataServicesElement(this.currentXElement, collectionNameAnnotation.Name);

            foreach (var typedValue in payloadElement)
            {
                XElement valueElement = CreateMetadataElement(collectionElement, "element");
                this.VisitPayloadElement(typedValue, valueElement);
            }

            PostProcessXElement(payloadElement, collectionElement);
        }

        private bool TryAttachXmlRepresentation(ODataPayloadElement payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var annotation = payloadElement.Annotations.OfType<XmlPayloadElementRepresentationAnnotation>().SingleOrDefault();
            if (annotation != null)
            {
                this.currentXElement.Add(annotation.XmlNodes);
                return true;
            }

            return false;
        }

        private void AttachValueAttributes(XElement element, ITypedValue typedValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(element, "element");
            ExceptionUtilities.CheckArgumentNotNull(typedValue, "typedValue");

            if (typedValue.FullTypeName != null)
            {
                CreateMetadataAttribute(element, ODataConstants.TypeAttributeName, typedValue.FullTypeName);
            }

            if (typedValue.IsNull)
            {
                CreateMetadataAttribute(element, ODataConstants.NullAttributeName, "true");
            }
        }
    }
}
