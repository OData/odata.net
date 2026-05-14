//---------------------------------------------------------------------
// <copyright file="UriPrimitiveTypeParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Buffers.Text;
using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Microsoft.OData.Core;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Parser which consumes the URI format of primitive types and converts it to primitive types.
    /// </summary>
    [System.Runtime.InteropServices.Guid("A77303D9-3A04-4829-BDDE-3B4D43E21CFC")]
    internal sealed class UriPrimitiveTypeParser : IUriLiteralParser
    {
        #region Singletons

        private static UriPrimitiveTypeParser singleInstance = new UriPrimitiveTypeParser();

        private UriPrimitiveTypeParser()
        {
        }

        public static UriPrimitiveTypeParser Instance
        {
            get
            {
                return singleInstance;
            }
        }

        #endregion

        #region IUriLiteralParser Implementation

        public object ParseUriStringToType(ReadOnlySpan<char> text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException)
        {
            object targetValue;

            if (TryUriStringToPrimitive(text, targetType, out targetValue, out parsingException))
            {
                return targetValue;
            }

            return null;
        }

        #endregion

        #region Internal Methods

        internal static bool TryParseUriStringToType(ReadOnlySpan<char> text, IEdmTypeReference targetType, out object targetValue, out UriLiteralParsingException parsingException)
        {
            return TryUriStringToPrimitive(text, targetType, out targetValue, out parsingException);
        }

        #endregion

        #region Private Methods

        /// <summary>Converts a string to a primitive value.</summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetType">Type to convert string to.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <param name="exception">The detailed parsing exception.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of the WebConvert.TryKeyStringToPrimitive</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity is not too high; handling all the cases in one method is preferable to refactoring.")]
        private static bool TryUriStringToPrimitive(ReadOnlySpan<char> text, IEdmTypeReference targetType, out object targetValue, out UriLiteralParsingException exception)
        {
            Debug.Assert(!text.IsEmpty, "!text.IsEmpty");
            Debug.Assert(targetType != null, "targetType != null");
            exception = null;

            if (text.IsEmpty)
            {
                targetValue = null;
                return false;
            }

            try
            {
                if (targetType.IsNullable)
                {
                    // COMPAT 38: Product does not support "null" literals in service operation arguments
                    // check for the 'null' constant for nullable types
                    if (text == ExpressionConstants.KeywordNull)
                    {
                        targetValue = null;
                        return true;
                    }
                }

                IEdmPrimitiveTypeReference primitiveTargetType = targetType.AsPrimitiveOrNull();
                if (primitiveTargetType == null)
                {
                    targetValue = null;
                    return false;
                }

                EdmPrimitiveTypeKind targetTypeKind = primitiveTargetType.PrimitiveKind();

                byte[] byteArrayValue;
                bool binaryResult = TryUriStringToByteArray(text, out byteArrayValue);// "binary'binaryValue'"
                if (targetTypeKind == EdmPrimitiveTypeKind.Binary)
                {
                    targetValue = (object)byteArrayValue;
                    return binaryResult;
                }
                else if (binaryResult)
                {
                    // this logic looks weird but let it be here for the sake of backward compatibility.
                    // We used to support parsing binary literals to string in V1, and we want to keep supporting that in later versions.
                    // So if the target type is not binary but the text can be parsed as binary, we will parse it as binary first and then convert the byte array to string and try parsing the string to the target primitive type.
                    string keyValue = Encoding.UTF8.GetString(byteArrayValue, 0, byteArrayValue.Length);
                    return TryUriStringToPrimitive(keyValue, targetType, out targetValue, out _);
                }
                else if (targetTypeKind == EdmPrimitiveTypeKind.Guid)
                {
                    Guid guidValue;
                    bool result = UriUtils.TryUriStringToGuid(text, out guidValue);
                    targetValue = guidValue;
                    return result;
                }
                else if (targetTypeKind == EdmPrimitiveTypeKind.Date)
                {
                    bool result = UriUtils.TryUriStringToDateOnly(text, out DateOnly dateValue);

                    targetValue = dateValue;
                    return result;
                }
                else if (targetTypeKind == EdmPrimitiveTypeKind.DateTimeOffset)
                {
                    DateTimeOffset dateTimeOffsetValue;
                    bool result = UriUtils.ConvertUriStringToDateTimeOffset(text, out dateTimeOffsetValue);
                    targetValue = dateTimeOffsetValue;
                    return result;
                }
                else if (targetTypeKind == EdmPrimitiveTypeKind.Duration)
                {
                    TimeSpan timespanValue;
                    bool result = TryUriStringToDuration(text, out timespanValue);
                    targetValue = timespanValue;
                    return result;
                }
                else if (targetTypeKind == EdmPrimitiveTypeKind.Geography)
                {
                    Geography geographyValue;
                    bool result = TryUriStringToGeography(text, out geographyValue, out exception);
                    targetValue = geographyValue;
                    return result;
                }
                else if (targetTypeKind == EdmPrimitiveTypeKind.Geometry)
                {
                    Geometry geometryValue;
                    bool result = TryUriStringToGeometry(text, out geometryValue, out exception);
                    targetValue = geometryValue;
                    return result;
                }
                else if (targetTypeKind == EdmPrimitiveTypeKind.TimeOfDay)
                {
                    TimeOnly timeOnlyValue;
                    bool result = UriUtils.TryUriStringToTimeOnly(text, out timeOnlyValue);
                    targetValue = timeOnlyValue;
                    return result;
                }
                else if (targetTypeKind == EdmPrimitiveTypeKind.String)
                {
                    if (UriParserHelper.TryRemoveQuotes(ref text, out string textString))
                    {
                        targetValue = textString ?? text.ToString();
                        return true;
                    }

                    targetValue = null;
                    return false;
                }

                try
                {
                    switch (targetTypeKind)
                    {
                        case EdmPrimitiveTypeKind.Boolean:
                            targetValue = text.ToBoolean();
                            break;
                        case EdmPrimitiveTypeKind.Byte:
                            targetValue = text.ToByte();
                            break;
                        case EdmPrimitiveTypeKind.SByte:
                            targetValue = text.ToSByte();
                            break;
                        case EdmPrimitiveTypeKind.Int16:
                            targetValue = text.ToInt16();
                            break;
                        case EdmPrimitiveTypeKind.Int32:
                            targetValue = text.ToInt32();

                            break;
                        case EdmPrimitiveTypeKind.Int64:
                            UriParserHelper.TryRemoveLiteralSuffix(ExpressionConstants.LiteralSuffixInt64, ref text);
                            targetValue = text.ToInt64();
                            break;
                        case EdmPrimitiveTypeKind.Single:
                            UriParserHelper.TryRemoveLiteralSuffix(ExpressionConstants.LiteralSuffixSingle, ref text);
                            targetValue = text.ToSingle();
                            break;
                        case EdmPrimitiveTypeKind.Double:
                            UriParserHelper.TryRemoveLiteralSuffix(ExpressionConstants.LiteralSuffixDouble, ref text);
                            targetValue = text.ToDouble();
                            break;
                        case EdmPrimitiveTypeKind.Decimal:
                            UriParserHelper.TryRemoveLiteralSuffix(ExpressionConstants.LiteralSuffixDecimal, ref text);
                            try
                            {
                                targetValue = text.ToDecimal();
                            }
                            catch (FormatException)
                            {
                                // we need to support exponential format for decimals since we used to support them in V1
                                decimal result;
                                if (decimal.TryParse(text, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result))
                                {
                                    targetValue = result;
                                }
                                else
                                {
                                    targetValue = default(decimal);
                                    return false;
                                }
                            }

                            break;
                        default:
                            throw new ODataException(Error.Format(SRResources.General_InternalError, InternalErrorCodes.UriPrimitiveTypeParser_TryUriStringToPrimitive));
                    }

                    return true;
                }
                catch (FormatException)
                {
                    targetValue = null;
                    return false;
                }
                catch (OverflowException)
                {
                    targetValue = null;
                    return false;
                }
            }

            catch (FormatException primitiveParserException)
            {
                exception = new UriLiteralParsingException(
                    Error.Format(SRResources.UriPrimitiveTypeParsers_FailedToParseTextToPrimitiveValue, text.ToString(), targetType),
                                                            primitiveParserException);
                targetValue = null;
                return false;
            }
            catch (OverflowException primitiveParserException)
            {
                exception = new UriLiteralParsingException(
                    Error.Format(SRResources.UriPrimitiveTypeParsers_FailedToParseTextToPrimitiveValue, text.ToString(), targetType),
                                                            primitiveParserException);
                targetValue = null;
                return false;
            }
            catch (ODataException primitiveParserException)
            {
                exception = new UriLiteralParsingException(
                    Error.Format(SRResources.UriPrimitiveTypeParsers_FailedToParseTextToPrimitiveValue, text.ToString(), targetType),
                                                            primitiveParserException);
                targetValue = null;
                return false;
            }
        }

        /// <summary>
        /// Converts a string to a byte[] value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToByteArray.</remarks>
        private static bool TryUriStringToByteArray(ReadOnlySpan<char> text, out byte[] targetValue)
        {
            Debug.Assert(!text.IsEmpty, "!text.IsEmpty");

            if (!UriParserHelper.TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixBinary, ref text))
            {
                targetValue = null;
                return false;
            }

            if (!UriParserHelper.TryRemoveSingleQuotes(ref text, out _))
            {
                targetValue = null;
                return false;
            }

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
                catch (FormatException)
                {
                }

                targetValue = null;
                return false;
            }
        }

        /// <summary>
        /// Converts a string to a Duration value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToTime.</remarks>
        private static bool TryUriStringToDuration(ReadOnlySpan<char> text, out TimeSpan targetValue)
        {
            if (!UriParserHelper.TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixDuration, ref text))
            {
                targetValue = default(TimeSpan);
                return false;
            }

            if (!UriParserHelper.TryRemoveSingleQuotes(ref text, out _))
            {
                targetValue = default(TimeSpan);
                return false;
            }

            try
            {
                targetValue = EdmValueParser.ParseDuration(text);
                return true;
            }
            catch (FormatException)
            {
                targetValue = default(TimeSpan);
                return false;
            }
        }

        /// <summary>
        /// Try to parse the given text to a Geography object.
        /// </summary>
        /// <param name="text">Text to parse.</param>
        /// <param name="targetValue">Geography to return.</param>
        /// <param name="parsingException">The detailed reason of parsing error.</param>
        /// <returns>True if succeeds, false if not.</returns>
        private static bool TryUriStringToGeography(ReadOnlySpan<char> text, out Geography targetValue, out UriLiteralParsingException parsingException)
        {
            parsingException = null;

            if (!UriParserHelper.TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixGeography, ref text))
            {
                targetValue = default(Geography);
                return false;
            }

            if (!UriParserHelper.TryRemoveSingleQuotes(ref text, out string value))
            {
                targetValue = default(Geography);
                return false;
            }

            try
            {
                string textString = value ?? text.ToString();
                targetValue = LiteralUtils.ParseGeography(textString);
                return true;
            }
            catch (ParseErrorException e)
            {
                targetValue = default(Geography);
                parsingException = new UriLiteralParsingException(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Try to parse the given text to a Geometry object.
        /// </summary>
        /// <param name="text">Text to parse.</param>
        /// <param name="targetValue">Geometry to return.</param>
        /// <param name="parsingFailureReasonException">The detailed reason of parsing error.</param>
        /// <returns>True if succeeds, false if not.</returns>
        private static bool TryUriStringToGeometry(ReadOnlySpan<char> text, out Geometry targetValue, out UriLiteralParsingException parsingFailureReasonException)
        {
            parsingFailureReasonException = null;

            if (!UriParserHelper.TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixGeometry, ref text))
            {
                targetValue = default(Geometry);
                return false;
            }

            if (!UriParserHelper.TryRemoveSingleQuotes(ref text, out string value))
            {
                targetValue = default(Geometry);
                return false;
            }

            try
            {
                string textString = value ?? text.ToString();
                targetValue = LiteralUtils.ParseGeometry(textString);
                return true;
            }
            catch (ParseErrorException e)
            {
                targetValue = default(Geometry);

                parsingFailureReasonException =
                    new UriLiteralParsingException(e.Message);
                return false;
            }
        }

        #endregion
    }
}