//---------------------------------------------------------------------
// <copyright file="EdmReferentialConstraintTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
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
            var pair = Assert.Single(testSubject.PropertyPairs);
            Assert.Same(this.otherTypeProperty1, pair.DependentProperty);
            Assert.Same(this.key1_1, pair.PrincipalProperty);
        }

        [Fact]
        public void CreateReferentialConstraintShouldSucceedForTwoPropertyKey()
        {
            var testSubject = EdmReferentialConstraint.Create(new[] {this.otherTypeProperty1, this.otherTypeProperty2}, this.typeWithTwoKeys.Key());
            Assert.Equal(2, testSubject.PropertyPairs.Count());

            Assert.Contains(testSubject.PropertyPairs, p => p.DependentProperty == this.otherTypeProperty1 && p.PrincipalProperty == this.key2_1);
            Assert.Contains(testSubject.PropertyPairs, p => p.DependentProperty == this.otherTypeProperty2 && p.PrincipalProperty == this.key2_2);
        }

        [Fact]
        public void CreateReferentialConstraintShouldSucceedForNonKeyPrincipalProperty()
        {
            var testSubject = EdmReferentialConstraint.Create(new[] { this.otherTypeProperty1 }, new []{ this.property1 });
            var pair = Assert.Single(testSubject.PropertyPairs);
            Assert.Same(this.otherTypeProperty1, pair.DependentProperty);
            Assert.Same(this.property1, pair.PrincipalProperty);
        }

        [Fact]
        public void CreateReferentialConstraintShouldNotChangeOrderOfProperties()
        {
            var testSubject = EdmReferentialConstraint.Create(new[] {this.otherTypeProperty2, this.otherTypeProperty1}, this.typeWithTwoKeys.Key());
            Assert.Equal(2, testSubject.PropertyPairs.Count());
            Assert.Contains(testSubject.PropertyPairs, p => p.DependentProperty == this.otherTypeProperty2 && p.PrincipalProperty == this.key2_1);
            Assert.Contains(testSubject.PropertyPairs, p => p.DependentProperty == this.otherTypeProperty1 && p.PrincipalProperty == this.key2_2);
        }

        [Fact]
        public void CreateReferentialConstraintShouldFailIfTooFewPropertiesAreGiven()
        {
            Action createWithTooFewProperties = () => EdmReferentialConstraint.Create(new[] { this.otherTypeProperty1 }, this.typeWithTwoKeys.Key());

            var exception = Assert.Throws<ArgumentException>(createWithTooFewProperties);
            Assert.Equal(Strings.Constructable_DependentPropertyCountMustMatchNumberOfPropertiesOnPrincipalType(2, 1), exception.Message);
        }

        [Fact]
        public void CreateReferentialConstraintShouldFailIfTooManyPropertiesAreGiven()
        {
            Action createWithTooManyProperties = () => EdmReferentialConstraint.Create(new[] { this.otherTypeProperty1, this.otherTypeProperty2 }, this.typeWithOneKey.Key());
            var exception = Assert.Throws<ArgumentException>(createWithTooManyProperties);
            Assert.Equal(Strings.Constructable_DependentPropertyCountMustMatchNumberOfPropertiesOnPrincipalType(1, 2), exception.Message);
        }

        [Fact]
        public void CreateReferentialConstraintShouldFailIfPropertiesAreNull()
        {
            Action createWithNullProperties = () => EdmReferentialConstraint.Create(null, this.typeWithOneKey.Key());
            Assert.Throws<ArgumentNullException>("dependentProperties", createWithNullProperties);
        }

        [Fact]
        public void CreateReferentialConstraintShouldFailIfPrincipalPropertiesAreNull()
        {
            Action createWithNullProperties = () => EdmReferentialConstraint.Create(Enumerable.Empty<IEdmStructuralProperty>(), null);
            Assert.Throws<ArgumentNullException>("principalProperties", createWithNullProperties);
        }

        [Fact]
        public void ReferentialConstraintConstructorShouldFailIfPairsAreNull()
        {
            Action constructWithNullPairs = () => new EdmReferentialConstraint(null);
            Assert.Throws<ArgumentNullException>("propertyPairs", constructWithNullPairs);
        }

    }
}
