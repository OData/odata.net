//---------------------------------------------------------------------
// <copyright file="JsonValueUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;
    using Microsoft.OData.Buffers;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Provides helper method for converting data values to and from the OData JSON format.
    /// </summary>
    internal static partial class JsonValueUtils
    {
        /// <summary>
        /// PositiveInfinitySymbol used in OData Json format
        /// </summary>
        internal const string ODataJsonPositiveInfinitySymbol = "INF";

        /// <summary>
        /// NegativeInfinitySymbol used in OData Json format
        /// </summary>
        internal const string ODataJsonNegativeInfinitySymbol = "-INF";

        /// <summary>
        /// The NumberFormatInfo used in OData Json format.
        /// </summary>
        internal static readonly NumberFormatInfo ODataNumberFormatInfo = JsonValueUtils.InitializeODataNumberFormatInfo();

        /// <summary>
        /// Const tick value for calculating tick values.
        /// </summary>
        private static readonly long JsonDateTimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        /// <summary>
        /// Characters which, if found inside a number, indicate that the number is a double when no other type information is available.
        /// </summary>
        private static readonly char[] DoubleIndicatingCharacters = new char[] { '.', 'e', 'E' };

        /// <summary>
        /// Map of special characters to strings.
        /// </summary>
        private static readonly string[] SpecialCharToEscapedStringMap = JsonValueUtils.CreateSpecialCharToEscapedStringMap();

        /// <summary>
        /// Write a char value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">The char value to write.</param>
        /// <param name="stringEscapeOption">The ODataStringEscapeOption to use in escaping the string.</param>
        internal static void WriteValue(TextWriter writer, char value, ODataStringEscapeOption stringEscapeOption)
        {
            Debug.Assert(writer != null, "writer != null");

            if (stringEscapeOption == ODataStringEscapeOption.EscapeNonAscii || value <= 0x7F)
            {
                string escapedString = JsonValueUtils.SpecialCharToEscapedStringMap[value];
                if (escapedString != null)
                {
                    writer.Write(escapedString);
                    return;
                }
            }

            writer.Write(value);
        }

        /// <summary>
        /// Write a boolean value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">The boolean value to write.</param>
        internal static void WriteValue(TextWriter writer, bool value)
        {
            Debug.Assert(writer != null, "writer != null");

            writer.Write(FormatAsBooleanLiteral(value));
        }

        /// <summary>
        /// Write an integer value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Integer value to be written.</param>
        internal static void WriteValue(TextWriter writer, int value)
        {
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
            Debug.Assert(writer != null, "writer != null");

            if (JsonSharedUtils.IsFloatValueSerializedAsString(value))
            {
                JsonValueUtils.WriteQuoted(writer, value.ToString(JsonValueUtils.ODataNumberFormatInfo));
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
            Debug.Assert(writer != null, "writer != null");

            writer.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a double value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Double value to be written.</param>
        internal static void WriteValue(TextWriter writer, double value)
        {
            Debug.Assert(writer != null, "writer != null");

            if (JsonSharedUtils.IsDoubleValueSerializedAsString(value))
            {
                JsonValueUtils.WriteQuoted(writer, value.ToString(JsonValueUtils.ODataNumberFormatInfo));
            }
            else
            {
                // double.ToString() supports a max scale of 14,
                // whereas double.MinValue and double.MaxValue have 16 digits scale. Hence we need
                // to use XmlConvert in all other cases, except infinity
                string valueToWrite = XmlConvert.ToString(value);

                writer.Write(valueToWrite);
                if (valueToWrite.IndexOfAny(JsonValueUtils.DoubleIndicatingCharacters) < 0)
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
            Debug.Assert(writer != null, "writer != null");

            JsonValueUtils.WriteQuoted(writer, value.ToString());
        }

        /// <summary>
        /// Write a decimal value
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Decimal value to be written.</param>
        internal static void WriteValue(TextWriter writer, decimal value)
        {
            Debug.Assert(writer != null, "writer != null");

            writer.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a DateTimeOffset value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">DateTimeOffset value to be written.</param>
        /// <param name="dateTimeFormat">The format to write out the DateTime value in.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.Json.JsonValueUtils.WriteQuoted(System.IO.TextWriter,System.String)", Justification = "Constant defined by the JSON spec.")]
        internal static void WriteValue(TextWriter writer, DateTimeOffset value, ODataJsonDateTimeFormat dateTimeFormat)
        {
            Debug.Assert(writer != null, "writer != null");

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
                        JsonValueUtils.WriteQuoted(writer, textValue);
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
                        string textValue = FormatDateTimeAsJsonTicksString(value);
                        JsonValueUtils.WriteQuoted(writer, textValue);
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
            Debug.Assert(writer != null, "writer != null");

            JsonValueUtils.WriteQuoted(writer, EdmValueWriter.DurationAsXml(value));
        }

        /// <summary>
        /// Write a TimeOfDay value
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">TimeOfDay value to be written.</param>
        internal static void WriteValue(TextWriter writer, TimeOfDay value)
        {
            Debug.Assert(writer != null, "writer != null");

            JsonValueUtils.WriteQuoted(writer, value.ToString());
        }

        /// <summary>
        /// Write a Date value
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Date value to be written.</param>
        internal static void WriteValue(TextWriter writer, Date value)
        {
            Debug.Assert(writer != null, "writer != null");

            JsonValueUtils.WriteQuoted(writer, value.ToString());
        }

        /// <summary>
        /// Write a byte value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Byte value to be written.</param>
        internal static void WriteValue(TextWriter writer, byte value)
        {
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
            Debug.Assert(writer != null, "writer != null");

            writer.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a string value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">String value to be written.</param>
        /// <param name="stringEscapeOption">The string escape option.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="arrayPool">Array pool for renting a buffer.</param>
        internal static void WriteValue(
            TextWriter writer,
            string value,
            ODataStringEscapeOption stringEscapeOption,
            Ref<char[]> buffer,
            ICharArrayPool arrayPool = null)
        {
            Debug.Assert(writer != null, "writer != null");

            if (value == null)
            {
                writer.Write(JsonConstants.JsonNullLiteral);
            }
            else
            {
                JsonValueUtils.WriteEscapedJsonString(writer, value, stringEscapeOption, buffer, arrayPool);
            }
        }

        /// <summary>
        /// Write a byte array.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Byte array to be written.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="arrayPool">Array pool for renting a buffer.</param>
        internal static void WriteValue(TextWriter writer, byte[] value, Ref<char[]> buffer, ICharArrayPool arrayPool = null)
        {
            Debug.Assert(writer != null, "writer != null");

            if (value == null)
            {
                writer.Write(JsonConstants.JsonNullLiteral);
            }
            else
            {
                writer.Write(JsonConstants.QuoteCharacter);
                WriteBinaryString(writer, value, buffer, arrayPool);
                writer.Write(JsonConstants.QuoteCharacter);
            }
        }

        /// <summary>
        /// Write a byte array.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Byte array to be written.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="arrayPool">Array pool for renting a buffer.</param>
        internal static void WriteBinaryString(TextWriter writer, byte[] value, Ref<char[]> buffer, ICharArrayPool arrayPool)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(value != null, "The value must not be null.");

            buffer.Value = BufferUtils.InitializeBufferIfRequired(arrayPool, buffer.Value);
            Debug.Assert(buffer != null);

            int bufferLength = buffer.Value.Length;

            // Try to hold base64 string as much as possible in one converting.
            int bufferByteSize = bufferLength * 3 / 4;

            for (int offsetIn = 0; offsetIn < value.Length; offsetIn += bufferByteSize)
            {
                int count = WriteByteArrayToBuffer(value, offsetIn, buffer.Value, bufferByteSize);
                writer.Write(buffer.Value, 0, count);
            }
        }

        /// <summary>
        /// Returns the string value with special characters escaped.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="inputString">Input string value.</param>
        /// <param name="stringEscapeOption">The string escape option.</param>
        /// <param name="buffer">Char buffer to use for streaming data</param>
        /// <param name="bufferPool">Array pool for renting a buffer.</param>
        internal static void WriteEscapedJsonString(
            TextWriter writer,
            string inputString,
            ODataStringEscapeOption stringEscapeOption,
            Ref<char[]> buffer,
            ICharArrayPool bufferPool = null)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(inputString != null, "The string value must not be null.");

            writer.Write(JsonConstants.QuoteCharacter);
            WriteEscapedJsonStringValue(writer, inputString, stringEscapeOption, buffer, bufferPool);
            writer.Write(JsonConstants.QuoteCharacter);
        }

        /// <summary>
        /// Writes the string value with special characters escaped.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="inputString">Input string value.</param>
        /// <param name="stringEscapeOption">The string escape option.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="bufferPool">Array pool for renting a buffer.</param>
        internal static void WriteEscapedJsonStringValue(
            TextWriter writer,
            string inputString,
            ODataStringEscapeOption stringEscapeOption,
            Ref<char[]> buffer,
            ICharArrayPool bufferPool)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(inputString != null, "The string value must not be null.");

            int firstIndex;
            if (!JsonValueUtils.CheckIfStringHasSpecialChars(inputString, stringEscapeOption, out firstIndex))
            {
                writer.Write(inputString);
            }
            else
            {
                int inputStringLength = inputString.Length;

                Debug.Assert(firstIndex < inputStringLength, "First index of the special character should be within the string");
                buffer.Value = BufferUtils.InitializeBufferIfRequired(bufferPool, buffer.Value);
                int bufferLength = buffer.Value.Length;
                int bufferIndex = 0;
                int currentIndex = 0;

                // Let's copy and flush strings up to the first index of the special char
                while (currentIndex < firstIndex)
                {
                    int subStrLength = firstIndex - currentIndex;

                    Debug.Assert(subStrLength > 0, "SubStrLength should be greater than 0 always");

                    // If the first index of the special character is larger than the buffer length,
                    // flush everything to the buffer first and reset the buffer to the next chunk.
                    // Otherwise copy to the buffer and go on from there.
                    if (subStrLength >= bufferLength)
                    {
                        inputString.CopyTo(currentIndex, buffer.Value, 0, bufferLength);
                        writer.Write(buffer.Value, 0, bufferLength);
                        currentIndex += bufferLength;
                    }
                    else
                    {
                        WriteSubstringToBuffer(inputString, ref currentIndex, buffer.Value, ref bufferIndex, subStrLength);
                    }
                }

                // start writing escaped strings
                WriteEscapedStringToBuffer(writer, inputString, ref currentIndex, buffer.Value, ref bufferIndex, stringEscapeOption);

                // write any remaining chars to the writer
                if (bufferIndex > 0)
                {
                    writer.Write(buffer.Value, 0, bufferIndex);
                }
            }
        }

        /// <summary>
        /// Escapes and writes a character array to a writer.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="inputArray">Character array to write.</param>
        /// <param name="inputArrayOffset">How many characters to skip in the input array.</param>
        /// <param name="inputArrayCount">How many characters to write from the input array.</param>
        /// <param name="stringEscapeOption">The string escape option.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="bufferPool">Character buffer pool.</param>
        internal static void WriteEscapedCharArray(
            TextWriter writer,
            char[] inputArray,
            int inputArrayOffset,
            int inputArrayCount,
            ODataStringEscapeOption stringEscapeOption,
            Ref<char[]> buffer, ICharArrayPool bufferPool)
        {
            int bufferIndex = 0;
            buffer.Value = BufferUtils.InitializeBufferIfRequired(bufferPool, buffer.Value);

            WriteEscapedCharArrayToBuffer(writer, inputArray, ref inputArrayOffset, inputArrayCount, buffer.Value, ref bufferIndex, stringEscapeOption);

            // write remaining bytes in buffer
            if (bufferIndex > 0)
            {
                writer.Write(buffer.Value, 0, bufferIndex);
            }
        }

        /// <summary>
        /// Converts the number of ticks from the JSON date time format to the one used in .NET DateTime or DateTimeOffset structure.
        /// </summary>
        /// <param name="ticks">The ticks to from the JSON date time format.</param>
        /// <returns>The ticks to use in the .NET DateTime of DateTimeOffset structure.</returns>
        internal static long JsonTicksToDateTimeTicks(long ticks)
        {
            // Ticks in .NET are in 100-nanoseconds and start at 1.1.0001.
            // Ticks in the JSON date time format are in milliseconds and start at 1.1.1970.
            return (ticks * 10000) + JsonValueUtils.JsonDateTimeMinTimeTicks;
        }

        /// <summary>
        /// Convert string to Json-formatted string with proper escaped special characters.
        /// Note that the return value is not enclosed by the top level double-quotes.
        /// </summary>
        /// <param name="inputString">string that might contain special characters.</param>
        /// <returns>A string with special characters escaped properly.</returns>
        internal static string GetEscapedJsonString(string inputString)
        {
            Debug.Assert(inputString != null, "The string value must not be null.");

            StringBuilder builder = new StringBuilder();
            int startIndex = 0;
            int inputStringLength = inputString.Length;
            int subStrLength;
            for (int currentIndex = 0; currentIndex < inputStringLength; currentIndex++)
            {
                char c = inputString[currentIndex];

                // Append the un-handled characters (that do not require special treatment)
                // to the string builder when special characters are detected.
                if (JsonValueUtils.SpecialCharToEscapedStringMap[c] == null)
                {
                    continue;
                }

                // Flush out the un-escaped characters we've built so far.
                subStrLength = currentIndex - startIndex;
                if (subStrLength > 0)
                {
                    builder.Append(inputString.Substring(startIndex, subStrLength));
                }

                builder.Append(JsonValueUtils.SpecialCharToEscapedStringMap[c]);
                startIndex = currentIndex + 1;
            }

            subStrLength = inputStringLength - startIndex;
            if (subStrLength > 0)
            {
                builder.Append(inputString.Substring(startIndex, subStrLength));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Escapes and writes a character buffer, flushing to the writer as the buffer fills.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="character">The character to write to the buffer.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="bufferIndex">The index into the buffer in which to write the character.</param>
        /// <param name="stringEscapeOption">The string escape option.</param>
        /// <returns>Current position in the buffer after the character has been written.</returns>
        /// <remarks>
        /// IMPORTANT: After all characters have been written,
        /// caller is responsible for writing the final buffer contents to the writer.
        /// </remarks>
        private static int EscapeAndWriteCharToBuffer(TextWriter writer, char character, char[] buffer, int bufferIndex, ODataStringEscapeOption stringEscapeOption)
        {
            int bufferLength = buffer.Length;
            string escapedString = null;

            if (stringEscapeOption == ODataStringEscapeOption.EscapeNonAscii || character <= 0x7F)
            {
                escapedString = JsonValueUtils.SpecialCharToEscapedStringMap[character];
            }

            // Append the unhandled characters (that do not require special treatment)
            // to the buffer.
            if (escapedString == null)
            {
                buffer[bufferIndex] = character;
                bufferIndex++;
            }
            else
            {
                // Okay, an unhandled character was detected.
                // First lets check if we can fit it in the existing buffer, if not,
                // flush the current buffer and reset. Add the escaped string to the buffer
                // and continue.
                int escapedStringLength = escapedString.Length;
                Debug.Assert(escapedStringLength <= bufferLength, "Buffer should be larger than the escaped string");

                if ((bufferIndex + escapedStringLength) > bufferLength)
                {
                    writer.Write(buffer, 0, bufferIndex);
                    bufferIndex = 0;
                }

                escapedString.CopyTo(0, buffer, bufferIndex, escapedStringLength);
                bufferIndex += escapedStringLength;
            }

            if (bufferIndex >= bufferLength)
            {
                Debug.Assert(bufferIndex == bufferLength,
                    "We should never encounter a situation where the buffer index is greater than the buffer length");
                writer.Write(buffer, 0, bufferIndex);
                bufferIndex = 0;
            }

            return bufferIndex;
        }

        /// <summary>
        /// Checks if the string contains special char and returns the first index
        /// of special char if present.
        /// </summary>
        /// <param name="inputString">string that might contain special characters.</param>
        /// <param name="stringEscapeOption">The string escape option.</param>
        /// <param name="firstIndex">first index of the special char</param>
        /// <returns>A value indicating whether the string contains special character</returns>
        private static bool CheckIfStringHasSpecialChars(string inputString, ODataStringEscapeOption stringEscapeOption, out int firstIndex)
        {
            Debug.Assert(inputString != null, "The string value must not be null.");

            firstIndex = -1;
            int inputStringLength = inputString.Length;
            for (int currentIndex = 0; currentIndex < inputStringLength; currentIndex++)
            {
                char c = inputString[currentIndex];

                if (stringEscapeOption == ODataStringEscapeOption.EscapeOnlyControls && c >= 0x7F)
                {
                    continue;
                }

                // Append the un-handled characters (that do not require special treatment)
                // to the string builder when special characters are detected.
                if (JsonValueUtils.SpecialCharToEscapedStringMap[c] != null)
                {
                    firstIndex = currentIndex;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Initialize static property ODataNumberFormatInfo.
        /// </summary>
        /// <returns>The <see cref=" NumberFormatInfo"/> object.</returns>
        private static NumberFormatInfo InitializeODataNumberFormatInfo()
        {
            NumberFormatInfo odataNumberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            odataNumberFormatInfo.PositiveInfinitySymbol = JsonValueUtils.ODataJsonPositiveInfinitySymbol;
            odataNumberFormatInfo.NegativeInfinitySymbol = JsonValueUtils.ODataJsonNegativeInfinitySymbol;
            return odataNumberFormatInfo;
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
            return (ticks - JsonValueUtils.JsonDateTimeMinTimeTicks) / 10000;
        }

        /// <summary>
        /// Creates the special character to escaped string map.
        /// </summary>
        /// <returns>The map of special characters to the corresponding escaped strings.</returns>
        private static string[] CreateSpecialCharToEscapedStringMap()
        {
            string[] specialCharToEscapedStringMap = new string[char.MaxValue + 1];
            for (int c = char.MinValue; c <= char.MaxValue; ++c)
            {
                if ((c < ' ') || (c > 0x7F))
                {
                    // We only need to populate for characters < ' ' and > 0x7F.
                    specialCharToEscapedStringMap[c] = string.Format(CultureInfo.InvariantCulture, "\\u{0:x4}", c);
                }
                else
                {
                    specialCharToEscapedStringMap[c] = null;
                }
            }

            specialCharToEscapedStringMap['\r'] = "\\r";
            specialCharToEscapedStringMap['\t'] = "\\t";
            specialCharToEscapedStringMap['\"'] = "\\\"";
            specialCharToEscapedStringMap['\\'] = "\\\\";
            specialCharToEscapedStringMap['\n'] = "\\n";
            specialCharToEscapedStringMap['\b'] = "\\b";
            specialCharToEscapedStringMap['\f'] = "\\f";

            return specialCharToEscapedStringMap;
        }

        /// <summary>
        /// Formats a DateTimeOffset value into JSON "\/Date(number of ticks)\/" format.
        /// </summary>
        /// <param name="value">DateTimeOffset value to be formatted.</param>
        /// <returns>The string representation of the DateTimeOffset value in JSON "\/Date(number of ticks)\/" format.</returns>
        private static string FormatDateTimeAsJsonTicksString(DateTimeOffset value)
        {
            Int32 offsetMinutes = (Int32)value.Offset.TotalMinutes;

            return String.Format(
                CultureInfo.InvariantCulture,
                JsonConstants.ODataDateTimeOffsetFormat,
                DateTimeTicksToJsonTicks(value.Ticks),
                offsetMinutes >= 0 ? JsonConstants.ODataDateTimeOffsetPlusSign : string.Empty,
                offsetMinutes);
        }

        /// <summary>
        /// Formats a boolean value into its equivalent string representation in JSON.
        /// </summary>
        /// <param name="value">Boolean value to be formatted.</param>
        /// <returns>The string representation of the boolean value.</returns>
        private static string FormatAsBooleanLiteral(bool value)
        {
            return value ? JsonConstants.JsonTrueLiteral : JsonConstants.JsonFalseLiteral;
        }

        /// <summary>
        /// Writes the byte array to the buffer.
        /// </summary>
        /// <param name="value">Byte array to be written.</param>
        /// <param name="offsetIn">A position in the byte array.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="bufferByteSize">Desired buffer byte size.</param>
        /// <returns>A count of the bytes written to the buffer.</returns>
        private static int WriteByteArrayToBuffer(byte[] value, int offsetIn, char[] buffer, int bufferByteSize)
        {
            Debug.Assert(value != null, "value != null");
            Debug.Assert(buffer != null, "buffer != null");

            if (offsetIn + bufferByteSize > value.Length)
            {
                bufferByteSize = value.Length - offsetIn;
            }

            return Convert.ToBase64CharArray(value, offsetIn, bufferByteSize, buffer, 0);
        }

        /// <summary>
        /// Writes a substring starting at a specified position on the string to the buffer.
        /// </summary>
        /// <param name="inputString">Input string value.</param>
        /// <param name="currentIndex">The index in the string at which the substring begins.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="bufferIndex">Current position in the buffer after the substring has been written.</param>
        /// <param name="substrLength">The length of the substring to be copied.</param>
        private static void WriteSubstringToBuffer(string inputString, ref int currentIndex, char[] buffer, ref int bufferIndex, int substrLength)
        {
            Debug.Assert(inputString != null, "inputString != null");
            Debug.Assert(buffer != null, "buffer != null");

            inputString.CopyTo(currentIndex, buffer, 0, substrLength);
            bufferIndex = substrLength;
            currentIndex += substrLength;
        }

        /// <summary>
        /// Writes an escaped string to the buffer.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="inputString">Input string value.</param>
        /// <param name="currentIndex">The index in the string at which copying should begin.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="bufferIndex">Current position in the buffer after the string has been written.</param>
        /// <param name="stringEscapeOption">The string escape option.</param>
        /// <remarks>
        /// IMPORTANT: After all characters have been written,
        /// caller is responsible for writing the final buffer contents to the writer.
        /// </remarks>
        private static void WriteEscapedStringToBuffer(
            TextWriter writer,
            string inputString,
            ref int currentIndex,
            char[] buffer,
            ref int bufferIndex,
            ODataStringEscapeOption stringEscapeOption)
        {
            Debug.Assert(inputString != null, "inputString != null");
            Debug.Assert(buffer != null, "buffer != null");

            for (; currentIndex < inputString.Length; currentIndex++)
            {
                bufferIndex = EscapeAndWriteCharToBuffer(writer, inputString[currentIndex], buffer, bufferIndex, stringEscapeOption);
            }
        }

        /// <summary>
        /// Writes an escaped char array to the buffer.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="inputArray">Character array to write.</param>
        /// <param name="inputArrayOffset">How many characters to skip in the input array.</param>
        /// <param name="inputArrayCount">How many characters to write from the input array.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="bufferIndex">Current position in the buffer after the string has been written.</param>
        /// <param name="stringEscapeOption">The string escape option.</param>
        /// <remarks>
        /// IMPORTANT: After all characters have been written,
        /// caller is responsible for writing the final buffer contents to the writer.
        /// </remarks>
        private static void WriteEscapedCharArrayToBuffer(
            TextWriter writer,
            char[] inputArray,
            ref int inputArrayOffset,
            int inputArrayCount,
            char[] buffer,
            ref int bufferIndex,
            ODataStringEscapeOption stringEscapeOption)
        {
            Debug.Assert(inputArray != null, "inputArray != null");
            Debug.Assert(buffer != null, "buffer != null");

            for (; inputArrayOffset < inputArrayCount; inputArrayOffset++)
            {
                bufferIndex = EscapeAndWriteCharToBuffer(writer, inputArray[inputArrayOffset], buffer, bufferIndex, stringEscapeOption);
            }
        }
    }
}
