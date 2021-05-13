//---------------------------------------------------------------------
// <copyright file="ODataJsonLightEntityReferenceLinkSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    #endregion Namespaces

    /// <summary>
    /// OData JsonLight serializer for entity reference links.
    /// </summary>
    internal sealed class ODataJsonLightEntityReferenceLinkSerializer : ODataJsonLightSerializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        internal ODataJsonLightEntityReferenceLinkSerializer(ODataJsonLightOutputContext jsonLightOutputContext)
            : base(jsonLightOutputContext, /*initContextUriBuilder*/ true)
        {
        }

        /// <summary>
        /// Writes a single top-level Uri in response to a $ref query.
        /// </summary>
        /// <param name="link">The entity reference link to write out.</param>
        internal void WriteEntityReferenceLink(ODataEntityReferenceLink link)
        {
            Debug.Assert(link != null, "link != null");

            this.WriteTopLevelPayload(
                () => this.WriteEntityReferenceLinkImplementation(link, /* isTopLevel */ true));
        }

        /// <summary>
        /// Writes a set of links (Uris) in response to a $ref query; includes optional count and next-page-link information.
        /// </summary>
        /// <param name="entityReferenceLinks">The set of entity reference links to write out.</param>
        internal void WriteEntityReferenceLinks(ODataEntityReferenceLinks entityReferenceLinks)
        {
            Debug.Assert(entityReferenceLinks != null, "entityReferenceLinks != null");

            this.WriteTopLevelPayload(
                () => this.WriteEntityReferenceLinksImplementation(entityReferenceLinks));
        }

        /// <summary>
        /// Asynchronously writes a single top-level Uri in response to a $ref query.
        /// </summary>
        /// <param name="link">The entity reference link to write out.</param>
        internal Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink link)
        {
            Debug.Assert(link != null, "link != null");

            return this.WriteTopLevelPayloadAsync(
                () => this.WriteEntityReferenceLinkImplementationAsync(link, /* isTopLevel */ true));
        }

        /// <summary>
        /// Asynchronously writes a set of links (Uris) in response to a $ref query; includes optional count and next-page-link information.
        /// </summary>
        /// <param name="entityReferenceLinks">The set of entity reference links to write out.</param>
        internal Task WriteEntityReferenceLinksAsync(ODataEntityReferenceLinks entityReferenceLinks)
        {
            Debug.Assert(entityReferenceLinks != null, "entityReferenceLinks != null");

            return this.WriteTopLevelPayloadAsync(
                () => this.WriteEntityReferenceLinksImplementationAsync(entityReferenceLinks));
        }

        /// <summary>
        /// Writes a single Uri in response to a $ref query.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to write out.</param>
        /// <param name="isTopLevel">true if the entity reference link being written is at the top level of the payload.</param>
        private void WriteEntityReferenceLinkImplementation(ODataEntityReferenceLink entityReferenceLink, bool isTopLevel)
        {
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            WriterValidationUtils.ValidateEntityReferenceLink(entityReferenceLink);

            this.JsonWriter.StartObjectScope();

            if (isTopLevel)
            {
                this.WriteContextUriProperty(ODataPayloadKind.EntityReferenceLink);
            }

            this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataId);
            this.JsonWriter.WriteValue(this.UriToString(entityReferenceLink.Url));

            this.InstanceAnnotationWriter.WriteInstanceAnnotations(entityReferenceLink.InstanceAnnotations);
            this.JsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes a set of links (Uris) in response to a $ref query; includes optional count and next-page-link information.
        /// </summary>
        /// <param name="entityReferenceLinks">The set of entity reference links to write out.</param>
        private void WriteEntityReferenceLinksImplementation(ODataEntityReferenceLinks entityReferenceLinks)
        {
            Debug.Assert(entityReferenceLinks != null, "entityReferenceLinks != null");

            bool wroteNextLink = false;

            // {
            this.JsonWriter.StartObjectScope();

            // "@odata.context": ...
            this.WriteContextUriProperty(ODataPayloadKind.EntityReferenceLinks);

            if (entityReferenceLinks.Count.HasValue)
            {
                // We try to write the count property at the top of the payload if one is available.
                // "@odata.count": ...
                this.WriteCountAnnotation(entityReferenceLinks.Count.Value);
            }

            if (entityReferenceLinks.NextPageLink != null)
            {
                // We try to write the next link at the top of the payload if one is available. If not, we try again at the end.
                wroteNextLink = true;

                // "@odata.next": ...
                this.WriteNextLinkAnnotation(entityReferenceLinks.NextPageLink);
            }

            this.InstanceAnnotationWriter.WriteInstanceAnnotations(entityReferenceLinks.InstanceAnnotations);

            // "value":
            this.JsonWriter.WriteValuePropertyName();

            // "[":
            this.JsonWriter.StartArrayScope();

            IEnumerable<ODataEntityReferenceLink> entityReferenceLinksEnumerable = entityReferenceLinks.Links;
            if (entityReferenceLinksEnumerable != null)
            {
                foreach (ODataEntityReferenceLink entityReferenceLink in entityReferenceLinksEnumerable)
                {
                    WriterValidationUtils.ValidateEntityReferenceLinkNotNull(entityReferenceLink);
                    this.WriteEntityReferenceLinkImplementation(entityReferenceLink, /* isTopLevel */ false);
                }
            }

            // "]"
            this.JsonWriter.EndArrayScope();

            if (!wroteNextLink && entityReferenceLinks.NextPageLink != null)
            {
                // "@odata.next": ...
                this.WriteNextLinkAnnotation(entityReferenceLinks.NextPageLink);
            }

            // "}"
            this.JsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes the next link property, which consists of the property name and value.
        /// </summary>
        /// <param name="nextPageLink">The non-null value of the next link to write.</param>
        private void WriteNextLinkAnnotation(Uri nextPageLink)
        {
            Debug.Assert(nextPageLink != null, "Expected non-null next link.");

            this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataNextLink);
            this.JsonWriter.WriteValue(this.UriToString(nextPageLink));
        }

        /// <summary>
        /// Writes the odata.count property, which consists of the property name and value.
        /// </summary>
        /// <param name="countValue">The value of the count property to write.</param>
        private void WriteCountAnnotation(long countValue)
        {
            this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataCount);
            this.JsonWriter.WriteValue(countValue);
        }

        /// <summary>
        /// Asynchronously writes a single Uri in response to a $ref query.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to write out.</param>
        /// <param name="isTopLevel">true if the entity reference link being written is at the top level of the payload.</param>
        private async Task WriteEntityReferenceLinkImplementationAsync(ODataEntityReferenceLink entityReferenceLink, bool isTopLevel)
        {
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            WriterValidationUtils.ValidateEntityReferenceLink(entityReferenceLink);

            await this.AsynchronousJsonWriter.StartObjectScopeAsync()
                .ConfigureAwait(false);

            if (isTopLevel)
            {
                await this.WriteContextUriPropertyAsync(ODataPayloadKind.EntityReferenceLink)
                    .ConfigureAwait(false);
            }

            await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataId)
                .ConfigureAwait(false);
            await this.AsynchronousJsonWriter.WriteValueAsync(this.UriToString(entityReferenceLink.Url))
                .ConfigureAwait(false);

            await this.InstanceAnnotationWriter.WriteInstanceAnnotationsAsync(entityReferenceLink.InstanceAnnotations)
                .ConfigureAwait(false);
            await this.AsynchronousJsonWriter.EndObjectScopeAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes a set of links (Uris) in response to a $ref query; includes optional count and next-page-link information.
        /// </summary>
        /// <param name="entityReferenceLinks">The set of entity reference links to write out.</param>
        private async Task WriteEntityReferenceLinksImplementationAsync(ODataEntityReferenceLinks entityReferenceLinks)
        {
            Debug.Assert(entityReferenceLinks != null, "entityReferenceLinks != null");

            bool wroteNextLink = false;

            // {
            await this.AsynchronousJsonWriter.StartObjectScopeAsync()
                .ConfigureAwait(false);

            // "@odata.context": ...
            await this.WriteContextUriPropertyAsync(ODataPayloadKind.EntityReferenceLinks)
                .ConfigureAwait(false);

            if (entityReferenceLinks.Count.HasValue)
            {
                // We try to write the count property at the top of the payload if one is available.
                // "@odata.count": ...
                await this.WriteCountAnnotationAsync(entityReferenceLinks.Count.Value)
                    .ConfigureAwait(false);
            }

            if (entityReferenceLinks.NextPageLink != null)
            {
                // We try to write the next link at the top of the payload if one is available. If not, we try again at the end.
                wroteNextLink = true;

                // "@odata.next": ...
                await this.WriteNextLinkAnnotationAsync(entityReferenceLinks.NextPageLink)
                    .ConfigureAwait(false);
            }

            this.InstanceAnnotationWriter.WriteInstanceAnnotations(entityReferenceLinks.InstanceAnnotations);

            // "value":
            await this.AsynchronousJsonWriter.WriteValuePropertyNameAsync()
                .ConfigureAwait(false);

            // "[":
            await this.AsynchronousJsonWriter.StartArrayScopeAsync()
                .ConfigureAwait(false);

            IEnumerable<ODataEntityReferenceLink> entityReferenceLinksEnumerable = entityReferenceLinks.Links;
            if (entityReferenceLinksEnumerable != null)
            {
                foreach (ODataEntityReferenceLink entityReferenceLink in entityReferenceLinksEnumerable)
                {
                    WriterValidationUtils.ValidateEntityReferenceLinkNotNull(entityReferenceLink);
                    await this.WriteEntityReferenceLinkImplementationAsync(entityReferenceLink, /* isTopLevel */ false)
                        .ConfigureAwait(false);
                }
            }

            // "]"
            await this.AsynchronousJsonWriter.EndArrayScopeAsync()
                .ConfigureAwait(false);

            if (!wroteNextLink && entityReferenceLinks.NextPageLink != null)
            {
                // "@odata.next": ...
                await this.WriteNextLinkAnnotationAsync(entityReferenceLinks.NextPageLink)
                    .ConfigureAwait(false);
            }

            // "}"
            await this.AsynchronousJsonWriter.EndObjectScopeAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the next link property, which consists of the property name and value.
        /// </summary>
        /// <param name="nextPageLink">The non-null value of the next link to write.</param>
        private async Task WriteNextLinkAnnotationAsync(Uri nextPageLink)
        {
            Debug.Assert(nextPageLink != null, "Expected non-null next link.");

            await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataNextLink)
                .ConfigureAwait(false);
            await this.AsynchronousJsonWriter.WriteValueAsync(this.UriToString(nextPageLink))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the odata.count property, which consists of the property name and value.
        /// </summary>
        /// <param name="countValue">The value of the count property to write.</param>
        private async Task WriteCountAnnotationAsync(long countValue)
        {
            await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataCount)
                .ConfigureAwait(false);
            await this.AsynchronousJsonWriter.WriteValueAsync(countValue)
                .ConfigureAwait(false);
        }
    }
}
