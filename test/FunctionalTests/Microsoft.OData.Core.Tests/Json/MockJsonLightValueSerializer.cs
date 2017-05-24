//---------------------------------------------------------------------
// <copyright file="MockJsonLightValueSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Json;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Tests.Json
{
    internal class MockJsonLightValueSerializer : ODataJsonLightValueSerializer
    {
        public const string NullOutput = "FAKE_NULL_VALUE";
        public const string PrimitiveOutput = "FAKE_PRIMITIVE_VALUE";
        public const string ComplexOutput = "FAKE_COMPLEX_VALUE";
        public const string CollectionOutput = "FAKE_COLLECTION_VALUE_";

        public Action<ODataResource, IEdmTypeReference, bool, bool, IDuplicatePropertyNameChecker> WriteComplexVerifier { get; set; }
        public Action<ODataCollectionValue, IEdmTypeReference, IEdmTypeReference, bool, bool, bool> WriteCollectionVerifier { get; set; }
        public Action<object, IEdmTypeReference> WritePrimitiveVerifier { get; set; }
        public Action<object, IEdmTypeReference> WriteEnumVerifier { get; set; }
        public Action WriteNullVerifier { get; set; }

        public MockJsonLightValueSerializer(ODataJsonLightOutputContext outputContext, bool initContextUriBuilder = false)
                    : base(outputContext, initContextUriBuilder)
        {
        }

        public override void WriteNullValue()
        {
            this.WriteNullVerifier.Should().NotBeNull("WriteNullValue was called.");
            this.WriteNullVerifier();
        }

        /// <summary>
        /// Write enum value
        /// </summary>
        /// <param name="value">enum value</param>
        /// <param name="expectedTypeReference">expected type reference</param>
        public override void WriteEnumValue(ODataEnumValue value, IEdmTypeReference expectedTypeReference)
        {
            this.WriteEnumVerifier.Should().NotBeNull("WriteEnumValue was called.");
            this.WriteEnumVerifier(value, expectedTypeReference);
        }

        public override void WriteCollectionValue(ODataCollectionValue collectionValue, IEdmTypeReference metadataTypeReference, IEdmTypeReference valueTypeReference, bool isTopLevelProperty, bool isInUri, bool isOpenPropertyType)
        {
            this.WriteCollectionVerifier.Should().NotBeNull("WriteCollectionValue was called.");
            this.WriteCollectionVerifier(collectionValue, metadataTypeReference, valueTypeReference, isTopLevelProperty, isInUri, isOpenPropertyType);
        }

        public override void WritePrimitiveValue(object value, IEdmTypeReference expectedTypeReference)
        {
            this.WritePrimitiveVerifier.Should().NotBeNull("WritePrimitiveValue was called.");
            this.WritePrimitiveVerifier(value, expectedTypeReference);
        }

        public override void WritePrimitiveValue(object value, IEdmTypeReference actualTypeReference, IEdmTypeReference expectedTypeReference)
        {
            this.WritePrimitiveValue(value, expectedTypeReference);
        }

        public override void WriteUntypedValue(ODataUntypedValue value)
        {
            this.WritePrimitiveVerifier.Should().NotBeNull("WriteUntypedValue was called.");
            this.WritePrimitiveVerifier(value, null);
        }
    }
}