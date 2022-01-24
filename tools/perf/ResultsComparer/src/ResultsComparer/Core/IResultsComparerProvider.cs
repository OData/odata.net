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
        /// <param name="filePath"></param>
        /// <returns></returns>
        IResultsComparer GetForFile(string filePath);

        /// <summary>
        /// Return the <see cref="IResultsComparer"/> with the specified ID.
        /// </summary>
        /// <param name="comparerId"></param>
        /// <returns></returns>
        IResultsComparer GetById(string comparerId);
    }
}
