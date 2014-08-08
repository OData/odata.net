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

namespace System.Data.Services.Serializers
{
    using System.Diagnostics;
    using System.IO;

    /// <summary>Provides support for serializing responses in binary format.</summary>
    /// <remarks>
    /// The file histroy should show a BinaryExceptionTextWriter which is no longer used.
    /// </remarks>
    internal struct BinarySerializer
    {
        /// <summary>Stream to which output is sent.</summary>
        private readonly Stream outputStream;

        /// <summary>Initializes a new <see cref="BinarySerializer"/> for the specified stream.</summary>
        /// <param name="output">Stream to which output should be sent.</param>
        internal BinarySerializer(Stream output)
        {
            Debug.Assert(output != null, "output != null");
            this.outputStream = output;
        }

        /// <summary>Handles the complete serialization for the specified content.</summary>
        /// <param name="content">Single Content to write..</param>
        /// <remarks><paramref name="content"/> should be a byte array.</remarks>
        internal void WriteRequest(object content)
        {
            Debug.Assert(content != null, "content != null");

            // The metadata layer should only accept byte arrays as binary-serialized values.
            byte[] bytes = content as byte[];
            if (bytes == null)
            {
                bytes = (byte[])((System.Data.Linq.Binary)content).ToArray();
            }

            this.outputStream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>Handles the complete serialization for the specified stream.</summary>
        /// <param name="inputStream">Input stream to write out.</param>
        /// <param name="bufferSize">Buffer size to use during copying.</param>
        internal void WriteRequest(Stream inputStream, int bufferSize)
        {
            Debug.Assert(inputStream != null, "stream != null");
            WebUtil.CopyStream(inputStream, this.outputStream, bufferSize);
        }
    }
}
