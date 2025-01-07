//---------------------------------------------------------------------
// <copyright file="JsonReaderUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using System.IO;
using System.Threading.Tasks;
using System;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    internal static class JsonReaderUtils
    {
        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload and verifies the read item.
        /// </summary>
        /// <param name="jsonReader">The <see cref="ODataJsonReader"/>.</param>
        /// <param name="verifyResourceSetAction">Delegate to verify the read <see cref="ODataResourceSet"/> item.</param>
        /// <param name="verifyResourceAction">Delegate to verify the read <see cref="ODataResource"/> item.</param>
        /// <param name="verifyNestedResourceInfoAction">Delegate to verify the read <see cref="ODataNestedResourceInfo"/> item.</param>
        /// <param name="verifyDeltaResourceSetAction">Delegate to verify the read <see cref="ODataDeltaResourceSet"/> item.</param>
        /// <param name="verifyDeletedResourceAction">Delegate to verify the read <see cref="ODataDeletedResource"/> item.</param>
        /// <param name="verifyDeltaLinkAction">Delegate to verify the read <see cref="ODataDeltaLink"/> item.</param>
        /// <param name="verifyEntityReferenceLinkAction">Delegate to verify the read <see cref="ODataEntityReferenceLink"/> item.</param>
        /// <param name="verifyNestedPropertyInfoAction">Delegate to verify the read <see cref="ODataPropertyInfo"/> item.</param>
        /// <param name="verifyBinaryStreamAction">Delegate to verify the read <see cref="Stream"/> item.</param>
        /// <param name="verifyTextStreamAction">Delegate to verify the read <see cref="TextReader"/> item.</param>
        /// <param name="verifyPrimitiveAction">Delegate to verify the read <see cref="ODataPrimitiveValue"/> item.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task DoReadAsync(
            ODataJsonReader jsonReader,
            Action<ODataResourceSet> verifyResourceSetAction = null,
            Action<ODataResource> verifyResourceAction = null,
            Action<ODataNestedResourceInfo> verifyNestedResourceInfoAction = null,
            Action<ODataDeltaResourceSet> verifyDeltaResourceSetAction = null,
            Action<ODataDeletedResource> verifyDeletedResourceAction = null,
            Action<ODataDeltaLinkBase> verifyDeltaLinkAction = null,
            Action<ODataEntityReferenceLink> verifyEntityReferenceLinkAction = null,
            Action<ODataPropertyInfo> verifyNestedPropertyInfoAction = null,
            Action<Stream> verifyBinaryStreamAction = null,
            Action<TextReader> verifyTextStreamAction = null,
            Action<ODataPrimitiveValue> verifyPrimitiveAction = null)
        {
            while (await jsonReader.ReadAsync())
            {
                switch (jsonReader.State)
                {
                    case ODataReaderState.ResourceSetStart:
                        if (verifyResourceSetAction != null)
                        {
                            verifyResourceSetAction(jsonReader.Item as ODataResourceSet);
                        }

                        break;
                    case ODataReaderState.ResourceSetEnd:
                        break;
                    case ODataReaderState.ResourceStart:
                        if (verifyResourceAction != null)
                        {
                            verifyResourceAction(jsonReader.Item as ODataResource);
                        }

                        break;
                    case ODataReaderState.ResourceEnd:
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        if (verifyNestedResourceInfoAction != null)
                        {
                            verifyNestedResourceInfoAction(jsonReader.Item as ODataNestedResourceInfo);
                        }

                        break;
                    case ODataReaderState.NestedResourceInfoEnd:
                        break;
                    case ODataReaderState.DeltaResourceSetStart:
                        if (verifyDeltaResourceSetAction != null)
                        {
                            verifyDeltaResourceSetAction(jsonReader.Item as ODataDeltaResourceSet);
                        }

                        break;
                    case ODataReaderState.DeltaResourceSetEnd:
                        break;
                    case ODataReaderState.DeletedResourceStart:
                        if (verifyDeletedResourceAction != null)
                        {
                            verifyDeletedResourceAction(jsonReader.Item as ODataDeletedResource);
                        }

                        break;
                    case ODataReaderState.DeletedResourceEnd:
                        break;
                    case ODataReaderState.Primitive:
                        if (verifyPrimitiveAction != null)
                        {
                            verifyPrimitiveAction(jsonReader.Item as ODataPrimitiveValue);
                        }

                        break;
                    case ODataReaderState.DeltaLink:
                        if (verifyDeltaLinkAction != null)
                        {
                            verifyDeltaLinkAction(jsonReader.Item as ODataDeltaLinkBase);
                        }

                        break;
                    case ODataReaderState.DeltaDeletedLink:
                        if (verifyDeltaLinkAction != null)
                        {
                            verifyDeltaLinkAction(jsonReader.Item as ODataDeltaLinkBase);
                        }

                        break;
                    case ODataReaderState.EntityReferenceLink:
                        if (verifyEntityReferenceLinkAction != null)
                        {
                            verifyEntityReferenceLinkAction(jsonReader.Item as ODataEntityReferenceLink);
                        }

                        break;
                    case ODataReaderState.Stream:
                        var streamItem = jsonReader.Item as ODataStreamItem;
                        Assert.NotNull(streamItem);

                        if (streamItem.PrimitiveTypeKind == EdmPrimitiveTypeKind.String)
                        {
                            using (var textReader = await jsonReader.CreateTextReaderAsync())
                            {
                                if (verifyTextStreamAction != null)
                                {
                                    verifyTextStreamAction(textReader);
                                }
                            }
                        }
                        else
                        {
                            using (var stream = await jsonReader.CreateReadStreamAsync())
                            {
                                if (verifyBinaryStreamAction != null)
                                {
                                    verifyBinaryStreamAction(stream);
                                }
                            }
                        }

                        break;
                    case ODataReaderState.NestedProperty:
                        var nestedPropertyInfo = jsonReader.Item as ODataPropertyInfo;
                        Assert.NotNull(nestedPropertyInfo);

                        if (verifyNestedPropertyInfoAction != null)
                        {
                            verifyNestedPropertyInfoAction(nestedPropertyInfo);
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the next <see cref="ODataItem"/> from the message payload and verifies the read item.
        /// </summary>
        /// <param name="jsonReader">The <see cref="ODataJsonReader"/>.</param>
        /// <param name="verifyResourceSetAction">Delegate to verify the read <see cref="ODataResourceSet"/> item.</param>
        /// <param name="verifyResourceAction">Delegate to verify the read <see cref="ODataResource"/> item.</param>
        /// <param name="verifyNestedResourceInfoAction">Delegate to verify the read <see cref="ODataNestedResourceInfo"/> item.</param>
        /// <param name="verifyDeltaResourceSetAction">Delegate to verify the read <see cref="ODataDeltaResourceSet"/> item.</param>
        /// <param name="verifyDeletedResourceAction">Delegate to verify the read <see cref="ODataDeletedResource"/> item.</param>
        /// <param name="verifyDeltaLinkAction">Delegate to verify the read <see cref="ODataDeltaLink"/> item.</param>
        /// <param name="verifyEntityReferenceLinkAction">Delegate to verify the read <see cref="ODataEntityReferenceLink"/> item.</param>
        /// <param name="verifyNestedPropertyInfoAction">Delegate to verify the read <see cref="ODataPropertyInfo"/> item.</param>
        /// <param name="verifyBinaryStreamAction">Delegate to verify the read <see cref="Stream"/> item.</param>
        /// <param name="verifyTextStreamAction">Delegate to verify the read <see cref="TextReader"/> item.</param>
        /// <param name="verifyPrimitiveAction">Delegate to verify the read <see cref="ODataPrimitiveValue"/> item.</param>
        public static void DoRead(
            ODataJsonReader jsonReader,
            Action<ODataResourceSet> verifyResourceSetAction = null,
            Action<ODataResource> verifyResourceAction = null,
            Action<ODataNestedResourceInfo> verifyNestedResourceInfoAction = null,
            Action<ODataDeltaResourceSet> verifyDeltaResourceSetAction = null,
            Action<ODataDeletedResource> verifyDeletedResourceAction = null,
            Action<ODataDeltaLinkBase> verifyDeltaLinkAction = null,
            Action<ODataEntityReferenceLink> verifyEntityReferenceLinkAction = null,
            Action<ODataPropertyInfo> verifyNestedPropertyInfoAction = null,
            Action<Stream> verifyBinaryStreamAction = null,
            Action<TextReader> verifyTextStreamAction = null,
            Action<ODataPrimitiveValue> verifyPrimitiveAction = null)
        {
            while (jsonReader.Read())
            {
                switch (jsonReader.State)
                {
                    case ODataReaderState.ResourceSetStart:
                        if (verifyResourceSetAction != null)
                        {
                            verifyResourceSetAction(jsonReader.Item as ODataResourceSet);
                        }

                        break;
                    case ODataReaderState.ResourceSetEnd:
                        break;
                    case ODataReaderState.ResourceStart:
                        if (verifyResourceAction != null)
                        {
                            verifyResourceAction(jsonReader.Item as ODataResource);
                        }

                        break;
                    case ODataReaderState.ResourceEnd:
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        if (verifyNestedResourceInfoAction != null)
                        {
                            verifyNestedResourceInfoAction(jsonReader.Item as ODataNestedResourceInfo);
                        }

                        break;
                    case ODataReaderState.NestedResourceInfoEnd:
                        break;
                    case ODataReaderState.DeltaResourceSetStart:
                        if (verifyDeltaResourceSetAction != null)
                        {
                            verifyDeltaResourceSetAction(jsonReader.Item as ODataDeltaResourceSet);
                        }

                        break;
                    case ODataReaderState.DeltaResourceSetEnd:
                        break;
                    case ODataReaderState.DeletedResourceStart:
                        if (verifyDeletedResourceAction != null)
                        {
                            verifyDeletedResourceAction(jsonReader.Item as ODataDeletedResource);
                        }

                        break;
                    case ODataReaderState.DeletedResourceEnd:
                        break;
                    case ODataReaderState.Primitive:
                        if (verifyPrimitiveAction != null)
                        {
                            verifyPrimitiveAction(jsonReader.Item as ODataPrimitiveValue);
                        }

                        break;
                    case ODataReaderState.DeltaLink:
                        if (verifyDeltaLinkAction != null)
                        {
                            verifyDeltaLinkAction(jsonReader.Item as ODataDeltaLinkBase);
                        }

                        break;
                    case ODataReaderState.DeltaDeletedLink:
                        if (verifyDeltaLinkAction != null)
                        {
                            verifyDeltaLinkAction(jsonReader.Item as ODataDeltaLinkBase);
                        }

                        break;
                    case ODataReaderState.EntityReferenceLink:
                        if (verifyEntityReferenceLinkAction != null)
                        {
                            verifyEntityReferenceLinkAction(jsonReader.Item as ODataEntityReferenceLink);
                        }

                        break;
                    case ODataReaderState.Stream:
                        var streamItem = jsonReader.Item as ODataStreamItem;
                        Assert.NotNull(streamItem);

                        if (streamItem.PrimitiveTypeKind == EdmPrimitiveTypeKind.String)
                        {
                            using (var textReader = jsonReader.CreateTextReader())
                            {
                                if (verifyTextStreamAction != null)
                                {
                                    verifyTextStreamAction(textReader);
                                }
                            }
                        }
                        else
                        {
                            using (var stream = jsonReader.CreateReadStream())
                            {
                                if (verifyBinaryStreamAction != null)
                                {
                                    verifyBinaryStreamAction(stream);
                                }
                            }
                        }

                        break;
                    case ODataReaderState.NestedProperty:
                        var nestedPropertyInfo = jsonReader.Item as ODataPropertyInfo;
                        Assert.NotNull(nestedPropertyInfo);

                        if (verifyNestedPropertyInfoAction != null)
                        {
                            verifyNestedPropertyInfoAction(nestedPropertyInfo);
                        }

                        break;
                    default:
                        break;
                }
            }
        }
    }
}
