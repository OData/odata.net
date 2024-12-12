//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsNavigationPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
    /// TODO: test the rest of CsdlSemanticsNavigationProperty
    /// </summary>
    public class CsdlSemanticsNavigationPropertyTests
    {
        private readonly CsdlSemanticsEntityTypeDefinition semanticEntityType;
        private readonly CsdlEntityType csdlEntityType;
        private readonly CsdlSemanticsNavigationProperty semanticCollectionNavigation;
        private readonly CsdlSemanticsNavigationProperty semanticReferenceNavigation;
        private readonly CsdlSemanticsNavigationProperty semanticNavigationWithoutPartner;
        private readonly CsdlNavigationProperty collectionProperty;
        private readonly CsdlNavigationProperty referenceProperty;

        public CsdlSemanticsNavigationPropertyTests()
        {
            var constraints = new[] { new CsdlReferentialConstraint("FK", "ID", null) };
            this.collectionProperty = new CsdlNavigationProperty("Collection", "Collection(FQ.NS.EntityType)", null, "Reference", false, null, constraints, null);
            this.referenceProperty = new CsdlNavigationProperty("Reference", "FQ.NS.EntityType", false, null, false, null, Enumerable.Empty<CsdlReferentialConstraint>(), null);

            var navigationWithoutPartner = new CsdlNavigationProperty("WithoutPartner", "FQ.NS.EntityType", false, null, false, null, Enumerable.Empty<CsdlReferentialConstraint>(), null);

            var idProperty = new CsdlProperty("ID", new CsdlNamedTypeReference("Edm.Int32", false, null), null, null);
            var fkProperty = new CsdlProperty("FK", new CsdlNamedTypeReference("Edm.Int32", false, null), null, null);
            this.csdlEntityType = new CsdlEntityType("EntityType", null, false, false, false, new CsdlKey(new[] { new CsdlPropertyReference("ID", null, null) }, null), new[] { idProperty, fkProperty }, new[] { collectionProperty, referenceProperty, navigationWithoutPartner }, null);

            var csdlSchema = new CsdlSchema("FQ.NS", null, null, new[] { this.csdlEntityType }, Enumerable.Empty<CsdlEnumType>(), Enumerable.Empty<CsdlOperation>(),Enumerable.Empty<CsdlTerm>(),Enumerable.Empty<CsdlEntityContainer>(),Enumerable.Empty<CsdlAnnotations>(), Enumerable.Empty<CsdlTypeDefinition>(), null);
            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(csdlSchema);

            var semanticModel = new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>());

            this.semanticEntityType = semanticModel.FindType("FQ.NS.EntityType") as CsdlSemanticsEntityTypeDefinition;
            Assert.NotNull(this.semanticEntityType);

            this.semanticCollectionNavigation = this.semanticEntityType.FindProperty("Collection") as CsdlSemanticsNavigationProperty;
            this.semanticReferenceNavigation = this.semanticEntityType.FindProperty("Reference") as CsdlSemanticsNavigationProperty;
            this.semanticNavigationWithoutPartner = this.semanticEntityType.FindProperty("WithoutPartner") as CsdlSemanticsNavigationProperty;

            Assert.NotNull(this.semanticCollectionNavigation);
            Assert.NotNull(this.semanticReferenceNavigation);
            Assert.NotNull(this.semanticNavigationWithoutPartner);
        }

        [Fact]
        public void NavigationPartnerShouldWorkIfExplicitlySpecified()
        {
            Assert.Equal("Reference", this.collectionProperty.PartnerPath.Path); // ensure that the test configuration is unchanged.
            Assert.Same(this.semanticReferenceNavigation, this.semanticCollectionNavigation.Partner);
        }

        [Fact]
        public void NavigationPartnerShouldWorkIfAnotherPropertyOnTheTargetTypeHasThisPropertyExplicitlySpecified()
        {
            Assert.Null(this.referenceProperty.PartnerPath); // ensure that the test configuration is unchanged.
            Assert.Same(this.semanticCollectionNavigation, this.semanticReferenceNavigation.Partner);
        }

        [Fact]
        public void NavigationPartnerShouldBeNullIfItDoesNotHaveOne()
        {
            Assert.Null(this.semanticNavigationWithoutPartner.Partner);
        }

        [Fact]
        public void NavigationPartnerShouldHaveErrorIfItCannotBeResolved()
        {
            var testSubject = this.ParseAndGetPartner("Nonexistent", "FQ.NS.EntityType");
            var unNpPath = Assert.IsType<UnresolvedNavigationPropertyPath>(testSubject);
            var error = Assert.Single(unNpPath.Errors);
            Assert.Equal(EdmErrorCode.BadUnresolvedNavigationPropertyPath, error.ErrorCode);
            Assert.Contains("Nonexistent", error.ErrorMessage);
        }

        [Fact]
        public void NavigationPartnerShouldHaveErrorIfTheTargetTypeCannotBeResolved()
        {
            var testSubject = this.ParseAndGetPartner("Navigation", "Fake.Fake");
            var unNpPath = Assert.IsType<UnresolvedNavigationPropertyPath>(testSubject);
            var error = Assert.Single(unNpPath.Errors);
            Assert.Equal(EdmErrorCode.BadUnresolvedNavigationPropertyPath, error.ErrorCode);
            Assert.Contains("Navigation", error.ErrorMessage);
            Assert.IsType<UnresolvedEntityType>(testSubject.DeclaringEntityType());
            Assert.Equal("Fake.Fake", testSubject.DeclaringEntityType().FullName());
            Assert.IsType<BadType>(testSubject.Type.Definition);
        }

        [Fact]
        public void PropertyShouldNotBePrincipalIfItHasAConstraint()
        {
            // assert test setup matches expectation
            Assert.NotEmpty(this.collectionProperty.ReferentialConstraints);
            Assert.Empty(this.referenceProperty.ReferentialConstraints);
            Assert.False(this.semanticCollectionNavigation.IsPrincipal());
        }

        [Fact]
        public void PropertyShouldNotBePrincipalIfPartnerIsNull()
        {
            Assert.False(this.semanticNavigationWithoutPartner.IsPrincipal());
        }

        [Fact]
        public void PropertyShouldBePrincipalIfItsPartnerHasAConstraint()
        {
            // assert test setup matches expectation
            Assert.NotEmpty(this.collectionProperty.ReferentialConstraints);
            Assert.Empty(this.referenceProperty.ReferentialConstraints);
            Assert.True(this.semanticReferenceNavigation.IsPrincipal());
        }

        [Fact]
        public void ConstraintShouldBeNullIfNotPresentInCsdl()
        {
            Assert.Null(this.semanticReferenceNavigation.ReferentialConstraint);
        }

        [Fact]
        public void ConstraintShouldBePopulatedIfPresentInCsdl()
        {
            Assert.NotNull(this.semanticCollectionNavigation.ReferentialConstraint);
            var property = Assert.Single(this.semanticCollectionNavigation.ReferentialConstraint.PropertyPairs);
            Assert.Equal("FK", property.DependentProperty.Name);
            Assert.False(property.DependentProperty is BadProperty);
            Assert.Equal("ID", property.PrincipalProperty.Name);
            Assert.False(property.PrincipalProperty is BadProperty);
        }

        [Fact]
        public void ConstraintShouldHaveUnresolvedPrincipalPropertyIfPropertyDoesNotExist()
        {
            var result = this.ParseSingleConstraint("FK", "NonExistent");
            var property = Assert.Single(result.ReferentialConstraint.PropertyPairs);
            Assert.IsType<UnresolvedProperty>(property.PrincipalProperty);
        }

        [Fact]
        public void ConstraintShouldHaveUnresolvedPrincipalPropertyIfPropertyIsEmpty()
        {
            var result = this.ParseSingleConstraint("FK", string.Empty);
            var property = Assert.Single(result.ReferentialConstraint.PropertyPairs);
            Assert.IsType<UnresolvedProperty>(property.PrincipalProperty);
        }

        [Fact]
        public void ConstraintShouldHaveUnresolvedDependentPropertyIfPropertyDoesNotExist()
        {
            var result = this.ParseSingleConstraint("NonExistent", "ID");
            var property = Assert.Single(result.DependentProperties());
            Assert.IsType<UnresolvedProperty>(property);
            Assert.Equal("NonExistent", property.Name);
        }

        [Fact]
        public void ConstraintShouldHaveUnresolvedDependentPropertyIfPropertyIsEmpty()
        {
            var result = this.ParseSingleConstraint(string.Empty, "ID");
            var property = Assert.Single(result.DependentProperties());
            Assert.IsType<UnresolvedProperty>(property);
        }

        [Fact]
        public void NavigationPropertyShouldNotAllowPrimitiveType()
        {
            var testSubject = this.ParseNavigation("Edm.Int32", null);
            var error = Assert.Single(testSubject.Errors());
            Assert.Equal(EdmErrorCode.BadUnresolvedEntityType, error.ErrorCode);
        }

        [Fact]
        public void NavigationPropertyShouldNotAllowNullabilityOnCollectionType()
        {
            var testSubject = this.ParseNavigation("Collection(FQ.NS.EntityType)", false);
            var error = Assert.Single(testSubject.Errors());
            Assert.Equal(EdmErrorCode.NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute, error.ErrorCode);
        }

        [Fact]
        public void NavigationPropertyShouldRespectNullableAttributeWhenFalse()
        {
            var testSubject = this.ParseNavigation("FQ.NS.EntityType", false);
            Assert.False(testSubject.Type.IsNullable);
        }

        [Fact]
        public void NavigationPropertyShouldRespectNullableAttributeWhenTrue()
        {
            var testSubject = this.ParseNavigation("FQ.NS.EntityType", true);
            Assert.True(testSubject.Type.IsNullable);
        }

        [Fact]
        public void NavigationPropertyNullabilityShouldBeDefaultWhenUnspecified()
        {
            var testSubject = this.ParseNavigation("FQ.NS.EntityType", null);
            Assert.Equal(CsdlConstants.Default_Nullable, testSubject.Type.IsNullable);
        }

        [Fact]
        public void NavigationPropertyTypeParsingShouldWorkForEntityType()
        {
            var testSubject = this.ParseNavigation("FQ.NS.EntityType", null);
            Assert.Same(this.semanticEntityType, testSubject.Type.Definition);
        }

        [Fact]
        public void NavigationPropertyTypeParsingShouldWorkForEntityCollectionType()
        {
            var testSubject = this.ParseNavigation("Collection(FQ.NS.EntityType)", null);
            Assert.True(testSubject.Type.IsCollection());
            Assert.Same(this.semanticEntityType, testSubject.Type.AsCollection().ElementType().Definition);
        }

        [Fact]
        public void NavigationPropertyTypeParsingShouldProduceErrorIfMalformedCollectionType()
        {
            var testSubject = this.ParseNavigation("Collection(FQ.NS.EntityType", null);
            var error = Assert.Single(testSubject.Errors());
            Assert.Equal(EdmErrorCode.BadUnresolvedEntityType, error.ErrorCode);
            Assert.Equal("Collection(FQ.NS.EntityType", testSubject.Type.FullName());
        }

        [Fact]
        public void NavigationPropertyTypeParsingShouldProduceErrorIfElementTypeCannotBeFound()
        {
            var testSubject = this.ParseNavigation("Collection(Fake.NonExistent)", null);
            var error = Assert.Single(testSubject.Errors());
            Assert.Equal(EdmErrorCode.BadUnresolvedEntityType, error.ErrorCode);
            Assert.True(testSubject.Type.IsCollection());
            Assert.Equal("Fake.NonExistent", testSubject.Type.AsCollection().ElementType().FullName());
        }

        private IEdmNavigationProperty ParseSingleConstraint(string property, string referencedProperty, CsdlLocation location = null)
        {
            var constraint = new CsdlReferentialConstraint(property, referencedProperty, location);
            var testSubject = new CsdlSemanticsNavigationProperty(this.semanticEntityType, new CsdlNavigationProperty("Fake", "Fake.Fake", false, "Fake", false, null, new[] { constraint }, null));
            return testSubject;
        }

        private IEdmNavigationProperty ParseAndGetPartner(string partnerName, string targetTypeName)
        {
            var testSubject = new CsdlSemanticsNavigationProperty(this.semanticEntityType, new CsdlNavigationProperty("Fake", targetTypeName, false, partnerName, false, null, Enumerable.Empty<CsdlReferentialConstraint>(), null));
            return testSubject.Partner;
        }

        private IEdmNavigationProperty ParseNavigation(string typeName, bool? nullable)
        {
            return new CsdlSemanticsNavigationProperty(this.semanticEntityType, new CsdlNavigationProperty("Fake", typeName, nullable, null, false, null, Enumerable.Empty<CsdlReferentialConstraint>(), null));
        }
    }
}
