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
        #region Default media types per payload kind
        /// <summary>
        /// An array that maps stores the supported media types for all <see cref="ODataPayloadKind"/> .
        /// </summary>
        /// <remarks>
        /// The set of supported media types is ordered (desc) by their precedence/priority with respect to (1) format and (2) media type.
        /// As a result the default media type for a given payloadKind is the first entry in the MediaTypeWithFormat array.
        /// </remarks>
        private static readonly MediaTypeWithFormat[][] mediaTypesForPayloadKind =
            new MediaTypeWithFormat[13][]
            {
                // feed
                new MediaTypeWithFormat[] 
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeAtomXmlSubType, new KeyValuePair<string, string>(MimeConstants.MimeTypeParameterName, MimeConstants.MimeTypeParameterValueFeed)) },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeAtomXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType) }
                },

                // entry
                new MediaTypeWithFormat[] 
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeAtomXmlSubType, new KeyValuePair<string, string>(MimeConstants.MimeTypeParameterName, MimeConstants.MimeTypeParameterValueEntry)) },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeAtomXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType) }
                },

                // property
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeTextType, MimeConstants.MimeXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType) }
                },

                // entity reference link
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeTextType, MimeConstants.MimeXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType) }
                },

                // entity reference links
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeTextType, MimeConstants.MimeXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType) }
                },

                // value
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Default, MediaType = new MediaType(MimeConstants.MimeTextType, MimeConstants.MimePlainSubType) },
                },

                // binary
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Default, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeOctetStreamSubType) },
                },

                // collection
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeTextType, MimeConstants.MimeXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType) }
                },

                // service document
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeAtomSvcXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType) }
                },

                // metadata document
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Default, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType) },
                },

                // error
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType) }
                },

                // batch
                new MediaTypeWithFormat[]
                { 
                    // Note that as per spec the multipart/mixed must have a boundary parameter which is not specified here. We will add that parameter
                    // when using this mime type because we need to generate a new boundary every time.
                    new MediaTypeWithFormat { Format = ODataFormat.Default, MediaType = new MediaType(MimeConstants.MimeMultipartType, MimeConstants.MimeMixedSubType) },
                },

                // parameter
                new MediaTypeWithFormat[]
                {
                    // We will only support parameters in Json format for now.
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType) }
                },
            };
        #endregion Default media types per payload kind

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
        /// <param name="mimeTypeForRawValue">
        /// The MIME type to be used for writing the content of the message. 
        /// Note that this is only supported for top-level raw values.
        /// </param>
        /// <param name="mediaType">The media type to be used in the response message.</param>
        /// <param name="encoding">The encoding to be used in the response message.</param>
        /// <returns>The <see cref="ODataFormat"/> used when serializing the response.</returns>
        internal static ODataFormat GetContentTypeFromSettings(
            ODataMessageWriterSettings settings,
            ODataPayloadKind payloadKind,
            string mimeTypeForRawValue,
            out MediaType mediaType,
            out Encoding encoding)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(settings != null, "settings != null");
            Debug.Assert(mimeTypeForRawValue == null || payloadKind == ODataPayloadKind.Value, "Explicit MIME type can only be set for top-level values.");

            // compute format, media type and encoding
            ODataFormat format;

            // get the supported and default media types for the specified payload kind
            MediaTypeWithFormat[] supportedMediaTypes = null;
            if (mimeTypeForRawValue == null)
            {
                supportedMediaTypes = mediaTypesForPayloadKind[(int)payloadKind];
            }
            else
            {
                IList<MediaType> rawValueMediaTypes = HttpUtils.ReadMediaTypes(mimeTypeForRawValue);
                supportedMediaTypes = rawValueMediaTypes.Select(mt => new MediaTypeWithFormat { Format = ODataFormat.Default, MediaType = mt }).ToArray();
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
                IList<MediaType> specifiedTypes = HttpUtils.MediaTypesFromString(settings.AcceptableMediaTypes);

                MediaTypeWithFormat selectedMediaTypeWithFormat;
                if (specifiedTypes == null || specifiedTypes.Count == 0)
                {
                    if (supportedMediaTypes == null || supportedMediaTypes.Length == 0)
                    {
                        throw new ODataContentTypeException(Strings.MediaTypeUtils_DidNotFindMatchingMediaType(null, settings.AcceptableMediaTypes));
                    }

                    selectedMediaTypeWithFormat = supportedMediaTypes[0];
                }
                else
                {
                    // match the specified media types against the supported/default ones and get the format
                    MediaTypeMatchInfo matchInfo = MatchMediaTypes(specifiedTypes, supportedMediaTypes.Select(smt => smt.MediaType));
                    if (matchInfo == null)
                    {
                        // We're calling the ToArray here since not all platforms support the string.Join which takes IEnumerable.
                        string supportedTypesAsString = string.Join(", ", supportedMediaTypes.Select(mt => mt.MediaType.ToText()).ToArray());
                        throw new ODataContentTypeException(Strings.MediaTypeUtils_DidNotFindMatchingMediaType(supportedTypesAsString, settings.AcceptableMediaTypes));
                    }

                    selectedMediaTypeWithFormat = supportedMediaTypes[matchInfo.TargetTypeIndex];
                }

                format = selectedMediaTypeWithFormat.Format;
                mediaType = selectedMediaTypeWithFormat.MediaType;

                encoding = GetEncoding(settings.AcceptableCharsets, payloadKind, mediaType);
            }

            return format;
        }

        /// <summary>
        /// Determine the <see cref="ODataFormat"/> to use for the given <paramref name="contentTypeHeader"/>. If no supported content type
        /// is found an exception is thrown.
        /// </summary>
        /// <param name="contentTypeHeader">The name of the content type to be checked.</param>
        /// <param name="supportedPayloadKinds">All possiblel kinds of payload that can be read with this content type.</param>
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
            out MediaType mediaType,
            out Encoding encoding,
            out ODataPayloadKind selectedPayloadKind,
            out string batchBoundary)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!supportedPayloadKinds.Contains(ODataPayloadKind.Unsupported), "!supportedPayloadKinds.Contains(ODataPayloadKind.Unsupported)");

            ODataFormat format = GetFormatFromContentType(contentTypeHeader, supportedPayloadKinds, out mediaType, out encoding, out selectedPayloadKind);

            // for batch payloads, read the batch boundary from the content type header; this is the only
            // content type parameter we support (and that is required for batch payloads)
            if (selectedPayloadKind == ODataPayloadKind.Batch)
            {
                KeyValuePair<string, string> boundaryPair = default(KeyValuePair<string, string>);
                IEnumerable<KeyValuePair<string, string>> parameters = mediaType.Parameters;
                if (parameters != null)
                {
                    boundaryPair = parameters.Where(p => p.Key == HttpConstants.HttpMultipartBoundary).SingleOrDefault();
                }

                if (string.CompareOrdinal(boundaryPair.Key, HttpConstants.HttpMultipartBoundary) != 0)
                {
                    throw new ODataException(Strings.ODataMediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads(contentTypeHeader, HttpConstants.HttpMultipartBoundary));
                }

                batchBoundary = boundaryPair.Value;
            }
            else
            {
                batchBoundary = null;
            }

            return format;
        }

        /// <summary>
        /// Determine the <see cref="ODataFormat"/> to use for the given <paramref name="contentTypeName"/>. If no supported content type
        /// is found an exception is thrown.
        /// </summary>
        /// <param name="contentTypeName">The name of the content type to be checked.</param>
        /// <param name="supportedPayloadKinds">All possiblel kinds of payload that can be read with this content type.</param>
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
            out MediaType mediaType, 
            out Encoding encoding,
            out ODataPayloadKind selectedPayloadKind)
        {
            Debug.Assert(!supportedPayloadKinds.Contains(ODataPayloadKind.Unsupported), "!supportedPayloadKinds.Contains(ODataPayloadKind.Unsupported)");

            // parse the content type into its parts, make sure we only allow one content type to be specified.
            IList<MediaType> specifiedTypes = HttpUtils.MediaTypesFromString(contentTypeName);
            if (specifiedTypes.Count != 1)
            {
                throw new ODataException(Strings.MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified(contentTypeName));
            }

            MediaType contentType = specifiedTypes[0];
            if (string.CompareOrdinal(MimeConstants.MimeStar, contentType.TypeName) == 0 ||
                string.CompareOrdinal(MimeConstants.MimeStar, contentType.SubTypeName) == 0)
            {
                throw new ODataException(Strings.ODataMessageReader_WildcardInContentType(contentTypeName));
            }

            MediaTypeWithFormat[] supportedMediaTypes = null;
            for (int i = 0; i < supportedPayloadKinds.Length; ++i)
            {
                // get the supported and default media types for the current payload kind
                ODataPayloadKind supportedPayloadKind = supportedPayloadKinds[i];
                supportedMediaTypes = mediaTypesForPayloadKind[(int)supportedPayloadKind];

                // match the specified media types against the supported/default ones
                // and get the format
                MediaTypeMatchInfo matchInfo = MatchMediaTypes(supportedMediaTypes.Select(smt => smt.MediaType), specifiedTypes);
                if (matchInfo != null)
                {
                    selectedPayloadKind = supportedPayloadKind;
                    mediaType = specifiedTypes[matchInfo.TargetTypeIndex];
                    encoding = mediaType.SelectEncoding();
                    return supportedMediaTypes[matchInfo.SourceTypeIndex].Format;
                }
            }

            // Special case for reading raw values or when determining the format. In both cases reading a raw value is 
            // allowed. For raw values, however, we support custom content types; if those types don't match the content types
            // supported for 'value' or 'binary value' payloads per default, we still treat them as binary values.
            if (supportedPayloadKinds.Length > 1)
            {
                Debug.Assert(supportedPayloadKinds.Contains(ODataPayloadKind.Value), "If more than one payload kind is supported 'ODataPayloadKind.Value' must be included.");
                selectedPayloadKind = ODataPayloadKind.BinaryValue;
                mediaType = specifiedTypes[0];
                encoding = mediaType.SelectEncoding();
                return ODataFormat.Default;
            }

            // We're calling the ToArray here since not all platforms support the string.Join which takes IEnumerable.
            Debug.Assert(supportedMediaTypes != null, "supportedMediaTypes != null");
            string supportedTypesAsString = string.Join(", ", supportedMediaTypes.Select(mt => mt.MediaType.ToText()).ToArray());
            throw new ODataContentTypeException(Strings.MediaTypeUtils_CannotDetermineFormatFromContentType(supportedTypesAsString, contentTypeName));
        }

        /// <summary>
        /// Gets the default media type for a given payload kind in a given format.
        /// </summary>
        /// <param name="supportedMediaTypes">An array of supported media types and formats.</param>
        /// <param name="specifiedFormat">The format in which to write the payload (can be ODataFormat.Default).</param>
        /// <param name="actualFormat">The default format for the specified payload kind (is never ODataFormat.Default)</param>
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
                if (specifiedFormat == ODataFormat.Default || supportedMediaType.Format == specifiedFormat)
                {
                    actualFormat = supportedMediaType.Format;
                    return supportedMediaType.MediaType;
                }
            }

            throw new ODataException(Strings.ODataUtils_DidNotFindDefaultMediaType(specifiedFormat.ToString()));
        }

        /// <summary>
        /// Parses the accepted charsets and matches them against the supported encodings for the given <paramref name="payloadKind"/>.
        /// </summary>
        /// <param name="acceptCharsetHeader">The Accept-Charset header of the request.</param>
        /// <param name="payloadKind">The <see cref="ODataPayloadKind"/> for which to compute the encoding.</param>
        /// <param name="mediaType">The media type used to compute the default encoding for the payload.</param>
        /// <returns>The encoding to be used for the response.</returns>
        private static Encoding GetEncoding(string acceptCharsetHeader, ODataPayloadKind payloadKind, MediaType mediaType)
        {
            if (payloadKind == ODataPayloadKind.BinaryValue)
            {
                return null;
            }

            // TODO: we currently assume that the media type does not have a 'charset' parameter specified. This is true for 
            //       the current uses of this method. Consider making this general purpose and also include the 'charset'
            //       parameter on the media type into the resolution process. (Currently the media type is only used to
            //       compute the default encoding).
            return HttpUtils.EncodingFromAcceptableCharsets(acceptCharsetHeader, mediaType, encodingUtf8NoPreamble, encodingUtf8NoPreamble);
        }

        /// <summary>
        /// Matches the supported media types against the list of media types specified in the Accept header or ContentType header of the message. Matching follows the
        /// rules for media type matching as described in RFC 2616.
        /// </summary>
        /// <param name="sourceTypes">The set of media types to be matched against the <paramref name="targetTypes"/>.</param>
        /// <param name="targetTypes">The set of media types the <paramref name="sourceTypes"/> will be matched against.</param>
        /// <returns>The best <see cref="MediaTypeMatchInfo"/> found during the matching process or null if no match was found.</returns>
        private static MediaTypeMatchInfo MatchMediaTypes(IEnumerable<MediaType> sourceTypes, IEnumerable<MediaType> targetTypes)
        {
            Debug.Assert(sourceTypes != null, "sourceTypes != null");
            Debug.Assert(targetTypes != null, "targetTypes != null");

            // cache the target types into an array since they are usually backed by a Linq expression
            // and we enumerate it multiple times (and want to avoid extra allocations)
            MediaType[] targetTypesArray = targetTypes.ToArray();

            MediaTypeMatchInfo selectedMatchInfo = null;

            int sourceIndex = 0;
            if (sourceTypes != null)
            {
                foreach (MediaType sourceType in sourceTypes)
                {
                    int targetIndex = 0;
                    foreach (MediaType targetType in targetTypesArray)
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
                                // the selected match is less specific than the current match
                                selectedMatchInfo = currentMatchInfo;
                            }
                        }

                        targetIndex ++;
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
        /// A private helper class to associate a <see cref="ODataFormat"/> with a media type.
        /// </summary>
        private sealed class MediaTypeWithFormat
        {
            /// <summary>The media type.</summary>
            public MediaType MediaType
            {
                get;
                set;
            }

            /// <summary>
            /// The <see cref="ODataFormat"/> for this media type.
            /// </summary>
            public ODataFormat Format
            {
                get;
                set;
            }
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
            /// true if this match info represents an exact match, i.e. both type name parts match
            /// and all parameters match as well.
            /// </summary>
            private bool IsExactMatch
            {
                get; 
                set;
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
                    // if one of the matches is exact choose it; if both are exact rely on the q value
                    bool bothAreExactMatches = this.IsExactMatch && other.IsExactMatch;
                    if (!bothAreExactMatches)
                    {
                        if (this.IsExactMatch)
                        {
                            return 1;
                        }

                        if (other.IsExactMatch)
                        {
                            return -1;
                        }
                    }

                    // Now check the parameters and see whether we find a more specific match.
                    if (this.MatchingParameterCount > other.MatchingParameterCount)
                    {
                        return 1;
                    }

                    if (this.MatchingParameterCount == other.MatchingParameterCount)
                    {
                        // check the quality value
                        return this.QualityValue.CompareTo(other.QualityValue);
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

                        if (string.Compare(parameterName, candidateParameterName, StringComparison.OrdinalIgnoreCase) == 0)
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
                return string.Compare(parameterName, HttpConstants.HttpQValueParameter, StringComparison.OrdinalIgnoreCase) == 0;
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
                            //       type values, subtype values and parameter names are case-insensitive).
                            string parameterValue;
                            if (TryFindMediaTypeParameter(targetParameters, parameterName, out parameterValue) &&
                                string.Compare(sourceParameters[i].Value.Trim(), parameterValue.Trim(), StringComparison.Ordinal) == 0)
                            {
                                this.MatchingParameterCount++;
                            }
                        }
                    }
                }

                // if we only have accept extensions (parameters after the q value) special rules apply
                if (!sourceHasParams || this.SourceTypeParameterCountForMatching == 0)
                {
                    if (targetHasParams)
                    {
                        if (this.MatchingTypeNamePartCount == 0 || this.MatchingTypeNamePartCount == 1)
                        {
                            // this is a media range specification using the '*' wildcard.
                            // we assume that all parameters are matched for such ranges
                            this.MatchingParameterCount = targetParameters.Count;
                            this.SourceTypeParameterCountForMatching = targetParameters.Count;
                        }
                        else
                        {
                            // the target type has parameters while this type has not;
                            // return 0 to indicate that none of the target type's parameters match.
                            this.MatchingParameterCount = 0;
                        }
                    }
                    else
                    {
                        // neither the source type nor the target type have parameters; in this case we return -1 to indicate
                        // that the candidate parameters are a perfect.
                        this.MatchingParameterCount = -1;
                    }
                }

                // the match is exact if both type name parts are matched
                // and all the parameters are matches as well.
                int targetParamsCount = targetHasParams ? targetParameters.Count : 0;
                this.IsExactMatch = this.MatchingTypeNamePartCount == 2 && (this.MatchingParameterCount == -1 || this.MatchingParameterCount == targetParamsCount);
            }
        }
    }
}
