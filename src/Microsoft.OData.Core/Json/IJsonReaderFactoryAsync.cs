//---------------------------------------------------------------------
// <copyright file="IJsonReaderFactoryAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    using System.IO;

    /// <summary>
    /// Interface of the factory to create asynchronous JSON readers.
    /// </summary>
    public interface IJsonReaderFactoryAsync
    {
        /// <summary>
        /// Creates a new asynchronous JSON reader of <see cref="IJsonReader"/>.
        /// </summary>
        /// <param name="textReader">The text reader to read input characters from.</param>
        /// <param name="isIeee754Compatible">True if it is IEEE754Compatible.</param>
        /// <returns>The JSON reader created.</returns>
        IJsonReaderAsync CreateAsynchronousJsonReader(TextReader textReader, bool isIeee754Compatible);
    }
}
