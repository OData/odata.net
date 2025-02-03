//---------------------------------------------------------------------
// <copyright file="CountDistinctTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    public class CountDistinctTests
    {
        [Fact]
        public void CountDistinct_Enumerable()
        {
            // Arrange/Act/Assert
            Assert.Equal(6, new[] { 0, 1, 1, 2, 3, 5, 8 }.CountDistinct(d1 => d1));
        }

        [Fact]
        public void CountDistinct_Queryable()
        {
            // Arrange/Act/Assert
            Assert.Equal(6, new[] { 0, 1, 1, 2, 3, 5, 8 }.AsQueryable().CountDistinct(d1 => d1));
        }

        [Fact]
        public void CountDistinct_EnumerableSourceIsNull()
        {
            // Arrange
            List<int> source = null;

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => source.CountDistinct(d => d));
        }

        [Fact]
        public void CountDistinct_QueryableSourceIsNull()
        {
            // Arrange
            IQueryable<int> source = null;

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => source.CountDistinct(d => d));
        }
    }
}
