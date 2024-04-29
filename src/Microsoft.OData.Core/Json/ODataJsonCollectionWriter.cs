//---------------------------------------------------------------------
// <copyright file="ODataJsonCollectionWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// ODataCollectionWriter for the Json format.
    /// </summary>
    internal sealed class ODataJsonCollectionWriter : ODataCollectionWriterCore
    {
        /// <summary>
        /// The output context to write to.
        /// </summary>
        private readonly ODataJsonOutputContext jsonOutputContext;

        /// <summary>
        /// The Json collection serializer to use.
        /// </summary>
        private readonly ODataJsonCollectionSerializer jsonCollectionSerializer;

        /// <summary>
        /// Constructor for creating a collection writer to use when writing operation result payloads.
        /// </summary>
        /// <param name="jsonOutputContext">The output context to write to.</param>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        internal ODataJsonCollectionWriter(ODataJsonOutputContext jsonOutputContext, IEdmTypeReference itemTypeReference)
            : base(jsonOutputContext, itemTypeReference)
        {
            Debug.Assert(jsonOutputContext != null, "jsonOutputContext != null");

            this.jsonOutputContext = jsonOutputContext;
            this.jsonCollectionSerializer = new ODataJsonCollectionSerializer(this.jsonOutputContext, /*writingTopLevelCollection*/true);
        }

        /// <summary>
        /// Constructor for creating a collection writer to use when writing parameter payloads.
        /// </summary>
        /// <param name="jsonOutputContext">The output context to write to.</param>
        /// <param name="expectedItemType">The type reference of the expected item type or null if no expected item type exists.</param>
        /// <param name="listener">If not null, the writer will notify the implementer of the interface of relevant state changes in the writer.</param>
        internal ODataJsonCollectionWriter(ODataJsonOutputContext jsonOutputContext, IEdmTypeReference expectedItemType, IODataReaderWriterListener listener)
            : base(jsonOutputContext, expectedItemType, listener)
        {
            Debug.Assert(jsonOutputContext != null, "jsonOutputContext != null");
            Debug.Assert(!jsonOutputContext.WritingResponse, "The collection writer constructor for parameter payloads must only be used for writing requests.");

            this.jsonOutputContext = jsonOutputContext;
            this.jsonCollectionSerializer = new ODataJsonCollectionSerializer(this.jsonOutputContext, /*writingTopLevelCollection*/false);
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
        /// Start writing an OData payload.
        /// </summary>
        protected override void StartPayload()
        {
            this.jsonCollectionSerializer.WritePayloadStart();
        }

        /// <summary>
        /// Finish writing an OData payload.
        /// </summary>
        protected override void EndPayload()
        {
            this.jsonCollectionSerializer.WritePayloadEnd();
        }

        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        protected override void StartCollection(ODataCollectionStart collectionStart)
        {
            this.jsonCollectionSerializer.WriteCollectionStart(collectionStart, this.ItemTypeReference);
        }

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        protected override void EndCollection()
        {
            this.jsonCollectionSerializer.ReturnDuplicatePropertyNameChecker(this.DuplicatePropertyNameChecker);
            this.jsonCollectionSerializer.WriteCollectionEnd();
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
                this.jsonOutputContext.WriterValidator.ValidateNullCollectionItem(expectedItemType);
                this.jsonOutputContext.JsonWriter.WriteValue((string)null);
            }
            else
            {
                ODataResourceValue resourceValue = item as ODataResourceValue;
                ODataEnumValue enumVal = null;
                if (resourceValue != null)
                {
                    this.jsonCollectionSerializer.AssertRecursionDepthIsZero();
                    this.jsonCollectionSerializer.WriteResourceValue(
                        resourceValue,
                        expectedItemType,
                        false /*isOpenPropertyType*/,
                        this.DuplicatePropertyNameChecker);
                    this.jsonCollectionSerializer.AssertRecursionDepthIsZero();
                    this.DuplicatePropertyNameChecker.Reset();
                }
                else if ((enumVal = item as ODataEnumValue) != null)
                {
                    if (enumVal.Value == null)
                    {
                        this.jsonCollectionSerializer.WriteNullValue();
                    }
                    else
                    {
                        // write ODataEnumValue.Value as string value
                        this.jsonCollectionSerializer.WritePrimitiveValue(enumVal.Value, EdmCoreModel.Instance.GetString(true));
                    }
                }
                else
                {
                    Debug.Assert(!(item is ODataCollectionValue), "!(item is ODataCollectionValue)");
                    Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");
                    this.jsonCollectionSerializer.WritePrimitiveValue(item, expectedItemType);
                }
            }
        }

        /// <summary>
        /// Asynchronously start writing an OData payload.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task StartPayloadAsync()
        {
            return this.jsonCollectionSerializer.WritePayloadStartAsync();
        }

        /// <summary>
        /// Asynchronously finish writing an OData payload.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task EndPayloadAsync()
        {
            return this.jsonCollectionSerializer.WritePayloadEndAsync();
        }

        /// <summary>
        /// Asynchronously start writing a collection.
        /// </summary>
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task StartCollectionAsync(ODataCollectionStart collectionStart)
        {
            return this.jsonCollectionSerializer.WriteCollectionStartAsync(collectionStart, this.ItemTypeReference);
        }

        /// <summary>
        /// Asynchronously finish writing a collection.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override Task EndCollectionAsync()
        {
            this.jsonCollectionSerializer.ReturnDuplicatePropertyNameChecker(this.DuplicatePropertyNameChecker);

            return this.jsonCollectionSerializer.WriteCollectionEndAsync();
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
                this.jsonOutputContext.WriterValidator.ValidateNullCollectionItem(expectedItemType);
                await this.jsonOutputContext.JsonWriter.WriteValueAsync((string)null)
                    .ConfigureAwait(false);
            }
            else
            {
                if (item is ODataResourceValue resourceValue)
                {
                    this.jsonCollectionSerializer.AssertRecursionDepthIsZero();
                    await this.jsonCollectionSerializer.WriteResourceValueAsync(
                        resourceValue,
                        expectedItemType,
                        false /*isOpenPropertyType*/,
                        this.DuplicatePropertyNameChecker).ConfigureAwait(false);
                    this.jsonCollectionSerializer.AssertRecursionDepthIsZero();
                    this.DuplicatePropertyNameChecker.Reset();
                }
                else if (item is ODataEnumValue enumVal)
                {
                    if (enumVal.Value == null)
                    {
                        await this.jsonCollectionSerializer.WriteNullValueAsync()
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        // write ODataEnumValue.Value as string value
                        await this.jsonCollectionSerializer.WritePrimitiveValueAsync(
                            enumVal.Value,
                            EdmCoreModel.Instance.GetString(true)).ConfigureAwait(false);
                    }
                }
                else
                {
                    Debug.Assert(!(item is ODataCollectionValue), "!(item is ODataCollectionValue)");
                    Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");
                    await this.jsonCollectionSerializer.WritePrimitiveValueAsync(item, expectedItemType)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
