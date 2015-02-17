//---------------------------------------------------------------------
// <copyright file="LinqExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Extension methods for LINQ queries to help with some common cases.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Returns all combinations of given lengths of the given list of items.
        /// </summary>
        /// <typeparam name="T">The type of a single item.</typeparam>
        /// <param name="source">The list of items.</param>
        /// <param name="lengths">The lengths of the combinations to generate. If the list is empty all possible combinations will be returned.</param>
        /// <returns>Enumeration of arrays of items, where each array represents a single combination of items.</returns>
        public static IEnumerable<T[]> Combinations<T>(this IList<T> source, params int[] lengths)
        {
            return Combinations(source, false, lengths);
        }

        /// <summary>
        /// Returns all combinations of given lengths of the given list of items (possibly including duplicates depending on <paramref name="includeDuplicates"/>).
        /// </summary>
        /// <typeparam name="T">The type of a single item.</typeparam>
        /// <param name="source">The list of items.</param>
        /// <param name="includeDuplicates">A flag controlling whether duplicate items should be included in the cominations or not.</param>
        /// <param name="lengths">The lengths of the combinations to generate. If the list is empty all possible combinations will be returned.</param>
        /// <returns>Enumeration of arrays of items, where each array represents a single combination of items.</returns>
        public static IEnumerable<T[]> Combinations<T>(this IList<T> source, bool includeDuplicates, params int[] lengths)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            IEnumerable<int> lengthsToReturn = lengths;
            if (lengths.Length == 0)
            {
                lengthsToReturn = Enumerable.Range(0, source.Count + 1);
            }

            foreach (int length in lengthsToReturn)
            {
                ExceptionUtilities.Assert(length >= 0 && length <= source.Count, "The length must be between 0 and the number of items in the source.");
                int[] stack = new int[length];
                for (int i = 0; i < length; i++)
                {
                    stack[i] = includeDuplicates ? 0 : i;
                }

                T[] selected = new T[length];
                int total = source.Count;
                while (true)
                {
                    for (int i = 0; i < length; i++)
                    {
                        selected[i] = source[stack[i]];
                    }

                    yield return selected.ToArray();

                    int j;
                    for (j = length - 1; j >= 0; j--)
                    {
                        stack[j]++;
                        bool found = true;
                        for (int k = j; k < length; k++)
                        {
                            stack[k] = includeDuplicates ? stack[j] : stack[j] + (k - j);
                            if (stack[k] >= total)
                            {
                                found = false;
                                break;
                            }
                        }

                        if (found)
                        {
                            break;
                        }
                    }

                    if (j < 0)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns all variations of given lengths of a given list of items.
        /// </summary>
        /// <typeparam name="T">The type of a single item.</typeparam>
        /// <param name="source">The list of items.</param>
        /// <param name="lengths">The lengths of variations to generate. If this is empty all possible variations will be generated.</param>
        /// <returns>Enumeration of arrays where each array represents one variation.</returns>
        public static IEnumerable<T[]> Variations<T>(this IList<T> source, params int[] lengths)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            IEnumerable<int> lengthsToReturn = lengths;
            if (lengths.Length == 0)
            {
                lengthsToReturn = Enumerable.Range(0, source.Count + 1);
            }

            foreach (int length in lengthsToReturn)
            {
                ExceptionUtilities.Assert(length >= 0 && length <= source.Count, "The length parameter must be between 0 and the size of the items list.");

                if (length == 0)
                {
                    yield return new T[0];
                    continue;
                }

                List<int> stack = new List<int>();

                int position = 0;
                T[] selected = new T[length];
                int start = 0;
                while (true)
                {
                    bool goToNextPosition = true;
                    for (int i = start; i < source.Count; i++)
                    {
                        if (stack.Contains(i))
                        {
                            continue;
                        }

                        selected[position] = source[i];
                        if (position == (length - 1))
                        {
                            yield return selected.ToArray();
                        }
                        else
                        {
                            position++;
                            stack.Insert(0, i);
                            start = 0;
                            goToNextPosition = false;
                            break;
                        }
                    }

                    if (goToNextPosition)
                    {
                        position--;
                        if (position >= 0)
                        {
                            start = stack[0]; stack.RemoveAt(0);
                            start++;
                            continue;
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns all permutations of given list of items.
        /// </summary>
        /// <typeparam name="T">The type of a single item.</typeparam>
        /// <param name="source">The list of items.</param>
        /// <returns>Enumeration of arrays where each array represents one permutation.</returns>
        public static IEnumerable<T[]> Permutations<T>(this IList<T> source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return source.Variations(source.Count);
        }

        /// <summary>
        /// Returns all the subsets of the given lengths of a given list of items.
        /// Note that the subsets remain in the original order of the <paramref name="source"/>. The 'set' in the name is not to
        /// imply undordered subranges.
        /// </summary>
        /// <typeparam name="T">The type of a single item.</typeparam>
        /// <param name="source">The list of items.</param>
        /// <param name="lengths">The lengths of the subsets to generate. If this is empty all possible subset lengths will be generated.</param>
        /// <returns>Enumeration of arrays where each array represents one subset.</returns>
        /// <remarks>
        /// The subsets returned from this method are in the original order of the <paramref name="source"/>; the relative ordering of
        /// the items in the <paramref="source" /> is thus maintained.
        /// </remarks>
        public static IEnumerable<T[]> Subsets<T>(this IList<T> source, params int[] lengths)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            IEnumerable<int> lengthsToReturn = lengths ?? new int[0];
            if (lengths.Length == 0)
            {
                lengthsToReturn = Enumerable.Range(0, source.Count + 1);
            }

            foreach (int length in lengthsToReturn)
            {
                ExceptionUtilities.Assert(length >= 0 && length <= source.Count, "The length parameter must be between 0 and the size of the items list.");

                if (length == 0)
                {
                    yield return new T[0];
                    continue;
                }

                for (int i = 0; i <= source.Count - length; ++i)
                {
                    // take a subset of length 'length' starting at i
                    T[] subset = new T[length];
                    for (int j = 0; j < length; ++j)
                    {
                        subset[j] = source[i + j];
                    }
                    yield return subset;
                }
            }
        }

        /// <summary>
        /// Iterates through the columns of the matrix and forms all combinations of length <paramref name="lengths"/>
        /// across the items in that column. This reduces the overall complexity (= number) of combinations across all cells of the 
        /// matrix. We use this to generate properties for entity types.
        /// </summary>
        /// <typeparam name="T">The type of a single item.</typeparam>
        /// <param name="matrix">The matrix of input items.</param>
        /// <param name="lengths">The lengths of the column combinations to generate. If this is empty all possible combination lengths will be generated.</param>
        /// <returns>Enumeration of arrays where each array represents one column combination.</returns>
        /// <remarks>
        /// Not all rows in the <paramref name="matrix"/>must have the same length.
        /// </remarks>
        public static IEnumerable<T[]> ColumnCombinations<T>(this T[][] matrix, params int[] lengths)
        {
            ExceptionUtilities.CheckArgumentNotNull(matrix, "matrix");
            IEnumerable<int> lengthsToReturn = lengths ?? new int[0];
            if (lengths.Length == 0)
            {
                lengthsToReturn = Enumerable.Range(0, matrix.Length + 1);
            }

            if (lengthsToReturn.Contains(0))
            {
                yield return new T[0];
            }

            List<T> columnItems = new List<T>();
            int columnIndex = 0;
            while (true)
            {
                // loop through all the columns and collect the items for this column
                for (int i = 0; i < matrix.Length; ++i)
                {
                    if (columnIndex < matrix[i].Length)
                    {
                        columnItems.Add(matrix[i][columnIndex]);
                    }
                }

                // if we find no more items we are done
                if (columnItems.Count == 0)
                {
                    break;
                }

                int[] lengthsToGenerate = lengthsToReturn.Where(i => i > 0 && i <= columnItems.Count).ToArray();
                if (lengthsToGenerate.Length == 0)
                {
                    // we won't find any columns with enough items to even generate the minimum length
                    // combination requested; we are done.
                    break;
                }

                foreach (T[] combination in columnItems.Combinations(false, lengthsToGenerate))
                {
                    yield return combination;
                }

                columnIndex++;
                columnItems.Clear();
            }
        }

        /// <summary>
        /// Returns enumeration from a single item.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="item">The item to return.</param>
        /// <returns>Enumeration containing a single item.</returns>
        public static IEnumerable<T> FromSingle<T>(T item)
        {
            yield return item;
        }

        /// <summary>
        /// Returns the source enumeration with one more item at the end.
        /// </summary>
        /// <typeparam name="T">The type of the item in enumeration.</typeparam>
        /// <param name="source">The source enumeration.</param>
        /// <param name="item">The item to add to the end.</param>
        /// <returns>The new enumeration.</returns>
        public static IEnumerable<T> ConcatSingle<T>(this IEnumerable<T> source, T item)
        {
            return source.Concat(FromSingle(item));
        }

        /// <summary>
        /// Returns endless enumerator which loops over the items in the source.
        /// </summary>
        /// <typeparam name="T">The type of the parameter to loop over.</typeparam>
        /// <param name="source">The source enumerable to get the items from.</param>
        /// <returns>Enumerator which never ends and loops over the items in the source.</returns>
        public static IEnumerator<T> EndLessLoop<T>(this IEnumerable<T> source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            List<T> cachedSource = source.ToList();
            ExceptionUtilities.Assert(cachedSource.Count > 0, "Can't loop over empty source.");

            IEnumerator<T> enumerator = cachedSource.GetEnumerator();
            while (true)
            {
                if (!enumerator.MoveNext())
                {
                    enumerator = cachedSource.GetEnumerator();
                    enumerator.MoveNext();
                }

                yield return enumerator.Current;
            };
        }

        /// <summary>
        /// Adds an item to the dictionary and returns the modified dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key to add.</typeparam>
        /// <typeparam name="TValue">The type of the value to add.</typeparam>
        /// <param name="dictionary">The dictionary to add the new item to.</param>
        /// <param name="key">The key of the new item.</param>
        /// <param name="value">The value of the new item.</param>
        /// <returns>The modified dictionary.</returns>
        public static IDictionary<TKey, TValue> With<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            ExceptionUtilities.CheckArgumentNotNull(dictionary, "dictionary");

            dictionary.Add(key, value);
            return dictionary;
        }

        /// <summary>
        /// Remove an item from the dictionary and returns the modified dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key to remove.</typeparam>
        /// <typeparam name="TValue">The type of the value to remove.</typeparam>
        /// <param name="dictionary">The dictionary to remove the new item from.</param>
        /// <param name="key">The key of the item to remove.</param>
        /// <returns>The modified dictionary.</returns>
        public static IDictionary<TKey, TValue> Without<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            ExceptionUtilities.CheckArgumentNotNull(dictionary, "dictionary");

            dictionary.Remove(key);
            return dictionary;
        }

        /// <summary>
        /// Sets an item in the dictionary and returns the modified dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key to set.</typeparam>
        /// <typeparam name="TValue">The type of the value to set.</typeparam>
        /// <param name="dictionary">The dictionary to set the new item into.</param>
        /// <param name="key">The key of the item to set.</param>
        /// <param name="value">The value of the item to set.</param>
        /// <returns>The modified dictionary.</returns>
        public static IDictionary<TKey, TValue> Set<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            ExceptionUtilities.CheckArgumentNotNull(dictionary, "dictionary");

            dictionary[key] = value;
            return dictionary;
        }

    }
}
