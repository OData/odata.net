//   OData .NET Libraries ver. 6.8.1
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
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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

                                    case EdmTypeKind.Enum:
                                        IEdmEnumTypeReference enumTypeReference = parameterTypeReference.AsEnum();
                                        parameterValue = this.ReadNonEntityValue(
                                            /*payloadTypeName*/ null,
                                            enumTypeReference,
                                            /*duplicatePropertyNamesChecker*/ null,
                                            /*collectionValidator*/ null,
                                            /*validateNullValue*/ true,
                                            /*isTopLevelPropertyValue*/ false,
                                            /*insideComplexValue*/ false,
                                            parameterName);
                                        state = ODataParameterReaderState.Value;
                                        break;

                                    case EdmTypeKind.TypeDefinition:
                                        IEdmTypeDefinitionReference typeDefinitionReference = parameterTypeReference.AsTypeDefinition();
                                        parameterValue = this.ReadNonEntityValue(
                                            /*payloadTypeName*/ null,
                                            typeDefinitionReference,
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

                                    case EdmTypeKind.Entity:
                                        parameterValue = null;
                                        state = ODataParameterReaderState.Entry;
                                        break;

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
                                            state = ODataParameterReaderState.Feed;
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
                                    state == ODataParameterReaderState.Collection || state == ODataParameterReaderState.Entry || state == ODataParameterReaderState.Feed || this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
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
