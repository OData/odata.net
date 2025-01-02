//---------------------------------------------------------------------
// <copyright file="ClientEdmModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Client.Metadata;
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

        [Fact]
        public async Task GetOrCreateEdmTypeIsThreadSafe()
        {
            // This test aims to reproduce the race condition
            // described in this issue: https://github.com/OData/odata.net/issues/2532
            // We run the test multiple times to increase the chances that, if the implementaton is not thread-safe,
            // the race conditions issues will occur at least once
            for (int i = 0; i < 5000; i++)
            {
                await TestGetOrCreateEdmTypeConsitency();
            }
        }

        private async Task TestGetOrCreateEdmTypeConsitency()
        {
            ClientEdmModel clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V401);
            Type productEntityType = typeof(ProductEntity);
            Type productType = typeof(Product);

            Task task1 = Task.Run(() =>
            {
                IEdmStructuredType edmType = clientEdmModel.GetOrCreateEdmType(productEntityType) as IEdmStructuredType;
                edmType.DeclaredProperties.ToArray(); // this ensures the properties are also loaded
            });

            Task task2 = Task.Run(() =>
            {
                IEdmStructuredType edmType = clientEdmModel.GetOrCreateEdmType(productEntityType) as IEdmStructuredType;
                edmType.DeclaredProperties.ToArray();
            });

            await Task.WhenAll(task1, task2);

            AssertClientTypeIsConsistent(productEntityType, clientEdmModel);
            AssertClientTypeIsConsistent(productType, clientEdmModel);
        }

        private void AssertClientTypeIsConsistent(Type type, ClientEdmModel clientEdmModel)
        {
            IEdmType edmType = clientEdmModel.GetOrCreateEdmType(type);
            ClientTypeAnnotation typeAnnotation = clientEdmModel.GetClientTypeAnnotation(type.FullName);
            IEdmSchemaType schemaType = clientEdmModel.FindDeclaredType(type.FullName);
            Assert.True(object.ReferenceEquals(edmType, typeAnnotation.EdmType));
            Assert.True(object.ReferenceEquals(schemaType, typeAnnotation.EdmType));
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
