//---------------------------------------------------------------------
// <copyright file="SelectBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    /// <summary>
    /// Test cases related $expand and $select.
    /// </summary>
    public class SelectExpandBinderTests
    {
        internal readonly ODataUriParserConfiguration V4configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);

        public SelectExpandBinderTests()
        {
            BinderForPerson = new SelectExpandBinder(this.V4configuration,
                new ODataPathInfo(HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet()), null);
            BinderForAddress = new SelectExpandBinder(this.V4configuration,
                new ODataPathInfo(HardCodedTestModel.GetAddressType(), null), null);
        }

        /// <summary>
        /// ~/.../People?$expand=...&$select=...
        /// </summary>
        internal SelectExpandBinder BinderForPerson { get; }

        /// <summary>
        ///  ~/.../.../Address?$expand=...&$select=...
        /// </summary>
        internal SelectExpandBinder BinderForAddress { get; }

        [Fact]
        public void ConfigurationCannotBeNull()
        {
            // Arrange & At & Assert
            Assert.Throws<ArgumentNullException>("configuration",
                () => new SelectExpandBinder(null, new ODataPathInfo(null, HardCodedTestModel.GetPeopleSet()), null));
        }

        [Fact]
        public void TargetEdmTypeCannotBeNull()
        {
            // Arrange & At & Assert
            Assert.Throws<ArgumentNullException>("edmType",
                () => new SelectExpandBinder(this.V4configuration, new ODataPathInfo(null, HardCodedTestModel.GetPeopleSet()), null));
        }

        [Fact]
        public void TargetEdmTypeIsSetCorrectly()
        {
            // Arrange & Act & Assert
            Assert.Same(HardCodedTestModel.GetPersonType(), BinderForPerson.EdmType);
        }

        [Fact]
        public void TargetNavigationSourceCanBeNull()
        {
            // Arrange & Act
            SelectExpandBinder binder = new SelectExpandBinder(this.V4configuration, new ODataPathInfo(ModelBuildingHelpers.BuildValidEntityType(), null), null);

            // Assert
            Assert.Null(binder.NavigationSource);
        }

        [Fact]
        public void ModelIsSetCorrectly()
        {
            // Arrange & Act & Assert
            Assert.Same(HardCodedTestModel.TestModel, BinderForPerson.Model);
        }

        [Fact]
        public void BindReturnsNotNullForNullExpandAndSelect()
        {
            // Arrange & Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken: null, selectToken: null);

            // Assert
            Assert.NotNull(clause);
            Assert.True(clause.AllSelected);
            Assert.Empty(clause.SelectedItems);
        }

        [Fact]
        public void SelectedAndExpandedNavPropProduceExpandedNavPropSelectionItemAndPathSelectionItem()
        {
            // Arrange: $select=MyDog&$expand=MyDog
            ExpandTermToken innerExpandTermToken = new ExpandTermToken(new NonSystemToken("MyDog", null, null));
            ExpandToken expandToken = new ExpandToken(new ExpandTermToken[] { innerExpandTermToken });

            SelectTermToken innerSelectTermToken = new SelectTermToken(new NonSystemToken("MyDog", null, null));
            SelectToken selectToken = new SelectToken(new SelectTermToken[] { innerSelectTermToken });

            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken, selectToken);

            // Assert
            Assert.NotNull(clause);
            Assert.False(clause.AllSelected);
            Assert.Equal(2, clause.SelectedItems.Count());

            clause.SelectedItems.First().ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp());
            clause.SelectedItems.Last().ShouldBePathSelectionItem(new ODataPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), HardCodedTestModel.GetPeopleSet())));
        }

        [Fact]
        public void SelectWildcardStoresSubsumed()
        {
            // Arrange: $select=Name,*,Birthdate
            SelectToken selectToken = new SelectToken(
                new SelectTermToken[]
                {
                    new SelectTermToken(new NonSystemToken("Name", null, null)),
                    new SelectTermToken(new NonSystemToken("*", null, null)),
                    new SelectTermToken(new NonSystemToken("Birthdate", null, null)),
                });


            // Act
            SelectExpandClause clause = BinderForPerson.Bind(expandToken: null, selectToken);

            // Assert
            Assert.NotNull(clause);
            Assert.False(clause.AllSelected);
            WildcardSelectItem wildcardSelectItem = Assert.Single(clause.SelectedItems) as WildcardSelectItem;
            Assert.NotNull(wildcardSelectItem);

            SelectItem[] subsumedSelectItems = wildcardSelectItem.SubsumedSelectItems.ToArray();
            Assert.Equal(2, subsumedSelectItems.Length);
            subsumedSelectItems[0].ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp())));
            subsumedSelectItems[1].ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonBirthdateProp())));
        }

        #region helpers
        public static ExpandToken ParseExpandToken(string expand)
        {
            SelectExpandParser parser = new SelectExpandParser(expand, ODataUriParserSettings.DefaultSelectExpandLimit);
            return parser.ParseExpand();
        }

        public static SelectToken ParseSelectToken(string select)
        {
            SelectExpandParser parser = new SelectExpandParser(select, ODataUriParserSettings.DefaultSelectExpandLimit);
            return parser.ParseSelect();
        }
        #endregion
    }
}
