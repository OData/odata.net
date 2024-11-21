//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Microsoft.OData.Json
{

    /// <summary>
    /// Default factory for JSON writer that uses <see cref="Utf8JsonWriter"/> internally.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class ODataUtf8JsonWriterFactory : IJsonWriterFactory
    {
        private readonly JavaScriptEncoder encoder = null;

        /// <summary>
        /// Creates a new instance of <see cref="ODataUtf8JsonWriterFactory"/>.
        /// </summary>
        /// <param name="encoder">The <see cref="JavaScriptEncoder"/> to use for escaping characters.</param>
        public ODataUtf8JsonWriterFactory(JavaScriptEncoder encoder = null)
        {
            this.encoder = encoder;
        }

        /// <summary>
        /// The default instance of the <see cref="ODataUtf8JsonWriterFactory"/>.
        /// </summary>
        public static ODataUtf8JsonWriterFactory Default { get; } = new ODataUtf8JsonWriterFactory();

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
    }
}
