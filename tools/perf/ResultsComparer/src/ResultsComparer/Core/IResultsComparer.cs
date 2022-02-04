//---------------------------------------------------------------------
// <copyright file="IResultComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core
{
    /// <summary>
    /// Can compare two files containing measurements of
    /// the same tests before and after some change.
    /// </summary>
    public interface IResultsComparer
    {
        /// <summary>
        /// The name of the comparer. Used for logging and debugging.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether this comparer supports reading the specified file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool CanReadFile(string path);

        /// <summary>
        /// Compares two report files and returns the results of the comparisons.
        /// </summary>
        /// <param name="basePath">The path to the measurements before the change.</param>
        /// <param name="diffPath">The path to the measurements after the change.</param>
        /// <param name="options">Options and settings used by the comparer.</param>
        /// <returns>An object summarizing the improvements and regressions detected by the comparer.</returns>
        ComparerResults CompareResults(string basePath, string diffPath, ComparerOptions options);
    }
}
