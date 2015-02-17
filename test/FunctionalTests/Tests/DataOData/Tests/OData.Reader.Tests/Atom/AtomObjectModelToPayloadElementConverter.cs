//---------------------------------------------------------------------
// <copyright file="AtomObjectModelToPayloadElementConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Converts OData object model to ODataPayloadElement model with Atom specific behavior.
    /// </summary>
    public class AtomObjectModelToPayloadElementConverter : ObjectModelToPayloadElementConverter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AtomObjectModelToPayloadElementConverter()
        {
        }

        /// <summary>
        /// Virtual method to create the visitor to perform the conversion.
        /// </summary>
        /// <param name="response">true if payload represents a response payload, false if it's a request payload.</param>
        /// <param name="payloadContainsId">Whether or not the payload contains identity values for entries.</param>
        /// <param name="payloadContainsEtagForType">A function for determining whether the payload contains etag property values for a given type.</param>
        /// <returns>The newly created visitor.</returns>
        protected override ObjectModelToPayloadElementConverterVisitor CreateVisitor(bool response, bool payloadContainsId, Func<string, bool> payloadContainsEtagForType)
        {
            return new AtomObjectModelToPayloadElementConverterVisitor(response, this.RequestManager);
        }

        /// <summary>
        /// The inner visitor which performs the conversion.
        /// </summary>
        protected class AtomObjectModelToPayloadElementConverterVisitor : ObjectModelToPayloadElementConverterVisitor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="response">true if payload represents a response payload, false if it's a request payload.</param>
            /// <param name="requestManager">The request manager used to convert batch payloads.</param>
            public AtomObjectModelToPayloadElementConverterVisitor(bool response, IODataRequestManager requestManager)
                : base(response, requestManager)
            {
            }

            /// <summary>
            /// Visits an entity reference links item.
            /// </summary>
            /// <param name="entityReferenceLinks">The entity reference links item to visit.</param>
            /// <returns>An ODataPayloadElement representing the entity reference links.</returns>
            protected override ODataPayloadElement VisitEntityReferenceLinks(ODataEntityReferenceLinks entityReferenceLinks)
            {
                ExceptionUtilities.CheckArgumentNotNull(entityReferenceLinks, "entityReferenceLinks");

                LinkCollection linkCollection = (LinkCollection)base.VisitEntityReferenceLinks(entityReferenceLinks);

                AtomLinkMetadata atomMetadata = entityReferenceLinks.GetAnnotation<AtomLinkMetadata>();
                if (atomMetadata != null)
                {
                    ConvertAtomLinkMetadata(atomMetadata, linkCollection);
                }

                return linkCollection;
            }

            /// <summary>
            /// Visits an entry item.
            /// </summary>
            /// <param name="entry">The entry item to visit.</param>
            /// <returns>An ODataPayloadElement representing the entry.</returns>
            protected override ODataPayloadElement VisitEntry(ODataEntry entry)
            {
                ExceptionUtilities.CheckArgumentNotNull(entry, "entry");

                EntityInstance entity = (EntityInstance)base.VisitEntry(entry);

                var atomMetadata = entry.GetAnnotation<AtomEntryMetadata>();
                if (atomMetadata != null)
                {
                    ConvertAtomEntryMetadata(atomMetadata, entity);
                }

                return entity;
            }

            /// <summary>
            /// Visits a feed item.
            /// </summary>
            /// <param name="feed">The feed item to visit.</param>
            /// <returns>An ODataPayloadElement representing the feed.</returns>
            protected override ODataPayloadElement VisitFeed(ODataFeed feed)
            {
                ExceptionUtilities.CheckArgumentNotNull(feed, "feed");

                EntitySetInstance entitySet = (EntitySetInstance)base.VisitFeed(feed);

                var atomMetadata = feed.GetAnnotation<AtomFeedMetadata>();
                if (atomMetadata != null)
                {
                    ConvertAtomFeedMetadata(atomMetadata, entitySet);
                }

                return entitySet;
            }

            /// <summary>
            /// Visits a navigation link item.
            /// </summary>
            /// <param name="navigationLink">The navigation link to visit.</param>
            /// <returns>An ODataPayloadElement representing the navigation link.</returns>
            protected override ODataPayloadElement VisitNavigationLink(ODataNavigationLink navigationLink)
            {
                ExceptionUtilities.CheckArgumentNotNull(navigationLink, "navigationLink");

                NavigationPropertyInstance navigationProperty = (NavigationPropertyInstance)base.VisitNavigationLink(navigationLink);

                // In ATOM even deferred links may know if they point to collection or singleton.
                // So add the content type annotation to them (through it IsCollection) so that comparison is precise.
                DeferredLink deferredLink = navigationProperty.Value as DeferredLink;
                if (deferredLink != null && navigationLink.IsCollection.HasValue)
                {
                    deferredLink.IsCollection(navigationLink.IsCollection.Value);
                }

                AtomLinkMetadata atomMetadata = navigationLink.GetAnnotation<AtomLinkMetadata>();
                if (atomMetadata != null && deferredLink != null)
                {
                    ConvertAtomLinkChildrenMetadata(atomMetadata, navigationProperty.Value);
                }

                return navigationProperty;
            }

            /// <summary>
            /// Visits a stream reference value item.
            /// </summary>
            /// <param name="streamReferenceValue">The stream reference value item to visit.</param>
            /// <returns>An ODataPayloadElement representing the stream reference value.</returns>
            protected override ODataPayloadElement VisitStreamReferenceValue(ODataStreamReferenceValue streamReferenceValue)
            {
                ExceptionUtilities.CheckArgumentNotNull(streamReferenceValue, "streamReferenceValue");

                NamedStreamInstance namedStreamInstance = (NamedStreamInstance)base.VisitStreamReferenceValue(streamReferenceValue);

                AtomStreamReferenceMetadata streamMetadata = streamReferenceValue.GetAnnotation<AtomStreamReferenceMetadata>();
                if (streamMetadata != null)
                {
                    if (streamMetadata.EditLink != null)
                    {
                        AtomLinkMetadata editMetadata = streamMetadata.EditLink;
                        namedStreamInstance.AtomNamedStreamLink(editMetadata.Href == null ? null : editMetadata.Href.OriginalString, editMetadata.Relation, editMetadata.MediaType, editMetadata.HrefLang, editMetadata.Title, ToString(editMetadata.Length));
                    }

                    if (streamMetadata.SelfLink != null)
                    {
                        AtomLinkMetadata selfMetadata = streamMetadata.SelfLink;
                        namedStreamInstance.AtomNamedStreamLink(selfMetadata.Href == null ? null : selfMetadata.Href.OriginalString, selfMetadata.Relation, selfMetadata.MediaType, selfMetadata.HrefLang, selfMetadata.Title, ToString(selfMetadata.Length));
                    }
                }

                return namedStreamInstance;
            }

            /// <summary>
            /// Visits a serviceDocument item.
            /// </summary>
            /// <param name="serviceDocument">The serviceDocument item to visit.</param>
            /// <returns>An ODataPayloadElement representing a service document enclosing the serviceDocument.</returns>
            protected override ODataPayloadElement VisitWorkspace(ODataServiceDocument serviceDocument)
            {
                ExceptionUtilities.CheckArgumentNotNull(serviceDocument, "serviceDocument");

                ServiceDocumentInstance wrappingServiceDocumentInstance = (ServiceDocumentInstance)base.VisitWorkspace(serviceDocument);

                WorkspaceInstance workspaceInstance = wrappingServiceDocumentInstance.Workspaces.Single();
                AtomWorkspaceMetadata atomMetadata = serviceDocument.GetAnnotation<AtomWorkspaceMetadata>();
                if (atomMetadata != null)
                {
                    if (atomMetadata.Title != null)
                    {
                        workspaceInstance.Title = atomMetadata.Title.Text;
                        workspaceInstance.AtomTitle(atomMetadata.Title.Text, ToString(atomMetadata.Title.Kind));
                    }
                }

                return wrappingServiceDocumentInstance;
            }

            /// <summary>
            /// Visits a resource collection.
            /// </summary>
            /// <param name="entitySetInfo">The resource collection to visit.</param>
            protected override ODataPayloadElement VisitResourceCollection(ODataEntitySetInfo entitySetInfo)
            {
                ExceptionUtilities.CheckArgumentNotNull(entitySetInfo, "entitySetInfo");

                ResourceCollectionInstance collectionInstance = (ResourceCollectionInstance)base.VisitResourceCollection(entitySetInfo);

                AtomResourceCollectionMetadata atomMetadata = entitySetInfo.GetAnnotation<AtomResourceCollectionMetadata>();
                if (atomMetadata != null)
                {
                    if (atomMetadata.Title != null)
                    {
                        collectionInstance.Title = atomMetadata.Title.Text;
                        collectionInstance.AtomTitle(atomMetadata.Title.Text, ToString(atomMetadata.Title.Kind));
                    }

                    if (atomMetadata.Accept != null)
                    {
                        collectionInstance.AppAccept(atomMetadata.Accept);
                    }

                    if (atomMetadata.Categories != null)
                    {
                        if (atomMetadata.Categories.Href != null)
                        {
                            collectionInstance.AppOutOfLineCategories(atomMetadata.Categories.Href.OriginalString);
                        }
                        else
                        {
                            collectionInstance.AppInlineCategories(
                                atomMetadata.Categories.Fixed == null ? null : (atomMetadata.Categories.Fixed.Value ? TestAtomConstants.AtomPublishingFixedYesValue : TestAtomConstants.AtomPublishingFixedNoValue),
                                atomMetadata.Categories.Scheme,
                                atomMetadata.Categories.Categories == null ? new XmlTreeAnnotation[0] :
                                    atomMetadata.Categories.Categories.Select(category => AtomMetadataBuilder.AtomCategory(category.Term, category.Scheme, category.Label)).ToArray());
                        }
                    }
                }

                return collectionInstance;
            }

            /// <summary>
            /// Converts the Object Model representation of Atom metadata for entries into appropriate PayloadElement annotations
            /// </summary>
            /// <param name="entryMetadata">the Atom entry metadata, in Object Model representation, to convert</param>
            /// <param name="entry">the EntityInstance to annotate</param>
            private static void ConvertAtomEntryMetadata(AtomEntryMetadata entryMetadata, EntityInstance entry)
            {
                ExceptionUtilities.CheckArgumentNotNull(entryMetadata, "entryMetadata");
                ExceptionUtilities.CheckArgumentNotNull(entry, "entry");

                foreach (var author in entryMetadata.Authors)
                {
                    entry.AtomAuthor(author.Name, author.Uri == null ? null : author.Uri.OriginalString, author.Email);
                }

                foreach (var category in entryMetadata.Categories)
                {
                    entry.AtomCategory(category.Term, category.Scheme, category.Label);
                }

                foreach (var contributor in entryMetadata.Contributors)
                {
                    entry.AtomContributor(contributor.Name, contributor.Uri == null? null : contributor.Uri.OriginalString, contributor.Email);
                }

                if (entryMetadata.EditLink != null)
                {
                    AtomLinkMetadata editLink = entryMetadata.EditLink;
                    entry.AtomEditLink(editLink.Href == null ? null : editLink.Href.OriginalString, editLink.MediaType, editLink.HrefLang, editLink.Title, ToString(editLink.Length));
                }

                foreach (var link in entryMetadata.Links)
                {
                    entry.AtomLink(link.Href == null ? null : link.Href.OriginalString, link.Relation, link.MediaType, link.HrefLang, link.Title, ToString(link.Length));
                }

                if (entryMetadata.Published.HasValue)
                {
                    entry.AtomPublished(ToString(entryMetadata.Published));
                }

                if (entryMetadata.Rights != null)
                {
                    entry.AtomRights(entryMetadata.Rights.Text, ToString(entryMetadata.Rights.Kind));
                }

                if (entryMetadata.SelfLink != null)
                {
                    AtomLinkMetadata selfLink = entryMetadata.SelfLink;
                    entry.AtomSelfLink(selfLink.Href == null ? null : selfLink.Href.OriginalString, selfLink.MediaType, selfLink.HrefLang, selfLink.Title, ToString(selfLink.Length));
                }

                if (entryMetadata.CategoryWithTypeName != null)
                {
                    AtomCategoryMetadata categoryWithTypeName = entryMetadata.CategoryWithTypeName;
                    entry.AtomCategoryWithTypeName(categoryWithTypeName.Term, categoryWithTypeName.Label);
                }

                if (entryMetadata.Source != null)
                {
                    EntitySetInstance tempSourceFeed = new EntitySetInstance();
                    ConvertAtomFeedMetadata(entryMetadata.Source, tempSourceFeed);
                    entry.AtomSource(tempSourceFeed);
                }

                if (entryMetadata.Summary != null)
                {
                    entry.AtomSummary(entryMetadata.Summary.Text, ToString(entryMetadata.Summary.Kind));
                }

                if (entryMetadata.Title != null)
                {
                    entry.AtomTitle(entryMetadata.Title.Text, ToString(entryMetadata.Title.Kind));
                }

                if (entryMetadata.Updated.HasValue)
                {
                    entry.AtomUpdated(ToString(entryMetadata.Updated));
                }
            }

            /// <summary>
            /// Converts the Object Model representation of Atom metadata for feeds into appropriate PayloadElement annotations
            /// </summary>
            /// <param name="feedMetadata">the Atom feed metadata, in Object Model representation, to convert</param>
            /// <param name="feed">the EntitySetInstance to annotate</param>
            private static void ConvertAtomFeedMetadata(AtomFeedMetadata feedMetadata, EntitySetInstance feed)
            {
                ExceptionUtilities.CheckArgumentNotNull(feedMetadata, "feedMetadata");
                ExceptionUtilities.CheckArgumentNotNull(feed, "feed");

                foreach (var author in feedMetadata.Authors)
                {
                    feed.AtomAuthor(author.Name, author.Uri == null ? null : author.Uri.OriginalString, author.Email);
                }

                foreach (var category in feedMetadata.Categories)
                {
                    feed.AtomCategory(category.Term, category.Scheme, category.Label);
                }

                foreach (var contributor in feedMetadata.Contributors)
                {
                    feed.AtomContributor(contributor.Name, contributor.Uri == null ? null : contributor.Uri.OriginalString, contributor.Email);
                }

                if (feedMetadata.Generator != null)
                {
                    feed.AtomGenerator(feedMetadata.Generator.Name, feedMetadata.Generator.Uri == null ? null : feedMetadata.Generator.Uri.OriginalString, feedMetadata.Generator.Version);
                }

                if (feedMetadata.Icon != null)
                {
                    feed.AtomIcon(feedMetadata.Icon.OriginalString);
                }

                foreach (var link in feedMetadata.Links)
                {
                    string linkLength = link.Length.HasValue ? link.Length.Value.ToString() : null;
                    feed.AtomLink(link.Href == null ? null : link.Href.OriginalString, link.Relation, link.MediaType, link.HrefLang, link.Title, linkLength);
                }

                if (feedMetadata.Logo != null)
                {
                    feed.AtomLogo(feedMetadata.Logo.OriginalString);
                }

                if (feedMetadata.Rights != null)
                {
                    feed.AtomRights(feedMetadata.Rights.Text, ToString(feedMetadata.Rights.Kind));
                }

                if (feedMetadata.SelfLink != null)
                {
                    AtomLinkMetadata selfLink = feedMetadata.SelfLink;
                    ExceptionUtilities.Assert(selfLink.Relation == TestAtomConstants.AtomSelfRelationAttributeValue, "The self link ATOM metadata must have the rel set to 'self'.");
                    string selfLinkLength = selfLink.Length.HasValue ? selfLink.Length.Value.ToString() : null;
                    feed.AtomSelfLink(selfLink.Href == null ? null : selfLink.Href.OriginalString, selfLink.MediaType, selfLink.HrefLang, selfLink.Title, selfLinkLength);
                }

                if (feedMetadata.NextPageLink != null)
                {
                    AtomLinkMetadata nextPageLink = feedMetadata.NextPageLink;
                    ExceptionUtilities.Assert(nextPageLink.Relation == TestAtomConstants.AtomNextRelationAttributeValue, "The next page link ATOM metadata must have the rel set to 'next'.");
                    string nextPageLinkLength = nextPageLink.Length.HasValue ? nextPageLink.Length.Value.ToString() : null;
                    feed.AtomNextPageLink(nextPageLink.Href == null ? null : nextPageLink.Href.OriginalString, nextPageLink.MediaType, nextPageLink.HrefLang, nextPageLink.Title, nextPageLinkLength);
                }

                if (feedMetadata.SourceId != null)
                {
                    // This should only occur when the metadata comes from the source element of an entry
                    // and we are annotating a temporary feed instance
                    feed.AtomId(UriUtils.UriToString(feedMetadata.SourceId));
                }

                if (feedMetadata.Subtitle != null)
                {
                    feed.AtomSubtitle(feedMetadata.Subtitle.Text, ToString(feedMetadata.Subtitle.Kind));
                }

                if (feedMetadata.Title != null)
                {
                    feed.AtomTitle(feedMetadata.Title.Text, ToString(feedMetadata.Title.Kind));
                }

                if (feedMetadata.Updated.HasValue)
                {
                    feed.AtomUpdated(ToString(feedMetadata.Updated));
                }
            }

            /// <summary>
            /// Converts the Object Model representation of Atom link metadata into appropriate annotations for a link ODataPayloadElement.
            /// </summary>
            /// <param name="linkMetadata">The Atom link metadata, in Object Model representation, to convert.</param>
            /// <param name="payloadElement">The payload element to annotate.</param>
            private static void ConvertAtomLinkMetadata(AtomLinkMetadata linkMetadata, ODataPayloadElement payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(linkMetadata, "linkMetadata");
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "link");

                string lengthValue = linkMetadata.Length.HasValue ? linkMetadata.Length.Value.ToString() : null;
                payloadElement.AtomLink(linkMetadata.Href == null ? null : linkMetadata.Href.OriginalString, linkMetadata.Relation, linkMetadata.MediaType, linkMetadata.HrefLang, linkMetadata.Title, lengthValue);
            }
        
            /// <summary>
            /// Converts the Object Model representation of Atom link metadata into appropriate annotations for a payload element representing a link.
            /// </summary>
            /// <param name="linkMetadata">The Atom link metadata, in Object Model representation, to convert.</param>
            /// <param name="linkPayloadElement">The link payload element to annotate.</param>
            /// <remarks>This method is only for use with payload elements that represent links, as it will skip over the root link annotation.</remarks>
            private static void ConvertAtomLinkChildrenMetadata(AtomLinkMetadata linkMetadata, ODataPayloadElement linkPayloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(linkMetadata, "linkMetadata");
                ExceptionUtilities.CheckArgumentNotNull(linkPayloadElement, "linkPayloadElement");

                // Since the payload element already represents a link, we annotate a temporary element 
                // and copy the "children" annotations onto the actual payload element.
                var tempPayloadElement = new EntityInstance();
                ExceptionUtilities.Assert(!tempPayloadElement.Annotations.OfType<XmlTreeAnnotation>().Any(), "Payload element should not have XmlTreeAnnotations after construction");
                ConvertAtomLinkMetadata(linkMetadata, tempPayloadElement);

                XmlTreeAnnotation linkAnnotation = tempPayloadElement.Annotations.OfType<XmlTreeAnnotation>().Single();
                foreach (XmlTreeAnnotation childAnnotation in linkAnnotation.Children)
                {
                    linkPayloadElement.AddAnnotation(childAnnotation);
                }
            }

            /// <summary>
            /// Converts a given <see cref="AtomTextConstructKind"/> to a string appropriate for Atom format.
            /// </summary>
            /// <param name="textConstructKind">The text construct kind to convert.</param>
            /// <returns>The string version of the text construct format in Atom format.</returns>
            private static string ToString(AtomTextConstructKind textConstructKind)
            {
                // This code copied from product's internal AtomValueUtils.ToString method
                ExceptionUtilities.CheckArgumentNotNull(textConstructKind, "textConstructorKind");

                switch (textConstructKind)
                {
                    case AtomTextConstructKind.Text:
                        return TestAtomConstants.AtomTextConstructTextKind;
                    case AtomTextConstructKind.Html:
                        return TestAtomConstants.AtomTextConstructHtmlKind;
                    case AtomTextConstructKind.Xhtml:
                        return TestAtomConstants.AtomTextConstructXHtmlKind;
                    default:
                        throw new TaupoArgumentException("Unknown AtomTextConstructKind " + textConstructKind.ToString());
                }
            }

            /// <summary>
            /// Converts a given nullable <see cref="DateTimeOffset"/> to a string.
            /// </summary>
            /// <param name="dateTimeOffset">The nullable DateTimeOffset instance to convert.</param>
            /// <returns>The string version of the DateTimeOffset, or null if the object is null.</returns>
            private static string ToString(DateTimeOffset? dateTimeOffset)
            {
                return dateTimeOffset.HasValue ? dateTimeOffset.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture) : null;
            }

            /// <summary>
            /// Converts a given nullable integer to a string
            /// </summary>
            /// <param name="nullableInteger">The nullable integer to convert.</param>
            /// <returns>The string version of the integer, or null if the integer is null.</returns>
            private static string ToString(int? nullableInteger)
            {
                return nullableInteger.HasValue ? nullableInteger.ToString() : null;
            }
        }
    }
}
