//---------------------------------------------------------------------
// <copyright file="ObjectModelToPayloadElementConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Common.Batch;
    using ODataUri = Microsoft.Test.Taupo.Astoria.Contracts.OData.ODataUri;
    #endregion Namespaces

    /// <summary>
    /// Converts OData object model to ODataPayloadElement model.
    /// </summary>
    public class ObjectModelToPayloadElementConverter 
    {
        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ObjectModelToPayloadElementConverter()
        {
        }

        /// <summary>
        /// Converts the OData object mode <paramref name="odataObject"/> to an <see cref="ODataPayloadElement"/>.
        /// </summary>
        /// <param name="odataObject">The OData object model object, for example ODataItem, ODataProperty, ...</param>
        /// <param name="response">true if the <paramref name="odataObject"/> represents a response payload, false if it's a request payload.</param>
        /// <returns>The <see cref="ODataPayloadElement"/> instance which holds the same information.</returns>
        public ODataPayloadElement Convert(object odataObject, bool response)
        {
            return this.Convert(odataObject, response, true, (t) => true);
        }

        /// <summary>
        /// Converts the OData object mode <paramref name="odataObject"/> to an <see cref="ODataPayloadElement"/>.
        /// </summary>
        /// <param name="odataObject">The OData object model object, for example ODataItem, ODataProperty, ...</param>
        /// <param name="response">true if the <paramref name="odataObject"/> represents a response payload, false if it's a request payload.</param>
        /// <param name="payloadContainsId">Whether or not the payload contains identity values for entries.</param>
        /// <param name="payloadContainsEtagForType">A function for determining whether the payload contains etag property values for a given type.</param>
        /// <returns>The <see cref="ODataPayloadElement"/> instance which holds the same information.</returns>
        public ODataPayloadElement Convert(object odataObject, bool response, bool payloadContainsId, Func<string, bool> payloadContainsEtagForType) 
        {
            ObjectModelToPayloadElementConverterVisitor visitor = this.CreateVisitor(response, payloadContainsId, payloadContainsEtagForType);
            return visitor.Visit(odataObject);
        }

        /// <summary>
        /// Virtual method to create the visitor to perform the conversion.
        /// </summary>
        /// <param name="response">true if payload represents a response payload, false if it's a request payload.</param>
        /// <param name="payloadContainsId">Whether or not the payload contains identity values for entries.</param>
        /// <param name="payloadContainsEtagForType">A function for determining whether the payload contains etag property values for a given type.</param>
        /// <returns>The newly created visitor.</returns>
        protected virtual ObjectModelToPayloadElementConverterVisitor CreateVisitor(bool response, bool payloadContainsId, Func<string, bool> payloadContainsEtagForType)
        {
            return new ObjectModelToPayloadElementConverterVisitor(response, this.RequestManager, payloadContainsId, payloadContainsEtagForType);
        }

        /// <summary>
        /// The inner visitor which performs the conversion.
        /// </summary>
        protected class ObjectModelToPayloadElementConverterVisitor : ODataObjectModelVisitor<ODataPayloadElement>
        {
            /// <summary>
            /// true if payload represents a response payload, false if it's a request payload.
            /// </summary>
            private readonly bool response;

            /// <summary>
            /// The request manager used to convert batch payloads.
            /// </summary>
            private readonly IODataRequestManager requestManager;

            private bool payloadContainsIdentityMetadata;

            private Func<string, bool> payloadContainsEtagPropertiesForType; 

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="response">true if payload represents a response payload, false if it's a request payload.</param>
            /// <param name="requestManager">The request manager used to convert batch payloads.</param>
            public ObjectModelToPayloadElementConverterVisitor(bool response, IODataRequestManager requestManager)
                : this(response, requestManager, true, (t) => true)
            {
                
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="response">true if payload represents a response payload, false if it's a request payload.</param>
            /// <param name="requestManager">The request manager used to convert batch payloads.</param>
        /// <param name="payloadContainsIdentityMetadata">Whether or not the payload contains identity values for entries.</param>
        /// <param name="payloadContainsEtagPropertiesForType">A function for determining whether the payload contains etag property values for a given type.</param>
            public ObjectModelToPayloadElementConverterVisitor(bool response, IODataRequestManager requestManager, bool payloadContainsIdentityMetadata, Func<string, bool> payloadContainsEtagPropertiesForType) 
            {
                this.response = response;
                this.requestManager = requestManager;
                this.payloadContainsIdentityMetadata = payloadContainsIdentityMetadata;
                this.payloadContainsEtagPropertiesForType = payloadContainsEtagPropertiesForType;
            }

            /// <summary>
            /// Visits a feed item.
            /// </summary>
            /// <param name="feed">The feed to visit.</param>
            protected override ODataPayloadElement VisitFeed(ODataResourceSet resourceCollection)
            {
                ExceptionUtilities.CheckArgumentNotNull(resourceCollection, "feed");

                EntitySetInstance entitySet = new EntitySetInstance()
                {
                    InlineCount = resourceCollection.Count,
                    NextLink = resourceCollection.NextPageLink == null ? null : resourceCollection.NextPageLink.OriginalString
                };

                if (resourceCollection.Id != null)
                {
                    entitySet.AtomId(UriUtils.UriToString(resourceCollection.Id));
                }

                // now check for the entries annotation on the feed
                IEnumerable<ODataResource> entries = resourceCollection.Entries();
                if (entries != null)
                {
                    foreach (ODataResource entry in entries)
                    {
                        entitySet.Add(this.Visit(entry));
                    }
                }

                return entitySet;
            }

            /// <summary>
            /// Visits an entry item.
            /// </summary>
            /// <param name="entry">The entry to visit.</param>
            protected override ODataPayloadElement VisitEntry(ODataResource entry)
            {
                ExceptionUtilities.CheckArgumentNotNull(entry, "entry");

                EntityInstance entity = new EntityInstance(entry.TypeName, false);

                if (this.payloadContainsIdentityMetadata)
                {
                    entity.Id = entry.Id == null ? null : entry.Id.OriginalString;

                    if (entry.ReadLink != null)
                    {
                        entity.WithSelfLink(entry.ReadLink.OriginalString);
                    }

                    if (entry.EditLink != null)
                    {
                        entity.WithEditLink(entry.EditLink.OriginalString);
                    }

                }

                if (this.payloadContainsEtagPropertiesForType(entry.TypeName))
                {
                    entity.ETag = entry.ETag;
                }

                ODataStreamReferenceValue mediaResource = entry.MediaResource;
                if (mediaResource != null)
                {
                    entity = entity.AsMediaLinkEntry();
                    NamedStreamInstance namedStreamInstance = (NamedStreamInstance)this.Visit(mediaResource);
                    entity.StreamETag = namedStreamInstance.ETag;
                    entity.StreamSourceLink = namedStreamInstance.SourceLink;
                    entity.StreamEditLink = namedStreamInstance.EditLink;
                    entity.StreamContentType = namedStreamInstance.SourceLinkContentType ?? namedStreamInstance.EditLinkContentType;

                    foreach (var namedStreamAtomLinkMetadataAnnotation in namedStreamInstance.Annotations.OfType<NamedStreamAtomLinkMetadataAnnotation>())
                    {
                        entity.AddAnnotation(namedStreamAtomLinkMetadataAnnotation);
                    }
                }

                // convert properties and navigation links
                entry.ProcessPropertiesPreservingPayloadOrder(
                    property => entity.Add((PropertyInstance)this.Visit(property)),
                    navigationLink => entity.Add((PropertyInstance)this.Visit(navigationLink)));

                var actions = entry.Actions;
                if (actions != null)
                {
                    foreach (var operation in actions)
                    {
                        var serviceOperationDescriptor = this.Visit(operation) as ServiceOperationDescriptor;
                        entity.Add(serviceOperationDescriptor);
                    }
                }

                var functions = entry.Functions;
                if (functions != null)
                {
                    foreach (var operation in functions)
                    {
                        var serviceOperationDescriptor = this.Visit(operation) as ServiceOperationDescriptor;
                        entity.Add(serviceOperationDescriptor);
                    }
                }

                this.ConvertSerializationTypeNameAnnotation(entry, entity);

                return entity;
            }

            /// <summary>
            /// Visits a property item.
            /// </summary>
            /// <param name="property">The property to visit.</param>
            protected override ODataPayloadElement VisitProperty(ODataProperty property)
            {
                ExceptionUtilities.CheckArgumentNotNull(property, "property");

                object value = property.Value;
                if (value == null)
                {
                    return new PrimitiveProperty() { Name = property.Name, Value = new PrimitiveValue(null, null) };
                }
                else
                {
                    ODataCollectionValue collectionValue = value as ODataCollectionValue;
                    if (collectionValue != null)
                    {
                        ODataPayloadElementCollection collection = (ODataPayloadElementCollection)this.Visit(collectionValue);
                        ComplexMultiValue complexCollection = collection as ComplexMultiValue;
                        if (complexCollection != null)
                        {
                            return new ComplexMultiValueProperty(property.Name, complexCollection);
                        }
                        else
                        {
                            return new PrimitiveMultiValueProperty(property.Name, (PrimitiveMultiValue)collection);
                        }
                    }
                    else
                    {
                        ODataStreamReferenceValue streamReferenceValue = value as ODataStreamReferenceValue;
                        if (streamReferenceValue != null)
                        {
                            NamedStreamInstance namedStream = (NamedStreamInstance)this.Visit(streamReferenceValue);
                            namedStream.Name = property.Name;
                            return namedStream;
                        }
                        else
                        {
                            return new PrimitiveProperty() { Name = property.Name, Value = (PrimitiveValue)this.Visit(value) };
                        }
                    }
                }
            }

            /// <summary>
            /// Visits a navigation link item.
            /// </summary>
            /// <param name="navigationLink">The navigation link to visit.</param>
            protected override ODataPayloadElement VisitNavigationLink(ODataNestedResourceInfo navigationLink)
            {
                ExceptionUtilities.CheckArgumentNotNull(navigationLink, "navigationLink");

                ODataPayloadElement navigationPropertyContent = null;

                // check whether there is an entry or feed associated with the link
                var expandedItemAnnotation = navigationLink.GetAnnotation<ODataNavigationLinkExpandedItemObjectModelAnnotation>();
                if (expandedItemAnnotation != null)
                {
                    string navigationLinkUrlString = !this.payloadContainsIdentityMetadata || navigationLink.Url == null ? null : navigationLink.Url.OriginalString;

                    if (expandedItemAnnotation.ExpandedItem is ODataResource)
                    {
                        navigationPropertyContent = new ExpandedLink(this.Visit((ODataResource)expandedItemAnnotation.ExpandedItem)) { UriString = navigationLinkUrlString };
                    }
                    else if (expandedItemAnnotation.ExpandedItem is ODataResourceSet)
                    {
                        navigationPropertyContent = new ExpandedLink(this.Visit((ODataResourceSet)expandedItemAnnotation.ExpandedItem)) { UriString = navigationLinkUrlString };
                    }
                    else if (expandedItemAnnotation.ExpandedItem is ODataEntityReferenceLink)
                    {
                        ExceptionUtilities.Assert(!this.response, "Entity reference links in navigation links can only appear in requests.");
                        navigationPropertyContent = this.VisitEntityReferenceLink((ODataEntityReferenceLink)expandedItemAnnotation.ExpandedItem);
                    }
                    else if (expandedItemAnnotation.ExpandedItem is List<ODataItem>)
                    {
                        ExceptionUtilities.Assert(!this.response, "Navigation links with multiple items in content can only appear in requests.");
                        LinkCollection linkCollection = new LinkCollection();
                        foreach (ODataItem item in (List<ODataItem>)expandedItemAnnotation.ExpandedItem)
                        {
                            if (item is ODataResourceSet)
                            {
                                linkCollection.Add(new ExpandedLink(this.Visit((ODataResourceSet)item)));
                            }
                            else
                            {
                                ExceptionUtilities.Assert(item is ODataEntityReferenceLink, "Only feed and entity reference links can appear in navigation link content with multiple items.");
                                linkCollection.Add(this.VisitEntityReferenceLink((ODataEntityReferenceLink)item));
                            }
                        }

                        navigationPropertyContent = linkCollection;
                    }
                    else
                    {
                        ExceptionUtilities.Assert(expandedItemAnnotation.ExpandedItem == null, "Only expanded entry, feed or null is allowed.");
                        navigationPropertyContent = new ExpandedLink(new EntityInstance(null, true)) { UriString = navigationLinkUrlString };
                    }
                }
                else
                {
                    ExceptionUtilities.Assert(this.response, "Deferred links are only valid in responses.");

                    // this is a deferred link
                    DeferredLink deferredLink = new DeferredLink()
                    {
                        UriString = !this.payloadContainsIdentityMetadata || navigationLink.Url == null ? null : navigationLink.Url.OriginalString,
                    };
                    navigationPropertyContent = deferredLink;
                }

                DeferredLink associationLink = null;
                if (this.payloadContainsIdentityMetadata && navigationLink.AssociationLinkUrl != null)
                {
                    associationLink = new DeferredLink()
                    {
                        UriString = navigationLink.AssociationLinkUrl.OriginalString
                    };
                }

                return new NavigationPropertyInstance(navigationLink.Name, navigationPropertyContent, associationLink);
            }

            /// <summary>
            /// Visits a collection item.
            /// </summary>
            /// <param name="collectionValue">The collection to visit.</param>
            protected override ODataPayloadElement VisitCollectionValue(ODataCollectionValue collectionValue)
            {
                if (collectionValue == null)
                {
                    return new PrimitiveMultiValue(null, true);
                }

                // Try to parse the type name and if the item type name is a primitive EDM type, then this is a primitive collection.
                string typeName = collectionValue.TypeName;

                List<object> items = new List<object>();
                foreach (object item in collectionValue.Items)
                {
                    items.Add((PrimitiveValue)this.Visit(item));
                }

                PrimitiveMultiValue primitiveCollection = new PrimitiveMultiValue(typeName, false);
                foreach (object item in items)
                {
                    primitiveCollection.Add((PrimitiveValue)item);
                }

                this.ConvertSerializationTypeNameAnnotation(collectionValue, primitiveCollection);

                return primitiveCollection;
            }

            /// <summary>
            /// Visits a collection start.
            /// </summary>
            /// <param name="collection">The collection start to visit.</param>
            protected override ODataPayloadElement VisitCollectionStart(ODataCollectionStart collection)
            {
                ExceptionUtilities.CheckArgumentNotNull(collection, "collection");

                // NOTE the Taupo OM does not currently support heterogenous collections; we determine the
                //      type of the collection by looking at the first non-null item
                ODataCollectionItemsObjectModelAnnotation itemsAnnotation = collection.GetAnnotation<ODataCollectionItemsObjectModelAnnotation>();
                ExceptionUtilities.Assert(itemsAnnotation != null, "itemsAnnotation != null");

                PrimitiveCollection primitiveCollection = PayloadBuilder.PrimitiveCollection(collection.Name);

                foreach (object item in itemsAnnotation)
                {
                    PrimitiveValue primitiveValue = (PrimitiveValue)this.Visit(item);
                    primitiveCollection.Add(primitiveValue);
                }

                return primitiveCollection;
            }

            /// <summary>
            /// Visits a collection of parameters.
            /// </summary>
            /// <param name="parameters">The parameters to visit.</param>
            protected override ODataPayloadElement VisitParameters(ODataParameters parameters)
            {
                ExceptionUtilities.CheckArgumentNotNull(parameters, "parameters");
                ComplexInstance result = new ComplexInstance();
                result.IsNull = parameters.Count == 0;
                foreach (var parameter in parameters)
                {
                    if (parameter.Value == null)
                    {
                        result.Add(new PrimitiveProperty(parameter.Key, null, null));
                        continue;
                    }

                    ODataCollectionStart odataCollectionStart = parameter.Value as ODataCollectionStart;

                    if (odataCollectionStart != null)
                    {
                        ODataCollectionItemsObjectModelAnnotation annotation = odataCollectionStart.GetAnnotation<ODataCollectionItemsObjectModelAnnotation>();

                        PrimitiveMultiValue primitiveCollection = PayloadBuilder.PrimitiveMultiValue();
                        foreach (var value in annotation)
                        {
                            primitiveCollection.Item(value);
                        }

                        PrimitiveMultiValueProperty primitiveCollectionProperty = new PrimitiveMultiValueProperty(parameter.Key, primitiveCollection);
                        result.Add(primitiveCollectionProperty);
                    }
                    else
                    {
                        result.Add(new PrimitiveProperty(parameter.Key, null, PayloadBuilder.PrimitiveValue(parameter.Value).ClrValue));
                    }
                }

                return result;
            }

            /// <summary>
            /// Visits an error.
            /// </summary>
            /// <param name="error">The error to visit.</param>
            protected override ODataPayloadElement VisitError(ODataError error)
            {
                ExceptionUtilities.CheckArgumentNotNull(error, "error");
                return PayloadBuilder.Error(error.ErrorCode)
                    .Message(error.Message)
                    .InnerError(error.InnerError == null ? null : (ODataInternalExceptionPayload)this.Visit(error.InnerError));
            }

            /// <summary>
            /// Visits an inner error.
            /// </summary>
            /// <param name="innerError">The inner error to visit.</param>
            protected override ODataPayloadElement VisitInnerError(ODataInnerError innerError)
            {
                if (innerError == null)
                {
                    return null;
                }

                return PayloadBuilder.InnerError()
                    .Message(innerError.Message)
                    .TypeName(innerError.TypeName)
                    .StackTrace(innerError.StackTrace)
                    .InnerError(innerError.InnerError == null ? null : (ODataInternalExceptionPayload)this.Visit(innerError.InnerError));
            }

            /// <summary>
            /// Visits a serviceDocument.
            /// </summary>
            /// <param name="serviceDocument">The serviceDocument to visit.</param>
            protected override ODataPayloadElement VisitWorkspace(ODataServiceDocument serviceDocument)
            {
                ExceptionUtilities.CheckArgumentNotNull(serviceDocument, "serviceDocument");

                WorkspaceInstance workspaceInstance = new WorkspaceInstance();

                IEnumerable<ODataEntitySetInfo> collections = serviceDocument.EntitySets;
                if (collections != null)
                {
                    foreach (ODataEntitySetInfo collection in collections)
                    {
                        ResourceCollectionInstance collectionInstance = (ResourceCollectionInstance)this.Visit(collection);
                        workspaceInstance.ResourceCollections.Add(collectionInstance);
                    }
                }

                // Since ODataLib only supports a single serviceDocument and does not have an OM representation of the service
                // document, wrap the single serviceDocument in a service document instance here.
                return new ServiceDocumentInstance(workspaceInstance);
            }

            /// <summary>
            /// Visits a resource collection.
            /// </summary>
            /// <param name="entitySetInfo">The resource collection to visit.</param>
            protected override ODataPayloadElement VisitResourceCollection(ODataEntitySetInfo entitySetInfo)
            {
                ExceptionUtilities.CheckArgumentNotNull(entitySetInfo, "entitySetInfo");

                ResourceCollectionInstance collectionInstance = new ResourceCollectionInstance();

                collectionInstance.Href = entitySetInfo.Url.OriginalString;

                collectionInstance.Name = entitySetInfo.Name;

                return collectionInstance;
            }

            /// <summary>
            /// Visits an entity reference link collection.
            /// </summary>
            /// <param name="entityReferenceLinks">The entity reference link collection to visit.</param>
            protected override ODataPayloadElement VisitEntityReferenceLinks(ODataEntityReferenceLinks entityReferenceLinks)
            {
                ExceptionUtilities.CheckArgumentNotNull(entityReferenceLinks, "entityReferenceLinks");

                LinkCollection linkCollection = PayloadBuilder.LinkCollection()
                    .InlineCount(entityReferenceLinks.Count)
                    .NextLink(entityReferenceLinks.NextPageLink == null ? null : entityReferenceLinks.NextPageLink.OriginalString);

                IEnumerable<ODataEntityReferenceLink> links = entityReferenceLinks.Links;
                if (links != null)
                {
                    foreach (ODataEntityReferenceLink link in links)
                    {
                        DeferredLink deferredLink = (DeferredLink)this.Visit(link);
                        linkCollection.Add(deferredLink);
                    }
                }

                return linkCollection;
            }

            /// <summary>
            /// Visits an entity reference link.
            /// </summary>
            /// <param name="entityReferenceLink">The entity reference link to visit.</param>
            protected override ODataPayloadElement VisitEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
            {
                ExceptionUtilities.CheckArgumentNotNull(entityReferenceLink, "entityReferenceLink");

                string uriString = entityReferenceLink.Url == null ? null : entityReferenceLink.Url.OriginalString;
                return PayloadBuilder.DeferredLink(uriString);
            }

            /// <summary>
            /// Visits a stream reference value (named stream).
            /// </summary>
            /// <param name="streamReferenceValue">The stream reference value to visit.</param>
            protected override ODataPayloadElement VisitStreamReferenceValue(ODataStreamReferenceValue streamReferenceValue)
            {
                ExceptionUtilities.CheckArgumentNotNull(streamReferenceValue, "streamReferenceValue");

                string streamSourceLink = !this.payloadContainsIdentityMetadata || streamReferenceValue.ReadLink == null ? null : streamReferenceValue.ReadLink.OriginalString;
                string streamEditLink = !this.payloadContainsIdentityMetadata || streamReferenceValue.EditLink == null ? null : streamReferenceValue.EditLink.OriginalString;

                // Note that we're creating a named stream instance without a name here - name will be supplied by the caller if necessary.
                return new NamedStreamInstance()
                {
                    SourceLink = streamSourceLink,
                    EditLink = streamEditLink,
                    ETag = streamReferenceValue.ETag,
                    SourceLinkContentType = streamEditLink == null ? streamReferenceValue.ContentType : null,
                    EditLinkContentType = streamEditLink != null ? streamReferenceValue.ContentType : null,
                };
            }

            /// <summary>
            /// Visits a primitive value.
            /// </summary>
            /// <param name="primitiveValue">The primitive value to visit.</param>
            protected override ODataPayloadElement VisitPrimitiveValue(object primitiveValue)
            {
                if (primitiveValue == null)
                {
                    return new PrimitiveValue(null, null);
                }
                else
                {
                    return new PrimitiveValue(EntityModelUtils.GetPrimitiveEdmType(primitiveValue.GetType()).FullEdmName(), primitiveValue);
                }
            }

            /// <summary>
            /// Visits a odata operation value.
            /// </summary>
            /// <param name="primitiveValue">The primitive value to visit.</param>
            protected override ODataPayloadElement VisitODataOperation(ODataOperation operation)
            {
                ServiceOperationDescriptor descriptor = new ServiceOperationDescriptor();
                descriptor.IsAction = true;
                if (operation is ODataFunction)
                {
                    descriptor.IsFunction = true;
                }

                descriptor.Metadata = operation.Metadata == null ? null : operation.Metadata.OriginalString;
                descriptor.Target = operation.Target == null ? null : operation.Target.OriginalString;
                descriptor.Title = operation.Title;

                return descriptor;
            }

            /// <summary>
            /// Visits an ODataBatch.
            /// </summary>
            /// <param name="batch">The batch to visit.</param>
            protected override ODataPayloadElement VisitBatch(ODataBatch batch)
            {
                List<IMimePart> parts = new List<IMimePart>();

                if (batch.Parts != null)
                {
                    foreach (ODataBatchPart part in batch.Parts)
                    {
                        ODataBatchChangeset changeset = part as ODataBatchChangeset;
                        if (changeset == null)
                        {
                            parts.Add(this.ConvertBatchOperation((ODataBatchOperation)part));
                        }
                        else
                        {
                            List<IMimePart> changeSetOperations = new List<IMimePart>();
                            if (changeset.Operations != null)
                            {
                                foreach (ODataBatchOperation batchOperation in changeset.Operations)
                                {
                                    changeSetOperations.Add(this.ConvertBatchOperation(batchOperation));
                                }
                            }

                            // TODO: the ODataBatchReader does not expose boundary strings or encodings; should it?
                            if (this.response)
                            {
                                BatchResponseChangeset responseChangeset =
                                    BatchPayloadBuilder.ResponseChangeset(/*boundary*/ null, /*charset*/ null, changeSetOperations.ToArray());
                                parts.Add(responseChangeset);
                            }
                            else
                            {
                                BatchRequestChangeset requestChangeset =
                                    BatchPayloadBuilder.RequestChangeset(/*boundary*/ null, /*charset*/ null, changeSetOperations.ToArray());
                                parts.Add(requestChangeset);
                            }
                        }
                    }
                }

                return this.response
                    ? (ODataPayloadElement)PayloadBuilder.BatchResponsePayload(parts.ToArray())
                    : (ODataPayloadElement)PayloadBuilder.BatchRequestPayload(parts.ToArray());
            }

            /// <summary>
            /// Converts an ODataBatchOperation to the corresponding IMimePart.
            /// </summary>
            /// <param name="operation">The operation to convert.</param>
            /// <returns>The converted operation.</returns>
            private IMimePart ConvertBatchOperation(ODataBatchOperation operation)
            {
                ODataPayloadElement payloadElement = operation.Payload == null ? null : this.Visit(operation.Payload);

                if (this.response)
                {
                    ODataBatchResponseOperation responseOperation = (ODataBatchResponseOperation)operation;
                    HttpResponseData httpResponse = new HttpResponseData
                    {
                        StatusCode = (HttpStatusCode)responseOperation.StatusCode,
                    };

                    if (responseOperation.Headers != null)
                    {
                        foreach (var kvp in responseOperation.Headers)
                        {
                            httpResponse.Headers.Add(kvp.Key, kvp.Value);
                        }
                    };
                    return new ODataResponse(httpResponse) { RootElement = payloadElement };
                }

                ODataBatchRequestOperation requestOperation = (ODataBatchRequestOperation)operation;

                // NOTE: this is abusing the ODataUriBuilder but is sufficient for our purposes
                // We use Unrecognized because the request URI may be relative (in the case of a reference)
                ODataUri requestUri = new ODataUri(ODataUriBuilder.Unrecognized(requestOperation.Url.OriginalString));
                ODataRequest request = this.requestManager.BuildRequest(
                    requestUri,    
                    (HttpVerb)Enum.Parse(typeof(HttpVerb), requestOperation.HttpMethod, /*ignoreCase*/ true),
                    requestOperation.Headers);
                if (payloadElement != null)
                {
                    string contentType;
                    if (!request.Headers.TryGetValue(Microsoft.OData.ODataConstants.ContentTypeHeader, out contentType))
                    {
                        throw new InvalidOperationException("ContentType header not found.");
                    }
                    request.Body = this.requestManager.BuildBody(contentType, requestUri, payloadElement);
                }

                return request;
            }

            /// <summary>
            /// Converts the SerializationTypeNameAnnotation from the <paramref name="odataAnnotatable"/> into a SerializationTypeNameTestAnnotation
            /// added to the <paramref name="payloadElement"/>.
            /// </summary>
            /// <param name="odataAnnotatable">The OData OM value to get the annotation from.</param>
            /// <param name="payloadElement">The payload element to add the converted annotation to.</param>
            private void ConvertSerializationTypeNameAnnotation(ODataAnnotatable odataAnnotatable, ODataPayloadElement payloadElement)
            {
                if (odataAnnotatable.TypeAnnotation != null)
                {
                    payloadElement.AddAnnotation(new SerializationTypeNameTestAnnotation { TypeName = odataAnnotatable.TypeAnnotation.TypeName });
                }
            }
        }
    }
}
