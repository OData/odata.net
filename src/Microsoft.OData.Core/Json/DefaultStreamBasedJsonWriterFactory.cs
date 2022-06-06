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

        private DefaultStreamBasedJsonWriterFactory()
        {
        }

        /// <summary>
        /// The default instance of the <see cref="DefaultStreamBasedJsonWriterFactory"/>.
        /// </summary>
        public static DefaultStreamBasedJsonWriterFactory Instance { get; } = new DefaultStreamBasedJsonWriterFactory();

        public IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
        
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            return new ODataUtf8JsonWriter(stream, isIeee754Compatible, encoding);
        }
    }
}
#endif
