//---------------------------------------------------------------------
// <copyright file="DefaultPrimitiveValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.PrimitivePayloadValueConverters
{
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using Microsoft.OData.Edm;
	using Microsoft.OData.Edm.Library;
	using Microsoft.OData.Core.Json;
	using Microsoft.OData.Core.Metadata;
	using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// The default implementation of primitive payload value converter for validation and converting:
    ///     converts string to Byte[],
    ///     converts string to Guid,
    ///     converts string to TimeSpan,
    ///     converts string to Date,
    ///     converts string to TimeOfDay,
    ///     converts string to DateTimeOffset,
    ///     converts string to Double,
    ///     converts string to Single,
    ///     converts decimal to Int32,
    ///     converts decimal to Int64,
    ///     converts decimal to Double,
    ///     converts decimal to Single,
    ///     converts Double to Single
    /// </summary>
    public class DefaultPrimitivePayloadValueConverter : IPrimitivePayloadValueConverter
    {
        public static readonly IPrimitivePayloadValueConverter Instance = new DefaultPrimitivePayloadValueConverter();

        public virtual object ConvertToPayloadValue(object value, IEdmTypeReference edmTypeReference, ODataMessageWriterSettings messageWriterSettings)
        {
            IEdmPrimitiveTypeReference actualTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType());
            if (edmTypeReference != null)
            {
                ValidationUtils.ValidateIsExpectedPrimitiveType(value, actualTypeReference, edmTypeReference, !messageWriterSettings.EnableFullValidation);
            }

            return value;
        }

        public virtual object ConvertFromPayloadValue(object value, IEdmTypeReference edmTypeReference, ODataMessageReaderSettings messageReaderSettings)
        {
            IEdmPrimitiveTypeReference primitiveTypeReference = edmTypeReference as IEdmPrimitiveTypeReference;
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");
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
        /// Converts the given JSON string value to the expected type as per OData conversion rules for JSON values.
        /// </summary>
        /// <param name="stringValue">String value to the converted.</param>
        /// <param name="targetType">Target type to which the string value needs to be converted.</param>
        /// <returns>Object which is in sync with the target type.</returns>
        private static object ConvertStringValue(string stringValue, Type targetType)
        {
            // COMPAT 53: Support for System.Data.Linq.Binary and System.Xml.Linq.XElement
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

            // Date
            if (targetType == typeof(Date))
            {
                return PlatformHelper.ConvertStringToDate(stringValue);
            }

            // Time
            if (targetType == typeof(TimeOfDay))
            {
                return PlatformHelper.ConvertStringToTimeOfDay(stringValue);
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
