//---------------------------------------------------------------------
// <copyright file="ConverterUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using Microsoft.OData.Core;
    #region Namespaces
    using System;
    using System.Globalization;
    #endregion Namespaces

    /// <summary>
    /// Same methods in "XmlConverter" but supports ReadOnlySpan<char>
    /// </summary>
    internal static class ConverterUtils
    {
        internal static readonly char[] WhitespaceChars = new char[] { ' ', '\t', '\n', '\r' };

        public static DateTimeOffset ToDateTimeOffset(this ReadOnlySpan<char> s)
        {
            return PlatformHelper.ConvertStringToDateTimeOffset(s);
        }

        public static short ToInt16(this ReadOnlySpan<char> s)
        {
            return short.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
        }


        public static int ToInt32(this ReadOnlySpan<char> s)
        {
            return int.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
        }

        public static long ToInt64(this ReadOnlySpan<char> s)
        {
            return long.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
        }

        public static byte ToByte(this ReadOnlySpan<char> s)
        {
            return byte.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
        }

        public static sbyte ToSByte(this ReadOnlySpan<char> s)
            => sbyte.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);

        public static uint ToUInt32(this ReadOnlySpan<char> s)
        {
            return uint.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
        }

        public static ulong ToUInt64(this ReadOnlySpan<char> s)
        {
            return ulong.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
        }

        public static decimal ToDecimal(this ReadOnlySpan<char> s)
            => decimal.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);

        public static float ToSingle(this ReadOnlySpan<char> s)
        {
            if (s.IsEmpty)
            {
                throw new ArgumentNullException(nameof(s));
            }

            ReadOnlySpan<char> value = s.Trim(WhitespaceChars);
            switch (value)
            {
                case "-INF":
                    return float.NegativeInfinity;
                case "INF":
                    return float.PositiveInfinity;
                default:
                    float f = float.Parse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo);
                    if (f == 0 && value[0] == '-')
                    {
                        return -0f;
                    }

                    return f;
            }
        }

        public static double ToDouble(this ReadOnlySpan<char> s)
        {
            if (s.IsEmpty)
            {
                throw new ArgumentNullException(nameof(s));
            }

            ReadOnlySpan<char> value = s.Trim(WhitespaceChars);
            switch (value)
            {
                case "-INF":
                    return double.NegativeInfinity;
                case "INF":
                    return double.PositiveInfinity;
                default:
                    double dVal = double.Parse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
                    if (dVal == 0 && value[0] == '-')
                    {
                        return -0d;
                    }

                    return dVal;
            }
        }

        public static Guid ToGuid(this ReadOnlySpan<char> s) => Guid.Parse(s);

        public static bool ToBoolean(this ReadOnlySpan<char> s)
        {
            // Copy from XmlConverter
            switch (s.Trim(WhitespaceChars))
            {
                case "1":
                case "true":
                    return true;
                case "0":
                case "false":
                    return false;
                default:
                    throw new FormatException($"The string '{s.ToString()}' is not a valid 'boolean' value.");
            }
        }

        public static TimeSpan ToTimeSpan(this ReadOnlySpan<char> s)
            => TimeSpan.Parse(s, CultureInfo.InvariantCulture);
    }
}
