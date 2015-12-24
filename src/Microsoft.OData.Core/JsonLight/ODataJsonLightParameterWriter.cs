//---------------------------------------------------------------------
// <copyright file="ODataJsonLightParameterWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;

    #endregion Namespaces

    /// <summary>
    /// ODataParameterWriter for the JsonLight format.
    /// </summary>
    internal sealed class ODataJsonLightParameterWriter : ODataParameterWriterCore
    {
        /// <summary>
        /// The output context to write to.
        /// </summary>
        private readonly ODataJsonLightOutputContext jsonLightOutputContext;

        /// <summary>
        /// The JsonLight property and value serializer to use.
        /// </summary>
        private readonly ODataJsonLightValueSerializer jsonLightValueSerializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="operation">The operation import whose parameters will be written.</param>
        internal ODataJsonLightParameterWriter(ODataJsonLightOutputContext jsonLightOutputContext, IEdmOperation operation)
            : base(jsonLightOutputContext, operation)
        {
            Debug.Assert(jsonLightOutputContext != null, "jsonLightOutputContext != null");

            this.jsonLightOutputContext = jsonLightOutputContext;
            this.jsonLightValueSerializer = new ODataJsonLightValueSerializer(this.jsonLightOutputContext);
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

#if ODATALIB_ASYNC
        /// <summary>
        /// Flush the output.
        /// </summary>
        /// <returns>Task representing the pending flush operation.</returns>
        protected override Task FlushAsynchronously()
        {
            return this.jsonLightOutputContext.FlushAsync();
        }
#endif

        /// <summary>
        /// Start writing an OData payload.
        /// </summary>
        protected override void StartPayload()
        {
            // NOTE: we are always writing a request payload here.
            this.jsonLightValueSerializer.WritePayloadStart();
            this.jsonLightOutputContext.JsonWriter.StartObjectScope();
        }

        /// <summary>
        /// Finish writing an OData payload.
        /// </summary>
        protected override void EndPayload()
        {
            // NOTE: we are always writing a request payload here.
            this.jsonLightOutputContext.JsonWriter.EndObjectScope();
            this.jsonLightValueSerializer.WritePayloadEnd();
        }

        /// <summary>
        /// Writes a value parameter (either primitive or complex)
        /// </summary>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <param name="parameterValue">The value of the parameter to write.</param>
        /// <param name="expectedTypeReference">The expected type reference of the parameter value.</param>
        protected override void WriteValueParameter(string parameterName, object parameterValue, IEdmTypeReference expectedTypeReference)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");
            this.jsonLightOutputContext.JsonWriter.WriteName(parameterName);
            if (parameterValue == null)
            {
                this.jsonLightOutputContext.JsonWriter.WriteValue((string)null);
            }
            else
            {
                ODataComplexValue complexValue = parameterValue as ODataComplexValue;
                ODataEnumValue enumVal = null;
                if (complexValue != null)
                {
                    this.jsonLightValueSerializer.AssertRecursionDepthIsZero();
                    this.jsonLightValueSerializer.WriteComplexValue(
                        complexValue,
                        expectedTypeReference,
                        false /*isTopLevel*/,
                        false /*isOpenPropertyType*/,
                        this.DuplicatePropertyNamesChecker);
                    this.jsonLightValueSerializer.AssertRecursionDepthIsZero();
                    this.DuplicatePropertyNamesChecker.Clear();
                }
                else if ((enumVal = parameterValue as ODataEnumValue) != null)
                {
                    this.jsonLightValueSerializer.WriteEnumValue(enumVal, expectedTypeReference);
                }
                else
                {
                    Debug.Assert(!(parameterValue is ODataCollectionValue), "!(parameterValue is ODataCollectionValue)");
                    Debug.Assert(!(parameterValue is ODataStreamReferenceValue), "!(parameterValue is ODataStreamReferenceValue)");
                    Debug.Assert(!(parameterValue is Stream), "!(parameterValue is Stream)");
                    this.jsonLightValueSerializer.WritePrimitiveValue(parameterValue, expectedTypeReference);
                }
            }
        }

        /// <summary>
        /// Creates a format specific <see cref="ODataCollectionWriter"/> to write the value of a collection parameter.
        /// </summary>
        /// <param name="parameterName">The name of the collection parameter to write.</param>
        /// <param name="expectedItemType">The type reference of the expected item type or null if no expected item type exists.</param>
        /// <returns>The newly created <see cref="ODataCollectionWriter"/>.</returns>
        protected override ODataCollectionWriter CreateFormatCollectionWriter(string parameterName, IEdmTypeReference expectedItemType)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");
            this.jsonLightOutputContext.JsonWriter.WriteName(parameterName);
            return new ODataJsonLightCollectionWriter(this.jsonLightOutputContext, expectedItemType, /*listener*/this);
        }

        /// <summary>Creates a format specific <see cref="ODataWriter"/> to write an entry.</summary>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <param name="expectedItemType">The type reference of the expected item type or null if no expected item type exists.</param>
        /// <returns>The newly created <see cref="ODataWriter"/>.</returns>
        protected override ODataWriter CreateFormatEntryWriter(string parameterName, IEdmTypeReference expectedItemType)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");
            this.jsonLightOutputContext.JsonWriter.WriteName(parameterName);
            return new ODataJsonLightWriter(this.jsonLightOutputContext, null, null, /*writingFeed*/false, /*writingParameter*/true, /*writingDelta*/false, /*listener*/this);
        }

        /// <summary>Creates a format specific <see cref="ODataWriter"/> to write a feed.</summary>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <param name="expectedItemType">The type reference of the expected item type or null if no expected item type exists.</param>
        /// <returns>The newly created <see cref="ODataWriter"/>.</returns>
        protected override ODataWriter CreateFormatFeedWriter(string parameterName, IEdmTypeReference expectedItemType)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");
            this.jsonLightOutputContext.JsonWriter.WriteName(parameterName);
            return new ODataJsonLightWriter(this.jsonLightOutputContext, null, null, /*writingFeed*/true, /*writingParameter*/true, /*writingDelta*/false, /*listener*/this);
        }
    }
}
