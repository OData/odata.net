//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods to work with media types.
    /// </summary>
    internal static class MediaTypeUtils
    {
        /// <summary>An array of all the supported payload kinds.</summary>
        private static readonly ODataPayloadKind[] allSupportedPayloadKinds = new ODataPayloadKind[]
        {
            ODataPayloadKind.Feed,
            ODataPayloadKind.Entry,
            ODataPayloadKind.Property,
            ODataPayloadKind.MetadataDocument,
            ODataPayloadKind.ServiceDocument,
            ODataPayloadKind.Value,
            ODataPayloadKind.BinaryValue,
            ODataPayloadKind.Collection,
            ODataPayloadKind.EntityReferenceLinks,
            ODataPayloadKind.EntityReferenceLink,
            ODataPayloadKind.Batch,
            ODataPayloadKind.Error,
            ODataPayloadKind.Parameter,
        };

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
        private static readonly UTF8Encoding encodingUtf8NoPreamble = new UTF8Encoding(false, true);

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
        internal static UTF8Encoding EncodingUtf8NoPreamble
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return encodingUtf8NoPreamble;
            }
        }

        /// <summary>
        /// Given the Accept and the Accept-Charset headers of the request message computes the media type, encoding and <see cref="ODataFormat"/>
        /// to be used for the response message.
        /// </summary>
        /// <param name="settings">The message writer settings to use for serializing the response payload.</param>
        /// <param name="payloadKind">The kind of payload to be serialized as part of the response message.</param>
        /// <param name="mediaTypeResolver">The media type resolver to use when interpreting the content type.</param>
        /// <param name="mediaType">The media type to be used in the response message.</param>
        /// <param name="encoding">The encoding to be used in the response message.</param>
        /// <returns>The <see cref="ODataFormat"/> used when serializing the response.</returns>
        internal static ODataFormat GetContentTypeFromSettings(
            ODataMessageWriterSettings settings,
            ODataPayloadKind payloadKind,
            MediaTypeResolver mediaTypeResolver,
            out MediaType mediaType,
            out Encoding encoding)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(settings != null, "settings != null");

            // compute format, media type and encoding
            ODataFormat format;

            // get the supported and default media types for the specified payload kind
            MediaTypeWithFormat[] supportedMediaTypes = mediaTypeResolver.GetMediaTypesForPayloadKind(payloadKind);
            if (supportedMediaTypes == null || supportedMediaTypes.Length == 0)
            {
                throw new ODataContentTypeException(Strings.MediaTypeUtils_DidNotFindMatchingMediaType(null, settings.AcceptableMediaTypes));
            }

            if (settings.UseFormat == true)
            {
                Debug.Assert(settings.AcceptableMediaTypes == null, "settings.AcceptableMediaTypes == null");
                Debug.Assert(settings.AcceptableCharsets == null, "settings.AcceptableCharsets == null");

                mediaType = GetDefaultMediaType(supportedMediaTypes, settings.Format, out format);

                // NOTE the default media types don't have any parameters (in particular no 'charset' parameters)
                encoding = mediaType.SelectEncoding();
            }
            else
            {
                // parse the accept header into its parts
                IList<KeyValuePair<MediaType, string>> specifiedTypes = HttpUtils.MediaTypesFromString(settings.AcceptableMediaTypes);

                if (settings.Version == ODataVersion.V3)
                {
                    specifiedTypes = RemoveApplicationJsonFromAcceptableMediaTypes(specifiedTypes, supportedMediaTypes, settings.AcceptableMediaTypes);
                }

                MediaTypeWithFormat selectedMediaTypeWithFormat;
                string specifiedCharset = null;
                if (specifiedTypes == null || specifiedTypes.Count == 0)
                {
                    selectedMediaTypeWithFormat = supportedMediaTypes[0];
                }
                else
                {
                    // match the specified media types against the supported/default ones and get the format
                    MediaTypeMatchInfo matchInfo = MatchMediaTypes(specifiedTypes.Select(kvp => kvp.Key), supportedMediaTypes.Select(smt => smt.MediaType).ToArray());
                    if (matchInfo == null)
                    {
                        // We're calling the ToArray here since not all platforms support the string.Join which takes IEnumerable.
                        string supportedTypesAsString = string.Join(", ", supportedMediaTypes.Select(mt => mt.MediaType.ToText()).ToArray());
                        throw new ODataContentTypeException(Strings.MediaTypeUtils_DidNotFindMatchingMediaType(supportedTypesAsString, settings.AcceptableMediaTypes));
                    }

                    selectedMediaTypeWithFormat = supportedMediaTypes[matchInfo.TargetTypeIndex];
                    specifiedCharset = specifiedTypes[matchInfo.SourceTypeIndex].Value;
                }

                format = selectedMediaTypeWithFormat.Format;
                mediaType = selectedMediaTypeWithFormat.MediaType;

                // If a charset was specified with the accept header, consider it for the encoding
                string acceptableCharsets = settings.AcceptableCharsets;
                if (specifiedCharset != null)
                {
                    acceptableCharsets = acceptableCharsets == null ? specifiedCharset : specifiedCharset + "," + acceptableCharsets;
                }

                encoding = GetEncoding(acceptableCharsets, payloadKind, mediaType, /*useDefaultEncoding*/ true);
            }

            return format;
        }

        /// <summary>
        /// Determine the <see cref="ODataFormat"/> to use for the given <paramref name="contentTypeHeader"/>. If no supported content type
        /// is found an exception is thrown.
        /// </summary>
        /// <param name="contentTypeHeader">The name of the content type to be checked.</param>
        /// <param name="supportedPayloadKinds">All possiblel kinds of payload that can be read with this content type.</param>
        /// <param name="mediaTypeResolver">The media type resolver to use when interpreting the content type.</param>
        /// <param name="mediaType">The media type parsed from the <paramref name="contentTypeHeader"/>.</param>
        /// <param name="encoding">The encoding from the content type or the default encoding for the <paramref name="mediaType" />.</param>
        /// <param name="selectedPayloadKind">
        /// The payload kind that was selected form the list of <paramref name="supportedPayloadKinds"/> for the 
        /// specified <paramref name="contentTypeHeader"/>.
        /// </param>
        /// <param name="batchBoundary">The batch boundary read from the content type for batch payloads; otherwise null.</param>
        /// <returns>The <see cref="ODataFormat"/> for the <paramref name="contentTypeHeader"/>.</returns>
        internal static ODataFormat GetFormatFromContentType(
            string contentTypeHeader,
            ODataPayloadKind[] supportedPayloadKinds,
            MediaTypeResolver mediaTypeResolver,
            out MediaType mediaType,
            out Encoding encoding,
            out ODataPayloadKind selectedPayloadKind,
            out string batchBoundary)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!supportedPayloadKinds.Contains(ODataPayloadKind.Unsupported), "!supportedPayloadKinds.Contains(ODataPayloadKind.Unsupported)");

            ODataFormat format = GetFormatFromContentType(contentTypeHeader, supportedPayloadKinds, mediaTypeResolver, out mediaType, out encoding, out selectedPayloadKind);

            // for batch payloads, read the batch boundary from the content type header; this is the only
            // content type parameter we support (and that is required for batch payloads)
            if (selectedPayloadKind == ODataPayloadKind.Batch)
            {
                KeyValuePair<string, string> boundaryPair = default(KeyValuePair<string, string>);
                IEnumerable<KeyValuePair<string, string>> parameters = mediaType.Parameters;
                if (parameters != null)
                {
                    bool boundaryPairFound = false;
                    foreach (KeyValuePair<string, string> pair in parameters.Where(p => HttpUtils.CompareMediaTypeParameterNames(ODataConstants.HttpMultipartBoundary, p.Key)))
                    {
                        if (boundaryPairFound)
                        {
                            throw new ODataException(Strings.MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads(contentTypeHeader, ODataConstants.HttpMultipartBoundary));
                        }

                        boundaryPair = pair;
                        boundaryPairFound = true;
                    }
                }

                if (boundaryPair.Key == null)
                {
                    throw new ODataException(Strings.MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads(contentTypeHeader, ODataConstants.HttpMultipartBoundary));
                }

                batchBoundary = boundaryPair.Value;
                ValidationUtils.ValidateBoundaryString(batchBoundary);
            }
            else
            {
                batchBoundary = null;
            }

            return format;
        }

        /// <summary>
        /// Gets all payload kinds and their corresponding formats that match the specified content type header.
        /// </summary>
        /// <param name="contentTypeHeader">The content type header to get the payload kinds for.</param>
        /// <param name="mediaTypeResolver">The media type resolver to use when interpreting the content type.</param>
        /// <param name="contentType">The parsed content type as <see cref="MediaType"/>.</param>
        /// <param name="encoding">The encoding from the content type or the default encoding from <see cref="MediaType" />.</param>
        /// <returns>The list of payload kinds and formats supported for the specified <paramref name="contentTypeHeader"/>.</returns>
        internal static IList<ODataPayloadKindDetectionResult> GetPayloadKindsForContentType(string contentTypeHeader, MediaTypeResolver mediaTypeResolver, out MediaType contentType, out Encoding encoding)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(contentTypeHeader), "Content-Type header must not be null or empty.");

            string charset;
            encoding = null;
            contentType = ParseContentType(contentTypeHeader, out charset);
            MediaType[] targetTypes = new MediaType[] { contentType };

            List<ODataPayloadKindDetectionResult> payloadKinds = new List<ODataPayloadKindDetectionResult>();

            MediaTypeWithFormat[] mediaTypesForKind = null;
            for (int i = 0; i < allSupportedPayloadKinds.Length; ++i)
            {
                // get the supported and default media types for the current payload kind
                ODataPayloadKind payloadKind = allSupportedPayloadKinds[i];
                mediaTypesForKind = mediaTypeResolver.GetMediaTypesForPayloadKind(payloadKind);

                // match the specified media types against the supported/default ones
                // and get the format
                MediaTypeMatchInfo matchInfo = MatchMediaTypes(mediaTypesForKind.Select(smt => smt.MediaType), targetTypes);
                if (matchInfo != null)
                {
                    Debug.Assert(matchInfo.TargetTypeIndex == 0, "Invalid target type index detected.");
                    payloadKinds.Add(new ODataPayloadKindDetectionResult(payloadKind, mediaTypesForKind[matchInfo.SourceTypeIndex].Format));
                }
            }

            if (!string.IsNullOrEmpty(charset))
            {
                encoding = HttpUtils.GetEncodingFromCharsetName(charset);
            }

            return payloadKinds;
        }

        /// <summary>
        /// Checks whether two media types with subtypes (but without parameters) are equal.
        /// </summary>
        /// <param name="firstTypeAndSubtype">The first media type and subtype.</param>
        /// <param name="secondTypeAndSubtype">The second media type and subtype.</param>
        /// <returns>true if the <paramref name="firstTypeAndSubtype"/> is equal to <paramref name="secondTypeAndSubtype"/>; otherwise false.</returns>
        internal static bool MediaTypeAndSubtypeAreEqual(string firstTypeAndSubtype, string secondTypeAndSubtype)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(firstTypeAndSubtype, "firstTypeAndSubtype");
            ExceptionUtils.CheckArgumentNotNull(secondTypeAndSubtype, "secondTypeAndSubtype");

            return HttpUtils.CompareMediaTypeNames(firstTypeAndSubtype, secondTypeAndSubtype);
        }

        /// <summary>
        /// Checks whether a media type starts with the expected type and subtype.
        /// </summary>
        /// <param name="mediaType">The media type to check.</param>
        /// <param name="typeAndSubtype">The type and subtype the <paramref name="mediaType"/> should start with.</param>
        /// <returns>true if the <paramref name="mediaType"/> starts with <paramref name="typeAndSubtype"/>; otherwise false.</returns>
        internal static bool MediaTypeStartsWithTypeAndSubtype(string mediaType, string typeAndSubtype)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(mediaType, "mediaType");
            ExceptionUtils.CheckArgumentNotNull(typeAndSubtype, "typeAndSubtype");

            return mediaType.StartsWith(typeAndSubtype, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks for wildcard characters in the <see cref="MediaType"/>.
        /// </summary>
        /// <param name="mediaType">The <see cref="MediaType"/> to check.</param>
        internal static void CheckMediaTypeForWildCards(MediaType mediaType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(mediaType != null, "mediaType != null");

            if (HttpUtils.CompareMediaTypeNames(MimeConstants.MimeStar, mediaType.TypeName) ||
                HttpUtils.CompareMediaTypeNames(MimeConstants.MimeStar, mediaType.SubTypeName))
            {
                throw new ODataException(Strings.ODataMessageReader_WildcardInContentType(mediaType.FullTypeName));
            }
        }

        /// <summary>
        /// Determine the <see cref="ODataFormat"/> to use for the given <paramref name="contentTypeName"/>. If no supported content type
        /// is found an exception is thrown.
        /// </summary>
        /// <param name="contentTypeName">The name of the content type to be checked.</param>
        /// <param name="supportedPayloadKinds">All possiblel kinds of payload that can be read with this content type.</param>
        /// <param name="mediaTypeResolver">The media type resolver to use when interpreting the content type.</param>
        /// <param name="mediaType">The media type parsed from the <paramref name="contentTypeName"/>.</param>
        /// <param name="encoding">The encoding from the content type or the default encoding for the <paramref name="mediaType" />.</param>
        /// <param name="selectedPayloadKind">
        /// The payload kind that was selected form the list of <paramref name="supportedPayloadKinds"/> for the 
        /// specified <paramref name="contentTypeName"/>.
        /// </param>
        /// <returns>The <see cref="ODataFormat"/> for the <paramref name="contentTypeName"/>.</returns>
        private static ODataFormat GetFormatFromContentType(
            string contentTypeName, 
            ODataPayloadKind[] supportedPayloadKinds,
            MediaTypeResolver mediaTypeResolver,
            out MediaType mediaType, 
            out Encoding encoding,
            out ODataPayloadKind selectedPayloadKind)
        {
            Debug.Assert(!supportedPayloadKinds.Contains(ODataPayloadKind.Unsupported), "!supportedPayloadKinds.Contains(ODataPayloadKind.Unsupported)");

            string charset;
            mediaType = ParseContentType(contentTypeName, out charset);

            // Special code to handle unsupported media types that cannot be caught by conneg
            // since they are sub-types of valid media types (i.e., application/json;odata=light would
            // match application/json).
            // NOTE: once we start supporting Json Light we have to make this check version-aware.
            FailOnUnsupportedMediaTypes(mediaType, contentTypeName, supportedPayloadKinds, mediaTypeResolver);

            MediaTypeWithFormat[] supportedMediaTypes = null;
            for (int i = 0; i < supportedPayloadKinds.Length; ++i)
            {
                // get the supported and default media types for the current payload kind
                ODataPayloadKind supportedPayloadKind = supportedPayloadKinds[i];
                supportedMediaTypes = mediaTypeResolver.GetMediaTypesForPayloadKind(supportedPayloadKind);

                // match the specified media types against the supported/default ones
                // and get the format
                MediaTypeMatchInfo matchInfo = MatchMediaTypes(supportedMediaTypes.Select(smt => smt.MediaType), new MediaType[] { mediaType });
                if (matchInfo != null)
                {
                    Debug.Assert(matchInfo.TargetTypeIndex == 0, "Invalid target type index detected.");
                    selectedPayloadKind = supportedPayloadKind;
                    encoding = GetEncoding(charset, selectedPayloadKind, mediaType, /*useDefaultEncoding*/ false);
                    return supportedMediaTypes[matchInfo.SourceTypeIndex].Format;
                }
            }

            // We're calling the ToArray here since not all platforms support the string.Join which takes IEnumerable.
            Debug.Assert(supportedMediaTypes != null, "supportedMediaTypes != null");
            string supportedTypesAsString = string.Join(", ", supportedPayloadKinds.SelectMany(pk => mediaTypeResolver.GetMediaTypesForPayloadKind(pk).Select(mt => mt.MediaType.ToText())).ToArray());
            throw new ODataContentTypeException(Strings.MediaTypeUtils_CannotDetermineFormatFromContentType(supportedTypesAsString, contentTypeName));
        }

        /// <summary>
        /// Parses the specified content type header into a media type instance.
        /// </summary>
        /// <param name="contentTypeHeader">The content type header to parse.</param>
        /// <param name="charset">The optional charset specified with the content type.</param>
        /// <returns>The <see cref="MediaType"/> of the parsed <paramref name="contentTypeHeader"/>.</returns>
        private static MediaType ParseContentType(string contentTypeHeader, out string charset)
        {
            Debug.Assert(!string.IsNullOrEmpty(contentTypeHeader), "!string.IsNullOrEmpty(contentTypeHeader)");

            // parse the content type header into its parts, make sure we only allow one content type to be specified.
            IList<KeyValuePair<MediaType, string>> specifiedTypes = HttpUtils.MediaTypesFromString(contentTypeHeader);
            if (specifiedTypes.Count != 1)
            {
                throw new ODataException(Strings.MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified(contentTypeHeader));
            }

            MediaType contentType = specifiedTypes[0].Key;
            CheckMediaTypeForWildCards(contentType);
            charset = specifiedTypes[0].Value;
            return contentType;
        }

        /// <summary>
        /// Fails early on unsupported content types that cannot be caught by content negotiation/resolution.
        /// </summary>
        /// <param name="contentType">The parsed media type from the <paramref name="contentTypeName"/>.</param>
        /// <param name="contentTypeName">The name of the content type to be checked (for error reporting only).</param>
        /// <param name="supportedPayloadKinds">All possible kinds of payload that can be read with this content type (for error reporting only).</param>
        /// <param name="mediaTypeResolver">The media type resolver to use when interpreting the content type (for error reporting only).</param>
        private static void FailOnUnsupportedMediaTypes(MediaType contentType, string contentTypeName, ODataPayloadKind[] supportedPayloadKinds, MediaTypeResolver mediaTypeResolver)
        {
            // We currently fail for application/json;odata=light for all version.
            // Once we support Json Light, we will only fail for V1 and V2 where Json Light continues to not be supported.
            // NOTE: once we support custom formats and custom content negotiation, we will have to move this logic into
            //       the format itself (i.e., also register Json Light for V1 and V2 but make it fail).
            if (HttpUtils.CompareMediaTypeNames(contentType.SubTypeName, MimeConstants.MimeJsonSubType) &&
                HttpUtils.CompareMediaTypeNames(contentType.TypeName, MimeConstants.MimeApplicationType) &&
                contentType.Parameters != null &&
                contentType.Parameters.Where(p =>
                    HttpUtils.CompareMediaTypeParameterNames(p.Key, MimeConstants.MimeODataParameterName) &&
                    string.Compare(MimeConstants.MimeODataParameterValueLight, p.Value, StringComparison.OrdinalIgnoreCase) == 0).Any())
            {
                string supportedTypesAsString = string.Join(", ", supportedPayloadKinds.SelectMany(pk => mediaTypeResolver.GetMediaTypesForPayloadKind(pk).Select(mt => mt.MediaType.ToText())).ToArray());
                throw new ODataContentTypeException(Strings.MediaTypeUtils_CannotDetermineFormatFromContentType(supportedTypesAsString, contentTypeName));
            }
        }

        /// <summary>
        /// Gets the default media type for a given payload kind in a given format.
        /// </summary>
        /// <param name="supportedMediaTypes">An array of supported media types and formats.</param>
        /// <param name="specifiedFormat">The user-specified format in which to write the payload (can be null).</param>
        /// <param name="actualFormat">The default format for the specified payload kind</param>
        /// <returns>The default media type for the given payload kind and format.</returns>
        private static MediaType GetDefaultMediaType(
            MediaTypeWithFormat[] supportedMediaTypes,
            ODataFormat specifiedFormat, 
            out ODataFormat actualFormat)
        {
            for (int i = 0; i < supportedMediaTypes.Length; ++i)
            {
                // NOTE: the supportedMediaTypes are sorted (desc) by format and media type; so the 
                //       default format and media type is the first entry in the array
                MediaTypeWithFormat supportedMediaType = supportedMediaTypes[i];
                if (specifiedFormat == null || supportedMediaType.Format == specifiedFormat)
                {
                    actualFormat = supportedMediaType.Format;
                    return supportedMediaType.MediaType;
                }
            }

            throw new ODataException(Strings.ODataUtils_DidNotFindDefaultMediaType(specifiedFormat));
        }

        /// <summary>
        /// Parses the accepted charsets and matches them against the supported encodings for the given <paramref name="payloadKind"/>.
        /// </summary>
        /// <param name="acceptCharsetHeader">The Accept-Charset header of the request.</param>
        /// <param name="payloadKind">The <see cref="ODataPayloadKind"/> for which to compute the encoding.</param>
        /// <param name="mediaType">The media type used to compute the default encoding for the payload.</param>
        /// <param name="useDefaultEncoding">true if the default encoding should be returned if no acceptable charset is found; otherwise false.</param>
        /// <returns>The encoding to be used for the response.</returns>
        private static Encoding GetEncoding(string acceptCharsetHeader, ODataPayloadKind payloadKind, MediaType mediaType, bool useDefaultEncoding)
        {
            if (payloadKind == ODataPayloadKind.BinaryValue)
            {
                return null;
            }

            return HttpUtils.EncodingFromAcceptableCharsets(
                acceptCharsetHeader, 
                mediaType, 
                /*utf8Encoding*/ encodingUtf8NoPreamble, 
                /*useDefaultEncoding*/ useDefaultEncoding ? encodingUtf8NoPreamble : null);
        }

        /// <summary>
        /// Matches the supported media types against the list of media types specified in the Accept header or ContentType header of the message. Matching follows the
        /// rules for media type matching as described in RFC 2616.
        /// </summary>
        /// <param name="sourceTypes">The set of media types to be matched against the <paramref name="targetTypes"/>.</param>
        /// <param name="targetTypes">The set of media types the <paramref name="sourceTypes"/> will be matched against.</param>
        /// <returns>The best <see cref="MediaTypeMatchInfo"/> found during the matching process or null if no match was found.</returns>
        private static MediaTypeMatchInfo MatchMediaTypes(IEnumerable<MediaType> sourceTypes, MediaType[] targetTypes)
        {
            Debug.Assert(sourceTypes != null, "sourceTypes != null");
            Debug.Assert(targetTypes != null, "targetTypes != null");

            MediaTypeMatchInfo selectedMatchInfo = null;

            int sourceIndex = 0;
            if (sourceTypes != null)
            {
                foreach (MediaType sourceType in sourceTypes)
                {
                    int targetIndex = 0;
                    foreach (MediaType targetType in targetTypes)
                    {
                        // match the type name parts and parameters of the media type
                        MediaTypeMatchInfo currentMatchInfo = new MediaTypeMatchInfo(sourceType, targetType, sourceIndex, targetIndex);
                        if (!currentMatchInfo.IsMatch)
                        {
                            targetIndex++;
                            continue;
                        }

                        if (selectedMatchInfo == null)
                        {
                            selectedMatchInfo = currentMatchInfo;
                        }
                        else 
                        {
                            int comparisonResult = selectedMatchInfo.CompareTo(currentMatchInfo);
                            if (comparisonResult < 0)
                            {
                                // If the selected match is less specific than the current match, use the current match.
                                selectedMatchInfo = currentMatchInfo;
                            }
                        }

                        targetIndex++;
                    }

                    sourceIndex++;
                }
            }

            if (selectedMatchInfo == null)
            {
                return null;
            }

            return selectedMatchInfo;
        }

        /// <summary>
        /// Removes the application/json media type from the set of specified types since it is not supported in V3 initially
        /// and must not be matched to application/json;odata=verbose.
        /// If no other media types are left, the method will throw.
        /// </summary>
        /// <param name="specifiedTypes">The parsed acceptable media types.</param>
        /// <param name="supportedMediaTypes">The set of supported media types.</param>
        /// <param name="acceptableMediaTypes">The string version of the acceptable media types (for error reporting).</param>
        /// <returns>If nothing was removed the unmodified <paramref name="specifiedTypes"/>; otherwise a cloned list with the unsupported types removed.</returns>
        private static IList<KeyValuePair<MediaType, string>> RemoveApplicationJsonFromAcceptableMediaTypes(
            IList<KeyValuePair<MediaType, string>> specifiedTypes, 
            MediaTypeWithFormat[] supportedMediaTypes, 
            string acceptableMediaTypes)
        {
            if (specifiedTypes == null)
            {
                return null;
            }

            List<KeyValuePair<MediaType, string>> filteredTypes = null;
            for (int i = specifiedTypes.Count - 1; i >= 0; --i)
            {
                MediaType mediaType = specifiedTypes[i].Key;
                if (HttpUtils.CompareMediaTypeNames(mediaType.SubTypeName, MimeConstants.MimeJsonSubType) &&
                    HttpUtils.CompareMediaTypeNames(mediaType.TypeName, MimeConstants.MimeApplicationType))
                {
                    if (mediaType.Parameters == null || 
                        !mediaType.Parameters.Where(p => HttpUtils.CompareMediaTypeParameterNames(p.Key, MimeConstants.MimeODataParameterName)).Any())
                    {
                        // application/json detected; remove it from the list
                        if (filteredTypes == null)
                        {
                            filteredTypes = new List<KeyValuePair<MediaType, string>>(specifiedTypes);
                        }

                        filteredTypes.RemoveAt(i);
                    }
                }
            }

            if (filteredTypes == null)
            {
                return specifiedTypes;
            }

            if (filteredTypes.Count == 0)
            {
                // If we modified the list make sure we still have acceptable media types left; otherwise fail.
                // We're calling the ToArray here since not all platforms support the string.Join which takes IEnumerable.
                string supportedTypesAsString = string.Join(", ", supportedMediaTypes.Select(mt => mt.MediaType.ToText()).ToArray());
                throw new ODataContentTypeException(Strings.MediaTypeUtils_DidNotFindMatchingMediaType(supportedTypesAsString, acceptableMediaTypes));
            }

            return filteredTypes;
        }

        /// <summary>
        /// Class representing the result of matching two <see cref="MediaType"/> instances.
        /// </summary>
        private sealed class MediaTypeMatchInfo : IComparable<MediaTypeMatchInfo>
        {
            /// <summary>The default quality value (in the normalized range from 0 .. 1000).</summary>
            private const int DefaultQualityValue = 1000;

            /// <summary>Index of the source type in the list of all source types.</summary>
            private readonly int sourceIndex;

            /// <summary>Index of the target type in the list of all target types.</summary>
            private readonly int targetIndex;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="sourceType">The source <see cref="MediaType"/> to match against the target type.</param>
            /// <param name="targetType">The target <see cref="MediaType"/> to match against the source type.</param>
            /// <param name="sourceIndex">Index of the source type in the list of all source types.</param>
            /// <param name="targetIndex">Index of the target type in the list of all target types.</param>
            public MediaTypeMatchInfo(MediaType sourceType, MediaType targetType, int sourceIndex, int targetIndex)
            {
                Debug.Assert(sourceType != null, "sourceType != null");
                Debug.Assert(targetType != null, "targetType != null");

                this.sourceIndex = sourceIndex;
                this.targetIndex = targetIndex;

                this.MatchTypes(sourceType, targetType);
            }

            /// <summary>
            /// Index of the source type in the list of all source types.
            /// </summary>
            public int SourceTypeIndex
            {
                get
                {
                    return this.sourceIndex;
                }
            }

            /// <summary>
            /// Index of the target type in the list of all target types.
            /// </summary>
            public int TargetTypeIndex
            {
                get
                {
                    return this.targetIndex;
                }
            }

            /// <summary>
            /// Represents the number of non-* matching type name parts or -1 if not matching at all.
            /// </summary>
            public int MatchingTypeNamePartCount 
            { 
                get; 
                private set; 
            }

            /// <summary>
            /// Represents the number of matching parameters or -1 if neither the source type nor the target type have parameters.
            /// </summary>
            public int MatchingParameterCount 
            { 
                get; 
                private set; 
            }

            /// <summary>The quality value of the target type (or -1 if none is specified).</summary>
            public int QualityValue 
            { 
                get; 
                private set; 
            }

            /// <summary>
            /// The number of parameters of the source type that are used for comparison. All accept-parameters are ignored.
            /// </summary>
            public int SourceTypeParameterCountForMatching 
            { 
                get; 
                private set; 
            }

            /// <summary>
            /// true if this <see cref="MediaTypeMatchInfo"/> represents a valid match (i.e., the source and target types match/are compatible); otherwise false.
            /// </summary>
            /// <remarks>
            /// Two types are considered compatible if at least one type name part matches (or we are dealing with a wildcard)
            /// and all the parameters in the source type have been matched.
            /// </remarks>
            public bool IsMatch
            {
                get
                {
                    if (this.QualityValue == 0)
                    {
                        return false;
                    }

                    if (this.MatchingTypeNamePartCount < 0)
                    {
                        // if none of the type name parts match, the types are not compatible; continue with next acceptable type.
                        return false;
                    }

                    if (this.MatchingTypeNamePartCount > 1)
                    {
                        // make sure we matched all the parameters in the source type
                        if (this.MatchingParameterCount != -1 && this.MatchingParameterCount < this.SourceTypeParameterCountForMatching)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            /// <summary>
            /// Implementation of <see cref="IComparable&lt;MediaTypeMatchInfo&gt;"/>.
            /// </summary>
            /// <param name="other">The <see cref="MediaTypeMatchInfo"/> to compare against.</param>
            /// <returns>
            /// -1 if this instance is a worse match than <paramref name="other"/>.
            /// 0 if both matches are the same.
            /// 1 if <paramref name="other"/> is a better match than this instance.
            /// </returns>
            public int CompareTo(MediaTypeMatchInfo other)
            {
                ExceptionUtils.CheckArgumentNotNull(other, "other");

                if (this.MatchingTypeNamePartCount > other.MatchingTypeNamePartCount)
                {
                    // the current match info matches more type name parts: choose it.
                    return 1;
                }

                if (this.MatchingTypeNamePartCount == other.MatchingTypeNamePartCount)
                {
                    // Now check the parameters and see whether we find a more specific match.
                    if (this.MatchingParameterCount > other.MatchingParameterCount)
                    {
                        return 1;
                    }

                    if (this.MatchingParameterCount == other.MatchingParameterCount)
                    {
                        // check the quality value
                        int qualityComparison = this.QualityValue.CompareTo(other.QualityValue);
                        if (qualityComparison == 0)
                        {
                            // if they also have the same quality value, we choose the one
                            // that appears earlier in the conneg tables; that's the one with the smaller target type index.
                            return other.TargetTypeIndex < this.TargetTypeIndex ? -1 : 1;
                        }

                        return qualityComparison;
                    }
                }

                return -1;
            }

            /// <summary>Selects a quality value for the specified type.</summary>
            /// <param name="qualityValueText">The text representation of the quality value.</param>
            /// <returns>The quality value, in range from 0 through 1000.</returns>
            /// <remarks>See http://tools.ietf.org/html/rfc2616#section-14.1 for further details.</remarks>
            private static int ParseQualityValue(string qualityValueText)
            {
                int qualityValue = DefaultQualityValue;
                if (qualityValueText.Length > 0)
                {
                    int textIndex = 0;
                    HttpUtils.ReadQualityValue(qualityValueText, ref textIndex, out qualityValue);
                }

                return qualityValue;
            }

            /// <summary>
            /// Tries to find a parameter with the specified <paramref name="parameterName"/> in the given list <paramref name="parameters"/> of parameters.
            /// Does not include accept extensions (i.e., parameters after the q quality value parameter)
            /// </summary>
            /// <param name="parameters">The list of parameters to search.</param>
            /// <param name="parameterName">The name of the parameter to find.</param>
            /// <param name="parameterValue">The parameter value of the parameter with the specified <paramref name="parameterName"/>.</param>
            /// <returns>True if a parameter with the specified <paramref name="parameterName"/> was found; otherwise false.</returns>
            private static bool TryFindMediaTypeParameter(IList<KeyValuePair<string, string>> parameters, string parameterName, out string parameterValue)
            {
                Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

                parameterValue = null;

                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Count; ++i)
                    {
                        string candidateParameterName = parameters[i].Key;

                        if (HttpUtils.CompareMediaTypeParameterNames(parameterName, candidateParameterName))
                        {
                            parameterValue = parameters[i].Value;
                            return true;
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// Returns a flag indicating whether a given media type parameter name is the Http quality value parameter.
            /// </summary>
            /// <param name="parameterName">The parameter name to check.</param>
            /// <returns>True if the parameter name is for the quality value; otherwise false.</returns>
            private static bool IsQualityValueParameter(string parameterName)
            {
                return HttpUtils.CompareMediaTypeParameterNames(ODataConstants.HttpQValueParameter, parameterName);
            }

            /// <summary>
            /// Matches the source type against the media type.
            /// </summary>
            /// <param name="sourceType">The source <see cref="MediaType"/> to match against the target type.</param>
            /// <param name="targetType">The target <see cref="MediaType"/> to match against the source type.</param>
            private void MatchTypes(MediaType sourceType, MediaType targetType)
            {
                this.MatchingTypeNamePartCount = -1;

                if (sourceType.TypeName == "*")
                {
                    this.MatchingTypeNamePartCount = 0;
                }
                else
                {
                    if (HttpUtils.CompareMediaTypeNames(sourceType.TypeName, targetType.TypeName))
                    {
                        if (sourceType.SubTypeName == "*")
                        {
                            // only type matches
                            this.MatchingTypeNamePartCount = 1;
                        }
                        else if (HttpUtils.CompareMediaTypeNames(sourceType.SubTypeName, targetType.SubTypeName))
                        {
                            // both type and subtype match
                            this.MatchingTypeNamePartCount = 2;
                        }
                    }
                }

                this.QualityValue = DefaultQualityValue;
                this.SourceTypeParameterCountForMatching = 0;
                this.MatchingParameterCount = 0;

                IList<KeyValuePair<string, string>> sourceParameters = sourceType.Parameters;
                IList<KeyValuePair<string, string>> targetParameters = targetType.Parameters;
                bool targetHasParams = targetParameters != null && targetParameters.Count > 0;
                bool sourceHasParams = sourceParameters != null && sourceParameters.Count > 0;

                if (sourceHasParams)
                {
                    for (int i = 0; i < sourceParameters.Count; ++i)
                    {
                        string parameterName = sourceParameters[i].Key;
                        if (IsQualityValueParameter(parameterName))
                        {
                            // once we hit the q-value in the parameters we know that only accept-params will follow
                            // that don't contribute to the matching. Parse the quality value but don't continue processing
                            // parameters.
                            this.QualityValue = ParseQualityValue(sourceParameters[i].Value.Trim());
                            break;
                        }

                        this.SourceTypeParameterCountForMatching = i + 1;

                        if (targetHasParams)
                        {
                            // find the current parameter name in the set of parameters of the candidate and compare the value;
                            // if they match increase the result count
                            // NOTE: according to RFC 2045, Section 2, parameter values are case sensitive per default (while
                            //       type values, subtype values and parameter names are case-insensitive); however, we
                            //       are more relaxed in ODL and allow case insensitive values.
                            string parameterValue;
                            if (TryFindMediaTypeParameter(targetParameters, parameterName, out parameterValue) &&
                                string.Compare(sourceParameters[i].Value.Trim(), parameterValue.Trim(), StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                this.MatchingParameterCount++;
                            }
                        }
                    }
                }

                // if the source does not have parameters or it only has accept extensions 
                // (parameters after the q value) or we match all the paramters we 
                // have a perfect parameter match.
                if (!sourceHasParams || 
                    this.SourceTypeParameterCountForMatching == 0 || 
                    this.MatchingParameterCount == this.SourceTypeParameterCountForMatching)
                {
                    this.MatchingParameterCount = -1;
                }
            }
        }
    }
}
