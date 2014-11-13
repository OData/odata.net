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
