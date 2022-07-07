//---------------------------------------------------------------------
// <copyright file="IStreamBasedJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// The interface of factories that create <see cref="IJsonWriter"/> instances
    /// that write directly to a <see cref="Stream"/> rather than a <see cref="TextWriter"/>.
    /// </summary>
    [CLSCompliant(false)]
    public interface IStreamBasedJsonWriterFactory
    {
        /// <summary>
        /// Creates a new JSON writer of <see cref="IJsonWriter"/>.
        /// </summary>
        /// <param name="stream">Output stream to which the resulting <see cref="IJsonWriter"/> should write data.</param>
        /// <param name="isIeee754Compatible">True if it is IEEE754Compatible.</param>
        /// <param name="encoding">The text encoding of the output data.</param>
        /// <returns>The JSON writer created.</returns>
        IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding);
    }
}
