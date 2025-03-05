namespace Microsoft.OData
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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="source"/> contains no elements.</exception>
        /// <exception cref="OverflowException">
        /// Thrown if the sum of the elements in the sequence is larger than <see cref="Int64.MaxValue"/>
        /// </exception>
        public static double Average(this IEnumerable<short> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    // this preserves the wording the .NET uses for its overloads
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

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="Int16"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Int16"/> values to calculate the average of.</param>
        /// <returns>The average of the sequence of values, or <see langword="null"/> if the source sequence is empty or contains only values that are <see langword="null"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <see langword="null"/></exception>
        /// <exception cref="OverflowException">
        /// Thrown if the sum of the elements in the sequence is larger than <see cref="Int64.MaxValue"/>
        /// </exception>
        public static double? Average(this IEnumerable<short?> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            // the overall behavior for this overload is:
            // 1. if there are no elements, return `null`
            // 2. if there are only `null` elements, return `null`
            // 3. if there are any non-`null` elements, average the non-`null` ones, skipping the `null` ones
            using (var enumerator = source.GetEnumerator())
            {
                short? current = null;
                while (enumerator.MoveNext() && !(current = enumerator.Current).HasValue)
                {
                    current = enumerator.Current;
                }

                if (!current.HasValue)
                {
                    return null;
                }

                long sum = current.Value;
                long count = 1;
                while (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                    if (current.HasValue)
                    {
                        checked
                        {
                            sum += current.Value;
                        }

                        ++count;
                    }
                }

                return (double)sum / count;
            }
        }
    }
}
