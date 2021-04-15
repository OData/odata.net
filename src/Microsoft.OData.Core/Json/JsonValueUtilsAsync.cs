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
    using System.Threading.Tasks;
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
        /// Write a char value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">The char value to write.</param>
        /// <param name="stringEscapeOption">The ODataStringEscapeOption to use in escaping the string.</param>
        internal static async Task WriteValueAsync(this TextWriter writer, char value, ODataStringEscapeOption stringEscapeOption)
        {
            Debug.Assert(writer != null, "writer != null");

            if (stringEscapeOption == ODataStringEscapeOption.EscapeNonAscii || value <= 0x7F)
            {
                string escapedString = SpecialCharToEscapedStringMap[value];
                if (escapedString != null)
                {
                    await writer.WriteAsync(escapedString).ConfigureAwait(false);
                    return;
                }
            }

            await writer.WriteAsync(value).ConfigureAwait(false);
        }

        /// <summary>
        /// Write a boolean value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">The boolean value to write.</param>
        internal static Task WriteValueAsync(this TextWriter writer, bool value)
        {
            Debug.Assert(writer != null, "writer != null");

            return writer.WriteAsync(FormatAsBooleanLiteral(value));
        }

        /// <summary>
        /// Write an integer value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Integer value to be written.</param>
        internal static Task WriteValueAsync(this TextWriter writer, int value)
        {
            Debug.Assert(writer != null, "writer != null");

            return writer.WriteAsync(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a float value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Float value to be written.</param>
        internal static Task WriteValueAsync(this TextWriter writer, float value)
        {
            Debug.Assert(writer != null, "writer != null");

            if (JsonSharedUtils.IsFloatValueSerializedAsString(value))
            {
                return writer.WriteQuotedAsync(value.ToString(ODataNumberFormatInfo));
            }
            else
            {
                // float.ToString() supports a max scale of six,
                // whereas float.MinValue and float.MaxValue have 8 digits scale. Hence we need
                // to use XmlConvert in all other cases, except infinity
                return writer.WriteAsync(XmlConvert.ToString(value));
            }
        }

        /// <summary>
        /// Write a short value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Short value to be written.</param>
        internal static Task WriteValueAsync(this TextWriter writer, short value)
        {
            Debug.Assert(writer != null, "writer != null");

            return writer.WriteAsync(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a long value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Long value to be written.</param>
        internal static Task WriteValueAsync(this TextWriter writer, long value)
        {
            Debug.Assert(writer != null, "writer != null");

            return writer.WriteAsync(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a double value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Double value to be written.</param>
        internal static async Task WriteValueAsync(this TextWriter writer, double value)
        {
            Debug.Assert(writer != null, "writer != null");

            if (JsonSharedUtils.IsDoubleValueSerializedAsString(value))
            {
                await writer.WriteQuotedAsync(value.ToString(ODataNumberFormatInfo)).ConfigureAwait(false);
            }
            else
            {
                // double.ToString() supports a max scale of 14,
                // whereas double.MinValue and double.MaxValue have 16 digits scale. Hence we need
                // to use XmlConvert in all other cases, except infinity
                string valueToWrite = XmlConvert.ToString(value);

                await writer.WriteAsync(valueToWrite).ConfigureAwait(false);
                if (valueToWrite.IndexOfAny(DoubleIndicatingCharacters) < 0)
                {
                    await writer.WriteAsync(".0").ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Write a Guid value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Guid value to be written.</param>
        internal static Task WriteValueAsync(this TextWriter writer, Guid value)
        {
            Debug.Assert(writer != null, "writer != null");

            return writer.WriteQuotedAsync(value.ToString());
        }

        /// <summary>
        /// Write a decimal value
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Decimal value to be written.</param>
        internal static Task WriteValueAsync(this TextWriter writer, decimal value)
        {
            Debug.Assert(writer != null, "writer != null");

            return writer.WriteAsync(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a DateTimeOffset value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">DateTimeOffset value to be written.</param>
        /// <param name="dateTimeFormat">The format to write out the DateTime value in.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.Json.JsonValueUtils.WriteQuotedAsync(System.IO.TextWriter,System.String)", Justification = "Constant defined by the JSON spec.")]
        internal static async Task WriteValueAsync(this TextWriter writer, DateTimeOffset value, ODataJsonDateTimeFormat dateTimeFormat)
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
                        await writer.WriteQuotedAsync(textValue).ConfigureAwait(false);
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
                        await writer.WriteQuotedAsync(textValue).ConfigureAwait(false);
                    }

                    break;
            }
        }

        /// <summary>
        /// Write a TimeSpan value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">TimeSpan value to be written.</param>
        internal static Task WriteValueAsync(this TextWriter writer, TimeSpan value)
        {
            Debug.Assert(writer != null, "writer != null");

            return writer.WriteQuotedAsync(EdmValueWriter.DurationAsXml(value));
        }

        /// <summary>
        /// Write a TimeOfDay value
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">TimeOfDay value to be written.</param>
        internal static Task WriteValueAsync(this TextWriter writer, TimeOfDay value)
        {
            Debug.Assert(writer != null, "writer != null");

            return writer.WriteQuotedAsync(value.ToString());
        }

        /// <summary>
        /// Write a Date value
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Date value to be written.</param>
        internal static Task WriteValueAsync(this TextWriter writer, Date value)
        {
            Debug.Assert(writer != null, "writer != null");

            return writer.WriteQuotedAsync(value.ToString());
        }

        /// <summary>
        /// Write a byte value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Byte value to be written.</param>
        internal static Task WriteValueAsync(this TextWriter writer, byte value)
        {
            Debug.Assert(writer != null, "writer != null");

            return writer.WriteAsync(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write an sbyte value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">SByte value to be written.</param>
        internal static Task WriteValueAsync(this TextWriter writer, sbyte value)
        {
            Debug.Assert(writer != null, "writer != null");

            return writer.WriteAsync(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write a string value.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">String value to be written.</param>
        /// <param name="stringEscapeOption">The string escape option.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="arrayPool">Array pool for renting a buffer.</param>
        internal static Task WriteValueAsync(
            this TextWriter writer,
            string value,
            ODataStringEscapeOption stringEscapeOption,
            Ref<char[]> buffer,
            ICharArrayPool arrayPool = null)
        {
            Debug.Assert(writer != null, "writer != null");

            if (value == null)
            {
                return writer.WriteAsync(JsonConstants.JsonNullLiteral);
            }
            else
            {
                return writer.WriteEscapedJsonStringAsync(value, stringEscapeOption, buffer, arrayPool);
            }
        }

        /// <summary>
        /// Write a byte array.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Byte array to be written.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="arrayPool">Array pool for renting a buffer.</param>
        internal static async Task WriteValueAsync(this TextWriter writer, byte[] value, Ref<char[]> buffer, ICharArrayPool arrayPool = null)
        {
            Debug.Assert(writer != null, "writer != null");

            if (value == null)
            {
                await writer.WriteAsync(JsonConstants.JsonNullLiteral).ConfigureAwait(false);
            }
            else
            {
                await writer.WriteAsync(JsonConstants.QuoteCharacter).ConfigureAwait(false);
                await writer.WriteBinaryStringAsync(value, buffer, arrayPool).ConfigureAwait(false);
                await writer.WriteAsync(JsonConstants.QuoteCharacter).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Write a byte array.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="value">Byte array to be written.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="arrayPool">Array pool for renting a buffer.</param>
        internal static async Task WriteBinaryStringAsync(this TextWriter writer, byte[] value, Ref<char[]> buffer, ICharArrayPool arrayPool)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(value != null, "The value must not be null.");

            buffer.Value = BufferUtils.InitializeBufferIfRequired(arrayPool, buffer.Value);
            Debug.Assert(buffer.Value != null);

            int bufferLength = buffer.Value.Length;

            // Try to hold base64 string as much as possible in one converting.
            int bufferByteSize = bufferLength * 3 / 4;

            for (int offsetIn = 0; offsetIn < value.Length; offsetIn += bufferByteSize)
            {
                int count = WriteByteArrayToBuffer(value, offsetIn, buffer.Value, bufferByteSize);
                await writer.WriteAsync(buffer.Value, 0, count).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Returns the string value with special characters escaped.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="inputString">Input string value.</param>
        /// <param name="stringEscapeOption">The string escape option.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="bufferPool">Array pool for renting a buffer.</param>
        internal static async Task WriteEscapedJsonStringAsync(
            this TextWriter writer,
            string inputString,
            ODataStringEscapeOption stringEscapeOption,
            Ref<char[]> buffer,
            ICharArrayPool bufferPool = null)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(inputString != null, "The string value must not be null.");

            await writer.WriteAsync(JsonConstants.QuoteCharacter).ConfigureAwait(false);
            await writer.WriteEscapedJsonStringValueAsync(inputString, stringEscapeOption, buffer, bufferPool).ConfigureAwait(false);
            await writer.WriteAsync(JsonConstants.QuoteCharacter).ConfigureAwait(false);
        }

        /// <summary>
        /// Writes the string value with special characters escaped.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="inputString">Input string value.</param>
        /// <param name="stringEscapeOption">The string escape option.</param>
        /// <param name="buffer">Char buffer to use for streaming data.</param>
        /// <param name="bufferPool">Array pool for renting a buffer.</param>
        internal static async Task WriteEscapedJsonStringValueAsync(
            this TextWriter writer,
            string inputString,
            ODataStringEscapeOption stringEscapeOption,
            Ref<char[]> buffer,
            ICharArrayPool bufferPool)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(inputString != null, "The string value must not be null.");

            int firstIndex;
            if (!CheckIfStringHasSpecialChars(inputString, stringEscapeOption, out firstIndex))
            {
                await writer.WriteAsync(inputString).ConfigureAwait(false);
            }
            else
            {
                Debug.Assert(firstIndex < inputString.Length, "First index of the special character should be within the string");
                buffer.Value = BufferUtils.InitializeBufferIfRequired(bufferPool, buffer.Value);
                int bufferLength = buffer.Value.Length;
                int bufferIndex = 0;
                int currentIndex = 0;

                // Let's copy and flush strings up to the first index of the special char
                while (currentIndex < firstIndex)
                {
                    int substrLength = firstIndex - currentIndex;

                    Debug.Assert(substrLength > 0, "SubStrLength should be greater than 0 always");

                    // If the first index of the special character is larger than the buffer length,
                    // flush everything to the buffer first and reset the buffer to the next chunk.
                    // Otherwise copy to the buffer and go on from there.
                    if (substrLength >= bufferLength)
                    {
                        inputString.CopyTo(currentIndex, buffer.Value, 0, bufferLength);
                        await writer.WriteAsync(buffer.Value, 0, bufferLength).ConfigureAwait(false);
                        currentIndex += bufferLength;
                    }
                    else
                    {
                        WriteSubstringToBuffer(inputString, ref currentIndex, buffer.Value, ref bufferIndex, substrLength);
                    }
                }

                // Write escaped string to buffer
                WriteEscapedStringToBuffer(writer, inputString, ref currentIndex, buffer.Value, ref bufferIndex, stringEscapeOption);

                // write any remaining chars to the writer
                if (bufferIndex > 0)
                {
                    await writer.WriteAsync(buffer.Value, 0, bufferIndex).ConfigureAwait(false);
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
        internal static async Task WriteEscapedCharArrayAsync(
            this TextWriter writer,
            char[] inputArray,
            int inputArrayOffset, 
            int inputArrayCount,
            ODataStringEscapeOption stringEscapeOption,
            Ref<char[]> buffer,
            ICharArrayPool bufferPool)
        {
            int bufferIndex = 0;
            buffer.Value = BufferUtils.InitializeBufferIfRequired(bufferPool, buffer.Value);
            
            WriteEscapedCharArrayToBuffer(writer, inputArray, ref inputArrayOffset, inputArrayCount, buffer.Value, ref bufferIndex, stringEscapeOption);

            // write remaining bytes in buffer
            if (bufferIndex > 0)
            {
                await writer.WriteAsync(buffer.Value, 0, bufferIndex).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Write the string value with quotes.
        /// </summary>
        /// <param name="writer">The text writer to write the output to.</param>
        /// <param name="text">String value to be written.</param>
        private static Task WriteQuotedAsync(this TextWriter writer, string text)
        {
            return writer.WriteAsync(
                string.Concat(JsonConstants.QuoteCharacter, text, JsonConstants.QuoteCharacter));
        }
    }
}
