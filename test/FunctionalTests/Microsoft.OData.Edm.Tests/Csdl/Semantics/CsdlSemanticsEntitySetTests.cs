//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsEntitySetTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Library.Annotations;
using Microsoft.OData.Edm.Validation;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Semantics
{
    /// <summary>
    /// TODO: test the rest of CsdlSemanticsEntitySet
    /// </summary>
    public class CsdlSemanticsEntitySetTests
    {
        private readonly CsdlEntityContainer csdlContainer;
        private readonly CsdlSemanticsEntityContainer semanticContainer;
        private readonly CsdlSemanticsSchema semanticSchema;
        private readonly CsdlSemanticsEntityTypeDefinition semanticEntityType;
        private readonly CsdlSemanticsNavigationProperty navigationProperty;
        private readonly CsdlEntitySet csdlEntitySet;
        private readonly CsdlEntityType csdlEntityType;

        public CsdlSemanticsEntitySetTests()
        {
            var referentialConstraints = new List<CsdlReferentialConstraint>();
            var csdlNavigation = new CsdlNavigationProperty("Navigation", null, null, null, false, null, referentialConstraints, null, null);
            this.csdlEntityType = new CsdlEntityType("EntityType", null, false, false, false, null, Enumerable.Empty<CsdlProperty>(), new[] { csdlNavigation }, null, null);
            var goodBinding = new CsdlNavigationPropertyBinding("Navigation", "EntitySet", null, new CsdlLocation(1, 1));
            this.csdlEntitySet = new CsdlEntitySet("EntitySet", "FQ.NS.EntityType", new[] { goodBinding }, null, null);
            this.csdlContainer = new CsdlEntityContainer("Container", null, new[] { this.csdlEntitySet }, Enumerable.Empty<CsdlSingleton>(), Enumerable.Empty<CsdlOperationImport>(), null, null);

            var derivedCsdlNavigation = new CsdlNavigationProperty("DerivedNavigation", null, null, null, false, null, referentialConstraints, null, null);
            var derivedCsdlEntityType = new CsdlEntityType("DerivedEntityType", "FQ.NS.EntityType", false, false, false, null, Enumerable.Empty<CsdlProperty>(), new[] { derivedCsdlNavigation }, null, null);

            var unrelatedCsdlEntityType = new CsdlEntityType("UnrelatedEntityType", null, false, false, false, null, Enumerable.Empty<CsdlProperty>(), Enumerable.Empty<CsdlNavigationProperty>(), null, null);

            var csdlSchema = new CsdlSchema("FQ.NS", null, null, new[] { this.csdlEntityType, derivedCsdlEntityType, unrelatedCsdlEntityType }, Enumerable.Empty<CsdlEnumType>(), Enumerable.Empty<CsdlOperation>(),Enumerable.Empty<CsdlTerm>(),Enumerable.Empty<CsdlEntityContainer>(),Enumerable.Empty<CsdlAnnotations>(), Enumerable.Empty<CsdlTypeDefinition>(), null, null);
            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(csdlSchema);
            var semanticModel = new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>());
            this.semanticSchema = new CsdlSemanticsSchema(semanticModel, csdlSchema);
            this.semanticContainer = new CsdlSemanticsEntityContainer(this.semanticSchema, this.csdlContainer);

            this.semanticEntityType = semanticModel.FindType("FQ.NS.EntityType") as CsdlSemanticsEntityTypeDefinition;
            this.semanticEntityType.Should().NotBeNull();
            this.navigationProperty = this.semanticEntityType.FindProperty("Navigation") as CsdlSemanticsNavigationProperty;
            this.navigationProperty.Should().NotBeNull();
        }

        [Fact]
        public void FindNavigationTargetShouldReturnUnresolvedEntitySetIfEntitySetIsNotFound()
        {
            var nonExistentBinding = new CsdlNavigationPropertyBinding("Navigation", "NonExistent", null, new CsdlLocation(1, 1));
            var testSubject = new CsdlSemanticsEntitySet(this.semanticContainer, new CsdlEntitySet("Fake", "FQ.NS.EntityType", new[] { nonExistentBinding }, null, null));
            var result = testSubject.FindNavigationTarget(this.navigationProperty);
            result.Should().BeAssignableTo<UnresolvedEntitySet>();
            result.As<UnresolvedEntitySet>().Name.Should().Be("NonExistent");
            result.Errors().Should().Contain(e => e.ErrorLocation == nonExistentBinding.Location && e.ErrorCode == EdmErrorCode.BadUnresolvedEntitySet);
        }

        [Fact]
        public void FindNavigationTargetShouldReturnNullIfNavigationPropertyHasNoBinding()
        {
            var testSubject = new CsdlSemanticsEntitySet(this.semanticContainer, this.csdlEntitySet);
            Assert.True(testSubject.FindNavigationTarget(new CsdlSemanticsNavigationProperty(this.semanticEntityType, new CsdlNavigationProperty("Fake", "FQ.NS.EntityType", null, null, false, null, Enumerable.Empty<CsdlReferentialConstraint>(), null, null))) is IEdmUnknownEntitySet);
        }

        [Fact]
        public void FindNavigationTargetShouldReturnSetIfItIsFound()
        {
            var testSubject = new CsdlSemanticsEntitySet(this.semanticContainer, this.csdlEntitySet);
            var result = testSubject.FindNavigationTarget(this.navigationProperty);
            result.Should().BeAssignableTo<CsdlSemanticsEntitySet>();
            result.As<CsdlSemanticsEntitySet>().Name.Should().Be("EntitySet");
            result.Errors().Should().BeEmpty();
        }

        [Fact]
        public void NavigationTargetsShouldStillContainMappingsThatAreCompletelyWrong()
        {
            var location = new CsdlLocation(1, 1);
            var result = this.ParseSingleBinding("NonExistentPath", "NonExistentSet", location);

            result.Target.Should().BeAssignableTo<UnresolvedEntitySet>();
            result.Target.As<UnresolvedEntitySet>().Name.Should().Be("NonExistentSet");
            result.Target.Errors().Should().Contain(e => e.ErrorLocation == location && e.ErrorCode == EdmErrorCode.BadUnresolvedEntitySet);

            result.NavigationProperty.Should().BeAssignableTo<UnresolvedNavigationPropertyPath>();
            result.NavigationProperty.As<UnresolvedNavigationPropertyPath>().Name.Should().Be("NonExistentPath");
            result.NavigationProperty.Errors().Should().Contain(e => e.ErrorLocation == location && e.ErrorCode == EdmErrorCode.BadUnresolvedNavigationPropertyPath);
        }

        [Fact]
        public void NavigationTargetsShouldHandleNavigationsOnDerivedTypes()
        {
            var result = this.ParseSingleBinding("FQ.NS.DerivedEntityType/DerivedNavigation", "EntitySet");

            result.Target.Should().BeAssignableTo<CsdlSemanticsEntitySet>();
            result.Target.Name.Should().Be("EntitySet");

            result.NavigationProperty.Should().BeAssignableTo<CsdlSemanticsNavigationProperty>();
            result.NavigationProperty.Name.Should().Be("DerivedNavigation");
            result.NavigationProperty.DeclaringEntityType().FullName().Should().Be("FQ.NS.DerivedEntityType");
        }

        [Fact]
        public void NavigationTargetsShouldHandleFullyQualifiedTypeWithPropertyEvenIfNotNeeded()
        {
            var result = this.ParseSingleBinding("FQ.NS.EntityType/Navigation", "EntitySet");

            result.NavigationProperty.Should().BeAssignableTo<CsdlSemanticsNavigationProperty>();
            result.NavigationProperty.Name.Should().Be("Navigation");
            result.NavigationProperty.DeclaringEntityType().FullName().Should().Be("FQ.NS.EntityType");
        }

        [Fact]
        public void NavigationTargetsShouldHandleAccessToBaseTypePropertyOnDerivedType()
        {
            var result = this.ParseSingleBinding("FQ.NS.DerivedEntityType/Navigation", "EntitySet");

            result.NavigationProperty.Should().BeAssignableTo<CsdlSemanticsNavigationProperty>();
            result.NavigationProperty.Name.Should().Be("Navigation");
            result.NavigationProperty.DeclaringEntityType().FullName().Should().Be("FQ.NS.EntityType");
        }

        [Fact]
        public void NavigationTargetShouldHaveUnresolvedPathIfMultipleSlashesAreFound()
        {
            const string pathWithMultipleSlashes = "FQ.NS.DerivedEntityType/DerivedNavigation/";
            var result = this.ParseSingleBinding(pathWithMultipleSlashes, "EntitySet");

            result.NavigationProperty.Should().BeAssignableTo<UnresolvedNavigationPropertyPath>();
            result.NavigationProperty.Name.Should().Be(pathWithMultipleSlashes);
        }

        [Fact]
        public void NavigationTargetShouldHaveUnresolvedPathIfSlashIsFirstCharacter()
        {
            const string pathStartingWithSlash = "/DerivedNavigation";
            var result = this.ParseSingleBinding(pathStartingWithSlash, "EntitySet");

            result.NavigationProperty.Should().BeAssignableTo<UnresolvedNavigationPropertyPath>();
            result.NavigationProperty.Name.Should().Be(pathStartingWithSlash);
        }

        [Fact]
        public void NavigationTargetShouldHaveUnresolvedPathIfSlashIsLastCharacter()
        {
            const string pathEndingWithSlash = "FQ.NS.DerivedEntityType/";
            var result = this.ParseSingleBinding(pathEndingWithSlash, "EntitySet");

            result.NavigationProperty.Should().BeAssignableTo<UnresolvedNavigationPropertyPath>();
            result.NavigationProperty.Name.Should().Be(pathEndingWithSlash);
        }

        [Fact]
        public void NavigationTargetShouldHaveUnresolvedPathIfTypeIsNotDerivedFromStartingType()
        {
            const string pathWithNonExistentType = "FQ.NS.UnrelatedEntityType/DerivedNavigation";
            var result = this.ParseSingleBinding(pathWithNonExistentType, "EntitySet");

            result.NavigationProperty.Should().BeAssignableTo<UnresolvedNavigationPropertyPath>();
            result.NavigationProperty.Name.Should().Be(pathWithNonExistentType);
        }

        [Fact]
        public void NavigationTargetShouldHaveUnresolvedPathIfTypeIsNotFound()
        {
            const string pathWithNonExistentType = "NonExistentType/DerivedNavigation";
            var result = this.ParseSingleBinding(pathWithNonExistentType, "EntitySet");

            result.NavigationProperty.Should().BeAssignableTo<UnresolvedNavigationPropertyPath>();
            result.NavigationProperty.Name.Should().Be(pathWithNonExistentType);
        }

        [Fact]
        public void NavigationTargetShouldHaveUnresolvedPathIfPathIsEmpty()
        {
            var result = this.ParseSingleBinding(string.Empty, "EntitySet");

            result.NavigationProperty.Should().BeAssignableTo<UnresolvedNavigationPropertyPath>();
            result.NavigationProperty.Name.Should().BeEmpty();
        }

        private IEdmNavigationPropertyBinding ParseSingleBinding(string path, string target, CsdlLocation location = null)
        {
            var binding = new CsdlNavigationPropertyBinding(path, target, null, location);
            var testSubject = new CsdlSemanticsEntitySet(this.semanticContainer, new CsdlEntitySet("Fake", "FQ.NS.EntityType", new[] { binding }, null, null));

            testSubject.NavigationPropertyBindings.Should().HaveCount(1);
            var result = testSubject.NavigationPropertyBindings.Single();
            return result;
        }
    }
}
