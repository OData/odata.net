//---------------------------------------------------------------------
// <copyright file="PayloadElementODataWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    /// <summary>
    ///     Implementation of IPayloadElementODataWriter that converts payload elements into OData OM instances 
    ///     and uses an OData writer to write them.
    /// </summary>
    [ImplementationName(typeof(IPayloadElementODataWriter), "PayloadElementODataWriter")]
    public sealed class PayloadElementODataWriter : ODataPayloadElementVisitorBase, IPayloadElementODataWriter, IODataPayloadElementVisitor
    {
        private ODataWriter writer;
        private List<ODataProperty> currentProperties = new List<ODataProperty>();
        private List<NavigationPropertyInstance> currentNavigationProperties = new List<NavigationPropertyInstance>();
        private ODataNavigationLink currentLink;

        /// <summary>
        /// Write the <paramref name="element"/> payload element to the <paramref name="writer"/> OData writer.
        /// </summary>
        /// <param name="writer">The OData writer to write to.</param>
        /// <param name="element">The element to write.</param>
        public void WritePayload(ODataWriter writer, ODataPayloadElement element)
        {
            this.writer = writer;
            try
            {
                base.Recurse(element);
                this.writer.Flush();
            }
            finally
            {
                this.writer = null;
            }
        }

        /// <summary>
        /// Visits an entity set instance: creates a new ODataFeed instance, calls ODataWriter.WriteStart()
        /// before visiting the entries and then calls ODataWriter.WriteEnd()
        /// </summary>
        /// <param name="payloadElement">The entity set instance to write.</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            // create an ODataFeed and write it
            ODataFeed feed = new ODataFeed()
            {
                // NOTE: the required Id is set when processing the annotations in AddFeedMetadata()
                Count = payloadElement.InlineCount,
                SerializationInfo = new ODataFeedAndEntrySerializationInfo()
                {
                    NavigationSourceEntityTypeName = "Null",
                    NavigationSourceName = "MySet",
                    ExpectedTypeName = "Null"
                }
            };

            if (payloadElement.NextLink != null)
            {
                feed.NextPageLink = new Uri(payloadElement.NextLink);
            }

            AddFeedMetadata(payloadElement, feed);

            this.writer.WriteStart(feed);
            base.Visit(payloadElement);
            this.writer.WriteEnd();
        }

        /// <summary>
        /// Visits an entity instance: creates a new ODataEntry instance, collects and sets all the properties,
        /// calls ODataWriter.WriteStart(), then visits the navigation properties and calls ODataWriter.WriteEnd()
        /// </summary>
        /// <param name="payloadElement">The entity instance to write.</param>
        public override void Visit(EntityInstance payloadElement)
        {
            // create an ODataEntry and write it
            string editLinkString = payloadElement.GetEditLink();
            string selfLinkString = payloadElement.GetSelfLink();
            string entryId = payloadElement.Id;

            var entry = new ODataEntry()
            {
                Id = string.IsNullOrEmpty(entryId) ? null:new Uri(entryId),
                ETag = payloadElement.ETag,
                EditLink = string.IsNullOrEmpty(editLinkString) ? null : new Uri(editLinkString),
                ReadLink = string.IsNullOrEmpty(selfLinkString) ? null : new Uri(selfLinkString),
                TypeName = payloadElement.FullTypeName,
                SerializationInfo = new ODataFeedAndEntrySerializationInfo()
                {
                    NavigationSourceEntityTypeName = payloadElement.FullTypeName,
                    NavigationSourceName = "MySet",
                    ExpectedTypeName = payloadElement.FullTypeName
                }
            };

            if (payloadElement.IsMediaLinkEntry())
            {
                ODataStreamReferenceValue mediaResource = new ODataStreamReferenceValue();
                mediaResource.ContentType = payloadElement.StreamContentType;
                mediaResource.ETag = payloadElement.StreamETag;

                mediaResource.ReadLink = new Uri(payloadElement.StreamSourceLink);
                mediaResource.EditLink = new Uri(payloadElement.StreamEditLink);

                entry.MediaResource = mediaResource;
            }

            // TODO: add support for custom extensions at some point
            AddEntryMetadata(payloadElement, entry);

            Debug.Assert(this.currentNavigationProperties.Count == 0);
            Debug.Assert(this.currentProperties.Count == 0);

            // visit the properties (incl. navigation properties and named stream properties)
            base.Visit(payloadElement);

            // set the primitive and complex properties
            if (this.currentProperties.Count > 0)
            {
                entry.Properties = this.currentProperties;
                this.currentProperties = new List<ODataProperty>();
            }

            this.writer.WriteStart(entry);

            var navigationProperties = new ReadOnlyCollection<NavigationPropertyInstance>(this.currentNavigationProperties);
            this.currentNavigationProperties.Clear();

            // process the navigation properties/links
            for (int i = 0; i < navigationProperties.Count; ++i)
            {
                NavigationPropertyInstance navigationProperty = navigationProperties[i];
                this.currentLink = new ODataNavigationLink()
                {
                    Name = navigationProperty.Name
                };
                base.Visit(navigationProperty);
                Debug.Assert(this.currentLink == null);
            }

            this.writer.WriteEnd();
        }

        /// <summary>
        /// Not supported; we collected primitive values already.
        /// <param name="payloadElement">A primitive value payload.</param>
        public override void Visit(PrimitiveValue payloadElement)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a primitive ODataProperty and sets the value.
        /// </summary>
        /// <param name="payloadElement">The primitive property to process.</param>
        public override void Visit(PrimitiveProperty payloadElement)
        {
            var odataProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = payloadElement.Value.ClrValue,
            };

            // not calling the base since we don't want to visit the value

            this.currentProperties.Add(odataProperty);
        }

        /// <summary>
        /// Creates a new MultiValue property and sets the value to the collection of primitive values.
        /// </summary>
        /// <param name="payloadElement">The primitive collection property to process.</param>
        public override void Visit(PrimitiveMultiValueProperty payloadElement)
        {
            var odataProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = new ODataCollectionValue() 
                { 
                    Items = payloadElement.Value.Select(pv => pv.ClrValue),
                    TypeName = payloadElement.Value.FullTypeName
                }
            };

            // not calling the base since we don't want to visit the primitive values in the collection

            this.currentProperties.Add(odataProperty);
        }

        /// <summary>
        /// Creates a new empty MultiValue property.
        /// </summary>
        /// <param name="payloadElement">The empty collection property to process.</param>
        public override void Visit(EmptyCollectionProperty payloadElement)
        {
            var odataProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = new ODataCollectionValue()
                {
                    TypeName = payloadElement.Value.FullTypeName
                }
            };

            base.Visit(payloadElement);

            this.currentProperties.Add(odataProperty);
        }

        /// <summary>
        /// Creates a new complex property; does not set a value that will happen in Visit(ComplexInstance)
        /// </summary>
        /// <param name="payloadElement">The primitive collection property to process.</param>
        public override void Visit(ComplexProperty payloadElement)
        {
            var odataProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = new ODataComplexValue()
                {
                    TypeName = payloadElement.Value.FullTypeName
                }
            };
            this.currentProperties.Add(odataProperty);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Creates a new complex value. If the complex instance is part of a complex collection value 
        /// adds the complex value to MultiValue property's items collection; otherwise sets the complex value as 
        /// value of a complex property.
        /// </summary>
        /// <param name="payloadElement"></param>
        public override void Visit(ComplexInstance payloadElement)
        {
            // figure out whether the complex value is part of a collection property (of complex values) or
            // the value for a complex property
            ODataProperty currentProperty = this.currentProperties.Last();
            ODataCollectionValue currentMultiValue = currentProperty.Value as ODataCollectionValue;

            ODataComplexValue complexValue;
            if (currentMultiValue != null)
            {
                // create a new complex value and add it to the MultiValue
                complexValue = new ODataComplexValue();
                complexValue.TypeName = payloadElement.FullTypeName.Substring(11, payloadElement.FullTypeName.Length - 12); ;
                // we construct the MultiValue so we know that the items are a list
                IList itemsAsList = currentMultiValue.Items as IList;
                itemsAsList.Add(complexValue);
            }
            else
            {
                complexValue = (ODataComplexValue)currentProperty.Value;
                complexValue.TypeName = payloadElement.FullTypeName;
            }

            var previousProperties = this.currentProperties;
            this.currentProperties = new List<ODataProperty>();

            base.Visit(payloadElement);

            complexValue.Properties = this.currentProperties;
            
            this.currentProperties = previousProperties;
        }


        /// <summary>
        /// Creates a new MultiValue property and sets the value to an empty collection that will be filled when visiting the complex instances.
        /// </summary>
        /// <param name="payloadElement">The complex MultiValue property to process.</param>
        public override void Visit(ComplexMultiValueProperty payloadElement)
        {
            var odataProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = new ODataCollectionValue() 
                { 
                    Items = new List<object>(),
                    TypeName = payloadElement.Value.FullTypeName
                }
            };
            this.currentProperties.Add(odataProperty);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Calls the base class method to process the instance collection.
        /// </summary>
        /// <param name="payloadElement">The complex instance collection to process.</param>
        public override void Visit(ComplexInstanceCollection payloadElement)
        {
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Calls the base class method to process the empty (untyped) collection.
        /// </summary>
        /// <param name="payloadElement">The empty collection to process.</param>
        public override void Visit(EmptyUntypedCollection payloadElement)
        {
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Calls the base class method to process the primitive collection.
        /// </summary>
        /// <param name="payloadElement">The primitive collection to process.</param>
        public override void Visit(PrimitiveCollection payloadElement)
        {
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Throws an InvalidOperationExcption; we expect null properties to be removed by the normalizer.
        /// </summary>
        /// <param name="payloadElement">The null property to process.</param>
        public override void Visit(NullPropertyInstance payloadElement)
        {
            ODataProperty property = new ODataProperty
            {
                Name=payloadElement.Name,
                Value=null
            };

            this.currentProperties.Add(property );
        }

        /// <summary>
        /// Calls the base class method to process an empty payload.
        /// </summary>
        /// <param name="payloadElement">The empty payload to process.</param>
        public override void Visit(EmptyPayload payloadElement)
        {
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Processes a navigation property.
        /// </summary>
        /// <param name="payloadElement">The navigation property to process.</param>
        public override void Visit(NavigationPropertyInstance payloadElement)
        {
            this.currentNavigationProperties.Add(payloadElement);
        }

        /// <summary>
        /// Initializes an ODataNavigationLink instance for the deferred link payload.
        /// </summary>
        /// <param name="payloadElement">The deferred link to process.</param>
        public override void Visit(DeferredLink payloadElement)
        {
            Debug.Assert(this.currentLink != null);
            ODataNavigationLink navigationLink = this.currentLink;
            this.currentLink = null;

            // TODO, ckerer: where do I get the info whether this links is a singleton or collection?
            navigationLink.Url = new Uri(payloadElement.UriString);

            AddLinkMetadata(payloadElement, navigationLink);

            this.writer.WriteStart(navigationLink);
            base.Visit(payloadElement);
            this.writer.WriteEnd();
        }

        /// <summary>
        /// Initializes a new expanded ODataNavigationLink instance and visits the payload.
        /// </summary>
        /// <param name="payloadElement">The expanded link to visit.</param>
        public override void Visit(ExpandedLink payloadElement)
        {
            Debug.Assert(this.currentLink != null);
            ODataNavigationLink navigationLink = this.currentLink;
            this.currentLink = null;

            // TODO, ckerer: where do I get the info whether this links is a singleton or collection?
            navigationLink.Url = new Uri(payloadElement.UriString);

            this.writer.WriteStart(navigationLink);
            base.Visit(payloadElement);
            this.writer.WriteEnd();
        }

        /// <summary>
        /// Throws a NotSupportedException as the ODataWriter does not support link collections.
        /// </summary>
        /// <param name="payloadElement">The link collection to process.</param>
        public override void Visit(LinkCollection payloadElement)
        {
            throw new NotSupportedException("A link collection is not supported.");
        }

        /// <summary>
        /// Creates a new ODataStreamReferenceValue for the named stream and initializes it.
        /// </summary>
        /// <param name="payloadElement">The named stream to process.</param>
        public override void Visit(NamedStreamInstance payloadElement)
        {
            var odataNamedStream = new ODataStreamReferenceValue()
            {
                ETag = payloadElement.ETag,
                ContentType = payloadElement.EditLink == null ? payloadElement.SourceLinkContentType : payloadElement.EditLinkContentType,
                ReadLink = payloadElement.SourceLink == null ? null : new Uri(payloadElement.SourceLink),
                EditLink = payloadElement.EditLink == null ? null : new Uri(payloadElement.EditLink),
            };

            var odataNamedStreamProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = odataNamedStream
            };

            this.currentProperties.Add(odataNamedStreamProperty);
            base.Visit(payloadElement);
        }

        private static void AddFeedMetadata(EntitySetInstance payloadElement, ODataFeed feed)
        {
            AtomFeedMetadata metadata = CreateFeedMetadata(payloadElement.Annotations.OfType<XmlTreeAnnotation>(), feed);

            // Fix up metadata for baselining
            metadata = metadata.Fixup();

            if (metadata != null)
            {
                feed.SetAnnotation<AtomFeedMetadata>(metadata);
            }
        }

        private static void AddEntryMetadata(EntityInstance payloadElement, ODataEntry entry)
        {
            AtomEntryMetadata metadata = null;

            foreach (XmlTreeAnnotation epmTree in payloadElement.Annotations.OfType<XmlTreeAnnotation>())
            {
                if (epmTree.NamespaceName == TestAtomConstants.AtomNamespace)
                {
                    if (metadata == null)
                    {
                        metadata = new AtomEntryMetadata();
                    }

                    string localName = epmTree.LocalName;
                    if (localName == TestAtomConstants.AtomAuthorElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        AtomPersonMetadata author = CreateAuthorMetadata(epmTree.Children);

                        List<AtomPersonMetadata> authors;
                        if (metadata.Authors == null)
                        {
                            authors = new List<AtomPersonMetadata>();
                            metadata.Authors = authors;
                        }
                        else
                        {
                            authors = (List<AtomPersonMetadata>)metadata.Authors;
                        }
                        authors.Add(author);
                    }
                    else if (localName == TestAtomConstants.AtomCategoryElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        AtomCategoryMetadata category = CreateCategoryMetadata(epmTree.Children);

                        List<AtomCategoryMetadata> categories;
                        if (metadata.Categories == null)
                        {
                            categories = new List<AtomCategoryMetadata>();
                            metadata.Categories = categories;
                        }
                        else
                        {
                            categories = (List<AtomCategoryMetadata>)metadata.Categories;
                        }
                        categories.Add(category);
                    }
                    else if (localName == TestAtomConstants.AtomContributorElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        AtomPersonMetadata contributor = CreateAuthorMetadata(epmTree.Children);

                        List<AtomPersonMetadata> contributors;
                        if (metadata.Contributors == null)
                        {
                            contributors = new List<AtomPersonMetadata>();
                            metadata.Contributors = contributors;
                        }
                        else
                        {
                            contributors = (List<AtomPersonMetadata>)metadata.Contributors;
                        }
                        contributors.Add(contributor);
                    }
                    else if (localName == TestAtomConstants.AtomIdElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        entry.Id = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : new Uri(epmTree.PropertyValue);
                    }
                    else if (localName == TestAtomConstants.AtomLinkElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        AtomLinkMetadata link = CreateLinkMetadata(epmTree.Children);

                        List<AtomLinkMetadata> links;
                        if (metadata.Links == null)
                        {
                            links = new List<AtomLinkMetadata>();
                            metadata.Links = links;
                        }
                        else
                        {
                            links = (List<AtomLinkMetadata>)metadata.Links;
                        }
                        links.Add(link);
                    }
                    else if (localName == TestAtomConstants.AtomPublishedElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        metadata.Published = string.IsNullOrEmpty(epmTree.PropertyValue) ? (DateTimeOffset?)null : DateTimeOffset.Parse(epmTree.PropertyValue);
                    }
                    else if (localName == TestAtomConstants.AtomRightsElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        AtomTextConstructKind atomConstructKind = GetAtomConstructKind(epmTree.Children);
                        metadata.Rights = new AtomTextConstruct { Kind = atomConstructKind, Text = epmTree.PropertyValue };
                    }
                    else if (localName == TestAtomConstants.AtomSourceElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        metadata.Source = CreateFeedMetadata(epmTree.Children, null);
                    }
                    else if (localName == TestAtomConstants.AtomSummaryElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        AtomTextConstructKind atomConstructKind = GetAtomConstructKind(epmTree.Children);
                        metadata.Summary = new AtomTextConstruct { Kind = atomConstructKind, Text = epmTree.PropertyValue };
                    }
                    else if (localName == TestAtomConstants.AtomTitleElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        AtomTextConstructKind atomConstructKind = GetAtomConstructKind(epmTree.Children);
                        metadata.Title = new AtomTextConstruct { Kind = atomConstructKind, Text = epmTree.PropertyValue };
                    }
                    else if (localName == TestAtomConstants.AtomUpdatedElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        metadata.Updated = string.IsNullOrEmpty(epmTree.PropertyValue) ? (DateTimeOffset?)null : DateTimeOffset.Parse(epmTree.PropertyValue);
                    }
                    else
                    {
                        throw new NotSupportedException("Unsupported atom metadata '" + localName + "' found for entry!");
                    }
                }
            }

            // Fix up metadata for baselining
            metadata = metadata.Fixup();

            if (metadata != null)
            {
                entry.SetAnnotation<AtomEntryMetadata>(metadata);
            }
        }

        private static void AddLinkMetadata(DeferredLink payloadElement, ODataNavigationLink link)
        {
            AtomLinkMetadata metadata = CreateLinkMetadata(payloadElement.Annotations.OfType<XmlTreeAnnotation>());
            if (metadata != null)
            {
                link.SetAnnotation<AtomLinkMetadata>(metadata);
            }
        }

        private static AtomCategoryMetadata CreateCategoryMetadata(IEnumerable<XmlTreeAnnotation> children)
        {
            AtomCategoryMetadata metadata = null;

            foreach (XmlTreeAnnotation epmTree in children.Where(child => child.IsAttribute))
            {
                if (string.IsNullOrEmpty(epmTree.NamespaceName))
                {
                    if (metadata == null)
                    {
                        metadata = new AtomCategoryMetadata();
                    }

                    string localName = epmTree.LocalName;
                    if (localName == TestAtomConstants.AtomCategoryTermAttributeName)
                    {
                        metadata.Term = epmTree.PropertyValue;
                    }
                    else if (localName == TestAtomConstants.AtomCategorySchemeAttributeName)
                    {
                        metadata.Scheme = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : epmTree.PropertyValue;
                    }
                    else if (localName == TestAtomConstants.AtomCategoryLabelAttributeName)
                    {
                        metadata.Label = epmTree.PropertyValue;
                    }
                    else
                    {
                        throw new NotSupportedException("Unsupported metadata '" + localName + "' found for category.");
                    }
                }
            }

            return metadata;
        }

        private static AtomPersonMetadata CreateAuthorMetadata(IEnumerable<XmlTreeAnnotation> children)
        {
            AtomPersonMetadata metadata = null;

            foreach (XmlTreeAnnotation epmTree in children)
            {
                if (epmTree.NamespaceName == TestAtomConstants.AtomNamespace)
                {
                    if (metadata == null)
                    {
                        metadata = new AtomPersonMetadata();
                    }

                    string localName = epmTree.LocalName;
                    if (localName == TestAtomConstants.AtomPersonNameElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        metadata.Name = epmTree.PropertyValue;
                    }
                    else if (localName == TestAtomConstants.AtomPersonUriElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        metadata.Uri = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : new Uri(epmTree.PropertyValue);
                    }
                    else if (localName == TestAtomConstants.AtomPersonEmailElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        metadata.Email = epmTree.PropertyValue;
                    }
                }
            }

            return metadata;
        }

        private static AtomTextConstructKind GetAtomConstructKind(IEnumerable<XmlTreeAnnotation> children)
        {
            foreach (XmlTreeAnnotation epmTree in children.Where(child => child.IsAttribute))
            {
                if (string.IsNullOrEmpty(epmTree.NamespaceName))
                {
                    string localName = epmTree.LocalName;
                    if (localName == TestAtomConstants.AtomTypeAttributeName)
                    {
                        Debug.Assert(epmTree.IsAttribute);
                        string epmTreeValue = epmTree.PropertyValue;
                        if (string.IsNullOrEmpty(epmTreeValue) || TestAtomConstants.AtomTextConstructTextKind == epmTreeValue)
                        {
                            return AtomTextConstructKind.Text;
                        }
                        else if (TestAtomConstants.AtomTextConstructHtmlKind == epmTreeValue)
                        {
                            return AtomTextConstructKind.Html;
                        }
                        else if (TestAtomConstants.AtomTextConstructXHtmlKind == epmTreeValue)
                        {
                            return AtomTextConstructKind.Xhtml;
                        }
                        else
                        {
                            throw new NotSupportedException("Text construct kind " + epmTreeValue + " is not supported.");
                        }
                    }
                }
            }

            return AtomTextConstructKind.Text;
        }

        private static AtomLinkMetadata CreateLinkMetadata(IEnumerable<XmlTreeAnnotation> children)
        {
            AtomLinkMetadata metadata = new AtomLinkMetadata();
            bool initialized = false;

            foreach (XmlTreeAnnotation epmTree in children.Where(child => child.IsAttribute))
            {
                string localName = epmTree.LocalName;
                if (localName == TestAtomConstants.AtomLinkHrefAttributeName)
                {
                    Debug.Assert(epmTree.IsAttribute);
                    metadata.Href = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : new Uri(epmTree.PropertyValue);
                    initialized = true;
                }
                else if (localName == TestAtomConstants.AtomLinkHrefLangAttributeName)
                {
                    Debug.Assert(epmTree.IsAttribute);
                    metadata.HrefLang = epmTree.PropertyValue;
                    initialized = true;
                }
                else if (localName == TestAtomConstants.AtomLinkLengthAttributeName)
                {
                    Debug.Assert(epmTree.IsAttribute);
                    metadata.Length = string.IsNullOrEmpty(epmTree.PropertyValue) ? (int?)null : int.Parse(epmTree.PropertyValue);
                    initialized = true;
                }
                else if (localName == TestAtomConstants.AtomLinkRelationAttributeName)
                {
                    Debug.Assert(epmTree.IsAttribute);
                    metadata.Relation = epmTree.PropertyValue;
                    initialized = true;
                }
                else if (localName == TestAtomConstants.AtomLinkTitleAttributeName)
                {
                    Debug.Assert(epmTree.IsAttribute);
                    metadata.Title = epmTree.PropertyValue;
                    initialized = true;
                }
                else if (localName == TestAtomConstants.AtomLinkTypeAttributeName)
                {
                    Debug.Assert(epmTree.IsAttribute);
                    metadata.MediaType = epmTree.PropertyValue;
                    initialized = true;
                }
                else
                {
                    throw new NotSupportedException("Unsupported metadata '" + localName + "' found for a link.");
                }
            }

            return initialized ? metadata : null;
        }

        private static AtomFeedMetadata CreateFeedMetadata(IEnumerable<XmlTreeAnnotation> children, ODataFeed feed)
        {
            AtomFeedMetadata metadata = null;

            foreach (XmlTreeAnnotation epmTree in children)
            {
                if (epmTree.NamespaceName == TestAtomConstants.AtomNamespace)
                {
                    if (metadata == null)
                    {
                        metadata = new AtomFeedMetadata();
                    }

                    string localName = epmTree.LocalName;
                    if (localName == TestAtomConstants.AtomAuthorElementName)
                    {
                        AtomPersonMetadata author = CreateAuthorMetadata(epmTree.Children);

                        List<AtomPersonMetadata> authors;
                        if (metadata.Authors == null)
                        {
                            authors = new List<AtomPersonMetadata>();
                            metadata.Authors = authors;
                        }
                        else
                        {
                            authors = (List<AtomPersonMetadata>)metadata.Authors;
                        }
                        authors.Add(author);
                    }
                    else if (localName == TestAtomConstants.AtomCategoryElementName)
                    {
                        AtomCategoryMetadata category = CreateCategoryMetadata(epmTree.Children);

                        List<AtomCategoryMetadata> categories;
                        if (metadata.Categories == null)
                        {
                            categories = new List<AtomCategoryMetadata>();
                            metadata.Categories = categories;
                        }
                        else
                        {
                            categories = (List<AtomCategoryMetadata>)metadata.Categories;
                        }
                        categories.Add(category);
                    }
                    else if (localName == TestAtomConstants.AtomContributorElementName)
                    {
                        AtomPersonMetadata contributor = CreateAuthorMetadata(epmTree.Children);

                        List<AtomPersonMetadata> contributors;
                        if (metadata.Contributors == null)
                        {
                            contributors = new List<AtomPersonMetadata>();
                            metadata.Contributors = contributors;
                        }
                        else
                        {
                            contributors = (List<AtomPersonMetadata>)metadata.Contributors;
                        }
                        contributors.Add(contributor);
                    }
                    else if (localName == TestAtomConstants.AtomGeneratorElementName)
                    {
                        metadata.Generator = CreateGeneratorMetadata(epmTree);
                    }
                    else if (localName == TestAtomConstants.AtomIconElementName)
                    {
                        metadata.Icon = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : new Uri(epmTree.PropertyValue);
                    }
                    else if (localName == TestAtomConstants.AtomIdElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        Uri id = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : new Uri(epmTree.PropertyValue, UriKind.Absolute);
                        if (feed == null)
                        {
                            // we are creating the metadata for an entry's 'source' metadata;
                            // we don't have a feed to store the Id on so it has to go into metadata
                            metadata.SourceId = id;
                        }
                        else
                        {
                            feed.Id = id;
                        }
                    }
                    else if (localName == TestAtomConstants.AtomLinkElementName)
                    {
                        AtomLinkMetadata link = CreateLinkMetadata(epmTree.Children);

                        List<AtomLinkMetadata> links;
                        if (metadata.Links == null)
                        {
                            links = new List<AtomLinkMetadata>();
                            metadata.Links = links;
                        }
                        else
                        {
                            links = (List<AtomLinkMetadata>)metadata.Contributors;
                        }
                        links.Add(link);
                    }
                    else if (localName == TestAtomConstants.AtomLogoElementName)
                    {
                        metadata.Logo = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : new Uri(epmTree.PropertyValue);
                    }
                    else if (localName == TestAtomConstants.AtomRightsElementName)
                    {
                        AtomTextConstructKind atomConstructKind = GetAtomConstructKind(epmTree.Children);
                        metadata.Rights = new AtomTextConstruct { Kind = atomConstructKind, Text = epmTree.PropertyValue };
                    }
                    else if (localName == TestAtomConstants.AtomSubtitleElementName)
                    {
                        AtomTextConstructKind atomConstructKind = GetAtomConstructKind(epmTree.Children);
                        metadata.Subtitle = new AtomTextConstruct { Kind = atomConstructKind, Text = epmTree.PropertyValue };
                    }
                    else if (localName == TestAtomConstants.AtomTitleElementName)
                    {
                        AtomTextConstructKind atomConstructKind = GetAtomConstructKind(epmTree.Children);
                        metadata.Title = new AtomTextConstruct { Kind = atomConstructKind, Text = epmTree.PropertyValue };
                    }
                    else if (localName == TestAtomConstants.AtomUpdatedElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        metadata.Updated = string.IsNullOrEmpty(epmTree.PropertyValue) ? (DateTimeOffset?)null : DateTimeOffset.Parse(epmTree.PropertyValue);
                    }
                    else
                    {
                        throw new NotSupportedException("Unsupported atom metadata found!");
                    }
                }
            }

            return metadata;
        }

        private static AtomGeneratorMetadata CreateGeneratorMetadata(XmlTreeAnnotation epmTree)
        {
            AtomGeneratorMetadata metadata = new AtomGeneratorMetadata()
            {
                Name = epmTree.PropertyValue
            };

            foreach (XmlTreeAnnotation attribute in epmTree.Children.Where(child => child.IsAttribute))
            {
                if (string.IsNullOrEmpty(attribute.NamespaceName))
                {
                    string localName = attribute.LocalName;
                    if (localName == TestAtomConstants.AtomGeneratorUriAttributeName)
                    {
                        metadata.Uri = string.IsNullOrEmpty(attribute.PropertyValue) ? null : new Uri(attribute.PropertyValue);
                    }
                    else if (localName == TestAtomConstants.AtomGeneratorVersionAttributeName)
                    {
                        metadata.Version = attribute.PropertyValue;
                    }
                    else
                    {
                        throw new NotSupportedException("Unsupported metadata '" + localName + "' found for a generator.");
                    }
                }
            }

            return metadata;
        }

    }
}
