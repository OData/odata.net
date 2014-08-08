//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.JsonLight
{
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;

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
        /// Version of OData being written.
        /// </summary>
        ODataVersion Version { get; }

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
