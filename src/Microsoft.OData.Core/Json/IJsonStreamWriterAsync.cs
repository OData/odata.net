//---------------------------------------------------------------------
// <copyright file="IJsonStreamWriterAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for writing JSON including streaming binary values.
    /// </summary>
    [CLSCompliant(false)]
    public interface IJsonStreamWriterAsync : IJsonWriterAsync
    {
        /// <summary>
        /// Asynchronously starts the stream property value scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. 
        /// The value of the TResult parameter contains the stream to write the property value to.</returns>
        Task<Stream> StartStreamValueScopeAsync();

        /// <summary>
        /// Asynchronously starts the TextWriter value scope.
        /// </summary>
        /// <param name="contentType">ContentType of the string being written.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The value of the TResult parameter contains the textwriter to write the text property value to.</returns>
        Task<TextWriter> StartTextWriterValueScopeAsync(string contentType);

        /// <summary>
        /// Asynchronously ends the current stream property value scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task EndStreamValueScopeAsync();

        /// <summary>
        /// Asynchronously ends the TextWriter value scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task EndTextWriterValueScopeAsync();
    }
}
