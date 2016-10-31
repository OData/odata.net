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

namespace System.Data.Services
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Data.OData;

    /// <summary>
    /// A <see cref="IODataResponseMessage"/> implementation that is only used for writing error payloads inside already existing error payloads.
    /// In this case, want to reuse the existing stream to write the second error, and we want ODL not to write any additional headers or do anything else.
    /// So, this implementation ignores SetHeader calls and throws on various other calls.
    /// </summary>
    internal class PartiallyWrittenStreamMessage : IODataResponseMessage
    {
        /// <summary>
        /// Stream to write the error to.
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// Build a new PartiallyWrittenStreamMessage with the given stream.
        /// </summary>
        /// <param name="stream">Backing stream of the message.</param>
        public PartiallyWrittenStreamMessage(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Getting headers from this implementation is not supported.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Getting/setting the status code is not supported for this implementation.
        /// </summary>
        public int StatusCode
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Getting arbitrary header values is not supported for this implementation.
        /// </summary>
        /// <param name="headerName">The name of the header to get.</param>
        /// <returns>
        /// The value of the HTTP header, or null if no such header was present on the message.
        /// </returns>
        public string GetHeader(string headerName)
        {
            // ODL will try to get the "Preference-Applied" header, return null to indicate that it's not set.
            return null;
        }

        /// <summary>
        /// This implementation ignores calls to SetHeader since headers have already been written in the real response message.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
        public void SetHeader(string headerName, string headerValue)
        {
        }

        /// <summary>
        /// Gets the stream for writing the error message.
        /// </summary>
        /// <returns>
        /// The stream for this message.
        /// </returns>
        public Stream GetStream()
        {
            return this.stream;
        }
    }
}
