//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Atom;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Utility methods around writing of ATOM values.
    /// </summary>
    internal static class AtomValueUtils
    {
        /// <summary>The characters that are considered to be whitespace by XmlConvert.</summary>
        private static readonly char[] XmlWhitespaceChars = new char[] { ' ', '\t', '\n', '\r' };

        /// <summary>
        /// Converts the given value to the ATOM string representation
        /// and uses the writer to write it.
        /// </summary>
        /// <param name="writer">The writer to write the stringified value.</param>
        /// <param name="value">The value to be written.</param>
        internal static void WritePrimitiveValue(XmlWriter writer, object value)
        {
            Debug.Assert(value != null, "value != null");

            if (!PrimitiveConverter.Instance.TryWriteAtom(value, writer))
            {
                string result = ConvertPrimitiveToString(value);
                ODataAtomWriterUtils.WriteString(writer, result);
            }
        }

        /// <summary>Converts the specified value to a serializable string in ATOM format, or throws an exception if the value cannot be converted.</summary>
        /// <param name="value">Non-null value to convert.</param>
        /// <returns>The specified value converted to an ATOM string.</returns>
        internal static string ConvertPrimitiveToString(object value)
        {
            string result;
            if (!TryConvertPrimitiveToString(value, out result))
            {
                throw new ODataException(Strings.AtomValueUtils_CannotConvertValueToAtomPrimitive(value.GetType().FullName));
            }

            return result;
        }

        /// <summary>
        /// Reads a string value of an XML element and gets TypeName from model's EdmEnumTypeReference.
        /// </summary>
        /// <param name="reader">The XML reader to read the value from.</param>
        /// <param name="enumTypeReference">The enum rype reference.</param>
        /// <returns>An ODataEnumValue</returns>
        internal static ODataEnumValue ReadEnumValue(XmlReader reader, IEdmEnumTypeReference enumTypeReference)
        {
            Debug.Assert(reader != null, "reader != null");

            // skip the validation on value or type name.
            string stringValue = reader.ReadElementContentValue();
            string typeName = (enumTypeReference != null) ? enumTypeReference.ODataFullName() : null;
            return new ODataEnumValue(stringValue, typeName);
        }

        /// <summary>
        /// Reads a value of an XML element and converts it to the target primitive value.
        /// </summary>
        /// <param name="reader">The XML reader to read the value from.</param>
        /// <param name="primitiveTypeReference">The primitive type reference to convert the value to.</param>
        /// <returns>The primitive value read.</returns>
        /// <remarks>This method does not read null values, it only reads the actual element value (not its attributes).</remarks>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element   - the element to read the value for.
        ///                  XmlNodeType.Attribute - an attribute on the element to read the value for.
        /// Post-Condition:  XmlNodeType.Element    - the element was empty.
        ///                  XmlNodeType.EndElement - the element had some value.
        /// </remarks>
        internal static object ReadPrimitiveValue(XmlReader reader, IEdmPrimitiveTypeReference primitiveTypeReference)
        {
            Debug.Assert(reader != null, "reader != null");

            object spatialValue;
            if (!PrimitiveConverter.Instance.TryTokenizeFromXml(reader, EdmLibraryExtensions.GetPrimitiveClrType(primitiveTypeReference), out spatialValue))
            {
                string stringValue = reader.ReadElementContentValue();
                return ConvertStringToPrimitive(stringValue, primitiveTypeReference);
            }

            return spatialValue;
        }

        /// <summary>
        /// Converts a given <see cref="AtomTextConstructKind"/> to a string appropriate for Atom format.
        /// </summary>
        /// <param name="textConstructKind">The text construct kind to convert.</param>
        /// <returns>The string version of the text construct format in Atom format.</returns>
        internal static string ToString(AtomTextConstructKind textConstructKind)
        {
            switch (textConstructKind)
            {
                case AtomTextConstructKind.Text:
                    return AtomConstants.AtomTextConstructTextKind;
                case AtomTextConstructKind.Html:
                    return AtomConstants.AtomTextConstructHtmlKind;
                case AtomTextConstructKind.Xhtml:
                    return AtomConstants.AtomTextConstructXHtmlKind;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataAtomConvert_ToString));
            }
        }

        /// <summary>Converts the specified value to a serializable string in ATOM format.</summary>
        /// <param name="value">Non-null value to convert.</param>
        /// <param name="result">The specified value converted to an ATOM string.</param>
        /// <returns>boolean value indicating conversion successful conversion</returns>
        internal static bool TryConvertPrimitiveToString(object value, out string result)
        {
            Debug.Assert(value != null, "value != null");
            result = null;

            TypeCode typeCode = PlatformHelper.GetTypeCode(value.GetType());
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    result = ODataAtomConvert.ToString((bool)value);
                    break;

                case TypeCode.Byte:
                    result = ODataAtomConvert.ToString((byte)value);
                    break;

                case TypeCode.Decimal:
                    result = ODataAtomConvert.ToString((decimal)value);
                    break;

                case TypeCode.Double:
                    result = ODataAtomConvert.ToString((double)value);
                    break;

                case TypeCode.Int16:
                    result = ODataAtomConvert.ToString((Int16)value);
                    break;

                case TypeCode.Int32:
                    result = ODataAtomConvert.ToString((Int32)value);
                    break;

                case TypeCode.Int64:
                    result = ODataAtomConvert.ToString((Int64)value);
                    break;

                case TypeCode.SByte:
                    result = ODataAtomConvert.ToString((SByte)value);
                    break;

                case TypeCode.String:
                    result = (string)value;
                    break;

                case TypeCode.Single:
                    result = ODataAtomConvert.ToString((Single)value);
                    break;

                default:
                    byte[] bytes = value as byte[];
                    if (bytes != null)
                    {
                        result = ODataAtomConvert.ToString(bytes);
                        break;
                    }

                    if (value is DateTimeOffset)
                    {
                        result = ODataAtomConvert.ToString((DateTimeOffset)value);
                        break;
                    }

                    if (value is Guid)
                    {
                        result = ODataAtomConvert.ToString((Guid)value);
                        break;
                    }

                    if (value is TimeSpan)
                    {
                        // Edm.Duration
                        result = ODataAtomConvert.ToString((TimeSpan)value);
                        break;
                    }

                    return false;
            }

            Debug.Assert(result != null, "result != null");
            return true;
        }

        /// <summary>
        /// Converts a string to a primitive value.
        /// </summary>
        /// <param name="text">The string text to convert.</param>
        /// <param name="targetTypeReference">Type to convert the string to.</param>
        /// <returns>The value converted to the target type.</returns>
        /// <remarks>This method does not convert null value.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time")]
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
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.AtomValueUtils_ConvertStringToPrimitive));
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
