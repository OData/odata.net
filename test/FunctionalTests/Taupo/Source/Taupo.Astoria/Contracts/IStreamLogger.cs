//---------------------------------------------------------------------
// <copyright file="IStreamLogger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    /// <summary>
    /// Contract for logging the state of a stream
    /// </summary>
    public interface IStreamLogger
    {
        /// <summary>
        /// Gets a value indicating whether the stream is closed
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// Gets a value indicating whether the stream has been read to the end
        /// </summary>
        bool IsEndOfStream { get; }

        /// <summary>
        /// Gets all the bytes that have been written
        /// </summary>
        /// <returns>All the bytes written to the stream</returns>
        byte[] GetAllBytesWritten();

        /// <summary>
        /// Gets all the bytes that have been read
        /// </summary>
        /// <returns>All the bytes read from the stream</returns>
        byte[] GetAllBytesRead();
    }
}
