//---------------------------------------------------------------------
// <copyright file="DefaultStreamBasedJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1_OR_GREATER
using System;
using System.IO;
using System.Text;

namespace Microsoft.OData.Json
{

    /// <summary>
    /// Default factory for JSON writer that writes directly to a stream
    /// rather than a TextWriter.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DefaultStreamBasedJsonWriterFactory : IStreamBasedJsonWriterFactory
    {
        public IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            // TODO: only supports Utf8-encoding for now. Support for other encodings
            // to be added later.
            if (encoding.EncodingName != Encoding.UTF8.EncodingName)
            {
                return null;
            }

            return new ODataUtf8JsonWriter(stream, isIeee754Compatible);
        }
    }
}
#endif
