//---------------------------------------------------------------------
// <copyright file="FullUriFunctionalTests.cs" company="Microsoft">
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

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Test code for end-to-end parsing of the full uri.
    /// </summary>
    public class FullUriFunctionalTests
    {
        [Fact]
        public void ParsePathWithLinks()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People(1)/MyDog/$ref"));
            ODataUri parsedUri = parser.ParseUri();
            List<ODataPathSegment> path = parsedUri.Path.ToList();
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        [Fact]
        public void ParsePathWithValue()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People(1)/$value"));
            ODataUri parsedUri = parser.ParseUri();
            List<ODataPathSegment> path = parsedUri.Path.ToList();
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeValueSegment();
        }

        [Fact]
        public void ParseFilter()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People?$filter=Name eq 'Bob'"));
            ODataUri parsedUri = parser.ParseUri();
            var equalOperator = parsedUri.Filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            equalOperator.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            equalOperator.Right.ShouldBeConstantQueryNode("Bob");
        }

        [Fact]
        public void ParseCompute()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People?$compute=tolower(Name) as Name"));
            ODataUri parsedUri = parser.ParseUri();
            parsedUri.Compute.ComputedItems.Single().Expression.ShouldBeSingleValueFunctionCallQueryNode();
        }

        [Fact]
        public void ParseOrderby()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People?$orderby=Name asc"));
            ODataUri parsedUri = parser.ParseUri();
            parsedUri.OrderBy.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            Assert.Equal(OrderByDirection.Ascending, parsedUri.OrderBy.Direction);
        }

        [Fact]
        public void ParseOrderbyWithBuiltInFunction()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People?$orderby=tolower(Name)"));
            ODataUri parsedUri = parser.ParseUri();
            SingleValueFunctionCallNode funcCall = parsedUri.OrderBy.Expression.ShouldBeSingleValueFunctionCallQueryNode("tolower", EdmCoreModel.Instance.GetString(true));
            QueryNode parameter = Assert.Single(funcCall.Parameters);
            parameter.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            Assert.Equal(OrderByDirection.Ascending, parsedUri.OrderBy.Direction);
        }

        [Fact]
        public void SelectOrExpandCanOnlyBeCalledOnEntity()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People(1)/Name?$select=Name"));
            Action parseWithNonEntity = () => parser.ParseUri();
            parseWithNonEntity.Throws<ODataException>(ODataErrorStrings.UriParser_TypeInvalidForSelectExpand("Edm.String"));
        }

        [Fact]
        public void ParseSelectExpand()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/Dogs?$select=Color, MyPeople&$expand=MyPeople"));
            ODataUri parsedUri = parser.ParseUri();
            Assert.Equal(3, parsedUri.SelectAndExpand.SelectedItems.Count());
            parsedUri.SelectAndExpand.SelectedItems.ElementAt(2).ShouldBePathSelectionItem(
                new ODataSelectPath(
                    new NavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp(), HardCodedTestModel.GetPeopleSet())));

            parsedUri.SelectAndExpand.SelectedItems.ElementAt(1).ShouldBePathSelectionItem(
                new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetDogColorProp())));

            var myPeopleExpand = parsedUri.SelectAndExpand.SelectedItems.First()
                .ShouldBeSelectedItemOfType<ExpandedNavigationSelectItem>();

            myPeopleExpand.PathToNavigationProperty.Single()
                .ShouldBeNavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp());

            var myPeopleSelectExpand = myPeopleExpand.SelectAndExpand;
            Assert.True(myPeopleSelectExpand.AllSelected);
            Assert.Empty(myPeopleSelectExpand.SelectedItems);
        }

        [Fact]
        public void ParseTop()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People?$top=1"));
            ODataUri parsedUri = parser.ParseUri();
            Assert.Equal(1, parsedUri.Top);
        }

        [Fact]
        public void ParseTopOnComplex()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People(1)/MyAddress?$top=1"));
            ODataUri parsedUri = parser.ParseUri();
            Assert.Equal(1, parsedUri.Top);

            // This test was originally written to expect an error but there currently
            // is no error in this case.
            //Action parseWitnNonEntity = () => parser.ParseUri();
            //parseWitnNonEntity.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_QueryOptionNotApplicable("$top"));
        }

        [Fact]
        public void ParseSkip()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People?$skip=1"));
            ODataUri parsedUri = parser.ParseUri();
            Assert.Equal(1, parsedUri.Skip);
        }

        [Fact]
        public void ParseSkipOnComplex()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People(1)/MyAddress?$skip=1"));
            ODataUri parsedUri = parser.ParseUri();
            Assert.Equal(1, parsedUri.Skip);

            // This test was originally written to expect an error but there currently
            // is no error in this case.
            //Action parseWitnNonEntity = () => parser.ParseUri();
            //parseWitnNonEntity.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_QueryOptionNotApplicable("$skip"));
        }

        [Fact]
        public void ParseIndex()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People(1)/RelatedIDs?$index=1"));
            ODataUri parsedUri = parser.ParseUri();
            Assert.Equal(1, parsedUri.Index);
        }

        [Fact]
        public void ParseCount()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People?$count=false"));
            ODataUri parsedUri = parser.ParseUri();
            Assert.False(parsedUri.QueryCount);
        }

       [Fact]
        public void ParseWithAllQueryOptionsWithoutAlias()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), new Uri("http://www.odata.com/OData/Dogs?$select=Color, MyPeople&$expand=MyPeople&$filter=startswith(Color, 'Blue')&$orderby=Color asc"));
            parser.ParsePath().FirstSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet());
            var myDogSelectedItems = parser.ParseSelectAndExpand().SelectedItems.ToList();
            Assert.Equal(3, myDogSelectedItems.Count);
            myDogSelectedItems[1].ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetDogColorProp())));
            var myPeopleExpansionSelectionItem = myDogSelectedItems[0].ShouldBeSelectedItemOfType<ExpandedNavigationSelectItem>();
            myPeopleExpansionSelectionItem.PathToNavigationProperty.Single().ShouldBeNavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp());
            Assert.Empty(myPeopleExpansionSelectionItem.SelectAndExpand.SelectedItems);
            var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("startswith").Parameters.ToList();
            startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp());
            startsWithArgs[1].ShouldBeConstantQueryNode("Blue");
            var orderby = parser.ParseOrderBy();
            Assert.Equal(OrderByDirection.Ascending, orderby.Direction);
            orderby.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp());
        }

        [Fact]
        public void ParseWithAllQueryOptionsWithAlias()
        {
            var fullUri = new Uri("http://www.odata.com/OData/People(1)/Fully.Qualified.Namespace.GetHotPeople(limit=@p1)" + "?$filter=startswith(Name, @p2)&@p1=123&@p3='Blue'&@p2=concat('is_p2',@p3)");
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

            var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("startswith").Parameters.ToList();
            startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            startsWithArgs[1].ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetString(true));

            // @p1
            Assert.True(parser.ParameterAliasNodes["@p1"].TypeReference.IsInt32());

            // @p2
            List<QueryNode> p2Node = parser.ParameterAliasNodes["@p2"].ShouldBeSingleValueFunctionCallQueryNode("concat").Parameters.ToList();
            p2Node[0].ShouldBeConstantQueryNode("is_p2");
            p2Node[1].ShouldBeParameterAliasNode("@p3", EdmCoreModel.Instance.GetString(true));

            // @p3
            parser.ParameterAliasNodes["@p3"].ShouldBeConstantQueryNode("Blue");

        }
    }
}
