//---------------------------------------------------------------------
// <copyright file="UtilUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using Microsoft.OData.Client;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UtilUnitTests
    {
        [TestMethod]
        public void CreateEmptyConstructorType()
        {
            var result = Util.ActivatorCreateInstance(typeof(Address)) as Address;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateTypeWithArguments()
        {
            var result = Util.ActivatorCreateInstance(typeof(AddressWithNoEmptyConstructor), "98102") as AddressWithNoEmptyConstructor;
            Assert.IsNotNull(result);
            result.ZipCode.Should().Be("98102");
        }

        [TestMethod]
        public void CreateTypeWithNoEmptyConstructor()
        {
            Action test = () => Util.ActivatorCreateInstance(typeof(AddressWithNoEmptyConstructor));
            test.ShouldThrow<MissingMethodException>().WithMessage("No parameterless constructor defined for this object.");
        }

        [TestMethod]
        public void CreateTypeWithConstructorArgsMismatch()
        {
            Action test = () => Util.ActivatorCreateInstance(typeof(AddressWithNoEmptyConstructor), "zipcode", "streetname");
            test.ShouldThrow<MissingMethodException>().WithMessage("Constructor on type 'AstoriaUnitTests.Tests.UtilUnitTests+AddressWithNoEmptyConstructor' not found.");
        }

        [TestMethod]
        public void IsNullableIntValueTypeShouldReturnFalse()
        {
            Util.IsNullableType(typeof(int)).Should().Be(false);
        }

        [TestMethod]
        public void IsNullableStringTypeShouldReturnTrue()
        {
            Util.IsNullableType(typeof(string)).Should().Be(true);
        }

        [TestMethod]
        public void IsNullableNullableIntValueTypeShouldReturnTrue()
        {
            Util.IsNullableType(typeof(int?)).Should().Be(true);
        }

        [TestMethod]
        public void IsNullableClassTypeShouldReturnTrue()
        {
            Util.IsNullableType(typeof(Address)).Should().Be(true);
        }

        //SaveChangesOptions.BatchWithSingleChangeset ==> batch with single changeset
        //SaveChangesOptions.BatchWithIndependentOpterations == > batch with independent operations
        [TestMethod]
        public void IsBatchShouldBeTrueForBatchWithSingleChangeset()
        {
            Util.IsBatch(SaveChangesOptions.BatchWithSingleChangeset).Should().BeTrue();
        }

        [TestMethod]
        public void IsBatchShouldBeTrueForBatchWithIndependentOperations()
        {
            Util.IsBatch(SaveChangesOptions.BatchWithIndependentOperations).Should().BeTrue();
        }

        [TestMethod]
        public void IsBatchShouldBeFalseForNonBatchOptions()
        {
            Util.IsBatch(SaveChangesOptions.ReplaceOnUpdate).Should().BeFalse();
        }

        [TestMethod]
        public void IsBatchShouldBeFalseForBatchBitwiseAndBatchWithIndependentOperations()
        {
            Util.IsBatch((SaveChangesOptions.BatchWithSingleChangeset & SaveChangesOptions.BatchWithIndependentOperations)).Should().BeFalse();
        }

        // test for IsBatchWithIndependentOperations
        [TestMethod]
        public void IsBatchWithIndependentOperationsShouldBeFalseForBatchWithSingleChangeset()
        {
            Util.IsBatchWithIndependentOperations(SaveChangesOptions.BatchWithSingleChangeset).Should().BeFalse();
        }

        [TestMethod]
        public void IsBatchWithIndependentOperationsShouldBeTrueForBatchWithIndependentOperations()
        {
            Util.IsBatchWithIndependentOperations(SaveChangesOptions.BatchWithIndependentOperations).Should().BeTrue();
        }

        [TestMethod]
        public void IsBatchWithIndependentOperationsShouldBeFalseForBothBatchFlagsBitwiseAnd()
        {
            Util.IsBatchWithIndependentOperations((SaveChangesOptions.BatchWithSingleChangeset & SaveChangesOptions.BatchWithIndependentOperations)).Should().BeFalse();
        }

        // test for IsBatchWithSingleChangeset
        [TestMethod]
        public void IsBatchWithSingleChangesetShouldBeTrueForBatchWithSingleChangeset()
        {
            Util.IsBatchWithSingleChangeset(SaveChangesOptions.BatchWithSingleChangeset).Should().BeTrue();
        }

        [TestMethod]
        public void IsBatchWithSingleChangesetShouldBeFalseForBatchWithIndependentOperations()
        {
            Util.IsBatchWithSingleChangeset(SaveChangesOptions.BatchWithIndependentOperations).Should().BeFalse();
        }

        [TestMethod]
        public void IsBatchWithSingleChangesetShouldBeFalseForBothBatchFlagsBitwiseAnd()
        {
            Util.IsBatchWithSingleChangeset((SaveChangesOptions.BatchWithSingleChangeset & SaveChangesOptions.BatchWithIndependentOperations)).Should().BeFalse();
        }


        public class Address
        {
            public string ZipCode { get; set; }
        }

        public class AddressWithNoEmptyConstructor
        {
            public AddressWithNoEmptyConstructor(string zipCode)
            {
                this.ZipCode = zipCode;
            }

            public string ZipCode { get; set; }
        }
    }
}
