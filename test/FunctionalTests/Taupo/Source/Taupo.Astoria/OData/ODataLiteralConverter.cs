//---------------------------------------------------------------------
// <copyright file="ODataLiteralConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Implementation of the odata literal converter contract
    /// </summary>
    [ImplementationName(typeof(IODataLiteralConverter), "Default")]
    [CLSCompliant(false)]
    public class ODataLiteralConverter : PrimitiveConverter<string>, IODataLiteralConverter
    {
        private const string GeographyPrefix = "geography";
        private const string GeometryPrefix = "geometry";
        private static readonly Regex GeographyRegex = new Regex("^" + GeographyPrefix + "'(.*)'$");
        private static readonly Regex GeometryRegex = new Regex("^" + GeometryPrefix + "'(.*)'$");

        private static readonly IDictionary<SpatialTypeKind, string> spatialLiteralPrefixMap = new Dictionary<SpatialTypeKind, string>()
        {
            { SpatialTypeKind.Geography, "geography" },
            { SpatialTypeKind.Geometry, "geometry" },
        };

        private static readonly IDictionary<Regex, SpatialTypeKind> spatialLiteralRegexMap = new Dictionary<Regex, SpatialTypeKind>()
        {
            { GeographyRegex, SpatialTypeKind.Geography },
            { GeometryRegex, SpatialTypeKind.Geometry },
        };

        private bool shouldCapitalizeIdentifiers = false;

        /// <summary>
        /// Gets or sets the spatial formatter.
        /// </summary>
        [InjectDependency]
        public IWellKnownTextSpatialFormatter SpatialFormatter { get; set; }

        /// <summary>
        /// Handles spatial primitives before delegating to the base type
        /// </summary>
        /// <param name="value">The primitive value to serialize</param>
        /// <param name="capitalizeIdentifiers">if set to <c>true</c> then identifiers for decimal, double, etc should be capitalized.</param>
        /// <returns>
        /// The wire representation of the primitive
        /// </returns>
        public string SerializePrimitive(object value, bool capitalizeIdentifiers)
        {
            if (value == null)
            {
                return this.SerializeNull();
            }

            string wellKnownText = null;
            if (this.SpatialFormatter.IfValid(false, f => this.SpatialFormatter.TryConvert(value, out wellKnownText)))
            {
                var kind = SpatialUtilities.InferSpatialTypeKind(value.GetType());
                string prefix;
                ExceptionUtilities.Assert(spatialLiteralPrefixMap.TryGetValue(kind, out prefix), "Could not find prefix for spatial type kind '{0}'", kind);
                return prefix + '\'' + wellKnownText + '\'';
            }

            bool oldCapitalizeValue = this.shouldCapitalizeIdentifiers;
            try
            {
                this.shouldCapitalizeIdentifiers = capitalizeIdentifiers;
                return base.SerializePrimitive(value);
            }
            finally
            {
                this.shouldCapitalizeIdentifiers = oldCapitalizeValue;
            }
        }

        /// <summary>
        /// Handles spatial primitives before delegating to the base type
        /// </summary>
        /// <param name="value">The wire representation of a primitive value</param>
        /// <param name="targetType">The type to deserialize into</param>
        /// <returns>A clr instance of the primitive value</returns>
        public override object DeserializePrimitive(string value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }

            foreach (var spatialRegexPair in spatialLiteralRegexMap)
            {
                var match = spatialRegexPair.Key.Match(value);
                if (match.Success)
                {
                    object spatialInstance = null;
                    if (this.SpatialFormatter.IfValid(false, f => this.SpatialFormatter.TryParse(match.Groups[1].Value, spatialRegexPair.Value, out spatialInstance)))
                    {
                        return spatialInstance;
                    }
                }
            }

            return base.DeserializePrimitive(value, targetType);
        }

        /// <summary>
        /// Returns whether the given type is a literal type
        /// </summary>
        /// <param name="targetType">The type to check</param>
        /// <returns>True if it is a literal type, false otherwise</returns>
        public bool IsLiteralType(Type targetType)
        {
            return this.IsPrimitiveType(targetType);
        }

        /// <summary>
        /// Returns whether the given type is a primitive type
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if it is a primitive type, false otherwise</returns>
        public override bool IsPrimitiveType(Type type)
        {
            SpatialTypeKind? kind;
            if (SpatialUtilities.TryInferSpatialTypeKind(type, out kind))
            {
                return true;
            }

            return base.IsPrimitiveType(type);
        }

        /// <summary>
        /// Serializes a null value
        /// </summary>
        /// <returns>A null string</returns>
        public override string SerializeNull()
        {
            return "null";
        }

        /// <summary>
        /// Serialize a boolean value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(bool value)
        {
            return (bool)value ? "true" : "false";
        }

        /// <summary>
        /// Serialize a byte value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(byte value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a char value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(char value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a decimal value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(decimal value)
        {
            var identifier = this.shouldCapitalizeIdentifiers ? "M" : "m";
            return string.Concat(XmlConvert.ToString((decimal)value), identifier);
        }

        /// <summary>
        /// Serialize a double value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(double value)
        {
            string result = XmlConvert.ToString(value);
            if (!double.IsInfinity(value) && !double.IsNaN(value))
            {
                var identifier = this.shouldCapitalizeIdentifiers ? "D" : "d";
                result += identifier;
            }

            return result;
        }

        /// <summary>
        /// Serialize a float value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(float value)
        {
            string result = XmlConvert.ToString(value);
            if (!float.IsInfinity(value) && !float.IsNaN(value) && !result.Contains(".") && !result.Contains("E"))
            {
                result += ".0";
            }

            var identifier = this.shouldCapitalizeIdentifiers ? "F" : "f";
            return string.Concat(result, identifier);
        }

        /// <summary>
        /// Serialize a short value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(short value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a int value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(int value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a long value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(long value)
        {
            var identifier = this.shouldCapitalizeIdentifiers ? "L" : "l";
            return string.Concat(XmlConvert.ToString(value), identifier);
        }

        /// <summary>
        /// Serialize a sbyte value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        [CLSCompliant(false)]
        public override string Serialize(sbyte value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a string value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(string value)
        {
            return string.Concat("'", value.Replace("'", "''"), "'");
        }

        /// <summary>
        /// Serialize a byte[] value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(byte[] value)
        {
            return FormatByteArrayForKey(value);
        }

        /// <summary>
        /// Serialize a datetime value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(DateTime value)
        {
            return string.Concat("datetime'", PlatformHelper.ConvertDateTimeToString(value), "'");
        }

        /// <summary>
        /// Serialize a guid value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(Guid value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a datetimeoffset value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(DateTimeOffset value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a timespan value 
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(TimeSpan value)
        {
            return string.Concat("duration'", XmlConvert.ToString(value), "'");
        }

        /// <summary>
        /// Deserialize a bool value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override bool DeserializeBool(string value)
        {
            return XmlConvert.ToBoolean(value);
        }

        /// <summary>
        /// Deserialize a byte value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override byte DeserializeByte(string value)
        {
            return XmlConvert.ToByte(value);
        }

        /// <summary>
        /// Deserialize a char value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override char DeserializeChar(string value)
        {
            return XmlConvert.ToChar(value);
        }

        /// <summary>
        /// Deserialize a decimal value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override decimal DeserializeDecimal(string value)
        {
            return (decimal)KeyExpressionParser.KeyStringToPrimitive(value);
        }

        /// <summary>
        /// Deserialize a double value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override double DeserializeDouble(string value)
        {
            return (double)KeyExpressionParser.KeyStringToPrimitive(value);
        }

        /// <summary>
        /// Deserialize a float value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override float DeserializeFloat(string value)
        {
            return (float)KeyExpressionParser.KeyStringToPrimitive(value);
        }

        /// <summary>
        /// Deserialize a int16 value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override short DeserializeInt16(string value)
        {
            return XmlConvert.ToInt16(value);
        }

        /// <summary>
        /// Deserialize a int32 value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override int DeserializeInt32(string value)
        {
            return XmlConvert.ToInt32(value);
        }

        /// <summary>
        /// Deserialize a int64 value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override long DeserializeInt64(string value)
        {
            return (long)KeyExpressionParser.KeyStringToPrimitive(value);
        }

        /// <summary>
        /// Deserialize a sbyte value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        [CLSCompliant(false)]
        public override sbyte DeserializeSByte(string value)
        {
            return XmlConvert.ToSByte(value);
        }

        /// <summary>
        /// Deserialize a string value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override string DeserializeString(string value)
        {
            return (string)KeyExpressionParser.KeyStringToPrimitive(value);
        }

        /// <summary>
        /// Deserialize a binary value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
            Justification = "Will not be disposed more than once")]
        public override byte[] DeserializeBinary(string value)
        {
            if (value.StartsWith("X", StringComparison.Ordinal))
            {
                value = value.Remove(0, 1);
            }

            if (value.StartsWith("binary", StringComparison.Ordinal))
            {
                value = value.Remove(0, "binary".Length);
            }

            value = StripQuotes(value);

            byte[] buffer = new byte[value.Length / 2];
            using (StringReader underlyingReader = new StringReader("<foo>" + value + "</foo>"))
            {
                using (XmlReader reader = XmlReader.Create(underlyingReader, new XmlReaderSettings() { CloseInput = false }))
                {
                    reader.Read();
                    reader.ReadElementContentAsBinHex(buffer, 0, buffer.Length);
                }
            }

            return buffer;
        }

        /// <summary>
        /// Deserialize a datetime value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override DateTime DeserializeDateTime(string value)
        {
            return (DateTime)KeyExpressionParser.KeyStringToPrimitive(value);
        }

        /// <summary>
        /// Deserialize a guid value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override Guid DeserializeGuid(string value)
        {
            return (Guid)KeyExpressionParser.KeyStringToPrimitive(value);
        }

        /// <summary>
        /// Deserialize a datetimeoffset value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override DateTimeOffset DeserializeDateTimeOffset(string value)
        {
            return (DateTimeOffset)KeyExpressionParser.KeyStringToPrimitive(value);
        }

        /// <summary>
        /// Deserialize a timespan value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override TimeSpan DeserializeTimeSpan(string value)
        {
            return (TimeSpan)KeyExpressionParser.KeyStringToPrimitive(value);
        }

        /// <summary>
        /// Determines whether the given serialized value represents null for the given type
        /// </summary>
        /// <param name="value">The serialized value to compare to null</param>
        /// <param name="targetType">The type that the value represents</param>
        /// <returns>Whether or not the value represents null</returns>
        protected override bool ValueIsNull(string value, Type targetType)
        {
            if (targetType == typeof(string))
            {
                return value == null;
            }

            return string.IsNullOrEmpty(value);
        }
        
        /// <summary>
        /// Formats the specified object into a value suitable for a uri.
        /// </summary>
        /// <param name="bytes">Origibal byte[] value.</param>
        /// <returns>Formatted string</returns>
        private static string FormatByteArrayForKey(byte[] bytes)
        {
            ExceptionUtilities.CheckArgumentNotNull(bytes, "bytes");

            if (bytes.Length == 0)
            {
                return "binary''";
            }
            else
            {
                return string.Concat("binary'", FormatByteArrayToString(bytes), "'");
            }
        }

        /// <summary>
        /// Formats the specified object into a string.
        /// </summary>
        /// <param name="bytes">Origibal byte[] value.</param>
        /// <returns>Formatted string</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
            Justification = "Will not be disposed more than once")]
        private static string FormatByteArrayToString(byte[] bytes)
        {
            // This is what Astoria product code does (for comparison purpose)
            ////        /// <summary>Constant table of nibble-to-hex convertion values.</summary>
            ////        private const string HexValues = "0123456789ABCDEF";

            ////         internal static string ConvertByteArrayToKeyString(byte[] byteArray)
            ////        {
            ////            StringBuilder hexBuilder = new StringBuilder(3 + byteArray.Length * 2);
            ////            hexBuilder.Append(XmlConstants.XmlBinaryPrefix);
            ////            hexBuilder.Append("'");
            ////            for (int i = 0; i < byteArray.Length; i++)
            ////            {
            ////                hexBuilder.Append(HexValues[byteArray[i] >> 4]);
            ////                hexBuilder.Append(HexValues[byteArray[i] & 0x0F]);
            ////            }

            ////            hexBuilder.Append("'");
            ////            return hexBuilder.ToString();
            ////        }

            string document;
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, new XmlWriterSettings() { CloseOutput = false }))
                {
                    writer.WriteStartElement("foo");
                    writer.WriteBinHex(bytes, 0, bytes.Length);
                    writer.WriteEndElement();
                    writer.Flush();
                }

                document = stringWriter.GetStringBuilder().ToString();
            }

            int startIndex = document.IndexOf("oo>", StringComparison.Ordinal) + 3;
            return document.Substring(startIndex, document.IndexOf("</foo", StringComparison.Ordinal) - startIndex);
        }

        /// <summary>
        /// Strips the quotes from a string.
        /// </summary>
        /// <param name="text">Origibal string value.</param>
        /// <returns>Updated string</returns>
        private static string StripQuotes(string text)
        {
            ExceptionUtilities.CheckArgumentNotNull(text, "text");
            ExceptionUtilities.Assert(text.Length >= 2, "Text is too short: [" + text + "]");

            text = text.Substring(1, text.Length - 2);
            return text.Replace("''", "'");
        }
    }
}