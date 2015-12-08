//---------------------------------------------------------------------
// <copyright file="EdmReferentialConstraintTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class EdmReferentialConstraintTests
    {
        private readonly EdmEntityType typeWithOneKey;
        private readonly EdmStructuralProperty key1_1;
        private readonly EdmEntityType typeWithTwoKeys;
        private readonly EdmStructuralProperty key2_1;
        private readonly EdmStructuralProperty key2_2;
        private readonly EdmStructuralProperty otherTypeProperty1;
        private readonly EdmStructuralProperty otherTypeProperty2;
        private readonly EdmStructuralProperty property1;
        private readonly EdmStructuralProperty property2;

        public EdmReferentialConstraintTests()
        {
            this.typeWithOneKey = new EdmEntityType("Fake", "OneKey");
            this.typeWithOneKey.AddKeys(this.key1_1 = this.typeWithOneKey.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            property1 = this.typeWithOneKey.AddStructuralProperty("prop1", EdmPrimitiveTypeKind.String);

            this.typeWithTwoKeys = new EdmEntityType("Fake", "TwoKeys");
            this.typeWithTwoKeys.AddKeys(this.key2_1 = this.typeWithTwoKeys.AddStructuralProperty("Id1", EdmPrimitiveTypeKind.Int32));
            this.typeWithTwoKeys.AddKeys(this.key2_2 = this.typeWithTwoKeys.AddStructuralProperty("Id2", EdmPrimitiveTypeKind.Int32));
            property2 = this.typeWithTwoKeys.AddStructuralProperty("prop2", EdmPrimitiveTypeKind.String);

            var otherType = new EdmEntityType("Fake", "Other");
            this.otherTypeProperty1 = otherType.AddStructuralProperty("Property1", EdmPrimitiveTypeKind.Int32);
            this.otherTypeProperty2 = otherType.AddStructuralProperty("Property2", EdmPrimitiveTypeKind.Int32);
        }

        [Fact]
        public void CreateReferentialConstraintShouldSucceedForSinglePropertyKey()
        {
            var testSubject = EdmReferentialConstraint.Create(new[] {this.otherTypeProperty1}, this.typeWithOneKey.Key());
            testSubject.PropertyPairs.Should().HaveCount(1)
                .And.Contain(p => p.DependentProperty == this.otherTypeProperty1 && p.PrincipalProperty == this.key1_1);
        }

        [Fact]
        public void CreateReferentialConstraintShouldSucceedForTwoPropertyKey()
        {
            var testSubject = EdmReferentialConstraint.Create(new[] {this.otherTypeProperty1, this.otherTypeProperty2}, this.typeWithTwoKeys.Key());
            testSubject.PropertyPairs.Should().HaveCount(2)
                .And.Contain(p => p.DependentProperty == this.otherTypeProperty1 && p.PrincipalProperty == this.key2_1)
                .And.Contain(p => p.DependentProperty == this.otherTypeProperty2 && p.PrincipalProperty == this.key2_2);
        }

        [Fact]
        public void CreateReferentialConstraintShouldSucceedForNonKeyPrincipalProperty()
        {
            var testSubject = EdmReferentialConstraint.Create(new[] { this.otherTypeProperty1 }, new []{ this.property1 });
            testSubject.PropertyPairs.Should().HaveCount(1).And.Contain(p => p.DependentProperty == this.otherTypeProperty1 && p.PrincipalProperty == property1);
        }

        [Fact]
        public void CreateReferentialConstraintShouldNotChangeOrderOfProperties()
        {
            var testSubject = EdmReferentialConstraint.Create(new[] {this.otherTypeProperty2, this.otherTypeProperty1}, this.typeWithTwoKeys.Key());
            testSubject.PropertyPairs.Should().HaveCount(2)
                .And.Contain(p => p.DependentProperty == this.otherTypeProperty2 && p.PrincipalProperty == this.key2_1)
                .And.Contain(p => p.DependentProperty == this.otherTypeProperty1 && p.PrincipalProperty == this.key2_2);
        }

        [Fact]
        public void CreateReferentialConstraintShouldFailIfTooFewPropertiesAreGiven()
        {
            Action createWithTooFewProperties = () => EdmReferentialConstraint.Create(new[] { this.otherTypeProperty1 }, this.typeWithTwoKeys.Key());
            createWithTooFewProperties.ShouldThrow<ArgumentException>().WithMessage(Strings.Constructable_DependentPropertyCountMustMatchNumberOfPropertiesOnPrincipalType(2, 1));
        }

        [Fact]
        public void CreateReferentialConstraintShouldFailIfTooManyPropertiesAreGiven()
        {
            Action createWithTooManyProperties = () => EdmReferentialConstraint.Create(new[] { this.otherTypeProperty1, this.otherTypeProperty2 }, this.typeWithOneKey.Key());
            createWithTooManyProperties.ShouldThrow<ArgumentException>().WithMessage(Strings.Constructable_DependentPropertyCountMustMatchNumberOfPropertiesOnPrincipalType(1, 2));
        }

        [Fact]
        public void CreateReferentialConstraintShouldFailIfPropertiesAreNull()
        {
            Action createWithNullProperties = () => EdmReferentialConstraint.Create(null, this.typeWithOneKey.Key());
            createWithNullProperties.ShouldThrow<ArgumentNullException>().WithMessage("dependentProperties", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void CreateReferentialConstraintShouldFailIfPrincipalPropertiesAreNull()
        {
            Action createWithNullProperties = () => EdmReferentialConstraint.Create(Enumerable.Empty<IEdmStructuralProperty>(), null);
            createWithNullProperties.ShouldThrow<ArgumentNullException>().WithMessage("principalProperties", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void ReferentialConstraintConstructorShouldFailIfPairsAreNull()
        {
            Action constructWithNullPairs = () => new EdmReferentialConstraint(null);
            constructWithNullPairs.ShouldThrow<ArgumentNullException>().WithMessage("propertyPairs", ComparisonMode.EquivalentSubstring);
        }

    }
}
