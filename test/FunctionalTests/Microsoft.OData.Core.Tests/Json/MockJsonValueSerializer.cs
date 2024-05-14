//---------------------------------------------------------------------
// <copyright file="MockJsonValueSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    internal class MockJsonValueSerializer : ODataJsonValueSerializer
    {
        public const string NullOutput = "FAKE_NULL_VALUE";
        public const string PrimitiveOutput = "FAKE_PRIMITIVE_VALUE";
        public const string ComplexOutput = "FAKE_COMPLEX_VALUE";
        public const string CollectionOutput = "FAKE_COLLECTION_VALUE_";

        public Action<ODataResourceValue, IEdmTypeReference, bool, IDuplicatePropertyNameChecker> WriteResourceValueVerifier { get; set; }
        public Action<ODataCollectionValue, IEdmTypeReference, IEdmTypeReference, bool, bool, bool> WriteCollectionVerifier { get; set; }
        public Action<object, IEdmTypeReference> WritePrimitiveVerifier { get; set; }
        public Action<object, IEdmTypeReference> WriteEnumVerifier { get; set; }
        public Action WriteNullVerifier { get; set; }

        public MockJsonValueSerializer(ODataJsonOutputContext outputContext, bool initContextUriBuilder = false)
                    : base(outputContext, initContextUriBuilder)
        {
        }

        public override void WriteNullValue()
        {
            Assert.NotNull(this.WriteNullVerifier);
            this.WriteNullVerifier();
        }

        public override void WriteResourceValue(ODataResourceValue resourceValue, IEdmTypeReference metadataTypeReference, bool isOpenPropertyType, IDuplicatePropertyNameChecker duplicatePropertyNamesChecker)
        {
            Assert.NotNull(this.WriteResourceValueVerifier);
            this.WriteResourceValueVerifier(resourceValue, metadataTypeReference, isOpenPropertyType, duplicatePropertyNamesChecker);
        }

        /// <summary>
        /// Write enum value
        /// </summary>
        /// <param name="value">enum value</param>
        /// <param name="expectedTypeReference">expected type reference</param>
        public override void WriteEnumValue(ODataEnumValue value, IEdmTypeReference expectedTypeReference)
        {
            Assert.NotNull(this.WriteEnumVerifier);
            this.WriteEnumVerifier(value, expectedTypeReference);
        }

        public override void WriteCollectionValue(ODataCollectionValue collectionValue, IEdmTypeReference metadataTypeReference, IEdmTypeReference valueTypeReference, bool isTopLevelProperty, bool isInUri, bool isOpenPropertyType)
        {
            Assert.NotNull(this.WriteCollectionVerifier);
            this.WriteCollectionVerifier(collectionValue, metadataTypeReference, valueTypeReference, isTopLevelProperty, isInUri, isOpenPropertyType);
        }

        public override void WritePrimitiveValue(object value, IEdmTypeReference expectedTypeReference)
        {
            Assert.NotNull(this.WritePrimitiveVerifier);
            this.WritePrimitiveVerifier(value, expectedTypeReference);
        }

        public override void WritePrimitiveValue(object value, IEdmTypeReference actualTypeReference, IEdmTypeReference expectedTypeReference)
        {
            this.WritePrimitiveValue(value, expectedTypeReference);
        }

        public override void WriteUntypedValue(ODataUntypedValue value)
        {
            Assert.NotNull(this.WritePrimitiveVerifier);
            this.WritePrimitiveVerifier(value, null);
        }
    }
}