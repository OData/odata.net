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
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight deserializer for service documents.
    /// </summary>
    internal sealed class ODataJsonLightServiceDocumentDeserializer : ODataJsonLightDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The JsonLight input context to read from.</param>
        internal ODataJsonLightServiceDocumentDeserializer(ODataJsonLightInputContext jsonLightInputContext)
            : base(jsonLightInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>An <see cref="ODataWorkspace"/> representing the read service document.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:        assumes that the JSON reader has not been used yet.
        /// Post-Condition: JsonNodeType.EndOfInput  
        /// </remarks>
        internal ODataWorkspace ReadServiceDocument()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            // Position the reader on the first node
            this.ReadPayloadStart(
                ODataPayloadKind.ServiceDocument,
                duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false);

            ODataWorkspace resultWorkspace = this.ReadServiceDocumentImplementation(duplicatePropertyNamesChecker);

            // Read the end of the response.
            this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return resultWorkspace;
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>A task which returns an <see cref="ODataWorkspace"/> representing the read service document.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:        assumes that the JSON reader has not been used yet.
        /// Post-Condition: JsonNodeType.EndOfInput  
        /// </remarks>
        internal Task<ODataWorkspace> ReadServiceDocumentAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            // Position the reader on the first node
            return this.ReadPayloadStartAsync(
                ODataPayloadKind.ServiceDocument,
                duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false)

                .FollowOnSuccessWith(t =>
                    {
                        ODataWorkspace resultWorkspace = this.ReadServiceDocumentImplementation(duplicatePropertyNamesChecker);

                        // Read the end of the response.
                        this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

                        Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
                        this.JsonReader.AssertNotBuffering();

                        return resultWorkspace;
                    });
        }
#endif

        /// <summary>
        /// Read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>An <see cref="ODataWorkspace"/> representing the read service document.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property   The property right after the metadata URI property.
        ///                 JsonNodeType.EndObject  The EndObject of the service document.
        /// Post-Condition: Any                     The node after the EndObject of the service document.
        /// </remarks>
        private ODataWorkspace ReadServiceDocumentImplementation(DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            List<ODataResourceCollectionInfo>[] collections = { null };

            // Read all the properties in the service document object; we ignore all except 'value'.
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // Property annotations are not allowed on the 'value' property, so fail if we see one.
                Func<string, object> readPropertyAnnotationInServiceDoc = annotationName => { throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocument(annotationName, JsonLightConstants.ODataValuePropertyName)); };

                this.ProcessProperty(
                    duplicatePropertyNamesChecker,
                    readPropertyAnnotationInServiceDoc,
                    (propertyParsingResult, propertyName) =>
                    {
                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocument(propertyName, JsonLightConstants.ODataValuePropertyName));

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                this.JsonReader.SkipValue();
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty(propertyName));

                            case PropertyParsingResult.PropertyWithValue:
                                if (string.CompareOrdinal(JsonLightConstants.ODataValuePropertyName, propertyName) == 0)
                                {
                                    // Fail if we've already processed a 'value' property.
                                    if (collections[0] != null)
                                    {
                                        throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument(JsonLightConstants.ODataValuePropertyName));
                                    }

                                    collections[0] = new List<ODataResourceCollectionInfo>();

                                    // Read the value of the 'value' property.
                                    this.JsonReader.ReadStartArray();
                                    DuplicatePropertyNamesChecker resourceCollectionDuplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

                                    while (this.JsonReader.NodeType != JsonNodeType.EndArray)
                                    {
                                        ODataResourceCollectionInfo collection = this.ReadResourceCollection(resourceCollectionDuplicatePropertyNamesChecker);
                                        collections[0].Add(collection);
                                        resourceCollectionDuplicatePropertyNamesChecker.Clear();
                                    }

                                    this.JsonReader.ReadEndArray();
                                }
                                else
                                {
                                    throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument(propertyName, JsonLightConstants.ODataValuePropertyName));
                                }

                                break;
                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));
                        }
                    });
            }

            if (collections[0] == null)
            {
                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_MissingValuePropertyInServiceDocument(JsonLightConstants.ODataValuePropertyName));
            }

            // Read over the end object (nothing else can happen after all properties have been read)
            this.JsonReader.ReadEndObject();

            return new ODataWorkspace
            {
                Collections = new ReadOnlyEnumerable<ODataResourceCollectionInfo>(collections[0])
            };
        }

        /// <summary>
        /// Reads a resource collection within a service document.
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The <see cref="DuplicatePropertyNamesChecker"/> to use for parsing annotations within the resource collection object.</param>
        /// <returns>A <see cref="ODataResourceCollectionInfo"/> representing the read resource collection.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject:     The beginning of the JSON object representing the resource collection.
        ///                 other:                        Will throw with an appropriate message on any other node type encountered.
        /// Post-Condition: JsonNodeType.StartObject:     The beginning of the next resource collection in the array.
        ///                 JsonNodeType.EndArray:        The end of the array.
        ///                 other:                        Any other node type occuring after the end object of the current resource collection. (Would be invalid).
        /// </remarks>
        private ODataResourceCollectionInfo ReadResourceCollection(DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            this.JsonReader.ReadStartObject();
            string[] entitySetName = { null };
            string[] entitySetUrl = { null };

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // OData property annotations are not supported in resource collection objects.
                Func<string, object> propertyAnnotationValueReader = annotationName => { throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInResourceCollection(annotationName)); };

                this.ProcessProperty(
                    duplicatePropertyNamesChecker,
                    propertyAnnotationValueReader,
                    (propertyParsingResult, propertyName) =>
                    {
                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInResourceCollection(propertyName));

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                this.JsonReader.SkipValue();
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty(propertyName));

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));

                            case PropertyParsingResult.PropertyWithValue:
                                if (string.CompareOrdinal(JsonLightConstants.ODataWorkspaceCollectionNameName, propertyName) == 0)
                                {
                                    if (entitySetName[0] != null)
                                    {
                                        throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInResourceCollection(JsonLightConstants.ODataWorkspaceCollectionNameName));
                                    }

                                    entitySetName[0] = this.JsonReader.ReadStringValue();
                                }
                                else if (string.CompareOrdinal(JsonLightConstants.ODataWorkspaceCollectionUrlName, propertyName) == 0)
                                {
                                    if (entitySetUrl[0] != null)
                                    {
                                        throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInResourceCollection(JsonLightConstants.ODataWorkspaceCollectionUrlName));
                                    }

                                    entitySetUrl[0] = this.JsonReader.ReadStringValue();
                                    ValidationUtils.ValidateResourceCollectionInfoUrl(entitySetUrl[0]);
                                }
                                else
                                {
                                    throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInResourceCollection(propertyName, JsonLightConstants.ODataWorkspaceCollectionNameName, JsonLightConstants.ODataWorkspaceCollectionUrlName));
                                }

                                break;
                        }
                    });
            }

            // URL and Name are mandatory
            if (string.IsNullOrEmpty(entitySetName[0]))
            {
                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInResourceCollection(JsonLightConstants.ODataWorkspaceCollectionNameName));
            }

            if (string.IsNullOrEmpty(entitySetUrl[0]))
            {
                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInResourceCollection(JsonLightConstants.ODataWorkspaceCollectionUrlName));
            }

            ODataResourceCollectionInfo collection = new ODataResourceCollectionInfo 
            {
                Url = this.ProcessUriFromPayload(entitySetUrl[0]), 
                Name = entitySetName[0],
            };

            this.JsonReader.ReadEndObject();

            return collection;
        }
    }
}
