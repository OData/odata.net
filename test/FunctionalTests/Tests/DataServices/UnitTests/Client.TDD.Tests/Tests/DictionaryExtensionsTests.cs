//---------------------------------------------------------------------
// <copyright file="DictionaryExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DictionaryExtensionsTests
    {
        [TestMethod]
        public void FindOrAddShouldReturnExistingWithoutCreatingNew()
        {
            new Dictionary<string, string> { { "foo", "bar" } }
                .FindOrAdd("foo", () => { throw new Exception(); })
                .Should().Be("bar");
        }

        [TestMethod]
        public void FindOrAddShouldCreateNewAndAddIt()
        {
            var testSubject = new Dictionary<string, string>();
            testSubject.FindOrAdd("foo", () => "bar").Should().Be("bar");
            testSubject.Should().Contain(new KeyValuePair<string, string>("foo", "bar"));
        }
    }
}