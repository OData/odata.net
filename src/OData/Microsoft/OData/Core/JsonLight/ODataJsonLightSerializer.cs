//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Core.Json;
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
        /// Set to true when odata.context is writen; set to false otherwise.
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
        /// <param name="contextUriGen">Function to generate context Uri.</param>
        /// <param name="condition">Condition for testing whether context uri need to be written, always write if condition is null.</param>
        /// <param name="propertyName">Property name to write contextUri on.</param>
        internal void WriteContextUriProperty(Func<Uri> contextUriGen, Func<bool> condition = null, string propertyName = null)
        {
            switch (this.jsonLightOutputContext.ContextUrlLevel)
            {
                case ODataContextUrlLevel.None:
                    return;
                case ODataContextUrlLevel.OnDemand:
                    if (condition != null && !condition())
                    {
                        return;
                    }

                    break;
            }

            Uri contextUri = null;

            if (contextUriGen != null)
            {
                contextUri = contextUriGen();
            }

            if (contextUri != null)
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    this.JsonWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataContext);
                }
                else
                {
                    this.JsonWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataContext);
                }

                this.JsonWriter.WritePrimitiveValue(contextUri.AbsoluteUri);
                this.allowRelativeUri = true;
            }
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
            if (this.jsonLightOutputContext.UrlResolver != null)
            {
                // The resolver returns 'null' if no custom resolution is desired.
                resultUri = this.jsonLightOutputContext.UrlResolver.ResolveUrl(metadataDocumentUri, uri);
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
