namespace Microsoft.OData.Core.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Xunit;

    public sealed class QueryableExtensionsUnitTests
    {
        [Fact]
        public void AverageShortsNullSource()
        {
            IQueryable<short> queryable = null;

            Assert.Throws<ArgumentNullException>(() => queryable.Average());
        }

        [Fact]
        public void AverageShorts()
        {
            var queryable = new short[] { 1, 2, 3, 4 }.AsQueryable();

            var average = queryable.Average();

            Assert.Equal(2.5, average);
        }
    }
}
