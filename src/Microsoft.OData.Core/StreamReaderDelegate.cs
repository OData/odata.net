//---------------------------------------------------------------------
// <copyright file="StreamReaderDelegate.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Threading.Tasks;

    /// <summary>
    /// Delegate for reading a specified maximum of characters from the current stream
    /// into a character array, beginning at the specified offset.
    /// </summary>
    /// <param name="chars">Character array with the values between <paramref name="offset"/> and
    /// (<paramref name="offset"/> + <paramref name="maxLength"/> - 1)
    /// replaced by the characters read from the current source.</param>
    /// <param name="offset">The index of <paramref name="chars"/> at which to being writing.</param>
    /// <param name="maxLength">The maximum number of characters to read</param>
    /// <returns>The number of characters that have been read, or 0 if at the end of the stream and no data was read.
    /// The number will be less than or equal to the <paramref name="maxLength"/> parameter,
    /// depending on whether the data is available within the stream.</returns>
    internal delegate int StreamReaderDelegate(char[] chars, int offset, int maxLength);

    /// <summary>
    /// Delegate for asynchronously reading a specified maximum of characters from the current stream
    /// into a character array, beginning at the specified offset.
    /// </summary>
    /// <param name="chars">Character array with the values between <paramref name="offset"/> and
    /// (<paramref name="offset"/> + <paramref name="maxLength"/> - 1)
    /// replaced by the characters read from the current source.</param>
    /// <param name="offset">The index of <paramref name="chars"/> at which to being writing.</param>
    /// <param name="maxLength">The maximum number of characters to read</param>
    /// <returns>A task that represents the asynchronous read operation.
    /// The value of the TResult parameter contains the number of characters that have been read,
    /// or 0 if at the end of the stream and no data was read.
    /// The number will be less than or equal to the <paramref name="maxLength"/> parameter,
    /// depending on whether the data is available within the stream.</returns>
    internal delegate Task<int> AsyncStreamReaderDelegate(char[] chars, int offset, int maxLength);
}
