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

namespace System.Data.OData
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
            new MediaTypeWithFormat[11][]
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

                // link
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
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType) }
                },

                // metadata document
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeAtomXmlSubType) },
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

        /// <summary>
        /// Enumeration to indicate the precedence of a media type relative to another.
        /// </summary>
        private enum PrecedenceLevel
        {
            /// <summary>Same precedence.</summary>
            Same = 0,

            /// <summary>Higher precedence.</summary>
            Higher = 1,

            /// <summary>Lower precedence.</summary>
            Lower = 2
        }

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
        /// <param name="settings">The writer settings to use for serializing the response payload.</param>
        /// <param name="payloadKind">The kind of payload to be serialized as part of the response message.</param>
        /// <param name="mediaType">The media type to be used in the response message.</param>
        /// <param name="encoding">The encoding to be used in the response message.</param>
        /// <returns>The <see cref="ODataFormat"/> used when serializing the response.</returns>
        internal static ODataFormat GetContentTypeFromSettings(
            ODataWriterSettings settings,
            ODataPayloadKind payloadKind,
            out MediaType mediaType,
            out Encoding encoding)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(settings != null, "settings != null");

            // compute format, media type and encoding
            ODataFormat format;

            if (settings.AcceptableMediaTypes != null)
            {
                mediaType = GetMediaType(settings.AcceptableMediaTypes, payloadKind, out format);
                encoding = GetEncoding(settings.AcceptableCharsets, payloadKind, mediaType);
            }
            else
            {
                mediaType = GetDefaultMediaType(payloadKind, settings.Format, out format);
                encoding = mediaType.SelectEncoding();
            }

            return format;
        }

        /// <summary>
        /// Gets the default media type for a given payload kind in a given format.
        /// </summary>
        /// <param name="payloadKind">The kind of payload to write.</param>
        /// <param name="specifiedFormat">The format in which to write the payload (can be ODataFormat.Default).</param>
        /// <param name="actualFormat">The default format for the specified payload kind (is never ODataFormat.Default)</param>
        /// <returns>The default media type for the given payload kind and format.</returns>
        private static MediaType GetDefaultMediaType(ODataPayloadKind payloadKind, ODataFormat specifiedFormat, out ODataFormat actualFormat)
        {
            // get the supported and default media types for the specified payload kind
            MediaTypeWithFormat[] supportedMediaTypes = mediaTypesForPayloadKind[(int)payloadKind];

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
        /// Parses the Accept header of the writer settings and matches the acceptable media types against the supported
        /// media types for the specified <see cref="ODataPayloadKind"/>.
        /// </summary>
        /// <param name="acceptHeader">The string value of the Accept header.</param>
        /// <param name="payloadKind">The payload kind for which to compute the media type.</param>
        /// <param name="format">The <see cref="ODataFormat"/> that we need to use when serializing the response.</param>
        /// <returns>The media type to use for the response message.</returns>
        private static MediaType GetMediaType(string acceptHeader, ODataPayloadKind payloadKind, out ODataFormat format)
        {
            // parse the accept header into its parts
            IList<MediaType> acceptableTypes = HttpUtils.MediaTypesFromAcceptHeader(acceptHeader);

            // get the supported and default media types for the specified payload kind
            MediaTypeWithFormat[] supportedMediaTypes = mediaTypesForPayloadKind[(int)payloadKind];

            // match the acceptable media types against the supported/default ones
            // and get the format
            MediaType mediaType = MatchMediaTypes(acceptableTypes, supportedMediaTypes, out format);
            if (mediaType == null)
            {
                // We're calling the ToArray here since not all platforms support the string.Join which takes IEnumerable.
                string supportedTypesAsString = string.Join(", ", supportedMediaTypes.Select(mt => mt.MediaType.ToText()).ToArray());
                throw new ODataException(Strings.ODataUtils_DidNotFindMatchingMediaType(supportedTypesAsString, acceptHeader));
            }

            return mediaType;
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
            else
            {
                // TODO: we currently assume that the media type does not have a 'charset' parameter specified. This is true for 
                //       the current uses of this method. Consider making this general purpose and also include the 'charset'
                //       parameter on the media type into the resolution process. (Currently the media type is only used to
                //       compute the default encoding).
                return HttpUtils.EncodingFromAcceptableCharsets(acceptCharsetHeader, mediaType, encodingUtf8NoPreamble, encodingUtf8NoPreamble);
            }
        }

        /// <summary>
        /// Matches the supported media types against the list of acceptable media types specified in the Accept header of the request. Matching follows the
        /// rules for media type matching as described in RFC 2616.
        /// </summary>
        /// <param name="acceptableTypes">The set of acceptable media types specified in the Accept header of the request.</param>
        /// <param name="supportedMediaTypes">The set of supported media types (sorted in descending precedence order).</param>
        /// <param name="format">The <see cref="ODataFormat"/> for the selected media type.</param>
        /// <returns>The media type that best matches the acceptable media types.</returns>
        private static MediaType MatchMediaTypes(IList<MediaType> acceptableTypes, MediaTypeWithFormat[] supportedMediaTypes, out ODataFormat format)
        {
            Debug.Assert(supportedMediaTypes != null, "supportedMediaTypes != null");

            MediaTypeWithFormat selectedContentType = null;
            int selectedMatchingTypeNameParts = -1;
            int selectedMatchingParameters = -2;        // -2 is an invalid value; GetMatchingParameters returns values >= -1
            int selectedQualityValue = 0;
            int selectedPreferenceIndex = Int32.MaxValue;
            bool selectedAcceptable = false;

            if (acceptableTypes != null)
            {
                foreach (MediaType acceptableType in acceptableTypes)
                {
                    for (int i = 0; i < supportedMediaTypes.Length; i++)
                    {
                        MediaTypeWithFormat supportedType = supportedMediaTypes[i];

                        // match the type name parts and parameters of the media type
                        int matchingParameters, matchingQualityValue, acceptableTypeParameterCount;
                        int matchingTypeNameParts = acceptableType.GetMatchingParts(supportedType.MediaType, out matchingParameters, out matchingQualityValue, out acceptableTypeParameterCount);

                        if (!IsValidCandidate(matchingTypeNameParts, matchingParameters, acceptableTypeParameterCount))
                        {
                            continue;
                        }

                        // only continue processing the type if it is acceptable or we don't have
                        // an acceptable type yet.
                        bool matchingAcceptable = matchingQualityValue != 0;
                        if (!matchingAcceptable && selectedAcceptable)
                        {
                            continue;
                        }

                        PrecedenceLevel precedence = ComputePrecedence(selectedAcceptable, matchingTypeNameParts, selectedMatchingTypeNameParts, matchingParameters, selectedMatchingParameters);
                        switch (precedence)
                        {
                            case PrecedenceLevel.Higher:
                                {
                                    // A more specific type wins.
                                    selectedContentType = supportedType;
                                    selectedMatchingTypeNameParts = matchingTypeNameParts;
                                    selectedMatchingParameters = matchingParameters;
                                    selectedQualityValue = matchingQualityValue;
                                    selectedPreferenceIndex = i;
                                    selectedAcceptable = matchingAcceptable;
                                }

                                break;
                            case PrecedenceLevel.Same:
                                {
                                    // A type with a higher q-value wins.
                                    if (matchingQualityValue > selectedQualityValue)
                                    {
                                        selectedContentType = supportedType;
                                        selectedQualityValue = matchingQualityValue;
                                        selectedPreferenceIndex = i;
                                        selectedAcceptable = selectedQualityValue != 0;
                                    }
                                    else if (matchingQualityValue == selectedQualityValue)
                                    {
                                        // A type that is earlier in the supportedMediaTypes array wins.
                                        if (i < selectedPreferenceIndex)
                                        {
                                            selectedContentType = supportedType;
                                            selectedPreferenceIndex = i;
                                        }
                                    }
                                }

                                break;
                            case PrecedenceLevel.Lower:
                                continue;
                            default:
                                throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataUtils_MatchMediaTypes_UnreachableCodePath));
                        }
                    }
                }
            }

            if (acceptableTypes == null || acceptableTypes.Count == 0)
            {
                // if not acceptable types are specified it means that all types are supported;
                // take the first supported type (as they are oredered by priority/precedence)
                selectedContentType = supportedMediaTypes[0];
            }
            else if (!selectedAcceptable)
            {
                format = ODataFormat.Default;
                return null;
            }

            format = selectedContentType.Format;
            return selectedContentType.MediaType;
        }

        /// <summary>
        /// Determines whether the results of matching two media types describe the types to be compatible or not.
        /// </summary>
        /// <param name="matchingTypeNameParts">The number of matching type name parts.</param>
        /// <param name="matchingParameters">The number of matching parameters.</param>
        /// <param name="acceptableTypeParameterCount">The number of parameters in the acceptable type.</param>
        /// <returns>True if the types are compatible; otherwise false.</returns>
        /// <remarks>
        /// Two types are considered compatible if at least one type name part matches (or we are dealing with a wildcard)
        /// and all the parameters in the acceptable type have been matched.
        /// </remarks>
        private static bool IsValidCandidate(int matchingTypeNameParts, int matchingParameters, int acceptableTypeParameterCount)
        {
            if (matchingTypeNameParts < 0)
            {
                // if none of the type name parts match the types are not compatible; continue with next acceptable type.
                return false;
            }
            else if (matchingTypeNameParts > 1)
            {
                // make sure we matched all the parameters in the acceptable type
                if (matchingParameters != -1 && matchingParameters < acceptableTypeParameterCount)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Computes whether matching two media types produced a better match than the previously selected one.
        /// </summary>
        /// <param name="selectedTypeIsAcceptable">Whether the currently selected type is acceptable to the client.</param>
        /// <param name="matchingTypeNameParts">The number of matched type name parts during type matching.</param>
        /// <param name="selectedMatchingTypeNameParts">The number of matched type name parts for the currently selected type.</param>
        /// <param name="matchingParameters">The number of matched parameters during type matching.</param>
        /// <param name="selectedMatchingParameters">The number of matched parameters for the currently selected type.</param>
        /// <returns>
        /// The <see cref="PrecedenceLevel"/> indicating whether the most recent match has higher precedence, 
        /// the same precedence or lower precedence than the currently selected one.
        /// </returns>
        private static PrecedenceLevel ComputePrecedence(
            bool selectedTypeIsAcceptable,
            int matchingTypeNameParts,
            int selectedMatchingTypeNameParts,
            int matchingParameters,
            int selectedMatchingParameters)
        {
            PrecedenceLevel precedence = PrecedenceLevel.Lower;
            if (!selectedTypeIsAcceptable)
            {
                // the currently selected type is not acceptable to the client; this one matches the type
                // and thus is considered having higher precedence (although we might determine below that this type is
                // also not acceptable to the client due to a 0 quality value).
                precedence = PrecedenceLevel.Higher;
            }
            else if (matchingTypeNameParts > selectedMatchingTypeNameParts)
            {
                // the current type matches more type name parts: choose it.
                precedence = PrecedenceLevel.Higher;
            }
            else if (matchingTypeNameParts == selectedMatchingTypeNameParts)
            {
                // for exact type/subtype matches we check whether both types have no parameters;
                // if that is the case this match is more specific than a previous one (where the
                // supported type might have had parameters).
                // NOTE: only do so for exact type/subtype matches; if the media type specifies a
                //       range ('*') we assume all parameters are supported.
                if (matchingTypeNameParts > 1 && matchingParameters == -1)
                {
                    // the current type perfectly matches the parameters of the 
                    // acceptable type (both have no parameters)
                    precedence = PrecedenceLevel.Higher;
                }

                // Now check the parameters and see whether we find a more specific match.
                if (matchingParameters > selectedMatchingParameters)
                {
                    precedence = PrecedenceLevel.Higher;
                }
                else if (matchingParameters == selectedMatchingParameters)
                {
                    precedence = PrecedenceLevel.Same;
                }
            }

            return precedence;
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
    }
}
