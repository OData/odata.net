//---------------------------------------------------------------------
// <copyright file="ODataJsonLightCollectionSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces

    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// OData JsonLight serializer for collections.
    /// </summary>
    internal sealed class ODataJsonLightCollectionSerializer : ODataJsonLightValueSerializer
    {
        /// <summary>true when writing a top-level collection that requires the 'value' wrapper object; otherwise false.</summary>
        private readonly bool writingTopLevelCollection;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="writingTopLevelCollection">true when writing a top-level collection that requires the 'value' wrapper object; otherwise false.</param>
        internal ODataJsonLightCollectionSerializer(ODataJsonLightOutputContext jsonLightOutputContext, bool writingTopLevelCollection)
            : base(jsonLightOutputContext, /*initContextUriBuilder*/true)
        {
            this.writingTopLevelCollection = writingTopLevelCollection;
        }

        /// <summary>
        /// Writes the start of a collection.
        /// </summary>
        /// <param name="collectionStart">The collection start to write.</param>
        /// <param name="itemTypeReference">The item type of the collection or null if no metadata is available.</param>
        internal void WriteCollectionStart(ODataCollectionStart collectionStart, IEdmTypeReference itemTypeReference)
        {
            Debug.Assert(collectionStart != null, "collectionStart != null");

            if (this.writingTopLevelCollection)
            {
                // "{"
                this.JsonWriter.StartObjectScope();

                // "@odata.context":...
                this.WriteContextUriProperty(ODataPayloadKind.Collection, () => ODataContextUrlInfo.Create(collectionStart.SerializationInfo, itemTypeReference));

                // "@odata.count":...
                if (collectionStart.Count.HasValue)
                {
                    this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataCount);
                    this.JsonWriter.WriteValue(collectionStart.Count.Value);
                }

                // "@odata.nextlink":...
                if (collectionStart.NextPageLink != null)
                {
                    this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataNextLink);
                    this.JsonWriter.WriteValue(this.UriToString(collectionStart.NextPageLink));
                }

                // "value":
                this.JsonWriter.WriteValuePropertyName();
            }

            // Write the start of the array for the collection items
            // "["
            this.JsonWriter.StartArrayScope();
        }

        /// <summary>
        /// Writes the end of a collection.
        /// </summary>
        internal void WriteCollectionEnd()
        {
            // Write the end of the array for the collection items
            // "]"
            this.JsonWriter.EndArrayScope();

            if (this.writingTopLevelCollection)
            {
                // "}"
                this.JsonWriter.EndObjectScope();
            }
        }

        /// <summary>
        /// Asynchronously writes the start of a collection.
        /// </summary>
        /// <param name="collectionStart">The collection start to write.</param>
        /// <param name="itemTypeReference">The item type of the collection or null if no metadata is available.</param>
        internal async Task WriteCollectionStartAsync(ODataCollectionStart collectionStart, IEdmTypeReference itemTypeReference)
        {
            Debug.Assert(collectionStart != null, "collectionStart != null");

            if (this.writingTopLevelCollection)
            {
                // "{"
                await this.AsynchronousJsonWriter.StartObjectScopeAsync()
                    .ConfigureAwait(false);

                // "@odata.context":...
                await this.WriteContextUriPropertyAsync(
                    ODataPayloadKind.Collection,
                    () => ODataContextUrlInfo.Create(collectionStart.SerializationInfo, itemTypeReference))
                        .ConfigureAwait(false);

                // "@odata.count":...
                if (collectionStart.Count.HasValue)
                {
                    await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataCount)
                        .ConfigureAwait(false);
                    await this.AsynchronousJsonWriter.WriteValueAsync(collectionStart.Count.Value)
                        .ConfigureAwait(false);
                }

                // "@odata.nextlink":...
                if (collectionStart.NextPageLink != null)
                {
                    await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataNextLink)
                        .ConfigureAwait(false);
                    await this.AsynchronousJsonWriter.WriteValueAsync(this.UriToString(collectionStart.NextPageLink))
                        .ConfigureAwait(false);
                }

                // "value":
                await this.AsynchronousJsonWriter.WriteValuePropertyNameAsync()
                    .ConfigureAwait(false);
            }

            // Write the start of the array for the collection items
            // "["
            await this.AsynchronousJsonWriter.StartArrayScopeAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the end of a collection.
        /// </summary>
        internal async Task WriteCollectionEndAsync()
        {
            // Write the end of the array for the collection items
            // "]"
            await this.AsynchronousJsonWriter.EndArrayScopeAsync()
                .ConfigureAwait(false);

            if (this.writingTopLevelCollection)
            {
                // "}"
                await this.AsynchronousJsonWriter.EndObjectScopeAsync()
                    .ConfigureAwait(false);
            }
        }
    }
}
