//---------------------------------------------------------------------
// <copyright file="ODataJsonLightDeltaWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Implementation of the ODataDeltaWriter for the JsonLight format.
    /// </summary>
    internal sealed class ODataJsonLightDeltaWriter : ODataDeltaWriter, IODataOutputInStreamErrorListener
    {
        #region Private Fields

        /// <summary>
        /// The output context to write to.
        /// </summary>
        private readonly ODataJsonLightOutputContext jsonLightOutputContext;

        /// <summary>
        /// JsonLightWriter
        /// </summary>
        private readonly ODataJsonLightWriter resourceWriter;

        /// <summary>
        /// NavigationSource
        /// </summary>
        private IEdmNavigationSource navigationSource;

        /// <summary>
        /// EntityType
        /// </summary>
        private IEdmEntityType entityType;

        /// <summary>An in-stream error listener to notify when in-stream error is to be written. Or null if we don't need to notify anybody.</summary>
        private IODataOutputInStreamErrorListener inStreamErrorListener;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        public ODataJsonLightDeltaWriter(ODataJsonLightOutputContext jsonLightOutputContext, IEdmNavigationSource navigationSource, IEdmEntityType entityType)
        {
            Debug.Assert(jsonLightOutputContext != null, "jsonLightOutputContext != null");

            // TODO: Replace the assertion with ODataException.
            Debug.Assert(jsonLightOutputContext.WritingResponse, "jsonLightOutputContext.WritingResponse is true");

            this.navigationSource = navigationSource;
            this.entityType = entityType;
            this.jsonLightOutputContext = jsonLightOutputContext;
            this.resourceWriter = new ODataJsonLightWriter(jsonLightOutputContext, navigationSource, entityType, true, writingDelta: true);
            this.inStreamErrorListener = resourceWriter;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The navigation source we are going to write entities for.
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get
            {
                return this.navigationSource;
            }

            set
            {
                Debug.Assert(true, "NavigationSource can be set but is never used");
                this.navigationSource = value;
            }
        }

        /// <summary>
        /// The entity type we are going to write entities for.
        /// </summary>
        public IEdmEntityType EntityType
        {
            get
            {
                return this.entityType;
            }

            set
            {
                Debug.Assert(true, "EntityType can be set but is never used");
                this.entityType = value;
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Start writing a delta resource set.
        /// </summary>
        /// <param name="deltaResourceSet">Delta resource set/collection to write.</param>
        public override void WriteStart(ODataDeltaResourceSet deltaResourceSet)
        {
            this.resourceWriter.WriteStart(deltaResourceSet);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously start writing a delta resource set.
        /// </summary>
        /// <param name="deltaResourceSet">Delta resource set/collection to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync(ODataDeltaResourceSet deltaResourceSet)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.resourceWriter.WriteStart(deltaResourceSet));
        }
#endif

        /// <summary>
        /// Finish writing a delta resource set.
        /// </summary>
        public override void WriteEnd()
        {
            this.resourceWriter.WriteEnd();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously finish writing a delta resource set.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteEndAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.resourceWriter.WriteEnd());
        }
#endif

        /// <summary>
        /// Start writing a nested resource info.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        public override void WriteStart(ODataNestedResourceInfo nestedResourceInfo)
        {
            this.resourceWriter.WriteStart(nestedResourceInfo);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously start writing a nested resource info.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync(ODataNestedResourceInfo nestedResourceInfo)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.resourceWriter.WriteStart(nestedResourceInfo));
        }
#endif

        /// <summary>
        /// Start writing an expanded resource set.
        /// </summary>
        /// <param name="expandedResourceSet">The expanded resource set to write.</param>
        public override void WriteStart(ODataResourceSet expandedResourceSet)
        {
            this.resourceWriter.WriteStart(expandedResourceSet);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously start writing an expanded resource set.
        /// </summary>
        /// <param name="expandedResourceSet">The expanded resource set to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync(ODataResourceSet expandedResourceSet)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.resourceWriter.WriteStart(expandedResourceSet));
        }
#endif

        /// <summary>
        /// Start writing a delta resource.
        /// </summary>
        /// <param name="deltaResource">The delta resource to write.</param>
        public override void WriteStart(ODataResource deltaResource)
        {
            this.resourceWriter.WriteStart(deltaResource);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously start writing a delta resource.
        /// </summary>
        /// <param name="deltaResource">The delta resource to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync(ODataResource deltaResource)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.resourceWriter.WriteStart(deltaResource));
        }
#endif

        /// <summary>
        /// Writing a delta deleted resource.
        /// </summary>
        /// <param name="deltaDeletedEntry">The delta deleted resource to write.</param>
        public override void WriteDeltaDeletedEntry(ODataDeltaDeletedEntry deltaDeletedEntry)
        {
            this.resourceWriter.WriteStart(ODataDeltaDeletedEntry.GetDeletedResource(deltaDeletedEntry));
            this.resourceWriter.WriteEnd();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writing a delta deleted resource.
        /// </summary>
        /// <param name="deltaDeletedEntry">The delta deleted resource to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteDeltaDeletedEntryAsync(ODataDeltaDeletedEntry deltaDeletedEntry)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.resourceWriter.WriteStart(ODataDeltaDeletedEntry.GetDeletedResource(deltaDeletedEntry)));
        }
#endif

        /// <summary>
        /// Writes a delta link.
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        public override void WriteDeltaLink(ODataDeltaLink deltaLink)
        {
            this.resourceWriter.WriteDeltaLink(deltaLink);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writes a delta link.
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteDeltaLinkAsync(ODataDeltaLink deltaLink)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.resourceWriter.WriteDeltaLink(deltaLink));
        }
#endif

        /// <summary>
        /// Writing a delta deleted link.
        /// </summary>
        /// <param name="deltaDeletedLink">The delta deleted link to write.</param>
        public override void WriteDeltaDeletedLink(ODataDeltaDeletedLink deltaDeletedLink)
        {
            this.resourceWriter.WriteDeltaDeletedLink(deltaDeletedLink);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writing a delta deleted link.
        /// </summary>
        /// <param name="deltaDeletedLink">The delta deleted link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteDeltaDeletedLinkAsync(ODataDeltaDeletedLink deltaDeletedLink)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.resourceWriter.WriteDeltaDeletedLink(deltaDeletedLink));
        }
#endif

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public override void Flush()
        {
            this.jsonLightOutputContext.Flush();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public override Task FlushAsync()
        {
            return this.jsonLightOutputContext.FlushAsync();
        }
#endif

        /// <summary>
        /// This method notifies the listener, that an in-stream error is to be written.
        /// </summary>
        /// <remarks>
        /// This listener can choose to fail, if the currently written payload doesn't support in-stream error at this position.
        /// If the listener returns, the writer should not allow any more writing, since the in-stream error is the last thing in the payload.
        /// </remarks>
        void IODataOutputInStreamErrorListener.OnInStreamError()
        {
            this.inStreamErrorListener.OnInStreamError();
        }

        #endregion
    }
}
