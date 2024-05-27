//---------------------------------------------------------------------
// <copyright file="DefaultJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// JSON writer factory
    /// </summary>
    [CLSCompliant(false)]
    public sealed class ODataJsonWriterFactory : IJsonWriterFactory
    {
        private ODataStringEscapeOption stringEscapeOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataJsonWriterFactory" />.
        /// </summary>
        public ODataJsonWriterFactory()
            : this(ODataStringEscapeOption.EscapeNonAscii)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataJsonWriterFactory" />.
        /// </summary>
        /// <param name="stringEscapeOption">The string escape option.</param>
        public ODataJsonWriterFactory(ODataStringEscapeOption stringEscapeOption)
        {
            this.stringEscapeOption = stringEscapeOption;
        }

        /// <summary>
        /// Creates a new JSON writer of <see cref="IJsonWriter"/>.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="isIeee754Compatible">True if it is IEEE754Compatible.</param>
        /// <returns>The JSON writer created.</returns>
        public IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            TextWriter textWriter = new StreamWriter(stream, encoding);

            return new JsonWriter(textWriter, isIeee754Compatible, this.stringEscapeOption);
        }
    }
}
