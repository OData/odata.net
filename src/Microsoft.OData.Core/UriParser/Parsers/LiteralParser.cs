//---------------------------------------------------------------------
// <copyright file="LiteralParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Spatial;

namespace Microsoft.OData.UriParser
{
    /// <summary>Use this class to parse literals from keys, etags, skiptokens, and filter/orderby expression constants.</summary>
    internal abstract class LiteralParser
    {
        /// <summary>
        /// Default singleton instance of the literal parser.
        /// </summary>
        private static readonly LiteralParser DefaultInstance = new DefaultLiteralParser();

        /// <summary>
        /// Singleton instance of the literal parser for when keys-as-segments is turned on, which does not wrap the formatted strings in any quotes or type-markers.
        /// </summary>
        private static readonly LiteralParser KeysAsSegmentsInstance = new KeysAsSegmentsLiteralParser();

        /// <summary>
        /// Mapping between primitive CLR types and lightweight parser classes for that type.
        /// </summary>
        private static readonly IDictionary<Type, PrimitiveParser> Parsers = new Dictionary<Type, PrimitiveParser>(ReferenceEqualityComparer<Type>.Instance)
        {
            // Type-specific parsers.
            { typeof(byte[]), new BinaryPrimitiveParser() },
            { typeof(String), new StringPrimitiveParser() },
            { typeof(Decimal), new DecimalPrimitiveParser() },
            { typeof(DateOnly), new DatePrimitiveParser() },

            // Types without single-quotes or type markers
            { typeof(Boolean), DelegatingPrimitiveParser<bool>.WithoutMarkup(ConverterUtils.ToBoolean) },
            { typeof(Byte), DelegatingPrimitiveParser<byte>.WithoutMarkup(ConverterUtils.ToByte) },
            { typeof(SByte), DelegatingPrimitiveParser<sbyte>.WithoutMarkup(ConverterUtils.ToSByte) },
            { typeof(Int16), DelegatingPrimitiveParser<short>.WithoutMarkup(ConverterUtils.ToInt16) },
            { typeof(Int32), DelegatingPrimitiveParser<int>.WithoutMarkup(ConverterUtils.ToInt32) },
            { typeof(DateTimeOffset), DelegatingPrimitiveParser<DateTimeOffset>.WithoutMarkup(ConverterUtils.ToDateTimeOffset) },
            { typeof(Guid), DelegatingPrimitiveParser<Guid>.WithoutMarkup(ConverterUtils.ToGuid) },

            // Types with prefixes and single-quotes.
            { typeof(TimeSpan), DelegatingPrimitiveParser<TimeSpan>.WithPrefix(EdmValueParser.ParseDuration, ExpressionConstants.LiteralPrefixDuration) },

            // Types with suffixes.
            { typeof(Int64), DelegatingPrimitiveParser<long>.WithSuffix(ConverterUtils.ToInt64, ExpressionConstants.LiteralSuffixInt64, /*required*/ false) },
            { typeof(Single), DelegatingPrimitiveParser<float>.WithSuffix(ConverterUtils.ToSingle, ExpressionConstants.LiteralSuffixSingle, /*required*/ false) },
            { typeof(Double), DelegatingPrimitiveParser<double>.WithSuffix(ConverterUtils.ToDouble, ExpressionConstants.LiteralSuffixDouble, /*required*/ false) }
        };

        /// <summary>
        /// Gets the literal parser to use for ETags.
        /// </summary>
        internal static LiteralParser ForETags
        {
            get
            {
                return DefaultInstance;
            }
        }

        /// <summary>
        /// Gets the literal parser for keys, based on whether the keys are formatted as segments.
        /// </summary>
        /// <param name="keyAsSegment">Whether or not the keys is formatted as a segment.</param>
        /// <returns>The literal parser to use.</returns>
        internal static LiteralParser ForKeys(bool keyAsSegment)
        {
            return keyAsSegment ? KeysAsSegmentsInstance : DefaultInstance;
        }

        /// <summary>Converts a string to a primitive value.</summary>
        /// <param name="targetType">Type to convert string to.</param>
        /// <param name="text">String text to convert.</param>
        /// <param name="result">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        internal abstract bool TryParseLiteral(Type targetType, string text, out object result);

#if DEBUG
        /// <summary>
        /// Test if the type is an ISpatial derived type
        /// </summary>
        /// <param name="type">the type to be tested</param>
        /// <returns>true if the type implements the ISpatial interface, false otherwise.</returns>
        private static bool IsSpatial(Type type)
        {
            return typeof(ISpatial).IsAssignableFrom(type);
        }
#endif

        /// <summary>
        /// Default literal parser which has type-markers and single-quotes. Also supports arbitrary literals being re-encoded in binary form.
        /// </summary>
        private sealed class DefaultLiteralParser : LiteralParser
        {
            /// <summary>Converts a string to a primitive value.</summary>
            /// <param name="targetType">Type to convert string to.</param>
            /// <param name="text">String text to convert.</param>
            /// <param name="result">After invocation, converted value.</param>
            /// <returns>true if the value was converted; false otherwise.</returns>
            internal override bool TryParseLiteral(Type targetType, string text, out object result)
            {
                Debug.Assert(text != null, "text != null");
                Debug.Assert(targetType != null, "expectedType != null");
#if DEBUG
                Debug.Assert(!IsSpatial(targetType), "Not supported for spatial types, as they cannot be part of a key, etag, or skiptoken");
#endif

                targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                bool binaryResult = TryRemoveFormattingAndConvert(text, typeof(byte[]), out result);
                if (binaryResult)
                {
                    byte[] byteArrayValue = (byte[])result;
                    if (targetType == typeof(byte[]))
                    {
                        result = byteArrayValue;
                        return true;
                    }

                    // we allow arbitary values to be encoded as a base 64 array, so we may have
                    // found a binary value in place of another type. If so, convert it to a UTF-8
                    // string and interpret it normally.
                    string keyValue = Encoding.UTF8.GetString(byteArrayValue, 0, byteArrayValue.Length);
                    return TryRemoveFormattingAndConvert(keyValue, targetType, out result);
                }

                if (targetType == typeof(byte[]))
                {
                    // if we got here, then the value was not binary.
                    result = null;
                    return false;
                }

                return TryRemoveFormattingAndConvert(text, targetType, out result);
            }

            /// <summary>
            /// Tries to parse the literal by first removing required formatting for the expected type, then converting the resulting string.
            /// </summary>
            /// <param name="text">String text to convert.</param>
            /// <param name="targetType">Type to convert string to.</param>
            /// <param name="targetValue">After invocation, converted value.</param>
            /// <returns>true if the value was converted; false otherwise.</returns>
            private static bool TryRemoveFormattingAndConvert(ReadOnlySpan<char> text, Type targetType, out object targetValue)
            {
                Debug.Assert(!text.IsEmpty, "!text.IsEmpty");
                Debug.Assert(targetType != null, "expectedType != null");
#if DEBUG
                Debug.Assert(!IsSpatial(targetType), "Not supported for spatial types, as they cannot be part of a key, etag, or skiptoken");
#endif

                Debug.Assert(Parsers.ContainsKey(targetType), "Unexpected type: " + targetType);
                PrimitiveParser parser = Parsers[targetType];
                if (!parser.TryRemoveFormatting(ref text))
                {
                    targetValue = null;
                    return false;
                }

                return parser.TryConvert(text, out targetValue);
            }
        }

        /// <summary>
        /// Simplified literal parser for keys-as-segments which does not expect type-markers, single-quotes, etc. Does not support re-encoding literals as binary.
        /// </summary>
        private sealed class KeysAsSegmentsLiteralParser : LiteralParser
        {
            /// <summary>Converts a string to a primitive value.</summary>
            /// <param name="targetType">Type to convert string to.</param>
            /// <param name="text">String text to convert.</param>
            /// <param name="result">After invocation, converted value.</param>
            /// <returns>true if the value was converted; false otherwise.</returns>
            internal override bool TryParseLiteral(Type targetType, string text, out object result)
            {
                Debug.Assert(text != null, "text != null");
                Debug.Assert(targetType != null, "expectedType != null");

                ReadOnlySpan<char> textSpan = UnescapeLeadingDollarSign(text.AsSpan());

                targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

#if DEBUG
                Debug.Assert(!IsSpatial(targetType), "Not supported for spatial types, as they cannot be part of a key, etag, or skiptoken");
#endif
                Debug.Assert(Parsers.ContainsKey(targetType), "Unexpected type: " + targetType);
                return Parsers[targetType].TryConvert(textSpan, out result);
            }

            /// <summary>
            /// If the string starts with '$', removes it.
            /// Also asserts that the 2nd character is also '$', as otherwise the string would be treated as a system segment.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <returns>The string value with a leading '$' removed, if the string started with one.</returns>
            private static ReadOnlySpan<char> UnescapeLeadingDollarSign(ReadOnlySpan<char> text)
            {
                Debug.Assert(!text.IsEmpty, "text is empty");
                if (text.Length > 1 && text[0] == '$' && text[1] == '$')
                {
                    text = text.Slice(1);
                }

                return text;
            }
        }

        /// <summary>
        /// Helper class for parsing a specific type of primitive literal.
        /// </summary>
        private abstract class PrimitiveParser
        {
            /// <summary>
            /// The expected prefix for the literal. Null indicates no prefix is expected.
            /// </summary>
            private readonly string prefix;

            /// <summary>
            /// The expected suffix for the literal. Null indicates that no suffix is expected.
            /// </summary>
            private readonly string suffix;

            /// <summary>
            /// Whether or not the suffix is required.
            /// </summary>
            private readonly bool suffixRequired;

            /// <summary>
            /// The expected type for this parser.
            /// </summary>
            private readonly Type expectedType;

            /// <summary>
            /// Initializes a new instance of the <see cref="PrimitiveParser"/> class.
            /// </summary>
            /// <param name="expectedType">The expected type for this parser.</param>
            /// <param name="suffix">The expected suffix for the literal. Null indicates that no suffix is expected.</param>
            /// <param name="suffixRequired">Whether or not the suffix is required.</param>
            protected PrimitiveParser(Type expectedType, string suffix, bool suffixRequired)
                : this(expectedType)
            {
                Debug.Assert(suffix != null, "suffix != null");
                this.prefix = null;
                this.suffix = suffix;
                this.suffixRequired = suffixRequired;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PrimitiveParser"/> class.
            /// </summary>
            /// <param name="expectedType">The expected type for this parser.</param>
            /// <param name="prefix">The expected prefix for the literal.</param>
            protected PrimitiveParser(Type expectedType, string prefix)
                : this(expectedType)
            {
                Debug.Assert(prefix != null, "prefix != null");
                this.prefix = prefix;
                this.suffix = null;
                this.suffixRequired = false;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PrimitiveParser"/> class.
            /// </summary>
            /// <param name="expectedType">The expected type for this parser.</param>
            protected PrimitiveParser(Type expectedType)
            {
                Debug.Assert(expectedType != null, "expectedType != null");
                this.expectedType = expectedType;
            }

            /// <summary>
            /// Tries to convert the given text into this parser's expected type. Conversion only, formatting should already have been removed.
            /// </summary>
            /// <param name="text">The text to convert.</param>
            /// <param name="targetValue">The target value.</param>
            /// <returns>Whether or not conversion was successful.</returns>
            internal abstract bool TryConvert(ReadOnlySpan<char> text, out object targetValue);

            /// <summary>
            /// Tries to remove formatting specific to this parser's expected type.
            /// </summary>
            /// <param name="text">The text to remove formatting from.</param>
            /// <returns>Whether or not the expected formatting was found and successfully removed.</returns>
            internal virtual bool TryRemoveFormatting(ref ReadOnlySpan<char> text)
            {
                if (this.prefix != null)
                {
                    if (!UriParserHelper.TryRemovePrefix(this.prefix, ref text))
                    {
                        return false;
                    }
                }

                bool shouldBeQuoted = this.prefix != null || ValueOfTypeCanContainQuotes(this.expectedType);
                if (shouldBeQuoted && !UriParserHelper.TryRemoveSingleQuotes(ref text, out _))
                {
                    return false;
                }

                if (this.suffix != null)
                {
                    // we need to try to remove the literal even if it isn't required.
                    if (!UriParserHelper.TryRemoveLiteralSuffix(this.suffix, ref text) && this.suffixRequired)
                    {
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// Determines whether the values for the specified types should be
            /// quoted in URI keys.
            /// </summary>
            /// <param name='type'>Type to check.</param>
            /// <returns>
            /// true if values of <paramref name='type' /> require quotes; false otherwise.
            /// </returns>
            private static bool ValueOfTypeCanContainQuotes(Type type)
            {
                Debug.Assert(type != null, "type != null");
                return type == typeof(string);
            }
        }

        /// <summary>
        /// Primitive parser which uses a delegate for conversion.
        /// </summary>
        /// <typeparam name="T">The expected CLR type when parsing.</typeparam>
        private class DelegatingPrimitiveParser<T> : PrimitiveParser
        {
            /// <summary>
            /// The delegate to use for conversion.
            /// </summary>
            private readonly Func<ReadOnlySpan<char>, T> convertMethod;

            /// <summary>
            /// Initializes a new instance of the <see cref="DelegatingPrimitiveParser&lt;T&gt;"/> class.
            /// </summary>
            /// <param name="convertMethod">The delegate to use for conversion.</param>
            /// <param name="suffix">The expected suffix for the literal. Null indicates that no suffix is expected.</param>
            /// <param name="suffixRequired">Whether or not the suffix is required.</param>
            protected DelegatingPrimitiveParser(Func<ReadOnlySpan<char>, T> convertMethod, string suffix, bool suffixRequired)
                : base(typeof(T), suffix, suffixRequired)
            {
                Debug.Assert(convertMethod != null, "convertMethod != null");
                this.convertMethod = convertMethod;
            }

            /// <summary>
            /// Prevents a default instance of the <see cref="DelegatingPrimitiveParser&lt;T&gt;"/> class from being created.
            /// </summary>
            /// <param name="convertMethod">The delegate to use for conversion.</param>
            private DelegatingPrimitiveParser(Func<ReadOnlySpan<char>, T> convertMethod)
                : base(typeof(T))
            {
                Debug.Assert(convertMethod != null, "convertMethod != null");
                this.convertMethod = convertMethod;
            }

            /// <summary>
            /// Prevents a default instance of the <see cref="DelegatingPrimitiveParser&lt;T&gt;"/> class from being created.
            /// </summary>
            /// <param name="convertMethod">The delegate to use for conversion.</param>
            /// <param name="prefix">The expected prefix for the literal.</param>
            private DelegatingPrimitiveParser(Func<ReadOnlySpan<char>, T> convertMethod, string prefix)
                : base(typeof(T), prefix)
            {
                Debug.Assert(convertMethod != null, "convertMethod != null");
                this.convertMethod = convertMethod;
            }

            /// <summary>
            /// Creates a primitive parser which wraps the given delegate and does not expect any extra markup in serialized literal.
            /// </summary>
            /// <param name="convertMethod">The delegate to use for conversion.</param>
            /// <returns>A new primitive parser.</returns>
            internal static DelegatingPrimitiveParser<T> WithoutMarkup(Func<ReadOnlySpan<char>, T> convertMethod)
            {
                return new DelegatingPrimitiveParser<T>(convertMethod);
            }

            /// <summary>
            /// Creates a primitive parser which wraps the given delegate and expects serialized literals to start with one of the given prefixes.
            /// </summary>
            /// <param name="convertMethod">The delegate to use for conversion.</param>
            /// <param name="prefix">The expected prefix for the literal.</param>
            /// <returns>A new primitive parser.</returns>
            internal static DelegatingPrimitiveParser<T> WithPrefix(Func<ReadOnlySpan<char>, T> convertMethod, string prefix)
            {
                return new DelegatingPrimitiveParser<T>(convertMethod, prefix);
            }

            /// <summary>
            /// Creates a primitive parser which wraps the given delegate and expects serialized literals to end with the given suffix.
            /// </summary>
            /// <param name="convertMethod">The delegate to use for conversion.</param>
            /// <param name="suffix">The expected suffix for the literal. Null indicates that no suffix is expected.</param>
            /// <returns>A new primitive parser.</returns>
            internal static DelegatingPrimitiveParser<T> WithSuffix(Func<ReadOnlySpan<char>, T> convertMethod, string suffix)
            {
                return WithSuffix(convertMethod, suffix, /*required*/ true);
            }

            /// <summary>
            /// Creates a primitive parser which wraps the given delegate and expects serialized literals to end with the given suffix.
            /// </summary>
            /// <param name="convertMethod">The delegate to use for conversion.</param>
            /// <param name="suffix">The expected suffix for the literal. Null indicates that no suffix is expected.</param>
            /// <param name="required">Whether or not the suffix is required.</param>
            /// <returns>A new primitive parser.</returns>
            internal static DelegatingPrimitiveParser<T> WithSuffix(Func<ReadOnlySpan<char>, T> convertMethod, string suffix, bool required)
            {
                return new DelegatingPrimitiveParser<T>(convertMethod, suffix, required);
            }

            /// <summary>
            /// Tries to convert the given text into this parser's expected type. Conversion only, formatting should already have been removed.
            /// </summary>
            /// <param name="text">The text to convert.</param>
            /// <param name="targetValue">The target value.</param>
            /// <returns>
            /// Whether or not conversion was successful.
            /// </returns>
            internal override bool TryConvert(ReadOnlySpan<char> text, out object targetValue)
            {
                try
                {
                    targetValue = this.convertMethod(text);
                    return true;
                }
                catch (FormatException)
                {
                    targetValue = default(T);
                    return false;
                }
                catch (OverflowException)
                {
                    targetValue = default(T);
                    return false;
                }
            }
        }

        /// <summary>
        /// Parser specific to the Edm.Decimal type.
        /// </summary>
        private sealed class DecimalPrimitiveParser : DelegatingPrimitiveParser<decimal>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DecimalPrimitiveParser"/> class.
            /// </summary>
            internal DecimalPrimitiveParser()
                : base(ConvertDecimal, ExpressionConstants.LiteralSuffixDecimal, false)
            {
            }

            /// <summary>
            /// Special helper to convert a string to a decimal that will allow more than what XmlConvert.ToDecimal supports by default.
            /// </summary>
            /// <param name="text">The text to convert.</param>
            /// <returns>The converted decimal value.</returns>
            private static decimal ConvertDecimal(ReadOnlySpan<char> text)
            {
                try
                {
                    return text.ToDecimal();
                }
                catch (FormatException)
                {
                    // we need to support exponential format for decimals since we used to support them in V1
                    decimal result;
                    if (decimal.TryParse(text, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result))
                    {
                        return result;
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Parser specific to the Edm.Binary type.
        /// </summary>
        private sealed class BinaryPrimitiveParser : PrimitiveParser
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BinaryPrimitiveParser"/> class.
            /// </summary>
            internal BinaryPrimitiveParser()
                : base(typeof(byte[]))
            {
            }

            /// <summary>
            /// Tries to convert the given text into this parser's expected type. Conversion only, formatting should already have been removed.
            /// </summary>
            /// <param name="text">The text to convert.</param>
            /// <param name="targetValue">The target value.</param>
            /// <returns>
            /// Whether or not conversion was successful.
            /// </returns>
            internal override bool TryConvert(ReadOnlySpan<char> text, out object targetValue)
            {
                try
                {
                    targetValue = Base64Url.DecodeFromChars(text);
                    return true;
                }
                catch (FormatException)
                {
                    try
                    {
                        targetValue = Convert.FromBase64String(text.ToString());
                        return true;
                    }
                    catch(FormatException)
                    {
                    }
                    targetValue = null;
                    return false;
                }
            }

            /// <summary>
            /// Tries to remove formatting specific to this parser's expected type.
            /// </summary>
            /// <param name="text">The text to remove formatting from.</param>
            /// <returns>
            /// Whether or not the expected formatting was found and succesfully removed.
            /// </returns>
            internal override bool TryRemoveFormatting(ref ReadOnlySpan<char> text)
            {
                if (!UriParserHelper.TryRemovePrefix(ExpressionConstants.LiteralPrefixBinary, ref text))
                {
                    return false;
                }

                if (!UriParserHelper.TryRemoveSingleQuotes(ref text, out _))
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Parser specific to the Edm.String type.
        /// </summary>
        private sealed class StringPrimitiveParser : PrimitiveParser
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StringPrimitiveParser"/> class.
            /// </summary>
            public StringPrimitiveParser()
                : base(typeof(string))
            {
            }

            /// <summary>
            /// Tries to convert the given text into this parser's expected type. Conversion only, formatting should already have been removed.
            /// </summary>
            /// <param name="text">The text to convert.</param>
            /// <param name="targetValue">The target value.</param>
            /// <returns>
            /// Whether or not conversion was successful.
            /// </returns>
            internal override bool TryConvert(ReadOnlySpan<char> text, out object targetValue)
            {
                targetValue = text.ToString();
                return true;
            }

            /// <summary>
            /// Tries to remove formatting specific to this parser's expected type.
            /// </summary>
            /// <param name="text">The text to remove formatting from.</param>
            /// <returns>
            /// Whether or not the expected formatting was found and succesfully removed.
            /// </returns>
            internal override bool TryRemoveFormatting(ref ReadOnlySpan<char> text)
            {
                return UriParserHelper.TryRemoveSingleQuotes(ref text, out _);
            }
        }

        /// <summary>
        /// Parser specific to the Edm.Date type.
        /// </summary>
        private sealed class DatePrimitiveParser : PrimitiveParser
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DatePrimitiveParser"/> class.
            /// </summary>
            public DatePrimitiveParser()
                : base(typeof(DateOnly))
            {
            }

            /// <summary>
            /// Tries to convert the given text into this parser's expected type. Conversion only, formatting should already have been removed.
            /// </summary>
            /// <param name="text">The text to convert.</param>
            /// <param name="targetValue">The target value.</param>
            /// <returns>
            /// Whether or not conversion was successful.
            /// </returns>
            internal override bool TryConvert(ReadOnlySpan<char> text, out object targetValue)
            {
                bool isSucceed = EdmValueParser.TryParseDateOnly(text, out DateOnly? date);
                targetValue = date;
                return isSucceed;
            }
        }
    }
}
