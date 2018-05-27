//---------------------------------------------------------------------
// <copyright file="EdmValueWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;

#if ODATA_SERVICE
namespace Microsoft.OData.Service
#else
#if ODATA_CLIENT
namespace Microsoft.OData.Client
#else
#if ODATA_CORE
namespace Microsoft.OData
#else
namespace Microsoft.OData.Edm.Csdl
#endif
#endif
#endif
{
    /// <summary>
    /// Contains methods to convert primitive values to their string representation.
    /// </summary>
    internal static class EdmValueWriter
    {
        /// <summary>
        /// Characters used in string representations of hexadecimal values
        /// </summary>
        private static char[] Hex = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

#if !ODATA_CORE && !ODATA_CLIENT && !ODATA_SERVICE
        /// <summary>
        /// Converts the IEdmPrimitiveValue to a String.
        /// </summary>
        /// <param name="v">The value to convert.</param>
        /// <returns>A string representation of the IEdmPrimitiveValue.</returns>
        internal static string PrimitiveValueAsXml(IEdmPrimitiveValue v)
        {
            switch (v.ValueKind)
            {
                case EdmValueKind.Boolean:
                    return BooleanAsXml(((IEdmBooleanValue)v).Value);
                case EdmValueKind.Integer:
                    return LongAsXml(((IEdmIntegerValue)v).Value);
                case EdmValueKind.Floating:
                    return FloatAsXml(((IEdmFloatingValue)v).Value);
                case EdmValueKind.Guid:
                    return GuidAsXml(((IEdmGuidValue)v).Value);
                case EdmValueKind.Binary:
                    return BinaryAsXml(((IEdmBinaryValue)v).Value);
                case EdmValueKind.Decimal:
                    return DecimalAsXml(((IEdmDecimalValue)v).Value);
                case EdmValueKind.String:
                    return StringAsXml(((IEdmStringValue)v).Value);
                case EdmValueKind.DateTimeOffset:
                    return DateTimeOffsetAsXml(((IEdmDateTimeOffsetValue)v).Value);
                case EdmValueKind.Date:
                    return DateAsXml(((IEdmDateValue)v).Value);
                case EdmValueKind.Duration:
                    return DurationAsXml(((IEdmDurationValue)v).Value);
                case EdmValueKind.TimeOfDay:
                    return TimeOfDayAsXml(((IEdmTimeOfDayValue)v).Value);
                default:
                    throw new NotSupportedException(Edm.Strings.ValueWriter_NonSerializableValue(v.ValueKind));
            }
        }
#endif
        /// <summary>
        /// Converts the String to a String.
        /// </summary>
        /// <param name="s">The value to convert.</param>
        /// <returns>The value to convert.</returns>
        internal static string StringAsXml(string s)
        {
            return s;
        }

        /// <summary>
        /// Converts the Byte[] to a String.
        /// </summary>
        /// <param name="binary">The value to convert.</param>
        /// <returns>A string representation of the Byte[].</returns>
        internal static string BinaryAsXml(byte[] binary)
        {
            var chars = new char[binary.Length * 2];
            for (int i = 0; i < binary.Length; ++i)
            {
                chars[i << 1] = Hex[binary[i] >> 4];
                chars[i << 1 | 1] = Hex[binary[i] & 0x0F];
            }

            return new string(chars);
        }

        /// <summary>
        /// Converts the Boolean to a String.
        /// </summary>
        /// <param name="b">The value to convert.</param>
        /// <returns>A string representation of the Boolean, that is, "true" or "false".</returns>
        internal static string BooleanAsXml(bool b)
        {
            return XmlConvert.ToString(b);
        }

        /// <summary>
        /// Converts the Boolean? to a String.
        /// </summary>
        /// <param name="b">The value to convert.</param>
        /// <returns>A string representation of the Boolean, that is, "true" or "false".</returns>
        internal static string BooleanAsXml(bool? b)
        {
            Debug.Assert(b.HasValue, "Serialized nullable boolean must have value.");
            return BooleanAsXml(b.Value);
        }

        /// <summary>
        /// Converts the Int32 to a String.
        /// </summary>
        /// <param name="i">The value to convert</param>
        /// <returns>A string representation of the Int32.</returns>
        internal static string IntAsXml(int i)
        {
            return XmlConvert.ToString(i);
        }

        /// <summary>
        /// Converts the Int32? to a String.
        /// </summary>
        /// <param name="i">The value to convert</param>
        /// <returns>A string representation of the Int32.</returns>
        internal static string IntAsXml(int? i)
        {
            Debug.Assert(i.HasValue, "Serialized nullable integer must have value.");
            return IntAsXml(i.Value);
        }

        /// <summary>
        /// Converts the Int64 to a String.
        /// </summary>
        /// <param name="l">The value to convert.</param>
        /// <returns>A string representation of the Int64.</returns>
        internal static string LongAsXml(long l)
        {
            return XmlConvert.ToString(l);
        }

        /// <summary>
        /// Converts the Double to a String.
        /// </summary>
        /// <param name="f">The value to convert.</param>
        /// <returns>A string representation of the Double.</returns>
        internal static string FloatAsXml(double f)
        {
            return XmlConvert.ToString(f);
        }

        /// <summary>
        /// Converts the Decimal to a String.
        /// </summary>
        /// <param name="d">The value to convert.</param>
        /// <returns>A string representation of the Decimal.</returns>
        internal static string DecimalAsXml(decimal d)
        {
            return XmlConvert.ToString(d);
        }

        /// <summary>
        /// Converts the TimeSpan to a String.
        /// </summary>
        /// <param name="d">The value to convert.</param>
        /// <returns>A string representation of the TimeSpan.</returns>
        internal static string DurationAsXml(TimeSpan d)
        {
            return XmlConvert.ToString(d);
        }

        /// <summary>
        /// Converts the DateTimeOffset to a String.
        /// </summary>
        /// <param name="d">The System.DateTimeOffset to be converted.</param>
        /// <returns>A System.String representation of the supplied System.DateTimeOffset.</returns>
        internal static string DateTimeOffsetAsXml(DateTimeOffset d)
        {
            var value = XmlConvert.ToString(d);
            Debug.Assert(EdmValueParser.DayTimeDurationValidator.IsMatch(value), "Edm.Duration values should not have year or month part");
            return value;
        }

        /// <summary>
        /// Converts the Date to a String.
        /// </summary>
        /// <param name="d">The <see cref="Microsoft.OData.Edm.Date"/> to be converted</param>
        /// <returns>A System.String representation of the supplied <see cref="Microsoft.OData.Edm.Date"/>.</returns>
        internal static string DateAsXml(Date d)
        {
            var value = d.ToString();
            return value;
        }

        /// <summary>
        /// Converts the TimeOfDay to a String.
        /// </summary>
        /// <param name="time">The <see cref="Microsoft.OData.Edm.TimeOfDay"/> to be converted</param>
        /// <returns>A System.String representation of the supplied <see cref="Microsoft.OData.Edm.TimeOfDay"/>.</returns>
        internal static string TimeOfDayAsXml(TimeOfDay time)
        {
            var value = time.ToString();
            return value;
        }

        /// <summary>
        /// Converts the Guid to a String.
        /// </summary>
        /// <param name="g">The value to convert.</param>
        /// <returns>A string representation of the Guid.</returns>
        internal static string GuidAsXml(Guid g)
        {
            return XmlConvert.ToString(g);
        }

        /// <summary>
        /// Converts the Uri to a String.
        /// </summary>
        /// <param name="uri">The value to convert.</param>
        /// <returns>A string representation of the Uri.</returns>
        internal static string UriAsXml(Uri uri)
        {
            Debug.Assert(uri != null, "uri != null");
            return uri.OriginalString;
        }
    }
}
