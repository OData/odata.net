//---------------------------------------------------------------------
// <copyright file="AtomMetadataBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    #endregion Namespaces

    /// <summary>
    /// Extension methods for annotating ODataPayloadElements with Atom metadata values
    /// </summary>
    public static class AtomMetadataBuilder
    {
        /// <summary>
        /// Annotates the resource collection with app:accept values.
        /// </summary>
        /// <param name="resourceCollection">The resource collection to annotate.</param>
        /// <param name="value">The value of the app:accept element.</param>
        /// <returns>The resource collection with the annotation applied.</returns>
        public static ResourceCollectionInstance AppAccept(this ResourceCollectionInstance resourceCollection, string value)
        {
            ExceptionUtilities.CheckArgumentNotNull(resourceCollection, "resourceCollection");

            resourceCollection.AddAnnotation(CreateAppElement(TestAtomConstants.AtomPublishingAcceptAttributeName, value));
            return resourceCollection;
        }

        /// <summary>
        /// Annotates the payload element with app:control values.
        /// </summary>
        /// <typeparam name="T">The type of ODataPayloadElement.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="draft">The value of the app:control's draft property</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AppControl<T>(this T payloadElement, string draft) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var controlChildren = new List<XmlTreeAnnotation>();
            if (draft != null)
            {
                controlChildren.Add(CreateAppElement(TestAtomConstants.AtomPublishingDraftElementName, draft));
            }

            payloadElement.AddAnnotation(CreateAppElement(TestAtomConstants.AtomPublishingControlElementName, null, controlChildren.ToArray()));
            return payloadElement;
        }


        /// <summary>
        /// Annotates the entry with app:edited values.
        /// </summary>
        /// <param name="entry">The entry to annotate.</param>
        /// <param name="editedDate">The value of the app:edited element.</param>
        /// <returns>The entry with the annotation applied.</returns>
        public static EntityInstance AppEdited(this EntityInstance entry, string editedDate)
        {
            ExceptionUtilities.CheckArgumentNotNull(entry, "entry");

            entry.AddAnnotation(CreateAppElement(TestAtomConstants.AtomPublishingEditedElementName, editedDate));
            return entry;
        }

        /// <summary>
        /// Annotates the resource collection with inline app:categories values.
        /// </summary>
        /// <param name="resourceCollection">The resource collection to annotate.</param>
        /// <param name="fixedValue">The value of the app:categories fixed property.</param>
        /// <param name="scheme">The value of the app:categories scheme property.</param>
        /// <param name="categories">The atom:category children of the app:categories.</param>
        /// <returns>The resource collection with the annotation applied.</returns>
        public static ResourceCollectionInstance AppInlineCategories(this ResourceCollectionInstance resourceCollection, string fixedValue, string scheme, params XmlTreeAnnotation[] categories)
        {
            ExceptionUtilities.CheckArgumentNotNull(resourceCollection, "resourceCollection");

            XmlTreeAnnotation[] categoriesAttributes = CreateAtomAttributes(
                new KeyValuePair<string, string>(TestAtomConstants.AtomPublishingFixedAttributeName, fixedValue),
                new KeyValuePair<string, string>(TestAtomConstants.AtomCategorySchemeAttributeName, scheme));

            resourceCollection.AddAnnotation(CreateAppElement("categories", null, categoriesAttributes.Union(categories).ToArray()));
            return resourceCollection;
        }

        /// <summary>
        /// Annotates the resource collection with out-of-line app:categories values.
        /// </summary>
        /// <param name="resourceCollection">The resource collection to annotate.</param>
        /// <param name="uri">The value of the app:categories href property.</param>
        /// <returns>The resource collection with the annotation applied.</returns>
        public static ResourceCollectionInstance AppOutOfLineCategories(this ResourceCollectionInstance resourceCollection, string uri)
        {
            ExceptionUtilities.CheckArgumentNotNull(resourceCollection, "resourceCollection");

            resourceCollection.AddAnnotation(CreateAppElement("categories", null, XmlTreeAnnotation.AtomAttribute(TestAtomConstants.AtomHRefAttributeName, uri)));
            return resourceCollection;
        }

        /// <summary>
        /// Annotates the payload element with atom:author values.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="name">The value of the atom:author's name property.</param>
        /// <param name="uri">The value of the atom:author's URI property.</param>
        /// <param name="email">The value of the atom:author's email address property.</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AtomAuthor<T>(this T payloadElement, string name, string uri, string email) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            return payloadElement.AtomPerson(TestAtomConstants.AtomAuthorElementName, name, uri, email);
        }

        /// <summary>
        /// Annotates the payload element with atom:category values.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="term">The value of the atom:category's term property.</param>
        /// <param name="scheme">The value of the atom:category's scheme property.</param>
        /// <param name="label">The value of the atom:category's label property.</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AtomCategory<T>(this T payloadElement, string term, string scheme, string label) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            payloadElement.AddAnnotation(AtomMetadataBuilder.AtomCategory(term, scheme, label));
            return payloadElement;
        }

        /// <summary>
        /// Annotates the payload element with atom:category value with the type name scheme.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="term">The value of the atom:category's term property.</param>
        /// <param name="label">The value of the atom:category's label property.</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AtomCategoryWithTypeName<T>(this T payloadElement, string term, string label) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            return payloadElement.AtomCategory(term, TestAtomConstants.ODataSchemeNamespace, label);
        }

        /// <summary>
        /// Creates an annotation representing an atom:category element.
        /// </summary>
        /// <param name="term">The value of the atom:category's term property.</param>
        /// <param name="scheme">The value of the atom:category's scheme property.</param>
        /// <param name="label">The value of the atom:category's label property.</param>
        /// <returns>An annotation representing the atom:category.</returns>
        public static XmlTreeAnnotation AtomCategory(string term, string scheme, string label)
        {
            var categoryAttributes = CreateAtomAttributes(
                    new KeyValuePair<string, string>(TestAtomConstants.AtomCategoryTermAttributeName, term),
                    new KeyValuePair<string, string>(TestAtomConstants.AtomCategorySchemeAttributeName, scheme),
                    new KeyValuePair<string, string>(TestAtomConstants.AtomCategoryLabelAttributeName, label));

            return XmlTreeAnnotation.Atom(TestAtomConstants.AtomCategoryElementName, null, categoryAttributes);
        }

        /// <summary>
        /// Annotates the payload element with atom:contributor values.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="name">The value of the atom:contributor's name property.</param>
        /// <param name="uri">The value of the atom:contributor's URI property.</param>
        /// <param name="email">The value of the atom:contributor's email address property.</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AtomContributor<T>(this T payloadElement, string name, string uri, string email) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            return payloadElement.AtomPerson(TestAtomConstants.AtomContributorElementName, name, uri, email);
        }

        /// <summary>
        /// Annotates the entry with edit atom:link values.
        /// </summary>
        /// <param name="entry">The entry to annotate.</param>
        /// <param name="href">The value of the atom:link's href property</param>
        /// <param name="type">The value of the atom:link's type property</param>
        /// <param name="hrefLang">The optional value of the atom:link's hrefLang property</param>
        /// <param name="title">The optional value of the atom:link's title property</param>
        /// <param name="length">The optional value of the atom:link's length property</param>
        /// <returns>The entry with the annotation applied.</returns>
        public static EntityInstance AtomEditLink(this EntityInstance entry, string href, string type, string hrefLang = null, string title = null, string length = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(entry, "entry");

            return entry.AtomLink(href, TestAtomConstants.AtomEditRelationAttributeValue, type, hrefLang, title, length);
        }

        /// <summary>
        /// Annotates the feed with atom:generator values.
        /// </summary>
        /// <param name="feed">The feed to annotate.</param>
        /// <param name="name">The value of the atom:generator element.</param>
        /// <param name="uri">The value of the atom:generator's URI property.</param>
        /// <param name="version">The value of the atom:generator's version property.</param>
        /// <returns>The feed with the annotation applied.</returns>
        public static EntitySetInstance AtomGenerator(this EntitySetInstance feed, string name, string uri, string version)
        {
            ExceptionUtilities.CheckArgumentNotNull(feed, "feed");

            var generatorAttributes = CreateAtomAttributes(
                new KeyValuePair<string, string>(TestAtomConstants.AtomGeneratorUriAttributeName, uri),
                new KeyValuePair<string, string>(TestAtomConstants.AtomGeneratorVersionAttributeName, version));

            feed.AddAnnotation(XmlTreeAnnotation.Atom(TestAtomConstants.AtomGeneratorElementName, name, generatorAttributes));
            return feed;
        }

        /// <summary>
        /// Annotates the feed with atom:icon values.
        /// </summary>
        /// <param name="feed">The feed to annotate.</param>
        /// <param name="uri">The value of the atom:generator element.</param>
        /// <returns>The feed with the annotation applied.</returns>
        public static EntitySetInstance AtomIcon(this EntitySetInstance feed, string uri)
        {
            ExceptionUtilities.CheckArgumentNotNull(feed, "feed");

            feed.AddAnnotation(XmlTreeAnnotation.Atom(TestAtomConstants.AtomIconElementName, uri));
            return feed;
        }

        /// <summary>
        /// Annotates the feed with atom:id value.
        /// </summary>
        /// <param name="feed">The feed to annotate.</param>
        /// <param name="idValue">The value of the atom:id element.</param>
        /// <returns>The feed with the annotation applied.</returns>
        public static EntitySetInstance AtomId(this EntitySetInstance feed, string idValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(feed, "feed");

            feed.AddAnnotation(XmlTreeAnnotation.Atom(TestAtomConstants.AtomIdElementName, idValue));
            return feed;
        }

        /// <summary>
        /// Annotates the payload element with a link annotation and adds the given atom:link attribute values as children on that annotation.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="href">The value of the atom:link's href property</param>
        /// <param name="rel">The value of the atom:link's rel property</param>
        /// <param name="type">The value of the atom:link's type property</param>
        /// <param name="hrefLang">The optional value of the atom:link's hrefLang property</param>
        /// <param name="title">The optional value of the atom:link's title property</param>
        /// <param name="length">The optional value of the atom:link's length property</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AtomLink<T>(this T payloadElement, string href, string rel, string type, string hrefLang = null, string title = null, string length = null) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var linkAttributes = CreateAtomAttributes(
                new KeyValuePair<string, string>(TestAtomConstants.AtomLinkHrefAttributeName, href),
                new KeyValuePair<string, string>(TestAtomConstants.AtomLinkRelationAttributeName, rel),
                new KeyValuePair<string, string>(TestAtomConstants.AtomLinkTypeAttributeName, type),
                new KeyValuePair<string, string>(TestAtomConstants.AtomLinkHrefLangAttributeName, hrefLang),
                new KeyValuePair<string, string>(TestAtomConstants.AtomLinkTitleAttributeName, title),
                new KeyValuePair<string, string>(TestAtomConstants.AtomLinkLengthAttributeName, length));

            payloadElement.AddAnnotation(XmlTreeAnnotation.Atom(TestAtomConstants.AtomLinkElementName, null, linkAttributes));
            return payloadElement;
        }

        /// <summary>
        /// Annotates a payload element representing a link with atom:link attribute values.
        /// </summary>
        /// <param name="linkPayloadElement">The payload element to annotate.</param>
        /// <param name="href">The value of the atom:link's href property</param>
        /// <param name="rel">The value of the atom:link's rel property</param>
        /// <param name="type">The value of the atom:link's type property</param>
        /// <param name="hrefLang">The optional value of the atom:link's hrefLang property</param>
        /// <param name="title">The optional value of the atom:link's title property</param>
        /// <param name="length">The optional value of the atom:link's length property</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AtomLinkAttributes<T>(this T linkPayloadElement, string href, string rel, string type, string hrefLang = null, string title = null, string length = null) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(linkPayloadElement, "linkPayloadElement");
            
            // Since the payload element already represents a link, we annotate a temporary element 
            // and copy the "children" annotations onto the actual payload element.
            var tempPayloadElement = new EntityInstance();
            ExceptionUtilities.Assert(!tempPayloadElement.Annotations.OfType<XmlTreeAnnotation>().Any(), "Payload element should not have XmlTreeAnnotations after construction");
            tempPayloadElement.AtomLink(href, rel, type, hrefLang, title, length);

            XmlTreeAnnotation linkAnnotation = tempPayloadElement.Annotations.OfType<XmlTreeAnnotation>().Single();
            foreach (XmlTreeAnnotation childAnnotation in linkAnnotation.Children)
            {
                linkPayloadElement.AddAnnotation(childAnnotation);
            }

            return linkPayloadElement;
        }

        /// <summary>
        /// Annotates the feed with atom:logo values.
        /// </summary>
        /// <param name="feed">The feed to annotate.</param>
        /// <param name="uri">The value of the atom:logo element.</param>
        /// <returns>The feed with the annotation applied.</returns>
        public static EntitySetInstance AtomLogo(this EntitySetInstance feed, string uri)
        {
            ExceptionUtilities.CheckArgumentNotNull(feed, "feed");

            feed.AddAnnotation(XmlTreeAnnotation.Atom(TestAtomConstants.AtomLogoElementName, uri));
            return feed;
        }

        /// <summary>
        /// Annotates the named stream with atom:link values for the 'edit' link.
        /// </summary>
        /// <param name="namedStream">The named stream to annotate.</param>
        /// <param name="href">The value of the atom:link's href property</param>
        /// <param name="type">The value of the atom:link's type property</param>
        /// <param name="hrefLang">The optional value of the atom:link's hrefLang property</param>
        /// <param name="title">The optional value of the atom:link's title property</param>
        /// <param name="length">The optional value of the atom:link's length property</param>
        /// <returns>The named stream with the annotations applied.</returns>
        public static NamedStreamInstance AtomNamedStreamEditLink(this NamedStreamInstance namedStream, string href, string type, string hrefLang = null, string title = null, string length = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(namedStream, "namedStream");

            string relation = TestAtomConstants.ODataStreamPropertyEditMediaRelatedLinkRelationPrefix + namedStream.Name;
            return namedStream.AtomNamedStreamLink(href, relation, type, hrefLang, title, length);
        }


        /// <summary>
        /// Annotates the named stream with atom:link values.
        /// </summary>
        /// <param name="namedStream">The named stream to annotate.</param>
        /// <param name="href">The value of the atom:link's href property</param>
        /// <param name="rel">The value of the atom:link's rel property</param>
        /// <param name="type">The value of the atom:link's type property</param>
        /// <param name="hrefLang">The optional value of the atom:link's hrefLang property</param>
        /// <param name="title">The optional value of the atom:link's title property</param>
        /// <param name="length">The optional value of the atom:link's length property</param>
        /// <returns>The named stream with the annotations applied.</returns>
        public static NamedStreamInstance AtomNamedStreamLink(this NamedStreamInstance namedStream, string href, string rel, string type, string hrefLang = null, string title = null, string length = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(namedStream, "namedStream");

            // The Test OM representation for Named Streams does not allow individual annotation of the two links that it represents.
            // Thus, we special case Named Streams, and apply two annotations to represent ATOM link metadata - one for serialization and one for comparison.

            // This annotation will replace the standard serialization of the NamedStreamInstance with the link XElement.
            var xmlAnnotation = namedStream.Annotations.OfType<XmlPayloadElementRepresentationAnnotation>().SingleOrDefault();
            if (xmlAnnotation == null)
            {
                xmlAnnotation = new XmlPayloadElementRepresentationAnnotation { XmlNodes = new XNode[0] };
                namedStream.AddAnnotation(xmlAnnotation);
            }

            XElement linkElement = new XElement(TestAtomConstants.AtomXNamespace.GetName(TestAtomConstants.AtomLinkElementName));
            AddXAttribute(linkElement, TestAtomConstants.AtomLinkHrefAttributeName, href);
            AddXAttribute(linkElement, TestAtomConstants.AtomLinkRelationAttributeName, rel);
            AddXAttribute(linkElement, TestAtomConstants.AtomLinkTypeAttributeName, type);
            AddXAttribute(linkElement, TestAtomConstants.AtomLinkHrefLangAttributeName, hrefLang);
            AddXAttribute(linkElement, TestAtomConstants.AtomLinkTitleAttributeName, title);
            AddXAttribute(linkElement, TestAtomConstants.AtomLinkLengthAttributeName, length);

            xmlAnnotation.XmlNodes = xmlAnnotation.XmlNodes.Union(new[] { linkElement }).ToArray();

            // This annotation captures the link metadata values for comparison with test result.
            namedStream.AddAnnotation(
                new NamedStreamAtomLinkMetadataAnnotation
                {
                    Href = href,
                    HrefLang = hrefLang,
                    Length = length,
                    Relation = rel,
                    Title = title,
                    Type = type,
                });

            return namedStream;
        }

        /// <summary>
        /// Annotates the named stream with atom:link values for the 'source' link.
        /// </summary>
        /// <param name="namedStream">The named stream to annotate.</param>
        /// <param name="href">The value of the atom:link's href property</param>
        /// <param name="type">The value of the atom:link's type property</param>
        /// <param name="hrefLang">The optional value of the atom:link's hrefLang property</param>
        /// <param name="title">The optional value of the atom:link's title property</param>
        /// <param name="length">The optional value of the atom:link's length property</param>
        /// <returns>The named stream with the annotations applied.</returns>
        public static NamedStreamInstance AtomNamedStreamSourceLink(this NamedStreamInstance namedStream, string href, string type, string hrefLang = null, string title = null, string length = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(namedStream, "namedStream");

            string relation = TestAtomConstants.ODataStreamPropertyMediaResourceRelatedLinkRelationPrefix + namedStream.Name;
            return namedStream.AtomNamedStreamLink(href, relation, type, hrefLang, title, length);
        }

        /// <summary>
        /// Annotates the entry with atom:published values.
        /// </summary>
        /// <param name="entry">The entry to annotate.</param>
        /// <param name="publishedDate">The value of the atom:published element.</param>
        /// <returns>The feed with the annotation applied.</returns>
        public static EntityInstance AtomPublished(this EntityInstance entry, string publishedDate)
        {
            ExceptionUtilities.CheckArgumentNotNull(entry, "entry");

            entry.AddAnnotation(XmlTreeAnnotation.Atom(TestAtomConstants.AtomPublishedElementName, publishedDate));
            return entry;
        }

        /// <summary>
        /// Annotates the payload element with atom:rights values.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="rightsValue">The value of the atom:rights element.</param>
        /// <param name="rightsValueType">The type of the atom:rights value.</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AtomRights<T>(this T payloadElement, string rightsValue, string rightsValueType) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            return payloadElement.AtomText(TestAtomConstants.AtomRightsElementName, rightsValue, rightsValueType);
        }

        /// <summary>
        /// Annotates the payload element with self atom:link values.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="href">The value of the atom:link's href property</param>
        /// <param name="type">The value of the atom:link's type property</param>
        /// <param name="hrefLang">The optional value of the atom:link's hrefLang property</param>
        /// <param name="title">The optional value of the atom:link's title property</param>
        /// <param name="length">The optional value of the atom:link's length property</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AtomSelfLink<T>(this T payloadElement, string href, string type, string hrefLang = null, string title = null, string length = null) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            return payloadElement.AtomLink(href, TestAtomConstants.AtomSelfRelationAttributeValue, type, hrefLang, title, length);
        }

        /// <summary>
        /// Annotates the payload element with next atom:link values.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="href">The value of the atom:link's href property</param>
        /// <param name="type">The value of the atom:link's type property</param>
        /// <param name="hrefLang">The optional value of the atom:link's hrefLang property</param>
        /// <param name="title">The optional value of the atom:link's title property</param>
        /// <param name="length">The optional value of the atom:link's length property</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AtomNextPageLink<T>(this T payloadElement, string href, string type, string hrefLang = null, string title = null, string length = null) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            return payloadElement.AtomLink(href, TestAtomConstants.AtomNextRelationAttributeValue, type, hrefLang, title, length);
        }

        /// <summary>
        /// Annotates the payload element with delta atom:link values.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="href">The value of the atom:link's href property</param>
        /// <param name="type">The value of the atom:link's type property</param>
        /// <param name="hrefLang">The optional value of the atom:link's hrefLang property</param>
        /// <param name="title">The optional value of the atom:link's title property</param>
        /// <param name="length">The optional value of the atom:link's length property</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AtomDeltaLink<T>(this T payloadElement, string href, string type, string hrefLang = null, string title = null, string length = null) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            return payloadElement.AtomLink(href, TestAtomConstants.AtomDeltaRelationAttributeValue, type, hrefLang, title, length);
        }


        /// <summary>
        /// Annotates the entry with atom:source values.
        /// </summary>
        /// <param name="entry">The entry to annotate.</param>
        /// <param name="sourceFeed">The feed containing metadata to copy.</param>
        /// <returns>The entry with the annotation applied.</returns>
        public static EntityInstance AtomSource(this EntityInstance entry, EntitySetInstance sourceFeed)
        {
            ExceptionUtilities.CheckArgumentNotNull(entry, "entry");
            ExceptionUtilities.CheckArgumentNotNull(sourceFeed, "sourceFeed");

            var sourceAnnotations = sourceFeed.Annotations.OfType<XmlTreeAnnotation>();
            entry.AddAnnotation(
                XmlTreeAnnotation.Atom(
                    TestAtomConstants.AtomSourceElementName, 
                    null, 
                    sourceAnnotations.Select(a => (XmlTreeAnnotation)a.Clone()).ToArray()));

            return entry;
        }

        /// <summary>
        /// Annotates the feed with atom:subtitle values.
        /// </summary>
        /// <param name="feed">The feed to annotate.</param>
        /// <param name="subtitleValue">The value of the atom:subtitle element.</param>
        /// <param name="subtitleValueType">The type of the atom:subtitle value.</param>
        /// <returns>The feed with the annotation applied.</returns>
        public static EntitySetInstance AtomSubtitle(this EntitySetInstance feed, string subtitleValue, string subtitleValueType)
        {
            ExceptionUtilities.CheckArgumentNotNull(feed, "feed");

            return feed.AtomText(TestAtomConstants.AtomSubtitleElementName, subtitleValue, subtitleValueType);
        }

        /// <summary>
        /// Annotates the entry with atom:summary values.
        /// </summary>
        /// <param name="entry">The entry to annotate.</param>
        /// <param name="summaryValue">The value of the atom:summary element.</param>
        /// <param name="summaryValueType">The type of the atom:summary value.</param>
        /// <returns>The entry with the annotation applied.</returns>
        public static EntityInstance AtomSummary(this EntityInstance entry, string summaryValue, string summaryValueType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entry, "entry");

            return entry.AtomText(TestAtomConstants.AtomSummaryElementName, summaryValue, summaryValueType);
        }

        /// <summary>
        /// Annotates the payload element with atom:title values.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="subtitleValue">The value of the atom:title element.</param>
        /// <param name="subtitleValueType">The type of the atom:title value.</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AtomTitle<T>(this T payloadElement, string titleValue, string titleValueType) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            return payloadElement.AtomText(TestAtomConstants.AtomTitleElementName, titleValue, titleValueType);
        }

        /// <summary>
        /// Annotates the payload element with atom:updated value.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="updatedValue">The value of the atom:update element.</param>
        /// <returns>The payload element with the annotation applied.</returns>
        public static T AtomUpdated<T>(this T payloadElement, string updatedValue) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            payloadElement.Annotations.Add(XmlTreeAnnotation.Atom(TestAtomConstants.AtomUpdatedElementName, updatedValue));
            return payloadElement;
        }

        /// <summary>
        /// Annotates an ODataPayloadElement with an Atom Person construct.
        /// </summary>
        /// <typeparam name="T">The type of ODataPayloadElement.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="elementName">The name of the Person element.</param>
        /// <param name="personName">The value of the Person's Name property.</param>
        /// <param name="personUri">The value of the Person's Uri property.</param>
        /// <param name="personEmail">The value of the Person's Email Address property.</param>
        /// <returns>The payload element with annotation applied.</returns>
        private static T AtomPerson<T>(this T payloadElement, string elementName, string personName, string personUri, string personEmail) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var personChildren = CreateAtomElements(
                new KeyValuePair<string, string>(TestAtomConstants.AtomPersonNameElementName, personName),
                new KeyValuePair<string, string>(TestAtomConstants.AtomPersonUriElementName, personUri),
                new KeyValuePair<string, string>(TestAtomConstants.AtomPersonEmailElementName, personEmail));

            XmlTreeAnnotation person = XmlTreeAnnotation.Atom(elementName, null, personChildren);
            payloadElement.AddAnnotation(person);
            return payloadElement;
        }

        /// <summary>
        /// Annotates an ODataPayloadElement with an Atom Text construct.
        /// </summary>
        /// <typeparam name="T">The type of ODataPayloadElement.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="elementName">The name of the Text element.</param>
        /// <param name="textValue">The value of the Text element.</param>
        /// <returns>The payload element with annotation applied.</returns>
        private static T AtomText<T>(this T payloadElement, string elementName, string textValue, string textType) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var textAttributes = CreateAtomAttributes(new KeyValuePair<string,string>(TestAtomConstants.AtomTypeAttributeName, textType));

            payloadElement.AddAnnotation(XmlTreeAnnotation.Atom(elementName, textValue, textAttributes));
            return payloadElement;
        }

        /// <summary>
        /// Creates the XmlTreeAnnotation annotations that represent the attributes with non-null values.
        /// </summary>
        /// <param name="attributeNameValuePairs">The attribute name-value pairs to convert to Atom attribute annotations.</param>
        /// <returns>An XmlTreeAnnotation for each non-null value attribute.</returns>
        private static XmlTreeAnnotation[] CreateAtomAttributes(params KeyValuePair<string, string>[] attributeNameValuePairs)
        {
            var atomAttributes = new List<XmlTreeAnnotation>();
            foreach (var attribute in attributeNameValuePairs.Where(p => p.Value != null))
            {
                atomAttributes.Add(XmlTreeAnnotation.AtomAttribute(attribute.Key, attribute.Value));
            }
            return atomAttributes.ToArray();
        }
 
        /// <summary>
        /// Creates the XmlTreeAnnotation annotations that represent the elements with non-null values.
        /// </summary>
        /// <param name="attributeNameValuePairs">The element name-value pairs to convert to Atom element annotations.</param>
        /// <returns>An XmlTreeAnnotation for each non-null value element.</returns>
        private static XmlTreeAnnotation[] CreateAtomElements(params KeyValuePair<string, string>[] elementNameValuePairs)
        {
            var atomElements = new List<XmlTreeAnnotation>();
            foreach (var element in elementNameValuePairs.Where(p => p.Value != null))
            {
                atomElements.Add(XmlTreeAnnotation.Atom(element.Key, element.Value));
            }
            return atomElements.ToArray();
        }

        /// <summary>
        /// Creates the XmlTreeAnnotation annotation that represents the Atom Publishing Protocol (APP) element.
        /// </summary>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="value">The value of the element.</param>
        /// <param name="children">Annotations representing the element's children.</param>
        /// <returns>An XmlTreeAnnotation representing the APP element.</returns>
        private static XmlTreeAnnotation CreateAppElement(string localName, string value, params XmlTreeAnnotation[] children)
        {
            return XmlTreeAnnotation.Custom(localName, TestAtomConstants.AtomApplicationNamespace, "app", false, value, children);
        }

        /// <summary>
        /// Adds an XAttribute to the XElement, if the attribute's value is non-null.
        /// </summary>
        /// <param name="parentElement">The element to add the attribute to.</param>
        /// <param name="attributeName">The name of the attribute to add.</param>
        /// <param name="attributeValue">The value of the attribute to add.</param>
        private static void AddXAttribute(XElement parentElement, string attributeName, string attributeValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(parentElement, "parentElement");

            if (attributeValue != null)
            {
                parentElement.Add(new XAttribute(attributeName, attributeValue));
            }
        }
    }
}
