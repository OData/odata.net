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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Data.OData.Atom;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Xml;
    #endregion Namespaces.

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
        /// <param name="expectedType">The expected resource type of the value or null if no metadata is available.</param>
        internal static void WritePrimitiveValue(XmlWriter writer, object value, ResourceType expectedType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");
            
            string result;
            bool preserveWhitespace;
            if (!TryConvertPrimitiveToString(value, out result, out preserveWhitespace))
            {
                throw new ODataException(Strings.AtomValueUtils_CannotConvertValueToAtomPrimitive(value.GetType().FullName));
            }

            if (expectedType != null)
            {
                ValidationUtils.ValidateIsExpectedPrimitiveType(value, expectedType);
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
                    preserveWhitespace = true;
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
    }
}
