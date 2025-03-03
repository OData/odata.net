namespace Microsoft.OData.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Computes the average of a sequence of <see cref="Int16"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Int16"/> values to calculate the average of.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="Exception">Thrown if <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="source"/> contains no elements.</exception>
        /// <exception cref="OverflowException">
        /// Thrown if <paramref name="source"/> contains elements whose sum is greater than <see cref="long.MaxValue"/>
        /// </exception>
        public static double Average(this IEnumerable<short> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }

                long sum = enumerator.Current;
                long count = 1;
                while (enumerator.MoveNext())
                {
                    checked
                    {
                        sum += enumerator.Current;
                    }

                    ++count;
                }

                return (double)sum / count;
            }
        }
    }
}
