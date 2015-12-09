//---------------------------------------------------------------------
// <copyright file="FullUriFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.Tests.UriParser.Binders;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Test code for end-to-end parsing of the full uri.
    /// </summary>
    public class FullUriFunctionalTests
    {
        [Fact(Skip = "This test currently fails.")]
        public void ParsePathWithLinks()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People(1)/MyDog/$ref"));
            ODataUri parsedUri = parser.ParseUri();
            List<ODataPathSegment> path = parsedUri.Path.ToList();
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        [Fact(Skip = "This test currently fails.")]
        public void ParsePathWithValue()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People(1)/$value"));
            ODataUri parsedUri = parser.ParseUri();
            List<ODataPathSegment> path = parsedUri.Path.ToList();
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeValueSegment();
        }

        [Fact(Skip = "This test currently fails.")]
        public void ParseFilter()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People?$filter=Name eq 'Bob'"));
            ODataUri parsedUri = parser.ParseUri();
            var equalOperator = parsedUri.Filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            equalOperator.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            equalOperator.Right.ShouldBeConstantQueryNode("Bob");
        }

       [Fact(Skip = "This test currently fails.")]
        public void ParseOrderby()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People?$orderby=Name asc"));
            ODataUri parsedUri = parser.ParseUri();
            parsedUri.OrderBy.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            parsedUri.OrderBy.Direction.Should().Be(OrderByDirection.Ascending);
        }

        [Fact]
        public void SelectOrExpandCanOnlyBeCalledOnEntity()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People(1)/Name?$select=Name"));
            Action parseWithNonEntity = () => parser.ParseUri();
            parseWithNonEntity.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_TypeInvalidForSelectExpand("Edm.String"));
        }

        [Fact(Skip = "This test currently fails.")]
        public void ParseSelectExpand()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/Dogs?$select=Color, MyPeople&$expand=MyPeople"));
            ODataUri parsedUri = parser.ParseUri();
            parsedUri.SelectAndExpand.SelectedItems.First().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetDogColorProp())));
            var myPeopleExpand = parsedUri.SelectAndExpand.SelectedItems.Last().ShouldBeSelectedItemOfType<ExpandedNavigationSelectItem>().And;
            myPeopleExpand.PathToNavigationProperty.Single().ShouldBeNavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp());
            var myPeopleSelectExpand = myPeopleExpand.SelectAndExpand;
            myPeopleSelectExpand.AllSelected.Should().BeTrue();
            myPeopleSelectExpand.SelectedItems.Should().BeEmpty();
        }

        [Fact(Skip = "This test currently fails.")]
        public void ParseTop()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People?$top=1"));
            ODataUri parsedUri = parser.ParseUri();
            parsedUri.Top.Should().Be(1);
        }

        [Fact(Skip = "This test currently fails.")]
        public void TopOnlyValidOnEntities()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People(1)/MyAddress?$top=1"));
            Action parseWitnNonEntity = () => parser.ParseUri();
            parseWitnNonEntity.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_QueryOptionNotApplicable("$top"));
        }

        [Fact(Skip = "This test currently fails.")]
        public void ParseSkip()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People?$skip=1"));
            ODataUri parsedUri = parser.ParseUri();
            parsedUri.Skip.Should().Be(1);
        }

        [Fact(Skip = "This test currently fails.")]
        public void SkipOnlyValidOnEntities()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People(1)/MyAddress?$skip=1"));
            Action parseWitnNonEntity = () => parser.ParseUri();
            parseWitnNonEntity.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_QueryOptionNotApplicable("$skip"));
        }

        [Fact(Skip = "This test currently fails.")]
        public void ParseCount()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People?$count=false"));
            ODataUri parsedUri = parser.ParseUri();
            parsedUri.QueryCount.Should().BeFalse();
        }

        [Fact]
        public void ParseWithAllQueryOptionsWithoutAlias()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), new Uri("http://www.odata.com/OData/Dogs?$select=Color, MyPeople&$expand=MyPeople&$filter=startswith(Color, 'Blue')&$orderby=Color asc"));
            parser.ParsePath().FirstSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet());
            var myDogSelectedItems = parser.ParseSelectAndExpand().SelectedItems.ToList();
            myDogSelectedItems.Count.Should().Be(3);
            myDogSelectedItems[1].ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetDogColorProp())));
            var myPeopleExpansionSelectionItem = myDogSelectedItems[0].ShouldBeSelectedItemOfType<ExpandedNavigationSelectItem>().And;
            myPeopleExpansionSelectionItem.PathToNavigationProperty.Single().ShouldBeNavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp());
            myPeopleExpansionSelectionItem.SelectAndExpand.SelectedItems.Should().BeEmpty();
            var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("startswith").And.Parameters.ToList();
            startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp());
            startsWithArgs[1].ShouldBeConstantQueryNode("Blue");
            var orderby = parser.ParseOrderBy();
            orderby.Direction.Should().Be(OrderByDirection.Ascending);
            orderby.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp());
        }

        [Fact]
        public void ParseWithAllQueryOptionsWithAlias()
        {
            var fullUri = new Uri("http://www.odata.com/OData/People(1)/Fully.Qualified.Namespace.GetHotPeople(limit=@p1)" + "?$filter=startswith(Name, @p2)&@p1=123&@p3='Blue'&@p2=concat('is_p2',@p3)");
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

            var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("startswith").And.Parameters.ToList();
            startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            startsWithArgs[1].ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetString(true));

            // @p1
            parser.ParameterAliasNodes["@p1"].TypeReference.IsInt32().Should().BeTrue();

            // @p2
            List<QueryNode> p2Node = parser.ParameterAliasNodes["@p2"].ShouldBeSingleValueFunctionCallQueryNode("concat").And.Parameters.ToList();
            p2Node[0].ShouldBeConstantQueryNode("is_p2");
            p2Node[1].ShouldBeParameterAliasNode("@p3", EdmCoreModel.Instance.GetString(true));

            // @p3
            parser.ParameterAliasNodes["@p3"].ShouldBeConstantQueryNode("Blue");

        }
    }
}
