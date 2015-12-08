//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsNavigationPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Library.Annotations;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Validation;
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
            var constraints = new[] { new CsdlReferentialConstraint("FK", "ID", null, null) };
            this.collectionProperty = new CsdlNavigationProperty("Collection", "Collection(FQ.NS.EntityType)", null, "Reference", false, null, constraints, null, null);
            this.referenceProperty = new CsdlNavigationProperty("Reference", "FQ.NS.EntityType", false, null, false, null, Enumerable.Empty<CsdlReferentialConstraint>(), null, null);

            var navigationWithoutPartner = new CsdlNavigationProperty("WithoutPartner", "FQ.NS.EntityType", false, null, false, null, Enumerable.Empty<CsdlReferentialConstraint>(), null, null);

            var idProperty = new CsdlProperty("ID", new CsdlNamedTypeReference("Edm.Int32", false, null), false, null, null, null);
            var fkProperty = new CsdlProperty("FK", new CsdlNamedTypeReference("Edm.Int32", false, null), false, null, null, null);
            this.csdlEntityType = new CsdlEntityType("EntityType", null, false, false, false, new CsdlKey(new[] { new CsdlPropertyReference("ID", null) }, null), new[] { idProperty, fkProperty }, new[] { collectionProperty, referenceProperty, navigationWithoutPartner }, null, null);

            var csdlSchema = new CsdlSchema("FQ.NS", null, null, new[] { this.csdlEntityType }, Enumerable.Empty<CsdlEnumType>(), Enumerable.Empty<CsdlOperation>(),Enumerable.Empty<CsdlTerm>(),Enumerable.Empty<CsdlEntityContainer>(),Enumerable.Empty<CsdlAnnotations>(), Enumerable.Empty<CsdlTypeDefinition>(), null, null);
            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(csdlSchema);

            var semanticModel = new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>());
           
            this.semanticEntityType = semanticModel.FindType("FQ.NS.EntityType") as CsdlSemanticsEntityTypeDefinition;
            this.semanticEntityType.Should().NotBeNull();

            this.semanticCollectionNavigation = this.semanticEntityType.FindProperty("Collection") as CsdlSemanticsNavigationProperty;
            this.semanticReferenceNavigation = this.semanticEntityType.FindProperty("Reference") as CsdlSemanticsNavigationProperty;
            this.semanticNavigationWithoutPartner = this.semanticEntityType.FindProperty("WithoutPartner") as CsdlSemanticsNavigationProperty;

            this.semanticCollectionNavigation.Should().NotBeNull();
            this.semanticReferenceNavigation.Should().NotBeNull();
            this.semanticNavigationWithoutPartner.Should().NotBeNull();
        }

        [Fact]
        public void NavigationPartnerShouldWorkIfExplicitlySpecified()
        {
            this.collectionProperty.Partner.Should().Be("Reference"); // ensure that the test configuration is unchanged.
            this.semanticCollectionNavigation.Partner.Should().BeSameAs(this.semanticReferenceNavigation);
        }

        [Fact]
        public void NavigationPartnerShouldWorkIfAnotherPropertyOnTheTargetTypeHasThisPropertyExplicitlySpecified()
        {
            this.referenceProperty.Partner.Should().BeNull(); // ensure that the test configuration is unchanged.
            this.semanticReferenceNavigation.Partner.Should().BeSameAs(this.semanticCollectionNavigation);
        }

        [Fact]
        public void NavigationPartnerShouldBeNullIfItDoesNotHaveOne()
        {
            this.semanticNavigationWithoutPartner.Partner.Should().BeNull();
        }

        [Fact]
        public void NavigationPartnerShouldHaveErrorIfItCannotBeResolved()
        {
            var testSubject = this.ParseAndGetPartner("Nonexistent", "FQ.NS.EntityType");
            testSubject.Should().BeAssignableTo<UnresolvedNavigationPropertyPath>();
            testSubject.As<UnresolvedNavigationPropertyPath>().Errors
                .Should().HaveCount(1)
                .And.Contain(e => e.ErrorCode == EdmErrorCode.BadUnresolvedNavigationPropertyPath && e.ErrorMessage.Contains("Nonexistent"));
        }

        [Fact]
        public void NavigationPartnerShouldHaveErrorIfTheTargetTypeCannotBeResolved()
        {
            var testSubject = this.ParseAndGetPartner("Navigation", "Fake.Fake");
            testSubject.Should().BeAssignableTo<UnresolvedNavigationPropertyPath>();
            testSubject.As<UnresolvedNavigationPropertyPath>().Errors
                .Should().HaveCount(1)
                .And.Contain(e => e.ErrorCode == EdmErrorCode.BadUnresolvedNavigationPropertyPath && e.ErrorMessage.Contains("Navigation"));
            testSubject.DeclaringEntityType().Should().BeAssignableTo<BadEntityType>();
            testSubject.DeclaringEntityType().FullName().Should().Be("Fake.Fake");
            testSubject.Type.Definition.Should().BeAssignableTo<BadType>();
        }

        [Fact]
        public void PropertyShouldNotBePrincipalIfItHasAConstraint()
        {
            // assert test setup matches expectation
            this.collectionProperty.ReferentialConstraints.Should().NotBeEmpty();
            this.referenceProperty.ReferentialConstraints.Should().BeEmpty();
            this.semanticCollectionNavigation.IsPrincipal().Should().BeFalse();
        }

        [Fact]
        public void PropertyShouldNotBePrincipalIfPartnerIsNull()
        {
            this.semanticNavigationWithoutPartner.IsPrincipal().Should().BeFalse();
        }

        [Fact]
        public void PropertyShouldBePrincipalIfItsPartnerHasAConstraint()
        {
            // assert test setup matches expectation
            this.collectionProperty.ReferentialConstraints.Should().NotBeEmpty();
            this.referenceProperty.ReferentialConstraints.Should().BeEmpty();
            this.semanticReferenceNavigation.IsPrincipal().Should().BeTrue();
        }

        [Fact]
        public void ConstraintShouldBeNullIfNotPresentInCsdl()
        {
            this.semanticReferenceNavigation.ReferentialConstraint.Should().BeNull();
        }

        [Fact]
        public void ConstraintShouldBePopulatedIfPresentInCsdl()
        {
            this.semanticCollectionNavigation.ReferentialConstraint.Should().NotBeNull();
            this.semanticCollectionNavigation.ReferentialConstraint.PropertyPairs
                .Should().HaveCount(1)
                .And.Contain(p => p.DependentProperty.Name == "FK" && !(p.DependentProperty is BadProperty)
                    && p.PrincipalProperty.Name == "ID" && !(p.PrincipalProperty is BadProperty));
        }

        [Fact]
        public void ConstraintShouldHaveUnresolvedPrincipalPropertyIfPropertyDoesNotExist()
        {
            var result = this.ParseSingleConstraint("FK", "NonExistent");
            result.ReferentialConstraint.PropertyPairs.Should().HaveCount(1).And.Contain(r => r.PrincipalProperty is UnresolvedProperty);
        }

        [Fact]
        public void ConstraintShouldHaveUnresolvedPrincipalPropertyIfPropertyIsEmpty()
        {
            var result = this.ParseSingleConstraint("FK", string.Empty);
            result.ReferentialConstraint.PropertyPairs.Should().HaveCount(1).And.Contain(r => r.PrincipalProperty is UnresolvedProperty);
        }

        [Fact]
        public void ConstraintShouldHaveUnresolvedDependentPropertyIfPropertyDoesNotExist()
        {
            var result = this.ParseSingleConstraint("NonExistent", "ID");
            result.DependentProperties().Should().HaveCount(1).And.Contain(p => p is UnresolvedProperty && p.Name == "NonExistent");
        }

        [Fact]
        public void ConstraintShouldHaveUnresolvedDependentPropertyIfPropertyIsEmpty()
        {
            var result = this.ParseSingleConstraint(string.Empty, "ID");
            result.DependentProperties().Should().HaveCount(1).And.Contain(p => p is UnresolvedProperty);
        }

        [Fact]
        public void NavigationPropertyShouldNotAllowPrimitiveType()
        {
            var testSubject = this.ParseNavigation("Edm.Int32", null);
            testSubject.Errors().Should().HaveCount(1).And.Contain(e => e.ErrorCode == EdmErrorCode.BadUnresolvedEntityType);
        }

        [Fact]
        public void NavigationPropertyShouldNotAllowNullabilityOnCollectionType()
        {
            var testSubject = this.ParseNavigation("Collection(FQ.NS.EntityType)", false);
            testSubject.Errors().Should().HaveCount(1).And.Contain(e => e.ErrorCode == EdmErrorCode.NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute);
        }

        [Fact]
        public void NavigationPropertyShouldRespectNullableAttributeWhenFalse()
        {
            var testSubject = this.ParseNavigation("FQ.NS.EntityType", false);
            testSubject.Type.IsNullable.Should().BeFalse();
        }

        [Fact]
        public void NavigationPropertyShouldRespectNullableAttributeWhenTrue()
        {
            var testSubject = this.ParseNavigation("FQ.NS.EntityType", true);
            testSubject.Type.IsNullable.Should().BeTrue();
        }

        [Fact]
        public void NavigationPropertyNullabilityShouldBeDefaultWhenUnspecified()
        {
            var testSubject = this.ParseNavigation("FQ.NS.EntityType", null);
            testSubject.Type.IsNullable.Should().Be(CsdlConstants.Default_Nullable);
        }

        [Fact]
        public void NavigationPropertyTypeParsingShouldWorkForEntityType()
        {
            var testSubject = this.ParseNavigation("FQ.NS.EntityType", null);
            testSubject.Type.Definition.Should().BeSameAs(this.semanticEntityType);
        }

        [Fact]
        public void NavigationPropertyTypeParsingShouldWorkForEntityCollectionType()
        {
            var testSubject = this.ParseNavigation("Collection(FQ.NS.EntityType)", null);
            testSubject.Type.IsCollection().Should().BeTrue();
            testSubject.Type.AsCollection().ElementType().Definition.Should().BeSameAs(this.semanticEntityType);
        }

        [Fact]
        public void NavigationPropertyTypeParsingShouldProduceErrorIfMalformedCollectionType()
        {
            var testSubject = this.ParseNavigation("Collection(FQ.NS.EntityType", null);
            testSubject.Errors().Should().HaveCount(1).And.Contain(e => e.ErrorCode == EdmErrorCode.BadUnresolvedEntityType);
            testSubject.Type.FullName().Should().Be("Collection(FQ.NS.EntityType");
        }

        [Fact]
        public void NavigationPropertyTypeParsingShouldProduceErrorIfElementTypeCannotBeFound()
        {
            var testSubject = this.ParseNavigation("Collection(Fake.NonExistent)", null);
            testSubject.Errors().Should().HaveCount(1).And.Contain(e => e.ErrorCode == EdmErrorCode.BadUnresolvedEntityType);
            testSubject.Type.IsCollection().Should().BeTrue();
            testSubject.Type.AsCollection().ElementType().FullName().Should().Be("Fake.NonExistent");
        }

        private IEdmNavigationProperty ParseSingleConstraint(string property, string referencedProperty, CsdlLocation location = null)
        {
            var constraint = new CsdlReferentialConstraint(property, referencedProperty, null, location);
            var testSubject = new CsdlSemanticsNavigationProperty(this.semanticEntityType, new CsdlNavigationProperty("Fake", "Fake.Fake", false, "Fake", false, null, new[] { constraint }, null, null));
            return testSubject;
        }

        private IEdmNavigationProperty ParseAndGetPartner(string partnerName, string targetTypeName)
        {
            var testSubject = new CsdlSemanticsNavigationProperty(this.semanticEntityType, new CsdlNavigationProperty("Fake", targetTypeName, false, partnerName, false, null, Enumerable.Empty<CsdlReferentialConstraint>(), null, null));
            return testSubject.Partner;
        }

        private IEdmNavigationProperty ParseNavigation(string typeName, bool? nullable)
        {
            return new CsdlSemanticsNavigationProperty(this.semanticEntityType, new CsdlNavigationProperty("Fake", typeName, nullable, null, false, null, Enumerable.Empty<CsdlReferentialConstraint>(), null, null));
        }
    }
}
