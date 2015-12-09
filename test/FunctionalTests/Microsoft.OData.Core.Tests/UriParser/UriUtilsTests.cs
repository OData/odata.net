//---------------------------------------------------------------------
// <copyright file="UriUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser
{
    /// <summary>
    /// Unit tests for UriUtils.
    /// </summary>
    public class UriUtilsTests
    {
        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_RelativeSecondUrlIsNotAllowed()
        {
            Action method = () => Microsoft.OData.Core.UriParser.UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/One/"), new Uri("/One/Two", UriKind.Relative));
            method.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_RelativeFirstUrlIsNotAllowed()
        {
            Action method = () => Microsoft.OData.Core.UriParser.UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("/One/", UriKind.Relative), new Uri("http://www.example.com/One/Two"));
            method.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_SameBaseShouldReturnTrue()
        {
            Microsoft.OData.Core.UriParser.UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/One/"), new Uri("http://www.example.com/One/Two")).Should().BeTrue();
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_IdenticalUrisShouldReturnTrue()
        {
            Microsoft.OData.Core.UriParser.UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/One/"), new Uri("http://www.example.com/One/")).Should().BeTrue();
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_ShouldBeCaseInsensitive()
        {
            Microsoft.OData.Core.UriParser.UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("HTTP://WwW.ExAmPlE.cOm/OnE/"), new Uri("http://www.example.com/One/")).Should().BeTrue();
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_ShouldIgnoreHostAndSchemeAndPort()
        {
            Microsoft.OData.Core.UriParser.UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("https://different.org:1234/One/"), new Uri("http://www.example.com:4567/One/Two")).Should().BeTrue();
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_ShouldIgnoreStuffAfterFinalSlash()
        {
            Microsoft.OData.Core.UriParser.UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/OData.svc"), new Uri("http://www.example.com/One/Two")).Should().BeTrue();
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_DifferentBaseShouldReturnFalse()
        {
            Microsoft.OData.Core.UriParser.UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/OData.svc/"), new Uri("http://www.example.com/One/Two")).Should().BeFalse();
        }
    }
}
