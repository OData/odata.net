//---------------------------------------------------------------------
// <copyright file="SelectedPropertiesNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.Edm;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.Evaluation
{
    public class SelectedPropertiesNodeTests
    {
        private static readonly SelectedPropertiesNode EntireSubtreeNode = SelectedPropertiesNode.Create((string)null);
        private static readonly SelectedPropertiesNode EmptyNode = SelectedPropertiesNode.Create(string.Empty);

        private EdmModel edmModel;
        private EdmEntityType cityType;
        private EdmEntityType townType;
        private EdmEntityType metropolisType;
        private EdmEntityType districtType;
        private EdmEntityContainer defaultContainer;
        private EdmAction action;
        private EdmAction actionConflictingWithPropertyName;
        private EdmActionImport actionImport;
        private EdmActionImport actionImportConflictingWithPropertyName;
        private EdmEntityType openType;

        public SelectedPropertiesNodeTests()
        {
            this.edmModel = new EdmModel();

            this.defaultContainer = new EdmEntityContainer("TestModel", "DefaultContainer");
            this.edmModel.AddElement(this.defaultContainer);

            this.townType = new EdmEntityType("TestModel", "Town");
            this.edmModel.AddElement(townType);
            EdmStructuralProperty townIdProperty = townType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            townType.AddKeys(townIdProperty);
            townType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(/*isNullable*/false));
            townType.AddStructuralProperty("Size", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));

            this.cityType = new EdmEntityType("TestModel", "City", this.townType);
            cityType.AddStructuralProperty("Photo", EdmCoreModel.Instance.GetStream(/*isNullable*/false));
            this.edmModel.AddElement(cityType);

            this.districtType = new EdmEntityType("TestModel", "District");
            EdmStructuralProperty districtIdProperty = districtType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            districtType.AddKeys(districtIdProperty);
            districtType.AddStructuralProperty("Zip", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            districtType.AddStructuralProperty("Thumbnail", EdmCoreModel.Instance.GetStream(/*isNullable*/false));
            this.edmModel.AddElement(districtType);

            cityType.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "Districts", Target = districtType, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo { Name = "City", Target = cityType, TargetMultiplicity = EdmMultiplicity.One });

            this.defaultContainer.AddEntitySet("Cities", cityType);
            this.defaultContainer.AddEntitySet("Districts", districtType);

            EdmComplexType airportType = new EdmComplexType("TestModel", "Airport");
            airportType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(/*isNullable*/false));
            airportType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "City", Target = cityType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            this.edmModel.AddElement(airportType);

            EdmComplexType regionalAirportType = new EdmComplexType("TestModel", "RegionalAirport", airportType);
            airportType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Region", Target = districtType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            this.edmModel.AddElement(regionalAirportType);

            this.metropolisType = new EdmEntityType("TestModel", "Metropolis", this.cityType);
            this.metropolisType.AddStructuralProperty("MetropolisStream", EdmCoreModel.Instance.GetStream(/*isNullable*/false));
            this.metropolisType.AddStructuralProperty("NearestAirport", new EdmComplexTypeReference(airportType,false));
            this.edmModel.AddElement(metropolisType);

            metropolisType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "MetropolisNavigation", Target = districtType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

            this.action = new EdmAction("TestModel", "Action", new EdmEntityTypeReference(this.cityType, true));
            this.action.AddParameter("Param", EdmCoreModel.Instance.GetInt32(false));
            this.edmModel.AddElement(action);
            this.actionImport = new EdmActionImport(this.defaultContainer, "Action", action);

            this.actionConflictingWithPropertyName = new EdmAction("TestModel", "Zip", new EdmEntityTypeReference(this.districtType, true));
            this.actionConflictingWithPropertyName.AddParameter("Param", EdmCoreModel.Instance.GetInt32(false));
            this.edmModel.AddElement(actionConflictingWithPropertyName);
            this.actionImportConflictingWithPropertyName = new EdmActionImport(this.defaultContainer, "Zip", actionConflictingWithPropertyName);

            this.openType = new EdmEntityType("TestModel", "OpenCity", this.cityType, false, true);
        }

        [Fact]
        public void MalformedSelectClauseShouldFail()
        {
            Action action = () => SelectedPropertiesNode.Create("*/Name");
            action.Throws<ODataException>(ErrorStrings.SelectedPropertiesNode_StarSegmentNotLastSegment);
        }

        [Fact]
        public void NoSelectClauseShouldIncludeEntireSubtree()
        {
            SelectedPropertiesNode.EntireSubtree.HaveEntireSubtree().IncludeEntireSubtree(this.cityType);
        }

        [Fact]
        public void EmptySelectClauseShouldNotIncludeAnyProperties()
        {
            SelectedPropertiesNode.Create(string.Empty).BeSameAsEmpty().BeEmpty(this.cityType);
        }

        [Fact]
        public void WhitespaceShouldBeTreatedAsEmpty()
        {
            SelectedPropertiesNode.Create(" ").BeSameAsEmpty();
        }

        [Fact]
        public void WildcardShouldSelectAllPropertiesParsingTest()
        {
            SelectedPropertiesNode.Create("*").HaveStreams(this.cityType, "Photo").HaveNavigations(this.cityType, "Districts").HaveChild(this.cityType, "Districts", c => c.BeSameAsEmpty());
        }

        [Fact]
        public void SingleStreamPropertyWithNormalProperty()
        {
            SelectedPropertiesNode.Create("Size,Photo").HaveOnlyStreams(this.cityType, "Photo");
        }

        [Fact]
        public void SpecifyingTheSameStreamTwiceShouldNotCauseDuplicates()
        {
            SelectedPropertiesNode.Create("Photo,Photo").HaveOnlyStreams(this.cityType, "Photo");
        }

        [Fact]
        public void SelectingANavigationShouldSelectTheEntireTree()
        {
            SelectedPropertiesNode.Create("Districts").HaveOnlyNavigations(this.cityType, "Districts").HaveChild(this.cityType, "Districts", c => c.HaveEntireSubtree());
        }

        [Fact]
        public void SpecifyingTheSameNavigationTwiceShouldNotCauseDuplicates()
        {
            SelectedPropertiesNode.Create("Districts,Districts").HaveOnlyNavigations(this.cityType, "Districts").HaveChild(this.cityType, "Districts", c => c.HaveEntireSubtree());
        }

        [Fact]
        public void SpecifyingAWildCardShouldNotCauseDuplicates()
        {
            SelectedPropertiesNode.Create("Districts,*,Photo").HaveStreams(this.cityType, "Photo").HaveNavigations(this.cityType, "Districts").HaveChild(this.cityType, "Districts", c => c.HaveEntireSubtree());
        }

        [Fact]
        public void SelectingANavigationShouldSelectTheEntireTreeIfWildcardAlsoPresent()
        {
            SelectedPropertiesNode.Create("Districts,Districts/*").HaveOnlyNavigations(this.cityType, "Districts").HaveChild(this.cityType, "Districts", c => c.HaveEntireSubtree());
        }

        [Fact]
        public void SelectingEntireSubtreeAndSpecificPathShouldNotResultInDuplicates()
        {
            SelectedPropertiesNode.Create("Districts,Districts/Thumbnail")
                .HaveOnlyNavigations(this.cityType, "Districts")
                .HaveChild(this.cityType, "Districts", c => c.HaveEntireSubtree());
        }

        [Fact]
        public void DeepAndWideSelection1()
        {
            // this is covering 2 issues with a naive implementation:
            // 1) '*' and 'Districts' should not be collapsed, because 'Districts' is deep while '*' is wide
            // 2) 'Districts/*' should not override 'Districts'
            SelectedPropertiesNode.Create("*,Districts,Districts/*")
                .HaveStreams(this.cityType, "Photo")
                .HaveNavigations(this.cityType, "Districts")
                .HaveChild(this.cityType, "Districts", c => c.HaveEntireSubtree());
        }

        [Fact]
        public void DeepAndWideSelection2()
        {
            SelectedPropertiesNode.Create("Districts/*,Districts/City,Districts/City/*")
                .HaveOnlyNavigations(this.cityType, "Districts")
                .HaveChild(
                    this.cityType,
                    "Districts",
                    c => c.HaveStreams(this.districtType, "Thumbnail")
                        .HaveChild(this.districtType, "City", c2 => c2.HaveEntireSubtree()));
        }

        [Fact]
        public void WildcardAfterNavigationShouldNotSelectTheEntireTree()
        {
            SelectedPropertiesNode.Create("Districts/*")
                .HaveOnlyNavigations(this.cityType, "Districts")
                .HaveChild(
                    this.cityType,
                    "Districts",
                    c => c.HaveStreams(this.districtType, "Thumbnail")
                        .HaveNavigations(this.districtType, "City"));
        }

        [Fact]
        public void SpecificDeepPaths()
        {
            SelectedPropertiesNode.Create("Districts/Thumbnail,Districts/Zip,Districts/City/Photo")
                .HaveOnlyNavigations(this.cityType, "Districts")
                .HaveChild(
                    this.cityType,
                    "Districts",
                    c => c.HaveStreams(this.districtType, "Thumbnail")
                        .HaveNavigations(this.districtType, "City")
                        .HaveChild(this.districtType, "City", c2 => c2.HaveOnlyStreams(this.cityType, "Photo")));
        }

        [Fact]
        public void SimpleTypeSegmentWithStream()
        {
            SelectedPropertiesNode.Create("TestModel.City/Photo").HaveOnlyStreams(this.cityType, "Photo");
        }

        [Fact]
        public void SimpleTypeSegmentWithNavigation()
        {
            SelectedPropertiesNode.Create("TestModel.City/Districts").HaveOnlyNavigations(this.cityType, "Districts");
        }

        [Fact]
        public void UnmatchedTypeSegments()
        {
            SelectedPropertiesNode.Create("TestModel.Fake/Photo,TestModel.Fake2/Districts").NotHaveStreams(this.cityType).NotHaveNavigations(this.cityType);
        }

        [Fact]
        public void TypeSegmentWithStreamUsingBaseTypeName()
        {
            SelectedPropertiesNode.Create("TestModel.City/Photo").HaveOnlyStreams(this.metropolisType, "Photo");
        }

        [Fact]
        public void TypeSegmentWithNavigationUsingBaseTypeName()
        {
            SelectedPropertiesNode.Create("TestModel.City/Districts").HaveOnlyNavigations(this.metropolisType, "Districts");
        }

        [Fact]
        public void TypeSegmentWithWildcardShouldFailWhenGettingStreams()
        {
            var node = SelectedPropertiesNode.Create("TestModel.City/*");
            Action action = () => node.GetSelectedStreamProperties(this.cityType);
            action.Throws<ODataException>(ErrorStrings.SelectedPropertiesNode_StarSegmentAfterTypeSegment);
        }

        [Fact]
        public void TypeSegmentWithWildcardShouldFailWhenGettingNavigations()
        {
            var node = SelectedPropertiesNode.Create("TestModel.City/*");
            Action action = () => node.GetSelectedNavigationProperties(this.cityType);
            action.Throws<ODataException>(ErrorStrings.SelectedPropertiesNode_StarSegmentAfterTypeSegment);
        }

        [Fact]
        public void TypeSegmentWithWildcardShouldFailWhenRecursing()
        {
            var node = SelectedPropertiesNode.Create("TestModel.City/*");
            Action action = () => node.GetSelectedPropertiesForNavigationProperty(this.cityType, "Districts");
            action.Throws<ODataException>(ErrorStrings.SelectedPropertiesNode_StarSegmentAfterTypeSegment);
        }

        [Fact]
        public void DeepTypeSegment()
        {
            SelectedPropertiesNode.Create("Districts/City/TestModel.Metropolis/MetropolisStream")
                .HaveChild(
                    this.cityType,
                    "Districts",
                    c => c.HaveChild(
                        this.districtType,
                        "City",
                        c2 => c2.HaveStreams(this.metropolisType, "MetropolisStream")));
        }

        [Fact]
        public void MultipleTypeSegmentsShouldNotProduceDuplicateStreams()
        {
            SelectedPropertiesNode.Create("TestModel.Town/Photo,TestModel.City/Photo").HaveOnlyStreams(this.cityType, "Photo");
        }

        [Fact]
        public void MultipleTypeSegmentsShouldNotProduceDuplicateNavigations()
        {
            SelectedPropertiesNode.Create("TestModel.City/Districts,TestModel.Metropolis/Districts").HaveOnlyNavigations(this.metropolisType, "Districts");
        }

        [Fact]
        public void StreamSpecifiedInBothTypeSegmentAndDirectlyShouldNotProduceDuplicates()
        {
            SelectedPropertiesNode.Create("TestModel.City/Photo,Photo").HaveOnlyStreams(this.cityType, "Photo");
        }

        [Fact]
        public void NavigationSpecifiedInBothTypeSegmentAndDirectlyShouldNotProduceDuplicates()
        {
            SelectedPropertiesNode.Create("TestModel.City/Districts,Districts").HaveOnlyNavigations(this.cityType, "Districts");
        }

        [Fact]
        public void SelectTokenForNavigationPropertyOnComplexShouldHaveOnlySelectedItem()
        {
            SelectedPropertiesNode.Create("NearestAirport/Districts").HaveChild(this.metropolisType, "NearestAirport", c => c.HaveNavigations(this.cityType, "Districts"));
        }

        [Fact]
        public void SubPropertyOfNavigationSpecifiedInBothTypeSegmentAndDirectlyShouldNotProduceDuplicates()
        {
            SelectedPropertiesNode.Create("TestModel.City/Districts/Thumbnail,Districts/Thumbnail")
                .HaveOnlyNavigations(this.cityType, "Districts")
                .HaveChild(this.cityType, "Districts", c=> c.HaveOnlyStreams(this.districtType, "Thumbnail"));
        }

        [Fact]
        public void CombiningEntireSubtreeWithAnythingShouldReturnEntireSubtree()
        {
            SelectedPropertiesNode.CombineNodes(EntireSubtreeNode, EntireSubtreeNode).HaveEntireSubtree();

            this.VerifyCombination(EmptyNode, EntireSubtreeNode, n => n.HaveEntireSubtree());
            this.VerifyCombination(SelectedPropertiesNode.Create("*"), EntireSubtreeNode, n => n.HaveEntireSubtree());
        }

        [Fact]
        public void CombiningEmptyWithAnythingShouldReturnTheOtherThing()
        {
            SelectedPropertiesNode.CombineNodes(EmptyNode, EmptyNode).BeSameAsEmpty();

            SelectedPropertiesNode nonEmpty = SelectedPropertiesNode.Create("*");
            this.VerifyCombination(EmptyNode, nonEmpty, n => Assert.Same(n, nonEmpty));
        }

        [Fact]
        public void CombiningPropertyWithWildcardShouldBeWildcard()
        {
            SelectedPropertiesNode left = SelectedPropertiesNode.Create("*");
            SelectedPropertiesNode right = SelectedPropertiesNode.Create("Fake");
            this.VerifyCombination(left, right, n => n.HaveStreams(this.cityType, "Photo").HaveNavigations(this.cityType, "Districts"));
        }

        [Fact]
        public void CombiningPartialNodesShouldCombineProperties()
        {
            var left = SelectedPropertiesNode.Create("Photo");
            var right = SelectedPropertiesNode.Create("Districts/Thumbnail");

            Action<SelectedPropertiesNode> verify = n => n
                .HaveStreams(this.cityType, "Photo")
                .HaveNavigations(this.cityType, "Districts")
                .HaveChild(this.cityType, "Districts", c => c.HaveOnlyStreams(this.districtType, "Thumbnail"));

            this.VerifyCombination(left, right, verify);
        }

        [Fact]
        public void CombiningDeepPartialNodesShouldCombineRecursively()
        {
            var left = SelectedPropertiesNode.Create("Districts/City/Districts");
            var right = SelectedPropertiesNode.Create("Districts/Thumbnail");

            Action<SelectedPropertiesNode> verify = n => n
                .HaveOnlyNavigations(this.cityType, "Districts")
                .HaveChild(
                    this.cityType,
                    "Districts",
                    c => c.HaveStreams(this.districtType, "Thumbnail")
                        .HaveNavigations(this.districtType, "City")
                        .HaveChild(this.districtType, "City", c2 => c2.HaveOnlyNavigations(this.cityType, "Districts")));

            this.VerifyCombination(left, right, verify);
        }

        [Fact]
        public void EmptySelectionShouldNotIncludeActions()
        {
            EmptyNode.NotHaveAction(this.cityType, this.action);
        }

        [Fact]
        public void EntireSubtreeShouldIncludeActions()
        {
            EntireSubtreeNode.HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void WildCardShouldNotIncludeActions()
        {
            SelectedPropertiesNode.Create("*").NotHaveAction(this.cityType, this.action);
        }

        [Fact]
        public void SpecificActionShouldBeNotSelectedForNonMatchingName()
        {
            SelectedPropertiesNode.Create("foo").NotHaveAction(this.cityType, this.action);
        }

        [Fact]
        public void SpecificActionShouldBeSelected()
        {
            SelectedPropertiesNode.Create("Action").HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void SpecificUnqualifedActionWithParametersShouldNotBeSelected()
        {
            SelectedPropertiesNode.Create("Action(Edm.Int32)").NotHaveAction(this.cityType, this.action);
        }

        [Fact]
        public void SpecificQualifiedActionWithParametersShouldBeSelected()
        {
            SelectedPropertiesNode.Create("TestModel.Action(Edm.Int32)").HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void NamespaceQualifiedActionNameShouldBeSelected()
        {
            SelectedPropertiesNode.Create("TestModel.Action").HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void NamespaceQualifiedWildcardShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.*").HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void UnQualifiedNameWithParametersShouldNotIncludeActionOnOpenType()
        {
            SelectedPropertiesNode.Create("Action(Edm.Int32)", this.openType, this.edmModel).NotHaveAction(this.openType, this.action);
        }

        [Fact]
        public void ExpandTokenWithoutTypeInfoShouldHaveEntireSubtree()
        {
            SelectedPropertiesNode.Create("Districts()").HaveEntireSubtree();
        }

        [Fact]
        public void ExpandTokenForNavigationPropertyShouldNotHaveEntireSubtree()
        {
            SelectedPropertiesNode.Create("MetropolisNavigation(Thumbnail)", this.metropolisType, this.edmModel).HaveEntireSubtree(false);
        }

       [Fact]
        public void SelectedPropertyAndExpandTokenShouldHavePartialSubtree()
        {
            SelectedPropertiesNode.Create("MetropolisStream,MetropolisNavigation(Thumbnail)", this.metropolisType, this.edmModel).HaveEntireSubtree(false);
            SelectedPropertiesNode.Create("MetropolisStream,MetropolisNavigation(Thumbnail)").HaveEntireSubtree(false);
        }

        [Fact]
        public void ExpandTokenForCollectionOfNavigationPropertyShouldHaveEntireSubtree()
        {
            SelectedPropertiesNode.Create("Districts()", this.cityType, this.edmModel).HaveEntireSubtree();
        }

        [Fact]
        public void ExpandTokenForNavigationPropertyShouldHaveEntireSubtree()
        {
            SelectedPropertiesNode.Create("MetropolisNavigation()", this.metropolisType, this.edmModel).HaveEntireSubtree();
        }

        [Fact]
        public void ExpandTokenForNavigationPropertyOnDerivedTypeShouldHaveEntireSubtree()
        {
            SelectedPropertiesNode.Create("TestModel.Metropolis/MetropolisNavigation()", this.cityType, this.edmModel).HaveEntireSubtree();
        }

        [Fact]
        public void ExpandTokenForNavigationPropertyOnDerivedComplexShouldHaveEntireSubtree()
        {
            SelectedPropertiesNode.Create("NearestAirport/TestModel.RegionalAirport/Region()", this.metropolisType, this.edmModel).HaveEntireSubtree();
        }

        [Fact]
        public void ExpandTokenForNavigationPropertyOnComplexShouldHaveEntireSubtree()
        {
            SelectedPropertiesNode.Create("NearestAirport/City()", this.metropolisType, this.edmModel).HaveEntireSubtree();
        }

        [Fact]
        public void ExpandTokenForNavigationPropertyOnDerivedComplexWithSelectShouldHavePartialSubtree()
        {
            SelectedPropertiesNode p = SelectedPropertiesNode.Create("NearestAirport/TestModel.RegionalAirport(Name),NearestAirport/TestModel.RegionalAirport/Region()", this.metropolisType, this.edmModel);
            Assert.False(p.IsEntireSubtree());
        }

        [Fact]
        public void ExpandTokenForNavigationPropertyOnComplexOnDerivedTypeShouldHaveEntireSubtree()
        {
            SelectedPropertiesNode.Create("TestModel.Metropolis/NearestAirport/City()", this.cityType, this.edmModel).HaveEntireSubtree();
        }

        [Fact]
        public void ExpandTokenForNavigationPropertyOnDerivedTypeShouldNotBeAddedToProperties()
        {
            var p = SelectedPropertiesNode.Create("Name,TestModel.Metropolis/MetropolisNavigation()", this.cityType, this.edmModel);
            Assert.False(p.IsEntireSubtree());
        }

        [Fact]
        public void InvalidExpandTokenShouldHaveEntireSubtree()
        {
            SelectedPropertiesNode.Create("FabrikamNavProp()", this.cityType, this.edmModel).HaveEntireSubtree();
        }

        [Fact]
        public void InvalidExpandTokenWithoutTypeShouldHaveEntireSubtree()
        {
            SelectedPropertiesNode.Create("FabrikamNavProp()").HaveEntireSubtree();
        }

        [Fact]
        public void NamespaceAndContainerQualifiedNameWithParametersShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.Action(Edm.Int32)", this.cityType, this.edmModel).HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void WrongTypeSegmentBeforeActionNameShouldNotIncludeAction()
        {
            SelectedPropertiesNode.Create("FQNS.Fake/Action").NotHaveAction(this.cityType, this.action);
        }

        [Fact]
        public void ExactTypeSegmentBeforeActionNameShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.City/Action").HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void BaseTypeSegmentBeforeActionNameShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.City/Action").HaveAction(this.metropolisType, this.action);
        }

        [Fact]
        public void DerivedTypeSegmentBeforeActionNameShouldNotIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.Metropolis/Action").NotHaveAction(this.cityType, this.action);
        }

        [Fact]
        public void ExactTypeSegmentBeforeContainerQualifiedActionNameShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.City/TestModel.Action").HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void ExactTypeSegmentBeforeContainerQualifiedWildCardShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.City/TestModel.*").HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void ActionsMustBeFullQualifiedIfConflictWithPropertyShouldNotIncludeAction()
        {
            SelectedPropertiesNode.Create("Zip").NotHaveAction(this.districtType, this.actionConflictingWithPropertyName);
        }

        [Fact]
        public void ActionsMustBeFullQualifiedIfConflictWithPropertyShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.Zip").HaveAction(this.districtType, this.actionConflictingWithPropertyName);
        }

        [Fact]
        public void OpenTypeWithoutContainerQualifiedNameShouldNotIncludeAction()
        {
            SelectedPropertiesNode.Create("Action").NotHaveAction(this.openType, this.action);
        }

        [Fact]
        public void OpenTypeWithContainerQualifiedNameShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.Action").HaveAction(this.openType, this.action);
        }

        [Fact]
        public void GetSelectedPropertiesForNavigationPropertyForEmptyNodeShouldReturnEmptyNodeWhenEntityTypeIsNull()
        {
            Assert.Same(EmptyNode, EmptyNode.GetSelectedPropertiesForNavigationProperty(structuredType: null, navigationPropertyName: "foo"));
        }

        [Fact]
        public void GetSelectedPropertiesForNavigationPropertyForNonEmptyNodeShouldAlwaysReturnEntireSubtreeWhenEntityTypeIsNull()
        {
            EntireSubtreeNode.GetSelectedPropertiesForNavigationProperty(structuredType: null, navigationPropertyName: "foo").HaveEntireSubtree();
            SelectedPropertiesNode.Create("bar").GetSelectedPropertiesForNavigationProperty(structuredType: null, navigationPropertyName: "foo").HaveEntireSubtree();
        }

        [Fact]
        public void GetSelectedNavigationPropertiesShouldAlwaysReturnEmptyEnumerationWhenEntityTypeIsNull()
        {
            Assert.Empty(EntireSubtreeNode.GetSelectedNavigationProperties(null));
            Assert.Empty(EmptyNode.GetSelectedNavigationProperties(null));
            Assert.Empty(SelectedPropertiesNode.Create("bar").GetSelectedNavigationProperties(null));
        }

        [Fact]
        public void GetSelectedStreamPropertiesShouldAlwaysReturnEmptyEnumerationWhenEntityTypeIsNull()
        {
            Assert.Empty(EntireSubtreeNode.GetSelectedStreamProperties(null));
            Assert.Empty(EmptyNode.GetSelectedStreamProperties(null));
            Assert.Empty(SelectedPropertiesNode.Create("bar").GetSelectedStreamProperties(null));
        }

        private void VerifyCombination(SelectedPropertiesNode left, SelectedPropertiesNode right, Action<SelectedPropertiesNode> verify)
        {
            var result = SelectedPropertiesNode.CombineNodes(left, right);
            verify(result);

            result = SelectedPropertiesNode.CombineNodes(right, left);
            verify(result);
        }
    }

    internal static class EvaluationAssertionsExtensions
    {
        private static readonly SelectedPropertiesNode EntireSubtreeNode = SelectedPropertiesNode.EntireSubtree;
        private static readonly SelectedPropertiesNode EmptyNode = SelectedPropertiesNode.Create(string.Empty);

        internal static SelectedPropertiesNode HaveStreams(this SelectedPropertiesNode node, IEdmEntityType entityType, params string[] streamPropertyNames)
        {
            var keys = node.GetSelectedStreamProperties(entityType).Keys;
            Assert.Equal(keys.Count(), streamPropertyNames.Length);
            foreach (var keyStr in keys)
            {
                Assert.Contains(keyStr, streamPropertyNames);
            }
            return node;
        }

        internal static SelectedPropertiesNode HaveOnlyStreams(this SelectedPropertiesNode node, IEdmEntityType entityType, params string[] streamPropertyNames)
        {
            return node.HaveStreams(entityType, streamPropertyNames).NotHaveNavigations(entityType);
        }

        internal static SelectedPropertiesNode NotHaveStreams(this SelectedPropertiesNode node, IEdmEntityType entityType)
        {
            Assert.Empty(node.GetSelectedStreamProperties(entityType).Keys);
            return node;
        }

        internal static SelectedPropertiesNode HaveNavigations(this SelectedPropertiesNode node, IEdmEntityType entityType, params string[] navigationNames)
        {
            Assert.Equal(navigationNames, node.GetSelectedNavigationProperties(entityType).Select(p => p.Name));
            return node;
        }

        internal static SelectedPropertiesNode NotHaveNavigations(this SelectedPropertiesNode node, IEdmEntityType entityType)
        {
            Assert.Empty(node.GetSelectedNavigationProperties(entityType));

            foreach (var navigation in entityType.NavigationProperties())
            {
                node.GetSelectedPropertiesForNavigationProperty(entityType, navigation.Name).BeSameAsEmpty();
            }

            return node;
        }

        internal static SelectedPropertiesNode HaveOnlyNavigations(this SelectedPropertiesNode node, IEdmEntityType entityType, params string[] streamPropertyNames)
        {
            return node.HaveNavigations(entityType, streamPropertyNames).NotHaveStreams(entityType);
        }

        internal static SelectedPropertiesNode BeSameAsEmpty(this SelectedPropertiesNode node)
        {
            Assert.Same(EmptyNode, node);
            return node;
        }

        internal static SelectedPropertiesNode IncludeEntireSubtree(this SelectedPropertiesNode node, IEdmEntityType entityType)
        {
            var expected = entityType.StructuralProperties().Where(p => p.Type.IsStream());
            var edmStructuralProperties = node.GetSelectedStreamProperties(entityType).Values;
            Assert.Equal(expected.Count(), edmStructuralProperties.Count);
            foreach (var prop in edmStructuralProperties)
            {
                Assert.Contains(prop, expected);
            }

            var navExpected = entityType.NavigationProperties();
            var edmNavProperties = node.GetSelectedNavigationProperties(entityType);
            Assert.Equal(navExpected.Count(), edmNavProperties.Count());
            foreach (var prop in edmNavProperties)
            {
                Assert.Contains(prop, navExpected);
            }

            foreach (var navigation in entityType.NavigationProperties())
            {
                node.GetSelectedPropertiesForNavigationProperty(entityType, navigation.Name).HaveEntireSubtree();
            }

            return node;
        }

        internal static SelectedPropertiesNode BeEmpty(this SelectedPropertiesNode node, IEdmEntityType entityType)
        {
            return node.NotHaveStreams(entityType).NotHaveNavigations(entityType);
        }

        internal static SelectedPropertiesNode HaveChild(this SelectedPropertiesNode node, IEdmEntityType entityType, string propertyName, Action<SelectedPropertiesNode> validateChild)
        {
            var child = node.GetSelectedPropertiesForNavigationProperty(entityType, propertyName);
            Assert.NotNull(child);
            validateChild(child);
            return node;
        }

        internal static SelectedPropertiesNode HaveAction(this SelectedPropertiesNode node, EdmEntityType entityType, IEdmOperation action)
        {
            Assert.True(node.IsOperationSelected(entityType, action, entityType.IsOpen));
            return node;
        }

        internal static SelectedPropertiesNode NotHaveAction(this SelectedPropertiesNode node, EdmEntityType entityType, IEdmOperation action)
        {
            Assert.False(node.IsOperationSelected(entityType, action, entityType.IsOpen));
            return node;
        }

        internal static SelectedPropertiesNode HaveEntireSubtree(this SelectedPropertiesNode node, bool isTrue = true)
        {
            Assert.Equal(isTrue, node.IsEntireSubtree());
            return node;
        }
    }
}
