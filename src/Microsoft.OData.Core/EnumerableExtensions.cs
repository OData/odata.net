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
        /// Thrown if <paramref name="source"/> contains elements whose sum is greater than <see cref="long.MaxValue"/>
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

        public static double? Average(this IEnumerable<short?> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            //// TODO the logic is: if there are no elements, return null; if there are only null elements, return null; if there are any non-null elements, average the non-null ones, skipping the null ones

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
