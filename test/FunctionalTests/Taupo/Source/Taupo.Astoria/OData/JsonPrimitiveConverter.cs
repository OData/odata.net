//---------------------------------------------------------------------
// <copyright file="JsonPrimitiveConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Json;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// The default primitive converter for Json
    /// </summary>
    [ImplementationName(typeof(IJsonPrimitiveConverter), "Default", HelpText = "The default json primitive converter")]
    [CLSCompliant(false)]
    public class JsonPrimitiveConverter : PrimitiveConverter<string>, IJsonPrimitiveConverter
    {
        /// <summary> Json datetime format </summary>
        private const string JsonDateTimeFormat = @"""\/Date({0})\/""";
        private const string JsonDateTimeOffsetFormat = @"""\/Date({0}{1}{2})\/""";
        
        /// <summary>
        /// Gets or sets the max protocol version of the server.
        /// </summary>
        [InjectTestParameter("MaxProtocolVersion", DefaultValueDescription = "MaxProtocolVersion of the server")]
        public DataServiceProtocolVersion MaxProtocolVersion { get; set; }

        /// <summary>
        /// Normalize DateTime and DateTimeOffset values
        /// </summary>
        /// <param name="value">Value to normalize</param>
        /// <returns>Normalized DateTime values</returns>
        public object Normalize(object value)
        {
            if (value == null)
            {
                return value;
            }

            var clrType = value.GetType();
            if (clrType == typeof(DateTime))
            {
                DateTime dateTimeValue = (DateTime)value;
                value = this.ConvertToDateTime(this.ConvertToMillisecondsSinceEpoch(dateTimeValue));
            }
            else if (clrType == typeof(DateTimeOffset))
            {
                DateTimeOffset dateTimeOffsetValue = (DateTimeOffset)value;
                var ticks = this.ConvertToMillisecondsSinceEpoch(dateTimeOffsetValue);
                var offset = (long)dateTimeOffsetValue.Offset.TotalMinutes;
                value = this.ConvertToDateTimeOffset(ticks, offset);
            }

            return value;
        }

        /// <summary>
        /// Converts a primitive value represented as a string in json back into the clr representation
        /// </summary>
        /// <param name="valueToConvert">The value to convert</param>
        /// <param name="expectedType">The expected type. Must be guid, datetime, or binary</param>
        /// <param name="convertedValue">The converted value</param>
        /// <returns>True if conversion was possible, false otherwise</returns>
        public bool TryConvertFromString(string valueToConvert, Type expectedType, out object convertedValue)
        {
            if (this.IsPrimitiveType(expectedType))
            {
                try
                {
                    convertedValue = this.DeserializePrimitive(valueToConvert, expectedType);
                }
                catch (TargetInvocationException)
                {
                    convertedValue = null;
                    return false;
                }

                return true;
            }

            convertedValue = null;
            return false;
        }

        /// <summary>
        /// Serialize the bool value
        /// </summary>
        /// <param name="value">bool value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(bool value)
        {
            return XmlConvert.ToString(value);
        }

        /// <summary>
        /// Serialize the int value
        /// </summary>
        /// <param name="value">int value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Serialize the float value
        /// </summary>
        /// <param name="value">float value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(float value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                return '"' + value.ToString(null, CultureInfo.InvariantCulture) + '"';
            }
            else
            {
                // float.ToString() supports a max scale of six,
                // whereas float.MinValue and float.MaxValue have 8 digits scale. Hence we need
                // to use XmlConvert in all other cases, except infinity
                return XmlConvert.ToString(value);
            }
        }

        /// <summary>
        /// Serialize the short value
        /// </summary>
        /// <param name="value">short value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(short value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Serialize the long value
        /// </summary>
        /// <param name="value">long value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(long value)
        {
            // Since Json only supports number, we need to convert long into string to prevent data loss
            return '"' + value.ToString(CultureInfo.InvariantCulture) + '"';
        }

        /// <summary>
        /// Serialize the double value
        /// </summary>
        /// <param name="value">double value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                return '"' + value.ToString(null, CultureInfo.InvariantCulture) + '"';
            }
            else
            {
                // double.ToString() supports a max scale of 14,
                // whereas float.MinValue and float.MaxValue have 16 digits scale. Hence we need
                // to use XmlConvert in all other cases, except infinity
                return XmlConvert.ToString(value);
            }
        }

        /// <summary>
        /// Serialize the Guid value
        /// </summary>
        /// <param name="value">double value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(Guid value)
        {
            return '"' + value.ToString() + '"';
        }

        /// <summary>
        /// Serialize the decimal value
        /// </summary>
        /// <param name="value">decimal value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(decimal value)
        {
            // Since Json doesn't have decimal support (it only has one data type - number),
            // we need to convert decimal to string to prevent data loss
            return '"' + value.ToString(CultureInfo.InvariantCulture) + '"';
        }

        /// <summary>
        /// Serialize the DateTime value
        /// </summary>
        /// <param name="value">dateTime value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(DateTime value)
        {
                return "\"" + PlatformHelper.ConvertDateTimeToString(value) + "\"";
        }

        /// <summary>
        /// Serialize the DateTimeOffset value
        /// </summary>
        /// <param name="value">DateTimeOffset value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(DateTimeOffset value)
        {
            return "\"" + XmlConvert.ToString(value) + "\"";
        }

        /// <summary>
        /// Serialize the TimeSpan value
        /// </summary>
        /// <param name="value">TimeSpan value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(TimeSpan value)
        {
            return '"' + XmlConvert.ToString(value) + '"';
        }

        /// <summary>
        /// Serialize the byte value
        /// </summary>
        /// <param name="value">byte value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(byte value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Serialize the sbyte value
        /// </summary>
        /// <param name="value">sbyte value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(sbyte value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Serialize the string value
        /// </summary>
        /// <param name="value">string value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(string value)
        {
            if (value == null)
            {
                return this.SerializeNull();
            }

            return '"' + JsonUtilities.BuildEscapedJavaScriptString(value) + '"';
        }

        /// <summary>
        /// Serialize the binary value
        /// </summary>
        /// <param name="value">binary value to be serialized</param>
        /// <returns>The serialized value</returns>
        public override string Serialize(byte[] value)
        {
            if (value == null)
            {
                return this.SerializeNull();
            }

            return '"' + Convert.ToBase64String(value) + '"';
        }

        /// <summary>
        /// Serializes a null value.
        /// </summary>
        /// <returns>The serialized value</returns>
        public override string SerializeNull()
        {
            return "null";
        }

        /// <summary>
        /// Serializes the character value.
        /// </summary>
        /// <param name="value">The character to serialize.</param>
        /// <returns>The serialzied value.</returns>
        public override string Serialize(char value)
        {
            return '"' + JsonUtilities.ToCharAsUnicode(value) + '"';
        }

        /// <summary>
        /// Deserializes a boolean value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized boolean value</returns>
        public override bool DeserializeBool(string value)
        {
            return XmlConvert.ToBoolean(value);
        }

        /// <summary>
        /// Deserializes a byte value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized byte value</returns>
        public override byte DeserializeByte(string value)
        {
            return byte.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Deserializes a character value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized character value</returns>
        public override char DeserializeChar(string value)
        {
            return value[0];
        }

        /// <summary>
        /// Deserializes a decimal value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized decimal value</returns>
        public override decimal DeserializeDecimal(string value)
        {
            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Deserializes a double value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized double value</returns>
        public override double DeserializeDouble(string value)
        {
            return double.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Deserializes a float value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized float value</returns>
        public override float DeserializeFloat(string value)
        {
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Deserializes a int16 value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized int16 value</returns>
        public override short DeserializeInt16(string value)
        {
            return short.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Deserializes a int32 value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized int32 value</returns>
        public override int DeserializeInt32(string value)
        {
            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Deserializes a int64 value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized int64 value</returns>
        public override long DeserializeInt64(string value)
        {
            return long.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Deserializes a sbyte value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized sbyte value</returns>
        public override sbyte DeserializeSByte(string value)
        {
            return sbyte.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Deserializes a string value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized string value</returns>
        public override string DeserializeString(string value)
        {
            return value;
        }

        /// <summary>
        /// Deserializes a binary value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized binary value</returns>
        public override byte[] DeserializeBinary(string value)
        {
            // binary values are base-64 encoded
            return Convert.FromBase64String(value);
        }

        /// <summary>
        /// Deserializes a datetime value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized datetime value</returns>
        public override DateTime DeserializeDateTime(string value)
        {
            // datetimes are formatted as a number of milliseconds since midnight on Jan 1st, 1970, wrapped in a string.
            Match m = ODataConstants.JsonDateTimeTickFormatRegex.Match(value);
            if (m.Success)
            {
                ExceptionUtilities.Assert(m.Success, "Value was not formatted to match the date-time regex: '{0}'", value);
                string ticksStr = m.Groups["ticks"].Value;
                long ticks;
                ExceptionUtilities.Assert(long.TryParse(ticksStr, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out ticks), "Tick count was not a number: '{0}'", ticksStr);
                return this.ConvertToDateTime(ticks);
            }
            else
            {
                DateTime parsedDate = DateTime.Now;
                try
                {
                    parsedDate = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
                }
                catch (Exception ex)
                {
                    throw new TaupoInvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, "Value was not formatted to match the date-time ISO Format: '{0}'. Error : {1}", value, ex.ToString()));
                }

                ExceptionUtilities.Assert(
                    this.MaxProtocolVersion == DataServiceProtocolVersion.Unspecified, 
                    "Datetime was returned in ISO format from the server when MPV is :{0}", 
                    this.MaxProtocolVersion.ToString());

                return parsedDate;
            }
        }

        /// <summary>
        /// Deserializes a datetimeoffset value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized datetimeoffset value</returns>
        public override DateTimeOffset DeserializeDateTimeOffset(string value)
        {
            // datetimeoffsets are formatted as a number of milliseconds since mindight on Jan 1st, 1970 with an offset in minutes            
            Match m = ODataConstants.JsonDateTimeOffsetTickFormatRegex.Match(value);
            if (m.Success)
            {
                string ticksStr = m.Groups["ticks"].Value;
                long ticks;
                ExceptionUtilities.Assert(long.TryParse(ticksStr, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out ticks), "Tick count was not a number: '{0}'", ticksStr);

                string offsetStr = m.Groups["offset"].Value;
                long minutesOffset;
                ExceptionUtilities.Assert(long.TryParse(offsetStr, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out minutesOffset), "Offset was not a number: '{0}'", offsetStr);

                return this.ConvertToDateTimeOffset(ticks, minutesOffset);
            }
            else
            {
                DateTimeOffset parsedDateTimeOffset;
                ExceptionUtilities.Assert(DateTimeOffset.TryParse(value, out parsedDateTimeOffset), "Value was not formatted to match the date-time ISO Format: '{0}'", value);
                ExceptionUtilities.Assert(this.MaxProtocolVersion >= DataServiceProtocolVersion.V4, "Datetime was returned in ISO format from the server when MPV is :{0}", this.MaxProtocolVersion.ToString());
                return parsedDateTimeOffset;
            }
        }

        /// <summary>
        /// Deserializes a timespan value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized timespan value</returns>
        public override TimeSpan DeserializeTimeSpan(string value)
        {
            return XmlConvert.ToTimeSpan(value);
        }

        /// <summary>
        /// Deserializes a guid value.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>The deserialized guid value</returns>
        public override Guid DeserializeGuid(string value)
        {
            // guids are formatted in a way handled by the guid constructor automatically
            return new Guid(value);
        }

        /// <summary>
        /// Converts a datetime into a number of milliseconds since the Epoch datetime (midnight Jan 1st, 1970 UTC)
        /// </summary>
        /// <param name="dateTime">The datetime to convert</param>
        /// <returns>The number of milliseconds since the epoch for the given datetime</returns>
        internal long ConvertToMillisecondsSinceEpoch(DateTime dateTime)
        {
            return (dateTime.Ticks - ODataConstants.DateTimeTicksForEpoch) / ODataConstants.TicksPerMillisecond;
        }

        /// <summary>
        /// Converts a datetimeoffset into a number of milliseconds since the Epoch datetime (midnight Jan 1st, 1970 UTC)
        /// </summary>
        /// <param name="offset">The datetimeoffset to convert</param>
        /// <returns>The number of milliseconds since the epoch for the given datetime</returns>
        internal long ConvertToMillisecondsSinceEpoch(DateTimeOffset offset)
        {
            return (offset.Ticks - ODataConstants.DateTimeTicksForEpoch) / ODataConstants.TicksPerMillisecond;
        }

        /// <summary>
        /// Converts a count of the milliseconds since the Epoch datetime into a datetime
        /// </summary>
        /// <param name="millisecondsSinceEpoch">The number of milliseconds since midnight Jan 1st, 1970 UTC</param>
        /// <returns>The converted datetime</returns>
        internal DateTime ConvertToDateTime(long millisecondsSinceEpoch)
        {
            return new DateTime((millisecondsSinceEpoch * ODataConstants.TicksPerMillisecond) + ODataConstants.DateTimeTicksForEpoch, DateTimeKind.Utc);
        }

        /// <summary>
        /// Converts a count of the milliseconds since the Epoch datetime into a datetimeoffset
        /// </summary>
        /// <param name="millisecondsSinceEpoch">The number of milliseconds since midnight Jan 1st, 1970 UTC</param>
        /// <param name="offsetInMinutes">The offset in minutes</param>
        /// <returns>The converted datetimeoffset</returns>
        internal DateTimeOffset ConvertToDateTimeOffset(long millisecondsSinceEpoch, long offsetInMinutes)
        {
            return new DateTimeOffset((millisecondsSinceEpoch * ODataConstants.TicksPerMillisecond) + ODataConstants.DateTimeTicksForEpoch, TimeSpan.FromMinutes(offsetInMinutes));
        }

        /// <summary>
        /// Determines whether the serialized value represents null.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>Whether the value is null</returns>
        protected override bool ValueIsNull(string value, Type targetType)
        {
            return value == null || value == "null";
        }
    }
}