//---------------------------------------------------------------------
// <copyright file="LinqExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LinqExtensionsTests
    {
        [TestMethod]
        public void DuplicatesShouldUseGivenComparer()
        {
            new[] {"foo", "bar", "FOO", "BAR"}
                .Duplicates(StringComparer.OrdinalIgnoreCase)
                .Should().ContainInOrder(new[] {"FOO", "BAR"});
        }

        [TestMethod]
        public void DuplicatesMayReturnMultipleEquivalentItems()
        {
            new[] { "foo", "bar", "foo", "foo", "bar"}
                .Duplicates(StringComparer.Ordinal)
                .Should().ContainInOrder(new[] {"foo", "foo", "bar"});
        }

        [TestMethod]
        public void DuplicatesShouldReturnEmptyIfAllUnique()
        {
            Enumerable.Range(0, 3).Duplicates(EqualityComparer<int>.Default).Should().BeEmpty();
        }

        [TestMethod]
        public void DuplicatesShouldReturnEmptyIfSourceIsEmpty()
        {
            Enumerable.Empty<object>().Duplicates(EqualityComparer<object>.Default).Should().BeEmpty();
        }
    }
}