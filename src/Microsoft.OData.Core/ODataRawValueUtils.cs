//---------------------------------------------------------------------
// <copyright file="ODataRawValueUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Utility methods around writing of raw values.
    /// </summary>
    internal static class ODataRawValueUtils
    {
        /// <summary>The characters that are considered to be whitespace by XmlConvert.</summary>
        private static readonly char[] XmlWhitespaceChars = new char[] { ' ', '\t', '\n', '\r' };

        /// <summary>Converts the specified value to a serializable string in ATOM format.</summary>
        /// <param name="value">Non-null value to convert.</param>
        /// <param name="result">The specified value converted to an ATOM string.</param>
        /// <returns>boolean value indicating conversion successful conversion</returns>
        internal static bool TryConvertPrimitiveToString(object value, out string result)
        {
            Debug.Assert(value != null, "value != null");

            if (value is bool)
            {
                result = ODataRawValueConverter.ToString((bool)value);
                return true;
            }

            if (value is byte)
            {
                result = ODataRawValueConverter.ToString((byte)value);
                return true;
            }

            if (value is decimal)
            {
                result = ODataRawValueConverter.ToString((decimal)value);
                return true;
            }

            if (value is double)
            {
                result = ODataRawValueConverter.ToString((double)value);
                return true;
            }

            if (value is Int16)
            {
                result = ODataRawValueConverter.ToString((Int16)value);
                return true;
            }

            if (value is Int32)
            {
                result = ODataRawValueConverter.ToString((Int32)value);
                return true;
            }

            if (value is Int64)
            {
                result = ODataRawValueConverter.ToString((Int64)value);
                return true;
            }

            if (value is SByte)
            {
                result = ODataRawValueConverter.ToString((SByte)value);
                return true;
            }

            var str = value as string;
            if (str != null)
            {
                result = str;
                return true;
            }

            if (value is Single)
            {
                result = ODataRawValueConverter.ToString((Single)value);
                return true;
            }

            byte[] bytes = value as byte[];
            if (bytes != null)
            {
                result = ODataRawValueConverter.ToString(bytes);
                return true;
            }

            if (value is DateTimeOffset)
            {
                result = ODataRawValueConverter.ToString((DateTimeOffset)value);
                return true;
            }

            if (value is Guid)
            {
                result = ODataRawValueConverter.ToString((Guid)value);
                return true;
            }

            if (value is TimeSpan)
            {
                // Edm.Duration
                result = ODataRawValueConverter.ToString((TimeSpan)value);
                return true;
            }

            if (value is Date)
            {
                // Edm.Date
                result = ODataRawValueConverter.ToString((Date)value);
                return true;
            }

            if (value is TimeOfDay)
            {
                // Edm.TimeOfDay
                result = ODataRawValueConverter.ToString((TimeOfDay)value);
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Converts a string to a primitive value.
        /// </summary>
        /// <param name="text">The string text to convert.</param>
        /// <param name="targetTypeReference">Type to convert the string to.</param>
        /// <returns>The value converted to the target type.</returns>
        /// <remarks>This method does not convert null value.</remarks>
        internal static object ConvertStringToPrimitive(string text, IEdmPrimitiveTypeReference targetTypeReference)
        {
            Debug.Assert(text != null, "text != null");
            Debug.Assert(targetTypeReference != null, "targetTypeReference != null");

            try
            {
                EdmPrimitiveTypeKind primitiveKind = targetTypeReference.PrimitiveKind();

                switch (primitiveKind)
                {
                    case EdmPrimitiveTypeKind.Binary:
                        return Convert.FromBase64String(text);
                    case EdmPrimitiveTypeKind.Boolean:
                        return ConvertXmlBooleanValue(text);
                    case EdmPrimitiveTypeKind.Byte:
                        return XmlConvert.ToByte(text);
                    case EdmPrimitiveTypeKind.DateTimeOffset:
                        return PlatformHelper.ConvertStringToDateTimeOffset(text);
                    case EdmPrimitiveTypeKind.Decimal:
                        return XmlConvert.ToDecimal(text);
                    case EdmPrimitiveTypeKind.Double:
                        return XmlConvert.ToDouble(text);
                    case EdmPrimitiveTypeKind.Guid:
                        return new Guid(text);
                    case EdmPrimitiveTypeKind.Int16:
                        return XmlConvert.ToInt16(text);
                    case EdmPrimitiveTypeKind.Int32:
                        return XmlConvert.ToInt32(text);
                    case EdmPrimitiveTypeKind.Int64:
                        return XmlConvert.ToInt64(text);
                    case EdmPrimitiveTypeKind.SByte:
                        return XmlConvert.ToSByte(text);
                    case EdmPrimitiveTypeKind.Single:
                        return XmlConvert.ToSingle(text);
                    case EdmPrimitiveTypeKind.String:
                        return text;
                    case EdmPrimitiveTypeKind.Duration:
                        return EdmValueParser.ParseDuration(text);
                    case EdmPrimitiveTypeKind.Date:
                        return PlatformHelper.ConvertStringToDate(text);
                    case EdmPrimitiveTypeKind.TimeOfDay:
                        return PlatformHelper.ConvertStringToTimeOfDay(text);
                    case EdmPrimitiveTypeKind.Stream:
                    case EdmPrimitiveTypeKind.None:
                    case EdmPrimitiveTypeKind.Geography:
                    case EdmPrimitiveTypeKind.GeographyCollection:
                    case EdmPrimitiveTypeKind.GeographyPoint:
                    case EdmPrimitiveTypeKind.GeographyLineString:
                    case EdmPrimitiveTypeKind.GeographyPolygon:
                    case EdmPrimitiveTypeKind.GeometryCollection:
                    case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                    case EdmPrimitiveTypeKind.GeographyMultiLineString:
                    case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    case EdmPrimitiveTypeKind.Geometry:
                    case EdmPrimitiveTypeKind.GeometryPoint:
                    case EdmPrimitiveTypeKind.GeometryLineString:
                    case EdmPrimitiveTypeKind.GeometryPolygon:
                    case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                    case EdmPrimitiveTypeKind.GeometryMultiLineString:
                    case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    default:
                        // Note that Astoria supports XElement and Binary as well, but they are serialized as string or byte[]
                        // and the metadata will actually talk about string and byte[] as well. Astoria will perform the conversion if necessary.
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawValueUtils_ConvertStringToPrimitive));
                }
            }
            catch (Exception e)
            {
                if (!ExceptionUtils.IsCatchableExceptionType(e))
                {
                    throw;
                }

                throw ReaderValidationUtils.GetPrimitiveTypeConversionException(targetTypeReference, e, text);
            }
        }

        /// <summary>
        /// Reimplementation of XmlConvert.ToBoolean that accepts 'True' and 'False' in addition
        /// to 'true' and 'false'.
        /// </summary>
        /// <param name="text">The string value read from the Xml reader.</param>
        /// <returns>The converted boolean value.</returns>
        private static bool ConvertXmlBooleanValue(string text)
        {
            text = text.Trim(XmlWhitespaceChars);

            switch (text)
            {
                case "true":
                case "True":
                case "1":
                    return true;

                case "false":
                case "False":
                case "0":
                    return false;

                default:
                    // We know that this will throw; call XmlConvert for the appropriate error message.
                    return XmlConvert.ToBoolean(text);
            }
        }
    }
}
