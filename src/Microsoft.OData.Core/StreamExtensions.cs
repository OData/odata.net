//---------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Extension methods to Stream
    /// </summary>
    internal static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this Stream instream)
        {
            Debug.Assert(instream != null, "instream != null");

            if (instream is MemoryStream)
            {
                return ((MemoryStream)instream).ToArray();
            }

            using (var memoryStream = new MemoryStream())
            {
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
