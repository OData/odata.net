//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Spatial;
    using System.Text;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Evaluation;
    using Microsoft.Data.OData.JsonLight;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.VerboseJson;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    using ODataPlatformHelper = Microsoft.Data.OData.PlatformHelper;
    #endregion

    /// <summary>
    /// Utility functions for writing values for use in a URL.
    /// </summary>
    internal static class ODataUriConversionUtils
    {
        /// <summary>
        /// Converts a primitive to a string for use in a Url.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="version">OData version to be compliant with.</param>
        /// <returns>A string representation of <paramref name="value"/> to be added to a Url.</returns>
        internal static string ConvertToUriPrimitiveLiteral(object value, ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(value, "value");

            /* This should have the same behavior of Astoria with these differences: (iawillia, 10/7/11)
             * TODO BUG 292670: Differences between Astoria and ODL's Uri literals
             * 1) Cannot handle the System.Data.Linq.Binary type
             * 2) Cannot handle the System.Data.Linq.XElement type
             * 3) Astoria does not put a 'd' or 'D' on double values
             */
            
            if (value is ISpatial)
            {
                ODataVersionChecker.CheckSpatialValue(version);
            }
            
            // for legacy backwards compatibility reasons, use the formatter which does not URL-encode the resulting string.
            return LiteralFormatter.ForConstantsWithoutEncoding.Format(value);
        }

        /// <summary>
        /// Converts the given string <paramref name="value"/> to an ODataComplexValue or ODataCollectionValue and returns it.
        /// Tries in both JSON light and Verbose JSON.
        /// </summary>
        /// <remarks>Does not handle primitive values.</remarks>
        /// <param name="value">Value to be deserialized.</param>
        /// <param name="version">ODataVersion to be compliant with.</param>
        /// <param name="model">Model to use for verification.</param>
        /// <param name="typeReference">Expected type reference from deserialization. If null, verification will be skipped.</param>
        /// <returns>An ODataComplexValue or ODataCollectionValue that results from the deserialization of <paramref name="value"/>.</returns>
        internal static object ConvertFromComplexOrCollectionValue(string value, ODataVersion version, IEdmModel model, IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();

            // Try read as JSON (light) so long as a custom model was provided
            if (model.IsUserModel())
            {
                try
                {
                    using (StringReader reader = new StringReader(value))
                    {
                        using (ODataJsonLightInputContext context = new ODataJsonLightInputContext(
                            ODataFormat.Json,
                            reader,
                            new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType),
                            settings,
                            version,
                            false /*readingResponse*/,
                            true /*synchronous*/,
                            model,
                            null /*urlResolver*/,
                            null /*payloadKindDetectionState*/))
                        {
                            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(context);

                            // TODO: The way JSON array literals look in the URI is different that response payload with an array in it.
                            // The fact that we have to manually setup the underlying reader shows this different in the protocol.
                            // There is a discussion on if we should change this or not.
                            deserializer.JsonReader.Read(); // Move to first thing
                            object rawResult = deserializer.ReadNonEntityValue(
                                null /*payloadTypeName*/,
                                typeReference,
                                null /*DuplicatePropertyNameChecker*/,
                                null /*CollectionWithoutExpectedTypeValidator*/,
                                true /*validateNullValue*/,
                                false /*isTopLevelPropertyValue*/,
                                false /*insideComplexValue*/,
                                null /*propertyName*/);
                            deserializer.ReadPayloadEnd(false);

                            Debug.Assert(rawResult is ODataComplexValue || rawResult is ODataCollectionValue, "rawResult is ODataComplexValue || rawResult is ODataCollectionValue");
                            return rawResult;
                        }
                    }
                }
                catch (ODataException)
                {
                    // Swallow. We'll try reading it as Verbose JSON instead. 
                    // If an error occurs then, we'll surface that error message. 
                    // Since we shipped Verbose JSON only support in 5.0, it's nice to preserve the Verbose JSON error messages.
                }
            }

            // Read as Verbose JSON
            using (StringReader reader = new StringReader(value))
            {
                using (ODataVerboseJsonInputContext context = new ODataVerboseJsonInputContext(
                    ODataFormat.VerboseJson,
                    reader,
                    settings,
                    version,
                    false /*readingResponse*/,
                    true /*synchronous*/,
                    model,
                    null /*urlResolver*/))
                {
                    ODataVerboseJsonPropertyAndValueDeserializer deserializer = new ODataVerboseJsonPropertyAndValueDeserializer(context);

                    deserializer.ReadPayloadStart(false);
                    object rawResult = deserializer.ReadNonEntityValue(
                        typeReference, 
                        null /*DuplicatePropertyNameChecker*/, 
                        null /*CollectionWithoutExpectedTypeValidator*/,
                        true /*validateNullValue*/,
                        null /*propertyName*/);
                    deserializer.ReadPayloadEnd(false);

                    Debug.Assert(rawResult is ODataComplexValue || rawResult is ODataCollectionValue, "rawResult is ODataComplexValue || rawResult is ODataCollectionValue");                
                    return rawResult;
                }
            }
        }

        /// <summary>
        /// Verifies that the given <paramref name="primitiveValue"/> is or can be coerced to <paramref name="expectedTypeReference"/>, and coerces it if necessary.
        /// </summary>
        /// <param name="primitiveValue">An EDM primitive value to verify.</param>
        /// <param name="model">Model to verify against.</param>
        /// <param name="expectedTypeReference">Expected type reference.</param>
        /// <param name="version">The version to use for reading.</param>
        /// <returns>Coerced version of the <paramref name="primitiveValue"/>.</returns>
        internal static object VerifyAndCoerceUriPrimitiveLiteral(object primitiveValue, IEdmModel model, IEdmTypeReference expectedTypeReference, ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(primitiveValue, "primitiveValue");
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(expectedTypeReference, "expectedTypeReference");

            // First deal with null literal
            ODataUriNullValue nullValue = primitiveValue as ODataUriNullValue;
            if (nullValue != null)
            {
                if (!expectedTypeReference.IsNullable)
                {
                    throw new ODataException(ODataErrorStrings.ODataUriUtils_ConvertFromUriLiteralNullOnNonNullableType(expectedTypeReference.ODataFullName()));
                }

                // If it was just 'null' on the wire (no typed null) then just take what was expected and use that
                if (nullValue.TypeName == null)
                {
                    nullValue.TypeName = expectedTypeReference.ODataFullName();
                    return nullValue;
                }

                IEdmType actualResolvedType = TypeNameOracle.ResolveAndValidateTypeName(model, nullValue.TypeName, expectedTypeReference.Definition.TypeKind);
                Debug.Assert(actualResolvedType != null, "This is a primitive-only codepath so actualResolvedType != null.");

                if (actualResolvedType.IsSpatial())
                {
                    ODataVersionChecker.CheckSpatialValue(version);
                }

                if (TypePromotionUtils.CanConvertTo(actualResolvedType.ToTypeReference(), expectedTypeReference))
                {
                    nullValue.TypeName = expectedTypeReference.ODataFullName();
                    return nullValue;
                }

                throw new ODataException(ODataErrorStrings.ODataUriUtils_ConvertFromUriLiteralNullTypeVerificationFailure(expectedTypeReference.ODataFullName(), nullValue.TypeName));
            }

            // Only other positive case is a numeric primitive that needs to be coerced
            IEdmPrimitiveTypeReference expectedPrimitiveTypeReference = expectedTypeReference.AsPrimitiveOrNull();
            if (expectedPrimitiveTypeReference == null)
            {
                throw new ODataException(ODataErrorStrings.ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure(expectedTypeReference.ODataFullName(), primitiveValue));
            }

            object coercedResult = CoerceNumericType(primitiveValue, expectedPrimitiveTypeReference.PrimitiveDefinition());
            if (coercedResult != null)
            {
                return coercedResult;
            }

            Type actualType = primitiveValue.GetType();
            Type targetType = TypeUtils.GetNonNullableType(EdmLibraryExtensions.GetPrimitiveClrType(expectedPrimitiveTypeReference));

            // If target type is assignable from actual type, we're OK
            if (targetType.IsAssignableFrom(actualType))
            {
                return primitiveValue;
            }

            throw new ODataException(ODataErrorStrings.ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure(expectedPrimitiveTypeReference.ODataFullName(), primitiveValue));
        }

        /// <summary>
        /// Converts a <see cref="ODataComplexValue"/> to a string for use in a Url.
        /// </summary>
        /// <param name="complexValue">Instance to convert.</param>
        /// <param name="model">Model to be used for validation. User model is optional. The EdmLib core model is expected as a minimum.</param>
        /// <param name="version">Version to be compliant with.</param>
        /// <param name="format">ODataFormat to use for the format of the literal.</param>
        /// <returns>A string representation of <paramref name="complexValue"/> to be added to a Url.</returns>
        internal static string ConvertToUriComplexLiteral(ODataComplexValue complexValue, IEdmModel model, ODataVersion version, ODataFormat format)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(complexValue, "complexValue");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            StringBuilder builder = new StringBuilder();
            using (TextWriter textWriter = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings()
                {
                    Version = version,
                    Indent = false
                };

                if (format == ODataFormat.VerboseJson)
                {
                    WriteJsonVerboseLiteral(
                        model,
                        messageWriterSettings,
                        textWriter,
                        (serializer) => serializer.WriteComplexValue(
                            complexValue,
                            null /*propertyTypeReference*/,
                            true /*isOpenPropertyType*/,
                            serializer.CreateDuplicatePropertyNamesChecker(),
                            null /*collectionValidator*/));
                }
                else if (format == ODataFormat.Json)
                {
                    WriteJsonLightLiteral(
                        model,
                        messageWriterSettings,
                        textWriter,
                        (serializer) => serializer.WriteComplexValue(
                            complexValue,
                            null /*metadataTypeReference*/,
                            false /*isTopLevel*/,
                            true /*isOpenPropetyType*/,
                            serializer.CreateDuplicatePropertyNamesChecker()));
                }
                else
                {
                    throw new ArgumentException(ODataErrorStrings.ODataUriUtils_ConvertToUriLiteralUnsupportedFormat(format.ToString()));
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts an <see cref="ODataUriNullValue"/> to a string for use in a Url.
        /// </summary>
        /// <param name="nullValue">Instance to convert.</param>
        /// <returns>A string representation of <paramref name="nullValue"/> to be added to a Url.</returns>
        internal static string ConvertToUriNullValue(ODataUriNullValue nullValue)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(nullValue, "nullValue");
            
            if (nullValue.TypeName != null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(ExpressionConstants.KeywordNull);
                builder.Append(ExpressionConstants.LiteralSingleQuote);
                builder.Append(nullValue.TypeName);
                builder.Append(ExpressionConstants.LiteralSingleQuote);
                return builder.ToString();
            }

            return ExpressionConstants.KeywordNull;
        }

        /// <summary>
        /// Converts a <see cref="ODataCollectionValue"/> to a string for use in a Url.
        /// </summary>
        /// <param name="collectionValue">Instance to convert.</param>
        /// <param name="model">Model to be used for validation. User model is optional. The EdmLib core model is expected as a minimum.</param>
        /// <param name="version">Version to be compliant with. Collection requires >= V3.</param>
        /// <param name="format">ODataFormat to use for the format of the literal.</param>
        /// <returns>A string representation of <paramref name="collectionValue"/> to be added to a Url.</returns>
        internal static string ConvertToUriCollectionLiteral(ODataCollectionValue collectionValue, IEdmModel model, ODataVersion version, ODataFormat format)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(collectionValue, "collectionValue");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            ODataVersionChecker.CheckCollectionValue(version);

            StringBuilder builder = new StringBuilder();
            using (TextWriter textWriter = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings()
                {
                    Version = version,
                    Indent = false
                };

                if (format == ODataFormat.VerboseJson)
                {
                    WriteJsonVerboseLiteral(
                        model,
                        messageWriterSettings,
                        textWriter,
                        (serializer) => serializer.WriteCollectionValue(
                            collectionValue,
                            null /*metadataTypeReference*/,
                            false /*isOpenPropertyType*/));
                }
                else if (format == ODataFormat.Json)
                {
                    WriteJsonLightLiteral(
                        model,
                        messageWriterSettings,
                        textWriter,
                        (serializer) => serializer.WriteCollectionValue(
                            collectionValue,
                            null /*metadataTypeReference*/,
                            false /*isTopLevelProperty*/,
                            true /*isInUri*/,
                            false /*isOpenPropertyType*/));
                }
                else
                {
                    throw new ArgumentException(ODataErrorStrings.ODataUriUtils_ConvertToUriLiteralUnsupportedFormat(format.ToString()));
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Write a literal value in JSON Verbose format.
        /// </summary>
        /// <param name="model">EDM Model to use for validation and type lookups.</param>
        /// <param name="messageWriterSettings">Settings to use when writing.</param>
        /// <param name="textWriter">TextWriter to use as the output for the value.</param>
        /// <param name="writeValue">Delegate to use to actually write the value.</param>
        private static void WriteJsonVerboseLiteral(IEdmModel model, ODataMessageWriterSettings messageWriterSettings, TextWriter textWriter, Action<ODataVerboseJsonPropertyAndValueSerializer> writeValue)
        {
            // Calling dispose since it's the right thing to do, but when created from a custom-built TextWriter
            // the output context Dispose will not actually dispose anything, it will just cleanup itself.
            using (ODataVerboseJsonOutputContext jsonOutputContext = new ODataVerboseJsonOutputContext(ODataFormat.VerboseJson, textWriter, messageWriterSettings, model))
            {
                ODataVerboseJsonPropertyAndValueSerializer jsonPropertyAndValueSerializer = new ODataVerboseJsonPropertyAndValueSerializer(jsonOutputContext);
                writeValue(jsonPropertyAndValueSerializer);
                jsonPropertyAndValueSerializer.AssertRecursionDepthIsZero();
            }
        }

        /// <summary>
        /// Write a literal value in JSON Light format.
        /// </summary>
        /// <param name="model">EDM Model to use for validation and type lookups.</param>
        /// <param name="messageWriterSettings">Settings to use when writing.</param>
        /// <param name="textWriter">TextWriter to use as the output for the value.</param>
        /// <param name="writeValue">Delegate to use to actually write the value.</param>
        private static void WriteJsonLightLiteral(IEdmModel model, ODataMessageWriterSettings messageWriterSettings, TextWriter textWriter, Action<ODataJsonLightValueSerializer> writeValue)
        {
            // Calling dispose since it's the right thing to do, but when created from a custom-built TextWriter
            // the output context Dispose will not actually dispose anything, it will just cleanup itself.
            using (ODataJsonLightOutputContext jsonOutputContext = new ODataJsonLightOutputContext(ODataFormat.Json, textWriter, messageWriterSettings, model))
            {
                ODataJsonLightValueSerializer jsonLightValueSerializer = new ODataJsonLightValueSerializer(jsonOutputContext);
                writeValue(jsonLightValueSerializer);
                jsonLightValueSerializer.AssertRecursionDepthIsZero();
            }
        }

        /// <summary>
        /// Coerces the given <paramref name="primitiveValue"/> to the appropriate CLR type based on <paramref name="targetEdmType"/>. 
        /// </summary>
        /// <param name="primitiveValue">Primitive value to coerce.</param>
        /// <param name="targetEdmType">Edm primitive type to check against.</param>
        /// <returns><paramref name="primitiveValue"/> as the corresponding CLR type indicated by <paramref name="targetEdmType"/>, or null if unable to coerce.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Centralized method for coercing numeric types in easiest to understand.")]
        private static object CoerceNumericType(object primitiveValue, IEdmPrimitiveType targetEdmType)
        {
            // This is implemented to match TypePromotionUtils and MetadataUtilsCommon.CanConvertPrimitiveTypeTo()
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(primitiveValue, "primitiveValue");
            ExceptionUtils.CheckArgumentNotNull(targetEdmType, "targetEdmType");

            Type fromType = primitiveValue.GetType();
            TypeCode fromTypeCode = ODataPlatformHelper.GetTypeCode(fromType);
            EdmPrimitiveTypeKind targetPrimitiveKind = targetEdmType.PrimitiveKind;

            switch (fromTypeCode)
            {
                case TypeCode.SByte:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.SByte:
                            return primitiveValue;
                        case EdmPrimitiveTypeKind.Int16:
                            return Convert.ToInt16((SByte)primitiveValue);
                        case EdmPrimitiveTypeKind.Int32:
                            return Convert.ToInt32((SByte)primitiveValue);
                        case EdmPrimitiveTypeKind.Int64:
                            return Convert.ToInt64((SByte)primitiveValue);
                        case EdmPrimitiveTypeKind.Single:
                            return Convert.ToSingle((SByte)primitiveValue);
                        case EdmPrimitiveTypeKind.Double:
                            return Convert.ToDouble((SByte)primitiveValue);
                        case EdmPrimitiveTypeKind.Decimal:
                            return Convert.ToDecimal((SByte)primitiveValue);
                    }

                    break;
                case TypeCode.Byte:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Byte:
                            return primitiveValue;
                        case EdmPrimitiveTypeKind.Int16:
                            return Convert.ToInt16((Byte)primitiveValue);
                        case EdmPrimitiveTypeKind.Int32:
                            return Convert.ToInt32((Byte)primitiveValue);
                        case EdmPrimitiveTypeKind.Int64:
                            return Convert.ToInt64((Byte)primitiveValue);
                        case EdmPrimitiveTypeKind.Single:
                            return Convert.ToSingle((Byte)primitiveValue);
                        case EdmPrimitiveTypeKind.Double:
                            return Convert.ToDouble((Byte)primitiveValue);
                        case EdmPrimitiveTypeKind.Decimal:
                            return Convert.ToDecimal((Byte)primitiveValue);
                    }

                    break;
                case TypeCode.Int16:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Int16:
                            return primitiveValue;
                        case EdmPrimitiveTypeKind.Int32:
                            return Convert.ToInt32((Int16)primitiveValue);
                        case EdmPrimitiveTypeKind.Int64:
                            return Convert.ToInt64((Int16)primitiveValue);
                        case EdmPrimitiveTypeKind.Single:
                            return Convert.ToSingle((Int16)primitiveValue);
                        case EdmPrimitiveTypeKind.Double:
                            return Convert.ToDouble((Int16)primitiveValue);
                        case EdmPrimitiveTypeKind.Decimal:
                            return Convert.ToDecimal((Int16)primitiveValue);
                    }

                    break;
                case TypeCode.Int32:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Int32:
                            return primitiveValue;
                        case EdmPrimitiveTypeKind.Int64:
                            return Convert.ToInt64((Int32)primitiveValue);
                        case EdmPrimitiveTypeKind.Single:
                            return Convert.ToSingle((Int32)primitiveValue);
                        case EdmPrimitiveTypeKind.Double:
                            return Convert.ToDouble((Int32)primitiveValue);
                        case EdmPrimitiveTypeKind.Decimal:
                            return Convert.ToDecimal((Int32)primitiveValue);
                    }

                    break;
                case TypeCode.Int64:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Int64:
                            return primitiveValue;
                        case EdmPrimitiveTypeKind.Single:
                            return Convert.ToSingle((Int64)primitiveValue);
                        case EdmPrimitiveTypeKind.Double:
                            return Convert.ToDouble((Int64)primitiveValue);
                        case EdmPrimitiveTypeKind.Decimal:
                            return Convert.ToDecimal((Int64)primitiveValue);
                    }

                    break;
                case TypeCode.Single:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Single:
                            return primitiveValue;
                        case EdmPrimitiveTypeKind.Double:
                            return Convert.ToDouble((Single)primitiveValue);
                    }

                    break;
                case TypeCode.Double:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Double:
                            return primitiveValue;
                    }

                    break;
                case TypeCode.Decimal:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Decimal:
                            return primitiveValue;
                    }

                    break;
                default:
                    break;
            }

            return null;
        }
    }
}
