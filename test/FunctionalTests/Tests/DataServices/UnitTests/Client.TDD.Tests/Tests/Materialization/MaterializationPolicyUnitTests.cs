//---------------------------------------------------------------------
// <copyright file="MaterializationPolicyUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Materialization;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MaterializationPolicyTests
    {
        private EdmComplexTypeReference edmAddressComplexTypeRef;

        [TestMethod]
        public void CreateComplexTypeTest()
        {
            var complexObject = new CustomMaterializationPolicy().CreateNewInstance(this.edmAddressComplexTypeRef, typeof(Address)) as Address;
            Assert.IsNotNull(complexObject);
        }

        public class Person
        {
            public int ID { get; set; }
        }

        [TestInitialize]
        public void Initialize()
        {
            var addressEdmType = new EdmComplexType("Default", "Address");
            addressEdmType.AddStructuralProperty("ZipCode", EdmPrimitiveTypeKind.String);

            this.edmAddressComplexTypeRef = new EdmComplexTypeReference(addressEdmType, true);
        }

        public class Address
        {
            public string ZipCode { get; set; }
        }

        internal class CustomMaterializationPolicy : MaterializationPolicy
        {
            
        }
    }   
}
