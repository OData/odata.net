//---------------------------------------------------------------------
// <copyright file="IODataJsonLightValueSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Json;

    /// <summary>
    /// Defines an interface for a class that can write OData values in Json Light.
    /// This is used internally for mocking.
    /// </summary>
    internal interface IODataJsonLightValueSerializer
    {
        /// <summary>
        /// JsonWriter this value serializer will use.
        /// </summary>
        IJsonWriter JsonWriter { get; }

        /// <summary>
        /// Model to use for type resolution and verification when writing.
        /// </summary>
        IEdmModel Model { get; }

        /// <summary>
        /// The message writer settings to use when writing the message payload.
        /// </summary>
        ODataMessageWriterSettings Settings { get; }

        /// <summary>
        /// Writes a null value.
        /// </summary>
        void WriteNullValue();

        /// <summary>
        /// Writes out the value of a complex property.
        /// </summary>
        /// <param name="complexValue">The complex value to write.</param>
        /// <param name="metadataTypeReference">The metadata type for the complex value.</param>
        /// <param name="isTopLevel">true when writing a top-level property; false for nested properties.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        void WriteComplexValue(
            ODataComplexValue complexValue,
            IEdmTypeReference metadataTypeReference,
            bool isTopLevel,
            bool isOpenPropertyType,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker);

        /// <summary>
        /// Write enum value
        /// </summary>
        /// <param name="value">enum value</param>
        /// <param name="expectedTypeReference">expected type reference</param>
        void WriteEnumValue(ODataEnumValue value, IEdmTypeReference expectedTypeReference);

        /// <summary>
        /// Writes out the value of a collection property.
        /// </summary>
        /// <param name="collectionValue">The collection value to write.</param>
        /// <param name="metadataTypeReference">The metadata type reference for the collection.</param>
        /// <param name="valueTypeReference">The value type reference for the collection.</param>
        /// <param name="isTopLevelProperty">Whether or not a top-level property is being written.</param>
        /// <param name="isInUri">Whether or not the value is being written for a URI.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        void WriteCollectionValue(
            ODataCollectionValue collectionValue,
            IEdmTypeReference metadataTypeReference,
            IEdmTypeReference valueTypeReference,
            bool isTopLevelProperty,
            bool isInUri,
            bool isOpenPropertyType);

        /// <summary>
        /// Writes a primitive value.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="expectedTypeReference">The expected type reference of the primitive value.</param>
        void WritePrimitiveValue(
            object value,
            IEdmTypeReference expectedTypeReference);

        /// <summary>
        /// Writes an untyped value.
        /// </summary>
        /// <param name="value">The untyped value to write.</param>
        void WriteUntypedValue(
            ODataUntypedValue value);

        /// <summary>
        /// Creates a <see cref="DuplicatePropertyNamesChecker"/> for checking duplication properties inside complex values.
        /// </summary>
        /// <returns>A new <see cref="DuplicatePropertyNamesChecker"/> instance.</returns>
        DuplicatePropertyNamesChecker CreateDuplicatePropertyNamesChecker();
    }
}