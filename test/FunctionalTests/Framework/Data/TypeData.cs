//---------------------------------------------------------------------
// <copyright file="TypeData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Data
{
    using System;
    using System.Collections.Generic;
#if !ClientSKUFramework
    using System.Data.Linq;
#endif
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Provides reference data about interesting types system.
    /// </summary>
    public class TypeData
    {
        /// <summary>Sample values for this type.</summary>
        private object[] sampleValues;

        /// <summary>Referenced CLR type.</summary>
        private Type clrType;

        /// <summary>Interesting values for testing types.</summary>
        private static TypeData[] values;

        /// <summary>Hide the constructor.</summary>
        private TypeData()
        {
        }

        /// <summary>Interesting values for testing types.</summary>
        public static TypeData[] Values
        {
            get
            {
                if (values == null)
                {
                    values = new TypeData[] {
                        ForSamples(typeof(bool), true, false),
                        ForSamples(typeof(byte), 0, 1, 255),
                        ForSamples(typeof(SByte), 0, 1, -1, sbyte.MaxValue, sbyte.MinValue),
                        ForSamples(typeof(char), '\x0', '\x1', 'A', 'a', '\xFF', '\x100'),
                        ForSamples(typeof(decimal), Decimal.MaxValue, Decimal.MinValue, Decimal.One, Decimal.Zero, Decimal.MinValue, Decimal.MaxValue),
                        ForSamples(typeof(double), 0, 1, -0.1, Double.Epsilon, Double.MaxValue, Double.MinValue, Double.NegativeInfinity, Double.PositiveInfinity, Double.NaN, 7E-06, 9e+09, 9E+16), /*last 2 cases are values with no periods in them*/
                        ForSamples(typeof(float), 0, 1, -0.1, Single.Epsilon, Single.MaxValue, Single.MinValue, Single.NegativeInfinity, Single.PositiveInfinity, Single.NaN, 7E-06f, 9E+09f), /*last 2 cases are values with no periods in them*/
                        ForSamples(typeof(UriComponents), ValuesForUriComponents()),
                        ForSamples(typeof(Int16), 0, 1, -1, Int16.MaxValue, Int16.MinValue),
                        ForSamples(typeof(Int32), 0, 1, -1, Int32.MaxValue, Int32.MinValue),
                        ForSamples(typeof(Int64), 0, 1, -1, Int64.MaxValue, Int64.MinValue),
                        ForSamples(typeof(UInt16), 0, 1, UInt16.MaxValue, UInt16.MinValue),
                        ForSamples(typeof(UInt32), 0, 1, UInt32.MaxValue, UInt32.MinValue),
                        ForSamples(typeof(UInt64), 0, 1, UInt64.MaxValue, UInt64.MinValue),
                        ForSamples(typeof(string), StringData.ToObjectArray(StringData.Values)),
                        ForSamples(typeof(char[]), ConvertToCharArrays(StringData.Values)),
                        ForSamples(typeof(byte[]), null, new byte[0], new byte[] { 0 }, new byte[] { 0, 1, byte.MinValue, byte.MaxValue }, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }),
                        ForSamples(typeof(sbyte[]), null, new sbyte[0], new sbyte[] { 0 }, new sbyte[] { 0, 1, sbyte.MinValue, sbyte.MaxValue }),
                        ForSamples(typeof(IntPtr), IntPtr.Zero),
                        ForSamples(typeof(UIntPtr), UIntPtr.Zero),
                        ForSamples(typeof(DateTimeOffset), DateTimeOffset.MaxValue, DateTimeOffset.MinValue, DateTimeOffset.Now, DateTimeOffset.UtcNow),
                        ForSamples(typeof(TimeSpan), TimeSpan.MaxValue, TimeSpan.MinValue, TimeSpan.FromDays(1.5)),
                        ForSamples(typeof(Uri), new Uri("foo", UriKind.Relative), new Uri("foo://foo", UriKind.Absolute), new Uri("foo://foo", UriKind.RelativeOrAbsolute)),
                        ForSamples(typeof(Guid), Guid.Empty, Guid.NewGuid()),
#if !ClientSKUFramework

                        ForSamples(typeof(Binary), null, new Binary(new byte[0]), new Binary(new byte[] { 1, 2, byte.MaxValue })),
#endif

                        ForSamples(typeof(XElement), null, XElement.Parse("<xelement>content<nested><!--comment--></nested> </xelement>")),
                    };
                    values = AddNullableVersions(values);
                }
                return values;
            }
        }

        /// <summary>Finds the TypeData that describes the specified edm type.</summary>
        /// <param name="edmTypeName">Edm type to describe.</param>
        /// <returns>The TypeData instance that describes the specified edm type.</returns>
        public static TypeData FindForType(string edmTypeName)
        {


            TypeData result = TypeData.Values.Where(d => edmTypeName.EndsWith(d.ClrType.Name)
                || (d.clrType == typeof(byte[]) && edmTypeName == "Edm.Binary")).SingleOrDefault();
            if (result == null)
            {
                throw new ArgumentException("Unable to find TypeData for " + edmTypeName);
            }

            return result;
        }

        /// <summary>Finds the TypeData that describes the specified type.</summary>
        /// <param name="type">Type to describe.</param>
        /// <returns>The TypeData instance that describes the specified type.</returns>
        public static TypeData FindForType(Type type)
        {
#if ClientFramework
            TestUtil.CheckArgumentNotNull(type, "type");
#endif
            TypeData result = TypeData.Values.Where(d => d.ClrType == type).SingleOrDefault();
            if (result == null)
            {
                throw new ArgumentException("Unable to find TypeData for " + type);
            }
            return result;
        }

        public bool IsEqualityComparableTo(TypeData other)
        {
#if ClientFramework
            TestUtil.CheckArgumentNotNull(other, "other");
#endif

            if (this == other)
            {
                return true;
            }

            // Nullability doesn't matter for equality comparisons.
            Type thisType = Nullable.GetUnderlyingType(this.clrType) ?? this.clrType;
            Type otherType = Nullable.GetUnderlyingType(other.clrType) ?? other.clrType;

            if (thisType == otherType) return true;
            if (IsTypeIntegral(thisType) && IsTypeIntegral(otherType)) return true;
            if (IsTypeReal(thisType) && IsTypeReal(otherType)) return true;
            if (thisType == typeof(decimal) && IsTypeIntegral(otherType)) return true;
            if (otherType == typeof(decimal) && IsTypeIntegral(thisType)) return true;
            
            // C# lossy promotability rules allow non-reals to be promoted to reals - yikes.
            if (IsTypeIntegral(thisType) && IsTypeReal(otherType)) return true;
            if (IsTypeReal(thisType) && IsTypeIntegral(otherType)) return true;

            return false;
        }

        public bool IsOrderComparableTo(TypeData other)
        {
#if ClientFramework
            TestUtil.CheckArgumentNotNull(other, "other");
#endif

            // Nullability doesn't matter for ordering comparisons.
            Type thisType = Nullable.GetUnderlyingType(this.clrType) ?? this.clrType;
            Type otherType = Nullable.GetUnderlyingType(other.clrType) ?? other.clrType;

            if (thisType == typeof(string) && otherType == typeof(string)) return true;
            if (thisType == typeof(bool) && otherType == typeof(bool)) return true;
            if (thisType == typeof(Guid) && otherType == typeof(Guid)) return true;
            if (IsTypeIntegral(thisType) && IsTypeIntegral(otherType)) return true;
            if (IsTypeReal(thisType) && IsTypeReal(otherType)) return true;
            if (thisType == typeof(decimal) && IsTypeIntegral(otherType)) return true;
            if (otherType == typeof(decimal) && IsTypeIntegral(thisType)) return true;
            if (thisType == typeof(decimal) && otherType == typeof(decimal)) return true;

            // C# lossy promotability rules allow non-reals to be promoted to reals - yikes.
            if (IsTypeIntegral(thisType) && IsTypeReal(otherType)) return true;
            if (IsTypeReal(thisType) && IsTypeIntegral(otherType)) return true;

            // Other comparable types.
            if (thisType == typeof(DateTime) && otherType == typeof(DateTime)) return true;
            if (thisType == typeof(DateTimeOffset) && otherType == typeof(DateTimeOffset)) return true;
            if (thisType == typeof(TimeSpan) && otherType == typeof(TimeSpan)) return true;

            return false;
        }

        public static bool IsTypeIntegral(Type type)
        {
#if ClientFramework
            TestUtil.CheckArgumentNotNull(type, "type");
#endif
            TypeCode code = Type.GetTypeCode(type);
            return
                code == TypeCode.SByte || code == TypeCode.Int16 || code == TypeCode.Int32 || code == TypeCode.Int64 ||
                code == TypeCode.Byte || code == TypeCode.UInt16 || code == TypeCode.UInt32 || code == TypeCode.UInt64;
        }

        public static bool IsTypeReal(Type type)
        {
#if ClientFramework
            TestUtil.CheckArgumentNotNull(type, "type");
#endif
            TypeCode code = Type.GetTypeCode(type);
            return code == TypeCode.Single || code == TypeCode.Double;
        }

        /// <summary>Checks whether the specified <paramref name="type"/> represents a numeric type.</summary>
        /// <param name="type"><see cref="Type"/> to check.</param>
        /// <returns>Whether the specified <paramref name="type"/> represents a numeric type.</returns>
        public static bool IsTypeNumeric(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return IsTypeNumeric(underlyingType);
            }

            return
                type == typeof(Byte) ||
                type == typeof(SByte) ||
                type == typeof(Int16) ||
                type == typeof(UInt16) ||
                type == typeof(Int32) ||
                type == typeof(UInt32) ||
                type == typeof(Int64) ||
                type == typeof(UInt64) ||
                type == typeof(double) ||
                type == typeof(float) ||
                type == typeof(decimal);
        }

        /// <summary>Referenced CLR type.</summary>
        public Type ClrType
        {
            get
            {
                return this.clrType;
            }
        }

        /// <summary>The default MIME type for this type of data.</summary>
        public string DefaultContentType
        {
            get
            {
                if (this.ClrType == typeof(byte[])
#if !ClientSKUFramework
 || this.ClrType == typeof(Binary)
#endif
)
                {
                    return System.Net.Mime.MediaTypeNames.Application.Octet;
                }
                else
                {
                    return System.Net.Mime.MediaTypeNames.Text.Plain;
                }
            }
        }

        /// <summary>Whether the described type is supported by Astoria.</summary>
        public bool IsTypeSupported
        {
            get
            {
                return IsTypeSupportedInternal(this.clrType);
            }
        }

        /// <summary>Whether the described type is supported by Astoria.</summary>
        public bool IsTypeSupportedAsKey
        {
            get
            {
                return IsTypeSupportedAsKeyInternal(this.clrType);
            }
        }

        /// <summary>Returns a non-null sample value.</summary>
        public object NonNullValue
        {
            get
            {
                for (int i = 0; i < this.sampleValues.Length; i++)
                {
                    if (this.sampleValues[i] != null)
                    {
                        return this.sampleValues[i];
                    }
                }

                throw new Exception("There is sample non-null value for " + this.ToString());
            }
        }

        private static bool IsTypeSupportedAsKeyInternal(Type type)
        {
            return IsTypeSupportedInternal(type) && !type.IsGenericType;
        }

        private static bool IsTypeSupportedInternal(Type type)
        {
            Type elementType = Nullable.GetUnderlyingType(type);
            if (elementType != null)
            {
                type = elementType;
            }

            return
                    type != typeof(sbyte[]) &&
                    type != typeof(IntPtr) &&
                    type != typeof(UIntPtr) &&
                    type != typeof(char) &&
                    type != typeof(Uri) &&
                    type != typeof(char[]) &&
                    type != typeof(UInt16) &&
                    type != typeof(UInt32) &&
                    type != typeof(UInt64) &&
                    !type.IsEnum;
        }

        /// <summary>Sample values for this type.</summary>
        public object[] SampleValues
        {
            get
            {
                return this.sampleValues;
            }
        }
#if ClientFramework

        /// <summary>Checks whether two values are equal.</summary>
        /// <param name="type">Type of values to check.</param>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="responseFormat">Response format from which actual value was retrieved.</param>
        /// <returns>true if the values are equal; false otherwise.</returns>
        public static bool AreEqual(Type type, object expected, object actual, string responseFormat)
        {

            TestUtil.CheckArgumentNotNull(type, "type");


            if (expected == null && actual == null)
            {
                return true;
            }

            if (expected == null || actual == null)
            {
                return false;
            }

            if (type == typeof(char[]))
            {
                return AreArraysEqual<char>(expected, actual);
            }
            else if (type == typeof(byte[]))
            {
                return AreArraysEqual<byte>(expected, actual);
            }
            else if (type == typeof(XElement))
            {
                return expected.ToString() == actual.ToString();
            }
            else if (responseFormat.StartsWith(SerializationFormatKinds.JsonMimeType, StringComparison.OrdinalIgnoreCase) && expected.GetType() == typeof(DateTime))
            {
                long expectedTicks = System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.ToUniversal((DateTime)expected).Ticks / 10000;
                long actualTicks = ((DateTime)actual).Ticks / 10000;
                return expectedTicks == actualTicks;
            }
            else if (responseFormat.StartsWith(SerializationFormatKinds.JsonMimeType, StringComparison.OrdinalIgnoreCase) && expected.GetType() == typeof(DateTimeOffset))
            {
                long expectedTicks = System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.ToUniversal((DateTimeOffset)expected).Ticks / 10000;
                long actualTicks = ((DateTimeOffset)actual).Ticks / 10000;
                return expectedTicks == actualTicks;
            }
            else
            {
                return expected.Equals(actual);
            }
        }

        /// <summary>
        /// Converts the text value into an instance of the type being described.
        /// </summary>
        /// <param name="text">Text value to convert.</param>
        /// <returns>An instance of the type being described for the specified text value.</returns>
        public object ValueFromXmlText(string text, string responseFormat)
        {
            return ValueFromXmlText(text, this.clrType, false, responseFormat);
        }

        public static string StripQuotes(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (text.Length < 2)
            {
                throw new ArgumentException("Text is too short: [" + text + "]");
            }

            text = text.Substring(1, text.Length - 2);
            return text.Replace("''", "'");
        }

        /// <summary>
        /// Converts the text value into an instance of the type being described.
        /// </summary>
        /// <param name="text">Text value to convert.</param>
        /// <param name="keySyntax">Whether key syntax is used.</param>
        /// <returns>An instance of the type being described for the specified text value.</returns>
        public object ValueFromXmlText(string text, bool keySyntax, string responseFormat)
        {
            return TypeData.ValueFromXmlText(text, this.clrType, keySyntax, responseFormat);
        }

        /// <summary>
        /// Converts the text value into an instance of the type being described.
        /// </summary>
        /// <param name="text">Text value to convert.</param>
        /// <param name="keySyntax">Whether key syntax is used.</param>
        /// <returns>An instance of the type being described for the specified text value.</returns>
        public static object ValueFromXmlText(string text, Type clrType, bool keySyntax, string responseFormat)
        {
            if (responseFormat.StartsWith(SerializationFormatKinds.JsonMimeType, StringComparison.OrdinalIgnoreCase))
            {
                return System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.StringToPrimitive(text, clrType);
            }

            if (clrType.IsGenericType &&
                clrType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return ValueFromXmlText(text, clrType.GetGenericArguments()[0], keySyntax, responseFormat);
            }

            if (clrType == typeof(bool))
            {
                return System.Xml.XmlConvert.ToBoolean(text);
            }
            if (clrType == typeof(byte))
            {
                return System.Xml.XmlConvert.ToByte(text);
            }
            if (clrType == typeof(sbyte))
            {
                return System.Xml.XmlConvert.ToSByte(text);
            }
            if (clrType == typeof(char))
            {
                if (keySyntax)
                {
                    text = StripQuotes(text);
                }

                return System.Xml.XmlConvert.ToChar(text);
            }
            if (clrType == typeof(decimal))
            {
                return System.Xml.XmlConvert.ToDecimal(text);
            }
            if (clrType == typeof(double))
            {
                return System.Xml.XmlConvert.ToDouble(text);
            }
            if (clrType == typeof(float))
            {
                return (float)System.Xml.XmlConvert.ToDouble(text);
            }
            if (clrType == typeof(UriComponents))
            {
                if (keySyntax)
                {
                    text = StripQuotes(text);
                }

                return Enum.Parse(typeof(UriComponents), text, false);
            }
            if (clrType == typeof(Int16))
            {
                return System.Xml.XmlConvert.ToInt16(text);
            }
            if (clrType == typeof(Int32))
            {
                return System.Xml.XmlConvert.ToInt32(text);
            }
            if (clrType == typeof(Int64))
            {
                return System.Xml.XmlConvert.ToInt64(text);
            }
            if (clrType == typeof(UInt16))
            {
                return System.Xml.XmlConvert.ToUInt16(text);
            }
            if (clrType == typeof(UInt32))
            {
                return System.Xml.XmlConvert.ToUInt32(text);
            }
            if (clrType == typeof(UInt64))
            {
                return System.Xml.XmlConvert.ToUInt64(text);
            }
            if (clrType == typeof(string) || clrType == typeof(XElement))
            {
                if (keySyntax)
                {
                    text = StripQuotes(text);
                }

                return text;
            }
            if (clrType == typeof(char[]))
            {
                if (keySyntax)
                {
                    text = StripQuotes(text);
                }

                return text.ToCharArray();
            }
            if (clrType == typeof(byte[])
#if !ClientSKUFramework
 || clrType == typeof(Binary)
#endif
)
            {
                byte[] resultArray;
                if (keySyntax)
                {
                    if (text.StartsWith("X")) text = text.Remove(0, 1);
                    if (text.StartsWith("binary")) text = text.Remove(0, "binary".Length);
                    text = StripQuotes(text);
                    byte[] buffer = new byte[text.Length / 2];
                    XmlReader reader = new XmlTextReader(new StringReader("<foo>" + text + "</foo>"));
                    reader.Read();
                    reader.ReadElementContentAsBinHex(buffer, 0, buffer.Length);
                    reader.Close();
                    resultArray = buffer;
                }
                else
                {
                    resultArray = Convert.FromBase64String(text);
                }

#if !ClientSKUFramework

                if (clrType == typeof(Binary))
                {
                    return new Binary(resultArray);
                }
                else
#endif
                {
                    return resultArray;
                }
            }
            if (clrType == typeof(IntPtr))
            {
                return Convert.ChangeType(text, typeof(IntPtr), CultureInfo.InvariantCulture);
            }
            if (clrType == typeof(UIntPtr))
            {
                return Convert.ChangeType(text, typeof(UIntPtr), CultureInfo.InvariantCulture);
            }
            if (clrType == typeof(DateTime))
            {
                return System.Xml.XmlConvert.ToDateTime(text, XmlDateTimeSerializationMode.RoundtripKind);
            }
            if (clrType == typeof(DateTimeOffset))
            {
                return System.Xml.XmlConvert.ToDateTimeOffset(text);
            }
            if (clrType == typeof(TimeSpan))
            {
                if (keySyntax)
                {
                    text = text.Remove(0, "time".Length);
                    text = StripQuotes(text);
                }

                return System.Xml.XmlConvert.ToTimeSpan(text);
            }
            if (clrType == typeof(Uri))
            {
                if (keySyntax)
                {
                    text = StripQuotes(text);
                }

                return new Uri(text, UriKind.RelativeOrAbsolute);
            }
            if (clrType == typeof(Guid))
            {
                return System.Xml.XmlConvert.ToGuid(text);
            }

            throw new NotImplementedException("ValueFromXmlText not implemented for type " + clrType);
        }

        /// <summary>
        /// Converts the specified <paramref name="value" />into its XML representation.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>The text representation for <paramref name="value"/>.</returns>
        public static string XmlValueFromObject(object value)
        {
            TestUtil.CheckArgumentNotNull(value, "value");

            Type valueType = value.GetType();
            if (valueType == typeof(bool))
            {
                return System.Xml.XmlConvert.ToString((bool)value);
            }
            if (valueType == typeof(byte))
            {
                return System.Xml.XmlConvert.ToString((byte)value);
            }
            if (valueType == typeof(char))
            {
                return System.Xml.XmlConvert.ToString((char)value);
            }
            if (valueType == typeof(decimal))
            {
                return System.Xml.XmlConvert.ToString((decimal)value);
            }
            if (valueType == typeof(double))
            {
                return System.Xml.XmlConvert.ToString((double)value);
            }
            if (valueType == typeof(float))
            {
                return System.Xml.XmlConvert.ToString((float)value);
            }
            if (valueType == typeof(Int16))
            {
                return System.Xml.XmlConvert.ToString((short)value);
            }
            if (valueType == typeof(Int32))
            {
                return System.Xml.XmlConvert.ToString((int)value);
            }
            if (valueType == typeof(Int64))
            {
                return System.Xml.XmlConvert.ToString((long)value);
            }
            if (valueType == typeof(SByte))
            {
                return System.Xml.XmlConvert.ToString((SByte)value);
            }
            if (valueType == typeof(string))
            {
                return (string)value;
            }
            if (valueType == typeof(XElement))
            {
                return ((XElement)value).ToString(SaveOptions.None);
            }
            if (valueType == typeof(byte[]))
            {
                return Convert.ToBase64String((byte[])value);
            }
#if !ClientSKUFramework

            if (valueType == typeof(Binary))
            {
                return Convert.ToBase64String(((Binary)value).ToArray());
            }
#endif

            if (valueType == typeof(DateTime))
            {
                return System.Xml.XmlConvert.ToString(ConvertDateTimeToDateTimeOffset((DateTime)value));
            }
            if (valueType == typeof(DateTimeOffset))
            {
                return XmlConvert.ToString((DateTimeOffset)value);
            }
            if (valueType == typeof(Guid))
            {
                return System.Xml.XmlConvert.ToString((Guid)value);
            }
            if (valueType == typeof(TimeSpan))
            {
                return XmlConvert.ToString((TimeSpan)value);
            }
            if (valueType == typeof(DBNull))
            {
                return null;
            }

            throw new NotImplementedException("XmlValueFromObject not implemented for type " + valueType);
        }

        /// <summary>Checks whether the specified <paramref name="value"/> has an XML value representaiton.</summary>
        /// <param name="value">The value to check.</param>
        /// <returns>true/false.</returns>
        /// <remarks>This method will return false for cases were XmlValueFromObject would throw.</remarks>
        public static bool HasXmlValueRepresentation(object value)
        {
            TestUtil.CheckArgumentNotNull(value, "value");
            Type valueType = value.GetType();
            if (valueType == typeof(bool) ||
                valueType == typeof(byte) ||
                valueType == typeof(char) ||
                valueType == typeof(decimal) ||
                valueType == typeof(double) ||
                valueType == typeof(float) ||
                valueType == typeof(Int16) ||
                valueType == typeof(Int32) ||
                valueType == typeof(Int64) ||
                valueType == typeof(SByte) ||
                valueType == typeof(string) ||
                valueType == typeof(XElement) ||
                valueType == typeof(byte[]) ||
#if !ClientSKUFramework
 valueType == typeof(Binary) ||
#endif
 valueType == typeof(DateTime) ||
                valueType == typeof(Guid) ||
                valueType == typeof(DBNull))
            {
                return true;
            }

            return false;
        }

        /// <summary>Converts a string to a primitive value.</summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetType">Type to convert string to.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        public static object ObjectFromXmlValue(string text, Type targetType)
        {
            Debug.Assert(text != null, "text != null");
            Debug.Assert(targetType != null, "targetType != null");

            if (targetType.IsGenericType &&
                targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                targetType = targetType.GetGenericArguments()[0];
            }

            if (typeof(String) == targetType)
            {
                return text;
            }
            else if (typeof(Boolean) == targetType)
            {
                return XmlConvert.ToBoolean(text);
            }
#if !ClientSKUFramework

            else if (typeof(Binary) == targetType)
            {
                return new Binary(Convert.FromBase64String(text));
            }
#endif

            else if (typeof(Byte) == targetType)
            {
                return XmlConvert.ToByte(text);
            }
            else if (typeof(byte[]) == targetType)
            {
                return Convert.FromBase64String(text);
            }
            else if (typeof(SByte) == targetType)
            {
                return XmlConvert.ToSByte(text);
            }
            else if (typeof(DateTime) == targetType)
            {
                return XmlConvert.ToDateTime(text, XmlDateTimeSerializationMode.RoundtripKind);
            }
            else if (typeof(DateTimeOffset) == targetType)
            {
                return XmlConvert.ToDateTimeOffset(text);
            }
            else if (typeof(Decimal) == targetType)
            {
                return XmlConvert.ToDecimal(text);
            }
            else if (typeof(Double) == targetType)
            {
                return XmlConvert.ToDouble(text);
            }
            else if (typeof(Guid) == targetType)
            {
                return new Guid(text);
            }
            else if (typeof(Int16) == targetType)
            {
                return XmlConvert.ToInt16(text);
            }
            else if (typeof(Int32) == targetType)
            {
                return XmlConvert.ToInt32(text);
            }
            else if (typeof(Int64) == targetType)
            {
                return XmlConvert.ToInt64(text);
            }
            else if (typeof(Single) == targetType)
            {
                return XmlConvert.ToSingle(text);
            }
            else if (typeof(UInt16) == targetType)
            {
                return XmlConvert.ToUInt16(text);
            }
            else if (typeof(UInt32) == targetType)
            {
                return XmlConvert.ToUInt32(text);
            }
            else if (typeof(UInt64) == targetType)
            {
                return XmlConvert.ToUInt64(text);
            }
            else if (typeof(XElement) == targetType)
            {
                return XElement.Parse(text, LoadOptions.PreserveWhitespace);
            }
            else if (typeof(TimeSpan) == targetType)
            {
                return XmlConvert.ToTimeSpan(text);
            }

            throw new NotImplementedException("XmlValueFromObject not implemented for type " + targetType);
        }



        /// <summary>
        /// Determines whether the two objects of the described type are equal.
        /// </summary>
        /// <param name="expected">Expected value to compare with.</param>
        /// <param name="actual">Actual value to compare.</param>
        /// <returns>true if the values are equal; false otherwise.</returns>
        public bool AreEqual(object expected, object actual, string responseFormat)
        {
            return AreEqual(this.ClrType, expected, actual, responseFormat);
        }



        /// <summary>Formats the specified object into an (unescaped) value suitable for a uri.</summary>
        /// <param name="o">Object value.</param>
        /// <returns></returns>
        public static string FormatForKey(object o, bool? useSmallCasing, bool useDoublePostFix)
        {
            if (o == null)
            {
                return "";
            }
            else
            {
                string result;
                IFormattable formattable = o as IFormattable;
                if (o is DateTime)
                {
                    result = XmlConvert.ToString(ConvertDateTimeToDateTimeOffset((DateTime)o));
                }
                else if (o is DateTimeOffset)
                {
                    result = XmlConvert.ToString((DateTimeOffset)o);
                }
                else if (o is Guid)
                {
                    result = XmlConvert.ToString((Guid)o);
                }
                else if (o is Int64)
                {
                    result = System.Xml.XmlConvert.ToString((Int64)o);
                    if (useSmallCasing.HasValue)
                        result += (useSmallCasing.Value ? "l" : "L");
                    else
                        result += "L";
                }
                else if (o is decimal)
                {
                    result = System.Xml.XmlConvert.ToString((decimal)o);
                    if (useSmallCasing.HasValue)
                        result += (useSmallCasing.HasValue ? "m" : "M");
                    else
                        result += "M";

                }
                else if (o is float)
                {
                    float f = (float)o;
                    result = System.Xml.XmlConvert.ToString(f);
                    if (!float.IsInfinity(f) && !float.IsNaN(f) && !result.Contains(".") && !result.Contains("E"))
                        result += ".0";
                    if (useSmallCasing.HasValue)
                        result += (useSmallCasing.Value ? "f" : "F");
                    else
                        result += "f";
                }
                else if (o is Single)
                {
                    Single s = (Single)o;
                    result = System.Xml.XmlConvert.ToString(s);
                    if (!Single.IsInfinity(s) && !Single.IsNaN(s) && !result.Contains(".") && !result.Contains("E"))
                        result += ".0";
                    if (useSmallCasing.HasValue)
                        result += (useSmallCasing.Value ? "f" : "F");
                    else
                        result += "f";
                }
                else if (o is TimeSpan)
                {
                    if (useSmallCasing.HasValue)
                        result = (useSmallCasing.Value ? "duration'" : "Duration'");
                    else
                        result = "duration'";
                    result += XmlConvert.ToString((TimeSpan)o) + "'";
                }
                else if (o is Double)
                {
                    Double d = (Double)o;
                    result = System.Xml.XmlConvert.ToString(d);
                    if (!Double.IsInfinity(d) && !Double.IsNaN(d) && !result.Contains(".") && !result.Contains("E"))
                        result += ".0";
                    if (useDoublePostFix && !Double.IsInfinity(d) && !Double.IsNaN(d))
                    {
                        if (useSmallCasing.HasValue)
                            result += (useSmallCasing.Value ? "d" : "D");
                        else
                            result += "D";
                    }
                    else
                    {
                        result = UriQueryBuilder.UrlEncodeString(result);
                    }
                }
                else if (o is bool)
                {
                    result = ((bool)o) ? "true" : "false";
                }
#if !ClientSKUFramework

                else if (o is Binary)
                {
                    byte[] bytes = (o == null) ? null : ((Binary)o).ToArray();
                    result = FormatByteArrayForKey(bytes, useSmallCasing);
                }
#endif

                else if (o is byte[])
                {
                    byte[] bytes = (byte[])o;
                    result = FormatByteArrayForKey(bytes, useSmallCasing);
                }
                else
                {
                    result = o.ToString();
                }

                if (o is string || o is XElement)
                {
                    result = "'" + result.Replace("'", "''") + "'";
                }

                return result;
            }
        }

        public static string FormatByteArrayToString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return "";
            else
            {
                return Convert.ToBase64String(bytes, 0, bytes.Length);
            }
        }

        public static string FormatByteArrayForKey(byte[] bytes, bool? useSmallCasing)
        {
            if (bytes == null || bytes.Length == 0)
            {
                if (useSmallCasing.HasValue)
                    return (useSmallCasing.Value ? "binary''" : "BiNaRy''");
                else
                    return "binary''";
            }
            else
            {
                if (useSmallCasing.HasValue)
                    return (useSmallCasing.Value ? "binary'" : "BiNaRy'") + FormatByteArrayToString(bytes) + "'";
                else
                    return "binary'" + FormatByteArrayToString(bytes) + "'";
            }

        }

#if !ClientSKUFramework

        /// <summary>Checks whether the specified type is a known primitive type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if the specified type is known to be a primitive type; false otherwise.</returns>
        public static bool IsPrimitiveType(Type type)
        {
            // This is really a one-to-one mapping with the internal method.
            Assembly assembly = typeof(Microsoft.OData.Service.IDataServiceHost).Assembly;
            Type providerType = assembly.GetType("Microsoft.OData.Service.WebUtil", true);
            MethodInfo method = providerType.GetMethod("IsPrimitiveType", BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(method != null, "method != null");
            return (bool)method.Invoke(null, new object[] { type });
        }


        /// <summary>Checks whether the specified type is sortable.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if the specified type is sortable; false otherwise.</returns>
        public static bool IsSortable(Type type)
        {
            Debug.Assert(type != null, "type != null");

            return IsPrimitiveType(type) && type != typeof(byte[]) && type != typeof(Binary) && type != typeof(XElement);
        }

        /// <summary>Checks whether the specified type is sortable.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if the specified type is sortable; false otherwise.</returns>
        public static bool IsSortable(IEdmType type)
        {
            Debug.Assert(type != null, "type != null");

            if (type.TypeKind == EdmTypeKind.Primitive)
            {
                return ((IEdmPrimitiveType)type).PrimitiveKind != EdmPrimitiveTypeKind.Binary;
            }

            return false;
        }
#endif

#endif
        /// <summary>Provides a string representation of this object.</summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            return this.clrType.ToString();
        }

#if ClientFramework
        /// <summary>
        /// Determines whether the two objects of the described type are equal,
        /// throwing an exception otherwise.
        /// </summary>
        /// <param name="expected">Expected value to compare with.</param>
        /// <param name="actual">Actual value to compare.</param>
        public void VerifyAreEqual(object expected, object actual, string responseFormat)
        {
            if (!AreEqual(expected, actual, responseFormat))
            {
                throw new Exception("Actual value [" + actual + "] does not match [" + expected + "].");
            }
        }

        /// <summary>
        /// Returns the edm type name for the given clr type
        /// </summary>
        /// <returns></returns>
        public string GetEdmTypeName()
        {
            Type clrType = Nullable.GetUnderlyingType(this.ClrType) ?? this.clrType;

            if (!this.IsTypeSupported)
            {
                return null;
            }
            else if (clrType == typeof(string))
            {
                return "Edm.String";
            }
            else if (clrType == typeof(bool))
            {
                return "Edm.Boolean";
            }
#if !ClientSKUFramework

            else if (clrType == typeof(Binary))
            {
                return "Edm.Binary";
            }
#endif

            else if (clrType == typeof(Byte))
            {
                return "Edm.Byte";
            }
            else if (clrType == typeof(DateTime))
            {
                return "Edm.DateTimeOffset";
            }
            else if (clrType == typeof(DateTimeOffset))
            {
                return "Edm.DateTimeOffset";
            }
            else if (clrType == typeof(Decimal))
            {
                return "Edm.Decimal";
            }
            else if (clrType == typeof(Double))
            {
                return "Edm.Double";
            }
            else if (clrType == typeof(Guid))
            {
                return "Edm.Guid";
            }
            else if (clrType == typeof(Single))
            {
                return "Edm.Single";
            }
            else if (clrType == typeof(Int16))
            {
                return "Edm.Int16";
            }
            else if (clrType == typeof(Int32))
            {
                return "Edm.Int32";
            }
            else if (clrType == typeof(Int64))
            {
                return "Edm.Int64";
            }
            else if (clrType == typeof(SByte))
            {
                return "Edm.SByte";
            }
            else if (clrType == typeof(byte[]))
            {
                return "Edm.Binary";
            }
            else if (clrType == typeof(XElement))
            {
                return "Edm.String";
            }
            else if (clrType == typeof(TimeSpan))
            {
                return "Edm.Duration";
            }

            throw new Exception(String.Format("Unexpected clr type encountered: '{0}'", this.ClrType.FullName));
        }
#endif

        public static DateTimeOffset ConvertDateTimeToDateTimeOffset(DateTime dt)
        {
            DateTimeOffset dto;
            if (dt.Kind == DateTimeKind.Unspecified)
            {
                dto = new DateTimeOffset(new DateTime(dt.Ticks, DateTimeKind.Utc));
            }
            else
            {
                dto = new DateTimeOffset(dt);
            }

            return dto;
        }

        /// <summary>
        /// Creates a new TypeData[] with the specified type data and
        /// the Nullable`1 version of the value types.
        /// </summary>
        /// <param name="typeData">Type data to start off from.</param>
        /// <returns>
        /// A new TypeData[] with the specified type data and
        /// the Nullable`1 version of the value types.
        /// </returns>
        private static TypeData[] AddNullableVersions(TypeData[] typeData)
        {
            List<TypeData> result = new List<TypeData>(typeData);
            foreach (TypeData data in typeData)
            {
                if (data.ClrType != null && data.ClrType.IsValueType)
                {
                    TypeData nullableTypeData = new TypeData();

                    nullableTypeData.clrType = typeof(Nullable<>).MakeGenericType(data.ClrType);

                    nullableTypeData.sampleValues = new object[data.sampleValues.Length + 1];
                    Array.Copy(data.sampleValues, nullableTypeData.sampleValues, data.SampleValues.Length);
                    nullableTypeData.sampleValues[nullableTypeData.sampleValues.Length - 1] = null;
                    result.Add(nullableTypeData);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Initializes a new TypeData instance to describe the specified type,
        /// including the sample data.
        /// </summary>
        /// <param name="clrType">Type to describe.</param>
        /// <param name="sampleValues">Sample values of type clrType (or convertible to those).</param>
        /// <returns>An initialized TypeData instance.</returns>
        private static TypeData ForSamples(Type clrType, params object[] sampleValues)
        {
            Debug.Assert(clrType != null);
            Debug.Assert(sampleValues != null, "sampleValues != null - for type " + clrType);

            TypeData result = new TypeData();
            result.clrType = clrType;
            result.sampleValues = sampleValues;

            // Ensure that all values are of the correct type; this makes
            // it easier to build the sample lists without coercing all
            // integer values to the right 'width', for example.
            for (int i = 0; i < result.sampleValues.Length; i++)
            {
                if (result.sampleValues[i] != null && result.sampleValues[i].GetType() != clrType)
                {
                    result.sampleValues[i] = Convert.ChangeType(result.sampleValues[i], clrType);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets an object array with all the values of the UriComponents enumeration.
        /// </summary>
        /// <returns>An object array with all the values of the UriComponents enumeration.</returns>
        private static object[] ValuesForUriComponents()
        {
            UriComponents[] components = (UriComponents[])Enum.GetValues(typeof(UriComponents));
            object[] result = new object[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                result[i] = components[i];
            }
            return result;
        }


        /// <summary>
        /// Converts the specified StringData[] into an array of char[] instances.
        /// </summary>
        /// <param name="stringData">StringData elements to convert.</param>
        /// <returns>A new array of char[] instances.</returns>
        private static char[][] ConvertToCharArrays(StringData[] stringData)
        {
            char[][] result = new char[stringData.Length][];
            for (int i = 0; i < stringData.Length; i++)
            {
                if (stringData[i].Value != null)
                {
                    result[i] = stringData[i].Value.ToCharArray();
                }
            }
            return result;
        }
#if ClientFramework
        /// <summary>
        /// Compares two arrays to determine whether they are equal.
        /// </summary>
        /// <typeparam name="T">Type of elements in the arrays.</typeparam>
        /// <param name="expected">Array with expected values.</param>
        /// <param name="actual">Array with actual values.</param>
        /// <returns>true if the arrays are equal; false otherwise.</returns>
        private static bool AreArraysEqual<T>(object expected, object actual)
        {
            T[] expectedArray = (T[])expected;
            T[] actualArray = (T[])actual;

            if (expectedArray.Length != actualArray.Length)
            {
                return false;
            }

            for (int i = 0; i < expectedArray.Length; i++)
            {
                if (!expectedArray[i].Equals(actualArray[i]))
                {
                    return false;
                }
            }

            return true;
        }
#endif
    }
}
