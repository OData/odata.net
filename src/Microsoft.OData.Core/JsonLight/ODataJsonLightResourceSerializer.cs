//---------------------------------------------------------------------
// <copyright file="ODataJsonLightResourceSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;

namespace Microsoft.OData.JsonLight
{
    /// <summary>
    /// OData JsonLight serializer for resources and resource sets.
    /// </summary>
    internal sealed class ODataJsonLightResourceSerializer : ODataJsonLightPropertySerializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        internal ODataJsonLightResourceSerializer(ODataJsonLightOutputContext jsonLightOutputContext)
            : base(jsonLightOutputContext, /*initContextUriBuilder*/ true)
        {
        }

        /// <summary>
        /// Gets the base Uri of the metadata document uri, if it has been set.
        /// </summary>
        private Uri MetadataDocumentBaseUri
        {
            get
            {
                // Note: If we are in no-metadata mode or serializing a request, we don't require the MetadataDocumentUri to be set.
                return this.JsonLightOutputContext.MessageWriterSettings.MetadataDocumentUri;
            }
        }

        /// <summary>
        /// Writes the metadata properties for a resource set which can only occur at the start.
        /// </summary>
        /// <param name="resourceSet">The resource set for which to write the metadata properties.</param>
        /// <param name="propertyName">The name of the property for which to write the resource set.</param>
        /// <param name="expectedResourceTypeName">The expected resource type name of the items in the resource set.</param>
        /// <param name="isUndeclared">true if the resource set is for an undeclared property</param>
        internal void WriteResourceSetStartMetadataProperties(ODataResourceSet resourceSet, string propertyName, string expectedResourceTypeName, bool isUndeclared)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            // Write the "@odata.type": "#typename"
            string typeName = this.JsonLightOutputContext.TypeNameOracle.GetResourceSetTypeNameForWriting(
                expectedResourceTypeName,
                resourceSet,
                isUndeclared);

            if (typeName != null && !typeName.Contains(ODataConstants.ContextUriFragmentUntyped))
            {
                if (propertyName == null)
                {
                    this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeName);
                }
                else
                {
                    this.ODataAnnotationWriter.WriteODataTypePropertyAnnotation(propertyName, typeName);
                }
            }
        }

        /// <summary>
        /// Writes the metadata properties for a resource which can only occur at the start.
        /// </summary>
        /// <param name="resourceState">The resource state for which to write the metadata properties.</param>
        internal void WriteResourceStartMetadataProperties(IODataJsonLightWriterResourceState resourceState)
        {
            Debug.Assert(resourceState != null, "resourceState != null");

            ODataResourceBase resource = resourceState.Resource;

            string expectedResourceTypeName = GetExpectedResourceTypeName(resourceState);

            // Write the "@odata.type": "typename"
            string typeName = this.JsonLightOutputContext.TypeNameOracle.GetResourceTypeNameForWriting(
                expectedResourceTypeName,
                resource,
                resourceState.IsUndeclared);

            if (typeName != null && !string.Equals(typeName, ODataConstants.ContextUriFragmentUntyped, StringComparison.Ordinal))
            {
                this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeName);
            }

            // Write the "@odata.id": "Entity Id"
            Uri id;
            if (resource.MetadataBuilder.TryGetIdForSerialization(out id))
            {
                this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataId);
                if (id != null && !resource.HasNonComputedId)
                {
                    id = this.MetadataDocumentBaseUri.MakeRelativeUri(id);
                }

                this.JsonWriter.WriteValue(id == null ? null : this.UriToString(id));
            }

            // Write the "@odata.etag": "ETag value"
            string etag = resource.ETag;
            if (etag != null)
            {
                this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataETag);
                this.JsonWriter.WriteValue(etag);
            }
        }

        /// <summary>
        /// Writes the metadata properties for a resource which can occur both at the start or at the end.
        /// </summary>
        /// <param name="resourceState">The resource state for which to write the metadata properties.</param>
        /// <remarks>
        /// This method will only write properties which were not written yet.
        /// </remarks>
        internal void WriteResourceMetadataProperties(IODataJsonLightWriterResourceState resourceState)
        {
            Debug.Assert(resourceState != null, "resourceState != null");

            ODataResourceBase resource = resourceState.Resource;

            // Write the "@odata.editLink": "edit-link-uri"
            if (resource.EditLink != null && !resourceState.EditLinkWritten)
            {
                this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataEditLink);
                this.JsonWriter.WriteValue(GetEditLinkForWriting(resource));
                resourceState.EditLinkWritten = true;
            }

            // Write the "@odata.readLink": "read-link-uri".
            // If readlink is identical to editlink, don't write readlink.
            if (resource.ReadLink != null && resource.ReadLink != resource.EditLink && !resourceState.ReadLinkWritten)
            {
                this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataReadLink);
                this.JsonWriter.WriteValue(GetReadLinkForWriting(resource));
                resourceState.ReadLinkWritten = true;
            }

            // Write MLE metadata
            ODataStreamReferenceValue mediaResource = resource.MediaResource;
            if (mediaResource != null)
            {
                // Write the "@odata.mediaEditLink": "edit-link-uri"
                if (mediaResource.EditLink != null && !resourceState.MediaEditLinkWritten)
                {
                    this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataMediaEditLink);
                    this.JsonWriter.WriteValue(GetEditLinkForWriting(mediaResource));
                    resourceState.MediaEditLinkWritten = true;
                }

                // Write the "@odata.mediaReadLink": "read-link-uri"
                // If mediaReadLink is identical to mediaEditLink, don't write readlink.
                if (mediaResource.ReadLink != null && mediaResource.ReadLink != mediaResource.EditLink && !resourceState.MediaReadLinkWritten)
                {
                    this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataMediaReadLink);
                    this.JsonWriter.WriteValue(GetReadLinkForWriting(mediaResource));
                    resourceState.MediaReadLinkWritten = true;
                }

                // Write the "@odata.mediaContentType": "content/type"
                string mediaContentType = mediaResource.ContentType;
                if (mediaContentType != null && !resourceState.MediaContentTypeWritten)
                {
                    this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataMediaContentType);
                    this.JsonWriter.WriteValue(mediaContentType);
                    resourceState.MediaContentTypeWritten = true;
                }

                // Write the "@odata.mediaEtag": "ETAG"
                string mediaETag = mediaResource.ETag;
                if (mediaETag != null && !resourceState.MediaETagWritten)
                {
                    this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataMediaETag);
                    this.JsonWriter.WriteValue(mediaETag);
                    resourceState.MediaETagWritten = true;
                }
            }

            // TODO: actions
            // TODO: functions
            // TODO: association links
        }

        /// <summary>
        /// Writes the metadata properties for a resource which can only occur at the end.
        /// </summary>
        /// <param name="resourceState">The resource state for which to write the metadata properties.</param>
        /// <param name="duplicatePropertyNameChecker">The DuplicatePropertyNameChecker to use.</param>
        internal void WriteResourceEndMetadataProperties(IODataJsonLightWriterResourceState resourceState, IDuplicatePropertyNameChecker duplicatePropertyNameChecker)
        {
            Debug.Assert(resourceState != null, "resourceState != null");

            ODataResourceBase resource = resourceState.Resource;

            // write computed navigation properties
            ODataJsonLightReaderNestedResourceInfo navigationLinkInfo = resource.MetadataBuilder.GetNextUnprocessedNavigationLink();
            while (navigationLinkInfo != null)
            {
                Debug.Assert(resource.MetadataBuilder != null, "resource.MetadataBuilder != null");
                navigationLinkInfo.NestedResourceInfo.MetadataBuilder = resource.MetadataBuilder;

                this.WriteNavigationLinkMetadata(navigationLinkInfo.NestedResourceInfo, duplicatePropertyNameChecker);
                navigationLinkInfo = resource.MetadataBuilder.GetNextUnprocessedNavigationLink();
            }

            // write computed stream properties
            ODataProperty streamProperty = resource.MetadataBuilder.GetNextUnprocessedStreamProperty();
            while (streamProperty != null)
            {
                this.WriteProperty(streamProperty, resourceState.ResourceType, /*isTopLevel*/ false, duplicatePropertyNameChecker, null /*metadataBuilder*/);
                streamProperty = resource.MetadataBuilder.GetNextUnprocessedStreamProperty();
            }

            // write "odata.actions" metadata
            IEnumerable<ODataAction> actions = resource.Actions;
            if (actions != null && actions.Any())
            {
                this.WriteOperations(actions.Cast<ODataOperation>(), /*isAction*/ true);
            }

            // write "odata.functions" metadata
            IEnumerable<ODataFunction> functions = resource.Functions;
            if (functions != null && functions.Any())
            {
                this.WriteOperations(functions.Cast<ODataOperation>(), /*isAction*/ false);
            }
        }

        /// <summary>
        /// Writes the navigation link metadata.
        /// </summary>
        /// <param name="nestedResourceInfo">The navigation link to write the metadata for.</param>
        /// <param name="duplicatePropertyNameChecker">The DuplicatePropertyNameChecker to use.</param>
        internal void WriteNavigationLinkMetadata(ODataNestedResourceInfo nestedResourceInfo, IDuplicatePropertyNameChecker duplicatePropertyNameChecker)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(!string.IsNullOrEmpty(nestedResourceInfo.Name), "The nested resource info Name should have been validated by now.");
            Debug.Assert(duplicatePropertyNameChecker != null);

            Uri navigationLinkUrl = nestedResourceInfo.Url;
            string navigationLinkName = nestedResourceInfo.Name;
            Uri associationLinkUrl = nestedResourceInfo.AssociationLinkUrl;
            if (associationLinkUrl != null)
            {
                duplicatePropertyNameChecker.ValidatePropertyOpenForAssociationLink(navigationLinkName);
                this.WriteAssociationLink(nestedResourceInfo.Name, associationLinkUrl);
            }

            if (navigationLinkUrl != null)
            {
                // The navigation link URL is a property annotation "NestedResourceInfoName@odata.navigationLinkUrl: 'url'"
                this.ODataAnnotationWriter.WritePropertyAnnotationName(navigationLinkName, ODataAnnotationNames.ODataNavigationLinkUrl);
                this.JsonWriter.WriteValue(this.UriToString(navigationLinkUrl));
            }
        }

        /// <summary>
        /// Writes the navigation link metadata.
        /// </summary>
        /// <param name="nestedResourceInfo">The navigation link to write the metadata for.</param>
        /// <param name="contextUrlInfo">The contextUrl information for current element.</param>
        internal void WriteNestedResourceInfoContextUrl(ODataNestedResourceInfo nestedResourceInfo, ODataContextUrlInfo contextUrlInfo)
        {
            this.WriteContextUriProperty(ODataPayloadKind.Resource, () => contextUrlInfo, /* parentContextUrlInfo*/ null, nestedResourceInfo.Name);
        }

        /// <summary>
        /// Writes "actions" or "functions" metadata.
        /// </summary>
        /// <param name="operations">The operations to write.</param>
        /// <param name="isAction">true when writing the resource's actions; false when writing the resource's functions.</param>
        internal void WriteOperations(IEnumerable<ODataOperation> operations, bool isAction)
        {
            // We cannot compare two URIs directly because the 'Equals' method on the 'Uri' class compares two 'Uri' instances without regard to the
            // fragment part of the URI. (E.G: For 'http://someuri/index.htm#EC.action1' and http://someuri/index.htm#EC.action2', the 'Equals' method
            // will return true.
            IEnumerable<IGrouping<string, ODataOperation>> metadataGroups = operations.GroupBy(o =>
            {
                // We need to validate here to ensure that the metadata is not null, otherwise call to the method 'UriToString' will throw.
                ValidationUtils.ValidateOperationNotNull(o, isAction);
                WriterValidationUtils.ValidateCanWriteOperation(o, this.JsonLightOutputContext.WritingResponse);
                ODataJsonLightValidationUtils.ValidateOperation(this.MetadataDocumentBaseUri, o);
                return this.GetOperationMetadataString(o);
            });

            foreach (IGrouping<string, ODataOperation> metadataGroup in metadataGroups)
            {
                this.WriteOperationMetadataGroup(metadataGroup);
            }
        }

        /// <summary>
        /// Tries to writes the context URI property for delta resource/resource set/link into the payload if one is available.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the resource.</param>
        /// <param name="kind">The delta kind to write.</param>
        /// <param name="parentContextUrlInfo">The parent contextUrlInfo.</param>
        /// <returns>The created context uri info.</returns>
        internal ODataContextUrlInfo WriteDeltaContextUri(ODataResourceTypeContext typeContext, ODataDeltaKind kind, ODataContextUrlInfo parentContextUrlInfo = null)
        {
            ODataUri odataUri = this.JsonLightOutputContext.MessageWriterSettings.ODataUri;
            return this.WriteContextUriProperty(ODataPayloadKind.Delta, () => ODataContextUrlInfo.Create(typeContext, this.MessageWriterSettings.Version ?? ODataVersion.V4, kind, odataUri), parentContextUrlInfo);
        }

        /// <summary>
        /// Tries to writes the context URI property for a resource into the payload if one is available.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the resource.</param>
        /// <param name="parentContextUrlInfo">The parent contextUrlInfo.</param>
        /// <returns>The created context uri info.</returns>
        internal ODataContextUrlInfo WriteResourceContextUri(ODataResourceTypeContext typeContext, ODataContextUrlInfo parentContextUrlInfo = null)
        {
            ODataUri odataUri = this.JsonLightOutputContext.MessageWriterSettings.ODataUri;
            return this.WriteContextUriProperty(ODataPayloadKind.Resource, () => ODataContextUrlInfo.Create(typeContext, this.MessageWriterSettings.Version ?? ODataVersion.V4, /* isSingle */ true, odataUri), parentContextUrlInfo);
        }

        /// <summary>
        /// Tries to writes the context URI property for a resource set into the payload if one is available.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the resource set.</param>
        /// <returns>The contextUrlInfo, if the context URI was successfully written.</returns>
        internal ODataContextUrlInfo WriteResourceSetContextUri(ODataResourceTypeContext typeContext)
        {
            ODataUri odataUri = this.JsonLightOutputContext.MessageWriterSettings.ODataUri;
            return this.WriteContextUriProperty(ODataPayloadKind.ResourceSet, () => ODataContextUrlInfo.Create(typeContext, this.MessageWriterSettings.Version ?? ODataVersion.V4, /* isSingle */ false, odataUri));
        }

        /// <summary>
        /// Asynchronously writes the metadata properties for a resource set which can only occur at the start.
        /// </summary>
        /// <param name="resourceSet">The resource set for which to write the metadata properties.</param>
        /// <param name="propertyName">The name of the property for which to write the resource set.</param>
        /// <param name="expectedResourceTypeName">The expected resource type name of the items in the resource set.</param>
        /// <param name="isUndeclared">true if the resource set is for an undeclared property</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal async Task WriteResourceSetStartMetadataPropertiesAsync(
            ODataResourceSet resourceSet,
            string propertyName,
            string expectedResourceTypeName,
            bool isUndeclared)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            // Write the "@odata.type": "#typename"
            string typeName = this.JsonLightOutputContext.TypeNameOracle.GetResourceSetTypeNameForWriting(
                expectedResourceTypeName,
                resourceSet,
                isUndeclared);

            if (typeName != null && !typeName.Contains(ODataConstants.ContextUriFragmentUntyped))
            {
                if (propertyName == null)
                {
                    await this.AsynchronousODataAnnotationWriter.WriteODataTypeInstanceAnnotationAsync(typeName)
                        .ConfigureAwait(false);
                }
                else
                {
                    await this.AsynchronousODataAnnotationWriter.WriteODataTypePropertyAnnotationAsync(propertyName, typeName)
                        .ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Asynchronously writes the metadata properties for a resource which can only occur at the start.
        /// </summary>
        /// <param name="resourceState">The resource state for which to write the metadata properties.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal async Task WriteResourceStartMetadataPropertiesAsync(IODataJsonLightWriterResourceState resourceState)
        {
            Debug.Assert(resourceState != null, "resourceState != null");

            ODataResourceBase resource = resourceState.Resource;

            string expectedResourceTypeName = GetExpectedResourceTypeName(resourceState);
            // Write the "@odata.type": "typename"
            string typeName = this.JsonLightOutputContext.TypeNameOracle.GetResourceTypeNameForWriting(
                expectedResourceTypeName,
                resource,
                resourceState.IsUndeclared);

            if (typeName != null && !string.Equals(typeName, ODataConstants.ContextUriFragmentUntyped, StringComparison.Ordinal))
            {
                await this.AsynchronousODataAnnotationWriter.WriteODataTypeInstanceAnnotationAsync(typeName)
                    .ConfigureAwait(false);
            }

            // Write the "@odata.id": "Entity Id"
            Uri id;
            if (resource.MetadataBuilder.TryGetIdForSerialization(out id))
            {
                await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataId)
                    .ConfigureAwait(false);
                if (id != null && !resource.HasNonComputedId)
                {
                    id = this.MetadataDocumentBaseUri.MakeRelativeUri(id);
                }

                await this.AsynchronousJsonWriter.WriteValueAsync(id == null ? null : this.UriToString(id))
                    .ConfigureAwait(false);
            }

            // Write the "@odata.etag": "ETag value"
            string etag = resource.ETag;
            if (etag != null)
            {
                await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataETag)
                    .ConfigureAwait(false);
                await this.AsynchronousJsonWriter.WriteValueAsync(etag)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes the metadata properties for a resource which can occur both at the start or at the end.
        /// </summary>
        /// <param name="resourceState">The resource state for which to write the metadata properties.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <remarks>
        /// This method will only write properties which were not written yet.
        /// </remarks>
        internal async Task WriteResourceMetadataPropertiesAsync(IODataJsonLightWriterResourceState resourceState)
        {
            Debug.Assert(resourceState != null, "resourceState != null");

            ODataResourceBase resource = resourceState.Resource;

            // Write the "@odata.editLink": "edit-link-uri"
            if (resource.EditLink != null && !resourceState.EditLinkWritten)
            {
                await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataEditLink)
                    .ConfigureAwait(false);
                await this.AsynchronousJsonWriter.WriteValueAsync(GetEditLinkForWriting(resource))
                    .ConfigureAwait(false);
                resourceState.EditLinkWritten = true;
            }

            // Write the "@odata.readLink": "read-link-uri".
            // If readlink is identical to editlink, don't write readlink.
            if (resource.ReadLink != null && resource.ReadLink != resource.EditLink && !resourceState.ReadLinkWritten)
            {
                await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataReadLink)
                    .ConfigureAwait(false);

                await this.AsynchronousJsonWriter.WriteValueAsync(GetReadLinkForWriting(resource))
                    .ConfigureAwait(false);
                resourceState.ReadLinkWritten = true;
            }

            // Write MLE metadata
            ODataStreamReferenceValue mediaResource = resource.MediaResource;
            if (mediaResource != null)
            {
                // Write the "@odata.mediaEditLink": "edit-link-uri"
                if (mediaResource.EditLink != null && !resourceState.MediaEditLinkWritten)
                {
                    await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataMediaEditLink)
                        .ConfigureAwait(false);
                    await this.AsynchronousJsonWriter.WriteValueAsync(GetEditLinkForWriting(mediaResource))
                        .ConfigureAwait(false);
                    resourceState.MediaEditLinkWritten = true;
                }

                // Write the "@odata.mediaReadLink": "read-link-uri"
                // If mediaReadLink is identical to mediaEditLink, don't write readlink.
                if (mediaResource.ReadLink != null && mediaResource.ReadLink != mediaResource.EditLink && !resourceState.MediaReadLinkWritten)
                {
                    await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataMediaReadLink)
                        .ConfigureAwait(false);
                    await this.AsynchronousJsonWriter.WriteValueAsync(GetReadLinkForWriting(mediaResource))
                        .ConfigureAwait(false);
                    resourceState.MediaReadLinkWritten = true;
                }

                // Write the "@odata.mediaContentType": "content/type"
                string mediaContentType = mediaResource.ContentType;
                if (mediaContentType != null && !resourceState.MediaContentTypeWritten)
                {
                    await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataMediaContentType)
                        .ConfigureAwait(false);
                    await this.AsynchronousJsonWriter.WriteValueAsync(mediaContentType)
                        .ConfigureAwait(false);
                    resourceState.MediaContentTypeWritten = true;
                }

                // Write the "@odata.mediaEtag": "ETAG"
                string mediaETag = mediaResource.ETag;
                if (mediaETag != null && !resourceState.MediaETagWritten)
                {
                    await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataMediaETag)
                        .ConfigureAwait(false);
                    await this.AsynchronousJsonWriter.WriteValueAsync(mediaETag)
                        .ConfigureAwait(false);
                    resourceState.MediaETagWritten = true;
                }
            }
        }

        /// <summary>
        /// Asynchronously writes the metadata properties for a resource which can only occur at the end.
        /// </summary>
        /// <param name="resourceState">The resource state for which to write the metadata properties.</param>
        /// <param name="duplicatePropertyNameChecker">The DuplicatePropertyNameChecker to use.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal async Task WriteResourceEndMetadataPropertiesAsync(
            IODataJsonLightWriterResourceState resourceState,
            IDuplicatePropertyNameChecker duplicatePropertyNameChecker)
        {
            Debug.Assert(resourceState != null, "resourceState != null");

            ODataResourceBase resource = resourceState.Resource;

            // Write computed navigation properties
            ODataJsonLightReaderNestedResourceInfo navigationLinkInfo = resource.MetadataBuilder.GetNextUnprocessedNavigationLink();
            while (navigationLinkInfo != null)
            {
                Debug.Assert(resource.MetadataBuilder != null, "resource.MetadataBuilder != null");
                navigationLinkInfo.NestedResourceInfo.MetadataBuilder = resource.MetadataBuilder;

                await this.WriteNavigationLinkMetadataAsync(navigationLinkInfo.NestedResourceInfo, duplicatePropertyNameChecker)
                    .ConfigureAwait(false);
                navigationLinkInfo = resource.MetadataBuilder.GetNextUnprocessedNavigationLink();
            }

            // Write computed stream properties
            ODataProperty streamProperty = resource.MetadataBuilder.GetNextUnprocessedStreamProperty();
            while (streamProperty != null)
            {
                await this.WritePropertyAsync(
                    streamProperty,
                    resourceState.ResourceType,
                    /*isTopLevel*/ false,
                    duplicatePropertyNameChecker,
                    null /*metadataBuilder*/).ConfigureAwait(false);
                streamProperty = resource.MetadataBuilder.GetNextUnprocessedStreamProperty();
            }

            // Write "odata.actions" metadata
            IEnumerable<ODataAction> actions = resource.Actions;
            if (actions != null && actions.Any())
            {
                await this.WriteOperationsAsync(actions.Cast<ODataOperation>(), /*isAction*/ true)
                    .ConfigureAwait(false);
            }

            // Write "odata.functions" metadata
            IEnumerable<ODataFunction> functions = resource.Functions;
            if (functions != null && functions.Any())
            {
                await this.WriteOperationsAsync(functions.Cast<ODataOperation>(), /*isAction*/ false)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes the navigation link metadata.
        /// </summary>
        /// <param name="nestedResourceInfo">The navigation link to write the metadata for.</param>
        /// <param name="duplicatePropertyNameChecker">The DuplicatePropertyNameChecker to use.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal async Task WriteNavigationLinkMetadataAsync(ODataNestedResourceInfo nestedResourceInfo, IDuplicatePropertyNameChecker duplicatePropertyNameChecker)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(!string.IsNullOrEmpty(nestedResourceInfo.Name), "The nested resource info Name should have been validated by now.");
            Debug.Assert(duplicatePropertyNameChecker != null);

            Uri navigationLinkUrl = nestedResourceInfo.Url;
            string navigationLinkName = nestedResourceInfo.Name;
            Uri associationLinkUrl = nestedResourceInfo.AssociationLinkUrl;
            if (associationLinkUrl != null)
            {
                duplicatePropertyNameChecker.ValidatePropertyOpenForAssociationLink(navigationLinkName);
                await this.WriteAssociationLinkAsync(nestedResourceInfo.Name, associationLinkUrl)
                    .ConfigureAwait(false);
            }

            if (navigationLinkUrl != null)
            {
                // The navigation link URL is a property annotation "NestedResourceInfoName@odata.navigationLinkUrl: 'url'"
                await this.AsynchronousODataAnnotationWriter.WritePropertyAnnotationNameAsync(navigationLinkName, ODataAnnotationNames.ODataNavigationLinkUrl)
                    .ConfigureAwait(false);
                await this.AsynchronousJsonWriter.WriteValueAsync(this.UriToString(navigationLinkUrl))
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes the navigation link metadata.
        /// </summary>
        /// <param name="nestedResourceInfo">The navigation link to write the metadata for.</param>
        /// <param name="contextUrlInfo">The contextUrl information for current element.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal Task WriteNestedResourceInfoContextUrlAsync(ODataNestedResourceInfo nestedResourceInfo, ODataContextUrlInfo contextUrlInfo)
        {
            return this.WriteContextUriPropertyAsync(
                ODataPayloadKind.Resource,
                () => contextUrlInfo,
                /* parentContextUrlInfo*/ null, nestedResourceInfo.Name);
        }

        /// <summary>
        /// Asynchronously writes "actions" or "functions" metadata.
        /// </summary>
        /// <param name="operations">The operations to write.</param>
        /// <param name="isAction">true when writing the resource's actions; false when writing the resource's functions.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal async Task WriteOperationsAsync(IEnumerable<ODataOperation> operations, bool isAction)
        {
            // We cannot compare two URIs directly because the 'Equals' method on the 'Uri' class compares two 'Uri' instances without regard to the
            // fragment part of the URI. (E.G: For 'http://someuri/index.htm#EC.action1' and http://someuri/index.htm#EC.action2', the 'Equals' method
            // will return true.
            IEnumerable<IGrouping<string, ODataOperation>> metadataGroups = operations.GroupBy(o =>
            {
                // We need to validate here to ensure that the metadata is not null, otherwise call to the method 'UriToString' will throw.
                ValidationUtils.ValidateOperationNotNull(o, isAction);
                WriterValidationUtils.ValidateCanWriteOperation(o, this.JsonLightOutputContext.WritingResponse);
                ODataJsonLightValidationUtils.ValidateOperation(this.MetadataDocumentBaseUri, o);
                return this.GetOperationMetadataString(o);
            });

            foreach (IGrouping<string, ODataOperation> metadataGroup in metadataGroups)
            {
                await this.WriteOperationMetadataGroupAsync(metadataGroup)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously tries to write the context URI property for delta resource/resource set/link into the payload if one is available.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the resource.</param>
        /// <param name="kind">The delta kind to write.</param>
        /// <param name="parentContextUrlInfo">The parent contextUrlInfo.</param>
        /// <returns>A task that represents the asynchronous write operation. 
        /// The value of the TResult parameter contains the created context uri info.</returns>
        internal Task<ODataContextUrlInfo> WriteDeltaContextUriAsync(ODataResourceTypeContext typeContext, ODataDeltaKind kind, ODataContextUrlInfo parentContextUrlInfo = null)
        {
            ODataUri odataUri = this.JsonLightOutputContext.MessageWriterSettings.ODataUri;

            return this.WriteContextUriPropertyAsync(
                ODataPayloadKind.Delta,
                () => ODataContextUrlInfo.Create(typeContext, this.MessageWriterSettings.Version ?? ODataVersion.V4, kind, odataUri),
                parentContextUrlInfo);
        }

        /// <summary>
        /// Asynchronously tries to write the context URI property for a resource into the payload if one is available.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the resource.</param>
        /// <param name="parentContextUrlInfo">The parent contextUrlInfo.</param>
        /// <returns>A task that represents the asynchronous write operation. 
        /// The value of the TResult parameter contains the created context uri info.</returns>
        internal Task<ODataContextUrlInfo> WriteResourceContextUriAsync(ODataResourceTypeContext typeContext, ODataContextUrlInfo parentContextUrlInfo = null)
        {
            ODataUri odataUri = this.JsonLightOutputContext.MessageWriterSettings.ODataUri;

            return this.WriteContextUriPropertyAsync(
                ODataPayloadKind.Resource,
                () => ODataContextUrlInfo.Create(typeContext, this.MessageWriterSettings.Version ?? ODataVersion.V4, /* isSingle */ true, odataUri),
                parentContextUrlInfo);
        }

        /// <summary>
        /// Asynchronously tries to write the context URI property for a resource set into the payload if one is available.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the resource set.</param>
        /// <returns>A task that represents the asynchronous write operation. 
        /// The value of the TResult parameter contains the created context uri info.</returns>
        internal Task<ODataContextUrlInfo> WriteResourceSetContextUriAsync(ODataResourceTypeContext typeContext)
        {
            ODataUri odataUri = this.JsonLightOutputContext.MessageWriterSettings.ODataUri;

            return this.WriteContextUriPropertyAsync(
                ODataPayloadKind.ResourceSet,
                () => ODataContextUrlInfo.Create(typeContext, this.MessageWriterSettings.Version ?? ODataVersion.V4, /* isSingle */ false, odataUri));
        }

        /// <summary>
        /// Writes an association link property annotation.
        /// </summary>
        /// <param name="propertyName">The name of the navigation property for which to write the association link.</param>
        /// <param name="associationLinkUrl">The association link URL to write.</param>
        private void WriteAssociationLink(string propertyName, Uri associationLinkUrl)
        {
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(associationLinkUrl != null, "associationLinkUrl != null");

            // The association link URL is a property annotation "NestedResourceInfoName@odata.associationLinkUrl: 'url'"
            this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataAssociationLinkUrl);
            this.JsonWriter.WriteValue(this.UriToString(associationLinkUrl));
        }

        /// <summary>
        /// Gets the metadata reference fragment from the operation context uri.
        /// i.e. if the operation context uri is {absolute metadata document uri}#{container-qualified-operation-name},
        /// this method will return #{container-qualified-operation-name}.
        /// </summary>
        /// <param name="operation">Operation in question.</param>
        /// <returns>The metadata reference fragment from the operation context uri.</returns>
        private string GetOperationMetadataString(ODataOperation operation)
        {
            Debug.Assert(operation != null && operation.Metadata != null, "operation != null && operation.Metadata != null");

            string operationMetadataString = UriUtils.UriToString(operation.Metadata);
            Debug.Assert(ODataJsonLightUtils.IsMetadataReferenceProperty(operationMetadataString), "ODataJsonLightUtils.IsMetadataReferenceProperty(operationMetadataString)");

            // If we don't have a metadata document URI (which is the case with nometadata mode), just write the string form of the Uri we were given.
            if (this.MetadataDocumentBaseUri == null)
            {
                return operation.Metadata.Fragment;
            }

            Debug.Assert(
                !ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(this.MetadataDocumentBaseUri, operationMetadataString),
                "Open metadata reference property is not supported, we should have thrown before this point.");

            return ODataConstants.ContextUriFragmentIndicator + ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.MetadataDocumentBaseUri, operationMetadataString);
        }

        /// <summary>
        /// Returns the target uri string from the given operation.
        /// </summary>
        /// <param name="operation">Operation in question.</param>
        /// <returns>Returns the target uri string from the given operation.</returns>
        private string GetOperationTargetUriString(ODataOperation operation)
        {
            return operation.Target == null ? null : this.UriToString(operation.Target);
        }

        /// <summary>
        /// Validates a group of operations with the same context Uri.
        /// </summary>
        /// <param name="operations">Operations to validate.</param>
        private void ValidateOperationMetadataGroup(IGrouping<string, ODataOperation> operations)
        {
            Debug.Assert(operations != null, "operations must not be null.");
            Debug.Assert(operations.Any(), "operations.Any()");
            Debug.Assert(operations.All(o => this.GetOperationMetadataString(o) == operations.Key), "The operations should be grouped by their metadata.");

            if (operations.Count() > 1 && operations.Any(o => o.Target == null))
            {
                throw new ODataException(Strings.ODataJsonLightResourceSerializer_ActionsAndFunctionsGroupMustSpecifyTarget(operations.Key));
            }

            foreach (IGrouping<string, ODataOperation> operationsByTarget in operations.GroupBy(this.GetOperationTargetUriString))
            {
                if (operationsByTarget.Count() > 1)
                {
                    throw new ODataException(Strings.ODataJsonLightResourceSerializer_ActionsAndFunctionsGroupMustNotHaveDuplicateTarget(operations.Key, operationsByTarget.Key));
                }
            }
        }

        /// <summary>
        /// Writes a group of operation (all actions or all functions) that have the same "metadata".
        /// </summary>
        /// <remarks>
        /// Expects the actions or functions scope to already be open.
        /// </remarks>
        /// <param name="operations">A grouping of operations that are all actions or all functions and share the same "metadata".</param>
        private void WriteOperationMetadataGroup(IGrouping<string, ODataOperation> operations)
        {
            this.ValidateOperationMetadataGroup(operations);
            this.JsonLightOutputContext.JsonWriter.WriteName(operations.Key);
            bool useArray = operations.Count() > 1;
            if (useArray)
            {
                this.JsonLightOutputContext.JsonWriter.StartArrayScope();
            }

            foreach (ODataOperation operation in operations)
            {
                this.WriteOperation(operation);
            }

            if (useArray)
            {
                this.JsonLightOutputContext.JsonWriter.EndArrayScope();
            }
        }

        /// <summary>
        /// Writes an operation (an action or a function).
        /// </summary>
        /// <remarks>
        /// Expects the write to already have written the "rel value" and opened an array.
        /// </remarks>
        /// <param name="operation">The operation to write.</param>
        private void WriteOperation(ODataOperation operation)
        {
            Debug.Assert(operation != null, "operation must not be null.");
            Debug.Assert(operation.Metadata != null, "operation.Metadata != null");

            this.JsonLightOutputContext.JsonWriter.StartObjectScope();

            if (operation.Title != null)
            {
                this.JsonLightOutputContext.JsonWriter.WriteName(JsonConstants.ODataOperationTitleName);
                this.JsonLightOutputContext.JsonWriter.WriteValue(operation.Title);
            }

            if (operation.Target != null)
            {
                string targetUrlString = this.GetOperationTargetUriString(operation);
                this.JsonLightOutputContext.JsonWriter.WriteName(JsonConstants.ODataOperationTargetName);
                this.JsonLightOutputContext.JsonWriter.WriteValue(targetUrlString);
            }

            this.JsonLightOutputContext.JsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Asynchronously writes an association link property annotation.
        /// </summary>
        /// <param name="propertyName">The name of the navigation property for which to write the association link.</param>
        /// <param name="associationLinkUrl">The association link URL to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteAssociationLinkAsync(string propertyName, Uri associationLinkUrl)
        {
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(associationLinkUrl != null, "associationLinkUrl != null");

            // The association link URL is a property annotation "NestedResourceInfoName@odata.associationLinkUrl: 'url'"
            await this.AsynchronousODataAnnotationWriter.WritePropertyAnnotationNameAsync(propertyName, ODataAnnotationNames.ODataAssociationLinkUrl)
                .ConfigureAwait(false);
            await this.AsynchronousJsonWriter.WriteValueAsync(this.UriToString(associationLinkUrl))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes a group of operation (all actions or all functions) that have the same "metadata".
        /// </summary>
        /// <remarks>
        /// Expects the actions or functions scope to already be open.
        /// </remarks>
        /// <param name="operations">A grouping of operations that are all actions or all functions and share the same "metadata".</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteOperationMetadataGroupAsync(IGrouping<string, ODataOperation> operations)
        {
            this.ValidateOperationMetadataGroup(operations);
            await this.JsonLightOutputContext.AsynchronousJsonWriter.WriteNameAsync(operations.Key)
                .ConfigureAwait(false);
            bool useArray = operations.Count() > 1;
            if (useArray)
            {
                await this.JsonLightOutputContext.AsynchronousJsonWriter.StartArrayScopeAsync()
                    .ConfigureAwait(false);
            }

            foreach (ODataOperation operation in operations)
            {
                await this.WriteOperationAsync(operation)
                    .ConfigureAwait(false);
            }

            if (useArray)
            {
                await this.JsonLightOutputContext.AsynchronousJsonWriter.EndArrayScopeAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes an operation (an action or a function).
        /// </summary>
        /// <param name="operation">The operation to write.</param>
        /// <remarks>
        /// Expects the write to already have written the "rel value" and opened an array.
        /// </remarks>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteOperationAsync(ODataOperation operation)
        {
            Debug.Assert(operation != null, "operation must not be null.");
            Debug.Assert(operation.Metadata != null, "operation.Metadata != null");

            await this.JsonLightOutputContext.AsynchronousJsonWriter.StartObjectScopeAsync()
                .ConfigureAwait(false);

            if (operation.Title != null)
            {
                await this.JsonLightOutputContext.AsynchronousJsonWriter.WriteNameAsync(JsonConstants.ODataOperationTitleName)
                    .ConfigureAwait(false);
                await this.JsonLightOutputContext.AsynchronousJsonWriter.WriteValueAsync(operation.Title)
                    .ConfigureAwait(false);
            }

            if (operation.Target != null)
            {
                string targetUrlString = this.GetOperationTargetUriString(operation);
                await this.JsonLightOutputContext.AsynchronousJsonWriter.WriteNameAsync(JsonConstants.ODataOperationTargetName)
                    .ConfigureAwait(false);
                await this.JsonLightOutputContext.AsynchronousJsonWriter.WriteValueAsync(targetUrlString)
                    .ConfigureAwait(false);
            }

            await this.JsonLightOutputContext.AsynchronousJsonWriter.EndObjectScopeAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the expected resource type name.
        /// </summary>
        /// <param name="resourceState">The resource state for which to write the metadata properties.</param>
        /// <returns>The expected resource type name.</returns>
        private string GetExpectedResourceTypeName(IODataJsonLightWriterResourceState resourceState)
        {
            // expectedResourceTypeName : if expected is of base type. but the resource real type is derived,
            // we need to set the resource type name.
            //      Writing response: expected type info can be identical with context uri info.
            //      Writing request: contextUri may not be provided, we always use the type from metadata info.
            //                       From model: if the resource can be found in model.
            //                       From serializationInfo: if user set the serializationInfo for the resource.

            string expectedResourceTypeName;
            if (this.WritingResponse)
            {
                expectedResourceTypeName = resourceState.GetOrCreateTypeContext(this.WritingResponse).ExpectedResourceTypeName;
            }
            else
            {
                if (resourceState.ResourceTypeFromMetadata == null)
                {
                    expectedResourceTypeName = resourceState.SerializationInfo == null
                     ? null
                     : resourceState.SerializationInfo.ExpectedTypeName;
                }
                else
                {
                    expectedResourceTypeName = resourceState.ResourceTypeFromMetadata.FullTypeName();
                }
            }

            return expectedResourceTypeName;
        }

        /// <summary>
        /// Determines the edit link uri to write to the payload.
        /// </summary>
        /// <param name="resource">The resource for which to write the edit link uri.</param>
        /// <returns>The edit link uri to write to the payload.</returns>
        private string GetEditLinkForWriting(ODataResourceBase resource)
        {
            Debug.Assert(resource != null, "resource != null");
            Debug.Assert(resource.EditLink != null, "resource.EditLink != null");

            Uri editLink = resource.EditLink;

            return this.UriToString(resource.HasNonComputedEditLink || !editLink.IsAbsoluteUri ?
                editLink : this.MetadataDocumentBaseUri.MakeRelativeUri(editLink));
        }

        /// <summary>
        /// Determines the read link uri to write to the payload.
        /// </summary>
        /// <param name="resource">The resource for which to write the read link uri.</param>
        /// <returns>The read link uri to write to the payload.</returns>
        private string GetReadLinkForWriting(ODataResourceBase resource)
        {
            Debug.Assert(resource != null, "resource != null");
            Debug.Assert(resource.ReadLink != null, "resource.ReadLink != null");

            Uri readLink = resource.ReadLink;

            return this.UriToString(resource.HasNonComputedReadLink ?
                readLink : this.MetadataDocumentBaseUri.MakeRelativeUri(readLink));
        }

        /// <summary>
        /// Determines the edit link uri to write to the payload.
        /// </summary>
        /// <param name="resource">The media resource for which to write the edit link uri.</param>
        /// <returns>The edit link uri to write to the payload.</returns>
        private string GetEditLinkForWriting(ODataStreamReferenceValue mediaResource)
        {
            Debug.Assert(mediaResource != null, "mediaResource != null");
            Debug.Assert(mediaResource.EditLink != null, "mediaResource.EditLink != null");

            Uri editLink = mediaResource.EditLink;

            return this.UriToString(mediaResource.HasNonComputedEditLink ?
                editLink : this.MetadataDocumentBaseUri.MakeRelativeUri(editLink));
        }

        /// <summary>
        /// Determines the read link uri to write to the payload.
        /// </summary>
        /// <param name="resource">The media resource for which to write the read link uri.</param>
        /// <returns>The read link uri to write to the payload.</returns>
        private string GetReadLinkForWriting(ODataStreamReferenceValue mediaResource)
        {
            Debug.Assert(mediaResource != null, "mediaResource != null");
            Debug.Assert(mediaResource.ReadLink != null, "mediaResource.ReadLink != null");

            Uri readLink = mediaResource.ReadLink;

            return this.UriToString(mediaResource.HasNonComputedReadLink ?
                readLink : this.MetadataDocumentBaseUri.MakeRelativeUri(readLink));
        }
    }
}
