//---------------------------------------------------------------------
// <copyright file="JsonPrimitiveValueTextAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json.TextAnnotations
{
    #region Namespaces
    using System;
    using System.Globalization;
    using System.Text;
    using System.Xml;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Annotation for primitive values
    /// </summary>
    public class JsonPrimitiveValueTextAnnotation : JsonTextAnnotation
    {
        /// <summary>
        /// Returns the default text annotation for a specified primitive value.
        /// </summary>
        /// <param name="primitiveValue">The primitive value.</param>
        /// <param name="writingJsonLight">true if we're writing JSON light, false otherwise.</param>
        /// <param name="isIeee754Compatible">if it is IEEE754Compatible</param>
        /// <returns>The default text representation of the primitive value.</returns>
        public static JsonPrimitiveValueTextAnnotation GetDefault(JsonPrimitiveValue primitiveValue, bool writingJsonLight, bool isIeee754Compatible)
        {
            string result = null;

            if (primitiveValue.Value == null)
            {
                result = "null";
            }
            else
            {
                switch (Type.GetTypeCode(primitiveValue.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        result = XmlConvert.ToString((bool)primitiveValue.Value);
                        break;
                    case TypeCode.Int32:
                        result = ((int)primitiveValue.Value).ToString(CultureInfo.InvariantCulture);
                        break;
                    case TypeCode.UInt32:
                        result = ((uint)primitiveValue.Value).ToString(CultureInfo.InvariantCulture);
                        break;
                    case TypeCode.Single:
                        float floatValue = (float)primitiveValue.Value;
                        if (float.IsInfinity(floatValue) || float.IsNaN(floatValue))
                        {
                            result = "\"" + floatValue.ToString(null, CultureInfo.InvariantCulture) + "\"";
                        }
                        else
                        {
                            result = XmlConvert.ToString(floatValue);
                        }

                        break;
                    case TypeCode.Int16:
                        result = ((short)primitiveValue.Value).ToString(CultureInfo.InvariantCulture);
                        break;
                    case TypeCode.UInt16:
                        result = ((ushort)primitiveValue.Value).ToString(CultureInfo.InvariantCulture);
                        break;
                    case TypeCode.Int64:
                        result = ((long)primitiveValue.Value).ToString(CultureInfo.InvariantCulture);
                        if (isIeee754Compatible)
                        {
                            result = "\"" + result + "\"";
                        }

                        break;
                    case TypeCode.UInt64:
                        result = ((ulong)primitiveValue.Value).ToString(CultureInfo.InvariantCulture);
                        if (isIeee754Compatible)
                        {
                            result = "\"" + result + "\"";
                        }
                        
                        break;
                    case TypeCode.Double:
                        double doubleValue = (double)primitiveValue.Value;
                        if (double.IsInfinity(doubleValue) || double.IsNaN(doubleValue))
                        {
                            result = "\"" + doubleValue.ToString(null, CultureInfo.InvariantCulture) + "\"";
                        }
                        else
                        {
                            result = XmlConvert.ToString(doubleValue);

                            if (writingJsonLight && result.IndexOfAny(new char[] {'e', 'E', '.'}) < 0)
                            {
                                result += ".0";
                            }
                        }

                        break;
                    case TypeCode.Decimal:
                        result = ((decimal)primitiveValue.Value).ToString(CultureInfo.InvariantCulture);
                        if (isIeee754Compatible)
                        {
                            result = "\"" + result + "\"";
                        }

                        break;
                    case TypeCode.Byte:
                        result = ((byte)primitiveValue.Value).ToString(CultureInfo.InvariantCulture);
                        break;
                    case TypeCode.SByte:
                        result = ((sbyte)primitiveValue.Value).ToString(CultureInfo.InvariantCulture);
                        break;
                    case TypeCode.String:
                        result = "\"" + WriteEscapedJsonString((string)primitiveValue.Value) + "\"";
                        break;
                    default:
                        if (primitiveValue.Value is DateTime)
                        {
                            DateTime dateTime = (DateTime)primitiveValue.Value;
                            if (dateTime.Kind == DateTimeKind.Local)
                            {
                                dateTime = dateTime.ToUniversalTime();
                            }
                            else if (dateTime.Kind == DateTimeKind.Unspecified)
                            {
                                dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
                            }

                            result =
                                String.Format(
                                    CultureInfo.InvariantCulture,
                                    JsonDateTimeFormat,
                                    ((dateTime.Ticks - DateTimeMinTimeTicks) / 10000));
                        }
                        else if (primitiveValue.Value is DateTimeOffset)
                        {
                            DateTimeOffset dateTimeOffset = (DateTimeOffset)primitiveValue.Value;
                            Int32 offsetMinutes = (Int32)dateTimeOffset.Offset.TotalMinutes;
                            result =
                                String.Format(
                                    CultureInfo.InvariantCulture,
                                    JsonDateTimeOffsetFormat,
                                    ((dateTimeOffset.Ticks - DateTimeMinTimeTicks) / 10000),
                                    offsetMinutes >= 0 ? "+" : string.Empty,
                                    offsetMinutes);
                        }
                        else if (primitiveValue.Value is Guid)
                        {
                            result = "\"" + ((Guid)primitiveValue.Value).ToString() + "\"";
                        }
                        else if (primitiveValue.Value is byte[])
                        {
                            result = "\"" + Convert.ToBase64String((byte[])primitiveValue.Value) + "\"";
                        }
                        else if (primitiveValue.Value is TimeSpan)
                        {
                            result = "\"" + XmlConvert.ToString((TimeSpan)primitiveValue.Value) + "\"";
                        }
                        else
                        {
                            throw new TaupoNotSupportedException(string.Format(CultureInfo.InvariantCulture, "Unsupported primitive value type for JSON: {0}", primitiveValue.Value.GetType().FullName));
                        }

                        break;
                }
            }

            return new JsonPrimitiveValueTextAnnotation() { Text = result };
        }

        /// <summary> const tick value for caculating tick values</summary>
        internal static readonly long DateTimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        /// <summary> Json datetime format </summary>
        private const string JsonDateTimeFormat = @"""\/Date({0})\/""";

        /// <summary> Json datetimeoffset format </summary>
        private const string JsonDateTimeOffsetFormat = @"""\/Date({0}{1}{2:D4})\/""";

        /// <summary>
        /// Writes string as an escaped JSON string.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        private static string WriteEscapedJsonString(string value)
        {
            StringBuilder builder = new StringBuilder(value.Length + 6);

            if (value != null)
            {
                int lastWritePosition = 0;
                int skipped = 0;
                char[] chars = null;

                for (int i = 0; i < value.Length; i++)
                {
                    char c = value[i];
                    string escapedValue;

                    switch (c)
                    {
                        case '\t':
                            escapedValue = @"\t";
                            break;
                        case '\n':
                            escapedValue = @"\n";
                            break;
                        case '\r':
                            escapedValue = @"\r";
                            break;
                        case '\f':
                            escapedValue = @"\f";
                            break;
                        case '\b':
                            escapedValue = @"\b";
                            break;
                        case '\\':
                            escapedValue = @"\\";
                            break;
                        case '\u0085': // Next Line
                            escapedValue = @"\u0085";
                            break;
                        case '\u2028': // Line Separator
                            escapedValue = @"\u2028";
                            break;
                        case '\u2029': // Paragraph Separator
                            escapedValue = @"\u2029";
                            break;
                        case '"':
                            escapedValue = "\\\"";
                            break;
                        case '\'':
                            escapedValue = @"\'";
                            break;
                        default:
                            escapedValue = (c <= '\u001f') ? ToCharAsUnicode(c) : null;
                            break;
                    }

                    if (escapedValue != null)
                    {
                        if (chars == null)
                        {
                            chars = value.ToCharArray();
                        }

                        // write skipped text
                        if (skipped > 0)
                        {
                            builder.Append(chars, lastWritePosition, skipped);
                            skipped = 0;
                        }

                        // write escaped value and note position
                        builder.Append(escapedValue);
                        lastWritePosition = i + 1;
                    }
                    else
                    {
                        skipped++;
                    }
                }

                // write any remaining skipped text
                if (skipped > 0)
                {
                    if (lastWritePosition == 0)
                    {
                        builder.Append(value);
                    }
                    else
                    {
                        builder.Append(chars, lastWritePosition, skipped);
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Writes the character as a unicode escape sequence.
        /// </summary>
        /// <param name="c">The character to write.</param>
        /// <returns>The escaped character.</returns>
        private static string ToCharAsUnicode(char c)
        {
            char h1 = IntToHex((c >> 12) & '\x000f');
            char h2 = IntToHex((c >> 8) & '\x000f');
            char h3 = IntToHex((c >> 4) & '\x000f');
            char h4 = IntToHex(c & '\x000f');

            return new string(new[] { '\\', 'u', h1, h2, h3, h4 });
        }

        /// <summary>
        /// Converts integer to its hex character.
        /// </summary>
        /// <param name="n">The integer to convert.</param>
        /// <returns>The hex character.</returns>
        private static char IntToHex(int n)
        {
            ExceptionUtilities.Assert(n >= 0 && n < 16, "The input must be a hex number 0 - 15.");

            if (n <= 9)
            {
                return (char)(n + 48);
            }

            return (char)((n - 10) + 97);
        }
    }
}
