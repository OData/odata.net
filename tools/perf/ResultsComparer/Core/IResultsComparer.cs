//---------------------------------------------------------------------
// <copyright file="IResultComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Threading.Tasks;

namespace ResultsComparer.Core
{
    /// <summary>
    /// Can compare two files containing measurements of
    /// the same tests before and after some change.
    /// </summary>
    public interface IResultsComparer
    {
        /// <summary>
        /// Whether this comparer supports reading the specified file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<bool> CanReadFile(string path);
        /// <summary>
        /// Compares two report files and returns the results of the comparisons.
        /// </summary>
        /// <param name="basePath">The path to the measurements before the change.</param>
        /// <param name="diffPath">The path to the measurements after the change.</param>
        /// <param name="options">Options and settings used by the comparer.</param>
        /// <returns>An object summarizing the improvements and regressions detected by the comparer.</returns>
        Task<ComparerResults> CompareResults(string basePath, string diffPath, ComparerOptions options);
    }
}
