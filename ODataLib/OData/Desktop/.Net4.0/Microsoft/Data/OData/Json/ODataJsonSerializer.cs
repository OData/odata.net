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

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using o = Microsoft.Data.OData;
    #endregion Namespaces

    /// <summary>
    /// Base class for all OData JSON serializers.
    /// </summary>
    internal class ODataJsonSerializer : ODataSerializer
    {
        /// <summary>
        /// The JSON output context to write to.
        /// </summary>
        private readonly ODataJsonOutputContext jsonOutputContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonOutputContext">The output context to write to.</param>
        internal ODataJsonSerializer(ODataJsonOutputContext jsonOutputContext)
            : base(jsonOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonOutputContext != null, "jsonOutputContext != null");

            this.jsonOutputContext = jsonOutputContext;
        }

        /// <summary>
        /// Returns the <see cref="JsonWriter"/> which is to be used to write the content of the message.
        /// </summary>
        internal JsonWriter JsonWriter
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.jsonOutputContext.JsonWriter;
            }
        }

        /// <summary>
        /// Writes the start of the entire JSON payload.
        /// </summary>
        internal void WritePayloadStart()
        {
            DebugUtils.CheckNoExternalCallers();
            this.WritePayloadStart(/*disableResponseWrapper*/ false);
        }

        /// <summary>
        /// Writes the start of the entire JSON payload.
        /// </summary>
        /// <param name="disableResponseWrapper">When set to true the "d" response wrapper won't be written even in responses</param>
        internal void WritePayloadStart(bool disableResponseWrapper)
        {
            DebugUtils.CheckNoExternalCallers();

            if (this.WritingResponse && !disableResponseWrapper)
            {
                // If we're writing a response payload the entire JSON should be wrapped in { "d":  } to guard against XSS attacks
                // it makes the payload a valid JSON but invalid JScript statement.
                this.JsonWriter.StartObjectScope();
                this.JsonWriter.WriteDataWrapper();
            }
        }

        /// <summary>
        /// Writes the end of the enitire JSON payload.
        /// </summary>
        internal void WritePayloadEnd()
        {
            DebugUtils.CheckNoExternalCallers();
            this.WritePayloadEnd(/*disableResponseWrapper*/ false);
        }

        /// <summary>
        /// Writes the end of the enitire JSON payload.
        /// </summary>
        /// <param name="disableResponseWrapper">When set to true the "d" response wrapper won't be written even in responses</param>
        internal void WritePayloadEnd(bool disableResponseWrapper)
        {
            DebugUtils.CheckNoExternalCallers();

            if (this.WritingResponse && !disableResponseWrapper)
            {
                // If we were writing a response payload the entire JSON is wrapped in an object scope, which we need to close here.
                this.JsonWriter.EndObjectScope();
            }
        }

        /// <summary>
        /// Helper method to write the data wrapper around a JSON payload.
        /// </summary>
        /// <param name="payloadWriterAction">The action that writes the actual JSON payload that is being wrapped.</param>
        internal void WriteTopLevelPayload(Action payloadWriterAction)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteTopLevelPayload(payloadWriterAction, /*disableResponseWrapper*/ false);
        }

        /// <summary>
        /// Helper method to write the data wrapper around a JSON payload.
        /// </summary>
        /// <param name="payloadWriterAction">The action that writes the actual JSON payload that is being wrapped.</param>
        /// <param name="disableResponseWrapper">When set to true the "d" response wrapper won't be written even in responses</param>
        internal void WriteTopLevelPayload(Action payloadWriterAction, bool disableResponseWrapper)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(payloadWriterAction != null, "payloadWriterAction != null");

            this.WritePayloadStart(disableResponseWrapper);

            payloadWriterAction();

            this.WritePayloadEnd(disableResponseWrapper);
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

            // Top-level error payloads in JSON don't use the "d" wrapper even in responses!
            this.WriteTopLevelPayload(
                () => ODataJsonWriterUtils.WriteError(this.JsonWriter, error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth),
                /*disableResponseWrapper*/ true);
        }

        /// <summary>
        /// Converts the specified URI into an absolute URI.
        /// </summary>
        /// <param name="uri">The uri to process.</param>
        /// <returns>An absolute URI which is either the specified <paramref name="uri"/> if it was absolute,
        /// or it's a combination of the BaseUri and the relative <paramref name="uri"/>.
        /// The return value is the string representation of the URI.</returns>
        /// <remarks>This method will fail if the specified <paramref name="uri"/> is relative and there's no base URI available.</remarks>
        internal string UriToAbsoluteUriString(Uri uri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");
            return this.UriToUriString(uri, /*makeAbsolute*/ true);
        }

        /// <summary>
        /// Returns the string representation of the URI; Converts the URI into an absolute URI if the <paramref name="makeAbsolute"/> parameter is set to true.
        /// </summary>
        /// <param name="uri">The uri to process.</param>
        /// <param name="makeAbsolute">true, if the URI needs to be translated into an absolute URI; false otherwise.</param>
        /// <returns>If the <paramref name="makeAbsolute"/> parameter is set to true, then a string representation of an absolute URI which is either the 
        /// specified <paramref name="uri"/> if it was absolute, or it's a combination of the BaseUri and the relative <paramref name="uri"/>; 
        /// otherwise a string representation of the specified <paramref name="uri"/>.
        /// </returns>
        /// <remarks>This method will fail if <paramref name="makeAbsolute"/> is set to true and the specified <paramref name="uri"/> is relative and there's no base URI available.</remarks>
        internal string UriToUriString(Uri uri, bool makeAbsolute)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            Uri resultUri;
            if (this.UrlResolver != null)
            {
                // The resolver returns 'null' if no custom resolution is desired.
                resultUri = this.UrlResolver.ResolveUrl(this.MessageWriterSettings.BaseUri, uri);
                if (resultUri != null)
                {
                    return UriUtilsCommon.UriToString(resultUri);
                }
            }

            resultUri = uri;

            if (!resultUri.IsAbsoluteUri)
            {
                if (makeAbsolute)
                {
                    if (this.MessageWriterSettings.BaseUri == null)
                    {
                        throw new ODataException(o.Strings.ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(UriUtilsCommon.UriToString(uri)));
                    }

                    resultUri = UriUtils.UriToAbsoluteUri(this.MessageWriterSettings.BaseUri, uri);
                }
                else
                {
                    // NOTE: the only URIs that are allowed to be relative are metadata URIs 
                    //       in operations; for such metadata URIs there is no base URI.
                    resultUri = UriUtils.EnsureEscapedRelativeUri(resultUri);
                }
            }

            return UriUtilsCommon.UriToString(resultUri);
        }
    }
}
