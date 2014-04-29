//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    using System.Globalization;
    using System.Xml;
    using Microsoft.OData.Core;
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
            return b ? AtomConstants.AtomTrueLiteral : AtomConstants.AtomFalseLiteral;
        }

        /// <summary>
        /// Converts a byte to the corresponding ATOM string representation.
        /// </summary>
        /// <param name="b">The byte value to convert.</param>
        /// <returns>The ATOM strings representing the byte value.</returns>
        internal static string ToString(byte b)
        {
            return XmlConvert.ToString(b);
        }

        /// <summary>
        /// Converts a decimal to the corresponding ATOM string representation.
        /// </summary>
        /// <param name="d">The decimal value to convert.</param>
        /// <returns>The ATOM strings representing the decimal value.</returns>
        internal static string ToString(decimal d)
        {
            return XmlConvert.ToString(d);
        }

        /// <summary>
        /// Converts the given DateTimeOffset value to string appropriate for Atom format.
        /// </summary>
        /// <param name="dateTime">Given DateTimeOffset value.</param>
        /// <returns>Atom format string representation of <paramref name="dateTime"/>.</returns>
        internal static string ToString(DateTimeOffset dateTime)
        {
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
            return EdmValueWriter.DurationAsXml(ts);
        }

        /// <summary>
        /// Converts the given double value to the string appropriate for Atom format
        /// </summary>
        /// <param name="d">The double value to convert.</param>
        /// <returns>The string version of the double value in Atom format.</returns>
        internal static string ToString(this double d)
        {
            return XmlConvert.ToString(d);
        }

        /// <summary>
        /// Converts the given Int16 value to the string appropriate for Atom format
        /// </summary>
        /// <param name="i">The Int16 value to convert.</param>
        /// <returns>The string version of the Int16 value in Atom format.</returns>
        internal static string ToString(this Int16 i)
        {
            return XmlConvert.ToString(i);
        }

        /// <summary>
        /// Converts the given Int32 value to the string appropriate for Atom format.
        /// </summary>
        /// <param name="i">The Int32 value to convert.</param>
        /// <returns>The string version of the Int32 in Atom format.</returns>
        internal static string ToString(this Int32 i)
        {
            return XmlConvert.ToString(i);
        }

        /// <summary>
        /// Converts the given Int64 value to the string appropriate for Atom format.
        /// </summary>
        /// <param name="i">The Int64 value to convert.</param>
        /// <returns>The string version of the Int64 in Atom format.</returns>
        internal static string ToString(this Int64 i)
        {
            return XmlConvert.ToString(i);
        }

        /// <summary>
        /// Converts the given SByte value to the string appropriate for Atom format.
        /// </summary>
        /// <param name="sb">The SByte value to convert.</param>
        /// <returns>The string version of the SByte in Atom format.</returns>
        internal static string ToString(this SByte sb)
        {
            return XmlConvert.ToString(sb);
        }

        /// <summary>
        /// Converts the given byte array value to the string appropriate for Atom format.
        /// </summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <returns>The string version of the byte array in Atom format.</returns>
        internal static string ToString(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts the given Single value to the string appropriate for Atom format.
        /// </summary>
        /// <param name="s">The Single value to convert.</param>
        /// <returns>The string version of the Single in Atom format.</returns>
        internal static string ToString(this Single s)
        {
            return XmlConvert.ToString(s);
        }

        /// <summary>
        /// Converts the given Guid value to the string appropriate for Atom format.
        /// </summary>
        /// <param name="guid">The Guid value to convert.</param>
        /// <returns>The string version of the Guid in Atom format.</returns>
        internal static string ToString(this Guid guid)
        {
            return XmlConvert.ToString(guid);
        }
    }
}
