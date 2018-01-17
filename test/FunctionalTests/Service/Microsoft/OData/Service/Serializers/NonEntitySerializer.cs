//---------------------------------------------------------------------
// <copyright file="NonEntitySerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Providers;

    #endregion Namespaces

    /// <summary>This class serializes non-entity resources (primitive, complex types, collection types, links, etc).</summary>
    internal sealed class NonEntitySerializer : Serializer
    {
        /// <summary>ODataMessageWriter instance which needs to be used to write the response.</summary>
        private readonly ODataMessageWriter writer;

        /// <summary>
        /// Collection writer used to write out collection of entities/elements.
        /// </summary>
        private ODataCollectionWriter collectionWriter;

        /// <summary>Initializes a new NonEntitySerializer instance.</summary>
        /// <param name="requestDescription">Description of request.</param>
        /// <param name="absoluteServiceUri">Base URI from which resources should be resolved.</param>
        /// <param name="service">Service with configuration and provider from which metadata should be gathered.</param>
        /// <param name="messageWriter">ODataMessageWriter instance which needs to be used to write the response.</param>
        internal NonEntitySerializer(RequestDescription requestDescription, Uri absoluteServiceUri, IDataService service, ODataMessageWriter messageWriter)
            : base(requestDescription, absoluteServiceUri, service, null)
        {
            Debug.Assert(messageWriter != null, "messageWriter != null");

            this.writer = messageWriter;
        }

        /// <summary>
        /// Flushes the content of the underlying writers
        /// </summary>
        internal override void Flush()
        {
            // Astoria-ODataLib-Integration: Astoria does not call flush before calling the IDataServiceHost.ProcessException method
            // If the request is for an entry/feed and the data source throws an error while these results are being enumerated and written,
            // the server doesn't flush the writer's stream before it calls HandleException and starts writing out the error. 
            // To handle this case, we'll make the EntitySerializer expose a method that calls Flush on the underlying ODL writer instance.
            if (this.collectionWriter != null)
            {
                this.collectionWriter.Flush();
            }
        }

        /// <summary>Writes a single top-level element.</summary>
        /// <param name="expandedResult">Expandd results on the specified <paramref name="element"/>.</param>
        /// <param name="element">Element to write, possibly null.</param>
        protected override void WriteTopLevelElement(IExpandedResult expandedResult, object element)
        {
            Debug.Assert(
                element != null || this.RequestDescription.TargetResourceType != null || this.RequestDescription.TargetKind == RequestTargetKind.OpenProperty,
                "element != null || this.RequestDescription.TargetResourceType != null || this.RequestDescription.TargetKind == RequestTargetKind.OpenProperty");
            Debug.Assert(
                this.RequestDescription.IsSingleResult,
                "this.RequestDescription.IsSingleResult -- primitive collections not currently supported");

            string propertyName = this.ComputeContainerName();

            if (this.RequestDescription.LinkUri)
            {
                bool needPop = this.PushSegmentForRoot();
                this.WriteLink(element);
                this.PopSegmentName(needPop);
            }
            else
            {
                ResourceType resourceType;
                if (element == null)
                {
                    resourceType = this.RequestDescription.TargetKind == RequestTargetKind.OpenProperty
                        ? ResourceType.PrimitiveStringResourceType
                        : this.RequestDescription.TargetResourceType;
                }
                else
                {
                    resourceType = this.RequestDescription.TargetKind == RequestTargetKind.Collection
                        ? this.RequestDescription.TargetResourceType
                        : WebUtil.GetResourceType(this.Provider, element);
                }

                if (resourceType == null)
                {
                    throw new InvalidOperationException(Microsoft.OData.Service.Strings.Serializer_UnsupportedTopLevelType(element.GetType()));
                }

                if (resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType)
                {
                    var odataWriter = this.writer.CreateODataResourceWriter();
                    var resource = this.GetComplexResource(propertyName, element);
                    resource.Resource.SetSerializationInfo(new ODataResourceSerializationInfo() { ExpectedTypeName = resourceType.FullName });
                    ODataWriterHelper.WriteResource(odataWriter, resource);
                    return;
                }

                if (resourceType.ResourceTypeKind == ResourceTypeKind.Collection
                    && resourceType.ElementType().ResourceTypeKind == ResourceTypeKind.ComplexType)
                {
                    var odataWriter = this.writer.CreateODataResourceSetWriter();
                    var resourceSet = this.GetComplexResourceSet(propertyName, (CollectionResourceType)resourceType, element);
                    resourceSet.ResourceSet.SetSerializationInfo(new ODataResourceSerializationInfo() { ExpectedTypeName = resourceType.FullName });
                    ODataWriterHelper.WriteResourceSet(odataWriter, resourceSet);
                    return;
                }

                var odataValue = this.GetPropertyValue(propertyName, resourceType, element, this.RequestDescription.TargetKind == RequestTargetKind.OpenProperty /*openProperty*/);
                if (odataValue == null || odataValue is ODataNullValue)
                {
                    return;
                }

                var odataProperty = new ODataProperty
                {
                    Name = propertyName,
                    Value = odataValue
                };

                this.writer.WriteProperty(odataProperty);
            }
        }

        /// <summary>Writes multiple top-level elements, possibly none.</summary>
        /// <param name="expanded">Expanded results for elements.</param>
        /// <param name="elements">Enumerator for elements to write.</param>
        protected override void WriteTopLevelElements(IExpandedResult expanded, QueryResultInfo elements)
        {
            Debug.Assert(
                !this.RequestDescription.IsSingleResult,
                "!this.RequestDescription.IsSingleResult -- otherwise WriteTopLevelElement should have been called");

            if (this.RequestDescription.LinkUri)
            {
                bool needPop = this.PushSegmentForRoot();
                this.WriteLinkCollection(elements);
                this.PopSegmentName(needPop);
            }
            else
            {
                MetadataProviderEdmModel model = this.Service.Provider.GetMetadataProviderEdmModel();
                OperationWrapper operation = this.RequestDescription.LastSegmentInfo.Operation;
                
                IEdmOperation edmOperation = model.GetRelatedOperation(operation);
                Debug.Assert(edmOperation != null, "edmOperation != null");

                IEdmCollectionTypeReference collectionType = (IEdmCollectionTypeReference)edmOperation.ReturnType;
                bool isJsonLightResponse = ContentTypeUtil.IsResponseMediaTypeJsonLight(this.Service, /*isEntryOrFeed*/ false);
                this.collectionWriter = this.writer.CreateODataCollectionWriter(isJsonLightResponse ? null : collectionType.ElementType());

                ODataCollectionStart collectionStart = new ODataCollectionStart { Name = this.ComputeContainerName() };
                collectionStart.SetSerializationInfo(new ODataCollectionStartSerializationInfo { CollectionTypeName = collectionType.FullName() });

                this.collectionWriter.WriteStart(collectionStart);
                while (elements.HasMoved)
                {
                    object element = elements.Current;
                    ResourceType resourceType = element == null ?
                        this.RequestDescription.TargetResourceType : WebUtil.GetResourceType(this.Provider, element);
                    if (resourceType == null)
                    {
                        throw new InvalidOperationException(Microsoft.OData.Service.Strings.Serializer_UnsupportedTopLevelType(element.GetType()));
                    }

                    this.collectionWriter.WriteItem(this.GetPropertyValue(XmlConstants.XmlCollectionItemElementName, resourceType, element, false /*openProperty*/).FromODataValue());
                    elements.MoveNext();
                }

                this.collectionWriter.WriteEnd();
                this.collectionWriter.Flush();
            }
        }

        /// <summary>
        /// Write out the uri for the given element
        /// </summary>
        /// <param name="element">element whose uri needs to be written out.</param>
        private void WriteLink(object element)
        {
            Debug.Assert(element != null, "element != null");

            this.IncrementSegmentResultCount();
            ODataEntityReferenceLink entityReferenceLink = new ODataEntityReferenceLink { Url = this.GetEntityEditLink(element) };

            this.writer.WriteEntityReferenceLink(entityReferenceLink);
        }

        /// <summary>
        /// Write out the uri for the given elements.
        /// </summary>
        /// <param name="elements">Elements whose uri need to be written out.</param>
        private void WriteLinkCollection(QueryResultInfo elements)
        {
            ODataEntityReferenceLinks linksCollection = new ODataEntityReferenceLinks();

            // write count?
            if (this.RequestDescription.CountOption == RequestQueryCountOption.CountQuery)
            {
                linksCollection.Count = this.RequestDescription.CountValue;
            }

            // assign the links collection
            linksCollection.Links = this.GetLinksCollection(elements, linksCollection);

            this.writer.WriteEntityReferenceLinks(linksCollection);
        }

        /// <summary>
        /// Return the collection of links as ODataEntityReferenceLink instances
        /// </summary>
        /// <param name="elements">Elements whose uri need to be written out.</param>
        /// <param name="linksCollection">LinkCollection instance which represents the collection getting written out.</param>
        /// <returns>Return the collection of links as ODataEntityReferenceLink instances.</returns>
        private IEnumerable<ODataEntityReferenceLink> GetLinksCollection(QueryResultInfo elements, ODataEntityReferenceLinks linksCollection)
        {
            object lastObject = null;
            IExpandedResult lastExpandedSkipToken = null;
            while (elements.HasMoved)
            {
                object element = elements.Current;
                IExpandedResult skipToken = null;
                if (element != null)
                {
                    IExpandedResult expanded = element as IExpandedResult;
                    if (expanded != null)
                    {
                        element = GetExpandedElement(expanded);
                        skipToken = this.GetSkipToken(expanded);
                    }
                }

                this.IncrementSegmentResultCount();
                yield return new ODataEntityReferenceLink { Url = this.GetEntityEditLink(element) };
                elements.MoveNext();
                lastObject = element;
                lastExpandedSkipToken = skipToken;
            }

            if (this.NeedNextPageLink(elements))
            {
                linksCollection.NextPageLink = this.GetNextLinkUri(lastObject, lastExpandedSkipToken, this.RequestDescription.ResultUri);
                yield break;
            }
        }

        /// <summary>
        /// Return the canonical uri (the edit link) of the element.
        /// </summary>
        /// <param name="element">Element whose canonical uri need to be returned.</param>
        /// <returns>Return the canonical uri of the element.</returns>
        private Uri GetEntityEditLink(object element)
        {
            Debug.Assert(element != null, "element != null");
            ResourceType resourceType = WebUtil.GetNonPrimitiveResourceType(this.Provider, element);
            EntityToSerialize entity = EntityToSerialize.Create(element, resourceType, this.CurrentContainer, this.Provider, this.AbsoluteServiceUri);
            return entity.SerializedKey.AbsoluteEditLink;
        }

        /// <summary>
        /// Computes the container name for the payload to write.
        /// </summary>
        /// <returns>The container name to use for writing.</returns>
        /// <remarks>Action and function names can be fully-qualified in the URI but must only use their local name in
        /// response payloads (since we treat '.', ':', '@' as reserved characters for property names and 
        /// use them as an extension mechanism).</remarks>
        private string ComputeContainerName()
        {
            bool isServiceAction = this.RequestDescription.LastSegmentInfo.IsServiceActionSegment;
            if (isServiceAction)
            {
                bool nameIsContainerQualified;
                return this.Provider.GetNameFromContainerQualifiedName(this.RequestDescription.ContainerName, out nameIsContainerQualified);
            }

            return this.RequestDescription.ContainerName;
        }
    }
}
