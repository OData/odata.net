namespace Microsoft.OData.Core.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="EnumerableExtensions"/>.
    /// </summary>
    public sealed class EnumerableExtensionsUnitTests
    {
        /// <summary>
        /// Asserts the correct exception is thrown when averaging a null source.
        /// </summary>
        [Fact]
        public void AverageShortsNullSource()
        {
            IEnumerable<short> data = null;

            Assert.Throws<ArgumentNullException>(() => data.Average());
        }

        /// <summary>
        /// Asserts the correct exception is thrown when averaging an empty source.
        /// </summary>
        [Fact]
        public void AverageShortsEmptySource()
        {
            var data = Enumerable.Empty<short>();

            Assert.Throws<InvalidOperationException>(() => data.Average());
        }

        /// <summary>
        /// Asserts the correct exception is thrown when averaging a source with too large of values.
        /// </summary>
        [Fact]
        public void AverageShortsOverflow()
        {
            var source = new RepeatEnumerable<short>(short.MaxValue, (long.MaxValue / short.MaxValue) + 1);

            Assert.Throws<OverflowException>(() => source.Average());
        }

        /// <summary>
        /// A <see cref="IEnumerable{T}"/> that repeats the same value.
        /// </summary>
        /// <typeparam name="T">The type of the value that is repeated.</typeparam>
        private sealed class RepeatEnumerable<T> : IEnumerable<T>
        {
            private readonly T value;
            private readonly long repetitions;

            /// <summary>
            /// Initializes a new instance of <see cref="RepeatEnumerable{T}"/>.
            /// </summary>
            /// <param name="value">The value that should be repeated</param>
            /// <param name="repetitions">The number of times to repeat the value</param>
            /// <exception cref="ArgumentOutOfRangeException">
            /// Thrown if <paramref name="repetitions"/> is a negative value
            /// </exception>
            public RepeatEnumerable(T value, long repetitions)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(repetitions);

                this.value = value;
                this.repetitions = repetitions;
            }

            /// <inheritdoc/>
            public IEnumerator<T> GetEnumerator()
            {
                for (long i = 0; i < this.repetitions; ++i)
                {
                    yield return this.value;
                }
            }

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// Asserts that the correct average is computed
        /// </summary>
        [Fact]
        public void AverageShorts()
        {
            var data = new short[] { 1, 2, 3, 4 };

            var average = data.Average();

            Assert.Equal(2.5, average);
        }
    }
}
