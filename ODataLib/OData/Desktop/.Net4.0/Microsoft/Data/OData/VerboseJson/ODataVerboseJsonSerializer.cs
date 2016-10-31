//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Base class for all OData Verbose JSON serializers.
    /// </summary>
    internal class ODataVerboseJsonSerializer : ODataSerializer
    {
        /// <summary>
        /// The Verbose JSON output context to write to.
        /// </summary>
        private readonly ODataVerboseJsonOutputContext verboseJsonOutputContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verboseJsonOutputContext">The output context to write to.</param>
        internal ODataVerboseJsonSerializer(ODataVerboseJsonOutputContext verboseJsonOutputContext)
            : base(verboseJsonOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(verboseJsonOutputContext != null, "verboseJsonOutputContext != null");

            this.verboseJsonOutputContext = verboseJsonOutputContext;
        }

        /// <summary>
        /// Returns the <see cref="ODataVerboseJsonOutputContext"/> which is to be used to write the content of the message.
        /// </summary>
        internal ODataVerboseJsonOutputContext VerboseJsonOutputContext
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.verboseJsonOutputContext;
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
                return this.verboseJsonOutputContext.JsonWriter;
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

            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.JsonWriter, this.MessageWriterSettings);

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

            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.JsonWriter, this.MessageWriterSettings);
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
                () => ODataJsonWriterUtils.WriteError(this.VerboseJsonOutputContext.JsonWriter, /*writeInstanceAnnotationsDelegate*/ null, error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth, /*writingJsonLight*/ false),
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

            return ODataJsonWriterUtils.UriToUriString(this.verboseJsonOutputContext, uri, makeAbsolute);
        }
    }
}
