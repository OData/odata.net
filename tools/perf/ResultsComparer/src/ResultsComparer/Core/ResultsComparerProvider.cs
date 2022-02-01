//---------------------------------------------------------------------
// <copyright file="ResultsComparerProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ResultsComparer.Core
{
    /// <summary>
    /// The default <see cref="IResultsComparerProvider"/>.
    /// </summary>
    public class ResultsComparerProvider : IResultsComparerProvider
    {
        Dictionary<string, IResultsComparer> comparers = new();
        // the list is used to maintain the order of insertion of comparers
        // because we want to test the comparers by order of priority
        // when auto-detecting suitable comparers
        List<IResultsComparer> comparersList = new();

        /// <summary>
        /// Registers the specified <see cref="IResultsComparer"/> with
        /// the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID to associate with the comparer for later retrieval.</param>
        /// <param name="comparer">The comparer to register.</param>
        public void RegisterComparer(string id, IResultsComparer comparer)
        {
            if (!comparers.TryAdd(id, comparer))
            {
                throw new Exception($"A comparer with id '{comparer}' has already been registered.");
            }

            comparersList.Add(comparer);
        }

        /// <inheritdoc/>
        public IResultsComparer GetById(string comparerId)
        {
            if (comparers.TryGetValue(comparerId, out var comparer))
            {
                return comparer;
            }

            throw new Exception($"Unknown comparer '{comparerId}'.");
        }

        /// <inheritdoc/>
        public IResultsComparer GetForFile(string filePath)
        {
            foreach (var comparer in comparersList)
            {
                if (comparer.CanReadFile(filePath))
                {
                    return comparer;
                }
            }

            throw new Exception($"No suitable comparer found for file \"{filePath}\"");
        }
    }
}
