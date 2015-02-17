//---------------------------------------------------------------------
// <copyright file="BinarySerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
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
