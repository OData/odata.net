//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsEntitySetTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
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
            var csdlNavigation = new CsdlNavigationProperty("Navigation", "FQ.NS.EntityType", null, null, false, null, referentialConstraints, null);
            this.csdlEntityType = new CsdlEntityType("EntityType", null, false, false, false, null, Enumerable.Empty<CsdlProperty>(), new[] { csdlNavigation }, null);
            var goodBinding = new CsdlNavigationPropertyBinding("Navigation", "EntitySet", new CsdlLocation(1, 1));
            this.csdlEntitySet = new CsdlEntitySet("EntitySet", "FQ.NS.EntityType", new[] { goodBinding }, null);
            this.csdlContainer = new CsdlEntityContainer("Container", null, new[] { this.csdlEntitySet }, Enumerable.Empty<CsdlSingleton>(), Enumerable.Empty<CsdlOperationImport>(), null);

            var derivedCsdlNavigation = new CsdlNavigationProperty("DerivedNavigation", "FQ.NS.EntityType", null, null, false, null, referentialConstraints, null);
            var derivedCsdlEntityType = new CsdlEntityType("DerivedEntityType", "FQ.NS.EntityType", false, false, false, null, Enumerable.Empty<CsdlProperty>(), new[] { derivedCsdlNavigation }, null);

            var unrelatedCsdlEntityType = new CsdlEntityType("UnrelatedEntityType", null, false, false, false, null, Enumerable.Empty<CsdlProperty>(), Enumerable.Empty<CsdlNavigationProperty>(), null);

            var csdlSchema = new CsdlSchema("FQ.NS", null, null, new[] { this.csdlEntityType, derivedCsdlEntityType, unrelatedCsdlEntityType }, Enumerable.Empty<CsdlEnumType>(), Enumerable.Empty<CsdlOperation>(),Enumerable.Empty<CsdlTerm>(),Enumerable.Empty<CsdlEntityContainer>(),Enumerable.Empty<CsdlAnnotations>(), Enumerable.Empty<CsdlTypeDefinition>(), null);
            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(csdlSchema);
            var semanticModel = new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>());
            this.semanticSchema = new CsdlSemanticsSchema(semanticModel, csdlSchema);
            this.semanticContainer = new CsdlSemanticsEntityContainer(this.semanticSchema, this.csdlContainer);

            this.semanticEntityType = semanticModel.FindType("FQ.NS.EntityType") as CsdlSemanticsEntityTypeDefinition;
            Assert.NotNull(this.semanticEntityType);
            this.navigationProperty = this.semanticEntityType.FindProperty("Navigation") as CsdlSemanticsNavigationProperty;
            Assert.NotNull(this.navigationProperty);
        }

        [Fact]
        public void FindNavigationTargetShouldReturnUnresolvedEntitySetIfEntitySetIsNotFound()
        {
            var nonExistentBinding = new CsdlNavigationPropertyBinding("Navigation", "NonExistent", new CsdlLocation(1, 1));
            var testSubject = new CsdlSemanticsEntitySet(this.semanticContainer, new CsdlEntitySet("Fake", "FQ.NS.EntityType", new[] { nonExistentBinding }, null));
            var result = testSubject.FindNavigationTarget(this.navigationProperty);
            var entitySet = Assert.IsType<UnresolvedEntitySet>(result);
            Assert.Equal("NonExistent", entitySet.Name);
            Assert.Contains(result.Errors(), e => e.ErrorLocation == nonExistentBinding.Location && e.ErrorCode == EdmErrorCode.BadUnresolvedEntitySet);
        }

        [Fact]
        public void FindNavigationTargetShouldReturnNullIfNavigationPropertyHasNoBinding()
        {
            var testSubject = new CsdlSemanticsEntitySet(this.semanticContainer, this.csdlEntitySet);
            Assert.True(testSubject.FindNavigationTarget(new CsdlSemanticsNavigationProperty(this.semanticEntityType, new CsdlNavigationProperty("Fake", "FQ.NS.EntityType", null, null, false, null, Enumerable.Empty<CsdlReferentialConstraint>(), null))) is IEdmUnknownEntitySet);
        }

        [Fact]
        public void FindNavigationTargetShouldReturnSetIfItIsFound()
        {
            var testSubject = new CsdlSemanticsEntitySet(this.semanticContainer, this.csdlEntitySet);
            var result = testSubject.FindNavigationTarget(this.navigationProperty);
            var entitySet = Assert.IsType<CsdlSemanticsEntitySet>(result);
            Assert.Equal("EntitySet", entitySet.Name);
            Assert.Empty(result.Errors());
        }

        [Fact]
        public void NavigationTargetsShouldStillContainMappingsThatAreCompletelyWrong()
        {
            var location = new CsdlLocation(1, 1);
            var result = this.ParseSingleBinding("NonExistentPath", "NonExistentSet", location);

            var entitySet = Assert.IsType<UnresolvedEntitySet>(result.Target);
            Assert.Equal("NonExistentSet", entitySet.Name);
            Assert.Contains(result.Target.Errors(), e => e.ErrorLocation == location && e.ErrorCode == EdmErrorCode.BadUnresolvedEntitySet);

            var npPath = Assert.IsType<UnresolvedNavigationPropertyPath>(result.NavigationProperty);
            Assert.Equal("NonExistentPath", npPath.Name);
            Assert.Contains(result.NavigationProperty.Errors(), e => e.ErrorLocation == location && e.ErrorCode == EdmErrorCode.BadUnresolvedNavigationPropertyPath);
        }

        [Fact]
        public void NavigationTargetsShouldHandleNavigationsOnDerivedTypes()
        {
            var result = this.ParseSingleBinding("FQ.NS.DerivedEntityType/DerivedNavigation", "EntitySet");

            Assert.IsType<CsdlSemanticsEntitySet>(result.Target);
            Assert.Equal("EntitySet", result.Target.Name);

            Assert.IsType<CsdlSemanticsNavigationProperty>(result.NavigationProperty);
            Assert.Equal("DerivedNavigation", result.NavigationProperty.Name);
            Assert.Equal("FQ.NS.DerivedEntityType", result.NavigationProperty.DeclaringEntityType().FullName());
        }

        [Fact]
        public void NavigationTargetsShouldHandleFullyQualifiedTypeWithPropertyEvenIfNotNeeded()
        {
            var result = this.ParseSingleBinding("FQ.NS.EntityType/Navigation", "EntitySet");

            Assert.IsType<CsdlSemanticsNavigationProperty>(result.NavigationProperty);
            Assert.Equal("Navigation", result.NavigationProperty.Name);
            Assert.Equal("FQ.NS.EntityType", result.NavigationProperty.DeclaringEntityType().FullName());
        }

        [Fact]
        public void NavigationTargetsShouldHandleAccessToBaseTypePropertyOnDerivedType()
        {
            var result = this.ParseSingleBinding("FQ.NS.DerivedEntityType/Navigation", "EntitySet");

            Assert.IsType<CsdlSemanticsNavigationProperty>(result.NavigationProperty);
            Assert.Equal("Navigation", result.NavigationProperty.Name);
            Assert.Equal("FQ.NS.EntityType", result.NavigationProperty.DeclaringEntityType().FullName());
        }

        [Fact]
        public void NavigationTargetShouldHaveUnresolvedPathIfMultipleSlashesAreFound()
        {
            const string pathWithMultipleSlashes = "FQ.NS.DerivedEntityType/DerivedNavigation/";
            var result = this.ParseSingleBinding(pathWithMultipleSlashes, "EntitySet");

            Assert.IsType<UnresolvedNavigationPropertyPath>(result.NavigationProperty);
            Assert.Equal(pathWithMultipleSlashes, result.NavigationProperty.Name);
        }

        [Fact]
        public void NavigationTargetShouldHaveUnresolvedPathIfSlashIsFirstCharacter()
        {
            const string pathStartingWithSlash = "/DerivedNavigation";
            var result = this.ParseSingleBinding(pathStartingWithSlash, "EntitySet");

            Assert.IsType<UnresolvedNavigationPropertyPath>(result.NavigationProperty);
            Assert.Equal(pathStartingWithSlash, result.NavigationProperty.Name);
        }

        [Fact]
        public void NavigationTargetShouldHaveUnresolvedPathIfSlashIsLastCharacter()
        {
            const string pathEndingWithSlash = "FQ.NS.DerivedEntityType/";
            var result = this.ParseSingleBinding(pathEndingWithSlash, "EntitySet");

            Assert.IsType<UnresolvedNavigationPropertyPath>(result.NavigationProperty);
            Assert.Equal(pathEndingWithSlash, result.NavigationProperty.Name);
        }

        [Fact]
        public void NavigationTargetShouldHaveUnresolvedPathIfTypeIsNotDerivedFromStartingType()
        {
            const string pathWithNonExistentType = "FQ.NS.UnrelatedEntityType/DerivedNavigation";
            var result = this.ParseSingleBinding(pathWithNonExistentType, "EntitySet");

            Assert.IsType<UnresolvedNavigationPropertyPath>(result.NavigationProperty);
            Assert.Equal(pathWithNonExistentType, result.NavigationProperty.Name);
        }

        [Fact]
        public void NavigationTargetShouldHaveUnresolvedPathIfTypeIsNotFound()
        {
            const string pathWithNonExistentType = "NonExistentType/DerivedNavigation";
            var result = this.ParseSingleBinding(pathWithNonExistentType, "EntitySet");

            Assert.IsType<UnresolvedNavigationPropertyPath>(result.NavigationProperty);
            Assert.Equal(pathWithNonExistentType, result.NavigationProperty.Name);
        }

        [Fact]
        public void NavigationTargetShouldHaveUnresolvedPathIfPathIsEmpty()
        {
            var result = this.ParseSingleBinding(string.Empty, "EntitySet");

            Assert.IsType<UnresolvedNavigationPropertyPath>(result.NavigationProperty);
            Assert.Empty(result.NavigationProperty.Name);
        }

        private IEdmNavigationPropertyBinding ParseSingleBinding(string path, string target, CsdlLocation location = null)
        {
            var binding = new CsdlNavigationPropertyBinding(path, target, location);
            var testSubject = new CsdlSemanticsEntitySet(this.semanticContainer, new CsdlEntitySet("Fake", "FQ.NS.EntityType", new[] { binding }, null));

            var result = Assert.Single(testSubject.NavigationPropertyBindings);
            return result;
        }
    }
}
