//---------------------------------------------------------------------
// <copyright file="XmlPrimitiveConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Globalization;
    using System.Xml;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// The default primitive converter for Xml
    /// </summary>
    [ImplementationName(typeof(IXmlPrimitiveConverter), "Default", HelpText = "The default xml primitive converter")]
    [CLSCompliant(false)]
    public class XmlPrimitiveConverter : PrimitiveConverter<string>, IXmlPrimitiveConverter
    {
        private static readonly TimeSpan zeroOffset = new TimeSpan(0, 0, 0);

        /// <summary>
        /// Constant for atom format of local date times
        /// </summary>
        private const string AtomLocalDateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:sszzz";

        /// <summary>
        /// Constant for atom format of UTC date time
        /// </summary>
        private const string AtomUniversalDateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:ssZ";

        /// <summary>
        /// Serializes a null value
        /// </summary>
        /// <returns>A null string</returns>
        public override string SerializeNull()
        {
            return null;
        }

        /// <summary>
        /// Serialize a boolean value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(bool value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a byte value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(byte value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a char value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(char value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a decimal value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(decimal value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a double value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(double value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a float value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(float value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a short value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(short value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize an int value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(int value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a long value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(long value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize an sbyte value using XmlConvert
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
            return value;
        }

        /// <summary>
        /// Serialize a byte array using Convert.ToBase64String
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(byte[] value)
        {
            return Convert.ToBase64String(value);
        }

        /// <summary>
        /// Serialize a DateTime value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(DateTime value)
        {
            return PlatformHelper.ConvertDateTimeToString(value);
        }

        /// <summary>
        /// Serialize a Guid value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(Guid value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a DateTimeOffset value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(DateTimeOffset value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize a TimeSpan value using XmlConvert
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public override string Serialize(TimeSpan value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Deserialize a char value using XmlConvert
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
            return XmlConvert.ToDecimal(value);
        }

        /// <summary>
        /// Deserialize a double value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override double DeserializeDouble(string value)
        {
            return XmlConvert.ToDouble(value);
        }

        /// <summary>
        /// Deserialize a float value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override float DeserializeFloat(string value)
        {
            return XmlConvert.ToSingle(value);
        }

        /// <summary>
        /// Deserialize a short value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override short DeserializeInt16(string value)
        {
            return XmlConvert.ToInt16(value);
        }

        /// <summary>
        /// Deserialize an int value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override int DeserializeInt32(string value)
        {
            return XmlConvert.ToInt32(value);
        }

        /// <summary>
        /// Deserialize a long value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override long DeserializeInt64(string value)
        {
            return XmlConvert.ToInt64(value);
        }

        /// <summary>
        /// Deserialize an sbyte value using XmlConvert
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
            return value;
        }

        /// <summary>
        /// Deserialize a binary value using Convert.FromBase64String
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override byte[] DeserializeBinary(string value)
        {
            return Convert.FromBase64String(value);
        }

        /// <summary>
        /// Deserialize an DateTime value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override DateTime DeserializeDateTime(string value)
        {
            return PlatformHelper.ConvertStringToDateTime(value);
        }

        /// <summary>
        /// Deserialize an DateTime value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override DateTimeOffset DeserializeDateTimeOffset(string value)
        {
            return XmlConvert.ToDateTimeOffset(value);
        }

        /// <summary>
        /// Deserialize an Guid value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override Guid DeserializeGuid(string value)
        {
            return XmlConvert.ToGuid(value);
        }

        /// <summary>
        /// Deserialize an DateTime value using XmlConvert
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public override TimeSpan DeserializeTimeSpan(string value)
        {
            return XmlConvert.ToTimeSpan(value);
        }

        /// <summary>
        /// Convert a datetime primitive value to atom datetime offset format
        /// </summary>
        /// <param name="value">DateTime clr value</param>
        /// <returns>string format of atom datetime offset</returns>
        public string ConvertToAtomDateTimeOffset(DateTime value)
        {
            DateTimeOffset offset = new DateTimeOffset(value);
            return this.ConvertToAtomDateTimeOffset(offset);
        }

        /// <summary>
        /// Convert a datetimeoffset primitive value to atom datetime offset format
        /// </summary>
        /// <param name="value">DateTimeOffset clr value</param>
        /// <returns>string format of atom datetime offset</returns>
        public string ConvertToAtomDateTimeOffset(DateTimeOffset value)
        {
            if (value.Offset == zeroOffset)
            {
                return value.ToUniversalTime().ToString(AtomUniversalDateTimeOffsetFormat, CultureInfo.InvariantCulture);
            }
            else
            {
                return value.ToString(AtomLocalDateTimeOffsetFormat, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Determines whether the given serialized value represents null for the given type
        /// </summary>
        /// <param name="value">The serialized value to compare to null</param>
        /// <param name="targetType">The type that the value represents</param>
        /// <returns>Whether or not the value represents null</returns>
        protected override bool ValueIsNull(string value, Type targetType)
        {
            if (targetType == typeof(string) || targetType == typeof(byte[]))
            {
                return value == null;
            }

            return string.IsNullOrEmpty(value);
        }
    }
}
