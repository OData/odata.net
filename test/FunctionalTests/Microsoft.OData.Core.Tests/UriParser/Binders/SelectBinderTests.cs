//---------------------------------------------------------------------
// <copyright file="SelectBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    public class SelectBinderTests
    {
        [Fact]
        public void NonSystemTokenTranslatedToSelectionItem()
        {
            var expandTree = new SelectExpandClause(new Collection<SelectItem>(), false);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>() { new NonSystemToken("Shoe", null, null) });

            binder.Bind(selectToken).SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonShoeProp())));
        }

        [Fact]
        public void WildcardSelectionItemPreemptsStructuralProperties()
        {
            var expandTree = new SelectExpandClause(new Collection<SelectItem>() 
                                        {
                                            new PathSelectItem(new ODataSelectPath(new PropertySegment( HardCodedTestModel.GetPersonNameProp()))),
                                        }, false);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>() { new NonSystemToken("*", null, null) });

            binder.Bind(selectToken).SelectedItems.Single().ShouldBeWildcardSelectionItem();
        }

        [Fact]
        public void WildcardDoesNotPreemptOtherSelectionItems()
        {
            ODataSelectPath coolPeoplePath = new ODataSelectPath(new OperationSegment(new IEdmOperation[] { HardCodedTestModel.GetChangeStateAction() }, null));
            ODataSelectPath stuffPath = new ODataSelectPath(new OpenPropertySegment("stuff"));
            var expandTree = new SelectExpandClause(new Collection<SelectItem>()
                                        {
                                            new PathSelectItem(coolPeoplePath),
                                            new PathSelectItem(new ODataSelectPath(new NavigationPropertySegment(HardCodedTestModel.GetPaintingOwnerNavProp(), null))),
                                            new PathSelectItem(stuffPath),
                                            new PathSelectItem(new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPaintingColorsProperty())))
                                        }, false);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>() { new NonSystemToken("*", null, null) });
            var item = binder.Bind(selectToken);
            item.SelectedItems.Should().HaveCount(4)
                       .And.Contain(x => x is PathSelectItem && x.As<PathSelectItem>().SelectedPath == coolPeoplePath)
                       .And.Contain(x => x is PathSelectItem && x.As<PathSelectItem>().SelectedPath == stuffPath)
                       .And.Contain(x => x is PathSelectItem && x.As<PathSelectItem>().SelectedPath.LastSegment is NavigationPropertySegment && x.As<PathSelectItem>().SelectedPath.LastSegment.As<NavigationPropertySegment>().NavigationProperty.Name == HardCodedTestModel.GetPaintingOwnerNavProp().Name)
                       .And.Contain(x => x is WildcardSelectItem);
        }

        [Fact]
        public void ExistingWildcardPreemptsAnyNewPropertiesAdded()
        {
            var expandTree = new SelectExpandClause(new Collection<SelectItem>() 
                                        {
                                            new WildcardSelectItem()
                                        }, false);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>() { new NonSystemToken("Name", null, null) });
            binder.Bind(selectToken).SelectedItems.Single().ShouldBeWildcardSelectionItem();
        }

        [Fact]
        public void AllSelectedIsSetIfSelectIsEmpty()
        {
            var expandTree = new SelectExpandClause(new Collection<SelectItem>(), false);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>());
            binder.Bind(selectToken).AllSelected.Should().BeTrue();
        }

        [Fact]
        public void AllSelectedIsNotSetIfSelectedIsNotEmpty()
        {
            var expandTree = new SelectExpandClause(new Collection<SelectItem>(), true);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>(){new NonSystemToken("Name", null, null)});
            binder.Bind(selectToken).AllSelected.Should().BeFalse();
        }

        [Fact]
        public void MultiLevelPathThrows()
        {
            SelectExpandClause expandTree = new SelectExpandClause(new Collection<SelectItem>() {new ExpandedNavigationSelectItem(
                                                                                                new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)), 
                                                                                                HardCodedTestModel.GetPeopleSet(),
                                                                                                new SelectExpandClause(new Collection<SelectItem>(), true))}, true);
            SelectBinder binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            SelectToken selectToken = new SelectToken(new List<PathSegmentToken>() { new NonSystemToken("MyDog", null, new NonSystemToken("Color", null, null)) });

            Action bindWithMultiLevelPath = () => binder.Bind(selectToken);
            bindWithMultiLevelPath.ShouldThrow<ODataException>().WithMessage(Strings.SelectBinder_MultiLevelPathInSelect);
        }

        [Fact]
        public void SelectionInStartingExpandIsOverriddenWhereNecessary()
        {
            SelectExpandClause expandTree = new SelectExpandClause(new Collection<SelectItem>(){new ExpandedNavigationSelectItem(
                                                                                                                new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)),
                                                                                                                HardCodedTestModel.GetPeopleSet(),
                                                                                                                new SelectExpandClause(new Collection<SelectItem>(), false))}, false);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>() { new NonSystemToken("Name", null, null) });
            var result = binder.Bind(selectToken);
            result.SelectedItems.Last().ShouldBePathSelectionItem(new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp())));
            result.SelectedItems.First().ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp())
                  .And.SelectAndExpand.SelectedItems.Should().BeEmpty();
        }

        [Fact]
        public void ExpandWithNoSelectionIsUnchanged()
        {
            SelectExpandClause expandTree = new SelectExpandClause(new Collection<SelectItem>(){ new ExpandedNavigationSelectItem(
                                                                                                new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)), 
                                                                                                HardCodedTestModel.GetPeopleSet(),
                                                                                                new SelectExpandClause(new Collection<SelectItem>(), true))}, true);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>());
            var result = binder.Bind(selectToken);
            result.AllSelected.Should().BeTrue();
            result.SelectedItems.Single().ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp())
                  .And.SelectAndExpand.AllSelected.Should().BeTrue();
        }

        [Fact]
        public void SelectAndExpandTheSameNavPropResolvesToBothTheSelectionAndExpandionAndAllSelectedIsFalse()
        {
            SelectExpandClause expandTree = new SelectExpandClause(
                                        new Collection<SelectItem>() { new ExpandedNavigationSelectItem(
                                                                                        new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)), 
                                                                                        HardCodedTestModel.GetPeopleSet(),
                                                                                        new SelectExpandClause(new Collection<SelectItem>(), false))}, false);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>() { new NonSystemToken("MyDog", null, null) });
            var result = binder.Bind(selectToken);
            result.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp()).And.SelectAndExpand.AllSelected.Should().BeFalse();
            result.SelectedItems.Single(x => x is PathSelectItem).ShouldBePathSelectionItem(new ODataPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), HardCodedTestModel.GetPeopleSet())));
        }

        [Fact]
        public void NavPropEndPathResultsInNavPropLinkSelectionItem()
        {
            SelectExpandClause expandTree = new SelectExpandClause(new Collection<SelectItem>(), false);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>() { new NonSystemToken("MyDog", null, null) });
            var result = binder.Bind(selectToken);
            result.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)));
        }

        [Fact]
        public void FunctionCallsInSelectTreeThrows()
        {
            SelectExpandClause expandTree = new SelectExpandClause(new Collection<SelectItem>()
                                                            { new ExpandedNavigationSelectItem(
                                                            new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)), 
                                                            HardCodedTestModel.GetPeopleSet(),
                                                            new SelectExpandClause(null, false))}, true);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>()
                {
                    new NonSystemToken("substring", null, null)
                });
            Action bindWithFunctionInSelect = () => binder.Bind(selectToken);
            bindWithFunctionInSelect.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetPersonType(), "substring"));
        }


        [Fact]
        public void CustomFunctionsThrow()
        {
            SelectExpandClause expandTree = new SelectExpandClause(new Collection<SelectItem>()
                                                                { new ExpandedNavigationSelectItem(
                                                                new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)), 
                                                                HardCodedTestModel.GetPeopleSet(),
                                                                new SelectExpandClause(null, false))}, true);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>()
                {
                    new NonSystemToken("GetCoolPeople", null, null)
                });
            Action bindWithFunctionInSelect = () => binder.Bind(selectToken);
            bindWithFunctionInSelect.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetPersonType(), "GetCoolPeople"));
        }

        [Fact]
        public void MustFindNonTypeSegmentBeforeTheEndOfTheChain()
        {
            SelectExpandClause expandTree = new SelectExpandClause(new Collection<SelectItem>(), false);
            var binder = new SelectBinder(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), 800, expandTree);
            var selectToken = new SelectToken(new List<PathSegmentToken>()
                {
                    new NonSystemToken("Fully.Qualified.Namespace.Employee", null, null)
                });
            var selectExpand = binder.Bind(selectToken);
            selectExpand.SelectedItems.Count().Should().Be(1);
        }
    }
}
