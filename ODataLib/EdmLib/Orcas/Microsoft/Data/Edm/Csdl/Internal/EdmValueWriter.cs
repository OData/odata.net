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
#if ORCAS
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
