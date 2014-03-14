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

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Base class for all OData JsonLight serializers.
    /// </summary>
    internal class ODataJsonLightSerializer : ODataSerializer
    {
        /// <summary>
        /// The JsonLight output context to write to.
        /// </summary>
        private readonly ODataJsonLightOutputContext jsonLightOutputContext;

        /// <summary>
        /// Instance annotation writer.
        /// </summary>
        private readonly SimpleLazy<JsonLightInstanceAnnotationWriter> instanceAnnotationWriter;

        /// <summary>
        /// Set to true when odata.metadata is writen; set to false otherwise.
        /// When value is false, all URIs writen to the payload must be absolute.
        /// </summary>
        private bool allowRelativeUri;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        internal ODataJsonLightSerializer(ODataJsonLightOutputContext jsonLightOutputContext)
            : base(jsonLightOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonLightOutputContext != null, "jsonLightOutputContext != null");

            this.jsonLightOutputContext = jsonLightOutputContext;
            this.instanceAnnotationWriter = new SimpleLazy<JsonLightInstanceAnnotationWriter>(() =>
                new JsonLightInstanceAnnotationWriter(new ODataJsonLightValueSerializer(jsonLightOutputContext), jsonLightOutputContext.TypeNameOracle));
        }

        /// <summary>
        /// Returns the <see cref="ODataJsonLightOutputContext"/> which is to be used to write the content of the message.
        /// </summary>
        internal ODataJsonLightOutputContext JsonLightOutputContext
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.jsonLightOutputContext;
            }
        }

        /// <summary>
        /// Returns the <see cref="JsonWriter"/> which is to be used to write the content of the message.
        /// </summary>
        internal IJsonWriter JsonWriter
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.jsonLightOutputContext.JsonWriter;
            }
        }

        /// <summary>
        /// Instance annotation writer.
        /// </summary>
        internal JsonLightInstanceAnnotationWriter InstanceAnnotationWriter
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.instanceAnnotationWriter.Value;
            }
        }

        /// <summary>
        /// Writes the start of the entire JSON payload.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This method is an instance method for consistency with other formats.")]
        internal void WritePayloadStart()
        {
            DebugUtils.CheckNoExternalCallers();
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.JsonWriter, this.MessageWriterSettings);
        }

        /// <summary>
        /// Writes the end of the entire JSON payload.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This method is an instance method for consistency with other formats.")]
        internal void WritePayloadEnd()
        {
            DebugUtils.CheckNoExternalCallers();
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.JsonWriter, this.MessageWriterSettings);
        }

        /// <summary>
        /// Writes the metadata URI property and the specified value into the payload.
        /// </summary>
        /// <param name="metadataUri">The metadata URI to write.</param>
        internal void WriteMetadataUriProperty(Uri metadataUri)
        {
            DebugUtils.CheckNoExternalCallers();

            this.JsonWriter.WriteName(ODataAnnotationNames.ODataMetadata);
            this.JsonWriter.WritePrimitiveValue(metadataUri.AbsoluteUri, this.Version);
            this.allowRelativeUri = true;
        }

        /// <summary>
        /// Helper method to write the data wrapper around a JSON payload.
        /// </summary>
        /// <param name="payloadWriterAction">The action that writes the actual JSON payload that is being wrapped.</param>
        internal void WriteTopLevelPayload(Action payloadWriterAction)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(payloadWriterAction != null, "payloadWriterAction != null");

            this.WritePayloadStart();

            payloadWriterAction();

            this.WritePayloadEnd();
        }

        /// <summary>
        /// Write a top-level error message.
        /// </summary>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        internal void WriteTopLevelError(ODataError error, bool includeDebugInformation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(error != null, "error != null");

            this.WriteTopLevelPayload(() => ODataJsonWriterUtils.WriteError(this.JsonLightOutputContext.JsonWriter, this.InstanceAnnotationWriter.WriteInstanceAnnotations, error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth, /*writingJsonLight*/ true));
        }

        /// <summary>
        /// Returns the string representation of the URI
        /// </summary>
        /// <param name="uri">The uri to process.</param>
        /// <returns>Returns the string representation of the URI.</returns>
        internal string UriToString(Uri uri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            // Get the metadataDocumentUri directly from MessageWriterSettings and not using MetadataUriBuilder because in the case of getting the service document with nometadata 
            // MetadataUriBuilder returns null, but the metadataDocumentUri is needed to calculate Absolute Uris in the service document. In any other case jsonLightOutputContext.CreateMetadataUriBuilder() should be used.
            ODataMetadataDocumentUri odataMetadataDocumentUri = this.jsonLightOutputContext.MessageWriterSettings.MetadataDocumentUri;
            Uri metadataDocumentUri = odataMetadataDocumentUri == null ? null : odataMetadataDocumentUri.BaseUri;

            Uri resultUri;
            if (this.jsonLightOutputContext.UrlResolver != null)
            {
                // The resolver returns 'null' if no custom resolution is desired.
                resultUri = this.jsonLightOutputContext.UrlResolver.ResolveUrl(metadataDocumentUri, uri);
                if (resultUri != null)
                {
                    return UriUtilsCommon.UriToString(resultUri);
                }
            }

            resultUri = uri;
            if (!resultUri.IsAbsoluteUri)
            {
                if (!this.allowRelativeUri)
                {
                    if (metadataDocumentUri == null)
                    {
                        throw new ODataException(Strings.ODataJsonLightSerializer_RelativeUriUsedWithoutMetadataDocumentUriOrMetadata(UriUtilsCommon.UriToString(resultUri)));
                    }

                    resultUri = UriUtils.UriToAbsoluteUri(metadataDocumentUri, uri);
                }
                else
                {
                    resultUri = UriUtils.EnsureEscapedRelativeUri(resultUri);
                }
            }

            return UriUtilsCommon.UriToString(resultUri);
        }
    }
}
