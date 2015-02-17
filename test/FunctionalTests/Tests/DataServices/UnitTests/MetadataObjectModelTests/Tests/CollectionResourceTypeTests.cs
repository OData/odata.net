//---------------------------------------------------------------------
// <copyright file="CollectionResourceTypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AstoriaTestNS = System.Data.Test.Astoria;
    #endregion Namespaces

    /// <summary>
    /// Tests for the CollectionResourceType class
    /// </summary>
    [TestClass, TestCase]
    public class CollectionResourceTypeTests
    {
        [TestMethod(), Variation("Verify the behavior of a collection type, and its property values.")]
        public void CollectionTypeValidation()
        {
            ResourceType entityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "foo", "Order", false);
            ResourceType complexType = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "foo", "bar", false);
            ResourceType complexType2 = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "foo", "bar2", false);
            complexType2.AddProperty(new ResourceProperty("CollectionProperty", ResourcePropertyKind.Collection, ResourceType.GetCollectionResourceType(complexType)));

            var itemTypes = new ResourceType[] { complexType, complexType2 }.Concat(ResourceTypeUtils.GetPrimitiveResourceTypes());
            var collectionTypes = itemTypes.Except(new[] { ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)) }).Select(it => new { ItemType = it, CollectionType = ResourceType.GetCollectionResourceType(it) });
            AstoriaTestNS.TestUtil.RunCombinations(
                collectionTypes,
                c =>
                {
                    Assert.AreEqual(c.ItemType, c.CollectionType.ItemType, "The item type of the collection doesn't match the one specified upon creation.");
                    Assert.AreEqual("Collection(" + c.ItemType.FullName + ")", c.CollectionType.FullName, "The full name of a collection type is wrong.");
                    Assert.AreEqual("Collection(" + c.ItemType.FullName + ")", c.CollectionType.Name, "The name of a collection type is wrong.");
                    Assert.AreEqual("", c.CollectionType.Namespace, "The namespace of a collection type should be empty.");
                    Assert.IsTrue(c.CollectionType.IsReadOnly, "The collection type is always read-only.");
                    Assert.IsFalse(c.CollectionType.IsAbstract, "Collection type is never abstract.");
                    Assert.IsFalse(c.CollectionType.IsOpenType, "Collection type is never open.");
                    Assert.IsFalse(c.CollectionType.IsMediaLinkEntry, "Collection type is never an MLE.");
                    Assert.AreEqual(typeof(IEnumerable<>).MakeGenericType(c.ItemType.InstanceType), c.CollectionType.InstanceType, "The instance type of the collection type is wrong.");
                    Assert.IsTrue(c.CollectionType.CanReflectOnInstanceType, "Collection type has CanReflectOnInstanceType always true.");
                    Assert.IsNull(c.CollectionType.BaseType, "Collection type has never a base type.");
                    Assert.AreEqual(ResourceTypeKind.Collection, c.CollectionType.ResourceTypeKind, "The kind of a collection type is always Collection.");
                    Assert.AreEqual(0, c.CollectionType.PropertiesDeclaredOnThisType.Count(), "Collection type has no properties.");
                    Assert.AreEqual(0, c.CollectionType.Properties.Count(), "Collection type has no properties.");
                    Assert.AreEqual(0, c.CollectionType.KeyProperties.Count(), "Collection type has no properties.");
                    Assert.AreEqual(0, c.CollectionType.ETagProperties.Count(), "Collection type has no properties.");
                    Assert.IsNull(c.CollectionType.CustomState, "Custom state should be null by default.");

                    ExceptionUtils.ThrowsException<InvalidOperationException>(
                        () => c.CollectionType.IsMediaLinkEntry = true,
                        "Setting MLE on collection type should fail.");
                    ExceptionUtils.ThrowsException<InvalidOperationException>(
                        () => c.CollectionType.IsOpenType = true,
                        "Setting IsOpenType on collection type should fail.");
                    ExceptionUtils.ThrowsException<InvalidOperationException>(
                        () => c.CollectionType.CanReflectOnInstanceType = false,
                        "Setting CanReflectOnInstanceType on collection type should fail.");
                    // Setting a custom state should still work
                    c.CollectionType.CustomState = "some value";
                    Assert.AreEqual("some value", c.CollectionType.CustomState, "Custom state doesn't persist its value.");

                    ExceptionUtils.ThrowsException<InvalidOperationException>(
                        () => c.CollectionType.AddProperty(new ResourceProperty("ID", ResourcePropertyKind.ComplexType, complexType)),
                        "Adding a property on collection type should fail.");
                    
                    c.CollectionType.SetReadOnly();
                    Assert.IsTrue(c.CollectionType.IsReadOnly, "The collection type is always read-only.");
                });

            // Verify that only primitive and complex types are allowed as items in a collection
            ResourceType collectionType = ResourceType.GetCollectionResourceType(complexType);
            foreach (var t in new ResourceType[] { entityType, collectionType })
            {
                Exception exception = AstoriaTestNS.TestUtil.RunCatching(() => ResourceType.GetCollectionResourceType(t));
                Assert.IsTrue(exception is ArgumentException, "Exception of a wrong type");
                Assert.AreEqual(
                    "Only collection properties that contain primitive types or complex types are supported.",
                    exception.Message, "Wrong exception message.");
            }
        }
    }
}
