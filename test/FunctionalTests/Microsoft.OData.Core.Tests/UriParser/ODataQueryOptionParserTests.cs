//---------------------------------------------------------------------
// <copyright file="ODataQueryOptionParserUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    /// <summary>
    /// Unit tests for ODataQueryOptionParser.
    /// </summary>
    public class ODataQueryOptionParserTests
    {
        [Fact]
        public void DefaultParameterAliasNodesShouldBeEmtpy()
        {
            var uriParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), new Dictionary<string, string>());
            Assert.Empty(uriParser.ParameterAliasNodes);
        }

        [Fact]
        public void NullInputQueryOptionShouldThrow()
        {
            Action action = () => new ODataQueryOptionParser(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), null);

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("Value cannot be null.", exception.Message);
        }

        [Fact]
        public void EmptyQueryOptionDictionaryShouldWork()
        {
            var uriParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), new Dictionary<string, string>());
            Assert.Null(uriParser.ParseFilter());
            Assert.Null(uriParser.ParseSelectAndExpand());
            Assert.Null(uriParser.ParseOrderBy());
            Assert.Null(uriParser.ParseTop());
            Assert.Null(uriParser.ParseSkip());
            Assert.Null(uriParser.ParseIndex());
            Assert.Null(uriParser.ParseCount());
            Assert.Null(uriParser.ParseSearch());
            Assert.Null(uriParser.ParseCompute());
        }

        [Fact]
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
                {"$index"   , ""},
                {"$count"   , ""},
                {"$search"  , ""},
                {"$compute" , ""},
                {"$unknow"  , ""},
            });

            Assert.Null(uriParser.ParseFilter());
            var results = uriParser.ParseSelectAndExpand();
            Assert.True(results.AllSelected);
            Assert.Empty(results.SelectedItems);
            Assert.Null(uriParser.ParseOrderBy());
            Assert.Null(uriParser.ParseCompute());
            Action action = () => uriParser.ParseTop();
            action.Throws<ODataException>(Strings.SyntacticTree_InvalidTopQueryOptionValue(""));
            action = () => uriParser.ParseSkip();
            action.Throws<ODataException>(Strings.SyntacticTree_InvalidSkipQueryOptionValue(""));
            action = () => uriParser.ParseIndex();
            action.Throws<ODataException>(Strings.SyntacticTree_InvalidIndexQueryOptionValue(""));
            action = () => uriParser.ParseCount();
            action.Throws<ODataException>(Strings.ODataUriParser_InvalidCount(""));
            action = () => uriParser.ParseSearch();
            action.Throws<ODataException>(Strings.UriQueryExpressionParser_ExpressionExpected(0, ""));
        }

        [Fact]
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
                {"index"    , null},
                {"$count"   , null},
                {"$search"  , null},
                {"$compute" , null},
                {"$unknow"  , null},
            });

            Assert.Null(uriParser.ParseFilter());
            Assert.Null(uriParser.ParseSelectAndExpand());
            Assert.Null(uriParser.ParseOrderBy());
            Assert.Null(uriParser.ParseTop());
            Assert.Null(uriParser.ParseSkip());
            Assert.Null(uriParser.ParseIndex());
            Assert.Null(uriParser.ParseCount());
            Assert.Null(uriParser.ParseSearch());
            Assert.Null(uriParser.ParseCompute());
        }
    }
}
