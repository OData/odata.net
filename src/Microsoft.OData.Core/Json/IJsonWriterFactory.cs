//---------------------------------------------------------------------
// <copyright file="IJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Interface of the factory to create JSON writers.
    /// </summary>
    [CLSCompliant(false)]
    public interface IJsonWriterFactory
    {
        /// <summary>
        /// Creates a new JSON writer of <see cref="IJsonWriter"/>.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="isIeee754Compatible">True if it is IEEE754Compatible.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>The JSON writer created.</returns>
        IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding);
    }
}