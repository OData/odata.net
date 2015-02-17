//---------------------------------------------------------------------
// <copyright file="KeyExpressionParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents the converter for key expressions
    /// </summary>
    public static class KeyExpressionParser
    {
        public static readonly Regex DateTimeRegex = new Regex("^(?i:datetime)'(.*)'$");
        public static readonly Regex GuidRegex = new Regex("^(?i:guid)'(.*)'$");
        public static readonly Regex LongRegex = new Regex("^(.*)[lL]$");
        public static readonly Regex DecimalRegex = new Regex("^(.*)[mM]$");
        public static readonly Regex DoubleInfinityRegex = new Regex("^-?INF$"); // need to check this first so that we don't mistake it for a float
        public static readonly Regex DoubleNanRegex = new Regex("^NaN$");
        public static readonly Regex FloatRegex = new Regex("^(.*)[fF]$");
        public static readonly Regex DoubleRegex = new Regex("^(.*)[dD]$");
        public static readonly Regex StringRegex = new Regex("^'(.*)'$");
        public static readonly Regex BoolRegex = new Regex("^(true|false)$");
        public static readonly Regex BinaryRegex = new Regex("^(?i:x|binary)'(.*)'$");
        public static readonly Regex DateTimeOffsetRegex = new Regex("^(?i:datetimeoffset)'(.*)'$");
        public static readonly Regex TimeSpanRegex = new Regex("^(?i:duration)'(.*)'$");

        /// <summary>
        /// Converts a string to a primitive type string.
        /// </summary>
        /// <param name="key">Original string value.</param>
        /// <returns>Formatted string</returns>
        public static object KeyStringToPrimitive(string key)
        {
            Match match;

            if (string.IsNullOrEmpty(key))
            {
                return key;
            }
            else if ((match = DateTimeRegex.Match(key)).Success)
            {
                return PlatformHelper.ConvertStringToDateTime(Uri.UnescapeDataString(match.Groups[1].Value));
            }
            else if ((match = GuidRegex.Match(key)).Success)
            {
                return XmlConvert.ToGuid(match.Groups[1].Value);
            }
            else if ((match = LongRegex.Match(key)).Success)
            {
                return XmlConvert.ToInt64(match.Groups[1].Value);
            }
            else if ((match = DecimalRegex.Match(key)).Success)
            {
                return XmlConvert.ToDecimal(match.Groups[1].Value);
            }
            else if (DoubleInfinityRegex.IsMatch(key) || DoubleNanRegex.IsMatch(key))
            {
                return XmlConvert.ToDouble(key);
            }
            else if ((match = FloatRegex.Match(key)).Success)
            {
                return XmlConvert.ToSingle(Uri.UnescapeDataString(match.Groups[1].Value));
            }
            else if ((match = DoubleRegex.Match(key)).Success)
            {
                return XmlConvert.ToDouble(Uri.UnescapeDataString(match.Groups[1].Value));
            }
            else if ((match = StringRegex.Match(key)).Success)
            {
                return match.Groups[1].Value;
            }
            else if ((match = DateTimeOffsetRegex.Match(key)).Success)
            {
                return XmlConvert.ToDateTimeOffset(Uri.UnescapeDataString(match.Groups[1].Value));
            }
            else if ((match = TimeSpanRegex.Match(key)).Success)
            {
                return XmlConvert.ToTimeSpan(Uri.UnescapeDataString(match.Groups[1].Value));
            }
            else
            {
                throw new TaupoInvalidOperationException("Could not parse key string: '" + key + "'");
            }
        }
    }
}
