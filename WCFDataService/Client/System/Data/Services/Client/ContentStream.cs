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

namespace System.Data.Services.Client
{
    using System.IO;

    /// <summary>
    /// Helper class to wrap the stream with the content of the request.
    /// We need to remember if the stream came from us (IsKnownMemoryStream is true)
    /// or if it came from outside. For backward compatibility we set the Content-Length for our streams
    /// since they are always MemoryStream and thus know their length.
    /// For outside streams (content of the MR requests) we don't set Content-Length since the stream
    /// might not be able to answer to the Length call.
    /// </summary>
    internal sealed class ContentStream
    {
        /// <summary>
        /// The stream with the content of the request
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// Set to true if the stream is a MemoryStream and we produced it (so it does have the buffer accesible)
        /// </summary>
        private readonly bool isKnownMemoryStream;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">The stream with the request content</param>
        /// <param name="isKnownMemoryStream">The stream was create by us and it's a MemoryStream</param>
        public ContentStream(Stream stream, bool isKnownMemoryStream)
        {
            this.stream = stream;
            this.isKnownMemoryStream = isKnownMemoryStream;
        }

        /// <summary>
        /// The stream with the content of the request
        /// </summary>
        public Stream Stream
        {
            get { return this.stream; }
        }

        /// <summary>
        /// Set to true if the stream is a MemoryStream and we produced it (so it does have the buffer accesible)
        /// </summary>
        public bool IsKnownMemoryStream
        {
            get { return this.isKnownMemoryStream; }
        }
    }
}
