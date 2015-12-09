//---------------------------------------------------------------------
// <copyright file="ExapndOptionFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
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
            var clause = this.Run("MyContainedDog($skip=21)", PersonType, PeopleSet);
            clause.SkipOption.Should().Be(21);
        }

        [Fact]
        public void SkipWithInvalidValue()
        {
            Action action = () => this.Run("MyContainedDog($skip=SKIP)", PersonType, PeopleSet);
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidSkipOption("SKIP"));
        }
        #endregion $skip

        #region $top
        [Fact]
        public void TopWithValidValue()
        {
            var clause = this.Run("MyContainedDog($top=22)", PersonType, PeopleSet);
            clause.TopOption.Should().Be(22);
        }

        [Fact]
        public void TopWithInvalidValue()
        {
            Action action = () => this.Run("MyContainedDog($top=TOP)", PersonType, PeopleSet);
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidTopOption("TOP"));

            action = () => this.Run("MyContainedDog($top=-1)", PersonType, PeopleSet);
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidTopOption("-1"));
        }
        #endregion $top

        #region $count
        [Fact]
        public void CountWithValidValue()
        {
            var clause = this.Run("MyContainedDog($count=true)", PersonType, PeopleSet);
            clause.CountOption.Should().BeTrue();
        }

        [Fact]
        public void CountWithInvalidValue()
        {
            Action action = () => this.Run("MyContainedDog($count=COUNT)", PersonType, PeopleSet);
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidCountOption("COUNT"));

            action = () => this.Run("MyContainedDog($count=-2)", PersonType, PeopleSet);
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidCountOption("-2"));
        }
        #endregion $count

        #region $levels
        [Fact]
        public void LevelsWithValueMax()
        {
            var clause = this.Run("Fully.Qualified.Namespace.Manager/DirectReports($levels=max)", PersonType, PeopleSet);
            clause.LevelsOption.IsMaxLevel.Should().BeTrue();
        }

        [Fact]
        public void LevelsWithPositiveValue()
        {
            var clause = this.Run("Fully.Qualified.Namespace.Manager/DirectReports($levels=6)", PersonType, PeopleSet);
            clause.LevelsOption.IsMaxLevel.Should().BeFalse();
            clause.LevelsOption.Level.Should().Be(6);
        }

        [Fact]
        public void LevelsWithZeroValueShouldWork()
        {
            var clause = this.Run("Fully.Qualified.Namespace.Manager/DirectReports($levels=0)", PersonType, PeopleSet);
            clause.LevelsOption.IsMaxLevel.Should().BeFalse();
            clause.LevelsOption.Level.Should().Be(0);
        }

        [Fact]
        public void LevelsWithNegativeValueShouldThrow()
        {
            Action action = () => this.Run("Fully.Qualified.Namespace.Manager/DirectReports($levels=-1)", PersonType, PeopleSet);
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidLevelsOption("-1"));
        }

        [Fact]
        public void LevelsWithInvalidValue()
        {
            Action action = () => this.Run("Fully.Qualified.Namespace.Manager/DirectReports($levels=LEVEL)", PersonType, PeopleSet);
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidLevelsOption("LEVEL"));
        }

        [Fact]
        public void LevelsOnInvalidNavigationProperty()
        {
            Action action = () => this.Run("MyPaintings($levels=6)", PersonType, PeopleSet);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpandItemBinder_LevelsNotAllowedOnIncompatibleRelatedType("MyPaintings", "Fully.Qualified.Namespace.Painting", "Fully.Qualified.Namespace.Person"));
        }
        #endregion $levels

        #region $levels wiht new type system
        [Fact]
        public void LevelsOnNavigationPropertyOfBaseType()
        {
            var clause = this.RunWithLocalModel("Nav21($levels=1)", T2Type, T2Set);
            clause.LevelsOption.Level.Should().Be(1);
        }

        [Fact]
        public void LevelsOnNavigationPropertyOfSameType()
        {
            var clause = this.RunWithLocalModel("Nav22($levels=2)", T2Type, T2Set);
            clause.LevelsOption.Level.Should().Be(2);
        }

        [Fact]
        public void LevelsOnNavigationPropertyOfDerivedType()
        {
            var clause = this.RunWithLocalModel("Nav23($levels=3)", T2Type, T2Set);
            clause.LevelsOption.Level.Should().Be(3);
        }

        [Fact]
        public void LevelsOnNavigationPropertyOfBaseTypeWithTypeCast()
        {
            var clause = this.RunWithLocalModel("NS.T3/Nav21($levels=4)", T2Type, T2Set);
            clause.LevelsOption.Level.Should().Be(4);
        }

        [Fact]
        public void LevelsOnNavigationPropertyOfSameTypeWithTypeCast()
        {
            var clause = this.RunWithLocalModel("NS.T3/Nav22($levels=5)", T2Type, T2Set);
            clause.LevelsOption.Level.Should().Be(5);
        }

        [Fact]
        public void LevelsOnNavigationPropertyOfDerivedTypeWithTypeCast()
        {
            var clause = this.RunWithLocalModel("NS.T2/Nav23($levels=6)", T3Type, T3Set);
            clause.LevelsOption.Level.Should().Be(6);
        }
        #endregion

        #region helper methods
        private ExpandedNavigationSelectItem Run(string expandStr, IEdmStructuredType elementType, IEdmNavigationSource navigationSource)
        {
            return new ODataQueryOptionParser(HardCodedTestModel.TestModel, elementType, navigationSource, new Dictionary<string, string> { { "$expand", expandStr } }).ParseSelectAndExpand().SelectedItems.First() as ExpandedNavigationSelectItem;
        }

        private ExpandedNavigationSelectItem RunWithLocalModel(string expandStr, IEdmStructuredType elementType, IEdmNavigationSource navigationSource)
        {
            return new ODataQueryOptionParser(Model, elementType, navigationSource, new Dictionary<string, string> { { "$expand", expandStr } }).ParseSelectAndExpand().SelectedItems.First() as ExpandedNavigationSelectItem;
        }
        #endregion helper methods
    }
}
