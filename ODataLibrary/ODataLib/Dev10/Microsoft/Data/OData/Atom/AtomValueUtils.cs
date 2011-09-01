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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Atom;

    #endregion Namespaces

    /// <summary>
    /// Utility methods around writing of ATOM values.
    /// </summary>
    internal static class AtomValueUtils
    {
        /// <summary>
        /// Converts the given value to the ATOM string representation
        /// and uses the writer to write it.
        /// </summary>
        /// <param name="writer">The writer to write the stringified value.</param>
        /// <param name="value">The value to be written.</param>
        /// <param name="expectedTypeReference">The expected type of the value or null if no metadata is available.</param>
        internal static void WritePrimitiveValue(XmlWriter writer, object value, IEdmTypeReference expectedTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");

            if (expectedTypeReference != null)
            {
                ValidationUtils.ValidateIsExpectedPrimitiveType(value, expectedTypeReference);
            }

            if (!WriterUtils.PrimitiveConverter.TryWriteAtom(value, writer))
            {
                string result;
                bool preserveWhitespace;
                if (!TryConvertPrimitiveToString(value, out result, out preserveWhitespace))
                {
                    throw new ODataException(Strings.AtomValueUtils_CannotConvertValueToAtomPrimitive(value.GetType().FullName));
                }

                if (preserveWhitespace)
                {
                    writer.WriteAttributeString(
                        AtomConstants.XmlNamespacePrefix,
                        AtomConstants.XmlSpaceAttributeName,
                        AtomConstants.XmlNamespace,
                        AtomConstants.XmlPreserveSpaceAttributeValue);
                }

                writer.WriteString(result);
            }
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(reader != null, "reader != null");

            string stringValue = reader.ReadElementContentValue();
            return ConvertStringToPrimitive(stringValue, primitiveTypeReference);
        }

        /// <summary>
        /// Converts a given <see cref="AtomTextConstructKind"/> to a string appropriate for Atom format.
        /// </summary>
        /// <param name="textConstructKind">The text construct kind to convert.</param>
        /// <returns>The string version of the text construct format in Atom format.</returns>
        internal static string ToString(AtomTextConstructKind textConstructKind)
        {
            DebugUtils.CheckNoExternalCallers();

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
        /// <param name="preserveWhitespace">A flag indicating whether whitespace needs to be preserved.</param>
        /// <returns>boolean value indicating conversion successful conversion</returns>
        internal static bool TryConvertPrimitiveToString(object value, out string result, out bool preserveWhitespace)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");
            result = null;
            preserveWhitespace = false;

            TypeCode typeCode = Type.GetTypeCode(value.GetType());
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    result = ODataAtomConvert.ToString((bool)value);
                    break;

                case TypeCode.Byte:
                    result = ODataAtomConvert.ToString((byte)value);
                    break;

                case TypeCode.DateTime:
                    result = ODataAtomConvert.ToString((DateTime)value);
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
                    int length = result.Length;
                    if (length > 0 && (Char.IsWhiteSpace(result[0]) || Char.IsWhiteSpace(result[length - 1])))
                    {
                        // xml:space="preserve"
                        preserveWhitespace = true;
                    }

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
                        // Edm.Time
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
        internal static object ConvertStringToPrimitive(string text, IEdmPrimitiveTypeReference targetTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
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
                        return XmlConvert.ToBoolean(text);
                    case EdmPrimitiveTypeKind.Byte:
                        return XmlConvert.ToByte(text);
                    case EdmPrimitiveTypeKind.DateTime:
                        return XmlConvert.ToDateTime(text, XmlDateTimeSerializationMode.RoundtripKind);
                    case EdmPrimitiveTypeKind.DateTimeOffset:
                        return XmlConvert.ToDateTimeOffset(text);
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
                    case EdmPrimitiveTypeKind.Time:
                        return XmlConvert.ToTimeSpan(text);
                    case EdmPrimitiveTypeKind.Stream:
                    case EdmPrimitiveTypeKind.None:
                    case EdmPrimitiveTypeKind.Geography:
                    case EdmPrimitiveTypeKind.GeographyCollection:
                    case EdmPrimitiveTypeKind.Point:
                    case EdmPrimitiveTypeKind.LineString:
                    case EdmPrimitiveTypeKind.Polygon:
                    case EdmPrimitiveTypeKind.GeometryCollection:
                    case EdmPrimitiveTypeKind.MultiPolygon:
                    case EdmPrimitiveTypeKind.MultiLineString:
                    case EdmPrimitiveTypeKind.MultiPoint:
                    case EdmPrimitiveTypeKind.Geometry:
                    case EdmPrimitiveTypeKind.GeometricPoint:
                    case EdmPrimitiveTypeKind.GeometricLineString:
                    case EdmPrimitiveTypeKind.GeometricPolygon:
                    case EdmPrimitiveTypeKind.GeometricMultiPolygon:
                    case EdmPrimitiveTypeKind.GeometricMultiLineString:
                    case EdmPrimitiveTypeKind.GeometricMultiPoint:
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

                throw ReaderValidationUtils.GetPrimitiveTypeConversionException(targetTypeReference, e);
            }
        }
    }
}
