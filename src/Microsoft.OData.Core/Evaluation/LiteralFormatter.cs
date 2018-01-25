//---------------------------------------------------------------------
// <copyright file="LiteralFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client
#else
#if ODATA_SERVICE
namespace Microsoft.OData.Service
#else
namespace Microsoft.OData.Evaluation
#endif
#endif
{
#if ODATA_SERVICE
    using System.Data.Linq;
#endif
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Linq;
    using System.Xml;
#if ODATA_CORE
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
#else
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using ExpressionConstants = XmlConstants;
#endif


    /// <summary>
    /// Component for formatting literals for use in URIs, ETags, and skip-tokens.
    /// </summary>
    internal abstract class LiteralFormatter
    {
        /// <summary>Default singleton instance for parenthetical keys, etags, or skiptokens.</summary>
        private static readonly LiteralFormatter DefaultInstance = new DefaultLiteralFormatter();

#if ODATA_CORE
        /// <summary>Default singleton instance which does not URL-encode the resulting string.</summary>
        private static readonly LiteralFormatter DefaultInstanceWithoutEncoding = new DefaultLiteralFormatter(/*disableUrlEncoding*/ true);
#endif

        /// <summary>Default singleton instance for keys formatted as segments.</summary>
        private static readonly LiteralFormatter KeyAsSegmentInstance = new KeysAsSegmentsLiteralFormatter();

#if ODATA_SERVICE
        /// <summary>
        /// Gets the literal formatter for ETags.
        /// </summary>
        internal static LiteralFormatter ForETag
        {
            get { return DefaultInstance; }
        }

        /// <summary>
        /// Gets the literal formatter for skip-tokens.
        /// </summary>
        internal static LiteralFormatter ForSkipToken
        {
            get { return DefaultInstance; }
        }
#else
        /// <summary>
        /// Gets the literal formatter for URL constants.
        /// </summary>
        internal static LiteralFormatter ForConstants
        {
            get
            {
                return DefaultInstance;
            }
        }
#endif

#if ODATA_CORE
        /// <summary>
        /// Gets the literal formatter for URL constants which does not URL-encode the string.
        /// </summary>
        internal static LiteralFormatter ForConstantsWithoutEncoding
        {
            get
            {
                return DefaultInstanceWithoutEncoding;
            }
        }
#endif

        /// <summary>
        /// Gets the literal formatter for keys.
        /// </summary>
        /// <param name="keysAsSegment">if set to <c>true</c> then the key is going to be written as a segment, rather than in parentheses.</param>
        /// <returns>The literal formatter for keys.</returns>
        internal static LiteralFormatter ForKeys(bool keysAsSegment)
        {
            return keysAsSegment ? KeyAsSegmentInstance : DefaultInstance;
        }

        /// <summary>Converts the specified value to an encoded, serializable string for URI key.</summary>
        /// <param name="value">Non-null value to convert.</param>
        /// <returns>value converted to a serializable string for URI key.</returns>
        internal abstract string Format(object value);

        /// <summary>
        /// Escapes the result accoridng to URI escaping rules.
        /// </summary>
        /// <param name="result">The result to escape.</param>
        /// <returns>The escaped string.</returns>
        protected virtual string EscapeResultForUri(string result)
        {
            // required for strings as data, DateTime for ':', numbers for '+'
            // we specifically do not want to encode leading and trailing "'" wrapping strings/datetime/guid
            return Uri.EscapeDataString(result);
        }

        /// <summary>Converts the given byte[] into string.</summary>
        /// <param name="byteArray">byte[] that needs to be converted.</param>
        /// <returns>String containing hex values representing the byte[].</returns>
        private static string ConvertByteArrayToKeyString(byte[] byteArray)
        {
            Debug.Assert(byteArray != null, "byteArray != null");
            return Convert.ToBase64String(byteArray, 0, byteArray.Length);
        }

        /// <summary>
        /// Formats the literal without a type prefix, quotes, or escaping.
        /// </summary>
        /// <param name="value">The non-null value to format.</param>
        /// <returns>The formatted literal, without type marker or quotes.</returns>
        private static string FormatRawLiteral(object value)
        {
            Debug.Assert(value != null, "value != null");

            string stringValue = value as string;
            if (stringValue != null)
            {
                return stringValue;
            }

            if (value is bool)
            {
                return XmlConvert.ToString((bool)value);
            }

            if (value is byte)
            {
                return XmlConvert.ToString((byte)value);
            }

#if ODATA_SERVICE
            if (value is DateTime)
            {
                // Since the server/client supports DateTime values, convert the DateTime value
                // to DateTimeOffset and use XmlCOnvert to convert to String.
                DateTimeOffset dto = WebUtil.ConvertDateTimeToDateTimeOffset((DateTime)value);
                return XmlConvert.ToString(dto);
            }
#endif

            if (value is decimal)
            {
                return XmlConvert.ToString((decimal)value);
            }

            if (value is double)
            {
                string formattedDouble = XmlConvert.ToString((double)value);
                formattedDouble = SharedUtils.AppendDecimalMarkerToDouble(formattedDouble);
                return formattedDouble;
            }

            if (value is Guid)
            {
                return value.ToString();
            }

            if (value is short)
            {
                return XmlConvert.ToString((Int16)value);
            }

            if (value is int)
            {
                return XmlConvert.ToString((Int32)value);
            }

            if (value is long)
            {
                return XmlConvert.ToString((Int64)value);
            }

            if (value is sbyte)
            {
                return XmlConvert.ToString((SByte)value);
            }

            if (value is float)
            {
                return XmlConvert.ToString((Single)value);
            }

            byte[] array = value as byte[];
            if (array != null)
            {
                return ConvertByteArrayToKeyString(array);
            }

            if (value is Date)
            {
                return value.ToString();
            }

            if (value is DateTimeOffset)
            {
                return XmlConvert.ToString((DateTimeOffset)value);
            }

            if (value is TimeOfDay)
            {
                return value.ToString();
            }

            if (value is TimeSpan)
            {
                return EdmValueWriter.DurationAsXml((TimeSpan)value);
            }

            Geography geography = value as Geography;
            if (geography != null)
            {
                return WellKnownTextSqlFormatter.Create(true).Write(geography);
            }

            Geometry geometry = value as Geometry;
            if (geometry != null)
            {
                return WellKnownTextSqlFormatter.Create(true).Write(geometry);
            }

            ODataEnumValue enumValue = value as ODataEnumValue;
            if (enumValue != null)
            {
                return enumValue.Value;
            }

            throw SharedUtils.CreateExceptionForUnconvertableType(value);
        }

        /// <summary>
        /// Formats the literal without a type prefix or quotes, but does escape it.
        /// </summary>
        /// <param name="value">The non-null value to format.</param>
        /// <returns>The formatted literal, without type marker or quotes.</returns>
        private string FormatAndEscapeLiteral(object value)
        {
            Debug.Assert(value != null, "value != null");

            string result = FormatRawLiteral(value);
            Debug.Assert(result != null, "result != null");

            if (value is string)
            {
                result = result.Replace("'", "''");
            }

            return this.EscapeResultForUri(result);
        }

        /// <summary>
        /// Helper utilities that capture any deltas between ODL, the WCF DS Client, and the WCF DS Server.
        /// </summary>
        private static class SharedUtils
        {
            /// <summary>
            /// Creates a new exception instance to be thrown if the value is not a type that can be formatted as a literal.
            /// DEVNOTE: Will return a different exception depending on whether this is ODataLib, the WCF DS Server, or the WCF DS client.
            /// </summary>
            /// <param name="value">The literal value that could not be converted.</param>
            /// <returns>The exception that should be thrown.</returns>
            internal static InvalidOperationException CreateExceptionForUnconvertableType(object value)
            {
#if ODATA_SERVICE
                return new InvalidOperationException(Microsoft.OData.Service.Strings.Serializer_CannotConvertValue(value));
#endif
#if ODATA_CLIENT
                return Error.InvalidOperation(Client.Strings.Context_CannotConvertKey(value));
#endif
#if ODATA_CORE
                return new ODataException(Strings.ODataUriUtils_ConvertToUriLiteralUnsupportedType(value.GetType().ToString()));
#endif
            }

            /// <summary>
            /// Tries to convert the given value to one of the standard recognized types. Used specifically for handling XML and binary types.
            /// </summary>
            /// <param name="value">The original value.</param>
            /// <param name="converted">The value converted to one of the standard types.</param>
            /// <returns>Whether or not the value was converted.</returns>
            internal static bool TryConvertToStandardType(object value, out object converted)
            {
                byte[] array;
                if (TryGetByteArrayFromBinary(value, out array))
                {
                    converted = array;
                    return true;
                }

#if !ODATA_CORE
                XElement xml = value as XElement;
                if (xml != null)
                {
                    converted = xml.ToString();
                    return true;
                }
#endif
                converted = null;
                return false;
            }

            /// <summary>
            /// Appends the decimal marker to string form of double value if necessary.
            /// DEVNOTE: Only used by the client and ODL, for legacy/back-compat reasons.
            /// </summary>
            /// <param name="input">Input string.</param>
            /// <returns>String with decimal marker optionally added.</returns>
            internal static string AppendDecimalMarkerToDouble(string input)
            {
                // DEVNOTE: for some reason, the client adds .0 to doubles where the server does not.
                // Unfortunately, it would be a breaking change to alter this behavior now.
#if ODATA_CLIENT || ODATA_CORE
                IEnumerable<char> characters = input.ToCharArray();

#if ODATA_CORE
                // negative numbers can also be 'whole', but the client did not take that into account.
                if (input[0] == '-')
                {
                    characters = characters.Skip(1);
                }
#endif
                // a whole number should be all digits.
                if (characters.All(char.IsDigit))
                {
                    return input + ".0";
                }

#endif
                // the server never appended anything, so it will fall through to here.
                return input;
            }

            /// <summary>
            /// Tries to convert an instance of System.Data.Linq.Binary to a byte array.
            /// </summary>
            /// <param name="value">The original value which might be an instance of System.Data.Linq.Binary.</param>
            /// <param name="array">The converted byte array, if it was converted.</param>
            /// <returns>Whether or not the value was converted.</returns>
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value", Justification = "Method is compiled into 3 assemblies, and the parameter is used in 2 of them.")]
            private static bool TryGetByteArrayFromBinary(object value, out byte[] array)
            {
                // DEVNOTE: the client does not have a reference to System.Data.Linq, but the server does.
                // So we need to interact with Binary differently.
#if ODATA_SERVICE
                Binary binary = value as Binary;
                if (binary != null)
                {
                    array = binary.ToArray();
                    return true;
                }
#endif
#if ODATA_CLIENT
                return ClientConvert.TryConvertBinaryToByteArray(value, out array);
#else
                array = null;
                return false;
#endif
            }
        }

        /// <summary>
        /// Default literal formatter implementation.
        /// </summary>
        private sealed class DefaultLiteralFormatter : LiteralFormatter
        {
            /// <summary>If true, literals will not be URL encoded.</summary>
            private readonly bool disableUrlEncoding;

            /// <summary>
            /// Creates a new instance of <see cref="DefaultLiteralFormatter"/>.
            /// </summary>
            internal DefaultLiteralFormatter()
                : this(false /*disableUrlEncoding*/)
            {
            }

#if ODATA_CORE
            /// <summary>
            /// Creates a new instance of <see cref="DefaultLiteralFormatter"/>.
            /// </summary>
            /// <param name="disableUrlEncoding">If true, literals will not be URL encoded.</param>
            internal DefaultLiteralFormatter(bool disableUrlEncoding)
#else
            /// <summary>
            /// Creates a new instance of <see cref="DefaultLiteralFormatter"/>.
            /// </summary>
            /// <param name="disableUrlEncoding">If true, literals will not be URL encoded.</param>
            private DefaultLiteralFormatter(bool disableUrlEncoding)
#endif
            {
                this.disableUrlEncoding = disableUrlEncoding;
            }

            /// <summary>Converts the specified value to an encoded, serializable string for URI key.</summary>
            /// <param name="value">Non-null value to convert.</param>
            /// <returns>value converted to a serializable string for URI key.</returns>
            internal override string Format(object value)
            {
                object converted;
                if (SharedUtils.TryConvertToStandardType(value, out converted))
                {
                    value = converted;
                }

                return this.FormatLiteralWithTypePrefix(value);
            }

            /// <summary>
            /// Escapes the result accoridng to URI escaping rules.
            /// </summary>
            /// <param name="result">The result to escape.</param>
            /// <returns>The escaped string.</returns>
            protected override string EscapeResultForUri(string result)
            {
#if !ODATA_CORE
                Debug.Assert(!this.disableUrlEncoding, "Only supported for ODataLib for backwards compatibility reasons.");
#endif
                if (!this.disableUrlEncoding)
                {
                    result = base.EscapeResultForUri(result);
                }

                return result;
            }

            /// <summary>
            /// Formats the literal with a type prefix and quotes (if the type requires it).
            /// </summary>
            /// <param name="value">The value to format.</param>
            /// <returns>The formatted literal, with type marker if needed.</returns>
            private string FormatLiteralWithTypePrefix(object value)
            {
                Debug.Assert(value != null, "value != null. Null values need to be handled differently in some cases.");

                var enumValue = value as ODataEnumValue;
                if (enumValue != null)
                {
                    if (string.IsNullOrEmpty(enumValue.TypeName))
                    {
                        // TODO: [Sizhong Du] Replace with error string #647.
                        throw new ODataException("Type name should not be null or empty when serializing an Enum value for URI key.");
                    }

                    return enumValue.TypeName + "'" + this.FormatAndEscapeLiteral(enumValue.Value) + "'";
                }

                string result = this.FormatAndEscapeLiteral(value);

                if (value is byte[])
                {
                    return ExpressionConstants.LiteralPrefixBinary + "'" + result + "'";
                }

                if (value is Geography)
                {
                    return ExpressionConstants.LiteralPrefixGeography + "'" + result + "'";
                }

                if (value is Geometry)
                {
                    return ExpressionConstants.LiteralPrefixGeometry + "'" + result + "'";
                }

                if (value is TimeSpan)
                {
                    return ExpressionConstants.LiteralPrefixDuration + "'" + result + "'";
                }

                if (value is string)
                {
                    return "'" + result + "'";
                }

                // for int32,int64,float,double, decimal, Infinity/NaN, just output them without prefix or suffix such as L/M/D/F.
                return result;
            }
        }

        /// <summary>
        /// Literal formatter for keys which are written as URI segments.
        /// Very similar to the default, but it never puts the type markers or single quotes around the value.
        /// </summary>
        private sealed class KeysAsSegmentsLiteralFormatter : LiteralFormatter
        {
            /// <summary>
            /// Creates a new instance of <see cref="KeysAsSegmentsLiteralFormatter"/>.
            /// </summary>
            internal KeysAsSegmentsLiteralFormatter()
            {
            }

            /// <summary>Converts the specified value to an encoded, serializable string for URI key.</summary>
            /// <param name="value">Non-null value to convert.</param>
            /// <returns>value converted to a serializable string for URI key.</returns>
            internal override string Format(object value)
            {
                Debug.Assert(value != null, "value != null");

                ODataEnumValue enumValue = value as ODataEnumValue;
                if (enumValue != null)
                {
                    value = enumValue.Value;
                }

                object converted;
                if (SharedUtils.TryConvertToStandardType(value, out converted))
                {
                    value = converted;
                }

                string stringValue = value as string;
                if (stringValue != null)
                {
                    value = EscapeLeadingDollarSign(stringValue);
                }

                return FormatAndEscapeLiteral(value);
            }

            /// <summary>
            /// If the string starts with a '$', prepends another '$' to escape it.
            /// </summary>
            /// <param name="stringValue">The string value.</param>
            /// <returns>The string value with a leading '$' escaped, if one was present.</returns>
            private static string EscapeLeadingDollarSign(string stringValue)
            {
                if (stringValue.Length > 0 && stringValue[0] == '$')
                {
                    stringValue = '$' + stringValue;
                }

                return stringValue;
            }
        }
    }
}
