//---------------------------------------------------------------------
// <copyright file="MediaTypeUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
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
            ODataPayloadKind.ResourceSet,
            ODataPayloadKind.Resource,
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
            ODataPayloadKind.IndividualProperty,
            ODataPayloadKind.Delta,
            ODataPayloadKind.Asynchronous
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

        /// <summary>
        /// Max size to cache match info.
        /// </summary>
        private const int MatchInfoCacheMaxSize = 1024;

        /// <summary>
        /// Concurrent cache to cache match info.
        /// </summary>
        private static MatchInfoConcurrentCache MatchInfoCache = new MatchInfoConcurrentCache(MatchInfoCacheMaxSize);

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
                return encodingUtf8NoPreamble;
            }
        }

        /// <summary>Encoding to fall back to an appropriate encoding is not available.</summary>
        internal static Encoding FallbackEncoding
        {
            get
            {
                return MediaTypeUtils.EncodingUtf8NoPreamble;
            }
        }

        /// <summary>Encoding implied by an unspecified encoding value.</summary>
        /// <remarks>See http://tools.ietf.org/html/rfc2616#section-3.4.1 for details.</remarks>
        internal static Encoding MissingEncoding
        {
            get
            {
#if !ORCAS
                return Encoding.UTF8;
#else
                return Encoding.GetEncoding("ISO-8859-1", new EncoderExceptionFallback(), new DecoderExceptionFallback());
#endif
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
            ODataMediaTypeResolver mediaTypeResolver,
            out ODataMediaType mediaType,
            out Encoding encoding)
        {
            Debug.Assert(settings != null, "settings != null");

            // compute format, media type and encoding
            ODataFormat format;

            // get the supported and default media types for the specified payload kind
            IList<ODataMediaTypeFormat> supportedMediaTypes = mediaTypeResolver.GetMediaTypeFormats(payloadKind).ToList();
            if (supportedMediaTypes == null || supportedMediaTypes.Count == 0)
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
                IList<KeyValuePair<ODataMediaType, string>> specifiedTypes = HttpUtils.MediaTypesFromString(settings.AcceptableMediaTypes);

                // Starting in V3 we replace all occurrences of application/json with application/json;odata.metadata=minimal
                // before handing the acceptable media types to the conneg code. This is necessary because for an accept
                // header 'application/json, we want to the result to be 'application/json;odata.metadata=minimal'
                ConvertApplicationJsonInAcceptableMediaTypes(specifiedTypes, settings.Version ?? ODataVersion.V4);

                ODataMediaTypeFormat selectedMediaTypeWithFormat;
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
                        string supportedTypesAsString = String.Join(", ", supportedMediaTypes.Select(mt => mt.MediaType.ToText()).ToArray());
                        throw new ODataContentTypeException(Strings.MediaTypeUtils_DidNotFindMatchingMediaType(supportedTypesAsString, settings.AcceptableMediaTypes));
                    }

                    selectedMediaTypeWithFormat = supportedMediaTypes[matchInfo.TargetTypeIndex];
                    specifiedCharset = specifiedTypes[matchInfo.SourceTypeIndex].Value;
                }

                format = selectedMediaTypeWithFormat.Format;
                mediaType = selectedMediaTypeWithFormat.MediaType;

                // If any accept types in request used non-prefixed odata metadata or streaming type parameter,
                // use the non-prefixed version in the response
                if (specifiedTypes != null && mediaType.Parameters != null)
                {
                    if (specifiedTypes.Any(t => t.Key.Parameters != null && t.Key.Parameters.Any(p => String.Compare(p.Key, MimeConstants.MimeShortMetadataParameterName, StringComparison.OrdinalIgnoreCase) == 0)))
                    {
                        mediaType = new ODataMediaType(mediaType.Type, mediaType.SubType, mediaType.Parameters.Select(p => new KeyValuePair<string, string>(String.Compare(p.Key, MimeConstants.MimeMetadataParameterName, StringComparison.OrdinalIgnoreCase) == 0 ? MimeConstants.MimeShortMetadataParameterName : p.Key, p.Value)));
                    }

                    if (specifiedTypes.Any(t => t.Key.Parameters != null && t.Key.Parameters.Any(p => String.Compare(p.Key, MimeConstants.MimeShortStreamingParameterName, StringComparison.OrdinalIgnoreCase) == 0)))
                    {
                        mediaType = new ODataMediaType(mediaType.Type, mediaType.SubType, mediaType.Parameters.Select(p => new KeyValuePair<string, string>(String.Compare(p.Key, MimeConstants.MimeStreamingParameterName, StringComparison.OrdinalIgnoreCase) == 0 ? MimeConstants.MimeShortStreamingParameterName : p.Key, p.Value)));
                    }
                }

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
        /// Gets all payload kinds and their corresponding formats that match the specified content type header.
        /// </summary>
        /// <param name="contentTypeHeader">The content type header to get the payload kinds for.</param>
        /// <param name="mediaTypeResolver">The media type resolver to use when interpreting the content type.</param>
        /// <param name="contentType">The parsed content type as <see cref="ODataMediaType"/>.</param>
        /// <param name="encoding">The encoding from the content type or the default encoding from <see cref="ODataMediaType" />.</param>
        /// <returns>The list of payload kinds and formats supported for the specified <paramref name="contentTypeHeader"/>.</returns>
        internal static IList<ODataPayloadKindDetectionResult> GetPayloadKindsForContentType(string contentTypeHeader, ODataMediaTypeResolver mediaTypeResolver, out ODataMediaType contentType, out Encoding encoding)
        {
            Debug.Assert(!String.IsNullOrEmpty(contentTypeHeader), "Content-Type header must not be null or empty.");

            string charset;
            encoding = null;
            contentType = ParseContentType(contentTypeHeader, out charset);
            ODataMediaType[] targetTypes = new ODataMediaType[] { contentType };

            List<ODataPayloadKindDetectionResult> payloadKinds = new List<ODataPayloadKindDetectionResult>();

            IList<ODataMediaTypeFormat> mediaTypesForKind = null;
            for (int i = 0; i < allSupportedPayloadKinds.Length; ++i)
            {
                // get the supported and default media types for the current payload kind
                ODataPayloadKind payloadKind = allSupportedPayloadKinds[i];
                mediaTypesForKind = mediaTypeResolver.GetMediaTypeFormats(payloadKind).ToList();

                // match the specified media types against the supported/default ones
                // and get the format
                MediaTypeMatchInfo matchInfo = MatchMediaTypes(mediaTypesForKind.Select(smt => smt.MediaType), targetTypes);
                if (matchInfo != null)
                {
                    Debug.Assert(matchInfo.TargetTypeIndex == 0, "Invalid target type index detected.");
                    payloadKinds.Add(new ODataPayloadKindDetectionResult(payloadKind, mediaTypesForKind[matchInfo.SourceTypeIndex].Format));
                }
            }

            if (!String.IsNullOrEmpty(charset))
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
            ExceptionUtils.CheckArgumentNotNull(mediaType, "mediaType");
            ExceptionUtils.CheckArgumentNotNull(typeAndSubtype, "typeAndSubtype");

            return mediaType.StartsWith(typeAndSubtype, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks whether the specified media type has a parameter with the expected value.
        /// </summary>
        /// <param name="mediaType">The media type to check the parameters for.</param>
        /// <param name="parameterName">The name of the expected parameter.</param>
        /// <param name="parameterValue">The value of the expected parameter.</param>
        /// <returns>true if the <paramref name="mediaType"/> has a parameter called <paramref name="parameterName"/>
        /// with value <paramref name="parameterValue"/>; otherwise false.</returns>
        internal static bool MediaTypeHasParameterWithValue(this ODataMediaType mediaType, string parameterName, string parameterValue)
        {
            Debug.Assert(mediaType != null, "mediaType != null");
            Debug.Assert(parameterName != null, "parameterName != null");

            if (mediaType.Parameters == null)
            {
                return false;
            }

            return mediaType.Parameters.Any(p =>
                HttpUtils.CompareMediaTypeParameterNames(p.Key, parameterName) &&
                String.Compare(p.Value, parameterValue, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Determines whether the media type has a 'streaming' parameter with the value 'true'.
        /// </summary>
        /// <param name="mediaType">The media type to check.</param>
        /// <returns>
        ///   <c>true</c> if  the media type has a 'streaming' parameter with the value 'true'; otherwise, <c>false</c>.
        /// </returns>
        internal static bool HasStreamingSetToTrue(this ODataMediaType mediaType)
        {
            return mediaType.MediaTypeHasParameterWithValue(MimeConstants.MimeStreamingParameterName, MimeConstants.MimeParameterValueTrue);
        }

        /// <summary>
        /// Determines whether the media type has a 'IEEE754Compatible' parameter with the value 'true'.
        /// </summary>
        /// <param name="mediaType">The media type to check</param>
        /// <returns>
        ///   <c>true</c> if  the media type has a 'IEEE754Compatible' parameter with the value 'true'; otherwise, <c>false</c>.
        /// </returns>
        internal static bool HasIeee754CompatibleSetToTrue(this ODataMediaType mediaType)
        {
            return mediaType.MediaTypeHasParameterWithValue(MimeConstants.MimeIeee754CompatibleParameterName, MimeConstants.MimeParameterValueTrue);
        }

        /// <summary>
        /// Checks for wildcard characters in the <see cref="ODataMediaType"/>.
        /// </summary>
        /// <param name="mediaType">The <see cref="ODataMediaType"/> to check.</param>
        internal static void CheckMediaTypeForWildCards(ODataMediaType mediaType)
        {
            Debug.Assert(mediaType != null, "mediaType != null");

            if (HttpUtils.CompareMediaTypeNames(MimeConstants.MimeStar, mediaType.Type) ||
                HttpUtils.CompareMediaTypeNames(MimeConstants.MimeStar, mediaType.SubType))
            {
                throw new ODataContentTypeException(Strings.ODataMessageReader_WildcardInContentType(mediaType.FullTypeName));
            }
        }

        /// <summary>
        /// JSONP - instead of writing 'application/json', we write 'text/javascript'. In all other ways we pretend it is JSON
        /// </summary>
        /// <param name="contentType">Original content-type value string.</param>
        /// <returns>New content-type value string.</returns>
        internal static string AlterContentTypeForJsonPadding(string contentType)
        {
            if (contentType.StartsWith(MimeConstants.MimeApplicationJson, StringComparison.OrdinalIgnoreCase))
            {
                return contentType.Remove(0, MimeConstants.MimeApplicationJson.Length).Insert(0, MimeConstants.TextJavaScript);
            }

            if (contentType.StartsWith(MimeConstants.MimeTextPlain, StringComparison.OrdinalIgnoreCase))
            {
                return contentType.Remove(0, MimeConstants.MimeTextPlain.Length).Insert(0, MimeConstants.TextJavaScript);
            }

            throw new ODataException(Strings.ODataMessageWriter_JsonPaddingOnInvalidContentType(contentType));
        }

        /// <summary>
        /// Determine the <see cref="ODataFormat"/> to use for the given <paramref name="contentTypeName"/>. If no supported content type
        /// is found an exception is thrown.
        /// </summary>
        /// <param name="contentTypeName">The name of the content type to be checked.</param>
        /// <param name="supportedPayloadKinds">All possible kinds of payload that can be read with this content type.</param>
        /// <param name="mediaTypeResolver">The media type resolver to use when interpreting the content type.</param>
        /// <param name="mediaType">The media type parsed from the <paramref name="contentTypeName"/>.</param>
        /// <param name="encoding">The encoding from the content type or the default encoding for the <paramref name="mediaType" />.</param>
        /// <param name="selectedPayloadKind">
        /// The payload kind that was selected form the list of <paramref name="supportedPayloadKinds"/> for the
        /// specified <paramref name="contentTypeName"/>.
        /// </param>
        /// <returns>The <see cref="ODataFormat"/> for the <paramref name="contentTypeName"/>.</returns>
        internal static ODataFormat GetFormatFromContentType(string contentTypeName, ODataPayloadKind[] supportedPayloadKinds, ODataMediaTypeResolver mediaTypeResolver, out ODataMediaType mediaType, out Encoding encoding, out ODataPayloadKind selectedPayloadKind)
        {
            Debug.Assert(!supportedPayloadKinds.Contains(ODataPayloadKind.Unsupported), "!supportedPayloadKinds.Contains(ODataPayloadKind.Unsupported)");

            string charset;
            mediaType = ParseContentType(contentTypeName, out charset);

            IList<ODataMediaTypeFormat> supportedMediaTypes = null;
            for (int i = 0; i < supportedPayloadKinds.Length; ++i)
            {
                // get the supported and default media types for the current payload kind
                ODataPayloadKind supportedPayloadKind = supportedPayloadKinds[i];
                supportedMediaTypes = mediaTypeResolver.GetMediaTypeFormats(supportedPayloadKind).ToList();

                // match the specified media types against the supported/default ones
                // and get the format
                var cacheKey = new MatchInfoCacheKey(mediaTypeResolver, supportedPayloadKind, contentTypeName);
                MediaTypeMatchInfo matchInfo;

                if (!MatchInfoCache.TryGetValue(cacheKey, out matchInfo))
                {
                    matchInfo = MatchMediaTypes(supportedMediaTypes.Select(smt => smt.MediaType), new[] { mediaType });
                    MatchInfoCache.Add(cacheKey, matchInfo);
                }

                if (matchInfo != null)
                {
                    Debug.Assert(matchInfo.TargetTypeIndex == 0, "Invalid target type index detected.");
                    selectedPayloadKind = supportedPayloadKind;
                    encoding = GetEncoding(charset, selectedPayloadKind, mediaType, /*useDefaultEncoding*/ false);
                    return supportedMediaTypes[matchInfo.SourceTypeIndex].Format;
                }
            }

            // We're calling the ToArray here since not all platforms support the string.Join which takes IEnumerable.
            string supportedTypesAsString = String.Join(", ", supportedPayloadKinds.SelectMany(pk => mediaTypeResolver.GetMediaTypeFormats(pk).Select(mt => mt.MediaType.ToText())).ToArray());
            throw new ODataContentTypeException(Strings.MediaTypeUtils_CannotDetermineFormatFromContentType(supportedTypesAsString, contentTypeName));
        }

        /// <summary>
        /// Parses the specified content type header into a media type instance.
        /// </summary>
        /// <param name="contentTypeHeader">The content type header to parse.</param>
        /// <param name="charset">The optional charset specified with the content type.</param>
        /// <returns>The <see cref="ODataMediaType"/> of the parsed <paramref name="contentTypeHeader"/>.</returns>
        private static ODataMediaType ParseContentType(string contentTypeHeader, out string charset)
        {
            Debug.Assert(!String.IsNullOrEmpty(contentTypeHeader), "!string.IsNullOrEmpty(contentTypeHeader)");

            // parse the content type header into its parts, make sure we only allow one content type to be specified.
            IList<KeyValuePair<ODataMediaType, string>> specifiedTypes = HttpUtils.MediaTypesFromString(contentTypeHeader);
            if (specifiedTypes.Count != 1)
            {
                throw new ODataContentTypeException(Strings.MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified(contentTypeHeader));
            }

            ODataMediaType contentType = specifiedTypes[0].Key;
            CheckMediaTypeForWildCards(contentType);
            charset = specifiedTypes[0].Value;
            return contentType;
        }

        /// <summary>
        /// Gets the default media type for a given payload kind in a given format.
        /// </summary>
        /// <param name="supportedMediaTypes">A list of supported media types and formats.</param>
        /// <param name="specifiedFormat">The user-specified format in which to write the payload (can be null).</param>
        /// <param name="actualFormat">The default format for the specified payload kind</param>
        /// <returns>The default media type for the given payload kind and format.</returns>
        private static ODataMediaType GetDefaultMediaType(
            IList<ODataMediaTypeFormat> supportedMediaTypes,
            ODataFormat specifiedFormat,
            out ODataFormat actualFormat)
        {
            for (int i = 0; i < supportedMediaTypes.Count; ++i)
            {
                // NOTE: the supportedMediaTypes are sorted (DESC) by format and media type; so the
                //       default format and media type is the first resource in the list
                ODataMediaTypeFormat supportedMediaType = supportedMediaTypes[i];
                if (specifiedFormat == null || supportedMediaType.Format == specifiedFormat)
                {
                    actualFormat = supportedMediaType.Format;
                    return supportedMediaType.MediaType;
                }
            }

            throw new ODataException(Strings.ODataUtils_DidNotFindDefaultMediaType(specifiedFormat));
        }

        /// <summary>
        /// Parses the accepted char sets and matches them against the supported encodings for the given <paramref name="payloadKind"/>.
        /// </summary>
        /// <param name="acceptCharsetHeader">The Accept-Charset header of the request.</param>
        /// <param name="payloadKind">The <see cref="ODataPayloadKind"/> for which to compute the encoding.</param>
        /// <param name="mediaType">The media type used to compute the default encoding for the payload.</param>
        /// <param name="useDefaultEncoding">true if the default encoding should be returned if no acceptable charset is found; otherwise false.</param>
        /// <returns>The encoding to be used for the response.</returns>
        private static Encoding GetEncoding(string acceptCharsetHeader, ODataPayloadKind payloadKind, ODataMediaType mediaType, bool useDefaultEncoding)
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
        private static MediaTypeMatchInfo MatchMediaTypes(IEnumerable<ODataMediaType> sourceTypes, ODataMediaType[] targetTypes)
        {
            Debug.Assert(sourceTypes != null, "sourceTypes != null");
            Debug.Assert(targetTypes != null, "targetTypes != null");

            MediaTypeMatchInfo selectedMatchInfo = null;

            int sourceIndex = 0;
            if (sourceTypes != null)
            {
                foreach (ODataMediaType sourceType in sourceTypes)
                {
                    int targetIndex = 0;
                    foreach (ODataMediaType targetType in targetTypes)
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
        /// Converts all occurrences of the 'application/json' media type to 'application/json;odata.metadata=minimal'.
        /// This is necessary because for an accept header 'application/json
        /// we want the result to be 'application/json;odata.metadata=minimal'
        /// </summary>
        /// <param name="specifiedTypes">The parsed acceptable media types.</param>
        /// <param name="version">The ODataVersion for which to convert the 'application/json' media type</param>
        private static void ConvertApplicationJsonInAcceptableMediaTypes(IList<KeyValuePair<ODataMediaType, string>> specifiedTypes, ODataVersion version)
        {
            if (specifiedTypes == null)
            {
                return;
            }

            for (int i = 0; i < specifiedTypes.Count; ++i)
            {
                ODataMediaType mediaType = specifiedTypes[i].Key;
                if (HttpUtils.CompareMediaTypeNames(mediaType.SubType, MimeConstants.MimeJsonSubType) &&
                    HttpUtils.CompareMediaTypeNames(mediaType.Type, MimeConstants.MimeApplicationType))
                {
                    if (mediaType.Parameters == null || !mediaType.Parameters.Any(p => HttpUtils.IsMetadataParameter(p.Key)))
                    {
                        // application/json detected; convert it to Json Light
                        IList<KeyValuePair<string, string>> existingParams = mediaType.Parameters != null ? mediaType.Parameters.ToList() : null;
                        int newCount = existingParams == null ? 1 : existingParams.Count + 1;
                        List<KeyValuePair<string, string>> newParams = new List<KeyValuePair<string, string>>(newCount);
                        newParams.Add(new KeyValuePair<string, string>(
                            version < ODataVersion.V401 ? MimeConstants.MimeMetadataParameterName : MimeConstants.MimeShortMetadataParameterName,
                            MimeConstants.MimeMetadataParameterValueMinimal));
                        if (existingParams != null)
                        {
                            newParams.AddRange(existingParams);
                        }

                        specifiedTypes[i] = new KeyValuePair<ODataMediaType, string>(new ODataMediaType(mediaType.Type, mediaType.SubType, newParams), specifiedTypes[i].Value);
                    }
                }
            }
        }

        /// <summary>
        /// Class representing the result of matching two <see cref="ODataMediaType"/> instances.
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
            /// <param name="sourceType">The source <see cref="ODataMediaType"/> to match against the target type.</param>
            /// <param name="targetType">The target <see cref="ODataMediaType"/> to match against the source type.</param>
            /// <param name="sourceIndex">Index of the source type in the list of all source types.</param>
            /// <param name="targetIndex">Index of the target type in the list of all target types.</param>
            public MediaTypeMatchInfo(ODataMediaType sourceType, ODataMediaType targetType, int sourceIndex, int targetIndex)
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
                Debug.Assert(!String.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

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
            /// <param name="sourceType">The source <see cref="ODataMediaType"/> to match against the target type.</param>
            /// <param name="targetType">The target <see cref="ODataMediaType"/> to match against the source type.</param>
            private void MatchTypes(ODataMediaType sourceType, ODataMediaType targetType)
            {
                this.MatchingTypeNamePartCount = -1;

                if (sourceType.Type == "*")
                {
                    this.MatchingTypeNamePartCount = 0;
                }
                else
                {
                    if (HttpUtils.CompareMediaTypeNames(sourceType.Type, targetType.Type))
                    {
                        if (sourceType.SubType == "*")
                        {
                            // only type matches
                            this.MatchingTypeNamePartCount = 1;
                        }
                        else if (HttpUtils.CompareMediaTypeNames(sourceType.SubType, targetType.SubType))
                        {
                            // both type and subtype match
                            this.MatchingTypeNamePartCount = 2;
                        }
                    }
                }

                this.QualityValue = DefaultQualityValue;
                this.SourceTypeParameterCountForMatching = 0;
                this.MatchingParameterCount = 0;

                IList<KeyValuePair<string, string>> sourceParameters = sourceType.Parameters != null ? sourceType.Parameters.ToList() : null;
                IList<KeyValuePair<string, string>> targetParameters = targetType.Parameters != null ? targetType.Parameters.ToList() : null;
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
                                String.Compare(sourceParameters[i].Value.Trim(), parameterValue.Trim(), StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                this.MatchingParameterCount++;
                            }
                        }
                    }
                }

                // if the source does not have parameters or it only has accept extensions
                // (parameters after the q value) or we match all the parameters we
                // have a perfect parameter match.
                if (!sourceHasParams ||
                    this.SourceTypeParameterCountForMatching == 0 ||
                    this.MatchingParameterCount == this.SourceTypeParameterCountForMatching)
                {
                    this.MatchingParameterCount = -1;
                }
            }
        }

        /// <summary>
        /// Class representing key of match info cache.
        /// </summary>
        private sealed class MatchInfoCacheKey
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="resolver">The media type resolver to use.</param>
            /// <param name="payloadKind">Kind of the payload.</param>
            /// <param name="contentTypeName">Name of content type.</param>
            public MatchInfoCacheKey(ODataMediaTypeResolver resolver, ODataPayloadKind payloadKind, string contentTypeName)
            {
                this.MediaTypeResolver = resolver;
                this.PayloadKind = payloadKind;
                this.ContentTypeName = contentTypeName;
            }

            /// <summary>
            /// Type of the mediaTypeResolver.
            /// </summary>
            private ODataMediaTypeResolver MediaTypeResolver { get; set; }

            /// <summary>
            /// Kind of the payload.
            /// </summary>
            private ODataPayloadKind PayloadKind { get; set; }

            /// <summary>
            /// Name of content type.
            /// </summary>
            private string ContentTypeName { get; set; }

            /// <summary>
            /// Returns a value indicating whether this instance is equal to a specified object.
            /// </summary>
            /// <param name="obj">An object to compare with this instance.</param>
            /// <returns>true if obj is equal to this instance; otherwise, false.</returns>
            public override bool Equals(object obj)
            {
                MatchInfoCacheKey cacheKey = obj as MatchInfoCacheKey;
                return cacheKey != null &&
                    this.MediaTypeResolver == cacheKey.MediaTypeResolver &&
                    this.PayloadKind == cacheKey.PayloadKind &&
                    this.ContentTypeName == cacheKey.ContentTypeName;
            }

            /// <summary>
            /// Returns the hash code for the value of this instance.
            /// </summary>
            /// <returns>A 32-bit signed integer hash code.</returns>
            public override int GetHashCode()
            {
                int result = this.MediaTypeResolver.GetHashCode() ^ this.PayloadKind.GetHashCode();
                return this.ContentTypeName != null ? result ^ this.ContentTypeName.GetHashCode() : result;
            }
        }

        /// <summary>
        /// Class representing the concurrent cache for match info.
        /// </summary>
        private sealed class MatchInfoConcurrentCache
        {
            /// <summary>
            /// Max size of the elements that the cache can contain.
            /// </summary>
            private readonly int maxSize;

            /// <summary>
            /// The dictionary to save elements.
            /// </summary>
            private IDictionary<MatchInfoCacheKey, MediaTypeMatchInfo> dict;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="maxSize">Max size of the elements that the cache can contain.</param>
            public MatchInfoConcurrentCache(int maxSize)
            {
                this.maxSize = maxSize;
                this.dict = new Dictionary<MatchInfoCacheKey, MediaTypeMatchInfo>(maxSize + 1);
            }

            /// <summary>
            /// Gets the value associated with the specified key.
            /// </summary>
            /// <param name="key">The key whose value to get.</param>
            /// <param name="value">The value associated with the specified key, if the key is found; otherwise, null.</param>
            /// <returns>true if the cache contains an element with the specified key; otherwise, false.</returns>
            public bool TryGetValue(MatchInfoCacheKey key, out MediaTypeMatchInfo value)
            {
                lock (this.dict)
                {
                    return dict.TryGetValue(key, out value);
                }
            }

            /// <summary>
            /// Adds an element with the provided key and value to the cache.
            /// </summary>
            /// <param name="key">The key of the element to add.</param>
            /// <param name="value">The value of the element to add.</param>
            public void Add(MatchInfoCacheKey key, MediaTypeMatchInfo value)
            {
                lock (this.dict)
                {
                    if (!dict.ContainsKey(key))
                    {
                        if (dict.Count == maxSize)
                        {
                            dict.Clear();
                        }

                        dict.Add(key, value);
                    }
                }
            }
        }
    }
}
