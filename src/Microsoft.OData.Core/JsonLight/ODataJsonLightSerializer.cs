//---------------------------------------------------------------------
// <copyright file="ODataJsonLightSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Base class for all OData JsonLight serializers.
    /// </summary>
    internal class ODataJsonLightSerializer : ODataSerializer
    {
        /// <summary>The context uri builder to use.</summary>
        protected readonly ODataContextUriBuilder ContextUriBuilder;

        /// <summary>
        /// The JsonLight output context to write to.
        /// </summary>
        private readonly ODataJsonLightOutputContext jsonLightOutputContext;

        /// <summary>
        /// Instance annotation writer.
        /// </summary>
        private readonly SimpleLazy<JsonLightInstanceAnnotationWriter> instanceAnnotationWriter;

        /// <summary>
        /// OData annotation writer.
        /// </summary>
        private readonly SimpleLazy<JsonLightODataAnnotationWriter> odataAnnotationWriter;

        /// <summary>
        /// Set to true when odata.context is writen; set to false otherwise.
        /// When value is false, all URIs writen to the payload must be absolute.
        /// </summary>
        private bool allowRelativeUri;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="initContextUriBuilder">Whether contextUriBuilder should be initialized.</param>
        internal ODataJsonLightSerializer(ODataJsonLightOutputContext jsonLightOutputContext, bool initContextUriBuilder = false)
            : base(jsonLightOutputContext)
        {
            Debug.Assert(jsonLightOutputContext != null, "jsonLightOutputContext != null");

            this.jsonLightOutputContext = jsonLightOutputContext;
            this.instanceAnnotationWriter = new SimpleLazy<JsonLightInstanceAnnotationWriter>(() =>
                new JsonLightInstanceAnnotationWriter(new ODataJsonLightValueSerializer(jsonLightOutputContext), jsonLightOutputContext.TypeNameOracle));
            this.odataAnnotationWriter = new SimpleLazy<JsonLightODataAnnotationWriter>(() =>
                new JsonLightODataAnnotationWriter(jsonLightOutputContext.JsonWriter,
                    this.JsonLightOutputContext.ODataSimplifiedOptions.EnableWritingODataAnnotationWithoutPrefix, this.MessageWriterSettings.Version));

            if (initContextUriBuilder)
            {
                // DEVNOTE: grab this early so that any validation errors are thrown at creation time rather than when Write___ is called.
                this.ContextUriBuilder = ODataContextUriBuilder.Create(
                    this.jsonLightOutputContext.MessageWriterSettings.MetadataDocumentUri,
                    this.jsonLightOutputContext.WritingResponse && !(this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel));
            }
        }

        /// <summary>
        /// Returns the <see cref="ODataJsonLightOutputContext"/> which is to be used to write the content of the message.
        /// </summary>
        internal ODataJsonLightOutputContext JsonLightOutputContext
        {
            get
            {
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
                return this.instanceAnnotationWriter.Value;
            }
        }

        /// <summary>
        /// OData annotation writer.
        /// </summary>
        internal JsonLightODataAnnotationWriter ODataAnnotationWriter
        {
            get
            {
                return this.odataAnnotationWriter.Value;
            }
        }

        /// <summary>
        /// Writes the start of the entire JSON payload.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This method is an instance method for consistency with other formats.")]
        internal void WritePayloadStart()
        {
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.JsonWriter, this.MessageWriterSettings);
        }

        /// <summary>
        /// Writes the end of the entire JSON payload.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This method is an instance method for consistency with other formats.")]
        internal void WritePayloadEnd()
        {
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.JsonWriter, this.MessageWriterSettings);
        }

        /// <summary>
        /// Writes the context URI property and the specified value into the payload.
        /// </summary>
        /// <param name="payloadKind">The ODataPayloadKind for the context URI.</param>
        /// <param name="contextUrlInfoGen">Function to generate contextUrlInfo.</param>
        /// <param name="parentContextUrlInfo">The parent contextUrlInfo.</param>
        /// <param name="propertyName">Property name to write contextUri on.</param>
        /// <returns>The contextUrlInfo, if the context URI was successfully written.</returns>
        internal ODataContextUrlInfo WriteContextUriProperty(ODataPayloadKind payloadKind, Func<ODataContextUrlInfo> contextUrlInfoGen = null, ODataContextUrlInfo parentContextUrlInfo = null, string propertyName = null)
        {
            if (this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel)
            {
                return null;
            }

            Uri contextUri = null;
            ODataContextUrlInfo contextUrlInfo = null;

            if (contextUrlInfoGen != null)
            {
                contextUrlInfo = contextUrlInfoGen();
            }

            if (contextUrlInfo != null && contextUrlInfo.IsHiddenBy(parentContextUrlInfo))
            {
                return null;
            }

            contextUri = this.ContextUriBuilder.BuildContextUri(payloadKind, contextUrlInfo);

            if (contextUri != null)
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataContext);
                }
                else
                {
                    this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataContext);
                }

                this.JsonWriter.WritePrimitiveValue(contextUri.IsAbsoluteUri ? contextUri.AbsoluteUri : contextUri.OriginalString);
                this.allowRelativeUri = true;
                return contextUrlInfo;
            }

            return null;
        }

        /// <summary>
        /// Helper method to write the data wrapper around a JSON payload.
        /// </summary>
        /// <param name="payloadWriterAction">The action that writes the actual JSON payload that is being wrapped.</param>
        internal void WriteTopLevelPayload(Action payloadWriterAction)
        {
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
            Debug.Assert(error != null, "error != null");

            this.WriteTopLevelPayload(() => ODataJsonWriterUtils.WriteError(this.JsonLightOutputContext.JsonWriter, this.InstanceAnnotationWriter.WriteInstanceAnnotationsForError, error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth, /*writingJsonLight*/ true));
        }

        /// <summary>
        /// Returns the string representation of the URI
        /// </summary>
        /// <param name="uri">The uri to process.</param>
        /// <returns>Returns the string representation of the URI.</returns>
        internal string UriToString(Uri uri)
        {
            Debug.Assert(uri != null, "uri != null");

            // Get the metadataDocumentUri directly from MessageWriterSettings and not using ContextUriBuilder because in the case of getting the service document with nometadata
            // ContextUriBuilder returns null, but the metadataDocumentUri is needed to calculate Absolute Uris in the service document. In any other case jsonLightOutputContext.CreateContextUriBuilder() should be used.
            Uri metadataDocumentUri = this.jsonLightOutputContext.MessageWriterSettings.MetadataDocumentUri;

            Uri resultUri;
            if (this.jsonLightOutputContext.PayloadUriConverter != null)
            {
                // The resolver returns 'null' if no custom resolution is desired.
                resultUri = this.jsonLightOutputContext.PayloadUriConverter.ConvertPayloadUri(metadataDocumentUri, uri);
                if (resultUri != null)
                {
                    return UriUtils.UriToString(resultUri);
                }
            }

            resultUri = uri;
            if (!resultUri.IsAbsoluteUri)
            {
                if (!this.allowRelativeUri)
                {
                    // TODO: Check if it is dead code to be removed.
                    if (metadataDocumentUri == null)
                    {
                        throw new ODataException(Strings.ODataJsonLightSerializer_RelativeUriUsedWithoutMetadataDocumentUriOrMetadata(UriUtils.UriToString(resultUri)));
                    }

                    resultUri = UriUtils.UriToAbsoluteUri(metadataDocumentUri, uri);
                }
                else
                {
                    resultUri = UriUtils.EnsureEscapedRelativeUri(resultUri);
                }
            }

            return UriUtils.UriToString(resultUri);
        }
    }
}