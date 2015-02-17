//---------------------------------------------------------------------
// <copyright file="ExpandAndSelectPathExtractingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExpandAndSelectPathExtractingTests
    {
        private readonly EdmModel model;
        private readonly EdmEntityType baseType;
        private readonly EdmEntityType derivedType;
        private readonly EdmEntitySet entitySet;

        public ExpandAndSelectPathExtractingTests()
        {
            this.baseType = new EdmEntityType("FQ.NS", "Base");
            this.baseType.AddKeys(this.baseType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var baseNavigation1 = this.baseType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Navigation1", Target = this.baseType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            var baseNavigation2 = this.baseType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Navigation2", Target = this.baseType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

            this.derivedType = new EdmEntityType("FQ.NS", "Derived", this.baseType);
            this.derivedType.AddStructuralProperty("Derived", EdmPrimitiveTypeKind.Int32);
            var derivedNavigation = this.derivedType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "DerivedNavigation", Target = this.derivedType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

            var container = new EdmEntityContainer("FQ.NS", "Container");
            this.entitySet = container.AddEntitySet("Entities", this.baseType);
            this.entitySet.AddNavigationTarget(baseNavigation1, this.entitySet);
            this.entitySet.AddNavigationTarget(baseNavigation2, this.entitySet);
            this.entitySet.AddNavigationTarget(derivedNavigation, this.entitySet);

            this.model = new EdmModel();
            this.model.AddElement(this.baseType);
            this.model.AddElement(this.derivedType);
            this.model.AddElement(container);
        }

        [TestMethod]
        public void EmptyExpand()
        {
            this.ParseAndExtract(string.Empty, null);
        }

        [TestMethod]
        public void EmptyExpandWithSelect()
        {
            this.ParseAndExtract(string.Empty, "Id");
        }

        [TestMethod]
        public void WideExpand()
        {
            const string expandClauseText = "Navigation1,Navigation2";
            const string expectedSelect = "";
            this.ParseAndExtract(expandClauseText, expectedSelectClauseFromOM: expectedSelect);
        }

        [TestMethod]
        public void DeepExpand()
        {
            const string expandClauseText = "Navigation1($expand=Navigation2($expand=Navigation1))";
            const string expectedExpandClauseText = "Navigation1($expand=Navigation2($expand=Navigation1))";
            const string expectedSelectClause = "";
            this.ParseAndExtract(expandClauseText, null, expectedExpandClauseText, expectedSelectClause);
        }

        [TestMethod]
        public void MultiLevelExpand()
        {
            const string expandClauseText = "Navigation1,Navigation2($expand=Navigation1)";
            const string expectedExpandClause = "Navigation1,Navigation2($expand=Navigation1)";
            const string expectedSelectClause = "";
            this.ParseAndExtract(expandClauseText, expectedExpandClauseFromOM: expectedExpandClause, expectedSelectClauseFromOM: expectedSelectClause);
        }

        [TestMethod]
        public void DeepThenWideExpand()
        {
            // Note that the SelectExpandClause.GetV3SelectExpandPaths() will always put the paths in a different order than the original clause.
            const string expandClauseText = "Navigation1($expand=Navigation1($expand=Navigation1)), Navigation1($expand=Navigation2($expand=Navigation1)), Navigation1($expand=Navigation2($expand=Navigation2))";
            const string expectedExpandClauseText = "Navigation1($expand=Navigation2($expand=Navigation1,Navigation2),Navigation1($expand=Navigation1))";
            const string expectedSelectClauseText = "";
            this.ParseAndExtract(expandClauseText, expectedExpandClauseFromOM: expectedExpandClauseText, expectedSelectClauseFromOM: expectedSelectClauseText);
        }

        [TestMethod]
        public void ExpandAtAllLevelsOfDeepPathShouldBeCollapsed()
        {
            const string expandClauseText = "Navigation1,Navigation1($expand=Navigation2),Navigation1($expand=Navigation2($expand=Navigation1))";
            const string expectedExpand = "Navigation1($expand=Navigation2($expand=Navigation1))";
            const string expectedSelect = "";
            this.ParseAndExtract(expandClauseText, expectedExpandClauseFromOM: expectedExpand, expectedSelectClauseFromOM: expectedSelect);
        }

        [TestMethod]
        public void ExpandWithTypeSegment()
        {
            const string expandClauseText = "FQ.NS.Derived/Navigation1";
            this.ParseAndExtract(expandClauseText, expectedSelectClauseFromOM: "");
        }

        [TestMethod]
        public void DeepExpandWithTypeSegment()
        {
            const string expandClauseText = "Navigation1($expand=FQ.NS.Derived/Navigation2)";
            const string expectedSelectClause = "";
            const string expectedExpandClause = "Navigation1($expand=FQ.NS.Derived/Navigation2)";
            this.ParseAndExtract(expandClauseText, null, expectedExpandClause, expectedSelectClause);
        }

        [TestMethod]
        public void EmptySelect()
        {
            this.ParseAndExtract(null, string.Empty);
        }

        [TestMethod]
        public void EmptySelectWithExpand()
        {
            const string expectedSelect = "";
            this.ParseAndExtract("Navigation1", string.Empty, null, expectedSelect);
        }

        [TestMethod]
        public void SelectSingleProperty()
        {
            const string selectClauseText = "Id";
            this.ParseAndExtract(selectClauseText: selectClauseText);
        }

        [TestMethod]
        public void SelectSinglePropertyWithTypeSegment()
        {
            const string selectClauseText = "FQ.NS.Derived/Id";
            this.ParseAndExtract(selectClauseText: selectClauseText);
        }

        [TestMethod]
        public void SelectNavigationProperty()
        {
            const string selectClauseText = "Navigation1";
            this.ParseAndExtract(selectClauseText: selectClauseText);
        }

        [TestMethod]
        public void SelectNavigationPropertyWithTypeSegment()
        {
            const string selectClauseText = "FQ.NS.Derived/Navigation1";
            this.ParseAndExtract(selectClauseText: selectClauseText);
        }

        [TestMethod]
        public void SelectNavigationPropertiesThatAreAlsoExpanded()
        {
            const string expandClauseText = "Navigation1($expand=Navigation1($select=Navigation2))";
            const string expectedExpandClauseText = "Navigation1($expand=Navigation1($select=Navigation2))";
            const string expectedSelectClauseText = "";
            this.ParseAndExtract(expandClauseText: expandClauseText, expectedExpandClauseFromOM: expectedExpandClauseText, expectedSelectClauseFromOM: expectedSelectClauseText);
        }

        [TestMethod]
        public void SelectPrimitiveAndNavigationPropertyThatIsAlsoExpanded()
        {
            const string selectClauseText = "Id,Navigation1";
            const string expectedSelectText = "Id,Navigation1";
            this.ParseAndExtract(selectClauseText: selectClauseText, expandClauseText: "Navigation1", expectedSelectClauseFromOM: expectedSelectText);
        }

        [TestMethod]
        public void SelectDuplicateProperty()
        {
            const string selectClauseText = "Id,Id";
            const string expectedSelectClauseText = "Id";
            this.ParseAndExtract(selectClauseText: selectClauseText, expandClauseText: null, expectedSelectClauseFromOM: expectedSelectClauseText, expectedExpandClauseFromOM: null);
        }

        [TestMethod]
        public void SelectPrimitiveAndNavigationPropertyThatIsNotExpanded()
        {
            const string selectClauseText = "Id,Navigation1";
            this.ParseAndExtract(selectClauseText: selectClauseText);
        }

        [TestMethod]
        public void SelectWildcardUnderNavigation()
        {
            const string expandClauseText = "Navigation1($select=*)";
            const string expectedSelectText = "";
            const string expectedExpandText = "Navigation1($select=*)";
            this.ParseAndExtract(expandClauseText: expandClauseText, expectedExpandClauseFromOM: expectedExpandText, expectedSelectClauseFromOM: expectedSelectText);
        }

        [TestMethod]
        public void SelectAndExpandWithTypeSegments()
        {
            const string selectClauseText = "FQ.NS.Derived/Navigation1";
            this.ParseAndExtract(selectClauseText: selectClauseText, expandClauseText: selectClauseText);
        }

        [TestMethod]
        public void SelectAndExpandWithTypeSegments2()
        {
            const string expandClauseText = "FQ.NS.Derived/Navigation1($select=FQ.NS.Derived/Derived,FQ.NS.Derived/Navigation2)";
            const string expectedExpandText = "FQ.NS.Derived/Navigation1($select=FQ.NS.Derived/Derived,FQ.NS.Derived/Navigation2)";
            const string expectedSelectText = "";
            this.ParseAndExtract(expandClauseText, expectedExpandClauseFromOM: expectedExpandText, expectedSelectClauseFromOM: expectedSelectText);
        }

        [TestMethod]
        public void NestedExpandWithDuplicteSelectTest()
        {
            const string expandClauseText = "Navigation1($select=Id),Navigation1($select=Id)";
            const string expectedExpandText = "Navigation1($select=Id)";
            const string expectedSelectText = "";
            this.ParseAndExtract(expandClauseText, expectedExpandClauseFromOM: expectedExpandText, expectedSelectClauseFromOM: expectedSelectText);
        }

        [TestMethod]
        public void ExpandNavigationThatIsNotSelectedAutomaticallySelectsLink()
        {
            const string selectClauseText = "Navigation2";
            const string expandClauseText = "Navigation1";
            const string expectedSelectClause = "Navigation2,Navigation1";
            this.ParseAndExtract(selectClauseText: selectClauseText, expandClauseText: expandClauseText, expectedSelectClauseFromOM: expectedSelectClause, expectedExpandClauseFromOM: "Navigation1");
        }

        [TestMethod]
        public void ExpandNavigationThatIsNotSelectedInNonTopLevelAutomaticallySelectsLink()
        {
            const string expandClauseText = "Navigation1($select=Navigation1;$expand=Navigation2)";
            const string expectedExpandClauseText = "Navigation1($select=Navigation1,Navigation2;$expand=Navigation2)";
            this.ParseAndExtract(selectClauseText: null, expandClauseText: expandClauseText, expectedSelectClauseFromOM: null, expectedExpandClauseFromOM: expectedExpandClauseText);
        }

        [TestMethod]
        public void SelectPropertiesDefinedOnDerivedTypesWithoutRepeatingTypeSegments()
        {
            const string expandClauseText = "FQ.NS.Derived/DerivedNavigation($expand=DerivedNavigation($select=Derived))";
            const string expectedExpandClause = "FQ.NS.Derived/DerivedNavigation($expand=DerivedNavigation($select=Derived))";
            const string expectedSelect = "";
            this.ParseAndExtract(expandClauseText: expandClauseText, expectedExpandClauseFromOM: expectedExpandClause, expectedSelectClauseFromOM: expectedSelect);
        }

        private void ParseAndExtract(string expandClauseText = null, string selectClauseText = null, string expectedExpandClauseFromOM = null, string expectedSelectClauseFromOM = null)
        {
            var expandClause = new ODataQueryOptionParser(this.model, this.baseType, this.entitySet, new Dictionary<string, string> { { "$expand", expandClauseText }, { "$select", selectClauseText } }).ParseSelectAndExpand();

            // Verify that the extension method gets the same result as the path extractor.
            string selectTextFromOM, expandTextFromOM;
            expandClause.GetSelectExpandPaths(out selectTextFromOM, out expandTextFromOM);
            selectTextFromOM.Should().Be(expectedSelectClauseFromOM ?? (selectClauseText ?? string.Empty));
            expandTextFromOM.Should().Be(expectedExpandClauseFromOM ?? (expandClauseText ?? string.Empty));
        }
    }
}
