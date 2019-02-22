//---------------------------------------------------------------------
// <copyright file="DefaultJsonReaderFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Default factory for creating a JsonReader
    /// </summary>
    internal sealed class DefaultJsonReaderFactory : IJsonReaderFactory
    {
        /// <summary>
        /// Method to create a <see cref="IJsonReader"/>.
        /// </summary>
        /// <param name="textReader">The underlying text reader</param>
        /// <param name="isIeee754Compatible">Whether the reader returns large integers as strings</param>
        /// <returns>A new <see cref="IJsonReader"/> instance.</returns>
        public IJsonReader CreateJsonReader(TextReader textReader, bool isIeee754Compatible)
        {
            return new JsonReader(textReader, isIeee754Compatible);
        }
    }
}
