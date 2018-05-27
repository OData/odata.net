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
    using Microsoft.OData;
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
        /// Visits an entity set instance: creates a new ODataResourceSet instance, calls ODataWriter.WriteStart()
        /// before visiting the entries and then calls ODataWriter.WriteEnd()
        /// </summary>
        /// <param name="payloadElement">The entity set instance to write.</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            // create an ODataResourceSet and write it
            ODataResourceSet feed = new ODataResourceSet()
            {
                // NOTE: the required Id is set when processing the annotations in AddFeedMetadata()
                Count = payloadElement.InlineCount,
                SerializationInfo = new ODataResourceSerializationInfo()
                {
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

            if (this.items.Count > 0 && this.items.Peek() is ODataNestedResourceInfo)
            {
                var currentLink = this.items.Peek() as ODataNestedResourceInfo;
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
                feed = (ODataResourceSet)items.Pop();
            }

            // If we are at the top level push this to items to make it the result.
            if (this.items.Count == 0)
            {
                this.items.Push(feed);
            }
        }

        /// <summary>
        /// Visits an entity instance: creates a new ODataResource instance, collects and sets all the properties,
        /// calls ODataWriter.WriteStart(), then visits the navigation properties and calls ODataWriter.WriteEnd()
        /// </summary>
        /// <param name="payloadElement">The entity instance to write.</param>
        public override void Visit(EntityInstance payloadElement)
        {
            // create an ODataResource and write it
            string editLinkString = payloadElement.GetEditLink();
            string selfLinkString = payloadElement.GetSelfLink();
            string entryId = payloadElement.Id;

            var entry = new ODataResource()
            {
                Id = string.IsNullOrEmpty(entryId) ? null : new Uri(entryId),
                ETag = payloadElement.ETag,
                EditLink = string.IsNullOrEmpty(editLinkString) ? null : new Uri(editLinkString),
                ReadLink = string.IsNullOrEmpty(selfLinkString) ? null : new Uri(selfLinkString),
                TypeName = payloadElement.FullTypeName,
                Properties = new List<ODataProperty>(),
                SerializationInfo = new ODataResourceSerializationInfo()
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

            if (this.items.Count > 0)
            {
                var parent = this.items.Peek();
                var navLink = parent as ODataNestedResourceInfo;
                if (navLink != null)
                {
                    navLink.SetAnnotation(new ODataNavigationLinkExpandedItemObjectModelAnnotation() { ExpandedItem = entry });
                }
                else
                {
                    var feed = parent as ODataResourceSet;
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
                entry = (ODataResource) this.items.Pop();
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
                var entry = parent as ODataResource;
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
                var entry = parent as ODataResource;
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
                var entry = parent as ODataResource;
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
                var entry = parent as ODataResource;
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
                var entry = parent as ODataResource;
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
                var entry = parent as ODataResource;
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

            var link = new ODataNestedResourceInfo()
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
                link = (ODataNestedResourceInfo)this.items.Pop();
            }
        }

        /// <summary>
        /// Initializes an ODataNestedResourceInfo instance for the deferred link payload.
        /// </summary>
        /// <param name="payloadElement">The deferred link to process.</param>
        public override void Visit(DeferredLink payloadElement)
        {
            if (this.items.Peek() is ODataNestedResourceInfo)
            {
                ODataNestedResourceInfo navigationLink = (ODataNestedResourceInfo)this.items.Pop();
                navigationLink.Url = new Uri(payloadElement.UriString);

                var contentType = payloadElement.Annotations.Where(a => a is ContentTypeAnnotation).SingleOrDefault();
                if (contentType != null)
                {
                    navigationLink.IsCollection = contentType.StringRepresentation.Contains("feed");
                }

                var entry = (ODataResource)this.items.Peek();

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
        /// Initializes a new expanded ODataNestedResourceInfo instance and visits the payload.
        /// </summary>
        /// <param name="payloadElement">The expanded link to visit.</param>
        public override void Visit(ExpandedLink payloadElement)
        {
            Debug.Assert(this.items.Peek() is ODataNestedResourceInfo);
            ODataNestedResourceInfo navigationLink = (ODataNestedResourceInfo)this.items.Pop();

            navigationLink.IsCollection = !payloadElement.IsSingleEntity;
            // TODO, ckerer: where do I get the info whether this links is a singleton or collection?
            if (payloadElement.UriString != null)
            {
                navigationLink.Url = new Uri(payloadElement.UriString);
            }
            
            var entry = (ODataResource) this.items.Peek();

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
                var entry = parent as ODataResource;
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
    }
}
