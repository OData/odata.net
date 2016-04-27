//---------------------------------------------------------------------
// <copyright file="ODataRawValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Xml;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Helper to convert values to strings compliant to the raw value format
    /// </summary>
    internal static class ODataRawValueConverter
    {
        /// <summary>'true' literal</summary>
        private const string RawValueTrueLiteral = "true";

        /// <summary>'false' literal</summary>
        private const string RawValueFalseLiteral = "false";

        /// <summary>
        /// Converts a boolean to the corresponding raw value string representation.
        /// </summary>
        /// <param name="b">The boolean value to convert.</param>
        /// <returns>The raw value strings representing boolean literals.</returns>
        internal static string ToString(bool b)
        {
            return b ? RawValueTrueLiteral : RawValueFalseLiteral;
        }

        /// <summary>
        /// Converts a byte to the corresponding raw value string representation.
        /// </summary>
        /// <param name="b">The byte value to convert.</param>
        /// <returns>The raw value strings representing the byte value.</returns>
        internal static string ToString(byte b)
        {
            return XmlConvert.ToString(b);
        }

        /// <summary>
        /// Converts a decimal to the corresponding raw value string representation.
        /// </summary>
        /// <param name="d">The decimal value to convert.</param>
        /// <returns>The raw value strings representing the decimal value.</returns>
        internal static string ToString(decimal d)
        {
            return XmlConvert.ToString(d);
        }

        /// <summary>
        /// Converts the given DateTimeOffset value to string appropriate for raw value format.
        /// </summary>
        /// <param name="dateTime">Given DateTimeOffset value.</param>
        /// <returns>raw value format string representation of <paramref name="dateTime"/>.</returns>
        internal static string ToString(DateTimeOffset dateTime)
        {
            return XmlConvert.ToString(dateTime);
        }

        /// <summary>
        /// Converts the given timespan value to the string appropriate for raw value format
        /// </summary>
        /// <param name="ts">The timespan value to convert.</param>
        /// <returns>The string version of the timespan value in raw value format.</returns>
        internal static string ToString(this TimeSpan ts)
        {
            return EdmValueWriter.DurationAsXml(ts);
        }

        /// <summary>
        /// Converts the given double value to the string appropriate for raw value format
        /// </summary>
        /// <param name="d">The double value to convert.</param>
        /// <returns>The string version of the double value in raw value format.</returns>
        internal static string ToString(this double d)
        {
            return XmlConvert.ToString(d);
        }

        /// <summary>
        /// Converts the given Int16 value to the string appropriate for raw value format
        /// </summary>
        /// <param name="i">The Int16 value to convert.</param>
        /// <returns>The string version of the Int16 value in raw value format.</returns>
        internal static string ToString(this Int16 i)
        {
            return XmlConvert.ToString(i);
        }

        /// <summary>
        /// Converts the given Int32 value to the string appropriate for raw value format.
        /// </summary>
        /// <param name="i">The Int32 value to convert.</param>
        /// <returns>The string version of the Int32 in raw value format.</returns>
        internal static string ToString(this Int32 i)
        {
            return XmlConvert.ToString(i);
        }

        /// <summary>
        /// Converts the given Int64 value to the string appropriate for raw value format.
        /// </summary>
        /// <param name="i">The Int64 value to convert.</param>
        /// <returns>The string version of the Int64 in raw value format.</returns>
        internal static string ToString(this Int64 i)
        {
            return XmlConvert.ToString(i);
        }

        /// <summary>
        /// Converts the given SByte value to the string appropriate for raw value format.
        /// </summary>
        /// <param name="sb">The SByte value to convert.</param>
        /// <returns>The string version of the SByte in raw value format.</returns>
        internal static string ToString(this SByte sb)
        {
            return XmlConvert.ToString(sb);
        }

        /// <summary>
        /// Converts the given byte array value to the string appropriate for raw value format.
        /// </summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <returns>The string version of the byte array in raw value format.</returns>
        internal static string ToString(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts the given Single value to the string appropriate for raw value format.
        /// </summary>
        /// <param name="s">The Single value to convert.</param>
        /// <returns>The string version of the Single in raw value format.</returns>
        internal static string ToString(this Single s)
        {
            return XmlConvert.ToString(s);
        }

        /// <summary>
        /// Converts the given Guid value to the string appropriate for raw value format.
        /// </summary>
        /// <param name="guid">The Guid value to convert.</param>
        /// <returns>The string version of the Guid in raw value format.</returns>
        internal static string ToString(this Guid guid)
        {
            return XmlConvert.ToString(guid);
        }

        /// <summary>
        /// Converts the given Date value to the string appropriate for raw value format.
        /// </summary>
        /// <param name="date">The Date value to convert.</param>
        /// <returns>The string version of the Date in raw value format.</returns>
        internal static string ToString(Date date)
        {
            return date.ToString();
        }

        /// <summary>
        /// Converts the given TimeOfDay value to the string appropriate for raw value format.
        /// </summary>
        /// <param name="time">The TimeOfDay value to convert.</param>
        /// <returns>The string version of the TimeOfDay in raw value format</returns>
        internal static string ToString(TimeOfDay time)
        {
            return time.ToString();
        }
    }
}
