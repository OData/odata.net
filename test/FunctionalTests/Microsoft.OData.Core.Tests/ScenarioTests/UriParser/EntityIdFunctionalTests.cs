//---------------------------------------------------------------------
// <copyright file="EntityIdFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Class containing tests for Entity-Id identified by $id system query parameter
    /// </summary>
    public class EntityIdFunctionalTests
    {
        [Fact]
        public void ParseEntityId()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://example.org/"), new Uri("http://example.org/People(0)/$ref?$id=http://test.org/People(1)"));
            Assert.Equal(new Uri("http://test.org/People(1)"), parser.ParseEntityId().Id);

            // The ParseEntityId() function first checks for the initialization of the object's entityIdSegment.
            // This second call is to simply hit the case where initialization has already taken place.
            Assert.Equal(new Uri("http://test.org/People(1)"), parser.ParseEntityId().Id);
        }

        [Fact]
        public void ParserRelativeEntityId()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://example.org/"), new Uri("http://example.org/Customers(0)/Orders(1)/$ref?$id=../../Orders(1)"));
            Assert.Equal(new Uri("http://example.org/Orders(1)"), parser.ParseEntityId().Id);
        }

        [Fact]
        public void ParserRelativeEntityIdWithRelativeFullUri()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("Customers(0)/Orders(1)/$ref?$id=../../Orders(1)", UriKind.Relative));
            Assert.Equal(new Uri("Orders(1)", UriKind.Relative), parser.ParseEntityId().Id);
        }

        [Fact]
        public void ParserAbsoluteEntityIdWithRelativeFullUri()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("Customers(0)/Orders(1)/$ref?$id=http://test.org/People(1)", UriKind.Relative));
            Assert.Equal(new Uri("http://test.org/People(1)"), parser.ParseEntityId().Id);
        }
    }
}
