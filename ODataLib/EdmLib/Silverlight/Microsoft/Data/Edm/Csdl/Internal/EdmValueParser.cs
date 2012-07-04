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
using System.Globalization;
using System.Xml;

namespace Microsoft.Data.Edm.Csdl.Internal
{
    /// <summary>
    /// Contains xml parsing methods for Edm.
    /// </summary>
    internal static class EdmValueParser
    {
        private const string TimeExp = @"[0-9]{2}:[0-9]{2}:[0-9]{2}(\.[0-9]{0,3})?";
        #if SILVERLIGHT || ORCAS
        // RegexOptions.Compiled does not exists in Silverlight
        private static System.Text.RegularExpressions.Regex TimeValidator = new System.Text.RegularExpressions.Regex(@"^" + TimeExp + @"$", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.None);
#else
        private static System.Text.RegularExpressions.Regex TimeValidator = new System.Text.RegularExpressions.Regex(@"^" + TimeExp + @"$", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.Compiled);
        #endif

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
                result = XmlConvert.ToDateTimeOffset(value);
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
