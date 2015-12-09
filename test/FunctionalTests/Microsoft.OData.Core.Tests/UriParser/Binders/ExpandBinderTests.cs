//---------------------------------------------------------------------
// <copyright file="ExpandBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    public class ExpandBinderTests
    {
        private readonly ODataUriParserConfiguration V4configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);
        private readonly SelectExpandBinder binderForPerson;
        private readonly SelectExpandBinder binderForAddress;

        public ExpandBinderTests()
        {
            this.binderForPerson = new SelectExpandBinder(this.V4configuration, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            this.binderForAddress = new SelectExpandBinder(this.V4configuration, HardCodedTestModel.GetAddressType(), null);
        }

        [Fact]
        public void TopLevelEntityTypeCannotBeNull()
        {
            Action createWithNullTopLevelEntityType =
                () => new SelectExpandBinder(this.V4configuration, null, HardCodedTestModel.GetPeopleSet());
            createWithNullTopLevelEntityType.ShouldThrow<Exception>(Error.ArgumentNull("topLevelEntityType").ToString());
        }

        [Fact]
        public void TopLevelEntitySetCanBeNull()
        {
            SelectExpandBinder binder = new SelectExpandBinder(this.V4configuration, ModelBuildingHelpers.BuildValidEntityType(), null);
            binder.NavigationSource.Should().BeNull();
        }

        [Fact]
        public void TopLevelEntityTypeIsSetCorrectly()
        {
            this.binderForPerson.EdmType.Should().Be(HardCodedTestModel.GetPersonType());
        }

        [Fact]
        public void ModelIsSetCorrectly()
        {
            this.binderForPerson.Model.Should().Be(HardCodedTestModel.TestModel);
        }

        // $expand=MyDog($expand=MyPeople;)
        [Fact]
        public void BindingOnTreeWithWithMultipleNavPropPathsThrows()
        {
            NonSystemToken topLevelSegment = new NonSystemToken("MyDog", null, null);
            NonSystemToken navProp = new NonSystemToken("MyPeople", null, topLevelSegment);
            ExpandTermToken expandTerm = new ExpandTermToken(navProp);
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] {expandTerm});
            Action bindTreeWithMultipleNavPropPaths = () => this.binderForPerson.Bind(BuildUnifiedSelectExpandToken(expandToken));
            bindTreeWithMultipleNavPropPaths.ShouldThrow<ODataException>()
                                .WithMessage(ODataErrorStrings.ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath);
        }

        // $expand=MyDog
        [Fact]
        public void SingleLevelExpandTermTokenWorks()
        {
            ExpandTermToken expandTermToken = new ExpandTermToken(new NonSystemToken("MyDog", null, null));
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] {expandTermToken});
            var item = this.binderForPerson.Bind(BuildUnifiedSelectExpandToken(expandToken));
            item.SelectedItems.First().ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp()).And.SelectAndExpand.AllSelected.Should().BeTrue();
        }

        // $expand=MyDog($expand=MyPeople;)
        [Fact]
        public void MultiLevelExpandTermTokenWorks()
        {
            ExpandTermToken innerExpandTerm = new ExpandTermToken(new NonSystemToken("MyPeople", null, null));
            ExpandTermToken outerExpandTerm = new ExpandTermToken(new NonSystemToken("MyDog", null, null), null,
                                                                  new ExpandToken(new ExpandTermToken[]
                                                                      {innerExpandTerm}));
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] {outerExpandTerm});
            var item = this.binderForPerson.Bind(BuildUnifiedSelectExpandToken(expandToken));
            var subSelectExpand = item.SelectedItems.First().ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp()).And.SelectAndExpand;
            subSelectExpand.AllSelected.Should().BeTrue();
            subSelectExpand.SelectedItems.First().ShouldBeExpansionFor(HardCodedTestModel.GetDogMyPeopleNavProp())
                .And.SelectAndExpand.AllSelected.Should().BeTrue();
        }

        // $select=<foo>&$expand=MyDog($select=Color;$expand=MyPeople;)
        [Fact]
        public void SelectIsBasedOnTheCurrentLevel()
        {
            ExpandTermToken innerExpandTerm = new ExpandTermToken(new NonSystemToken("MyPeople", null, null));
            ExpandTermToken outerExpandTerm = new ExpandTermToken(new NonSystemToken("MyDog", null, null),
                                                                  new SelectToken(new List<PathSegmentToken>()
                                                                      {
                                                                          new NonSystemToken("Color", null, null)
                                                                      }),
                                                                  new ExpandToken(new ExpandTermToken[]
                                                                      {innerExpandTerm}));
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] {outerExpandTerm});
            var item = this.binderForPerson.Bind(BuildUnifiedSelectExpandToken(expandToken));
            var subExpand =
                item.SelectedItems.First()
                    .ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp())
                    .And.SelectAndExpand;
            subExpand.AllSelected.Should().BeFalse();
            subExpand.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetDogMyPeopleNavProp())
                .And.SelectAndExpand.AllSelected.Should().BeTrue();
            subExpand.SelectedItems.Last(x => x is PathSelectItem).ShouldBePathSelectionItem(new ODataPath( new PropertySegment(HardCodedTestModel.GetDogColorProp())));
        }

        // $expand=Blah
        [Fact]
        public void NonexistentPropertyThrowsUsefulError()
        {
            ExpandToken expandToken = new ExpandToken(new[] {new ExpandTermToken(new NonSystemToken("Blah", null, null))});
            Action bind = () => this.binderForPerson.Bind(BuildUnifiedSelectExpandToken(expandToken));
            bind.ShouldThrow<ODataException>()
                               .WithMessage(ODataErrorStrings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetPersonType().FullName(), "Blah"));
        }

        // $expand=Shoe
        [Fact]
        public void NonNavigationPropertyThrowsUsefulErrorIfKnobIsNotFlipped()
        {
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] {new ExpandTermToken(new NonSystemToken("Shoe", null, null))});
            Action bind = () => this.binderForPerson.Bind(BuildUnifiedSelectExpandToken(expandToken));
            bind.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpandItemBinder_PropertyIsNotANavigationProperty("Shoe", HardCodedTestModel.GetPersonType().FullName()));
        }

        // $select=<foo>&$expand=MyDog
        [Fact]
        // ToDo: I think this is less necessary in V4 because the top level select doesn't effect anything below it, so we always know what's selected at a given level based on the expand token syntax.
        public void NestedExpandsAutomaticallySelectedInExpandOptionMode()
        {
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[]
                {
                    new ExpandTermToken(new NonSystemToken("MyDog", null, null))
                });
            var item = this.binderForPerson.Bind(BuildUnifiedSelectExpandToken(expandToken));
            var myDogSelection =
                item.SelectedItems.First()
                    .ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp())
                    .And.SelectAndExpand;
            myDogSelection.AllSelected.Should().BeTrue(); // the inner expansion is all by default for the option-mode binder, as expected for this test.
            myDogSelection.SelectedItems.Should().BeEmpty();
        }

        [Fact]
        public void TypeTokensAreProperlyRecognizedAndCollapsedIntoASingleProperty()
        {
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[]
                {
                    new ExpandTermToken(new NonSystemToken("Fully.Qualified.Namespace.Employee", null,
                                                      new NonSystemToken("MyDog", null, null)))
                });
            var item = this.binderForPerson.Bind(BuildUnifiedSelectExpandToken(expandToken));
            var myDogExpansion = item.SelectedItems.First().ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp()).And.SelectAndExpand;
            myDogExpansion.SelectedItems.Should().BeEmpty();
            myDogExpansion.AllSelected.Should().BeTrue();
        }

        // $select=<foo>&$expand=MyDog($expand=MyPeople;)
        [Fact]
        public void EntitySetCorrectlyPopulatedAtEachLevel()
        {
            ExpandTermToken innerExpandTerm = new ExpandTermToken(new NonSystemToken("MyPeople", null, null));
            ExpandTermToken outerExpandTerm = new ExpandTermToken(new NonSystemToken("MyDog", null, null),
                                                                  null,
                                                                  new ExpandToken(new ExpandTermToken[] { innerExpandTerm }));
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] { outerExpandTerm });
            var item = this.binderForPerson.Bind(BuildUnifiedSelectExpandToken(expandToken));
            var myDogExpandItem = item.SelectedItems.First().ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp()).And;
            myDogExpandItem.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
            var myPeopleExpandItem = myDogExpandItem.SelectAndExpand.SelectedItems.First().ShouldBeExpansionFor(HardCodedTestModel.GetDogMyPeopleNavProp()).And;
            myPeopleExpandItem.NavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
            myPeopleExpandItem.SelectAndExpand.SelectedItems.Should().BeEmpty();
        }

        // $select=MyDog&$expand=MyDog
        [Fact]
        public void SelectedAndExpandedNavPropProduceExpandedNavPropSelectionItemAndPathSelectionItem()
        {
            ExpandTermToken innerExpandTermToken = new ExpandTermToken(new NonSystemToken("MyDog", null, null));
            ExpandToken innerExpandToken = new ExpandToken(new ExpandTermToken[] { innerExpandTermToken });
            ExpandTermToken topLevelExpandTermToken = new ExpandTermToken(new SystemToken(ExpressionConstants.It, /*nextToken*/null), new SelectToken(new List<PathSegmentToken>(){new NonSystemToken("MyDog", /*namedValues*/null, /*nextToken*/null)}), innerExpandToken);
            ExpandToken topLevelExpandToken = new ExpandToken(new ExpandTermToken[]{topLevelExpandTermToken});
            var item = this.binderForPerson.Bind(topLevelExpandToken);
            item.SelectedItems.Should().HaveCount(2);
            item.SelectedItems.First().ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp());
            item.SelectedItems.Last().ShouldBePathSelectionItem(new ODataPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), HardCodedTestModel.GetPeopleSet())));
        }

        [Fact]
        public void CanSelectPropertyOnNonEntityType()
        {
            ExpandTermToken expandTermToken = new ExpandTermToken(new SystemToken(ExpressionConstants.It, null), new SelectToken(new List<PathSegmentToken>(){new NonSystemToken("City", null, null)}), null );
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] { expandTermToken });
            var item = this.binderForAddress.Bind(expandToken);
            item.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetAddressCityProperty())));
        }

        [Fact]
        public void LowerLevelEmptySelectedItemsListDoesNotThrow()
        {
            ExpandToken lowerLevelExpand = new ExpandToken(new List<ExpandTermToken>());
            ExpandTermToken topLevelExpandTermToken = new ExpandTermToken(new NonSystemToken("MyDog", /*namedValues*/null, /*nextToken*/null), null, lowerLevelExpand);
            ExpandToken topLevelExpand = new ExpandToken(new List<ExpandTermToken>(){topLevelExpandTermToken});
            Action bindWithEmptySelectedItemsList = () => this.binderForPerson.Bind(BuildUnifiedSelectExpandToken(topLevelExpand));
            bindWithEmptySelectedItemsList.ShouldNotThrow();
        }

        private static ExpandToken BuildUnifiedSelectExpandToken(ExpandToken expandToken)
        {
            return BuildUnifiedSelectExpandToken(expandToken, null);
        }

        private static ExpandToken BuildUnifiedSelectExpandToken(ExpandToken expandToken, SelectToken selectToken)
        {
            return new ExpandToken(
                new ExpandTermToken[]
                {
                    new ExpandTermToken(new NonSystemToken(ExpressionConstants.It, /*namedValues*/null, /*nextToken*/null), selectToken, expandToken)
                });
        }
    }
}
