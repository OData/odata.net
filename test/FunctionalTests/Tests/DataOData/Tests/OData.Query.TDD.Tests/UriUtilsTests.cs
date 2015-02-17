//---------------------------------------------------------------------
// <copyright file="UriUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for UriUtils.
    /// </summary>
    [TestClass]
    public class UriUtilsTests
    {
        [TestMethod]
        public void UriInvariantInsensitiveIsBaseOf_RelativeSecondUrlIsNotAllowed()
        {
            Action method = () => UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/One/"), new Uri("/One/Two", UriKind.Relative));
            method.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void UriInvariantInsensitiveIsBaseOf_RelativeFirstUrlIsNotAllowed()
        {
            Action method = () => UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("/One/", UriKind.Relative), new Uri("http://www.example.com/One/Two"));
            method.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void UriInvariantInsensitiveIsBaseOf_SameBaseShouldReturnTrue()
        {
            UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/One/"), new Uri("http://www.example.com/One/Two")).Should().BeTrue();
        }

        [TestMethod]
        public void UriInvariantInsensitiveIsBaseOf_IdenticalUrisShouldReturnTrue()
        {
            UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/One/"), new Uri("http://www.example.com/One/")).Should().BeTrue();
        }

        [TestMethod]
        public void UriInvariantInsensitiveIsBaseOf_ShouldBeCaseInsensitive()
        {
            UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("HTTP://WwW.ExAmPlE.cOm/OnE/"), new Uri("http://www.example.com/One/")).Should().BeTrue();
        }

        [TestMethod]
        public void UriInvariantInsensitiveIsBaseOf_ShouldIgnoreHostAndSchemeAndPort()
        {
            UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("https://different.org:1234/One/"), new Uri("http://www.example.com:4567/One/Two")).Should().BeTrue();
        }

        [TestMethod]
        public void UriInvariantInsensitiveIsBaseOf_ShouldIgnoreStuffAfterFinalSlash()
        {
            UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/OData.svc"), new Uri("http://www.example.com/One/Two")).Should().BeTrue();
        }

        [TestMethod]
        public void UriInvariantInsensitiveIsBaseOf_DifferentBaseShouldReturnFalse()
        {
            UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/OData.svc/"), new Uri("http://www.example.com/One/Two")).Should().BeFalse();
        }
    }
}
