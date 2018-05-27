//---------------------------------------------------------------------
// <copyright file="ODataJsonLightReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#if PORTABLELIB
using System.Threading.Tasks;
#endif
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.JsonLight
{
    /// <summary>
    /// OData reader for the JsonLight format.
    /// </summary>
    internal sealed class ODataJsonLightReader : ODataReaderCoreAsync
    {
        #region private fields
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataJsonLightInputContext jsonLightInputContext;

        /// <summary>The resource and resource set deserializer to read input with.</summary>
        private readonly ODataJsonLightResourceDeserializer jsonLightResourceDeserializer;

        /// <summary>The scope associated with the top level of this payload.</summary>
        private readonly JsonLightTopLevelScope topLevelScope;

        /// <summary>true if the reader is created for reading parameter; false otherwise.</summary>
        private readonly bool readingParameter;
        #endregion private fields

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The input to read the payload from.</param>
        /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
        /// <param name="expectedResourceType">The expected structured type for the resource to be read (in case of resource reader) or entries in the resource set to be read (in case of resource set reader).</param>
        /// <param name="readingResourceSet">true if the reader is created for reading a resource set; false when it is created for reading a resource.</param>
        /// <param name="readingParameter">true if the reader is created for reading a parameter; false otherwise.</param>
        /// <param name="readingDelta">true if the reader is created for reading expanded navigation property in delta response; false otherwise.</param>
        /// <param name="listener">If not null, the Json reader will notify the implementer of the interface of relevant state changes in the Json reader.</param>
        internal ODataJsonLightReader(
            ODataJsonLightInputContext jsonLightInputContext,
            IEdmNavigationSource navigationSource,
            IEdmStructuredType expectedResourceType,
            bool readingResourceSet,
            bool readingParameter = false,
            bool readingDelta = false,
            IODataReaderWriterListener listener = null)
            : base(jsonLightInputContext, readingResourceSet, readingDelta, listener)
        {
            Debug.Assert(jsonLightInputContext != null, "jsonLightInputContext != null");
            Debug.Assert(
                expectedResourceType == null || jsonLightInputContext.Model.IsUserModel(),
                "If the expected type is specified we need model as well. We should have verified that by now.");

            this.jsonLightInputContext = jsonLightInputContext;
            this.jsonLightResourceDeserializer = new ODataJsonLightResourceDeserializer(jsonLightInputContext);
            this.readingParameter = readingParameter;
            this.topLevelScope = new JsonLightTopLevelScope(navigationSource, expectedResourceType, new ODataUri());
            this.EnterScope(this.topLevelScope);
        }

        #endregion

        #region private properties

        /// <summary>
        /// Returns the current resource state.
        /// </summary>
        private IODataJsonLightReaderResourceState CurrentResourceState
        {
            get
            {
                Debug.Assert(
                    this.State == ODataReaderState.ResourceStart || this.State == ODataReaderState.ResourceEnd ||
                    this.State == ODataReaderState.DeletedResourceStart || this.State == ODataReaderState.DeletedResourceEnd,
                    "This property can only be accessed in the EntryStart or EntryEnd scope.");
                return (IODataJsonLightReaderResourceState)this.CurrentScope;
            }
        }

        /// <summary>
        /// Returns current scope cast to JsonLightResourceSetScope
        /// </summary>
        private JsonLightResourceSetScope CurrentJsonLightResourceSetScope
        {
            get
            {
                return ((JsonLightResourceSetScope)this.CurrentScope);
            }
        }

        /// <summary>
        /// Returns current scope cast to JsonLightNestedResourceInfoScope
        /// </summary>
        private JsonLightNestedResourceInfoScope CurrentJsonLightNestedResourceInfoScope
        {
            get
            {
                return ((JsonLightNestedResourceInfoScope)this.CurrentScope);
            }
        }

        /// <summary>
        /// Returns nest info of current resource.
        /// </summary>
        private ODataNestedResourceInfo ParentNestedInfo
        {
            get
            {
                // NestInfo/Resource or NestInfo/ResourceSet/Resource
                Scope scope = SeekScope<JsonLightNestedResourceInfoScope>(maxDepth: 3);

                return scope != null ? (ODataNestedResourceInfo)scope.Item : null;
            }
        }
        #endregion private properties

        #region ReadAt<>Implementation Methods
        #region ReadAtStartImplementation
        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: when reading a resource set:    the reader is positioned on the first item in the resource set or the end array node of an empty resource set
        ///                 when reading a resource:  the first node of the first nested resource info value, null for a null expanded link or an end object
        ///                                         node if there are no navigation links.
        /// </remarks>
        protected override bool ReadAtStartImplementation()
        {
            Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
            Debug.Assert(
                this.IsReadingNestedPayload ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.None,
                "Pre-Condition: expected JsonNodeType.None when not reading a nested payload.");

            PropertyAndAnnotationCollector propertyAndAnnotationCollector =
                this.jsonLightInputContext.CreatePropertyAndAnnotationCollector();

            // Position the reader on the first node depending on whether we are reading a nested payload or a Uri Operation Parameter or not.
            ODataPayloadKind payloadKind = this.ReadingDelta
                ? ODataPayloadKind.Delta
                : this.ReadingResourceSet ?
                    ODataPayloadKind.ResourceSet
                    : ODataPayloadKind.Resource;

            // Following parameter "this.IsReadingNestedPayload || this.readingParameter" indicates whether to read
            // { "value" :
            // or
            // { "parameterName" :
            this.jsonLightResourceDeserializer.ReadPayloadStart(
                payloadKind,
                propertyAndAnnotationCollector,
                this.IsReadingNestedPayload || this.readingParameter,
                /*allowEmptyPayload*/false);

            ResolveScopeInfoFromContextUrl();

            return this.ReadAtStartImplementationSynchronously(propertyAndAnnotationCollector);
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: when reading a resource set:    the reader is positioned on the first item in the resource set or the end array node of an empty resource set
        ///                 when reading a resource:  the first node of the first nested resource info value, null for a null expanded link or an end object
        ///                                         node if there are no navigation links.
        /// </remarks>
        protected override Task<bool> ReadAtStartImplementationAsync()
        {
            Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
            Debug.Assert(
                this.IsReadingNestedPayload ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.None,
                "Pre-Condition: expected JsonNodeType.None when not reading a nested payload.");

            PropertyAndAnnotationCollector propertyAndAnnotationCollector =
                this.jsonLightInputContext.CreatePropertyAndAnnotationCollector();

            // Position the reader on the first node depending on whether we are reading a nested payload or not.
            ODataPayloadKind payloadKind = this.ReadingDelta
                ? ODataPayloadKind.Delta
                : this.ReadingResourceSet ?
                    ODataPayloadKind.ResourceSet
                    : ODataPayloadKind.Resource;

            return this.jsonLightResourceDeserializer.ReadPayloadStartAsync(
                payloadKind,
                propertyAndAnnotationCollector,
                this.IsReadingNestedPayload,
                /*allowEmptyPayload*/false)
                .FollowOnSuccessWith(t => ResolveScopeInfoFromContextUrl())
                .FollowOnSuccessWith(t =>
                    this.ReadAtStartImplementationSynchronously(propertyAndAnnotationCollector));
        }
#endif
        #endregion ReadAtStartImplementation

        #region ResourceSet
        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  Any start node            - The first resource in the resource set
        ///                 JsonNodeType.EndArray     - The end of the resource set
        /// Post-Condition: The reader is positioned over the StartObject node of the first resource in the resource set or
        ///                 on the node following the resource set end in case of an empty resource set
        /// </remarks>
        protected override bool ReadAtResourceSetStartImplementation()
        {
            return this.ReadAtResourceSetStartImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  Any start node            - The first resource in the resource set
        ///                 JsonNodeType.EndArray     - The end of the resource set
        /// Post-Condition: The reader is positioned over the StartObject node of the first resource in the resource set or
        ///                 on the node following the resource set end in case of an empty resource set
        /// </remarks>
        protected override Task<bool> ReadAtResourceSetStartImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtResourceSetStartImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.Property        if the resource set has further instance or property annotations after the resource set property
        ///                JsonNodeType.EndObject       if the resource set has no further instance or property annotations after the resource set property
        /// Post-Condition: JsonNodeType.EndOfInput     for a top-level resource set when not reading a nested payload
        ///                 JsonNodeType.Property       more properties exist on the owning resource after the expanded link containing the resource set
        ///                 JsonNodeType.EndObject      no further properties exist on the owning resource after the expanded link containing the resource set
        ///                 JsonNodeType.EndArray       end of expanded link in request, in this case the resource set doesn't actually own the array object and it won't read it.
        ///                 Any                         in case of expanded resource set in request, this might be the next item in the expanded array, which is not a resource
        /// </remarks>
        protected override bool ReadAtResourceSetEndImplementation()
        {
            return this.ReadAtResourceSetEndImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.Property        if the resource set has further instance or property annotations after the resource set property
        ///                JsonNodeType.EndObject       if the resource set has no further instance or property annotations after the resource set property
        /// Post-Condition: JsonNodeType.EndOfInput     for a top-level resource set when not reading a nested payload
        ///                 JsonNodeType.Property       more properties exist on the owning resource after the expanded link containing the resource set
        ///                 JsonNodeType.EndObject      no further properties exist on the owning resource after the expanded link containing the resource set
        ///                 JsonNodeType.EndArray       end of expanded link in request, in this case the resource set doesn't actually own the array object and it won't read it.
        ///                 Any                         in case of expanded resource set in request, this might be the next item in the expanded array, which is not a resource
        /// </remarks>
        protected override Task<bool> ReadAtResourceSetEndImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtResourceSetEndImplementationSynchronously);
        }
#endif
        #endregion ResourceSet

        #region Resource
        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded resource set of the nested resource info to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded resource set of the nested resource info to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// </remarks>
        protected override bool ReadAtResourceStartImplementation()
        {
            return this.ReadAtResourceStartImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded resource set of the nested resource info to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded resource set of the nested resource info to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// </remarks>
        protected override Task<bool> ReadAtResourceStartImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtResourceStartImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              end of object of the resource
        ///                 JsonNodeType.PrimitiveValue         end of primitive value in a collection
        /// Post-Condition: The reader is positioned on the first node after the resource's end-object node
        /// </remarks>
        protected override bool ReadAtResourceEndImplementation()
        {
            return this.ReadAtResourceEndImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              end of object of the resource
        ///                 JsonNodeType.PrimitiveValue (null)  end of null expanded resource
        /// Post-Condition: The reader is positioned on the first node after the resource's end-object node
        /// </remarks>
        protected override Task<bool> ReadAtResourceEndImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtResourceEndImplementationSynchronously);
        }
#endif

        #endregion Resource

        #region Primitive
        /// <summary>
        /// Implementation of the reader logic when in state 'Primitive'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         end of primitive value in a collection
        /// Post-Condition: The reader is positioned on the first node after the primitive value
        /// </remarks>
        protected override bool ReadAtPrimitiveImplementation()
        {
            return this.ReadAtPrimitiveSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'Primitive'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         end of primitive value in a collection
        /// Post-Condition: The reader is positioned on the first node after the primitive value
        /// </remarks>
        protected override Task<bool> ReadAtPrimitiveImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtEntityReferenceLinkSynchronously);
        }
#endif
        #endregion Primitive

        #region NestedResourceInfo
        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfoStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            start of an expanded resource
        ///                 JsonNodeType.StartArray             start of an expanded resource set
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null resource
        ///                 JsonNodeType.Property               deferred link with more properties in owning resource
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning resource
        /// Post-Condition: JsonNodeType.StartArray:            start of expanded resource
        ///                 JsonNodeType.StartObject            start of expanded resource set
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null resource
        ///                 JsonNodeType.Property               deferred link with more properties in owning resource
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning resource
        /// </remarks>
        protected override bool ReadAtNestedResourceInfoStartImplementation()
        {
            return this.ReadAtNestedResourceInfoStartImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfoStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            start of an expanded resource
        ///                 JsonNodeType.StartArray             start of an expanded resource set
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null resource
        ///                 JsonNodeType.Property               deferred link with more properties in owning resource
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning resource
        /// Post-Condition: JsonNodeType.StartArray:            start of expanded resource
        ///                 JsonNodeType.StartObject            start of expanded resource set
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null resource
        ///                 JsonNodeType.Property               deferred link with more properties in owning resource
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning resource
        /// </remarks>
        protected override Task<bool> ReadAtNestedResourceInfoStartImplementationAsync()
        {
            return
                TaskUtils.GetTaskForSynchronousOperation<bool>(
                    this.ReadAtNestedResourceInfoStartImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfoEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject:         nested resource info is last property in owning resource
        ///                 JsonNodeType.Property:          there are more properties after the nested resource info in the owning resource
        /// Post-Condition: JsonNodeType.StartObject        start of the expanded resource nested resource info to read next
        ///                 JsonNodeType.StartArray         start of the expanded resource set nested resource info to read next
        ///                 JsonNoteType.Primitive (null)   expanded null resource nested resource info to read next
        ///                 JsonNoteType.Property           property after deferred link or entity reference link
        ///                 JsonNodeType.EndObject          end of the parent resource
        /// </remarks>
        protected override bool ReadAtNestedResourceInfoEndImplementation()
        {
            return this.ReadAtNestedResourceInfoEndImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfoEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject:         nested resource info is last property in owning resource
        ///                 JsonNodeType.Property:          there are more properties after the nested resource info in the owning resource
        /// Post-Condition: JsonNodeType.StartObject        start of the expanded resource nested resource info to read next
        ///                 JsonNodeType.StartArray         start of the expanded resource set nested resource info to read next
        ///                 JsonNoteType.Primitive (null)   expanded null resource nested resource info to read next
        ///                 JsonNoteType.Property           property after deferred link or entity reference link
        ///                 JsonNodeType.EndObject          end of the parent resource
        /// </remarks>
        protected override Task<bool> ReadAtNestedResourceInfoEndImplementationAsync()
        {
            return
                TaskUtils.GetTaskForSynchronousOperation<bool>(
                    this.ReadAtNestedResourceInfoEndImplementationSynchronously);
        }
#endif

        #endregion NestedResourceInfo

        #region EntityReferenceLink
        /// <summary>
        /// Implementation of the reader logic when in state 'EntityReferenceLink'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// This method doesn't move the reader
        /// Pre-Condition:  JsonNodeType.EndObject:         expanded link property is last property in owning resource
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning resource
        ///                 Any:                            expanded collection link - the node after the entity reference link.
        /// Post-Condition: JsonNodeType.EndObject:         expanded link property is last property in owning resource
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning resource
        ///                 Any:                            expanded collection link - the node after the entity reference link.
        /// </remarks>
        protected override bool ReadAtEntityReferenceLink()
        {
            return this.ReadAtEntityReferenceLinkSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'EntityReferenceLink'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// This method doesn't move the reader
        /// Pre-Condition:  JsonNodeType.EndObject:         expanded link property is last property in owning resource
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning resource
        ///                 Any:                            expanded collection link - the node after the entity reference link.
        /// Post-Condition: JsonNodeType.EndObject:         expanded link property is last property in owning resource
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning resource
        ///                 Any:                            expanded collection link - the node after the entity reference link.
        /// </remarks>
        protected override Task<bool> ReadAtEntityReferenceLinkAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtEntityReferenceLinkSynchronously);
        }
#endif
        #endregion EntityReferenceLink

        #region DeltaResourceSet

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  Any start node            - The first resource in the resource set
        ///                 JsonNodeType.EndArray     - The end of the resource set
        /// Post-Condition: The reader is positioned over the StartObject node of the first resource in the resource set or
        ///                 on the node following the resource set end in case of an empty resource set
        /// </remarks>
        protected override bool ReadAtDeltaResourceSetStartImplementation()
        {
            return this.ReadAtResourceSetStartImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  Any start node            - The first resource in the resource set
        ///                 JsonNodeType.EndArray     - The end of the resource set
        /// Post-Condition: The reader is positioned over the StartObject node of the first resource in the resource set or
        ///                 on the node following the resource set end in case of an empty resource set
        /// </remarks>
        protected override Task<bool> ReadAtDeltaResourceSetStartImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtResourceSetStartImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.Property        if the resource set has further instance or property annotations after the resource set property
        ///                JsonNodeType.EndObject       if the resource set has no further instance or property annotations after the resource set property
        /// Post-Condition: JsonNodeType.EndOfInput     for a top-level resource set when not reading a nested payload
        ///                 JsonNodeType.Property       more properties exist on the owning resource after the expanded link containing the resource set
        ///                 JsonNodeType.EndObject      no further properties exist on the owning resource after the expanded link containing the resource set
        ///                 JsonNodeType.EndArray       end of expanded link in request, in this case the resource set doesn't actually own the array object and it won't read it.
        ///                 Any                         in case of expanded resource set in request, this might be the next item in the expanded array, which is not a resource
        /// </remarks>
        protected override bool ReadAtDeltaResourceSetEndImplementation()
        {
            // Logic is same as for ResourceSet
            return this.ReadAtResourceSetEndImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.Property        if the resource set has further instance or property annotations after the resource set property
        ///                JsonNodeType.EndObject       if the resource set has no further instance or property annotations after the resource set property
        /// Post-Condition: JsonNodeType.EndOfInput     for a top-level resource set when not reading a nested payload
        ///                 JsonNodeType.Property       more properties exist on the owning resource after the expanded link containing the resource set
        ///                 JsonNodeType.EndObject      no further properties exist on the owning resource after the expanded link containing the resource set
        ///                 JsonNodeType.EndArray       end of expanded link in request, in this case the resource set doesn't actually own the array object and it won't read it.
        ///                 Any                         in case of expanded resource set in request, this might be the next item in the expanded array, which is not a resource
        /// </remarks>
        protected override Task<bool> ReadAtDeltaResourceSetEndImplementationAsync()
        {
            // Logic is same as for ResourceSet
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtResourceSetEndImplementationSynchronously);
        }
#endif
        #endregion DeltaResourceSet

        #region DeltaDeletedEntry

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaDeletedEntryStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded resource set of the nested resource info to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded resource set of the nested resource info to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// </remarks>
        protected override bool ReadAtDeletedResourceStartImplementation()
        {
            return this.ReadAtDeletedResourceStartImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaDeletedResourceStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded resource set of the nested resource info to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded resource set of the nested resource info to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// </remarks>
        protected override Task<bool> ReadAtDeletedResourceStartImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeletedResourceStartImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'DeletedResourceEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              end of object of the resource
        ///                 JsonNodeType.PrimitiveValue         end of primitive value in a collection
        /// Post-Condition: The reader is positioned on the first node after the deleted resource's end-object node
        /// </remarks>
        protected override bool ReadAtDeletedResourceEndImplementation()
        {
            // Same logic as ReadAtResourceEndImplementation
            return this.ReadAtResourceEndImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'DeletedResourceEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              end of object of the resource
        ///                 JsonNodeType.PrimitiveValue (null)  end of null expanded resource
        /// Post-Condition: The reader is positioned on the first node after the deleted resource's end-object node
        /// </remarks>
        protected override Task<bool> ReadAtDeletedResourceEndImplementationAsync()
        {
            // Same logic as ReadAtResourceEndImplementationAsync
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtResourceEndImplementationSynchronously);
        }
#endif

        #endregion DeltaDeletedEntry

        #region DeltaLink

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaLinkImplementation'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndArry               If no (more) properties exist in the resource's content
        /// </remarks>
        protected override bool ReadAtDeltaLinkImplementation()
        {
            return this.ReadAtDeltaLinkImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaLinkImplementation'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// </remarks>
        protected override Task<bool> ReadAtDeltaLinkImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeltaLinkImplementationSynchronously);
        }
#endif

        #endregion DeltaLink

        #region DeltaDeletedLink

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaLinkImplementation'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndArry               If no (more) properties exist in the resource's content
        /// </remarks>
        protected override bool ReadAtDeltaDeletedLinkImplementation()
        {
            return this.ReadAtDeltaDeletedLinkImplementationSynchronously();
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaDeletedLinkImplementation'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// </remarks>
        protected override Task<bool> ReadAtDeltaDeletedLinkImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeltaDeletedLinkImplementationSynchronously);
        }
#endif

        #endregion DeltaDeletedLink

        #endregion ReadAt<>Implementation methods

        #region ReadAt<>ImplementationSynchronously methods

        #region Start

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: when reading a resource set:    the reader is positioned on the first item in the resource set or the end array node of an empty resource set
        ///                 when reading a resource:  the first node of the first nested resource info value, null for a null expanded link or an end object
        ///                                         node if there are no navigation links.
        /// </remarks>
        private bool ReadAtStartImplementationSynchronously(
            PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            // For nested payload (e.g., expanded resource set or resource in delta $entity payload),
            // we usually don't have a context URL for the resource set or resource:
            // {
            //   "@odata.context":"...", <--- this context URL is for delta entity only
            //   "value": [
            //     {
            //       ...
            //       "NavigationProperty": <--- usually we don't have a context URL for this
            //       [ <--- nested payload start
            //         {...}
            //       ] <--- nested payload end
            //     }
            //    ]
            // }
            //
            // The consequence is that the resource we read out from a nested payload doesn't
            // have an entity metadata builder thus you cannot compute read link, edit link,
            // etc. from the resource object.
            if (this.jsonLightInputContext.ReadingResponse && !this.IsReadingNestedPayload)
            {
                Debug.Assert(this.jsonLightResourceDeserializer.ContextUriParseResult != null,
                    "We should have failed by now if we don't have parse results for context URI.");

                // Validate the context URI parsed from the payload against the entity set and entity type passed in through the API.
                ReaderValidationUtils.ValidateResourceSetOrResourceContextUri(
                    this.jsonLightResourceDeserializer.ContextUriParseResult, this.CurrentScope, true);
            }

            // Get the $select query option from the metadata link, if we have one.
            string selectQueryOption = this.jsonLightResourceDeserializer.ContextUriParseResult == null
                ? null
                : this.jsonLightResourceDeserializer.ContextUriParseResult.SelectQueryOption;

            SelectedPropertiesNode selectedProperties = SelectedPropertiesNode.Create(selectQueryOption);

            if (this.ReadingResourceSet)
            {
                // Store the duplicate property names checker to use it later when reading the resource set end
                // (since we allow resourceSet-related annotations to appear after the resource set's data).
                this.topLevelScope.PropertyAndAnnotationCollector = propertyAndAnnotationCollector;

                bool isReordering = this.jsonLightInputContext.JsonReader is ReorderingJsonReader;

                if (this.ReadingDelta)
                {
                    ODataDeltaResourceSet resourceSet = new ODataDeltaResourceSet();

                    // Read top-level resource set annotations for delta resource sets.
                    this.jsonLightResourceDeserializer.ReadTopLevelResourceSetAnnotations(
                        resourceSet, propertyAndAnnotationCollector, /*forResourceSetStart*/true,
                        /*readAllFeedProperties*/isReordering);
                    this.ReadDeltaResourceSetStart(resourceSet, selectedProperties);

                    this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndArray, JsonNodeType.StartObject);
                }
                else
                {
                    ODataResourceSet resourceSet = new ODataResourceSet();
                    if (!this.IsReadingNestedPayload)
                    {
                        if (!this.readingParameter)
                        {
                            // Skip top-level resource set annotations for nested resource sets.
                            this.jsonLightResourceDeserializer.ReadTopLevelResourceSetAnnotations(
                                resourceSet, propertyAndAnnotationCollector, /*forResourceSetStart*/true,
                                /*readAllFeedProperties*/isReordering);
                        }
                        else
                        {
                            // This line will be used to read the first node of a resource set in Uri operation parameter, The first node is : '['
                            // Node is in following format:
                            // [
                            //      {...}, <------------ complex object.
                            //      {...}, <------------ complex object.
                            // ]
                            this.jsonLightResourceDeserializer.JsonReader.Read();
                        }
                    }

                    this.ReadResourceSetStart(resourceSet, selectedProperties);
                }

                return true;
            }

            this.ReadResourceSetItemStart(propertyAndAnnotationCollector, selectedProperties);
            return true;
        }

        #endregion Start

        #region ResourceSet

        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  Any start node            - The first resource in the resource set
        ///                 JsonNodeType.EndArray     - The end of the resource set
        /// Post-Condition: The reader is positioned over the StartObject node of the first resource in the resource set or
        ///                 on the node following the resource set end in case of an empty resource set
        /// </remarks>
        private bool ReadAtResourceSetStartImplementationSynchronously()
        {
            this.ReadNextResourceSetItem();
            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.Property        if the resource set has further instance or property annotations after the resource set property
        ///                JsonNodeType.EndObject       if the resource set has no further instance or property annotations after the resource set property
        ///                JsonNodeType.EndOfInput      if the resource set is in a Uri operation parameter.
        ///                JsonNodeType.StartArray      if the resource set is a member of an untyped collection followed by a collection
        ///                JsonNodeType.PrimitiveValue  if the resource set is a member of an untyped collection followed by a primitive value
        ///                JsonNodeType.StartObject     if the resource set is a member of an untyped collection followed by a resource
        ///                JsonNodeType.EndArray        if the resource set is the last member of an untyped collection
        /// Post-Condition: JsonNodeType.EndOfInput     for a top-level resource set when not reading a nested payload
        ///                 JsonNodeType.Property       more properties exist on the owning resource after the expanded link containing the resource set
        ///                 JsonNodeType.EndObject      no further properties exist on the owning resource after the expanded link containing the resource set
        ///                 JsonNodeType.EndArray       end of expanded link in request, in this case the resource set doesn't actually own the array object and it won't read it.
        ///                 Any                         in case of expanded resource set in request, this might be the next item in the expanded array, which is not a resource
        /// </remarks>
        private bool ReadAtResourceSetEndImplementationSynchronously()
        {
            Debug.Assert(this.State == ODataReaderState.ResourceSetEnd || this.State == ODataReaderState.DeltaResourceSetEnd, "Not in (delta) resource set end state.");
            Debug.Assert(
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput ||
                (this.ParentScope != null && this.ParentScope.ResourceType.TypeKind == EdmTypeKind.Untyped &&
                    (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue ||
                    this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                    this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                    this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndArray)) ||
                !this.IsTopLevel && !this.jsonLightInputContext.ReadingResponse,
                "Pre-Condition: expected JsonNodeType.EndObject or JsonNodeType.Property, or JsonNodeType.StartArray, JsonNodeTypeStart.Object, or JsonNodeType.EndArray with an untyped collection");

            bool isTopLevelResourceSet = this.IsTopLevel;
            bool isExpandedLinkContent = this.IsExpandedLinkContent;

            this.PopScope(this.State == ODataReaderState.ResourceSetEnd ? ODataReaderState.ResourceSetEnd : ODataReaderState.DeltaResourceSetEnd);

            // When we finish a top-level resource set in a nested payload (inside parameter or delta payload),
            // we can directly turn the reader into Completed state because we don't have any JSON token
            // (e.g., EndObject in a normal resource set payload) left in the stream.
            //
            // Nested resource set payload:
            // [
            //   {...},
            //   ...
            // ]
            // EOF <--- current reader position
            //
            // Normal resource set payload:
            // {
            //   "@odata.context":"...",
            //   ...,
            //   "value": [
            //     {...},
            //     ...
            //   ],
            //   "@odata.nextLink":"..."
            // } <--- current reader position
            // EOF
            //
            // Normal resource set payload as uri operation parameter
            // [
            //   {...},
            //   ...
            // ]
            // EOF <--- current reader position
            if ((this.IsReadingNestedPayload || this.readingParameter) && isTopLevelResourceSet)
            {
                // replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                return false;
            }

            if (isTopLevelResourceSet)
            {
                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");

                // Read the end-object node of the resource set object and position the reader on the next input node
                // This can hit the end of the input.
                this.jsonLightResourceDeserializer.JsonReader.Read();

                // read the end-of-payload
                this.jsonLightResourceDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);

                // replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                return false;
            }
            else if (isExpandedLinkContent)
            {
                // finish reading the expanded link
                this.ReadExpandedNestedResourceInfoEnd(true);
                return true;
            }

            // read the next item in an untyped collection
            this.ReadNextResourceSetItem();
            return true;
        }

        #endregion ResourceSet

        #region Resource
        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded resource set of the nested resource info to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded resource set of the nested resource info to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// </remarks>
        private bool ReadAtResourceStartImplementationSynchronously()
        {
            ODataResourceBase currentResource = this.Item as ODataResourceBase;
            if (currentResource != null && !this.IsReadingNestedPayload)
            {
                this.CurrentResourceState.ResourceTypeFromMetadata = this.ParentScope.ResourceType;
                ODataResourceMetadataBuilder builder =
                    this.jsonLightResourceDeserializer.MetadataContext.GetResourceMetadataBuilderForReader(
                        this.CurrentResourceState,
                        this.jsonLightInputContext.ODataSimplifiedOptions.EnableReadingKeyAsSegment);
                if (builder != currentResource.MetadataBuilder)
                {
                    ODataNestedResourceInfo parentNestInfo = this.ParentNestedInfo;
                    ODataConventionalResourceMetadataBuilder conventionalResourceMetadataBuilder =
                        builder as ODataConventionalResourceMetadataBuilder;

                    // If it's ODataConventionalResourceMetadataBuilder, then it means we need to build nested relation ship for it in containment case
                    if (conventionalResourceMetadataBuilder != null)
                    {
                        if (parentNestInfo != null)
                        {
                            conventionalResourceMetadataBuilder.NameAsProperty = parentNestInfo.Name;
                            conventionalResourceMetadataBuilder.IsFromCollection = parentNestInfo.IsCollection == true;
                            conventionalResourceMetadataBuilder.ODataUri = ResolveODataUriFromContextUrl(parentNestInfo) ?? CurrentScope.ODataUri;
                        }

                        conventionalResourceMetadataBuilder.StartResource();
                    }

                    // Set the metadata builder and parent metadata builder for the resource itself
                    currentResource.MetadataBuilder = builder;
                    if (parentNestInfo != null && parentNestInfo.MetadataBuilder != null)
                    {
                        currentResource.MetadataBuilder.ParentMetadataBuilder = parentNestInfo.MetadataBuilder;
                    }
                }
            }

            if (currentResource == null)
            {
                // Debug.Assert(this.IsExpandedLinkContent || this.CurrentResourceType.IsODataComplexTypeKind() || this.CurrentResourceType.TypeKind == EdmTypeKind.Untyped,
                //    "null or untyped resource can only be reported in an expanded link or in collection of complex instance.");
                this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.PrimitiveValue);

                // There's nothing to read, so move to the end resource state
                this.EndEntry();
            }
            else if (this.CurrentResourceState.FirstNestedResourceInfo != null)
            {
                this.StartNestedResourceInfo(this.CurrentResourceState.FirstNestedResourceInfo);
            }
            else
            {
                // End of resource
                // All the properties have already been read before we acually entered the EntryStart state (since we read as far as we can in any given state).
                this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject);
                this.EndEntry();
            }

            Debug.Assert(
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue &&
                 this.jsonLightResourceDeserializer.JsonReader.Value == null) ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.PrimitiveValue (null) or JsonNodeType.Property or JsonNodeType.EndObject");

            return true;
        }

        /// <summary>
        /// If the nested info has context url, resolve it to ODataUri.
        /// </summary>
        /// <param name="nestedInfo">The nestedInfo to be evaluated.</param>
        /// <returns>The odata uri resolved from context url.</returns>
        private ODataUri ResolveODataUriFromContextUrl(ODataNestedResourceInfo nestedInfo)
        {
            if (nestedInfo != null && nestedInfo.ContextUrl != null)
            {
                var payloadKind = nestedInfo.IsCollection.GetValueOrDefault()
                    ? ODataPayloadKind.ResourceSet
                    : ODataPayloadKind.Resource;
                var odataPath = ODataJsonLightContextUriParser.Parse(
                    this.jsonLightResourceDeserializer.Model,
                    UriUtils.UriToString(nestedInfo.ContextUrl),
                    payloadKind,
                    this.jsonLightResourceDeserializer.MessageReaderSettings.ClientCustomTypeResolver,
                    this.jsonLightResourceDeserializer.JsonLightInputContext.ReadingResponse).Path;

                return new ODataUri() { Path = odataPath };
            }

            return null;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              end of object of the resource
        ///                 JsonNodeType.PrimitiveValue (null)  end of null expanded resource
        /// Post-Condition: The reader is positioned on the first node after the resource's end-object node
        /// </remarks>
        private bool ReadAtResourceEndImplementationSynchronously()
        {
            Debug.Assert(
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue &&
                this.jsonLightResourceDeserializer.JsonReader.Value == null),
                "Pre-Condition: JsonNodeType.EndObject or JsonNodeType.PrimitiveValue (null)");

            // We have to cache these values here, since the PopScope below will destroy them.
            bool isTopLevel = this.IsTopLevel;
            bool isExpandedLinkContent = this.IsExpandedLinkContent;

            this.PopScope(this.State == ODataReaderState.ResourceEnd ? ODataReaderState.ResourceEnd : ODataReaderState.DeletedResourceEnd);

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This can hit the end of the input.
            this.jsonLightResourceDeserializer.JsonReader.Read();

            // Analyze the next Json token to determine whether it is start object (next resource), end array (resource set end) or eof (top-level resource end)
            bool result = true;
            if (isTopLevel)
            {
                // NOTE: we rely on the underlying JSON reader to fail if there is more than one value at the root level.
                Debug.Assert(
                    this.IsReadingNestedPayload ||
                    this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput,
                    "Expected JSON reader to have reached the end of input when not reading a nested payload.");

                // read the end-of-payload
                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
                this.jsonLightResourceDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);
                Debug.Assert(
                    this.IsReadingNestedPayload ||
                    this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput,
                    "Expected JSON reader to have reached the end of input when not reading a nested payload.");

                // replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                result = false;
            }
            else if (isExpandedLinkContent)
            {
                Debug.Assert(
                    this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject || // expanded link resource as last property of the owning resource
                    this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property, // expanded link resource with more properties on the resource
                    "Invalid JSON reader state for reading end of resource in expanded link.");

                // finish reading the expanded link
                this.ReadExpandedNestedResourceInfoEnd(false);
            }
            else
            {
                this.ReadNextResourceSetItem();
            }

            return result;
        }

        #endregion Resource

        #region Primitive

        /// <summary>
        /// Implementation of the reader logic when in state 'Primitive'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         primitive value
        /// Post-Condition: The reader is positioned on the first node after the primitive value
        /// </remarks>
        private bool ReadAtPrimitiveSynchronously()
        {
            Debug.Assert(
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue &&
                (this.jsonLightResourceDeserializer.JsonReader.Value == null || this.CurrentResourceType.TypeKind == EdmTypeKind.Untyped),
                "Pre-Condition: JsonNodeType.PrimitiveValue (null or untyped)");

            this.PopScope(ODataReaderState.Primitive);

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This should never hit the end of the input.
            this.jsonLightResourceDeserializer.JsonReader.Read();
            this.ReadNextResourceSetItem();
            return true;
        }

        #endregion Primitive

        #region DeletedEntry

        /// <summary>
        /// Implementation of the reader logic when in state 'DeletedEntryStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded resource set of the nested resource info to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded resource of the nested resource info to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded resource set of the nested resource info to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// </remarks>
        private bool ReadAtDeletedResourceStartImplementationSynchronously()
        {
            Debug.Assert(this.CurrentScope is JsonLightDeletedResourceScope);

            if (((JsonLightDeletedResourceScope)(this.CurrentScope)).Is40DeletedResource)
            {
                this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject);
                this.EndEntry();
                return true;
            }

            return this.ReadAtResourceStartImplementationSynchronously();
        }

        #endregion DeletedEntry

        #region (Deleted)Link

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaLink'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The next annotation.
        ///                 JsonNodeType.EndObject              No more other annotation or property in the link.
        /// Post-Condition: The reader is positioned on the first node after the link's end-object node.
        /// </remarks>
        private bool ReadAtDeltaLinkImplementationSynchronously()
        {
            return this.EndDeltaLink(ODataReaderState.DeltaLink);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaDeletedLink'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The next annotation.
        ///                 JsonNodeType.EndObject              No more other annotation or property in the link.
        /// Post-Condition: The reader is positioned on the first node after the link's end-object node.
        /// </remarks>
        private bool ReadAtDeltaDeletedLinkImplementationSynchronously()
        {
            return this.EndDeltaLink(ODataReaderState.DeltaDeletedLink);
        }

        /// <summary>
        /// Reads the end of the delta(deleted)link.
        /// </summary>
        /// <param name="readerState">The state of the link or deleted link being completed.</param>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The next annotation.
        ///                 JsonNodeType.EndObject              No more other annotation or property in the link.
        /// Post-Condition: The reader is positioned on the first node after the link's end-object node.
        /// </remarks>
        private bool EndDeltaLink(ODataReaderState readerState)
        {
            Debug.Assert(readerState == ODataReaderState.DeltaLink || readerState == ODataReaderState.DeltaDeletedLink, "ReadAtDeltaLinkImplementation called when not on delta(deleted)link");
            Debug.Assert(
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject,
                "Not positioned at end of object after reading delta link");

            this.PopScope(readerState);

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This should never hit the end of the input.
            this.jsonLightResourceDeserializer.JsonReader.Read();
            this.ReadNextResourceSetItem();
            return true;
        }

        #endregion (Deleted)Link

        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfoStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            start of an expanded resource
        ///                 JsonNodeType.StartArray             start of an expanded resource set
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null resource
        ///                 JsonNodeType.Property               deferred link with more properties in owning resource
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning resource or
        ///                                                     reporting projected navigation links missing in the payload
        /// Post-Condition: JsonNodeType.StartArray:            start of expanded resource
        ///                 JsonNodeType.StartObject            start of expanded resource set
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null resource
        ///                 JsonNodeType.Property               deferred link with more properties in owning resource
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning resource or
        ///                                                     reporting projected navigation links missing in the payload
        /// </remarks>
        private bool ReadAtNestedResourceInfoStartImplementationSynchronously()
        {
            Debug.Assert(
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue &&
                this.jsonLightResourceDeserializer.JsonReader.Value == null,
                "Pre-Condition: expected JsonNodeType.Property, JsonNodeType.EndObject, JsonNodeType.StartObject, JsonNodeType.StartArray or JsonNodeType.Primitive (null)");

            ODataNestedResourceInfo currentLink = this.CurrentNestedResourceInfo;

            IODataJsonLightReaderResourceState parentResourceState = (IODataJsonLightReaderResourceState)this.ParentScope;

            if (this.jsonLightInputContext.ReadingResponse)
            {
                // If we are reporting a nested resource info that was projected but not included in the payload,
                // simply change state to NestedResourceInfoEnd.
                if (parentResourceState.ProcessingMissingProjectedNestedResourceInfos)
                {
                    this.ReplaceScope(ODataReaderState.NestedResourceInfoEnd);
                }
                else if (!this.jsonLightResourceDeserializer.JsonReader.IsOnValueNode())
                {
                    // Deferred link (nested resource info which doesn't have a value and is in the response)
                    ReaderUtils.CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(
                        parentResourceState.PropertyAndAnnotationCollector, currentLink);
                    this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

                    // Record that we read the link on the parent resource's scope.
                    parentResourceState.NavigationPropertiesRead.Add(currentLink.Name);

                    this.ReplaceScope(ODataReaderState.NestedResourceInfoEnd);
                }
                else if (!currentLink.IsCollection.Value)
                {
                    // We should get here only for declared or undeclared navigation properties.
                    Debug.Assert(this.CurrentResourceType != null || this.CurrentNestedResourceInfo.Name != null,
                        "We must have a declared navigation property to read expanded links.");

                    // Expanded resource
                    ReaderUtils.CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(
                        parentResourceState.PropertyAndAnnotationCollector, currentLink);
                    this.ReadExpandedNestedResourceInfoStart(currentLink);
                }
                else
                {
                    // Expanded resource set
                    ReaderUtils.CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(
                        parentResourceState.PropertyAndAnnotationCollector, currentLink);

                    // We store the precreated expanded resource set in the nested resource info since it carries the annotations for it.
                    ODataJsonLightReaderNestedResourceInfo nestedResourceInfo =
                        this.CurrentJsonLightNestedResourceInfoScope.ReaderNestedResourceInfo;
                    Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
                    Debug.Assert(nestedResourceInfo.NestedResourceSet != null,
                        "We must have a precreated expanded resource set already.");
                    JsonLightResourceBaseScope parentScope = (JsonLightResourceBaseScope)this.ParentScope;
                    SelectedPropertiesNode parentSelectedProperties = parentScope.SelectedProperties;
                    Debug.Assert(parentSelectedProperties != null, "parentProjectedProperties != null");

                    ODataResourceSet resourceSet = nestedResourceInfo.NestedResourceSet as ODataResourceSet;
                    if (resourceSet != null)
                    {
                        this.ReadResourceSetStart(resourceSet, parentSelectedProperties.GetSelectedPropertiesForNavigationProperty(parentScope.ResourceType, currentLink.Name));
                    }
                    else
                    {
                        ODataDeltaResourceSet deltaResourceSet = nestedResourceInfo.NestedResourceSet as ODataDeltaResourceSet;
                        Debug.Assert(deltaResourceSet != null, "Nested recource collection is not a resource set or a delta resource set");
                        this.ReadDeltaResourceSetStart(deltaResourceSet, parentSelectedProperties.GetSelectedPropertiesForNavigationProperty(parentScope.ResourceType, currentLink.Name));
                    }
                }
            }
            else
            {
                // Navigation link in request - report entity reference links and then possible expanded value.
                ReaderUtils.CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(
                    parentResourceState.PropertyAndAnnotationCollector,
                    currentLink);

                this.ReadNextNestedResourceInfoContentItemInRequest();
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfoEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject:         nested resource info is last property in owning resource or
        ///                                                 reporting projected navigation links missing in the payload
        ///                 JsonNodeType.Property:          there are more properties after the nested resource info in the owning resource
        /// Post-Condition: JsonNodeType.StartObject        start of the expanded resource nested resource info to read next
        ///                 JsonNodeType.StartArray         start of the expanded resource set nested resource info to read next
        ///                 JsonNoteType.Primitive (null)   expanded null resource nested resource info to read next
        ///                 JsonNoteType.Property           property after deferred link or entity reference link
        ///                 JsonNodeType.EndObject          end of the parent resource
        /// </remarks>
        private bool ReadAtNestedResourceInfoEndImplementationSynchronously()
        {
            this.jsonLightResourceDeserializer.AssertJsonCondition(
                JsonNodeType.EndObject,
                JsonNodeType.Property);

            this.PopScope(ODataReaderState.NestedResourceInfoEnd);
            Debug.Assert(this.State == ODataReaderState.ResourceStart || this.State == ODataReaderState.DeletedResourceStart, "this.State == ODataReaderState.ResourceStart");

            ODataJsonLightReaderNestedResourceInfo readerNestedResourceInfo = null;
            IODataJsonLightReaderResourceState resourceState = this.CurrentResourceState;

            if (this.jsonLightInputContext.ReadingResponse &&
                resourceState.ProcessingMissingProjectedNestedResourceInfos)
            {
                // We are reporting navigation links that were projected but missing from the payload
                readerNestedResourceInfo = resourceState.Resource.MetadataBuilder.GetNextUnprocessedNavigationLink();
            }
            else
            {
                readerNestedResourceInfo = this.jsonLightResourceDeserializer.ReadResourceContent(resourceState);
            }

            if (readerNestedResourceInfo == null)
            {
                // End of the resource
                this.EndEntry();
            }
            else
            {
                // Next nested resource info on the resource
                this.StartNestedResourceInfo(readerNestedResourceInfo);
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'EntityReferenceLink'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// This method doesn't move the reader
        /// Pre-Condition:  JsonNodeType.EndObject:         expanded link property is last property in owning resource
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning resource
        ///                 Any:                            expanded collection link - the node after the entity reference link.
        /// Post-Condition: JsonNodeType.EndObject:         expanded link property is last property in owning resource
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning resource
        ///                 Any:                            expanded collection link - the node after the entity reference link.
        /// </remarks>
        private bool ReadAtEntityReferenceLinkSynchronously()
        {
            this.PopScope(ODataReaderState.EntityReferenceLink);
            Debug.Assert(this.State == ODataReaderState.NestedResourceInfoStart,
                "this.State == ODataReaderState.NestedResourceInfoStart");

            this.ReadNextNestedResourceInfoContentItemInRequest();
            return true;
        }

        #endregion ReadAt<>Synchronously methods

        /// <summary>
        /// Reads the next entity or complex value (or primitive or collection value for an untyped collection) in a resource set.
        /// </summary>
        private void ReadNextResourceSetItem()
        {
            Debug.Assert(this.State == ODataReaderState.ResourceSetStart ||
                this.State == ODataReaderState.DeltaResourceSetStart,
                "Reading a resource set item while not in a ResourceSetStart or DeltaResourceSetStart state.");
            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndArray, JsonNodeType.PrimitiveValue,
                JsonNodeType.StartObject, JsonNodeType.StartArray);

            // End of item in a resource set
            switch (this.jsonLightResourceDeserializer.JsonReader.NodeType)
            {
                case JsonNodeType.StartObject:
                    // another resource in a resource set
                    this.ReadResourceSetItemStart( /*propertyAndAnnotationCollector*/
                        null, this.CurrentJsonLightResourceSetScope.SelectedProperties);
                    break;
                case JsonNodeType.StartArray:
                    // we are at the start of a nested resource set
                    this.ReadResourceSetStart(new ODataResourceSet(), SelectedPropertiesNode.EntireSubtree);
                    break;
                case JsonNodeType.EndArray:
                    // we are at the end of a resource set
                    this.ReadResourceSetEnd();
                    break;
                case JsonNodeType.PrimitiveValue:
                    // we are at a null value, or a non-null primitive value within an untyped collection
                    object primitiveValue = this.jsonLightResourceDeserializer.JsonReader.Value;
                    if (primitiveValue != null && this.CurrentResourceType.TypeKind == EdmTypeKind.Untyped)
                    {
                        this.EnterScope(new JsonLightPrimitiveScope(new ODataPrimitiveValue(primitiveValue),
                            this.CurrentNavigationSource, this.CurrentResourceType, this.CurrentScope.ODataUri));
                    }
                    else
                    {
                        // null resource (ReadResourceStart will raise the appropriate error for a non-null primitive value)
                        this.ReadResourceSetItemStart( /*propertyAndAnnotationCollector*/
                            null, this.CurrentJsonLightResourceSetScope.SelectedProperties);
                    }

                    break;
                default:
                    throw new ODataException(
                        ODataErrorStrings.ODataJsonReader_CannotReadResourcesOfResourceSet(
                            this.jsonLightResourceDeserializer.JsonReader.NodeType));
            }
        }

        #region Read<> methods

        #region ResourceSet

        /// <summary>
        /// Reads the start of the JSON array for the content of the resource set and sets up the reader state correctly.
        /// </summary>
        /// <param name="resourceSet">The resource set to read the contents for.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        /// <remarks>
        /// Pre-Condition:  The first node of the resource set property value; this method will throw if the node is not
        ///                 JsonNodeType.StartArray
        /// Post-Condition: The reader is positioned on the first item in the resource set, or on the end array of the resource set.
        /// </remarks>
        private void ReadResourceSetStart(ODataResourceSet resourceSet, SelectedPropertiesNode selectedProperties)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            this.jsonLightResourceDeserializer.ReadResourceSetContentStart();
            IJsonReader jsonReader = this.jsonLightResourceDeserializer.JsonReader;
            if (jsonReader.NodeType != JsonNodeType.EndArray && jsonReader.NodeType != JsonNodeType.StartObject
                && !(jsonReader.NodeType == JsonNodeType.PrimitiveValue && jsonReader.Value == null)
                && !(this.CurrentResourceType.TypeKind == EdmTypeKind.Untyped && (jsonReader.NodeType == JsonNodeType.PrimitiveValue || jsonReader.NodeType == JsonNodeType.StartArray)))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_InvalidNodeTypeForItemsInResourceSet(jsonReader.NodeType));
            }

            this.EnterScope(new JsonLightResourceSetScope(resourceSet, this.CurrentNavigationSource,
                this.CurrentResourceType, selectedProperties, this.CurrentScope.ODataUri, /*isDelta*/ false));
        }

        /// <summary>
        /// Reads the end of the current resource set.
        /// </summary>
        private void ReadResourceSetEnd()
        {
            Debug.Assert(this.State == ODataReaderState.ResourceSetStart || this.State == ODataReaderState.DeltaResourceSetStart,
                "Not in ResourceSetStart or DeltaResourceSetStart state when reading end of (delta) resource set.");
            Debug.Assert(this.Item is ODataResourceSetBase, "Current Item is not ResourceSetBase");

            this.jsonLightResourceDeserializer.ReadResourceSetContentEnd();

            ODataJsonLightReaderNestedResourceInfo expandedNestedResourceInfo = null;
            JsonLightNestedResourceInfoScope parentNestedResourceInfoScope = (JsonLightNestedResourceInfoScope)this.ExpandedLinkContentParentScope;
            if (parentNestedResourceInfoScope != null)
            {
                expandedNestedResourceInfo = parentNestedResourceInfoScope.ReaderNestedResourceInfo;
            }

            if (!this.IsReadingNestedPayload && (this.IsExpandedLinkContent || this.IsTopLevel))
            {
                // Temp ban reading the instance annotation after the resource set in parameter payload. (!this.IsReadingNestedPayload => !this.readingParameter)
                // Nested resource set payload won't have a NextLink annotation after the resource set itself since the payload is NOT pageable.
                this.jsonLightResourceDeserializer.ReadNextLinkAnnotationAtResourceSetEnd(this.Item as ODataResourceSetBase,
                    expandedNestedResourceInfo, this.topLevelScope.PropertyAndAnnotationCollector);
            }

            this.ReplaceScope(this.State == ODataReaderState.ResourceSetStart ? ODataReaderState.ResourceSetEnd : ODataReaderState.DeltaResourceSetEnd);
        }

        #endregion ResourceSet

        #region NestedResourceInfo

        /// <summary>
        /// Reads the start of an expanded resource (null or non-null).
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info that is being expanded.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            The start of the resource object
        ///                 JsonNodeType.PrimitiveValue (null)  The null resource value
        /// Post-Condition: JsonNodeType.StartObject            Start of expanded resource of the nested resource info to read next
        ///                 JsonNodeType.StartArray             Start of expanded resource set of the nested resource info to read next
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next, or the null value of the current null resource
        ///                 JsonNodeType.Property               Property after deferred link or expanded entity reference
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// </remarks>
        private void ReadExpandedNestedResourceInfoStart(ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");

            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                Debug.Assert(this.jsonLightResourceDeserializer.JsonReader.Value == null,
                    "If a primitive value is representing an expanded resource its value must be null.");

                var structuralProperty =
                    this.CurrentJsonLightNestedResourceInfoScope.ReaderNestedResourceInfo.StructuralProperty;
                if (structuralProperty != null && !structuralProperty.Type.IsNullable)
                {
                    ODataNullValueBehaviorKind nullValueReadBehaviorKind =
                        this.jsonLightResourceDeserializer.ReadingResponse
                            ? ODataNullValueBehaviorKind.Default
                            : this.jsonLightResourceDeserializer.Model.NullValueReadBehaviorKind(structuralProperty);

                    if (nullValueReadBehaviorKind == ODataNullValueBehaviorKind.Default)
                    {
                        throw new ODataException(
                            Strings.ReaderValidationUtils_NullNamedValueForNonNullableType(nestedResourceInfo.Name,
                                structuralProperty.Type.FullName()));
                    }
                }

                // Expanded null resource
                // The expected type and expected navigation source for an expanded resource are the same as for the nested resource info around it.
                this.EnterScope(new JsonLightResourceScope(ODataReaderState.ResourceStart, /*resource*/ null,
                    this.CurrentNavigationSource, this.CurrentResourceType, /*propertyAndAnnotationCollector*/null,
                    /*projectedProperties*/null, this.CurrentScope.ODataUri));
            }
            else
            {
                // Expanded resource
                // The expected type for an expanded resource is the same as for the nested resource info around it.
                JsonLightResourceBaseScope parentScope = (JsonLightResourceBaseScope)this.ParentScope;
                SelectedPropertiesNode parentSelectedProperties = parentScope.SelectedProperties;
                Debug.Assert(parentSelectedProperties != null, "parentProjectedProperties != null");
                this.ReadResourceSetItemStart(/*propertyAndAnnotationCollector*/ null, parentSelectedProperties.GetSelectedPropertiesForNavigationProperty(parentScope.ResourceType, nestedResourceInfo.Name));
            }
        }

        /// <summary>
        /// Verifies that the current item is an <see cref="ODataNestedResourceInfo"/> instance,
        /// sets the cardinality of the link (IsCollection property) and moves the reader
        /// into state 'NestedResourceInfoEnd'.
        /// </summary>
        /// <param name="isCollection">A flag indicating whether the link represents a collection or not.</param>
        private void ReadExpandedNestedResourceInfoEnd(bool isCollection)
        {
            Debug.Assert(this.State == ODataReaderState.NestedResourceInfoStart,
                "this.State == ODataReaderState.NestedResourceInfoStart");
            this.CurrentNestedResourceInfo.IsCollection = isCollection;

            // Record that we read the link on the parent resource's scope.
            IODataJsonLightReaderResourceState parentResourceState = (IODataJsonLightReaderResourceState)this.ParentScope;
            parentResourceState.NavigationPropertiesRead.Add(this.CurrentNestedResourceInfo.Name);

            // replace the 'NestedResourceInfoStart' scope with the 'NestedResourceInfoEnd' scope
            this.ReplaceScope(ODataReaderState.NestedResourceInfoEnd);
        }

        #endregion NestedResourceInfo

        #region Resource

        /// <summary>
        /// Reads the start of a (standard, delta, primitive, or null) resource and sets up the reader state correctly
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the resource;
        /// or null if a new one should be created.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            If the resource is in a resource set - the start of the resource object
        ///                 JsonNodeType.Property               If the resource is a top-level resource and has at least one property
        ///                 JsonNodeType.EndObject              If the resource is a top-level resource and has no properties
        /// Post-Condition: JsonNodeType.StartObject            Start of expanded resource of the nested resource info to read next
        ///                 JsonNodeType.StartArray             Start of expanded resource set of the nested resource info to read next
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource of the nested resource info to read next
        ///                 JsonNodeType.Property               Property after deferred link or expanded entity reference
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// </remarks>
        private void ReadResourceSetItemStart(PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            SelectedPropertiesNode selectedProperties)
        {
            IEdmNavigationSource source = this.CurrentNavigationSource;
            IEdmStructuredType resourceType = this.CurrentResourceType;

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.StartObject, JsonNodeType.Property,
                JsonNodeType.EndObject, JsonNodeType.PrimitiveValue);

            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                object primitiveValue = this.jsonLightResourceDeserializer.JsonReader.Value;
                if (primitiveValue != null)
                {
                    // primitive value in an untyped collection
                    if (this.CurrentResourceType.TypeKind == EdmTypeKind.Untyped)
                    {
                        this.EnterScope(new JsonLightPrimitiveScope(new ODataPrimitiveValue(primitiveValue),
                            this.CurrentNavigationSource, this.CurrentResourceType, this.CurrentScope.ODataUri));
                    }
                    else
                    {
                        throw new ODataException(Strings.ODataJsonLightReader_UnexpectedPrimitiveValueForODataResource);
                    }
                }
                else
                {
                    // null resource
                    this.EnterScope(new JsonLightResourceScope(ODataReaderState.ResourceStart, /*resource*/ null,
                        this.CurrentNavigationSource, this.CurrentResourceType, /*propertyAndAnnotationCollector*/null,
                        /*projectedProperties*/null, this.CurrentScope.ODataUri));
                }

                return;
            }

            // If the reader is on StartObject then read over it. This happens for entries in resource set.
            // For top-level entries the reader will be positioned on the first resource property (after odata.context if it was present).
            if (this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject)
            {
                this.jsonLightResourceDeserializer.JsonReader.Read();
            }

            ODataDeltaKind resourceKind = ODataDeltaKind.Resource;
            if (this.ReadingResourceSet || this.IsExpandedLinkContent || this.ReadingDelta)
            {
                string contextUriStr =
                    this.jsonLightResourceDeserializer.ReadContextUriAnnotation(ODataPayloadKind.Resource,
                        propertyAndAnnotationCollector, false);
                if (contextUriStr != null)
                {
                    contextUriStr =
                        UriUtils.UriToString(this.jsonLightResourceDeserializer.ProcessUriFromPayload(contextUriStr));
                    var parseResult = ODataJsonLightContextUriParser.Parse(
                        this.jsonLightResourceDeserializer.Model,
                        contextUriStr,
                        this.ReadingDelta ? ODataPayloadKind.Delta : ODataPayloadKind.Resource,
                        this.jsonLightResourceDeserializer.MessageReaderSettings.ClientCustomTypeResolver,
                        this.jsonLightInputContext.ReadingResponse);
                    if (parseResult != null)
                    {
                        resourceKind = parseResult.DeltaKind;
                        if (this.jsonLightInputContext.ReadingResponse)
                        {
                            // a top-level (deleted) resource in a delta response can come from any entity set
                            if (this.ReadingDelta && this.IsTopLevel && (resourceKind == ODataDeltaKind.Resource || resourceKind == ODataDeltaKind.DeletedEntry))
                            {
                                IEdmStructuredType parsedType = parseResult.EdmType as IEdmStructuredType;
                                if (parsedType != null)
                                {
                                    resourceType = parsedType;
                                    source = parseResult.NavigationSource;
                                }
                            }
                            else
                            {
                                ReaderValidationUtils.ValidateResourceSetOrResourceContextUri(parseResult, this.CurrentScope,
                                    false);
                            }
                        }
                    }
                }
            }

            // If this is a resource in a delta resource set, check to see if it's a 4.01 deleted resource
            ODataDeletedResource deletedResource = null;
            if (this.ReadingDelta && (resourceKind == ODataDeltaKind.Resource || resourceKind == ODataDeltaKind.DeletedEntry))
            {
                deletedResource = this.jsonLightResourceDeserializer.IsDeletedResource();
                if (deletedResource != null)
                {
                    resourceKind = ODataDeltaKind.DeletedEntry;
                }
            }

            switch (resourceKind)
            {
                case ODataDeltaKind.None:
                case ODataDeltaKind.Resource:
                    // Setup the new resource state
                    this.StartResource(source, resourceType, propertyAndAnnotationCollector, selectedProperties);

                    // Start reading the resource up to the first nested resource info
                    this.StartReadingResource();

                    break;

                case ODataDeltaKind.ResourceSet:
                    this.ReadAtResourceSetStartImplementation();
                    break;

                case ODataDeltaKind.DeletedEntry:
                    // OData 4.0 deleted entry
                    if (deletedResource == null)
                    {
                        deletedResource = this.jsonLightResourceDeserializer.ReadDeletedEntry();
                        this.StartDeletedResource(
                            deletedResource,
                            source,
                            resourceType,
                            propertyAndAnnotationCollector,
                            selectedProperties,
                            true /*is 4.0 Deleted Resource*/);
                    }
                    else // OData 4.01 deleted entry
                    {
                        this.StartDeletedResource(
                            deletedResource,
                            source,
                            resourceType,
                            propertyAndAnnotationCollector,
                            selectedProperties);

                        // Start reading the resource up to the first nested resource info
                        this.StartReadingResource();
                    }

                    break;

                case ODataDeltaKind.DeletedLink:
                    this.StartDeltaLink(ODataReaderState.DeltaDeletedLink);
                    break;

                case ODataDeltaKind.Link:
                    this.StartDeltaLink(ODataReaderState.DeltaLink);
                    break;

                default:
                    Debug.Assert(true, "Uknown ODataDeltaKind " + resourceKind.ToString());
                    break;
            }
        }

        #endregion Resource

        #region DeltaResourceSet
        /// <summary>
        /// Reads the start of the JSON array for the content of the delta resource set and sets up the reader state correctly.
        /// </summary>
        /// <param name="deltaResourceSet">The delta resource set to read the contents for.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        /// <remarks>
        /// Pre-Condition:  The first node of the resource set property value; this method will throw if the node is not
        ///                 JsonNodeType.StartArray
        /// Post-Condition: The reader is positioned on the first item in the resource set, or on the end array of the resource set.
        /// </remarks>
        private void ReadDeltaResourceSetStart(ODataDeltaResourceSet deltaResourceSet, SelectedPropertiesNode selectedProperties)
        {
            Debug.Assert(deltaResourceSet != null, "resourceSet != null");

            this.jsonLightResourceDeserializer.ReadResourceSetContentStart();
            IJsonReader jsonReader = this.jsonLightResourceDeserializer.JsonReader;
            if (jsonReader.NodeType != JsonNodeType.EndArray && jsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_InvalidNodeTypeForItemsInResourceSet(jsonReader.NodeType));
            }

            Debug.Assert(this.CurrentResourceType is IEdmEntityType, "Delta resource type is not an entity");

            this.EnterScope(new JsonLightResourceSetScope(
                deltaResourceSet,
                this.CurrentNavigationSource,
                this.CurrentResourceType as IEdmEntityType,
                selectedProperties,
                this.CurrentScope.ODataUri,
                /*isDelta*/ true));
        }

        #endregion DeltaResourceSet

        #endregion Read<> methods

        #region private methods

        /// <summary>
        /// Read the resource up to the first nested resource info.
        /// </summary>
        private void StartReadingResource()
        {
            ODataResourceBase currentResource = this.Item as ODataResourceBase;

            // Read the odata.type annotation.
            this.jsonLightResourceDeserializer.ReadResourceTypeName(this.CurrentResourceState);

            // Resolve the type name
            this.ApplyResourceTypeNameFromPayload(currentResource.TypeName);

            // Validate type with resource set validator if available and not reading top-level delta resource set
            if (this.CurrentResourceSetValidator != null && !(this.ReadingDelta && this.CurrentResourceDepth == 0))
            {
                this.CurrentResourceSetValidator.ValidateResource(this.CurrentResourceType);
            }

            this.CurrentResourceState.FirstNestedResourceInfo =
                this.jsonLightResourceDeserializer.ReadResourceContent(this.CurrentResourceState);
            this.jsonLightResourceDeserializer.AssertJsonCondition(
                JsonNodeType.Property,
                JsonNodeType.StartObject,
                JsonNodeType.StartArray,
                JsonNodeType.EndObject,
                JsonNodeType.PrimitiveValue);
        }

        /// <summary>
        /// Reads the next item in a nested resource info content in a request payload.
        /// </summary>
        private void ReadNextNestedResourceInfoContentItemInRequest()
        {
            Debug.Assert(this.CurrentScope.State == ODataReaderState.NestedResourceInfoStart,
                "Must be on 'NestedResourceInfoStart' scope.");

            ODataJsonLightReaderNestedResourceInfo nestedResourceInfo =
                this.CurrentJsonLightNestedResourceInfoScope.ReaderNestedResourceInfo;
            if (nestedResourceInfo.HasEntityReferenceLink)
            {
                this.EnterScope(new Scope(ODataReaderState.EntityReferenceLink,
                    nestedResourceInfo.ReportEntityReferenceLink(), null, null, this.CurrentScope.ODataUri));
            }
            else if (nestedResourceInfo.HasValue)
            {
                if (nestedResourceInfo.NestedResourceInfo.IsCollection == true)
                {
                    // because this is a request, there is no $select query option.
                    SelectedPropertiesNode selectedProperties = SelectedPropertiesNode.EntireSubtree;
                    ODataDeltaResourceSet deltaResourceSet = nestedResourceInfo.NestedResourceSet as ODataDeltaResourceSet;
                    if (deltaResourceSet != null)
                    {
                        this.ReadDeltaResourceSetStart(deltaResourceSet, selectedProperties);
                    }
                    else
                    {
                        ODataResourceSet resourceSet = nestedResourceInfo.NestedResourceSet as ODataResourceSet;
                        this.ReadResourceSetStart(resourceSet ?? new ODataResourceSet(), selectedProperties);
                    }
                }
                else
                {
                    this.ReadExpandedNestedResourceInfoStart(nestedResourceInfo.NestedResourceInfo);
                }
            }
            else
            {
                // replace the 'NestedResourceInfoStart' scope with the 'NestedResourceInfoEnd' scope
                this.ReplaceScope(ODataReaderState.NestedResourceInfoEnd);
            }
        }

        /// <summary>
        /// Starts the resource, initializing the scopes and such. This method starts a non-null resource only.
        /// </summary>
        /// <param name="source">The source for the resource</param>
        /// <param name="resourceType">The entity type of the resource</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the resource;
        /// or null if a new one should be created.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        private void StartResource(IEdmNavigationSource source, IEdmStructuredType resourceType, PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            SelectedPropertiesNode selectedProperties)
        {
            this.EnterScope(new JsonLightResourceScope(
                ODataReaderState.ResourceStart,
                ReaderUtils.CreateNewResource(),
                source,
                resourceType,
                propertyAndAnnotationCollector ?? this.jsonLightInputContext.CreatePropertyAndAnnotationCollector(),
                selectedProperties,
                this.CurrentScope.ODataUri));
        }

        /// <summary>
        /// Starts the deleted resource, initializing the scopes and such. This method starts a non-null resource only.
        /// </summary>
        /// <param name="deletedResource">The deletedResource to be created.</param>
        /// <param name="source">The navigation source of the deleted resource.</param>
        /// <param name="resourceType">The entity type of the deleted resource.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the resource;
        /// or null if a new one should be created.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        /// <param name="is40DeletedResource">Whether the current resource being read is a 4.0-style deleted resource.</param>
        private void StartDeletedResource(ODataDeletedResource deletedResource, IEdmNavigationSource source, IEdmStructuredType resourceType, PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            SelectedPropertiesNode selectedProperties, bool is40DeletedResource = false)
        {
            this.EnterScope(new JsonLightDeletedResourceScope(
                ODataReaderState.DeletedResourceStart,
                deletedResource,
                source,
                resourceType,
                propertyAndAnnotationCollector ?? this.jsonLightInputContext.CreatePropertyAndAnnotationCollector(),
                selectedProperties,
                this.CurrentScope.ODataUri,
                is40DeletedResource));
        }

        /// <summary>
        /// Starts the (deleted) link, initializing the scopes and such. This method starts a non-null resource only.
        /// </summary>
        /// <param name="state">The reader state to switch to.</param>
        private void StartDeltaLink(ODataReaderState state)
        {
            Debug.Assert(
                state == ODataReaderState.DeltaLink || state == ODataReaderState.DeltaDeletedLink,
                "state must be either DeltaResource or DeltaDeletedEntry or DeltaLink or DeltaDeletedLink.");
            Debug.Assert(this.CurrentResourceType is IEdmEntityType, "DeltaLink is not from an entity type");

            ODataDeltaLinkBase link;
            if (state == ODataReaderState.DeltaLink)
            {
                link = new ODataDeltaLink(null, null, null);
            }
            else
            {
                link = new ODataDeltaDeletedLink(null, null, null);
            }

            this.EnterScope(new JsonLightDeltaLinkScope(
                state,
                link,
                this.CurrentNavigationSource,
                this.CurrentResourceType as IEdmEntityType,
                this.CurrentScope.ODataUri));

            this.jsonLightResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

            // Read source property.
            this.jsonLightResourceDeserializer.ReadDeltaLinkSource(link);

            // Read relationship property.
            this.jsonLightResourceDeserializer.ReadDeltaLinkRelationship(link);

            // Read target property.
            this.jsonLightResourceDeserializer.ReadDeltaLinkTarget(link);

            Debug.Assert(this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject, "Unexpected content in a delta (deleted) link");
        }

        /// <summary>
        /// Starts the nested resource info.
        /// Does metadata validation of the nested resource info and sets up the reader to report it.
        /// </summary>
        /// <param name="readerNestedResourceInfo">The nested resource info for the nested resource info to start.</param>
        private void StartNestedResourceInfo(ODataJsonLightReaderNestedResourceInfo readerNestedResourceInfo)
        {
            Debug.Assert(readerNestedResourceInfo != null, "readerNestedResourceInfo != null");
            ODataNestedResourceInfo nestedResourceInfo = readerNestedResourceInfo.NestedResourceInfo;
            IEdmProperty nestedProperty = readerNestedResourceInfo.NestedProperty;
            IEdmStructuredType targetResourceType = readerNestedResourceInfo.NestedResourceType;

            Debug.Assert(
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.jsonLightResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue &&
                this.jsonLightResourceDeserializer.JsonReader.Value == null,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.Primitive (null), or JsonNodeType.Property, JsonNodeType.EndObject");
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(!string.IsNullOrEmpty(nestedResourceInfo.Name), "Navigation links must have a name.");
            Debug.Assert(nestedProperty == null || nestedResourceInfo.Name == nestedProperty.Name,
                "The navigation property must match the nested resource info.");

            // we are at the beginning of a link
            if (targetResourceType == null && nestedProperty != null)
            {
                IEdmTypeReference nestedPropertyType = nestedProperty.Type;
                targetResourceType = nestedPropertyType.IsCollection()
                    ? nestedPropertyType.AsCollection().ElementType().AsStructured().StructuredDefinition()
                    : nestedPropertyType.AsStructured().StructuredDefinition();
            }

            // Since we don't have the entity metadata builder for the resource read out from a nested payload
            // as stated in ReadAtResourceSetEndImplementationSynchronously(), we cannot access it here which otherwise
            // would lead to an exception.
            if (this.jsonLightInputContext.ReadingResponse && !this.IsReadingNestedPayload
                && (targetResourceType == null || targetResourceType.IsStructuredOrStructuredCollectionType()))
            {
                // Hookup the metadata builder to the nested resource info.
                // Note that we set the metadata builder even when navigationProperty is null, which is the case when the link is undeclared.
                // For undeclared links, we will apply conventional metadata evaluation just as declared links.
                this.CurrentResourceState.ResourceTypeFromMetadata = this.ParentScope.ResourceType;
                ODataResourceMetadataBuilder resourceMetadataBuilder =
                    this.jsonLightResourceDeserializer.MetadataContext.GetResourceMetadataBuilderForReader(
                        this.CurrentResourceState,
                        this.jsonLightInputContext.ODataSimplifiedOptions.EnableReadingKeyAsSegment);
                nestedResourceInfo.MetadataBuilder = resourceMetadataBuilder;
            }

            Debug.Assert(
                this.CurrentNavigationSource != null || this.readingParameter ||
                this.CurrentNavigationSource == null && this.CurrentScope.ResourceType.IsODataComplexTypeKind(),
                "Json requires an navigation source when not reading parameter.");

            IEdmNavigationProperty navigationProperty = readerNestedResourceInfo.NavigationProperty;

            IEdmNavigationSource navigationSource;

            // Since we are entering a nested info scope, check whether the current resource is derived type in order to correctly further property or navigation property.
            JsonLightResourceBaseScope currentScope = this.CurrentScope as JsonLightResourceBaseScope;
            ODataUri odataUri = this.CurrentScope.ODataUri.Clone();
            ODataPath odataPath = odataUri.Path ?? new ODataPath();

            if (currentScope != null && currentScope.ResourceTypeFromMetadata != currentScope.ResourceType)
            {
                odataPath.Add(new TypeSegment(currentScope.ResourceType, null));
            }

            if (navigationProperty == null)
            {
                navigationSource = this.CurrentNavigationSource;
            }
            else
            {
                IEdmPathExpression bindingPath;
                navigationSource = this.CurrentNavigationSource == null
                    ? null
                    : this.CurrentNavigationSource.FindNavigationTarget(navigationProperty,
                        BindingPathHelper.MatchBindingPath, odataPath.ToList(), out bindingPath);
            }

            if (navigationProperty != null)
            {
                if (navigationSource is IEdmContainedEntitySet)
                {
                    if (TryAppendEntitySetKeySegment(ref odataPath))
                    {
                        odataPath = odataPath.AppendNavigationPropertySegment(navigationProperty, navigationSource);
                    }
                }
                else if (navigationSource != null && !(navigationSource is IEdmUnknownEntitySet))
                {
                    var entitySet = navigationSource as IEdmEntitySet;
                    odataPath = entitySet != null
                        ? new ODataPath(new EntitySetSegment(entitySet))
                        : new ODataPath(new SingletonSegment(navigationSource as IEdmSingleton));
                }
                else
                {
                    odataPath = new ODataPath();
                }
            }
            else if (nestedProperty != null)
            {
                odataPath = odataPath.AppendPropertySegment(nestedProperty as IEdmStructuralProperty);
            }

            odataUri.Path = odataPath;

            this.EnterScope(new JsonLightNestedResourceInfoScope(readerNestedResourceInfo, navigationSource,
                targetResourceType, odataUri));
        }

        /// <summary>
        /// Try to append key segment.
        /// </summary>
        /// <param name="odataPath">The ODataPath to be evaluated.</param>
        /// <returns>True if successfully append key segment.</returns>
        private bool TryAppendEntitySetKeySegment(ref ODataPath odataPath)
        {
            try
            {
                if (EdmExtensionMethods.HasKey(this.CurrentScope.NavigationSource, this.CurrentScope.ResourceType))
                {
                    IEdmEntityType currentEntityType = this.CurrentScope.ResourceType as IEdmEntityType;
                    ODataResourceBase resource = this.CurrentScope.Item as ODataResourceBase;
                    KeyValuePair<string, object>[] keys = ODataResourceMetadataContext.GetKeyProperties(resource, null, currentEntityType);
                    odataPath = odataPath.AppendKeySegment(keys, currentEntityType, this.CurrentScope.NavigationSource);
                }
            }
            catch (ODataException)
            {
                odataPath = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Replaces the current scope with a new scope with the specified <paramref name="state"/> and
        /// the item of the current scope.
        /// </summary>
        /// <param name="state">The <see cref="ODataReaderState"/> to use for the new scope.</param>
        private void ReplaceScope(ODataReaderState state)
        {
            this.ReplaceScope(new Scope(state, this.Item, this.CurrentNavigationSource, this.CurrentResourceType,
                this.CurrentScope.ODataUri));
        }

        /// <summary>
        /// Called to transition into the EntryEnd state.
        /// </summary>
        private void EndEntry()
        {
            IODataJsonLightReaderResourceState resourceState = this.CurrentResourceState;
            ODataResourceBase currentResource = this.Item as ODataResourceBase;

            if (currentResource != null && !this.IsReadingNestedPayload)
            {
                // Builder should not be used outside the odataresource, lazy builder logic does not work here
                // We should refactor this
                foreach (string navigationPropertyName in this.CurrentResourceState.NavigationPropertiesRead)
                {
                    currentResource.MetadataBuilder.MarkNestedResourceInfoProcessed(navigationPropertyName);
                }

                ODataConventionalEntityMetadataBuilder builder =
                    currentResource.MetadataBuilder as ODataConventionalEntityMetadataBuilder;
                if (builder != null)
                {
                    builder.EndResource();
                }
            }

            this.jsonLightResourceDeserializer.ValidateMediaEntity(resourceState);

            // In non-delta responses, ensure that all projected properties get created.
            // Also ignore cases where the resource is 'null' which happens for expanded null entries.
            if (this.jsonLightInputContext.ReadingResponse && !this.ReadingDelta && currentResource != null)
            {
                // If we have a projected nested resource info that was missing from the payload, report it now.
                ODataJsonLightReaderNestedResourceInfo unprocessedNestedResourceInfo =
                    currentResource.MetadataBuilder.GetNextUnprocessedNavigationLink();
                if (unprocessedNestedResourceInfo != null)
                {
                    this.CurrentResourceState.ProcessingMissingProjectedNestedResourceInfos = true;
                    this.StartNestedResourceInfo(unprocessedNestedResourceInfo);
                    return;
                }
            }

            if (this.State == ODataReaderState.ResourceStart)
            {
                this.EndEntry(
                    new JsonLightResourceScope(
                        ODataReaderState.ResourceEnd,
                        (ODataResource)this.Item,
                        this.CurrentNavigationSource,
                        this.CurrentResourceType,
                        this.CurrentResourceState.PropertyAndAnnotationCollector,
                        this.CurrentResourceState.SelectedProperties,
                        this.CurrentScope.ODataUri));
            }
            else
            {
                this.EndEntry(
                    new JsonLightDeletedResourceScope(
                        ODataReaderState.DeletedResourceEnd,
                        (ODataDeletedResource)this.Item,
                        this.CurrentNavigationSource,
                        this.CurrentResourceType,
                        this.CurrentResourceState.PropertyAndAnnotationCollector,
                        this.CurrentResourceState.SelectedProperties,
                        this.CurrentScope.ODataUri));
            }
        }

        /// <summary>
        /// Add info resolved from context url to current scope.
        /// </summary>
        private void ResolveScopeInfoFromContextUrl()
        {
            if (this.jsonLightResourceDeserializer.ContextUriParseResult != null)
            {
                this.CurrentScope.ODataUri.Path = this.jsonLightResourceDeserializer.ContextUriParseResult.Path;

                if (this.CurrentScope.NavigationSource == null)
                {
                    this.CurrentScope.NavigationSource =
                        this.jsonLightResourceDeserializer.ContextUriParseResult.NavigationSource;
                }

                if (this.CurrentScope.ResourceType == null)
                {
                    IEdmType typeFromContext = this.jsonLightResourceDeserializer.ContextUriParseResult.EdmType;
                    if (typeFromContext != null)
                    {
                        if (typeFromContext.TypeKind == EdmTypeKind.Collection)
                        {
                            typeFromContext = ((IEdmCollectionType)typeFromContext).ElementType.Definition;
                            if (!(typeFromContext is IEdmStructuredType))
                            {
                                typeFromContext = new EdmUntypedStructuredType();
                                this.jsonLightResourceDeserializer.ContextUriParseResult.EdmType = new EdmCollectionType(typeFromContext.ToTypeReference());
                            }
                        }

                        IEdmStructuredType resourceType = typeFromContext as IEdmStructuredType;
                        if (resourceType == null)
                        {
                            resourceType = new EdmUntypedStructuredType();
                            this.jsonLightResourceDeserializer.ContextUriParseResult.EdmType = resourceType;
                        }

                        this.CurrentScope.ResourceType = resourceType;
                    }
                }
            }
        }

#endregion private methods

        #region scopes

        /// <summary>
        /// A reader top-level scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightTopLevelScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
            /// <param name="expectedResourceType">The expected type for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri attached to this scope.</param>
            /// <remarks>The <paramref name="expectedResourceType"/> has the following meaning
            ///   it's the expected base type of the top-level resource or resource set in the top-level resource set.
            /// In all cases the specified type must be a structured type.</remarks>
            internal JsonLightTopLevelScope(IEdmNavigationSource navigationSource, IEdmStructuredType expectedResourceType, ODataUri odataUri)
                : base(ODataReaderState.Start, /*item*/ null, navigationSource, expectedResourceType, odataUri)
            {
            }

            /// <summary>
            /// The duplicate property names checker for the top level scope represented by the current state.
            /// </summary>
            public PropertyAndAnnotationCollector PropertyAndAnnotationCollector { get; set; }
        }

        /// <summary>
        /// A reader primitive scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightPrimitiveScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="primitiveValue">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
            /// <param name="expectedType">The expected type for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            internal JsonLightPrimitiveScope(
                ODataValue primitiveValue,
                IEdmNavigationSource navigationSource,
                IEdmStructuredType expectedType,
                ODataUri odataUri)
                : base(ODataReaderState.Primitive, primitiveValue, navigationSource, expectedType, odataUri)
            {
                Debug.Assert(primitiveValue is ODataPrimitiveValue, "Primitive value scope created with non-primitive value");
            }
        }

        /// <summary>
        /// A reader resource scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private abstract class JsonLightResourceBaseScope : Scope, IODataJsonLightReaderResourceState
        {
            /// <summary>The set of names of the navigation properties we have read so far while reading the resource.</summary>
            private List<string> navigationPropertiesRead;

            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="readerState">The reader state of the new scope that is being created.</param>
            /// <param name="resource">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
            /// <param name="expectedResourceType">The expected type for the scope.</param>
            /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for this resource scope.</param>
            /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedResourceType"/> has the following meaning
            ///   it's the expected base type of the resource. If the resource has no type name specified
            ///   this type will be assumed. Otherwise the specified type name must be
            ///   the expected type or a more derived type.
            /// In all cases the specified type must be an entity type.</remarks>
            protected JsonLightResourceBaseScope(
                ODataReaderState readerState,
                ODataResourceBase resource,
                IEdmNavigationSource navigationSource,
                IEdmStructuredType expectedResourceType,
                PropertyAndAnnotationCollector propertyAndAnnotationCollector,
                SelectedPropertiesNode selectedProperties,
                ODataUri odataUri)
                : base(readerState, resource, navigationSource, expectedResourceType, odataUri)
            {
                Debug.Assert(
                    readerState == ODataReaderState.ResourceStart || readerState == ODataReaderState.ResourceEnd ||
                    readerState == ODataReaderState.DeletedResourceStart || readerState == ODataReaderState.DeletedResourceEnd,
                    "Resource scope created for invalid reader state: " + readerState);

                this.PropertyAndAnnotationCollector = propertyAndAnnotationCollector;
                this.SelectedProperties = selectedProperties;
            }

            /// <summary>
            /// The metadata builder instance for the resource.
            /// </summary>
            public ODataResourceMetadataBuilder MetadataBuilder { get; set; }

            /// <summary>
            /// Flag which indicates that during parsing of the resource represented by this state,
            /// any property which is not an instance annotation was found. This includes property annotations
            /// for property which is not present in the payload.
            /// </summary>
            /// <remarks>
            /// This is used to detect incorrect ordering of the payload (for example odata.id must not come after the first property).
            /// </remarks>
            public bool AnyPropertyFound { get; set; }

            /// <summary>
            /// If the reader finds a nested resource info to report, but it must first report the parent resource
            /// it will store the nested resource info in this property. So this will only ever store the first nested resource info of a resource.
            /// </summary>
            public ODataJsonLightReaderNestedResourceInfo FirstNestedResourceInfo { get; set; }

            /// <summary>
            /// The duplicate property names checker for the resource represented by the current state.
            /// </summary>
            public PropertyAndAnnotationCollector PropertyAndAnnotationCollector { get; private set; }

            /// <summary>
            /// The selected properties that should be expanded during template evaluation.
            /// </summary>
            public SelectedPropertiesNode SelectedProperties { get; private set; }

            /// <summary>
            /// The set of names of the navigation properties we have read so far while reading the resource.
            /// true if we have started processing missing projected navigation links, false otherwise.
            /// </summary>
            public List<string> NavigationPropertiesRead
            {
                get { return this.navigationPropertiesRead ?? (this.navigationPropertiesRead = new List<string>()); }
            }

            /// <summary>
            /// true if we have started processing missing projected navigation links, false otherwise.
            /// </summary>
            public bool ProcessingMissingProjectedNestedResourceInfos { get; set; }

            /// <summary>
            /// The expected type defined in the model for the resource.
            /// </summary>
            public IEdmStructuredType ResourceTypeFromMetadata { get; set; }

            /// <summary>
            /// The resource being read.
            /// </summary>
            ODataResourceBase IODataJsonLightReaderResourceState.Resource
            {
                get
                {
                    Debug.Assert(
                        this.State == ODataReaderState.ResourceStart || this.State == ODataReaderState.ResourceEnd ||
                        this.State == ODataReaderState.DeletedResourceStart || this.State == ODataReaderState.DeletedResourceEnd,
                        "The IODataJsonLightReaderResourceState is only supported on ResourceStart or ResourceEnd scope.");
                    return (ODataResourceBase)this.Item;
                }
            }

            /// <summary>
            /// The structured type for the resource (if available).
            /// </summary>
            IEdmStructuredType IODataJsonLightReaderResourceState.ResourceType
            {
                get
                {
                    Debug.Assert(
                        this.State == ODataReaderState.ResourceStart || this.State == ODataReaderState.ResourceEnd | this.State == ODataReaderState.DeletedResourceStart || this.State == ODataReaderState.DeletedResourceEnd,
                        "The IODataJsonLightReaderResourceState is only supported on (Deleted)ResourceStart or (Deleted)ResourceEnd scope.");
                    return this.ResourceType;
                }
            }

            /// <summary>
            /// The navigation source for the resource (if available)
            /// </summary>
            IEdmNavigationSource IODataJsonLightReaderResourceState.NavigationSource
            {
                get { return this.NavigationSource; }
            }
        }

        /// <summary>
        /// Base class for a reader resource scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightResourceScope : JsonLightResourceBaseScope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="readerState">The reader state of the new scope that is being created.</param>
            /// <param name="resource">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
            /// <param name="expectedResourceType">The expected type for the scope.</param>
            /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for this resource scope.</param>
            /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedResourceType"/> has the following meaning
            ///   it's the expected base type of the resource. If the resource has no type name specified
            ///   this type will be assumed. Otherwise the specified type name must be
            ///   the expected type or a more derived type.
            /// In all cases the specified type must be an entity type.</remarks>
            internal JsonLightResourceScope(
                ODataReaderState readerState,
                ODataResourceBase resource,
                IEdmNavigationSource navigationSource,
                IEdmStructuredType expectedResourceType,
                PropertyAndAnnotationCollector propertyAndAnnotationCollector,
                SelectedPropertiesNode selectedProperties,
                ODataUri odataUri)
                : base(readerState, resource, navigationSource, expectedResourceType, propertyAndAnnotationCollector, selectedProperties, odataUri)
            {
            }
        }

        /// <summary>
        /// A reader deleted resource scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightDeletedResourceScope : JsonLightResourceBaseScope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="readerState">The reader state of the new scope that is being created.</param>
            /// <param name="resource">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
            /// <param name="expectedResourceType">The expected type for the scope.</param>
            /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for this resource scope.</param>
            /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <param name="is40DeletedResource">Whether the deleted resource being read is an OData 4.0 Deleted Resource</param>
            /// <remarks>The <paramref name="expectedResourceType"/> has the following meaning
            ///   it's the expected base type of the resource. If the resource has no type name specified
            ///   this type will be assumed. Otherwise the specified type name must be
            ///   the expected type or a more derived type.
            /// In all cases the specified type must be an entity type.</remarks>
            internal JsonLightDeletedResourceScope(
                ODataReaderState readerState,
                ODataDeletedResource resource,
                IEdmNavigationSource navigationSource,
                IEdmStructuredType expectedResourceType,
                PropertyAndAnnotationCollector propertyAndAnnotationCollector,
                SelectedPropertiesNode selectedProperties,
                ODataUri odataUri,
                bool is40DeletedResource = false)
                : base(readerState, resource, navigationSource, expectedResourceType, propertyAndAnnotationCollector, selectedProperties, odataUri)
            {
                this.Is40DeletedResource = is40DeletedResource;
            }

            /// <summary>Whether the payload is an OData 4.0 deleted resource.</summary>
            internal bool Is40DeletedResource { get; }
        }

        /// <summary>
        /// A reader resource set scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightResourceSetScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="resourceSet">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedResourceType">The expected type for the scope.</param>
            /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <param name="isDelta">True of the ResourceSetScope is for a delta resource set</param>
            /// <remarks>The <paramref name="expectedResourceType"/> has the following meaning
            ///   it's the expected base type of the entries in the resource set.
            ///   note that it might be a more derived type than the base type of the entity set for the resource set.
            /// In all cases the specified type must be an entity type.</remarks>
            internal JsonLightResourceSetScope(ODataResourceSetBase resourceSet, IEdmNavigationSource navigationSource, IEdmStructuredType expectedResourceType, SelectedPropertiesNode selectedProperties, ODataUri odataUri, bool isDelta)
                : base(isDelta ? ODataReaderState.DeltaResourceSetStart : ODataReaderState.ResourceSetStart, resourceSet, navigationSource, expectedResourceType, odataUri)
            {
                this.SelectedProperties = selectedProperties;
            }

            /// <summary>
            /// The selected properties that should be expanded during template evaluation.
            /// </summary>
            public SelectedPropertiesNode SelectedProperties { get; private set; }
        }

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightNestedResourceInfoScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="nestedResourceInfo">The nested resource info attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedStructuredType">The expected type for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedStructuredType"/> is the expected base type the items in the nested resource info.</remarks>
            internal JsonLightNestedResourceInfoScope(ODataJsonLightReaderNestedResourceInfo nestedResourceInfo, IEdmNavigationSource navigationSource, IEdmStructuredType expectedStructuredType, ODataUri odataUri)
                : base(ODataReaderState.NestedResourceInfoStart, nestedResourceInfo.NestedResourceInfo, navigationSource, expectedStructuredType, odataUri)
            {
                this.ReaderNestedResourceInfo = nestedResourceInfo;
            }

            /// <summary>
            /// The nested resource info for the nested resource info to report.
            /// This is only used on a StartNestedResourceInfo scope in responses.
            /// </summary>
            public ODataJsonLightReaderNestedResourceInfo ReaderNestedResourceInfo { get; private set; }
        }

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightDeltaLinkScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="state">The reader state of the new scope that is being created.</param>
            /// <param name="link">The link info attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedEntityType">The expected type for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedEntityType"/> has the following meaning
            ///   it's the expected base type the entries in the expanded link (either the single resource
            ///   or entries in the expanded resource set).
            /// In all cases the specified type must be an entity type.</remarks>
            public JsonLightDeltaLinkScope(ODataReaderState state, ODataDeltaLinkBase link, IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType, ODataUri odataUri)
                : base(state, link, navigationSource, expectedEntityType, odataUri)
            {
                Debug.Assert(
                    state == ODataReaderState.DeltaLink && link is ODataDeltaLink ||
                    state == ODataReaderState.DeltaDeletedLink && link is ODataDeltaDeletedLink,
                    "link must be either DeltaLink or DeltaDeletedLink.");
            }
        }

        #endregion Scopes
    }
}
