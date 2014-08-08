//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

#if SPATIAL
namespace Microsoft.Data.Spatial
#else
namespace Microsoft.Data.OData.Json
#endif
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    #endregion Namespaces

    /// <summary>
    /// Provides helper method for converting data values to and from the OData JSON format.
    /// </summary>
    internal static class JsonValueUtils
    {
        /// <summary>
        /// Const tick value for caculating tick values.
        /// </summary>
        private static readonly long JsonDateTimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        /// <summary>
        /// Characters which, if found inside a number, indicate that the number is a double when no other type information is available.
        /// </summary>
        private static readonly char[] DoubleIndicatingCharacters = new char[] { '.', 'e', 'E' };

        /// <summary>
        /// Map of special characters to strings.
        /// </summary>
        private static readonly string[] SpecialCharToEscapedStringMap = CreateSpecialCharToEscapedStringMap();

        /// <summary>
        /// Write a boolean value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">The boolean value to write.</param>
        internal static void WriteValue(TextWriter writer, bool value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            writer.Write(value ? JsonConstants.JsonTrueLiteral : JsonConstants.JsonFalseLiteral);
        }

        /// <summary>
        /// Write an integer value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Integer value to be written.</param>
        internal static void WriteValue(TextWriter writer, int value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            writer.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a float value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Float value to be written.</param>
        internal static void WriteValue(TextWriter writer, float value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            if (float.IsInfinity(value) || float.IsNaN(value))
            {
                WriteQuoted(writer, value.ToString(null, CultureInfo.InvariantCulture));
            }
            else
            {
                // float.ToString() supports a max scale of six,
                // whereas float.MinValue and float.MaxValue have 8 digits scale. Hence we need
                // to use XmlConvert in all other cases, except infinity
                writer.Write(XmlConvert.ToString(value));
            }
        }

        /// <summary>
        /// Write a short value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Short value to be written.</param>
        internal static void WriteValue(TextWriter writer, short value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            writer.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a long value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Long value to be written.</param>
        internal static void WriteValue(TextWriter writer, long value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            // Since Json only supports number, we need to convert long into string to prevent data loss
            WriteQuoted(writer, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a double value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Double value to be written.</param>
        /// <param name="mustIncludeDecimalPoint">If true, all double values will be written so that they either have an 'E' for scientific notation or contain a decimal point.</param>
        internal static void WriteValue(TextWriter writer, double value, bool mustIncludeDecimalPoint)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            if (JsonSharedUtils.IsDoubleValueSerializedAsString(value))
            {
                WriteQuoted(writer, value.ToString(null, CultureInfo.InvariantCulture));
            }
            else
            {
                // double.ToString() supports a max scale of 14,
                // whereas double.MinValue and double.MaxValue have 16 digits scale. Hence we need
                // to use XmlConvert in all other cases, except infinity
                string valueToWrite = XmlConvert.ToString(value);

                writer.Write(valueToWrite);
                if (mustIncludeDecimalPoint && valueToWrite.IndexOfAny(DoubleIndicatingCharacters) < 0)
                {
                    writer.Write(".0");
                }
            }
        }

        /// <summary>
        /// Write a Guid value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Guid value to be written.</param>
        internal static void WriteValue(TextWriter writer, Guid value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            WriteQuoted(writer, value.ToString());
        }

        /// <summary>
        /// Write a decimal value
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Decimal value to be written.</param>
        internal static void WriteValue(TextWriter writer, decimal value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            // Since Json doesn't have decimal support (it only has one data type - number),
            // we need to convert decimal to string to prevent data loss
            WriteQuoted(writer, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a DateTime value
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">DateTime value to be written.</param>
        /// <param name="dateTimeFormat">The format to write out the DateTime value in.</param>
#if SPATIAL
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Data.Spatial.JsonValueUtils.WriteQuoted(System.IO.TextWriter,System.String)", Justification = "Constant defined by the JSON spec.")]
#else
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Data.OData.Json.JsonValueUtils.WriteQuoted(System.IO.TextWriter,System.String)", Justification = "Constant defined by the JSON spec.")]
#endif
        internal static void WriteValue(TextWriter writer, DateTime value, ODataJsonDateTimeFormat dateTimeFormat)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            switch (dateTimeFormat)
            {
                case ODataJsonDateTimeFormat.ISO8601DateTime:
                    {
                        // jsonDateTime= quotation-mark   
                        //  YYYY-MM-DDThh:mm:ss.sTZD 
                        //  [("+" / "-") offset] 
                        //  quotation-mark  
                        string textValue = PlatformHelper.ConvertDateTimeToString(value);
                        WriteQuoted(writer, textValue);
                    }

                    break;

                case ODataJsonDateTimeFormat.ODataDateTime:
                    {
                        // taken from the Atlas serializer
                        // DevDiv 41127: Never confuse atlas serialized strings with dates
                        // Serialized date: "\/Date(123)\/"
                        // sb.Append(@"""\/Date(");
                        // sb.Append((datetime.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000);
                        // sb.Append(@")\/""");
                        value = GetUniversalDate(value);
                        Debug.Assert(value.Kind == DateTimeKind.Utc, "dateTime.Kind == DateTimeKind.Utc");

                        string textValue = String.Format(
                            CultureInfo.InvariantCulture,
                            JsonConstants.ODataDateTimeFormat,
                            DateTimeTicksToJsonTicks(value.Ticks));
                        WriteQuoted(writer, textValue);
                    }

                    break;
            }
        }

        /// <summary>
        /// Write a DateTimeOffset value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">DateTimeOffset value to be written.</param>
        /// <param name="dateTimeFormat">The format to write out the DateTime value in.</param>
#if SPATIAL
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Data.Spatial.JsonValueUtils.WriteQuoted(System.IO.TextWriter,System.String)", Justification = "Constant defined by the JSON spec.")]
#else
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Data.OData.Json.JsonValueUtils.WriteQuoted(System.IO.TextWriter,System.String)", Justification = "Constant defined by the JSON spec.")]
#endif
        internal static void WriteValue(TextWriter writer, DateTimeOffset value, ODataJsonDateTimeFormat dateTimeFormat)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            Int32 offsetMinutes = (Int32)value.Offset.TotalMinutes;

            switch (dateTimeFormat)
            {
                case ODataJsonDateTimeFormat.ISO8601DateTime:
                    {
                        // Uses the same format as DateTime but with offset:
                        // jsonDateTime= quotation-mark   
                        //  YYYY-MM-DDThh:mm:ss.sTZD 
                        //  [("+" / "-") offset] 
                        //  quotation-mark  
                        //
                        // offset = 4DIGIT
                        string textValue = XmlConvert.ToString(value);
                        WriteQuoted(writer, textValue);
                    }

                    break;

                case ODataJsonDateTimeFormat.ODataDateTime:
                    {
                        // Uses the same format as DateTime but with offset:
                        // jsonDateTime= quotation-mark   
                        //  "\/Date("  
                        //  ticks 
                        //  [("+" / "-") offset] 
                        //  ")\/"  
                        //  quotation-mark  
                        //
                        // ticks = *DIGIT
                        // offset = 4DIGIT
                        string textValue = String.Format(
                            CultureInfo.InvariantCulture,
                            JsonConstants.ODataDateTimeOffsetFormat,
                            DateTimeTicksToJsonTicks(value.Ticks),
                            offsetMinutes >= 0 ? JsonConstants.ODataDateTimeOffsetPlusSign : string.Empty,
                            offsetMinutes);
                        WriteQuoted(writer, textValue);
                    }

                    break;
            }
        }

        /// <summary>
        /// Write a TimeSpan value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">TimeSpan value to be written.</param>
        internal static void WriteValue(TextWriter writer, TimeSpan value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            WriteQuoted(writer, XmlConvert.ToString(value));
        }

        /// <summary>
        /// Write a byte value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Byte value to be written.</param>
        internal static void WriteValue(TextWriter writer, byte value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            writer.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write an sbyte value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">SByte value to be written.</param>
        internal static void WriteValue(TextWriter writer, sbyte value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            writer.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a string value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">String value to be written.</param>
        internal static void WriteValue(TextWriter writer, string value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            if (value == null)
            {
                writer.Write(JsonConstants.JsonNullLiteral);
            }
            else
            {
                WriteEscapedJsonString(writer, value);
            }
        }

        /// <summary>
        /// Returns the string value with special characters escaped.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="inputString">Input string value.</param>
        internal static void WriteEscapedJsonString(TextWriter writer, string inputString)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(inputString != null, "The string value must not be null.");

            writer.Write(JsonConstants.QuoteCharacter);

            int startIndex = 0;
            int inputStringLength = inputString.Length;
            int subStrLength;
            for (int currentIndex = 0; currentIndex < inputStringLength; currentIndex++)
            {
                char c = inputString[currentIndex];

                // Append the unhandled characters (that do not require special treament)
                // to the string builder when special characters are detected.
                string value;
                switch (c)
                {
                    case '\r':
                        value = "\\r";
                        break;
                    case '\t':
                        value = "\\t";
                        break;
                    case '\"':
                        value = "\\\"";
                        break;
                    case '\\':
                        value = "\\\\";
                        break;
                    case '\n':
                        value = "\\n";
                        break;
                    case '\b':
                        value = "\\b";
                        break;
                    case '\f':
                        value = "\\f";
                        break;
                    default:
                        if ((c < ' ') || (c > 0x7F))
                        {
                            value = SpecialCharToEscapedStringMap[c];
                        }
                        else
                        {
                            continue;
                        }

                        break;
                }

                // Flush out the unescaped characters we've built so far.
                subStrLength = currentIndex - startIndex;
                if (subStrLength > 0)
                {
                    writer.Write(inputString.Substring(startIndex, subStrLength));
                }

                writer.Write(value);
                startIndex = currentIndex + 1;
            }

            subStrLength = inputStringLength - startIndex;
            if (subStrLength > 0)
            {
                writer.Write(inputString.Substring(startIndex, subStrLength));
            }

            writer.Write(JsonConstants.QuoteCharacter);
        }

        /// <summary>
        /// Converts the number of ticks from the JSON date time format to the one used in .NET DateTime or DateTimeOffset structure.
        /// </summary>
        /// <param name="ticks">The ticks to from the JSON date time format.</param>
        /// <returns>The ticks to use in the .NET DateTime of DateTimeOffset structure.</returns>
        internal static long JsonTicksToDateTimeTicks(long ticks)
        {
            DebugUtils.CheckNoExternalCallers();

            // Ticks in .NET are in 100-nanoseconds and start at 1.1.0001.
            // Ticks in the JSON date time format are in milliseconds and start at 1.1.1970.
            return (ticks * 10000) + JsonDateTimeMinTimeTicks;
        }

        /// <summary>
        /// Write the string value with quotes.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="text">String value to be written.</param>
        private static void WriteQuoted(TextWriter writer, string text)
        {
            writer.Write(JsonConstants.QuoteCharacter);
            writer.Write(text);
            writer.Write(JsonConstants.QuoteCharacter);
        }

        /// <summary>
        /// Converts the number of ticks from the .NET DateTime or DateTimeOffset structure to the ticks use in the JSON date time format.
        /// </summary>
        /// <param name="ticks">The ticks from the .NET DateTime of DateTimeOffset structure.</param>
        /// <returns>The ticks to use in the JSON date time format.</returns>
        private static long DateTimeTicksToJsonTicks(long ticks)
        {
            // Ticks in .NET are in 100-nanoseconds and start at 1.1.0001.
            // Ticks in the JSON date time format are in milliseconds and start at 1.1.1970.
            return (ticks - JsonDateTimeMinTimeTicks) / 10000;
        }

        /// <summary>
        /// Converts a given date time to its universal date time equivalent.
        /// </summary>
        /// <param name="value">The date time to convert to UTC</param>
        /// <returns>universal date time equivalent of the value.</returns>
        private static DateTime GetUniversalDate(DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Local:
                    value = value.ToUniversalTime();
                    break;
                case DateTimeKind.Unspecified:
                    value = new DateTime(value.Ticks, DateTimeKind.Utc);
                    break;
                case DateTimeKind.Utc:
                    break;
            }

            return value;
        }

        /// <summary>
        /// Creates the special character to escaped string map.
        /// </summary>
        /// <returns>The map of special characters to the corresponding escaped strings.</returns>
        private static string[] CreateSpecialCharToEscapedStringMap()
        {
            string[] specialCharToEscapedStringMap = new string[char.MaxValue + 1];
            for (int c = char.MinValue; c <= char.MaxValue; c++)
            {
                // We only need to populate for characters < ' ' and > 0x7F. For simplicity sake, we populate for all char values here.
                specialCharToEscapedStringMap[c] = string.Format(CultureInfo.InvariantCulture, "\\u{0:x4}", c);
            }

            return specialCharToEscapedStringMap;
        }
    }
}
