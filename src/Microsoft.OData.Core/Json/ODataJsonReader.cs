//---------------------------------------------------------------------
// <copyright file="ODataJsonReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Evaluation;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.UriParser;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// OData reader for the Json format.
    /// </summary>
    internal sealed class ODataJsonReader : ODataReaderCoreAsync
    {
        #region private fields
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataJsonInputContext jsonInputContext;

        /// <summary>The resource and resource set deserializer to read input with.</summary>
        private readonly ODataJsonResourceDeserializer jsonResourceDeserializer;

        /// <summary>The scope associated with the top level of this payload.</summary>
        private readonly JsonTopLevelScope topLevelScope;

        /// <summary>true if the reader is created for reading parameter; false otherwise.</summary>
        private readonly bool readingParameter;
        #endregion private fields

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The input to read the payload from.</param>
        /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
        /// <param name="expectedResourceType">The expected structured type for the resource to be read (in case of resource reader) or entries in the resource set to be read (in case of resource set reader).</param>
        /// <param name="readingResourceSet">true if the reader is created for reading a resource set; false when it is created for reading a resource.</param>
        /// <param name="readingParameter">true if the reader is created for reading a parameter; false otherwise.</param>
        /// <param name="readingDelta">true if the reader is created for reading expanded navigation property in delta response; false otherwise.</param>
        /// <param name="listener">If not null, the Json reader will notify the implementer of the interface of relevant state changes in the Json reader.</param>
        internal ODataJsonReader(
            ODataJsonInputContext jsonInputContext,
            IEdmNavigationSource navigationSource,
            IEdmStructuredType expectedResourceType,
            bool readingResourceSet,
            bool readingParameter = false,
            bool readingDelta = false,
            IODataReaderWriterListener listener = null)
            : base(jsonInputContext, readingResourceSet, readingDelta, listener)
        {
            Debug.Assert(jsonInputContext != null, "jsonInputContext != null");
            Debug.Assert(
                expectedResourceType == null || jsonInputContext.Model.IsUserModel(),
                "If the expected type is specified we need model as well. We should have verified that by now.");

            this.jsonInputContext = jsonInputContext;
            this.jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);
            this.readingParameter = readingParameter;
            this.topLevelScope = new JsonTopLevelScope(navigationSource, expectedResourceType, new ODataUri());
            this.EnterScope(this.topLevelScope);
        }

        #endregion

        #region private properties

        /// <summary>
        /// Returns the current resource state.
        /// </summary>
        private IODataJsonReaderResourceState CurrentResourceState
        {
            get
            {
                Debug.Assert(
                    this.State == ODataReaderState.ResourceStart || this.State == ODataReaderState.ResourceEnd ||
                    this.State == ODataReaderState.DeletedResourceStart || this.State == ODataReaderState.DeletedResourceEnd,
                    "This property can only be accessed in the EntryStart or EntryEnd scope.");
                return (IODataJsonReaderResourceState)this.CurrentScope;
            }
        }

        /// <summary>
        /// Returns current scope cast to JsonResourceSetScope
        /// </summary>
        private JsonResourceSetScope CurrentJsonResourceSetScope
        {
            get
            {
                return ((JsonResourceSetScope)this.CurrentScope);
            }
        }

        /// <summary>
        /// Returns current scope cast to JsonNestedResourceInfoScope
        /// </summary>
        private JsonNestedResourceInfoScope CurrentJsonNestedResourceInfoScope
        {
            get
            {
                return ((JsonNestedResourceInfoScope)this.CurrentScope);
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
                Scope scope = SeekScope<JsonNestedResourceInfoScope>(maxDepth: 3);

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
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.None,
                "Pre-Condition: expected JsonNodeType.None when not reading a nested payload.");

            PropertyAndAnnotationCollector propertyAndAnnotationCollector =
                this.jsonInputContext.CreatePropertyAndAnnotationCollector();

            // Position the reader on the first node depending on whether we are reading a nested payload or a Uri Operation Parameter or not.
            ODataPayloadKind payloadKind = this.ReadingResourceSet ?
                this.ReadingDelta ? ODataPayloadKind.Delta : ODataPayloadKind.ResourceSet : ODataPayloadKind.Resource;

            // Following parameter "this.IsReadingNestedPayload || this.readingParameter" indicates whether to read
            // { "value" :
            // or
            // { "parameterName" :
            this.jsonResourceDeserializer.ReadPayloadStart(
                payloadKind,
                propertyAndAnnotationCollector,
                this.IsReadingNestedPayload || this.readingParameter,
                /*allowEmptyPayload*/false);

            ResolveScopeInfoFromContextUrl();

            Scope currentScope = this.CurrentScope;
            if (this.jsonInputContext.Model.IsUserModel())
            {
                IEnumerable<string> derivedTypeConstraints = this.jsonInputContext.Model.GetDerivedTypeConstraints(currentScope.NavigationSource);
                if (derivedTypeConstraints != null)
                {
                    currentScope.DerivedTypeValidator = new DerivedTypeValidator(currentScope.ResourceType, derivedTypeConstraints, "navigation source", currentScope.NavigationSource.Name);
                }
            }

            return this.ReadAtStartImplementationSynchronously(propertyAndAnnotationCollector);
        }

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
        protected override async Task<bool> ReadAtStartImplementationAsync()
        {
            Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
            Debug.Assert(
                this.IsReadingNestedPayload ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.None,
                "Pre-Condition: expected JsonNodeType.None when not reading a nested payload.");

            PropertyAndAnnotationCollector propertyAndAnnotationCollector =
                this.jsonInputContext.CreatePropertyAndAnnotationCollector();

            // Position the reader on the first node depending on whether we are reading a nested payload or not.
            ODataPayloadKind payloadKind = this.ReadingResourceSet ?
                this.ReadingDelta ? ODataPayloadKind.Delta : ODataPayloadKind.ResourceSet : ODataPayloadKind.Resource;

            await this.jsonResourceDeserializer.ReadPayloadStartAsync(
                payloadKind,
                propertyAndAnnotationCollector,
                this.IsReadingNestedPayload,
                allowEmptyPayload: false,
                this.CurrentNavigationSource).ConfigureAwait(false);

            ResolveScopeInfoFromContextUrl();

            return await this.ReadAtStartInternalImplementationAsync(propertyAndAnnotationCollector)
                .ConfigureAwait(false);
        }

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
            return this.ReadAtResourceSetStartInternalImplementationAsync();
        }

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
            return this.ReadAtResourceSetEndInternalImplementationAsync();
        }

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
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtResourceStartImplementation);
        }

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
            return this.ReadAtResourceEndInternalImplementationAsync();
        }

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

        /// <summary>
        /// Implementation of the reader logic when in state 'Primitive'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         end of primitive value in a collection
        /// Post-Condition: The reader is positioned on the first node after the primitive value
        /// </remarks>
        protected override async Task<bool> ReadAtPrimitiveImplementationAsync()
        {
            Debug.Assert(
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue,
                "Pre-Condition: JsonNodeType.PrimitiveValue (null or untyped)");

            this.PopScope(ODataReaderState.Primitive);

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This should never hit the end of the input.
            await this.jsonResourceDeserializer.JsonReader.ReadAsync()
                .ConfigureAwait(false);
            await this.ReadNextResourceSetItemAsync()
                .ConfigureAwait(false);

            return true;
        }

        #endregion Primitive

        #region Property

        /// <summary>
        /// Implementation of the reader logic when in state 'PropertyInfo'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected override bool ReadAtNestedPropertyInfoImplementation()
        {
            return this.ReadAtNestedPropertyInfoSynchronously();
        }

        /// <summary>
        /// Asynchronous implementation of the reader logic when in state 'PropertyInfo'.
        /// </summary>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more items can be read from the reader; otherwise false
        /// </returns>
        protected override Task<bool> ReadAtNestedPropertyInfoImplementationAsync()
        {
            return this.ReadAtNestedPropertyInfoAsynchronously();
        }

        #endregion

        #region Stream

        /// <summary>
        /// Implementation of the reader logic when in state 'Stream'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected override bool ReadAtStreamImplementation()
        {
            return this.ReadAtStreamSynchronously();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Stream'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected override async Task<bool> ReadAtStreamImplementationAsync()
        {
            this.PopScope(ODataReaderState.Stream);

            if (this.State == ODataReaderState.ResourceSetStart ||
                this.State == ODataReaderState.DeltaResourceSetStart)
            {
                // We are reading a stream within a collection
                await this.ReadNextResourceSetItemAsync()
                    .ConfigureAwait(false);

                return true;
            }

            if (this.State == ODataReaderState.NestedProperty)
            {
                this.PopScope(ODataReaderState.NestedProperty);
            }

            // We are reading a stream value
            return await this.ReadNextNestedInfoAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a stream for reading an inline stream property.
        /// </summary>
        /// <returns>A stream for reading the stream property.</returns>
        protected override Stream CreateReadStreamImplementation()
        {
            return this.jsonInputContext.JsonReader.CreateReadStream();
        }

        protected override TextReader CreateTextReaderImplementation()
        {
            return this.jsonInputContext.JsonReader.CreateTextReader();
        }

        /// <summary>
        /// Asynchronously creates <see cref="Stream"/> for reading an inline stream property.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="Stream"/> for reading the stream property.
        /// </returns>
        protected override Task<Stream> CreateReadStreamImplementationAsync()
        {
            return this.jsonInputContext.JsonReader.CreateReadStreamAsync();
        }

        /// <summary>
        /// Asynchronously creates <see cref="TextReader"/> for reading an inline string property.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="TextReader"/> for reading the string property.
        /// </returns>
        protected override Task<TextReader> CreateTextReaderImplementationAsync()
        {
            return this.jsonInputContext.JsonReader.CreateTextReaderAsync();
        }

        #endregion

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
            return this.ReadAtNestedResourceInfoStartInternalImplementationAsync();
        }

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
        protected override async Task<bool> ReadAtNestedResourceInfoEndImplementationAsync()
        {
            this.PopScope(ODataReaderState.NestedResourceInfoEnd);

            return await this.ReadNextNestedInfoAsync()
                .ConfigureAwait(false);
        }

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
        protected override async Task<bool> ReadAtEntityReferenceLinkAsync()
        {
            this.PopScope(ODataReaderState.EntityReferenceLink);
            Debug.Assert(this.State == ODataReaderState.NestedResourceInfoStart,
                "this.State == ODataReaderState.NestedResourceInfoStart");

            await this.ReadNextNestedResourceInfoContentItemInRequestAsync()
                .ConfigureAwait(false);

            return true;
        }

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
            return this.ReadAtResourceSetStartInternalImplementationAsync();
        }

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
            return this.ReadAtResourceSetEndInternalImplementationAsync();
        }

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
            return this.ReadAtResourceEndInternalImplementationAsync();
        }

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
        ///                 JsonNodeType.EndArray               If no (more) properties exist in the resource's content
        /// </remarks>
        protected override bool ReadAtDeltaLinkImplementation()
        {
            return this.ReadAtDeltaLinkImplementationSynchronously();
        }

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
            return this.EndDeltaLinkAsync(ODataReaderState.DeltaLink);
        }

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
        ///                 JsonNodeType.EndArray               If no (more) properties exist in the resource's content
        /// </remarks>
        protected override bool ReadAtDeltaDeletedLinkImplementation()
        {
            return this.ReadAtDeltaDeletedLinkImplementationSynchronously();
        }

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
            return this.EndDeltaLinkAsync(ODataReaderState.DeltaDeletedLink);
        }

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
            if (this.jsonInputContext.ReadingResponse && !this.IsReadingNestedPayload)
            {
                Debug.Assert(this.jsonResourceDeserializer.ContextUriParseResult != null,
                    "We should have failed by now if we don't have parse results for context URI.");

                // Validate the context URI parsed from the payload against the entity set and entity type passed in through the API.
                ReaderValidationUtils.ValidateResourceSetOrResourceContextUri(
                    this.jsonResourceDeserializer.ContextUriParseResult, this.CurrentScope, true);
            }

            // Get the $select query option from the metadata link, if we have one.
            string selectQueryOption = this.jsonResourceDeserializer.ContextUriParseResult == null
                ? null
                : this.jsonResourceDeserializer.ContextUriParseResult.SelectQueryOption;

            SelectedPropertiesNode selectedProperties = SelectedPropertiesNode.Create(selectQueryOption, (this.CurrentResourceTypeReference != null) ? this.CurrentResourceTypeReference.AsStructured().StructuredDefinition() : null, this.jsonInputContext.Model);

            if (this.ReadingResourceSet)
            {
                // Store the duplicate property names checker to use it later when reading the resource set end
                // (since we allow resourceSet-related annotations to appear after the resource set's data).
                this.topLevelScope.PropertyAndAnnotationCollector = propertyAndAnnotationCollector;

                bool isReordering = this.jsonInputContext.JsonReader is ReorderingJsonReader;

                if (this.ReadingDelta)
                {
                    ODataDeltaResourceSet resourceSet = new ODataDeltaResourceSet();

                    // Read top-level resource set annotations for delta resource sets.
                    this.jsonResourceDeserializer.ReadTopLevelResourceSetAnnotations(
                        resourceSet, propertyAndAnnotationCollector, /*forResourceSetStart*/true,
                        /*readAllFeedProperties*/isReordering);
                    this.ReadDeltaResourceSetStart(resourceSet, selectedProperties);

                    this.jsonResourceDeserializer.AssertJsonCondition(JsonNodeType.EndArray, JsonNodeType.StartObject);
                }
                else
                {
                    ODataResourceSet resourceSet = new ODataResourceSet();
                    if (!this.IsReadingNestedPayload)
                    {
                        if (!this.readingParameter)
                        {
                            // Skip top-level resource set annotations for nested resource sets.
                            this.jsonResourceDeserializer.ReadTopLevelResourceSetAnnotations(
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
                            this.jsonResourceDeserializer.JsonReader.Read();
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
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput ||
                (this.ParentScope != null && (this.ParentScope.ResourceType == null || this.ParentScope.ResourceType.TypeKind == EdmTypeKind.Untyped) &&
                    (this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue ||
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndArray)) ||
                !this.IsTopLevel && !this.jsonInputContext.ReadingResponse,
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
                this.jsonResourceDeserializer.JsonReader.Read();

                // read the end-of-payload
                this.jsonResourceDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);

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
                this.CurrentResourceState.ResourceTypeFromMetadata = this.ParentScope.ResourceType as IEdmStructuredType;
                ODataResourceMetadataBuilder builder =
                    this.jsonResourceDeserializer.MetadataContext.GetResourceMetadataBuilderForReader(
                        this.CurrentResourceState,
                        this.jsonInputContext.MessageReaderSettings.EnableReadingKeyAsSegment,
                        this.ReadingDelta);
                if (builder != currentResource.MetadataBuilder)
                {
                    ODataNestedResourceInfo parentNestInfo = this.ParentNestedInfo;
                    ODataConventionalResourceMetadataBuilder conventionalResourceMetadataBuilder =
                        builder as ODataConventionalResourceMetadataBuilder;

                    // If it's ODataConventionalResourceMetadataBuilder, then it means we need to build nested relationship for it in containment case
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
                this.jsonResourceDeserializer.AssertJsonCondition(JsonNodeType.PrimitiveValue);

                // There's nothing to read, so move to the end resource state
                this.EndEntry();
            }
            else if (this.CurrentResourceState.FirstNestedInfo != null)
            {
                this.ReadNestedInfo(this.CurrentResourceState.FirstNestedInfo);
            }
            else
            {
                // End of resource
                // All the properties have already been read before we actually entered the EntryStart state (since we read as far as we can in any given state).
                this.jsonResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject);
                this.EndEntry();
            }

            Debug.Assert(
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.PrimitiveValue or JsonNodeType.Property or JsonNodeType.EndObject");

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
                ODataPayloadKind payloadKind = nestedInfo.IsCollection.GetValueOrDefault()
                    ? ODataPayloadKind.ResourceSet
                    : ODataPayloadKind.Resource;
                ODataPath odataPath = ODataJsonContextUriParser.Parse(
                    this.jsonResourceDeserializer.Model,
                    UriUtils.UriToString(nestedInfo.ContextUrl),
                    payloadKind,
                    this.jsonResourceDeserializer.MessageReaderSettings.ClientCustomTypeResolver,
                    this.jsonResourceDeserializer.JsonInputContext.ReadingResponse,
                    true,
                    this.jsonResourceDeserializer.MessageReaderSettings.BaseUri,
                    this.CurrentNavigationSource).Path;

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
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                (this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue &&
                this.jsonResourceDeserializer.JsonReader.GetValue() == null),
                "Pre-Condition: JsonNodeType.EndObject or JsonNodeType.PrimitiveValue (null)");

            // We have to cache these values here, since the PopScope below will destroy them.
            bool isTopLevel = this.IsTopLevel;
            bool isExpandedLinkContent = this.IsExpandedLinkContent;

            this.PopScope(this.State == ODataReaderState.ResourceEnd ? ODataReaderState.ResourceEnd : ODataReaderState.DeletedResourceEnd);

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This can hit the end of the input.
            this.jsonResourceDeserializer.JsonReader.Read();

            // Analyze the next Json token to determine whether it is start object (next resource), end array (resource set end) or eof (top-level resource end)
            bool result = true;
            if (isTopLevel)
            {
                // NOTE: we rely on the underlying JSON reader to fail if there is more than one value at the root level.
                Debug.Assert(
                    this.IsReadingNestedPayload ||
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput,
                    "Expected JSON reader to have reached the end of input when not reading a nested payload.");

                // read the end-of-payload
                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
                this.jsonResourceDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);
                Debug.Assert(
                    this.IsReadingNestedPayload ||
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput,
                    "Expected JSON reader to have reached the end of input when not reading a nested payload.");

                // replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                result = false;
            }
            else if (isExpandedLinkContent)
            {
                Debug.Assert(
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject || // expanded link resource as last property of the owning resource
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property, // expanded link resource with more properties on the resource
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
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue,
                "Pre-Condition: JsonNodeType.PrimitiveValue (null or untyped)");

            this.PopScope(ODataReaderState.Primitive);

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This should never hit the end of the input.
            this.jsonResourceDeserializer.JsonReader.Read();
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
            Debug.Assert(this.CurrentScope is JsonDeletedResourceScope);

            if (((JsonDeletedResourceScope)(this.CurrentScope)).Is40DeletedResource)
            {
                this.jsonResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject);
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
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject,
                "Not positioned at end of object after reading delta link");

            this.PopScope(readerState);

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This should never hit the end of the input.
            this.jsonResourceDeserializer.JsonReader.Read();
            this.ReadNextResourceSetItem();
            return true;
        }

        #endregion (Deleted)Link

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
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue &&
                this.jsonResourceDeserializer.JsonReader.GetValue() == null,
                "Pre-Condition: expected JsonNodeType.Property, JsonNodeType.EndObject, JsonNodeType.StartObject, JsonNodeType.StartArray or JsonNodeType.Primitive (null)");

            ODataNestedResourceInfo currentLink = this.CurrentNestedResourceInfo;

            IODataJsonReaderResourceState parentResourceState = (IODataJsonReaderResourceState)this.ParentScope;

            if (this.jsonInputContext.ReadingResponse)
            {
                // If we are reporting a nested resource info that was projected but not included in the payload,
                // simply change state to NestedResourceInfoEnd.
                if (parentResourceState.ProcessingMissingProjectedNestedResourceInfos)
                {
                    this.ReplaceScope(ODataReaderState.NestedResourceInfoEnd);
                }
                else if (!this.jsonResourceDeserializer.JsonReader.IsOnValueNode())
                {
                    // Deferred link (nested resource info which doesn't have a value and is in the response)
                    ReaderUtils.CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(
                        parentResourceState.PropertyAndAnnotationCollector, currentLink);
                    this.jsonResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

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
                    ODataJsonReaderNestedResourceInfo nestedResourceInfo =
                        this.CurrentJsonNestedResourceInfoScope.ReaderNestedResourceInfo;
                    Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
                    Debug.Assert(nestedResourceInfo.NestedResourceSet != null,
                        "We must have a precreated expanded resource set already.");
                    JsonResourceBaseScope parentScope = (JsonResourceBaseScope)this.ParentScope;
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
                        Debug.Assert(deltaResourceSet != null, "Nested resource collection is not a resource set or a delta resource set");
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
            this.PopScope(ODataReaderState.NestedResourceInfoEnd);
            return this.ReadNextNestedInfo();
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'PropertyInfo'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property:          there are more properties after the nested resource info in the owning resource
        /// Post-Condition: JsonNodeType.StartObject        start of the expanded resource nested resource info to read next
        ///                 JsonNodeType.StartArray         start of the expanded resource set nested resource info to read next
        ///                 JsonNoteType.Primitive (null)   expanded null resource nested resource info to read next
        ///                 JsonNoteType.Property           property after deferred link or entity reference link
        ///                 JsonNodeType.EndObject          end of the parent resource
        /// </remarks>
        private bool ReadAtNestedPropertyInfoSynchronously()
        {
            Debug.Assert(this.CurrentScope.State == ODataReaderState.NestedProperty, "Must be on 'NestedProperty' scope.");
            JsonNestedPropertyInfoScope nestedPropertyInfoScope = (JsonNestedPropertyInfoScope)this.CurrentScope;
            ODataJsonReaderNestedPropertyInfo nestedPropertyInfo = nestedPropertyInfoScope.ReaderNestedPropertyInfo;
            Debug.Assert(nestedPropertyInfo != null);

            ODataPropertyInfo propertyInfo = this.CurrentScope.Item as ODataPropertyInfo;
            Debug.Assert(propertyInfo != null, "Reading Nested Property Without an ODataPropertyInfo");

            ODataStreamPropertyInfo streamPropertyInfo = propertyInfo as ODataStreamPropertyInfo;
            if (streamPropertyInfo != null && !String.IsNullOrEmpty(streamPropertyInfo.ContentType))
            {
                this.StartNestedStreamInfo(new ODataJsonReaderStreamInfo(streamPropertyInfo.PrimitiveTypeKind, streamPropertyInfo.ContentType));
            }
            else if (!nestedPropertyInfo.WithValue)
            {
                // It's a nested non-stream property
                this.PopScope(ODataReaderState.NestedProperty);

                // We are reading a next nested info.
                return this.ReadNextNestedInfo();
            }
            else
            {
                this.StartNestedStreamInfo(
                   new ODataJsonReaderStreamInfo(propertyInfo.PrimitiveTypeKind));
            }

            return true;
        }

        /// <summary>
        /// Asynchronous implementation of the reader logic when in state 'PropertyInfo'.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more items can be read from the reader; otherwise false
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property:          there are more properties after the nested resource info in the owning resource
        /// Post-Condition: JsonNodeType.StartObject        start of the expanded resource nested resource info to read next
        ///                 JsonNodeType.StartArray         start of the expanded resource set nested resource info to read next
        ///                 JsonNoteType.Primitive (null)   expanded null resource nested resource info to read next
        ///                 JsonNoteType.Property           property after deferred link or entity reference link
        ///                 JsonNodeType.EndObject          end of the parent resource
        /// </remarks>
        private async Task<bool> ReadAtNestedPropertyInfoAsynchronously()
        {
            Debug.Assert(this.CurrentScope.State == ODataReaderState.NestedProperty, "Must be on 'NestedProperty' scope.");
            JsonNestedPropertyInfoScope nestedPropertyInfoScope = (JsonNestedPropertyInfoScope)this.CurrentScope;
            ODataJsonReaderNestedPropertyInfo nestedPropertyInfo = nestedPropertyInfoScope.ReaderNestedPropertyInfo;
            Debug.Assert(nestedPropertyInfo != null);

            ODataPropertyInfo propertyInfo = this.CurrentScope.Item as ODataPropertyInfo;
            Debug.Assert(propertyInfo != null, "Reading Nested Property Without an ODataPropertyInfo");

            ODataStreamPropertyInfo streamPropertyInfo = propertyInfo as ODataStreamPropertyInfo;
            if (streamPropertyInfo != null && !String.IsNullOrEmpty(streamPropertyInfo.ContentType))
            {
                this.StartNestedStreamInfo(new ODataJsonReaderStreamInfo(streamPropertyInfo.PrimitiveTypeKind, streamPropertyInfo.ContentType));
            }
            else if (!nestedPropertyInfo.WithValue)
            {
                // It's a nested non-stream property
                this.PopScope(ODataReaderState.NestedProperty);

                // We are reading a next nested info.
                return await this.ReadNextNestedInfoAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                this.StartNestedStreamInfo(
                   new ODataJsonReaderStreamInfo(propertyInfo.PrimitiveTypeKind));
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Stream'.
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
        private bool ReadAtStreamSynchronously()
        {
            this.PopScope(ODataReaderState.Stream);
            if (this.State == ODataReaderState.ResourceSetStart ||
                this.State == ODataReaderState.DeltaResourceSetStart)
            {
                // We are reading a stream within a collection
                this.ReadNextResourceSetItem();
                return true;
            }

            if (this.State == ODataReaderState.NestedProperty)
            {
                this.PopScope(ODataReaderState.NestedProperty);
            }

            // We are reading a stream value
            return this.ReadNextNestedInfo();
        }

        private bool ReadNextNestedInfo()
        {
            this.jsonResourceDeserializer.AssertJsonCondition(
                JsonNodeType.EndObject,
                JsonNodeType.Property);
            Debug.Assert(this.State == ODataReaderState.ResourceStart || this.State == ODataReaderState.DeletedResourceStart, "Should be in (deleted) resource start state after reading stream.");

            ODataJsonReaderNestedInfo readerNestedInfo = null;
            IODataJsonReaderResourceState resourceState = this.CurrentResourceState;

            if (this.jsonInputContext.ReadingResponse &&
                resourceState.ProcessingMissingProjectedNestedResourceInfos)
            {
                // We are reporting navigation links that were projected but missing from the payload
                readerNestedInfo = resourceState.Resource.MetadataBuilder.GetNextUnprocessedNavigationLink();
            }
            else
            {
                readerNestedInfo = this.jsonResourceDeserializer.ReadResourceContent(resourceState);
            }

            if (readerNestedInfo == null)
            {
                // End of the resource
                this.EndEntry();
            }
            else
            {
                this.ReadNestedInfo(readerNestedInfo);
            }

            return true;
        }

        private void ReadNestedInfo(ODataJsonReaderNestedInfo nestedInfo)
        {
            ODataJsonReaderNestedResourceInfo readerNestedResourceInfo = nestedInfo as ODataJsonReaderNestedResourceInfo;
            if (readerNestedResourceInfo != null)
            {
                // Next nested resource info on the resource
                this.StartNestedResourceInfo(readerNestedResourceInfo);
            }
            else
            {
                ODataJsonReaderNestedPropertyInfo readerNestedStreamInfo = nestedInfo as ODataJsonReaderNestedPropertyInfo;
                Debug.Assert(readerNestedStreamInfo != null, "NestedInfo is not a resource, stream, string");
                if (readerNestedStreamInfo != null)
                {
                    this.StartNestedPropertyInfo(readerNestedStreamInfo);
                }
            }
        }

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
        private bool ReadAtEntityReferenceLinkSynchronously()
        {
            this.PopScope(ODataReaderState.EntityReferenceLink);
            Debug.Assert(this.State == ODataReaderState.NestedResourceInfoStart,
                "this.State == ODataReaderState.NestedResourceInfoStart");

            this.ReadNextNestedResourceInfoContentItemInRequest();
            return true;
        }

        #endregion EntityReferenceLink

        #endregion ReadAt<>Synchronously methods

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

            this.jsonResourceDeserializer.ReadResourceSetContentStart();
            IJsonReader jsonReader = this.jsonResourceDeserializer.JsonReader;
            if (jsonReader.NodeType != JsonNodeType.EndArray
                && jsonReader.NodeType != JsonNodeType.StartObject
                && jsonReader.NodeType != JsonNodeType.PrimitiveValue
                && jsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonResourceDeserializer_InvalidNodeTypeForItemsInResourceSet(jsonReader.NodeType));
            }

            this.EnterScope(new JsonResourceSetScope(resourceSet, this.CurrentNavigationSource,
                this.CurrentScope.ResourceTypeReference, selectedProperties, this.CurrentScope.ODataUri, /*isDelta*/ false));
        }

        /// <summary>
        /// Reads the end of the current resource set.
        /// </summary>
        private void ReadResourceSetEnd()
        {
            Debug.Assert(this.State == ODataReaderState.ResourceSetStart || this.State == ODataReaderState.DeltaResourceSetStart,
                "Not in ResourceSetStart or DeltaResourceSetStart state when reading end of (delta) resource set.");
            Debug.Assert(this.Item is ODataResourceSetBase, "Current Item is not ResourceSetBase");

            this.jsonResourceDeserializer.ReadResourceSetContentEnd();

            ODataJsonReaderNestedResourceInfo expandedNestedResourceInfo = null;
            JsonNestedResourceInfoScope parentNestedResourceInfoScope = (JsonNestedResourceInfoScope)this.ExpandedLinkContentParentScope;
            if (parentNestedResourceInfoScope != null)
            {
                expandedNestedResourceInfo = parentNestedResourceInfoScope.ReaderNestedResourceInfo;
            }

            if (!this.IsReadingNestedPayload && (this.IsExpandedLinkContent || this.IsTopLevel))
            {
                // Temp ban reading the instance annotation after the resource set in parameter payload. (!this.IsReadingNestedPayload => !this.readingParameter)
                // Nested resource set payload won't have a NextLink annotation after the resource set itself since the payload is NOT pageable.
                this.jsonResourceDeserializer.ReadNextLinkAnnotationAtResourceSetEnd(this.Item as ODataResourceSetBase,
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

            if (this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                Debug.Assert(this.jsonResourceDeserializer.JsonReader.GetValue() == null,
                    "If a primitive value is representing an expanded resource its value must be null.");

                IEdmStructuralProperty structuralProperty =
                    this.CurrentJsonNestedResourceInfoScope.ReaderNestedResourceInfo.StructuralProperty;
                if (structuralProperty != null && !structuralProperty.Type.IsNullable)
                {
                    ODataNullValueBehaviorKind nullValueReadBehaviorKind =
                        this.jsonResourceDeserializer.ReadingResponse
                            ? ODataNullValueBehaviorKind.Default
                            : this.jsonResourceDeserializer.Model.NullValueReadBehaviorKind(structuralProperty);

                    if (nullValueReadBehaviorKind == ODataNullValueBehaviorKind.Default)
                    {
                        throw new ODataException(
                            Strings.ReaderValidationUtils_NullNamedValueForNonNullableType(nestedResourceInfo.Name,
                                structuralProperty.Type.FullName()));
                    }
                }

                // Expanded null resource
                // The expected type and expected navigation source for an expanded resource are the same as for the nested resource info around it.
                this.EnterScope(new JsonResourceScope(ODataReaderState.ResourceStart, /*resource*/ null,
                    this.CurrentNavigationSource, this.CurrentResourceTypeReference, /*propertyAndAnnotationCollector*/null,
                    /*projectedProperties*/null, this.CurrentScope.ODataUri));
            }
            else
            {
                // Expanded resource
                // The expected type for an expanded resource is the same as for the nested resource info around it.
                JsonResourceBaseScope parentScope = (JsonResourceBaseScope)this.ParentScope;
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
            IODataJsonReaderResourceState parentResourceState = (IODataJsonReaderResourceState)this.ParentScope;
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
            IEdmTypeReference resourceTypeReference = this.CurrentResourceTypeReference;

            this.jsonResourceDeserializer.AssertJsonCondition(JsonNodeType.StartObject, JsonNodeType.Property,
                JsonNodeType.EndObject, JsonNodeType.PrimitiveValue);

            if (this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                object primitiveValue = this.jsonResourceDeserializer.JsonReader.GetValue();
                if (primitiveValue != null)
                {
                    // primitive value in an untyped collection
                    if (this.CurrentResourceType.TypeKind == EdmTypeKind.Untyped)
                    {
                        this.EnterScope(new JsonPrimitiveScope(new ODataPrimitiveValue(primitiveValue),
                            this.CurrentNavigationSource, this.CurrentResourceTypeReference, this.CurrentScope.ODataUri));
                    }
                    else
                    {
                        throw new ODataException(Strings.ODataJsonReader_UnexpectedPrimitiveValueForODataResource);
                    }
                }
                else
                {
                    // null resource
                    if (resourceTypeReference.IsComplex() || resourceTypeReference.IsUntyped())
                    {
                        this.jsonResourceDeserializer.MessageReaderSettings.Validator.ValidateNullValue(this.CurrentResourceTypeReference, true, "", null);
                    }

                    this.EnterScope(new JsonResourceScope(ODataReaderState.ResourceStart, /*resource*/ null,
                        this.CurrentNavigationSource, this.CurrentResourceTypeReference, /*propertyAndAnnotationCollector*/null,
                        /*projectedProperties*/null, this.CurrentScope.ODataUri));
                }

                return;
            }

            // If the reader is on StartObject then read over it. This happens for entries in resource set.
            // For top-level entries the reader will be positioned on the first resource property (after odata.context if it was present).
            if (this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject)
            {
                this.jsonResourceDeserializer.JsonReader.Read();
            }

            ODataDeltaKind resourceKind = ODataDeltaKind.Resource;

            // if this is a resourceSet, expanded link, or non-top level resource in a delta result, read the contextUrl
            if (this.ReadingResourceSet || this.IsExpandedLinkContent || (this.ReadingDelta && !this.IsTopLevel))
            {
                string contextUriStr =
                    this.jsonResourceDeserializer.ReadContextUriAnnotation(ODataPayloadKind.Resource,
                        propertyAndAnnotationCollector, false);
                if (contextUriStr != null)
                {
                    contextUriStr =
                        UriUtils.UriToString(this.jsonResourceDeserializer.ProcessUriFromPayload(contextUriStr));
                    ODataJsonContextUriParseResult parseResult = ODataJsonContextUriParser.Parse(
                        this.jsonResourceDeserializer.Model,
                        contextUriStr,
                        this.ReadingDelta ? ODataPayloadKind.Delta : ODataPayloadKind.Resource,
                        this.jsonResourceDeserializer.MessageReaderSettings.ClientCustomTypeResolver,
                        this.jsonInputContext.ReadingResponse || this.ReadingDelta,
                        true,
                        this.jsonResourceDeserializer.BaseUri, 
                        this.CurrentNavigationSource);

                    if (parseResult != null)
                    {
                        // a top-level (deleted) resource in a delta response can come from any entity set
                        resourceKind = parseResult.DeltaKind;
                        if (this.ReadingDelta && this.IsTopLevel && (resourceKind == ODataDeltaKind.Resource || resourceKind == ODataDeltaKind.DeletedEntry))
                        {
                            IEdmStructuredType parsedType = parseResult.EdmType as IEdmStructuredType;
                            if (parsedType != null)
                            {
                                resourceTypeReference = parsedType.ToTypeReference(true);
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

            // If this is a resource in a delta resource set, check to see if it's a 4.01 deleted resource
            ODataDeletedResource deletedResource = null;
            if (this.ReadingDelta && (resourceKind == ODataDeltaKind.Resource || resourceKind == ODataDeltaKind.DeletedEntry))
            {
                deletedResource = this.jsonResourceDeserializer.ReadDeletedResource();
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
                    this.StartResource(source, resourceTypeReference, propertyAndAnnotationCollector, selectedProperties);

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
                        deletedResource = this.jsonResourceDeserializer.ReadDeletedEntry();
                        this.StartDeletedResource(
                            deletedResource,
                            source,
                            resourceTypeReference,
                            propertyAndAnnotationCollector,
                            selectedProperties,
                            true /*is 4.0 Deleted Resource*/);
                    }
                    else // OData 4.01 deleted entry
                    {
                        this.StartDeletedResource(
                            deletedResource,
                            source,
                            resourceTypeReference,
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
                    Debug.Assert(true, "Unknown ODataDeltaKind " + resourceKind.ToString());
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

            this.jsonResourceDeserializer.ReadResourceSetContentStart();
            IJsonReader jsonReader = this.jsonResourceDeserializer.JsonReader;
            if (jsonReader.NodeType != JsonNodeType.EndArray && jsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonResourceDeserializer_InvalidNodeTypeForItemsInResourceSet(jsonReader.NodeType));
            }

            Debug.Assert(this.CurrentResourceType is IEdmEntityType, "Delta resource type is not an entity");

            this.EnterScope(new JsonResourceSetScope(
                deltaResourceSet,
                this.CurrentNavigationSource,
                this.CurrentResourceTypeReference as IEdmEntityTypeReference,
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
            this.jsonResourceDeserializer.ReadResourceTypeName(this.CurrentResourceState);

            // Resolve the type name
            this.ApplyResourceTypeNameFromPayload(currentResource.TypeName);

            // Validate type with derived type validator if available
            if (this.CurrentDerivedTypeValidator != null)
            {
                this.CurrentDerivedTypeValidator.ValidateResourceType(this.CurrentResourceType);
            }

            // Validate type with resource set validator if available and not reading top-level delta resource set
            if (this.CurrentResourceSetValidator != null && !(this.ReadingDelta && this.CurrentResourceDepth == 0))
            {
                this.CurrentResourceSetValidator.ValidateResource(this.CurrentResourceType);
            }

            this.CurrentResourceState.FirstNestedInfo =
                this.jsonResourceDeserializer.ReadResourceContent(this.CurrentResourceState);

            this.jsonResourceDeserializer.AssertJsonCondition(
                JsonNodeType.Property,
                JsonNodeType.StartObject,
                JsonNodeType.StartArray,
                JsonNodeType.EndObject,
                JsonNodeType.PrimitiveValue);
        }

        /// <summary>
        /// Reads the next entity or complex value (or primitive or collection value for an untyped collection) in a resource set.
        /// </summary>
        private void ReadNextResourceSetItem()
        {
            Debug.Assert(this.State == ODataReaderState.ResourceSetStart ||
                this.State == ODataReaderState.DeltaResourceSetStart,
                "Reading a resource set item while not in a ResourceSetStart or DeltaResourceSetStart state.");
            this.jsonResourceDeserializer.AssertJsonCondition(JsonNodeType.EndArray, JsonNodeType.PrimitiveValue,
                JsonNodeType.StartObject, JsonNodeType.StartArray);
            IEdmType resourceType = this.CurrentScope.ResourceType;

            // End of item in a resource set
            switch (this.jsonResourceDeserializer.JsonReader.NodeType)
            {
                case JsonNodeType.StartObject:
                    // another resource in a resource set
                    this.ReadResourceSetItemStart( /*propertyAndAnnotationCollector*/
                        null, this.CurrentJsonResourceSetScope.SelectedProperties);
                    break;
                case JsonNodeType.StartArray:
                    // we are at the start of a nested resource set
                    this.ReadResourceSetStart(new ODataResourceSet(), new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree));
                    break;
                case JsonNodeType.EndArray:
                    // we are at the end of a resource set
                    this.ReadResourceSetEnd();
                    break;
                case JsonNodeType.PrimitiveValue:
                    if (resourceType?.TypeKind == EdmTypeKind.Entity && !resourceType.IsOpen())
                    {
                        throw new ODataException(
                        ODataErrorStrings.ODataJsonReader_CannotReadResourcesOfResourceSet(
                            this.jsonResourceDeserializer.JsonReader.NodeType));
                    }

                    // Is this a stream, or a binary or string value with a collection that the client wants to read as a stream
                    if (!TryReadPrimitiveAsStream(resourceType))
                    {
                        // we are at a null value, or a non-null primitive value within an untyped collection
                        object primitiveValue = this.jsonResourceDeserializer.JsonReader.GetValue();
                        if (primitiveValue != null)
                        {
                            this.EnterScope(new JsonPrimitiveScope(new ODataPrimitiveValue(primitiveValue),
                                this.CurrentNavigationSource, this.CurrentResourceTypeReference, this.CurrentScope.ODataUri));
                        }
                        else
                        {
                            if (resourceType.TypeKind == EdmTypeKind.Primitive || resourceType.TypeKind == EdmTypeKind.Enum)
                            {
                                // null primitive
                                this.EnterScope(new JsonPrimitiveScope(new ODataNullValue(),
                                    this.CurrentNavigationSource, this.CurrentResourceTypeReference, this.CurrentScope.ODataUri));
                            }
                            else
                            {
                                // null resource (ReadResourceStart will raise the appropriate error for a non-null primitive value)
                                this.ReadResourceSetItemStart( /*propertyAndAnnotationCollector*/
                                    null, this.CurrentJsonResourceSetScope.SelectedProperties);
                            }
                        }
                    }

                    break;
                default:
                    throw new ODataException(
                        ODataErrorStrings.ODataJsonReader_CannotReadResourcesOfResourceSet(
                            this.jsonResourceDeserializer.JsonReader.NodeType));
            }
        }

        private bool TryReadPrimitiveAsStream(IEdmType resourceType)
        {
            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> readAsStream = this.jsonInputContext.MessageReaderSettings.ReadAsStreamFunc;

            // Should stream primitive if
            // 1. Primitive is a stream value
            // 2. Primitive is a string or binary value (within an untyped or streamed collection) that the reader wants to read as a stream
            if (
                (resourceType != null && resourceType.IsStream()) ||
                (resourceType != null
                   && readAsStream != null
                   && (resourceType.IsBinary() || resourceType.IsString())
                   && readAsStream(resourceType as IEdmPrimitiveType, false, null, null)))
            {
                if (resourceType == null || resourceType.IsUntyped())
                {
                    this.StartNestedStreamInfo(new ODataJsonReaderStreamInfo(
                        EdmPrimitiveTypeKind.None));
                }
                else if (resourceType.IsString())
                {
                    this.StartNestedStreamInfo(new ODataJsonReaderStreamInfo(
                        EdmPrimitiveTypeKind.String));
                }
                else if (resourceType.IsStream() || resourceType.IsBinary())
                {
                    this.StartNestedStreamInfo(new ODataJsonReaderStreamInfo(EdmPrimitiveTypeKind.Binary));
                }
                else
                {
                    Debug.Assert(false, "We thought we could read as stream, but ran out of options");
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads the next item in a nested resource info content in a request payload.
        /// </summary>
        private void ReadNextNestedResourceInfoContentItemInRequest()
        {
            Debug.Assert(this.CurrentScope.State == ODataReaderState.NestedResourceInfoStart,
                "Must be on 'NestedResourceInfoStart' scope.");

            ODataJsonReaderNestedResourceInfo nestedResourceInfo =
                this.CurrentJsonNestedResourceInfoScope.ReaderNestedResourceInfo;
            if (nestedResourceInfo.HasEntityReferenceLink)
            {
                this.EnterScope(new Scope(ODataReaderState.EntityReferenceLink, nestedResourceInfo.ReportEntityReferenceLink(), this.CurrentScope.ODataUri));
            }
            else if (nestedResourceInfo.HasValue)
            {
                if (nestedResourceInfo.NestedResourceInfo.IsCollection == true)
                {
                    // because this is a request, there is no $select query option.
                    SelectedPropertiesNode selectedProperties = new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree);
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
        private void StartResource(IEdmNavigationSource source, IEdmTypeReference resourceType, PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            SelectedPropertiesNode selectedProperties)
        {
            this.EnterScope(new JsonResourceScope(
                ODataReaderState.ResourceStart,
                ReaderUtils.CreateNewResource(),
                source,
                resourceType,
                propertyAndAnnotationCollector ?? this.jsonInputContext.CreatePropertyAndAnnotationCollector(),
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
        private void StartDeletedResource(ODataDeletedResource deletedResource, IEdmNavigationSource source, IEdmTypeReference resourceType, PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            SelectedPropertiesNode selectedProperties, bool is40DeletedResource = false)
        {
            this.EnterScope(new JsonDeletedResourceScope(
                ODataReaderState.DeletedResourceStart,
                deletedResource,
                source,
                resourceType,
                propertyAndAnnotationCollector ?? this.jsonInputContext.CreatePropertyAndAnnotationCollector(),
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

            this.EnterScope(new JsonDeltaLinkScope(
                state,
                link,
                this.CurrentNavigationSource,
                this.CurrentResourceType as IEdmEntityType,
                this.CurrentScope.ODataUri));

            this.jsonResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

            // Read source property.
            this.jsonResourceDeserializer.ReadDeltaLinkSource(link);

            // Read relationship property.
            this.jsonResourceDeserializer.ReadDeltaLinkRelationship(link);

            // Read target property.
            this.jsonResourceDeserializer.ReadDeltaLinkTarget(link);

            Debug.Assert(this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject, "Unexpected content in a delta (deleted) link");
        }

        /// <summary>
        /// Starts the nested resource info.
        /// Does metadata validation of the nested resource info and sets up the reader to report it.
        /// </summary>
        /// <param name="readerNestedResourceInfo">The nested resource info for the nested resource info to start.</param>
        private void StartNestedResourceInfo(ODataJsonReaderNestedResourceInfo readerNestedResourceInfo)
        {
            Debug.Assert(readerNestedResourceInfo != null, "readerNestedResourceInfo != null");
            ODataNestedResourceInfo nestedResourceInfo = readerNestedResourceInfo.NestedResourceInfo;
            IEdmProperty nestedProperty = readerNestedResourceInfo.NestedProperty;
            IEdmTypeReference targetResourceTypeReference = readerNestedResourceInfo.NestedResourceTypeReference;

            Debug.Assert(
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue &&
                this.jsonResourceDeserializer.JsonReader.GetValue() == null,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.Primitive (null), or JsonNodeType.Property, JsonNodeType.EndObject");
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(!string.IsNullOrEmpty(nestedResourceInfo.Name), "Navigation links must have a name.");
            Debug.Assert(nestedProperty == null || nestedResourceInfo.Name == nestedProperty.Name,
                "The navigation property must match the nested resource info.");

            // we are at the beginning of a link
            if (targetResourceTypeReference == null && nestedProperty != null)
            {
                IEdmTypeReference nestedPropertyType = nestedProperty.Type;
                targetResourceTypeReference = nestedPropertyType.IsCollection()
                    ? nestedPropertyType.AsCollection().ElementType().AsStructured()
                    : nestedPropertyType.AsStructured();
            }

            // Since we don't have the entity metadata builder for the resource read out from a nested payload
            // as stated in ReadAtResourceSetEndImplementationSynchronously(), we cannot access it here which otherwise
            // would lead to an exception.
            if (this.jsonInputContext.ReadingResponse && !this.IsReadingNestedPayload
                && (targetResourceTypeReference == null || targetResourceTypeReference.Definition.IsStructuredOrStructuredCollectionType()))
            {
                // Hookup the metadata builder to the nested resource info.
                // Note that we set the metadata builder even when navigationProperty is null, which is the case when the link is undeclared.
                // For undeclared links, we will apply conventional metadata evaluation just as declared links.
                this.CurrentResourceState.ResourceTypeFromMetadata = this.ParentScope.ResourceType as IEdmStructuredType;
                ODataResourceMetadataBuilder resourceMetadataBuilder =
                    this.jsonResourceDeserializer.MetadataContext.GetResourceMetadataBuilderForReader(
                        this.CurrentResourceState,
                        this.jsonInputContext.MessageReaderSettings.EnableReadingKeyAsSegment,
                        this.ReadingDelta);
                nestedResourceInfo.MetadataBuilder = resourceMetadataBuilder;
            }

            Debug.Assert(
                this.CurrentNavigationSource != null || this.readingParameter ||
                this.CurrentNavigationSource == null && this.CurrentScope.ResourceType.IsODataComplexTypeKind(),
                "Json requires an navigation source when not reading parameter.");

            IEdmNavigationProperty navigationProperty = readerNestedResourceInfo.NavigationProperty;

            IEdmNavigationSource navigationSource;

            // Since we are entering a nested info scope, check whether the current resource is derived type in order to correctly further property or navigation property.
            JsonResourceBaseScope currentScope = this.CurrentScope as JsonResourceBaseScope;
            ODataUri odataUri = this.CurrentScope.ODataUri.Clone();
            ODataPath odataPath = odataUri.Path ?? new ODataPath();

            if (currentScope != null && currentScope.ResourceTypeFromMetadata != currentScope.ResourceType)
            {
                odataPath = odataPath.AddSegment(new TypeSegment(currentScope.ResourceType, null));
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
                        BindingPathHelper.MatchBindingPath, odataPath.Segments, out bindingPath);
            }

            if (navigationProperty != null)
            {
                if (navigationSource is IEdmContainedEntitySet)
                {
                    if (TryAppendEntitySetKeySegment(ref odataPath))
                    {
                        odataPath = odataPath.AddNavigationPropertySegment(navigationProperty, navigationSource);
                    }
                }
                else if (navigationSource != null && !(navigationSource is IEdmUnknownEntitySet))
                {
                    IEdmEntitySet entitySet = navigationSource as IEdmEntitySet;
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
                odataPath = odataPath.AddPropertySegment(nestedProperty as IEdmStructuralProperty);
            }

            odataUri.Path = odataPath;

            JsonNestedResourceInfoScope newScope = new JsonNestedResourceInfoScope(readerNestedResourceInfo, navigationSource,
                targetResourceTypeReference, odataUri);

            IEnumerable<string> derivedTypeConstraints = this.jsonInputContext.Model.GetDerivedTypeConstraints(nestedProperty);
            if (derivedTypeConstraints != null)
            {
                newScope.DerivedTypeValidator = new DerivedTypeValidator(nestedProperty.Type.ToStructuredType(), derivedTypeConstraints, "nested resource", nestedProperty.Name);
            }

            this.EnterScope(newScope);
        }

        /// <summary>
        /// Starts the nested property info.
        /// </summary>
        /// <param name="readerNestedPropertyInfo">The nested resource info for the nested resource info to start.</param>
        private void StartNestedPropertyInfo(ODataJsonReaderNestedPropertyInfo readerNestedPropertyInfo)
        {
            Debug.Assert(readerNestedPropertyInfo != null, "readerNestedResourceInfo != null");

            this.EnterScope(new JsonNestedPropertyInfoScope(readerNestedPropertyInfo, this.CurrentNavigationSource, this.CurrentScope.ODataUri));
        }

        /// <summary>
        /// Starts the nested stream info.
        /// </summary>
        /// <param name="readerStreamInfo">The nested resource info for the nested resource info to start.</param>
        private void StartNestedStreamInfo(ODataJsonReaderStreamInfo readerStreamInfo)
        {
            Debug.Assert(readerStreamInfo != null, "readerNestedResourceInfo != null");

            this.EnterScope(new JsonStreamScope(readerStreamInfo, this.CurrentNavigationSource, this.CurrentScope.ODataUri));
        }

        /// <summary>
        /// Try to append key segment.
        /// </summary>
        /// <param name="odataPath">The ODataPath to be evaluated.</param>
        /// <returns>True if successfully append key segment.</returns>
        private bool TryAppendEntitySetKeySegment(ref ODataPath odataPath)
        {
            if (EdmExtensionMethods.HasKey(this.CurrentScope.NavigationSource, this.CurrentScope.ResourceType as IEdmStructuredType))
            {
                IEdmEntityType currentEntityType = this.CurrentScope.ResourceType as IEdmEntityType;
                ODataResourceBase resource = this.CurrentScope.Item as ODataResourceBase;
                IList<KeyValuePair<string, object>> keys = ODataResourceMetadataContext.GetKeyProperties(resource, null, currentEntityType, false);
                if (keys.Count == 0)
                {
                    odataPath = null;
                    return false;
                }

                odataPath = odataPath.AddKeySegment(keys, currentEntityType, this.CurrentScope.NavigationSource);
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
            this.ReplaceScope(new Scope(state, this.Item, this.CurrentNavigationSource, this.CurrentResourceTypeReference,
                this.CurrentScope.ODataUri));
        }

        /// <summary>
        /// Called to transition into the EntryEnd state.
        /// </summary>
        private void EndEntry()
        {
            IODataJsonReaderResourceState resourceState = this.CurrentResourceState;
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

            if (!this.ReadingDelta)
            {
                this.jsonResourceDeserializer.ValidateMediaEntity(resourceState);
            }

            // In non-delta responses, ensure that all projected properties get created.
            // Also ignore cases where the resource is 'null' which happens for expanded null entries.
            if (this.jsonInputContext.ReadingResponse && !this.ReadingDelta && currentResource != null)
            {
                // If we have a projected nested resource info that was missing from the payload, report it now.
                ODataJsonReaderNestedResourceInfo unprocessedNestedResourceInfo =
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
                    new JsonResourceScope(
                        ODataReaderState.ResourceEnd,
                        (ODataResource)this.Item,
                        this.CurrentNavigationSource,
                        this.CurrentResourceTypeReference,
                        this.CurrentResourceState.PropertyAndAnnotationCollector,
                        this.CurrentResourceState.SelectedProperties,
                        this.CurrentScope.ODataUri));
            }
            else
            {
                this.EndEntry(
                    new JsonDeletedResourceScope(
                        ODataReaderState.DeletedResourceEnd,
                        (ODataDeletedResource)this.Item,
                        this.CurrentNavigationSource,
                        this.CurrentResourceTypeReference,
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
            if (this.jsonResourceDeserializer.ContextUriParseResult != null)
            {
                this.CurrentScope.ODataUri.Path = this.jsonResourceDeserializer.ContextUriParseResult.Path;

                if (this.CurrentScope.NavigationSource == null)
                {
                    this.CurrentScope.NavigationSource =
                        this.jsonResourceDeserializer.ContextUriParseResult.NavigationSource;
                }

                if (this.CurrentScope.ResourceType == null)
                {
                    IEdmType typeFromContext = this.jsonResourceDeserializer.ContextUriParseResult.EdmType;
                    if (typeFromContext != null)
                    {
                        if (typeFromContext.TypeKind == EdmTypeKind.Collection)
                        {
                            typeFromContext = ((IEdmCollectionType)typeFromContext).ElementType.Definition;
                            if (!(typeFromContext is IEdmStructuredType))
                            {
                                typeFromContext = new EdmUntypedStructuredType();
                                this.jsonResourceDeserializer.ContextUriParseResult.EdmType = new EdmCollectionType(typeFromContext.ToTypeReference());
                            }
                        }

                        IEdmStructuredType resourceType = typeFromContext as IEdmStructuredType;
                        if (resourceType == null)
                        {
                            resourceType = new EdmUntypedStructuredType();
                            this.jsonResourceDeserializer.ContextUriParseResult.EdmType = resourceType;
                        }

                        this.CurrentScope.ResourceTypeReference = resourceType.ToTypeReference(true).AsStructured();
                    }
                }
            }
        }

        #endregion private methods

        #region private async methods

        /// <summary>
        /// Asynchronous implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more items can be read from the reader; otherwise false.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: when reading a resource set:    the reader is positioned on the first item in the resource set or the end array node of an empty resource set
        ///                 when reading a resource:  the first node of the first nested resource info value, null for a null expanded link or an end object
        ///                                         node if there are no navigation links.
        /// </remarks>
        private async Task<bool> ReadAtStartInternalImplementationAsync(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
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
            if (this.jsonInputContext.ReadingResponse && !this.IsReadingNestedPayload)
            {
                Debug.Assert(this.jsonResourceDeserializer.ContextUriParseResult != null,
                    "We should have failed by now if we don't have parse results for context URI.");

                // Validate the context URI parsed from the payload against the entity set and entity type passed in through the API.
                ReaderValidationUtils.ValidateResourceSetOrResourceContextUri(
                    this.jsonResourceDeserializer.ContextUriParseResult, this.CurrentScope, true);
            }

            // Get the $select query option from the metadata link, if we have one.
            string selectQueryOption = this.jsonResourceDeserializer.ContextUriParseResult == null
                ? null
                : this.jsonResourceDeserializer.ContextUriParseResult.SelectQueryOption;

            SelectedPropertiesNode selectedProperties = SelectedPropertiesNode.Create(
                selectQueryOption,
                (this.CurrentResourceTypeReference != null) ? this.CurrentResourceTypeReference.AsStructured().StructuredDefinition() : null,
                this.jsonInputContext.Model);

            if (this.ReadingResourceSet)
            {
                // Store the duplicate property names checker to use it later when reading the resource set end
                // (since we allow resourceSet-related annotations to appear after the resource set's data).
                this.topLevelScope.PropertyAndAnnotationCollector = propertyAndAnnotationCollector;

                bool isReordering = this.jsonInputContext.JsonReader is ReorderingJsonReader;

                if (this.ReadingDelta)
                {
                    ODataDeltaResourceSet resourceSet = new ODataDeltaResourceSet();

                    // Read top-level resource set annotations for delta resource sets.
                    await this.jsonResourceDeserializer.ReadTopLevelResourceSetAnnotationsAsync(
                        resourceSet,
                        propertyAndAnnotationCollector,
                        forResourceSetStart: true,
                        readAllResourceSetProperties: isReordering).ConfigureAwait(false);
                    await this.ReadDeltaResourceSetStartAsync(resourceSet, selectedProperties)
                        .ConfigureAwait(false);

                    this.jsonResourceDeserializer.AssertJsonCondition(JsonNodeType.EndArray, JsonNodeType.StartObject);
                }
                else
                {
                    ODataResourceSet resourceSet = new ODataResourceSet();
                    if (!this.IsReadingNestedPayload)
                    {
                        if (!this.readingParameter)
                        {
                            // Skip top-level resource set annotations for nested resource sets.
                            await this.jsonResourceDeserializer.ReadTopLevelResourceSetAnnotationsAsync(
                                resourceSet,
                                propertyAndAnnotationCollector,
                                forResourceSetStart: true,
                                readAllResourceSetProperties: isReordering).ConfigureAwait(false);
                        }
                        else
                        {
                            // This line will be used to read the first node of a resource set in Uri operation parameter, The first node is : '['
                            // Node is in following format:
                            // [
                            //      {...}, <------------ complex object.
                            //      {...}, <------------ complex object.
                            // ]
                            await this.jsonResourceDeserializer.JsonReader.ReadAsync()
                                .ConfigureAwait(false);
                        }
                    }

                    await this.ReadResourceSetStartAsync(resourceSet, selectedProperties)
                        .ConfigureAwait(false);
                }

                return true;
            }

            await this.ReadResourceSetItemStartAsync(propertyAndAnnotationCollector, selectedProperties)
                .ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronous implementation of the reader logic when in state 'ResourceSetStart'.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more items can be read from the reader; otherwise false.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  Any start node            - The first resource in the resource set
        ///                 JsonNodeType.EndArray     - The end of the resource set
        /// Post-Condition: The reader is positioned over the StartObject node of the first resource in the resource set or
        ///                 on the node following the resource set end in case of an empty resource set
        /// </remarks>
        private async Task<bool> ReadAtResourceSetStartInternalImplementationAsync()
        {
            await this.ReadNextResourceSetItemAsync()
                .ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronous implementation of the reader logic when in state 'ResourceSetEnd'.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more items can be read from the reader; otherwise false.
        /// </returns>
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
        private async Task<bool> ReadAtResourceSetEndInternalImplementationAsync()
        {
            Debug.Assert(this.State == ODataReaderState.ResourceSetEnd || this.State == ODataReaderState.DeltaResourceSetEnd, "Not in (delta) resource set end state.");
            Debug.Assert(
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput ||
                (this.ParentScope != null && (this.ParentScope.ResourceType == null || this.ParentScope.ResourceType.TypeKind == EdmTypeKind.Untyped) &&
                    (this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue ||
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndArray)) ||
                !this.IsTopLevel && !this.jsonInputContext.ReadingResponse,
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
                // Replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);

                return false;
            }

            if (isTopLevelResourceSet)
            {
                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");

                // Read the end-object node of the resource set object and position the reader on the next input node
                // This can hit the end of the input.
                await this.jsonResourceDeserializer.JsonReader.ReadAsync()
                    .ConfigureAwait(false);

                // Read the end-of-payload
                await this.jsonResourceDeserializer.ReadPayloadEndAsync(this.IsReadingNestedPayload)
                    .ConfigureAwait(false);

                // Replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);

                return false;
            }
            else if (isExpandedLinkContent)
            {
                // Finish reading the expanded link
                this.ReadExpandedNestedResourceInfoEnd(true);

                return true;
            }

            // Read the next item in an untyped collection
            await this.ReadNextResourceSetItemAsync()
                .ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronous implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more items can be read from the reader; otherwise false.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              end of object of the resource
        ///                 JsonNodeType.PrimitiveValue (null)  end of null expanded resource
        /// Post-Condition: The reader is positioned on the first node after the resource's end-object node
        /// </remarks>
        private async Task<bool> ReadAtResourceEndInternalImplementationAsync()
        {
            Debug.Assert(
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                (this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue &&
                await this.jsonResourceDeserializer.JsonReader.GetValueAsync().ConfigureAwait(false) == null),
                "Pre-Condition: JsonNodeType.EndObject or JsonNodeType.PrimitiveValue (null)");

            // We have to cache these values here, since the PopScope below will destroy them.
            bool isTopLevel = this.IsTopLevel;
            bool isExpandedLinkContent = this.IsExpandedLinkContent;

            this.PopScope(this.State == ODataReaderState.ResourceEnd ? ODataReaderState.ResourceEnd : ODataReaderState.DeletedResourceEnd);

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This can hit the end of the input.
            await this.jsonResourceDeserializer.JsonReader.ReadAsync()
                .ConfigureAwait(false);

            // Analyze the next Json token to determine whether it is start object (next resource), end array (resource set end) or eof (top-level resource end)
            bool result = true;

            if (isTopLevel)
            {
                // NOTE: We rely on the underlying JSON reader to fail if there is more than one value at the root level.
                Debug.Assert(
                    this.IsReadingNestedPayload ||
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput,
                    "Expected JSON reader to have reached the end of input when not reading a nested payload.");

                // Read the end-of-payload
                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
                await this.jsonResourceDeserializer.ReadPayloadEndAsync(this.IsReadingNestedPayload)
                    .ConfigureAwait(false);
                Debug.Assert(
                    this.IsReadingNestedPayload ||
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput,
                    "Expected JSON reader to have reached the end of input when not reading a nested payload.");

                // Replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                result = false;
            }
            else if (isExpandedLinkContent)
            {
                Debug.Assert(
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject || // expanded link resource as last property of the owning resource
                    this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property, // expanded link resource with more properties on the resource
                    "Invalid JSON reader state for reading end of resource in expanded link.");

                // Finish reading the expanded link
                this.ReadExpandedNestedResourceInfoEnd(false);
            }
            else
            {
                await this.ReadNextResourceSetItemAsync()
                    .ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Asynchronous implementation of the reader logic when in state 'NestedResourceInfoStart'.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more items can be read from the reader; otherwise false.
        /// </returns>
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
        private async Task<bool> ReadAtNestedResourceInfoStartInternalImplementationAsync()
        {
            Debug.Assert(
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue &&
                await this.jsonResourceDeserializer.JsonReader.GetValueAsync().ConfigureAwait(false) == null,
                "Pre-Condition: expected JsonNodeType.Property, JsonNodeType.EndObject, JsonNodeType.StartObject, JsonNodeType.StartArray or JsonNodeType.Primitive (null)");

            ODataNestedResourceInfo currentLink = this.CurrentNestedResourceInfo;

            IODataJsonReaderResourceState parentResourceState = (IODataJsonReaderResourceState)this.ParentScope;

            if (this.jsonInputContext.ReadingResponse)
            {
                // If we are reporting a nested resource info that was projected but not included in the payload,
                // simply change state to NestedResourceInfoEnd.
                if (parentResourceState.ProcessingMissingProjectedNestedResourceInfos)
                {
                    this.ReplaceScope(ODataReaderState.NestedResourceInfoEnd);
                }
                else if (!this.jsonResourceDeserializer.JsonReader.IsOnValueNode())
                {
                    // Deferred link (nested resource info which doesn't have a value and is in the response)
                    ReaderUtils.CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(
                        parentResourceState.PropertyAndAnnotationCollector,
                        currentLink);
                    this.jsonResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

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
                    await this.ReadExpandedNestedResourceInfoStartAsync(currentLink)
                        .ConfigureAwait(false);
                }
                else
                {
                    // Expanded resource set
                    ReaderUtils.CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(
                        parentResourceState.PropertyAndAnnotationCollector, currentLink);

                    // We store the precreated expanded resource set in the nested resource info since it carries the annotations for it.
                    ODataJsonReaderNestedResourceInfo nestedResourceInfo =
                        this.CurrentJsonNestedResourceInfoScope.ReaderNestedResourceInfo;
                    Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
                    Debug.Assert(nestedResourceInfo.NestedResourceSet != null,
                        "We must have a precreated expanded resource set already.");
                    JsonResourceBaseScope parentScope = (JsonResourceBaseScope)this.ParentScope;
                    SelectedPropertiesNode parentSelectedProperties = parentScope.SelectedProperties;
                    Debug.Assert(parentSelectedProperties != null, "parentProjectedProperties != null");

                    ODataResourceSet resourceSet = nestedResourceInfo.NestedResourceSet as ODataResourceSet;
                    if (resourceSet != null)
                    {
                        await this.ReadResourceSetStartAsync(
                            resourceSet,
                            parentSelectedProperties.GetSelectedPropertiesForNavigationProperty(
                                parentScope.ResourceType,
                                currentLink.Name)).ConfigureAwait(false);
                    }
                    else
                    {
                        ODataDeltaResourceSet deltaResourceSet = nestedResourceInfo.NestedResourceSet as ODataDeltaResourceSet;
                        Debug.Assert(deltaResourceSet != null, "Nested resource collection is not a resource set or a delta resource set");
                        await this.ReadDeltaResourceSetStartAsync(
                            deltaResourceSet,
                            parentSelectedProperties.GetSelectedPropertiesForNavigationProperty(
                                parentScope.ResourceType,
                                currentLink.Name)).ConfigureAwait(false);
                    }
                }
            }
            else
            {
                // Navigation link in request - report entity reference links and then possible expanded value.
                ReaderUtils.CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(
                    parentResourceState.PropertyAndAnnotationCollector,
                    currentLink);

                await this.ReadNextNestedResourceInfoContentItemInRequestAsync()
                    .ConfigureAwait(false);
            }

            return true;
        }

        /// <summary>
        /// Asynchronously reads the next item in a nested resource info content.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more items can be read from the reader; otherwise false.
        /// </returns>
        private async Task<bool> ReadNextNestedInfoAsync()
        {
            this.jsonResourceDeserializer.AssertJsonCondition(
                JsonNodeType.EndObject,
                JsonNodeType.Property);
            Debug.Assert(this.State == ODataReaderState.ResourceStart || this.State == ODataReaderState.DeletedResourceStart,
                "Should be in (deleted) resource start state after reading stream.");

            ODataJsonReaderNestedInfo readerNestedInfo;
            IODataJsonReaderResourceState resourceState = this.CurrentResourceState;

            if (this.jsonInputContext.ReadingResponse &&
                resourceState.ProcessingMissingProjectedNestedResourceInfos)
            {
                // We are reporting navigation links that were projected but missing from the payload
                readerNestedInfo = resourceState.Resource.MetadataBuilder.GetNextUnprocessedNavigationLink();
            }
            else
            {
                readerNestedInfo = await this.jsonResourceDeserializer.ReadResourceContentAsync(resourceState)
                    .ConfigureAwait(false);
            }

            if (readerNestedInfo == null)
            {
                // End of the resource
                this.EndEntry();
            }
            else
            {
                this.ReadNestedInfo(readerNestedInfo);
            }

            return true;
        }

        /// <summary>
        /// Asynchronously reads the next item in a nested resource info content in a request payload.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private async Task ReadNextNestedResourceInfoContentItemInRequestAsync()
        {
            Debug.Assert(
                this.CurrentScope.State == ODataReaderState.NestedResourceInfoStart,
                "Must be on 'NestedResourceInfoStart' scope.");

            ODataJsonReaderNestedResourceInfo nestedResourceInfo =
                this.CurrentJsonNestedResourceInfoScope.ReaderNestedResourceInfo;
            if (nestedResourceInfo.HasEntityReferenceLink)
            {
                this.EnterScope(new Scope(ODataReaderState.EntityReferenceLink, nestedResourceInfo.ReportEntityReferenceLink(), this.CurrentScope.ODataUri));
            }
            else if (nestedResourceInfo.HasValue)
            {
                if (nestedResourceInfo.NestedResourceInfo.IsCollection == true)
                {
                    // because this is a request, there is no $select query option.
                    SelectedPropertiesNode selectedProperties = new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree);
                    ODataDeltaResourceSet deltaResourceSet = nestedResourceInfo.NestedResourceSet as ODataDeltaResourceSet;
                    if (deltaResourceSet != null)
                    {
                        await this.ReadDeltaResourceSetStartAsync(deltaResourceSet, selectedProperties)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        ODataResourceSet resourceSet = nestedResourceInfo.NestedResourceSet as ODataResourceSet;
                        await this.ReadResourceSetStartAsync(resourceSet ?? new ODataResourceSet(), selectedProperties)
                            .ConfigureAwait(false);
                    }
                }
                else
                {
                    await this.ReadExpandedNestedResourceInfoStartAsync(nestedResourceInfo.NestedResourceInfo)
                        .ConfigureAwait(false);
                }
            }
            else
            {
                // replace the 'NestedResourceInfoStart' scope with the 'NestedResourceInfoEnd' scope
                this.ReplaceScope(ODataReaderState.NestedResourceInfoEnd);
            }
        }

        /// <summary>
        /// Asynchronously starts the (deleted) link, initializing the scopes and such. This method starts a non-null resource only.
        /// </summary>
        /// <param name="state">The reader state to switch to.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private async Task StartDeltaLinkAsync(ODataReaderState state)
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

            this.EnterScope(new JsonDeltaLinkScope(
                state,
                link,
                this.CurrentNavigationSource,
                this.CurrentResourceType as IEdmEntityType,
                this.CurrentScope.ODataUri));

            this.jsonResourceDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

            // Read source property.
            await this.jsonResourceDeserializer.ReadDeltaLinkSourceAsync(link)
                .ConfigureAwait(false);

            // Read relationship property.
            await this.jsonResourceDeserializer.ReadDeltaLinkRelationshipAsync(link)
                .ConfigureAwait(false);

            // Read target property.
            await this.jsonResourceDeserializer.ReadDeltaLinkTargetAsync(link)
                .ConfigureAwait(false);

            Debug.Assert(this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject, "Unexpected content in a delta (deleted) link");
        }

        /// <summary>
        /// Asynchronously reads the start of the JSON array for the content of the resource set and sets up the reader state correctly.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <param name="resourceSet">The resource set to read the contents for.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        /// <remarks>
        /// Pre-Condition:  The first node of the resource set property value; this method will throw if the node is not
        ///                 JsonNodeType.StartArray
        /// Post-Condition: The reader is positioned on the first item in the resource set, or on the end array of the resource set.
        /// </remarks>
        private async Task ReadResourceSetStartAsync(ODataResourceSet resourceSet, SelectedPropertiesNode selectedProperties)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            await this.jsonResourceDeserializer.ReadResourceSetContentStartAsync()
                .ConfigureAwait(false);
            IJsonReader jsonReader = this.jsonResourceDeserializer.JsonReader;
            if (jsonReader.NodeType != JsonNodeType.EndArray
                && jsonReader.NodeType != JsonNodeType.StartObject
                && jsonReader.NodeType != JsonNodeType.PrimitiveValue
                && jsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonResourceDeserializer_InvalidNodeTypeForItemsInResourceSet(jsonReader.NodeType));
            }

            this.EnterScope(new JsonResourceSetScope(
                resourceSet,
                this.CurrentNavigationSource,
                this.CurrentScope.ResourceTypeReference,
                selectedProperties,
                this.CurrentScope.ODataUri,
                isDelta: false));
        }

        /// <summary>
        /// Asynchronously reads the end of the current resource set.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private async Task ReadResourceSetEndAsync()
        {
            Debug.Assert(
                this.State == ODataReaderState.ResourceSetStart || this.State == ODataReaderState.DeltaResourceSetStart,
                "Not in ResourceSetStart or DeltaResourceSetStart state when reading end of (delta) resource set.");
            Debug.Assert(this.Item is ODataResourceSetBase, "Current Item is not ResourceSetBase");

            await this.jsonResourceDeserializer.ReadResourceSetContentEndAsync()
                .ConfigureAwait(false);

            ODataJsonReaderNestedResourceInfo expandedNestedResourceInfo = null;
            JsonNestedResourceInfoScope parentNestedResourceInfoScope = (JsonNestedResourceInfoScope)this.ExpandedLinkContentParentScope;
            if (parentNestedResourceInfoScope != null)
            {
                expandedNestedResourceInfo = parentNestedResourceInfoScope.ReaderNestedResourceInfo;
            }

            if (!this.IsReadingNestedPayload && (this.IsExpandedLinkContent || this.IsTopLevel))
            {
                // Temp ban reading the instance annotation after the resource set in parameter payload. (!this.IsReadingNestedPayload => !this.readingParameter)
                // Nested resource set payload won't have a NextLink annotation after the resource set itself since the payload is NOT pageable.
                await this.jsonResourceDeserializer.ReadNextLinkAnnotationAtResourceSetEndAsync(
                    this.Item as ODataResourceSetBase,
                    expandedNestedResourceInfo,
                    this.topLevelScope.PropertyAndAnnotationCollector).ConfigureAwait(false);
            }

            this.ReplaceScope(this.State == ODataReaderState.ResourceSetStart ? ODataReaderState.ResourceSetEnd : ODataReaderState.DeltaResourceSetEnd);
        }

        /// <summary>
        /// Asynchronously reads the start of an expanded resource (null or non-null).
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
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
        private async Task ReadExpandedNestedResourceInfoStartAsync(ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");

            if (this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                Debug.Assert(
                    await this.jsonResourceDeserializer.JsonReader.GetValueAsync().ConfigureAwait(false) == null,
                    "If a primitive value is representing an expanded resource its value must be null.");

                IEdmStructuralProperty structuralProperty =
                    this.CurrentJsonNestedResourceInfoScope.ReaderNestedResourceInfo.StructuralProperty;
                if (structuralProperty != null && !structuralProperty.Type.IsNullable)
                {
                    ODataNullValueBehaviorKind nullValueReadBehaviorKind =
                        this.jsonResourceDeserializer.ReadingResponse
                            ? ODataNullValueBehaviorKind.Default
                            : this.jsonResourceDeserializer.Model.NullValueReadBehaviorKind(structuralProperty);

                    if (nullValueReadBehaviorKind == ODataNullValueBehaviorKind.Default)
                    {
                        throw new ODataException(Strings.ReaderValidationUtils_NullNamedValueForNonNullableType(
                            nestedResourceInfo.Name,
                            structuralProperty.Type.FullName()));
                    }
                }

                // Expanded null resource
                // The expected type and expected navigation source for an expanded resource are the same as for the nested resource info around it.
                this.EnterScope(new JsonResourceScope(
                    ODataReaderState.ResourceStart,
                    resource: null,
                    navigationSource: this.CurrentNavigationSource,
                    expectedResourceTypeReference: this.CurrentResourceTypeReference,
                    propertyAndAnnotationCollector: null,
                    selectedProperties: null,
                    odataUri: this.CurrentScope.ODataUri));
            }
            else
            {
                // Expanded resource
                // The expected type for an expanded resource is the same as for the nested resource info around it.
                JsonResourceBaseScope parentScope = (JsonResourceBaseScope)this.ParentScope;
                SelectedPropertiesNode parentSelectedProperties = parentScope.SelectedProperties;
                Debug.Assert(parentSelectedProperties != null, "parentProjectedProperties != null");
                await this.ReadResourceSetItemStartAsync(
                    propertyAndAnnotationCollector: null,
                    selectedProperties: parentSelectedProperties.GetSelectedPropertiesForNavigationProperty(
                        parentScope.ResourceType,
                        nestedResourceInfo.Name)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously reads the start of a (standard, delta, primitive, or null) resource and sets up the reader state correctly
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
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
        private async Task ReadResourceSetItemStartAsync(PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            SelectedPropertiesNode selectedProperties)
        {
            IEdmNavigationSource source = this.CurrentNavigationSource;
            IEdmTypeReference resourceTypeReference = this.CurrentResourceTypeReference;

            this.jsonResourceDeserializer.AssertJsonCondition(
                JsonNodeType.StartObject,
                JsonNodeType.Property,
                JsonNodeType.EndObject,
                JsonNodeType.PrimitiveValue);

            if (this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                object primitiveValue = await this.jsonResourceDeserializer.JsonReader.GetValueAsync()
                    .ConfigureAwait(false);

                if (primitiveValue != null)
                {
                    // primitive value in an untyped collection
                    if (this.CurrentResourceType.TypeKind == EdmTypeKind.Untyped)
                    {
                        this.EnterScope(new JsonPrimitiveScope(
                            new ODataPrimitiveValue(primitiveValue),
                            this.CurrentNavigationSource,
                            this.CurrentResourceTypeReference,
                            this.CurrentScope.ODataUri));
                    }
                    else
                    {
                        throw new ODataException(Strings.ODataJsonReader_UnexpectedPrimitiveValueForODataResource);
                    }
                }
                else
                {
                    // null resource
                    if (resourceTypeReference.IsComplex() || resourceTypeReference.IsUntyped())
                    {
                        this.jsonResourceDeserializer.MessageReaderSettings.Validator.ValidateNullValue(
                            this.CurrentResourceTypeReference,
                            validateNullValue: true,
                            propertyName: "",
                            isDynamicProperty: null);
                    }

                    this.EnterScope(new JsonResourceScope(
                        ODataReaderState.ResourceStart,
                        resource: null,
                        navigationSource: this.CurrentNavigationSource,
                        expectedResourceTypeReference: this.CurrentResourceTypeReference,
                        propertyAndAnnotationCollector: null,
                        selectedProperties: null,
                        odataUri: this.CurrentScope.ODataUri));
                }

                return;
            }

            // If the reader is on StartObject then read over it. This happens for entries in resource set.
            // For top-level entries the reader will be positioned on the first resource property (after odata.context if it was present).
            if (this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.StartObject)
            {
                await this.jsonResourceDeserializer.JsonReader.ReadAsync()
                    .ConfigureAwait(false);
            }

            ODataDeltaKind resourceKind = ODataDeltaKind.Resource;

            // if this is a resourceSet, expanded link, or non-top level resource in a delta result, read the contextUrl
            if (this.ReadingResourceSet || this.IsExpandedLinkContent || (this.ReadingDelta && !this.IsTopLevel))
            {
                string contextUriStr = await this.jsonResourceDeserializer.ReadContextUriAnnotationAsync(
                    ODataPayloadKind.Resource,
                    propertyAndAnnotationCollector,
                    failOnMissingContextUriAnnotation: false).ConfigureAwait(false);

                if (contextUriStr != null)
                {
                    contextUriStr = UriUtils.UriToString(this.jsonResourceDeserializer.ProcessUriFromPayload(contextUriStr));
                    ODataJsonContextUriParseResult parseResult = ODataJsonContextUriParser.Parse(
                        this.jsonResourceDeserializer.Model,
                        contextUriFromPayload: contextUriStr,
                        payloadKind: this.ReadingDelta ? ODataPayloadKind.Delta : ODataPayloadKind.Resource,
                        clientCustomTypeResolver: this.jsonResourceDeserializer.MessageReaderSettings.ClientCustomTypeResolver,
                        needParseFragment: this.jsonInputContext.ReadingResponse || this.ReadingDelta,
                        true,
                        this.jsonResourceDeserializer.BaseUri, 
                        this.CurrentNavigationSource);

                    if (parseResult != null)
                    {
                        // a top-level (deleted) resource in a delta response can come from any entity set
                        resourceKind = parseResult.DeltaKind;
                        if (this.ReadingDelta && this.IsTopLevel && (resourceKind == ODataDeltaKind.Resource || resourceKind == ODataDeltaKind.DeletedEntry))
                        {
                            IEdmStructuredType parsedType = parseResult.EdmType as IEdmStructuredType;
                            if (parsedType != null)
                            {
                                resourceTypeReference = parsedType.ToTypeReference(true);
                                source = parseResult.NavigationSource;
                            }
                        }
                        else
                        {
                            ReaderValidationUtils.ValidateResourceSetOrResourceContextUri(parseResult, this.CurrentScope, false);
                        }
                    }
                }
            }

            // If this is a resource in a delta resource set, check to see if it's a 4.01 deleted resource
            ODataDeletedResource deletedResource = null;
            if (this.ReadingDelta && (resourceKind == ODataDeltaKind.Resource || resourceKind == ODataDeltaKind.DeletedEntry))
            {
                deletedResource = await this.jsonResourceDeserializer.ReadDeletedResourceAsync()
                    .ConfigureAwait(false);
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
                    this.StartResource(source, resourceTypeReference, propertyAndAnnotationCollector, selectedProperties);

                    // Start reading the resource up to the first nested resource info
                    await this.StartReadingResourceAsync()
                        .ConfigureAwait(false);

                    break;

                case ODataDeltaKind.ResourceSet:
                    await this.ReadAtResourceSetStartImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataDeltaKind.DeletedEntry:
                    // OData 4.0 deleted entry
                    if (deletedResource == null)
                    {
                        deletedResource = await this.jsonResourceDeserializer.ReadDeletedEntryAsync()
                            .ConfigureAwait(false);
                        this.StartDeletedResource(
                            deletedResource,
                            source,
                            resourceTypeReference,
                            propertyAndAnnotationCollector,
                            selectedProperties,
                            is40DeletedResource: true);
                    }
                    else // OData 4.01 deleted entry
                    {
                        this.StartDeletedResource(
                            deletedResource,
                            source,
                            resourceTypeReference,
                            propertyAndAnnotationCollector,
                            selectedProperties);

                        // Start reading the resource up to the first nested resource info
                        await this.StartReadingResourceAsync()
                            .ConfigureAwait(false);
                    }

                    break;

                case ODataDeltaKind.DeletedLink:
                    await this.StartDeltaLinkAsync(ODataReaderState.DeltaDeletedLink)
                        .ConfigureAwait(false);
                    break;

                case ODataDeltaKind.Link:
                    await this.StartDeltaLinkAsync(ODataReaderState.DeltaLink)
                        .ConfigureAwait(false);
                    break;

                default:
                    Debug.Assert(true, "Unknown ODataDeltaKind " + resourceKind.ToString());
                    break;
            }
        }

        /// <summary>
        /// Asynchronously reads the next entity or complex value (or primitive or collection value for an untyped collection) in a resource set.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private async Task ReadNextResourceSetItemAsync()
        {
            Debug.Assert(this.State == ODataReaderState.ResourceSetStart ||
                this.State == ODataReaderState.DeltaResourceSetStart,
                "Reading a resource set item while not in a ResourceSetStart or DeltaResourceSetStart state.");
            this.jsonResourceDeserializer.AssertJsonCondition(
                JsonNodeType.EndArray,
                JsonNodeType.PrimitiveValue,
                JsonNodeType.StartObject,
                JsonNodeType.StartArray);
            IEdmType resourceType = this.CurrentScope.ResourceType;

            // End of item in a resource set
            switch (this.jsonResourceDeserializer.JsonReader.NodeType)
            {
                case JsonNodeType.StartObject:
                    // Another resource in a resource set
                    await this.ReadResourceSetItemStartAsync(
                        propertyAndAnnotationCollector: null,
                        selectedProperties: this.CurrentJsonResourceSetScope.SelectedProperties).ConfigureAwait(false);
                    break;
                case JsonNodeType.StartArray:
                    // We are at the start of a nested resource set
                    await this.ReadResourceSetStartAsync(
                        new ODataResourceSet(),
                        new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree)).ConfigureAwait(false);
                    break;
                case JsonNodeType.EndArray:
                    // We are at the end of a resource set
                    await this.ReadResourceSetEndAsync()
                        .ConfigureAwait(false);
                    break;
                case JsonNodeType.PrimitiveValue:
                    if (resourceType?.TypeKind == EdmTypeKind.Entity && !resourceType.IsOpen())
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReader_CannotReadResourcesOfResourceSet(
                            this.jsonResourceDeserializer.JsonReader.NodeType));
                    }

                    // Is this a stream, or a binary or string value with a collection that the client wants to read as a stream
                    if (!TryReadPrimitiveAsStream(resourceType))
                    {
                        // We are at a null value, or a non-null primitive value within an untyped collection
                        object primitiveValue = await this.jsonResourceDeserializer.JsonReader.GetValueAsync()
                            .ConfigureAwait(false);

                        if (primitiveValue != null)
                        {
                            this.EnterScope(new JsonPrimitiveScope(
                                new ODataPrimitiveValue(primitiveValue),
                                this.CurrentNavigationSource,
                                this.CurrentResourceTypeReference,
                                this.CurrentScope.ODataUri));
                        }
                        else
                        {
                            if (resourceType.TypeKind == EdmTypeKind.Primitive || resourceType.TypeKind == EdmTypeKind.Enum)
                            {
                                // null primitive
                                this.EnterScope(new JsonPrimitiveScope(
                                    new ODataNullValue(),
                                    this.CurrentNavigationSource,
                                    this.CurrentResourceTypeReference,
                                    this.CurrentScope.ODataUri));
                            }
                            else
                            {
                                // null resource (ReadResourceStart will raise the appropriate error for a non-null primitive value)
                                await this.ReadResourceSetItemStartAsync(
                                    propertyAndAnnotationCollector: null,
                                    selectedProperties: this.CurrentJsonResourceSetScope.SelectedProperties).ConfigureAwait(false);
                            }
                        }
                    }

                    break;
                default:
                    throw new ODataException(ODataErrorStrings.ODataJsonReader_CannotReadResourcesOfResourceSet(
                        this.jsonResourceDeserializer.JsonReader.NodeType));
            }
        }
        /// <summary>
        /// Asynchronously reads the start of the JSON array for the content of the delta resource set and sets up the reader state correctly.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <param name="deltaResourceSet">The delta resource set to read the contents for.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        /// <remarks>
        /// Pre-Condition:  The first node of the resource set property value; this method will throw if the node is not
        ///                 JsonNodeType.StartArray
        /// Post-Condition: The reader is positioned on the first item in the resource set, or on the end array of the resource set.
        /// </remarks>
        private async Task ReadDeltaResourceSetStartAsync(ODataDeltaResourceSet deltaResourceSet, SelectedPropertiesNode selectedProperties)
        {
            Debug.Assert(deltaResourceSet != null, "resourceSet != null");

            await this.jsonResourceDeserializer.ReadResourceSetContentStartAsync()
                .ConfigureAwait(false);
            IJsonReader jsonReader = this.jsonResourceDeserializer.JsonReader;
            if (jsonReader.NodeType != JsonNodeType.EndArray && jsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonResourceDeserializer_InvalidNodeTypeForItemsInResourceSet(jsonReader.NodeType));
            }

            Debug.Assert(this.CurrentResourceType is IEdmEntityType, "Delta resource type is not an entity");

            this.EnterScope(new JsonResourceSetScope(
                deltaResourceSet,
                this.CurrentNavigationSource,
                this.CurrentResourceTypeReference as IEdmEntityTypeReference,
                selectedProperties,
                this.CurrentScope.ODataUri,
                isDelta: true));
        }

        /// <summary>
        /// Asynchronously read the resource up to the first nested resource info.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private async Task StartReadingResourceAsync()
        {
            ODataResourceBase currentResource = this.Item as ODataResourceBase;

            // Read the odata.type annotation.
            await this.jsonResourceDeserializer.ReadResourceTypeNameAsync(this.CurrentResourceState)
                .ConfigureAwait(false);

            // Resolve the type name
            this.ApplyResourceTypeNameFromPayload(currentResource.TypeName);

            // Validate type with derived type validator if available
            if (this.CurrentDerivedTypeValidator != null)
            {
                this.CurrentDerivedTypeValidator.ValidateResourceType(this.CurrentResourceType);
            }

            // Validate type with resource set validator if available and not reading top-level delta resource set
            if (this.CurrentResourceSetValidator != null && !(this.ReadingDelta && this.CurrentResourceDepth == 0))
            {
                this.CurrentResourceSetValidator.ValidateResource(this.CurrentResourceType);
            }

            this.CurrentResourceState.FirstNestedInfo = await this.jsonResourceDeserializer.ReadResourceContentAsync(
                this.CurrentResourceState).ConfigureAwait(false);

            this.jsonResourceDeserializer.AssertJsonCondition(
                JsonNodeType.Property,
                JsonNodeType.StartObject,
                JsonNodeType.StartArray,
                JsonNodeType.EndObject,
                JsonNodeType.PrimitiveValue);
        }

        /// <summary>
        /// Asynchronously reads the end of the delta(deleted)link.
        /// </summary>
        /// <param name="readerState">The state of the link or deleted link being completed.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more items can be read from the reader; otherwise false.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The next annotation.
        ///                 JsonNodeType.EndObject              No more other annotation or property in the link.
        /// Post-Condition: The reader is positioned on the first node after the link's end-object node.
        /// </remarks>
        private async Task<bool> EndDeltaLinkAsync(ODataReaderState readerState)
        {
            Debug.Assert(
                readerState == ODataReaderState.DeltaLink || readerState == ODataReaderState.DeltaDeletedLink,
                "ReadAtDeltaLinkImplementation called when not on delta(deleted)link");
            Debug.Assert(
                this.jsonResourceDeserializer.JsonReader.NodeType == JsonNodeType.EndObject,
                "Not positioned at end of object after reading delta link");

            this.PopScope(readerState);

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This should never hit the end of the input.
            await this.jsonResourceDeserializer.JsonReader.ReadAsync()
                .ConfigureAwait(false);
            await this.ReadNextResourceSetItemAsync()
                .ConfigureAwait(false);
            return true;
        }

        #endregion private async methods

        #region scopes

        /// <summary>
        /// A reader top-level scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonTopLevelScope : Scope
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
            internal JsonTopLevelScope(IEdmNavigationSource navigationSource, IEdmStructuredType expectedResourceType, ODataUri odataUri)
                : base(ODataReaderState.Start, /*item*/ null, navigationSource, expectedResourceType.ToTypeReference(true), odataUri)
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
        private sealed class JsonPrimitiveScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="primitiveValue">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
            /// <param name="expectedTypeReference">The expected type reference for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            internal JsonPrimitiveScope(
                ODataValue primitiveValue,
                IEdmNavigationSource navigationSource,
                IEdmTypeReference expectedTypeReference,
                ODataUri odataUri)
                : base(ODataReaderState.Primitive, primitiveValue, navigationSource, expectedTypeReference, odataUri)
            {
                Debug.Assert(primitiveValue is ODataPrimitiveValue || primitiveValue is ODataNullValue, "Primitive value scope created with non-primitive value");
            }
        }

        /// <summary>
        /// A reader resource scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private abstract class JsonResourceBaseScope : Scope, IODataJsonReaderResourceState
        {
            /// <summary>The set of names of the navigation properties we have read so far while reading the resource.</summary>
            private List<string> navigationPropertiesRead;

            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="readerState">The reader state of the new scope that is being created.</param>
            /// <param name="resource">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
            /// <param name="expectedResourceTypeReference">The expected type reference for the scope.</param>
            /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for this resource scope.</param>
            /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedResourceTypeReference"/> has the following meaning
            ///   it's the expected base type of the resource. If the resource has no type name specified
            ///   this type will be assumed. Otherwise the specified type name must be
            ///   the expected type or a more derived type.
            /// In all cases the specified type must be an entity type.</remarks>
            protected JsonResourceBaseScope(
                ODataReaderState readerState,
                ODataResourceBase resource,
                IEdmNavigationSource navigationSource,
                IEdmTypeReference expectedResourceTypeReference,
                PropertyAndAnnotationCollector propertyAndAnnotationCollector,
                SelectedPropertiesNode selectedProperties,
                ODataUri odataUri)
                : base(readerState, resource, navigationSource, expectedResourceTypeReference, odataUri)
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
            public ODataJsonReaderNestedInfo FirstNestedInfo { get; set; }

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
            /// The resource type for this resource.
            /// </summary>
            public new IEdmStructuredType ResourceType
            {
                get
                {
                    return base.ResourceType as IEdmStructuredType;
                }
            }

            /// <summary>
            /// The resource being read.
            /// </summary>
            ODataResourceBase IODataJsonReaderResourceState.Resource
            {
                get
                {
                    Debug.Assert(
                        this.State == ODataReaderState.ResourceStart || this.State == ODataReaderState.ResourceEnd ||
                        this.State == ODataReaderState.DeletedResourceStart || this.State == ODataReaderState.DeletedResourceEnd,
                        "The IODataJsonReaderResourceState is only supported on ResourceStart or ResourceEnd scope.");
                    return (ODataResourceBase)this.Item;
                }
            }

            /// <summary>
            /// The structured type for the resource (if available).
            /// </summary>
            IEdmStructuredType IODataJsonReaderResourceState.ResourceType
            {
                get
                {
                    Debug.Assert(
                        this.State == ODataReaderState.ResourceStart || this.State == ODataReaderState.ResourceEnd | this.State == ODataReaderState.DeletedResourceStart || this.State == ODataReaderState.DeletedResourceEnd,
                        "The IODataJsonReaderResourceState is only supported on (Deleted)ResourceStart or (Deleted)ResourceEnd scope.");
                    return this.ResourceType;
                }
            }

            /// <summary>
            /// The navigation source for the resource (if available)
            /// </summary>
            IEdmNavigationSource IODataJsonReaderResourceState.NavigationSource
            {
                get { return this.NavigationSource; }
            }
        }

        /// <summary>
        /// Base class for a reader resource scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonResourceScope : JsonResourceBaseScope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="readerState">The reader state of the new scope that is being created.</param>
            /// <param name="resource">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
            /// <param name="expectedResourceTypeReference">The expected type for the scope.</param>
            /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for this resource scope.</param>
            /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedResourceTypeReference"/> has the following meaning
            ///   it's the expected base type of the resource. If the resource has no type name specified
            ///   this type will be assumed. Otherwise the specified type name must be
            ///   the expected type or a more derived type.
            /// In all cases the specified type must be an entity type.</remarks>
            internal JsonResourceScope(
                ODataReaderState readerState,
                ODataResourceBase resource,
                IEdmNavigationSource navigationSource,
                IEdmTypeReference expectedResourceTypeReference,
                PropertyAndAnnotationCollector propertyAndAnnotationCollector,
                SelectedPropertiesNode selectedProperties,
                ODataUri odataUri)
                : base(readerState, resource, navigationSource, expectedResourceTypeReference, propertyAndAnnotationCollector, selectedProperties, odataUri)
            {
            }
        }

        /// <summary>
        /// A reader deleted resource scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonDeletedResourceScope : JsonResourceBaseScope
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
            internal JsonDeletedResourceScope(
                ODataReaderState readerState,
                ODataDeletedResource resource,
                IEdmNavigationSource navigationSource,
                IEdmTypeReference expectedResourceType,
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
        private sealed class JsonResourceSetScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="resourceSet">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedResourceTypeReference">The expected type reference for the scope.</param>
            /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <param name="isDelta">True of the ResourceSetScope is for a delta resource set</param>
            /// <remarks>The <paramref name="expectedResourceTypeReference"/> has the following meaning
            ///   it's the expected base type of the entries in the resource set.
            ///   note that it might be a more derived type than the base type of the entity set for the resource set.
            /// In all cases the specified type must be an entity type.</remarks>
            internal JsonResourceSetScope(ODataResourceSetBase resourceSet, IEdmNavigationSource navigationSource, IEdmTypeReference expectedResourceTypeReference, SelectedPropertiesNode selectedProperties, ODataUri odataUri, bool isDelta)
                : base(isDelta ? ODataReaderState.DeltaResourceSetStart : ODataReaderState.ResourceSetStart, resourceSet, navigationSource, expectedResourceTypeReference, odataUri)
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
        private sealed class JsonNestedResourceInfoScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="nestedResourceInfo">The nested resource info attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedTypeReference">The expected type reference for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedTypeReference"/> is the expected base type reference the items in the nested resource info.</remarks>
            internal JsonNestedResourceInfoScope(ODataJsonReaderNestedResourceInfo nestedResourceInfo, IEdmNavigationSource navigationSource, IEdmTypeReference expectedTypeReference, ODataUri odataUri)
                : base(ODataReaderState.NestedResourceInfoStart, nestedResourceInfo.NestedResourceInfo, navigationSource, expectedTypeReference, odataUri)
            {
                this.ReaderNestedResourceInfo = nestedResourceInfo;
            }

            /// <summary>
            /// The nested resource info for the nested resource info to report.
            /// This is only used on a StartNestedResourceInfo scope in responses.
            /// </summary>
            public ODataJsonReaderNestedResourceInfo ReaderNestedResourceInfo { get; private set; }
        }

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonNestedPropertyInfoScope : Scope
        {
            /// <summary>
            /// Constructor creating a new nested property info scope.
            /// </summary>
            /// <param name="nestedPropertyInfo">The nested property info attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            internal JsonNestedPropertyInfoScope(ODataJsonReaderNestedPropertyInfo nestedPropertyInfo, IEdmNavigationSource navigationSource, ODataUri odataUri)
                : base(ODataReaderState.NestedProperty, nestedPropertyInfo.NestedPropertyInfo,
                      navigationSource, EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Stream, true), odataUri)
            {
                Debug.Assert(nestedPropertyInfo != null, "JsonNestedInfoScope created with a null nestedPropertyInfo");
                ReaderNestedPropertyInfo = nestedPropertyInfo;
            }

            /// <summary>
            /// The nested property info for the nested property info to report.
            /// </summary>
            public ODataJsonReaderNestedPropertyInfo ReaderNestedPropertyInfo { get; }
        }

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonStreamScope : StreamScope
        {
            /// <summary>
            /// Constructor creating a new nested property info scope.
            /// </summary>
            /// <param name="streamInfo">The stream info attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            internal JsonStreamScope(ODataJsonReaderStreamInfo streamInfo, IEdmNavigationSource navigationSource, ODataUri odataUri)
                : base(ODataReaderState.Stream, new ODataStreamItem(streamInfo.PrimitiveTypeKind, streamInfo.ContentType),
                      navigationSource, EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Stream, true), odataUri)
            {
                Debug.Assert(streamInfo != null, "JsonNestedStreamScope created with a null streamInfo");
            }
        }

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonDeltaLinkScope : Scope
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
            public JsonDeltaLinkScope(ODataReaderState state, ODataDeltaLinkBase link, IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType, ODataUri odataUri)
                : base(state, link, navigationSource, expectedEntityType.ToTypeReference(true), odataUri)
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
