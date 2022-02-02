//---------------------------------------------------------------------
// <copyright file="IResultsComparerProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core
{
    /// <summary>
    /// Retrieves registered <see cref="IResultsComparer"/>
    /// </summary>
    public interface IResultsComparerProvider
    {
        /// <summary>
        /// Returns an <see cref="IResultsComparer"/> that can process
        /// the specified file.
        /// </summary>
        /// <param name="filePath">The path of the file for which to retrieve a compatible <see cref="IResultsComparer"/>.</param>
        /// <returns>A <see cref="IResultsComparer"/> that can process the specified file.</returns>
        /// <remarks>An exception is thrown if no suitable <see cref="IResultsComparer"/> is found.</remarks>
        IResultsComparer GetForFile(string filePath);

        /// <summary>
        /// Return the <see cref="IResultsComparer"/> with the specified ID.
        /// </summary>
        /// <param name="comparerId">The ID of the <see cref="IResultsComparer"/> to retrieve.</param>
        /// <returns>The <see cref="IResultsComparer"/> with the specified ID.</returns>
        /// <remarks>An exception is thrown if an <see cref="IResultsComparer"/> with the specified ID cannot be found.</remarks>
        IResultsComparer GetById(string comparerId);
    }
}
