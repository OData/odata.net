//---------------------------------------------------------------------
// <copyright file="ClientEdmModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.Tests
{
    public class ClientEdmModelTests
    {
        /// <summary>
        /// A test to show that an abstract complex type with no properties defined is 
        /// assignable from a concrete complex type that extends it.
        /// Product is the abstract complex type with no properties defined and
        /// ProductA is the concrete complex type that extends the abstract 
        /// complex type(Product) with no properties defined. 
        /// </summary>
        [Fact]
        public void BaseTypeOfProductAIsProduct()
        {
            //Arrange
            Type productA = typeof(ProductA);
            Type product = typeof(Product);
            var expectedBaseTypeOfProductA = product.FullName;

            //Act
            ClientEdmModel clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V401);
            IEdmStructuredType type = clientEdmModel.GetOrCreateEdmType(productA) as IEdmStructuredType;
            var resultingBaseTypeOfProductA = type.BaseType.ToString();

            //Assert
            Assert.Equal(expectedBaseTypeOfProductA, resultingBaseTypeOfProductA);
        }
      
    }

    /// <summary>
    /// an abstract complex type without properties defined
    /// </summary>
    public abstract class Product
    {

    }
    /// <summary>
    /// an concrete complex type that extends an abstract complex type with no properties defined.
    /// </summary>
    public class ProductA : Product
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public string DetailsA { get; set; }

    }
    //Entity Type
    public class ProductEntity
    {
        public int Id { get; set; }
        public Product Product { get; set; }
    }



}
