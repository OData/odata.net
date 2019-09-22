//---------------------------------------------------------------------
// <copyright file="ExpandOptionFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// UriParser Function tests for expand Options
    /// </summary>
    public class ExapndOptionFunctionalTests
    {
        private static readonly IEdmEntityType PersonType = HardCodedTestModel.GetPersonType();
        private static readonly IEdmEntitySet PeopleSet = HardCodedTestModel.GetPeopleSet();
        private static readonly IEdmModel Model;
        private static readonly IEdmEntityType T2Type;
        private static readonly IEdmEntityType T3Type;
        private static readonly IEdmEntitySet T2Set;
        private static readonly IEdmEntitySet T3Set;

        static ExapndOptionFunctionalTests()
        {
            string namespaceA = "NS";
            EdmModel edmModel = new EdmModel();
            EdmEntityType type1 = new EdmEntityType(namespaceA, "T1");
            EdmEntityType type2 = new EdmEntityType(namespaceA, "T2", type1);
            EdmEntityType type3 = new EdmEntityType(namespaceA, "T3", type2);
            EdmNavigationProperty nav21 = type2.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Nav21",
                ContainsTarget = false,
                Target = type1,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
            });

            EdmNavigationProperty nav22 = type2.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Nav22",
                ContainsTarget = false,
                Target = type2,
                TargetMultiplicity = EdmMultiplicity.One,
            });

            EdmNavigationProperty nav23 = type2.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Nav23",
                ContainsTarget = false,
                Target = type3,
                TargetMultiplicity = EdmMultiplicity.Many,
            });

            
            edmModel.AddElement(type1);
            edmModel.AddElement(type2);
            edmModel.AddElement(type3);

            EdmEntityContainer container = new EdmEntityContainer(namespaceA, "Con1");
            var set1 = container.AddEntitySet(namespaceA, type1);
            var set2 = container.AddEntitySet(namespaceA, type2);
            var set3 = container.AddEntitySet(namespaceA, type3);
            edmModel.AddElement(container);
            set2.AddNavigationTarget(nav21, set1);
            set2.AddNavigationTarget(nav22, set2);
            set2.AddNavigationTarget(nav23, set3);

            Model = edmModel;
            T2Type = type2;
            T3Type = type3;
            T2Set = set2;
            T3Set = set3;
        }

        #region $skip
        [Fact]
        public void SkipWithValidValue()
        {
            // Arrange & Act
            var clause = this.Run("MyContainedDog($skip=21)", PersonType, PeopleSet);

            // Assert
            Assert.NotNull(clause.SkipOption);
            Assert.Equal(21, clause.SkipOption);
        }

        [Fact]
        public void SkipWithInvalidValue()
        {
            // Arrange & Act & Assert
            Action action = () => this.Run("MyContainedDog($skip=SKIP)", PersonType, PeopleSet);
            action.Throws<ODataException>(Strings.UriSelectParser_InvalidSkipOption("SKIP"));
        }

        [Fact]
        public void SkipOnSelectWithValidValue()
        {
            // Arrange & Act
            var clause = this.RunSelect("PreviousAddresses($skip=21)", PersonType, PeopleSet);

            // Assert
            Assert.NotNull(clause.SkipOption);
            Assert.Equal(21, clause.SkipOption);
        }
        #endregion $skip

        #region $top
        [Fact]
        public void TopWithValidValue()
        {
            var clause = this.Run("MyContainedDog($top=22)", PersonType, PeopleSet);
            Assert.Equal(22, clause.TopOption);
        }

        [Fact]
        public void TopWithInvalidValue()
        {
            // Arrange & Act & Assert
            Action action = () => this.Run("MyContainedDog($top=TOP)", PersonType, PeopleSet);
            action.Throws<ODataException>(Strings.UriSelectParser_InvalidTopOption("TOP"));

            // Arrange & Act & Assert
            action = () => this.Run("MyContainedDog($top=-1)", PersonType, PeopleSet);
            action.Throws<ODataException>(Strings.UriSelectParser_InvalidTopOption("-1"));
        }

        [Fact]
        public void TopOnSelectWithValidValue()
        {
            // Arrange & Act
            var clause = this.RunSelect("PreviousAddresses($top=22)", PersonType, PeopleSet);

            // Assert
            Assert.NotNull(clause.TopOption);
            Assert.Equal(22, clause.TopOption);
        }

        [Fact]
        public void TopOnSelectWithInvalidValue()
        {
            // Arrange & Act & Assert
            Action action = () => this.RunSelect("PreviousAddresses($top=TOP)", PersonType, PeopleSet);
            action.Throws<ODataException>(Strings.UriSelectParser_InvalidTopOption("TOP"));

            // Arrange & Act & Assert
            action = () => this.RunSelect("PreviousAddresses($top=-1)", PersonType, PeopleSet);
            action.Throws<ODataException>(Strings.UriSelectParser_InvalidTopOption("-1"));
        }
        #endregion $top

        #region $count
        [Fact]
        public void CountWithValidValue()
        {
            var clause = this.Run("MyContainedDog($count=true)", PersonType, PeopleSet);
            Assert.True(clause.CountOption);
        }

        [Fact]
        public void CountWithInvalidValue()
        {
            Action action = () => this.Run("MyContainedDog($count=COUNT)", PersonType, PeopleSet);
            action.Throws<ODataException>(Strings.UriSelectParser_InvalidCountOption("COUNT"));

            action = () => this.Run("MyContainedDog($count=-2)", PersonType, PeopleSet);
            action.Throws<ODataException>(Strings.UriSelectParser_InvalidCountOption("-2"));
        }
        #endregion $count

        #region $levels
        [Fact]
        public void LevelsWithValueMax()
        {
            var clause = this.Run("Fully.Qualified.Namespace.Manager/DirectReports($levels=max)", PersonType, PeopleSet);
            Assert.True(clause.LevelsOption.IsMaxLevel);
        }

        [Fact]
        public void LevelsWithPositiveValue()
        {
            var clause = this.Run("Fully.Qualified.Namespace.Manager/DirectReports($levels=6)", PersonType, PeopleSet);
            Assert.False(clause.LevelsOption.IsMaxLevel);
            Assert.Equal(6, clause.LevelsOption.Level);
        }

        [Fact]
        public void LevelsWithZeroValueShouldWork()
        {
            var clause = this.Run("Fully.Qualified.Namespace.Manager/DirectReports($levels=0)", PersonType, PeopleSet);
            Assert.False(clause.LevelsOption.IsMaxLevel);
            Assert.Equal(0, clause.LevelsOption.Level);
        }

        [Fact]
        public void LevelsWithNegativeValueShouldThrow()
        {
            Action action = () => this.Run("Fully.Qualified.Namespace.Manager/DirectReports($levels=-1)", PersonType, PeopleSet);
            action.Throws<ODataException>(Strings.UriSelectParser_InvalidLevelsOption("-1"));
        }

        [Fact]
        public void LevelsWithInvalidValue()
        {
            Action action = () => this.Run("Fully.Qualified.Namespace.Manager/DirectReports($levels=LEVEL)", PersonType, PeopleSet);
            action.Throws<ODataException>(Strings.UriSelectParser_InvalidLevelsOption("LEVEL"));
        }

        [Fact]
        public void LevelsOnInvalidNavigationProperty()
        {
            Action action = () => this.Run("MyPaintings($levels=6)", PersonType, PeopleSet);
            action.Throws<ODataException>(Strings.ExpandItemBinder_LevelsNotAllowedOnIncompatibleRelatedType("MyPaintings", "Fully.Qualified.Namespace.Painting", "Fully.Qualified.Namespace.Person"));
        }
        #endregion $levels

        #region $levels wiht new type system
        [Fact]
        public void LevelsOnNavigationPropertyOfBaseType()
        {
            var clause = this.RunWithLocalModel("Nav21($levels=1)", T2Type, T2Set);
            Assert.Equal(1, clause.LevelsOption.Level);
        }

        [Fact]
        public void LevelsOnNavigationPropertyOfSameType()
        {
            var clause = this.RunWithLocalModel("Nav22($levels=2)", T2Type, T2Set);
            Assert.Equal(2, clause.LevelsOption.Level);
        }

        [Fact]
        public void LevelsOnNavigationPropertyOfDerivedType()
        {
            var clause = this.RunWithLocalModel("Nav23($levels=3)", T2Type, T2Set);
            Assert.Equal(3, clause.LevelsOption.Level);
        }

        [Fact]
        public void LevelsOnNavigationPropertyOfBaseTypeWithTypeCast()
        {
            var clause = this.RunWithLocalModel("NS.T3/Nav21($levels=4)", T2Type, T2Set);
            Assert.Equal(4, clause.LevelsOption.Level);
        }

        [Fact]
        public void LevelsOnNavigationPropertyOfSameTypeWithTypeCast()
        {
            var clause = this.RunWithLocalModel("NS.T3/Nav22($levels=5)", T2Type, T2Set);
            Assert.Equal(5, clause.LevelsOption.Level);
        }

        [Fact]
        public void LevelsOnNavigationPropertyOfDerivedTypeWithTypeCast()
        {
            var clause = this.RunWithLocalModel("NS.T2/Nav23($levels=6)", T3Type, T3Set);
            Assert.Equal(6, clause.LevelsOption.Level);
        }
        #endregion

        #region $compute
        [Fact]
        public void ComputeWithValidExpression()
        {
            var clause = this.Run("MyFriendsDogs($compute=Weight mul 8 as Ratio)", PersonType, PeopleSet);
            Assert.NotNull(clause.ComputeOption);
        }

        [Fact]
        public void ComputeWithInvalidExpression()
        {
            Action action = () => this.Run("MyFriendsDogs($compute=Invalid Expression)", PersonType, PeopleSet);
            var exception = Assert.Throws<ODataException>(action);
            Assert.Equal("'as' expected at position 8 in 'Invalid Expression'.", exception.Message);
        }
        #endregion $compute

        #region $apply
        [Fact]
        public void ApplyWithValidExpression()
        {
            var clause = this.Run("MyFriendsDogs($apply=aggregate(Weight with sum as Total))", PersonType, PeopleSet);
            Assert.NotNull(clause.ApplyOption);
        }

        [Fact]
        public void ApplyWithInvalidExpression()
        {
            Action action = () => this.Run("MyFriendsDogs($apply=Invalid Expression)", PersonType, PeopleSet);
            var exception = Assert.Throws<ODataException>(action);
            Assert.Equal("'aggregate|filter|groupby|compute|expand' expected at position 0 in 'Invalid Expression'.", exception.Message);
        }
        #endregion $apply

        #region helper methods
        private ExpandedNavigationSelectItem Run(string expandStr, IEdmStructuredType elementType, IEdmNavigationSource navigationSource)
        {
            return new ODataQueryOptionParser(HardCodedTestModel.TestModel, elementType, navigationSource, new Dictionary<string, string> { { "$expand", expandStr } })
                .ParseSelectAndExpand().SelectedItems.First() as ExpandedNavigationSelectItem;
        }

        private PathSelectItem RunSelect(string selectStr, IEdmStructuredType elementType, IEdmNavigationSource navigationSource)
        {
            return new ODataQueryOptionParser(HardCodedTestModel.TestModel, elementType, navigationSource, new Dictionary<string, string> { { "$select", selectStr } })
                .ParseSelectAndExpand().SelectedItems.First() as PathSelectItem;
        }

        private ExpandedNavigationSelectItem RunWithLocalModel(string expandStr, IEdmStructuredType elementType, IEdmNavigationSource navigationSource)
        {
            return new ODataQueryOptionParser(Model, elementType, navigationSource, new Dictionary<string, string> { { "$expand", expandStr } })
                .ParseSelectAndExpand().SelectedItems.First() as ExpandedNavigationSelectItem;
        }
        #endregion helper methods
    }
}
