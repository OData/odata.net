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

namespace Microsoft.Data.Edm.Csdl.Internal
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// Contains xml parsing methods for Edm.
    /// </summary>
    internal static class EdmValueParser
    {
        private const string TimeExp = @"[0-9]{2}:[0-9]{2}:[0-9]{2}(\.[0-9]{0,3})?";
        private static Regex TimeValidator = PlatformHelper.CreateCompiled(@"^" + TimeExp + @"$", RegexOptions.Singleline);

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

        internal static bool TryParseDateTime(string value, out DateTime? result)
        {
            try
            {
                result = PlatformHelper.ConvertStringToDateTime(value);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
        }

        internal static bool TryParseTime(string value, out TimeSpan? result)
        {
            if (!TimeValidator.IsMatch(value))
            {
                result = null;
                return false;
            }

            try
            {
                // Timespan includes days, while xsd:Time only has hours so we set days to 0
#if ORCAS
                result = TimeSpan.Parse("00:" + value);
#else
                result = TimeSpan.Parse("00:" + value, CultureInfo.InvariantCulture);
#endif
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
        }

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
        }

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
        }

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
        }

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
        }

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
        }

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
