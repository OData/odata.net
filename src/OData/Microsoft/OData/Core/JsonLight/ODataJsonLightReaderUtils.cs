//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    using ODataPlatformHelper = Microsoft.OData.Core.PlatformHelper;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData reader for the JsonLight format.
    /// </summary>
    internal static class ODataJsonLightReaderUtils
    {
        /// <summary>
        /// Enumeration of all properties in error payloads, the value of the enum is the bitmask which identifies
        /// a bit per property.
        /// </summary>
        /// <remarks>
        /// We only use a single enumeration for both top-level as well as inner errors. 
        /// This means that some bits are never set for top-level (or inner errors).
        /// </remarks>
        [Flags]
        internal enum ErrorPropertyBitMask
        {
            /// <summary>No property found yet.</summary>
            None = 0,

            /// <summary>The "error" of the top-level object.</summary>
            Error = 1,

            /// <summary>The "code" property.</summary>
            Code = 2,

            /// <summary>The "message" property of either the error object or the inner error object.</summary>
            Message = 4,

            /// <summary>The "value" property of the message object.</summary>
            MessageValue = 16,

            /// <summary>The "innererror" or "internalexception" property of the error object or an inner error object.</summary>
            InnerError = 32,

            /// <summary>The "type" property of an inner error object.</summary>
            TypeName = 64,

            /// <summary>The "stacktrace" property of an inner error object.</summary>
            StackTrace = 128,
        }

        /// <summary>
        /// Checks whether the specified property has already been found before.
        /// </summary>
        /// <param name="propertiesFoundBitField">
        /// The bit field which stores which properties of an error or inner error were found so far.
        /// </param>
        /// <param name="propertyFoundBitMask">The bit mask for the property to check.</param>
        /// <returns>true if the property has not been read before; otherwise false.</returns>
        internal static bool ErrorPropertyNotFound(
            ref ODataJsonLightReaderUtils.ErrorPropertyBitMask propertiesFoundBitField,
            ODataJsonLightReaderUtils.ErrorPropertyBitMask propertyFoundBitMask)
        {
            Debug.Assert(((int)propertyFoundBitMask & (((int)propertyFoundBitMask) - 1)) == 0, "propertyFoundBitMask is not a power of 2.");

            if ((propertiesFoundBitField & propertyFoundBitMask) == propertyFoundBitMask)
            {
                return false;
            }

            propertiesFoundBitField |= propertyFoundBitMask;
            return true;
        }



        /// <summary>
        /// Converts the given JSON value to the expected type as per OData conversion rules for JSON values.
        /// </summary>
        /// <param name="value">Value to the converted.</param>
        /// <param name="primitiveTypeReference">Type reference to which the value needs to be converted.</param>
        /// <param name="messageReaderSettings">The message reader settings used for reading.</param>
        /// <param name="version">The version of the OData protocol used for reading.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>Object which is in sync with the property type (modulo the V1 exception of converting numbers to non-compatible target types).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time")]
        internal static object ConvertValue(
            object value,
            IEdmPrimitiveTypeReference primitiveTypeReference,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool validateNullValue,
            string propertyName)
        {
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");

            if (value == null)
            {
                // Only primitive type references are validated. Core model is sufficient.
                ReaderValidationUtils.ValidateNullValue(
                    EdmCoreModel.Instance,
                    primitiveTypeReference,
                    messageReaderSettings,
                    validateNullValue,
                    version,
                    propertyName);
                return null;
            }

            try
            {
                Type targetType = EdmLibraryExtensions.GetPrimitiveClrType(primitiveTypeReference.PrimitiveDefinition(), false);

                string stringValue = value as string;
                if (stringValue != null)
                {
                    return ConvertStringValue(stringValue, targetType);
                }
                else if (value is Int32)
                {
                    return ConvertInt32Value((int)value, targetType, primitiveTypeReference);
                }
                else if (value is Decimal)
                {
                    Decimal decimalValue = (Decimal)value;
                    if (targetType == typeof(Int64))
                    {
                        return Convert.ToInt64(decimalValue);
                    }

                    if (targetType == typeof(Double))
                    {
                        return Convert.ToDouble(decimalValue);
                    }

                    if (targetType == typeof(Single))
                    {
                        return Convert.ToSingle(decimalValue);
                    }

                    if (targetType != typeof(Decimal))
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertDecimal(primitiveTypeReference.ODataFullName()));
                    }
                }
                else if (value is Double)
                {
                    Double doubleValue = (Double)value;
                    if (targetType == typeof(Single))
                    {
                        return Convert.ToSingle(doubleValue);
                    }

                    if (targetType != typeof(Double))
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertDouble(primitiveTypeReference.ODataFullName()));
                    }
                }
                else if (value is bool)
                {
                    if (targetType != typeof(bool))
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertBoolean(primitiveTypeReference.ODataFullName()));
                    }
                }
                else if (value is DateTime)
                {
                    if (targetType != typeof(DateTime))
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertDateTime(primitiveTypeReference.ODataFullName()));
                    }
                }
                else if (value is DateTimeOffset)
                {
                    if (targetType != typeof(DateTimeOffset))
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertDateTimeOffset(primitiveTypeReference.ODataFullName()));
                    }
                }
            }
            catch (Exception e)
            {
                if (!ExceptionUtils.IsCatchableExceptionType(e))
                {
                    throw;
                }

                throw ReaderValidationUtils.GetPrimitiveTypeConversionException(primitiveTypeReference, e, value.ToString());
            }

            // otherwise just return the value without doing any conversion
            return value;
        }

        /// <summary>
        /// Ensure that the <paramref name="instance"/> is not null; if so create a new instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance to check.</typeparam>
        /// <param name="instance">The instance to check for null.</param>
        internal static void EnsureInstance<T>(ref T instance)
            where T : class, new()
        {
            if (instance == null)
            {
                instance = new T();
            }
        }

        /// <summary>
        /// Determines if the specified <paramref name="propertyName"/> is an OData annotation property name.
        /// </summary>
        /// <param name="propertyName">The property name to test.</param>
        /// <returns>true if the property name is an OData annotation property name, false otherwise.</returns>
        internal static bool IsODataAnnotationName(string propertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            return propertyName.StartsWith(JsonLightConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines if the specified property name is a name of an annotation property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>true if <paramref name="propertyName"/> is a name of an annotation property, false otherwise.</returns>
        /// <remarks>
        /// This method returns true both for normal annotation as well as property annotations.
        /// </remarks>
        internal static bool IsAnnotationProperty(string propertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            return propertyName.IndexOf('.') >= 0;
        }

        /// <summary>
        /// Validates that the annotation value is valid.
        /// </summary>
        /// <param name="propertyValue">The value of the annotation.</param>
        /// <param name="annotationName">The name of the (instance or property) annotation (used for error reporting).</param>
        internal static void ValidateAnnotationValue(object propertyValue, string annotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            if (propertyValue == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightReaderUtils_AnnotationWithNullValue(annotationName));
            }
        }

        /// <summary>
        /// Gets the payload type name for an OData OM instance for JsonLight.
        /// </summary>
        /// <param name="payloadItem">The payload item to get the type name for.</param>
        /// <returns>The type name as read from the payload item (or constructed for primitive items).</returns>
        internal static string GetPayloadTypeName(object payloadItem)
        {
            if (payloadItem == null)
            {
                return null;
            }

            TypeCode typeCode = ODataPlatformHelper.GetTypeCode(payloadItem.GetType());
            switch (typeCode)
            {
                // In JSON only boolean, String, Int32 and Double are recognized as primitive types
                // (without additional type conversion). So only check for those; if not one of these primitive
                // types it must be a complex, entity or collection value.
                case TypeCode.Boolean: return Metadata.EdmConstants.EdmBooleanTypeName;
                case TypeCode.String: return Metadata.EdmConstants.EdmStringTypeName;
                case TypeCode.Int32: return Metadata.EdmConstants.EdmInt32TypeName;
                case TypeCode.Double: return Metadata.EdmConstants.EdmDoubleTypeName;
                default:
                    Debug.Assert(typeCode == TypeCode.Object, "If not one of the primitive types above, it must be an object in JSON.");
                    break;
            }

            ODataComplexValue complexValue = payloadItem as ODataComplexValue;
            if (complexValue != null)
            {
                return complexValue.TypeName;
            }

            ODataCollectionValue collectionValue = payloadItem as ODataCollectionValue;
            if (collectionValue != null)
            {
                return EdmLibraryExtensions.GetCollectionTypeFullName(collectionValue.TypeName);
            }

            ODataEntry entry = payloadItem as ODataEntry;
            if (entry != null)
            {
                return entry.TypeName;
            }

            throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonLightReader_ReadEntryStart));
        }

        /// <summary>
        /// Converts the given JSON string value to the expected type as per OData conversion rules for JSON values.
        /// </summary>
        /// <param name="stringValue">String value to the converted.</param>
        /// <param name="targetType">Target type to which the string value needs to be converted.</param>
        /// <returns>Object which is in sync with the target type.</returns>
        private static object ConvertStringValue(string stringValue, Type targetType)
        {
            if (targetType == typeof(byte[]))
            {
                return Convert.FromBase64String(stringValue);
            }

            if (targetType == typeof(Guid))
            {
                return new Guid(stringValue);
            }

            // Convert.ChangeType does not support TimeSpan.
            if (targetType == typeof(TimeSpan))
            {
                return EdmValueParser.ParseDuration(stringValue);
            }

            // DateTimeOffset needs to be read using the XML rules (as per the JSON Light spec).
            if (targetType == typeof(DateTimeOffset))
            {
                return PlatformHelper.ConvertStringToDateTimeOffset(stringValue);
            }

            if (targetType == typeof(Double) || targetType == typeof(Single))
            {
                // Accept Infinity and -Infinity to perserve consistence
                if (stringValue == CultureInfo.InvariantCulture.NumberFormat.PositiveInfinitySymbol)
                {
                    stringValue = JsonValueUtils.ODataJsonPositiveInfinitySymbol;
                }
                else if (stringValue == CultureInfo.InvariantCulture.NumberFormat.NegativeInfinitySymbol)
                {
                    stringValue = JsonValueUtils.ODataJsonNegativeInfinitySymbol;
                }

                return Convert.ChangeType(stringValue, targetType, JsonValueUtils.ODataNumberFormatInfo);
            }

            // For string types, we support conversion to all possible primitive types
            return Convert.ChangeType(stringValue, targetType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the given JSON int value to the expected type as per OData conversion rules for JSON values.
        /// </summary>
        /// <param name="intValue">Int32 value to the converted.</param>
        /// <param name="targetType">Target type to which the int value needs to be converted.</param>
        /// <param name="primitiveTypeReference">Type reference to which the value needs to be converted.</param>
        /// <returns>Object which is in sync with the property type.</returns>
        private static object ConvertInt32Value(int intValue, Type targetType, IEdmPrimitiveTypeReference primitiveTypeReference)
        {
            if (targetType == typeof(Int16))
            {
                return Convert.ToInt16(intValue);
            }

            if (targetType == typeof(Byte))
            {
                return Convert.ToByte(intValue);
            }

            if (targetType == typeof(SByte))
            {
                return Convert.ToSByte(intValue);
            }

            if (targetType == typeof(Single))
            {
                return Convert.ToSingle(intValue);
            }

            if (targetType == typeof(Double))
            {
                return Convert.ToDouble(intValue);
            }

            if (targetType == typeof(Decimal))
            {
                return Convert.ToDecimal(intValue);
            }

            if (targetType == typeof(Int64))
            {
                return Convert.ToInt64(intValue);
            }

            if (targetType != typeof(Int32))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertInt32(primitiveTypeReference.ODataFullName()));
            }

            return intValue;
        }
    }
}
