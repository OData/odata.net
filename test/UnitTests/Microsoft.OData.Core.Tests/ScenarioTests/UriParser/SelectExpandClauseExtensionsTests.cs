//---------------------------------------------------------------------
// <copyright file="SelectExpandClauseExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    public class SelectExpandClauseExtensionsTests
    {
        [Fact]
        public void GetCurrentLevelSelectListTestMultiSelect()
        {
            //Arrange - $select=Shoe,Name
            var expected = new List<string>();
            expected.Add("Shoe");
            expected.Add("Name");

            //Act
            var clause = CreateSelectExpandClauseMultiSelect();
            var actual = SelectExpandClauseExtensions.GetCurrentLevelSelectList(clause);

            //Assert
            Assert.True(expected.SequenceEqual(actual));    
        }

        [Fact]
        public void GetCurrentLevelSelectListTestNestedSelect()
        {
            //Arrange - $select=FavoriteDate,Name,MyAddress/City
            var expected = new List<string>();
            expected.Add("FavoriteDate");
            expected.Add("Name");
            expected.Add("MyAddress/City");

            //Act
            var clause = CreateSelectExpandClauseNestedSelect();
            var actual = SelectExpandClauseExtensions.GetCurrentLevelSelectList(clause);

            //Assert
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact]
        public void GetCurrentLevelSelectListTestNestedSelectComplexType()
        {
            //Arrange - $select=FavoriteDate,Name,MyAddress,MyAddress/City
            var expected = new List<string>();
            expected.Add("FavoriteDate");
            expected.Add("Name");
            expected.Add("MyAddress");

            //Act
            var clause = CreateSelectExpandClauseNestedSelectWithComplexType();
            var actual = SelectExpandClauseExtensions.GetCurrentLevelSelectList(clause);

            //Assert
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact]
        public void GetCurrentLevelSelectListTestNestedSelectComplexTypeWithStar_FullSet()
        {
            //Arrange - $select=*,MyAddress/City
            var expected = new List<string>();
            expected.Add("*");
          

            //Act
            var clause = CreateSelectExpandClauseNestedSelectWithComplexTypeWithStar(false);
            var actual = SelectExpandClauseExtensions.GetCurrentLevelSelectList(clause);

            //Assert
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact]
        public void GetCurrentLevelSelectListTestNestedSelectComplexTypeWithStar_SubSet()
        {
            //Arrange - $select=*,MyAddress/City
            var expected = new List<string>();
            expected.Add("MyAddress/City");
            expected.Add("*");

            //Act
            var clause = CreateSelectExpandClauseNestedSelectWithComplexTypeWithStar(true);
            var actual = SelectExpandClauseExtensions.GetCurrentLevelSelectList(clause);

            //Assert
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact]
        public void GetCurrentLevelSelectListTestNestedSelectNestedType()
        {
            //Arrange - $select=FavoriteDate,Name,MyAddress,MyAddress/MyNeighbors/City
            var expected = new List<string>();
            expected.Add("FavoriteDate");
            expected.Add("Name");
            expected.Add("MyAddress");
            expected.Add("MyAddress/MyNeighbors/City");

            //Act
            var clause = CreateSelectExpandClauseNestedSelectWithNestedType();
            var actual = SelectExpandClauseExtensions.GetCurrentLevelSelectList(clause);

            //Assert
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact]
        public void GetCurrentLevelSelectListTestNestedSelectNestedTypeWithComplex()
        {
            //Arrange - $select=FavoriteDate,Name,MyAddress,MyAddress/MyNeighbors,MyAddress/MyNeighbors/City
            var expected = new List<string>();
            expected.Add("FavoriteDate");
            expected.Add("Name");
            expected.Add("MyAddress");

            //Act
            var clause = CreateSelectExpandClauseNestedSelectWithNestedAndComplex();
            var actual = SelectExpandClauseExtensions.GetCurrentLevelSelectList(clause);

            //Assert
            Assert.True(expected.SequenceEqual(actual));
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

        private static SelectExpandClause CreateSelectExpandClauseMultiSelect()
        {
            ODataSelectPath personNamePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp()));
            ODataSelectPath personShoePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonShoeProp()));

            var clause = new SelectExpandClause(new List<SelectItem>(), false);
            clause.AddToSelectedItems(new PathSelectItem(personShoePath));
            clause.AddToSelectedItems(new PathSelectItem(personNamePath));

            return clause;
        }

        private static SelectExpandClause CreateSelectExpandClauseNestedSelect()
        {
            ODataSelectPath personNamePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp()));
            ODataSelectPath personShoePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonFavoriteDateProp()));
            ODataSelectPath addrPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonAddressProp()));
            ODataSelectPath cityPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetAddressCityProperty()));

            var cityClause = new SelectExpandClause(new List<SelectItem>(), false);
            cityClause.AddToSelectedItems(new PathSelectItem(cityPath));
            var addritem = new PathSelectItem(addrPath);
            addritem.SelectAndExpand = cityClause;

            var clause = new SelectExpandClause(new List<SelectItem>(), false);
            clause.AddToSelectedItems(new PathSelectItem(personShoePath));
            clause.AddToSelectedItems(new PathSelectItem(personNamePath));
            clause.AddToSelectedItems(addritem);
            
            return clause;
        }


        private static SelectExpandClause CreateSelectExpandClauseNestedSelectWithComplexType()
        {
            ODataSelectPath personNamePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp()));
            ODataSelectPath personShoePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonFavoriteDateProp()));
            ODataSelectPath addrPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonAddressProp()));
            ODataSelectPath cityPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetAddressCityProperty()));

            var cityClause = new SelectExpandClause(new List<SelectItem>(), false);
            cityClause.AddToSelectedItems(new PathSelectItem(cityPath));
            var addritem = new PathSelectItem(addrPath);
            addritem.SelectAndExpand = cityClause;

            var addritem1 = new PathSelectItem(addrPath);

            var clause = new SelectExpandClause(new List<SelectItem>(), false);
            clause.AddToSelectedItems(new PathSelectItem(personShoePath));
            clause.AddToSelectedItems(new PathSelectItem(personNamePath));
            clause.AddToSelectedItems(addritem);
            clause.AddToSelectedItems(addritem1);

            return clause;
        }

        private static SelectExpandClause CreateSelectExpandClauseNestedSelectWithComplexTypeWithStar(bool subset)
        {
            ODataSelectPath personNamePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp()));
            ODataSelectPath personShoePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonFavoriteDateProp()));
            ODataSelectPath addrPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonAddressProp()));
            ODataSelectPath cityPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetAddressCityProperty()));

            var cityClause = new SelectExpandClause(new List<SelectItem>(), false);
            cityClause.AddToSelectedItems(new PathSelectItem(cityPath));
            var addritem1 = new PathSelectItem(addrPath);
            addritem1.SelectAndExpand = cityClause;

            var clause = new SelectExpandClause(new List<SelectItem>(), true);
            clause.AddToSelectedItems(new PathSelectItem(personShoePath));
            clause.AddToSelectedItems(new PathSelectItem(personNamePath));
            
            clause.AddToSelectedItems(addritem1);
            clause.AddToSelectedItems(new WildcardSelectItem(),subset);

            return clause;
        }


        private static SelectExpandClause CreateSelectExpandClauseNestedSelectWithNestedType()
        {
            ODataSelectPath personNamePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp()));
            ODataSelectPath personShoePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonFavoriteDateProp()));
            ODataSelectPath addrPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonAddressProp()));
            ODataSelectPath cityPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetAddressCityProperty()));
            ODataSelectPath neighboursPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetAddressMyNeighborsProperty()));

            var cityClause = new SelectExpandClause(new List<SelectItem>(), false);
            cityClause.AddToSelectedItems(new PathSelectItem(cityPath));
            var addritem = new PathSelectItem(addrPath);
            addritem.SelectAndExpand = cityClause;

            var addritem1 = new PathSelectItem(addrPath);

            var nClause = new SelectExpandClause(new List<SelectItem>(), false);
            var neighboursPathItem = new PathSelectItem(neighboursPath);
            neighboursPathItem.SelectAndExpand = cityClause;
            nClause.AddToSelectedItems(neighboursPathItem);
            var addritem2 = new PathSelectItem(addrPath);
            addritem2.SelectAndExpand = nClause;

            var clause = new SelectExpandClause(new List<SelectItem>(), false);
            clause.AddToSelectedItems(new PathSelectItem(personShoePath));
            clause.AddToSelectedItems(new PathSelectItem(personNamePath));
            clause.AddToSelectedItems(addritem);
            clause.AddToSelectedItems(addritem1);
            clause.AddToSelectedItems(addritem2);

            return clause;
        }


        private static SelectExpandClause CreateSelectExpandClauseNestedSelectWithNestedAndComplex()
        {
            ODataSelectPath personNamePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp()));
            ODataSelectPath personShoePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonFavoriteDateProp()));
            ODataSelectPath addrPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonAddressProp()));
            ODataSelectPath cityPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetAddressCityProperty()));
            ODataSelectPath neighboursPath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetAddressMyNeighborsProperty()));

            var cityClause = new SelectExpandClause(new List<SelectItem>(), false);
            cityClause.AddToSelectedItems(new PathSelectItem(cityPath));
            var addritem = new PathSelectItem(addrPath);
            addritem.SelectAndExpand = cityClause;


            var neighboursPathClause = new SelectExpandClause(new List<SelectItem>(), false);
            var neighboursPathItem = new PathSelectItem(neighboursPath);
            neighboursPathItem.SelectAndExpand = cityClause;
            neighboursPathClause.AddToSelectedItems(neighboursPathItem);
            var addritem1 = new PathSelectItem(addrPath);
            addritem1.SelectAndExpand = neighboursPathClause;

            var neighboursPathClause2 = new SelectExpandClause(new List<SelectItem>(), false);
            var neighboursPathItem3 = new PathSelectItem(neighboursPath);
            neighboursPathClause2.AddToSelectedItems(neighboursPathItem3);
            var addritem2 = new PathSelectItem(addrPath);
            addritem2.SelectAndExpand = neighboursPathClause2;

            var addritem3 = new PathSelectItem(addrPath);
         
            var clause = new SelectExpandClause(new List<SelectItem>(), false);
            clause.AddToSelectedItems(new PathSelectItem(personShoePath));
            clause.AddToSelectedItems(new PathSelectItem(personNamePath));
            clause.AddToSelectedItems(addritem);
            clause.AddToSelectedItems(addritem1);
            clause.AddToSelectedItems(addritem2);
            clause.AddToSelectedItems(addritem3);

            return clause;
        }


        private static SelectExpandClause CreateSelectExpandClauseAllSelected()
        {
            var clause = new SelectExpandClause(new List<SelectItem>(), true);
            clause.AddToSelectedItems(new WildcardSelectItem());
            return clause;
        }

        private static SelectExpandClause CreateSelectExpandClauseAllSelectedWithNamespace()
        {
            var clause = new SelectExpandClause(new List<SelectItem>(), true);
            clause.AddToSelectedItems(new NamespaceQualifiedWildcardSelectItem("namespace"));
            return clause;
        }

    }
}
