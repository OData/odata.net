//---------------------------------------------------------------------
// <copyright file="SemanticEqualityFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Functional tests for SemanticAst equality.
    /// </summary>
    public class SemanticEqualityFunctionalTests
    {
        [Fact]
        public void FilterClausesWithEquivalentParsedTreesAreEqual()
        {
            FilterClause left = ParseFilter("MyDog/Color eq 'Brown'");
            FilterClause right = ParseFilter("MyDog/Color eq 'Brown'");

            Assert.Equal(left, right);
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
            Assert.Equal(left.Expression, right.Expression);
            Assert.Equal(left.RangeVariable, right.RangeVariable);
        }

        [Fact]
        public void FilterClausesWithDifferentParsedTreesAreNotEqual()
        {
            FilterClause left = ParseFilter("MyDog/Color eq 'Brown'");
            FilterClause right = ParseFilter("MyDog/Color eq 'Black'");

            Assert.NotEqual(left, right);
            Assert.NotEqual(left.Expression, right.Expression);
        }

        [Fact]
        public void OrderByClausesWithEquivalentParsedTreesAreEqual()
        {
            OrderByClause left = ParseOrderBy("MyDog/Color desc,Name asc");
            OrderByClause right = ParseOrderBy("MyDog/Color desc,Name asc");

            Assert.Equal(left, right);
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
            Assert.Equal(left.ThenBy, right.ThenBy);
        }

        [Fact]
        public void SearchClausesWithEquivalentParsedTreesAreEqual()
        {
            SearchClause left = ParseSearch("NOT bike OR \"mountain bike\"");
            SearchClause right = ParseSearch("NOT bike OR \"mountain bike\"");

            Assert.Equal(left, right);
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
            Assert.Equal(left.Expression, right.Expression);
        }

        [Fact]
        public void SelectExpandClausesWithEquivalentParsedTreesAreEqual()
        {
            SelectExpandClause left = ParseSelectAndExpand(
                "Name,RelatedIDs/$count",
                "MyDog($select=Color;$filter=Color eq 'Brown';$search=dog;$compute=tolower(Color) as LowerColor)");
            SelectExpandClause right = ParseSelectAndExpand(
                "Name,RelatedIDs/$count",
                "MyDog($select=Color;$filter=Color eq 'Brown';$search=dog;$compute=tolower(Color) as LowerColor)");

            Assert.Equal(left, right);
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public void SelectExpandClausesWithDifferentParsedTreesAreNotEqual()
        {
            SelectExpandClause left = ParseSelectAndExpand("Name", "MyDog($search=dog)");
            SelectExpandClause right = ParseSelectAndExpand("Name", "MyDog($search=cat)");

            Assert.NotEqual(left, right);
        }

        private static FilterClause ParseFilter(string text)
        {
            return new ODataQueryOptionParser(
                HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(),
                HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string> { { "$filter", text } }).ParseFilter();
        }

        private static OrderByClause ParseOrderBy(string text)
        {
            return new ODataQueryOptionParser(
                HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(),
                HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string> { { "$orderby", text } }).ParseOrderBy();
        }

        private static SearchClause ParseSearch(string text)
        {
            return new ODataQueryOptionParser(
                HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(),
                HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string> { { "$search", text } }).ParseSearch();
        }

        private static SelectExpandClause ParseSelectAndExpand(string select, string expand)
        {
            return new ODataQueryOptionParser(
                HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(),
                HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string> { { "$select", select }, { "$expand", expand } }).ParseSelectAndExpand();
        }
    }
}
