//---------------------------------------------------------------------
// <copyright file="MockJsonLightValueSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.Json;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.Tests.Json
{
    internal class MockJsonLightValueSerializer : IODataJsonLightValueSerializer
    {
        public const string NullOutput = "FAKE_NULL_VALUE";
        public const string PrimitiveOutput = "FAKE_PRIMITIVE_VALUE";
        public const string ComplexOutput = "FAKE_COMPLEX_VALUE";
        public const string CollectionOutput = "FAKE_COLLECTION_VALUE_";

        public Action<ODataComplexValue, IEdmTypeReference, bool, bool, DuplicatePropertyNamesChecker> WriteComplexVerifier { get; set; }
        public Action<ODataCollectionValue, IEdmTypeReference, IEdmTypeReference, bool, bool, bool> WriteCollectionVerifier { get; set; }
        public Action<object, IEdmTypeReference> WritePrimitiveVerifier { get; set; }
        public Action WriteNullVerifier { get; set; }

        public MockJsonLightValueSerializer(IJsonWriter jsonWriter, IEdmModel model)
        {
            this.JsonWriter = jsonWriter;
            this.Model = model;
        }

        public void WriteNullValue()
        {
            this.WriteNullVerifier.Should().NotBeNull("WriteNullValue was called.");
            this.WriteNullVerifier();
        }

        public IEdmModel Model { get; private set; }

        public void WriteComplexValue(ODataComplexValue complexValue, IEdmTypeReference metadataTypeReference, bool isTopLevel, bool isOpenPropertyType, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            this.WriteComplexVerifier.Should().NotBeNull("WriteComplexValue was called.");
            this.WriteComplexVerifier(complexValue, metadataTypeReference, isTopLevel, isOpenPropertyType, duplicatePropertyNamesChecker);
        }

        /// <summary>
        /// Write enum value
        /// </summary>
        /// <param name="value">enum value</param>
        /// <param name="expectedTypeReference">expected type reference</param>
        public void WriteEnumValue(ODataEnumValue value, IEdmTypeReference expectedTypeReference)
        {
            throw new NotImplementedException();
        }

        public void WriteCollectionValue(ODataCollectionValue collectionValue, IEdmTypeReference metadataTypeReference, IEdmTypeReference valueTypeReference, bool isTopLevelProperty, bool isInUri, bool isOpenPropertyType)
        {
            this.WriteCollectionVerifier.Should().NotBeNull("WriteCollectionValue was called.");
            this.WriteCollectionVerifier(collectionValue, metadataTypeReference, valueTypeReference, isTopLevelProperty, isInUri, isOpenPropertyType);
        }

        public void WritePrimitiveValue(object value, IEdmTypeReference expectedTypeReference)
        {
            this.WritePrimitiveVerifier.Should().NotBeNull("WritePrimitiveValue was called.");
            this.WritePrimitiveVerifier(value, expectedTypeReference);
        }

        public void WriteUntypedValue(ODataUntypedValue value)
        {
            this.WritePrimitiveVerifier.Should().NotBeNull("WriteUntypedValue was called.");
            this.WritePrimitiveVerifier(value, null);
        }

        public DuplicatePropertyNamesChecker CreateDuplicatePropertyNamesChecker()
        {
            return new DuplicatePropertyNamesChecker(true /*allowDuplicateProperties*/, true /*isRepsonse*/);
        }

        public IJsonWriter JsonWriter { get; set; }

        public ODataMessageWriterSettings Settings { get; set; }
    }
}