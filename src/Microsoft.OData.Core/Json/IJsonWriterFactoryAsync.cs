//---------------------------------------------------------------------
// <copyright file="IJsonWriterFactoryAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Interface of the factory to create asynchronous JSON writers.
    /// </summary>
    [CLSCompliant(false)]
    public interface IJsonWriterFactoryAsync
    {
        /// <summary>
        /// Creates a new JSON writer of <see cref="IJsonWriterAsync"/> with support for writing asynchronously.
        /// </summary>
        /// <param name="textWriter">Writer to which text needs to be written.</param>
        /// <param name="isIeee754Compatible">True if it is IEEE754Compatible.</param>
        /// <returns>The JSON writer created.</returns>
        IJsonWriterAsync CreateAsynchronousJsonWriter(TextWriter textWriter, bool isIeee754Compatible);
    }
}
