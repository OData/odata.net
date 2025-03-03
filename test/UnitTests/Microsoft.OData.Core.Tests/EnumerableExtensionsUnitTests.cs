namespace Microsoft.OData.Core.Tests
{
    using System;
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
        public void AverageShorts()
        {
            var data = new short[] { 1, 2, 3, 4 };

            var average = data.Average();

            Assert.Equal(2.5, average);
        }
    }
}
