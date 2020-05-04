//---------------------------------------------------------------------
// <copyright file="MaterializationPolicyUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
 
    using Microsoft.OData.Client.Materialization;
    using Microsoft.OData.Edm;
    using Xunit;

    public class MaterializationPolicyTests
    {
        private EdmComplexTypeReference edmAddressComplexTypeRef;

        [Fact]
        public void CreateComplexTypeTest()
        {
            var complexObject = new CustomMaterializationPolicy().CreateNewInstance(this.edmAddressComplexTypeRef, typeof(Address)) as Address;
            Assert.NotNull(complexObject);
        }

        public class Person
        {
            public int ID { get; set; }
        }

        public MaterializationPolicyTests()
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
