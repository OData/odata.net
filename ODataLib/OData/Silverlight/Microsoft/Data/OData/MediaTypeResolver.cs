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

        #region Additional WCF DS client media types for pre-v3 payloads.
        /// <summary>
        /// Additional media types per payload kind for the WCF DS client on pre-v3 payloads.
        /// Anything that normally accepts application/atom+xml should also accept application/xml, and vice versa.
        /// </summary>
        private static readonly Dictionary<ODataPayloadKind, MediaTypeWithFormat[]> additionalClientV2MediaTypes =
           new Dictionary<ODataPayloadKind, MediaTypeWithFormat[]>(EqualityComparer<ODataPayloadKind>.Default)
                {
                    { 
                        ODataPayloadKind.Feed, 
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType, new KeyValuePair<string, string>(MimeConstants.MimeTypeParameterName, MimeConstants.MimeTypeParameterValueFeed)) },
                            new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationXmlMediaType },
                        }
                    },
                    {
                        ODataPayloadKind.Entry,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = new MediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType, new KeyValuePair<string, string>(MimeConstants.MimeTypeParameterName, MimeConstants.MimeTypeParameterValueEntry)) },
                            new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationXmlMediaType },
                        }
                    },
                    {
                        ODataPayloadKind.Property,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationAtomXmlMediaType },
                        }
                    },
                    {
                        ODataPayloadKind.EntityReferenceLink,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationAtomXmlMediaType },
                        }
                    },
                    {
                        ODataPayloadKind.EntityReferenceLinks,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationAtomXmlMediaType },
                        }
                    },
                    {
                        ODataPayloadKind.Collection,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationAtomXmlMediaType },
                        }
                    },
                    {
                        ODataPayloadKind.ServiceDocument,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationAtomXmlMediaType },
                        }
                    },
                    {
                        ODataPayloadKind.MetadataDocument,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationAtomXmlMediaType },
                        }
                    },
                    {
                        ODataPayloadKind.Error,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.Atom, MediaType = ApplicationAtomXmlMediaType },
                        }
                    },
                };
        #endregion Additional WCF DS client media types for pre-v3 payloads.

        /// <summary>
        /// Additional media types that create following effect:
        /// Wherever we normally have json verbose, add plain application/json as well
        /// </summary>
        /// <remarks>
        /// This is used for V1 and V2 payloads when "application/json" means "application/json;odata=verbose".
        /// In V3 and beyond, "application/json" means "application/json;odata=verbose".
        /// </remarks>
        private static readonly Dictionary<ODataPayloadKind, MediaTypeWithFormat[]> plainAppJsonMeansVerbose =
           new Dictionary<ODataPayloadKind, MediaTypeWithFormat[]>(EqualityComparer<ODataPayloadKind>.Default)
                {
                    { 
                        ODataPayloadKind.Feed, 
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonMediaType }
                        }
                    },
                    {
                        ODataPayloadKind.Entry,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonMediaType }
                        }
                    },
                    {
                        ODataPayloadKind.Property,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonMediaType }
                        }
                    },
                    {
                        ODataPayloadKind.EntityReferenceLink,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonMediaType }
                        }
                    },
                    {
                        ODataPayloadKind.EntityReferenceLinks,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonMediaType }
                        }
                    },
                    {
                        ODataPayloadKind.Collection,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonMediaType }
                        }
                    },
                    {
                        ODataPayloadKind.ServiceDocument,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonMediaType }
                        }
                    },
                    {
                        ODataPayloadKind.Error,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonMediaType }
                        }
                    },
                    {
                        ODataPayloadKind.Parameter,
                        new MediaTypeWithFormat[]
                        {
                            new MediaTypeWithFormat { Format = ODataFormat.VerboseJson, MediaType = ApplicationJsonMediaType }
                        }
                    }
                };

        /// <summary>
        /// Backing field for the default media type resolver. 
        /// This gets initialized when the corresponding property is first accessed.
        /// </summary>
        private static MediaTypeResolver defaultMediaTypeResolver;

        /// <summary>
        /// Array of supported media types and formats for each payload kind.
        /// The index into the array matches the order of the ODataPayloadKind enum.
        /// </summary>
        private MediaTypeWithFormat[][] mediaTypesForPayloadKind;

        /// <summary>
        /// Creates a new media type resolver with the default mappings.
        /// </summary>
        /// <param name="shouldPlainAppJsonImplyVerboseJson">Whether "application/json" with no "odata" parameter should be interpreted as "application/json;odata=verbose"</param>
        /// <param name="shouldAppXmlAndAppAtomXmlBeInterchangeable">Whether "application/xml" and "application/atom+xml" should be interchangeable.</param>
        internal MediaTypeResolver(bool shouldPlainAppJsonImplyVerboseJson, bool shouldAppXmlAndAppAtomXmlBeInterchangeable)
        {
            DebugUtils.CheckNoExternalCallers();

            this.mediaTypesForPayloadKind = new MediaTypeWithFormat[defaultMediaTypes.Length][];
            for (int i = 0; i < defaultMediaTypes.Length; i++)
            {
                this.mediaTypesForPayloadKind[i] = new MediaTypeWithFormat[defaultMediaTypes[i].Length];
                defaultMediaTypes[i].CopyTo(this.mediaTypesForPayloadKind[i], 0);
            }

            if (shouldPlainAppJsonImplyVerboseJson)
            {
                this.AddCustomMediaTypes(plainAppJsonMeansVerbose);
            }

            if (shouldAppXmlAndAppAtomXmlBeInterchangeable)
            {
                this.AddCustomMediaTypes(additionalClientV2MediaTypes);
            }
        }

        /// <summary>
        /// Accesses the default media type resolver.
        /// </summary>
        public static MediaTypeResolver DefaultMediaTypeResolver
        {
            get
            {
                if (defaultMediaTypeResolver == null)
                {
                    defaultMediaTypeResolver = new MediaTypeResolver(
                        false /*shouldPlainAppJsonImplyVerboseJson*/,
                        false /*shouldAppXmlAndAppAtomXmlBeInterchangeable*/);
                }

                return defaultMediaTypeResolver;
            }
        }

        /// <summary>
        /// Gets the supported media types and formats for the given payload kind.
        /// </summary>
        /// <param name="payloadKind">The payload kind to get media types for.</param>
        /// <returns>An array of media type / format pairs, sorted by priority.</returns>
        internal MediaTypeWithFormat[] GetMediaTypesForPayloadKind(ODataPayloadKind payloadKind)
        {
            DebugUtils.CheckNoExternalCallers(); 
            return this.mediaTypesForPayloadKind[(int)payloadKind];
        }

        /// <summary>
        /// Modifies the MediaTypeResolver to include additional media types.
        /// </summary>
        /// <param name="customMediaTypes">The payload kind mappings to add.</param>
        private void AddCustomMediaTypes(Dictionary<ODataPayloadKind, MediaTypeWithFormat[]> customMediaTypes)
        {
            foreach (KeyValuePair<ODataPayloadKind, MediaTypeWithFormat[]> payloadKindMediaTypePair in customMediaTypes)
            {
                MediaTypeWithFormat[] newMappings = payloadKindMediaTypePair.Value;
                MediaTypeWithFormat[] existingMappings = this.mediaTypesForPayloadKind[(int)payloadKindMediaTypePair.Key];
                MediaTypeWithFormat[] combinedMappings = new MediaTypeWithFormat[existingMappings.Length + newMappings.Length];

                // Copy the existing and new mappings into the combinedMappings array.
                existingMappings.CopyTo(combinedMappings, 0);
                newMappings.CopyTo(combinedMappings, existingMappings.Length);

                this.mediaTypesForPayloadKind[(int)payloadKindMediaTypePair.Key] = combinedMappings;
            }
        }
    }
}
