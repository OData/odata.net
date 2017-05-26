//---------------------------------------------------------------------
// <copyright file="IJsonReaderFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Interface of the factory to create JSON readers.
    /// </summary>
    public interface IJsonReaderFactory
    {
        /// <summary>
        /// Creates a new JSON reader of <see cref="IJsonReader"/>.
        /// </summary>
        /// <param name="textReader">The text reader to read input characters from.</param>
        /// <param name="isIeee754Compatible">True if it is IEEE754Compatible.</param>
        /// <returns>The JSON reader created.</returns>
        IJsonReader CreateJsonReader(TextReader textReader, bool isIeee754Compatible);
    }
}