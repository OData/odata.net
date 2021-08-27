//---------------------------------------------------------------------
// <copyright file="ContentTypeUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client
#else
namespace Microsoft.OData.Service
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
#if ODATA_CLIENT
    using System.Net;
#endif
    using Microsoft.OData;

    /// <summary>Provides helper methods for processing HTTP requests.</summary>
    internal static class ContentTypeUtil
    {
        /// <summary>UTF-8 encoding, without the BOM preamble.</summary>
        /// <remarks>
        /// While a BOM preamble on UTF8 is generally benign, it seems that some MIME handlers under IE6 will not
        /// process the payload correctly when included.
        ///
        /// Because the data service should include the encoding as part of the Content-Type in the response,
        /// there should be no ambiguity as to what encoding is being used.
        ///
        /// For further information, see http://www.unicode.org/faq/utf_bom.html#BOM.
        /// </remarks>
        internal static readonly UTF8Encoding EncodingUtf8NoPreamble = new UTF8Encoding(false, true);

#if !ODATA_CLIENT
        /// <summary>
        /// Allowable Media Types for an Entity or Feed in V2.
        /// </summary>
        private static readonly string[] MediaTypesForEntityOrFeedV2 = new string[]
            {
                XmlConstants.MimeApplicationJson,
                XmlConstants.MimeApplicationAtom,
            };

        /// <summary>
        /// Allowable Media Types for something besides an Entity or Feed in V2.
        /// </summary>
        private static readonly string[] MediaTypesForOtherV2 = new string[]
            {
                XmlConstants.MimeApplicationJson,
                XmlConstants.MimeApplicationXml,
                XmlConstants.MimeTextXml,
            };

        /// <summary>
        /// Allowable Media Types for Entities or Feeds in V3.
        /// </summary>
        private static readonly string[] MediaTypesForEntityOrFeedV3 = new string[]
            {
                XmlConstants.MimeApplicationJson,
                XmlConstants.MimeApplicationAtom,
                XmlConstants.MimeApplicationJsonODataMinimalMetadata,
                XmlConstants.MimeApplicationJsonODataFullMetadata,
                XmlConstants.MimeApplicationJsonODataNoMetadata,
            };

        /// <summary>
        /// Allowable Media Types for something other than Entities or Feeds in V3.
        /// </summary>
        private static readonly string[] MediaTypesForOtherV3 = new string[]
            {
                XmlConstants.MimeApplicationJson,
                XmlConstants.MimeApplicationXml,
                XmlConstants.MimeTextXml,
                XmlConstants.MimeApplicationJsonODataMinimalMetadata,
                XmlConstants.MimeApplicationJsonODataFullMetadata,
                XmlConstants.MimeApplicationJsonODataNoMetadata,
            };
#endif

        /// <summary>Encoding to fall back to an appropriate encoding is not available.</summary>
        internal static Encoding FallbackEncoding
        {
            get
            {
                return EncodingUtf8NoPreamble;
            }
        }

        /// <summary>Encoding implied by an unspecified encoding value.</summary>
        /// <remarks>See http://tools.ietf.org/html/rfc2616#section-3.4.1 for details.</remarks>
        private static Encoding MissingEncoding
        {
            get
            {
                return Encoding.GetEncoding("ISO-8859-1", new EncoderExceptionFallback(), new DecoderExceptionFallback());
            }
        }

#if !ODATA_CLIENT

        /// <summary>Selects an acceptable MIME type that satisfies the Accepts header.</summary>
        /// <param name="acceptTypesText">Text for Accepts header.</param>
        /// <param name="availableTypes">
        /// Types that the server is willing to return, in descending order
        /// of preference.
        /// </param>
        /// <returns>The best MIME type for the client</returns>
        internal static string SelectMimeType(string acceptTypesText, string[] availableTypes)
        {
            Debug.Assert(availableTypes != null, "acceptableTypes != null");
            string selectedContentType = null;
            int selectedMatchingParts = -1;
            int selectedQualityValue = 0;
            int selectedPreferenceIndex = Int32.MaxValue;
            bool acceptable = false;
            bool acceptTypesEmpty = true;
            if (!String.IsNullOrEmpty(acceptTypesText))
            {
                IEnumerable<MediaType> acceptTypes = MimeTypesFromAcceptHeader(acceptTypesText);
                foreach (MediaType acceptType in acceptTypes)
                {
                    acceptTypesEmpty = false;
                    for (int i = 0; i < availableTypes.Length; i++)
                    {
                        string availableType = availableTypes[i];
                        int matchingParts = acceptType.GetMatchingParts(availableType);
                        if (matchingParts < 0)
                        {
                            continue;
                        }

                        if (matchingParts > selectedMatchingParts)
                        {
                            // A more specific type wins.
                            selectedContentType = availableType;
                            selectedMatchingParts = matchingParts;
                            selectedQualityValue = acceptType.SelectQualityValue();
                            selectedPreferenceIndex = i;
                            acceptable = selectedQualityValue != 0;
                        }
                        else if (matchingParts == selectedMatchingParts)
                        {
                            // A type with a higher q-value wins.
                            int candidateQualityValue = acceptType.SelectQualityValue();
                            if (candidateQualityValue > selectedQualityValue)
                            {
                                selectedContentType = availableType;
                                selectedQualityValue = candidateQualityValue;
                                selectedPreferenceIndex = i;
                                acceptable = selectedQualityValue != 0;
                            }
                            else if (candidateQualityValue == selectedQualityValue)
                            {
                                // A type that is earlier in the availableTypes array wins.
                                if (i < selectedPreferenceIndex)
                                {
                                    selectedContentType = availableType;
                                    selectedPreferenceIndex = i;
                                }
                            }
                        }
                    }
                }
            }

            if (acceptTypesEmpty)
            {
                selectedContentType = availableTypes[0];
            }
            else if (!acceptable)
            {
                selectedContentType = null;
            }

            return selectedContentType;
        }

        /// <summary>Gets the appropriate MIME type for the request, throwing if there is none.</summary>
        /// <param name='acceptTypesText'>Text as it appears in an HTTP Accepts header.</param>
        /// <param name='exactContentType'>Preferred content type to match if an exact media type is given - this is in descending order of preference.</param>
        /// <param name='inexactContentType'>Preferred fallback content type for inexact matches.</param>
        /// <returns>One of exactContentType or inexactContentType.</returns>
        internal static string SelectRequiredMimeType(
            string acceptTypesText,
            string[] exactContentType,
            string inexactContentType)
        {
            Debug.Assert(exactContentType != null && exactContentType.Length != 0, "exactContentType != null && exactContentType.Length != 0");
            Debug.Assert(inexactContentType != null, "inexactContentType != null");

            string selectedContentType = null;
            int selectedMatchingParts = -1;
            int selectedQualityValue = 0;
            bool acceptable = false;
            bool acceptTypesEmpty = true;
            bool foundExactMatch = false;

            if (!String.IsNullOrEmpty(acceptTypesText))
            {
                IEnumerable<MediaType> acceptTypes = MimeTypesFromAcceptHeader(acceptTypesText);
                foreach (MediaType acceptType in acceptTypes)
                {
                    acceptTypesEmpty = false;
                    for (int i = 0; i < exactContentType.Length; i++)
                    {
                        if (CompareMimeType(acceptType.MimeType, exactContentType[i]))
                        {
                            selectedContentType = exactContentType[i];
                            selectedQualityValue = acceptType.SelectQualityValue();
                            acceptable = selectedQualityValue != 0;
                            foundExactMatch = true;
                            break;
                        }
                    }

                    if (foundExactMatch)
                    {
                        break;
                    }

                    int matchingParts = acceptType.GetMatchingParts(inexactContentType);
                    if (matchingParts < 0)
                    {
                        continue;
                    }

                    if (matchingParts > selectedMatchingParts)
                    {
                        // A more specific type wins.
                        selectedContentType = inexactContentType;
                        selectedMatchingParts = matchingParts;
                        selectedQualityValue = acceptType.SelectQualityValue();
                        acceptable = selectedQualityValue != 0;
                    }
                    else if (matchingParts == selectedMatchingParts)
                    {
                        // A type with a higher q-value wins.
                        int candidateQualityValue = acceptType.SelectQualityValue();
                        if (candidateQualityValue > selectedQualityValue)
                        {
                            selectedContentType = inexactContentType;
                            selectedQualityValue = candidateQualityValue;
                            acceptable = selectedQualityValue != 0;
                        }
                    }
                }
            }

            if (!acceptable && !acceptTypesEmpty)
            {
                throw Error.HttpHeaderFailure(415, Strings.DataServiceException_UnsupportedMediaType);
            }

            if (acceptTypesEmpty)
            {
                Debug.Assert(selectedContentType == null, "selectedContentType == null - otherwise accept types were not empty");
                selectedContentType = inexactContentType;
            }

            Debug.Assert(selectedContentType != null, "selectedContentType != null - otherwise no selection was made");
            return selectedContentType;
        }

        /// <summary>Gets the best encoding available for the specified charset request.</summary>
        /// <param name="acceptCharset">
        /// The Accept-Charset header value (eg: "iso-8859-5, unicode-1-1;q=0.8").
        /// </param>
        /// <returns>An Encoding object appropriate to the specifed charset request.</returns>
        internal static Encoding EncodingFromAcceptCharset(string acceptCharset)
        {
            // Determines the appropriate encoding mapping according to
            // RFC 2616.14.2 (http://tools.ietf.org/html/rfc2616#section-14.2).
            Encoding result = null;
            if (!String.IsNullOrEmpty(acceptCharset))
            {
                // PERF: keep a cache of original strings to resolved Encoding.
                List<CharsetPart> parts = new List<CharsetPart>(AcceptCharsetParts(acceptCharset));
                parts.Sort((x, y) => y.Quality - x.Quality);

                var encoderFallback = new EncoderExceptionFallback();
                var decoderFallback = new DecoderExceptionFallback();
                foreach (CharsetPart part in parts)
                {
                    if (part.Quality > 0)
                    {
                        // When UTF-8 is specified, select the version that doesn't use the BOM.
                        if (String.Compare(XmlConstants.Utf8Encoding, part.Charset, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            result = FallbackEncoding;
                            break;
                        }

                        try
                        {
                            result = Encoding.GetEncoding(part.Charset, encoderFallback, decoderFallback);
                            break;
                        }
                        catch (ArgumentException)
                        {
                            // This exception is thrown when the character
                            // set isn't supported - it is ignored so
                            // other possible charsets are evaluated.
                        }
                    }
                }
            }

            // No Charset was specified, or if charsets were specified, no valid charset was found.
            // Returning a different charset is also valid.
            return result ?? FallbackEncoding;
        }

        /// <summary>
        /// Selects a response format for the requestMessage's request and sets the appropriate response header.
        /// </summary>
        /// <param name="acceptTypesText">A comma-delimited list of client-supported MIME accept types.</param>
        /// <param name="entityTarget">Whether the target is an entity.</param>
        /// <param name="effectiveMaxResponseVersion">The effective max response version.</param>
        /// <returns>The selected media type.</returns>
        internal static string SelectResponseMediaType(string acceptTypesText, bool entityTarget, Version effectiveMaxResponseVersion)
        {
            string[] availableTypes = GetAvailableMediaTypes(effectiveMaxResponseVersion, entityTarget);

            string contentType = SelectMimeType(acceptTypesText, availableTypes);

            // never respond with just app/json
            if (CompareMimeType(contentType, XmlConstants.MimeApplicationJson))
            {
                contentType = XmlConstants.MimeApplicationJsonODataMinimalMetadata;
            }

            return contentType;
        }

        /// <summary>
        /// Does a ordinal ignore case comparision of the given mime types.
        /// </summary>
        /// <param name="mimeType1">mime type1.</param>
        /// <param name="mimeType2">mime type2.</param>
        /// <returns>returns true if the mime type are the same.</returns>
        internal static bool CompareMimeType(string mimeType1, string mimeType2)
        {
            return String.Equals(mimeType1, mimeType2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the response media type would be JSON light for the given accept-header text.
        /// </summary>
        /// <param name="acceptTypesText">The text from the request's accept header.</param>
        /// <param name="entityTarget">Whether the target is an entity.</param>
        /// <param name="effectiveMaxResponseVersion">The effective max response version.</param>
        /// <returns>True if the response type is Json Light.</returns>
        internal static bool IsResponseMediaTypeJsonLight(string acceptTypesText, bool entityTarget, Version effectiveMaxResponseVersion)
        {
            string selectedMediaType;
            try
            {
                selectedMediaType = SelectResponseMediaType(acceptTypesText, entityTarget, effectiveMaxResponseVersion);
            }
            catch (DataServiceException)
            {
                // The acceptTypesText does not contain a supported mime type.
                selectedMediaType = null;
            }

            return string.Equals(XmlConstants.MimeApplicationJsonODataMinimalMetadata, selectedMediaType, StringComparison.OrdinalIgnoreCase)
                || string.Equals(XmlConstants.MimeApplicationJsonODataFullMetadata, selectedMediaType, StringComparison.OrdinalIgnoreCase)
                || string.Equals(XmlConstants.MimeApplicationJsonODataNoMetadata, selectedMediaType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the response media type would be JSON light for the request.
        /// </summary>
        /// <param name="dataService">The data service instance to determine the response media type.</param>
        /// <param name="isEntryOrFeed">true if the target of the request is an entry or a feed, false otherwise.</param>
        /// <returns>true if the response type is Json Light; false otherwise</returns>
        internal static bool IsResponseMediaTypeJsonLight(IDataService dataService, bool isEntryOrFeed)
        {
            AstoriaRequestMessage requestMessage = dataService.OperationContext.RequestMessage;
            Version effectiveMaxResponseVersion = VersionUtil.GetEffectiveMaxResponseVersion(dataService.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion(), requestMessage.RequestMaxVersion);
            return IsResponseMediaTypeJsonLight(requestMessage.GetAcceptableContentTypes(), isEntryOrFeed, effectiveMaxResponseVersion);
        }

        /// <summary>
        /// Determines whether the response content type is a JSON-based format.
        /// </summary>
        /// <param name="responseContentType">The response content-type.</param>
        /// <returns>
        ///   <c>true</c> if the content-type is JSON; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsNotJson(string responseContentType)
        {
            return responseContentType == null || !responseContentType.StartsWith(XmlConstants.MimeApplicationJson, StringComparison.OrdinalIgnoreCase);
        }
#endif

#if ODATA_CLIENT
        /// <summary>Reads a Content-Type header and extracts the MIME type/subtype.</summary>
        /// <param name="contentType">The Content-Type header.</param>
        /// <param name="mime">The MIME type in standard type/subtype form, without parameters.</param>
        /// <returns>parameters of content type</returns>
        internal static MediaParameter[] ReadContentType(string contentType, out string mime)
        {
            if (String.IsNullOrEmpty(contentType))
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_ContentTypeMissing);
            }

            MediaType mediaType = ReadMediaType(contentType);
            mime = mediaType.MimeType;
            return mediaType.Parameters;
        }

        /// <summary>Builds a Content-Type given the mime type and all the parameters.</summary>
        /// <param name="mimeType">The MIME type in standard type/subtype form, without parameters.</param>
        /// <param name="parameters">Parameters to be appended in the mime type.</param>
        /// <returns>content type containing the mime type and all the parameters.</returns>
        internal static string WriteContentType(string mimeType, MediaParameter[] parameters)
        {
            Debug.Assert(!string.IsNullOrEmpty(mimeType), "!string.IsNullOrEmpty(mimeType)");
            Debug.Assert(parameters != null, "parameters != null");

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(mimeType);

            foreach (var parameter in parameters)
            {
                stringBuilder.Append(';');
                stringBuilder.Append(parameter.Name);
                stringBuilder.Append("=");
                stringBuilder.Append(parameter.GetOriginalValue());
            }

            return stringBuilder.ToString();
        }

#endif

        /// <summary>Reads a Content-Type header and extracts the MIME type/subtype and encoding.</summary>
        /// <param name="contentType">The Content-Type header.</param>
        /// <param name="mime">The MIME type in standard type/subtype form, without parameters.</param>
        /// <param name="encoding">Encoding (possibly null).</param>
        /// <returns>parameters of content type</returns>
        internal static MediaParameter[] ReadContentType(string contentType, out string mime, out Encoding encoding)
        {
            if (String.IsNullOrEmpty(contentType))
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_ContentTypeMissing);
            }

            MediaType mediaType = ReadMediaType(contentType);
            mime = mediaType.MimeType;
            encoding = mediaType.SelectEncoding();
            return mediaType.Parameters;
        }

        /// <summary>Gets the named encoding if specified.</summary>
        /// <param name="name">Name (possibly null or empty).</param>
        /// <returns>
        /// The named encoding if specified; the encoding for HTTP missing
        /// charset specification otherwise.
        /// </returns>
        /// <remarks>
        /// See http://tools.ietf.org/html/rfc2616#section-3.4.1 for details.
        /// </remarks>
        private static Encoding EncodingFromName(string name)
        {
            if (name == null)
            {
                return MissingEncoding;
            }

            name = name.Trim();
            if (name.Length == 0)
            {
                return MissingEncoding;
            }

            try
            {
                return Encoding.GetEncoding(name);
            }
            catch (ArgumentException)
            {
                // 400 - Bad Request
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_EncodingNotSupported(name));
            }
        }

#if !ODATA_CLIENT
        /// <summary>Creates a new exception for parsing errors.</summary>
        /// <param name="message">Message for error.</param>
        /// <returns>A new exception that can be thrown for a parsing error.</returns>
        private static DataServiceException CreateParsingException(string message)
        {
            // Status code "400"  ; Section 10.4.1: Bad Request
            return Error.HttpHeaderFailure(400, message);
        }

        /// <summary>
        /// Returns the list of available media types.
        /// </summary>
        /// <param name="effectiveMaxResponseVersion">The effective max response version of the request.</param>
        /// <param name="isEntityOrFeed">true if the response will contain an entity or feed.</param>
        /// <returns>A list of recognized media types.</returns>
        private static string[] GetAvailableMediaTypes(Version effectiveMaxResponseVersion, bool isEntityOrFeed)
        {
            return isEntityOrFeed ? MediaTypesForEntityOrFeedV3 : MediaTypesForOtherV3;
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
                throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
            }

            return textIndex + literal.Length == text.Length;
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

            throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
        }

        /// <summary>Returns all MIME types from the specified (non-blank) <paramref name='text' />.</summary>
        /// <param name='text'>Non-blank text, as it appears on an HTTP Accepts header.</param>
        /// <returns>An enumerable object with media type descriptions.</returns>
        private static IEnumerable<MediaType> MimeTypesFromAcceptHeader(string text)
        {
            Debug.Assert(!String.IsNullOrEmpty(text), "!String.IsNullOrEmpty(text)");
            List<MediaType> mediaTypes = new List<MediaType>();
            int textIndex = 0;
            while (!SkipWhitespace(text, ref textIndex))
            {
                string type;
                string subType;
                ReadMediaTypeAndSubtype(text, ref textIndex, out type, out subType);

                MediaParameter[] parameters = null;
                while (!SkipWhitespace(text, ref textIndex))
                {
                    if (text[textIndex] == ',')
                    {
                        textIndex++;
                        break;
                    }

                    if (text[textIndex] != ';')
                    {
                        throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeRequiresSemicolonBeforeParameter);
                    }

                    textIndex++;
                    if (SkipWhitespace(text, ref textIndex))
                    {
                        // ';' should be a leading separator, but we choose to be a
                        // bit permissive and allow it as a final delimiter as well.
                        break;
                    }

                    ReadMediaTypeParameter(text, ref textIndex, ref parameters);
                }

                mediaTypes.Add(new MediaType(type, subType, parameters));
            }

            return mediaTypes;
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
        private static void ReadQualityValue(string text, ref int textIndex, out int qualityValue)
        {
            char digit = text[textIndex++];
            if (digit == '0')
            {
                qualityValue = 0;
            }
            else if (digit == '1')
            {
                qualityValue = 1;
            }
            else
            {
                throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
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
                    throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
                }
            }
            else
            {
                qualityValue *= 1000;
            }
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
            int headerIndex = 0;        // Index of character being processed on headerValue.
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
                    throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
                }

                headerStart = headerIndex;
                headerNameEnd = headerStart;

                bool endReached = ReadToken(headerValue, ref headerNameEnd);
                if (headerNameEnd == headerIndex)
                {
                    // Invalid charset name.
                    throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
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
                                throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
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
                        throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
                    }
                }

                yield return new CharsetPart(headerValue.Substring(headerStart, headerNameEnd - headerStart), qualityValue);

                // Prepare for next charset; we require at least one comma before we process it.
                commaRequired = true;
                headerIndex = headerEnd;
            }
        }

#endif

        /// <summary>Reads the type and subtype specifications for a MIME type.</summary>
        /// <param name='text'>Text in which specification exists.</param>
        /// <param name='textIndex'>Pointer into text.</param>
        /// <param name='type'>Type of media found.</param>
        /// <param name='subType'>Subtype of media found.</param>
        private static void ReadMediaTypeAndSubtype(string text, ref int textIndex, out string type, out string subType)
        {
            Debug.Assert(text != null, "text != null");
            int textStart = textIndex;
            if (ReadToken(text, ref textIndex))
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeUnspecified);
            }

            if (text[textIndex] != '/')
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeRequiresSlash);
            }

            type = text.Substring(textStart, textIndex - textStart);
            textIndex++;

            int subTypeStart = textIndex;
            ReadToken(text, ref textIndex);

            if (textIndex == subTypeStart)
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeRequiresSubType);
            }

            subType = text.Substring(subTypeStart, textIndex - subTypeStart);
        }

        /// <summary>Reads a media type definition as used in a Content-Type header.</summary>
        /// <param name="text">Text to read.</param>
        /// <returns>The <see cref="MediaType"/> defined by the specified <paramref name="text"/></returns>
        /// <remarks>All syntactic errors will produce a 400 - Bad Request status code.</remarks>
        private static MediaType ReadMediaType(string text)
        {
            Debug.Assert(text != null, "text != null");

            string type;
            string subType;
            int textIndex = 0;
            ReadMediaTypeAndSubtype(text, ref textIndex, out type, out subType);

            MediaParameter[] parameters = null;
            while (!SkipWhitespace(text, ref textIndex))
            {
                if (text[textIndex] != ';')
                {
                    throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeRequiresSemicolonBeforeParameter);
                }

                textIndex++;
                if (SkipWhitespace(text, ref textIndex))
                {
                    // ';' should be a leading separator, but we choose to be a
                    // bit permissive and allow it as a final delimiter as well.
                    break;
                }

                ReadMediaTypeParameter(text, ref textIndex, ref parameters);
            }

            return new MediaType(type, subType, parameters);
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
        /// Skips whitespace in the specified text by advancing an index to
        /// the next non-whitespace character.
        /// </summary>
        /// <param name="text">Text to scan.</param>
        /// <param name="textIndex">Index to begin scanning from.</param>
        /// <returns>true if the end of the string was reached, false otherwise.</returns>
        private static bool SkipWhitespace(string text, ref int textIndex)
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

        /// <summary>Read a parameter for a media type/range.</summary>
        /// <param name="text">Text to read from.</param>
        /// <param name="textIndex">Pointer in text.</param>
        /// <param name="parameters">Array with parameters to grow as necessary.</param>
        private static void ReadMediaTypeParameter(string text, ref int textIndex, ref MediaParameter[] parameters)
        {
            int startIndex = textIndex;
            if (ReadToken(text, ref textIndex))
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeMissingValue);
            }

            string parameterName = text.Substring(startIndex, textIndex - startIndex);
            if (text[textIndex] != '=')
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeMissingValue);
            }

            textIndex++;

            MediaParameter parameter = ReadQuotedParameterValue(parameterName, text, ref textIndex);

            // Add the parameter name/value pair to the list.
            if (parameters == null)
            {
                parameters = new MediaParameter[1];
            }
            else
            {
                var grow = new MediaParameter[parameters.Length + 1];
                Array.Copy(parameters, grow, parameters.Length);
                parameters = grow;
            }

            parameters[parameters.Length - 1] = parameter;
        }

        /// <summary>
        /// Reads Mime type parameter value for a particular parameter in the Content-Type/Accept headers.
        /// </summary>
        /// <param name="parameterName">Name of parameter.</param>
        /// <param name="headerText">Header text.</param>
        /// <param name="textIndex">Parsing index in <paramref name="headerText"/>.</param>
        /// <returns>String representing the value of the <paramref name="parameterName"/> parameter.</returns>
        private static MediaParameter ReadQuotedParameterValue(string parameterName, string headerText, ref int textIndex)
        {
            StringBuilder parameterValue = new StringBuilder();
            bool isQuoted = false;

            // Check if the value is quoted.
            bool valueIsQuoted = false;
            if (textIndex < headerText.Length)
            {
                if (headerText[textIndex] == '\"')
                {
                    textIndex++;
                    valueIsQuoted = true;
                    isQuoted = true;
                }
            }

            while (textIndex < headerText.Length)
            {
                char currentChar = headerText[textIndex];

                if (currentChar == '\\' || currentChar == '\"')
                {
                    if (!valueIsQuoted)
                    {
                        throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_EscapeCharWithoutQuotes(parameterName));
                    }

                    textIndex++;

                    // End of quoted parameter value.
                    if (currentChar == '\"')
                    {
                        valueIsQuoted = false;
                        break;
                    }

                    if (textIndex >= headerText.Length)
                    {
                        throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_EscapeCharAtEnd(parameterName));
                    }

                    currentChar = headerText[textIndex];
                }
                else
                if (!IsHttpToken(currentChar))
                {
                    // If the given character is special, we stop processing.
                    break;
                }

                parameterValue.Append(currentChar);
                textIndex++;
            }

            if (valueIsQuoted)
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_ClosingQuoteNotFound(parameterName));
            }

            return new MediaParameter(parameterName, parameterValue.ToString(), isQuoted);
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

#if !ODATA_CLIENT
        /// <summary>Provides a struct to encapsulate a charset name and its relative desirability.</summary>
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
#endif

        /// <summary>Class to store media parameter information.</summary>
        internal class MediaParameter
        {
            /// <summary>
            /// Creates a new instance of MediaParameter.
            /// </summary>
            /// <param name="name">Name of the parameter.</param>
            /// <param name="value">Value of the parameter.</param>
            /// <param name="isQuoted">True if the value of the parameter is quoted, otherwise false.</param>
            public MediaParameter(string name, string value, bool isQuoted)
            {
                this.Name = name;
                this.Value = value;
                this.IsQuoted = isQuoted;
            }

            /// <summary>Gets the name of the parameter.</summary>
            public string Name { get; private set; }

            /// <summary>Value of the parameter.</summary>
            public string Value { get; private set; }

            /// <summary>true if the value is quoted, otherwise false.</summary>
            private bool IsQuoted { get; set; }

            /// <summary>
            /// Gets the original value of the parameter.
            /// </summary>
            /// <returns>the original value of the parameter.</returns>
            public string GetOriginalValue()
            {
                return this.IsQuoted ? "\"" + this.Value + "\"" : this.Value;
            }
        }

        /// <summary>Use this class to represent a media type definition.</summary>
        [DebuggerDisplay("MediaType [{type}/{subType}]")]
        private sealed class MediaType
        {
            /// <summary>Parameters specified on the media type.</summary>
            private readonly MediaParameter[] parameters;

            /// <summary>Sub-type specification (for example, 'plain').</summary>
            private readonly string subType;

            /// <summary>Type specification (for example, 'text').</summary>
            private readonly string type;

            /// <summary>
            /// Initializes a new <see cref="MediaType"/> read-only instance.
            /// </summary>
            /// <param name="type">Type specification (for example, 'text').</param>
            /// <param name="subType">Sub-type specification (for example, 'plain').</param>
            /// <param name="parameters">Parameters specified on the media type.</param>
            internal MediaType(string type, string subType, MediaParameter[] parameters)
            {
                Debug.Assert(type != null, "type != null");
                Debug.Assert(subType != null, "subType != null");

                this.type = type;
                this.subType = subType;
                this.parameters = parameters;
            }

            /// <summary>Returns the MIME type in standard type/subtype form, without parameters.</summary>
            internal string MimeType
            {
                get { return this.type + "/" + this.subType; }
            }

            /// <summary>media type parameters</summary>
            internal MediaParameter[] Parameters
            {
                get { return this.parameters; }
            }

#if !ODATA_CLIENT
            /// <summary>Gets a number of non-* matching types, or -1 if not matching at all.</summary>
            /// <param name="candidate">Candidate MIME type to match.</param>
            /// <returns>The number of non-* matching types, or -1 if not matching at all.</returns>
            internal int GetMatchingParts(string candidate)
            {
                Debug.Assert(candidate != null, "candidate must not be null.");

                return this.GetMatchingParts(MimeTypesFromAcceptHeader(candidate).Single());
            }

            /// <summary>Gets a number of non-* matching types, or -1 if not matching at all.</summary>
            /// <param name="candidate">Candidate MIME type to match.</param>
            /// <returns>The number of non-* matching types, or -1 if not matching at all.</returns>
            internal int GetMatchingParts(MediaType candidate)
            {
                Debug.Assert(candidate != null, "candidate must not be null.");

                int result = -1;
                if (candidate != null)
                {
                    if (this.type == "*")
                    {
                        result = 0;
                    }
                    else
                    {
                        if (candidate.subType != null)
                        {
                            string candidateType = candidate.type;
                            if (CompareMimeType(this.type, candidateType))
                            {
                                if (this.subType == "*")
                                {
                                    result = 1;
                                }
                                else
                                {
                                    string candidateSubType = candidate.subType;
                                    if (CompareMimeType(this.subType, candidateSubType))
                                    {
                                        if (String.Equals(this.GetParameterValue(XmlConstants.MimeODataParameterName), candidate.GetParameterValue(XmlConstants.MimeODataParameterName), StringComparison.OrdinalIgnoreCase))
                                        {
                                            result = 2;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return result;
            }

            /// <summary>
            /// Searches for the parameter with the given name and returns its value.
            /// </summary>
            /// <param name="parameterName">name of the parameter whose value needs to be returned.</param>
            /// <returns>returns the value of the parameter with the given name. Returns null, if the parameter is not found.</returns>
            internal string GetParameterValue(string parameterName)
            {
                if (this.parameters == null)
                {
                    return null;
                }

                foreach (MediaParameter parameterInfo in this.parameters)
                {
                    if (String.Equals(parameterInfo.Name, parameterName, StringComparison.OrdinalIgnoreCase))
                    {
                        string parameterValue = parameterInfo.Value.Trim();
                        if (parameterValue.Length > 0)
                        {
                            return parameterInfo.Value;
                        }
                    }
                }

                return null;
            }

            /// <summary>Selects a quality value for the specified type.</summary>
            /// <returns>The quality value, in range from 0 through 1000.</returns>
            /// <remarks>See http://tools.ietf.org/html/rfc2616#section-14.1 for further details.</remarks>
            internal int SelectQualityValue()
            {
                string qvalueText = this.GetParameterValue(XmlConstants.HttpQValueParameter);
                if (qvalueText == null)
                {
                    return 1000;
                }

                int result;
                int textIndex = 0;
                ReadQualityValue(qvalueText, ref textIndex, out result);

                return result;
            }
#endif

            /// <summary>
            /// Selects the encoding appropriate for this media type specification
            /// (possibly null).
            /// </summary>
            /// <returns>
            /// The encoding explicitly defined on the media type specification, or
            /// the default encoding for well-known media types.
            /// </returns>
            /// <remarks>
            /// As per http://tools.ietf.org/html/rfc2616#section-3.7, the type,
            /// subtype and parameter name attributes are case-insensitive.
            /// </remarks>
            internal Encoding SelectEncoding()
            {
                if (this.parameters != null)
                {
                    foreach (MediaParameter parameter in this.parameters)
                    {
                        if (String.Equals(parameter.Name, XmlConstants.HttpCharsetParameter, StringComparison.OrdinalIgnoreCase))
                        {
                            string encodingName = parameter.Value.Trim();
                            if (encodingName.Length > 0)
                            {
                                return EncodingFromName(parameter.Value);
                            }
                        }
                    }
                }

                // Select the default encoding for this media type.
                if (String.Equals(this.type, XmlConstants.MimeTextType, StringComparison.OrdinalIgnoreCase))
                {
                    // HTTP 3.7.1 Canonicalization and Text Defaults
                    // "text" subtypes default to ISO-8859-1
                    //
                    // Unless the subtype is XML, in which case we should default
                    // to us-ascii. Instead we return null, to let the encoding
                    // in the <?xml ...?> PI win (http://tools.ietf.org/html/rfc3023#section-3.1)
                    if (String.Equals(this.subType, XmlConstants.MimeXmlSubType, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                    return MissingEncoding;
                }

                if (String.Equals(this.type, XmlConstants.MimeApplicationType, StringComparison.OrdinalIgnoreCase) &&
                    String.Equals(this.subType, XmlConstants.MimeJsonSubType, StringComparison.OrdinalIgnoreCase))
                {
                    // http://tools.ietf.org/html/rfc4627#section-3
                    // The default encoding is UTF-8.
                    return FallbackEncoding;
                }

                return null;
            }
        }
    }
}
