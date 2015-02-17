//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementToObjectModelConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    public class ODataPayloadElementToObjectModelConverter : ODataPayloadElementVisitorBase
    {
        private Stack<object> items = new Stack<object>();
        private int currentPropertyPosition = 0;
        private readonly bool response;
        
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="response">true if the payload we're converting is a response, false if it's a request.</param>
        public ODataPayloadElementToObjectModelConverter(bool response)
        {
            this.response = response;
        }

        public object Convert(ODataPayloadElement payload)
        {
            this.items.Clear();
            payload.Accept(this);
            return items.SingleOrDefault();
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
                SerializationInfo = new ODataFeedAndEntrySerializationInfo() {
                    NavigationSourceEntityTypeName = "Null",
                    NavigationSourceName = "MySet",
                    ExpectedTypeName = "Null"
                }
            };

            var idAnnotation = payloadElement.Annotations.Where(a => 
                {
                    var annotation = a as XmlTreeAnnotation;
                    if (annotation != null)
                        return annotation.LocalName.Equals("id");
                    return false;
                }).SingleOrDefault();
            
            if (idAnnotation != null)
            {
                feed.Id = new Uri((idAnnotation as XmlTreeAnnotation).PropertyValue, UriKind.Absolute);
            }

            if (payloadElement.NextLink != null)
            {
                feed.NextPageLink = new Uri(payloadElement.NextLink);
            }

            AddFeedMetadata(payloadElement, feed);

            if (this.items.Count > 0 && this.items.Peek() is ODataNavigationLink)
            {
                var currentLink = this.items.Peek() as ODataNavigationLink;
                ExceptionUtilities.CheckObjectNotNull(currentLink, "Feed can only exist at top level or inside a navigation link");
                currentLink.SetAnnotation(new ODataNavigationLinkExpandedItemObjectModelAnnotation() { ExpandedItem = feed });
            }

            try
            {
                items.Push(feed);
                base.Visit(payloadElement);
            }
            finally
            {
                feed = (ODataFeed)items.Pop();
            }

            // If we are at the top level push this to items to make it the result.
            if (this.items.Count == 0)
            {
                this.items.Push(feed);
            }
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
                Id = string.IsNullOrEmpty(entryId) ? null : new Uri(entryId),
                ETag = payloadElement.ETag,
                EditLink = string.IsNullOrEmpty(editLinkString) ? null : new Uri(editLinkString),
                ReadLink = string.IsNullOrEmpty(selfLinkString) ? null : new Uri(selfLinkString),
                TypeName = payloadElement.FullTypeName,
                Properties = new List<ODataProperty>(),
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

            if (this.items.Count > 0)
            {
                var parent = this.items.Peek();
                var navLink = parent as ODataNavigationLink;
                if (navLink != null)
                {
                    navLink.SetAnnotation(new ODataNavigationLinkExpandedItemObjectModelAnnotation() { ExpandedItem = entry });
                }
                else
                {
                    var feed = parent as ODataFeed;
                    ExceptionUtilities.CheckObjectNotNull(feed, "Feed was expected");
                    ODataFeedEntriesObjectModelAnnotation annotation = feed.GetAnnotation<ODataFeedEntriesObjectModelAnnotation>();
                    if (annotation == null)
                    {
                        annotation = new ODataFeedEntriesObjectModelAnnotation();
                        feed.SetAnnotation(annotation);
                    }
                    annotation.Add(entry);
                }
            }

            //Initialise stuff for the entry
            var originalPosition = this.currentPropertyPosition;
            this.currentPropertyPosition = 0;

            try
            {
                this.items.Push(entry);
                // visit the properties (incl. navigation properties and named stream properties)
                base.Visit(payloadElement);
            }
            finally
            {
                entry = (ODataEntry) this.items.Pop();
            }

            //Return to original values
            this.currentPropertyPosition = originalPosition;

            //If we are at the top level push this to items to make it the result.
            if (this.items.Count == 0)
            {
                this.items.Push(entry);
            }
        }

        /// <summary>
        /// Not supported; we collected primitive values already.
        /// <param name="payloadElement">A primitive value payload.</param>
        public override void Visit(PrimitiveValue payloadElement)
        {
            ExceptionUtilities.Assert(this.items.Count == 0, "This should not get called unless primitive is top level");
            this.items.Push(payloadElement.ClrValue);
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

            var parent = this.items.Peek();

            // not calling the base since we don't want to visit the value
            //If there is no parent then this is top level and add to items
            //otherwise add to parent.
            if (parent == null)
            {
                this.items.Push(odataProperty);
            }
            else
            {
                var entry = parent as ODataEntry;
                if (entry != null)
                {
                    var properties = (List<ODataProperty>)entry.Properties;
                    properties.Add(odataProperty);
                }

                var complexValue = parent as ODataComplexValue;
                if (complexValue != null)
                {
                    var properties = (List<ODataProperty>)complexValue.Properties;
                    properties.Add(odataProperty);
                }
            }
        }

        /// <summary>
        /// Creates a new MultiValue property and sets the value to the collection of primitive values.
        /// </summary>
        /// <param name="payloadElement">The primitive collection property to process.</param>
        public override void Visit(PrimitiveMultiValueProperty payloadElement)
        {
            var parent = this.items.Peek();
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
            //If there is no parent then this is top level and add to items
            //otherwise add to parent.
            if (parent == null)
            {
                this.items.Push(odataProperty);
            }
            else
            {
                var entry = parent as ODataEntry;
                if (entry != null)
                {
                    var properties = (List<ODataProperty>)entry.Properties;
                    properties.Add(odataProperty);
                }

                var complexValue = parent as ODataComplexValue;
                if (complexValue != null)
                {
                    var properties = (List<ODataProperty>)complexValue.Properties;
                    properties.Add(odataProperty);
                    complexValue.Properties = properties;
                }
            }
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

            var parent = this.items.Peek();

            // if parent is null this is top level and we should push the property onto items
            // otherwise we add this property to the parent.
            if (parent == null)
            {
                this.items.Push(odataProperty);
            }
            else
            {
                var entry = parent as ODataEntry;
                if (entry != null)
                {
                    var properties = (List<ODataProperty>)entry.Properties;
                    properties.Add(odataProperty);
                }

                var complexValue = parent as ODataComplexValue;
                if (complexValue != null)
                {
                    var properties = (List<ODataProperty>)complexValue.Properties;
                    properties.Add(odataProperty);
                }
            }
        }

        /// <summary>
        /// Creates a new complex property; does not set a value that will happen in Visit(ComplexInstance)
        /// </summary>
        /// <param name="payloadElement">The primitive collection property to process.</param>
        public override void Visit(ComplexProperty payloadElement)
        {
            var value = new ODataComplexValue()
            {
                TypeName = payloadElement.Value.FullTypeName,
                Properties = new List<ODataProperty>()
            };

            this.items.Push(value);
            base.Visit(payloadElement);

            var odataProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = value
            };

            //remove ComplexValue from items as it will be added to parent or added as property
            this.items.Pop();

            object parent = null;
            if (this.items.Count > 0)
            {
                parent = this.items.Peek();
            }

            if (parent != null)
            {
                var entry = parent as ODataEntry;
                if (entry != null)
                {
                    var properties = (List<ODataProperty>)entry.Properties;
                    properties.Add(odataProperty);
                }

                var complexValue = parent as ODataComplexValue;
                if (complexValue != null)
                {
                    var properties = (List<ODataProperty>)complexValue.Properties;
                    properties.Add(odataProperty);
                }

                var collection = parent as ODataCollectionValue;
                if (collection != null)
                {
                    var items = (List<object>)collection.Items;
                    items.Add(odataProperty);
                }
            }
            else
            {
                //since there is no parent add property to items
                this.items.Push(odataProperty);
            }
        }

        /// <summary>
        /// Creates a new complex value. If the complex instance is part of a complex collection value 
        /// adds the complex value to MultiValue property's items collection; otherwise sets the complex value as 
        /// value of a complex property.
        /// </summary>
        /// <param name="payloadElement"></param>
        public override void Visit(ComplexInstance payloadElement)
        {
            // figure out whether the complex value is part of a collection property (of complex values),
            // the value for a complex property, or top level
            object parent = null;
            if (this.items.Any())
            {
                parent = this.items.Peek();
            }

            ODataComplexValue complexValue = null;
            if (parent != null)
            {
                var currentMultiValue = parent as ODataCollectionValue;
                if (currentMultiValue != null)
                {
                    // create a new complex value and add it to the MultiValue
                    complexValue = new ODataComplexValue()
                    {
                        TypeName = payloadElement.FullTypeName,
                        Properties = new List<ODataProperty>()
                    };

                    // we construct the MultiValue so we know that the items are a list
                    IList itemsAsList = currentMultiValue.Items as IList;
                    itemsAsList.Add(complexValue);
                }

                var currentComplex = parent as ODataComplexValue;
                if (currentComplex != null)
                {
                    complexValue = currentComplex;
                }
            }
            else
            {
                complexValue = new ODataComplexValue()
                {
                    TypeName = payloadElement.FullTypeName,
                    Properties = new List<ODataProperty>()
                };
            }

            this.items.Push(complexValue);

            base.Visit(payloadElement);

            //if parent exists
            if (parent != null)
            {
                this.items.Pop();
            }
        }

        /// <summary>
        /// Creates a new MultiValue property and sets the value to an empty collection that will be filled when visiting the complex instances.
        /// </summary>
        /// <param name="payloadElement">The complex MultiValue property to process.</param>
        public override void Visit(ComplexMultiValueProperty payloadElement)
        {
            object parent = null;
            if (this.items.Any())
            {
                parent = this.items.Peek();
            }

            var value = new ODataCollectionValue()
            {
                Items = new List<object>(),
                TypeName = payloadElement.Value.FullTypeName
            };

            this.items.Push(value);
            base.Visit(payloadElement);

            var odataProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = value
            };
            
            if (parent != null)
            {
                var entry = parent as ODataEntry;
                if (entry != null)
                {
                    var properties = (List<ODataProperty>)entry.Properties;
                    properties.Add(odataProperty);
                }

                var complexValue = parent as ODataComplexValue;
                if (complexValue != null)
                {
                    var properties = (List<ODataProperty>)complexValue.Properties;
                    properties.Add(odataProperty);
                }

                // Remove property from items as it has been added to parent.
                this.items.Pop();
            }
            else
            {
                // Remove the ODataCollectionValue and add the ODataProperty.
                this.items.Pop();
                this.items.Push(odataProperty);
            }
        }

        /// <summary>
        /// Calls the base class method to process the instance collection.
        /// </summary>
        /// <param name="payloadElement">The complex instance collection to process.</param>
        public override void Visit(ComplexInstanceCollection payloadElement)
        {
            ODataCollectionStart collectionStart = new ODataCollectionStart();
            this.items.Push(collectionStart);
            var annotation = new ODataCollectionItemsObjectModelAnnotation();
            foreach (var complex in payloadElement)
            {
                this.items.Push(new ODataComplexValue()
                {
                    TypeName = complex.FullTypeName
                });

                this.Recurse(complex);
                annotation.Add(this.items.Pop());
            }

            collectionStart.SetAnnotation<ODataCollectionItemsObjectModelAnnotation>(annotation);
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
            ODataCollectionStart collectionStart = new ODataCollectionStart();
            var annotation = new ODataCollectionItemsObjectModelAnnotation();
            
            foreach (var primitive in payloadElement)
            {
                annotation.Add(primitive.ClrValue);
            }

            collectionStart.SetAnnotation<ODataCollectionItemsObjectModelAnnotation>(annotation);
        }

        /// <summary>
        /// Throws an InvalidOperationExcption; we expect null properties to be removed by the normalizer.
        /// </summary>
        /// <param name="payloadElement">The null property to process.</param>
        public override void Visit(NullPropertyInstance payloadElement)
        {
            var parent = this.items.Peek();
            ODataProperty property = new ODataProperty
            {
                Name = payloadElement.Name,
                Value = null
            };

            if (parent == null)
            {
                this.items.Push(property);
            }
            else
            { 
                var entry = parent as ODataEntry;
                if (entry != null)
                {
                    var properties = (List<ODataProperty>)entry.Properties;
                    properties.Add(property);
                }

                var complex = parent as ODataComplexValue;
                if (complex != null)
                {
                    var properties = (List<ODataProperty>)complex.Properties;
                    properties.Add(property);
                }
            }
        }

        /// <summary>
        /// Calls the base class method to process an empty payload.
        /// </summary>
        /// <param name="payloadElement">The empty payload to process.</param>
        public override void Visit(EmptyPayload payloadElement)
        {
            base.Visit(payloadElement);
        }

        public override void Visit(ODataErrorPayload payloadElement)
        {
            ExceptionUtilities.Assert(this.items.Count == 0, "Error should only be top level");
            var error = new ODataError()
            {
                ErrorCode = payloadElement.Code,
                Message = payloadElement.Message
            };

            this.items.Push(error);
            base.Visit(payloadElement);
        }

        public override void Visit(ODataInternalExceptionPayload payloadElement)
        {
            ODataInnerError innerError = new ODataInnerError()
            {
                Message = payloadElement.Message,
                TypeName = payloadElement.TypeName,
                StackTrace = payloadElement.StackTrace,
            };

            this.items.Push(innerError);
            base.Visit(payloadElement);
            
            //This should now have its innerError set if one exists
            innerError = this.items.Pop() as ODataInnerError;

            var higherLevel = this.items.Peek();
            var error = higherLevel as ODataError;
            if (error != null)
            {
                error.InnerError = innerError;
            }
            else
            {
                var higherLevelInnerError = higherLevel as ODataInnerError;
                ExceptionUtilities.CheckObjectNotNull(higherLevelInnerError, "Expected ODataError or ODataInnerError");
                higherLevelInnerError.InnerError = innerError;
            }
        }

        /// <summary>
        /// Processes a navigation property.
        /// </summary>
        /// <param name="payloadElement">The navigation property to process.</param>
        public override void Visit(NavigationPropertyInstance payloadElement)
        {
            var contentType = payloadElement.Annotations.Where(a => a is ContentTypeAnnotation).SingleOrDefault();
            bool? collection = null;
            if (contentType != null)
            {
                collection = contentType.StringRepresentation.Contains("feed");
            }

            var link = new ODataNavigationLink()
            {
                Name = payloadElement.Name,
                IsCollection = collection
            };

            if (payloadElement.AssociationLink != null)
            {
                link.AssociationLinkUrl = new Uri(payloadElement.AssociationLink.UriString);
            }
            try
            {
                this.items.Push(link);
                base.Visit(payloadElement);
            }
            finally
            {
                link = (ODataNavigationLink) this.items.Pop();
            }
        }

        /// <summary>
        /// Initializes an ODataNavigationLink instance for the deferred link payload.
        /// </summary>
        /// <param name="payloadElement">The deferred link to process.</param>
        public override void Visit(DeferredLink payloadElement)
        {
            if (this.items.Peek() is ODataNavigationLink)
            {
                ODataNavigationLink navigationLink = (ODataNavigationLink)this.items.Pop();
                navigationLink.Url = new Uri(payloadElement.UriString);
                AddLinkMetadata(payloadElement, navigationLink);

                var contentType = payloadElement.Annotations.Where(a => a is ContentTypeAnnotation).SingleOrDefault();
                if (contentType != null)
                {
                    navigationLink.IsCollection = contentType.StringRepresentation.Contains("feed");
                }

                var entry = (ODataEntry)this.items.Peek();

                var annotation = entry.GetAnnotation<ODataEntryNavigationLinksObjectModelAnnotation>();
                if (annotation == null)
                {
                    annotation = new ODataEntryNavigationLinksObjectModelAnnotation();
                    entry.SetAnnotation<ODataEntryNavigationLinksObjectModelAnnotation>(annotation);
                }

                annotation.Add(navigationLink, this.currentPropertyPosition);

                this.items.Push(navigationLink);

                if (!this.response)
                {
                    navigationLink.SetAnnotation(new ODataNavigationLinkExpandedItemObjectModelAnnotation()
                    {
                        ExpandedItem = new ODataEntityReferenceLink() { Url = navigationLink.Url }
                    });
                }

                base.Visit(payloadElement);
                this.currentPropertyPosition++;
            }
            else
            {
                this.items.Push(new ODataEntityReferenceLink() { Url = new Uri(payloadElement.UriString) });
            }
        }

        /// <summary>
        /// Initializes a new expanded ODataNavigationLink instance and visits the payload.
        /// </summary>
        /// <param name="payloadElement">The expanded link to visit.</param>
        public override void Visit(ExpandedLink payloadElement)
        {
            Debug.Assert(this.items.Peek() is ODataNavigationLink);
            ODataNavigationLink navigationLink = (ODataNavigationLink) this.items.Pop();
           
            navigationLink.IsCollection = !payloadElement.IsSingleEntity;
            // TODO, ckerer: where do I get the info whether this links is a singleton or collection?
            if (payloadElement.UriString != null)
            {
                navigationLink.Url = new Uri(payloadElement.UriString);
            }

            var entry = (ODataEntry) this.items.Peek();
            var annotation = entry.GetAnnotation<ODataEntryNavigationLinksObjectModelAnnotation>();
            if (annotation == null)
            {
                annotation = new ODataEntryNavigationLinksObjectModelAnnotation();
                entry.SetAnnotation<ODataEntryNavigationLinksObjectModelAnnotation>(annotation);
            }

            annotation.Add(navigationLink, this.currentPropertyPosition);
            this.items.Push(navigationLink);
            base.Visit(payloadElement);
            this.currentPropertyPosition++;
        }

        /// <summary>
        /// Throws a NotSupportedException as the ODataWriter does not support link collections.
        /// </summary>
        /// <param name="payloadElement">The link collection to process.</param>
        public override void Visit(LinkCollection payloadElement)
        {
            var newLinks = new List<ODataEntityReferenceLink>();
            foreach (var link in payloadElement)
            {
                newLinks.Add(new ODataEntityReferenceLink() { Url = new Uri(link.UriString) });
            }

            var links = new ODataEntityReferenceLinks()
            {
                Count = payloadElement.Count,
                Links = newLinks,
                NextPageLink = new Uri(payloadElement.NextLink)
            };
            this.items.Push(links);
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

            var parent = this.items.Peek();
            if (parent != null)
            {
                var entry = parent as ODataEntry;
                if (entry != null)
                {
                    var properties = (List<ODataProperty>)entry.Properties;
                    properties.Add(odataNamedStreamProperty);
                }

                var complex = parent as ODataComplexValue;
                if (complex != null)
                {
                    var properties = (List<ODataProperty>)complex.Properties;
                    properties.Add(odataNamedStreamProperty);
                }
            }
            
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
                if (epmTree.NamespaceName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomNamespace)
                {
                    if (metadata == null)
                    {
                        metadata = new AtomEntryMetadata();
                    }

                    string localName = epmTree.LocalName;
                    if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomAuthorElementName)
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
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomCategoryElementName)
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
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomContributorElementName)
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
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomIdElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        entry.Id = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : new Uri(epmTree.PropertyValue);
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomLinkElementName)
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
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomPublishedElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        metadata.Published = string.IsNullOrEmpty(epmTree.PropertyValue) ? (DateTimeOffset?)null : DateTimeOffset.Parse(epmTree.PropertyValue);
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomRightsElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        AtomTextConstructKind atomConstructKind = GetAtomConstructKind(epmTree.Children);
                        metadata.Rights = new AtomTextConstruct { Kind = atomConstructKind, Text = epmTree.PropertyValue };
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomSourceElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        metadata.Source = CreateFeedMetadata(epmTree.Children, null);
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomSummaryElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        AtomTextConstructKind atomConstructKind = GetAtomConstructKind(epmTree.Children);
                        metadata.Summary = new AtomTextConstruct { Kind = atomConstructKind, Text = epmTree.PropertyValue };
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomTitleElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        AtomTextConstructKind atomConstructKind = GetAtomConstructKind(epmTree.Children);
                        metadata.Title = new AtomTextConstruct { Kind = atomConstructKind, Text = epmTree.PropertyValue };
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomUpdatedElementName)
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
                    if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomCategoryTermAttributeName)
                    {
                        metadata.Term = epmTree.PropertyValue;
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomCategorySchemeAttributeName)
                    {
                        metadata.Scheme = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : epmTree.PropertyValue;
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomCategoryLabelAttributeName)
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
                if (epmTree.NamespaceName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomNamespace)
                {
                    if (metadata == null)
                    {
                        metadata = new AtomPersonMetadata();
                    }

                    string localName = epmTree.LocalName;
                    if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomPersonNameElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        metadata.Name = epmTree.PropertyValue;
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomPersonUriElementName)
                    {
                        Debug.Assert(!epmTree.IsAttribute);
                        metadata.Uri = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : new Uri(epmTree.PropertyValue);
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomPersonEmailElementName)
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
                    if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomTypeAttributeName)
                    {
                        Debug.Assert(epmTree.IsAttribute);
                        string epmTreeValue = epmTree.PropertyValue;
                        if (string.IsNullOrEmpty(epmTreeValue) || Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomTextConstructTextKind == epmTreeValue)
                        {
                            return AtomTextConstructKind.Text;
                        }
                        else if (Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomTextConstructHtmlKind == epmTreeValue)
                        {
                            return AtomTextConstructKind.Html;
                        }
                        else if (Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomTextConstructXHtmlKind == epmTreeValue)
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
                if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomLinkHrefAttributeName)
                {
                    Debug.Assert(epmTree.IsAttribute);
                    metadata.Href = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : new Uri(epmTree.PropertyValue);
                    initialized = true;
                }
                else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomLinkHrefLangAttributeName)
                {
                    Debug.Assert(epmTree.IsAttribute);
                    metadata.HrefLang = epmTree.PropertyValue;
                    initialized = true;
                }
                else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomLinkLengthAttributeName)
                {
                    Debug.Assert(epmTree.IsAttribute);
                    metadata.Length = string.IsNullOrEmpty(epmTree.PropertyValue) ? (int?)null : int.Parse(epmTree.PropertyValue);
                    initialized = true;
                }
                else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomLinkRelationAttributeName)
                {
                    Debug.Assert(epmTree.IsAttribute);
                    metadata.Relation = epmTree.PropertyValue;
                    initialized = true;
                }
                else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomLinkTitleAttributeName)
                {
                    Debug.Assert(epmTree.IsAttribute);
                    metadata.Title = epmTree.PropertyValue;
                    initialized = true;
                }
                else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomLinkTypeAttributeName)
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
                if (epmTree.NamespaceName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomNamespace)
                {
                    if (metadata == null)
                    {
                        metadata = new AtomFeedMetadata();
                    }

                    string localName = epmTree.LocalName;
                    if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomAuthorElementName)
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
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomCategoryElementName)
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
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomContributorElementName)
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
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomGeneratorElementName)
                    {
                        metadata.Generator = CreateGeneratorMetadata(epmTree);
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomIconElementName)
                    {
                        metadata.Icon = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : new Uri(epmTree.PropertyValue);
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomIdElementName)
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
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomLinkElementName)
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
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomLogoElementName)
                    {
                        metadata.Logo = string.IsNullOrEmpty(epmTree.PropertyValue) ? null : new Uri(epmTree.PropertyValue);
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomRightsElementName)
                    {
                        AtomTextConstructKind atomConstructKind = GetAtomConstructKind(epmTree.Children);
                        metadata.Rights = new AtomTextConstruct { Kind = atomConstructKind, Text = epmTree.PropertyValue };
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomSubtitleElementName)
                    {
                        AtomTextConstructKind atomConstructKind = GetAtomConstructKind(epmTree.Children);
                        metadata.Subtitle = new AtomTextConstruct { Kind = atomConstructKind, Text = epmTree.PropertyValue };
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomTitleElementName)
                    {
                        AtomTextConstructKind atomConstructKind = GetAtomConstructKind(epmTree.Children);
                        metadata.Title = new AtomTextConstruct { Kind = atomConstructKind, Text = epmTree.PropertyValue };
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomUpdatedElementName)
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
                    if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomGeneratorUriAttributeName)
                    {
                        metadata.Uri = string.IsNullOrEmpty(attribute.PropertyValue) ? null : new Uri(attribute.PropertyValue);
                    }
                    else if (localName == Microsoft.Test.Taupo.OData.Atom.TestAtomConstants.AtomGeneratorVersionAttributeName)
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
