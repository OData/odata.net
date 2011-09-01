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

#if SPATIAL
namespace System.Data.Spatial.Json
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
        internal static void WriteValue(TextWriter writer, double value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                WriteQuoted(writer, value.ToString(null, CultureInfo.InvariantCulture));
            }
            else
            {
                // double.ToString() supports a max scale of 14,
                // whereas float.MinValue and float.MaxValue have 16 digits scale. Hence we need
                // to use XmlConvert in all other cases, except infinity
                writer.Write(XmlConvert.ToString(value));
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
#if SPATIAL
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Spatial.Json.JsonValueUtils.WriteQuoted(System.IO.TextWriter,System.String)", Justification = "Constant defined by the JSON spec.")]
#else
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Data.OData.Json.JsonValueUtils.WriteQuoted(System.IO.TextWriter,System.String)", Justification = "Constant defined by the JSON spec.")]
#endif
        internal static void WriteValue(TextWriter writer, DateTime value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            // taken from the Atlas serializer
            // DevDiv 41127: Never confuse atlas serialized strings with dates
            // Serialized date: "\/Date(123)\/"
            // sb.Append(@"""\/Date(");
            // sb.Append((datetime.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000);
            // sb.Append(@")\/""");
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

            System.Diagnostics.Debug.Assert(value.Kind == DateTimeKind.Utc, "dateTime.Kind == DateTimeKind.Utc");
            string textValue = String.Format(
                CultureInfo.InvariantCulture,
                JsonConstants.ODataDateTimeFormat,
                DateTimeTicksToJsonTicks(value.Ticks));
            WriteQuoted(writer, textValue);
        }

        /// <summary>
        /// Write a DateTimeOffset value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">DateTimeOffset value to be written.</param>
#if SPATIAL
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Spatial.Json.JsonValueUtils.WriteQuoted(System.IO.TextWriter,System.String)", Justification = "Constant defined by the JSON spec.")]
#else
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Data.OData.Json.JsonValueUtils.WriteQuoted(System.IO.TextWriter,System.String)", Justification = "Constant defined by the JSON spec.")]
#endif
        internal static void WriteValue(TextWriter writer, DateTimeOffset value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

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
            Int32 offsetMinutes = (Int32)value.Offset.TotalMinutes;
            string textValue = String.Format(
                CultureInfo.InvariantCulture,
                JsonConstants.ODataDateTimeOffsetFormat,
                DateTimeTicksToJsonTicks(value.UtcTicks),
                offsetMinutes >= 0 ? JsonConstants.ODataDateTimeOffsetPlusSign : string.Empty,
                offsetMinutes);
            WriteQuoted(writer, textValue);
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
            int traversedCharacterCount = 0;
            for (int i = 0; i < inputString.Length; i++)
            {
                char c = inputString[i];

                // Append the unhandled characters (that do not require special treament)
                // to the string builder when special characters are detected.
                if (c == '\r' || c == '\t' || c == '\"' ||
                    c == '\\' || c == '\n' || c < ' ' || c > 0x7F || c == '\b' || c == '\f')
                {
                    // Flush out the unescaped characters we've built so far.
                    writer.Write(inputString.Substring(startIndex, traversedCharacterCount));
                    startIndex = i + 1;
                    traversedCharacterCount = 0;
                }
                else
                {
                    traversedCharacterCount++;
                    continue;
                }

                switch (c)
                {
                    case '\r':
                        writer.Write("\\r");
                        break;
                    case '\t':
                        writer.Write("\\t");
                        break;
                    case '\"':
                        writer.Write("\\\"");
                        break;
                    case '\\':
                        writer.Write("\\\\");
                        break;
                    case '\n':
                        writer.Write("\\n");
                        break;
                    case '\b':
                        writer.Write("\\b");
                        break;
                    case '\f':
                        writer.Write("\\f");
                        break;
                    default:
                        Debug.Assert((c < ' ') || (c > 0x7F), "Unexpected character.");
                        string value = string.Format(CultureInfo.InvariantCulture, "\\u{0:x4}", (int)c);
                        writer.Write(value);
                        break;
                }
            }

            if (traversedCharacterCount > 0)
            {
                writer.Write(inputString.Substring(startIndex, traversedCharacterCount));
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
    }
}
