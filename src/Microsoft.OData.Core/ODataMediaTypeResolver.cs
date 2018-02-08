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
        /// <summary>application/xml media type</summary>
        private static readonly ODataMediaType ApplicationXmlMediaType = new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType);

        /// <summary>application/json media type</summary>
        private static readonly ODataMediaType ApplicationJsonMediaType = new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType);

        #region Default media types per payload kind
        /// <summary>
        /// An array that maps stores the supported media types for all <see cref="ODataPayloadKind"/>, ATOM excluded
        /// </summary>
        /// <remarks>
        /// The set of supported media types is ordered (desc) by their precedence/priority with respect to (1) format and (2) media type.
        /// As a result the default media type for a given payloadKind is the first resource in the MediaTypeWithFormat array.
        /// </remarks>
        private static readonly ODataMediaTypeFormat[][] defaultMediaTypes =
            new ODataMediaTypeFormat[][]
                {
                // resource set
                new ODataMediaTypeFormat[]
                {
                },

                // resource
                new ODataMediaTypeFormat[]
                {
                },

                // property
                new ODataMediaTypeFormat[]
                {
                },

                // entity reference link
                new ODataMediaTypeFormat[]
                {
                },

                // entity reference links
                new ODataMediaTypeFormat[]
                {
                },

                // value
                new ODataMediaTypeFormat[]
                {
                    new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeTextType, MimeConstants.MimePlainSubType), ODataFormat.RawValue),
                },

                // binary
                new ODataMediaTypeFormat[]
                {
                    new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeOctetStreamSubType), ODataFormat.RawValue),
                },

                // collection
                new ODataMediaTypeFormat[]
                {
                },

                // service document
                new ODataMediaTypeFormat[]
                {
                },

                // metadata document
                new ODataMediaTypeFormat[]
                {
                    new ODataMediaTypeFormat(ApplicationXmlMediaType, ODataFormat.Metadata),
                },

                // error
                new ODataMediaTypeFormat[]
                {
                },

                // batch
                new ODataMediaTypeFormat[]
                {
                    // Note that as per spec the multipart/mixed must have a boundary parameter which is not specified here. We will add that parameter
                    // when using this mime type because we need to generate a new boundary every time.
                    new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeMultipartType, MimeConstants.MimeMixedSubType), ODataFormat.Batch),
                },

                // parameter
                new ODataMediaTypeFormat[]
                {
                },

                 // individual property
                new ODataMediaTypeFormat[]
                {
                },

                // delta
                new ODataMediaTypeFormat[]
                {
                },

                // async
                new ODataMediaTypeFormat[]
                {
                    new ODataMediaTypeFormat(new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeHttpSubType), ODataFormat.RawValue),
                },
            };
        #endregion Default media types per payload kind

        /// <summary>
        /// Array of supported media types and formats for each payload kind.
        /// The index into the array matches the order of the ODataPayloadKind enum.
        /// </summary>
        private readonly List<ODataMediaTypeFormat>[] mediaTypesForPayloadKind;

        /// <summary>
        /// The set of payload kinds which are supported for the JSON formats.
        /// </summary>
        private static readonly ODataPayloadKind[] JsonPayloadKinds = new[]
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
            ODataPayloadKind.IndividualProperty,
            ODataPayloadKind.Batch
        };

        /// <summary>
        /// MediaTypeResolver without atom support
        /// </summary>
        private static readonly ODataMediaTypeResolver MediaTypeResolver = new ODataMediaTypeResolver();

        /// <summary>
        /// Creates a new media type resolver with the mappings.
        /// </summary>
        public ODataMediaTypeResolver()
        {
            this.mediaTypesForPayloadKind = CloneDefaultMediaTypes();

            // Add JSON-light media types into the media type table
            this.AddJsonLightMediaTypes();
        }

        /// <summary>
        /// Gets the supported media types and formats for the given payload kind.
        /// </summary>
        /// <param name="payloadKind">The payload kind to get media types for.</param>
        /// <returns>Media type-format pairs, sorted by priority.</returns>
        public virtual IEnumerable<ODataMediaTypeFormat> GetMediaTypeFormats(ODataPayloadKind payloadKind)
        {
            return this.mediaTypesForPayloadKind[(int)payloadKind];
        }

        internal static ODataMediaTypeResolver GetMediaTypeResolver(IServiceProvider container)
        {
            if (container == null)
            {
                return MediaTypeResolver;
            }

            return container.GetRequiredService<ODataMediaTypeResolver>();
        }

        /// <summary>
        /// Clones the default media types.
        /// </summary>
        /// <returns>The cloned media type table.</returns>
        private static List<ODataMediaTypeFormat>[] CloneDefaultMediaTypes()
        {
            ODataMediaTypeFormat[][] mediaTypes = defaultMediaTypes;

            List<ODataMediaTypeFormat>[] clone = new List<ODataMediaTypeFormat>[mediaTypes.Length];

            for (int i = 0; i < mediaTypes.Length; i++)
            {
                clone[i] = new List<ODataMediaTypeFormat>(mediaTypes[i]);
            }

            return clone;
        }

        /// <summary>
        /// Configure the media type tables so that Json Light is the first JSON format in the table, except for the
        /// case of Batch payload kind, where the JsonLight media types is added to the back of the list to preserve
        /// original behavior of default MIME media type.
        /// </summary>
        /// <remarks>
        /// This is only used in V4 and beyond.
        /// </remarks>
        private void AddJsonLightMediaTypes()
        {
            var optionalParameters = new[]
            {
                new
                {
                    ParameterName = MimeConstants.MimeMetadataParameterName,
                    Values = new[] { MimeConstants.MimeMetadataParameterValueMinimal, MimeConstants.MimeMetadataParameterValueFull, MimeConstants.MimeMetadataParameterValueNone }
                },
                new
                {
                    ParameterName = MimeConstants.MimeStreamingParameterName,
                    Values = new[] { MimeConstants.MimeParameterValueTrue, MimeConstants.MimeParameterValueFalse }
                },
                new
                {
                    ParameterName = MimeConstants.MimeIeee754CompatibleParameterName,
                    Values = new[] { MimeConstants.MimeParameterValueFalse, MimeConstants.MimeParameterValueTrue }
                },
            };

            // initial seed for the list will be extended in breadth-first passes over the optional parameters
            var mediaTypesToAdd = new LinkedList<ODataMediaType>();
            mediaTypesToAdd.AddFirst(ApplicationJsonMediaType);

            foreach (var optionalParameter in optionalParameters)
            {
                // go through each one so far and extend it
                for (LinkedListNode<ODataMediaType> currentNode = mediaTypesToAdd.First; currentNode != null; currentNode = currentNode.Next)
                {
                    ODataMediaType typeToExtend = currentNode.Value;
                    foreach (string valueToAdd in optionalParameter.Values)
                    {
                        var extendedParameters = new List<KeyValuePair<string, string>>(typeToExtend.Parameters ?? Enumerable.Empty<KeyValuePair<string, string>>())
                        {
                            new KeyValuePair<string, string>(optionalParameter.ParameterName, valueToAdd)
                        };

                        var extendedMediaType = new ODataMediaType(typeToExtend.Type, typeToExtend.SubType, extendedParameters);

                        // always match more specific things first
                        mediaTypesToAdd.AddBefore(currentNode, extendedMediaType);
                    }
                }
            }

            List<ODataMediaTypeFormat> mediaTypeWithFormat = mediaTypesToAdd.Select(mediaType => new ODataMediaTypeFormat(mediaType, ODataFormat.Json)).ToList();

            foreach (ODataPayloadKind kind in JsonPayloadKinds)
            {
                if (kind == ODataPayloadKind.Batch)
                {
                    // Appending the json media types AFTER the existing MIME media type(s), which is the default media type for Batch payload kind.
                    this.mediaTypesForPayloadKind[(int)kind].AddRange(mediaTypeWithFormat);
                }
                else
                {   // For other payload kinds, insert the json media types to the front.
                    this.mediaTypesForPayloadKind[(int)kind].InsertRange(0, mediaTypeWithFormat);
                }
            }
        }
    }
}
