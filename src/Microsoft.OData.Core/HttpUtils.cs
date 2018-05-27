//---------------------------------------------------------------------
// <copyright file="HttpUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods to work with HTTP concepts
    /// </summary>
    internal static class HttpUtils
    {
        /// <summary>Reads a Content-Type header and extracts the media type's name (type/subtype) and parameters.</summary>
        /// <param name="contentType">The Content-Type header.</param>
        /// <param name="mediaTypeName">The media type in standard type/subtype form, without parameters.</param>
        /// <param name="mediaTypeCharset">The (optional) charset parameter of the media type.</param>
        /// <returns>The parameters of the media type not including the 'charset' parameter.</returns>
        internal static IList<KeyValuePair<string, string>> ReadMimeType(string contentType, out string mediaTypeName, out string mediaTypeCharset)
        {
            if (String.IsNullOrEmpty(contentType))
            {
                throw new ODataContentTypeException(Strings.HttpUtils_ContentTypeMissing);
            }

            IList<KeyValuePair<ODataMediaType, string>> mediaTypes = ReadMediaTypes(contentType);
            if (mediaTypes.Count != 1)
            {
                throw new ODataContentTypeException(Strings.HttpUtils_NoOrMoreThanOneContentTypeSpecified(contentType));
            }

            ODataMediaType mediaType = mediaTypes[0].Key;
            MediaTypeUtils.CheckMediaTypeForWildCards(mediaType);

            mediaTypeName = mediaType.FullTypeName;
            mediaTypeCharset = mediaTypes[0].Value;

            return mediaType.Parameters != null ? mediaType.Parameters.ToList() : null;
        }

        /// <summary>Builds a Content-Type header which includes media type and encoding information.</summary>
        /// <param name="mediaType">Media type to be used.</param>
        /// <param name="encoding">Encoding to be used in response, possibly null.</param>
        /// <returns>The value for the Content-Type header.</returns>
        internal static string BuildContentType(ODataMediaType mediaType, Encoding encoding)
        {
            Debug.Assert(mediaType != null, "mediaType != null");

            return mediaType.ToText(encoding);
        }

        /// <summary>Returns all media types from the specified (non-blank) <paramref name='text' />.</summary>
        /// <param name='text'>Non-blank text, as it appears on an HTTP Accepts header.</param>
        /// <returns>An enumerable object with key/value pairs of media type descriptions with their (optional) charset parameter values.</returns>
        internal static IList<KeyValuePair<ODataMediaType, string>> MediaTypesFromString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            return ReadMediaTypes(text);
        }

        /// <summary>
        /// Does an ordinal ignore case comparision of the given media type names.
        /// </summary>
        /// <param name="mediaTypeName1">First media type name.</param>
        /// <param name="mediaTypeName2">Second media type name.</param>
        /// <returns>returns true if the media type names are the same.</returns>
        internal static bool CompareMediaTypeNames(string mediaTypeName1, string mediaTypeName2)
        {
            return string.Equals(mediaTypeName1, mediaTypeName2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Does an ordinal ignore case comparision of the given MIME type parameter name.
        /// </summary>
        /// <param name="parameterName1">First parameter name.</param>
        /// <param name="parameterName2">Second parameter name.</param>
        /// <returns>returns true if the parameter names are the same.</returns>
        internal static bool CompareMediaTypeParameterNames(string parameterName1, string parameterName2)
        {
            return string.Equals(parameterName1, parameterName2, StringComparison.OrdinalIgnoreCase)
                || (IsMetadataParameter(parameterName1) && IsMetadataParameter(parameterName2))
                || (IsStreamingParameter(parameterName1) && IsStreamingParameter(parameterName2));
        }

        /// <summary>
        /// Determines whether or not a parameter is the odata metadata parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>returns true if the parameter name is the odata metadata parameter.</returns>
        internal static bool IsMetadataParameter(string parameterName)
        {
            return (String.Compare(parameterName, MimeConstants.MimeMetadataParameterName, StringComparison.OrdinalIgnoreCase) == 0
                || String.Compare(parameterName, MimeConstants.MimeShortMetadataParameterName, StringComparison.OrdinalIgnoreCase) == 0);
        }


        /// <summary>
        /// Determines whether or not a parameter is the odata streaming parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>returns true if the parameter name is the odata streaming parameter.</returns>
        internal static bool IsStreamingParameter(string parameterName)
        {
            return (String.Compare(parameterName, MimeConstants.MimeStreamingParameterName, StringComparison.OrdinalIgnoreCase) == 0
                || String.Compare(parameterName, MimeConstants.MimeShortStreamingParameterName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>Gets the best encoding available for the specified charset request.</summary>
        /// <param name="acceptableCharsets">
        /// The Accept-Charset header value (eg: "iso-8859-5, unicode-1-1;q=0.8").
        /// </param>
        /// <param name="mediaType">The media type used to compute the default encoding for the payload.</param>
        /// <param name="utf8Encoding">The encoding to use for UTF-8 charsets; we use the one without the BOM.</param>
        /// <param name="defaultEncoding">The encoding to use if no encoding could be computed from the <paramref name="acceptableCharsets"/> or <paramref name="mediaType"/>.</param>
        /// <returns>An Encoding object appropriate to the specifed charset request.</returns>
        internal static Encoding EncodingFromAcceptableCharsets(string acceptableCharsets, ODataMediaType mediaType, Encoding utf8Encoding, Encoding defaultEncoding)
        {
            Debug.Assert(mediaType != null, "mediaType != null");

            // Determines the appropriate encoding mapping according to
            // RFC 2616.14.2 (http://tools.ietf.org/html/rfc2616#section-14.2).
            Encoding result = null;
            if (!string.IsNullOrEmpty(acceptableCharsets))
            {
                // PERF: in the future if we find that computing the encoding from the accept charsets is
                //       too expensive we could introduce a cache of original strings to resolved encoding.
                CharsetPart[] parts = new List<CharsetPart>(AcceptCharsetParts(acceptableCharsets)).ToArray();

                // NOTE: List<T>.Sort uses an unstable sort algorithm; if charsets have the same quality value
                //       we want to pick the first one specified so we need a stable sort.
                KeyValuePair<int, CharsetPart>[] sortedParts = parts.StableSort(delegate(CharsetPart x, CharsetPart y)
                {
                    return y.Quality - x.Quality;
                });

                foreach (KeyValuePair<int, CharsetPart> sortedPart in sortedParts)
                {
                    CharsetPart part = sortedPart.Value;
                    if (part.Quality > 0)
                    {
                        // When UTF-8 is specified, select the version that doesn't use the BOM.
                        if (String.Compare("utf-8", part.Charset, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            result = utf8Encoding;
                            break;
                        }
                        else
                        {
                            result = GetEncodingFromCharsetName(part.Charset);
                            if (result != null)
                            {
                                break;
                            }

                            // If the charset is not supported it is ignored so other possible charsets are evaluated.
                        }
                    }
                }
            }

            // No Charset was specifed, or if charsets were specified, no valid charset was found.
            // Returning a different charset is also valid. Get the default encoding for the media type.
            if (result == null)
            {
                result = mediaType.SelectEncoding();
                if (result == null)
                {
                    return defaultEncoding;
                }
            }

            return result;
        }

        /// <summary>
        /// Reads the numeric part of a quality value substring, normalizing it to 0-1000
        /// rather than the standard 0.000-1.000 ranges.
        /// </summary>
        /// <param name="text">Text to read qvalue from.</param>
        /// <param name="textIndex">Index into text where the qvalue starts.</param>
        /// <param name="qualityValue">After the method executes, the normalized qvalue.</param>
        /// <remarks>
        /// For more information, see RFC 2616.3.8.
        /// </remarks>
        internal static void ReadQualityValue(string text, ref int textIndex, out int qualityValue)
        {
            char digit = text[textIndex++];
            switch (digit)
            {
                case '0':
                    qualityValue = 0;
                    break;
                case '1':
                    qualityValue = 1;
                    break;
                default:
                    throw new ODataContentTypeException(Strings.HttpUtils_InvalidQualityValueStartChar(text, digit));
            }

            if (textIndex < text.Length && text[textIndex] == '.')
            {
                textIndex++;

                int adjustFactor = 1000;
                while (adjustFactor > 1 && textIndex < text.Length)
                {
                    char c = text[textIndex];
                    int charValue = DigitToInt32(c);
                    if (charValue >= 0)
                    {
                        textIndex++;
                        adjustFactor /= 10;
                        qualityValue *= 10;
                        qualityValue += charValue;
                    }
                    else
                    {
                        break;
                    }
                }

                qualityValue *= adjustFactor;
                if (qualityValue > 1000)
                {
                    // Too high of a value in qvalue.
                    throw new ODataContentTypeException(Strings.HttpUtils_InvalidQualityValue(qualityValue / 1000, text));
                }
            }
            else
            {
                qualityValue *= 1000;
            }
        }

        /// <summary>
        /// Validates that the HTTP method string matches one of the supported HTTP methods.
        /// </summary>
        /// <param name="httpMethodString">The HTTP method string to validate.</param>
        internal static void ValidateHttpMethod(string httpMethodString)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(httpMethodString, "httpMethodString");

            if (string.CompareOrdinal(httpMethodString, ODataConstants.MethodGet) != 0
                && string.CompareOrdinal(httpMethodString, ODataConstants.MethodDelete) != 0
                && string.CompareOrdinal(httpMethodString, ODataConstants.MethodPatch) != 0
                && string.CompareOrdinal(httpMethodString, ODataConstants.MethodPost) != 0
                && string.CompareOrdinal(httpMethodString, ODataConstants.MethodPut) != 0)
            {
                throw new ODataException(Strings.HttpUtils_InvalidHttpMethodString(httpMethodString));
            }
        }

        /// <summary>
        /// Determines whether the given HTTP method is one that is accepted for queries. GET is accepted for queries.
        /// </summary>
        /// <param name="httpMethod">The HTTP method to check.</param>
        /// <returns>True if the given httpMethod is GET.</returns>
        internal static bool IsQueryMethod(string httpMethod)
        {
            return string.CompareOrdinal(httpMethod, ODataConstants.MethodGet) == 0;
        }

        /// <summary>
        /// Gets the string status message for a given Http response status code.
        /// </summary>
        /// <param name="statusCode">The status code to get the status message for.</param>
        /// <returns>The string status message for the <paramref name="statusCode"/>.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This is a large switch on all the Http response codes; no complexity here.")]
        internal static string GetStatusMessage(int statusCode)
        {
            // Non-localized messages for status codes.
            // These are the recommended reason phrases as per HTTP RFC 2616, Section 6.1.1
            switch (statusCode)
            {
                case 100:
                    return "Continue";
                case 101:
                    return "Switching Protocols";
                case 200:
                    return "OK";
                case 201:
                    return "Created";
                case 202:
                    return "Accepted";
                case 203:
                    return "Non-Authoritative Information";
                case 204:
                    return "No Content";
                case 205:
                    return "Reset Content";
                case 206:
                    return "Partial Content";
                case 300:
                    return "Multiple Choices";
                case 301:
                    return "Moved Permanently";
                case 302:
                    return "Found";
                case 303:
                    return "See Other";
                case 304:
                    return "Not Modified";
                case 305:
                    return "Use Proxy";
                case 307:
                    return "Temporary Redirect";
                case 400:
                    return "Bad Request";
                case 401:
                    return "Unauthorized";
                case 402:
                    return "Payment Required";
                case 403:
                    return "Forbidden";
                case 404:
                    return "Not Found";
                case 405:
                    return "Method Not Allowed";
                case 406:
                    return "Not Acceptable";
                case 407:
                    return "Proxy Authentication Required";
                case 408:
                    return "Request Time-out";
                case 409:
                    return "Conflict";
                case 410:
                    return "Gone";
                case 411:
                    return "Length Required";
                case 412:
                    return "Precondition Failed";
                case 413:
                    return "Request Entity Too Large";
                case 414:
                    return "Request-URI Too Large";
                case 415:
                    return "Unsupported Media Type";
                case 416:
                    return "Requested range not satisfiable";
                case 417:
                    return "Expectation Failed";
                case 500:
                    return "Internal Server Error";
                case 501:
                    return "Not Implemented";
                case 502:
                    return "Bad Gateway";
                case 503:
                    return "Service Unavailable";
                case 504:
                    return "Gateway Time-out";
                case 505:
                    return "HTTP Version not supported";
                default:
                    return "Unknown Status Code";
            }
        }

        /// <summary>
        /// Returns the encoding object for the specified charset name.
        /// </summary>
        /// <param name="charsetName">The of the charset to get the encoding for.</param>
        /// <returns>The encoding object or null if such encoding is not supported.</returns>
        internal static Encoding GetEncodingFromCharsetName(string charsetName)
        {
            try
            {
#if !ORCAS
                // The default behavior without the fallbacks is to use either replacement or best-fit for unencodable characters.
                // That would be the wrong behavior for us. On the other hand in Silverlight the only supported encodings
                // are UTF-8 and UTF-16 (LE and BE), all of which can encode any character, so the fallbacks are never used.
                // Thus it's OK to use the default behavior here.
                return Encoding.GetEncoding(charsetName);
#else
                return Encoding.GetEncoding(charsetName, new EncoderExceptionFallback(), new DecoderExceptionFallback());
#endif
            }
            catch (ArgumentException)
            {
                // This exception is thrown when the character
                // set isn't supported.
                return null;
            }
        }

        /// <summary>
        /// Reads a token or quoted-string value from the header.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerText">Header text.</param>
        /// <param name="textIndex">Parsing index in <paramref name="headerText"/>.</param>
        /// <param name="isQuotedString">Returns true if the value is a quoted-string, false if the value is a token.</param>
        /// <param name="createException">Func to create the appropriate exception to throw from the given error message.</param>
        /// <returns>The token or quoted-string value that was read from the header.</returns>
        internal static string ReadTokenOrQuotedStringValue(string headerName, string headerText, ref int textIndex, out bool isQuotedString, Func<string, Exception> createException)
        {
            //// NOTE: See RFC 2616, Sections 3.6 and 2.2 for the full grammar for HTTP parameter values
            ////
            //// parameter-value    =   token | quoted-string
            //// token              = 1*<any CHAR except CTLs or separators>
            //// CHAR               = <any US-ASCII character (octets 0 - 127)>
            //// CTL                = <any US-ASCII control character
            ////                      (octets 0 - 31) and DEL (127)>
            //// separators         = "(" | ")" | "<" | ">" | "@"
            ////                    | "," | ";" | ":" | "\" | <">
            ////                    | "/" | "[" | "]" | "?" | "="
            ////                    | "{" | "}" | SP | HT
            //// quoted-string      = ( <"> *(qdtext | quoted-pair ) <"> )
            //// qdtext             = <any TEXT except <">>
            //// TEXT               = <any OCTET except CTLs, but including LWS>
            //// quoted-pair        = "\" CHAR

            StringBuilder parameterValue = new StringBuilder();

            // Check if the value is quoted.
            isQuotedString = false;
            if (textIndex < headerText.Length)
            {
                if (headerText[textIndex] == '\"')
                {
                    textIndex++;
                    isQuotedString = true;
                }
            }

            char currentChar = default(char);
            while (textIndex < headerText.Length)
            {
                currentChar = headerText[textIndex];

                if (currentChar == '\\' || currentChar == '\"')
                {
                    if (!isQuotedString)
                    {
                        throw createException(Strings.HttpUtils_EscapeCharWithoutQuotes(headerName, headerText, textIndex, currentChar));
                    }

                    textIndex++;

                    // End of quoted parameter value.
                    if (currentChar == '\"')
                    {
                        break;
                    }

                    if (textIndex >= headerText.Length)
                    {
                        throw createException(Strings.HttpUtils_EscapeCharAtEnd(headerName, headerText, textIndex, currentChar));
                    }

                    currentChar = headerText[textIndex];
                }
                else
                {
                    if (!isQuotedString && !IsHttpToken(currentChar))
                    {
                        // If the given character is special, we stop processing.
                        break;
                    }

                    if (isQuotedString && !IsValidInQuotedHeaderValue(currentChar))
                    {
                        throw createException(Strings.HttpUtils_InvalidCharacterInQuotedParameterValue(headerName, headerText, textIndex, currentChar));
                    }
                }

                parameterValue.Append(currentChar);
                textIndex++;
            }

            if (isQuotedString && currentChar != '\"')
            {
                throw createException(Strings.HttpUtils_ClosingQuoteNotFound(headerName, headerText, textIndex));
            }

            return parameterValue.ToString();
        }

        /// <summary>
        /// Skips whitespace in the specified text by advancing an index to
        /// the next non-whitespace character.
        /// </summary>
        /// <param name="text">Text to scan.</param>
        /// <param name="textIndex">Index to begin scanning from.</param>
        /// <returns>true if the end of the string was reached, false otherwise.</returns>
        internal static bool SkipWhitespace(string text, ref int textIndex)
        {
            Debug.Assert(text != null, "text != null");
            Debug.Assert(text.Length >= 0, "text >= 0");
            Debug.Assert(textIndex <= text.Length, "text <= text.Length");

            while (textIndex < text.Length && Char.IsWhiteSpace(text, textIndex))
            {
                textIndex++;
            }

            return (textIndex == text.Length);
        }

        /// <summary>
        /// Enumerates each charset part in the specified Accept-Charset header.
        /// </summary>
        /// <param name="headerValue">Non-null and non-empty header value for Accept-Charset.</param>
        /// <returns>
        /// A (non-sorted) enumeration of CharsetPart elements, which include
        /// a charset name and a quality (preference) value, normalized to 0-1000.
        /// </returns>
        private static IEnumerable<CharsetPart> AcceptCharsetParts(string headerValue)
        {
            Debug.Assert(!String.IsNullOrEmpty(headerValue), "!String.IsNullOrEmpty(headerValuer)");

            // PERF: optimize for common patterns.
            bool commaRequired = false; // Whether a comma should be found
            int headerIndex = 0;        // Index of character being procesed on headerValue.
            int headerStart;            // Index into headerValue for the start of the charset name.
            int headerNameEnd;          // Index into headerValue for the end of the charset name (+1).
            int headerEnd;              // Index into headerValue for this charset part (+1).
            int qualityValue;           // Normalized qvalue for this charset.

            while (headerIndex < headerValue.Length)
            {
                if (SkipWhitespace(headerValue, ref headerIndex))
                {
                    yield break;
                }

                if (headerValue[headerIndex] == ',')
                {
                    commaRequired = false;
                    headerIndex++;
                    continue;
                }

                if (commaRequired)
                {
                    // Comma missing between charset elements.
                    throw new ODataContentTypeException(Strings.HttpUtils_MissingSeparatorBetweenCharsets(headerValue));
                }

                headerStart = headerIndex;
                headerNameEnd = headerStart;

                bool endReached = ReadToken(headerValue, ref headerNameEnd);
                if (headerNameEnd == headerIndex)
                {
                    // Invalid (empty) charset name.
                    throw new ODataContentTypeException(Strings.HttpUtils_InvalidCharsetName(headerValue));
                }

                if (endReached)
                {
                    qualityValue = 1000;
                    headerEnd = headerNameEnd;
                }
                else
                {
                    char afterNameChar = headerValue[headerNameEnd];
                    if (IsHttpSeparator(afterNameChar))
                    {
                        if (afterNameChar == ';')
                        {
                            if (ReadLiteral(headerValue, headerNameEnd, ";q="))
                            {
                                // Unexpected end of qvalue.
                                throw new ODataContentTypeException(Strings.HttpUtils_UnexpectedEndOfQValue(headerValue));
                            }

                            headerEnd = headerNameEnd + 3;
                            ReadQualityValue(headerValue, ref headerEnd, out qualityValue);
                        }
                        else
                        {
                            qualityValue = 1000;
                            headerEnd = headerNameEnd;
                        }
                    }
                    else
                    {
                        // Invalid separator character.
                        throw new ODataContentTypeException(Strings.HttpUtils_InvalidSeparatorBetweenCharsets(headerValue));
                    }
                }

                yield return new CharsetPart(headerValue.Substring(headerStart, headerNameEnd - headerStart), qualityValue);

                // Prepare for next charset; we require at least one comma before we process it.
                commaRequired = true;
                headerIndex = headerEnd;
            }
        }

        /// <summary>Reads a media type definition as used in a Content-Type header.</summary>
        /// <param name="text">Text to read.</param>
        /// <returns>A list of key/value pairs representing the <see cref="ODataMediaType"/>s and their (optional) 'charset' parameters
        /// parsed from the specified <paramref name="text"/></returns>
        private static IList<KeyValuePair<ODataMediaType, string>> ReadMediaTypes(string text)
        {
            Debug.Assert(text != null, "text != null");

            List<KeyValuePair<string, string>> parameters = null;
            List<KeyValuePair<ODataMediaType, string>> mediaTypes = new List<KeyValuePair<ODataMediaType, string>>();
            int textIndex = 0;

            while (!SkipWhitespace(text, ref textIndex))
            {
                string type;
                string subType;
                ReadMediaTypeAndSubtype(text, ref textIndex, out type, out subType);

                string charset = null;
                while (!SkipWhitespace(text, ref textIndex))
                {
                    if (text[textIndex] == ',')
                    {
                        textIndex++;
                        break;
                    }

                    if (text[textIndex] != ';')
                    {
                        throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeRequiresSemicolonBeforeParameter(text));
                    }

                    textIndex++;
                    if (SkipWhitespace(text, ref textIndex))
                    {
                        // ';' should be a leading separator, but we choose to be a
                        // bit permissive and allow it as a final delimiter as well.
                        break;
                    }

                    ReadMediaTypeParameter(text, ref textIndex, ref parameters, ref charset);
                }

                mediaTypes.Add(
                    new KeyValuePair<ODataMediaType, string>(
                        new ODataMediaType(type, subType, parameters),
                        charset));
                parameters = null;
            }

            return mediaTypes;
        }

        /// <summary>Read a parameter for a media type/range.</summary>
        /// <param name="text">Text to read from.</param>
        /// <param name="textIndex">Pointer in text.</param>
        /// <param name="parameters">Array with parameters to grow as necessary.</param>
        /// <param name="charset">The (optional) charset parameter value.</param>
        private static void ReadMediaTypeParameter(string text, ref int textIndex, ref List<KeyValuePair<string, string>> parameters, ref string charset)
        {
            int startIndex = textIndex;
            bool eof = ReadToken(text, ref textIndex);
            string parameterName = text.Substring(startIndex, textIndex - startIndex);

            if (parameterName.Length == 0)
            {
                throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeMissingParameterName);
            }

            if (eof)
            {
                throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeMissingParameterValue(parameterName));
            }

            if (text[textIndex] != '=')
            {
                throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeMissingParameterValue(parameterName));
            }

            textIndex++;

            bool isQuotedString;
            string parameterValue = ReadTokenOrQuotedStringValue(ODataConstants.ContentTypeHeader, text, ref textIndex, out isQuotedString, message => new ODataContentTypeException(message));

            if (CompareMediaTypeParameterNames(ODataConstants.Charset, parameterName))
            {
                charset = parameterValue;
            }
            else
            {
                // Add the parameter name/value pair to the list.
                if (parameters == null)
                {
                    parameters = new List<KeyValuePair<string, string>>(1);
                }

                parameters.Add(new KeyValuePair<string, string>(parameterName, parameterValue));
            }
        }

        /// <summary>Reads the type and subtype specifications for a media type name.</summary>
        /// <param name='mediaTypeName'>Text in which specification exists.</param>
        /// <param name='textIndex'>Pointer into text.</param>
        /// <param name='type'>Type of media found.</param>
        /// <param name='subType'>Subtype of media found.</param>
        private static void ReadMediaTypeAndSubtype(string mediaTypeName, ref int textIndex, out string type, out string subType)
        {
            Debug.Assert(mediaTypeName != null, "text != null");
            int textStart = textIndex;
            if (ReadToken(mediaTypeName, ref textIndex))
            {
                throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeUnspecified(mediaTypeName));
            }

            if (mediaTypeName[textIndex] != '/')
            {
                throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeRequiresSlash(mediaTypeName));
            }

            type = mediaTypeName.Substring(textStart, textIndex - textStart);
            textIndex++;

            int subTypeStart = textIndex;
            ReadToken(mediaTypeName, ref textIndex);

            if (textIndex == subTypeStart)
            {
                throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeRequiresSubType(mediaTypeName));
            }

            subType = mediaTypeName.Substring(subTypeStart, textIndex - subTypeStart);
        }

        /// <summary>
        /// Determines whether the specified character is a valid HTTP header token character.
        /// </summary>
        /// <param name="c">Character to verify.</param>
        /// <returns>true if c is a valid HTTP header token character; false otherwise.</returns>
        private static bool IsHttpToken(char c)
        {
            // A token character is any character (0-127) except control (0-31) or
            // separators. 127 is DEL, a control character.
            return c < '\x7F' && c > '\x1F' && !IsHttpSeparator(c);
        }

        /// <summary>
        /// Determines whether the specified character is valid in the quoted header values.
        /// </summary>
        /// <param name="c">Character to verify.</param>
        /// <returns>true if c is a valid in a quoted HTTP header value; false otherwise.</returns>
        private static bool IsValidInQuotedHeaderValue(char c)
        {
            Debug.Assert(c != '\\' && c != '\"', "Control characters should have been handled already.");

            // Any non-control OCTET, including space and tab but excluding \". Control characters are 0 - 31 and 127.
            // NOTE: we do not check for [CRLF] since we do not support multi-line HTTP headers.
            // Batch reader handling of multiline headers
            // ODataLib batch writer support for multiline headers in operations
            int intValueOfCharacter = (int)c;
            if (intValueOfCharacter < 32 && c != ' ' && c != '\t' || intValueOfCharacter == 127)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified character is a valid HTTP separator.
        /// </summary>
        /// <param name="c">Character to verify.</param>
        /// <returns>true if c is a separator; false otherwise.</returns>
        /// <remarks>
        /// See RFC 2616 2.2 for further information.
        /// </remarks>
        private static bool IsHttpSeparator(char c)
        {
            return
                c == '(' || c == ')' || c == '<' || c == '>' || c == '@' ||
                c == ',' || c == ';' || c == ':' || c == '\\' || c == '"' ||
                c == '/' || c == '[' || c == ']' || c == '?' || c == '=' ||
                c == '{' || c == '}' || c == ' ' || c == '\x9';
        }

        /// <summary>
        /// Reads a token on the specified text by advancing an index on it.
        /// </summary>
        /// <param name="text">Text to read token from.</param>
        /// <param name="textIndex">Index for the position being scanned on text.</param>
        /// <returns>true if the end of the text was reached; false otherwise.</returns>
        private static bool ReadToken(string text, ref int textIndex)
        {
            while (textIndex < text.Length && IsHttpToken(text[textIndex]))
            {
                textIndex++;
            }

            return (textIndex == text.Length);
        }

        /// <summary>
        /// Converts the specified character from the ASCII range to a digit.
        /// </summary>
        /// <param name="c">Character to convert.</param>
        /// <returns>
        /// The Int32 value for c, or -1 if it is an element separator.
        /// </returns>
        private static int DigitToInt32(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }

            if (IsHttpElementSeparator(c))
            {
                return -1;
            }

            throw new ODataException(Strings.HttpUtils_CannotConvertCharToInt(c));
        }

        /// <summary>
        /// Verfies whether the specified character is a valid separator in
        /// an HTTP header list of element.
        /// </summary>
        /// <param name="c">Character to verify.</param>
        /// <returns>true if c is a valid character for separating elements; false otherwise.</returns>
        private static bool IsHttpElementSeparator(char c)
        {
            return c == ',' || c == ' ' || c == '\t';
        }

        /// <summary>
        /// "Reads" a literal from the specified string by verifying that
        /// the exact text can be found at the specified position.
        /// </summary>
        /// <param name="text">Text within which a literal should be checked.</param>
        /// <param name="textIndex">Index in text where the literal should be found.</param>
        /// <param name="literal">Literal to check at the specified position.</param>
        /// <returns>true if the end of string is found; false otherwise.</returns>
        private static bool ReadLiteral(string text, int textIndex, string literal)
        {
            if (String.Compare(text, textIndex, literal, 0, literal.Length, StringComparison.Ordinal) != 0)
            {
                // Failed to find expected literal.
                throw new ODataException(Strings.HttpUtils_ExpectedLiteralNotFoundInString(literal, textIndex, text));
            }

            return textIndex + literal.Length == text.Length;
        }

        /// <summary>
        /// Structure to represent a charset name with a quality value.
        /// </summary>
        private struct CharsetPart
        {
            /// <summary>Name of the charset.</summary>
            internal readonly string Charset;

            /// <summary>Charset quality (desirability), normalized to 0-1000.</summary>
            internal readonly int Quality;

            /// <summary>
            /// Initializes a new CharsetPart with the specified values.
            /// </summary>
            /// <param name="charset">Name of charset.</param>
            /// <param name="quality">Charset quality (desirability), normalized to 0-1000.</param>
            internal CharsetPart(string charset, int quality)
            {
                Debug.Assert(charset != null, "charset != null");
                Debug.Assert(charset.Length > 0, "charset.Length > 0");
                Debug.Assert(0 <= quality && quality <= 1000, "0 <= quality && quality <= 1000");

                this.Charset = charset;
                this.Quality = quality;
            }
        }
    }
}
