//---------------------------------------------------------------------
// <copyright file="ODataJsonLightWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;

namespace Microsoft.OData.JsonLight
{
    /// <summary>
    /// Implementation of the ODataWriter for the JsonLight format.
    /// </summary>
    internal sealed class ODataJsonLightWriter : ODataWriterCore
    {
        /// <summary>
        /// The output context to write to.
        /// </summary>
        private readonly ODataJsonLightOutputContext jsonLightOutputContext;

        /// <summary>
        /// The JsonLight resource and resource set serializer to use.
        /// </summary>
        private readonly ODataJsonLightResourceSerializer jsonLightResourceSerializer;

        /// <summary>
        /// The JsonLight value serializer to use for primitive values in an untyped collection.
        /// </summary>
        private readonly ODataJsonLightValueSerializer jsonLightValueSerializer;

        /// <summary>
        /// The JsonLight property serializer.
        /// </summary>
        private readonly ODataJsonLightPropertySerializer jsonLightPropertySerializer;

        /// <summary>
        /// True if the writer was created for writing a parameter; false otherwise.
        /// </summary>
        private readonly bool writingParameter;

        /// <summary>
        /// The underlying JSON writer.
        /// </summary>
        private readonly IJsonWriter jsonWriter;

        /// <summary>
        /// The underlying JSON writer.
        /// </summary>
        private readonly IJsonStreamWriter jsonStreamWriter;

        /// <summary>
        /// OData annotation writer.
        /// </summary>
        private readonly JsonLightODataAnnotationWriter odataAnnotationWriter;

        /// <summary>
        /// The underlying asynchronous JSON writer.
        /// </summary>
        private readonly IJsonWriterAsync asynchronousJsonWriter;

        /// <summary>
        /// The underlying asynchronous JSON writer.
        /// </summary>
        private readonly IJsonStreamWriterAsync asynchronousJsonStreamWriter;

        /// <summary>
        /// OData asynchronous annotation writer.
        /// </summary>
        private readonly JsonLightODataAnnotationWriter asynchronousODataAnnotationWriter;

        /// <summary>
        /// OData instance annotation writer
        /// </summary>
        private readonly JsonLightInstanceAnnotationWriter instanceAnnotationWriter;

        /// <summary>
        /// True if the writer was created for writing a response; false otherwise.
        /// </summary>
        private readonly bool writingResponse;

        /// <summary>
        /// The message writer settings to be used for writing.
        /// </summary>
        private readonly ODataMessageWriterSettings messageWriterSettings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
        /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <param name="writingResourceSet">true if the writer is created for writing a resource set; false when it is created for writing a resource.</param>
        /// <param name="writingParameter">true if the writer is created for writing a parameter; false otherwise.</param>
        /// <param name="writingDelta">True if the writer is created for writing delta payload; false otherwise.</param>
        /// <param name="listener">If not null, the writer will notify the implementer of the interface of relevant state changes in the writer.</param>
        internal ODataJsonLightWriter(
            ODataJsonLightOutputContext jsonLightOutputContext,
            IEdmNavigationSource navigationSource,
            IEdmStructuredType resourceType,
            bool writingResourceSet,
            bool writingParameter = false,
            bool writingDelta = false,
            IODataReaderWriterListener listener = null)
            : base(jsonLightOutputContext, navigationSource, resourceType, writingResourceSet, writingDelta, listener)
        {
            Debug.Assert(jsonLightOutputContext != null, "jsonLightOutputContext != null");

            this.jsonLightOutputContext = jsonLightOutputContext;
            this.jsonLightResourceSerializer = new ODataJsonLightResourceSerializer(this.jsonLightOutputContext);
            this.jsonLightValueSerializer = new ODataJsonLightValueSerializer(this.jsonLightOutputContext);
            this.jsonLightPropertySerializer = new ODataJsonLightPropertySerializer(this.jsonLightOutputContext);

            this.writingParameter = writingParameter;
            this.jsonWriter = this.jsonLightOutputContext.JsonWriter;
            this.jsonStreamWriter = this.jsonWriter as IJsonStreamWriter;
            this.odataAnnotationWriter = this.jsonLightValueSerializer.ODataAnnotationWriter;
            this.asynchronousJsonWriter = this.jsonLightOutputContext.AsynchronousJsonWriter;
            this.asynchronousJsonStreamWriter = this.asynchronousJsonWriter as IJsonStreamWriterAsync;
            this.asynchronousODataAnnotationWriter = this.jsonLightValueSerializer.AsynchronousODataAnnotationWriter;
            this.instanceAnnotationWriter = this.jsonLightValueSerializer.InstanceAnnotationWriter;
            this.writingResponse = this.jsonLightOutputContext.WritingResponse;
            this.messageWriterSettings = this.jsonLightOutputContext.MessageWriterSettings;
        }

        /// <summary>
        /// Returns the current JsonLightResourceScope.
        /// </summary>
        private JsonLightResourceScope CurrentResourceScope
        {
            get
            {
                JsonLightResourceScope currentJsonLightResourceScope = this.CurrentScope as JsonLightResourceScope;
                Debug.Assert(currentJsonLightResourceScope != null, "Asking for JsonLightResourceScope when the current scope is not an JsonLightResourceScope.");
                return currentJsonLightResourceScope;
            }
        }

        /// <summary>
        /// Returns the current JsonLightDeletedResourceScope.
        /// </summary>
        private JsonLightDeletedResourceScope CurrentDeletedResourceScope
        {
            get
            {
                JsonLightDeletedResourceScope currentJsonLightDeletedResourceScope = this.CurrentScope as JsonLightDeletedResourceScope;
                Debug.Assert(currentJsonLightDeletedResourceScope != null, "Asking for JsonLightResourceScope when the current scope is not an JsonLightResourceScope.");
                return currentJsonLightDeletedResourceScope;
            }
        }

        /// <summary>
        /// Returns the current JsonLightDeltaLinkScope.
        /// </summary>
        private JsonLightDeltaLinkScope CurrentDeltaLinkScope
        {
            get
            {
                JsonLightDeltaLinkScope jsonLightDeltaLinkScope = this.CurrentScope as JsonLightDeltaLinkScope;
                Debug.Assert(jsonLightDeltaLinkScope != null, "Asking for JsonLightDeltaLinkScope when the current scope is not an JsonLightDeltaLinkScope.");
                return jsonLightDeltaLinkScope;
            }
        }

        /// <summary>
        /// Returns the current JsonLightResourceSetScope.
        /// </summary>
        private JsonLightResourceSetScope CurrentResourceSetScope
        {
            get
            {
                JsonLightResourceSetScope currentJsonLightResourceSetScope = this.CurrentScope as JsonLightResourceSetScope;
                Debug.Assert(currentJsonLightResourceSetScope != null, "Asking for JsonResourceSetScope when the current scope is not a JsonResourceSetScope.");
                return currentJsonLightResourceSetScope;
            }
        }

        /// <summary>
        /// Returns the current JsonLightDeltaResourceSetScope.
        /// </summary>
        private JsonLightDeltaResourceSetScope CurrentDeltaResourceSetScope
        {
            get
            {
                JsonLightDeltaResourceSetScope currentJsonLightDeltaResourceSetScope = this.CurrentScope as JsonLightDeltaResourceSetScope;
                Debug.Assert(currentJsonLightDeltaResourceSetScope != null, "Asking for JsonDeltaResourceSetScope when the current scope is not a JsonDeltaResourceSetScope.");
                return currentJsonLightDeltaResourceSetScope;
            }
        }

        /// <summary>
        /// Check if the object has been disposed; called from all public API methods. Throws an ObjectDisposedException if the object
        /// has already been disposed.
        /// </summary>
        protected override void VerifyNotDisposed()
        {
            this.jsonLightOutputContext.VerifyNotDisposed();
        }

        /// <summary>
        /// Flush the output.
        /// </summary>
        protected override void FlushSynchronously()
        {
            this.jsonLightOutputContext.Flush();
        }

        /// <summary>
        /// Flush the output.
        /// </summary>
        /// <returns>Task representing the pending flush operation.</returns>
        protected override Task FlushAsynchronously()
        {
            return this.jsonLightOutputContext.FlushAsync();
        }

        /// <summary>
        /// Starts writing a payload (called exactly once before anything else)
        /// </summary>
        protected override void StartPayload()
        {
            this.jsonLightResourceSerializer.WritePayloadStart();
        }

        /// <summary>
        /// Ends writing a payload (called exactly once after everything else in case of success)
        /// </summary>
        protected override void EndPayload()
        {
            this.jsonLightResourceSerializer.WritePayloadEnd();
        }

        /// <summary>
        /// Place where derived writers can perform custom steps before the resource is written, at the beginning of WriteStartEntryImplementation.
        /// </summary>
        /// <param name="resourceScope">The ResourceScope.</param>
        /// <param name="resource">Resource to write.</param>
        /// <param name="writingResponse">True if writing response.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        protected override void PrepareResourceForWriteStart(ResourceScope resourceScope, ODataResource resource, bool writingResponse, SelectedPropertiesNode selectedProperties)
        {
            ODataResourceTypeContext typeContext = resourceScope.GetOrCreateTypeContext(writingResponse);
            if (this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel)
            {
                // 1. NoMetadata level: always enable its NullResourceMetadataBuilder
                InnerPrepareResourceForWriteStart(resource, typeContext, selectedProperties);
            }
            else
            {
                // 2. Minimal/Full Metadata level: Use ODataConventionalEntityMetadataBuilder for entity, ODataConventionalResourceMetadataBuilder for other cases.
                if (this.jsonLightOutputContext.Model.IsUserModel() || resourceScope.SerializationInfo != null)
                {
                    InnerPrepareResourceForWriteStart(resource, typeContext, selectedProperties);
                }

                // 3. Here fallback to the default NoOpResourceMetadataBuilder, when model and serializationInfo are both null.
            }
        }

        /// <summary>
        /// Place where derived writers can perform custom steps before the deleted resource is written, at the beginning of WriteStartEntryImplementation.
        /// </summary>
        /// <param name="resourceScope">The ResourceScope.</param>
        /// <param name="deletedResource">Deleted resource to write.</param>
        /// <param name="writingResponse">True if writing response.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        protected override void PrepareDeletedResourceForWriteStart(DeletedResourceScope resourceScope, ODataDeletedResource deletedResource, bool writingResponse, SelectedPropertiesNode selectedProperties)
        {
            if (this.messageWriterSettings.Version > ODataVersion.V4)
            {
                // If:
                //      1. Minimal/Full Metadata level: Use ODataConventionalEntityMetadataBuilder for entity, ODataConventionalResourceMetadataBuilder for other cases.
                //      2. NoMetadata level: always enable its NullResourceMetadataBuilder
                // otherwise:
                //      3. Fallback to the default NoOpResourceMetadataBuilder, when model and serializationInfo are both null.
                if (this.jsonLightOutputContext.Model.IsUserModel() || resourceScope.SerializationInfo != null || this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel)
                {
                    ODataResourceTypeContext typeContext = resourceScope.GetOrCreateTypeContext(writingResponse);
                    InnerPrepareResourceForWriteStart(deletedResource, typeContext, selectedProperties);
                }
            }
            else if (deletedResource.Id == null && this.jsonLightOutputContext.Model.IsUserModel())
            {
                // Create an instance of the conventional UriBuilder and build id from key values
                IEdmEntityType entityType = resourceScope.ResourceType as IEdmEntityType;
                if (deletedResource.SerializationInfo != null)
                {
                    string entityTypeName = deletedResource.SerializationInfo.ExpectedTypeName;
                    if (!String.IsNullOrEmpty(entityTypeName))
                    {
                        entityType = this.jsonLightOutputContext.Model.FindType(entityTypeName) as IEdmEntityType ?? entityType;
                    }
                }

                Debug.Assert(entityType != null, "No entity type specified in resourceScope or serializationInfo.");

                ODataConventionalUriBuilder uriBuilder = new ODataConventionalUriBuilder(
                    new Uri(this.messageWriterSettings.MetadataDocumentUri, "./"),
                    this.jsonLightOutputContext.ODataSimplifiedOptions.EnableWritingKeyAsSegment ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses
                    );

                Uri uri = uriBuilder.BuildBaseUri();
                uri = uriBuilder.BuildEntitySetUri(uri, resourceScope.NavigationSource.Name);
                uri = uriBuilder.BuildEntityInstanceUri(uri, ComputeKeyProperties(deletedResource, entityType, this.jsonLightOutputContext.Model), entityType.FullTypeName());

                deletedResource.Id = uri;
            }
        }

        internal static ICollection<KeyValuePair<string, object>> ComputeKeyProperties(ODataDeletedResource resource, IEdmEntityType entityType, IEdmModel model)
        {
            Debug.Assert(entityType.Key().Any(), "actual entity type has no keys defined");

            ICollection<KeyValuePair<string, object>> computedKeyProperties = new List<KeyValuePair<string, object>>();
            foreach (IEdmStructuralProperty edmKeyProperty in entityType.Key())
            {
                foreach (ODataProperty property in resource.NonComputedProperties)
                {
                    if (property.Name == edmKeyProperty.Name)
                    {
                        object newValue = model.ConvertToUnderlyingTypeIfUIntValue(property.Value);
                        computedKeyProperties.Add(new KeyValuePair<string, object>(edmKeyProperty.Name, newValue));
                        break;
                    }
                }
            }

            return computedKeyProperties;
        }

        /// <summary>
        /// Start writing a property.
        /// </summary>
        /// <param name="property">The property info to write.</param>
        protected override void StartProperty(ODataPropertyInfo property)
        {
            ResourceBaseScope scope = this.ParentScope as ResourceBaseScope;
            Debug.Assert(scope != null, "Writing a property and the parent scope is not a resource");
            ODataResource resource = scope.Item as ODataResource;
            Debug.Assert(resource != null && resource.MetadataBuilder != null, "Writing a property with no parent resource MetadataBuilder");

            ODataProperty propertyWithValue = property as ODataProperty;
            if (propertyWithValue != null)
            {
                this.jsonLightPropertySerializer.WriteProperty(
                propertyWithValue,
                scope.ResourceType,
                false /*isTopLevel*/,
                this.DuplicatePropertyNameChecker,
                resource.MetadataBuilder);
            }
            else
            {
                this.jsonLightPropertySerializer.WritePropertyInfo(
                property,
                scope.ResourceType,
                false /*isTopLevel*/,
                this.DuplicatePropertyNameChecker,
                resource.MetadataBuilder);
            }
        }

        /// <summary>
        /// End writing a property.
        /// </summary>
        /// <param name="property">The property to write.</param>
        protected override void EndProperty(ODataPropertyInfo property)
        {
            // nothing to do
        }

        /// <summary>
        /// Start writing a resource.
        /// </summary>
        /// <param name="resource">The resource to write.</param>
        protected override void StartResource(ODataResource resource)
        {
            ODataNestedResourceInfo parentNavLink = this.ParentNestedResourceInfo;
            if (parentNavLink != null)
            {
                // For a null value, write the type as a property annotation
                if (resource == null)
                {
                    if (parentNavLink.TypeAnnotation != null && parentNavLink.TypeAnnotation.TypeName != null)
                    {
                        this.odataAnnotationWriter.WriteODataTypePropertyAnnotation(parentNavLink.Name, parentNavLink.TypeAnnotation.TypeName);
                    }

                    this.instanceAnnotationWriter.WriteInstanceAnnotations(parentNavLink.GetInstanceAnnotations(), parentNavLink.Name);
                }

                // Write the property name of an expanded navigation property to start the value.
                this.jsonWriter.WriteName(parentNavLink.Name);
            }

            if (resource == null)
            {
                this.jsonWriter.WriteValue((string)null);
                return;
            }

            // Write just the object start, nothing else, since we might not have complete information yet
            this.jsonWriter.StartObjectScope();

            JsonLightResourceScope resourceScope = this.CurrentResourceScope;

            if (this.IsTopLevel && !(this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel))
            {
                ODataContextUrlInfo contextUriInfo = this.jsonLightResourceSerializer.WriteResourceContextUri(
                        resourceScope.GetOrCreateTypeContext(this.writingResponse));

                // Is writing an undeclared resource.
                if (contextUriInfo != null)
                {
                    resourceScope.IsUndeclared = contextUriInfo.IsUndeclared.HasValue && contextUriInfo.IsUndeclared.Value;
                }
            }
            else if (this.ParentScope.State == WriterState.DeltaResourceSet && this.ScopeLevel == 3)
            {
                DeltaResourceSetScope deltaResourceSetScope = this.ParentScope as DeltaResourceSetScope;
                Debug.Assert(deltaResourceSetScope != null, "Writing child of delta set and parent scope is not DeltaResourceSetScope");

                string expectedNavigationSource =
                    deltaResourceSetScope.NavigationSource == null ? null : deltaResourceSetScope.NavigationSource.Name;
                string currentNavigationSource =
                    resource.SerializationInfo != null ? resource.SerializationInfo.NavigationSourceName :
                    resourceScope.NavigationSource == null ? null : resourceScope.NavigationSource.Name;

                if (String.IsNullOrEmpty(currentNavigationSource) || currentNavigationSource != expectedNavigationSource)
                {
                    this.jsonLightResourceSerializer.WriteDeltaContextUri(
                        this.CurrentResourceScope.GetOrCreateTypeContext(true), ODataDeltaKind.Resource,
                        deltaResourceSetScope.ContextUriInfo);
                }
            }

            // Write the metadata
            this.jsonLightResourceSerializer.WriteResourceStartMetadataProperties(resourceScope);
            this.jsonLightResourceSerializer.WriteResourceMetadataProperties(resourceScope);

            this.jsonLightOutputContext.PropertyCacheHandler.SetCurrentResourceScopeLevel(this.ScopeLevel);

            // Write custom instance annotations
            this.instanceAnnotationWriter.WriteInstanceAnnotations(resource.InstanceAnnotations, resourceScope.InstanceAnnotationWriteTracker);

            this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
            if (resource.NonComputedProperties != null)
            {
                this.jsonLightResourceSerializer.WriteProperties(
                    this.ResourceType,
                    resource.NonComputedProperties,
                    false /* isComplexValue */,
                    this.DuplicatePropertyNameChecker,
                    resource.MetadataBuilder);
                this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
            }

            // COMPAT 48: Position of navigation properties/links in JSON differs.
        }

        /// <summary>
        /// Finish writing a resource.
        /// </summary>
        /// <param name="resource">The resource to write.</param>
        protected override void EndResource(ODataResource resource)
        {
            if (resource == null)
            {
                return;
            }

            // Get the projected properties
            JsonLightResourceScope resourceScope = this.CurrentResourceScope;

            this.jsonLightResourceSerializer.WriteResourceMetadataProperties(resourceScope);

            // Write custom instance annotations
            this.instanceAnnotationWriter.WriteInstanceAnnotations(resource.InstanceAnnotations, resourceScope.InstanceAnnotationWriteTracker);

            this.jsonLightResourceSerializer.WriteResourceEndMetadataProperties(resourceScope, resourceScope.DuplicatePropertyNameChecker);

            // Close the object scope
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Finish writing a deleted resource.
        /// </summary>
        /// <param name="deletedResource">The resource to write.</param>
        protected override void EndDeletedResource(ODataDeletedResource deletedResource)
        {
            if (deletedResource == null)
            {
                return;
            }

            // Get the projected properties
            // JsonLightResourceScope resourceScope = this.CurrentResourceScope;

            // this.jsonLightResourceSerializer.WriteResourceMetadataProperties(resourceScope);

            // Write custom instance annotations
            // this.instanceAnnotationWriter.WriteInstanceAnnotations(deletedResource.InstanceAnnotations, resourceScope.InstanceAnnotationWriteTracker);

            // this.jsonLightResourceSerializer.WriteResourceEndMetadataProperties(resourceScope, resourceScope.DuplicatePropertyNameChecker);

            // Close the object scope
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Start writing a resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set to write.</param>
        protected override void StartResourceSet(ODataResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            if (this.ParentNestedResourceInfo == null && (this.writingParameter || this.ParentScope.State == WriterState.ResourceSet))
            {
                // Start array which will hold the entries in the resource set.
                this.jsonWriter.StartArrayScope();
            }
            else if (this.ParentNestedResourceInfo == null)
            {
                // Top-level resource set.
                // "{"
                this.jsonWriter.StartObjectScope();

                // @odata.context
                this.jsonLightResourceSerializer.WriteResourceSetContextUri(this.CurrentResourceSetScope.GetOrCreateTypeContext(this.writingResponse));

                if (this.writingResponse)
                {
                    // write "odata.actions" metadata
                    IEnumerable<ODataAction> actions = resourceSet.Actions;
                    if (actions != null && actions.Any())
                    {
                        this.jsonLightResourceSerializer.WriteOperations(actions.Cast<ODataOperation>(), /*isAction*/ true);
                    }

                    // write "odata.functions" metadata
                    IEnumerable<ODataFunction> functions = resourceSet.Functions;
                    if (functions != null && functions.Any())
                    {
                        this.jsonLightResourceSerializer.WriteOperations(functions.Cast<ODataOperation>(), /*isAction*/ false);
                    }

                    // Write the inline count if it's available.
                    this.WriteResourceSetCount(resourceSet.Count, /*propertyName*/null);

                    // Write the next link if it's available.
                    this.WriteResourceSetNextLink(resourceSet.NextPageLink, /*propertyName*/null);

                    // Write the delta link if it's available.
                    this.WriteResourceSetDeltaLink(resourceSet.DeltaLink);
                }

                // Write custom instance annotations
                this.instanceAnnotationWriter.WriteInstanceAnnotations(resourceSet.InstanceAnnotations, this.CurrentResourceSetScope.InstanceAnnotationWriteTracker);

                // "value":
                this.jsonWriter.WriteValuePropertyName();

                // Start array which will hold the entries in the resource set.
                this.jsonWriter.StartArrayScope();
            }
            else
            {
                // Expanded resource set.
                Debug.Assert(
                    this.ParentNestedResourceInfo != null && (!this.ParentNestedResourceInfo.IsCollection.HasValue || this.ParentNestedResourceInfo.IsCollection.Value),
                    "We should have verified that resource sets can only be written into IsCollection = true links in requests.");

                this.ValidateNoDeltaLinkForExpandedResourceSet(resourceSet);
                this.ValidateNoCustomInstanceAnnotationsForExpandedResourceSet(resourceSet);

                string propertyName = this.ParentNestedResourceInfo.Name;
                bool isUndeclared = (this.CurrentScope as JsonLightResourceSetScope).IsUndeclared;
                string expectedResourceTypeName =
                    this.CurrentResourceSetScope.GetOrCreateTypeContext(this.writingResponse)
                    .ExpectedResourceTypeName;

                if (this.writingResponse)
                {
                    // Write the inline count if it's available.
                    this.WriteResourceSetCount(resourceSet.Count, propertyName);

                    // Write the next link if it's available.
                    this.WriteResourceSetNextLink(resourceSet.NextPageLink, propertyName);

                    // Write the odata type.
                    this.jsonLightResourceSerializer.WriteResourceSetStartMetadataProperties(resourceSet, propertyName, expectedResourceTypeName, isUndeclared);

                    // And then write the property name to start the value.
                    this.jsonWriter.WriteName(propertyName);

                    // Start array which will hold the entries in the resource set.
                    this.jsonWriter.StartArrayScope();
                }
                else
                {
                    JsonLightNestedResourceInfoScope navigationLinkScope = (JsonLightNestedResourceInfoScope)this.ParentNestedResourceInfoScope;
                    if (!navigationLinkScope.ResourceSetWritten)
                    {
                        // Close the entity reference link array (if written)
                        if (navigationLinkScope.EntityReferenceLinkWritten)
                        {
                            this.jsonWriter.EndArrayScope();
                        }

                        // Write the odata type.
                        this.jsonLightResourceSerializer.WriteResourceSetStartMetadataProperties(resourceSet, propertyName, expectedResourceTypeName, isUndeclared);

                        // And then write the property name to start the value.
                        this.jsonWriter.WriteName(propertyName);

                        // Start array which will hold the entries in the resource set.
                        this.jsonWriter.StartArrayScope();

                        navigationLinkScope.ResourceSetWritten = true;
                    }
                }
            }

            this.jsonLightOutputContext.PropertyCacheHandler.EnterResourceSetScope(this.CurrentResourceSetScope.ResourceType, this.ScopeLevel);
        }

        /// <summary>
        /// Finish writing a resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set to write.</param>
        protected override void EndResourceSet(ODataResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            if (this.ParentNestedResourceInfo == null && (this.writingParameter || this.ParentScope.State == WriterState.ResourceSet))
            {
                // End the array which holds the entries in the resource set.
                this.jsonWriter.EndArrayScope();
            }
            else if (this.ParentNestedResourceInfo == null)
            {
                // End the array which holds the entries in the resource set.
                this.jsonWriter.EndArrayScope();

                // Write custom instance annotations
                this.instanceAnnotationWriter.WriteInstanceAnnotations(resourceSet.InstanceAnnotations, this.CurrentResourceSetScope.InstanceAnnotationWriteTracker);

                if (this.writingResponse)
                {
                    // Write the next link if it's available.
                    this.WriteResourceSetNextLink(resourceSet.NextPageLink, /*propertyName*/null);

                    // Write the delta link if it's available.
                    this.WriteResourceSetDeltaLink(resourceSet.DeltaLink);
                }

                // Close the object wrapper.
                this.jsonWriter.EndObjectScope();
            }
            else
            {
                Debug.Assert(
                    this.ParentNestedResourceInfo != null && (!this.ParentNestedResourceInfo.IsCollection.HasValue || this.ParentNestedResourceInfo.IsCollection.Value),
                    "We should have verified that resource sets can only be written into IsCollection = true links in requests.");
                string propertyName = this.ParentNestedResourceInfo.Name;

                this.ValidateNoDeltaLinkForExpandedResourceSet(resourceSet);
                this.ValidateNoCustomInstanceAnnotationsForExpandedResourceSet(resourceSet);

                if (this.writingResponse)
                {
                    // End the array which holds the entries in the resource set.
                    // NOTE: in requests we will only write the EndArray of a resource set
                    //       when we hit the nested resource info end since a nested resource info
                    //       can contain multiple resource sets that get collapsed into a single array value.
                    this.jsonWriter.EndArrayScope();

                    // Write the next link if it's available.
                    this.WriteResourceSetNextLink(resourceSet.NextPageLink, propertyName);
                }
            }

            this.jsonLightOutputContext.PropertyCacheHandler.LeaveResourceSetScope();
        }

        /// <summary>
        /// Start writing a delta resource set.
        /// </summary>
        /// <param name="deltaResourceSet">The delta resource set to write.</param>
        protected override void StartDeltaResourceSet(ODataDeltaResourceSet deltaResourceSet)
        {
            Debug.Assert(deltaResourceSet != null, "resourceSet != null");

            if (this.ParentNestedResourceInfo == null)
            {
                this.jsonWriter.StartObjectScope();

                // Write ContextUrl
                this.CurrentDeltaResourceSetScope.ContextUriInfo = this.jsonLightResourceSerializer.WriteDeltaContextUri(
                    this.CurrentDeltaResourceSetScope.GetOrCreateTypeContext(this.writingResponse),
                    ODataDeltaKind.ResourceSet);

                // Write Count, if available
                this.WriteResourceSetCount(deltaResourceSet.Count, /*propertyname*/ null);

                // Write NextLink, if available
                this.WriteResourceSetNextLink(deltaResourceSet.NextPageLink, /*propertyname*/ null);

                // If we haven't written the delta link yet and it's available, write it.
                this.WriteResourceSetDeltaLink(deltaResourceSet.DeltaLink);

                // Write annotations
                this.instanceAnnotationWriter.WriteInstanceAnnotations(deltaResourceSet.InstanceAnnotations, this.CurrentDeltaResourceSetScope.InstanceAnnotationWriteTracker);

                // Write Value Start
                this.jsonWriter.WriteValuePropertyName();
                this.jsonWriter.StartArrayScope();
            }
            else
            {
                // Nested Delta
                Debug.Assert(
                    this.ParentNestedResourceInfo != null && this.ParentNestedResourceInfo.IsCollection.HasValue && this.ParentNestedResourceInfo.IsCollection.Value,
                    "We should have verified that resource sets can only be written into IsCollection = true links in requests.");

                string propertyName = this.ParentNestedResourceInfo.Name;

                // Write the inline count if it's available.
                this.WriteResourceSetCount(deltaResourceSet.Count, propertyName);

                // Write the next link if it's available.
                this.WriteResourceSetNextLink(deltaResourceSet.NextPageLink, propertyName);

                //// Write the name for the nested delta payload
                this.jsonWriter.WritePropertyAnnotationName(propertyName, JsonLightConstants.ODataDeltaPropertyName);

                // Start array which will hold the entries in the nested delta resource set.
                this.jsonWriter.StartArrayScope();
            }
        }

        /// <summary>
        /// Finish writing a delta resource set.
        /// </summary>
        /// <param name="deltaResourceSet">The resource set to write.</param>
        protected override void EndDeltaResourceSet(ODataDeltaResourceSet deltaResourceSet)
        {
            Debug.Assert(deltaResourceSet != null, "deltaResourceSet != null");

            if (this.ParentNestedResourceInfo == null)
            {
                // End the array which holds the entries in the resource set.
                this.jsonWriter.EndArrayScope();

                // Write custom instance annotations
                this.instanceAnnotationWriter.WriteInstanceAnnotations(deltaResourceSet.InstanceAnnotations, this.CurrentDeltaResourceSetScope.InstanceAnnotationWriteTracker);

                // Write the next link if it's available.
                this.WriteResourceSetNextLink(deltaResourceSet.NextPageLink, /*propertynamne*/ null);

                // Write the delta link if it's available.
                this.WriteResourceSetDeltaLink(deltaResourceSet.DeltaLink);

                // Close the object wrapper.
                this.jsonWriter.EndObjectScope();
            }
            else
            {
                // End the array which holds the entries in the resource set.
                this.jsonWriter.EndArrayScope();
            }
        }

        /// <summary>
        /// Start writing a delta deleted resource.
        /// </summary>
        /// <param name="resource">The resource to write.</param>
        protected override void StartDeletedResource(ODataDeletedResource resource)
        {
            Debug.Assert(resource != null, "resource != null");
            Debug.Assert(!this.IsTopLevel, "Delta resource cannot be on top level.");
            DeletedResourceScope resourceScope = this.CurrentDeletedResourceScope;
            Debug.Assert(resourceScope != null, "Writing deleted entry and scope is not DeltaResourceScope");
            ODataNestedResourceInfo parentNavLink = this.ParentNestedResourceInfo;
            if (parentNavLink != null)
            {
                // Writing a nested deleted resource
                if (this.Version == null || this.Version < ODataVersion.V401)
                {
                    throw new ODataException(Strings.ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry);
                }
                else
                {
                    // Write the property name of an expanded navigation property to start the value.
                    this.jsonWriter.WriteName(parentNavLink.Name);
                    this.jsonWriter.StartObjectScope();
                    this.WriteDeletedEntryContents(resource);
                }
            }
            else
            {
                // Writing a deleted resource within an entity set
                DeltaResourceSetScope deltaResourceSetScope = this.ParentScope as DeltaResourceSetScope;
                Debug.Assert(deltaResourceSetScope != null, "Writing child of delta set and parent scope is not DeltaResourceSetScope");

                this.jsonWriter.StartObjectScope();

                if (this.Version == null || this.Version < ODataVersion.V401)
                {
                    // Write ContextUrl
                    this.jsonLightResourceSerializer.WriteDeltaContextUri(
                            this.CurrentDeletedResourceScope.GetOrCreateTypeContext(this.writingResponse),
                            ODataDeltaKind.DeletedEntry, deltaResourceSetScope.ContextUriInfo);
                    this.WriteV4DeletedEntryContents(resource);
                }
                else
                {
                    // Only write ContextUrl for V4.01 deleted resource if it is in a top level delta
                    // resource set and comes from a different entity set
                    string expectedNavigationSource =
                        deltaResourceSetScope.NavigationSource == null ? null : deltaResourceSetScope.NavigationSource.Name;
                    string currentNavigationSource =
                        resource.SerializationInfo != null ? resource.SerializationInfo.NavigationSourceName :
                        resourceScope.NavigationSource == null ? null : resourceScope.NavigationSource.Name;

                    if (String.IsNullOrEmpty(currentNavigationSource) || currentNavigationSource != expectedNavigationSource)
                    {
                        Debug.Assert(this.ScopeLevel == 3, "Writing a nested deleted resource of the wrong type should already have been caught.");

                        // We are writing a deleted resource in a top level delta resource set
                        // from a different entity set, so include the context Url
                        this.jsonLightResourceSerializer.WriteDeltaContextUri(
                                this.CurrentDeletedResourceScope.GetOrCreateTypeContext(this.writingResponse),
                                ODataDeltaKind.DeletedEntry, deltaResourceSetScope.ContextUriInfo);
                    }

                    this.WriteDeletedEntryContents(resource);
                }
            }
        }

        /// <summary>
        /// Start writing a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        protected override void StartDeltaLink(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");

            this.jsonWriter.StartObjectScope();

            if (link is ODataDeltaLink)
            {
                this.WriteDeltaLinkContextUri(ODataDeltaKind.Link);
            }
            else
            {
                Debug.Assert(link is ODataDeltaDeletedLink, "link must be either DeltaLink or DeltaDeletedLink.");
                this.WriteDeltaLinkContextUri(ODataDeltaKind.DeletedLink);
            }

            this.WriteDeltaLinkSource(link);
            this.WriteDeltaLinkRelationship(link);
            this.WriteDeltaLinkTarget(link);
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Write a primitive type inside an untyped collection.
        /// </summary>
        /// <param name="primitiveValue">The primitive value to write.</param>
        protected override void WritePrimitiveValue(ODataPrimitiveValue primitiveValue)
        {
            ODataPropertyInfo property;
            if (this.ParentScope != null && (property = this.ParentScope.Item as ODataPropertyInfo) != null)
            {
                this.jsonWriter.WriteName(property.Name);
            }

            if (primitiveValue == null)
            {
                this.jsonLightValueSerializer.WriteNullValue();
            }
            else
            {
                this.jsonLightValueSerializer.WritePrimitiveValue(primitiveValue.Value, /*expectedType*/ null);
            }
        }

        /// <summary>
        /// Create a stream for writing a binary value.
        /// </summary>
        /// <returns>Stream for writing a binary value.</returns>
        protected override Stream StartBinaryStream()
        {
            ODataPropertyInfo property;
            if (this.ParentScope != null && (property = this.ParentScope.Item as ODataPropertyInfo) != null)
            {
                // writing a stream property - write the property name
                this.jsonWriter.WriteName(property.Name);
                this.jsonWriter.Flush();
            }

            Stream stream;
            if (this.jsonStreamWriter == null)
            {
                this.jsonLightOutputContext.BinaryValueStream = new MemoryStream();
                stream = this.jsonLightOutputContext.BinaryValueStream;
            }
            else
            {
                stream = this.jsonStreamWriter.StartStreamValueScope();
            }

            return stream;
        }

        /// <summary>
        /// Finish writing a stream value.
        /// </summary>
        protected sealed override void EndBinaryStream()
        {
            if (this.jsonStreamWriter == null)
            {
                this.jsonWriter.WriteValue(this.jsonLightOutputContext.BinaryValueStream.ToArray());
                this.jsonLightOutputContext.BinaryValueStream.Flush();
                this.jsonLightOutputContext.BinaryValueStream.Dispose();
                this.jsonLightOutputContext.BinaryValueStream = null;
            }
            else
            {
                this.jsonStreamWriter.EndStreamValueScope();
            }
        }

        /// <summary>
        /// Create a TextWriter for writing a string value.
        /// </summary>
        /// <returns>TextWriter for writing a string value.</returns>
        protected override TextWriter StartTextWriter()
        {
            ODataPropertyInfo property = null;
            if (this.ParentScope != null && (property = this.ParentScope.Item as ODataPropertyInfo) != null)
            {
                // writing a text property - write the property name
                this.jsonWriter.WriteName(property.Name);
                this.jsonWriter.Flush();
            }

            TextWriter writer;
            if (this.jsonStreamWriter == null)
            {
                this.jsonLightOutputContext.StringWriter = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
                writer = this.jsonLightOutputContext.StringWriter;
            }
            else
            {
                string contentType = "text/plain";
                ODataStreamPropertyInfo streamInfo = property as ODataStreamPropertyInfo;
                if (streamInfo != null && streamInfo.ContentType != null)
                {
                    contentType = streamInfo.ContentType;
                }

                writer = this.jsonStreamWriter.StartTextWriterValueScope(contentType);
            }

            return writer;
        }

        /// <summary>
        /// Finish writing a text value.
        /// </summary>
        protected sealed override void EndTextWriter()
        {
            if (this.jsonStreamWriter == null)
            {
                Debug.Assert(this.jsonLightOutputContext.StringWriter != null, "Calling EndTextWriter with a non-streaming JsonWriter and a null StringWriter");
                this.jsonLightOutputContext.StringWriter.Flush();
                this.jsonWriter.WriteValue(this.jsonLightOutputContext.StringWriter.GetStringBuilder().ToString());
                this.jsonLightOutputContext.StringWriter.Dispose();
                this.jsonLightOutputContext.StringWriter = null;
            }
            else
            {
                this.jsonStreamWriter.EndTextWriterValueScope();
            }
        }

        /// <summary>
        /// Start writing a deferred (non-expanded) nested resource info.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        protected override void WriteDeferredNestedResourceInfo(ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(this.writingResponse, "Deferred links are only supported in response, we should have verified this already.");

            // A deferred nested resource info is just the link metadata, no value.
            this.jsonLightResourceSerializer.WriteNavigationLinkMetadata(nestedResourceInfo, this.DuplicatePropertyNameChecker);
        }

        /// <summary>
        /// Start writing the nested resource info with content.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        protected override void StartNestedResourceInfoWithContent(ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(!string.IsNullOrEmpty(nestedResourceInfo.Name), "The nested resource info name should have been verified by now.");

            if (this.writingResponse)
            {
                // Write @odata.context annotation for navigation property
                IEdmContainedEntitySet containedEntitySet = this.CurrentScope.NavigationSource as IEdmContainedEntitySet;
                if (containedEntitySet != null
                    && this.messageWriterSettings.LibraryCompatibility < ODataLibraryCompatibility.Version7
                    && this.messageWriterSettings.Version < ODataVersion.V401)
                {
                    ODataContextUrlInfo info = ODataContextUrlInfo.Create(
                                                this.CurrentScope.NavigationSource,
                                                this.CurrentScope.ResourceType.FullTypeName(),
                                                containedEntitySet.NavigationProperty.Type.TypeKind() != EdmTypeKind.Collection,
                                                this.CurrentScope.ODataUri,
                                                this.messageWriterSettings.Version ?? ODataVersion.V4);

                    this.jsonLightResourceSerializer.WriteNestedResourceInfoContextUrl(nestedResourceInfo, info);
                }

                // Write the nested resource info metadata first. The rest is written by the content resource or resource set.
                this.jsonLightResourceSerializer.WriteNavigationLinkMetadata(nestedResourceInfo, this.DuplicatePropertyNameChecker);
            }
            else
            {
                this.WriterValidator.ValidateNestedResourceInfoHasCardinality(nestedResourceInfo);
            }
        }

        /// <summary>
        /// Finish writing nested resource info with content.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        protected override void EndNestedResourceInfoWithContent(ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");

            JsonLightNestedResourceInfoScope navigationLinkScope = (JsonLightNestedResourceInfoScope)this.CurrentScope;

            // If we wrote entity reference links for a collection navigation property but no
            // resource set afterwards, we have to now close the array of links.
            if (!this.writingResponse)
            {
                if (navigationLinkScope.EntityReferenceLinkWritten && !navigationLinkScope.ResourceSetWritten && nestedResourceInfo.IsCollection.Value)
                {
                    this.jsonWriter.EndArrayScope();
                }

                // In requests, the nested resource info may have multiple entries in multiple resource sets in it; if we
                // wrote at least one resource set, close the resulting array here.
                if (navigationLinkScope.ResourceSetWritten)
                {
                    Debug.Assert(nestedResourceInfo.IsCollection == null || nestedResourceInfo.IsCollection.Value, "nestedResourceInfo.IsCollection.Value");
                    this.jsonWriter.EndArrayScope();
                }
            }
        }

        /// <summary>
        /// Write an entity reference link.
        /// </summary>
        /// <param name="parentNestedResourceInfo">The parent navigation link which is being written around the entity reference link.</param>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        protected override void WriteEntityReferenceInNavigationLinkContent(ODataNestedResourceInfo parentNestedResourceInfo, ODataEntityReferenceLink entityReferenceLink)
        {
            Debug.Assert(parentNestedResourceInfo != null, "parentNestedResourceInfo != null");
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            // In JSON Light, we can only write entity reference links at the beginning of a navigation link in requests;
            // once we wrote a resource set, entity reference links are not allowed anymore (we require all the entity reference
            // link to come first because of the grouping in the JSON Light wire format).
            JsonLightNestedResourceInfoScope nestedResourceScope = this.CurrentScope as JsonLightNestedResourceInfoScope;
            if (nestedResourceScope == null)
            {
                nestedResourceScope = this.ParentNestedResourceInfoScope as JsonLightNestedResourceInfoScope;
            }

            if (nestedResourceScope.ResourceSetWritten)
            {
                throw new ODataException(Strings.ODataJsonLightWriter_EntityReferenceLinkAfterResourceSetInRequest);
            }

            if (!nestedResourceScope.EntityReferenceLinkWritten)
            {
                // In request
                if (!this.writingResponse)
                {
                    if (this.Version == null || this.Version < ODataVersion.V401)
                    {
                        // Write the property annotation for the entity reference link(s)
                        this.odataAnnotationWriter.WritePropertyAnnotationName(parentNestedResourceInfo.Name, ODataAnnotationNames.ODataBind);
                    }
                    else
                    {
                        this.jsonWriter.WriteName(parentNestedResourceInfo.Name);
                    }

                    Debug.Assert(parentNestedResourceInfo.IsCollection.HasValue, "parentNestedResourceInfo.IsCollection.HasValue");
                    if (parentNestedResourceInfo.IsCollection.Value)
                    {
                        // write [ for the collection
                        this.jsonWriter.StartArrayScope();
                    }
                }
                else
                {
                    // In response
                    Debug.Assert(parentNestedResourceInfo.IsCollection.HasValue, "parentNestedResourceInfo.IsCollection.HasValue");
                    if (!parentNestedResourceInfo.IsCollection.Value)
                    {
                        // Write the property name for single-nested resource,
                        // for the collection nested resource, it's write at top level when writing ODataResourceSet
                        this.jsonWriter.WriteName(parentNestedResourceInfo.Name);
                    }
                }

                nestedResourceScope.EntityReferenceLinkWritten = true;
            }

            if (!this.writingResponse &&
                (this.Version == null || this.Version < ODataVersion.V401))
            {
                Debug.Assert(entityReferenceLink.Url != null, "The entity reference link Url should have been validated by now.");
                this.jsonWriter.WriteValue(this.jsonLightResourceSerializer.UriToString(entityReferenceLink.Url));
            }
            else
            {
                WriteEntityReferenceLinkImplementation(entityReferenceLink);
            }
        }

        /// <summary>
        /// Create a new resource set scope.
        /// </summary>
        /// <param name="resourceSet">The resource set for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write resources for.</param>
        /// <param name="itemType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <param name="isUndeclared">true if the resource set is for an undeclared property</param>
        /// <returns>The newly create scope.</returns>
        protected override ResourceSetScope CreateResourceSetScope(
            ODataResourceSet resourceSet,
            IEdmNavigationSource navigationSource,
            IEdmType itemType,
            bool skipWriting,
            SelectedPropertiesNode selectedProperties,
            ODataUri odataUri,
            bool isUndeclared)
        {
            return new JsonLightResourceSetScope(resourceSet, navigationSource, itemType, skipWriting, selectedProperties, odataUri, isUndeclared);
        }

        /// <summary>
        /// Create a new delta resource set scope.
        /// </summary>
        /// <param name="deltaResourceSet">The delta resource set for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
        /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <param name="isUndeclared">true if the resource set is for an undeclared property</param>
        /// <returns>The newly create scope.</returns>
        protected override DeltaResourceSetScope CreateDeltaResourceSetScope(
            ODataDeltaResourceSet deltaResourceSet,
            IEdmNavigationSource navigationSource,
            IEdmStructuredType resourceType,
            bool skipWriting,
            SelectedPropertiesNode selectedProperties,
            ODataUri odataUri,
            bool isUndeclared)
        {
            return new JsonLightDeltaResourceSetScope(deltaResourceSet, navigationSource, resourceType, selectedProperties, odataUri);
        }

        /// <summary>
        /// Create a new resource scope.
        /// </summary>
        /// <param name="deltaResource">The resource for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write resources for.</param>
        /// <param name="resourceType">The entity type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <param name="isUndeclared">true if the resource is for an undeclared property</param>
        /// <returns>The newly create scope.</returns>
        protected override DeletedResourceScope CreateDeletedResourceScope(ODataDeletedResource deltaResource, IEdmNavigationSource navigationSource, IEdmEntityType resourceType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri, bool isUndeclared)
        {
            return new JsonLightDeletedResourceScope(
                deltaResource,
                this.GetResourceSerializationInfo(deltaResource),
                navigationSource,
                resourceType,
                skipWriting,
                this.messageWriterSettings,
                selectedProperties,
                odataUri,
                isUndeclared);
        }

        /// <summary>
        /// Create a new property scope.
        /// </summary>
        /// <param name="property">The property for the new scope.</param>
        /// <param name="navigationSource">The navigation source.</param>
        /// <param name="resourceType">The structured type for the resource containing the property to be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <returns>The newly created property scope.</returns>
        protected override PropertyInfoScope CreatePropertyInfoScope(ODataPropertyInfo property, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
        {
            return new JsonLightPropertyScope(property, navigationSource, resourceType, selectedProperties, odataUri);
        }

        /// <summary>
        /// Create a new delta link scope.
        /// </summary>
        /// <param name="link">The link for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <returns>The newly create scope.</returns>
        protected override DeltaLinkScope CreateDeltaLinkScope(ODataDeltaLinkBase link, IEdmNavigationSource navigationSource, IEdmEntityType entityType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
        {
            return new JsonLightDeltaLinkScope(
                link is ODataDeltaLink ? WriterState.DeltaLink : WriterState.DeltaDeletedLink,
                link,
                this.GetLinkSerializationInfo(link),
                navigationSource,
                entityType,
                selectedProperties,
                odataUri);
        }

        /// <summary>
        /// Create a new resource scope.
        /// </summary>
        /// <param name="resource">The resource for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write resources for.</param>
        /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <param name="isUndeclared">true if the resource is for an undeclared property</param>
        /// <returns>The newly create scope.</returns>
        protected override ResourceScope CreateResourceScope(ODataResource resource, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri, bool isUndeclared)
        {
            return new JsonLightResourceScope(
                resource,
                this.GetResourceSerializationInfo(resource),
                navigationSource,
                resourceType,
                skipWriting,
                this.messageWriterSettings,
                selectedProperties,
                odataUri,
                isUndeclared);
        }

        /// <summary>
        /// Creates a new JSON Light nested resource info scope.
        /// </summary>
        /// <param name="writerState">The writer state for the new scope.</param>
        /// <param name="navLink">The nested resource info for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="itemType">The type for the items in the resource set to be written (or null if the navigationSource base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <returns>The newly created JSON Light  nested resource info scope.</returns>
        protected override NestedResourceInfoScope CreateNestedResourceInfoScope(WriterState writerState, ODataNestedResourceInfo navLink, IEdmNavigationSource navigationSource, IEdmType itemType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
        {
            return new JsonLightNestedResourceInfoScope(writerState, navLink, navigationSource, itemType, skipWriting, selectedProperties, odataUri);
        }

        /// <summary>
        /// Asynchronously starts writing a payload (called exactly once before anything else)
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task StartPayloadAsync()
        {
            return this.jsonLightResourceSerializer.WritePayloadStartAsync();
        }

        /// <summary>
        /// Asynchronously ends writing a payload (called exactly once after everything else in case of success)
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task EndPayloadAsync()
        {
            return this.jsonLightResourceSerializer.WritePayloadEndAsync();
        }

        /// <summary>
        /// Asynchronously start writing a resource.
        /// </summary>
        /// <param name="resource">The resource to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task StartResourceAsync(ODataResource resource)
        {
            ODataNestedResourceInfo parentNavLink = this.ParentNestedResourceInfo;
            if (parentNavLink != null)
            {
                // For a null value, write the type as a property annotation
                if (resource == null)
                {
                    if (parentNavLink.TypeAnnotation?.TypeName != null)
                    {
                        await this.asynchronousODataAnnotationWriter.WriteODataTypePropertyAnnotationAsync(
                            parentNavLink.Name,
                            parentNavLink.TypeAnnotation.TypeName).ConfigureAwait(false);
                    }

                    await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                        parentNavLink.GetInstanceAnnotations(),
                        parentNavLink.Name).ConfigureAwait(false);
                }

                // Write the property name of an expanded navigation property to start the value.
                await this.asynchronousJsonWriter.WriteNameAsync(parentNavLink.Name)
                    .ConfigureAwait(false);
            }

            if (resource == null)
            {
                await this.asynchronousJsonWriter.WriteValueAsync((string)null)
                    .ConfigureAwait(false);
                return;
            }

            // Write just the object start, nothing else, since we might not have complete information yet
            await this.asynchronousJsonWriter.StartObjectScopeAsync()
                .ConfigureAwait(false);

            JsonLightResourceScope resourceScope = this.CurrentResourceScope;

            if (this.IsTopLevel && !(this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel))
            {
                ODataContextUrlInfo contextUriInfo = await this.jsonLightResourceSerializer.WriteResourceContextUriAsync(
                        resourceScope.GetOrCreateTypeContext(
                            this.writingResponse)).ConfigureAwait(false);

                // Is writing an undeclared resource.
                if (contextUriInfo != null)
                {
                    resourceScope.IsUndeclared = contextUriInfo.IsUndeclared.HasValue && contextUriInfo.IsUndeclared.Value;
                }
            }
            else if (this.ParentScope.State == WriterState.DeltaResourceSet && this.ScopeLevel == 3)
            {
                DeltaResourceSetScope deltaResourceSetScope = this.ParentScope as DeltaResourceSetScope;
                Debug.Assert(deltaResourceSetScope != null, "Writing child of delta set and parent scope is not DeltaResourceSetScope");

                string expectedNavigationSource = deltaResourceSetScope.NavigationSource?.Name;
                string currentNavigationSource = resource.SerializationInfo?.NavigationSourceName ?? resourceScope.NavigationSource?.Name;

                if (String.IsNullOrEmpty(currentNavigationSource) || currentNavigationSource != expectedNavigationSource)
                {
                    await this.jsonLightResourceSerializer.WriteDeltaContextUriAsync(
                        this.CurrentResourceScope.GetOrCreateTypeContext(true),
                        ODataDeltaKind.Resource,
                        deltaResourceSetScope.ContextUriInfo).ConfigureAwait(false);
                }
            }

            // Write the metadata
            await this.jsonLightResourceSerializer.WriteResourceStartMetadataPropertiesAsync(resourceScope)
                .ConfigureAwait(false);
            await this.jsonLightResourceSerializer.WriteResourceMetadataPropertiesAsync(resourceScope)
                .ConfigureAwait(false);

            this.jsonLightOutputContext.PropertyCacheHandler.SetCurrentResourceScopeLevel(this.ScopeLevel);

            // Write custom instance annotations
            await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                resource.InstanceAnnotations,
                resourceScope.InstanceAnnotationWriteTracker).ConfigureAwait(false);

            this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
            if (resource.NonComputedProperties != null)
            {
                await this.jsonLightResourceSerializer.WritePropertiesAsync(
                    this.ResourceType,
                    resource.NonComputedProperties,
                    false /* isComplexValue */,
                    this.DuplicatePropertyNameChecker,
                    resource.MetadataBuilder).ConfigureAwait(false);
                this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
            }
        }

        /// <summary>
        /// Asynchronously finish writing a resource.
        /// </summary>
        /// <param name="resource">The resource to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task EndResourceAsync(ODataResource resource)
        {
            if (resource == null)
            {
                return;
            }

            // Get the projected properties
            JsonLightResourceScope resourceScope = this.CurrentResourceScope;

            await this.jsonLightResourceSerializer.WriteResourceMetadataPropertiesAsync(resourceScope)
                .ConfigureAwait(false);

            // Write custom instance annotations
            await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                resource.InstanceAnnotations,
                resourceScope.InstanceAnnotationWriteTracker).ConfigureAwait(false);

            await this.jsonLightResourceSerializer.WriteResourceEndMetadataPropertiesAsync(
                resourceScope,
                resourceScope.DuplicatePropertyNameChecker).ConfigureAwait(false);

            // Close the object scope
            await this.asynchronousJsonWriter.EndObjectScopeAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously start writing a property.
        /// </summary>
        /// <param name="property">The property info to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task StartPropertyAsync(ODataPropertyInfo property)
        {
            ResourceBaseScope scope = this.ParentScope as ResourceBaseScope;
            Debug.Assert(scope != null, "Writing a property and the parent scope is not a resource");
            ODataResource resource = scope.Item as ODataResource;
            Debug.Assert(resource != null && resource.MetadataBuilder != null, "Writing a property with no parent resource MetadataBuilder");

            ODataProperty propertyWithValue = property as ODataProperty;
            if (propertyWithValue != null)
            {
                return this.jsonLightPropertySerializer.WritePropertyAsync(
                    propertyWithValue,
                    scope.ResourceType,
                    false /*isTopLevel*/,
                    this.DuplicatePropertyNameChecker,
                    resource.MetadataBuilder);
            }
            else
            {
                return this.jsonLightPropertySerializer.WritePropertyInfoAsync(
                    property,
                    scope.ResourceType,
                    false /*isTopLevel*/,
                    this.DuplicatePropertyNameChecker,
                    resource.MetadataBuilder);
            }
        }

        /// <summary>
        /// Asynchronously end writing a property.
        /// </summary>
        /// <param name="property">The property to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task EndPropertyAsync(ODataPropertyInfo property)
        {
            return TaskUtils.CompletedTask;
        }

        /// <summary>
        /// Asynchronously start writing a resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task StartResourceSetAsync(ODataResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            if (this.ParentNestedResourceInfo == null && (this.writingParameter || this.ParentScope.State == WriterState.ResourceSet))
            {
                // Start array which will hold the entries in the resource set.
                await this.asynchronousJsonWriter.StartArrayScopeAsync()
                    .ConfigureAwait(false);
            }
            else if (this.ParentNestedResourceInfo == null)
            {
                // Top-level resource set.
                // "{"
                await this.asynchronousJsonWriter.StartObjectScopeAsync()
                    .ConfigureAwait(false);

                // @odata.context
                await this.jsonLightResourceSerializer.WriteResourceSetContextUriAsync(
                    this.CurrentResourceSetScope.GetOrCreateTypeContext(
                        this.writingResponse)).ConfigureAwait(false);

                if (this.writingResponse)
                {
                    // Write "odata.actions" metadata
                    IEnumerable<ODataAction> actions = resourceSet.Actions;
                    if (actions != null && actions.Any())
                    {
                        await this.jsonLightResourceSerializer.WriteOperationsAsync(actions.Cast<ODataOperation>(), /*isAction*/ true)
                            .ConfigureAwait(false);
                    }

                    // Write "odata.functions" metadata
                    IEnumerable<ODataFunction> functions = resourceSet.Functions;
                    if (functions != null && functions.Any())
                    {
                        await this.jsonLightResourceSerializer.WriteOperationsAsync(functions.Cast<ODataOperation>(), /*isAction*/ false)
                            .ConfigureAwait(false);
                    }

                    // Write the inline count if it's available.
                    await this.WriteResourceSetCountAsync(resourceSet.Count, /*propertyName*/null)
                        .ConfigureAwait(false);

                    // Write the next link if it's available.
                    await this.WriteResourceSetNextLinkAsync(resourceSet.NextPageLink, /*propertyName*/null)
                        .ConfigureAwait(false);

                    // Write the delta link if it's available.
                    await this.WriteResourceSetDeltaLinkAsync(resourceSet.DeltaLink)
                        .ConfigureAwait(false);
                }

                // Write custom instance annotations
                await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                    resourceSet.InstanceAnnotations,
                    this.CurrentResourceSetScope.InstanceAnnotationWriteTracker).ConfigureAwait(false);

                // "value":
                await this.asynchronousJsonWriter.WriteValuePropertyNameAsync()
                    .ConfigureAwait(false);

                // Start array which will hold the entries in the resource set.
                await this.asynchronousJsonWriter.StartArrayScopeAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                // Expanded resource set.
                Debug.Assert(
                    this.ParentNestedResourceInfo != null && (!this.ParentNestedResourceInfo.IsCollection.HasValue || this.ParentNestedResourceInfo.IsCollection.Value),
                    "We should have verified that resource sets can only be written into IsCollection = true links in requests.");

                this.ValidateNoDeltaLinkForExpandedResourceSet(resourceSet);
                this.ValidateNoCustomInstanceAnnotationsForExpandedResourceSet(resourceSet);

                string propertyName = this.ParentNestedResourceInfo.Name;
                bool isUndeclared = (this.CurrentScope as JsonLightResourceSetScope).IsUndeclared;
                string expectedResourceTypeName = this.CurrentResourceSetScope.GetOrCreateTypeContext(
                    this.writingResponse).ExpectedResourceTypeName;

                if (this.writingResponse)
                {
                    // Write the inline count if it's available.
                    await this.WriteResourceSetCountAsync(resourceSet.Count, propertyName)
                        .ConfigureAwait(false);

                    // Write the next link if it's available.
                    await this.WriteResourceSetNextLinkAsync(resourceSet.NextPageLink, propertyName)
                        .ConfigureAwait(false);

                    // Write the odata type.
                    await this.jsonLightResourceSerializer.WriteResourceSetStartMetadataPropertiesAsync(
                        resourceSet,
                        propertyName,
                        expectedResourceTypeName,
                        isUndeclared).ConfigureAwait(false);

                    // And then write the property name to start the value.
                    await this.asynchronousJsonWriter.WriteNameAsync(propertyName)
                        .ConfigureAwait(false);

                    // Start array which will hold the entries in the resource set.
                    await this.asynchronousJsonWriter.StartArrayScopeAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    JsonLightNestedResourceInfoScope navigationLinkScope = (JsonLightNestedResourceInfoScope)this.ParentNestedResourceInfoScope;
                    if (!navigationLinkScope.ResourceSetWritten)
                    {
                        // Close the entity reference link array (if written)
                        if (navigationLinkScope.EntityReferenceLinkWritten)
                        {
                            await this.asynchronousJsonWriter.EndArrayScopeAsync()
                                .ConfigureAwait(false);
                        }

                        // Write the odata type.
                        await this.jsonLightResourceSerializer.WriteResourceSetStartMetadataPropertiesAsync(
                            resourceSet,
                            propertyName,
                            expectedResourceTypeName,
                            isUndeclared).ConfigureAwait(false);

                        // And then write the property name to start the value.
                        await this.asynchronousJsonWriter.WriteNameAsync(propertyName)
                            .ConfigureAwait(false);

                        // Start array which will hold the entries in the resource set.
                        await this.asynchronousJsonWriter.StartArrayScopeAsync()
                            .ConfigureAwait(false);

                        navigationLinkScope.ResourceSetWritten = true;
                    }
                }
            }

            this.jsonLightOutputContext.PropertyCacheHandler.EnterResourceSetScope(
                this.CurrentResourceSetScope.ResourceType,
                this.ScopeLevel);
        }

        /// <summary>
        /// Asynchronously finish writing a resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task EndResourceSetAsync(ODataResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            if (this.ParentNestedResourceInfo == null && (this.writingParameter || this.ParentScope.State == WriterState.ResourceSet))
            {
                // End the array which holds the entries in the resource set.
                await this.asynchronousJsonWriter.EndArrayScopeAsync()
                    .ConfigureAwait(false);
            }
            else if (this.ParentNestedResourceInfo == null)
            {
                // End the array which holds the entries in the resource set.
                await this.asynchronousJsonWriter.EndArrayScopeAsync()
                    .ConfigureAwait(false);

                // Write custom instance annotations
                await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                    resourceSet.InstanceAnnotations,
                    this.CurrentResourceSetScope.InstanceAnnotationWriteTracker).ConfigureAwait(false);

                if (this.writingResponse)
                {
                    // Write the next link if it's available.
                    await this.WriteResourceSetNextLinkAsync(resourceSet.NextPageLink, /*propertyName*/null)
                        .ConfigureAwait(false);

                    // Write the delta link if it's available.
                    await this.WriteResourceSetDeltaLinkAsync(resourceSet.DeltaLink)
                        .ConfigureAwait(false);
                }

                // Close the object wrapper.
                await this.asynchronousJsonWriter.EndObjectScopeAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                Debug.Assert(
                    this.ParentNestedResourceInfo != null && (!this.ParentNestedResourceInfo.IsCollection.HasValue || this.ParentNestedResourceInfo.IsCollection.Value),
                    "We should have verified that resource sets can only be written into IsCollection = true links in requests.");

                string propertyName = this.ParentNestedResourceInfo.Name;

                this.ValidateNoDeltaLinkForExpandedResourceSet(resourceSet);
                this.ValidateNoCustomInstanceAnnotationsForExpandedResourceSet(resourceSet);

                if (this.writingResponse)
                {
                    // End the array which holds the entries in the resource set.
                    // NOTE: in requests we will only write the EndArray of a resource set
                    //       when we hit the nested resource info end since a nested resource info
                    //       can contain multiple resource sets that get collapsed into a single array value.
                    await this.asynchronousJsonWriter.EndArrayScopeAsync()
                        .ConfigureAwait(false);

                    // Write the next link if it's available.
                    await this.WriteResourceSetNextLinkAsync(resourceSet.NextPageLink, propertyName)
                        .ConfigureAwait(false);
                }
            }

            this.jsonLightOutputContext.PropertyCacheHandler.LeaveResourceSetScope();
        }

        /// <summary>
        /// Asynchronously start writing a delta resource set.
        /// </summary>
        /// <param name="deltaResourceSet">The delta resource set to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task StartDeltaResourceSetAsync(ODataDeltaResourceSet deltaResourceSet)
        {
            Debug.Assert(deltaResourceSet != null, "resourceSet != null");

            if (this.ParentNestedResourceInfo == null)
            {
                await this.asynchronousJsonWriter.StartObjectScopeAsync()
                    .ConfigureAwait(false);

                // Write ContextUrl
                this.CurrentDeltaResourceSetScope.ContextUriInfo = await this.jsonLightResourceSerializer.WriteDeltaContextUriAsync(
                    this.CurrentDeltaResourceSetScope.GetOrCreateTypeContext(this.writingResponse),
                    ODataDeltaKind.ResourceSet).ConfigureAwait(false);

                // Write Count, if available
                await this.WriteResourceSetCountAsync(deltaResourceSet.Count, /*propertyname*/ null)
                    .ConfigureAwait(false);

                // Write NextLink, if available
                await this.WriteResourceSetNextLinkAsync(deltaResourceSet.NextPageLink, /*propertyname*/ null)
                    .ConfigureAwait(false);

                // If we haven't written the delta link yet and it's available, write it.
                await this.WriteResourceSetDeltaLinkAsync(deltaResourceSet.DeltaLink)
                    .ConfigureAwait(false);

                // Write annotations
                await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                    deltaResourceSet.InstanceAnnotations,
                    this.CurrentDeltaResourceSetScope.InstanceAnnotationWriteTracker).ConfigureAwait(false);

                // Write Value Start
                await this.asynchronousJsonWriter.WriteValuePropertyNameAsync()
                    .ConfigureAwait(false);
                await this.asynchronousJsonWriter.StartArrayScopeAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                // Nested Delta
                Debug.Assert(
                    this.ParentNestedResourceInfo != null && this.ParentNestedResourceInfo.IsCollection.HasValue && this.ParentNestedResourceInfo.IsCollection.Value,
                    "We should have verified that resource sets can only be written into IsCollection = true links in requests.");

                string propertyName = this.ParentNestedResourceInfo.Name;

                // Write the inline count if it's available.
                await this.WriteResourceSetCountAsync(deltaResourceSet.Count, propertyName)
                    .ConfigureAwait(false);

                // Write the next link if it's available.
                await this.WriteResourceSetNextLinkAsync(deltaResourceSet.NextPageLink, propertyName)
                    .ConfigureAwait(false);

                //// Write the name for the nested delta payload
                await this.asynchronousJsonWriter.WritePropertyAnnotationNameAsync(
                    propertyName,
                    JsonLightConstants.ODataDeltaPropertyName).ConfigureAwait(false);

                // Start array which will hold the entries in the nested delta resource set.
                await this.asynchronousJsonWriter.StartArrayScopeAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously finish writing a delta resource set.
        /// </summary>
        /// <param name="deltaResourceSet">The resource set to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task EndDeltaResourceSetAsync(ODataDeltaResourceSet deltaResourceSet)
        {
            Debug.Assert(deltaResourceSet != null, "deltaResourceSet != null");

            if (this.ParentNestedResourceInfo == null)
            {
                // End the array which holds the entries in the resource set.
                await this.asynchronousJsonWriter.EndArrayScopeAsync()
                    .ConfigureAwait(false);

                // Write custom instance annotations
                await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                    deltaResourceSet.InstanceAnnotations,
                    this.CurrentDeltaResourceSetScope.InstanceAnnotationWriteTracker).ConfigureAwait(false);

                // Write the next link if it's available.
                await this.WriteResourceSetNextLinkAsync(deltaResourceSet.NextPageLink, /*propertynamne*/ null)
                    .ConfigureAwait(false);

                // Write the delta link if it's available.
                await this.WriteResourceSetDeltaLinkAsync(deltaResourceSet.DeltaLink)
                    .ConfigureAwait(false);

                // Close the object wrapper.
                await this.asynchronousJsonWriter.EndObjectScopeAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                // End the array which holds the entries in the resource set.
                await this.asynchronousJsonWriter.EndArrayScopeAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously start writing a delta deleted resource.
        /// </summary>
        /// <param name="resource">The resource to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task StartDeletedResourceAsync(ODataDeletedResource resource)
        {
            Debug.Assert(resource != null, "resource != null");
            Debug.Assert(!this.IsTopLevel, "Delta resource cannot be on top level.");
            DeletedResourceScope resourceScope = this.CurrentDeletedResourceScope;
            Debug.Assert(resourceScope != null, "Writing deleted entry and scope is not DeltaResourceScope");

            ODataNestedResourceInfo parentNavLink = this.ParentNestedResourceInfo;
            if (parentNavLink != null)
            {
                // Writing a nested deleted resource
                if (this.Version == null || this.Version < ODataVersion.V401)
                {
                    throw new ODataException(Strings.ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry);
                }
                else
                {
                    // Write the property name of an expanded navigation property to start the value.
                    await this.asynchronousJsonWriter.WriteNameAsync(parentNavLink.Name)
                        .ConfigureAwait(false);
                    await this.asynchronousJsonWriter.StartObjectScopeAsync()
                        .ConfigureAwait(false);
                    await this.WriteDeletedEntryContentsAsync(resource)
                        .ConfigureAwait(false);
                }
            }
            else
            {
                // Writing a deleted resource within an entity set
                DeltaResourceSetScope deltaResourceSetScope = this.ParentScope as DeltaResourceSetScope;
                Debug.Assert(deltaResourceSetScope != null, "Writing child of delta set and parent scope is not DeltaResourceSetScope");

                await this.asynchronousJsonWriter.StartObjectScopeAsync()
                    .ConfigureAwait(false);

                if (this.Version == null || this.Version < ODataVersion.V401)
                {
                    // Write ContextUrl
                    await this.jsonLightResourceSerializer.WriteDeltaContextUriAsync(
                            this.CurrentDeletedResourceScope.GetOrCreateTypeContext(this.writingResponse),
                            ODataDeltaKind.DeletedEntry,
                            deltaResourceSetScope.ContextUriInfo).ConfigureAwait(false);
                    await this.WriteV4DeletedEntryContentsAsync(resource)
                        .ConfigureAwait(false);
                }
                else
                {
                    // Only write ContextUrl for V4.01 deleted resource if it is in a top level delta
                    // resource set and comes from a different entity set
                    string expectedNavigationSource = deltaResourceSetScope.NavigationSource?.Name;
                    string currentNavigationSource = resource.SerializationInfo?.NavigationSourceName ?? resourceScope.NavigationSource?.Name;

                    if (String.IsNullOrEmpty(currentNavigationSource) || currentNavigationSource != expectedNavigationSource)
                    {
                        Debug.Assert(this.ScopeLevel == 3, "Writing a nested deleted resource of the wrong type should already have been caught.");

                        // We are writing a deleted resource in a top level delta resource set
                        // from a different entity set, so include the context Url
                        await this.jsonLightResourceSerializer.WriteDeltaContextUriAsync(
                                this.CurrentDeletedResourceScope.GetOrCreateTypeContext(this.writingResponse),
                                ODataDeltaKind.DeletedEntry,
                                deltaResourceSetScope.ContextUriInfo).ConfigureAwait(false);
                    }

                    await this.WriteDeletedEntryContentsAsync(resource)
                        .ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Asynchronously finish writing a deleted resource.
        /// </summary>
        /// <param name="deletedResource">The resource to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task EndDeletedResourceAsync(ODataDeletedResource deletedResource)
        {
            if (deletedResource == null)
            {
                return TaskUtils.CompletedTask;
            }

            // Close the object scope
            return this.asynchronousJsonWriter.EndObjectScopeAsync();
        }

        /// <summary>
        /// Asynchronously start writing a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task StartDeltaLinkAsync(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");

            await this.asynchronousJsonWriter.StartObjectScopeAsync()
                .ConfigureAwait(false);

            if (link is ODataDeltaLink)
            {
                await this.WriteDeltaLinkContextUriAsync(ODataDeltaKind.Link)
                    .ConfigureAwait(false);
            }
            else
            {
                Debug.Assert(link is ODataDeltaDeletedLink, "link must be either DeltaLink or DeltaDeletedLink.");
                await this.WriteDeltaLinkContextUriAsync(ODataDeltaKind.DeletedLink)
                    .ConfigureAwait(false);
            }

            await this.WriteDeltaLinkSourceAsync(link)
                .ConfigureAwait(false);
            await this.WriteDeltaLinkRelationshipAsync(link)
                .ConfigureAwait(false);
            await this.WriteDeltaLinkTargetAsync(link)
                .ConfigureAwait(false);
            await this.asynchronousJsonWriter.EndObjectScopeAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously create a <see cref="Stream"/> for writing a binary value.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation. 
        /// The value of the TResult parameter contains the <see cref="Stream"/> for writing the binary value.</returns>
        protected override async Task<Stream> StartBinaryStreamAsync()
        {
            ODataPropertyInfo property;
            if ((property = this.ParentScope?.Item as ODataPropertyInfo) != null)
            {
                // writing a stream property - write the property name
                await this.asynchronousJsonWriter.WriteNameAsync(property.Name)
                    .ConfigureAwait(false);
                await this.asynchronousJsonWriter.FlushAsync()
                    .ConfigureAwait(false);
            }

            if (this.asynchronousJsonStreamWriter == null)
            {
                this.jsonLightOutputContext.BinaryValueStream = new MemoryStream();
                return this.jsonLightOutputContext.BinaryValueStream;
            }
            else
            {
                return await this.asynchronousJsonStreamWriter.StartStreamValueScopeAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously finish writing a stream value.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected sealed override async Task EndBinaryStreamAsync()
        {
            if (this.asynchronousJsonStreamWriter == null)
            {
                await this.asynchronousJsonWriter.WriteValueAsync(
                    this.jsonLightOutputContext.BinaryValueStream.ToArray()).ConfigureAwait(false);
                await this.jsonLightOutputContext.BinaryValueStream.FlushAsync()
                    .ConfigureAwait(false);
                this.jsonLightOutputContext.BinaryValueStream.Dispose();
                this.jsonLightOutputContext.BinaryValueStream = null;
            }
            else
            {
                await this.asynchronousJsonStreamWriter.EndStreamValueScopeAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously create a <see cref="TextWriter"/> for writing a string value.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. 
        /// The value of the TResult parameter contains the <see cref="TextWriter"/> for writing a string value.</returns>
        protected override async Task<TextWriter> StartTextWriterAsync()
        {
            ODataPropertyInfo property;
            if ((property = this.ParentScope?.Item as ODataPropertyInfo) != null)
            {
                // Writing a text property - write the property name
                await this.asynchronousJsonWriter.WriteNameAsync(property.Name)
                    .ConfigureAwait(false);
                await this.asynchronousJsonWriter.FlushAsync()
                    .ConfigureAwait(false);
            }

            TextWriter writer;
            if (this.asynchronousJsonStreamWriter == null)
            {
                this.jsonLightOutputContext.StringWriter = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
                writer = this.jsonLightOutputContext.StringWriter;
            }
            else
            {
                string contentType = "text/plain";
                ODataStreamPropertyInfo streamInfo = property as ODataStreamPropertyInfo;
                if (streamInfo?.ContentType != null)
                {
                    contentType = streamInfo.ContentType;
                }

                writer = await this.asynchronousJsonStreamWriter.StartTextWriterValueScopeAsync(contentType)
                    .ConfigureAwait(false);
            }

            return writer;
        }

        /// <summary>
        /// Asynchronously finish writing a text value.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected sealed override async Task EndTextWriterAsync()
        {
            if (this.asynchronousJsonStreamWriter == null)
            {
                Debug.Assert(this.jsonLightOutputContext.StringWriter != null,
                    "Calling EndTextWriter with a non-streaming asynchronous JsonWriter and a null StringWriter");

                await this.jsonLightOutputContext.StringWriter.FlushAsync()
                    .ConfigureAwait(false);
                await this.asynchronousJsonWriter.WriteValueAsync(
                    this.jsonLightOutputContext.StringWriter.GetStringBuilder().ToString()).ConfigureAwait(false);
                this.jsonLightOutputContext.StringWriter.Dispose();
                this.jsonLightOutputContext.StringWriter = null;
            }
            else
            {
                await this.asynchronousJsonStreamWriter.EndTextWriterValueScopeAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously write a primitive type inside an untyped collection.
        /// </summary>
        /// <param name="primitiveValue">The primitive value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task WritePrimitiveValueAsync(ODataPrimitiveValue primitiveValue)
        {
            ODataPropertyInfo property;
            if ((property = this.ParentScope?.Item as ODataPropertyInfo) != null)
            {
                await this.asynchronousJsonWriter.WriteNameAsync(property.Name)
                    .ConfigureAwait(false);
            }

            if (primitiveValue == null)
            {
                await this.jsonLightValueSerializer.WriteNullValueAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                await this.jsonLightValueSerializer.WritePrimitiveValueAsync(primitiveValue.Value, /*expectedType*/ null)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously start writing a deferred (non-expanded) nested resource info.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task WriteDeferredNestedResourceInfoAsync(ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(this.writingResponse, "Deferred links are only supported in response, we should have verified this already.");

            // A deferred nested resource info is just the link metadata, no value.
            return this.jsonLightResourceSerializer.WriteNavigationLinkMetadataAsync(
                nestedResourceInfo,
                this.DuplicatePropertyNameChecker);
        }

        /// <summary>
        /// Asynchronously start writing the nested resource info with content.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task StartNestedResourceInfoWithContentAsync(ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(!string.IsNullOrEmpty(nestedResourceInfo.Name),
                "The nested resource info name should have been verified by now.");

            if (this.writingResponse)
            {
                // Write @odata.context annotation for navigation property
                IEdmContainedEntitySet containedEntitySet = this.CurrentScope.NavigationSource as IEdmContainedEntitySet;
                if (containedEntitySet != null
                    && this.messageWriterSettings.LibraryCompatibility < ODataLibraryCompatibility.Version7
                    && this.messageWriterSettings.Version < ODataVersion.V401)
                {
                    ODataContextUrlInfo info = ODataContextUrlInfo.Create(
                        this.CurrentScope.NavigationSource,
                        this.CurrentScope.ResourceType.FullTypeName(),
                        containedEntitySet.NavigationProperty.Type.TypeKind() != EdmTypeKind.Collection,
                        this.CurrentScope.ODataUri,
                        this.messageWriterSettings.Version ?? ODataVersion.V4);

                    await this.jsonLightResourceSerializer.WriteNestedResourceInfoContextUrlAsync(nestedResourceInfo, info)
                        .ConfigureAwait(false);
                }

                // Write the nested resource info metadata first. The rest is written by the content resource or resource set.
                await this.jsonLightResourceSerializer.WriteNavigationLinkMetadataAsync(
                    nestedResourceInfo,
                    this.DuplicatePropertyNameChecker).ConfigureAwait(false);
            }
            else
            {
                this.WriterValidator.ValidateNestedResourceInfoHasCardinality(nestedResourceInfo);
            }
        }

        /// <summary>
        /// Asynchronously finish writing nested resource info with content.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task EndNestedResourceInfoWithContentAsync(ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");

            JsonLightNestedResourceInfoScope navigationLinkScope = (JsonLightNestedResourceInfoScope)this.CurrentScope;

            // If we wrote entity reference links for a collection navigation property but no
            // resource set afterwards, we have to now close the array of links.
            if (!this.writingResponse)
            {
                if (navigationLinkScope.EntityReferenceLinkWritten && !navigationLinkScope.ResourceSetWritten && nestedResourceInfo.IsCollection.Value)
                {
                    await this.asynchronousJsonWriter.EndArrayScopeAsync()
                        .ConfigureAwait(false);
                }

                // In requests, the nested resource info may have multiple entries in multiple resource sets in it; if we
                // wrote at least one resource set, close the resulting array here.
                if (navigationLinkScope.ResourceSetWritten)
                {
                    Debug.Assert(nestedResourceInfo.IsCollection == null || nestedResourceInfo.IsCollection.Value, "nestedResourceInfo.IsCollection.Value");
                    await this.asynchronousJsonWriter.EndArrayScopeAsync()
                        .ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Asynchronously write an entity reference link.
        /// </summary>
        /// <param name="parentNestedResourceInfo">The parent navigation link which is being written around the entity reference link.</param>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task WriteEntityReferenceInNavigationLinkContentAsync(
            ODataNestedResourceInfo parentNestedResourceInfo,
            ODataEntityReferenceLink entityReferenceLink)
        {
            Debug.Assert(parentNestedResourceInfo != null, "parentNestedResourceInfo != null");
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            // In JSON Light, we can only write entity reference links at the beginning of a navigation link in requests;
            // once we write a resource set, entity reference links are not allowed anymore (we require all the entity reference
            // link to come first because of the grouping in the JSON Light wire format).
            JsonLightNestedResourceInfoScope nestedResourceScope = this.CurrentScope as JsonLightNestedResourceInfoScope;
            if (nestedResourceScope == null)
            {
                nestedResourceScope = this.ParentNestedResourceInfoScope as JsonLightNestedResourceInfoScope;
            }

            if (nestedResourceScope.ResourceSetWritten)
            {
                throw new ODataException(Strings.ODataJsonLightWriter_EntityReferenceLinkAfterResourceSetInRequest);
            }

            if (!nestedResourceScope.EntityReferenceLinkWritten)
            {
                // In request
                if (!this.writingResponse)
                {
                    if (this.Version == null || this.Version < ODataVersion.V401)
                    {
                        // Write the property annotation for the entity reference link(s)
                        await this.asynchronousODataAnnotationWriter.WritePropertyAnnotationNameAsync(
                            parentNestedResourceInfo.Name,
                            ODataAnnotationNames.ODataBind).ConfigureAwait(false);
                    }
                    else
                    {
                        await this.asynchronousJsonWriter.WriteNameAsync(parentNestedResourceInfo.Name)
                            .ConfigureAwait(false);
                    }

                    Debug.Assert(parentNestedResourceInfo.IsCollection.HasValue, "parentNestedResourceInfo.IsCollection.HasValue");
                    if (parentNestedResourceInfo.IsCollection.Value)
                    {
                        // Write [ for the collection
                        await this.asynchronousJsonWriter.StartArrayScopeAsync()
                            .ConfigureAwait(false);
                    }
                }
                else
                {
                    // In response
                    Debug.Assert(parentNestedResourceInfo.IsCollection.HasValue, "parentNestedResourceInfo.IsCollection.HasValue");
                    if (!parentNestedResourceInfo.IsCollection.Value)
                    {
                        // Write the property name for single-nested resource,
                        // for the collection nested resource, it's write at top level when writing ODataResourceSet
                        await this.asynchronousJsonWriter.WriteNameAsync(parentNestedResourceInfo.Name)
                            .ConfigureAwait(false);
                    }
                }

                nestedResourceScope.EntityReferenceLinkWritten = true;
            }

            if (!this.writingResponse && (this.Version == null || this.Version < ODataVersion.V401))
            {
                Debug.Assert(entityReferenceLink.Url != null,
                    "The entity reference link Url should have been validated by now.");
                await this.asynchronousJsonWriter.WriteValueAsync(
                    this.jsonLightResourceSerializer.UriToString(entityReferenceLink.Url)).ConfigureAwait(false);
            }
            else
            {
                await WriteEntityReferenceLinkImplementationAsync(entityReferenceLink)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Place where derived writers can perform custom steps before the resource is written, 
        /// at the beginning of WriteStartAsync(ODataResource).
        /// </summary>
        /// <param name="resourceScope">The ResourceScope.</param>
        /// <param name="resource">Resource to write.</param>
        /// <param name="writingResponse">True if writing response.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected override Task PrepareResourceForWriteStartAsync(
            ResourceScope resourceScope,
            ODataResource resource,
            bool writingResponse,
            SelectedPropertiesNode selectedProperties)
        {
            // Currently, no asynchronous operation is involved when preparing resource for writing
            return TaskUtils.GetTaskForSynchronousOperation(
                () => this.PrepareResourceForWriteStart(
                    resourceScope,
                    resource,
                    writingResponse,
                    selectedProperties));
        }

        /// <summary>
        /// Place where derived writers can perform custom steps before the deleted resource is written, 
        /// at the beginning of WriteStartAsync(ODataDeletedResource).
        /// </summary>
        /// <param name="resourceScope">The ResourceScope.</param>
        /// <param name="deletedResource">Resource to write.</param>
        /// <param name="writingResponse">True if writing response.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected override Task PrepareDeletedResourceForWriteStartAsync(
            DeletedResourceScope resourceScope,
            ODataDeletedResource deletedResource,
            bool writingResponse,
            SelectedPropertiesNode selectedProperties)
        {
            // Currently, no asynchronous operation is involved when preparing deleted resource for writing
            return TaskUtils.GetTaskForSynchronousOperation(
                () => this.PrepareDeletedResourceForWriteStart(
                    resourceScope,
                    deletedResource,
                    writingResponse,
                    selectedProperties));
        }

        /// <summary>
        /// Write the entity reference link.
        /// </summary>
        /// <param name="entityReferenceLink">The OData entity reference link.</param>
        private void WriteEntityReferenceLinkImplementation(ODataEntityReferenceLink entityReferenceLink)
        {
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            WriterValidationUtils.ValidateEntityReferenceLink(entityReferenceLink);

            this.jsonWriter.StartObjectScope();

            this.odataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataId);

            Uri id = this.messageWriterSettings.MetadataDocumentUri.MakeRelativeUri(entityReferenceLink.Url);

            this.jsonWriter.WriteValue(id == null ? null : this.jsonLightResourceSerializer.UriToString(id));

            this.instanceAnnotationWriter.WriteInstanceAnnotations(entityReferenceLink.InstanceAnnotations);

            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Sets resource's metadata builder based on current metadata level.
        /// </summary>
        /// <param name="resource">Resource to write.</param>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the resource or resource set.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        private void InnerPrepareResourceForWriteStart(ODataResourceBase resource, ODataResourceTypeContext typeContext, SelectedPropertiesNode selectedProperties)
        {
            ODataResourceSerializationInfo serializationInfo;
            IEdmStructuredType resourceType;
            ODataUri uri;

            if (resource is ODataResource)
            {
                ResourceScope resourceScope = (ResourceScope)this.CurrentScope;
                Debug.Assert(resourceScope != null, "resourceScope != null");
                serializationInfo = resourceScope.SerializationInfo;
                resourceType = resourceScope.ResourceType;
                uri = resourceScope.ODataUri;
            }
            else
            {
                DeletedResourceScope resourceScope = (DeletedResourceScope)this.CurrentScope;
                Debug.Assert(resourceScope != null, "resourceScope != null");
                serializationInfo = resourceScope.SerializationInfo;
                resourceType = resourceScope.ResourceType;
                uri = resourceScope.ODataUri;
            }

            ODataResourceMetadataBuilder builder = this.jsonLightOutputContext.MetadataLevel.CreateResourceMetadataBuilder(
                resource,
                typeContext,
                serializationInfo,
                resourceType,
                selectedProperties,
                this.writingResponse,
                this.jsonLightOutputContext.ODataSimplifiedOptions.EnableWritingKeyAsSegment,
                uri,
                this.messageWriterSettings);

            if (builder != null)
            {
                builder.NameAsProperty = this.BelongingNestedResourceInfo != null ? this.BelongingNestedResourceInfo.Name : null;
                builder.IsFromCollection = this.BelongingNestedResourceInfo != null && this.BelongingNestedResourceInfo.IsCollection == true;

                if (builder is ODataConventionalResourceMetadataBuilder)
                {
                    builder.ParentMetadataBuilder = this.FindParentResourceMetadataBuilder();
                }

                this.jsonLightOutputContext.MetadataLevel.InjectMetadataBuilder(resource, builder);
            }
        }

        /// <summary>
        /// Find instance of the metadata builder which belong to the parent odata resource
        /// </summary>
        /// <returns>
        /// The metadata builder of the parent odata resource
        /// Or null if there is no parent odata resource
        /// </returns>
        private ODataResourceMetadataBuilder FindParentResourceMetadataBuilder()
        {
            ResourceScope parentResourceScope = this.GetParentResourceScope();

            if (parentResourceScope != null)
            {
                ODataResourceBase resource = parentResourceScope.Item as ODataResourceBase;
                if (resource != null)
                {
                    return resource.MetadataBuilder;
                }
            }

            return null;
        }

        /// <summary>
        /// Writes the odata.count annotation for a resource set if it has not been written yet (and the count is specified on the resource set).
        /// </summary>
        /// <param name="count">The count to write for the resource set.</param>
        /// <param name="propertyName">The name of the expanded nav property or null for a top-level resource set.</param>
        private void WriteResourceSetCount(long? count, string propertyName)
        {
            if (count.HasValue)
            {
                if (propertyName == null)
                {
                    this.odataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataCount);
                }
                else
                {
                    this.odataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataCount);
                }

                this.jsonWriter.WriteValue(count.Value);
            }
        }

        /// <summary>
        /// Writes the odata.nextLink annotation for a resource set if it has not been written yet (and the next link is specified on the resource set).
        /// </summary>
        /// <param name="nextPageLink">The nextLink to write, if available.</param>
        /// <param name="propertyName">The name of the expanded nav property or null for a top-level resource set.</param>
        private void WriteResourceSetNextLink(Uri nextPageLink, string propertyName)
        {
            bool nextPageWritten = this.State == WriterState.ResourceSet ? this.CurrentResourceSetScope.NextPageLinkWritten : this.CurrentDeltaResourceSetScope.NextPageLinkWritten;
            if (nextPageLink != null && !nextPageWritten)
            {
                if (propertyName == null)
                {
                    this.odataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataNextLink);
                }
                else
                {
                    this.odataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataNextLink);
                }

                this.jsonWriter.WriteValue(this.jsonLightResourceSerializer.UriToString(nextPageLink));

                if (this.State == WriterState.ResourceSet)
                {
                    this.CurrentResourceSetScope.NextPageLinkWritten = true;
                }
                else
                {
                    this.CurrentDeltaResourceSetScope.NextPageLinkWritten = true;
                }
            }
        }

        /// <summary>
        /// Writes the odata.deltaLink annotation for a resource set if it has not been written yet (and the delta link is specified on the resource set).
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        private void WriteResourceSetDeltaLink(Uri deltaLink)
        {
            if (deltaLink == null)
            {
                return;
            }

            Debug.Assert(this.State == WriterState.ResourceSet || this.State == WriterState.DeltaResourceSet, "Write ResourceSet Delta Link called when not in ResourceSet or DeltaResourceSet state");

            bool deltaLinkWritten = this.State == WriterState.ResourceSet ? this.CurrentResourceSetScope.DeltaLinkWritten : this.CurrentDeltaResourceSetScope.DeltaLinkWritten;
            if (!deltaLinkWritten)
            {
                this.odataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataDeltaLink);
                this.jsonWriter.WriteValue(this.jsonLightResourceSerializer.UriToString(deltaLink));
                if (this.State == WriterState.ResourceSet)
                {
                    this.CurrentResourceSetScope.DeltaLinkWritten = true;
                }
                else
                {
                    this.CurrentDeltaResourceSetScope.DeltaLinkWritten = true;
                }
            }
        }

        private void WriteV4DeletedEntryContents(ODataDeletedResource resource)
        {
            this.WriteDeletedResourceId(resource);
            this.WriteDeltaResourceReason(resource);
        }

        private void WriteDeletedEntryContents(ODataDeletedResource resource)
        {
            this.odataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataRemoved);
            this.jsonWriter.StartObjectScope();
            this.WriteDeltaResourceReason(resource);
            this.jsonWriter.EndObjectScope();

            JsonLightDeletedResourceScope resourceScope = this.CurrentDeletedResourceScope;

            // Write the metadata
            this.jsonLightResourceSerializer.WriteResourceStartMetadataProperties(resourceScope);
            this.jsonLightResourceSerializer.WriteResourceMetadataProperties(resourceScope);

            this.jsonLightOutputContext.PropertyCacheHandler.SetCurrentResourceScopeLevel(this.ScopeLevel);

            // Write custom instance annotations
            this.instanceAnnotationWriter.WriteInstanceAnnotations(resource.InstanceAnnotations, resourceScope.InstanceAnnotationWriteTracker);

            this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
            this.WriteDeltaResourceProperties(resource);
            this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
        }

        /// <summary>
        /// Writes the id property for a delta deleted resource.
        /// </summary>
        /// <param name="resource">The resource to write the id for.</param>
        private void WriteDeletedResourceId(ODataDeletedResource resource)
        {
            Debug.Assert(resource != null, "resource != null");
            if (this.Version == null || this.Version < ODataVersion.V401)
            {
                this.jsonWriter.WriteName(JsonLightConstants.ODataIdPropertyName);
                this.jsonWriter.WriteValue(resource.Id.OriginalString);
            }
            else
            {
                Uri id;
                if (resource.MetadataBuilder.TryGetIdForSerialization(out id))
                {
                    this.jsonWriter.WriteInstanceAnnotationName(JsonLightConstants.ODataIdPropertyName);
                    this.jsonWriter.WriteValue(id.OriginalString);
                }
            }
        }

        /// <summary>
        /// Writes the properties for a delta resource.
        /// </summary>
        /// <param name="resource">The resource whose properties to write.</param>
        private void WriteDeltaResourceProperties(ODataResourceBase resource)
        {
            this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
            if (resource.NonComputedProperties != null)
            {
                this.jsonLightResourceSerializer.WriteProperties(
                    this.ResourceType,
                    resource.NonComputedProperties,
                    false /* isComplexValue */,
                    this.DuplicatePropertyNameChecker,
                    resource.MetadataBuilder);
                this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
            }
        }

        /// <summary>
        /// Writes the reason annotation for a delta deleted resource.
        /// </summary>
        /// <param name="resource">The resource to write the reason for.</param>
        private void WriteDeltaResourceReason(ODataDeletedResource resource)
        {
            Debug.Assert(resource != null, "resource != null");

            if (!resource.Reason.HasValue)
            {
                return;
            }

            this.jsonWriter.WriteName(JsonLightConstants.ODataReasonPropertyName);

            switch (resource.Reason.Value)
            {
                case DeltaDeletedEntryReason.Deleted:
                    this.jsonWriter.WriteValue(JsonLightConstants.ODataReasonDeletedValue);
                    break;
                case DeltaDeletedEntryReason.Changed:
                    this.jsonWriter.WriteValue(JsonLightConstants.ODataReasonChangedValue);
                    break;
                default:
                    Debug.Assert(false, "Unknown reason.");
                    break;
            }
        }

        /// <summary>
        /// Writes the context uri for a delta (deleted) link.
        /// </summary>
        /// <param name="kind">The delta kind of link.</param>
        private void WriteDeltaLinkContextUri(ODataDeltaKind kind)
        {
            Debug.Assert(kind == ODataDeltaKind.Link || kind == ODataDeltaKind.DeletedLink, "kind must be either DeltaLink or DeltaDeletedLink.");
            this.jsonLightResourceSerializer.WriteDeltaContextUri(this.CurrentDeltaLinkScope.GetOrCreateTypeContext(), kind);
        }

        /// <summary>
        /// Writes the source for a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write source for.</param>
        private void WriteDeltaLinkSource(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link is ODataDeltaLink || link is ODataDeltaDeletedLink, "link must be either DeltaLink or DeltaDeletedLink.");

            this.jsonWriter.WriteName(JsonLightConstants.ODataSourcePropertyName);
            this.jsonWriter.WriteValue(UriUtils.UriToString(link.Source));
        }

        /// <summary>
        /// Writes the relationship for a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write relationship for.</param>
        private void WriteDeltaLinkRelationship(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link is ODataDeltaLink || link is ODataDeltaDeletedLink, "link must be either DeltaLink or DeltaDeletedLink.");

            this.jsonWriter.WriteName(JsonLightConstants.ODataRelationshipPropertyName);
            this.jsonWriter.WriteValue(link.Relationship);
        }

        /// <summary>
        /// Writes the target for a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write target for.</param>
        private void WriteDeltaLinkTarget(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link is ODataDeltaLink || link is ODataDeltaDeletedLink, "link must be either DeltaLink or DeltaDeletedLink.");

            this.jsonWriter.WriteName(JsonLightConstants.ODataTargetPropertyName);
            this.jsonWriter.WriteValue(UriUtils.UriToString(link.Target));
        }

        /// <summary>
        /// Validates that the ODataResourceSet.InstanceAnnotations collection is empty for the given expanded resource set.
        /// </summary>
        /// <param name="resourceSet">The expanded resource set in question.</param>
        private void ValidateNoCustomInstanceAnnotationsForExpandedResourceSet(ODataResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(
                this.ParentNestedResourceInfo != null && (!this.ParentNestedResourceInfo.IsCollection.HasValue || this.ParentNestedResourceInfo.IsCollection.Value == true),
                "This should only be called when writing an expanded resource set.");

            if (resourceSet.InstanceAnnotations.Count > 0)
            {
                throw new ODataException(Strings.ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet);
            }
        }

        /// <summary>
        /// Asynchronously write the entity reference link.
        /// </summary>
        /// <param name="entityReferenceLink">The OData entity reference link.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteEntityReferenceLinkImplementationAsync(ODataEntityReferenceLink entityReferenceLink)
        {
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            WriterValidationUtils.ValidateEntityReferenceLink(entityReferenceLink);

            await this.asynchronousJsonWriter.StartObjectScopeAsync()
                .ConfigureAwait(false);

            await this.asynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataId)
                .ConfigureAwait(false);

            Uri id = this.messageWriterSettings.MetadataDocumentUri.MakeRelativeUri(entityReferenceLink.Url);

            await this.asynchronousJsonWriter.WriteValueAsync(
                id == null ? null : this.jsonLightResourceSerializer.UriToString(id)).ConfigureAwait(false);

            await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                entityReferenceLink.InstanceAnnotations).ConfigureAwait(false);

            await this.asynchronousJsonWriter.EndObjectScopeAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the odata.count annotation for a resource set if it has not been written yet
        /// (and the count is specified on the resource set).
        /// </summary>
        /// <param name="count">The count to write for the resource set.</param>
        /// <param name="propertyName">The name of the expanded nav property or null for a top-level resource set.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteResourceSetCountAsync(long? count, string propertyName)
        {
            if (count.HasValue)
            {
                if (propertyName == null)
                {
                    await this.asynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(
                        ODataAnnotationNames.ODataCount).ConfigureAwait(false);
                }
                else
                {
                    await this.asynchronousODataAnnotationWriter.WritePropertyAnnotationNameAsync(
                        propertyName,
                        ODataAnnotationNames.ODataCount).ConfigureAwait(false);
                }

                await this.asynchronousJsonWriter.WriteValueAsync(count.Value)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes the odata.nextLink annotation for a resource set if it has not been written yet (and the next link is specified on the resource set).
        /// </summary>
        /// <param name="nextPageLink">The nextLink to write, if available.</param>
        /// <param name="propertyName">The name of the expanded nav property or null for a top-level resource set.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteResourceSetNextLinkAsync(Uri nextPageLink, string propertyName)
        {
            bool nextPageWritten = this.State == WriterState.ResourceSet ?
                this.CurrentResourceSetScope.NextPageLinkWritten :
                this.CurrentDeltaResourceSetScope.NextPageLinkWritten;

            if (nextPageLink != null && !nextPageWritten)
            {
                if (propertyName == null)
                {
                    await this.asynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(
                        ODataAnnotationNames.ODataNextLink).ConfigureAwait(false);
                }
                else
                {
                    await this.asynchronousODataAnnotationWriter.WritePropertyAnnotationNameAsync(
                        propertyName,
                        ODataAnnotationNames.ODataNextLink).ConfigureAwait(false);
                }

                await this.asynchronousJsonWriter.WriteValueAsync(
                    this.jsonLightResourceSerializer.UriToString(nextPageLink)).ConfigureAwait(false);

                if (this.State == WriterState.ResourceSet)
                {
                    this.CurrentResourceSetScope.NextPageLinkWritten = true;
                }
                else
                {
                    this.CurrentDeltaResourceSetScope.NextPageLinkWritten = true;
                }
            }
        }

        /// <summary>
        /// Asynchronously writes the odata.deltaLink annotation for a resource set if it has not been written yet
        /// (and the delta link is specified on the resource set).
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteResourceSetDeltaLinkAsync(Uri deltaLink)
        {
            if (deltaLink == null)
            {
                return;
            }

            Debug.Assert(this.State == WriterState.ResourceSet || this.State == WriterState.DeltaResourceSet,
                "Write ResourceSet Delta Link called when not in ResourceSet or DeltaResourceSet state");

            bool deltaLinkWritten = this.State == WriterState.ResourceSet
                ? this.CurrentResourceSetScope.DeltaLinkWritten :
                this.CurrentDeltaResourceSetScope.DeltaLinkWritten;

            if (!deltaLinkWritten)
            {
                await this.asynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(
                    ODataAnnotationNames.ODataDeltaLink).ConfigureAwait(false);
                await this.asynchronousJsonWriter.WriteValueAsync(
                    this.jsonLightResourceSerializer.UriToString(deltaLink)).ConfigureAwait(false);

                if (this.State == WriterState.ResourceSet)
                {
                    this.CurrentResourceSetScope.DeltaLinkWritten = true;
                }
                else
                {
                    this.CurrentDeltaResourceSetScope.DeltaLinkWritten = true;
                }
            }
        }

        /// <summary>
        /// Asynchronously write deleted entry contents for V4.
        /// </summary>
        /// <param name="resource">The deleted resource to write deleted entry contents for</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteV4DeletedEntryContentsAsync(ODataDeletedResource resource)
        {
            await this.WriteDeletedResourceIdAsync(resource)
                .ConfigureAwait(false);
            await this.WriteDeltaResourceReasonAsync(resource)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously write deleted entry contents.
        /// </summary>
        /// <param name="resource">The deleted resource to write deleted entry contents for</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteDeletedEntryContentsAsync(ODataDeletedResource resource)
        {
            await this.asynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(
                ODataAnnotationNames.ODataRemoved).ConfigureAwait(false);
            await this.asynchronousJsonWriter.StartObjectScopeAsync()
                .ConfigureAwait(false);
            await this.WriteDeltaResourceReasonAsync(resource)
                .ConfigureAwait(false);
            await this.asynchronousJsonWriter.EndObjectScopeAsync()
                .ConfigureAwait(false);

            JsonLightDeletedResourceScope resourceScope = this.CurrentDeletedResourceScope;

            // Write the metadata
            await this.jsonLightResourceSerializer.WriteResourceStartMetadataPropertiesAsync(resourceScope)
                .ConfigureAwait(false);
            await this.jsonLightResourceSerializer.WriteResourceMetadataPropertiesAsync(resourceScope)
                .ConfigureAwait(false);

            this.jsonLightOutputContext.PropertyCacheHandler.SetCurrentResourceScopeLevel(this.ScopeLevel);

            // Write custom instance annotations
            await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                resource.InstanceAnnotations,
                resourceScope.InstanceAnnotationWriteTracker).ConfigureAwait(false);

            this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
            await this.WriteDeltaResourcePropertiesAsync(resource)
                .ConfigureAwait(false);
            this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
        }

        /// <summary>
        /// Asynchronously writes the id property for a delta deleted resource.
        /// </summary>
        /// <param name="resource">The resource to write the id for.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteDeletedResourceIdAsync(ODataDeletedResource resource)
        {
            Debug.Assert(resource != null, "resource != null");
            if (this.Version == null || this.Version < ODataVersion.V401)
            {
                await this.asynchronousJsonWriter.WriteNameAsync(JsonLightConstants.ODataIdPropertyName)
                    .ConfigureAwait(false);
                await this.asynchronousJsonWriter.WriteValueAsync(resource.Id.OriginalString)
                    .ConfigureAwait(false);
            }
            else
            {
                Uri id;
                if (resource.MetadataBuilder.TryGetIdForSerialization(out id))
                {
                    await this.asynchronousJsonWriter.WriteInstanceAnnotationNameAsync(
                        JsonLightConstants.ODataIdPropertyName).ConfigureAwait(false);
                    await this.asynchronousJsonWriter.WriteValueAsync(id.OriginalString)
                        .ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Asynchronously writes the properties for a delta resource.
        /// </summary>
        /// <param name="resource">The resource whose properties to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private Task WriteDeltaResourcePropertiesAsync(ODataResourceBase resource)
        {
            Debug.Assert(resource != null, "resource != null");

            Task writePropertiesTask = TaskUtils.CompletedTask;

            this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
            if (resource.NonComputedProperties != null)
            {
                writePropertiesTask = this.jsonLightResourceSerializer.WritePropertiesAsync(
                    this.ResourceType,
                    resource.NonComputedProperties,
                    false /* isComplexValue */,
                    this.DuplicatePropertyNameChecker,
                    resource.MetadataBuilder);
                this.jsonLightResourceSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
            }

            return writePropertiesTask;
        }

        /// <summary>
        /// Asynchronously writes the reason annotation for a delta deleted resource.
        /// </summary>
        /// <param name="resource">The resource to write the reason for.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteDeltaResourceReasonAsync(ODataDeletedResource resource)
        {
            Debug.Assert(resource != null, "resource != null");

            if (!resource.Reason.HasValue)
            {
                return;
            }

            await this.asynchronousJsonWriter.WriteNameAsync(JsonLightConstants.ODataReasonPropertyName)
                .ConfigureAwait(false);

            switch (resource.Reason.Value)
            {
                case DeltaDeletedEntryReason.Deleted:
                    await this.asynchronousJsonWriter.WriteValueAsync(JsonLightConstants.ODataReasonDeletedValue)
                        .ConfigureAwait(false);
                    break;
                case DeltaDeletedEntryReason.Changed:
                    await this.asynchronousJsonWriter.WriteValueAsync(JsonLightConstants.ODataReasonChangedValue)
                        .ConfigureAwait(false);
                    break;
                default:
                    Debug.Assert(false, "Unknown reason.");
                    break;
            }
        }

        /// <summary>
        /// Asynchronously writes the context uri for a delta (deleted) link.
        /// </summary>
        /// <param name="kind">The delta kind of link.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private Task WriteDeltaLinkContextUriAsync(ODataDeltaKind kind)
        {
            Debug.Assert(kind == ODataDeltaKind.Link || kind == ODataDeltaKind.DeletedLink,
                "kind must be either DeltaLink or DeltaDeletedLink.");
            return this.jsonLightResourceSerializer.WriteDeltaContextUriAsync(
                this.CurrentDeltaLinkScope.GetOrCreateTypeContext(), kind);
        }

        /// <summary>
        /// Asynchronously writes the source for a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write source for.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteDeltaLinkSourceAsync(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link is ODataDeltaLink || link is ODataDeltaDeletedLink,
                "link must be either DeltaLink or DeltaDeletedLink.");

            await this.asynchronousJsonWriter.WriteNameAsync(JsonLightConstants.ODataSourcePropertyName)
                .ConfigureAwait(false);
            await this.asynchronousJsonWriter.WriteValueAsync(UriUtils.UriToString(link.Source))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the relationship for a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write relationship for.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteDeltaLinkRelationshipAsync(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link is ODataDeltaLink || link is ODataDeltaDeletedLink,
                "link must be either DeltaLink or DeltaDeletedLink.");

            await this.asynchronousJsonWriter.WriteNameAsync(JsonLightConstants.ODataRelationshipPropertyName)
                .ConfigureAwait(false);
            await this.asynchronousJsonWriter.WriteValueAsync(link.Relationship)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the target for a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write target for.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteDeltaLinkTargetAsync(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link is ODataDeltaLink || link is ODataDeltaDeletedLink,
                "link must be either DeltaLink or DeltaDeletedLink.");

            await this.asynchronousJsonWriter.WriteNameAsync(JsonLightConstants.ODataTargetPropertyName)
                .ConfigureAwait(false);
            await this.asynchronousJsonWriter.WriteValueAsync(UriUtils.UriToString(link.Target))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// A scope for a JSON lite resource set.
        /// </summary>
        private sealed class JsonLightResourceSetScope : ResourceSetScope
        {
            /// <summary>true if the odata.nextLink was already written, false otherwise.</summary>
            private bool nextLinkWritten;

            /// <summary>true if the odata.deltaLink was already written, false otherwise.</summary>
            private bool deltaLinkWritten;

            /// <summary>true if the resource set represents a collection of complex property or a collection navigation property that is undeclared, false otherwise.</summary>
            private bool isUndeclared;

            /// <summary>
            /// Constructor to create a new resource set scope.
            /// </summary>
            /// <param name="resourceSet">The resource set for the new scope.</param>
            /// <param name="navigationSource">The navigation source we are going to write resources for.</param>
            /// <param name="itemType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            /// <param name="isUndeclared">true if the resource set is for an undeclared property</param>
            internal JsonLightResourceSetScope(ODataResourceSet resourceSet, IEdmNavigationSource navigationSource, IEdmType itemType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri, bool isUndeclared)
                : base(resourceSet, navigationSource, itemType, skipWriting, selectedProperties, odataUri)
            {
                this.isUndeclared = isUndeclared;
            }

            /// <summary>
            /// true if the odata.nextLink annotation was already written, false otherwise.
            /// </summary>
            internal bool NextPageLinkWritten
            {
                get
                {
                    return this.nextLinkWritten;
                }

                set
                {
                    this.nextLinkWritten = value;
                }
            }

            /// <summary>
            /// true if the odata.deltaLink annotation was already written, false otherwise.
            /// </summary>
            internal bool DeltaLinkWritten
            {
                get
                {
                    return this.deltaLinkWritten;
                }

                set
                {
                    this.deltaLinkWritten = value;
                }
            }

            /// <summary>
            /// true if the resource set represents a collection of complex property or a collection navigation property that is undeclared, false otherwise.
            /// </summary>
            internal bool IsUndeclared
            {
                get { return isUndeclared; }
            }
        }

        /// <summary>
        /// A scope for a deleted resource in JSON Light writer.
        /// </summary>
        private sealed class JsonLightDeletedResourceScope : DeletedResourceScope, IODataJsonLightWriterResourceState
        {
            /// <summary>Bit field of the JSON Light metadata properties written so far.</summary>
            private int alreadyWrittenMetadataProperties;

            /// <summary>true if the resource set represents a complex property or a singleton navigation property that is undeclared, false otherwise.</summary>
            private bool isUndeclared;

            /// <summary>
            /// Constructor to create a new resource scope.
            /// </summary>
            /// <param name="resource">The resource for the new scope.</param>
            /// <param name="serializationInfo">The serialization info for the current resource.</param>
            /// <param name="navigationSource">The navigation source we are going to write resources for.</param>
            /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="writerSettings">The <see cref="ODataMessageWriterSettings"/> The settings of the writer.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            /// <param name="isUndeclared">true if the resource is for an undeclared property</param>
            internal JsonLightDeletedResourceScope(
                ODataDeletedResource resource,
                ODataResourceSerializationInfo serializationInfo,
                IEdmNavigationSource navigationSource,
                IEdmEntityType resourceType,
                bool skipWriting,
                ODataMessageWriterSettings writerSettings,
                SelectedPropertiesNode selectedProperties,
                ODataUri odataUri,
                bool isUndeclared)
                : base(resource, serializationInfo, navigationSource, resourceType, writerSettings, selectedProperties, odataUri)
            {
                this.isUndeclared = isUndeclared;
            }

            /// <summary>
            /// Enumeration of JSON Light metadata property flags, used to keep track of which properties were already written.
            /// </summary>
            [Flags]
            private enum JsonLightEntryMetadataProperty
            {
                /// <summary>The odata.editLink property.</summary>
                EditLink = 0x1,

                /// <summary>The odata.readLink property.</summary>
                ReadLink = 0x2,

                /// <summary>The odata.mediaEditLink property.</summary>
                MediaEditLink = 0x4,

                /// <summary>The odata.mediaReadLink property.</summary>
                MediaReadLink = 0x8,

                /// <summary>The odata.mediaContentType property.</summary>
                MediaContentType = 0x10,

                /// <summary>The odata.mediaEtag property.</summary>
                MediaETag = 0x20,
            }

            /// <summary>
            /// The resource being written.
            /// </summary>
            ODataResourceBase IODataJsonLightWriterResourceState.Resource
            {
                get { return (ODataResourceBase)this.Item; }
            }

            /// <summary>
            /// true if the resource set represents a complex property or a singleton navigation property that is undeclared, false otherwise.
            /// </summary>
            public bool IsUndeclared
            {
                get { return isUndeclared; }
            }

            /// <summary>
            /// Flag which indicates that the odata.editLink metadata property has been written.
            /// </summary>
            public bool EditLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.EditLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.EditLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.readLink metadata property has been written.
            /// </summary>
            public bool ReadLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.ReadLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.ReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaEditLink metadata property has been written.
            /// </summary>
            public bool MediaEditLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaEditLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaEditLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaReadLink metadata property has been written.
            /// </summary>
            public bool MediaReadLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaReadLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaContentType metadata property has been written.
            /// </summary>
            public bool MediaContentTypeWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaContentType);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaContentType);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaEtag metadata property has been written.
            /// </summary>
            public bool MediaETagWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaETag);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaETag);
                }
            }

            /// <summary>
            /// Marks the <paramref name="jsonLightMetadataProperty"/> as written in this resource scope.
            /// </summary>
            /// <param name="jsonLightMetadataProperty">The metadata property which was written.</param>
            private void SetWrittenMetadataProperty(JsonLightEntryMetadataProperty jsonLightMetadataProperty)
            {
                Debug.Assert(!this.IsMetadataPropertyWritten(jsonLightMetadataProperty), "Can't write the same metadata property twice.");
                this.alreadyWrittenMetadataProperties |= (int)jsonLightMetadataProperty;
            }

            /// <summary>
            /// Determines if the <paramref name="jsonLightMetadataProperty"/> was already written for this resource scope.
            /// </summary>
            /// <param name="jsonLightMetadataProperty">The metadata property to test for.</param>
            /// <returns>true if the <paramref name="jsonLightMetadataProperty"/> was already written for this resource scope; false otherwise.</returns>
            private bool IsMetadataPropertyWritten(JsonLightEntryMetadataProperty jsonLightMetadataProperty)
            {
                return (this.alreadyWrittenMetadataProperties & (int)jsonLightMetadataProperty) == (int)jsonLightMetadataProperty;
            }
        }

        /// <summary>
        /// A scope for a property in JSON Light writer.
        /// </summary>
        private sealed class JsonLightPropertyScope : PropertyInfoScope
        {
            /// <summary>
            /// Constructor to create a new property scope.
            /// </summary>
            /// <param name="property">The property for the new scope.</param>
            /// <param name="navigationSource">The navigation source.</param>
            /// <param name="resourceType">The structured type for the resource containing the property to be written.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            internal JsonLightPropertyScope(ODataPropertyInfo property, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(property, navigationSource, resourceType, selectedProperties, odataUri)
            {
            }
        }

        /// <summary>
        /// A scope for a delta link in JSON Light writer.
        /// </summary>
        private sealed class JsonLightDeltaLinkScope : DeltaLinkScope
        {
            /// <summary>
            /// Constructor to create a new delta link scope.
            /// </summary>
            /// <param name="state">The writer state of this scope.</param>
            /// <param name="link">The link for the new scope.</param>
            /// <param name="serializationInfo">The serialization info for the current resource.</param>
            /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
            /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            public JsonLightDeltaLinkScope(WriterState state, ODataItem link, ODataResourceSerializationInfo serializationInfo, IEdmNavigationSource navigationSource, IEdmEntityType entityType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(state, link, serializationInfo, navigationSource, entityType, selectedProperties, odataUri)
            {
            }
        }

        /// <summary>
        /// A scope for a JSON lite resource set.
        /// </summary>
        private sealed class JsonLightDeltaResourceSetScope : DeltaResourceSetScope
        {
            /// <summary>true if the odata.nextLink was already written, false otherwise.</summary>
            private bool nextLinkWritten;

            /// <summary>true if the odata.deltaLink was already written, false otherwise.</summary>
            private bool deltaLinkWritten;

            /// <summary>
            /// Constructor to create a new resource set scope.
            /// </summary>
            /// <param name="resourceSet">The resource set for the new scope.</param>
            /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
            /// <param name="resourceType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            public JsonLightDeltaResourceSetScope(ODataDeltaResourceSet resourceSet, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(resourceSet, navigationSource, resourceType, selectedProperties, odataUri)
            {
            }

            /// <summary>
            /// true if the odata.nextLink annotation was already written, false otherwise.
            /// </summary>
            internal bool NextPageLinkWritten
            {
                get
                {
                    return this.nextLinkWritten;
                }

                set
                {
                    this.nextLinkWritten = value;
                }
            }

            /// <summary>
            /// true if the odata.deltaLink annotation was already written, false otherwise.
            /// </summary>
            internal bool DeltaLinkWritten
            {
                get
                {
                    return this.deltaLinkWritten;
                }

                set
                {
                    this.deltaLinkWritten = value;
                }
            }
        }

        /// <summary>
        /// A scope for a resource in JSON Light writer.
        /// </summary>
        private sealed class JsonLightResourceScope : ResourceScope, IODataJsonLightWriterResourceState
        {
            /// <summary>Bit field of the JSON Light metadata properties written so far.</summary>
            private int alreadyWrittenMetadataProperties;

            /// <summary>true if the resource set represents a complex property or a singleton navigation property that is undeclared, false otherwise.</summary>
            private bool isUndeclared;

            /// <summary>
            /// Constructor to create a new resource scope.
            /// </summary>
            /// <param name="resource">The resource for the new scope.</param>
            /// <param name="serializationInfo">The serialization info for the current resource.</param>
            /// <param name="navigationSource">The navigation source we are going to write resources for.</param>
            /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="writerSettings">The <see cref="ODataMessageWriterSettings"/> The settings of the writer.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            /// <param name="isUndeclared">true if the resource is for an undeclared property</param>
            internal JsonLightResourceScope(
                ODataResource resource,
                ODataResourceSerializationInfo serializationInfo,
                IEdmNavigationSource navigationSource,
                IEdmStructuredType resourceType,
                bool skipWriting,
                ODataMessageWriterSettings writerSettings,
                SelectedPropertiesNode selectedProperties,
                ODataUri odataUri,
                bool isUndeclared)
                : base(resource, serializationInfo, navigationSource, resourceType, skipWriting, writerSettings, selectedProperties, odataUri)
            {
                this.isUndeclared = isUndeclared;
            }

            /// <summary>
            /// Enumeration of JSON Light metadata property flags, used to keep track of which properties were already written.
            /// </summary>
            [Flags]
            private enum JsonLightEntryMetadataProperty
            {
                /// <summary>The odata.editLink property.</summary>
                EditLink = 0x1,

                /// <summary>The odata.readLink property.</summary>
                ReadLink = 0x2,

                /// <summary>The odata.mediaEditLink property.</summary>
                MediaEditLink = 0x4,

                /// <summary>The odata.mediaReadLink property.</summary>
                MediaReadLink = 0x8,

                /// <summary>The odata.mediaContentType property.</summary>
                MediaContentType = 0x10,

                /// <summary>The odata.mediaEtag property.</summary>
                MediaETag = 0x20,
            }

            /// <summary>
            /// The resource being written.
            /// </summary>
            ODataResourceBase IODataJsonLightWriterResourceState.Resource
            {
                get { return (ODataResourceBase)this.Item; }
            }

            /// <summary>
            /// true if the resource set represents a complex property or a singleton navigation property that is undeclared, false otherwise.
            /// </summary>
            public bool IsUndeclared
            {
                get { return isUndeclared; }
                set { isUndeclared = value; }
            }

            /// <summary>
            /// Flag which indicates that the odata.editLink metadata property has been written.
            /// </summary>
            public bool EditLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.EditLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.EditLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.readLink metadata property has been written.
            /// </summary>
            public bool ReadLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.ReadLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.ReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaEditLink metadata property has been written.
            /// </summary>
            public bool MediaEditLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaEditLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaEditLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaReadLink metadata property has been written.
            /// </summary>
            public bool MediaReadLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaReadLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaContentType metadata property has been written.
            /// </summary>
            public bool MediaContentTypeWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaContentType);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaContentType);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaEtag metadata property has been written.
            /// </summary>
            public bool MediaETagWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaETag);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaETag);
                }
            }

            /// <summary>
            /// Marks the <paramref name="jsonLightMetadataProperty"/> as written in this resource scope.
            /// </summary>
            /// <param name="jsonLightMetadataProperty">The metadata property which was written.</param>
            private void SetWrittenMetadataProperty(JsonLightEntryMetadataProperty jsonLightMetadataProperty)
            {
                Debug.Assert(!this.IsMetadataPropertyWritten(jsonLightMetadataProperty), "Can't write the same metadata property twice.");
                this.alreadyWrittenMetadataProperties |= (int)jsonLightMetadataProperty;
            }

            /// <summary>
            /// Determines if the <paramref name="jsonLightMetadataProperty"/> was already written for this resource scope.
            /// </summary>
            /// <param name="jsonLightMetadataProperty">The metadata property to test for.</param>
            /// <returns>true if the <paramref name="jsonLightMetadataProperty"/> was already written for this resource scope; false otherwise.</returns>
            private bool IsMetadataPropertyWritten(JsonLightEntryMetadataProperty jsonLightMetadataProperty)
            {
                return (this.alreadyWrittenMetadataProperties & (int)jsonLightMetadataProperty) == (int)jsonLightMetadataProperty;
            }
        }

        /// <summary>
        /// A scope for a JSON Light nested resource info.
        /// </summary>
        private sealed class JsonLightNestedResourceInfoScope : NestedResourceInfoScope
        {
            /// <summary>true if we have already written an entity reference link for this nested resource info in requests; otherwise false.</summary>
            private bool entityReferenceLinkWritten;

            /// <summary>true if we have written at least one resource set for this nested resource info in requests; otherwise false.</summary>
            private bool resourceSetWritten;

            /// <summary>
            /// Constructor to create a new JSON Light nested resource info scope.
            /// </summary>
            /// <param name="writerState">The writer state for the new scope.</param>
            /// <param name="navLink">The nested resource info for the new scope.</param>
            /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
            /// <param name="itemType">The type for the items in the resource set to be written (or null if the navigationSource base type should be used).</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            internal JsonLightNestedResourceInfoScope(WriterState writerState, ODataNestedResourceInfo navLink, IEdmNavigationSource navigationSource, IEdmType itemType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(writerState, navLink, navigationSource, itemType, skipWriting, selectedProperties, odataUri)
            {
            }

            /// <summary>
            /// true if we have already written an entity reference link for this navigation link in requests; otherwise false.
            /// </summary>
            internal bool EntityReferenceLinkWritten
            {
                get
                {
                    return this.entityReferenceLinkWritten;
                }

                set
                {
                    this.entityReferenceLinkWritten = value;
                }
            }

            /// <summary>
            /// true if we have written at least one resource set for this nested resource info in requests; otherwise false.
            /// </summary>
            internal bool ResourceSetWritten
            {
                get
                {
                    return this.resourceSetWritten;
                }

                set
                {
                    this.resourceSetWritten = value;
                }
            }

            /// <summary>
            /// Clones this JSON Light nested resource info scope and sets a new writer state.
            /// </summary>
            /// <param name="newWriterState">The writer state to set.</param>
            /// <returns>The cloned nested resource info scope with the specified writer state.</returns>
            internal override NestedResourceInfoScope Clone(WriterState newWriterState)
            {
                return new JsonLightNestedResourceInfoScope(newWriterState, (ODataNestedResourceInfo)this.Item, this.NavigationSource, this.ItemType, this.SkipWriting, this.SelectedProperties, this.ODataUri)
                {
                    EntityReferenceLinkWritten = this.entityReferenceLinkWritten,
                    ResourceSetWritten = this.resourceSetWritten,
                    DerivedTypeConstraints = this.DerivedTypeConstraints
                };
            }
        }
    }
}
