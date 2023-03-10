//---------------------------------------------------------------------
// <copyright file="ODataWriterWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using Microsoft.OData;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Forwards calls to an OData Writer
    /// </summary>
    internal class ODataWriterWrapper
    {
        /// <summary> The odataWriter. </summary>
        private readonly ODataWriter odataWriter;

        /// <summary> The payload writing events. </summary>
        private readonly DataServiceClientRequestPipelineConfiguration requestPipeline;

        /// <summary>
        /// Prevents a default instance of the <see cref="ODataWriterWrapper"/> class from being created.
        /// </summary>
        /// <param name="odataWriter">The odata writer.</param>
        /// <param name="requestPipeline">The request pipeline configuration.</param>
        private ODataWriterWrapper(ODataWriter odataWriter, DataServiceClientRequestPipelineConfiguration requestPipeline)
        {
            Debug.Assert(odataWriter != null, "odataWriter != null");
            Debug.Assert(requestPipeline != null, "DataServiceClientRequestPipelineConfiguration != null");

            this.odataWriter = odataWriter;
            this.requestPipeline = requestPipeline;
        }

        /// <summary>
        /// Creates the odata entry writer.
        /// </summary>
        /// <remarks>We never create a feed writer as the client doesn't support deep insert, if we did this would need to change</remarks>
        /// <param name="messageWriter">The message writer.</param>
        /// <param name="requestPipeline">The request pipeline configuration.</param>
        /// <returns>The odata Writer Wrapper</returns>
        internal static ODataWriterWrapper CreateForEntry(ODataMessageWriter messageWriter, DataServiceClientRequestPipelineConfiguration requestPipeline)
        {
            return new ODataWriterWrapper(messageWriter.CreateODataResourceWriter(), requestPipeline);
        }

        /// <summary>
        /// Creates the OData delta feed writer.
        /// </summary>
        /// <param name="messageWriter">The message writer.</param>
        /// <param name="entitySetName">The entity set name of the top-level descriptor.</param>
        /// <param name="requestPipeline">The request pipeline configuration.</param>
        /// <param name="messageWrapper">The messagewrapper.</param>
        /// <param name="requestInfo">Where to pull changes from.</param>
        /// <returns>The OData writer wrapper.</returns>
        internal static ODataWriterWrapper CreateForDeltaFeed(ODataMessageWriter messageWriter, string entitySetName, DataServiceClientRequestPipelineConfiguration requestPipeline, ODataRequestMessageWrapper messageWrapper, RequestInfo requestInfo)
        {
            EntityDescriptor entityDescriptor = messageWrapper.Descriptor as EntityDescriptor;
            IEdmEntityContainer entityContainer = requestInfo.Format.ServiceModel.EntityContainer;
            IEdmStructuredType entityType = requestInfo.Model.GetOrCreateEdmType(entityDescriptor.Entity.GetType()) as IEdmStructuredType;
            
            Debug.Assert(entityType != null, "entityType != null");

            if (!string.IsNullOrEmpty(entityDescriptor.EntitySetName))
            {
                entitySetName = entityDescriptor.EntitySetName;
            }

            IEdmEntitySet edmEntitySet = new EdmEntitySet(entityContainer, entitySetName, entityType as IEdmEntityType);
  
            if (edmEntitySet == null)
            {
                throw Error.InvalidOperation(Strings.DataBinding_Util_UnknownEntitySetName(entityDescriptor.Entity.GetType().FullName));
            }

            return new ODataWriterWrapper(messageWriter.CreateODataDeltaResourceSetWriter(edmEntitySet, entityType), requestPipeline);
        }

        /// <summary>
        /// Creates the odata entry writer for testing purposes only
        /// </summary>
        /// <param name="writer">The odata writer.</param>
        /// <param name="requestPipeline">The request pipeline configuration.</param>
        /// <returns>The odata Writer Wrapper</returns>
        internal static ODataWriterWrapper CreateForEntryTest(ODataWriter writer, DataServiceClientRequestPipelineConfiguration requestPipeline)
        {
            return new ODataWriterWrapper(writer, requestPipeline);
        }

        /// <summary>
        /// Writes the start.
        /// </summary>
        /// <param name="resourceSet">The resource set.</param>
        internal void WriteStart(ODataResourceSet resourceSet)
        {
            this.odataWriter.WriteStart(resourceSet);
        }

        /// <summary>
        /// Writes the start of the deleted resource.
        /// </summary>
        /// <param name="deletedResource">The deleted resource.</param>
        internal void WriteStart(ODataDeletedResource deletedResource)
        {
            this.odataWriter.WriteStart(deletedResource);
        }

        /// <summary>
        /// Writes the start of the delta resource set.
        /// </summary>
        /// <param name="resourceSet">The delta resource set.</param>
        internal void WriteStart(ODataDeltaResourceSet resourceSet)
        {
            this.odataWriter.WriteStart(resourceSet);
        }

        /// <summary>
        /// Writes the end.
        /// </summary>
        internal void WriteEnd()
        {
            this.odataWriter.WriteEnd();
        }

        /// <summary>
        /// Writes the start.
        /// </summary>
        /// <param name="resource">The resource.</param>
        internal void WriteStartResource(ODataResource resource)
        {
            this.odataWriter.WriteStart(resource);
        }

        /// <summary>
        /// Writes the start for the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="entity">The entity.</param>
        internal void WriteStart(ODataResource resource, object entity)
        {
            this.requestPipeline.ExecuteOnEntryStartActions(resource, entity);
            this.odataWriter.WriteStart(resource);
        }

        /// <summary>
        /// Writes the end.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="entity">The entity.</param>
        internal void WriteEnd(ODataResource entry, object entity)
        {
            this.requestPipeline.ExecuteOnEntryEndActions(entry, entity);
            this.odataWriter.WriteEnd();
        }

        /// <summary>
        /// Writes the end.
        /// </summary>
        /// <param name="navlink">The link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void WriteEnd(ODataNestedResourceInfo navlink, object source, object target)
        {
            this.requestPipeline.ExecuteOnNestedResourceInfoEndActions(navlink, source, target);
            this.odataWriter.WriteEnd();
        }

        /// <summary>
        /// Writes the end Actions.
        /// </summary>
        /// <param name="navlink">The link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void WriteNestedResourceInfoEnd(ODataNestedResourceInfo navlink, object source, object target)
        {
            this.requestPipeline.ExecuteOnNestedResourceInfoEndActions(navlink, source, target);
        }

        /// <summary>
        /// Writes the navigation link end.
        /// </summary>
        internal void WriteNestedResourceInfoEnd()
        {
            this.odataWriter.WriteEnd();
        }

        /// <summary>
        /// Writes the start.
        /// </summary>
        /// <param name="navigationLink">The navigation link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void WriteStart(ODataNestedResourceInfo navigationLink, object source, object target)
        {
            this.requestPipeline.ExecuteOnNestedResourceInfoStartActions(navigationLink, source, target);
            this.odataWriter.WriteStart(navigationLink);
        }

        /// <summary>
        /// Writes the start for Navigation link.
        /// </summary>
        /// <param name="navigationLink">The navigation link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void WriteNestedResourceInfoStart(ODataNestedResourceInfo navigationLink, object source, object target)
        {
            this.requestPipeline.ExecuteOnNestedResourceInfoStartActions(navigationLink, source, target);
        }

        /// <summary>
        /// Writes the start for a collection of navigation links.
        /// </summary>
        /// <param name="navigationLink">The navigation link.</param>
        internal void WriteNestedResourceInfoStart(ODataNestedResourceInfo navigationLink)
        {
            this.odataWriter.WriteStart(navigationLink);
        }

        /// <summary>
        /// Writes the entity reference link.
        /// </summary>
        /// <param name="referenceLink">The reference link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void WriteEntityReferenceLink(ODataEntityReferenceLink referenceLink, object source, object target)
        {
            this.requestPipeline.ExecuteEntityReferenceLinkActions(referenceLink, source, target);
            this.odataWriter.WriteEntityReferenceLink(referenceLink);
        }
    }
}
