//---------------------------------------------------------------------
// <copyright file="UriPrimitiveTypeParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;
    using System.Xml;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Parsers.Common;
    using Microsoft.OData.Core.UriParser.Parsers.UriParsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Spatial;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// Parser which consumes the URI format of primitive types and converts it to primitive types.
    /// </summary>
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

        public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException)
        {
            object targetValue;

            if (this.TryUriStringToPrimitive(text, targetType, out targetValue, out parsingException))
            {
                return targetValue;
            }

            return null;
        }

        #endregion

        #region Internal Methods

        internal bool TryParseUriStringToType(string text, IEdmTypeReference targetType, out object targetValue, out UriLiteralParsingException parsingException)
        {
            return this.TryUriStringToPrimitive(text, targetType, out targetValue, out parsingException);
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
        [SuppressMessage("DataWeb.Usage", "AC0014:DoNotHandleProhibitedExceptionsRule", Justification = "We're calling this correctly")]
        private bool TryUriStringToPrimitive(string text, IEdmTypeReference targetType, out object targetValue, out UriLiteralParsingException exception)
        {
            Debug.Assert(text != null, "text != null");
            Debug.Assert(targetType != null, "targetType != null");
            exception = null;

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
                bool binaryResult = TryUriStringToByteArray(text, out byteArrayValue);
                if (targetTypeKind == EdmPrimitiveTypeKind.Binary)
                {
                    targetValue = (object)byteArrayValue;
                    return binaryResult;
                }
                else if (binaryResult)
                {
                    string keyValue = Encoding.UTF8.GetString(byteArrayValue, 0, byteArrayValue.Length);
                    return TryUriStringToPrimitive(keyValue, targetType, out targetValue);
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
                    Date dateValue;
                    bool result = UriUtils.TryUriStringToDate(text, out dateValue);
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
                    TimeOfDay timeOfDayValue;
                    bool result = UriUtils.TryUriStringToTimeOfDay(text, out timeOfDayValue);
                    targetValue = timeOfDayValue;
                    return result;
                }

                bool quoted = targetTypeKind == EdmPrimitiveTypeKind.String;
                if (quoted != UriParserHelper.IsUriValueQuoted(text))
                {
                    targetValue = null;
                    return false;
                }

                if (quoted)
                {
                    text = UriParserHelper.RemoveQuotes(text);
                }

                try
                {
                    switch (targetTypeKind)
                    {
                        case EdmPrimitiveTypeKind.String:
                            targetValue = text;
                            break;
                        case EdmPrimitiveTypeKind.Boolean:
                            targetValue = XmlConvert.ToBoolean(text);
                            break;
                        case EdmPrimitiveTypeKind.Byte:
                            targetValue = XmlConvert.ToByte(text);
                            break;
                        case EdmPrimitiveTypeKind.SByte:
                            targetValue = XmlConvert.ToSByte(text);
                            break;
                        case EdmPrimitiveTypeKind.Int16:
                            targetValue = XmlConvert.ToInt16(text);
                            break;
                        case EdmPrimitiveTypeKind.Int32:
                            targetValue = XmlConvert.ToInt32(text);
                            break;
                        case EdmPrimitiveTypeKind.Int64:
                            UriParserHelper.TryRemoveLiteralSuffix(ExpressionConstants.LiteralSuffixInt64, ref text);
                            targetValue = XmlConvert.ToInt64(text);
                            break;
                        case EdmPrimitiveTypeKind.Single:
                            UriParserHelper.TryRemoveLiteralSuffix(ExpressionConstants.LiteralSuffixSingle, ref text);
                            targetValue = XmlConvert.ToSingle(text);
                            break;
                        case EdmPrimitiveTypeKind.Double:
                            UriParserHelper.TryRemoveLiteralSuffix(ExpressionConstants.LiteralSuffixDouble, ref text);
                            targetValue = XmlConvert.ToDouble(text);
                            break;
                        case EdmPrimitiveTypeKind.Decimal:
                            UriParserHelper.TryRemoveLiteralSuffix(ExpressionConstants.LiteralSuffixDecimal, ref text);
                            try
                            {
                                targetValue = XmlConvert.ToDecimal(text);
                            }
                            catch (FormatException)
                            {
                                // we need to support exponential format for decimals since we used to support them in V1
                                decimal result;
                                if (Decimal.TryParse(text, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result))
                                {
                                    targetValue = result;
                                }
                                else
                                {
                                    targetValue = default(Decimal);
                                    return false;
                                }
                            }

                            break;
                        default:
                            throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.UriPrimitiveTypeParser_TryUriStringToPrimitive));
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
            catch (Exception primitiveParserException)
            {
                exception = new UriLiteralParsingException(
                    string.Format(CultureInfo.InvariantCulture, ODataErrorStrings.UriPrimitiveTypeParsers_FailedToParseTextToPrimitiveValue(text, targetType),
                                                        primitiveParserException));
                targetValue = null;
                return false;
            }
        }

        /// <summary>Converts a string to a primitive value.</summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetType">Type to convert string to.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of the WebConvert.TryKeyStringToPrimitive</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity is not too high; handling all the cases in one method is preferable to refactoring.")]
        private bool TryUriStringToPrimitive(string text, IEdmTypeReference targetType, out object targetValue)
        {
            UriLiteralParsingException exception;
            return TryUriStringToPrimitive(text, targetType, out targetValue, out exception);
        }

        /// <summary>
        /// Converts a string to a byte[] value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToByteArray.</remarks>
        private static bool TryUriStringToByteArray(string text, out byte[] targetValue)
        {
            Debug.Assert(text != null, "text != null");

            if (!UriParserHelper.TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixBinary, ref text))
            {
                targetValue = null;
                return false;
            }

            if (!UriParserHelper.TryRemoveQuotes(ref text))
            {
                targetValue = null;
                return false;
            }

            try
            {
                targetValue = Convert.FromBase64String(text);
            }
            catch (FormatException)
            {
                targetValue = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Converts a string to a Duration value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToTime.</remarks>
        private static bool TryUriStringToDuration(string text, out TimeSpan targetValue)
        {
            if (!UriParserHelper.TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixDuration, ref text))
            {
                targetValue = default(TimeSpan);
                return false;
            }

            if (!UriParserHelper.TryRemoveQuotes(ref text))
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
        private static bool TryUriStringToGeography(string text, out Geography targetValue, out UriLiteralParsingException parsingException)
        {
            parsingException = null;

            if (!UriParserHelper.TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixGeography, ref text))
            {
                targetValue = default(Geography);
                return false;
            }

            if (!UriParserHelper.TryRemoveQuotes(ref text))
            {
                targetValue = default(Geography);
                return false;
            }

            try
            {
                targetValue = LiteralUtils.ParseGeography(text);
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
        private static bool TryUriStringToGeometry(string text, out Geometry targetValue, out UriLiteralParsingException parsingFailureReasonException)
        {
            parsingFailureReasonException = null;

            if (!UriParserHelper.TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixGeometry, ref text))
            {
                targetValue = default(Geometry);
                return false;
            }

            if (!UriParserHelper.TryRemoveQuotes(ref text))
            {
                targetValue = default(Geometry);
                return false;
            }

            try
            {
                targetValue = LiteralUtils.ParseGeometry(text);
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