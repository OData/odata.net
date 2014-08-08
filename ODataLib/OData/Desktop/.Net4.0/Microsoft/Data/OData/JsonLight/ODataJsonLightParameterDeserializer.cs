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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight deserializer for parameter payloads.
    /// </summary>
    internal sealed class ODataJsonLightParameterDeserializer : ODataJsonLightPropertyAndValueDeserializer
    {
        /// <summary>OData property annotation reader for parameter payloads.</summary>
        /// <remarks>OData property annotations are not supported in parameter payloads.</remarks>
        private static readonly Func<string, object> propertyAnnotationValueReader =
            annotationName => { throw new ODataException(ODataErrorStrings.ODataJsonLightParameterDeserializer_PropertyAnnotationForParameters); };

        /// <summary>The JSON Light parameter reader.</summary>
        private readonly ODataJsonLightParameterReader parameterReader;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parameterReader">The JSON Light parameter reader.</param>
        /// <param name="jsonLightInputContext">The JsonLight input context to read from.</param>
        internal ODataJsonLightParameterDeserializer(ODataJsonLightParameterReader parameterReader, ODataJsonLightInputContext jsonLightInputContext)
            : base(jsonLightInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
            this.parameterReader = parameterReader;
        }

        /// <summary>
        /// Reads the next parameter from the parameters payload.
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker used to read a parameter payload.</param>
        /// <returns>true if a parameter was read from the payload; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  Property or EndObject   the property node of the parameter to read or the end object node if there are not parameters
        /// Post-Condition: Property or EndObject   the node after the property value of a primitive, complex or null collection parameter
        ///                 Any                     the start of the value representing a non-null collection parameter (the collection reader will fail if this is not a StartArray node)
        /// </remarks>
        internal bool ReadNextParameter(DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            bool parameterRead = false;
            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                bool foundCustomInstanceAnnotation = false;
                this.ProcessProperty(
                    duplicatePropertyNamesChecker,
                    propertyAnnotationValueReader,
                    (propertyParsingResult, parameterName) =>
                    {
                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                // OData instance annotations are not supported in parameter payloads.
                                throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(parameterName));

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                this.JsonReader.SkipValue();
                                foundCustomInstanceAnnotation = true;
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(ODataErrorStrings.ODataJsonLightParameterDeserializer_PropertyAnnotationWithoutPropertyForParameters(parameterName));

                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(parameterName));

                            case PropertyParsingResult.PropertyWithValue:
                                IEdmTypeReference parameterTypeReference = this.parameterReader.GetParameterTypeReference(parameterName);
                                Debug.Assert(parameterTypeReference != null, "parameterTypeReference != null");

                                ODataParameterReaderState state;
                                object parameterValue;
                                EdmTypeKind parameterTypeKind = parameterTypeReference.TypeKind();
                                switch (parameterTypeKind)
                                {
                                    case EdmTypeKind.Primitive:
                                        IEdmPrimitiveTypeReference primitiveTypeReference = parameterTypeReference.AsPrimitive();
                                        if (primitiveTypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.Stream)
                                        {
                                            throw new ODataException(ODataErrorStrings.ODataJsonLightParameterDeserializer_UnsupportedPrimitiveParameterType(parameterName, primitiveTypeReference.PrimitiveKind()));
                                        }

                                        parameterValue = this.ReadNonEntityValue(
                                            /*payloadTypeName*/ null,
                                            primitiveTypeReference,
                                            /*duplicatePropertyNamesChecker*/ null,
                                            /*collectionValidator*/ null,
                                            /*validateNullValue*/ true,
                                            /*isTopLevelPropertyValue*/ false,
                                            /*insideComplexValue*/ false,
                                            parameterName);
                                        state = ODataParameterReaderState.Value;
                                        break;

                                    case EdmTypeKind.Complex:
                                        parameterValue = this.ReadNonEntityValue(
                                            /*payloadTypeName*/ null,
                                            parameterTypeReference,
                                            /*duplicatePropertyNamesChecker*/ null,
                                            /*collectionValidator*/ null,
                                            /*validateNullValue*/ true,
                                            /*isTopLevelPropertyValue*/ false,
                                            /*insideComplexValue*/ false,
                                            parameterName);
                                        state = ODataParameterReaderState.Value;
                                        break;

#if SUPPORT_ENTITY_PARAMETER
                                    case EdmTypeKind.Entity:
                                        parameterValue = null;
                                        state = ODataParameterReaderState.Entry;
                                        break;
#endif

                                    case EdmTypeKind.Collection:
                                        parameterValue = null;
                                        if (this.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
                                        {
                                            // NOTE: we support null collections in parameter payloads but nowhere else.
                                            parameterValue = this.JsonReader.ReadPrimitiveValue();
                                            if (parameterValue != null)
                                            {
                                                throw new ODataException(ODataErrorStrings.ODataJsonLightParameterDeserializer_NullCollectionExpected(JsonNodeType.PrimitiveValue, parameterValue));
                                            }

                                            state = ODataParameterReaderState.Value;
                                        }
                                        else if (((IEdmCollectionType)parameterTypeReference.Definition).ElementType.TypeKind() == EdmTypeKind.Entity)
                                        {
#if SUPPORT_ENTITY_PARAMETER
                                        state = ODataParameterReaderState.Feed;
#else
                                            throw new ODataException(ODataErrorStrings.ODataJsonLightParameterDeserializer_UnsupportedParameterTypeKind(parameterName, "Entity Collection"));
#endif
                                        }
                                        else
                                        {
                                            state = ODataParameterReaderState.Collection;
                                        }

                                        break;
                                    default:
                                        throw new ODataException(ODataErrorStrings.ODataJsonLightParameterDeserializer_UnsupportedParameterTypeKind(parameterName, parameterTypeReference.TypeKind()));
                                }

                                parameterRead = true;
                                this.parameterReader.EnterScope(state, parameterName, parameterValue);
                                Debug.Assert(
                                    state == ODataParameterReaderState.Collection || this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                                    "Expected any node for a collection; 'Property' or 'EndObject' if it is a primitive or complex value.");
                                break;

                            default:
                                throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonLightParameterDeserializer_ReadNextParameter));
                        }
                    });

                if (foundCustomInstanceAnnotation)
                {
                    return this.ReadNextParameter(duplicatePropertyNamesChecker);
                }
            }

            if (!parameterRead && this.JsonReader.NodeType == JsonNodeType.EndObject)
            {
                this.JsonReader.ReadEndObject();
                this.ReadPayloadEnd(/*isReadingNestedPayload*/false);

                // Pop the scope for the previous parameter if there is any
                if (this.parameterReader.State != ODataParameterReaderState.Start)
                {
                    this.parameterReader.PopScope(this.parameterReader.State);
                }

                // Pop the 'Start' scope and enter the 'Completed' scope
                this.parameterReader.PopScope(ODataParameterReaderState.Start);
                this.parameterReader.EnterScope(ODataParameterReaderState.Completed, /*parameterName*/null, /*parameterValue*/null);
                this.AssertJsonCondition(JsonNodeType.EndOfInput);
            }

            return parameterRead;
        }
    }
}
