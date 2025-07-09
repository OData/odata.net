//---------------------------------------------------------------------
// <copyright file="ODataJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Implementation of the ODataWriter for the Json format.
    /// </summary>
    internal sealed class ODataJsonWriter : ODataWriterCore
    {
        /// <summary>
        /// The output context to write to.
        /// </summary>
        private readonly ODataJsonOutputContext jsonOutputContext;

        /// <summary>
        /// The Json resource and resource set serializer to use.
        /// </summary>
        private readonly ODataJsonResourceSerializer jsonResourceSerializer;

        /// <summary>
        /// The Json value serializer to use for primitive values in an untyped collection.
        /// </summary>
        private readonly ODataJsonValueSerializer jsonValueSerializer;

        /// <summary>
        /// The Json property serializer.
        /// </summary>
        private readonly ODataJsonPropertySerializer jsonPropertySerializer;

        /// <summary>
        /// True if the writer was created for writing a parameter; false otherwise.
        /// </summary>
        private readonly bool writingParameter;

        /// <summary>
        /// The underlying JSON writer.
        /// </summary>
        private readonly IJsonWriter jsonWriter;

        /// <summary>
        /// OData annotation writer.
        /// </summary>
        private readonly JsonODataAnnotationWriter odataAnnotationWriter;

        /// <summary>
        /// OData instance annotation writer
        /// </summary>
        private readonly JsonInstanceAnnotationWriter instanceAnnotationWriter;

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
        /// <param name="jsonOutputContext">The output context to write to.</param>
        /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
        /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <param name="writingResourceSet">true if the writer is created for writing a resource set; false when it is created for writing a resource.</param>
        /// <param name="writingParameter">true if the writer is created for writing a parameter; false otherwise.</param>
        /// <param name="writingDelta">True if the writer is created for writing delta payload; false otherwise.</param>
        /// <param name="listener">If not null, the writer will notify the implementer of the interface of relevant state changes in the writer.</param>
        internal ODataJsonWriter(
            ODataJsonOutputContext jsonOutputContext,
            IEdmNavigationSource navigationSource,
            IEdmStructuredType resourceType,
            bool writingResourceSet,
            bool writingParameter = false,
            bool writingDelta = false,
            IODataReaderWriterListener listener = null)
            : base(jsonOutputContext, navigationSource, resourceType, writingResourceSet, writingDelta, listener)
        {
            Debug.Assert(jsonOutputContext != null, "jsonOutputContext != null");

            this.jsonOutputContext = jsonOutputContext;
            this.jsonResourceSerializer = new ODataJsonResourceSerializer(this.jsonOutputContext);
            this.jsonValueSerializer = new ODataJsonValueSerializer(this.jsonOutputContext);
            this.jsonPropertySerializer = new ODataJsonPropertySerializer(this.jsonOutputContext);

            this.writingParameter = writingParameter;
            this.jsonWriter = this.jsonOutputContext.JsonWriter;
            this.odataAnnotationWriter = this.jsonValueSerializer.ODataAnnotationWriter;
            this.instanceAnnotationWriter = this.jsonValueSerializer.InstanceAnnotationWriter;
            this.writingResponse = this.jsonOutputContext.WritingResponse;
            this.messageWriterSettings = this.jsonOutputContext.MessageWriterSettings;
        }

        /// <summary>
        /// Returns the current JsonResourceScope.
        /// </summary>
        private JsonResourceScope CurrentResourceScope
        {
            get
            {
                JsonResourceScope currentJsonResourceScope = this.CurrentScope as JsonResourceScope;
                Debug.Assert(currentJsonResourceScope != null, "Asking for JsonResourceScope when the current scope is not an JsonResourceScope.");
                return currentJsonResourceScope;
            }
        }

        /// <summary>
        /// Returns the current JsonDeletedResourceScope.
        /// </summary>
        private JsonDeletedResourceScope CurrentDeletedResourceScope
        {
            get
            {
                JsonDeletedResourceScope currentJsonDeletedResourceScope = this.CurrentScope as JsonDeletedResourceScope;
                Debug.Assert(currentJsonDeletedResourceScope != null, "Asking for JsonResourceScope when the current scope is not an JsonResourceScope.");
                return currentJsonDeletedResourceScope;
            }
        }

        /// <summary>
        /// Returns the current JsonDeltaLinkScope.
        /// </summary>
        private JsonDeltaLinkScope CurrentDeltaLinkScope
        {
            get
            {
                JsonDeltaLinkScope jsonDeltaLinkScope = this.CurrentScope as JsonDeltaLinkScope;
                Debug.Assert(jsonDeltaLinkScope != null, "Asking for JsonDeltaLinkScope when the current scope is not an JsonDeltaLinkScope.");
                return jsonDeltaLinkScope;
            }
        }

        /// <summary>
        /// Returns the current JsonResourceSetScope.
        /// </summary>
        private JsonResourceSetScope CurrentResourceSetScope
        {
            get
            {
                JsonResourceSetScope currentJsonResourceSetScope = this.CurrentScope as JsonResourceSetScope;
                Debug.Assert(currentJsonResourceSetScope != null, "Asking for JsonResourceSetScope when the current scope is not a JsonResourceSetScope.");
                return currentJsonResourceSetScope;
            }
        }

        /// <summary>
        /// Returns the current JsonDeltaResourceSetScope.
        /// </summary>
        private JsonDeltaResourceSetScope CurrentDeltaResourceSetScope
        {
            get
            {
                JsonDeltaResourceSetScope currentJsonDeltaResourceSetScope = this.CurrentScope as JsonDeltaResourceSetScope;
                Debug.Assert(currentJsonDeltaResourceSetScope != null, "Asking for JsonDeltaResourceSetScope when the current scope is not a JsonDeltaResourceSetScope.");
                return currentJsonDeltaResourceSetScope;
            }
        }

        /// <summary>
        /// Check if the object has been disposed; called from all public API methods. Throws an ObjectDisposedException if the object
        /// has already been disposed.
        /// </summary>
        protected override void VerifyNotDisposed()
        {
            this.jsonOutputContext.VerifyNotDisposed();
        }

        /// <summary>
        /// Flush the output.
        /// </summary>
        protected override void FlushSynchronously()
        {
            this.jsonOutputContext.Flush();
        }

        /// <summary>
        /// Flush the output.
        /// </summary>
        /// <returns>Task representing the pending flush operation.</returns>
        protected override Task FlushAsynchronously()
        {
            return this.jsonOutputContext.FlushAsync();
        }

        /// <summary>
        /// Starts writing a payload (called exactly once before anything else)
        /// </summary>
        protected override void StartPayload()
        {
            this.jsonResourceSerializer.WritePayloadStart();
        }

        /// <summary>
        /// Ends writing a payload (called exactly once after everything else in case of success)
        /// </summary>
        protected override void EndPayload()
        {
            this.jsonResourceSerializer.WritePayloadEnd();
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
            if (this.jsonOutputContext.MetadataLevel is JsonNoMetadataLevel)
            {
                // 1. NoMetadata level: always enable its NullResourceMetadataBuilder
                InnerPrepareResourceForWriteStart(resource, typeContext, selectedProperties);
            }
            else
            {
                // 2. Minimal/Full Metadata level: Use ODataConventionalEntityMetadataBuilder for entity, ODataConventionalResourceMetadataBuilder for other cases.
                if (this.jsonOutputContext.Model.IsUserModel() || resourceScope.SerializationInfo != null)
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
                if (this.jsonOutputContext.Model.IsUserModel() || resourceScope.SerializationInfo != null || this.jsonOutputContext.MetadataLevel is JsonNoMetadataLevel)
                {
                    ODataResourceTypeContext typeContext = resourceScope.GetOrCreateTypeContext(writingResponse);
                    InnerPrepareResourceForWriteStart(deletedResource, typeContext, selectedProperties);
                }
            }
            else if (deletedResource.Id == null && this.jsonOutputContext.Model.IsUserModel())
            {
                // Create an instance of the conventional UriBuilder and build id from key values
                IEdmEntityType entityType = resourceScope.ResourceType as IEdmEntityType;
                if (deletedResource.SerializationInfo != null)
                {
                    string entityTypeName = deletedResource.SerializationInfo.ExpectedTypeName;
                    if (!String.IsNullOrEmpty(entityTypeName))
                    {
                        entityType = this.jsonOutputContext.Model.FindType(entityTypeName) as IEdmEntityType ?? entityType;
                    }
                }

                Debug.Assert(entityType != null, "No entity type specified in resourceScope or serializationInfo.");

                ODataConventionalUriBuilder uriBuilder = new ODataConventionalUriBuilder(
                    new Uri(this.messageWriterSettings.MetadataDocumentUri, "./"),
                    this.jsonOutputContext.MessageWriterSettings.EnableWritingKeyAsSegment ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses
                    );

                Uri uri = uriBuilder.BuildBaseUri();
                uri = uriBuilder.BuildEntitySetUri(uri, resourceScope.NavigationSource.Name);
                uri = uriBuilder.BuildEntityInstanceUri(uri, ComputeKeyProperties(deletedResource, entityType, this.jsonOutputContext.Model), entityType.FullTypeName());

                deletedResource.Id = uri;
            }
        }

        internal static ICollection<KeyValuePair<string, object>> ComputeKeyProperties(ODataDeletedResource resource, IEdmEntityType entityType, IEdmModel model)
        {
            Debug.Assert(entityType.Key().Any(), "actual entity type has no keys defined");

            ICollection<KeyValuePair<string, object>> computedKeyProperties = new List<KeyValuePair<string, object>>();
            foreach (IEdmStructuralProperty edmKeyProperty in entityType.Key())
            {
                foreach (ODataPropertyInfo propertyInfo in resource.NonComputedProperties)
                {
                    if (propertyInfo is not ODataProperty property)
                    {
                        continue;
                    }

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
                this.jsonPropertySerializer.WriteProperty(
                propertyWithValue,
                scope.ResourceType,
                false /*isTopLevel*/,
                this.DuplicatePropertyNameChecker,
                resource.MetadataBuilder);
            }
            else
            {
                this.jsonPropertySerializer.WritePropertyInfo(
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

            JsonResourceScope resourceScope = this.CurrentResourceScope;

            if (this.IsTopLevel && !(this.jsonOutputContext.MetadataLevel is JsonNoMetadataLevel))
            {
                ODataContextUrlInfo contextUriInfo = this.jsonResourceSerializer.WriteResourceContextUri(
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
                    this.jsonResourceSerializer.WriteDeltaContextUri(
                        this.CurrentResourceScope.GetOrCreateTypeContext(true), ODataDeltaKind.Resource,
                        deltaResourceSetScope.ContextUriInfo);
                }
            }

            // Write the metadata
            this.jsonResourceSerializer.WriteResourceStartMetadataProperties(resourceScope);
            this.jsonResourceSerializer.WriteResourceMetadataProperties(resourceScope);

            this.jsonOutputContext.PropertyCacheHandler.SetCurrentResourceScopeLevel(this.ScopeLevel);

            // Write custom instance annotations
            this.instanceAnnotationWriter.WriteInstanceAnnotations(resource.InstanceAnnotations, resourceScope.InstanceAnnotationWriteTracker);

            this.jsonResourceSerializer.JsonValueSerializer.AssertRecursionDepthIsZero();
            if (resource.NonComputedProperties != null)
            {
                this.jsonResourceSerializer.WriteProperties(
                    this.ResourceType,
                    resource.NonComputedProperties,
                    false /* isComplexValue */,
                    this.DuplicatePropertyNameChecker,
                    resource.MetadataBuilder);
                this.jsonResourceSerializer.JsonValueSerializer.AssertRecursionDepthIsZero();
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
            JsonResourceScope resourceScope = this.CurrentResourceScope;

            this.jsonResourceSerializer.WriteResourceMetadataProperties(resourceScope);

            // Write custom instance annotations
            this.instanceAnnotationWriter.WriteInstanceAnnotations(resource.InstanceAnnotations, resourceScope.InstanceAnnotationWriteTracker);

            this.jsonResourceSerializer.WriteResourceEndMetadataProperties(resourceScope, resourceScope.DuplicatePropertyNameChecker);

            // Release the DuplicatePropertyNameChecker to the object pool since an instance is removed from the object pool each time we create a ResourceScope.
            this.jsonResourceSerializer.ReturnDuplicatePropertyNameChecker(resourceScope.DuplicatePropertyNameChecker);

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
            // JsonResourceScope resourceScope = this.CurrentResourceScope;

            // this.jsonResourceSerializer.WriteResourceMetadataProperties(resourceScope);

            // Write custom instance annotations
            // this.instanceAnnotationWriter.WriteInstanceAnnotations(deletedResource.InstanceAnnotations, resourceScope.InstanceAnnotationWriteTracker);

            // this.jsonResourceSerializer.WriteResourceEndMetadataProperties(resourceScope, resourceScope.DuplicatePropertyNameChecker);

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
                this.jsonResourceSerializer.WriteResourceSetContextUri(this.CurrentResourceSetScope.GetOrCreateTypeContext(this.writingResponse));

                if (this.writingResponse)
                {
                    // write "odata.actions" metadata
                    IEnumerable<ODataAction> actions = resourceSet.Actions;
                    if (actions != null && actions.Any())
                    {
                        this.jsonResourceSerializer.WriteOperations(actions.Cast<ODataOperation>(), /*isAction*/ true);
                    }

                    // write "odata.functions" metadata
                    IEnumerable<ODataFunction> functions = resourceSet.Functions;
                    if (functions != null && functions.Any())
                    {
                        this.jsonResourceSerializer.WriteOperations(functions.Cast<ODataOperation>(), /*isAction*/ false);
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
                bool isUndeclared = (this.CurrentScope as JsonResourceSetScope).IsUndeclared;
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
                    this.jsonResourceSerializer.WriteResourceSetStartMetadataProperties(resourceSet, propertyName, expectedResourceTypeName, isUndeclared);

                    // And then write the property name to start the value.
                    this.jsonWriter.WriteName(propertyName);

                    // Start array which will hold the entries in the resource set.
                    this.jsonWriter.StartArrayScope();
                }
                else
                {
                    JsonNestedResourceInfoScope navigationLinkScope = (JsonNestedResourceInfoScope)this.ParentNestedResourceInfoScope;
                    if (!navigationLinkScope.ResourceSetWritten)
                    {
                        // Close the entity reference link array (if written)
                        if (navigationLinkScope.EntityReferenceLinkWritten)
                        {
                            this.jsonWriter.EndArrayScope();
                        }

                        // Write the odata type.
                        this.jsonResourceSerializer.WriteResourceSetStartMetadataProperties(resourceSet, propertyName, expectedResourceTypeName, isUndeclared);

                        // And then write the property name to start the value.
                        this.jsonWriter.WriteName(propertyName);

                        // Start array which will hold the entries in the resource set.
                        this.jsonWriter.StartArrayScope();

                        navigationLinkScope.ResourceSetWritten = true;
                    }
                }
            }

            this.jsonOutputContext.PropertyCacheHandler.EnterResourceSetScope(this.CurrentResourceSetScope.ResourceType, this.ScopeLevel);
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

            this.jsonOutputContext.PropertyCacheHandler.LeaveResourceSetScope();
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
                this.CurrentDeltaResourceSetScope.ContextUriInfo = this.jsonResourceSerializer.WriteDeltaContextUri(
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
                this.jsonWriter.WritePropertyAnnotationName(propertyName, ODataJsonConstants.ODataDeltaPropertyName);

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
            DeletedResourceScope resourceScope = this.CurrentDeletedResourceScope;
            Debug.Assert(resourceScope != null, "Writing deleted entry and scope is not DeltaResourceScope");
            ODataNestedResourceInfo parentNavLink = this.ParentNestedResourceInfo;
            if (parentNavLink != null)
            {
                // Writing a nested deleted resource
                if (this.Version == null || this.Version < ODataVersion.V401)
                {
                    throw new ODataException(SRResources.ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry);
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
                Debug.Assert(this.ParentScope.State == WriterState.Start || deltaResourceSetScope != null, "Writing child of delta set and parent scope is not DeltaResourceSetScope");

                this.jsonWriter.StartObjectScope();

                if (this.Version == null || this.Version < ODataVersion.V401)
                {
                    // Write ContextUrl
                    this.jsonResourceSerializer.WriteDeltaContextUri(
                            this.CurrentDeletedResourceScope.GetOrCreateTypeContext(this.writingResponse),
                            ODataDeltaKind.DeletedEntry, deltaResourceSetScope?.ContextUriInfo);
                    this.WriteV4DeletedEntryContents(resource);
                }
                else
                {
                    // Only write ContextUrl for V4.01 deleted resource if it is in a top level delta
                    // resource set and comes from a different entity set
                    string expectedNavigationSource =
                        deltaResourceSetScope?.NavigationSource == null ? null : deltaResourceSetScope.NavigationSource.Name;
                    string currentNavigationSource =
                        resource.SerializationInfo != null ? resource.SerializationInfo.NavigationSourceName :
                        resourceScope.NavigationSource?.Name;

                    if (String.IsNullOrEmpty(currentNavigationSource) || currentNavigationSource != expectedNavigationSource)
                    {
                        Debug.Assert(this.ScopeLevel <= 3, "Writing a nested deleted resource of the wrong type should already have been caught.");

                        // We are writing a deleted resource in a top level delta resource set
                        // from a different entity set, so include the context Url
                        this.jsonResourceSerializer.WriteDeltaContextUri(
                                this.CurrentDeletedResourceScope.GetOrCreateTypeContext(this.writingResponse),
                                ODataDeltaKind.DeletedEntry, deltaResourceSetScope?.ContextUriInfo);
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
                this.jsonValueSerializer.WriteNullValue();
            }
            else
            {
                this.jsonValueSerializer.WritePrimitiveValue(primitiveValue.Value, /*expectedType*/ null);
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
            if (this.jsonWriter == null)
            {
                this.jsonOutputContext.BinaryValueStream = new MemoryStream();
                stream = this.jsonOutputContext.BinaryValueStream;
            }
            else
            {
                stream = this.jsonWriter.StartStreamValueScope();
            }

            return stream;
        }

        /// <summary>
        /// Finish writing a stream value.
        /// </summary>
        protected sealed override void EndBinaryStream()
        {
            if (this.jsonWriter == null)
            {
                this.jsonWriter.WriteValue(this.jsonOutputContext.BinaryValueStream.ToArray());
                this.jsonOutputContext.BinaryValueStream.Flush();
                this.jsonOutputContext.BinaryValueStream.Dispose();
                this.jsonOutputContext.BinaryValueStream = null;
            }
            else
            {
                this.jsonWriter.EndStreamValueScope();
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

            string contentType = "text/plain";
            ODataStreamPropertyInfo streamInfo = property as ODataStreamPropertyInfo;
            if (streamInfo != null && streamInfo.ContentType != null)
            {
                contentType = streamInfo.ContentType;
            }

            return this.jsonWriter.StartTextWriterValueScope(contentType);
        }

        /// <summary>
        /// Finish writing a text value.
        /// </summary>
        protected sealed override void EndTextWriter()
        {
            this.jsonWriter.EndTextWriterValueScope();
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
            this.jsonResourceSerializer.WriteNavigationLinkMetadata(nestedResourceInfo, this.DuplicatePropertyNameChecker, count: true);
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
                    && this.messageWriterSettings.LibraryCompatibility.HasFlag(ODataLibraryCompatibility.WriteODataContextAnnotationForNavProperty)
                    && this.messageWriterSettings.Version < ODataVersion.V401)
                {
                    ODataContextUrlInfo info = ODataContextUrlInfo.Create(
                                                this.CurrentScope.NavigationSource,
                                                this.CurrentScope.ResourceType.FullTypeName(),
                                                containedEntitySet.NavigationProperty.Type.TypeKind() != EdmTypeKind.Collection,
                                                this.CurrentScope.ODataUri,
                                                this.messageWriterSettings.Version ?? ODataVersion.V4);

                    this.jsonResourceSerializer.WriteNestedResourceInfoContextUrl(nestedResourceInfo, info);
                }

                // Write the nested resource info metadata first. The rest is written by the content resource or resource set.
                this.jsonResourceSerializer.WriteNavigationLinkMetadata(nestedResourceInfo, this.DuplicatePropertyNameChecker, count: false);
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

            JsonNestedResourceInfoScope navigationLinkScope = (JsonNestedResourceInfoScope)this.CurrentScope;

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

            // In Json, we can only write entity reference links at the beginning of a navigation link in requests;
            // once we wrote a resource set, entity reference links are not allowed anymore (we require all the entity reference
            // link to come first because of the grouping in the Json wire format).
            JsonNestedResourceInfoScope nestedResourceScope = this.CurrentScope as JsonNestedResourceInfoScope;
            if (nestedResourceScope == null)
            {
                nestedResourceScope = this.ParentNestedResourceInfoScope as JsonNestedResourceInfoScope;
            }

            if (nestedResourceScope.ResourceSetWritten)
            {
                throw new ODataException(SRResources.ODataJsonWriter_EntityReferenceLinkAfterResourceSetInRequest);
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
                this.jsonWriter.WriteValue(this.jsonResourceSerializer.UriToString(entityReferenceLink.Url));
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
            in ODataUriSlim odataUri,
            bool isUndeclared)
        {
            return new JsonResourceSetScope(resourceSet, navigationSource, itemType, skipWriting, selectedProperties, odataUri, isUndeclared);
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
            in ODataUriSlim odataUri,
            bool isUndeclared)
        {
            return new JsonDeltaResourceSetScope(deltaResourceSet, navigationSource, resourceType, selectedProperties, odataUri);
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
        protected override DeletedResourceScope CreateDeletedResourceScope(ODataDeletedResource deltaResource, IEdmNavigationSource navigationSource, IEdmEntityType resourceType, bool skipWriting, SelectedPropertiesNode selectedProperties, in ODataUriSlim odataUri, bool isUndeclared)
        {
            return new JsonDeletedResourceScope(
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
        protected override PropertyInfoScope CreatePropertyInfoScope(ODataPropertyInfo property, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, SelectedPropertiesNode selectedProperties, in ODataUriSlim odataUri)
        {
            return new JsonPropertyScope(property, navigationSource, resourceType, selectedProperties, odataUri);
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
        protected override DeltaLinkScope CreateDeltaLinkScope(ODataDeltaLinkBase link, IEdmNavigationSource navigationSource, IEdmEntityType entityType, SelectedPropertiesNode selectedProperties, in ODataUriSlim odataUri)
        {
            return new JsonDeltaLinkScope(
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
        protected override ResourceScope CreateResourceScope(ODataResource resource, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, bool skipWriting, SelectedPropertiesNode selectedProperties, in ODataUriSlim odataUri, bool isUndeclared)
        {
            return new JsonResourceScope(
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
        /// Creates a new Json nested resource info scope.
        /// </summary>
        /// <param name="writerState">The writer state for the new scope.</param>
        /// <param name="navLink">The nested resource info for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="itemType">The type for the items in the resource set to be written (or null if the navigationSource base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <returns>The newly created Json  nested resource info scope.</returns>
        protected override NestedResourceInfoScope CreateNestedResourceInfoScope(WriterState writerState, ODataNestedResourceInfo navLink, IEdmNavigationSource navigationSource, IEdmType itemType, bool skipWriting, SelectedPropertiesNode selectedProperties, in ODataUriSlim odataUri)
        {
            Debug.Assert(this.CurrentScope != null, "Creating a nested resource info scope with a null parent scope.");
            return new JsonNestedResourceInfoScope(writerState, navLink, navigationSource, itemType, skipWriting, selectedProperties, odataUri, this.CurrentScope);
        }

        /// <summary>
        /// Asynchronously starts writing a payload (called exactly once before anything else)
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task StartPayloadAsync()
        {
            return this.jsonResourceSerializer.WritePayloadStartAsync();
        }

        /// <summary>
        /// Asynchronously ends writing a payload (called exactly once after everything else in case of success)
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task EndPayloadAsync()
        {
            return this.jsonResourceSerializer.WritePayloadEndAsync();
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
                        await this.odataAnnotationWriter.WriteODataTypePropertyAnnotationAsync(
                            parentNavLink.Name,
                            parentNavLink.TypeAnnotation.TypeName).ConfigureAwait(false);
                    }

                    await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                        parentNavLink.GetInstanceAnnotations(),
                        parentNavLink.Name).ConfigureAwait(false);
                }

                // Write the property name of an expanded navigation property to start the value.
                await this.jsonWriter.WriteNameAsync(parentNavLink.Name)
                    .ConfigureAwait(false);
            }

            if (resource == null)
            {
                await this.jsonWriter.WriteValueAsync((string)null)
                    .ConfigureAwait(false);
                return;
            }

            // Write just the object start, nothing else, since we might not have complete information yet
            await this.jsonWriter.StartObjectScopeAsync()
                .ConfigureAwait(false);

            JsonResourceScope resourceScope = this.CurrentResourceScope;

            if (this.IsTopLevel && !(this.jsonOutputContext.MetadataLevel is JsonNoMetadataLevel))
            {
                ODataContextUrlInfo contextUriInfo = await this.jsonResourceSerializer.WriteResourceContextUriAsync(
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
                    await this.jsonResourceSerializer.WriteDeltaContextUriAsync(
                        this.CurrentResourceScope.GetOrCreateTypeContext(true),
                        ODataDeltaKind.Resource,
                        deltaResourceSetScope.ContextUriInfo).ConfigureAwait(false);
                }
            }

            // Write the metadata
            await this.jsonResourceSerializer.WriteResourceStartMetadataPropertiesAsync(resourceScope)
                .ConfigureAwait(false);
            await this.jsonResourceSerializer.WriteResourceMetadataPropertiesAsync(resourceScope)
                .ConfigureAwait(false);

            this.jsonOutputContext.PropertyCacheHandler.SetCurrentResourceScopeLevel(this.ScopeLevel);

            // Write custom instance annotations
            await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                resource.InstanceAnnotations,
                resourceScope.InstanceAnnotationWriteTracker).ConfigureAwait(false);

            this.jsonResourceSerializer.JsonValueSerializer.AssertRecursionDepthIsZero();
            if (resource.NonComputedProperties != null)
            {
                await this.jsonResourceSerializer.WritePropertiesAsync(
                    this.ResourceType,
                    resource.NonComputedProperties,
                    false /* isComplexValue */,
                    this.DuplicatePropertyNameChecker,
                    resource.MetadataBuilder).ConfigureAwait(false);
                this.jsonResourceSerializer.JsonValueSerializer.AssertRecursionDepthIsZero();
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
            JsonResourceScope resourceScope = this.CurrentResourceScope;

            await this.jsonResourceSerializer.WriteResourceMetadataPropertiesAsync(resourceScope)
                .ConfigureAwait(false);

            // Write custom instance annotations
            await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                resource.InstanceAnnotations,
                resourceScope.InstanceAnnotationWriteTracker).ConfigureAwait(false);

            await this.jsonResourceSerializer.WriteResourceEndMetadataPropertiesAsync(
                resourceScope,
                resourceScope.DuplicatePropertyNameChecker).ConfigureAwait(false);

            // Release the DuplicatePropertyNameChecker to the object pool since an instance is removed from the object pool each time we create a ResourceScope.
            this.jsonResourceSerializer.ReturnDuplicatePropertyNameChecker(resourceScope.DuplicatePropertyNameChecker);

            // Close the object scope
            await this.jsonWriter.EndObjectScopeAsync()
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
                return this.jsonPropertySerializer.WritePropertyAsync(
                    propertyWithValue,
                    scope.ResourceType,
                    false /*isTopLevel*/,
                    this.DuplicatePropertyNameChecker,
                    resource.MetadataBuilder);
            }
            else
            {
                return this.jsonPropertySerializer.WritePropertyInfoAsync(
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
                await this.jsonWriter.StartArrayScopeAsync()
                    .ConfigureAwait(false);
            }
            else if (this.ParentNestedResourceInfo == null)
            {
                // Top-level resource set.
                // "{"
                await this.jsonWriter.StartObjectScopeAsync()
                    .ConfigureAwait(false);

                // @odata.context
                await this.jsonResourceSerializer.WriteResourceSetContextUriAsync(
                    this.CurrentResourceSetScope.GetOrCreateTypeContext(
                        this.writingResponse)).ConfigureAwait(false);

                if (this.writingResponse)
                {
                    // Write "odata.actions" metadata
                    IEnumerable<ODataAction> actions = resourceSet.Actions;
                    if (actions != null && actions.Any())
                    {
                        await this.jsonResourceSerializer.WriteOperationsAsync(actions.Cast<ODataOperation>(), /*isAction*/ true)
                            .ConfigureAwait(false);
                    }

                    // Write "odata.functions" metadata
                    IEnumerable<ODataFunction> functions = resourceSet.Functions;
                    if (functions != null && functions.Any())
                    {
                        await this.jsonResourceSerializer.WriteOperationsAsync(functions.Cast<ODataOperation>(), /*isAction*/ false)
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
                await this.jsonWriter.WriteValuePropertyNameAsync()
                    .ConfigureAwait(false);

                // Start array which will hold the entries in the resource set.
                await this.jsonWriter.StartArrayScopeAsync()
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
                bool isUndeclared = (this.CurrentScope as JsonResourceSetScope).IsUndeclared;
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
                    await this.jsonResourceSerializer.WriteResourceSetStartMetadataPropertiesAsync(
                        resourceSet,
                        propertyName,
                        expectedResourceTypeName,
                        isUndeclared).ConfigureAwait(false);

                    // And then write the property name to start the value.
                    await this.jsonWriter.WriteNameAsync(propertyName)
                        .ConfigureAwait(false);

                    // Start array which will hold the entries in the resource set.
                    await this.jsonWriter.StartArrayScopeAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    JsonNestedResourceInfoScope navigationLinkScope = (JsonNestedResourceInfoScope)this.ParentNestedResourceInfoScope;
                    if (!navigationLinkScope.ResourceSetWritten)
                    {
                        // Close the entity reference link array (if written)
                        if (navigationLinkScope.EntityReferenceLinkWritten)
                        {
                            await this.jsonWriter.EndArrayScopeAsync()
                                .ConfigureAwait(false);
                        }

                        // Write the odata type.
                        await this.jsonResourceSerializer.WriteResourceSetStartMetadataPropertiesAsync(
                            resourceSet,
                            propertyName,
                            expectedResourceTypeName,
                            isUndeclared).ConfigureAwait(false);

                        // And then write the property name to start the value.
                        await this.jsonWriter.WriteNameAsync(propertyName)
                            .ConfigureAwait(false);

                        // Start array which will hold the entries in the resource set.
                        await this.jsonWriter.StartArrayScopeAsync()
                            .ConfigureAwait(false);

                        navigationLinkScope.ResourceSetWritten = true;
                    }
                }
            }

            this.jsonOutputContext.PropertyCacheHandler.EnterResourceSetScope(
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
                await this.jsonWriter.EndArrayScopeAsync()
                    .ConfigureAwait(false);
            }
            else if (this.ParentNestedResourceInfo == null)
            {
                // End the array which holds the entries in the resource set.
                await this.jsonWriter.EndArrayScopeAsync()
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
                await this.jsonWriter.EndObjectScopeAsync()
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
                    await this.jsonWriter.EndArrayScopeAsync()
                        .ConfigureAwait(false);

                    // Write the next link if it's available.
                    await this.WriteResourceSetNextLinkAsync(resourceSet.NextPageLink, propertyName)
                        .ConfigureAwait(false);
                }
            }

            this.jsonOutputContext.PropertyCacheHandler.LeaveResourceSetScope();
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
                await this.jsonWriter.StartObjectScopeAsync()
                    .ConfigureAwait(false);

                // Write ContextUrl
                this.CurrentDeltaResourceSetScope.ContextUriInfo = await this.jsonResourceSerializer.WriteDeltaContextUriAsync(
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
                await this.jsonWriter.WriteValuePropertyNameAsync()
                    .ConfigureAwait(false);
                await this.jsonWriter.StartArrayScopeAsync()
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
                await this.jsonWriter.WritePropertyAnnotationNameAsync(
                    propertyName,
                    ODataJsonConstants.ODataDeltaPropertyName).ConfigureAwait(false);

                // Start array which will hold the entries in the nested delta resource set.
                await this.jsonWriter.StartArrayScopeAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously finish writing a delta resource set.
        /// </summary>
        /// <param name="deltaResourceSet">The resource set to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task EndDeltaResourceSetAsync(ODataDeltaResourceSet deltaResourceSet)
        {
            Debug.Assert(deltaResourceSet != null, "deltaResourceSet != null");

            if (this.ParentNestedResourceInfo == null)
            {
                return EndDeltaResourceSetInnerAsync(deltaResourceSet);

                async Task EndDeltaResourceSetInnerAsync(ODataDeltaResourceSet innerDeltaResourceSet)
                {
                    // End the array which holds the entries in the resource set.
                    await this.jsonWriter.EndArrayScopeAsync()
                        .ConfigureAwait(false);

                    // Write custom instance annotations
                    await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                        innerDeltaResourceSet.InstanceAnnotations,
                        this.CurrentDeltaResourceSetScope.InstanceAnnotationWriteTracker).ConfigureAwait(false);

                    // Write the next link if it's available.
                    await this.WriteResourceSetNextLinkAsync(innerDeltaResourceSet.NextPageLink, propertyName: null)
                        .ConfigureAwait(false);

                    // Write the delta link if it's available.
                    await this.WriteResourceSetDeltaLinkAsync(innerDeltaResourceSet.DeltaLink)
                        .ConfigureAwait(false);

                    // Close the object wrapper.
                    await this.jsonWriter.EndObjectScopeAsync()
                        .ConfigureAwait(false);
                }
            }
            else
            {
                // End the array which holds the entries in the resource set.
                return this.jsonWriter.EndArrayScopeAsync();
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
            DeletedResourceScope resourceScope = this.CurrentDeletedResourceScope;
            Debug.Assert(resourceScope != null, "Writing deleted entry and scope is not DeltaResourceScope");

            ODataNestedResourceInfo parentNavLink = this.ParentNestedResourceInfo;
            if (parentNavLink != null)
            {
                // Writing a nested deleted resource
                if (this.Version == null || this.Version < ODataVersion.V401)
                {
                    throw new ODataException(SRResources.ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry);
                }
                else
                {
                    // Write the property name of an expanded navigation property to start the value.
                    await this.jsonWriter.WriteNameAsync(parentNavLink.Name)
                        .ConfigureAwait(false);
                    await this.jsonWriter.StartObjectScopeAsync()
                        .ConfigureAwait(false);
                    await this.WriteDeletedEntryContentsAsync(resource)
                        .ConfigureAwait(false);
                }
            }
            else
            {
                // Writing a deleted resource within an entity set
                DeltaResourceSetScope deltaResourceSetScope = this.ParentScope as DeltaResourceSetScope;
                Debug.Assert(this.ParentScope.State == WriterState.Start || deltaResourceSetScope != null, "Writing child of delta set and parent scope is not DeltaResourceSetScope");

                await this.jsonWriter.StartObjectScopeAsync()
                    .ConfigureAwait(false);

                if (this.Version == null || this.Version < ODataVersion.V401)
                {
                    // Write ContextUrl
                    await this.jsonResourceSerializer.WriteDeltaContextUriAsync(
                            this.CurrentDeletedResourceScope.GetOrCreateTypeContext(this.writingResponse),
                            ODataDeltaKind.DeletedEntry,
                            deltaResourceSetScope?.ContextUriInfo).ConfigureAwait(false);
                    await this.WriteV4DeletedEntryContentsAsync(resource)
                        .ConfigureAwait(false);
                }
                else
                {
                    // Only write ContextUrl for V4.01 deleted resource if it is in a top level delta
                    // resource set and comes from a different entity set
                    string expectedNavigationSource = deltaResourceSetScope?.NavigationSource?.Name;
                    string currentNavigationSource = resource.SerializationInfo?.NavigationSourceName ?? resourceScope.NavigationSource?.Name;

                    if (!String.IsNullOrEmpty(currentNavigationSource) && currentNavigationSource != expectedNavigationSource)
                    {
                        Debug.Assert(this.ScopeLevel <= 3, "Writing a nested deleted resource of the wrong type should already have been caught.");

                        // We are writing a deleted resource in a top level delta resource set
                        // from a different entity set, so include the context Url
                        await this.jsonResourceSerializer.WriteDeltaContextUriAsync(
                                this.CurrentDeletedResourceScope.GetOrCreateTypeContext(this.writingResponse),
                                ODataDeltaKind.DeletedEntry,
                                deltaResourceSetScope?.ContextUriInfo).ConfigureAwait(false);
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
            return this.jsonWriter.EndObjectScopeAsync();
        }

        /// <summary>
        /// Asynchronously start writing a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task StartDeltaLinkAsync(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");

            await this.jsonWriter.StartObjectScopeAsync()
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
            await this.jsonWriter.EndObjectScopeAsync()
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
                await this.jsonWriter.WriteNameAsync(property.Name)
                    .ConfigureAwait(false);
                await this.jsonWriter.FlushAsync()
                    .ConfigureAwait(false);
            }

            if (this.jsonWriter == null)
            {
                this.jsonOutputContext.BinaryValueStream = new MemoryStream();
                return this.jsonOutputContext.BinaryValueStream;
            }
            else
            {
                return await this.jsonWriter.StartStreamValueScopeAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously finish writing a stream value.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected sealed override Task EndBinaryStreamAsync()
        {
            if (this.jsonWriter == null)
            {
                return EndBinaryStreamInnerAsync();

                async Task EndBinaryStreamInnerAsync()
                {
                    await this.jsonWriter.WriteValueAsync(
                    this.jsonOutputContext.BinaryValueStream.ToArray()).ConfigureAwait(false);
                    await this.jsonOutputContext.BinaryValueStream.FlushAsync()
                        .ConfigureAwait(false);
                    this.jsonOutputContext.BinaryValueStream.Dispose();
                    this.jsonOutputContext.BinaryValueStream = null;
                }
            }
            else
            {
                return this.jsonWriter.EndStreamValueScopeAsync();
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
                await this.jsonWriter.WriteNameAsync(property.Name)
                    .ConfigureAwait(false);
                await this.jsonWriter.FlushAsync()
                    .ConfigureAwait(false);
            }

            string contentType = "text/plain";
            ODataStreamPropertyInfo streamInfo = property as ODataStreamPropertyInfo;
            if (streamInfo?.ContentType != null)
            {
                contentType = streamInfo.ContentType;
            }

            return await this.jsonWriter.StartTextWriterValueScopeAsync(contentType)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously finish writing a text value.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected sealed override Task EndTextWriterAsync()
        {
            return this.jsonWriter.EndTextWriterValueScopeAsync();
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
                await this.jsonWriter.WriteNameAsync(property.Name)
                    .ConfigureAwait(false);
            }

            if (primitiveValue == null)
            {
                await this.jsonValueSerializer.WriteNullValueAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                await this.jsonValueSerializer.WritePrimitiveValueAsync(primitiveValue.Value, /*expectedType*/ null)
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
            return this.jsonResourceSerializer.WriteNavigationLinkMetadataAsync(
                nestedResourceInfo,
                this.DuplicatePropertyNameChecker, count: true);
        }

        /// <summary>
        /// Asynchronously start writing the nested resource info with content.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task StartNestedResourceInfoWithContentAsync(ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(!string.IsNullOrEmpty(nestedResourceInfo.Name),
                "The nested resource info name should have been verified by now.");

            if (this.writingResponse)
            {
                return StartNestedResourceInfoWithContentInnerAsync(nestedResourceInfo);

                async Task StartNestedResourceInfoWithContentInnerAsync(ODataNestedResourceInfo innerNestedResourceInfo)
                {
                    // Write @odata.context annotation for navigation property
                    IEdmContainedEntitySet containedEntitySet = this.CurrentScope.NavigationSource as IEdmContainedEntitySet;
                    if (containedEntitySet != null
                        && this.messageWriterSettings.LibraryCompatibility.HasFlag(ODataLibraryCompatibility.WriteODataContextAnnotationForNavProperty)
                        && this.messageWriterSettings.Version < ODataVersion.V401)
                    {
                        ODataContextUrlInfo info = ODataContextUrlInfo.Create(
                            this.CurrentScope.NavigationSource,
                            this.CurrentScope.ResourceType.FullTypeName(),
                            containedEntitySet.NavigationProperty.Type.TypeKind() != EdmTypeKind.Collection,
                            this.CurrentScope.ODataUri,
                            this.messageWriterSettings.Version ?? ODataVersion.V4);

                        await this.jsonResourceSerializer.WriteNestedResourceInfoContextUrlAsync(innerNestedResourceInfo, info)
                            .ConfigureAwait(false);
                    }

                    // Write the nested resource info metadata first. The rest is written by the content resource or resource set.
                    await this.jsonResourceSerializer.WriteNavigationLinkMetadataAsync(
                        innerNestedResourceInfo,
                        this.DuplicatePropertyNameChecker, count: false).ConfigureAwait(false);
                }
            }
            else
            {
                this.WriterValidator.ValidateNestedResourceInfoHasCardinality(nestedResourceInfo);

                return TaskUtils.CompletedTask;
            }
        }

        /// <summary>
        /// Asynchronously finish writing nested resource info with content.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task EndNestedResourceInfoWithContentAsync(ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");

            // If we wrote entity reference links for a collection navigation property but no
            // resource set afterwards, we have to now close the array of links.
            if (!this.writingResponse)
            {
                return EndNestedResourceInfoWithContentInnerAsync(nestedResourceInfo);

                async Task EndNestedResourceInfoWithContentInnerAsync(ODataNestedResourceInfo innerNestedResourceInfo)
                {
                    JsonNestedResourceInfoScope navigationLinkScope = (JsonNestedResourceInfoScope)this.CurrentScope;

                    if (navigationLinkScope.EntityReferenceLinkWritten && !navigationLinkScope.ResourceSetWritten && innerNestedResourceInfo.IsCollection.Value)
                    {
                        await this.jsonWriter.EndArrayScopeAsync()
                            .ConfigureAwait(false);
                    }

                    // In requests, the nested resource info may have multiple entries in multiple resource sets in it; if we
                    // wrote at least one resource set, close the resulting array here.
                    if (navigationLinkScope.ResourceSetWritten)
                    {
                        Debug.Assert(innerNestedResourceInfo.IsCollection == null || innerNestedResourceInfo.IsCollection.Value, "nestedResourceInfo.IsCollection.Value");
                        await this.jsonWriter.EndArrayScopeAsync()
                            .ConfigureAwait(false);
                    }
                }
            }

            return TaskUtils.CompletedTask;
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

            // In Json, we can only write entity reference links at the beginning of a navigation link in requests;
            // once we write a resource set, entity reference links are not allowed anymore (we require all the entity reference
            // link to come first because of the grouping in the Json wire format).
            JsonNestedResourceInfoScope nestedResourceScope = this.CurrentScope as JsonNestedResourceInfoScope;
            if (nestedResourceScope == null)
            {
                nestedResourceScope = this.ParentNestedResourceInfoScope as JsonNestedResourceInfoScope;
            }

            if (nestedResourceScope.ResourceSetWritten)
            {
                throw new ODataException(SRResources.ODataJsonWriter_EntityReferenceLinkAfterResourceSetInRequest);
            }

            if (!nestedResourceScope.EntityReferenceLinkWritten)
            {
                // In request
                if (!this.writingResponse)
                {
                    if (this.Version == null || this.Version < ODataVersion.V401)
                    {
                        // Write the property annotation for the entity reference link(s)
                        await this.odataAnnotationWriter.WritePropertyAnnotationNameAsync(
                            parentNestedResourceInfo.Name,
                            ODataAnnotationNames.ODataBind).ConfigureAwait(false);
                    }
                    else
                    {
                        await this.jsonWriter.WriteNameAsync(parentNestedResourceInfo.Name)
                            .ConfigureAwait(false);
                    }

                    Debug.Assert(parentNestedResourceInfo.IsCollection.HasValue, "parentNestedResourceInfo.IsCollection.HasValue");
                    if (parentNestedResourceInfo.IsCollection.Value)
                    {
                        // Write [ for the collection
                        await this.jsonWriter.StartArrayScopeAsync()
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
                        await this.jsonWriter.WriteNameAsync(parentNestedResourceInfo.Name)
                            .ConfigureAwait(false);
                    }
                }

                nestedResourceScope.EntityReferenceLinkWritten = true;
            }

            if (!this.writingResponse && (this.Version == null || this.Version < ODataVersion.V401))
            {
                Debug.Assert(entityReferenceLink.Url != null,
                    "The entity reference link Url should have been validated by now.");
                await this.jsonWriter.WriteValueAsync(
                    this.jsonResourceSerializer.UriToString(entityReferenceLink.Url)).ConfigureAwait(false);
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
            return TaskUtils.GetTaskForSynchronousOperation((
                thisParam,
                resourceScopeParam,
                resourceParam,
                writingResponseParam,
                selectedPropertiesParam) => thisParam.PrepareResourceForWriteStart(
                    resourceScopeParam,
                    resourceParam,
                    writingResponseParam,
                    selectedPropertiesParam),
                this,
                resourceScope,
                resource,
                writingResponse,
                selectedProperties);
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
            return TaskUtils.GetTaskForSynchronousOperation((
                thisParam,
                resourceScopeParam,
                deletedResourceParam,
                writingResponseParam,
                selectedPropertiesParam) => this.PrepareDeletedResourceForWriteStart(
                    resourceScopeParam,
                    deletedResourceParam,
                    writingResponseParam,
                    selectedPropertiesParam),
                this,
                resourceScope,
                deletedResource,
                writingResponse,
                selectedProperties);
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

            this.jsonWriter.WriteValue(id == null ? null : this.jsonResourceSerializer.UriToString(id));

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
            ODataUriSlim uri;

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

            ODataResourceMetadataBuilder builder = this.jsonOutputContext.MetadataLevel.CreateResourceMetadataBuilder(
                resource,
                typeContext,
                serializationInfo,
                resourceType,
                selectedProperties,
                this.writingResponse,
                this.jsonOutputContext.MessageWriterSettings.EnableWritingKeyAsSegment,
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

                this.jsonOutputContext.MetadataLevel.InjectMetadataBuilder(resource, builder);
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

                this.jsonWriter.WriteValue(this.jsonResourceSerializer.UriToString(nextPageLink));

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
                this.jsonWriter.WriteValue(this.jsonResourceSerializer.UriToString(deltaLink));
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

            JsonDeletedResourceScope resourceScope = this.CurrentDeletedResourceScope;

            // Write the metadata
            this.jsonResourceSerializer.WriteResourceStartMetadataProperties(resourceScope);
            this.jsonResourceSerializer.WriteResourceMetadataProperties(resourceScope);

            this.jsonOutputContext.PropertyCacheHandler.SetCurrentResourceScopeLevel(this.ScopeLevel);

            // Write custom instance annotations
            this.instanceAnnotationWriter.WriteInstanceAnnotations(resource.InstanceAnnotations, resourceScope.InstanceAnnotationWriteTracker);

            this.jsonResourceSerializer.JsonValueSerializer.AssertRecursionDepthIsZero();
            this.WriteDeltaResourceProperties(resource);
            this.jsonResourceSerializer.JsonValueSerializer.AssertRecursionDepthIsZero();
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
                this.jsonWriter.WriteName(ODataJsonConstants.ODataIdPropertyName);
                this.jsonWriter.WriteValue(resource.Id.OriginalString);
            }
            else
            {
                Uri id;
                if (resource.MetadataBuilder.TryGetIdForSerialization(out id))
                {
                    this.jsonWriter.WriteInstanceAnnotationName(ODataJsonConstants.ODataIdPropertyName);
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
            this.jsonResourceSerializer.JsonValueSerializer.AssertRecursionDepthIsZero();
            if (resource.NonComputedProperties != null)
            {
                this.jsonResourceSerializer.WriteProperties(
                    this.ResourceType,
                    resource.NonComputedProperties,
                    false /* isComplexValue */,
                    this.DuplicatePropertyNameChecker,
                    resource.MetadataBuilder);
                this.jsonResourceSerializer.JsonValueSerializer.AssertRecursionDepthIsZero();
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

            this.jsonWriter.WriteName(ODataJsonConstants.ODataReasonPropertyName);

            switch (resource.Reason.Value)
            {
                case DeltaDeletedEntryReason.Deleted:
                    this.jsonWriter.WriteValue(ODataJsonConstants.ODataReasonDeletedValue);
                    break;
                case DeltaDeletedEntryReason.Changed:
                    this.jsonWriter.WriteValue(ODataJsonConstants.ODataReasonChangedValue);
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
            this.jsonResourceSerializer.WriteDeltaContextUri(this.CurrentDeltaLinkScope.GetOrCreateTypeContext(), kind);
        }

        /// <summary>
        /// Writes the source for a delta (deleted) link.
        /// </summary>
        /// <param name="link">The link to write source for.</param>
        private void WriteDeltaLinkSource(ODataDeltaLinkBase link)
        {
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link is ODataDeltaLink || link is ODataDeltaDeletedLink, "link must be either DeltaLink or DeltaDeletedLink.");

            this.jsonWriter.WriteName(ODataJsonConstants.ODataSourcePropertyName);
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

            this.jsonWriter.WriteName(ODataJsonConstants.ODataRelationshipPropertyName);
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

            this.jsonWriter.WriteName(ODataJsonConstants.ODataTargetPropertyName);
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
                throw new ODataException(SRResources.ODataJsonWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet);
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

            await this.jsonWriter.StartObjectScopeAsync()
                .ConfigureAwait(false);

            await this.odataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataId)
                .ConfigureAwait(false);

            Uri id = this.messageWriterSettings.MetadataDocumentUri.MakeRelativeUri(entityReferenceLink.Url);

            await this.jsonWriter.WriteValueAsync(
                id == null ? null : this.jsonResourceSerializer.UriToString(id)).ConfigureAwait(false);

            await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                entityReferenceLink.InstanceAnnotations).ConfigureAwait(false);

            await this.jsonWriter.EndObjectScopeAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the odata.count annotation for a resource set if it has not been written yet
        /// (and the count is specified on the resource set).
        /// </summary>
        /// <param name="count">The count to write for the resource set.</param>
        /// <param name="propertyName">The name of the expanded nav property or null for a top-level resource set.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private Task WriteResourceSetCountAsync(long? count, string propertyName)
        {
            if (count.HasValue)
            {
                return WriteResourceSetCountInnerAsync(count.Value, propertyName);

                async Task WriteResourceSetCountInnerAsync(long innerCount, string innerPropertyName)
                {
                    if (innerPropertyName == null)
                    {
                        await this.odataAnnotationWriter.WriteInstanceAnnotationNameAsync(
                            ODataAnnotationNames.ODataCount).ConfigureAwait(false);
                    }
                    else
                    {
                        await this.odataAnnotationWriter.WritePropertyAnnotationNameAsync(
                            innerPropertyName,
                            ODataAnnotationNames.ODataCount).ConfigureAwait(false);
                    }

                    await this.jsonWriter.WriteValueAsync(innerCount)
                        .ConfigureAwait(false);
                }
            }

            return TaskUtils.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes the odata.nextLink annotation for a resource set if it has not been written yet (and the next link is specified on the resource set).
        /// </summary>
        /// <param name="nextPageLink">The nextLink to write, if available.</param>
        /// <param name="propertyName">The name of the expanded nav property or null for a top-level resource set.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private Task WriteResourceSetNextLinkAsync(Uri nextPageLink, string propertyName)
        {
            bool nextPageWritten = this.State == WriterState.ResourceSet ?
                this.CurrentResourceSetScope.NextPageLinkWritten :
                this.CurrentDeltaResourceSetScope.NextPageLinkWritten;

            if (nextPageLink != null && !nextPageWritten)
            {
                return WriteResourceSetNextLinkInnerAsync(nextPageLink, propertyName);

                async Task WriteResourceSetNextLinkInnerAsync(Uri innerNextPageLink, string innerPropertyName)
                {
                    if (innerPropertyName == null)
                    {
                        await this.odataAnnotationWriter.WriteInstanceAnnotationNameAsync(
                            ODataAnnotationNames.ODataNextLink).ConfigureAwait(false);
                    }
                    else
                    {
                        await this.odataAnnotationWriter.WritePropertyAnnotationNameAsync(
                            innerPropertyName,
                            ODataAnnotationNames.ODataNextLink).ConfigureAwait(false);
                    }

                    await this.jsonWriter.WriteValueAsync(
                        this.jsonResourceSerializer.UriToString(innerNextPageLink)).ConfigureAwait(false);

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

            return TaskUtils.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes the odata.deltaLink annotation for a resource set if it has not been written yet
        /// (and the delta link is specified on the resource set).
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private Task WriteResourceSetDeltaLinkAsync(Uri deltaLink)
        {
            if (deltaLink == null)
            {
                return TaskUtils.CompletedTask;
            }

            Debug.Assert(this.State == WriterState.ResourceSet || this.State == WriterState.DeltaResourceSet,
                "Write ResourceSet Delta Link called when not in ResourceSet or DeltaResourceSet state");

            bool deltaLinkWritten = this.State == WriterState.ResourceSet
                ? this.CurrentResourceSetScope.DeltaLinkWritten :
                this.CurrentDeltaResourceSetScope.DeltaLinkWritten;

            if (!deltaLinkWritten)
            {
                return WriteResourceSetDeltaLinkInnerAsync(deltaLink);

                async Task WriteResourceSetDeltaLinkInnerAsync(Uri innerDeltaLink)
                {
                    await this.odataAnnotationWriter.WriteInstanceAnnotationNameAsync(
                        ODataAnnotationNames.ODataDeltaLink).ConfigureAwait(false);
                    await this.jsonWriter.WriteValueAsync(
                        this.jsonResourceSerializer.UriToString(innerDeltaLink)).ConfigureAwait(false);

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

            return TaskUtils.CompletedTask;
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
            await this.odataAnnotationWriter.WriteInstanceAnnotationNameAsync(
                ODataAnnotationNames.ODataRemoved).ConfigureAwait(false);
            await this.jsonWriter.StartObjectScopeAsync()
                .ConfigureAwait(false);
            await this.WriteDeltaResourceReasonAsync(resource)
                .ConfigureAwait(false);
            await this.jsonWriter.EndObjectScopeAsync()
                .ConfigureAwait(false);

            JsonDeletedResourceScope resourceScope = this.CurrentDeletedResourceScope;

            // Write the metadata
            await this.jsonResourceSerializer.WriteResourceStartMetadataPropertiesAsync(resourceScope)
                .ConfigureAwait(false);
            await this.jsonResourceSerializer.WriteResourceMetadataPropertiesAsync(resourceScope)
                .ConfigureAwait(false);

            this.jsonOutputContext.PropertyCacheHandler.SetCurrentResourceScopeLevel(this.ScopeLevel);

            // Write custom instance annotations
            await this.instanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                resource.InstanceAnnotations,
                resourceScope.InstanceAnnotationWriteTracker).ConfigureAwait(false);

            this.jsonResourceSerializer.JsonValueSerializer.AssertRecursionDepthIsZero();
            await this.WriteDeltaResourcePropertiesAsync(resource)
                .ConfigureAwait(false);
            this.jsonResourceSerializer.JsonValueSerializer.AssertRecursionDepthIsZero();
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
                await this.jsonWriter.WriteNameAsync(ODataJsonConstants.ODataIdPropertyName)
                    .ConfigureAwait(false);
                await this.jsonWriter.WriteValueAsync(resource.Id.OriginalString)
                    .ConfigureAwait(false);
            }
            else
            {
                Uri id;
                if (resource.MetadataBuilder.TryGetIdForSerialization(out id))
                {
                    await this.jsonWriter.WriteInstanceAnnotationNameAsync(
                        ODataJsonConstants.ODataIdPropertyName).ConfigureAwait(false);
                    await this.jsonWriter.WriteValueAsync(id.OriginalString)
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

            this.jsonResourceSerializer.JsonValueSerializer.AssertRecursionDepthIsZero();
            if (resource.NonComputedProperties != null)
            {
                writePropertiesTask = this.jsonResourceSerializer.WritePropertiesAsync(
                    this.ResourceType,
                    resource.NonComputedProperties,
                    false /* isComplexValue */,
                    this.DuplicatePropertyNameChecker,
                    resource.MetadataBuilder);
                this.jsonResourceSerializer.JsonValueSerializer.AssertRecursionDepthIsZero();
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

            await this.jsonWriter.WriteNameAsync(ODataJsonConstants.ODataReasonPropertyName)
                .ConfigureAwait(false);

            switch (resource.Reason.Value)
            {
                case DeltaDeletedEntryReason.Deleted:
                    await this.jsonWriter.WriteValueAsync(ODataJsonConstants.ODataReasonDeletedValue)
                        .ConfigureAwait(false);
                    break;
                case DeltaDeletedEntryReason.Changed:
                    await this.jsonWriter.WriteValueAsync(ODataJsonConstants.ODataReasonChangedValue)
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
            return this.jsonResourceSerializer.WriteDeltaContextUriAsync(
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

            await this.jsonWriter.WriteNameAsync(ODataJsonConstants.ODataSourcePropertyName)
                .ConfigureAwait(false);
            await this.jsonWriter.WriteValueAsync(UriUtils.UriToString(link.Source))
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

            await this.jsonWriter.WriteNameAsync(ODataJsonConstants.ODataRelationshipPropertyName)
                .ConfigureAwait(false);
            await this.jsonWriter.WriteValueAsync(link.Relationship)
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

            await this.jsonWriter.WriteNameAsync(ODataJsonConstants.ODataTargetPropertyName)
                .ConfigureAwait(false);
            await this.jsonWriter.WriteValueAsync(UriUtils.UriToString(link.Target))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// A scope for a JSON resource set.
        /// </summary>
        private sealed class JsonResourceSetScope : ResourceSetScope
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
            internal JsonResourceSetScope(ODataResourceSet resourceSet, IEdmNavigationSource navigationSource, IEdmType itemType, bool skipWriting, SelectedPropertiesNode selectedProperties, in ODataUriSlim odataUri, bool isUndeclared)
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
        /// A scope for a deleted resource in Json writer.
        /// </summary>
        private sealed class JsonDeletedResourceScope : DeletedResourceScope, IODataJsonWriterResourceState
        {
            /// <summary>Bit field of the Json metadata properties written so far.</summary>
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
            internal JsonDeletedResourceScope(
                ODataDeletedResource resource,
                ODataResourceSerializationInfo serializationInfo,
                IEdmNavigationSource navigationSource,
                IEdmEntityType resourceType,
                bool skipWriting,
                ODataMessageWriterSettings writerSettings,
                SelectedPropertiesNode selectedProperties,
                in ODataUriSlim odataUri,
                bool isUndeclared)
                : base(resource, serializationInfo, navigationSource, resourceType, writerSettings, selectedProperties, odataUri)
            {
                this.isUndeclared = isUndeclared;
            }

            /// <summary>
            /// Enumeration of Json metadata property flags, used to keep track of which properties were already written.
            /// </summary>
            [Flags]
            private enum JsonEntryMetadataProperty
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
            ODataResourceBase IODataJsonWriterResourceState.Resource
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
                    return this.IsMetadataPropertyWritten(JsonEntryMetadataProperty.EditLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonEntryMetadataProperty.EditLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.readLink metadata property has been written.
            /// </summary>
            public bool ReadLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonEntryMetadataProperty.ReadLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonEntryMetadataProperty.ReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaEditLink metadata property has been written.
            /// </summary>
            public bool MediaEditLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonEntryMetadataProperty.MediaEditLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonEntryMetadataProperty.MediaEditLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaReadLink metadata property has been written.
            /// </summary>
            public bool MediaReadLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonEntryMetadataProperty.MediaReadLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonEntryMetadataProperty.MediaReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaContentType metadata property has been written.
            /// </summary>
            public bool MediaContentTypeWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonEntryMetadataProperty.MediaContentType);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonEntryMetadataProperty.MediaContentType);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaEtag metadata property has been written.
            /// </summary>
            public bool MediaETagWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonEntryMetadataProperty.MediaETag);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonEntryMetadataProperty.MediaETag);
                }
            }

            /// <summary>
            /// Marks the <paramref name="jsonMetadataProperty"/> as written in this resource scope.
            /// </summary>
            /// <param name="jsonMetadataProperty">The metadata property which was written.</param>
            private void SetWrittenMetadataProperty(JsonEntryMetadataProperty jsonMetadataProperty)
            {
                Debug.Assert(!this.IsMetadataPropertyWritten(jsonMetadataProperty), "Can't write the same metadata property twice.");
                this.alreadyWrittenMetadataProperties |= (int)jsonMetadataProperty;
            }

            /// <summary>
            /// Determines if the <paramref name="jsonMetadataProperty"/> was already written for this resource scope.
            /// </summary>
            /// <param name="jsonMetadataProperty">The metadata property to test for.</param>
            /// <returns>true if the <paramref name="jsonMetadataProperty"/> was already written for this resource scope; false otherwise.</returns>
            private bool IsMetadataPropertyWritten(JsonEntryMetadataProperty jsonMetadataProperty)
            {
                return (this.alreadyWrittenMetadataProperties & (int)jsonMetadataProperty) == (int)jsonMetadataProperty;
            }
        }

        /// <summary>
        /// A scope for a property in Json writer.
        /// </summary>
        private sealed class JsonPropertyScope : PropertyInfoScope
        {
            /// <summary>
            /// Constructor to create a new property scope.
            /// </summary>
            /// <param name="property">The property for the new scope.</param>
            /// <param name="navigationSource">The navigation source.</param>
            /// <param name="resourceType">The structured type for the resource containing the property to be written.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            internal JsonPropertyScope(ODataPropertyInfo property, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, SelectedPropertiesNode selectedProperties, in ODataUriSlim odataUri)
                : base(property, navigationSource, resourceType, selectedProperties, odataUri)
            {
            }
        }

        /// <summary>
        /// A scope for a delta link in Json writer.
        /// </summary>
        private sealed class JsonDeltaLinkScope : DeltaLinkScope
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
            public JsonDeltaLinkScope(WriterState state, ODataItem link, ODataResourceSerializationInfo serializationInfo, IEdmNavigationSource navigationSource, IEdmEntityType entityType, SelectedPropertiesNode selectedProperties, in ODataUriSlim odataUri)
                : base(state, link, serializationInfo, navigationSource, entityType, selectedProperties, odataUri)
            {
            }
        }

        /// <summary>
        /// A scope for a JSON resource set.
        /// </summary>
        private sealed class JsonDeltaResourceSetScope : DeltaResourceSetScope
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
            public JsonDeltaResourceSetScope(ODataDeltaResourceSet resourceSet, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, SelectedPropertiesNode selectedProperties, in ODataUriSlim odataUri)
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
        /// A scope for a resource in Json writer.
        /// </summary>
        private sealed class JsonResourceScope : ResourceScope, IODataJsonWriterResourceState
        {
            /// <summary>Bit field of the Json metadata properties written so far.</summary>
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
            internal JsonResourceScope(
                ODataResource resource,
                ODataResourceSerializationInfo serializationInfo,
                IEdmNavigationSource navigationSource,
                IEdmStructuredType resourceType,
                bool skipWriting,
                ODataMessageWriterSettings writerSettings,
                SelectedPropertiesNode selectedProperties,
                in ODataUriSlim odataUri,
                bool isUndeclared)
                : base(resource, serializationInfo, navigationSource, resourceType, skipWriting, writerSettings, selectedProperties, odataUri)
            {
                this.isUndeclared = isUndeclared;
            }

            /// <summary>
            /// Enumeration of Json metadata property flags, used to keep track of which properties were already written.
            /// </summary>
            [Flags]
            private enum JsonEntryMetadataProperty
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
            ODataResourceBase IODataJsonWriterResourceState.Resource
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
                    return this.IsMetadataPropertyWritten(JsonEntryMetadataProperty.EditLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonEntryMetadataProperty.EditLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.readLink metadata property has been written.
            /// </summary>
            public bool ReadLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonEntryMetadataProperty.ReadLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonEntryMetadataProperty.ReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaEditLink metadata property has been written.
            /// </summary>
            public bool MediaEditLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonEntryMetadataProperty.MediaEditLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonEntryMetadataProperty.MediaEditLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaReadLink metadata property has been written.
            /// </summary>
            public bool MediaReadLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonEntryMetadataProperty.MediaReadLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonEntryMetadataProperty.MediaReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaContentType metadata property has been written.
            /// </summary>
            public bool MediaContentTypeWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonEntryMetadataProperty.MediaContentType);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonEntryMetadataProperty.MediaContentType);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaEtag metadata property has been written.
            /// </summary>
            public bool MediaETagWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonEntryMetadataProperty.MediaETag);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonEntryMetadataProperty.MediaETag);
                }
            }

            /// <summary>
            /// Marks the <paramref name="jsonMetadataProperty"/> as written in this resource scope.
            /// </summary>
            /// <param name="jsonMetadataProperty">The metadata property which was written.</param>
            private void SetWrittenMetadataProperty(JsonEntryMetadataProperty jsonMetadataProperty)
            {
                Debug.Assert(!this.IsMetadataPropertyWritten(jsonMetadataProperty), "Can't write the same metadata property twice.");
                this.alreadyWrittenMetadataProperties |= (int)jsonMetadataProperty;
            }

            /// <summary>
            /// Determines if the <paramref name="jsonMetadataProperty"/> was already written for this resource scope.
            /// </summary>
            /// <param name="jsonMetadataProperty">The metadata property to test for.</param>
            /// <returns>true if the <paramref name="jsonMetadataProperty"/> was already written for this resource scope; false otherwise.</returns>
            private bool IsMetadataPropertyWritten(JsonEntryMetadataProperty jsonMetadataProperty)
            {
                return (this.alreadyWrittenMetadataProperties & (int)jsonMetadataProperty) == (int)jsonMetadataProperty;
            }
        }

        /// <summary>
        /// A scope for a Json nested resource info.
        /// </summary>
        private sealed class JsonNestedResourceInfoScope : NestedResourceInfoScope
        {
            /// <summary>true if we have already written an entity reference link for this nested resource info in requests; otherwise false.</summary>
            private bool entityReferenceLinkWritten;

            /// <summary>true if we have written at least one resource set for this nested resource info in requests; otherwise false.</summary>
            private bool resourceSetWritten;

            /// <summary>
            /// Constructor to create a new Json nested resource info scope.
            /// </summary>
            /// <param name="writerState">The writer state for the new scope.</param>
            /// <param name="navLink">The nested resource info for the new scope.</param>
            /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
            /// <param name="itemType">The type for the items in the resource set to be written (or null if the navigationSource base type should be used).</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            /// <param name="parentScope">The scope of the parent.</param>
            internal JsonNestedResourceInfoScope(WriterState writerState, ODataNestedResourceInfo navLink, IEdmNavigationSource navigationSource, IEdmType itemType, bool skipWriting, SelectedPropertiesNode selectedProperties, in ODataUriSlim odataUri, Scope parentScope)
                : base(writerState, navLink, navigationSource, itemType, skipWriting, selectedProperties, odataUri, parentScope)
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
            /// Clones this Json nested resource info scope and sets a new writer state.
            /// </summary>
            /// <param name="newWriterState">The writer state to set.</param>
            /// <returns>The cloned nested resource info scope with the specified writer state.</returns>
            internal override NestedResourceInfoScope Clone(WriterState newWriterState)
            {
                return new JsonNestedResourceInfoScope(newWriterState, (ODataNestedResourceInfo)this.Item, this.NavigationSource, this.ItemType, this.SkipWriting, this.SelectedProperties, this.ODataUri, this.parentScope)
                {
                    EntityReferenceLinkWritten = this.entityReferenceLinkWritten,
                    ResourceSetWritten = this.resourceSetWritten,
                    DerivedTypeConstraints = this.DerivedTypeConstraints
                };
            }
        }
    }
}
