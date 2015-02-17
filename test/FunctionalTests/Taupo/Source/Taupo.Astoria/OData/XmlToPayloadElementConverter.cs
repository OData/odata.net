//---------------------------------------------------------------------
// <copyright file="XmlToPayloadElementConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// The converter from an atom/xml into a rich payload element representation.
    /// </summary>
    [ImplementationName(typeof(IXmlToPayloadElementConverter), "Default", HelpText = "The default converter an atom/xml to rich payload element representation.")]
    public class XmlToPayloadElementConverter : IXmlToPayloadElementConverter
    {
        private const string RelNext = "next";
        private const string RelEdit = "edit";
        private const string RelSrc = "src";
        private const string RelSelf = "self";
        private const string RelEditMedia = "edit-media";
        private const string EdmxElementName = "Edmx";

        private static readonly XName AtomFeed = ODataConstants.AtomNamespace.GetName("feed");
        private static readonly XName AtomEntry = ODataConstants.AtomNamespace.GetName("entry");
        private static readonly XName AtomLink = ODataConstants.AtomNamespace.GetName("link");
        private static readonly XName AtomId = ODataConstants.AtomNamespace.GetName("id");
        private static readonly XName AtomCategory = ODataConstants.AtomNamespace.GetName("category");
        private static readonly XName AtomContent = ODataConstants.AtomNamespace.GetName("content");
        private static readonly XName AtomTitle = ODataConstants.AtomNamespace.GetName(ODataConstants.TitleElementName);
        private static readonly XName AtomSummary = ODataConstants.AtomNamespace.GetName("summary");
        private static readonly XName AtomRights = ODataConstants.AtomNamespace.GetName("rights");
        private static readonly XName AtomPublished = ODataConstants.AtomNamespace.GetName("published");
        private static readonly XName AtomUpdated = ODataConstants.AtomNamespace.GetName(ODataConstants.UpdatedElementName);
        private static readonly XName AtomAuthor = ODataConstants.AtomNamespace.GetName(ODataConstants.AuthorElementName);
        private static readonly XName AtomContributor = ODataConstants.AtomNamespace.GetName(ODataConstants.ContributorElementName);

        private static readonly XName Rel = XName.Get("rel");
        private static readonly XName Href = XName.Get("href");
        private static readonly XName Term = XName.Get("term");
        private static readonly XName Type = XName.Get("type");
        private static readonly XName Src = XName.Get("src");
        private static readonly XName XNameTitle = XName.Get("title");
        private static readonly XName XNameTarget = XName.Get("target");
        private static readonly XName XNameMetadata = XName.Get("metadata");

        private static readonly XName DataServicesLinks = ODataConstants.DataServicesNamespace.GetName("ref");
        private static readonly XName DataServicesUri = ODataConstants.DataServicesNamespace.GetName("uri");
        private static readonly XName DataServicesElement = ODataConstants.DataServicesMetadataNamespace.GetName(ODataConstants.CollectionItemElementName);
        private static readonly XName DataServicesNext = ODataConstants.DataServicesNamespace.GetName("next");

        private static readonly XName MetadataUri = ODataConstants.DataServicesMetadataNamespace.GetName("uri");
        private static readonly XName MetadataType = ODataConstants.DataServicesMetadataNamespace.GetName("type");
        private static readonly XName MetadataCount = ODataConstants.DataServicesMetadataNamespace.GetName("count");
        private static readonly XName MetadataETag = ODataConstants.DataServicesMetadataNamespace.GetName("etag");
        private static readonly XName MetadataProperties = ODataConstants.DataServicesMetadataNamespace.GetName("properties");
        private static readonly XName MetadataInline = ODataConstants.DataServicesMetadataNamespace.GetName("inline");
        private static readonly XName MetadataNull = ODataConstants.DataServicesMetadataNamespace.GetName(ODataConstants.NullAttributeName);
        private static readonly XName MetadataAction = ODataConstants.DataServicesMetadataNamespace.GetName("action");
        private static readonly XName MetadataFunction = ODataConstants.DataServicesMetadataNamespace.GetName("function");

        /// <summary>
        /// Gets or sets the primitive converter to use while deserializing
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IXmlPrimitiveConverter PrimitiveConverter { get; set; }

        /// <summary>
        /// Gets or sets the ICSDLParser object
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ICsdlParser CsdlParser { get; set; }

        /// <summary>
        /// Gets or sets the spatial formatter to use while deserializing
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IGmlSpatialFormatter SpatialFormatter { get; set; }

        /// <summary>
        /// Gets or sets the primitive data type converter
        /// </summary>
        [InjectDependency]
        public IClrToPrimitiveDataTypeConverter PrimitiveDataTypeConverter { get; set; }

        /// <summary>
        /// Converts the given XML element into a rich payload element representation.
        /// </summary>
        /// <param name="element">XML element to convert.</param>
        /// <returns>A payload element representing the given element.</returns>
        public ODataPayloadElement ConvertToPayloadElement(XElement element)
        {
            ExceptionUtilities.CheckArgumentNotNull(element, "element");
            
            ODataPayloadElement result;
            if (element.Name == AtomFeed)
            {
                // if its a feed, deserialize it as an entity set
                result = this.DeserializeEntitySet(element);
            }
            else if (element.Name == AtomEntry)
            {
                // if its an entry, deserialize it as an entity
                result = this.DeserializeEntity(element);
            }
            else if (element.Name == DataServicesLinks)
            {
                // if its a plain-xml links collection, deserialize it as a collection of deferred links
                result = this.DeserializeLinks(element);
            }
            else if (element.Name == DataServicesUri || element.Name == MetadataUri)
            {
                // if its a plain-xml single link, deserialize it as a deferred link
                result = this.DeserializeLink(element);
            }
            else if (element.Name == EdmConstants.EdmxOasisNamespace.GetName(EdmxElementName))
            {
                // if its a metadata document
                result = this.DeserializeMetadata(element);
            }
            else
            {
                // TODO: service operations? primitive collection?

                // otherwise deserialize it as a property
                result = this.DeserializeProperty(element);
            }

            return result;
        }

        /// <summary>
        /// Parses a string like Bag(foo) to return foo
        /// </summary>
        /// <param name="typeName">name of type</param>
        /// <returns>Element of the Bag type</returns>
        internal static string ParseBagElementTypeName(string typeName)
        {
            string elementTypeName;
            if (ODataUtilities.TryGetMultiValueElementTypeName(typeName, out elementTypeName))
            {
                return elementTypeName;
            }

            return null;
        }

        private static void AddXmlBaseAnnotation(ODataPayloadElement result, XElement element)
        {
            var xmlBase = element.Attribute(XNamespace.Xml.GetName("base"));
            if (xmlBase != null)
            {
                result.Annotations.Add(new XmlBaseAnnotation(xmlBase.Value));
            }
        }

        /// <summary>
        /// Deserializes the given element, which is assumed to be an atom feed, into an entity set
        /// </summary>
        /// <param name="feed">An atom feed xml element</param>
        /// <returns>The deserialized entity set</returns>
        private EntitySetInstance DeserializeEntitySet(XElement feed)
        {
            ExceptionUtilities.CheckArgumentNotNull(feed, "feed");

            EntitySetInstance entitySet = new EntitySetInstance();

            AddXmlBaseAnnotation(entitySet, feed);

            // set title
            XElement title = feed.Element(AtomTitle);
            if (title != null)
            {
                entitySet.Annotations.Add(new TitleAnnotation(title.Value));
            }

            // read the count, if present
            XElement count = feed.Element(MetadataCount);
            if (count != null)
            {
                entitySet.InlineCount = long.Parse(count.Value, CultureInfo.InvariantCulture);
            }

            // read the next link, if present
            XElement nextLink = feed.Elements(AtomLink)
                .Select(l => l.Attribute(Rel))
                .Where(rel => rel != null)
                .Where(rel => rel.Value == RelNext)
                .Select(rel => rel.Parent)
                .SingleOrDefault();
            if (nextLink != null)
            {
                XAttribute href = nextLink.Attribute(Href);
                if (href != null)
                {
                    entitySet.NextLink = href.Value;
                }
            }

            // deserialize and add each entry
            foreach (XElement entry in feed.Elements(AtomEntry))
            {
                entitySet.Add(this.DeserializeEntity(entry));
            }

            this.AddFeedEPMAnnotations(feed, entitySet);
            return entitySet;
        }

        /// <summary>
        /// Deserializes an edmx response payload into a metadata payload representation
        /// </summary>
        /// <param name="edmxElement">The root of the edmx document</param>
        /// <returns>The metadata payload representation</returns>
        private MetadataPayloadElement DeserializeMetadata(XElement edmxElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(edmxElement, "edmxElement");
            ExceptionUtilities.Assert(edmxElement.Name.LocalName == EdmxElementName, "Deserialize metadata must be called on the 'edmx' element");

            var edmxNamespace = edmxElement.Name.Namespace;

            // Find Version attribute on metadataElement
            XAttribute versionAttribute = edmxElement.Attribute("Version");
            ExceptionUtilities.CheckObjectNotNull(versionAttribute, "Could not find 'Version' attribute");
            string edmxVersion = versionAttribute.Value;

            // Find dataservice element
            XElement dataServicesElement = edmxElement.Element(edmxNamespace.GetName("DataServices"));
            ExceptionUtilities.CheckObjectNotNull(dataServicesElement, "Could not find 'DataServices' element");

            MetadataPayloadElement metadataPayload = new MetadataPayloadElement()
            {
                EdmxVersion = edmxVersion,
            };

            AddXmlBaseAnnotation(metadataPayload, edmxElement);

            XElement[] schemaElements = dataServicesElement.Elements().Where(e => e.Name.LocalName == "Schema").ToArray();
            
            ExceptionUtilities.CheckObjectNotNull(this.CsdlParser, "Cannot deserialize metadata without a CSDL parser.");
            var model = this.CsdlParser.Parse(schemaElements);
            metadataPayload.EntityModelSchema = model;
            foreach (XElement schemaElement in schemaElements)
            {
                string edmNamespace = schemaElement.Name.NamespaceName;
                string schemaNamespace = schemaElement.Attributes().Where(a => a.Name.LocalName == "Namespace").Single().Value;
                model.Annotations.Add(new SchemaNamespaceAnnotation() { EdmNamespaceVersion = edmNamespace, SchemaNamespace = schemaNamespace });
            }

            return metadataPayload;
        }

        /// <summary>
        /// Deserializes an atom entry into an entity instance
        /// </summary>
        /// <param name="entry">The xml for an atom entry</param>
        /// <returns>An entity instance</returns>
        private EntityInstance DeserializeEntity(XElement entry)
        {
            EntityInstance entity = new EntityInstance();
            entity.IsNull = false;

            AddXmlBaseAnnotation(entity, entry);

            this.DeserializeEntryAttributes(entry, entity);
            
            // get the content and properties, as well as any MLE metadata
            XElement content = entry.Element(AtomContent);
            XElement properties = null;
            if (content != null)
            {
                XAttribute mediaSrc = content.Attribute(Src);
                XAttribute contentType = content.Attribute(Type);
                if (mediaSrc != null)
                {
                    entity.StreamSourceLink = mediaSrc.Value;
                    if (contentType != null)
                    {
                        entity.StreamContentType = contentType.Value;
                    }
                }
                else
                {
                    if (contentType != null)
                    {
                        entity.WithContentType(contentType.Value);
                    }

                    properties = content.Element(MetadataProperties);
                }
            }

            if (properties == null)
            {
                properties = entry.Element(MetadataProperties);
                if (properties != null)
                {
                    entity.AsMediaLinkEntry();
                }
            }

            if (properties != null)
            {
                foreach (XElement property in properties.Elements())
                {
                    entity.Add(this.DeserializeProperty(property));
                }
            }

            // get the navigation properties
            var links = entry.Elements(AtomLink);
            var navigations = this.DeserializeNavigationProperties(links);
            navigations = this.DeserializeRelationshipLinks(links, navigations.ToList());
            foreach (NavigationPropertyInstance nav in navigations)
            {
                entity.Add(nav);
            }

            var namedStreams = this.DeserializeNamedStreamProperties(links);
            foreach (NamedStreamInstance mediaStream in namedStreams)
            {
                entity.Add(mediaStream);
            }

            // Get actions and function descriptors
            var serviceOperationDescriptorXElements = entry.Elements().Where(e => e.Name == MetadataAction || e.Name == MetadataFunction);
            var serviceOperationDescriptors = this.DeserializeServiceOperatonDescriptors(serviceOperationDescriptorXElements);
            foreach (var actionDescriptorAnnotation in serviceOperationDescriptors)
            {
                entity.ServiceOperationDescriptors.Add(actionDescriptorAnnotation);
            }

            return entity;
        }

        private void DeserializeEntryAttributes(XElement entry, EntityInstance entity)
        {
            // get the edit link
            string linkValue;
            if (this.TryGetSpecificLink(entry, RelEdit, out linkValue))
            {
                entity.WithEditLink(linkValue);
            }

            // get the self link
            if (this.TryGetSpecificLink(entry, RelSelf, out linkValue))
            {
                entity.WithSelfLink(linkValue);
            }

            // get the edit-media link
            XElement editMediaLink;
            if (this.TryGetSpecificLink(entry, RelEditMedia, out editMediaLink))
            {
                XAttribute streamHref = editMediaLink.Attribute(Href);
                if (streamHref != null)
                {
                    entity.StreamEditLink = streamHref.Value;
                }

                XAttribute streamEtag = editMediaLink.Attribute(MetadataETag);
                if (streamEtag != null)
                {
                    entity.StreamETag = streamEtag.Value;
                }
            }

            // get the id
            XElement id = entry.Element(AtomId);
            if (id != null)
            {
                entity.Id = id.Value;
            }

            // get the etag
            XAttribute etag = entry.Attribute(MetadataETag);
            if (etag != null)
            {
                entity.ETag = etag.Value;
            }

            // get the type name
            XElement category = entry.Element(AtomCategory);
            if (category != null)
            {
                XAttribute term = category.Attribute(Term);
                if (term != null)
                {
                    entity.FullTypeName = term.Value;
                }
            }
        }

        private bool TryGetSpecificLink(XElement entry, string relValue, out XElement link)
        {
            // get the link
            link = entry.Elements(AtomLink)
                .Select(l => l.Attribute(Rel))
                .Where(rel => rel != null)
                .Where(rel => rel.Value == relValue)
                .Select(rel => rel.Parent)
                .SingleOrDefault();

            return link != null;
        }

        private bool TryGetSpecificLink(XElement entry, string relValue, out string hrefValue)
        {
            // get the link
            XElement link;
            if (this.TryGetSpecificLink(entry, relValue, out link))
            {
                XAttribute hrefAttribute = link.Attribute(Href);
                if (hrefAttribute != null)
                {
                    hrefValue = hrefAttribute.Value;
                    return true;
                }
            }

            hrefValue = null;
            return false;
        }

        private IEnumerable<NavigationPropertyInstance> DeserializeNavigationProperties(IEnumerable<XElement> links)
        {
            // get the links and convert them into navigation properties
            ILookup<string, XElement> linksByName = links
                .Select(link => link.Attribute(Rel))
                .Where(rel => rel != null)
                .Where(rel => rel.Value.StartsWith(ODataConstants.DataServicesRelatedNamespaceName, StringComparison.OrdinalIgnoreCase))
                .ToLookup(
                    rel => rel.Value.Substring(ODataConstants.DataServicesRelatedNamespaceName.Length),
                    rel => rel.Parent);

            List<NavigationPropertyInstance> results = new List<NavigationPropertyInstance>(linksByName.Count);
            foreach (var group in linksByName)
            {
                NavigationPropertyInstance navProp = new NavigationPropertyInstance();
                navProp.Name = group.Key;
                List<XElement> elements = group.ToList();

                if (elements.Count > 1)
                {
                    LinkCollection collection = new LinkCollection();
                    foreach (XElement link in elements)
                    {
                        ODataPayloadElement linkValue = this.DeserializeLink(link);
                        ExceptionUtilities.Assert(
                            linkValue.ElementType == ODataPayloadElementType.DeferredLink,
                            "Navigation property '" + group.Key + "' had multiple <link> elements which were not all deferred");

                        collection.Add(linkValue);
                    }

                    navProp.Value = collection;
                }
                else
                {
                    // Note: it is possible in cases where we are parsing an update payload, that this was really a collection of size one
                    // however, we cannot possibly detect this case here, so we will have to deal with it elsewhere should it arise
                    navProp.Value = this.DeserializeLink(elements[0]);
                }

                results.Add(navProp);
            }

            return results;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Two pass parse of XML payload")]
        private IEnumerable<NamedStreamInstance> DeserializeNamedStreamProperties(IEnumerable<XElement> links)
        {
            // This is a bit complex, but the goal here is to take all the edit and source links for named streams
            // and produce representations without leaving any information out. So, if duplicate edit or source links
            // with the same name are present, we want to represent them *somehow* so that later verification can fail.
            // However, edit/source links with the same name should be combined based on the order in which they appear.
            // The algorithm for this is as follows:
            //  Gather all source and edit links
            //  For each link:
            //    determine if it is an edit or source link
            //    if there is not an un-matched source/edit link with the same name:
            //      create a new instance and add it to the list of un-matched edit/source links
            //    else
            //      remove the link for the list of un-matched links, and update its properties
            //
            // find any media links and convert them into named stream properties
            var mediaLinks = links
                .Select(link => link.Attribute(Rel))
                .Where(rel => rel != null)
                .Where(rel => rel.Value.StartsWith(ODataConstants.DataServicesMediaResourceEditNamespaceName, StringComparison.Ordinal)
                    || rel.Value.StartsWith(ODataConstants.DataServicesMediaResourceNamespaceName, StringComparison.Ordinal))
                .Select(rel => rel.Parent)
                .ToList();

            // go through the links, generating named-stream-instances or updating instances created on earlier passes
            // when source/edit links can be combined
            List<NamedStreamInstance> results = new List<NamedStreamInstance>(mediaLinks.Count);
            var unmatchedEditLinks = new List<NamedStreamInstance>();
            var unmatchedSourceLinks = new List<NamedStreamInstance>();
            foreach (var mediaLink in mediaLinks)
            {
                var rel = mediaLink.Attribute(Rel).Value;
                string name;
                bool isEdit;
                List<NamedStreamInstance> possibleMatches;
                List<NamedStreamInstance> unmatchedList;

                // edit links should be matched with source links, and vice versa
                if (rel.StartsWith(ODataConstants.DataServicesMediaResourceEditNamespaceName, StringComparison.Ordinal))
                {
                    isEdit = true;
                    name = rel.Substring(ODataConstants.DataServicesMediaResourceEditNamespaceName.Length);
                    possibleMatches = unmatchedSourceLinks;
                    unmatchedList = unmatchedEditLinks;
                }
                else
                {
                    ExceptionUtilities.Assert(rel.StartsWith(ODataConstants.DataServicesMediaResourceNamespaceName, StringComparison.Ordinal), "Unexpected rel: {0}", rel);
                    isEdit = false;
                    name = rel.Substring(ODataConstants.DataServicesMediaResourceNamespaceName.Length);
                    possibleMatches = unmatchedEditLinks;
                    unmatchedList = unmatchedSourceLinks;
                }

                var namedStreamInstance = possibleMatches.FirstOrDefault(n => n.Name == name);
                if (namedStreamInstance == null)
                {
                    namedStreamInstance = new NamedStreamInstance(name);
                    results.Add(namedStreamInstance);
                    unmatchedList.Add(namedStreamInstance);
                }
                else
                {
                    possibleMatches.Remove(namedStreamInstance);
                }

                // get the media link href
                var href = mediaLink.Attribute(Href);
                if (href != null)
                {
                    if (isEdit)
                    {
                        namedStreamInstance.EditLink = href.Value;
                    }
                    else
                    {
                        namedStreamInstance.SourceLink = href.Value;
                    }
                }

                // get the media link type
                var type = mediaLink.Attribute(Type);
                if (type != null)
                {
                    if (isEdit)
                    {
                        namedStreamInstance.EditLinkContentType = type.Value;
                    }
                    else
                    {
                        namedStreamInstance.SourceLinkContentType = type.Value;
                    }
                }

                // get the media link etag
                var etag = mediaLink.Attribute(MetadataETag);
                if (etag != null)
                {
                    namedStreamInstance.ETag = etag.Value;
                }
            }

            return results;
        }

        private IEnumerable<NavigationPropertyInstance> DeserializeRelationshipLinks(IEnumerable<XElement> links, List<NavigationPropertyInstance> navigationProperties)
        {
            // get the links and convert them into relationship links
            IDictionary<string, XElement> relationshipLinks = links
                .Select(link => link.Attribute(Rel))
                .Where(rel => rel != null)
                .Where(rel => rel.Value.StartsWith(ODataConstants.DataServicesRelatedLinksNamespaceName, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(
                    rel => rel.Value.Substring(ODataConstants.DataServicesRelatedLinksNamespaceName.Length),
                    rel => rel.Parent);

            foreach (var relationshipLink in relationshipLinks)
            {
                NavigationPropertyInstance np = navigationProperties.FirstOrDefault(n => string.Compare(n.Name, relationshipLink.Key, StringComparison.Ordinal) == 0);
                XAttribute hrefAttribute = relationshipLink.Value.Attribute(Href);

                if (np == null)
                {
                    NavigationPropertyInstance newNp = new NavigationPropertyInstance();
                    newNp.Name = relationshipLink.Key;

                    if (hrefAttribute != null)
                    {
                        newNp.AssociationLink = new DeferredLink() { UriString = hrefAttribute.Value };
                    }

                    this.AddLinkAttributes(newNp.AssociationLink, relationshipLink.Value);
                    navigationProperties.Add(newNp);
                }
                else
                {
                    if (hrefAttribute != null)
                    {
                        np.AssociationLink = new DeferredLink() { UriString = hrefAttribute.Value };
                    }

                    this.AddLinkAttributes(np.AssociationLink, relationshipLink.Value);
                }
            }

            return navigationProperties;
        }

        private void AddLinkAttributes(ODataPayloadElement link, XElement linkValue)
        {
            XAttribute typeAttribute = linkValue.Attribute(Type);
            XAttribute titleAttribute = linkValue.Attribute(XNameTitle);

            if (typeAttribute != null)
            {
                link.Annotations.Add(new ContentTypeAnnotation(typeAttribute.Value));
            }

            if (titleAttribute != null)
            {
                link.Annotations.Add(new TitleAnnotation(titleAttribute.Value));
            }
            
            AddXmlBaseAnnotation(link, linkValue);
        }

        private void AddFeedEPMAnnotations(XElement feedXElement, EntitySetInstance entitySetInstance)
        {
            // get all the properties mapped to atom-specific locations
            // note that this is driven off what is found, not what is expected based on metadata

            // Id
            foreach (var id in feedXElement.Elements(AtomId))
            {
                this.AddEpmTree(entitySetInstance, id);
            }

            // Title
            foreach (var title in feedXElement.Elements(AtomTitle))
            {
                this.AddEpmTree(entitySetInstance, title);
            }

            // Summary
            foreach (var summary in feedXElement.Elements(AtomSummary))
            {
                this.AddEpmTree(entitySetInstance, summary);
            }

            // Rights
            foreach (var rights in feedXElement.Elements(AtomRights))
            {
                this.AddEpmTree(entitySetInstance, rights);
            }

            // Published
            foreach (var published in feedXElement.Elements(AtomPublished))
            {
                this.AddEpmTree(entitySetInstance, published);
            }

            // Updated
            foreach (var updated in feedXElement.Elements(AtomUpdated))
            {
                this.AddEpmTree(entitySetInstance, updated);
            }

            // Author
            foreach (var author in feedXElement.Elements(AtomAuthor))
            {
                // will recurse automatically
                this.AddEpmTree(entitySetInstance, author);
            }

            // Contributor
            foreach (var contributor in feedXElement.Elements(AtomContributor))
            {
                // will recurse automatically
                this.AddEpmTree(entitySetInstance, contributor);
            }
        }

        /// <summary>
        /// Adds an entity-property-mapping tree represented by the given element as an annotation on the payloadElement
        /// </summary>
        /// <param name="payloadElement"> the payloadElement to add the EpmnTree annotations to</param>
        /// <param name="element"> The element representing the mapped property </param>
        private void AddEpmTree(ODataPayloadElement payloadElement, XElement element)
        {
            var mappedProperty = this.BuildEpmTree(element);
            payloadElement.Annotations.Add(mappedProperty);
        }

        /// <summary>
        /// Builds the entity-property-mapping tree represented by the given xml element
        /// </summary>
        /// <param name="element">The element to build from</param>
        /// <returns>The entity-property-mapping tree</returns>
        private XmlTreeAnnotation BuildEpmTree(XElement element)
        {
            var mappedProperty = new XmlTreeAnnotation()
            {
                IsAttribute = false,
                LocalName = element.Name.LocalName,
                NamespaceName = element.Name.NamespaceName,
                NamespacePrefix = element.GetPrefixOfNamespace(element.Name.Namespace),
                PropertyValue = element.Nodes().OfType<XText>().Select(t => t.Value).SingleOrDefault(),
            };

            foreach (var subElement in element.Elements())
            {
                mappedProperty.Children.Add(this.BuildEpmTree(subElement));
            }

            foreach (var attribute in element.Attributes().Where(a => !a.IsNamespaceDeclaration))
            {
                mappedProperty.Children.Add(this.BuildEpmTree(attribute));
            }

            return mappedProperty;
        }

        /// <summary>
        /// Builds the entity-property-mapping tree represented by the given xml attribute
        /// </summary>
        /// <param name="attribute">The attribute to build from</param>
        /// <returns>The entity-property-mapping tree</returns>
        private XmlTreeAnnotation BuildEpmTree(XAttribute attribute)
        {
            return new XmlTreeAnnotation()
            {
                IsAttribute = true,
                LocalName = attribute.Name.LocalName,
                NamespaceName = attribute.Name.NamespaceName,
                NamespacePrefix = attribute.Parent.GetPrefixOfNamespace(attribute.Name.Namespace),
                PropertyValue = attribute.Value
            };
        }

        /// <summary>
        /// Deserializes a collection of links
        /// </summary>
        /// <param name="links">Xml element representing the links collection</param>
        /// <returns>Deserialized link collection</returns>
        private LinkCollection DeserializeLinks(XElement links)
        {
            LinkCollection collection = new LinkCollection();

            AddXmlBaseAnnotation(collection, links);

            // read the count, if present
            XElement count = links.Element(MetadataCount);
            if (count != null)
            {
                collection.InlineCount = long.Parse(count.Value, CultureInfo.InvariantCulture);
            }

            // read the next-link if present
            XElement nextLink = links.Element(DataServicesNext);
            if (nextLink != null)
            {
                collection.NextLink = nextLink.Value;
            }

            // deserialize each element as a link and add it to the collection
            foreach (var link in links.Elements(MetadataUri))
            {
                collection.Add(this.DeserializeLink(link));
            }

            return collection;
        }

        /// <summary>
        /// Deserialize a single link
        /// </summary>
        /// <param name="link">the xml representing the link</param>
        /// <returns>Either an expanded or deferred link</returns>
        private ODataPayloadElement DeserializeLink(XElement link)
        {
            if (link.Name == MetadataUri)
            {
                var result = new DeferredLink() { UriString = link.Attribute(AtomId).Value };
                AddXmlBaseAnnotation(result, link);
                
                // add the element so that later validation can happen to validate the MetadataUri namespace is not used
                var xmlPayloadRep = new XmlPayloadElementRepresentationAnnotation() { XmlNodes = new XNode[] { link } };
                result.Annotations.Add(xmlPayloadRep);
                return result;
            }

            string hrefValue = null;
            XAttribute hrefAttribute = link.Attribute(Href);
            if (hrefAttribute != null)
            {
                if (string.IsNullOrEmpty(hrefAttribute.Value))
                {
                    // special case: navigation properties with null values are represented as empty href tags
                    return null;
                }

                hrefValue = hrefAttribute.Value;
            }

            // if the link has an inline element, assume it is expanded
            XElement inline = link.Element(MetadataInline);
            if (inline != null)
            {
                // deserialize the expanded element
                ExpandedLink expanded = new ExpandedLink() { UriString = hrefValue };
                
                if (inline.HasElements)
                {
                    expanded.ExpandedElement = this.ConvertToPayloadElement(inline.Elements().Single());
                }

                this.AddLinkAttributes(expanded, link);

                return expanded;
            }
            else
            {
                // otherwise it must be deferred
                DeferredLink deferred = new DeferredLink() { UriString = hrefValue };

                this.AddLinkAttributes(deferred, link);

                return deferred;
            }
        }

        /// <summary>
        /// Deserializes the element as either a complex, a primitive, or a null property, based on the content
        /// </summary>
        /// <param name="property">The xml to deserialize</param>
        /// <returns>A property representing the given xml</returns>
        private PropertyInstance DeserializeProperty(XElement property)
        {
            return this.DeserializeProperty(property, null);
        }

        /// <summary>
        /// Deserializes the element as either a complex, a primitive, or a null property, based on the content
        /// </summary>
        /// <param name="property">The xml to deserialize</param>
        /// <param name="typeNameFallback">TypeName to use instead of the one from the XElement[type] attribute</param>
        /// <returns>A property representing the given xml</returns>
        private PropertyInstance DeserializeProperty(XElement property, string typeNameFallback)
        {
            string propertyName = property.Name.LocalName;

            // get the type name
            string typeNameFromPayload = null;
            XAttribute typeAttribute = property.Attribute(MetadataType);
            if (typeAttribute != null)
            {
                typeNameFromPayload = typeAttribute.Value;
            }

            // set type to be fallback when typeattribute does not exist
            var typeNameForClrTypeLookup = typeNameFromPayload;
            if (typeNameForClrTypeLookup == null && !string.IsNullOrEmpty(typeNameFallback))
            {
                typeNameForClrTypeLookup = typeNameFallback;
            }

            // try to infer the clr type
            Type clrType = null;
            if (!string.IsNullOrEmpty(typeNameForClrTypeLookup))
            {
                ExceptionUtilities.CheckObjectNotNull(this.PrimitiveDataTypeConverter, "Cannot infer clr type from edm type without converter");
                clrType = this.PrimitiveDataTypeConverter.ToClrType(typeNameForClrTypeLookup);
            }

            PropertyInstance result;
            if (property.HasElements)
            {
                // must be complex, a multivalue, or spatial
                ExceptionUtilities.CheckObjectNotNull(this.SpatialFormatter, "Cannot safely deserialize element with children without spatial formatter.");

                // try to infer which spatial type hierarchy it is from the type name in the payload
                SpatialTypeKind? kind = null;
                if (clrType != null)
                {
                    SpatialUtilities.TryInferSpatialTypeKind(clrType, out kind);
                }

                object spatialInstance;
                if (this.SpatialFormatter.TryParse(property.Elements().First(), kind, out spatialInstance))
                {
                    ExceptionUtilities.Assert(property.Elements().Count() == 1, "Spatial property had more than 1 sub-element");
                    result = new PrimitiveProperty(propertyName, typeNameFromPayload, spatialInstance);
                }
                else if (property.Elements().All(e => e.Name == DataServicesElement))
                {
                    result = this.DeserializeCollectionProperty(property);
                }
                else
                {
                    result = new ComplexProperty(propertyName, this.DeserializeComplexInstance(property));
                }
            }
            else
            {
                // check for the null attribute
                bool isNull = false;
                XAttribute isNullAttribute = property.Attribute(MetadataNull);
                if (isNullAttribute != null)
                {
                    isNull = bool.Parse(isNullAttribute.Value);
                }

                // If its null and we can't tell whether it is primitive or complex, then return a null marker
                if (isNull && clrType == null)
                {
                    result = new NullPropertyInstance(propertyName, typeNameFromPayload);
                }
                else if (typeNameFromPayload != null && typeNameFromPayload.StartsWith(ODataConstants.BeginMultiValueTypeIdentifier, StringComparison.Ordinal))
                {
                    ExceptionUtilities.CheckObjectNotNull(this.PrimitiveDataTypeConverter, "Cannot infer clr type from edm type without converter");

                    string elementTypeName = ParseBagElementTypeName(typeNameFromPayload);
                    if (this.PrimitiveDataTypeConverter.ToClrType(elementTypeName) != null)
                    {
                        result = new PrimitiveMultiValueProperty(propertyName, new PrimitiveMultiValue(typeNameFromPayload, isNull));
                    }
                    else
                    {
                        result = new ComplexMultiValueProperty(propertyName, new ComplexMultiValue(typeNameFromPayload, isNull));
                    }
                }
                else
                {
                    object value;
                    if (isNull)
                    {
                        value = null;
                    }
                    else if (clrType != null)
                    {
                        ExceptionUtilities.CheckObjectNotNull(this.PrimitiveConverter, "PrimitiveConverter has not been set.");
                        value = this.PrimitiveConverter.DeserializePrimitive(property.Value, clrType);
                    }
                    else
                    {
                        value = property.Value;
                    }

                    result = new PrimitiveProperty(propertyName, typeNameFromPayload, value);
                }
            }

            AddXmlBaseAnnotation(result, property);

            return result;
        }

        /// <summary>
        /// Deserialize the element as a complex instance
        /// </summary>
        /// <param name="element">The xml representing a complex instance</param>
        /// <returns>The deserialized complex instance</returns>
        private ComplexInstance DeserializeComplexInstance(XElement element)
        {
            ComplexInstance instance = new ComplexInstance();

            // get the type
            XAttribute type = element.Attribute(MetadataType);
            if (type != null)
            {
                instance.FullTypeName = type.Value;
            }

            // check for the null attribute
            XAttribute isNull = element.Attribute(MetadataNull);
            if (isNull != null)
            {
                instance.IsNull = bool.Parse(isNull.Value);
            }
            else if (element.HasElements)
            {
                instance.IsNull = false;
            }

            // get the properties
            foreach (XElement subElement in element.Elements())
            {
                instance.Add(this.DeserializeProperty(subElement));
            }

            return instance;
        }

        /// <summary>
        /// Deserializes a collection property
        /// </summary>
        /// <param name="propertyElement">The xml element representing the property</param>
        /// <returns>The deserialized property</returns>
        private PropertyInstance DeserializeCollectionProperty(XElement propertyElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyElement, "propertyElement");
            ExceptionUtilities.CheckCollectionNotEmpty(propertyElement.Elements(), "propertyElement.Elements()");
            ExceptionUtilities.Assert(propertyElement.Elements().All(e => e.Name == DataServicesElement), "Collection property sub elements had inconsistent names");

            string propertyName = propertyElement.Name.LocalName;

            string fullTypeName = null;
            string bagElementTypeName = null;
            var type = propertyElement.Attribute(MetadataType);
            if (type != null)
            {
                fullTypeName = type.Value;
                bagElementTypeName = ParseBagElementTypeName(type.Value);
            }

            // convert the elements first, then decide if its a primitive or complex collection
            // note that a collection containing all null values will appear as a primitive collection, regardless of what the type really is
            List<PropertyInstance> elements = null;
            if (bagElementTypeName != null)
            {
                elements = propertyElement.Elements().Select(e => this.DeserializeProperty(e, bagElementTypeName)).ToList();
            }
            else
            {
                elements = propertyElement.Elements().Select(e => this.DeserializeProperty(e)).ToList();
            }

            if (elements.OfType<ComplexProperty>().Any())
            {
                var complex = new ComplexMultiValueProperty(propertyName, new ComplexMultiValue(fullTypeName, false));

                foreach (var complexElement in elements)
                {
                    if (complexElement.ElementType == ODataPayloadElementType.NullPropertyInstance)
                    {
                        var nullProperty = complexElement as NullPropertyInstance;
                        complex.Value.Add(new ComplexInstance(nullProperty.FullTypeName, true));
                    }
                    else
                    {
                        ExceptionUtilities.Assert(complexElement.ElementType == ODataPayloadElementType.ComplexProperty, "Complex value collection contained non-complex, non-null value");
                        complex.Value.Add((complexElement as ComplexProperty).Value);
                    }
                }

                return complex;
            }
            else
            {
                var primitive = new PrimitiveMultiValueProperty(propertyName, new PrimitiveMultiValue(fullTypeName, false));

                foreach (var primitiveElement in elements)
                {
                    if (primitiveElement.ElementType == ODataPayloadElementType.NullPropertyInstance)
                    {
                        var nullProperty = primitiveElement as NullPropertyInstance;
                        primitive.Value.Add(new PrimitiveValue(nullProperty.FullTypeName, null));
                    }
                    else
                    {
                        ExceptionUtilities.Assert(primitiveElement.ElementType == ODataPayloadElementType.PrimitiveProperty, "Primitive value collection contained non-primitive, non-null value");
                        primitive.Value.Add((primitiveElement as PrimitiveProperty).Value);
                    }
                }

                return primitive;
            }
        }

        private IEnumerable<ServiceOperationDescriptor> DeserializeServiceOperatonDescriptors(IEnumerable<XElement> serviceOperationPayloadElements)
        {
            List<ServiceOperationDescriptor> serviceOperationDescriptors = new List<ServiceOperationDescriptor>();
            foreach (var xelement in serviceOperationPayloadElements)
            {
                var serviceOperationDescriptor = new ServiceOperationDescriptor();

                if (xelement.Name == MetadataAction)
                {
                    serviceOperationDescriptor.IsAction = true;
                }

                serviceOperationDescriptor.Metadata = this.GetXAttributeValueIfExists(xelement.Attribute(XNameMetadata));
                serviceOperationDescriptor.Title = this.GetXAttributeValueIfExists(xelement.Attribute(XNameTitle));
                serviceOperationDescriptor.Target = this.GetXAttributeValueIfExists(xelement.Attribute(XNameTarget));
                serviceOperationDescriptors.Add(serviceOperationDescriptor);
            }

            return serviceOperationDescriptors.AsEnumerable();
        }

        private string GetXAttributeValueIfExists(XAttribute xattribute)
        {
            string val = null;
            if (xattribute != null)
            {
                val = xattribute.Value;
            }

            return val;
        }
    }
}
