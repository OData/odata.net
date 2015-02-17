//---------------------------------------------------------------------
// <copyright file="SimpleLazyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SimpleLazyTests
    {
        [TestMethod]
        public void SimpleLazyOfTDefaultConstructorShouldDelayConstructValue()
        {
            int factoryCallCount = 0;
            SimpleLazy<int> lazy = new SimpleLazy<int>(() =>
            {
                factoryCallCount++;
                return 1;
            });
            factoryCallCount.Should().Be(0);
            lazy.Value.Should().Be(1);
            factoryCallCount.Should().Be(1);
            lazy.Value.Should().Be(1);
            factoryCallCount.Should().Be(1);
        }

        [TestMethod]
        public void SimpleLazyOfTThreadSafeShouldDelayConstructValue()
        {
            int factoryCallCount = 0;
            SimpleLazy<string> lazy = new SimpleLazy<string>(() =>
            {
                factoryCallCount++;
                return "foo";
            },
            true /*isThreadSafe*/);
            factoryCallCount.Should().Be(0);
            lazy.Value.Should().Be("foo");
            factoryCallCount.Should().Be(1);
            lazy.Value.Should().Be("foo");
            factoryCallCount.Should().Be(1);
        }

        [TestMethod]
        public void SimpleLazyOfTNotThreadSafeShouldDelayConstructValue()
        {
            Uri abcpqr = new Uri("http://abc/pqr", UriKind.Absolute);
            int factoryCallCount = 0;
            SimpleLazy<Uri> lazy = new SimpleLazy<Uri>(() =>
            {
                factoryCallCount++;
                return abcpqr;
            }, false /*isThreadSafe*/);
            factoryCallCount.Should().Be(0);
            lazy.Value.Should().Be(abcpqr);
            factoryCallCount.Should().Be(1);
            lazy.Value.Should().Be(abcpqr);
            factoryCallCount.Should().Be(1);
        }
    }
}
