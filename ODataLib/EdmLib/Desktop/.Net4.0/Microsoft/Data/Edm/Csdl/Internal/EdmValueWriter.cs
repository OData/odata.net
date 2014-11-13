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

using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Csdl.Internal
{
    internal static class EdmValueWriter
    {
        private static char[] Hex = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

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
                case EdmValueKind.DateTime:
                    return DateTimeAsXml(((IEdmDateTimeValue)v).Value);
                case EdmValueKind.DateTimeOffset:
                    return DateTimeOffsetAsXml(((IEdmDateTimeOffsetValue)v).Value);
                case EdmValueKind.Time:
                    return TimeAsXml(((IEdmTimeValue)v).Value);
                default:
                    throw new NotSupportedException(Edm.Strings.ValueWriter_NonSerializableValue(v.ValueKind));
            }
        }

        internal static string StringAsXml(string s)
        {
            return s;
        }

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

        internal static string BooleanAsXml(bool b)
        {
            return XmlConvert.ToString(b);
        }

        internal static string BooleanAsXml(bool? b)
        {
            Debug.Assert(b.HasValue, "Serialized nullable boolean must have value.");
            return BooleanAsXml(b.Value);
        }

        internal static string IntAsXml(int i)
        {
            return XmlConvert.ToString(i);
        }

        internal static string IntAsXml(int? i)
        {
            Debug.Assert(i.HasValue, "Serialized nullable integer must have value.");
            return IntAsXml(i.Value);
        }

        internal static string LongAsXml(long l)
        {
            return XmlConvert.ToString(l);
        }

        internal static string FloatAsXml(double f)
        {
            return XmlConvert.ToString(f);
        }

        internal static string DecimalAsXml(decimal d)
        {
            return XmlConvert.ToString(d);
        }

        internal static string DateTimeAsXml(DateTime d)
        {
            return PlatformHelper.ConvertDateTimeToString(d);
        }

        internal static string TimeAsXml(TimeSpan d)
        {
#if ORCAS || PORTABLELIB
            return d.Hours.ToString("00", CultureInfo.InvariantCulture) + ":" + 
             d.Minutes.ToString("00", CultureInfo.InvariantCulture) + ":" + 
             d.Seconds.ToString("00", CultureInfo.InvariantCulture) + "." + 
             d.Milliseconds.ToString("000", CultureInfo.InvariantCulture);
#else
            return d.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
#endif
        }

        internal static string DateTimeOffsetAsXml(DateTimeOffset d)
        {
            return XmlConvert.ToString(d);
        }

        internal static string GuidAsXml(Guid g)
        {
            return XmlConvert.ToString(g);
        }
    }
}
