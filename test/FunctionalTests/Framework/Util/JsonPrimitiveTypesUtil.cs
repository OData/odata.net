//---------------------------------------------------------------------
// <copyright file="JsonPrimitiveTypesUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
#if !ClientSKUFramework
using System.Data.Linq;
#endif

using System.Text.RegularExpressions;

namespace System.Data.Test.Astoria.Util
{
    public static class JsonPrimitiveTypesUtil
    {
        /// <summary>Types whose JSON literals require quotes. Does not include nullable primitives.</summary>
        private static Dictionary<Type, bool> quotedTypes;

        internal const string DateTimeFormat = @"\/Date({0})\/";
        internal const string DateTimeOffsetFormat = @"\/Date({0}{1}{2:D4})\/";
        public static readonly Regex DateTimeRegex = new Regex(@"^/Date\((?<ticks>-?[0-9]+)\)/", RegexOptions.Compiled);
        public static readonly Regex DateTimeOffsetRegex = new Regex(@"^/Date\((?<ticks>-?[0-9]+)(?<offset>[+-][0-9]{4})\)/", RegexOptions.Compiled);

        public static readonly long DatetimeMinTimeTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        public static string GetJsonDateTimeStringValue(DateTime dateTime)
        {
            return String.Format(CultureInfo.InvariantCulture, DateTimeFormat, (ToUniversal(dateTime).Ticks - DatetimeMinTimeTicks) / 10000);
        }

        public static string GetJsonDateTimeOffsetStringValue(DateTimeOffset dateTimeOffset)
        {
            Int32 offsetMins = (Int32)dateTimeOffset.Offset.TotalMinutes;
            return String.Format(
                CultureInfo.InvariantCulture,
                DateTimeOffsetFormat,
                (ToUniversal(dateTimeOffset.DateTime).Ticks - DatetimeMinTimeTicks) / 10000,
                offsetMins >= 0 ? "+" : string.Empty,
                offsetMins);
        }

        /// <summary>
        /// Filters a JSON stream reader by stripping out the leading and trailing
        /// comment marks.
        /// </summary>
        /// <param name="json">Reader to filter.</param>
        /// <returns>A reader that filters content.</returns>
        /// <remarks>
        /// This method relies on the reader never writing literal '*' 
        /// characters except in the leading and traling comments.
        /// </remarks>
        public static string FilterJson(string json)
        {
            const string prefix = "{\"d\":";
            const string suffix = "}";

            if (json == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (json.StartsWith(prefix) && json.EndsWith(suffix))
            {
                return json.Substring(prefix.Length, json.Length - prefix.Length - suffix.Length);
            }
            else
            {
                return json;
            }
        }

        public static DateTime ToUniversal(DateTime dateTime)
        {
            switch (dateTime.Kind)
            {
                case DateTimeKind.Local:
                    return dateTime.ToUniversalTime();
                case DateTimeKind.Unspecified:
                    return new DateTime(dateTime.Ticks, DateTimeKind.Utc);
                case DateTimeKind.Utc:
                default:
                    return dateTime;
            }
        }

        public static DateTimeOffset ToUniversal(DateTimeOffset dateTimeOffset)
        {
            switch (dateTimeOffset.DateTime.Kind)
            {
                case DateTimeKind.Local:
                    return new DateTimeOffset(dateTimeOffset.DateTime.ToUniversalTime(), dateTimeOffset.Offset);
                case DateTimeKind.Unspecified:
                    // for UTC datetime, the offset must be zero
                    return new DateTimeOffset(new DateTime(dateTimeOffset.DateTime.Ticks, DateTimeKind.Utc));
                case DateTimeKind.Utc:
                default:
                    return dateTimeOffset;
            }
        }

        private static string ConvertToJsonString(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return String.Empty;
            }

            StringBuilder b = null;
            int startIndex = 0;
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                // Append the unhandled characters (that do not require special treament)
                // to the string builder when special characters are detected.
                if (c == '\r' || c == '\t' || c == '\"' || c == '\'' ||
                    c == '\\' || c == '\r' || c < ' ' || c > 0x7F)
                {
                    if (b == null)
                    {
                        // Since 6 is the max number of characters that can be inserted for one character
                        b = new StringBuilder(s.Length + 6);
                    }

                    if (count > 0)
                    {
                        b.Append(s, startIndex, count);
                    }

                    startIndex = i + 1;
                    count = 0;
                }

                switch (c)
                {
                    case '\r':
                        b.Append("\\r");
                        break;
                    case '\t':
                        b.Append("\\t");
                        break;
                    case '\"':
                        b.Append("\\\"");
                        break;
                    case '\'':
                        b.Append("\\\'");
                        break;
                    case '\\':
                        b.Append("\\\\");
                        break;
                    case '\n':
                        b.Append("\\n");
                        break;
                    default:
                        if ((c < ' ') || (c > 0x7F))
                        {
                            b.AppendFormat(CultureInfo.InvariantCulture, "\\u{0:x4}", (int)c);
                        }
                        else
                        {
                            count++;
                        }

                        break;
                }
            }

            string processedString = s;
            if (b != null)
            {
                if (count > 0)
                {
                    b.Append(s, startIndex, count);
                }

                processedString = b.ToString();
            }

            return processedString;
        }
        private static string GetQuotedJsonString(string value)
        {
            return String.Format("\"{0}\"", value);
        }

        /// <summary>
        /// Checks whether the JSON literal for the specified type requires quotes - 
        /// does not handle nullability.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <remarks>true if the type is quoted; false otherwise.</remarks>
        public static bool IsTypeQuoted(Type type)
        {
            if (quotedTypes == null)
            {
                quotedTypes = new Dictionary<Type, bool>();
                quotedTypes.Add(typeof(byte[]), true);
#if !ClientSKUFramework
                quotedTypes.Add(typeof(Binary), true);
#endif
                quotedTypes.Add(typeof(Boolean), false);
                quotedTypes.Add(typeof(Byte), false);
                quotedTypes.Add(typeof(DateTime), true);
                quotedTypes.Add(typeof(DateTimeOffset), true);
                quotedTypes.Add(typeof(Decimal), true);
                quotedTypes.Add(typeof(Double), false);
                quotedTypes.Add(typeof(Guid), true);
                quotedTypes.Add(typeof(Int16), false);
                quotedTypes.Add(typeof(Int32), false);
                quotedTypes.Add(typeof(Int64), true);
                quotedTypes.Add(typeof(SByte), false);
                quotedTypes.Add(typeof(Single), false);
                quotedTypes.Add(typeof(String), true);
                quotedTypes.Add(typeof(TimeSpan), true);
                quotedTypes.Add(typeof(UInt16), false);
                quotedTypes.Add(typeof(UInt32), true);
                quotedTypes.Add(typeof(UInt64), true);
                quotedTypes.Add(typeof(System.Xml.Linq.XElement), true);
            }

            return quotedTypes[type];
        }

        public static string PrimitiveToString(object value, Type inputPrimitiveType)
        {
            if (value == null)
            {
                return "null";
            }

            Type primitiveType = inputPrimitiveType;
            if (primitiveType == null && value != null)
            {
                primitiveType = value.GetType();
            }

            primitiveType = Nullable.GetUnderlyingType(primitiveType) ?? primitiveType;

            string stringValue = null;

            if (primitiveType == typeof(byte[]))
            {
                stringValue = Convert.ToBase64String((byte[])value, Base64FormattingOptions.None);
            }
#if !ClientSKUFramework

            else if (primitiveType == typeof(Binary))
            {
                stringValue = Convert.ToBase64String(((Binary)value).ToArray(), Base64FormattingOptions.None);
            }
#endif
            else if (primitiveType == typeof(bool))
            {
                stringValue = value.ToString().ToLowerInvariant();
            }
            else if (primitiveType == typeof(byte) ||
                     primitiveType == typeof(Guid) ||
                     primitiveType == typeof(Int16) ||
                     primitiveType == typeof(Int32) ||
                     primitiveType == typeof(Int64) ||
                     primitiveType == typeof(sbyte) ||
                     primitiveType == typeof(UInt16) ||
                     primitiveType == typeof(UInt32) ||
                     primitiveType == typeof(UInt64))
            {
                stringValue = value.ToString();
            }
            else if (primitiveType == typeof(Double))
            {
                double d = (double)value;
                stringValue = XmlConvert.ToString(d);
                if (Double.IsInfinity(d) || Double.IsNaN(d))
                {
                    stringValue = GetQuotedJsonString(d.ToString(Globalization.CultureInfo.InvariantCulture));
                }
            }
            else if (primitiveType == typeof(Decimal))
            {
                Decimal d = (Decimal)value;
                stringValue = XmlConvert.ToString(d);
            }
            else if (primitiveType == typeof(Single))
            {
                float f = (float)value;
                stringValue = XmlConvert.ToString(f);
                if (Single.IsInfinity(f) || Single.IsNaN(f))
                {
                    stringValue = GetQuotedJsonString(f.ToString(Globalization.CultureInfo.InvariantCulture));
                }
            }
            else if (primitiveType == typeof(DateTime))
            {
                stringValue = GetJsonDateTimeStringValue((DateTime)value);
            }
            else if (primitiveType == typeof(DateTimeOffset))
            {
                stringValue = GetJsonDateTimeOffsetStringValue((DateTimeOffset)value);
            }
            else if (primitiveType == typeof(string))
            {
                // if there are any unicode characters
                stringValue = ConvertToJsonString((string)value);
            }
            else if (primitiveType == typeof(System.Xml.Linq.XElement))
            {
                // if there are any unicode characters
                stringValue = ((System.Xml.Linq.XElement)value).ToString(System.Xml.Linq.SaveOptions.None);
                stringValue = ConvertToJsonString(stringValue);
            }
            else if (primitiveType == typeof(TimeSpan))
            {
                stringValue = XmlConvert.ToString((TimeSpan)value);
            }
            else if (inputPrimitiveType == null && primitiveType == typeof(DBNull))
            {
                return null;
            }
            else
            {
                throw new Exception(String.Format("Invalid PrimitiveType encountered : {0}", primitiveType.FullName));
            }

            if (PrimitiveTypeRequiresQuotes(primitiveType))
            {
                return GetQuotedJsonString(stringValue);
            }
            return stringValue;
        }
        
        public static string PrimitiveToKeyString(object value, Type inputPrimitiveType)
        {
            Type primitiveType = inputPrimitiveType;
            if (primitiveType == null && value != null)
            {
                primitiveType = value.GetType();
            }

            if (value == null)
            {
                return "null";
            }

            string stringValue = null;

            if (primitiveType == typeof(byte[]))
            {
                stringValue = Convert.ToBase64String((byte[])value, Base64FormattingOptions.None);
            }
#if !ClientSKUFramework

            else if (primitiveType == typeof(Binary))
            {
                stringValue = Convert.ToBase64String(((Binary)value).ToArray(), Base64FormattingOptions.None);
            }
#endif
            else if (primitiveType == typeof(bool))
            {
                stringValue = value.ToString().ToLowerInvariant();
            }
            else if (primitiveType == typeof(byte) ||
                     primitiveType == typeof(Decimal) ||
                     primitiveType == typeof(Guid) ||
                     primitiveType == typeof(Int16) ||
                     primitiveType == typeof(Int32) ||
                     primitiveType == typeof(Int64) ||
                     primitiveType == typeof(sbyte) ||
                     primitiveType == typeof(UInt16) ||
                     primitiveType == typeof(UInt32) ||
                     primitiveType == typeof(UInt64))
            {
                stringValue = value.ToString();
            }
            else if (primitiveType == typeof(Double))
            {
                double d = (double)value;
                stringValue = XmlConvert.ToString(d);
            }
            else if (primitiveType == typeof(Single))
            {
                float f = (float)value;
                stringValue = XmlConvert.ToString(f);
            }
            else if (primitiveType == typeof(DateTime))
            {
                stringValue = GetJsonDateTimeStringValue((DateTime)value);
            }
            else if (primitiveType == typeof(string))
            {
                // if there are any unicode characters
                stringValue = ConvertToJsonString((string)value);
            }
            else if (primitiveType == typeof(System.Xml.Linq.XElement))
            {
                // if there are any unicode characters
                stringValue = ((System.Xml.Linq.XElement)value).ToString(System.Xml.Linq.SaveOptions.None);
                stringValue = ConvertToJsonString((string)value);
            }
            else if (inputPrimitiveType == null && primitiveType == typeof(DBNull))
            {
                return null;
            }
            else
            {
                throw new Exception(String.Format("Invalid PrimitiveType encountered : {0}", primitiveType.FullName));
            }

            if (PrimitiveTypeRequiresQuotes(primitiveType))
            {
                if (IsTypeQuoted(inputPrimitiveType))
                {
                    return String.Format("'{0}'", value);
                }
            }
            return stringValue;
        }

        public static string DateTimeToString(object value)
        {
            if (value == null)
                return "null";
            else
                return String.Format("'{0}'", System.Xml.XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.RoundtripKind));
        }

        public static string PrimitiveToStringUnquoted(object value, Type inputPrimitiveType)
        {
            Type primitiveType = inputPrimitiveType;
            if (primitiveType == null && value != null)
            {
                primitiveType = value.GetType();
            }

            if (value == null)
            {
                return "null";
            }

            string stringValue = null;

            if (primitiveType == typeof(byte[]))
            {
                stringValue = Convert.ToBase64String((byte[])value, Base64FormattingOptions.None);
            }
            else if (primitiveType == typeof(bool))
            {
                stringValue = value.ToString().ToLowerInvariant();
            }
            else if (primitiveType == typeof(byte) ||
                     primitiveType == typeof(Decimal) ||
                     primitiveType == typeof(Guid) ||
                     primitiveType == typeof(Int16) ||
                     primitiveType == typeof(Int32) ||
                     primitiveType == typeof(Int64) ||
                     primitiveType == typeof(sbyte) ||
                     primitiveType == typeof(UInt16) ||
                     primitiveType == typeof(UInt32) ||
                     primitiveType == typeof(UInt64))
            {
                stringValue = value.ToString();
            }
            else if (primitiveType == typeof(Double))
            {
                double d = (double)value;
                stringValue = XmlConvert.ToString(d);
                if (Double.IsInfinity(d) || Double.IsNaN(d))
                {
                    stringValue = d.ToString();
                }
            }
            else if (primitiveType == typeof(Single))
            {
                float f = (float)value;
                stringValue = XmlConvert.ToString(f);
                if (Single.IsInfinity(f) || Single.IsNaN(f))
                {
                    stringValue = f.ToString();
                }
            }
            else if (primitiveType == typeof(DateTime))
            {
                stringValue = GetJsonDateTimeStringValue((DateTime)value);
            }
            else if (primitiveType == typeof(string))
            {
                // if there are any unicode characters
                stringValue = ConvertToJsonString((string)value);
            }
            else if (inputPrimitiveType == null && primitiveType == typeof(DBNull))
            {
                return null;
            }
            else
            {
                throw new Exception(String.Format("Invalid PrimitiveType encountered : {0}", primitiveType.FullName));
            }

            return stringValue;
        }

        /// <summary>
        /// Checks whether the specified type requires quotes in JSON format.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if the type uses quotes in its literal form; false otherwise.</returns>
        public static bool PrimitiveTypeRequiresQuotes(Type type)
        {
            TestUtil.CheckArgumentNotNull(type, "type");
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return IsTypeQuoted(type);
        }

        public static object StringToPrimitive(string value, Type primitiveType)
        {
            if (value == null)
            {
                AstoriaTestLog.IsTrue(primitiveType.IsClass);
                return null;
            }

            if (primitiveType.IsGenericType &&
                primitiveType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                primitiveType = primitiveType.GetGenericArguments()[0];
            }

            //value = GetUnquotedJsonString(value, primitiveType);
            if (primitiveType == typeof(byte[]))
            {
                return Convert.FromBase64String(value);
            }
#if !ClientSKUFramework

            else if (primitiveType == typeof(Binary))
            {
                return new Binary(Convert.FromBase64String(value));
            }
#endif
            else if (primitiveType == typeof(bool))
            {
                return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
            }
            else if (primitiveType == typeof(byte))
            {
                return Convert.ToByte(value, CultureInfo.InvariantCulture);
            }
            else if (primitiveType == typeof(DateTime))
            {
                Match m = DateTimeRegex.Match(value);
                if (m.Success)
                {
                    string ticksStr = m.Groups["ticks"].Value;

                    long ticks;
                    if (long.TryParse(ticksStr, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out ticks))
                    {
                        return new DateTime(ticks * 10000 + DatetimeMinTimeTicks, DateTimeKind.Utc);
                    }
                }

                return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
            }
            else if (primitiveType == typeof(Decimal))
            {
                NumberFormatInfo formatInfo = new NumberFormatInfo();

                return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            }
            else if (primitiveType == typeof(Double))
            {
                double d;
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                    return d;
                return XmlConvert.ToDouble(value); // +- INF
            }
            else if (primitiveType == typeof(Guid))
            {
                return new Guid(value);
            }
            else if (primitiveType == typeof(Int16))
            {
                return Convert.ToInt16(value, CultureInfo.InvariantCulture);
            }
            else if (primitiveType == typeof(Int32))
            {
                return Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }
            else if (primitiveType == typeof(Int64))
            {
                return Convert.ToInt64(value, CultureInfo.InvariantCulture);
            }
            else if (primitiveType == typeof(sbyte))
            {
                return Convert.ToSByte(value, CultureInfo.InvariantCulture);
            }
            else if (primitiveType == typeof(Single))
            {
                Single s;
                if (Single.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out s))
                    return s;
                return XmlConvert.ToSingle(value); // +- INF
            }
            else if (primitiveType == typeof(string))
            {
                if (value == null)
                {
                    return "null";
                }
                return value;
            }
            else if (primitiveType == typeof(UInt16))
            {
                return Convert.ToUInt16(value, CultureInfo.InvariantCulture);
            }
            else if (primitiveType == typeof(UInt32))
            {
                return Convert.ToUInt32(value, CultureInfo.InvariantCulture);
            }
            else if (primitiveType == typeof(UInt64))
            {
                return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            }
            else if (primitiveType == typeof(System.Xml.Linq.XElement))
            {
                return System.Xml.Linq.XElement.Parse(value, System.Xml.Linq.LoadOptions.PreserveWhitespace);
            }
            else if (primitiveType == typeof(TimeSpan))
            {
                return XmlConvert.ToTimeSpan(value);
            }
            else if (primitiveType == typeof(DateTimeOffset))
            {
                Match m = DateTimeOffsetRegex.Match(value);
                if (m.Success)
                {
                    string ticksStr = m.Groups["ticks"].Value;
                    string offset = m.Groups["offset"].Value;

                    long ticks;
                    Int32 offsetMins;
                    if (long.TryParse(ticksStr, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out ticks) &&
                        Int32.TryParse(offset, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out offsetMins))
                    {
                        return new DateTimeOffset(ticks * 10000 + DatetimeMinTimeTicks, new TimeSpan(0, offsetMins, 0));
                    }
                }

                return XmlConvert.ToDateTimeOffset(value);
            }

            throw new Exception(String.Format("Invalid PrimitiveType encountered : {0}", primitiveType.FullName));
        }

        /// <summary>
        /// This method returns the clr type that the given clr type maps to by default by the json reader.
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        public static Type JsonPrimitiveTypeMapping(object value)
        {
            if (value == null)
            {
                return typeof(string);
            }

            Type inputType = value.GetType();
            if (inputType == typeof(bool))
            {
                return typeof(bool);
            }

            if (inputType == typeof(byte) ||
                inputType == typeof(sbyte) ||
                inputType == typeof(Int16) ||
                inputType == typeof(Int32))
            {
                return typeof(Int32);
            }

            if (inputType == typeof(DateTime))
            {
                return typeof(DateTime);
            }

            if (inputType == typeof(DateTimeOffset))
            {
                return typeof(DateTimeOffset);
            }

            if (IsTypeQuoted(inputType))
            {
                return typeof(String);
            }

            // for some cases, the type depends on the value
            // for e.g. single, double - if the value can be parsed as int32, then it becomes int32
            // otherwise its an double.
            if (inputType == typeof(Single) ||
                inputType == typeof(Double))
            {
                int result;
                if (Int32.TryParse(value.ToString(), out result))
                {
                    return typeof(Int32);
                }
                else
                {
                    Double d;
                    Single? s = value as Single?;
                    if (s != null)
                    {
                        d = Double.Parse(PrimitiveToStringUnquoted(value, value.GetType()));
                    }
                    else
                    {
                        d = (Double)value;
                    }

                    if (double.IsInfinity(d) || double.IsNaN(d))
                    {
                        return typeof(string);
                    }
                    else
                    {
                        return typeof(double);
                    }
                }
            }

            throw new Exception(String.Format("Unexpected value passed, type: '{0}', value: '{1}'", inputType.FullName, value.ToString()));
        }

        internal class JsonStreamReader : TextReader
        {
            private bool eof;
            private bool hasComments;
            private TextReader reader;

            internal JsonStreamReader(TextReader reader)
            {
                if (reader == null)
                {
                    throw new ArgumentNullException("reader");
                }

                if (reader.Peek() != -1 && ((char)reader.Peek()) == '/')
                {
                    char[] prefix = new char[3];
                    reader.Read(prefix, 0, 3);
                    if (new string(prefix) != "/* ")
                    {
                        throw new ArgumentException("JSON stream doesn't start with '/* ', it starts with '" + new string(prefix) + "'");
                    }

                    this.hasComments = true;
                }
                else
                {
                    this.hasComments = false;
                }

                this.reader = reader;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing && this.reader != null)
                {
                    this.reader.Dispose();
                    this.reader = null;
                }
            }

            public override int Peek()
            {
                if (!this.hasComments) return this.reader.Peek();

                if (this.eof)
                {
                    return -1;
                }

                int result = this.reader.Peek();
                if (result != -1)
                {
                    char c = (char)result;
                    if (c == '*')
                    {
                        this.eof = true;
                        return -1;
                    }
                }

                return result;
            }

            public override int Read()
            {
                if (!this.hasComments) return this.reader.Read();

                if (this.eof) return -1;

                int result = this.reader.Read();
                if (result != -1)
                {
                    char c = (char)result;
                    if (c == '*')
                    {
                        this.eof = true;
                        return -1;
                    }
                }

                return result;
            }
        }
    }
}
