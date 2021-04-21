//---------------------------------------------------------------------
// <copyright file="ODataJsonLightCollectionWriter.cs" company="Microsoft">
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
    /// ODataCollectionWriter for the JsonLight format.
    /// </summary>
    internal sealed class ODataJsonLightCollectionWriter : ODataCollectionWriterCore
    {
        /// <summary>
        /// The output context to write to.
        /// </summary>
        private readonly ODataJsonLightOutputContext jsonLightOutputContext;

        /// <summary>
        /// The JsonLight collection serializer to use.
        /// </summary>
        private readonly ODataJsonLightCollectionSerializer jsonLightCollectionSerializer;

        /// <summary>
        /// Constructor for creating a collection writer to use when writing operation result payloads.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        internal ODataJsonLightCollectionWriter(ODataJsonLightOutputContext jsonLightOutputContext, IEdmTypeReference itemTypeReference)
            : base(jsonLightOutputContext, itemTypeReference)
        {
            Debug.Assert(jsonLightOutputContext != null, "jsonLightOutputContext != null");

            this.jsonLightOutputContext = jsonLightOutputContext;
            this.jsonLightCollectionSerializer = new ODataJsonLightCollectionSerializer(this.jsonLightOutputContext, /*writingTopLevelCollection*/true);
        }

        /// <summary>
        /// Constructor for creating a collection writer to use when writing parameter payloads.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="expectedItemType">The type reference of the expected item type or null if no expected item type exists.</param>
        /// <param name="listener">If not null, the writer will notify the implementer of the interface of relevant state changes in the writer.</param>
        internal ODataJsonLightCollectionWriter(ODataJsonLightOutputContext jsonLightOutputContext, IEdmTypeReference expectedItemType, IODataReaderWriterListener listener)
            : base(jsonLightOutputContext, expectedItemType, listener)
        {
            Debug.Assert(jsonLightOutputContext != null, "jsonLightOutputContext != null");
            Debug.Assert(!jsonLightOutputContext.WritingResponse, "The collection writer constructor for parameter payloads must only be used for writing requests.");

            this.jsonLightOutputContext = jsonLightOutputContext;
            this.jsonLightCollectionSerializer = new ODataJsonLightCollectionSerializer(this.jsonLightOutputContext, /*writingTopLevelCollection*/false);
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
        /// Start writing an OData payload.
        /// </summary>
        protected override void StartPayload()
        {
            this.jsonLightCollectionSerializer.WritePayloadStart();
        }

        /// <summary>
        /// Finish writing an OData payload.
        /// </summary>
        protected override void EndPayload()
        {
            this.jsonLightCollectionSerializer.WritePayloadEnd();
        }

        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        protected override void StartCollection(ODataCollectionStart collectionStart)
        {
            this.jsonLightCollectionSerializer.WriteCollectionStart(collectionStart, this.ItemTypeReference);
        }

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        protected override void EndCollection()
        {
            this.jsonLightCollectionSerializer.WriteCollectionEnd();
        }

        /// <summary>
        /// Writes a collection item (either primitive, enum or resource)
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        /// <param name="expectedItemType">The expected type of the collection item or null if no expected item type exists.</param>
        protected override void WriteCollectionItem(object item, IEdmTypeReference expectedItemType)
        {
            if (item == null)
            {
                this.jsonLightOutputContext.WriterValidator.ValidateNullCollectionItem(expectedItemType);
                this.jsonLightOutputContext.JsonWriter.WriteValue((string)null);
            }
            else
            {
                ODataResourceValue resourceValue = item as ODataResourceValue;
                ODataEnumValue enumVal = null;
                if (resourceValue != null)
                {
                    this.jsonLightCollectionSerializer.AssertRecursionDepthIsZero();
                    this.jsonLightCollectionSerializer.WriteResourceValue(
                        resourceValue,
                        expectedItemType,
                        false /*isOpenPropertyType*/,
                        this.DuplicatePropertyNameChecker);
                    this.jsonLightCollectionSerializer.AssertRecursionDepthIsZero();
                    this.DuplicatePropertyNameChecker.Reset();
                }
                else if ((enumVal = item as ODataEnumValue) != null)
                {
                    if (enumVal.Value == null)
                    {
                        this.jsonLightCollectionSerializer.WriteNullValue();
                    }
                    else
                    {
                        // write ODataEnumValue.Value as string value
                        this.jsonLightCollectionSerializer.WritePrimitiveValue(enumVal.Value, EdmCoreModel.Instance.GetString(true));
                    }
                }
                else
                {
                    Debug.Assert(!(item is ODataCollectionValue), "!(item is ODataCollectionValue)");
                    Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");
                    this.jsonLightCollectionSerializer.WritePrimitiveValue(item, expectedItemType);
                }
            }
        }

        /// <summary>
        /// Asynchronously start writing an OData payload.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task StartPayloadAsync()
        {
            return this.jsonLightCollectionSerializer.WritePayloadStartAsync();
        }

        /// <summary>
        /// Asynchronously finish writing an OData payload.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task EndPayloadAsync()
        {
            return this.jsonLightCollectionSerializer.WritePayloadEndAsync();
        }

        /// <summary>
        /// Asynchronously start writing a collection.
        /// </summary>
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task StartCollectionAsync(ODataCollectionStart collectionStart)
        {
            return this.jsonLightCollectionSerializer.WriteCollectionStartAsync(collectionStart, this.ItemTypeReference);
        }

        /// <summary>
        /// Asynchronously finish writing a collection.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task EndCollectionAsync()
        {
            return this.jsonLightCollectionSerializer.WriteCollectionEndAsync();
        }

        /// <summary>
        /// Asynchronously writes a collection item (either primitive or complex)
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        /// <param name="expectedItemTypeReference">The expected type of the collection item or null if no expected item type exists.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task WriteCollectionItemAsync(object item, IEdmTypeReference expectedItemType)
        {
            if (item == null)
            {
                this.jsonLightOutputContext.WriterValidator.ValidateNullCollectionItem(expectedItemType);
                await this.jsonLightOutputContext.AsynchronousJsonWriter.WriteValueAsync((string)null)
                    .ConfigureAwait(false);
            }
            else
            {
                if (item is ODataResourceValue resourceValue)
                {
                    this.jsonLightCollectionSerializer.AssertRecursionDepthIsZero();
                    await this.jsonLightCollectionSerializer.WriteResourceValueAsync(
                        resourceValue,
                        expectedItemType,
                        false /*isOpenPropertyType*/,
                        this.DuplicatePropertyNameChecker).ConfigureAwait(false);
                    this.jsonLightCollectionSerializer.AssertRecursionDepthIsZero();
                    this.DuplicatePropertyNameChecker.Reset();
                }
                else if (item is ODataEnumValue enumVal)
                {
                    if (enumVal.Value == null)
                    {
                        await this.jsonLightCollectionSerializer.WriteNullValueAsync()
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        // write ODataEnumValue.Value as string value
                        await this.jsonLightCollectionSerializer.WritePrimitiveValueAsync(
                            enumVal.Value,
                            EdmCoreModel.Instance.GetString(true)).ConfigureAwait(false);
                    }
                }
                else
                {
                    Debug.Assert(!(item is ODataCollectionValue), "!(item is ODataCollectionValue)");
                    Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");
                    await this.jsonLightCollectionSerializer.WritePrimitiveValueAsync(item, expectedItemType)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
