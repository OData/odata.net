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
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    #endregion Namespaces
    
    /// <summary>
    /// Base class for all OData deserializers.
    /// </summary>
    internal abstract class ODataDeserializer
    {
        /// <summary>The input context to use for reading.</summary>
        private readonly ODataInputContext inputContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input context to read from.</param>
        protected ODataDeserializer(ODataInputContext inputContext)
        {
            Debug.Assert(inputContext != null, "inputContext != null");

            this.inputContext = inputContext;
        }

        /// <summary>
        /// The message reader settings.
        /// </summary>
        internal ODataMessageReaderSettings MessageReaderSettings
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.inputContext.MessageReaderSettings;
            }
        }

        /// <summary>
        /// The OData version of the input.
        /// </summary>
        internal ODataVersion Version
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.inputContext.Version;
            }
        }

        /// <summary>
        /// true if the input is a response payload; false if it's a request payload.
        /// </summary>
        internal bool ReadingResponse
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.inputContext.ReadingResponse;
            }
        }

        /// <summary>
        /// The model to use.
        /// </summary>
        internal IEdmModel Model
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.inputContext.Model;
            }
        }

        /// <summary>
        /// Method to use the custom URL resolver to resolve a base URI and a payload URI.
        /// This method returns null if not custom resolution is desired.
        /// If the method returns a non-null URL that value will be used without further validation.
        /// </summary>
        /// <param name="baseUri">The (optional) base URI to use for the resolution.</param>
        /// <param name="payloadUri">The URI read from the payload.</param>
        /// <returns>
        /// A <see cref="Uri"/> instance that reflects the custom resolution of the method arguments
        /// into a URL or null if no custom resolution is desired; in that case the default resolution is used.
        /// </returns>
        internal Uri ResolveUri(Uri baseUri, Uri payloadUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(payloadUri != null, "uri != null");

            IODataUrlResolver urlResolver = this.inputContext.UrlResolver;
            if (urlResolver != null)
            {
                return urlResolver.ResolveUrl(baseUri, payloadUri);
            }

            return null;
        }

        /// <summary>
        /// Creates a new instance of a duplicate property names checker.
        /// </summary>
        /// <returns>The newly created instance of duplicate property names checker.</returns>
        internal DuplicatePropertyNamesChecker CreateDuplicatePropertyNamesChecker()
        {
            DebugUtils.CheckNoExternalCallers();

            return this.inputContext.CreateDuplicatePropertyNamesChecker();
        }
    }
}
