//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core
{
    using System;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;

    /// <summary>
    /// Class provides context informatation of certain <see cref="IODataRequestMessage"/>
    /// or <see cref="IODataResponseMessage"/>
    /// </summary>
    public sealed class ODataMessageInfo
    {
        /// <summary>The parsed content type as <see cref="ODataMediaType"/>.</summary>
        public ODataMediaType MediaType { get; internal set; }

        /// <summary>The encoding specified in the charset parameter of contentType or the default encoding from MediaType.</summary>
        public Encoding Encoding { get; internal set; }

        /// <summary>The <see cref="IEdmModel"/> for the payload.</summary>
        public IEdmModel Model { get; internal set; }

        /// <summary>
        /// Whether is dealing with response.
        /// </summary>
        public bool IsResponse { get; internal set; }

        /// <summary>
        /// Function to get the message stream
        /// </summary>
        public Func<Stream> GetMessageStream { get; internal set; }

        /// <summary>
        /// The optional URL resolver to perform custom URL resolution for URLs read from the payload.
        /// </summary>
        public IODataUrlResolver UrlResolver { get; internal set; }

#if ODATALIB_ASYNC
        /// <summary>
        /// Function to get the message stream task
        /// </summary>
        public Func<Task<Stream>> GetMessageStreamAsync { get; internal set; }
#endif

        /// <summary>
        /// The payload kind for the message, currently used by <see cref="ODataRawInputContext"/> only.
        /// </summary>
        internal ODataPayloadKind PayloadKind { get; set; }
    }
}
