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
    using System.Threading.Tasks;
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
        /// Asynchronous OData annotation writer.
        /// </summary>
        private readonly SimpleLazy<JsonLightODataAnnotationWriter> asynchronousODataAnnotationWriter;

        /// <summary>
        /// Set to true when odata.context is written; set to false otherwise.
        /// When value is false, all URIs written to the payload must be absolute.
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

            // NOTE: Ideally, we should instantiate a JsonLightODataAnnotationWriter that supports EITHER synchronous OR asynchronous writing.
            // Based on the value of `jsonLightOutputContext.Synchronous`
            // However, some higher level classes expose asynchronous wrappers for synchronous methods. Asynchronous wrappers for synchronous methods
            // that depend on the instance of JsonLightODataAnnotationWriter that supports synchronous writing would break hence the reason to maintain
            // the two separate instances when asynchronous API implementation is in progress
            this.odataAnnotationWriter = new SimpleLazy<JsonLightODataAnnotationWriter>(
                () => new JsonLightODataAnnotationWriter(
                    jsonLightOutputContext.JsonWriter,
                    this.JsonLightOutputContext.OmitODataPrefix,
                    this.MessageWriterSettings.Version));
            this.asynchronousODataAnnotationWriter = new SimpleLazy<JsonLightODataAnnotationWriter>(
                () => new JsonLightODataAnnotationWriter(
                    jsonLightOutputContext.AsynchronousJsonWriter,
                    this.JsonLightOutputContext.OmitODataPrefix,
                    this.MessageWriterSettings.Version));

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
        /// Returns the <see cref="JsonWriter"/> which is to be used to write the content of the message asynchronously.
        /// </summary>
        internal IJsonWriterAsync AsynchronousJsonWriter
        {
            get
            {
                return this.jsonLightOutputContext.AsynchronousJsonWriter;
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
        /// OData annotation writer.
        /// </summary>
        internal JsonLightODataAnnotationWriter AsynchronousODataAnnotationWriter
        {
            get
            {
                return this.asynchronousODataAnnotationWriter.Value;
            }
        }

        /// <summary>
        /// Writes the start of the entire JSON payload.
        /// </summary>
        internal void WritePayloadStart()
        {
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.JsonWriter, this.MessageWriterSettings);
        }

        /// <summary>
        /// Writes the end of the entire JSON payload.
        /// </summary>
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
        internal ODataContextUrlInfo WriteContextUriProperty(
            ODataPayloadKind payloadKind,
            Func<ODataContextUrlInfo> contextUrlInfoGen = null,
            ODataContextUrlInfo parentContextUrlInfo = null,
            string propertyName = null)
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
        /// Asynchronously writes the start of the entire JSON payload.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal Task WritePayloadStartAsync()
        {
            return ODataJsonWriterUtils.StartJsonPaddingIfRequiredAsync(this.AsynchronousJsonWriter, this.MessageWriterSettings);
        }

        /// <summary>
        /// Asynchronously writes the end of the entire JSON payload.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal Task WritePayloadEndAsync()
        {
            return ODataJsonWriterUtils.EndJsonPaddingIfRequiredAsync(this.AsynchronousJsonWriter, this.MessageWriterSettings);
        }

        /// <summary>
        /// Asynchronously writes the context URI property and the specified value into the payload.
        /// </summary>
        /// <param name="payloadKind">The ODataPayloadKind for the context URI.</param>
        /// <param name="contextUrlInfoGen">Function to generate contextUrlInfo.</param>
        /// <param name="parentContextUrlInfo">The parent contextUrlInfo.</param>
        /// <param name="propertyName">Property name to write contextUri on.</param>
        /// <returns>A task that represents the asynchronous read operation. 
        /// The value of the TResult parameter contains the contextUrlInfo, 
        /// if the context URI was successfully written.</returns>
        internal async Task<ODataContextUrlInfo> WriteContextUriPropertyAsync(
            ODataPayloadKind payloadKind,
            Func<ODataContextUrlInfo> contextUrlInfoGen = null,
            ODataContextUrlInfo parentContextUrlInfo = null,
            string propertyName = null)
        {
            if (this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel)
            {
                return null;
            }

            ODataContextUrlInfo contextUrlInfo = null;

            if (contextUrlInfoGen != null)
            {
                contextUrlInfo = contextUrlInfoGen();
            }

            if (contextUrlInfo != null && contextUrlInfo.IsHiddenBy(parentContextUrlInfo))
            {
                return null;
            }

            Uri contextUri = ContextUriBuilder.BuildContextUri(payloadKind, contextUrlInfo);

            if (contextUri != null)
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    await this.AsynchronousODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataContext)
                        .ConfigureAwait(false);
                }
                else
                {
                    await this.AsynchronousODataAnnotationWriter.WritePropertyAnnotationNameAsync(propertyName, ODataAnnotationNames.ODataContext)
                        .ConfigureAwait(false);
                }

                await this.AsynchronousJsonWriter.WritePrimitiveValueAsync(contextUri.IsAbsoluteUri ? contextUri.AbsoluteUri : contextUri.OriginalString)
                    .ConfigureAwait(false);
                this.allowRelativeUri = true;

                return contextUrlInfo;
            }

            return null;
        }

        /// <summary>
        /// Asynchronously writes the data wrapper around a JSON payload.
        /// </summary>
        /// <param name="payloadWriterAction">The action that writes the actual JSON payload that is being wrapped.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal async Task WriteTopLevelPayloadAsync(Func<Task> payloadWriterFunc)
        {
            Debug.Assert(payloadWriterFunc != null, "payloadWriterAction != null");

            await this.WritePayloadStartAsync().ConfigureAwait(false);

            await payloadWriterFunc().ConfigureAwait(false);

            await this.WritePayloadEndAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes a top-level error message.
        /// </summary>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal Task WriteTopLevelErrorAsync(ODataError error, bool includeDebugInformation)
        {
            Debug.Assert(error != null, "error != null");

            return this.WriteTopLevelPayloadAsync(
                () =>
                {
                    return ODataJsonWriterUtils.WriteErrorAsync(
                        this.AsynchronousJsonWriter,
                        this.InstanceAnnotationWriter.WriteInstanceAnnotationsForErrorAsync,
                        error,
                        includeDebugInformation,
                        this.MessageWriterSettings.MessageQuotas.MaxNestingDepth);
                });
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
