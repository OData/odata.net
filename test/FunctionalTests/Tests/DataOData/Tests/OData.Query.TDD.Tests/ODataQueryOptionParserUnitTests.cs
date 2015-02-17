//---------------------------------------------------------------------
// <copyright file="ODataQueryOptionParserUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Unit tests for ODataQueryOptionParser.
    /// </summary>
    [TestClass]
    public class ODataQueryOptionParserUnitTests
    {
        [TestMethod]
        public void DefaultParameterAliasNodesShouldBeEmtpy()
        {
            var uriParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), new Dictionary<string, string>());
            uriParser.ParameterAliasNodes.Count.Should().Be(0);
        }

        [TestMethod]
        public void NullInputQueryOptionShouldThrow()
        {
            Action action = () => new ODataQueryOptionParser(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), null);
            action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.", ComparisonMode.Substring);
        }

        [TestMethod]
        public void EmptyQueryOptionDictionaryShouldWork()
        {
            var uriParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), new Dictionary<string, string>());
            uriParser.ParseFilter().Should().BeNull();
            uriParser.ParseSelectAndExpand().Should().BeNull();
            uriParser.ParseOrderBy().Should().BeNull();
            uriParser.ParseTop().Should().Be(null);
            uriParser.ParseSkip().Should().Be(null);
            uriParser.ParseCount().Should().Be(null);
            uriParser.ParseSearch().Should().BeNull();
        }

        [TestMethod]
        public void QueryOptionWithEmptyValueShouldWork()
        {
            var uriParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), new Dictionary<string, string>()
            {
                {"$filter"  , ""},
                {"$expand"  , ""},
                {"$select"  , ""},
                {"$orderby" , ""},
                {"$top"     , ""},
                {"$skip"    , ""},
                {"$count"   , ""},
                {"$search"  , ""},
                {"$unknow"  , ""},
            });

            uriParser.ParseFilter().Should().BeNull();
            var results = uriParser.ParseSelectAndExpand();
            results.AllSelected.Should().BeTrue();
            results.SelectedItems.Should().HaveCount(0);
            uriParser.ParseOrderBy().Should().BeNull();
            Action action = () => uriParser.ParseTop();
            action.ShouldThrow<ODataException>().WithMessage(Strings.SyntacticTree_InvalidTopQueryOptionValue(""));
            action = () => uriParser.ParseSkip();
            action.ShouldThrow<ODataException>().WithMessage(Strings.SyntacticTree_InvalidSkipQueryOptionValue(""));
            action = () => uriParser.ParseCount();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataUriParser_InvalidCount(""));
            action = () => uriParser.ParseSearch();
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_ExpressionExpected(0, ""));
        }

        [TestMethod]
        public void QueryOptionWithNullValueShouldWork()
        {
            var uriParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), new Dictionary<string, string>()
            {
                {"$filter"  , null},
                {"$expand"  , null},
                {"$select"  , null},
                {"$orderby" , null},
                {"$top"     , null},
                {"$skip"    , null},
                {"$count"   , null},
                {"$search"  , null},
                {"$unknow"  , null},
            });

            uriParser.ParseFilter().Should().BeNull();
            uriParser.ParseSelectAndExpand().Should().BeNull();
            uriParser.ParseOrderBy().Should().BeNull();
            uriParser.ParseTop().Should().Be(null);
            uriParser.ParseSkip().Should().Be(null);
            uriParser.ParseCount().Should().Be(null);
            uriParser.ParseSearch().Should().BeNull();
        }
    }
}
