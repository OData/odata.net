//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
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
