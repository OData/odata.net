//---------------------------------------------------------------------
// <copyright file="ResourceSetTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using System;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AstoriaTestNS = System.Data.Test.Astoria;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ResourceSet class.
    /// </summary>
    [TestClass, TestCase]
    public class ResourceSetTests
    {
        [TestMethod, Variation("Verifies that resource set doesn't allow certain operations.")]
        public void InvalidCasesTest()
        {
            ResourceType complexType = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "foo", "Address", false);
            ResourceType primitiveType = ResourceType.GetPrimitiveResourceType(typeof(int));
            ResourceType collectionType = ResourceType.GetCollectionResourceType(primitiveType);

            AstoriaTestNS.TestUtil.RunCombinations(
                new ResourceType[] { complexType, primitiveType, collectionType },
                (resourceType) =>
                {
                    ExceptionUtils.ExpectedException<ArgumentException>(
                        () => new ResourceSet("Foo", resourceType),
                        "The ResourceTypeKind property of a ResourceType instance that is associated with a ResourceSet must have a value of 'EntityType'.",
                        "Cannot add non-entity types to container");
                });
        }

        [TestMethod, Variation("Verified that resource set correctly stores property values.")]
        public void PropertiesValidationTest()
        {
            ResourceType entityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "TestNS", "Customer", false);
            ResourceSet resourceSet = new ResourceSet("Customers", entityType);

            Assert.IsFalse(resourceSet.IsReadOnly, "ResourceSet should be created writable.");
            Assert.IsNull(resourceSet.CustomState, "No custom state should be present after creation.");
            Assert.AreEqual("Customers", resourceSet.Name, "The name of the resource set was not set correctly.");
            Assert.IsTrue(object.ReferenceEquals(entityType, resourceSet.ResourceType), "The element type of the resource set was not set correctly.");
            Assert.IsFalse(resourceSet.UseMetadataKeyOrder, "The default value for UseMetadataKeyOrder should be false.");

            resourceSet.UseMetadataKeyOrder = true;
            Assert.IsTrue(resourceSet.UseMetadataKeyOrder, "New value for UseMetadataKeyOrder was not set correctly.");
        }
    }
}
