//---------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.IO;

    /// <summary>
    /// Extension methods to Stream for .NET35.
    /// </summary>
    internal static class StreamExtensions
    {
#if !PORTABLELIB
        private static int BUFFER_SIZE = 81920;
        private static byte[] buffer = null;

        /// Extension method to copy one stream to another
        /// <param name="source">Stream to copy from</param>
        /// <param name="target">Stream to copy to</param>
        internal static void CopyTo(this Stream source, Stream target)
        {
            if (buffer == null)
            {
                buffer = new byte[BUFFER_SIZE];
            }

            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                target.Write(buffer, 0, bytesRead);
            }
        }
#endif
    }
}
