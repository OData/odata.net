//---------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods to Stream
    /// </summary>
    internal static class StreamExtensions
    {
        /// <summary>
        /// Reads the data in the stream into a byte array.
        /// </summary>
        /// <param name="inStream">The stream to read data from.</param>
        /// <returns>The byte array containing the read data.</returns>
        public static byte[] ReadAllBytes(this Stream inStream)
        {
            Debug.Assert(inStream != null, "inStream != null");

            if (inStream is MemoryStream)
            {
                return ((MemoryStream)inStream).ToArray();
            }

            using (var memoryStream = new MemoryStream())
            {
                inStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Asyncrhonously reads the data in the stream into a byte array.
        /// </summary>
        /// <param name="inStream">The stream to read data from.</param>
        /// <returns>A task representing the asynchronous read operation.
        /// On success, the result of the task will be the byte array containing
        /// the read data.</returns>
        public static async Task<byte[]> ReadAllBytesAsync(this Stream inStream)
        {
            Debug.Assert(inStream != null, "inStream != null");

            if (inStream is MemoryStream)
            {
                return ((MemoryStream)inStream).ToArray();
            }

            using (var memoryStream = new MemoryStream())
            {
                await inStream.CopyToAsync(memoryStream).ConfigureAwait(false);
                return memoryStream.ToArray();
            }
        }
    }
}
