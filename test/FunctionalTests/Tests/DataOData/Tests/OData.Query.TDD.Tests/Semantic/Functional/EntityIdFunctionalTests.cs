//---------------------------------------------------------------------
// <copyright file="EntityIdFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.Functional
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class containing tests for Entity-Id identified by $id system query parameter
    /// </summary>
    [TestClass]
    public class EntityIdFunctionalTests
    {

        [TestMethod]
        public void ParseEntityId()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://example.org/"), new Uri("http://example.org/People(0)/$ref?$id=http://test.org/People(1)"));
            Assert.AreEqual(new Uri("http://test.org/People(1)"), parser.ParseEntityId().Id);
        }

        [TestMethod]
        public void ParserRelativeEntityId()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://example.org/"), new Uri("http://example.org/Customers(0)/Orders(1)/$ref?$id=../../Orders(1)"));
            Assert.AreEqual(new Uri("http://example.org/Orders(1)"), parser.ParseEntityId().Id);
        }

        [TestMethod]
        public void ParserRelativeEntityIdWithRelativeFullUri()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("Customers(0)/Orders(1)/$ref?$id=../../Orders(1)", UriKind.Relative));
            Assert.AreEqual(new Uri("Orders(1)", UriKind.Relative), parser.ParseEntityId().Id);
        }

        [TestMethod]
        public void ParserAbsoluteEntityIdWithRelativeFullUri()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("Customers(0)/Orders(1)/$ref?$id=http://test.org/People(1)", UriKind.Relative));
            Assert.AreEqual(new Uri("http://test.org/People(1)"), parser.ParseEntityId().Id);
        }
    }
}
