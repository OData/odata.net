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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    using System.Globalization;
    using System.Xml;
    using Microsoft.Data.OData;
    #endregion Namespaces

    /// <summary>
    /// Helper to convert values to strings compliant to the ATOM format
    /// </summary>
    internal static class ODataAtomConvert
    {
        /// <summary>Used for settings the updated element properly.</summary>
        private static readonly TimeSpan zeroOffset = new TimeSpan(0, 0, 0);

        /// <summary>
        /// Converts a boolean to the corresponding ATOM string representation.
        /// </summary>
        /// <param name="b">The boolean value to convert.</param>
        /// <returns>The ATOM strings representing boolean literals.</returns>
        internal static string ToString(bool b)
        {
            DebugUtils.CheckNoExternalCallers();

            return b ? AtomConstants.AtomTrueLiteral : AtomConstants.AtomFalseLiteral;
        }

        /// <summary>
        /// Converts a byte to the corresponding ATOM string representation.
        /// </summary>
        /// <param name="b">The byte value to convert.</param>
        /// <returns>The ATOM strings representing the byte value.</returns>
        internal static string ToString(byte b)
        {
            DebugUtils.CheckNoExternalCallers();

            return XmlConvert.ToString(b);
        }

        /// <summary>
        /// Converts a decimal to the corresponding ATOM string representation.
        /// </summary>
        /// <param name="d">The decimal value to convert.</param>
        /// <returns>The ATOM strings representing the decimal value.</returns>
        internal static string ToString(decimal d)
        {
            DebugUtils.CheckNoExternalCallers();

            return XmlConvert.ToString(d);
        }

        /// <summary>
        /// Converts the given date/time value to the string appropriate for Atom format
        /// </summary>
        /// <param name="dt">The date/time value to convert.</param>
        /// <returns>The string version of the date/time value in Atom format.</returns>
        internal static string ToString(this DateTime dt)
        {
            DebugUtils.CheckNoExternalCallers();

            return PlatformHelper.ConvertDateTimeToString(dt);
        }

        /// <summary>
        /// Converts the given DateTimeOffset value to string appropriate for Atom format.
        /// </summary>
        /// <param name="dateTime">Given DateTimeOffset value.</param>
        /// <returns>Atom format string representation of <paramref name="dateTime"/>.</returns>
        internal static string ToString(DateTimeOffset dateTime)
        {
            DebugUtils.CheckNoExternalCallers();

            return XmlConvert.ToString(dateTime);
        }

        /// <summary>
        /// Converts the given DateTimeOffset value to string appropriate for Atom format.
        /// ToAtomString is used to write values in atom specific elements like updated, etc.
        /// </summary>
        /// <param name="dateTime">Given DateTimeOffset value.</param>
        /// <returns>Atom format string representation of <paramref name="dateTime"/>.</returns>
        internal static string ToAtomString(DateTimeOffset dateTime)
        {
            DebugUtils.CheckNoExternalCallers();

            if (dateTime.Offset == zeroOffset)
            {
                return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            }

            return dateTime.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the given timespan value to the string appropriate for Atom format
        /// </summary>
        /// <param name="ts">The timespan value to convert.</param>
        /// <returns>The string version of the timespan value in Atom format.</returns>
        internal static string ToString(this TimeSpan ts)
        {
            DebugUtils.CheckNoExternalCallers();

            return XmlConvert.ToString(ts);
        }

        /// <summary>
        /// Converts the given double value to the string appropriate for Atom format
        /// </summary>
        /// <param name="d">The double value to convert.</param>
        /// <returns>The string version of the double value in Atom format.</returns>
        internal static string ToString(this double d)
        {
            DebugUtils.CheckNoExternalCallers();

            return XmlConvert.ToString(d);
        }

        /// <summary>
        /// Converts the given Int16 value to the string appropriate for Atom format
        /// </summary>
        /// <param name="i">The Int16 value to convert.</param>
        /// <returns>The string version of the Int16 value in Atom format.</returns>
        internal static string ToString(this Int16 i)
        {
            DebugUtils.CheckNoExternalCallers();

            return XmlConvert.ToString(i);
        }

        /// <summary>
        /// Converts the given Int32 value to the string appropriate for Atom format.
        /// </summary>
        /// <param name="i">The Int32 value to convert.</param>
        /// <returns>The string version of the Int32 in Atom format.</returns>
        internal static string ToString(this Int32 i)
        {
            DebugUtils.CheckNoExternalCallers();

            return XmlConvert.ToString(i);
        }

        /// <summary>
        /// Converts the given Int64 value to the string appropriate for Atom format.
        /// </summary>
        /// <param name="i">The Int64 value to convert.</param>
        /// <returns>The string version of the Int64 in Atom format.</returns>
        internal static string ToString(this Int64 i)
        {
            DebugUtils.CheckNoExternalCallers();

            return XmlConvert.ToString(i);
        }

        /// <summary>
        /// Converts the given SByte value to the string appropriate for Atom format.
        /// </summary>
        /// <param name="sb">The SByte value to convert.</param>
        /// <returns>The string version of the SByte in Atom format.</returns>
        internal static string ToString(this SByte sb)
        {
            DebugUtils.CheckNoExternalCallers();

            return XmlConvert.ToString(sb);
        }

        /// <summary>
        /// Converts the given byte array value to the string appropriate for Atom format.
        /// </summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <returns>The string version of the byte array in Atom format.</returns>
        internal static string ToString(this byte[] bytes)
        {
            DebugUtils.CheckNoExternalCallers();

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts the given Single value to the string appropriate for Atom format.
        /// </summary>
        /// <param name="s">The Single value to convert.</param>
        /// <returns>The string version of the Single in Atom format.</returns>
        internal static string ToString(this Single s)
        {
            DebugUtils.CheckNoExternalCallers();

            return XmlConvert.ToString(s);
        }

        /// <summary>
        /// Converts the given Guid value to the string appropriate for Atom format.
        /// </summary>
        /// <param name="guid">The Guid value to convert.</param>
        /// <returns>The string version of the Guid in Atom format.</returns>
        internal static string ToString(this Guid guid)
        {
            DebugUtils.CheckNoExternalCallers();

            return XmlConvert.ToString(guid);
        }
    }
}
