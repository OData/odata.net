//---------------------------------------------------------------------
// <copyright file="SelectExpandClauseExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.Tests.UriParser.Binders;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;
using Microsoft.OData.UriParser.Aggregation;

namespace Microsoft.OData.Tests
{
    public class SelectExpandClauseExtensionsTests
    {
        [Fact]
        public void GetCurrentLevelSelectListTestMultiSelect()
        {
            //Arrange
            var expected = new List<string>();
            expected.Add("Name");
            expected.Add("Shoe");

            //Act
            var clause = CreateSelectExpandClauseMultiSelect();
            var actual = SelectExpandClauseExtensions.GetCurrentLevelSelectList(clause);

            //Assert
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(0,expected.Except(actual).Count());
            Assert.Equal(0,actual.Except(expected).Count());
        }

        [Fact]
        public void GetCurrentLevelSelectListTestNestedSelect()
        {
            //Arrange
            var expected = new List<string>();
            expected.Add("Name");
            expected.Add("FavoriteDate");
            expected.Add("MyAddress/City");

            //Act
            var clause = CreateSelectExpandClauseNestedSelect();
            var actual = SelectExpandClauseExtensions.GetCurrentLevelSelectList(clause);

            //Assert
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(0, expected.Except(actual).Count());
            Assert.Equal(0, actual.Except(expected).Count());
        }

        [Fact]
        public void GetCurrentLevelSelectListTestAllSelected()
        {
            //Arrange
            var expected = new List<string>();
            expected.Add("*");
            
            //Act
            var clause = CreateSelectExpandClauseAllSelected();
            var actual = SelectExpandClauseExtensions.GetCurrentLevelSelectList(clause);

            //Assert
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected[0], actual[0]);            
        }

        [Fact]
        public void GetCurrentLevelSelectListTestAllSelectedWithNamespace()
        {
            //Arrange
            var expected = new List<string>();
            expected.Add("namespace.*");

            //Act
            var clause = CreateSelectExpandClauseAllSelectedWithNamespace();
            var actual = SelectExpandClauseExtensions.GetCurrentLevelSelectList(clause);

            //Assert
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected[0], actual[0]);
        }

        private SelectExpandClause CreateSelectExpandClauseMultiSelect()
        {
            ODataSelectPath personNamePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp()));
            ODataSelectPath personShoePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonShoeProp()));

            var clause = new SelectExpandClause(new List<ExpandedNavigationSelectItem>(), false);
            clause.AddToSelectedItems(new PathSelectItem(personShoePath));
            clause.AddToSelectedItems(new PathSelectItem(personNamePath));

            return clause;
        }

        private SelectExpandClause CreateSelectExpandClauseNestedSelect()
        {
            ODataSelectPath personNamePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp()));
            ODataSelectPath personShoePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonFavoriteDateProp()));
            ODataSelectPath addrPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonAddressProp()));
            ODataSelectPath cityPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetAddressCityProperty()));

            var cityClause = new SelectExpandClause(new List<ExpandedNavigationSelectItem>(), false);
            cityClause.AddToSelectedItems(new PathSelectItem(cityPath));
            var addritem = new PathSelectItem(addrPath);
            addritem.SelectAndExpand = cityClause;

            var clause = new SelectExpandClause(new List<ExpandedNavigationSelectItem>(), false);
            clause.AddToSelectedItems(new PathSelectItem(personShoePath));
            clause.AddToSelectedItems(new PathSelectItem(personNamePath));
            clause.AddToSelectedItems(addritem);

            return clause;
        }

        private SelectExpandClause CreateSelectExpandClauseAllSelected()
        {
            var clause = new SelectExpandClause(new List<ExpandedNavigationSelectItem>(), true);
            clause.AddToSelectedItems(new WildcardSelectItem()); 
            return clause;
        }

        private SelectExpandClause CreateSelectExpandClauseAllSelectedWithNamespace()
        {
            var clause = new SelectExpandClause(new List<ExpandedNavigationSelectItem>(), true);
            clause.AddToSelectedItems(new NamespaceQualifiedWildcardSelectItem("namespace"));
            return clause;
        }

    }
}
