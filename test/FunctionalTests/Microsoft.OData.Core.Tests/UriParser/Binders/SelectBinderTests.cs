//---------------------------------------------------------------------
// <copyright file="SelectBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    /// <summary>
    /// Test cases related $select and nested query options in $select.
    /// </summary>
    public class SelectBinderTests : SelectExpandBinderTests
    {
        [Fact]
        public void SystemTokenInSelectionThrows()
        {
            // Arrange
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("Shoe", null, new SystemToken("$value", null)))
            });

            // Act
            Action test = () => BinderForPerson.Bind(null, selectToken);

            // Assert
            test.Throws<ODataException>(Strings.SelectExpandBinder_SystemTokenInSelect("$value"));
        }

        [Fact]
        public void NonSystemTokenTranslatedToSelectionItem()
        {
            // Arrange: $select=Shoe
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("Shoe", null, null))
            });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(null, selectToken);

            // Assert
            Assert.NotNull(clause);
            Assert.False(clause.AllSelected);
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            selectItem.ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonShoeProp())));
        }

        [Fact]
        public void CanSelectPropertyOnNonEntityType()
        {
            // Arrange
            SelectToken select = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("City", null, null))
            });

            // Arrange
            SelectExpandClause item = BinderForAddress.Bind(null, select);

            // Assert
            item.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetAddressCityProperty())));
        }

        [Fact]
        public void NavigationPropertyEndPathResultsInNavPropLinkSelectionItem()
        {
            // Arrange: $select=MyDog
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("MyDog", null, null))
            });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(null, selectToken);

            // Assert
            Assert.NotNull(clause);
            Assert.False(clause.AllSelected);
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            selectItem.ShouldBePathSelectionItem(new ODataPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)));
        }

        [Fact]
        public void WildcardSelectionItemPreemptsStructuralProperties()
        {
            // Arrange : $select=*
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("*", null, null))
            });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(null, selectToken);

            // Assert
            Assert.NotNull(clause);
            Assert.False(clause.AllSelected);
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            selectItem.ShouldBeWildcardSelectionItem();
        }

        [Fact]
        public void NamespaceWildcardSelectionItemPreemptsStructuralProperties()
        {
            // Arrange : $select=*
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("Fully.Qualified.Namespace.*", null, null))
            });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(null, selectToken);

            // Assert
            Assert.NotNull(clause);
            Assert.False(clause.AllSelected);
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            NamespaceQualifiedWildcardSelectItem namespaceSelectItem = Assert.IsType<NamespaceQualifiedWildcardSelectItem>(selectItem);
            Assert.Equal("Fully.Qualified.Namespace", namespaceSelectItem.Namespace);
        }

        [Fact]
        public void WildcardDoesNotPreemptOtherSelectionItems()
        {
            // Arrange
            ODataSelectPath coolPeoplePath = new ODataSelectPath(new OperationSegment(new[] { HardCodedTestModel.GetColorAtPositionFunction() }, null));
            ODataSelectPath stuffPath = new ODataSelectPath(new DynamicPathSegment("stuff"));

            SelectExpandBinder binderForPainting = new SelectExpandBinder(this.V4configuration,
                new ODataPathInfo(HardCodedTestModel.GetPaintingType(), null), null);

            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("Fully.Qualified.Namespace.GetColorAtPosition", null, null)), // operation
                new SelectTermToken(new NonSystemToken("Owner", null, null)), // navigation property
                new SelectTermToken(new NonSystemToken("stuff", null, null)), // dynamic property
                new SelectTermToken(new NonSystemToken("Colors", null, null)), // structural property
                new SelectTermToken(new NonSystemToken("*", null, null)) // *
            });

            // Act
            SelectExpandClause clause = binderForPainting.Bind(null, selectToken);

            // Assert
            Assert.NotNull(clause);
            Assert.Equal(4, clause.SelectedItems.Count());

            // one is "*"
            Assert.Single(clause.SelectedItems.OfType<WildcardSelectItem>());

            IList<PathSelectItem> pathSelectItems = clause.SelectedItems.OfType<PathSelectItem>().ToList();
            Assert.Equal(3, pathSelectItems.Count);
            Assert.Contains(pathSelectItems, (x) => x.SelectedPath.Equals(coolPeoplePath));
            Assert.Contains(pathSelectItems, (x) => x.SelectedPath.Equals(stuffPath));
            Assert.Contains(pathSelectItems, (x) =>
            {
                NavigationPropertySegment segment = x.SelectedPath.LastSegment as NavigationPropertySegment;
                if (segment != null)
                {
                    return segment.NavigationProperty.Name == "Owner";
                }

                return false;
            });
        }

        [Fact]
        public void ExistingWildcardPreemptsAnyNewPropertiesAdded()
        {
            // Arrange
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("*", null, null)), // *
                new SelectTermToken(new NonSystemToken("Name", null, null)) // normal property
            });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(null, selectToken);

            // Assert
            Assert.NotNull(clause);
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            selectItem.ShouldBeWildcardSelectionItem();
        }

        [Fact]
        public void InvlidIdentifierInSelectionThrows()
        {
            // Arrange
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("Dotted.Name", null, null))
            });

            // Act
            Action test = () => BinderForPerson.Bind(null, selectToken);

            // Assert
            test.Throws<ODataException>(Strings.MetadataBinder_InvalidIdentifierInQueryOption("Dotted.Name"));
        }

        [Fact]
        public void AllSelectedIsSetIfSelectIsEmpty()
        {
            // Arrange & Act
            SelectExpandClause clause = BinderForPerson.Bind(new ExpandToken(), selectToken: null);

            // Assert
            Assert.NotNull(clause);
            Assert.True(clause.AllSelected);
        }

        [Fact]
        public void AllSelectedIsNotSetIfSelectedIsNotEmpty()
        {
            // Arrange
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("Name", null, null)) // normal property
            });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(null, selectToken);

            // Assert
            Assert.NotNull(clause);
            Assert.False(clause.AllSelected);
        }

        [Fact]
        public void MultiLevelPathAfterNavigationPropertyThrows()
        {
            // Arrange : $select=MyDog/Color
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("MyDog", null, new NonSystemToken("Color", null, null)))
            });

            // Act
            Action test = () => BinderForPerson.Bind(null, selectToken);

            // Assert
            test.Throws<ODataException>(Strings.SelectBinder_MultiLevelPathInSelect);
        }

        [Theory]
        [InlineData("*($top=2)", "*")]
        [InlineData("MyDog($count=true)", "MyDog")]
        public void QueryOptionsNestedInNotAllowedSelectionThrows(string select, string identifier)
        {
            // Arrange
            SelectToken selectToken = ParseSelectToken(select);

            // Act
            Action test = () => BinderForPerson.Bind(null, selectToken);

            // Assert
            test.Throws<ODataException>(Strings.SelectExpandBinder_InvalidQueryOptionNestedSelection(identifier));
        }

        [Fact]
        public void FunctionCallsInSelectTreeThrows()
        {
            // Arrange
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("substring", null, null))
            });

            // Act
            Action test = () => BinderForPerson.Bind(null, selectToken);

            // Arrange
            test.Throws<ODataException>(Strings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetPersonType(), "substring"));
        }

        [Fact]
        public void CustomFunctionsThrow()
        {
            // Arrange
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("GetCoolPeople", null, null))
            });

            // Act
            Action test = () => BinderForPerson.Bind(null, selectToken);

            // Assert
            test.Throws<ODataException>(Strings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetPersonType(), "GetCoolPeople"));
        }

        [Fact]
        public void MustFindNonTypeSegmentBeforeTheEndOfTheChain()
        {
            // Arrange
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(new NonSystemToken("Fully.Qualified.Namespace.Employee", null, new NonSystemToken("WorkEmail", null, null)))
            });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(null, selectToken);

            // Assert
            Assert.NotNull(clause);
            Assert.False(clause.AllSelected);
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            PathSelectItem subPathSelect = Assert.IsType<PathSelectItem>(selectItem);
            Assert.Equal("Fully.Qualified.Namespace.Employee/WorkEmail", subPathSelect.SelectedPath.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses));
        }

        #region nested $filter
        [Fact]
        public void NestedFilterWithValidExpression()
        {
            // Arrange
            SelectToken select = ParseSelectToken("PreviousAddresses($filter=Street eq 'abc')");

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken: null, selectToken: select);

            // Assert
            Assert.NotNull(clause);
            Assert.NotNull(clause.SelectedItems);

            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            PathSelectItem subPathSelect = Assert.IsType<PathSelectItem>(selectItem);
            Assert.Equal("PreviousAddresses", subPathSelect.SelectedPath.FirstSegment.Identifier);

            Assert.NotNull(subPathSelect.FilterOption);
            subPathSelect.FilterOption.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }
        #endregion

        #region nested $orderby
        [Fact]
        public void NestedOrderbyWithValidExpression()
        {
            // Arrange
            SelectToken select = ParseSelectToken("PreviousAddresses($orderby=Street asc)");

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken: null, selectToken: select);

            // Assert
            Assert.NotNull(clause);
            Assert.NotNull(clause.SelectedItems);

            // FavoriteColors
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            PathSelectItem subPathSelect = Assert.IsType<PathSelectItem>(selectItem);
            Assert.Equal("PreviousAddresses", subPathSelect.SelectedPath.FirstSegment.Identifier);
            Assert.NotNull(subPathSelect.OrderByOption);

            var propType = HardCodedTestModel.GetPersonPreviousAddressesProp().Type.AsCollection().ElementType().AsStructured();
            var property = propType.FindProperty("Street");
            subPathSelect.OrderByOption.Expression.ShouldBeSingleValuePropertyAccessQueryNode(property);
        }
        #endregion

        #region nested $search
        [Fact]
        public void NestedSearchWithValidExpression()
        {
            // Arrange
            SelectToken select = ParseSelectToken("PreviousAddresses($search=Name)");

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken: null, selectToken: select);

            // Assert
            Assert.NotNull(clause);
            Assert.NotNull(clause.SelectedItems);

            // FavoriteColors
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            PathSelectItem subPathSelect = Assert.IsType<PathSelectItem>(selectItem);
            Assert.Equal("PreviousAddresses", subPathSelect.SelectedPath.FirstSegment.Identifier);

            Assert.NotNull(subPathSelect.SearchOption);
            subPathSelect.SearchOption.Expression.ShouldBeSearchTermNode("Name");
        }
        #endregion

        #region nested $top, $skip, $count
        [Fact]
        public void NestedTopWithValidExpression()
        {
            // Arrange
            SelectToken select = ParseSelectToken("FavoriteColors($top=5)");

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken: null, selectToken: select);

            // Assert
            Assert.NotNull(clause);
            Assert.NotNull(clause.SelectedItems);

            // FavoriteColors
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            PathSelectItem subPathSelect = Assert.IsType<PathSelectItem>(selectItem);
            Assert.NotNull(subPathSelect.TopOption);
            Assert.Equal(5, subPathSelect.TopOption.Value);
        }

        [Fact]
        public void NestedSkipWithValidExpression()
        {
            SelectToken select = ParseSelectToken("FavoriteColors($skip=5)");

            SelectExpandClause clause = BinderForPerson.Bind(expandToken: null, selectToken: select);
            Assert.NotNull(clause);
            Assert.NotNull(clause.SelectedItems);

            // FavoriteColors
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            PathSelectItem subPathSelect = Assert.IsType<PathSelectItem>(selectItem);
            Assert.NotNull(subPathSelect.SkipOption);
            Assert.Equal(5, subPathSelect.SkipOption.Value);
        }

        [Fact]
        public void NestedCountWithValidExpression()
        {
            // Assert
            SelectToken select = ParseSelectToken("FavoriteColors($count=true)");

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken: null, selectToken: select);

            // Assert
            Assert.NotNull(clause);
            Assert.NotNull(clause.SelectedItems);

            // FavoriteColors
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            PathSelectItem subPathSelect = Assert.IsType<PathSelectItem>(selectItem);
            Assert.NotNull(subPathSelect.CountOption);
            Assert.True(subPathSelect.CountOption.Value);
        }

        [Fact]
        public void NestedTopAndSkipAndCountWithValidExpression()
        {
            // Arrange
            SelectToken select = ParseSelectToken("FavoriteColors($skip=4;$count=false;$top=2)");

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken: null, selectToken: select);

            // Assert
            Assert.NotNull(clause);
            Assert.NotNull(clause.SelectedItems);

            // FavoriteColors
            SelectItem selectItem = Assert.Single(clause.SelectedItems);
            PathSelectItem subPathSelect = Assert.IsType<PathSelectItem>(selectItem);

            Assert.NotNull(subPathSelect.SkipOption);
            Assert.Equal(4, subPathSelect.SkipOption.Value);

            Assert.NotNull(subPathSelect.CountOption);
            Assert.False(subPathSelect.CountOption.Value);

            Assert.NotNull(subPathSelect.TopOption);
            Assert.Equal(2, subPathSelect.TopOption.Value);
        }
        #endregion

        #region nested $select
        [Fact]
        public void NestedSelectWithValidExpression()
        {
            // Arrange
            SelectToken select = ParseSelectToken("MyAddress($select=Street,NextHome($select=MyNeighbors))");

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken: null, selectToken: select);

            // Assert
            Assert.NotNull(clause);

            // only one top level $select
            SelectItem selectItem = Assert.Single(clause.SelectedItems);

            // it should be "PathSelectItem
            PathSelectItem pathSelect = Assert.IsType<PathSelectItem>(selectItem);

            Assert.NotNull(pathSelect.SelectAndExpand);
            Assert.Equal(2, pathSelect.SelectAndExpand.SelectedItems.Count()); // Street & NextHome

            // Street
            selectItem = pathSelect.SelectAndExpand.SelectedItems.First();
            PathSelectItem subPathSelect = Assert.IsType<PathSelectItem>(selectItem);
            Assert.NotNull(subPathSelect.SelectAndExpand);
            Assert.Equal("Street", subPathSelect.SelectedPath.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses));

            // NextHome
            selectItem = pathSelect.SelectAndExpand.SelectedItems.Last();
            pathSelect = Assert.IsType<PathSelectItem>(selectItem);
            Assert.Equal("NextHome", pathSelect.SelectedPath.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses));
            Assert.NotNull(pathSelect.SelectAndExpand);

            selectItem = Assert.Single(pathSelect.SelectAndExpand.SelectedItems);
            pathSelect = Assert.IsType<PathSelectItem>(selectItem);
            Assert.Equal("MyNeighbors", pathSelect.SelectedPath.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses));
        }

        [Fact]
        public void NestedSelectWithValidExpressionOnCollection()
        {
            // Arrange
            SelectToken select = ParseSelectToken("PreviousAddresses($select=Street)");

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken: null, selectToken: select);

            // Assert
            Assert.NotNull(clause);

            // only one top level $select
            SelectItem selectItem = Assert.Single(clause.SelectedItems);

            // it should be "PathSelectItem
            PathSelectItem pathSelect = Assert.IsType<PathSelectItem>(selectItem);

            Assert.NotNull(pathSelect.SelectAndExpand);

            // Street
            selectItem = Assert.Single(pathSelect.SelectAndExpand.SelectedItems);
            PathSelectItem subPathSelect = Assert.IsType<PathSelectItem>(selectItem);
            Assert.Equal("Street", subPathSelect.SelectedPath.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses));
            Assert.NotNull(subPathSelect.SelectAndExpand);
        }
        #endregion

        #region $compute
        [Fact]
        public void NestedComputeWithValidExpression()
        {
            // Arrange
            SelectToken select = ParseSelectToken("PreviousAddresses($compute=tolower(Street) as lowStreet;$select=lowStreet)");

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken: null, selectToken: select);

            // Assert
            Assert.NotNull(clause);
            Assert.NotNull(clause.SelectedItems);

            // FavoriteColors
            var selectItem = Assert.Single(clause.SelectedItems);
            PathSelectItem subPathSelect = Assert.IsType<PathSelectItem>(selectItem);
            Assert.NotNull(subPathSelect.ComputeOption);

            ComputeExpression computeExpr = Assert.Single(subPathSelect.ComputeOption.ComputedItems);
            Assert.Equal("lowStreet", computeExpr.Alias);
            var functionCall = Assert.IsType<SingleValueFunctionCallNode>(computeExpr.Expression);
            Assert.Equal("tolower", functionCall.Name);
            Assert.Empty(functionCall.Functions);

            Assert.NotNull(subPathSelect.SelectAndExpand);

            selectItem = Assert.Single(subPathSelect.SelectAndExpand.SelectedItems);

            // it should be "lowStreet
            PathSelectItem pathSelect = Assert.IsType<PathSelectItem>(selectItem);

            ODataPathSegment segment = Assert.Single(pathSelect.SelectedPath);
            DynamicPathSegment dynamic = Assert.IsType<DynamicPathSegment>(segment);
            Assert.Equal("lowStreet", dynamic.Identifier);
        }
        #endregion
    }
}
