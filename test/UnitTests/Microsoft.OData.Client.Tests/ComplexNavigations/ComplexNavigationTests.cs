//---------------------------------------------------------------------
// <copyright file="ComplexNavigationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client.Metadata;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.OData.Client.Tests.ComplexNavigations
{
    /// <summary>
    ///tests to show that it is possible to have an entity navigation on a complex type. 
    /// </summary>
    public class ComplexNavigationTests
    {
        /// <summary>
        /// This test checks whether the declaring type kind of the navigation property created is complex.
        /// </summary>
        [Fact]
        public void DeclaringTypeOFAnEntityNavigationCanBeAComplexType()
        {
            //Arrange
            Type complexDeclaringType = typeof(Address);          
            Type entityNavigationType = typeof(City);        
            EdmTypeKind expectedDeclaringTypeKind = EdmTypeKind.Complex;

            //Act
            ClientEdmModel clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V401);
            IEdmType edmTypeOfComplexDeclaringType = clientEdmModel.GetOrCreateEdmType(complexDeclaringType);
            IEdmType edmTypeOfEntityNavigationType = clientEdmModel.GetOrCreateEdmType(entityNavigationType);
            IEdmStructuredType entiyNavigationType = clientEdmModel.GetOrCreateEdmType(complexDeclaringType) as IEdmStructuredType;
            EdmNavigationProperty edmNavigationProperty = EdmNavigationProperty.CreateNavigationPropertyWithPartner("City",ClientTypeUtil.ToEdmTypeReference(edmTypeOfEntityNavigationType, true),null,null,false, EdmOnDeleteAction.None, "Partner", ClientTypeUtil.ToEdmTypeReference(edmTypeOfComplexDeclaringType, true),null,null,false,EdmOnDeleteAction.None);
            EdmTypeKind resultingDeclaringTypeKind = edmNavigationProperty.DeclaringType.TypeKind;

            //Assert
            Assert.Equal(expectedDeclaringTypeKind, resultingDeclaringTypeKind);
        }
    }

    [EntityType]
    [Key("UserName")]
    public class Person
    {
        public string UserName { get; set; }
        public Address Address { get; set; }
        public List<Address> Addresses { get; set; }
    }

    [EntityType]
    [Key("Name")]
    public class City
    {
        public string Name { get; set; }
    }

    public class Address
    {
        public string Road { get; set; }
        public City City { get; set; }
    }
}
