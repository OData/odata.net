//---------------------------------------------------------------------
// <copyright file="ODataPayloadValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Json;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Class for defining a payload value conversion for given model. Currently supports primitive only.
    /// </summary>
    public class ODataPayloadValueConverter
    {
        /// <summary>
        /// The default instance for <see cref="ODataPayloadValueConverter"/>.
        /// </summary>
        private static readonly ODataPayloadValueConverter Default = new ODataPayloadValueConverter();

        /// <summary>
        /// Converts the given primitive value defined in a type definition from the payload object.
        /// </summary>
        /// <param name="value">The given CLR value.</param>
        /// <param name="edmTypeReference">The expected type reference from model.</param>
        /// <returns>The converted payload value of the underlying type.</returns>
        public virtual object ConvertToPayloadValue(object value, IEdmTypeReference edmTypeReference)
        {
            return value;
        }

        /// <summary>
        /// Converts the given payload value to the type defined in a type definition.
        /// </summary>
        /// <param name="value">The given payload value.</param>
        /// <param name="edmTypeReference">The expected type reference from model.</param>
        /// <returns>The converted value of the type.</returns>
        public virtual object ConvertFromPayloadValue(object value, IEdmTypeReference edmTypeReference)
        {
            IEdmPrimitiveTypeReference primitiveTypeReference = edmTypeReference as IEdmPrimitiveTypeReference;
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");
            if (primitiveTypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.PrimitiveType)
            {
                return value;
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
                        throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertDecimal(primitiveTypeReference.FullName()));
                    }
                }
                else if (value is Double)
                {
                    return ConvertDoubleValue((Double)value, targetType, primitiveTypeReference);
                }
                else if (value is bool)
                {
                    if (targetType != typeof(bool))
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertBoolean(primitiveTypeReference.FullName()));
                    }
                }
                else if (value is DateTime)
                {
                    if (targetType != typeof(DateTime))
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertDateTime(primitiveTypeReference.FullName()));
                    }
                }
                else if (value is DateTimeOffset)
                {
                    if (targetType != typeof(DateTimeOffset))
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertDateTimeOffset(primitiveTypeReference.FullName()));
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

        internal static ODataPayloadValueConverter GetPayloadValueConverter(IServiceProvider container)
        {
            if (container == null)
            {
                return Default;
            }

            return container.GetRequiredService<ODataPayloadValueConverter>();
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
                // Accept Infinity and -Infinity to preserve consistency
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
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertInt32(primitiveTypeReference.FullName()));
            }

            return intValue;
        }

        private static object ConvertDoubleValue(double doubleValue, Type targetType, IEdmPrimitiveTypeReference primitiveTypeReference)
        {
            if (targetType == typeof(Single))
            {
                return Convert.ToSingle(doubleValue);
            }

            if (targetType == typeof(Decimal))
            {
                decimal doubleToDecimalR;

                // To keep the full precision of the current value, which if necessary is all 17 digits of precision supported by the Double type.
                if (decimal.TryParse(doubleValue.ToString("R", CultureInfo.InvariantCulture),
                    out doubleToDecimalR))
                {
                    return doubleToDecimalR;
                }

                return Convert.ToDecimal(doubleValue);
            }

            if (targetType != typeof(Double))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertDouble(primitiveTypeReference.FullName()));
            }

            return doubleValue;
        }
    }
}
