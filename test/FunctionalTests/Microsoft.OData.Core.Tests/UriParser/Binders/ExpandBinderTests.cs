//---------------------------------------------------------------------
// <copyright file="ExpandBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    /// <summary>
    /// Test cases related $expand and nested query options in $expand.
    /// </summary>
    public class ExpandBinderTests : SelectExpandBinderTests
    {
        [Fact]
        public void BindingOnTreeWithWithMultipleNavPropPathsThrows()
        {
            // Arrange: $expand=MyDog/MyPeople
            NonSystemToken innerSegment = new NonSystemToken("MyPeople", null, null);
            NonSystemToken navProp = new NonSystemToken("MyDog", null, innerSegment);
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(navProp) });

            // Act & Assert
            Action test = () => BinderForPerson.Bind(expandToken, null);
            test.Throws<ODataException>(ODataErrorStrings.ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath);
        }

        // $expand=MyDog
        [Fact]
        public void SingleLevelExpandTermTokenWorks()
        {
            // Arrange
            ExpandTermToken expandTermToken = new ExpandTermToken(new NonSystemToken("MyDog", null, null));
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] { expandTermToken });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken, null);

            // Arrange
            Assert.NotNull(clause);
            SelectItem selectItem = Assert.Single(clause.SelectedItems);

            ExpandedNavigationSelectItem expandedItem = selectItem.ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp());
            Assert.NotNull(expandedItem);
            Assert.NotNull(expandedItem.SelectAndExpand);
        }

        [Fact]
        public void MultiLevelExpandTermTokenWorks()
        {
            // Arrange: $expand=MyDog($expand=MyPeople)
            ExpandTermToken innerExpandTerm = new ExpandTermToken(new NonSystemToken("MyPeople", null, null));
            ExpandTermToken outerExpandTerm = new ExpandTermToken(new NonSystemToken("MyDog", null, null), null,
                                                                  new ExpandToken(new ExpandTermToken[] { innerExpandTerm }));
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] { outerExpandTerm });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken, null);

            // Assert
            Assert.True(clause.AllSelected);
            SelectItem selectItem = Assert.Single(clause.SelectedItems);

            ExpandedNavigationSelectItem expandedItem = selectItem.ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp());

            // sub-level
            Assert.NotNull(expandedItem.SelectAndExpand);
            Assert.True(expandedItem.SelectAndExpand.AllSelected);
            selectItem = Assert.Single(expandedItem.SelectAndExpand.SelectedItems);
            expandedItem = selectItem.ShouldBeExpansionFor(HardCodedTestModel.GetDogMyPeopleNavProp());

            // sub-sub level
            Assert.NotNull(expandedItem.SelectAndExpand);
        }

        [Fact]
        public void SelectIsBasedOnTheCurrentLevel()
        {
            // Arrange: $expand=MyDog($select=Color;$expand=MyPeople;)
            ExpandTermToken innerExpandTerm = new ExpandTermToken(new NonSystemToken("MyPeople", null, null));
            ExpandTermToken outerExpandTerm = new ExpandTermToken(new NonSystemToken("MyDog", null, null),
                                                                  new SelectToken(new List<PathSegmentToken>()
                                                                      {
                                                                          new NonSystemToken("Color", null, null)
                                                                      }),
                                                                  new ExpandToken(new ExpandTermToken[] { innerExpandTerm }));
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] { outerExpandTerm });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken, null);

            // $expand=MyDog
            Assert.True(clause.AllSelected);
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            ExpandedNavigationSelectItem expandedItem = selectItem.ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp());

            // Sub-level ($select=Color;$expand=MyPeople;)
            SelectExpandClause subSelectExpand = expandedItem.SelectAndExpand;
            Assert.False(subSelectExpand.AllSelected);
            Assert.Equal(2, subSelectExpand.SelectedItems.Count());
            selectItem = Assert.Single(subSelectExpand.SelectedItems, x => x is ExpandedNavigationSelectItem);
            expandedItem = selectItem.ShouldBeExpansionFor(HardCodedTestModel.GetDogMyPeopleNavProp());

            Assert.NotNull(expandedItem.SelectAndExpand);
        }

        [Fact]
        public void NonexistentPropertyThrowsUsefulError()
        {
            // Arrange: $expand=Blah
            ExpandToken expandToken = new ExpandToken(new[] { new ExpandTermToken(new NonSystemToken("Blah", null, null)) });

            // Act & Assert
            Action test = () => BinderForPerson.Bind(expandToken, null);
            test.Throws<ODataException>(ODataErrorStrings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetPersonType().FullName(), "Blah"));
        }

        [Fact]
        public void NonNavigationPropertyThrowsUsefulErrorIfKnobIsNotFlipped()
        {
            // Arrange: $expand=Shoe
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(new NonSystemToken("Shoe", null, null)) });

            // Act
            Action test = () => BinderForPerson.Bind(expandToken, null);
            test.Throws<ODataException>(ODataErrorStrings.ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty("Shoe", HardCodedTestModel.GetPersonType().FullName()));
        }

        [Fact]
        public void NestedExpandsAutomaticallySelectedInExpandOptionMode()
        {
            // Arrange: $select=<foo>&$expand=MyDog
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[]
                {
                    new ExpandTermToken(new NonSystemToken("MyDog", null, null))
                });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken, null);

            // Assert
            Assert.NotNull(clause);
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            ExpandedNavigationSelectItem expandedItem = selectItem.ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp());

            Assert.NotNull(expandedItem.SelectAndExpand);
            Assert.True(expandedItem.SelectAndExpand.AllSelected);
            Assert.Empty(expandedItem.SelectAndExpand.SelectedItems);
        }

        [Fact]
        public void TypeTokensAreProperlyRecognizedAndCollapsedIntoASingleProperty()
        {
            // Arrange: $expand=Fully.Qualified.Namespace.Employee/MyDog
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[]
            {
                new ExpandTermToken(
                    new NonSystemToken("Fully.Qualified.Namespace.Employee", null,
                    new NonSystemToken("MyDog", null, null)))
            });

            // Act
            SelectExpandClause clause =BinderForPerson.Bind(expandToken, null);

            // Assert
            Assert.NotNull(clause);
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            ExpandedNavigationSelectItem expandedItem = selectItem.ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp());

            Assert.NotNull(expandedItem.SelectAndExpand);
        }

        [Fact]
        public void EntitySetCorrectlyPopulatedAtEachLevel()
        {
            // Arrange: $expand=MyDog($expand=MyPeople;)
            ExpandTermToken innerExpandTerm = new ExpandTermToken(new NonSystemToken("MyPeople", null, null));
            ExpandTermToken outerExpandTerm = new ExpandTermToken(new NonSystemToken("MyDog", null, null),
                                                                  null,
                                                                  new ExpandToken(new ExpandTermToken[] { innerExpandTerm }));
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] { outerExpandTerm });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken, null);

            // Assert
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            ExpandedNavigationSelectItem myDogExpandItem = selectItem.ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp());
            Assert.Same(HardCodedTestModel.GetDogsSet(), myDogExpandItem.NavigationSource);

            // Sub-level ($expand=MyPeople)
            Assert.NotNull(myDogExpandItem.SelectAndExpand);
            selectItem = Assert.Single(myDogExpandItem.SelectAndExpand.SelectedItems);
            ExpandedNavigationSelectItem myPeopleExpandItem = selectItem.ShouldBeExpansionFor(HardCodedTestModel.GetDogMyPeopleNavProp());
            Assert.Same(HardCodedTestModel.GetPeopleSet(), myPeopleExpandItem.NavigationSource);

            // Sub-sub-level
            Assert.NotNull(myPeopleExpandItem.SelectAndExpand);
        }

        [Fact]
        public void LowerLevelEmptySelectedItemsListDoesNotThrow()
        {
            // Arrange
            ExpandToken lowerLevelExpand = new ExpandToken(new List<ExpandTermToken>());
            ExpandTermToken topLevelExpandTermToken = new ExpandTermToken(new NonSystemToken("MyDog", /*namedValues*/null, /*nextToken*/null), null, lowerLevelExpand);
            ExpandToken topLevelExpand = new ExpandToken(new List<ExpandTermToken>() { topLevelExpandTermToken });

            // Act
            Action bindWithEmptySelectedItemsList = () => BinderForPerson.Bind(topLevelExpand, null);

            // Assert
            var ex = Record.Exception(bindWithEmptySelectedItemsList);
            Assert.Null(ex);
        }

        [Fact]
        public void BindingOnTreeWithWithTypeTokenDoesNotThrow()
        {
            // Arrange: $expand=MyPeople/Fully.Qualified.Namespace.Employee
            NonSystemToken innerSegment = new NonSystemToken("Fully.Qualified.Namespace.Employee", null, null);
            NonSystemToken navProp = new NonSystemToken("MyPeople", null, innerSegment);
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(navProp) });

            // Act
            var binderForDog = new SelectExpandBinder(this.V4configuration, new ODataPathInfo(HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet()), null);
            SelectExpandClause selectExpandClause = binderForDog.Bind(expandToken, null);

            // Assert
            Assert.NotNull(selectExpandClause);
            var selectItem = Assert.Single(selectExpandClause.SelectedItems, x => x is ExpandedNavigationSelectItem);
            ExpandedNavigationSelectItem expandedNavigationSelectItem = selectItem as ExpandedNavigationSelectItem;
            Assert.Equal(2, expandedNavigationSelectItem.PathToNavigationProperty.Count);

            NavigationPropertySegment navPropSegment = Assert.IsType<NavigationPropertySegment>(expandedNavigationSelectItem.PathToNavigationProperty.Segments.First());
            TypeSegment typeSegment = Assert.IsType<TypeSegment>(expandedNavigationSelectItem.PathToNavigationProperty.Segments.Last());
            Assert.Equal("MyPeople", navPropSegment.Identifier);
            Assert.Equal("Collection(Fully.Qualified.Namespace.Person)", navPropSegment.EdmType.FullTypeName());
            Assert.Equal("Fully.Qualified.Namespace.Employee", typeSegment.EdmType.FullTypeName());
        }
    }
}
