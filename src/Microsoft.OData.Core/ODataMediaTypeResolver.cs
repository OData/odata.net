//---------------------------------------------------------------------
// <copyright file="ODataMediaTypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    #endregion Namespaces

    /// <summary>
    /// Class with the responsibility of resolving media types (MIME types) into formats and payload kinds.
    /// </summary>
    public class ODataMediaTypeResolver
    {
        /// <summary>
        /// The set of payload kinds which are supported for the JSON formats.
        /// </summary>
        private static readonly HashSet<ODataPayloadKind> JsonPayloadKindSet = new HashSet<ODataPayloadKind>
        {
            ODataPayloadKind.ResourceSet,
            ODataPayloadKind.Resource,
            ODataPayloadKind.Property,
            ODataPayloadKind.EntityReferenceLink,
            ODataPayloadKind.EntityReferenceLinks,
            ODataPayloadKind.Collection,
            ODataPayloadKind.ServiceDocument,
            ODataPayloadKind.Error,
            ODataPayloadKind.Parameter,
            ODataPayloadKind.Delta,
            ODataPayloadKind.IndividualProperty
        };

        /// <summary>
        /// MediaTypeResolver without atom support
        /// </summary>
        private static readonly ODataMediaTypeResolver MediaTypeResolver = new ODataMediaTypeResolver();

        /// <summary>
        /// The special map of payload kind and supported media type.
        /// </summary>
        private static IDictionary<ODataPayloadKind, IEnumerable<ODataMediaTypeFormat>> SpecialMediaTypeFormat = new Dictionary<ODataPayloadKind, IEnumerable<ODataMediaTypeFormat>>
        {
            {
                ODataPayloadKind.Batch,
                new[] { new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeMultipartType, MimeConstants.MimeMixedSubType), ODataFormat.Batch) }
            },
            {
                ODataPayloadKind.Value,
                new[] { new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeTextType, MimeConstants.MimePlainSubType), ODataFormat.RawValue) }
            },
            {
                ODataPayloadKind.BinaryValue,
                new[] { new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeOctetStreamSubType), ODataFormat.RawValue) }
            },
            {
                ODataPayloadKind.MetadataDocument,
                new[]
                {
                    new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType), ODataFormat.Metadata),
                    new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType), ODataFormat.Metadata)
                }
            },
            {
                ODataPayloadKind.Asynchronous,
                new[] { new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeHttpSubType), ODataFormat.RawValue) }
            }
        };

        /// <summary>
        /// Array of supported media types and formats for JSON related payload kind.
        /// </summary>
        private static IEnumerable<ODataMediaTypeFormat> JsonMediaTypeFormats = SetJsonLightMediaTypes();

        /// <summary>
        /// Gets the supported media types and formats for the given payload kind.
        /// </summary>
        /// <param name="payloadKind">The payload kind to get media types for.</param>
        /// <returns>Media type-format pairs, sorted by priority.</returns>
        public virtual IEnumerable<ODataMediaTypeFormat> GetMediaTypeFormats(ODataPayloadKind payloadKind)
        {
            if (JsonPayloadKindSet.Contains(payloadKind))
            {
                return JsonMediaTypeFormats;
            }
            else if (payloadKind == ODataPayloadKind.Batch)
            {
                return SpecialMediaTypeFormat[payloadKind].Concat(JsonMediaTypeFormats);
            }
            else
            {
                return SpecialMediaTypeFormat[payloadKind];
            }
        }

        internal static ODataMediaTypeResolver GetMediaTypeResolver(IServiceProvider container)
        {
            if (container == null)
            {
                return MediaTypeResolver;
            }

            return container.GetRequiredService<ODataMediaTypeResolver>();
        }

        private static IEnumerable<ODataMediaTypeFormat> SetJsonLightMediaTypes()
        {
            var minimal = new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueMinimal);
            var full = new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueFull);
            var none = new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueNone);

            var streamTrue = new KeyValuePair<string, string>(MimeConstants.MimeStreamingParameterName, MimeConstants.MimeParameterValueTrue);
            var streamFalse = new KeyValuePair<string, string>(MimeConstants.MimeStreamingParameterName, MimeConstants.MimeParameterValueFalse);

            var ieeeFalse = new KeyValuePair<string, string>(MimeConstants.MimeIeee754CompatibleParameterName, MimeConstants.MimeParameterValueFalse);
            var ieeeTrue = new KeyValuePair<string, string>(MimeConstants.MimeIeee754CompatibleParameterName, MimeConstants.MimeParameterValueTrue);

            return new List<ODataMediaTypeFormat>
            {
                // minimal
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { minimal, streamTrue, ieeeFalse }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { minimal, streamTrue, ieeeTrue }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { minimal, streamTrue }), ODataFormat.Json),

                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { minimal, streamFalse, ieeeFalse }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { minimal, streamFalse, ieeeTrue }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { minimal, streamFalse }), ODataFormat.Json),

                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { minimal, ieeeFalse }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { minimal, ieeeTrue }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { minimal }), ODataFormat.Json),

                // full
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { full, streamTrue, ieeeFalse }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { full, streamTrue, ieeeTrue }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { full, streamTrue }), ODataFormat.Json),

                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { full, streamFalse, ieeeFalse }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { full, streamFalse, ieeeTrue }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { full, streamFalse }), ODataFormat.Json),

                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { full, ieeeFalse }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { full, ieeeTrue }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { full }), ODataFormat.Json),

                // none
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { none, streamTrue, ieeeFalse }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { none, streamTrue, ieeeTrue }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { none, streamTrue }), ODataFormat.Json),

                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { none, streamFalse, ieeeFalse }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { none, streamFalse, ieeeTrue }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { none, streamFalse }), ODataFormat.Json),

                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { none, ieeeFalse }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { none, ieeeTrue }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { none }), ODataFormat.Json),

                // without metadata
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { streamTrue, ieeeFalse }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { streamTrue, ieeeTrue }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { streamTrue }), ODataFormat.Json),

                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { streamFalse, ieeeFalse }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { streamFalse, ieeeTrue }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { streamFalse }), ODataFormat.Json),

                // without metadata and stream
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { ieeeFalse }), ODataFormat.Json),
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new[] { ieeeTrue }), ODataFormat.Json),

                // without parameters
                new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType), ODataFormat.Json)
            };
        }
    }
}
