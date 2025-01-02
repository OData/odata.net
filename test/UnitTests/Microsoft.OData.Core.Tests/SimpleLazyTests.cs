//---------------------------------------------------------------------
// <copyright file="SimpleLazyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Tests
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

            Assert.Equal(0, factoryCallCount);
            Assert.Equal(1, lazy.Value);
            Assert.Equal(1, factoryCallCount);
            Assert.Equal(1, lazy.Value);
            Assert.Equal(1, factoryCallCount);
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

            Assert.Equal(0, factoryCallCount);
            Assert.Equal("foo", lazy.Value);
            Assert.Equal(1, factoryCallCount);
            Assert.Equal("foo", lazy.Value);
            Assert.Equal(1, factoryCallCount);
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

            Assert.Equal(0, factoryCallCount);
            Assert.Equal(abcpqr, lazy.Value);
            Assert.Equal(1, factoryCallCount);
            Assert.Equal(abcpqr, lazy.Value);
            Assert.Equal(1, factoryCallCount);
        }
    }
}
