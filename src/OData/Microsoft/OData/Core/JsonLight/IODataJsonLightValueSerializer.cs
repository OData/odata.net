//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
        /// <param name="isTopLevelProperty">Whether or not a top-level property is being written.</param>
        /// <param name="isInUri">Whether or not the value is being written for a URI.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        void WriteCollectionValue(
            ODataCollectionValue collectionValue,
            IEdmTypeReference metadataTypeReference,
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
        /// Creates a <see cref="DuplicatePropertyNamesChecker"/> for checking duplication properties inside complex values.
        /// </summary>
        /// <returns>A new <see cref="DuplicatePropertyNamesChecker"/> instance.</returns>
        DuplicatePropertyNamesChecker CreateDuplicatePropertyNamesChecker();
    }
}
