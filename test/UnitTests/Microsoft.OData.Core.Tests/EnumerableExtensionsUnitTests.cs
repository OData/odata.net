namespace Microsoft.OData.Core.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public sealed class EnumerableExtensionsUnitTests
    {
        [Fact]
        public void AverageShortsNullSource()
        {
            IEnumerable<short> data = null;

            Assert.Throws<ArgumentNullException>(() => data.Average());
        }

        [Fact]
        public void AverageShortsEmptySource()
        {
            var data = Enumerable.Empty<short>();

            Assert.Throws<InvalidOperationException>(() => data.Average());
        }

        [Fact]
        public void AverageShortsOverflow()
        {
            var source = new RepeatEnumerable<short>(short.MaxValue, (long.MaxValue / short.MaxValue) + 1);

            Assert.Throws<OverflowException>(() => source.Average());
        }

        private sealed class RepeatEnumerable<T> : IEnumerable<T>
        {
            private readonly T value;
            private readonly long repetitions;

            public RepeatEnumerable(T value, long repetitions)
            {
                this.value = value;
                this.repetitions = repetitions;
            }

            public IEnumerator<T> GetEnumerator()
            {
                for (long i = 0; i < this.repetitions; ++i)
                {
                    yield return this.value;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        [Fact]
        public void AverageShorts()
        {
            var numberOfElements = long.MaxValue / short.MaxValue;
            var numberOfConcats = numberOfElements / int.MaxValue;


            var thing = 131076;

            var data = new short[] { 1, 2, 3, 4 };

            var average = data.Average();

            Assert.Equal(2.5, average);
        }
    }
}
