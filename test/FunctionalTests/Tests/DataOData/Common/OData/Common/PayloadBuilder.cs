//---------------------------------------------------------------------
// <copyright file="PayloadBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Builder for ODataPayloadElement instances which works without metadata.
    /// </summary>
    public static class PayloadBuilder
    {
        /// <summary>
        /// Creates a batch request payload
        /// </summary>
        /// <param name="requestManager">Used to build the body of the payload</param>
        /// <param name="payloadBody">The payload to be written into the batch request</param>
        /// <param name="boundary">String representing the batch boundary</param>
        /// <param name="request">(Optional)The request data to be used in the payload</param>
        /// <returns>Returns the batch payload constructed with the payloadBody</returns>
        public static BatchRequestPayload BatchRequestPayload(IODataRequestManager requestManager, ODataPayloadElement payloadBody, String boundary, HttpRequestData request = null)
        {
            if (request == null)
            {
                request = CreateDefaultRequestData();
            }

            var requestOperation = request.AsBatchFragment();
            var encoding = Encoding.UTF8;

            // Build an OData Uri
            ODataUri odataUri = BuildODataUri("http://www.odata.org");

            var headers = new Dictionary<string, string> { { "HName", "HValue" } };
            var odataMergeRequest = requestManager.BuildRequest(odataUri, request.Verb, request.Headers);

            string mergeContentType = HttpUtilities.BuildContentType(MimeTypes.ApplicationXml, encoding.WebName, null);

            odataMergeRequest.Body = requestManager.BuildBody(
                mergeContentType,
                odataMergeRequest.Uri,
                payloadBody
                );

            var batchRequest = new BatchRequestPayload()
            {
                odataMergeRequest,
                BatchPayloadBuilder.RequestChangeset(boundary, mergeContentType, odataMergeRequest),
            };

            return batchRequest;
        }

        /// <summary>
        /// Creates a batch response payload
        /// </summary>
        /// <param name="requestManager">Used to build the body of the payload</param>
        /// <param name="payloadBody">The payload to be written into the batch request</param>
        /// <param name="boundary">String representing the batch boundary</param>
        /// <param name="request">(Optional)The request data to be used in the payload</param>
        /// <returns>Returns the batch payload constructed with the payloadBody</returns>
        public static BatchRequestPayload BatchRequestPayload(params IMimePart[] parts)
        {
            ExceptionUtilities.CheckArgumentNotNull(parts, "parts");

            var headers = new Dictionary<string, string> { { "ResponseHeader", "ResponseValue" } };
            var baseUri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org/service.svc")));

            var batchRequest = new BatchRequestPayload();
            foreach (var part in parts)
            {
                IHttpRequest operation = part as IHttpRequest;
                if (operation != null)
                {
                    batchRequest.Add(operation.AsBatchFragment());
                }

                BatchRequestChangeset changeset = part as BatchRequestChangeset;
                if (changeset != null)
                {
                    batchRequest.Add(changeset);
                }

                ExceptionUtilities.Assert(operation != null || changeset != null, "Unrecognized type: '{0}'", part.GetType());
            }

            return batchRequest;
        }

        /// <summary>
        /// Creates a batch response payload
        /// </summary>
        /// <param name="requestManager">Used to build the body of the payload</param>
        /// <param name="payloadBody">The payload to be written into the batch request</param>
        /// <param name="boundary">String representing the batch boundary</param>
        /// <param name="request">(Optional)The request data to be used in the payload</param>
        /// <returns>Returns the batch payload constructed with the payloadBody</returns>
        public static BatchResponsePayload BatchResponsePayload(params IMimePart[] parts)
        {
            ExceptionUtilities.CheckArgumentNotNull(parts, "parts");

            var headers = new Dictionary<string, string> { { "ResponseHeader", "ResponseValue" } };
            var baseUri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org/service.svc")));

            var batchResponse = new BatchResponsePayload();
            foreach (var part in parts)
            {
                HttpResponseData operation = part as HttpResponseData;
                if (operation != null)
                {
                    batchResponse.Add(operation.AsBatchFragment());
                }

                BatchResponseChangeset changeset = part as BatchResponseChangeset;
                if (changeset != null)
                {
                    batchResponse.Add(changeset);
                }

                ExceptionUtilities.Assert(operation != null || changeset != null, "Unrecognized type: '{0}'", part.GetType());
            }

            return batchResponse;
        }

        /// <summary>
        /// Creates a primitive value element.
        /// </summary>
        /// <param name="value">The CLR primitive value to creat the element for.</param>
        /// <returns>The primitive value element representing the <paramref name="value"/>.
        /// The right EDM type will be assigned to the value.</returns>
        public static PrimitiveValue PrimitiveValue(object value)
        {
            if (value == null)
            {
                return new PrimitiveValue(null, null);
            }
            else
            {
                return new PrimitiveValue(EntityModelUtils.GetPrimitiveEdmType(value.GetType()).FullEdmName(), value);
            }
        }

        /// <summary>
        /// Creates a primitive value element.
        /// </summary>
        /// <param name="value">The CLR primitive value to creat the element for.</param>
        /// <returns>The primitive value element representing the <paramref name="value"/>.
        /// The right EDM type will be assigned to the value.</returns>
        public static IEdmTypeReference PrimitiveValueType(object value)
        {
            if (value == null)
            {
                return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.None).ToTypeReference();
            }
            else
            {
                return EdmModelUtils.GetPrimitiveTypeByName(value.GetType().FullName).ToTypeReference();
            }
        }

        /// <summary>
        /// Creates a primitive property element.
        /// </summary>
        /// <param name="propertyName">The name of the property to create.</param>
        /// <param name="value">The CLR primitive value to create the property for.</param>
        /// <returns>The primitive property element representing the <paramref name="value"/>.
        /// The right EDM type will be assigned to the value.</returns>
        public static PropertyInstance PrimitiveProperty(string propertyName, object value)
        {
            return new PrimitiveProperty() { Name = propertyName, Value = PayloadBuilder.PrimitiveValue(value) };
        }

        /// <summary>
        /// Creates a property element.
        /// </summary>
        /// <param name="propertyName">The name of the property to create.</param>
        /// <param name="value">The value of the property represented as a payload element.</param>
        /// <returns>Based on the type of the <paramref name="value"/> this creates the right property element.</returns>
        public static PropertyInstance Property(string propertyName, ODataPayloadElement value)
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");

            TypedValue typedValue = value as TypedValue;
            if (typedValue != null && typedValue.IsNull)
            {
                var newValue = new PrimitiveValue(typedValue.FullTypeName, null);
                newValue.CopyAnnotation<PrimitiveValue, EntityModelTypeAnnotation>(value);
                return new PrimitiveProperty { Name = propertyName, Value = newValue };
            }

            switch (value.ElementType)
            {
                case ODataPayloadElementType.PrimitiveValue:
                    PrimitiveValue primitiveValue = (PrimitiveValue)value;
                    return new PrimitiveProperty() { Name = propertyName, Value = primitiveValue };
                case ODataPayloadElementType.ComplexInstance:
                    ComplexInstance complexValue = (ComplexInstance)value;
                    return new ComplexProperty(propertyName, complexValue);
                case ODataPayloadElementType.PrimitiveMultiValue:
                    PrimitiveMultiValue primitiveMultiValue = (PrimitiveMultiValue)value;
                    return new PrimitiveMultiValueProperty(propertyName, primitiveMultiValue);
                case ODataPayloadElementType.ComplexMultiValue:
                    ComplexMultiValue complexMultiValue = (ComplexMultiValue)value;
                    return new ComplexMultiValueProperty(propertyName, complexMultiValue);
                default:
                    ExceptionUtilities.Assert(false, "Element type {0} not supported as a property value.", value.ElementType);
                    return null;
            }
        }

        /// <summary>
        /// Creates a deferred navigation property, including an optional association URL.
        /// </summary>
        /// <param name="propertyName">The name of the property to create.</param>
        /// <param name="url">The URL to use for the deferred link of the navigation property.</param>
        /// <param name="associationUrl">The (optional) URL to use for the association link of the navigation property.</param>
        /// <returns>The newly created navigation property.</returns>
        public static NavigationPropertyInstance NavigationProperty(string propertyName, string url, string associationUrl = null)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyName, "propertyName");

            DeferredLink deferredLink = url == null ? null : new DeferredLink { UriString = url };
            DeferredLink associationLink = associationUrl == null ? null : new DeferredLink { UriString = associationUrl };

            return DeferredNavigationProperty(propertyName, deferredLink, associationLink);
        }

        /// <summary>
        /// Creates a deferred navigation property, including an optional association URL.
        /// </summary>
        /// <param name="propertyName">The name of the property to create.</param>
        /// <param name="link">The deferred link.</param>
        /// <param name="associationLink">The (optional) association link of the navigation property.</param>
        /// <returns>The newly created navigation property.</returns>
        public static NavigationPropertyInstance DeferredNavigationProperty(string propertyName, DeferredLink link, DeferredLink associationLink = null)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyName, "propertyName");
            return new NavigationPropertyInstance(propertyName, link, associationLink);
        }

        /// <summary>
        /// Creates an expanded link navigation property, including an optional association URL.
        /// </summary>
        /// <param name="propertyName">The name of the property to create.</param>
        /// <param name="value">The value to be used as the content of the expanded link.</param>
        /// <param name="href">The (optional) URL to use for UriString of the Expanded link.</param>
        /// <param name="associationUrl">The (optional) URL to use for the association link of the navigation property.</param>
        /// <returns>The newly created navigation property.</returns>
        public static NavigationPropertyInstance ExpandedNavigationProperty(string propertyName, ODataPayloadElement value, string href = null, string associationUrl = null)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyName, "propertyName");

            DeferredLink associationLink = associationUrl == null ? null : new DeferredLink { UriString = associationUrl };
            return ExpandedNavigationProperty(propertyName, value, href, associationLink);
        }

        /// <summary>
        /// Creates an expanded link navigation property, including an optional association URL.
        /// </summary>
        /// <param name="propertyName">The name of the property to create.</param>
        /// <param name="value">The value to be used as the content of the expanded link.</param>
        /// <param name="href">The (optional) URL to use for UriString of the Expanded link.</param>
        /// <param name="associationLink">The (optional) URL to use for the association link of the navigation property.</param>
        /// <returns>The newly created navigation property.</returns>
        public static NavigationPropertyInstance ExpandedNavigationProperty(string propertyName, ODataPayloadElement value, string href, DeferredLink associationLink)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyName, "propertyName");

            ExpandedLink expandedLink = new ExpandedLink { ExpandedElement = value, UriString = href };

            bool isCollection = value != null && value is EntitySetInstance;
            return new NavigationPropertyInstance(propertyName, expandedLink, associationLink).IsCollection(isCollection);
        }

        /// <summary>
        /// Sets the cardinality of navigation property.
        /// </summary>
        /// <param name="navigationProperty">The navigation property to set the cardinality on.</param>
        /// <param name="isCollection">true if it's a collection, false for a singleton, null for unknown.</param>
        /// <returns>The navigation property with the cardinality set.</returns>
        public static NavigationPropertyInstance IsCollection(this NavigationPropertyInstance navigationProperty, bool? isCollection)
        {
            ExceptionUtilities.CheckArgumentNotNull(navigationProperty, "navigationProperty");

            NavigationPropertyCardinalityAnnotation annotation = navigationProperty.GetAnnotation<NavigationPropertyCardinalityAnnotation>();
            if (annotation == null)
            {
                annotation = new NavigationPropertyCardinalityAnnotation();
                navigationProperty.AddAnnotation(annotation);
            }

            annotation.IsCollection = isCollection;
            return navigationProperty;
        }

        /// <summary>
        /// Creates a stream reference property.
        /// </summary>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="readLink">The read link for the stream reference value.</param>
        /// <param name="editLink">The edit link for the stream reference value.</param>
        /// <param name="contentType">The content type for the stream reference value.</param>
        /// <param name="etag">The ETag for the stream reference value.</param>
        /// <returns>The newly created stream reference property.</returns>
        public static NamedStreamInstance StreamProperty(string propertyName, string readLink = null, string editLink = null, string contentType = null, string etag = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            NamedStreamInstance namedStream = new NamedStreamInstance(propertyName)
            {
                SourceLink = readLink,
                EditLink = editLink,
                ETag = etag,
            };

            if (editLink != null)
            {
                namedStream.EditLinkContentType = contentType;
            }
            else
            {
                namedStream.SourceLinkContentType = contentType;
            }

            return namedStream;
        }

        /// <summary>
        /// Creates a new empty non-null complex value.
        /// </summary>
        /// <returns>The new complex value created.</returns>
        public static ComplexInstance ComplexValue()
        {
            return PayloadBuilder.ComplexValue(null);
        }

        /// <summary>
        /// Creates a new empty non-null typed complex value.
        /// </summary>
        /// <param name="fullTypeName">The full name of the type for the complex value (can be null).</param>
        /// <returns>The new complex value created.</returns>
        public static ComplexInstance ComplexValue(string fullTypeName)
        {
            return PayloadBuilder.ComplexValue(fullTypeName, false);
        }

        /// <summary>
        /// Creates a new empty non-null typed complex value.
        /// </summary>
        /// <param name="fullTypeName">The full name of the type for the complex value (can be null).</param>
        /// <param name="isNull">true if the complex value represents 'null'; otherwise false.</param>
        /// <returns>The new complex value created.</returns>
        public static ComplexInstance ComplexValue(string fullTypeName, bool isNull)
        {
            return new ComplexInstance(fullTypeName, isNull);
        }

        /// <summary>
        /// Creates a new empty non-null untyped primitive collection.
        /// </summary>
        /// <returns>The new collection created.</returns>
        public static PrimitiveMultiValue PrimitiveMultiValue()
        {
            return PayloadBuilder.PrimitiveMultiValue(null);
        }

        /// <summary>
        /// Creates a new empty non-null typed primitive collection.
        /// </summary>
        /// <param name="fullTypeName">The full name of the type for the collection (can be null).</param>
        /// <returns>The new collection created.</returns>
        public static PrimitiveMultiValue PrimitiveMultiValue(string fullTypeName)
        {
            return new PrimitiveMultiValue(fullTypeName, false);
        }

        /// <summary>
        /// Creates a new empty non-null untyped complex collection.
        /// </summary>
        /// <returns>The new collection created.</returns>
        public static ComplexMultiValue ComplexMultiValue()
        {
            return PayloadBuilder.ComplexMultiValue(null);
        }

        /// <summary>
        /// Creates a new empty non-null typed complex collection.
        /// </summary>
        /// <param name="fullTypeName">The full name of the type for the collection (can be null).</param>
        /// <returns>The new collection created.</returns>
        public static ComplexMultiValue ComplexMultiValue(string fullTypeName)
        {
            return new ComplexMultiValue(fullTypeName, false);
        }

        /// <summary>
        /// Creates a new empty primitive collection.
        /// </summary>
        /// <returns>The new primitive collection created.</returns>
        public static PrimitiveCollection PrimitiveCollection()
        {
            return PayloadBuilder.PrimitiveCollection(null);
        }

        /// <summary>
        /// Creates a new empty primitive collection with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the collection (only in ATOM).</param>
        /// <returns>The new primitive collection created.</returns>
        public static PrimitiveCollection PrimitiveCollection(string name)
        {
            PrimitiveCollection collection = new PrimitiveCollection();
            if (name != null)
            {
                collection.AddAnnotation(new CollectionNameAnnotation { Name = name });
            }
            return collection;
        }

        /// <summary>
        /// Creates a new empty complex collection.
        /// </summary>
        /// <returns>The new complex collection created.</returns>
        public static ComplexInstanceCollection ComplexCollection()
        {
            return PayloadBuilder.ComplexCollection(null);
        }

        /// <summary>
        /// Creates a new empty complex collection with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the collection (only in ATOM).</param>
        /// <returns>The new complex collection created.</returns>
        public static ComplexInstanceCollection ComplexCollection(string name)
        {
            ComplexInstanceCollection collection = new ComplexInstanceCollection();
            if (name != null)
            {
                collection.AddAnnotation(new CollectionNameAnnotation { Name = name });
            }
            return collection;
        }

        /// <summary>
        /// Creates a new <see cref="ODataErrorPayload"/> with the specified <paramref name="code"/> (if any).
        /// </summary>
        /// <param name="code">The error code for the error; the default value is 'null'.</param>
        /// <returns>A new <see cref="ODataErrorPayload"/> instance with the error code set.</returns>
        public static ODataErrorPayload Error(string code = null)
        {
            return new ODataErrorPayload()
            {
                Code = code,
            };
        }

        /// <summary>
        /// Creates a new <see cref="ODataInternalExceptionPayload"/>.
        /// </summary>
        /// <returns>A new <see cref="ODataInternalExceptionPayload"/> instance.</returns>
        public static ODataInternalExceptionPayload InnerError()
        {
            return new ODataInternalExceptionPayload();
        }

        /// <summary>
        /// Creates a deferred link.
        /// </summary>
        /// <param name="uri">The URI string for the deferred link.</param>
        /// <returns>A new <see cref="DeferredLink"/> instance.</returns>
        public static DeferredLink DeferredLink(string uri)
        {
            return new DeferredLink
            {
                UriString = uri
            };
        }

        /// <summary>
        /// Create a new empty entity set.
        /// </summary>
        /// <returns>A new entity set instance.</returns>
        public static EntitySetInstance EntitySet()
        {
            return new EntitySetInstance();
        }

        /// <summary>
        /// Creates a new entity set instance
        /// </summary>
        /// <param name="nextLink">NextLink for the entity set instance</param>
        /// <param name="inlineCount">InlineCount for the entity set instance</param>
        /// <returns>a new entity set instance</returns>
        public static EntitySetInstance EntitySet(string nextLink = null, int? inlineCount = null)
        {
            return new EntitySetInstance().InlineCount(inlineCount).NextLink(nextLink);
        }

        /// <summary>
        /// Creates a new entity set instance
        /// </summary>
        /// <param name="nextLink">NextLink for the entity set instance</param>
        /// <param name="inlineCount">InlineCount for the entity set instance</param>
        /// <returns>a new entity set instance</returns>
        public static EntitySetInstance EntitySet(IEnumerable<EntityInstance> entries, string nextLink = null, int? inlineCount = null)
        {
            return PayloadBuilder.EntitySet(nextLink, inlineCount).Append(entries);
        }

        /// <summary>
        /// Creates a new deferred link collection.
        /// </summary>
        /// <returns>A new <see cref="LinkCollection"/> instance.</returns>
        public static LinkCollection LinkCollection()
        {
            return new LinkCollection();
        }

        /// <summary>
        /// Creates a new resource collection instance.
        /// </summary>
        /// <param name="title">Title for the resource collection. Will be used as the title as part of the ATOM metadata annotation as well as the name property.</param>
        /// <param name="href">Href for the resource collection.</param>
        /// <returns>A new resource collection instance.</returns>
        public static ResourceCollectionInstance ResourceCollection(string title, string href)
        {
            return new ResourceCollectionInstance() { Title = title, Href = href, Name = title };
        }

        /// <summary>
        /// Create a new empty service document.
        /// </summary>
        /// <returns>A new service document instance.</returns>
        public static ServiceDocumentInstance ServiceDocument()
        {
            return new ServiceDocumentInstance();
        }

        /// <summary>
        /// Create a new empty workspace.
        /// </summary>
        /// <returns>A new workspace instance.</returns>
        public static WorkspaceInstance Workspace()
        {
            return new WorkspaceInstance();
        }

        /// <summary>
        /// Appends a set of entities to a given entity set. Returns the entity set for composability.
        /// </summary>
        /// <param name="entitySet">The <see cref="EntitySetInstance"/> to add the <paramref name="entries"/> to.</param>
        /// <param name="entries">The entries to append to the <paramref name="entitySet"/>.</param>
        /// <returns>The <paramref name="entitySet" /> with all the <paramref name="entries" /> added.</returns>
        public static EntitySetInstance Append(this EntitySetInstance entitySet, IEnumerable<EntityInstance> entries)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");

            return entitySet.Append(entries == null ? null : entries.ToArray());
        }

        /// <summary>
        /// Appends a set of entities to a given entity set. Returns the entity set for composability.
        /// </summary>
        /// <param name="entitySet">The <see cref="EntitySetInstance"/> to add the <paramref name="entries"/> to.</param>
        /// <param name="entries">The entries to append to the <paramref name="entitySet"/>.</param>
        /// <returns>The <paramref name="entitySet" /> with all the <paramref name="entries" /> added.</returns>
        public static EntitySetInstance Append(this EntitySetInstance entitySet, params EntityInstance[] entries)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");

            return entitySet.InsertAt(entitySet.Count, entries);
        }

        /// <summary>
        /// Prepends a set of entities to a given entity set. Returns the entity set for composability.
        /// </summary>
        /// <param name="entitySet">The <see cref="EntitySetInstance"/> to prepend the <paramref name="entries"/> to.</param>
        /// <param name="entries">The entries to prepend to the <paramref name="entitySet"/>.</param>
        /// <returns>The <paramref name="entitySet" /> with all the <paramref name="entries" /> prepended.</returns>
        public static EntitySetInstance Prepend(this EntitySetInstance entitySet, IEnumerable<EntityInstance> entries)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");

            return entitySet.Prepend(entries == null ? null : entries.ToArray());
        }

        /// <summary>
        /// Prepends a set of entities to a given entity set. Returns the entity set for composability.
        /// </summary>
        /// <param name="entitySet">The <see cref="EntitySetInstance"/> to prepend the <paramref name="entries"/> to.</param>
        /// <param name="entries">The entries to prepend to the <paramref name="entitySet"/>.</param>
        /// <returns>The <paramref name="entitySet" /> with all the <paramref name="entries" /> prepended.</returns>
        public static EntitySetInstance Prepend(this EntitySetInstance entitySet, params EntityInstance[] entries)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");

            return entitySet.InsertAt(0, entries);
        }

        /// <summary>
        /// Inserts a set of entities at a given position in the given entity set. Returns the entity set for composability.
        /// </summary>
        /// <param name="entitySet">The <see cref="EntitySetInstance"/> to insert the <paramref name="entries"/> into.</param>
        /// <param name="entries">The entries to insert into the <paramref name="entitySet"/>.</param>
        /// <returns>The <paramref name="entitySet" /> with all the <paramref name="entries" /> inserted.</returns>
        public static EntitySetInstance InsertAt(this EntitySetInstance entitySet, int position, IEnumerable<EntityInstance> entries)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.Assert(position >= 0, "position >= 0");
            ExceptionUtilities.Assert(position <= entitySet.Count, "position <= entitySet.Count");

            return entitySet.InsertAt(position, entries == null ? null : entries.ToArray());
        }

        /// <summary>
        /// Inserts a set of entities at a given position in the given entity set. Returns the entity set for composability.
        /// </summary>
        /// <param name="entitySet">The <see cref="EntitySetInstance"/> to insert the <paramref name="entries"/> into.</param>
        /// <param name="entries">The entries to insert into the <paramref name="entitySet"/>.</param>
        /// <returns>The <paramref name="entitySet" /> with all the <paramref name="entries" /> inserted.</returns>
        public static EntitySetInstance InsertAt(this EntitySetInstance entitySet, int position, params EntityInstance[] entries)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.Assert(position >= 0, "position >= 0");
            ExceptionUtilities.Assert(position <= entitySet.Count, "position <= entitySet.Count");

            if (entries != null && entries.Length > 0)
            {
                if (position == entitySet.Count)
                {
                    entitySet.AddRange(entries);
                }
                else
                {
                    for (int i = entries.Length - 1; i >= 0; --i)
                    {
                        entitySet.Insert(position, entries[i]);
                    }
                }
            }

            return entitySet;
        }

        /// <summary>
        /// Adds specified InlineCount to entity set instance
        /// </summary>
        /// <param name="entitySet">entity set instance to add InlineCount to</param>
        /// <param name="count">InlineCount to be added</param>
        /// <returns>entity set instance with InlineCount Added</returns>
        public static EntitySetInstance InlineCount(this EntitySetInstance entitySet, int? count)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");

            entitySet.InlineCount = count;
            return entitySet;
        }

        /// <summary>
        /// Add next link to an entityset
        /// </summary>
        /// <param name="entitySet">entity set instance to add the next link to</param>
        /// <param name="nextLink">next link to add to the entity set instance</param>
        /// <returns>entity set instance with next link added</returns>
        public static EntitySetInstance NextLink(this EntitySetInstance entitySet, string nextLink)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            entitySet.NextLink = nextLink;
            return entitySet;
        }

        /// <summary>
        /// Create a new empty, non-null entity without a type name.
        /// </summary>
        /// <returns>A new entity instance (without type name).</returns>
        public static EntityInstance Entity()
        {
            return PayloadBuilder.Entity(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static EntityInstance NullEntity()
        {
            return new EntityInstance(null, true);
        }

        /// <summary>
        /// Create a new non-null, empty entity with the specified type name.
        /// </summary>
        /// <param name="fullTypeName">The full type name of the entity's type.</param>
        /// <returns>A new entity instance with the specified type name.</returns>
        public static EntityInstance Entity(string fullTypeName)
        {
            return new EntityInstance(fullTypeName, false);
        }

        /// <summary>
        /// Sets the ID of an entity.
        /// </summary>
        /// <param name="entity">The entity to set the ID on.</param>
        /// <param name="etag">The ID value.</param>
        /// <returns>The modified entity (for composability).</returns>
        public static EntityInstance Id(this EntityInstance entity, string id)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            entity.Id = id;
            return entity;
        }

        /// <summary>
        /// Sets the ETag of an entity.
        /// </summary>
        /// <param name="entity">The entity to set the ETag on.</param>
        /// <param name="etag">The ETag value.</param>
        /// <returns>The modified entity (for composability).</returns>
        public static EntityInstance ETag(this EntityInstance entity, string etag)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            entity.ETag = etag;
            return entity;
        }

        /// <summary>
        /// Sets the full type name of an entity.
        /// </summary>
        /// <param name="entity">The entity to set the type name on.</param>
        /// <param name="etag">The type name value.</param>
        /// <returns>The modified entity (for composability).</returns>
        public static EntityInstance FullTypeName(this EntityInstance entity, string fullTypeName)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            entity.FullTypeName = fullTypeName;
            return entity;
        }

        /// <summary>
        /// Sets the content type of the default stream of an entity.
        /// </summary>
        /// <param name="entity">The entity to set the default stream content type on.</param>
        /// <param name="contentType">The content type value.</param>
        /// <returns>The modified entity (for composability).</returns>
        public static EntityInstance StreamContentType(this EntityInstance entity, string contentType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            entity.StreamContentType = contentType;
            return entity;
        }

        /// <summary>
        /// Sets the ETag of the default stream of an entity.
        /// </summary>
        /// <param name="entity">The entity to set the default stream ETag on.</param>
        /// <param name="etag">The ETag value.</param>
        /// <returns>The modified entity (for composability).</returns>
        public static EntityInstance StreamETag(this EntityInstance entity, string etag)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            entity.StreamETag = etag;
            return entity;
        }

        /// <summary>
        /// Sets the default stream source link of an entity.
        /// </summary>
        /// <param name="entity">The entity to set the default stream source link on.</param>
        /// <param name="link">The link value.</param>
        /// <returns>The modified entity (for composability).</returns>
        public static EntityInstance StreamSourceLink(this EntityInstance entity, string link)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            entity.StreamSourceLink = link;
            return entity;
        }

        /// <summary>
        /// Sets the default stream edit link of an entity.
        /// </summary>
        /// <param name="entity">The entity to set the default stream edit link on.</param>
        /// <param name="link">The link value.</param>
        /// <returns>The modified entity (for composability).</returns>
        public static EntityInstance StreamEditLink(this EntityInstance entity, string link)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            entity.StreamEditLink = link;
            return entity;
        }

        /// <summary>
        /// Returns a property from entity or complex value by its name.
        /// </summary>
        /// <param name="payloadElement">The payload element to get the property from.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The property instance found, or null if no such property exists.</returns>
        /// <remarks>
        /// Note that EntityInstance derives from ComplexInstance so this method works on both.
        /// The method will throw if there's more than one such property.
        /// </remarks>
        public static PropertyInstance GetProperty(this ComplexInstance payloadElement, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckStringNotNullOrEmpty(propertyName, "propertyName");

            return payloadElement.Properties.SingleOrDefault(property => property.Name == propertyName);
        }

        /// <summary>
        /// Adds a property to a payload element (Entity, Complex).
        /// </summary>
        /// <typeparam name="T">The type of the payload element to add the property to.</typeparam>
        /// <param name="payloadElement">The payload element to add the property to.</param>
        /// <param name="propertyInstance">The property to add.</param>
        /// <returns>The payload element with the property added.</returns>
        /// <remarks>Note the EntityInstance derives from ComplexInstance, so this method works on both.</remarks>
        public static T Property<T>(this T payloadElement, PropertyInstance propertyInstance) where T : ComplexInstance
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckArgumentNotNull(propertyInstance, "propertyInstance");

            payloadElement.Add(propertyInstance);
            return payloadElement;
        }

        /// <summary>
        /// Adds a property to a payload element (Entity, Complex).
        /// </summary>
        /// <typeparam name="T">The type of the payload element to add the property to.</typeparam>
        /// <param name="payloadElement">The payload element to add the property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="value">The value of the property as a payload element.</param>
        /// <returns>The payload element with the property added.</returns>
        /// <remarks>Note the EntityInstance derives from ComplexInstance, so this method works on both.</remarks>
        public static T Property<T>(this T payloadElement, string propertyName, ODataPayloadElement value) where T : ComplexInstance
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            return payloadElement.Property(PayloadBuilder.Property(propertyName, value));
        }

        /// <summary>
        /// Adds a primitive property to a payload element (Entity, Complex).
        /// </summary>
        /// <typeparam name="T">The type of the payload element to add the property to.</typeparam>
        /// <param name="payloadElement">The payload element to add the property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="value">The primitive value of the property as a CLR instance.</param>
        /// <returns>The payload element with the property added.</returns>
        /// <remarks>Note the EntityInstance derives from ComplexInstance, so this method works on both.</remarks>
        public static T PrimitiveProperty<T>(this T payloadElement, string propertyName, object value) where T : ComplexInstance
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            payloadElement.Add(PayloadBuilder.PrimitiveProperty(propertyName, value));
            return payloadElement;
        }

        public static EntityInstance IsComplex(this EntityInstance entity, bool isComplex)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            entity.IsComplex = isComplex;
            return entity;
        }

        /// <summary>
        /// Adds a stream reference property to an entity instance.
        /// </summary>
        /// <param name="entity">The entity instance to add the stream reference property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="readLink">The read link for the stream reference value.</param>
        /// <param name="editLink">The edit link for the stream reference value.</param>
        /// <param name="contentType">The content type for the stream reference value.</param>
        /// <param name="etag">The ETag for the stream reference value.</param>
        /// <returns>The <paramref name="entity"/> with the added stream reference property, useful for composition.</returns>
        public static EntityInstance StreamProperty(this EntityInstance entity, string propertyName, string readLink = null, string editLink = null, string contentType = null, string etag = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            NamedStreamInstance namedStream = PayloadBuilder.StreamProperty(propertyName, readLink, editLink, contentType, etag);
            entity.Add(namedStream);
            return entity;
        }

        /// <summary>
        /// Adds a stream reference property to an entity instance.
        /// </summary>
        /// <param name="namedStream">The named stream instance to add.</param>
        /// <returns>The <paramref name="entity"/> with the added stream reference property, useful for composition.</returns>
        public static EntityInstance StreamProperty(this EntityInstance entity, NamedStreamInstance namedStream)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            entity.Add(namedStream);
            return entity;
        }

        /// <summary>
        /// Adds a deferred navigation property to an entity instance, including an optional association URL.
        /// </summary>
        /// <param name="entity">The entity instance to add the navigation property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="url">The URL to use for the deferred link of the navigation property.</param>
        /// <param name="associationUrl">The (optional) URL to use for the association link of the navigation property.</param>
        /// <returns>The <paramref name="entity"/> with the added deferred link navigation property, useful for composition.</returns>
        public static EntityInstance NavigationProperty(this EntityInstance entity, string propertyName, string url, string associationUrl = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            entity.Add(PayloadBuilder.NavigationProperty(propertyName, url, associationUrl));
            return entity;
        }

        /// <summary>
        /// Adds a navigation property to an entity instance.
        /// </summary>
        /// <param name="entity">The entity instance to add the navigation property to.</param>
        /// <param name="navigationProperty">The navigation property to add.</param>
        /// <returns>The <paramref name="entity"/> with the added navigation property, useful for composition.</returns>
        public static EntityInstance NavigationProperty(this EntityInstance entity, NavigationPropertyInstance navigationProperty)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            entity.Add(navigationProperty);
            return entity;
        }

        /// <summary>
        /// Adds an operation descriptor to an entity instance.
        /// </summary>
        /// <param name="entity">The entity instance to add the operation to.</param>
        /// <param name="operation">The operation to add.</param>
        /// <returns>The <paramref name="entity"/> with the added operation, useful for composition.</returns>
        public static EntityInstance OperationDescriptor(this EntityInstance entity, ServiceOperationDescriptor operation)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            entity.Add(operation);
            return entity;
        }

        /// <summary>
        /// Adds a deferred navigation property to an entity instance, including an optional association URL.
        /// </summary>
        /// <param name="entity">The entity instance to add the navigation property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="link">The deferred link.</param>
        /// <param name="associationLink">The (optional) URL to use for the association link of the navigation property.</param>
        /// <returns>The <paramref name="entity"/> with the added expanded link navigation property, useful for composition.</returns>
        public static EntityInstance DeferredNavigationProperty(this EntityInstance entity, string propertyName, DeferredLink link, DeferredLink associationLink = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            entity.Add(PayloadBuilder.DeferredNavigationProperty(propertyName, link, associationLink));
            return entity;
        }

        /// <summary>
        /// Adds an expanded link navigation property to an entity instance, including an optional association URL.
        /// </summary>
        /// <param name="entity">The entity instance to add the navigation property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="value">The value to be used as the content of the expanded link.</param>
        /// <param name="href">The (optional) URL to use for the link of the navigation property.</param>
        /// <returns>The <paramref name="entity"/> with the added expanded link navigation property, useful for composition.</returns>
        public static EntityInstance ExpandedNavigationProperty(this EntityInstance entity, string propertyName, ODataPayloadElement value, string href = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            entity.Add(PayloadBuilder.ExpandedNavigationProperty(propertyName, value, href, (string)null));
            return entity;
        }

        /// <summary>
        /// Adds an expanded link navigation property to an entity instance, including an optional association URL.
        /// </summary>
        /// <param name="entity">The entity instance to add the navigation property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="value">The value to be used as the content of the expanded link.</param>
        /// <param name="associationLink">The (optional) association link of the navigation property.</param>
        /// <returns>The <paramref name="entity"/> with the added expanded link navigation property, useful for composition.</returns>
        public static EntityInstance ExpandedNavigationProperty(this EntityInstance entity, string propertyName, ODataPayloadElement value, DeferredLink associationLink)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            entity.Add(PayloadBuilder.ExpandedNavigationProperty(propertyName, value, /*href*/null, associationLink));
            return entity;
        }

        /// <summary>
        /// Adds an operation to the entity instance.
        /// </summary>
        /// <param name="instance">The entity instance</param>
        /// <param name="contentTypeValue">The value for the annotation</param>
        /// <returns>The same entity instance</returns>
        public static EntityInstance Operation(this EntityInstance instance, ServiceOperationDescriptor operation)
        {
            instance.Add(operation);
            return instance;
        }

        /// <summary>
        /// Adds an XmlTreeAnnotation to the entity instance.
        /// </summary>
        /// <param name="entity">The entity instance to add the annotation.</param>
        /// <param name="xmlTree">The XML tree annotation to add.</param>
        /// <returns>The <paramref name="entity"/> with the added annotation.</returns>
        public static EntityInstance XmlTree(this EntityInstance entity, XmlTreeAnnotation xmlTree)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            entity.Add(xmlTree);
            return entity;
        }

        /// <summary>
        /// Applies a set of functions to the <paramref name="entity"/> and returns the modified entity at the end.
        /// </summary>
        /// <param name="entity">The entity to apply the functions to.</param>
        /// <param name="funcs">The set of functions to apply.</param>
        /// <returns>The modified <paramref name="entity"/>, useful for composition.</returns>
        public static EntityInstance Apply(this EntityInstance entity, IEnumerable<Func<EntityInstance, EntityInstance>> funcs)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            ExceptionUtilities.CheckArgumentNotNull(funcs, "funcs");

            foreach (Func<EntityInstance, EntityInstance> func in funcs)
            {
                entity = func(entity);
            }

            return entity;
        }

        /// <summary>
        /// Adds a new item to a primitive collection.
        /// </summary>
        /// <param name="collection">The collection to add the item to.</param>
        /// <param name="primitiveValue">The <see cref="PrimitiveValue"/> to add.</param>
        /// <returns>The <paramref name="collection"/> with the new item added.</returns>
        public static PrimitiveMultiValue Item(this PrimitiveMultiValue collection, PrimitiveValue primitiveValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            collection.Add(primitiveValue);
            return collection;
        }

        /// <summary>
        /// Adds a new item to a primitive collection.
        /// </summary>
        /// <param name="collection">The collection to add the item to.</param>
        /// <param name="itemValue">The CLR value to add.</param>
        /// <returns>The <paramref name="collection"/> with the new item added.</returns>
        public static PrimitiveMultiValue Item(this PrimitiveMultiValue collection, object itemValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            collection.Add(PayloadBuilder.PrimitiveValue(itemValue));
            return collection;
        }

        /// <summary>
        /// Adds new items to a primitive collection.
        /// </summary>
        /// <param name="collection">The collection to add the items to.</param>
        /// <param name="itemValues">The CLR values to add.</param>
        /// <returns>The <paramref name="collection"/> with the new items added.</returns>
        public static PrimitiveMultiValue Items(this PrimitiveMultiValue collection, IEnumerable<object> itemValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            ExceptionUtilities.CheckArgumentNotNull(itemValues, "itemValues");
            foreach (object itemValue in itemValues)
            {
                collection.Add(PayloadBuilder.PrimitiveValue(itemValue));
            }

            return collection;
        }

        /// <summary>
        /// Adds a new item to a complex collection.
        /// </summary>
        /// <param name="collection">The collection to add the item to.</param>
        /// <param name="itemValue">The complex value to add.</param>
        /// <returns>The <paramref name="collection"/> with the new item added.</returns>
        public static ComplexMultiValue Item(this ComplexMultiValue collection, ComplexInstance itemValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            collection.Add(itemValue);
            return collection;
        }

        /// <summary>
        /// Adds new items to a complex collection.
        /// </summary>
        /// <param name="collection">The collection to add the items to.</param>
        /// <param name="itemValues">The complex values to add.</param>
        /// <returns>The <paramref name="collection"/> with the new items added.</returns>
        public static ComplexMultiValue Items(this ComplexMultiValue collection, IEnumerable<ComplexInstance> itemValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            ExceptionUtilities.CheckArgumentNotNull(itemValues, "itemValues");
            foreach (ComplexInstance itemValue in itemValues)
            {
                collection.Add(itemValue);
            }

            return collection;
        }

        /// <summary>
        /// Adds a new workspace to a service document.
        /// </summary>
        /// <param name="serviceDocument">The service document to add the workspace to.</param>
        /// <param name="workspace">The workspace to add.</param>
        /// <returns>The <paramref name="serviceDocument"/> with the new workspace added.</returns>
        public static ServiceDocumentInstance Workspace(this ServiceDocumentInstance serviceDocument, WorkspaceInstance workspace)
        {
            ExceptionUtilities.CheckArgumentNotNull(serviceDocument, "serviceDocument");
            ExceptionUtilities.CheckArgumentNotNull(workspace, "workspace");

            serviceDocument.Workspaces.Add(workspace);
            return serviceDocument;
        }

        /// <summary>
        /// Sets the title of a workspace.
        /// </summary>
        /// <param name="workspace">The workspace to set the title on.</param>
        /// <param name="title">The title to set.</param>
        /// <returns>The <paramref name="workspace"/> with the title set.</returns>
        public static WorkspaceInstance WithTitle(this WorkspaceInstance workspace, string title)
        {
            ExceptionUtilities.CheckArgumentNotNull(workspace, "workspace");

            workspace.Title = title;
            return workspace;
        }

        /// <summary>
        /// Adds a resource collection to a workspace.
        /// </summary>
        /// <param name="workspace">The workspace to add the resource collection to.</param>
        /// <param name="collection">The resource collection to add.</param>
        /// <returns>The <paramref name="workspace"/> with the resource collection added.</returns>
        public static WorkspaceInstance ResourceCollection(this WorkspaceInstance workspace, ResourceCollectionInstance collection)
        {
            ExceptionUtilities.CheckArgumentNotNull(workspace, "workspace");
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");

            workspace.ResourceCollections.Add(collection);
            return workspace;
        }

        /// <summary>
        /// Adds a resource collection to a workspace.
        /// </summary>
        /// <param name="workspace">The workspace to add the resource collection to.</param>
        /// <param name="title">The title of the resource collection to add.</param>
        /// <param name="href">The href of the resource collection to add.</param>
        /// <returns>The <paramref name="workspace"/> with the resource collection added.</returns>
        public static WorkspaceInstance ResourceCollection(this WorkspaceInstance workspace, string title, string href)
        {
            // In ATOM, the title annotation and the name property on the resource collection are the same, so copy the "title" value to the name property here.
            return workspace.ResourceCollection(title, href, title);
        }

        /// <summary>
        /// Adds a resource collection to a workspace.
        /// </summary>
        /// <param name="workspace">The workspace to add the resource collection to.</param>
        /// <param name="title">The title of the resource collection to add.</param>
        /// <param name="href">The href of the resource collection to add.</param>
        /// <param name="name">The name of the resource collection to add.</param>
        /// <returns>The <paramref name="workspace"/> with the resource collection added.</returns>
        public static WorkspaceInstance ResourceCollection(this WorkspaceInstance workspace, string title, string href, string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(workspace, "workspace");

            workspace.ResourceCollections.Add(new ResourceCollectionInstance { Title = title, Href = href, Name = name });
            return workspace;
        }

        /// <summary>
        /// Sets the <paramref name="code"/> of the error.
        /// </summary>
        /// <param name="error">The <see cref="ODataErrorPayload"/> to set the value and language for.</param>
        /// <param name="code">The error code of the error.</param>
        /// <returns>The <paramref name="error"/> instance (for composability reasons).</returns>
        public static ODataErrorPayload Code(this ODataErrorPayload error, string code = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(error, "error");
            error.Code = code;
            return error;
        }

        /// <summary>
        /// Sets the <paramref name="value"/> and <paramref name="language"/> of the error.
        /// </summary>
        /// <param name="error">The <see cref="ODataErrorPayload"/> to set the value and language for.</param>
        /// <param name="value">The value (i.e., message) of the error.</param>
        /// <returns>The <paramref name="error"/> instance (for composability reasons).</returns>
        public static ODataErrorPayload Message(this ODataErrorPayload error, string value = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(error, "error");
            error.Message = value;
            return error;
        }

        /// <summary>
        /// Sets the inner error message of the error.
        /// </summary>
        /// <param name="error">The <see cref="ODataErrorPayload"/> to set the value and language for.</param>
        /// <param name="innererror">The inner error.</param>
        /// <returns>The <paramref name="error"/> instance (for composability reasons).</returns>
        public static ODataErrorPayload InnerError(this ODataErrorPayload error, ODataInternalExceptionPayload innererror)
        {
            ExceptionUtilities.CheckArgumentNotNull(error, "error");
            error.InnerError = innererror;
            return error;
        }

        /// <summary>
        /// Sets the message of the inner error.
        /// </summary>
        /// <param name="innerError">The <see cref="ODataInternalExceptionPayload"/> to set the message for.</param>
        /// <param name="message">The message of the inner error.</param>
        /// <returns>The <paramref name="innerError"/> instance (for composability reasons).</returns>
        public static ODataInternalExceptionPayload Message(this ODataInternalExceptionPayload innerError, string message = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(innerError, "innerError");
            innerError.Message = message;
            return innerError;
        }

        /// <summary>
        /// Sets the type name of the inner error.
        /// </summary>
        /// <param name="innerError">The <see cref="ODataInternalExceptionPayload"/> to set the type name for.</param>
        /// <param name="typeName">The type name of the inner error.</param>
        /// <returns>The <paramref name="innerError"/> instance (for composability reasons).</returns>
        public static ODataInternalExceptionPayload TypeName(this ODataInternalExceptionPayload innerError, string typeName = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(innerError, "innerError");
            innerError.TypeName = typeName;
            return innerError;
        }

        /// <summary>
        /// Sets the stack trace of the inner error.
        /// </summary>
        /// <param name="innerError">The <see cref="ODataInternalExceptionPayload"/> to set the stack trace for.</param>
        /// <param name="stackTrace">The stack trace of the inner error.</param>
        /// <returns>The <paramref name="innerError"/> instance (for composability reasons).</returns>
        public static ODataInternalExceptionPayload StackTrace(this ODataInternalExceptionPayload innerError, string stackTrace = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(innerError, "innerError");

            innerError.StackTrace = stackTrace;
            return innerError;
        }

        /// <summary>
        /// Sets the inner error message of the error.
        /// </summary>
        /// <param name="innerError">The <see cref="ODataErrorPayload"/> to set the value and language for.</param>
        /// <param name="nestedInnerError">The nested inner error to set on the <paramref name="innerError"/>.</param>
        /// <returns>The <paramref name="innerError"/> instance (for composability reasons).</returns>
        public static ODataInternalExceptionPayload InnerError(this ODataInternalExceptionPayload innerError, ODataInternalExceptionPayload nestedInnerError)
        {
            ExceptionUtilities.CheckArgumentNotNull(innerError, "innerError");

            innerError.InternalException = nestedInnerError;
            return innerError;
        }

        /// <summary>
        /// Adds a <see cref="IsMediaLinkEntryAnnotation"/> annotation to the <paramref name="entity"/>
        /// if none exists.
        /// </summary>
        /// <param name="entity">The entity to mark as media link entry.</param>
        /// <returns>The <paramref name="entity"/> marked as media link entry, useful for composition.</returns>
        public static EntityInstance EnsureMediaLinkEntry(this EntityInstance entity)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            bool isMediaLinkEntry = entity.Annotations.OfType<IsMediaLinkEntryAnnotation>().Any();
            return isMediaLinkEntry ? entity : entity.AsMediaLinkEntry();
        }

        /// <summary>
        /// Generated the specified number of entries by making a copy of the current entry. 
        /// TODO: Decide what we want to do with the properties, we might need to have a flag to incide whether we want to use
        /// random values or fixed values.
        /// </summary>
        /// <param name="currentEntry">entry to make a copy off</param>
        /// <param name="numberOfEntries">number of entries to create</param>
        /// <returns>specified number of entries which are a copy of the original entry</returns>
        public static IEnumerable<EntityInstance> GenerateSimilarEntries(this EntityInstance currentEntry, int numberOfEntries)
        {
            return Enumerable.Range(0, numberOfEntries).Select(x => ((EntityInstance)currentEntry.DeepCopy()));
        }

        /// <summary>
        /// Marks the deferred link as pointing to collection or not.
        /// Note that this should only be used for ATOM tests, as JSON doesn't have this distinction.
        /// </summary>
        /// <param name="link">The link to set the flag on.</param>
        /// <param name="isCollection">true if the link represents a collection; false if it represents a singleton.
        /// Do not call this method if the link "doesn't know".</param>
        /// <returns>The <paramref name="link"/> after the collection flag was set.</returns>
        public static DeferredLink IsCollection(this DeferredLink link, bool isCollection)
        {
            ExceptionUtilities.CheckArgumentNotNull(link, "link");

            // The way to mark the link as collection or not is to add a content type annotation to it
            link.Add(new ContentTypeAnnotation(MimeTypes.ApplicationAtomXml + (isCollection ? ";type=feed" : ";type=entry")));
            return link;
        }

        /// <summary>
        /// Adds the collection name to the <paramref name="primitiveCollection"/>.
        /// </summary>
        /// <param name="primitiveCollection">The primitive collection to add the collection name to.</param>
        /// <param name="collectionName">The collection name to be added.</param>
        /// <returns>The <paramref name="primitiveCollection"/> with the collection name added.</returns>
        public static PrimitiveCollection CollectionName(this PrimitiveCollection primitiveCollection, string collectionName)
        {
            ExceptionUtilities.CheckArgumentNotNull(primitiveCollection, "primitiveCollection");

            if (collectionName == null)
            {
                return primitiveCollection;
            }
            else
            {
                return primitiveCollection.AddAnnotation(new CollectionNameAnnotation() { Name = collectionName }); 
            }
        }

        /// <summary>
        /// Adds a new item to a primitive collection.
        /// </summary>
        /// <param name="collection">The collection to add the item to.</param>
        /// <param name="item">The item to add.</param>
        /// <returns>The <paramref name="collection"/> with the new item added.</returns>
        public static PrimitiveCollection Item(this PrimitiveCollection collection, PrimitiveValue item)
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            collection.Add(item);
            return collection;
        }

        /// <summary>
        /// Adds a new item to a primitive collection.
        /// </summary>
        /// <param name="collection">The collection to add the item to.</param>
        /// <param name="itemValue">The primitive CLR value for the item to add.</param>
        /// <returns>The <paramref name="collection"/> with the new item added.</returns>
        public static PrimitiveCollection Item(this PrimitiveCollection collection, object itemValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            collection.Add(PayloadBuilder.PrimitiveValue(itemValue));
            return collection;
        }

        /// <summary>
        /// Adds the collection name to the <paramref name="complexCollection"/>.
        /// </summary>
        /// <param name="complexCollection">The complex instance collection to add the collection name to.</param>
        /// <param name="collectionName">The collection name to be added.</param>
        /// <returns>The <paramref name="complexCollection"/> with the collection name added</returns>
        public static ComplexInstanceCollection CollectionName(this ComplexInstanceCollection complexCollection, string collectionName)
        {
            ExceptionUtilities.CheckArgumentNotNull(complexCollection, "complexCollection");

            if (collectionName == null)
            {
                return complexCollection;
            }
            else
            {
                return complexCollection.AddAnnotation(new CollectionNameAnnotation() {Name = collectionName});
            }
        }

        /// <summary>
        /// Adds a new item to a complex collection.
        /// </summary>
        /// <param name="collection">The collection to add the item to.</param>
        /// <param name="item">The item to add.</param>
        /// <returns>The <paramref name="collection"/> with the new item added.</returns>
        public static ComplexInstanceCollection Item(this ComplexInstanceCollection collection, ComplexInstance item)
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            collection.Add(item);
            return collection;
        }

        /// <summary>
        /// Adds an inline count to the <paramref name="linkCollection"/>.
        /// </summary>
        /// <param name="linkCollection">The link collection to add the inline count to.</param>
        /// <param name="inlineCount">The inline count to add.</param>
        /// <returns>The <paramref name="linkCollection"/> with the <paramref name="inlineCount"/> added; for composability.</returns>
        public static LinkCollection InlineCount(this LinkCollection linkCollection, long? inlineCount)
        {
            ExceptionUtilities.CheckArgumentNotNull(linkCollection, "linkCollection");

            linkCollection.InlineCount = inlineCount;
            return linkCollection;
        }

        /// <summary>
        /// Adds a next link to the <paramref name="linkCollection"/>.
        /// </summary>
        /// <param name="linkCollection">The link collection to add the next link to.</param>
        /// <param name="nextLink">The next link to add.</param>
        /// <returns>The <paramref name="linkCollection"/> with the <paramref name="nextLink"/> added; for composability.</returns>
        public static LinkCollection NextLink(this LinkCollection linkCollection, string nextLink)
        {
            ExceptionUtilities.CheckArgumentNotNull(linkCollection, "linkCollection");

            linkCollection.NextLink = nextLink;
            return linkCollection;
        }

        /// <summary>
        /// Adds a new link to a link collection.
        /// </summary>
        /// <param name="linkCollection">The link collection to add the item to.</param>
        /// <param name="item">The link to add.</param>
        /// <returns>The <paramref name="linkCollection"/> with the <paramref name="item"/> added; for composability.</returns>
        public static LinkCollection Item(this LinkCollection linkCollection, Link item)
        {
            ExceptionUtilities.CheckArgumentNotNull(linkCollection, "linkCollection");

            linkCollection.Add(item);
            return linkCollection;
        }

        /// <summary>
        /// Add the <see cref="EntityModelTypeAnnotation"/> with the specified entity type to the payload element.
        /// </summary>
        /// <param name="payloadElement">The payload element to add the <see cref="EntityModelTypeAnnotation"/> to.</param>
        /// <param name="entityType">The entity type to create the annotation with.</param>
        /// <returns>The <paramref name="payloadElement"/> after adding the <see cref="EntityModelTypeAnnotation"/> to it.</returns>
        public static T WithTypeAnnotation<T>(this T payloadElement, EdmEntityType entityType, bool nullable = true) where T : ODataPayloadElement
        {
            return payloadElement.WithTypeAnnotation(entityType.ToTypeReference(nullable));
        }

        /// <summary>
        /// Add the <see cref="EntityModelTypeAnnotation"/> with the specified data type to the payload element.
        /// </summary>
        /// <param name="payloadElement">The payload element to add the <see cref="EntityModelTypeAnnotation"/> to.</param>
        /// <param name="dataType">The data type to create the annotation with.</param>
        /// <param name="nullable">Nullable or not.</param>
        /// <returns>The <paramref name="payloadElement"/> after adding the <see cref="EntityModelTypeAnnotation"/> to it.</returns>
        public static T WithTypeAnnotation<T>(this T payloadElement, IEdmType dataType, bool nullable = false) where T : ODataPayloadElement
        {
            if (dataType != null)
            {
                EntityModelTypeAnnotation annotation = new EntityModelTypeAnnotation(dataType.ToTypeReference(nullable));
                payloadElement.AddAnnotation(annotation);
            }

            return payloadElement;
        }

        /// <summary>
        /// Add the <see cref="EntityModelTypeAnnotation"/> with the specified data type to the payload element.
        /// </summary>
        /// <param name="payloadElement">The payload element to add the <see cref="EntityModelTypeAnnotation"/> to.</param>
        /// <param name="dataType">The data type to create the annotation with.</param>
        /// <returns>The <paramref name="payloadElement"/> after adding the <see cref="EntityModelTypeAnnotation"/> to it.</returns>
        public static T WithTypeAnnotation<T>(this T payloadElement, IEdmTypeReference dataType) where T : ODataPayloadElement
        {
            if (dataType != null)
            {
                EntityModelTypeAnnotation annotation = new EntityModelTypeAnnotation(dataType);
                payloadElement.AddAnnotation(annotation);
            }

            return payloadElement;
        }

        /// <summary>
        /// Adds random property values to the entityInstance
        /// </summary>
        /// <param name="entity">entity to add the Random Property values to</param>
        /// <param name="randomDataGeneratorResolver">random data generator resolver</param>
        /// <param name="randomNumberGenerator">random number generator</param>
        /// <param name="random">random flag to indicate whether random values should be used</param>
        public static EntityInstance OverwriteWithRandomPropertyValues(this EntityInstance entity, IRandomDataGeneratorResolver randomDataGeneratorResolver, IRandomNumberGenerator randomNumberGenerator)
        {
            entity.Properties.SetRandomValues(randomDataGeneratorResolver, randomNumberGenerator);
            return entity;
        }

        /// <summary>
        /// Adds random property values to the complexInstance
        /// </summary>
        /// <param name="complex">complexInstance to add property values to</param>
        /// <param name="randomDataGeneratorResolver">random data generator resolver</param>
        /// <param name="randomNumberGenerator">random number generator</param>
        /// <returns>complexInstance with property values added</returns>
        public static ComplexInstance OverwriteWithRandomPropertyValues(this ComplexInstance complex, IRandomDataGeneratorResolver randomDataGeneratorResolver, IRandomNumberGenerator randomNumberGenerator)
        {
            complex.Properties.SetRandomValues(randomDataGeneratorResolver, randomNumberGenerator);
            return complex;
        }

        /// <summary>
        /// Sets random property values for properties. It actually overwrites existing property values if any
        /// </summary>
        /// <param name="properties">Properties to set random values for</param>
        /// <param name="randomDataGeneratorResolver">Resolver for random data generators</param>
        /// <param name="randomNumberGenerator">Random number generator</param>
        private static void SetRandomValues(this IEnumerable<PropertyInstance> properties, IRandomDataGeneratorResolver randomDataGeneratorResolver, IRandomNumberGenerator randomNumberGenerator)
        {
            foreach (var property in properties)
            {
                var primitive = property as PrimitiveProperty;
                if (primitive != null)
                {
                    Type propertyClrType = null;

                    var primitiveDataType = ModelBuilder.GetPayloadEdmElementPropertyValueType(primitive) as IEdmPrimitiveTypeReference;
                    if (primitiveDataType != null)
                    {
                        propertyClrType = MetadataUtils.GetPrimitiveClrType(primitiveDataType);
                    }
                    else if (primitive.Value.ClrValue != null)
                    {
                        propertyClrType = primitive.Value.ClrValue.GetType();
                    }

                    if (propertyClrType != null)
                    {
                        IDataGenerator dataGenerator = randomDataGeneratorResolver.ResolveRandomDataGenerator(propertyClrType, randomNumberGenerator, DataGenerationHints.NoNulls);
                        primitive.Value = new PrimitiveValue(primitive.Value.FullTypeName, dataGenerator.GenerateData());
                    }
                    else
                    {
                        // We cannot infer whether the type is nullable or not, so disallow them for now
                        throw new NotSupportedException(string.Format("property:{0} has not type associated with it", property.ToString()));
                    }
                }

                var complex = property as ComplexProperty;
                if (complex != null)
                {
                    complex.Value.Properties.SetRandomValues(randomDataGeneratorResolver, randomNumberGenerator);
                }

                // TODO: add support for collection properties and expanded navigation properties.
            }
        }

        private static ODataUri BuildODataUri(string baseUrl)
        {
            Uri serviceRootUri = new Uri(baseUrl);
            ODataUriSegment serviceRoot = ODataUriBuilder.Root(serviceRootUri);
            ODataUriSegment[] uriSegment = new ODataUriSegment[] { serviceRoot };
            ODataUri odataUri = new ODataUri(uriSegment);
            return odataUri;
        }

        private static HttpRequestData CreateDefaultRequestData()
        {
            var encoding = Encoding.UTF8;
            HttpRequestData requestData = new HttpRequestData();
            requestData.Headers.Add("HeaderName", "HeaderValue");
            requestData.Uri = new Uri("http://www.odata.org/Customers");
            requestData.Body = encoding.GetBytes("TestPayloadData");
            return requestData;
        }

        private static HttpResponseData CreateDefaultResponseData()
        {
            var encoding = Encoding.UTF8;
            HttpResponseData responseData = new HttpResponseData();
            responseData.Headers.Add("HeaderName", "HeaderValue");
            responseData.StatusCode = HttpStatusCode.OK;
            responseData.Body = encoding.GetBytes("TestPayloadData");
            return responseData;
        }
    }
}
