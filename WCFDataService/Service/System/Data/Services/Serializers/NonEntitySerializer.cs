//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Serializers
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.OData;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;

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
                    throw new InvalidOperationException(System.Data.Services.Strings.Serializer_UnsupportedTopLevelType(element.GetType()));
                }

                var odataProperty = new ODataProperty
                {
                    Name = propertyName,
                    Value = this.GetPropertyValue(propertyName, resourceType, element, this.RequestDescription.TargetKind == RequestTargetKind.OpenProperty /*openProperty*/)
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
                
                IEdmFunctionImport functionImport = model.EnsureDefaultEntityContainer().EnsureFunctionImport(operation);
                Debug.Assert(functionImport != null, "functionImport != null");

                IEdmCollectionTypeReference collectionType = (IEdmCollectionTypeReference)functionImport.ReturnType;
                bool isJsonLightResponse = ContentTypeUtil.IsResponseMediaTypeJsonLight(this.Service, /*isEntryOrFeed*/ false);
                this.collectionWriter = this.writer.CreateODataCollectionWriter(isJsonLightResponse ? null : collectionType.ElementType());

                ODataCollectionStart collectionStart = new ODataCollectionStart { Name = this.ComputeContainerName() };
                collectionStart.SetSerializationInfo(new ODataCollectionStartSerializationInfo { CollectionTypeName = collectionType.ODataFullName() });

                this.collectionWriter.WriteStart(collectionStart);
                while (elements.HasMoved)
                {
                    object element = elements.Current;
                    ResourceType resourceType = element == null ?
                        this.RequestDescription.TargetResourceType : WebUtil.GetResourceType(this.Provider, element);
                    if (resourceType == null)
                    {
                        throw new InvalidOperationException(System.Data.Services.Strings.Serializer_UnsupportedTopLevelType(element.GetType()));
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

            IEdmEntitySet sourceEntitySet = this.GetTargetLinkEntitySet();
            IEdmNavigationProperty navigationProperty = this.GetTargetNavigationProperty();
            if (ContentTypeUtil.IsResponseMediaTypeJsonLight(this.Service, /*isEntryOrFeed*/ false))
            {
                string typeCast = sourceEntitySet.ElementType == navigationProperty.DeclaringEntityType() ? null : navigationProperty.DeclaringEntityType().FullName();
                entityReferenceLink.SetSerializationInfo(new ODataEntityReferenceLinkSerializationInfo { SourceEntitySetName = sourceEntitySet.Name, Typecast = typeCast, NavigationPropertyName = navigationProperty.Name, IsCollectionNavigationProperty = navigationProperty.Type.IsCollection() });
                sourceEntitySet = null;
                navigationProperty = null;
            }

            this.writer.WriteEntityReferenceLink(entityReferenceLink, sourceEntitySet, navigationProperty);
        }

        /// <summary>
        /// Write out the uri for the given elements.
        /// </summary>
        /// <param name="elements">Elements whose uri need to be written out.</param>
        private void WriteLinkCollection(QueryResultInfo elements)
        {
            ODataEntityReferenceLinks linksCollection = new ODataEntityReferenceLinks();

            // write count?
            if (this.RequestDescription.CountOption == RequestQueryCountOption.Inline)
            {
                linksCollection.Count = this.RequestDescription.CountValue;
            }

            // assign the links collection
            linksCollection.Links = this.GetLinksCollection(elements, linksCollection);
            
            IEdmEntitySet sourceEntitySet = this.GetTargetLinkEntitySet();
            IEdmNavigationProperty navigationProperty = this.GetTargetNavigationProperty();
            if (ContentTypeUtil.IsResponseMediaTypeJsonLight(this.Service, /*isEntryOrFeed*/ false))
            {
                string typeCast = sourceEntitySet.ElementType == navigationProperty.DeclaringEntityType() ? null : navigationProperty.DeclaringEntityType().FullName();
                linksCollection.SetSerializationInfo(new ODataEntityReferenceLinksSerializationInfo { SourceEntitySetName = sourceEntitySet.Name, Typecast = typeCast, NavigationPropertyName = navigationProperty.Name });
                sourceEntitySet = null;
                navigationProperty = null;
            }

            this.writer.WriteEntityReferenceLinks(linksCollection, sourceEntitySet, navigationProperty);
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

        /// <summary>
        /// Get the edm property which is the target property for this request.
        /// </summary>
        /// <returns>an IEdmProperty which is the target property for this request.</returns>
        private IEdmNavigationProperty GetTargetNavigationProperty()
        {
            Debug.Assert(this.RequestDescription.SegmentInfos.Count > 1, "For navigation properties, the # of segmentInfos must be greater than 1");
            Debug.Assert(this.RequestDescription.Property.TypeKind == ResourceTypeKind.EntityType, "WriteLink must be called for navigation properties only");

            IEdmEntityType entityType = this.GetTargetPropertyDeclaringType() as IEdmEntityType;
            Debug.Assert(entityType != null, "entityType != null");

            IEdmNavigationProperty property = entityType.FindProperty(this.RequestDescription.Property.Name) as IEdmNavigationProperty;
            Debug.Assert(property != null, "property != null");
            return property;
        }

        /// <summary>
        /// Get the parent type of the target property.
        /// </summary>
        /// <returns>IEdmStructuredType which is the parent type of the target property.</returns>
        private IEdmStructuredType GetTargetPropertyDeclaringType()
        {
            Debug.Assert(this.RequestDescription.SegmentInfos.Count >= 2, "this.RequestDescription.SegmentInfos.Count >= 2");
            MetadataProviderEdmModel model = this.Service.Provider.GetMetadataProviderEdmModel();
            ResourceType parentResourceType = this.RequestDescription.SegmentInfos[this.RequestDescription.SegmentInfos.Count - 2].TargetResourceType;
            Debug.Assert(parentResourceType != null, "this.parentResourceType != null");

            IEdmStructuredType structuredType = model.EnsureSchemaType(parentResourceType) as IEdmStructuredType;
            Debug.Assert(structuredType != null, "structuredType != null");

            return structuredType;
        }

        /// <summary>
        /// Get the entitySet of a link request.
        /// </summary>
        /// <returns>IEdmEntitySet of the link.</returns>
        private IEdmEntitySet GetTargetLinkEntitySet()
        {
            Debug.Assert(this.RequestDescription.LinkUri, "Can only use this function when request has $links");
            Debug.Assert(this.RequestDescription.SegmentInfos.Count >= 2, "this.RequestDescription.SegmentInfos.Count >= 2");
            IEdmModel model = this.Service.Provider.GetMetadataProviderEdmModel();

            int sourceEntityIndex = Deserializer.GetIndexOfEntityResourceToModify(this.RequestDescription);
            ResourceSetWrapper entitySetWrapper = this.RequestDescription.SegmentInfos[sourceEntityIndex].TargetResourceSet;

            return WebUtil.GetEntitySet(this.Service.Provider, model, entitySetWrapper);
        }
    }
}
