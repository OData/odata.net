//---------------------------------------------------------------------
// <copyright file="TestMediaTypeUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    public static class TestMediaTypeUtils
    {
        /// <summary>UTF-8 encoding, without the BOM preamble.</summary>
        public static readonly UTF8Encoding EncodingUtf8NoPreamble = new UTF8Encoding(false, true);

        /// <summary>Western European DOS encoding</summary>
#if !SILVERLIGHT && !WINDOWS_PHONE   // Encoding not supported on these platforms
        public static readonly Encoding Ibm850Encoding = Encoding.GetEncoding("ibm850", new EncoderExceptionFallback(), new DecoderExceptionFallback());
#endif

#if SILVERLIGHT || WINDOWS_PHONE   // ISO-8859-1 not available
        private static readonly Encoding Latin1Encoding = Encoding.UTF8;
#else
        public static readonly Encoding Latin1Encoding = Encoding.GetEncoding("ISO-8859-1", new EncoderExceptionFallback(), new DecoderExceptionFallback());
#endif

        /// <summary>
        /// An array that maps stores the supported media types for all <see cref="ODataPayloadKind"/> .
        /// </summary>
        /// <remarks>
        /// The set of supported media types is ordered (desc) by their precedence/priority with respect to (1) format and (2) media type.
        /// As a result the default media type for a given payloadKind is the first entry in the MediaTypeWithFormat array.
        /// </remarks>
        private static readonly MediaTypeWithFormat[][] mediaTypesForPayloadKind =
            new MediaTypeWithFormat[][]
            {
                // feed
                new MediaTypeWithFormat[]
                {
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = MimeConstants.MimeApplicationType + "/" + MimeConstants.MimeJsonSubType }
                },

                // entry
                new MediaTypeWithFormat[]
                {
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = MimeConstants.MimeApplicationType + "/" + MimeConstants.MimeJsonSubType }
                },

                // property
                new MediaTypeWithFormat[]
                {
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = MimeConstants.MimeApplicationType + "/" + MimeConstants.MimeJsonSubType }
                },

                // entity reference link
                new MediaTypeWithFormat[]
                {
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = MimeConstants.MimeApplicationType + "/" + MimeConstants.MimeJsonSubType }
                },

                // entity reference links
                new MediaTypeWithFormat[]
                {
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = MimeConstants.MimeApplicationType + "/" +  MimeConstants.MimeJsonSubType }
                },

                // value
                new MediaTypeWithFormat[]
                {
                    new MediaTypeWithFormat { Format = ODataFormat.RawValue, MediaType = MimeConstants.MimeTextType + "/" + MimeConstants.MimePlainSubType },
                },

                // binary
                new MediaTypeWithFormat[]
                {
                    new MediaTypeWithFormat { Format = ODataFormat.RawValue, MediaType = MimeConstants.MimeApplicationType + "/" + MimeConstants.MimeOctetStreamSubType },
                },

                // collection
                new MediaTypeWithFormat[]
                {
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = MimeConstants.MimeApplicationType + "/" + MimeConstants.MimeJsonSubType }
                },

                // service document
                new MediaTypeWithFormat[]
                {
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = MimeConstants.MimeApplicationType + "/" + MimeConstants.MimeJsonSubType }
                },

                // metadata document
                new MediaTypeWithFormat[]
                {
                    new MediaTypeWithFormat { Format = ODataFormat.Metadata, MediaType = MimeConstants.MimeApplicationType + "/" + MimeConstants.MimeXmlSubType },
                },

                // error
                new MediaTypeWithFormat[]
                {
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = MimeConstants.MimeApplicationType + "/" + MimeConstants.MimeJsonSubType }
                },

                // batch
                new MediaTypeWithFormat[]
                {
                    // Note that as per spec the multipart/mixed must have a boundary parameter which is not specified here. We will add that parameter
                    // when using this mime type because we need to generate a new boundary every time.
                    new MediaTypeWithFormat { Format = ODataFormat.Batch, MediaType = MimeConstants.MimeMultipartType + "/" + MimeConstants.MimeMixedSubType },
                },

                // parameter
                new MediaTypeWithFormat[]
                {
                    // We will only support parameters in Json format for now.
                    new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = MimeConstants.MimeApplicationType + "/" + MimeConstants.MimeJsonSubType }
                },
            };

        /// <summary>
        /// Constant values related to media types.
        /// </summary>
        private static class MimeConstants
        {
            /// <summary>'application' - media type for application types.</summary>
            internal const string MimeApplicationType = "application";

            /// <summary>'text' - media type for text subtypes.</summary>
            internal const string MimeTextType = "text";

            /// <summary>'multipart' - media type.</summary>
            internal const string MimeMultipartType = "multipart";

            /// <summary>'atom+xml' - constant for atom+xml subtypes.</summary>
            internal const string MimeAtomXmlSubType = "atom+xml";

            /// <summary>'atomsvc+xml' - constant for atomsvc+xml subtypes.</summary>
            internal const string MimeAtomSvcXmlSubType = "atomsvc+xml";

            /// <summary>'xml' - constant for xml subtypes.</summary>
            internal const string MimeXmlSubType = "xml";

            /// <summary>'json' - constant for JSON subtypes.</summary>
            internal const string MimeJsonSubType = "json";

            /// <summary>'plain' - constant for text subtypes.</summary>
            internal const string MimePlainSubType = "plain";

            /// <summary>'octet-stream' subtype.</summary>
            internal const string MimeOctetStreamSubType = "octet-stream";

            /// <summary>'mixed' subtype.</summary>
            internal const string MimeMixedSubType = "mixed";

            /// <summary>'http' subtype.</summary>
            internal const string MimeHttpSubType = "http";

            /// <summary>Parameter name for 'type' parameters.</summary>
            internal const string MimeTypeParameterName = "type";

            /// <summary>Parameter value for type 'entry'.</summary>
            internal const string MimeTypeParameterValueEntry = "entry";

            /// <summary>Parameter value for type 'feed'.</summary>
            internal const string MimeTypeParameterValueFeed = "feed";
        }

        /// <summary>
        /// Get the default encoding for a payload kind.
        /// </summary>
        /// <param name="kind">The <see cref="ODataPayloadKind"/> to get the default encoding for.</param>
        /// <param name="format">The format to get the default encoding for.</param>
        /// <returns>The default encoding for <paramref name="kind"/>.</returns>
        public static Encoding GetDefaultEncoding(ODataPayloadKind kind, ODataFormat format)
        {
            if (format == ODataFormat.Json)
            {
                return null;
            }

            if (format == ODataFormat.Metadata)
            {
                return null;
            }

            if (format == ODataFormat.Batch)
            {
                return null;
            }

            if (format == ODataFormat.RawValue)
            {
                return Latin1Encoding;
            }

            throw new NotSupportedException("Unsupported format: " + format);
        }

        /// <summary>
        /// Gets the expected format for a given payload kind and format.
        /// The provided format can be 'Default' which will be mapped to the default
        /// format used by the writer for the given payload kind.
        /// </summary>
        /// <param name="kind">The payload kind to get the default format for.</param>
        /// <param name="format">The format from the test configuration.</param>
        /// <returns>The expected output format used by the message writer.</returns>
        public static ODataFormat GetDefaultFormat(ODataPayloadKind kind, ODataFormat format)
        {
            if (format == null)
            {
                switch (kind)
                {
                    case ODataPayloadKind.MetadataDocument:
                        return ODataFormat.Metadata;
                    case ODataPayloadKind.Batch:
                        return ODataFormat.Batch;
                    case ODataPayloadKind.Value:
                    case ODataPayloadKind.BinaryValue:
                        return ODataFormat.RawValue;
                    default:
                        return ODataFormat.Json;
                }
            }

            return format;
        }

        /// <summary>
        /// Gets the default content type for a given payload kind and format.
        /// </summary>
        /// <param name="kind">The payload kind to get the default format for.</param>
        /// <param name="format">The format from the test configuration.</param>
        /// <returns>The default content type used by the message writer.</returns>
        public static string GetDefaultContentType(ODataPayloadKind kind, ODataFormat format)
        {
            switch (kind)
            {
                case ODataPayloadKind.ResourceSet:
                case ODataPayloadKind.EntityReferenceLinks:
                    if (format == ODataFormat.Json) return "application/json;odata.metadata=minimal;odata.streaming=true;charset=utf-8";
                    else throw new NotSupportedException("Unsupported format for " + kind.ToString() + ".");
                case ODataPayloadKind.Resource:
                    if (format == ODataFormat.Json) return "application/json;odata.metadata=minimal;odata.streaming=true;charset=utf-8";
                    else throw new NotSupportedException("Unsupported format for " + kind.ToString() + ".");
                case ODataPayloadKind.Value:
                    if (format == ODataFormat.RawValue || format == null) return "text/plain;charset=iso-8859-1";
                    else throw new NotSupportedException("Unsupported format for " + kind.ToString() + ".");
                case ODataPayloadKind.BinaryValue:
                    if (format == ODataFormat.RawValue || format == null) return "application/octet-stream";
                    else throw new NotSupportedException("Unsupported format for " + kind.ToString() + ".");
                case ODataPayloadKind.Property:             // fall through
                case ODataPayloadKind.Collection:           // fall through
                case ODataPayloadKind.EntityReferenceLink:  // fall through
                case ODataPayloadKind.Error:                // fall through
                case ODataPayloadKind.ServiceDocument:
                    if (format == ODataFormat.Json) return "application/json;odata.metadata=minimal;odata.streaming=true;charset=utf-8";
                    else throw new NotSupportedException("Unsupported format for " + kind.ToString() + ".");
                case ODataPayloadKind.Batch:
                    if (format == ODataFormat.Batch || format == null) return "multipart/mixed";
                    else throw new NotSupportedException("Unsupported format for " + kind.ToString() + ".");
                case ODataPayloadKind.MetadataDocument:
                    if (format == ODataFormat.Metadata || format == null) return "application/xml";
                    else throw new NotSupportedException("Unsupported format for " + kind.ToString() + ".");
                case ODataPayloadKind.Parameter:
                    if (format == ODataFormat.Json) return "application/json;odata.metadata=minimal;odata.streaming=true;charset=utf-8";
                    else throw new NotSupportedException("Unsupported format for " + kind.ToString() + ".");
                case ODataPayloadKind.Unsupported:
                default:
                    throw new NotSupportedException("Unsupported payload kind found: " + kind.ToString());
            }
        }

        /// <summary>
        /// Get the supported media types for a payload kind.
        /// </summary>
        /// <param name="kind">The <see cref="ODataPayloadKind"/> to get the supported media types for.</param>
        /// <param name="includeApplicationJson">true if application/json should be included as supported media type (for reading versions &lt; V3).</param>
        /// <param name="includeApplicationJsonLight">true if JsonLight media types should be included.</param>
        /// <returns>The string concatenating all supported media types for <paramref name="kind"/>.</returns>
        public static string GetSupportedMediaTypes(ODataPayloadKind kind, bool includeApplicationJson = true, bool includeApplicationJsonLight = true)
        {
            StringBuilder builder = new StringBuilder();
            bool hasTailingComma = false;

            // Add the JSON media types to the supported payload kinds
            switch (kind)
            {
                case ODataPayloadKind.ResourceSet:
                case ODataPayloadKind.Resource:
                case ODataPayloadKind.Property:
                case ODataPayloadKind.Collection:
                case ODataPayloadKind.EntityReferenceLink:
                case ODataPayloadKind.EntityReferenceLinks:
                case ODataPayloadKind.Error:
                case ODataPayloadKind.ServiceDocument:
                case ODataPayloadKind.Parameter:
                    AddJsonMediaTypes(includeApplicationJson, includeApplicationJsonLight, builder);
                    break;
                default:
                    break;
            }

            switch (kind)
            {
                case ODataPayloadKind.ResourceSet:
                case ODataPayloadKind.EntityReferenceLinks:
                    break;
                case ODataPayloadKind.Resource:
                    break;
                case ODataPayloadKind.Property:
                    break;
                case ODataPayloadKind.Collection:
                    break;
                case ODataPayloadKind.Value:
                    builder.Append("text/plain");
                    break;
                case ODataPayloadKind.BinaryValue:
                    builder.Append("application/octet-stream");
                    break;
                case ODataPayloadKind.EntityReferenceLink:
                    break;
                case ODataPayloadKind.Error:
                    break;
                case ODataPayloadKind.ServiceDocument:
                    break;
                case ODataPayloadKind.Batch:
                    builder.Append("multipart/mixed, ");
                    AddJsonMediaTypes(includeApplicationJson, includeApplicationJsonLight, builder);
                    hasTailingComma = true;
                    break;
                case ODataPayloadKind.MetadataDocument:
                    break;
                case ODataPayloadKind.Parameter:
                    builder.Append(string.Empty);
                    break;
                case ODataPayloadKind.Unsupported:
                default:
                    throw new NotSupportedException("Unsupported payload kind found: " + kind.ToString());
            }

            return hasTailingComma
                    ? builder.ToString(0, builder.Length - 1)
                    : builder.ToString();
        }

        /// <summary>
        /// Populate the names of all possible application/json media types to the string builder passed in.
        /// </summary>
        /// <param name="includeApplicationJson">true if application/json should be included as supported media type (for reading versions &lt; V3).</param>
        /// <param name="includeApplicationJsonLight">true if JsonLight media types should be included (for OData V4 and beyond).</param>
        /// <param name="builder">string builder object to accept the eligible media type names.</param>
        /// <returns>The string concatenating all supported media types for <paramref name="kind"/>.</returns>
        private static void AddJsonMediaTypes(bool includeApplicationJson, bool includeApplicationJsonLight,
            StringBuilder builder)
        {
            if (includeApplicationJsonLight)
            {
                builder.Append("application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false");
                builder.Append(", application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=true");
                builder.Append(", application/json;odata.metadata=minimal;odata.streaming=true");
                builder.Append(", application/json;odata.metadata=minimal;odata.streaming=false;IEEE754Compatible=false");
                builder.Append(", application/json;odata.metadata=minimal;odata.streaming=false;IEEE754Compatible=true");
                builder.Append(", application/json;odata.metadata=minimal;odata.streaming=false");
                builder.Append(", application/json;odata.metadata=minimal;IEEE754Compatible=false");
                builder.Append(", application/json;odata.metadata=minimal;IEEE754Compatible=true");
                builder.Append(", application/json;odata.metadata=minimal");
                builder.Append(", application/json;odata.metadata=full;odata.streaming=true;IEEE754Compatible=false");
                builder.Append(", application/json;odata.metadata=full;odata.streaming=true;IEEE754Compatible=true");
                builder.Append(", application/json;odata.metadata=full;odata.streaming=true");
                builder.Append(", application/json;odata.metadata=full;odata.streaming=false;IEEE754Compatible=false");
                builder.Append(", application/json;odata.metadata=full;odata.streaming=false;IEEE754Compatible=true");
                builder.Append(", application/json;odata.metadata=full;odata.streaming=false");
                builder.Append(", application/json;odata.metadata=full;IEEE754Compatible=false");
                builder.Append(", application/json;odata.metadata=full;IEEE754Compatible=true");
                builder.Append(", application/json;odata.metadata=full");
                builder.Append(", application/json;odata.metadata=none;odata.streaming=true;IEEE754Compatible=false");
                builder.Append(", application/json;odata.metadata=none;odata.streaming=true;IEEE754Compatible=true");
                builder.Append(", application/json;odata.metadata=none;odata.streaming=true");
                builder.Append(", application/json;odata.metadata=none;odata.streaming=false;IEEE754Compatible=false");
                builder.Append(", application/json;odata.metadata=none;odata.streaming=false;IEEE754Compatible=true");
                builder.Append(", application/json;odata.metadata=none;odata.streaming=false");
                builder.Append(", application/json;odata.metadata=none;IEEE754Compatible=false");
                builder.Append(", application/json;odata.metadata=none;IEEE754Compatible=true");
                builder.Append(", application/json;odata.metadata=none");
                builder.Append(", application/json;odata.streaming=true;IEEE754Compatible=false");
                builder.Append(", application/json;odata.streaming=true;IEEE754Compatible=true");
                builder.Append(", application/json;odata.streaming=true");
                builder.Append(", application/json;odata.streaming=false;IEEE754Compatible=false");
                builder.Append(", application/json;odata.streaming=false;IEEE754Compatible=true");
                builder.Append(", application/json;odata.streaming=false");
                builder.Append(", application/json;IEEE754Compatible=false");
                builder.Append(", application/json;IEEE754Compatible=true");
                builder.Append(", ");
            }

            if (includeApplicationJson)
            {
                builder.Append("application/json");
                builder.Append(", ");
            }
        }

        /// <summary>
        /// Gets the ODataFormat associated with the contentType and the payload
        /// </summary>
        /// <param name="contentType">The content type associated with the payload</param>
        /// <param name="payload">The payload associated with the content type</param>
        /// <returns>The appropriate ODataFormat</returns>
        public static ODataFormat GetODataFormat(string contentType, ODataPayloadKind payloadKind)
        {
            ExceptionUtilities.CheckArgumentNotNull(contentType, "contentType");
            ExceptionUtilities.CheckArgumentNotNull(payloadKind, "payloadKind");
            var indexOfSeparator = contentType.IndexOf(';');
            if (indexOfSeparator > -1)
            {
                contentType = contentType.Substring(0, indexOfSeparator);
            }
            foreach (var mediaType in TestMediaTypeUtils.mediaTypesForPayloadKind[(int)payloadKind])
            {
                if (mediaType.MediaType.Contains(contentType))
                {
                    return mediaType.Format;
                }
            }

            throw new TaupoArgumentException("The content type provided does not support the payload kind");
        }

        /// <summary>
        /// A private helper class to associate a <see cref="ODataFormat"/> with a media type.
        /// </summary>
        private sealed class MediaTypeWithFormat
        {
            /// <summary>The media type.</summary>
            public string MediaType
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
