//---------------------------------------------------------------------
// <copyright file="DefaultStreamBasedJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1_OR_GREATER
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;

namespace Microsoft.OData.Json
{

    /// <summary>
    /// Default factory for JSON writer that writes directly to a stream
    /// rather than a TextWriter.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DefaultStreamBasedJsonWriterFactory : IStreamBasedJsonWriterFactory
    {
        private readonly JavaScriptEncoder encoder = null;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultStreamBasedJsonWriterFactory"/>.
        /// </summary>
        /// <param name="encoder">The <see cref="JavaScriptEncoder"/> to use for escaping characters.</param>
        public DefaultStreamBasedJsonWriterFactory(JavaScriptEncoder encoder = null)
        {
            this.encoder = encoder;
        }

        /// <summary>
        /// The default instance of the <see cref="DefaultStreamBasedJsonWriterFactory"/>.
        /// </summary>
        public static DefaultStreamBasedJsonWriterFactory Default { get; } = new DefaultStreamBasedJsonWriterFactory();

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

            return new ODataUtf8JsonWriter(stream, isIeee754Compatible, encoding, encoder: this.encoder, leaveStreamOpen: true);
        }

        public IJsonWriterAsync CreateAsynchronousJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            return new ODataUtf8JsonWriter(stream, isIeee754Compatible, encoding, encoder: this.encoder, leaveStreamOpen: true);
        }
    }
}
#endif
