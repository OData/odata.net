//---------------------------------------------------------------------
// <copyright file="RandomExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class RandomExtensionMethods
    {
        /// <summary>
        /// Gets the next random value between zero and one.
        /// </summary>
        /// <param name="random">The random number generator.</param>
        /// <returns>Generated value x, such that 0.0 is less than or equal to 'x' and 'x' is less than or equal to 1.0</returns>
        public static double NextDoubleBetweenZeroAndOne(this IRandomNumberGenerator random)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");

            return (double)random.Next(int.MaxValue) / (int.MaxValue - 1);
        }

        /// <summary>
        /// Gets the next random value from the specified range.
        /// </summary>
        /// <param name="random">The random.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>Generated value x, such that minValue is less than or equal to 'x' and 'x' is less than or equal to maxValue</returns>
        public static int NextFromRange(this IRandomNumberGenerator random, int minValue, int maxValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckValidRange(minValue, "minValue", maxValue, "maxValue");

            return minValue + random.Next(maxValue - minValue + 1);
        }

        /// <summary>
        /// Gets the random value from the specified range.
        /// </summary>
        /// <param name="random">The random.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>Generated value x, such that minValue is less than or equal to 'x' and 'x' is less than or equal to maxValue</returns>
        public static double NextDoubleFromRange(this IRandomNumberGenerator random, double minValue, double maxValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckValidRange(minValue, "minValue", maxValue, "maxValue");

            return minValue + (random.NextDoubleBetweenZeroAndOne() * (maxValue - minValue));
        }

        /// <summary>
        /// Randomly chooses an element from the specified list.
        /// </summary>
        /// <typeparam name="T">List element type.</typeparam>
        /// <param name="random">The random number generator.</param>
        /// <param name="items">The list to choose from.</param>
        /// <returns>Selected item.</returns>
        public static T ChooseFrom<T>(this IRandomNumberGenerator random, IEnumerable<T> items)
        {
            T value;

            if (!random.TryChooseFrom(items, out value))
            {
                throw new TaupoArgumentException("List has no elements!");
            }

            return value;
        }

        /// <summary>
        /// Randomly chooses random number of items from the specified list. It may randomly choose 0 or all items.
        /// </summary>
        /// <typeparam name="T">List element type.</typeparam>
        /// <param name="random">The random number generator.</param>
        /// <param name="items">The list to choose from.</param>
        /// <returns>Selected items.</returns>
        public static IEnumerable<T> ChooseRandomNumberOfItemsFrom<T>(this IRandomNumberGenerator random, IEnumerable<T> items)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(items, "items");

            int maxCount = items.Count();
            if (maxCount == 0)
            {
                return Enumerable.Empty<T>();
            }

            int count = random.Next(maxCount + 1);
            return random.ChooseFrom(items, count, false);
        }

        /// <summary>
        /// Randomly chooses <paramref name="count"/> number of elements from the specified list.
        /// </summary>
        /// <typeparam name="T">List element type.</typeparam>
        /// <param name="random">The random number generator.</param>
        /// <param name="items">The list to choose from.</param>
        /// <param name="count">The count of elements to choose.</param>
        /// <param name="allowDuplicates">The value indicating if duplicates allowed or not.</param>
        /// <returns>Selected items.</returns>
        public static IEnumerable<T> ChooseFrom<T>(this IRandomNumberGenerator random, IEnumerable<T> items, int count, bool allowDuplicates)
        {
            ExceptionUtilities.CheckArgumentNotNull(items, "items");
            if (count < 0)
            {
                throw new TaupoArgumentException("Count cannot be negative.");
            }

            List<T> values = new List<T>();

            for (int i = 0; i < count; i++)
            {
                T value = random.ChooseFrom(items.Where(item => allowDuplicates || !values.Contains(item)));
                values.Add(value);
            }

            return values.AsReadOnly();
        }

        /// <summary>
        /// Randomly chooses an element from the specified list.
        /// </summary>
        /// <typeparam name="T">List element type.</typeparam>
        /// <param name="random">The random number generator.</param>
        /// <param name="items">The list to choose from.</param>
        /// <returns>Selected item.</returns>
        public static T ChooseOrDefault<T>(this IRandomNumberGenerator random, IEnumerable<T> items)
        {
            T value;

            if (!random.TryChooseFrom(items, out value))
            {
                return default(T);
            }

            return value;
        }

        /// <summary>
        /// Tries to choose a random element from a given list.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="random">The random number generator.</param>
        /// <param name="items">The items to choose from.</param>
        /// <param name="value">The output parameter which will be assigned with chosen value.</param>
        /// <returns>True if the item was selected, false otherwise.</returns>
        public static bool TryChooseFrom<T>(this IRandomNumberGenerator random, IEnumerable<T> items, out T value)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(items, "items");

            IList<T> itemList = (items as IList<T>) ?? items.ToList();

            if (itemList.Count == 0)
            {
                value = default(T);
                return false;
            }

            value = itemList[random.Next(itemList.Count)];
            return true;
        }

        /// <summary>
        /// Randomly chooses an element from the specified list where the probability of choosing each element is proportional
        /// to the specified weight.
        /// </summary>
        /// <typeparam name="T">List element type.</typeparam>
        /// <param name="random">The random number generator.</param>
        /// <param name="items">The items to choose from.</param>
        /// <param name="weights">The weights corresponding to items.</param>
        /// <returns>Selected item.</returns>
        /// <remarks>When possible, precalculate the sum of all weights and pass it expliticly to another overload of this method
        /// in order to improrve performance.</remarks>
        public static T ChooseFrom<T>(this IRandomNumberGenerator random, IEnumerable<T> items, IEnumerable<double> weights)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(items, "items");
            ExceptionUtilities.CheckArgumentNotNull(weights, "weights");

            IList<T> itemList = (items as IList<T>) ?? items.ToList();
            IList<double> weightList = (weights as IList<double>) ?? weights.ToList();

            if (itemList.Count != weightList.Count)
            {
                throw new TaupoArgumentException("Parameters 'items' and 'weights' must have the same count.");
            }

            double totalWeight = 0.0;

            for (int i = 0; i < weightList.Count; ++i)
            {
                double w = weightList[i];
                if (w < 0.0)
                {
                    throw new TaupoArgumentException("Weight #" + i + " is invalid. Must be a positive number.");
                }

                totalWeight += w;
            }

            double selector = random.NextDoubleFromRange(0, totalWeight);
            T lastItem = default(T);
            for (int i = 0; i < itemList.Count; ++i)
            {
                double w = weightList[i];

                if (selector < w)
                {
                    return itemList[i];
                }
                else
                {
                    selector -= w;
                    lastItem = itemList[i];
                }
            }

            return lastItem;
        }

        /// <summary>
        /// Shuffles the given collection by randomly changing position of every item in the list.
        /// </summary>
        /// <typeparam name="T">Type of each item.</typeparam>
        /// <param name="random">The random.</param>
        /// <param name="items">The items.</param>
        /// <returns>Shuffled collection.</returns>
        public static IEnumerable<T> Shuffle<T>(this IRandomNumberGenerator random, IEnumerable<T> items)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(items, "items");

            var temporary = new List<T>(items);
            for (int i = 0; i < temporary.Count - 1; ++i)
            {
                int targetPosition = random.NextFromRange(i + 1, temporary.Count - 1);
                T tmp = temporary[i];
                temporary[i] = temporary[targetPosition];
                temporary[targetPosition] = tmp;
            }

            return temporary.AsReadOnly();
        }

        /// <summary>
        /// Randomizes the capitalization of a string
        /// </summary>
        /// <param name="random">The random number generator to use</param>
        /// <param name="value">The string to capitalize</param>
        /// <returns>The string with random capitalization</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Need to build lowercase string")]
        public static string RandomizeCapitalization(this IRandomNumberGenerator random, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            string upper = value.ToUpperInvariant();
            string lower = value.ToLowerInvariant();
            var builder = new StringBuilder();
            for (int i = 0; i < upper.Length; i++)
            {
                builder.Append(random.Next(2) == 0 ? upper[i] : lower[i]);
            }

            return builder.ToString();
        }
    }
}
