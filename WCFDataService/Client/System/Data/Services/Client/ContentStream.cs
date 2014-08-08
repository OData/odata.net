//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
