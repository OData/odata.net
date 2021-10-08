//---------------------------------------------------------------------
// <copyright file="IJsonStreamReaderAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


namespace Microsoft.OData.Json
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for a class that can read arbitrary JSON and stream binary properties asynchronously.
    /// </summary>
    public interface IJsonStreamReaderAsync : IJsonReaderAsync
    {
        /// <summary>
        /// Asynchronously creates a Stream for reading a binary value.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="Stream"/> for reading the binary value.</returns>
        Task<Stream> CreateReadStreamAsync();

        /// <summary>
        /// Asynchronously creates a TextReader for reading a string value.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="TextReader"/> for reading the string value.</returns>
        Task<TextReader> CreateTextReaderAsync();
    }
}
