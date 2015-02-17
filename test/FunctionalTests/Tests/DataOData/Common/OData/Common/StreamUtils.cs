//---------------------------------------------------------------------
// <copyright file="StreamUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.IO;
    #endregion Namespaces

    /// <summary>
    /// Helper methods to work with streams.
    /// </summary>
    public static class StreamUtils
    {
        /// <summary>Copies content from one stream into another.</summary>
        /// <param name="source">Stream to read from.</param>
        /// <param name="destination">Stream to write to.</param>
        /// <returns>The number of bytes copied from the source.</returns>
        public static long CopyStream(Stream source, Stream destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            long bytesCopied = 0;
            int bytesRead;
            byte[] buffer = new byte[64 * 1024];
            do
            {
                bytesRead = source.Read(buffer, 0, buffer.Length);
                destination.Write(buffer, 0, bytesRead);
                bytesCopied += bytesRead;
            }
            while (bytesRead > 0);

            return bytesCopied;
        }
    }
}
