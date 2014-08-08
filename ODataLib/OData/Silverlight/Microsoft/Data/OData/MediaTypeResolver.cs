//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    #endregion Namespaces

    /// <summary>
    /// Class with the responsibility of resolving media types (MIME types) into formats and payload kinds.
    /// </summary>
    internal sealed class MediaTypeResolver
    {
        /// <summary>application/atom+xml media type</summary>
        private static readonly MediaType ApplicationAtomXmlMediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeAtomXmlSubType);

        /// <summary>application/xml media type</summary>
        private static readonly MediaType ApplicationXmlMediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType);

        /// <summary>text/xml media type</summary>
        private static readonly MediaType TextXmlMediaType = new MediaType(MimeConstants.MimeTextType, MimeConstants.MimeXmlSubType);

        /// <summary>application/json media type</summary>
        private static readonly MediaType ApplicationJsonMediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType);

        /// <summary>application/json;odata=verbose media type</summary>
        private static readonly MediaType ApplicationJsonVerboseMediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType, new KeyValuePair<string, string>(MimeConstants.MimeODataParameterName, MimeConstants.MimeODataParameterValueVerbose));

        #region Default media types per payload kind
        /// <summary>
        /// An array that maps stores the supported media types for all <see cref="ODataPayloadKind"/> .
        /// </summary>
        /// <remarks>
        /// The set of supported media types is ordered (desc) by their precedence/priority with respect to (1) format and (2) media type.
        /// As a result the default media type for a given payloadKind is the first entry in the MediaTypeWithFormat array.
        /// </remarks>
        private static readonly MediaTypeWithFormat[][] defaultMediaTypes =
            new MediaTypeWithFormat[13][]
            {
                // feed
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeAtomXmlSubType, new KeyValuePair<string, string>(MimeConstants.MimeTypeParameterName, MimeConstants.MimeTypeParameterValueFeed)) },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationAtomXmlMediaType },
                    new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonVerboseMediaType }
                },

                // entry
                new MediaTypeWithFormat[] 
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeAtomXmlSubType, new KeyValuePair<string, string>(MimeConstants.MimeTypeParameterName, MimeConstants.MimeTypeParameterValueEntry)) },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationAtomXmlMediaType },
                    new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonVerboseMediaType }
                },

                // property
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationXmlMediaType },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = TextXmlMediaType },
                    new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonVerboseMediaType }
                },

                // entity reference link
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationXmlMediaType },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = TextXmlMediaType },
                    new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonVerboseMediaType }
                },

                // entity reference links
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationXmlMediaType },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = TextXmlMediaType },
                    new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonVerboseMediaType }
                },

                // value
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.RawValue, MediaType = new MediaType(MimeConstants.MimeTextType, MimeConstants.MimePlainSubType) },
                },

                // binary
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.RawValue, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeOctetStreamSubType) },
                },

                // collection
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationXmlMediaType },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = TextXmlMediaType },
                    new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonVerboseMediaType }
                },

                // service document
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationXmlMediaType },
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeAtomSvcXmlSubType) },
                    new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonVerboseMediaType }
                },

                // metadata document
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Metadata, MediaType = ApplicationXmlMediaType },
                },

                // error
                new MediaTypeWithFormat[]
                { 
                    new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationXmlMediaType },
                    new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonVerboseMediaType }
                },

                // batch
                new MediaTypeWithFormat[]
                { 
                    // Note that as per spec the multipart/mixed must have a boundary parameter which is not specified here. We will add that parameter
                    // when using this mime type because we need to generate a new boundary every time.
                    new MediaTypeWithFormat { Format = ODataFormat.Batch, MediaType = new MediaType(MimeConstants.MimeMultipartType, MimeConstants.MimeMixedSubType) },
                },

                // parameter
                new MediaTypeWithFormat[]
                {
                    // We will only support parameters in Json format for now.
                    new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonVerboseMediaType }
                },
            };
        #endregion Default media types per payload kind

        /// <summary>Cache for MediaTypeResolvers for each version.</summary>
        private static readonly ODataVersionCache<MediaTypeResolver> MediaTypeResolverCache = new ODataVersionCache<MediaTypeResolver>(version => new MediaTypeResolver(version));

        /// <summary>
        /// The version the media type resolver is used with.
        /// </summary>
        private readonly ODataVersion version;

        /// <summary>
        /// Array of supported media types and formats for each payload kind.
        /// The index into the array matches the order of the ODataPayloadKind enum.
        /// </summary>
        private readonly IList<MediaTypeWithFormat>[] mediaTypesForPayloadKind;

        /// <summary>
        /// The set of payload kinds which are supported for the JSON formats.
        /// </summary>
        private static readonly ODataPayloadKind[] JsonPayloadKinds = new[]
        {
            ODataPayloadKind.Feed,
            ODataPayloadKind.Entry,
            ODataPayloadKind.Property,
            ODataPayloadKind.EntityReferenceLink,
            ODataPayloadKind.EntityReferenceLinks,
            ODataPayloadKind.Collection,
            ODataPayloadKind.ServiceDocument,
            ODataPayloadKind.Error,
            ODataPayloadKind.Parameter
        };

        /// <summary>
        /// Creates a new media type resolver for writers with the mappings for the specified version.
        /// </summary>
        /// <param name="version">The version used to write the payload.</param>
        private MediaTypeResolver(ODataVersion version)
        {
            this.version = version;
            this.mediaTypesForPayloadKind = CloneDefaultMediaTypes();

            if (this.version < ODataVersion.V3)
            {
                // Treat "application/json" the same as "application/json;odata=verbose" in < V3.
                MediaTypeWithFormat jsonMediaTypeWithFormat =
                    new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonMediaType };
                this.AddForJsonPayloadKinds(jsonMediaTypeWithFormat);
            }
            else
            {
                // Add JSON-light media types into the media type table
                this.AddJsonLightMediaTypes();
            }
        }

        /// <summary>
        /// Creates a new media type resolver for readers with the mappings for the specified version and behavior kind.
        /// </summary>
        /// <param name="version">The version used to read the payload.</param>
        /// <param name="formatBehaviorKind">The format behavior kind used.</param>
        private MediaTypeResolver(ODataVersion version, ODataBehaviorKind formatBehaviorKind)
            : this(version)
        {
            DebugUtils.CheckNoExternalCallers();
            
            if (this.version < ODataVersion.V3)
            {
                if (formatBehaviorKind == ODataBehaviorKind.WcfDataServicesClient)
                {
                    this.AddV2ClientMediaTypes();
                }
            }
        }

        /// <summary>
        /// Accesses the default media type resolver.
        /// </summary>
        public static MediaTypeResolver DefaultMediaTypeResolver
        {
            get
            {
                return MediaTypeResolverCache[ODataVersion.V1];
            }
        }

        /// <summary>
        /// Creates a new media type resolver for writers with the mappings for the specified version.
        /// </summary>
        /// <param name="version">The version used to write the payload.</param>
        /// <returns>A new media type resolver for readers with the mappings for the specified version and behavior kind.</returns>
        internal static MediaTypeResolver GetWriterMediaTypeResolver(ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            return MediaTypeResolverCache[version];
        }

        /// <summary>
        /// Creates a new media type resolver for readers with the mappings for the specified version and behavior kind.
        /// </summary>
        /// <param name="version">The version used to read the payload.</param>
        /// <param name="formatBehaviorKind">The format behavior kind used.</param>
        /// <returns>A new media type resolver for readers with the mappings for the specified version and behavior kind.</returns>
        internal static MediaTypeResolver CreateReaderMediaTypeResolver(ODataVersion version, ODataBehaviorKind formatBehaviorKind)
        {
            DebugUtils.CheckNoExternalCallers();
            return new MediaTypeResolver(version, formatBehaviorKind);
        }

        /// <summary>
        /// Gets the supported media types and formats for the given payload kind.
        /// </summary>
        /// <param name="payloadKind">The payload kind to get media types for.</param>
        /// <returns>An array of media type / format pairs, sorted by priority.</returns>
        internal IList<MediaTypeWithFormat> GetMediaTypesForPayloadKind(ODataPayloadKind payloadKind)
        {
            DebugUtils.CheckNoExternalCallers();
            return this.mediaTypesForPayloadKind[(int)payloadKind];
        }

        /// <summary>
        /// Checks for an illegal media type that cannot be caught during content negotiation/resolution
        /// since it would match an unsupported media type.
        /// </summary>
        /// <param name="mediaType">The parsed media type to check.</param>
        /// <returns>true if the media type is illegal (and we should fail); otherwise false.</returns>
        internal bool IsIllegalMediaType(MediaType mediaType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(mediaType != null, "mediaType != null");

            // We fail for JSON-Light for all versions < V3.
            // NOTE: once we support custom formats and custom content negotiation, we will have to move this logic into
            //       the format itself (i.e., also register Json Light for V1 and V2 but make it fail).
            if (this.version < ODataVersion.V3 &&
                HttpUtils.CompareMediaTypeNames(mediaType.SubTypeName, MimeConstants.MimeJsonSubType) &&
                HttpUtils.CompareMediaTypeNames(mediaType.TypeName, MimeConstants.MimeApplicationType) &&
                (mediaType.MediaTypeHasParameterWithValue(MimeConstants.MimeODataParameterName, MimeConstants.MimeODataParameterValueMinimalMetadata)
                 || mediaType.MediaTypeHasParameterWithValue(MimeConstants.MimeODataParameterName, MimeConstants.MimeODataParameterValueFullMetadata)
                 || mediaType.MediaTypeHasParameterWithValue(MimeConstants.MimeODataParameterName, MimeConstants.MimeODataParameterValueNoMetadata)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clones the default media types.
        /// </summary>
        /// <returns>The cloned media type table.</returns>
        private static IList<MediaTypeWithFormat>[] CloneDefaultMediaTypes()
        {
            IList<MediaTypeWithFormat>[] clone = new IList<MediaTypeWithFormat>[defaultMediaTypes.Length];

            for (int i = 0; i < defaultMediaTypes.Length; i++)
            {
                clone[i] = new List<MediaTypeWithFormat>(defaultMediaTypes[i]);
            }

            return clone;
        }

        /// <summary>
        /// Inserts the specified media type before the first occurrence of <paramref name="beforeFormat"/>.
        /// </summary>
        /// <param name="mediaTypeList">The media type list to insert into.</param>
        /// <param name="mediaTypeToInsert">The media type to insert.</param>
        /// <param name="beforeFormat">The format of the media type before which <paramref name="mediaTypeToInsert"/> should be inserted.</param>
        private static void AddMediaTypeEntryOrdered(IList<MediaTypeWithFormat> mediaTypeList, MediaTypeWithFormat mediaTypeToInsert, ODataFormat beforeFormat)
        {
            Debug.Assert(mediaTypeList != null, "mediaTypeList != null");
            Debug.Assert(mediaTypeToInsert != null, "mediaTypeToInsert != null");
            Debug.Assert(beforeFormat != null, "beforeFormat != null");

            // Go through the list and find the first entry that has the specified format to insert before.
            for (int i = 0; i < mediaTypeList.Count; ++i)
            {
                if (mediaTypeList[i].Format == beforeFormat)
                {
                    mediaTypeList.Insert(i, mediaTypeToInsert);
                    return;
                }
            }

            mediaTypeList.Add(mediaTypeToInsert);
        }

        /// <summary>
        /// Configure the media type tables so that Json Light is the first JSON format in the table.
        /// </summary>
        /// <remarks>
        /// This is only used in V3 and beyond.
        /// </remarks>
        private void AddJsonLightMediaTypes()
        {
            var optionalParameters = new[]
            {
                new
                {
                    ParameterName = MimeConstants.MimeODataParameterName,
                    Values = new[] { MimeConstants.MimeODataParameterValueMinimalMetadata, MimeConstants.MimeODataParameterValueFullMetadata, MimeConstants.MimeODataParameterValueNoMetadata }
                },
                new
                {
                    ParameterName = MimeConstants.MimeStreamingParameterName,
                    Values = new[] { MimeConstants.MimeStreamingParameterValueTrue, MimeConstants.MimeStreamingParameterValueFalse }
                },
            };

            // initial seed for the list will be extended in breadth-first passes over the optional parameters
            var mediaTypesToAdd = new LinkedList<MediaType>();
            mediaTypesToAdd.AddFirst(ApplicationJsonMediaType);

            foreach (var optionalParameter in optionalParameters)
            {
                // go through each one so far and extend it
                for (LinkedListNode<MediaType> currentNode = mediaTypesToAdd.First; currentNode != null; currentNode = currentNode.Next)
                {
                    MediaType typeToExtend = currentNode.Value;
                    foreach (string valueToAdd in optionalParameter.Values)
                    {
                        var extendedParameters = new List<KeyValuePair<string, string>>(typeToExtend.Parameters ?? Enumerable.Empty<KeyValuePair<string, string>>())
                        {
                            new KeyValuePair<string, string>(optionalParameter.ParameterName, valueToAdd)
                        };

                        var extendedMediaType = new MediaType(typeToExtend.TypeName, typeToExtend.SubTypeName, extendedParameters);

                        // always match more specific things first
                        mediaTypesToAdd.AddBefore(currentNode, extendedMediaType);
                    }
                }
            }

            foreach (MediaType mediaType in mediaTypesToAdd)
            {
                var mediaTypeWithFormat = new MediaTypeWithFormat { Format = ODataFormat.Json, MediaType = mediaType };
                if (mediaType == ApplicationJsonMediaType)
                {
                    this.AddForJsonPayloadKinds(mediaTypeWithFormat);
                }
                else
                {
                    this.InsertForJsonPayloadKinds(mediaTypeWithFormat, ODataFormat.VerboseJson);
                }
            }
        }

        /// <summary>
        /// Adds the given media type/format for all the payload kinds which support JSON.
        /// </summary>
        /// <param name="mediaTypeWithFormat">The media type/format pair to add.</param>
        private void AddForJsonPayloadKinds(MediaTypeWithFormat mediaTypeWithFormat)
        {
            foreach (ODataPayloadKind kind in JsonPayloadKinds)
            {
                this.mediaTypesForPayloadKind[(int)kind].Add(mediaTypeWithFormat);
            }
        }

        /// <summary>
        /// Inserts the given media type/format for all the payload kinds which support JSON before the first media type with the given format.
        /// </summary>
        /// <param name="mediaTypeWithFormat">The media type/format pair to add.</param>
        /// <param name="beforeFormat">The format of the media type before which <paramref name="mediaTypeWithFormat"/> should be inserted.</param>
        private void InsertForJsonPayloadKinds(MediaTypeWithFormat mediaTypeWithFormat, ODataFormat beforeFormat)
        {
            foreach (ODataPayloadKind kind in JsonPayloadKinds)
            {
                AddMediaTypeEntryOrdered(this.mediaTypesForPayloadKind[(int)kind], mediaTypeWithFormat, beforeFormat);
            }
        }

        /// <summary>
        /// Additional media types per payload kind for the WCF DS client on pre-v3 payloads.
        /// Anything that normally accepts application/atom+xml should also accept application/xml, and vice versa.
        /// </summary>
        private void AddV2ClientMediaTypes()
        {
            MediaTypeWithFormat appAtomXmlMediaType = new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationAtomXmlMediaType };
            MediaTypeWithFormat appXmlMediaType = new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationXmlMediaType };

            IList<MediaTypeWithFormat> feedMediaTypes = this.mediaTypesForPayloadKind[(int)ODataPayloadKind.Feed];
            feedMediaTypes.Add(new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType, new KeyValuePair<string, string>(MimeConstants.MimeTypeParameterName, MimeConstants.MimeTypeParameterValueFeed)) });
            feedMediaTypes.Add(appXmlMediaType);

            IList<MediaTypeWithFormat> entryMediaTypes = this.mediaTypesForPayloadKind[(int)ODataPayloadKind.Entry];
            entryMediaTypes.Add(new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType, new KeyValuePair<string, string>(MimeConstants.MimeTypeParameterName, MimeConstants.MimeTypeParameterValueEntry)) });
            entryMediaTypes.Add(appXmlMediaType);

            this.mediaTypesForPayloadKind[(int)ODataPayloadKind.Property].Add(appAtomXmlMediaType);
            this.mediaTypesForPayloadKind[(int)ODataPayloadKind.EntityReferenceLink].Add(appAtomXmlMediaType);
            this.mediaTypesForPayloadKind[(int)ODataPayloadKind.EntityReferenceLinks].Add(appAtomXmlMediaType);
            this.mediaTypesForPayloadKind[(int)ODataPayloadKind.Collection].Add(appAtomXmlMediaType);
            this.mediaTypesForPayloadKind[(int)ODataPayloadKind.ServiceDocument].Add(appAtomXmlMediaType);
            this.mediaTypesForPayloadKind[(int)ODataPayloadKind.MetadataDocument].Add(appAtomXmlMediaType);
            this.mediaTypesForPayloadKind[(int)ODataPayloadKind.Error].Add(appAtomXmlMediaType);
        }
    }
}
