//---------------------------------------------------------------------
// <copyright file="SimpleLazyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class SimpleLazyTests
    {
        [Fact]
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

        [Fact]
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

        [Fact]
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
