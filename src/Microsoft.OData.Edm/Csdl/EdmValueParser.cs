//---------------------------------------------------------------------
// <copyright file="EdmValueParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.OData.Edm;

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
    /// Contains xml parsing methods for Edm.
    /// </summary>
    internal static class EdmValueParser
    {
        /// <summary>
        /// This pattern eliminates all durations with year or month fields, leaving only those with day, hour, minutes, and/or seconds fields
        /// </summary>
        internal static readonly Regex DayTimeDurationValidator = PlatformHelper.CreateCompiled("^[^YM]*[DT].*$", RegexOptions.Singleline);

        /// <summary>
        /// Converts a string to a TimeSpan.
        /// </summary>
        /// <param name="value">The string to be converted. The string must be an edm:Duration expression</param>
        /// <returns>A TimeSpan equivalent of the string.</returns>
        /// <exception cref="System.FormatException">Throws if the given string is not an edm:Duration expression</exception>
        /// <exception cref="System.OverflowException">Throws if the given duration is greater than P10675199DT2H48M5.4775807S or less than P10675199DT2H48M5.4775807S</exception>
        internal static TimeSpan ParseDuration(string value)
        {
            if (value == null || !DayTimeDurationValidator.IsMatch(value))
            {
                throw new FormatException(Strings.ValueParser_InvalidDuration(value));
            }

            return XmlConvert.ToTimeSpan(value);
        }

        /// <summary>
        /// Attempts to parse a byte[] value from the specified text.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">The byte[] resulting from parsing the string value</param>
        /// <returns>true if the value was parsed successfully, false otherwise</returns>
        internal static bool TryParseBinary(string value, out byte[] result)
        {
            if (value.Length % 2 != 0)
            {
                result = null;
                return false;
            }

            result = new byte[value.Length >> 1];
            for (int i = 0; i < value.Length; ++i)
            {
                byte h;
                if (!TryParseCharAsBinary(value[i], out h))
                {
                    result = null;
                    return false;
                }

                byte l;
                if (!TryParseCharAsBinary(value[++i], out l))
                {
                    result = null;
                    return false;
                }

                result[i >> 1] = (byte)((h << 4) | l);
            }

            return true;
        }

        /// <summary>
        /// Attempts to parse a bool value from the specified text.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">The bool resulting from parsing the string value</param>
        /// <returns>true if the value was parsed successfully, false otherwise</returns>
        internal static bool TryParseBool(string value, out bool? result)
        {
            switch (value.Length)
            {
                case 4:
                    if ((value[0] == 't' || value[0] == 'T') &&
                        (value[1] == 'r' || value[1] == 'R') &&
                        (value[2] == 'u' || value[2] == 'U') &&
                        (value[3] == 'e' || value[3] == 'E'))
                    {
                        result = true;
                        return true;
                    }

                    break;

                case 5:
                    if ((value[0] == 'f' || value[0] == 'F') &&
                        (value[1] == 'a' || value[1] == 'A') &&
                        (value[2] == 'l' || value[2] == 'L') &&
                        (value[3] == 's' || value[3] == 'S') &&
                        (value[4] == 'e' || value[4] == 'E'))
                    {
                        result = false;
                        return true;
                    }

                    break;

                case 1:
                    switch (value[0])
                    {
                        case '1':
                            result = true;
                            return true;
                        case '0':
                            result = false;
                            return true;
                    }

                    break;

                default:
                    break;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Attempts to parse a TimeSpan value from the specified text.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">The TimeSpan resulting from parsing the string value</param>
        /// <returns>true if the value was parsed successfully, false otherwise</returns>
        internal static bool TryParseDuration(string value, out TimeSpan? result)
        {
            try
            {
                result = ParseDuration(value);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
            catch (OverflowException)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to parse a DateTimeOffset value from the specified text.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">The DateTimeOffset resulting from parsing the string value</param>
        /// <returns>true if the value was parsed successfully, false otherwise</returns>
        internal static bool TryParseDateTimeOffset(string value, out DateTimeOffset? result)
        {
            try
            {
                result = PlatformHelper.ConvertStringToDateTimeOffset(value);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to parse a int value from the specified text.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">The int resulting from parsing the string value</param>
        /// <returns>true if the value was parsed successfully, false otherwise</returns>
        internal static bool TryParseInt(string value, out int? result)
        {
            try
            {
                result = XmlConvert.ToInt32(value);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
            catch (OverflowException)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to parse a long value from the specified text.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">The long resulting from parsing the string value</param>
        /// <returns>true if the value was parsed successfully, false otherwise</returns>
        internal static bool TryParseLong(string value, out long? result)
        {
            try
            {
                result = XmlConvert.ToInt64(value);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
            catch (OverflowException)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to parse a decimal value from the specified text.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">The decimal resulting from parsing the string value</param>
        /// <returns>true if the value was parsed successfully, false otherwise</returns>
        internal static bool TryParseDecimal(string value, out decimal? result)
        {
            try
            {
                result = XmlConvert.ToDecimal(value);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
            catch (OverflowException)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to parse a double value from the specified text.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">The double resulting from parsing the string value</param>
        /// <returns>true if the value was parsed successfully, false otherwise</returns>
        internal static bool TryParseFloat(string value, out double? result)
        {
            try
            {
                result = XmlConvert.ToDouble(value);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
            catch (OverflowException)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to parse a Guid value from the specified text.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">The Guid resulting from parsing the string value</param>
        /// <returns>true if the value was parsed successfully, false otherwise</returns>
        internal static bool TryParseGuid(string value, out Guid? result)
        {
            try
            {
                result = XmlConvert.ToGuid(value);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to parse a Date value from the specified text.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">The Date resulting from parsing the string value</param>
        /// <returns>true if the value was parsed successfully, false otherwise</returns>
        internal static bool TryParseDate(string value, out Date? result)
        {
            try
            {
                result = PlatformHelper.ConvertStringToDate(value);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to parse a TimeOfDay value from the specified text.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">The TimeOfDay resulting from parsing the string value</param>
        /// <returns>true if the value was parsed successfully, false otherwise</returns>
        internal static bool TryParseTimeOfDay(string value, out TimeOfDay? result)
        {
            try
            {
                result = PlatformHelper.ConvertStringToTimeOfDay(value);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to parse a byte value from the specified char.
        /// </summary>
        /// <param name="c">Input char</param>
        /// <param name="b">The byte resulting from parsing the char value</param>
        /// <returns>true if the value was parsed successfully, false otherwise</returns>
        private static bool TryParseCharAsBinary(char c, out byte b)
        {
            uint v = (uint)c - (uint)'0';
            if (v >= 0 && v <= 9)
            {
                b = (byte)v;
                return true;
            }

            v = (uint)c - (uint)'A';
            if (v < 0 || v > 5)
            {
                v = (uint)c - (uint)'a';
            }

            if (v >= 0 && v <= 5)
            {
                b = (byte)(v + 10);
                return true;
            }

            b = default(byte);
            return false;
        }
    }
}
