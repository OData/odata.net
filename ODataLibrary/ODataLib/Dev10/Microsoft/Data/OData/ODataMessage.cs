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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Base class for the internal wrappers around IODataRequestMessageAsync and IODataResponseMessageAsync.
    /// </summary>
    internal abstract class ODataMessage
    {
        /// <summary>
        /// Constructs a new ODataMessage.
        /// </summary>
        protected ODataMessage()
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Returns an enumerable over all the headers for this message.
        /// </summary>
        internal abstract IEnumerable<KeyValuePair<string, string>> Headers
        {
            // TODO: do we want to impose a certain order of the headers?
            get;
        }

        /// <summary>
        /// Returns a value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to get.</param>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        internal abstract string GetHeader(string headerName);

        /// <summary>
        /// Sets the value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value for the header with name <paramref name="headerName"/>.</param>
        internal abstract void SetHeader(string headerName, string headerValue);

        /// <summary>
        /// Get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Intentionally a method.")]
        internal abstract Stream GetStream();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Intentionally a method.")]
        internal abstract Task<Stream> GetStreamAsync();
#endif
    }
}
