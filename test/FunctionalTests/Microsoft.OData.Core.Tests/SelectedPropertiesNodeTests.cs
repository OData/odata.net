//---------------------------------------------------------------------
// <copyright file="SelectedPropertiesNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.Evaluation
{
    public class SelectedPropertiesNodeTests
    {
        private static readonly SelectedPropertiesNode EntireSubtreeNode = SelectedPropertiesNode.EntireSubtree;
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

            this.metropolisType = new EdmEntityType("TestModel", "Metropolis", this.cityType);
            this.metropolisType.AddStructuralProperty("MetropolisStream", EdmCoreModel.Instance.GetStream(/*isNullable*/false));
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
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.SelectedPropertiesNode_StarSegmentNotLastSegment);
        }

        [Fact]
        public void NoSelectClauseShouldIncludeEntireSubtree()
        {
            SelectedPropertiesNode.EntireSubtree.Should().BeSameAsEntireSubtree().And.IncludeEntireSubtree(this.cityType);
        }

        [Fact]
        public void EmptySelectClauseShouldNotIncludeAnyProperties()
        {
            SelectedPropertiesNode.Create(string.Empty).Should().BeSameAsEmpty().And.BeEmpty(this.cityType);
        }

        [Fact]
        public void WhitespaceShouldBeTreatedAsEmpty()
        {
            SelectedPropertiesNode.Create(" ").Should().BeSameAsEmpty();
        }

        [Fact]
        public void WildcardShouldSelectAllPropertiesParsingTest()
        {
            SelectedPropertiesNode.Create("*").Should().HaveStreams(this.cityType, "Photo").And.HaveNavigations(this.cityType, "Districts").And.HaveChild(this.cityType, "Districts", c => c.Should().BeSameAsEmpty());
        }

        [Fact]
        public void SingleStreamPropertyWithNormalProperty()
        {
            SelectedPropertiesNode.Create("Size,Photo").Should().HaveOnlyStreams(this.cityType, "Photo");
        }

        [Fact]
        public void SpecifyingTheSameStreamTwiceShouldNotCauseDuplicates()
        {
            SelectedPropertiesNode.Create("Photo,Photo").Should().HaveOnlyStreams(this.cityType, "Photo");
        }

        [Fact]
        public void SelectingANavigationShouldSelectTheEntireTree()
        {
            SelectedPropertiesNode.Create("Districts").Should().HaveOnlyNavigations(this.cityType, "Districts").And.HaveChild(this.cityType, "Districts", c => c.Should().BeSameAsEntireSubtree());
        }

        [Fact]
        public void SpecifyingTheSameNavigationTwiceShouldNotCauseDuplicates()
        {
            SelectedPropertiesNode.Create("Districts,Districts").Should().HaveOnlyNavigations(this.cityType, "Districts").And.HaveChild(this.cityType, "Districts", c => c.Should().BeSameAsEntireSubtree());
        }

        [Fact]
        public void SpecifyingAWildCardShouldNotCauseDuplicates()
        {
            SelectedPropertiesNode.Create("Districts,*,Photo").Should().HaveStreams(this.cityType, "Photo").And.HaveNavigations(this.cityType, "Districts").And.HaveChild(this.cityType, "Districts", c => c.Should().BeSameAsEntireSubtree());
        }

        [Fact]
        public void SelectingANavigationShouldSelectTheEntireTreeIfWildcardAlsoPresent()
        {
            SelectedPropertiesNode.Create("Districts,Districts/*").Should().HaveOnlyNavigations(this.cityType, "Districts").And.HaveChild(this.cityType, "Districts", c => c.Should().BeSameAsEntireSubtree());
        }

        [Fact]
        public void SelectingEntireSubtreeAndSpecificPathShouldNotResultInDuplicates()
        {
            SelectedPropertiesNode.Create("Districts,Districts/Thumbnail")
                .Should()
                .HaveOnlyNavigations(this.cityType, "Districts")
                .And.HaveChild(this.cityType, "Districts", c => c.Should().BeSameAsEntireSubtree());
        }

        [Fact]
        public void DeepAndWideSelection1()
        {
            // this is covering 2 issues with a naive implementation:
            // 1) '*' and 'Districts' should not be collapsed, because 'Districts' is deep while '*' is wide
            // 2) 'Districts/*' should not override 'Districts'
            SelectedPropertiesNode.Create("*,Districts,Districts/*").Should()
                .HaveStreams(this.cityType, "Photo")
                .And.HaveNavigations(this.cityType, "Districts")
                .And.HaveChild(this.cityType, "Districts", c => c.Should().BeSameAsEntireSubtree());
        }

        [Fact]
        public void DeepAndWideSelection2()
        {
            SelectedPropertiesNode.Create("Districts/*,Districts/City,Districts/City/*").Should()
                .HaveOnlyNavigations(this.cityType, "Districts")
                .And.HaveChild(
                    this.cityType,
                    "Districts", 
                    c => c.Should()
                        .HaveStreams(this.districtType, "Thumbnail")
                        .And.HaveChild(this.districtType, "City", c2 => c2.Should().BeSameAsEntireSubtree()));
        }

        [Fact]
        public void WildcardAfterNavigationShouldNotSelectTheEntireTree()
        {
            SelectedPropertiesNode.Create("Districts/*").Should()
                .HaveOnlyNavigations(this.cityType, "Districts")
                .And.HaveChild(
                    this.cityType,
                    "Districts", 
                    c => c.Should()
                        .HaveStreams(this.districtType, "Thumbnail")
                        .And.HaveNavigations(this.districtType, "City"));
        }

        [Fact]
        public void SpecificDeepPaths()
        {
            SelectedPropertiesNode.Create("Districts/Thumbnail,Districts/Zip,Districts/City/Photo").Should()
                .HaveOnlyNavigations(this.cityType, "Districts")
                .And.HaveChild(
                    this.cityType,
                    "Districts",
                    c => c.Should()
                        .HaveStreams(this.districtType, "Thumbnail")
                        .And.HaveNavigations(this.districtType, "City")
                        .And.HaveChild(this.districtType, "City", c2 => c2.Should().HaveOnlyStreams(this.cityType, "Photo")));
        }

        [Fact]
        public void SimpleTypeSegmentWithStream()
        {
            SelectedPropertiesNode.Create("TestModel.City/Photo").Should().HaveOnlyStreams(this.cityType, "Photo");
        }

        [Fact]
        public void SimpleTypeSegmentWithNavigation()
        {
            SelectedPropertiesNode.Create("TestModel.City/Districts").Should().HaveOnlyNavigations(this.cityType, "Districts");
        }

        [Fact]
        public void UnmatchedTypeSegments()
        {
            SelectedPropertiesNode.Create("TestModel.Fake/Photo,TestModel.Fake2/Districts").Should().NotHaveStreams(this.cityType).And.NotHaveNavigations(this.cityType);
        }

        [Fact]
        public void TypeSegmentWithStreamUsingBaseTypeName()
        {
            SelectedPropertiesNode.Create("TestModel.City/Photo").Should().HaveOnlyStreams(this.metropolisType, "Photo");
        }

        [Fact]
        public void TypeSegmentWithNavigationUsingBaseTypeName()
        {
            SelectedPropertiesNode.Create("TestModel.City/Districts").Should().HaveOnlyNavigations(this.metropolisType, "Districts");
        }

        [Fact]
        public void TypeSegmentWithWildcardShouldFailWhenGettingStreams()
        {
            var node = SelectedPropertiesNode.Create("TestModel.City/*");
            Action action = () => node.GetSelectedStreamProperties(this.cityType);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.SelectedPropertiesNode_StarSegmentAfterTypeSegment);
        }

        [Fact]
        public void TypeSegmentWithWildcardShouldFailWhenGettingNavigations()
        {
            var node = SelectedPropertiesNode.Create("TestModel.City/*");
            Action action = () => node.GetSelectedNavigationProperties(this.cityType);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.SelectedPropertiesNode_StarSegmentAfterTypeSegment);
        }

        [Fact]
        public void TypeSegmentWithWildcardShouldFailWhenRecursing()
        {
            var node = SelectedPropertiesNode.Create("TestModel.City/*");
            Action action = () => node.GetSelectedPropertiesForNavigationProperty(this.cityType, "Districts");
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.SelectedPropertiesNode_StarSegmentAfterTypeSegment);
        }

        [Fact]
        public void DeepTypeSegment()
        {
            SelectedPropertiesNode.Create("Districts/City/TestModel.Metropolis/MetropolisStream")
                .Should().HaveChild(
                    this.cityType,
                    "Districts",
                    c => c.Should().HaveChild(
                        this.districtType, 
                        "City", 
                        c2 => c2.Should().HaveStreams(this.metropolisType, "MetropolisStream")));
        }

        [Fact]
        public void MultipleTypeSegmentsShouldNotProduceDuplicateStreams()
        {
            SelectedPropertiesNode.Create("TestModel.Town/Photo,TestModel.City/Photo").Should().HaveOnlyStreams(this.cityType, "Photo");
        }

        [Fact]
        public void MultipleTypeSegmentsShouldNotProduceDuplicateNavigations()
        {
            SelectedPropertiesNode.Create("TestModel.City/Districts,TestModel.Metropolis/Districts").Should().HaveOnlyNavigations(this.metropolisType, "Districts");
        }

        [Fact]
        public void StreamSpecifiedInBothTypeSegmentAndDirectlyShouldNotProduceDuplicates()
        {
            SelectedPropertiesNode.Create("TestModel.City/Photo,Photo").Should().HaveOnlyStreams(this.cityType, "Photo");
        }

        [Fact]
        public void NavigationSpecifiedInBothTypeSegmentAndDirectlyShouldNotProduceDuplicates()
        {
            SelectedPropertiesNode.Create("TestModel.City/Districts,Districts").Should().HaveOnlyNavigations(this.cityType, "Districts");
        }

        [Fact]
        public void SubPropertyOfNavigationSpecifiedInBothTypeSegmentAndDirectlyShouldNotProduceDuplicates()
        {
            SelectedPropertiesNode.Create("TestModel.City/Districts/Thumbnail,Districts/Thumbnail").Should()
                .HaveOnlyNavigations(this.cityType, "Districts")
                .And.HaveChild(this.cityType, "Districts", c=> c.Should().HaveOnlyStreams(this.districtType, "Thumbnail"));
        }

        [Fact]
        public void CombiningEntireSubtreeWithAnythingShouldReturnEntireSubtree()
        {
            SelectedPropertiesNode.CombineNodes(EntireSubtreeNode, EntireSubtreeNode).Should().BeSameAsEntireSubtree();
            
            this.VerifyCombination(EmptyNode, EntireSubtreeNode, n => n.Should().BeSameAsEntireSubtree());
            this.VerifyCombination(SelectedPropertiesNode.Create("*"), EntireSubtreeNode, n => n.Should().BeSameAsEntireSubtree());
        }

        [Fact]
        public void CombiningEmptyWithAnythingShouldReturnTheOtherThing()
        {
            SelectedPropertiesNode.CombineNodes(EmptyNode, EmptyNode).Should().BeSameAsEmpty();

            SelectedPropertiesNode nonEmpty = SelectedPropertiesNode.Create("*");
            this.VerifyCombination(EmptyNode, nonEmpty, n => n.Should().BeSameAs(nonEmpty));
        }

        [Fact]
        public void CombiningPropertyWithWildcardShouldBeWildcard()
        {
            SelectedPropertiesNode left = SelectedPropertiesNode.Create("*");
            SelectedPropertiesNode right = SelectedPropertiesNode.Create("Fake");
            this.VerifyCombination(left, right, n => n.Should().HaveStreams(this.cityType, "Photo").And.HaveNavigations(this.cityType, "Districts"));
        }

        [Fact]
        public void CombiningPartialNodesShouldCombineProperties()
        {
            var left = SelectedPropertiesNode.Create("Photo");
            var right = SelectedPropertiesNode.Create("Districts/Thumbnail");

            Action<SelectedPropertiesNode> verify = n => n.Should()
                .HaveStreams(this.cityType, "Photo")
                .And.HaveNavigations(this.cityType, "Districts")
                .And.HaveChild(this.cityType, "Districts", c => c.Should().HaveOnlyStreams(this.districtType, "Thumbnail"));
            
            this.VerifyCombination(left, right, verify);
        }

        [Fact]
        public void CombiningDeepPartialNodesShouldCombineRecursively()
        {
            var left = SelectedPropertiesNode.Create("Districts/City/Districts");
            var right = SelectedPropertiesNode.Create("Districts/Thumbnail");

            Action<SelectedPropertiesNode> verify = n => n.Should()
                .HaveOnlyNavigations(this.cityType, "Districts")
                .And.HaveChild(
                    this.cityType,
                    "Districts",
                    c => c.Should()
                        .HaveStreams(this.districtType, "Thumbnail")
                        .And.HaveNavigations(this.districtType, "City")
                        .And.HaveChild(this.districtType, "City", c2 => c2.Should().HaveOnlyNavigations(this.cityType, "Districts")));

            this.VerifyCombination(left, right, verify);
        }

        [Fact]
        public void EmptySelectionShouldNotIncludeActions()
        {
            EmptyNode.Should().NotHaveAction(this.cityType, this.action);
        }

        [Fact]
        public void EntireSubtreeShouldIncludeActions()
        {
            EntireSubtreeNode.Should().HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void WildCardShouldNotIncludeActions()
        {
            SelectedPropertiesNode.Create("*").Should().NotHaveAction(this.cityType, this.action);
        }

        [Fact]
        public void SpecificActionShouldBeNotSelectedForNonMatchingName()
        {
            SelectedPropertiesNode.Create("foo").Should().NotHaveAction(this.cityType, this.action);
        }

        [Fact]
        public void SpecificActionShouldBeSelected()
        {
            SelectedPropertiesNode.Create("Action").Should().HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void SpecificActionWithParametersShouldBeSelected()
        {
            SelectedPropertiesNode.Create("Action(Edm.Int32)").Should().HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void NamespaceQualifiedActionNameShouldBeSelected()
        {
            SelectedPropertiesNode.Create("TestModel.Action").Should().HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void NamespaceQualifiedWildcardShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.*").Should().HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void UnQualifiedNameWithParametersShouldNotIncludeActionOnOpenType()
        {
            SelectedPropertiesNode.Create("Action(Edm.Int32)").Should().NotHaveAction(this.openType, this.action);
        }

        [Fact]
        public void NamespaceAndContainerQualifiedNameWithParametersShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.Action(Edm.Int32)").Should().HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void WrongTypeSegmentBeforeActionNameShouldNotIncludeAction()
        {
            SelectedPropertiesNode.Create("FQNS.Fake/Action").Should().NotHaveAction(this.cityType, this.action);
        }

        [Fact]
        public void ExactTypeSegmentBeforeActionNameShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.City/Action").Should().HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void BaseTypeSegmentBeforeActionNameShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.City/Action").Should().HaveAction(this.metropolisType, this.action);
        }

        [Fact]
        public void DerivedTypeSegmentBeforeActionNameShouldNotIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.Metropolis/Action").Should().NotHaveAction(this.cityType, this.action);
        }

        [Fact]
        public void ExactTypeSegmentBeforeContainerQualifiedActionNameShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.City/TestModel.Action").Should().HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void ExactTypeSegmentBeforeContainerQualifiedWildCardShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.City/TestModel.*").Should().HaveAction(this.cityType, this.action);
        }

        [Fact]
        public void ActionsMustBeFullQualifiedIfConflictWithPropertyShouldNotIncludeAction()
        {
            SelectedPropertiesNode.Create("Zip").Should().NotHaveAction(this.districtType, this.actionConflictingWithPropertyName);
        }

        [Fact]
        public void ActionsMustBeFullQualifiedIfConflictWithPropertyShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.Zip").Should().HaveAction(this.districtType, this.actionConflictingWithPropertyName);
        }

        [Fact]
        public void OpenTypeWithoutContainerQualifiedNameShouldNotIncludeAction()
        {
            SelectedPropertiesNode.Create("Action").Should().NotHaveAction(this.openType, this.action);
        }

        [Fact]
        public void OpenTypeWithContainerQualifiedNameShouldIncludeAction()
        {
            SelectedPropertiesNode.Create("TestModel.Action").Should().HaveAction(this.openType, this.action);
        }

        [Fact]
        public void GetSelectedPropertiesForNavigationPropertyForEmptyNodeShouldReturnEmptyNodeWhenEntityTypeIsNull()
        {
            EmptyNode.GetSelectedPropertiesForNavigationProperty(entityType: null, navigationPropertyName: "foo").Should().BeSameAs(EmptyNode);
        }

        [Fact]
        public void GetSelectedPropertiesForNavigationPropertyForNonEmptyNodeShouldAlwaysReturnEntireSubtreeWhenEntityTypeIsNull()
        {
            EntireSubtreeNode.GetSelectedPropertiesForNavigationProperty(entityType: null, navigationPropertyName: "foo").Should().BeSameAs(EntireSubtreeNode);
            SelectedPropertiesNode.Create("bar").GetSelectedPropertiesForNavigationProperty(entityType: null, navigationPropertyName: "foo").Should().BeSameAs(EntireSubtreeNode);
        }

        [Fact]
        public void GetSelectedNavigationPropertiesShouldAlwaysReturnEmptyEnumerationWhenEntityTypeIsNull()
        {
            EntireSubtreeNode.GetSelectedNavigationProperties(entityType: null).Should().BeEmpty();
            EmptyNode.GetSelectedNavigationProperties(entityType: null).Should().BeEmpty();
            SelectedPropertiesNode.Create("bar").GetSelectedNavigationProperties(entityType: null).Should().BeEmpty();
        }

        [Fact]
        public void GetSelectedStreamPropertiesShouldAlwaysReturnEmptyEnumerationWhenEntityTypeIsNull()
        {
            EntireSubtreeNode.GetSelectedStreamProperties(entityType: null).Should().BeEmpty();
            EmptyNode.GetSelectedStreamProperties(entityType: null).Should().BeEmpty();
            SelectedPropertiesNode.Create("bar").GetSelectedStreamProperties(entityType: null).Should().BeEmpty();
        }

        private void VerifyCombination(SelectedPropertiesNode left, SelectedPropertiesNode right, Action<SelectedPropertiesNode> verify)
        {
            var result = SelectedPropertiesNode.CombineNodes(left, right);
            verify(result);

            result = SelectedPropertiesNode.CombineNodes(right, left);
            verify(result);
        }
    }

    internal class SelectedPropertiesNodeAssertions : ObjectAssertions
    {
        private static readonly SelectedPropertiesNode EntireSubtreeNode = SelectedPropertiesNode.EntireSubtree;
        private static readonly SelectedPropertiesNode EmptyNode = SelectedPropertiesNode.Create(string.Empty);

        internal SelectedPropertiesNodeAssertions(SelectedPropertiesNode node) : base(node)
        {
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> HaveStreams(IEdmEntityType entityType, params string[] streamPropertyNames)
        {
            this.Subject.As<SelectedPropertiesNode>().GetSelectedStreamProperties(entityType).Keys.Should().BeEquivalentTo(streamPropertyNames);
            return new AndConstraint<SelectedPropertiesNodeAssertions>(this);
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> HaveOnlyStreams(IEdmEntityType entityType, params string[] streamPropertyNames)
        {
            return this.HaveStreams(entityType, streamPropertyNames).And.NotHaveNavigations(entityType);
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> NotHaveStreams(IEdmEntityType entityType)
        {
            this.Subject.As<SelectedPropertiesNode>().GetSelectedStreamProperties(entityType).Keys.Should().BeEmpty();
            return new AndConstraint<SelectedPropertiesNodeAssertions>(this);
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> HaveNavigations(IEdmEntityType entityType, params string[] navigationNames)
        {
            this.Subject.As<SelectedPropertiesNode>().GetSelectedNavigationProperties(entityType).Select(p => p.Name).Should().BeEquivalentTo(navigationNames);
            return new AndConstraint<SelectedPropertiesNodeAssertions>(this);
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> NotHaveNavigations(IEdmEntityType entityType)
        {
            this.Subject.As<SelectedPropertiesNode>().GetSelectedNavigationProperties(entityType).Should().BeEmpty();

            foreach (var navigation in entityType.NavigationProperties())
            {
                this.Subject.As<SelectedPropertiesNode>().GetSelectedPropertiesForNavigationProperty(entityType, navigation.Name).Should().BeSameAsEmpty();
            }

            return new AndConstraint<SelectedPropertiesNodeAssertions>(this);
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> HaveOnlyNavigations(IEdmEntityType entityType, params string[] streamPropertyNames)
        {
            return this.HaveNavigations(entityType, streamPropertyNames).And.NotHaveStreams(entityType);
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> BeSameAsEntireSubtree()
        {
            this.Subject.Should().BeSameAs(EntireSubtreeNode);
            return new AndConstraint<SelectedPropertiesNodeAssertions>(this);
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> BeSameAsEmpty()
        {
            this.Subject.Should().BeSameAs(EmptyNode);
            return new AndConstraint<SelectedPropertiesNodeAssertions>(this);
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> IncludeEntireSubtree(IEdmEntityType entityType)
        {
            this.Subject.As<SelectedPropertiesNode>().GetSelectedStreamProperties(entityType).Values.Should().BeEquivalentTo(entityType.StructuralProperties().Where(p => p.Type.IsStream()));
            this.Subject.As<SelectedPropertiesNode>().GetSelectedNavigationProperties(entityType).Should().BeEquivalentTo(entityType.NavigationProperties());

            foreach (var navigation in entityType.NavigationProperties())
            {
                this.Subject.As<SelectedPropertiesNode>().GetSelectedPropertiesForNavigationProperty(entityType, navigation.Name).Should().BeSameAsEntireSubtree();
            }

            return new AndConstraint<SelectedPropertiesNodeAssertions>(this);
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> BeEmpty(IEdmEntityType entityType)
        {
            return this.NotHaveStreams(entityType).And.NotHaveNavigations(entityType);
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> HaveChild(IEdmEntityType entityType, string propertyName, Action<SelectedPropertiesNode> validateChild)
        {
            var child = this.Subject.As<SelectedPropertiesNode>().GetSelectedPropertiesForNavigationProperty(entityType, propertyName);
            child.Should().NotBeNull();
            validateChild(child);
            return new AndConstraint<SelectedPropertiesNodeAssertions>(this);
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> HaveAction(EdmEntityType entityType, IEdmOperation action)
        {
            this.Subject.As<SelectedPropertiesNode>().IsOperationSelected(entityType, action, entityType.IsOpen).Should().BeTrue();
            return new AndConstraint<SelectedPropertiesNodeAssertions>(this);
        }

        internal AndConstraint<SelectedPropertiesNodeAssertions> NotHaveAction(EdmEntityType entityType, IEdmOperation action)
        {
            this.Subject.As<SelectedPropertiesNode>().IsOperationSelected(entityType, action, entityType.IsOpen).Should().BeFalse();
            return new AndConstraint<SelectedPropertiesNodeAssertions>(this);
        }
    }

    internal static class EvaluationAssertionsExtensions
    {
        internal static SelectedPropertiesNodeAssertions Should(this SelectedPropertiesNode node)
        {
            return new SelectedPropertiesNodeAssertions(node);
        }
    }
}
