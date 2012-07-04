//   Copyright 2011 Microsoft Corporation
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
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.Metadata;
    using o = Microsoft.Data.OData;
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
             * TODO BUG 292670: Differences betwen Astoria and ODL's Uri literals
             * 1) Cannot handle the System.Data.Linq.Binary type
             * 2) Cannot handle the System.Data.Linq.XElement type
             * 3) Astoria does not put a 'd' or 'D' on double values
             */

            StringBuilder builder = new StringBuilder();

            Type valueType = value.GetType();
            valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;

            TypeCode code = o.PlatformHelper.GetTypeCode(valueType);
            switch (code)
            {
                case TypeCode.Boolean:
                    builder.Append(XmlConvert.ToString((bool)value));
                    return builder.ToString();

                case TypeCode.Byte:
                    builder.Append(XmlConvert.ToString((byte)value));
                    return builder.ToString();

                case TypeCode.DateTime:
                    builder.Append(ExpressionConstants.LiteralPrefixDateTime);
                    builder.Append(ExpressionConstants.LiteralSingleQuote);
                    builder.Append(o.PlatformHelper.ConvertDateTimeToString((DateTime)value));
                    builder.Append(ExpressionConstants.LiteralSingleQuote);
                    return builder.ToString();

                case TypeCode.Decimal:
                    builder.Append(XmlConvert.ToString((Decimal)value));
                    builder.Append(ExpressionConstants.LiteralSuffixDecimal);
                    return builder.ToString();

                case TypeCode.Double:
                    builder.Append(AppendDecimalMarkerToDouble(XmlConvert.ToString((Double)value)));
                    builder.Append(ExpressionConstants.LiteralSuffixDouble);
                    return builder.ToString();

                case TypeCode.Int16:
                    builder.Append(XmlConvert.ToString((Int16)value));
                    return builder.ToString();

                case TypeCode.Int32:
                    builder.Append(XmlConvert.ToString((Int32)value));
                    return builder.ToString();

                case TypeCode.Int64:
                    builder.Append(XmlConvert.ToString((Int64)value));
                    builder.Append(ExpressionConstants.LiteralSuffixInt64);
                    return builder.ToString();

                case TypeCode.Object:
                    if (valueType == typeof(byte[]))
                    {
                        builder.Append(ConvertByteArrayToKeyString((byte[])value));
                    }
                    else if (valueType == typeof(Guid))
                    {
                        builder.Append(ExpressionConstants.LiteralPrefixGuid);
                        builder.Append(ExpressionConstants.LiteralSingleQuote);
                        builder.Append(value.ToString());
                        builder.Append(ExpressionConstants.LiteralSingleQuote);
                    }
                    else if (valueType == typeof(DateTimeOffset))
                    {
                        builder.Append(ExpressionConstants.LiteralPrefixDateTimeOffset);
                        builder.Append(ExpressionConstants.LiteralSingleQuote);
                        builder.Append(XmlConvert.ToString((DateTimeOffset)value));
                        builder.Append(ExpressionConstants.LiteralSingleQuote);
                    }
                    else if (valueType == typeof(TimeSpan))
                    {
                        builder.Append(ExpressionConstants.LiteralPrefixTime);
                        builder.Append(ExpressionConstants.LiteralSingleQuote);
                        builder.Append(XmlConvert.ToString((TimeSpan)value));
                        builder.Append(ExpressionConstants.LiteralSingleQuote);
                    }
                    else if (typeof(Geography).IsAssignableFrom(valueType))
                    {
                        ODataVersionChecker.CheckSpatialValue(version);
                        builder.Append(ExpressionConstants.LiteralPrefixGeography);
                        builder.Append(ExpressionConstants.LiteralSingleQuote);
                        builder.Append(WellKnownTextSqlFormatter.Create(true).Write((Geography)value));
                        builder.Append(ExpressionConstants.LiteralSingleQuote);
                    }
                    else if (typeof(Geometry).IsAssignableFrom(valueType))
                    {
                        ODataVersionChecker.CheckSpatialValue(version);
                        builder.Append(ExpressionConstants.LiteralPrefixGeometry);
                        builder.Append(ExpressionConstants.LiteralSingleQuote);
                        builder.Append(WellKnownTextSqlFormatter.Create(true).Write((Geometry)value));
                        builder.Append(ExpressionConstants.LiteralSingleQuote);
                    }
                    else
                    {
                        throw new ODataException(o.Strings.ODataUriUtils_ConvertToUriLiteralUnsupportedType(valueType.ToString()));
                    }

                    return builder.ToString();

                case TypeCode.SByte:
                    builder.Append(XmlConvert.ToString((SByte)value));
                    return builder.ToString();

                case TypeCode.Single:
                    builder.Append(XmlConvert.ToString((Single)value));
                    builder.Append(ExpressionConstants.LiteralSuffixSingle);
                    return builder.ToString();

                case TypeCode.String:
                    builder.Append(ExpressionConstants.LiteralSingleQuote);
                    builder.Append(((String)value).Replace("'", "''"));
                    builder.Append(ExpressionConstants.LiteralSingleQuote);
                    return builder.ToString();

                default:
                    throw new ODataException(o.Strings.ODataUriUtils_ConvertToUriLiteralUnsupportedType(valueType.ToString()));
            }
        }

        /// <summary>
        /// Converts the given string <paramref name="value"/> to an ODataComplexValue or ODataCollectionValue and returns it.
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
            using (StringReader reader = new StringReader(value))
            {
                using (ODataJsonInputContext context = new ODataJsonInputContext(
                    ODataFormat.VerboseJson,
                    reader,
                    settings,
                    version,
                    false /*readingResponse*/,
                    true /*synchronous*/,
                    model,
                    null /*urlResolver*/))
                {
                    ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(context);

                    deserializer.ReadPayloadStart(false);
                    object rawResult = deserializer.ReadNonEntityValue(typeReference, null /*DuplicatePropertyNameChecker*/, null /*CollectionWithoutExpectedTypeValidator*/, true /*validateNullValue*/);
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
                    throw new ODataException(o.Strings.ODataUriUtils_ConvertFromUriLiteralNullOnNonNullableType(expectedTypeReference.ODataFullName()));
                }

                IEdmType actualResolvedType = ValidationUtils.ValidateValueTypeName(model, nullValue.TypeName, expectedTypeReference.Definition.TypeKind);
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

                throw new ODataException(o.Strings.ODataUriUtils_ConvertFromUriLiteralNullTypeVerificationFailure(expectedTypeReference.ODataFullName(), nullValue.TypeName));
            }

            // Only other positive case is a numeric primitive that needs to be coerced
            IEdmPrimitiveTypeReference expectedPrimitiveTypeReference = expectedTypeReference.AsPrimitiveOrNull();
            if (expectedPrimitiveTypeReference == null)
            {
                throw new ODataException(o.Strings.ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure(expectedTypeReference.ODataFullName(), primitiveValue));
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

            throw new ODataException(o.Strings.ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure(expectedPrimitiveTypeReference.ODataFullName(), primitiveValue));
        }

        /// <summary>
        /// Converts a <see cref="ODataComplexValue"/> to a string for use in a Url.
        /// </summary>
        /// <param name="complexValue">Instance to convert.</param>
        /// <param name="model">Model to be used for validation. User model is optional. The EdmLib core model is expected as a minimum.</param>
        /// <param name="version">Version to be compliant with.</param>
        /// <returns>A string representation of <paramref name="complexValue"/> to be added to a Url.</returns>
        internal static string ConvertToUriComplexLiteral(ODataComplexValue complexValue, IEdmModel model, ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(complexValue, "complexValue");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            StringBuilder builder = new StringBuilder();
            using (TextWriter textWriter = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                JsonWriter jsonWriter = new JsonWriter(textWriter, false);
                bool writingResponse = false;
                ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings()
                {
                    Version = version
                };

                // Calling dispose since it's the right thing to do, but when created from JsonWriter
                // the output context Dispose will not actually dispose anything, it will just cleanup itself.
                using (ODataJsonOutputContext jsonOutputContext = ODataJsonOutputContext.Create(
                    ODataFormat.VerboseJson,
                    jsonWriter,
                    messageWriterSettings,
                    writingResponse,
                    model,
                    null /*urlResolver*/))
                {
                    ODataJsonPropertyAndValueSerializer jsonPropertyAndValueSerializer = new ODataJsonPropertyAndValueSerializer(jsonOutputContext);

                    jsonPropertyAndValueSerializer.WriteComplexValue(
                        complexValue,
                        null, /*propertyTypeReference - this call will fill this in and verify if possible*/
                        true, /*isOpenPropertyType - this determines if the TypeName will be written*/
                        jsonPropertyAndValueSerializer.CreateDuplicatePropertyNamesChecker(),
                        null /*collectionValidator*/);
                    jsonPropertyAndValueSerializer.AssertRecursionDepthIsZero();
                }
            }
            
            return builder.ToString();
        }

        /// <summary>
        /// Converts an <see cref="ODataUriNullValue"/> to a string for use in a Url.
        /// </summary>
        /// <param name="nullValue">Instance to convert.</param>
        /// <param name="model">Model to verify against. If not a user model, <paramref name="nullValue"/> will only be checked against empty/whitespace.</param>
        /// <returns>A string representation of <paramref name="nullValue"/> to be added to a Url.</returns>
        internal static string ConvertToUriNullValue(ODataUriNullValue nullValue, IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(nullValue, "nullValue");
            
            if (nullValue.TypeName != null)
            {
                ValidationUtils.ValidateTypeName(model, nullValue.TypeName);

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
        /// <returns>A string representation of <paramref name="collectionValue"/> to be added to a Url.</returns>
        internal static string ConvertToUriCollectionLiteral(ODataCollectionValue collectionValue, IEdmModel model, ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(collectionValue, "collectionValue");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            ODataVersionChecker.CheckCollectionValue(version);

            StringBuilder builder = new StringBuilder();
            using (TextWriter textWriter = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                JsonWriter jsonWriter = new JsonWriter(textWriter, false);
                ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings()
                {
                    Version = version
                };

                // Calling dispose since it's the right thing to do, but when created from JsonWriter
                // the output context Dispose will not actually dispose anything, it will just cleanup itself.
                using (ODataJsonOutputContext jsonOutputContext = ODataJsonOutputContext.Create(
                    ODataFormat.VerboseJson,
                    jsonWriter,
                    messageWriterSettings,
                    false /*writingResponse*/,
                    model,
                    null /*urlResolver*/))
                {
                    ODataJsonPropertyAndValueSerializer jsonPropertyAndValueSerializer = new ODataJsonPropertyAndValueSerializer(jsonOutputContext);

                    jsonPropertyAndValueSerializer.WriteCollectionValue(
                        collectionValue,
                        null, /*propertyTypeRefereence*/
                        false /*openPropertyType - this determines if the TypeName will be written*/);
                    jsonPropertyAndValueSerializer.AssertRecursionDepthIsZero();
                }
            }

            return builder.ToString();
        }

        /// <summary>Appends the decimal marker to string form of double value if necessary.</summary>
        /// <param name="input">Input string.</param>
        /// <returns>String with decimal marker optionally added.</returns>
        private static string AppendDecimalMarkerToDouble(string input)
        {
            if (input.Contains(".") || input.Contains("INF") || input.Contains("NaN"))
            {
                return input;
            }

            return input + ".0";
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
            TypeCode fromTypeCode = o.PlatformHelper.GetTypeCode(fromType);
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

        /// <summary>Converts the given byte[] into string.</summary>
        /// <param name="byteArray">byte[] that needs to be converted.</param>
        /// <returns>String containing hex values representing the byte[].</returns>
        private static string ConvertByteArrayToKeyString(byte[] byteArray)
        {
            StringBuilder hexBuilder = new StringBuilder(3 + byteArray.Length * 2);
            hexBuilder.Append(ExpressionConstants.LiteralPrefixShortBinary);
            hexBuilder.Append(ExpressionConstants.LiteralSingleQuote);
            for (int i = 0; i < byteArray.Length; i++)
            {
                hexBuilder.Append(byteArray[i].ToString("X2", CultureInfo.InvariantCulture));
            }

            hexBuilder.Append(ExpressionConstants.LiteralSingleQuote);
            return hexBuilder.ToString();
        }
    }
}
