//---------------------------------------------------------------------
// <copyright file="DefaultJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Default JSON writer factory
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DefaultJsonWriterFactory : IJsonWriterFactory
    {
        private ODataStringEscapeOption stringEscapeOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultJsonWriterFactory" />.
        /// </summary>
        public DefaultJsonWriterFactory()
            : this(ODataStringEscapeOption.EscapeNonAscii)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultJsonWriterFactory" />.
        /// </summary>
        /// <param name="stringEscapeOption">The string escape option.</param>
        public DefaultJsonWriterFactory(ODataStringEscapeOption stringEscapeOption)
        {
            this.stringEscapeOption = stringEscapeOption;
        }

        /// <summary>
        /// Creates a new JSON writer of <see cref="IJsonWriter"/>.
        /// </summary>
        /// <param name="textWriter">Writer to which text needs to be written.</param>
        /// <param name="isIeee754Compatible">True if it is IEEE754Compatible.</param>
        /// <returns>The JSON writer created.</returns>
        public IJsonWriter CreateJsonWriter(TextWriter textWriter, bool isIeee754Compatible)
        {
            return new JsonWriter(textWriter, isIeee754Compatible, this.stringEscapeOption);
        }
    }
}
