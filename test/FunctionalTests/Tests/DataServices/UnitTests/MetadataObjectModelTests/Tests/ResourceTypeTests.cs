//---------------------------------------------------------------------
// <copyright file="ResourceTypeTests.cs" company="Microsoft">
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
    /// Tests for the ResourceType class
    /// </summary>
    [TestClass, TestCase]
    public class ResourceTypeTests
    {
        [TestMethod, Variation("Verify behavior of property on ResourceType.")]
        public void SettablePropertiesValidationTest()
        {
            ResourceType rt = (ResourceType)ResourceTypeUtils.GetTestInstance(typeof(ResourceType));

            // Validate custom state can be modified when its not readonly
            Assert.IsTrue(rt.CustomState == null, "expecting custom state to be null");
            rt.CustomState = "Foo";
            Assert.IsTrue((string)rt.CustomState == "Foo", "expecting custom state to be changed");

            // Validate properties can be modified/added/deleted and the change shows up in the properties collection
            int c1 = rt.PropertiesDeclaredOnThisType.Count;
            int c2 = rt.Properties.Count;

            rt.AddProperty(new ResourceProperty("bar", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(double))));

            Assert.IsTrue(rt.PropertiesDeclaredOnThisType.Count == c1 + 1, "New properties should show up in PropertiesDeclaredOnThisType collection");
            Assert.IsTrue(rt.Properties.Count == c2 + 1, "New Property must show up in Properties collection");

            bool isOpenType = rt.IsOpenType;
            rt.IsOpenType = !isOpenType;
            Assert.IsTrue(rt.IsOpenType != isOpenType, "IsOpenType can not be updated");

            // add a new key property and make sure it shows up
            int k1 = rt.KeyProperties.Count;
            rt.AddProperty(new ResourceProperty("anotherKeyProperty", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(Single))));
            Assert.IsTrue(rt.KeyProperties.Count == k1 + 1, "Key Properties count must have incremented");
            Assert.IsTrue(rt.KeyProperties.Single(p => p.Name == "anotherKeyProperty") != null, "must be able to find the new key property");

            // Check for etag properties
            int e1 = rt.ETagProperties.Count;
            rt.AddProperty(new ResourceProperty("etagProperty", ResourcePropertyKind.Primitive | ResourcePropertyKind.ETag, ResourceType.GetPrimitiveResourceType(typeof(Single))));
            Assert.IsTrue(rt.ETagProperties.Count == e1 + 1, "Etag Properties count must have incremented");
            Assert.IsTrue(rt.ETagProperties.Single(p => p.Name == "etagProperty") != null, "must be able to find the new etag property");
        }

        [TestMethod, Variation("Validates creation of all available primitive resource types.")]
        public void PrimitiveTypesTest()
        {
            // This will call GetPrimitiveResourceType for all valid primitive types
            foreach (ResourceType resourceType in ResourceTypeUtils.GetPrimitiveResourceTypes())
            {
                Assert.AreEqual(ResourceTypeKind.Primitive, resourceType.ResourceTypeKind, "The kind of a primitive resource type should be Primitive.");
            }

            // Now try a few invalid cases
            foreach (Type type in new Type[] { typeof(System.Xml.Linq.XDocument), typeof(DBNull), typeof(UInt16) })
            {
                Assert.IsNull(
                    ResourceType.GetPrimitiveResourceType(type),
                    "ResourceType.GetPrimitiveResourceType should not work for type " + type.ToString() + ".");
            }
        }

        [TestMethod, Variation("Validates the behavior of ResourceType properties collection.")]
        public void AddingPropertiesTest()
        {
            ResourceType complexType = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "namespace", "Address", false);
            ResourceType entityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Namespace", "Order", false);

            IEnumerable<ResourceProperty> primitiveResourceProperties =
                ResourceTypeUtils.GetPrimitiveResourceTypes().Where(rt => rt.FullName != "Edm.Stream").Select(rt => new ResourceProperty("PrimitiveProperty", ResourcePropertyKind.Primitive, rt));
            IEnumerable<ResourceProperty> keyResourceProperties =
                GetKeyResourceTypes().Where(rt => !rt.FullName.StartsWith("Edm.Geography") && !rt.FullName.StartsWith("Edm.Geometry")).Select(rt => new ResourceProperty("KeyProperty", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, rt));
            IEnumerable<ResourceProperty> etagResourceProperties =
                ResourceTypeUtils.GetPrimitiveResourceTypes().Where(rt => !rt.FullName.StartsWith("Edm.Geography") && !rt.FullName.StartsWith("Edm.Geometry") && rt.FullName != "Edm.Stream").Select(rt => new ResourceProperty("ETagProperty", ResourcePropertyKind.Primitive | ResourcePropertyKind.ETag, rt));

            IEnumerable<ResourceProperty> complexProperties = new ResourceProperty[] {
                    new ResourceProperty("ComplexProperty", ResourcePropertyKind.ComplexType, complexType) };
            IEnumerable<ResourceProperty> navigationProperties = new ResourceProperty[] {
                    new ResourceProperty("ResourceReferenceProperty", ResourcePropertyKind.ResourceReference, entityType),
                    new ResourceProperty("ResourceSetReferenceProperty", ResourcePropertyKind.ResourceSetReference, entityType)};
            IEnumerable<ResourceProperty> collectionProperties = ResourceTypeUtils.GetPrimitiveResourceTypes().Except(new [] { ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)) }).Concat(new ResourceType[] { complexType })
                .Select(rt => new ResourceProperty("CollectionProperty", ResourcePropertyKind.Collection, ResourceType.GetCollectionResourceType(rt)));

            // Primitive resource type - no properties can be added
            foreach (ResourceType primitiveResourceType in ResourceTypeUtils.GetPrimitiveResourceTypes())
                foreach (ResourceProperty property in primitiveResourceProperties.Concat(keyResourceProperties).Concat(etagResourceProperties)
                                                        .Concat(complexProperties).Concat(navigationProperties).Concat(collectionProperties))
                {
                    ExceptionUtils.ThrowsException<InvalidOperationException>(
                        () => primitiveResourceType.AddProperty(property),
                        string.Format("Adding property {0} to primitive resource type {1} should fail.", property.Name, primitiveResourceType.FullName));
                }

            // Collection resource type - no properties can be added
            foreach (ResourceType CollectionResourceType in ResourceTypeUtils.GetPrimitiveResourceTypes().Except(new[] { ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)) }).Concat(new ResourceType[] { complexType })
                                                            .Select(rt => ResourceType.GetCollectionResourceType(rt)))
                foreach (ResourceProperty property in primitiveResourceProperties.Concat(keyResourceProperties).Concat(etagResourceProperties)
                                                        .Concat(complexProperties).Concat(navigationProperties).Concat(collectionProperties))
                {
                    ExceptionUtils.ThrowsException<InvalidOperationException>(
                        () => CollectionResourceType.AddProperty(property),
                        string.Format("Adding property {0} to a collection resource type {1} should fail.", property.Name, CollectionResourceType.FullName));
                }

            // Collection resource type - no properties can be added
            foreach (ResourceType collectionResourceType in (new ResourceType[] { entityType }).Select(rt => ResourceType.GetEntityCollectionResourceType(rt)))
                foreach (ResourceProperty property in primitiveResourceProperties.Concat(keyResourceProperties).Concat(etagResourceProperties)
                                                        .Concat(complexProperties).Concat(navigationProperties).Concat(collectionProperties))
                {
                    ExceptionUtils.ThrowsException<InvalidOperationException>(
                        () => collectionResourceType.AddProperty(property),
                        string.Format("Adding property {0} to a collection resource type {1} should fail.", property.Name, collectionResourceType.FullName));
                }

            // Complex resource type - only primitive, complex and collection properties can be added
            foreach (ResourceProperty property in primitiveResourceProperties.Concat(complexProperties).Concat(collectionProperties))
            {
                ResourceType t = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "Ns", "Address", false);
                t.AddProperty(property);
            }

            // It is possible to add navigation property to a complex type.
            // Once this is fixed remove this foreach and add navigationProperties to the list for the next foreach.
            foreach (var p in navigationProperties)
            {
                ResourceType t = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "Ns", "Address", false);
                t.AddProperty(p);
                t.SetReadOnly();
            }

            foreach (ResourceProperty property in keyResourceProperties.Concat(etagResourceProperties))
            {
                ResourceType t = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "Ns", "Address", false);
                ExceptionUtils.ThrowsException<InvalidOperationException>(
                    () => t.AddProperty(property),
                    string.Format("Adding key or etag property {0} to complex resource type should fail.", property.Name));
            }

            // Entity resource type
            foreach (ResourceProperty property in primitiveResourceProperties.Concat(keyResourceProperties).Concat(etagResourceProperties)
                                                    .Concat(complexProperties).Concat(navigationProperties).Concat(collectionProperties))
            {
                ResourceType t = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Ns", "Order", false);
                if (property.ResourceType.InstanceType == typeof(System.IO.Stream) && (property.Kind & ResourcePropertyKind.Stream) != ResourcePropertyKind.Stream)
                {
                    ExceptionUtils.ThrowsException<InvalidOperationException>(
                        () =>
                        {
                            t.AddProperty(property);
                        },
                        "Adding Stream type as primitive property should fail");
                }
                else
                {
                    t.AddProperty(property);
                }
            }
        }

        [TestMethod, Variation("Verifies that resource type doesn't allow certain operations.")]
        public void InvalidCasesTest()
        {
            ResourceType rt = (ResourceType)ResourceTypeUtils.GetTestInstance(typeof(ResourceType));

            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => rt.AddProperty(new ResourceProperty("ID", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(double)))),
                "A property with same name 'ID' already exists in type 'Foo.NoBaseType'. Please make sure that there is no property with the same name defined in one of the base types.",
                "cannot add property with the same name");

            ResourceType derivedType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, rt, "Namespace", "CustomerWithBirthday", false);

            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => derivedType.AddProperty(new ResourceProperty("p1", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(int)))),
                "Key properties cannot be defined in derived types.",
                "Cannot add key properties to derived type");

            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => {
                    ResourceType resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "NoBaseType", false);
                    resourceType.SetReadOnly();
                    // Accessing any properties collection will invoke the validation logic
                    resourceType.KeyProperties.Single();
                },
                "The entity type 'Foo.NoBaseType' does not have any key properties. Please make sure that one or more key properties are defined for this entity type.",
                "Entity Type must have key Properties");

            // should be able to add property with same name as that in the base type. This will throw when we make the derived type readonly.
            derivedType.AddProperty(new ResourceProperty("ID", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))));
            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => {
                    derivedType.SetReadOnly();
                    // Accessing any properties collection will invoke the validation logic
                    derivedType.Properties.First();
                },
                "A property with same name 'ID' already exists in type 'Namespace.CustomerWithBirthday'. Please make sure that there is no property with the same name defined in one of the base types.",
                "cannot add property with the same name as that in base type");

            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceType),
                "ResourceTypeKind.Primitive, ResourceTypeKind.Collection and ResourceTypeKind.EntityCollection are not valid values for the 'resourceTypeKind' parameter.\r\nParameter name: resourceTypeKind",
                typeof(object), ResourceTypeKind.Primitive, null, "foo", "bar", false);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceType),
                "ResourceTypeKind.Primitive, ResourceTypeKind.Collection and ResourceTypeKind.EntityCollection are not valid values for the 'resourceTypeKind' parameter.\r\nParameter name: resourceTypeKind",
                typeof(object), ResourceTypeKind.Collection, null, "foo", "bar", false);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceType),
                "ResourceTypeKind.Primitive, ResourceTypeKind.Collection and ResourceTypeKind.EntityCollection are not valid values for the 'resourceTypeKind' parameter.\r\nParameter name: resourceTypeKind",
                typeof(object), ResourceTypeKind.EntityCollection, null, "foo", "bar", false);

            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceType),
                "The CLR type for the resource type cannot be a value type.\r\nParameter name: instanceType", 
                typeof(int), ResourceTypeKind.ComplexType, null, "foo", "bar", false);

            ExceptionUtils.ExpectedException<ArgumentException>(
                () => ResourceType.GetCollectionResourceType(ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))),
                "The ItemType of a collection resource type cannot be of type 'Edm.Stream'.\r\nParameter name: itemType");

            ResourceType ct = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "Foo", "Bar", false);
            ResourceType pt = ResourceType.GetPrimitiveResourceType(typeof(string));
            ResourceType primitiveCollection = ResourceType.GetCollectionResourceType(pt);
            ResourceType et = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "EntityResourceType", false);
            ResourceType entityCollection = ResourceType.GetEntityCollectionResourceType(et);

            ExceptionUtils.ExpectedException<ArgumentException>(
                () => ResourceType.GetEntityCollectionResourceType(null),
                "Value cannot be null.\r\nParameter name: itemType");
            ResourceType[] types = new ResourceType[] { ct, pt, primitiveCollection, entityCollection };
            AstoriaTestNS.TestUtil.RunCombinations(
                types,
                r =>
                {
                    ExceptionUtils.ExpectedException<ArgumentException>(
                        () => ResourceType.GetEntityCollectionResourceType(r),
                        "Only collections of an entity type are supported.");
                });

            types = new ResourceType[] { ct, pt, primitiveCollection, entityCollection };
            ResourceProperty key = new ResourceProperty("KeyProperty", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string)));
            ResourceProperty etag = new ResourceProperty("ETagProperty", ResourcePropertyKind.ETag | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string)));
            ResourceProperty primitive = new ResourceProperty("Property", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string)));
            AstoriaTestNS.TestUtil.RunCombinations(
                types,
                r =>
                {
                    ExceptionUtils.ThrowsException<InvalidOperationException>(
                        () => r.AddProperty(key),
                        "Cannot add key property to non-entity types");

                    ExceptionUtils.ThrowsException<InvalidOperationException>(
                        () => r.AddProperty(etag),
                        "Cannot add key property to non-entity types");

                    ExceptionUtils.ThrowsException<InvalidOperationException>(
                        () => r.AddProperty(new ResourceProperty("Stream1", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)))),
                        "Cannot add key property to non-entity types");
                });

            ExceptionUtils.ThrowsException<InvalidOperationException>(
                () =>
                {
                    pt.AddProperty(primitive);
                    primitiveCollection.AddProperty(primitive);
                    entityCollection.AddProperty(primitive);
                },
                "Cannot add property to primitive, primitive collection or entity collection types");

            types = new ResourceType[] { pt, ct, et, primitiveCollection, entityCollection };
            AstoriaTestNS.TestUtil.RunCombinations(
                new ResourceTypeKind[] { ResourceTypeKind.EntityType, ResourceTypeKind.ComplexType },
                types,
                (derivedTypeKind, baseResourceType) =>
                {
                    if (baseResourceType.ResourceTypeKind == derivedTypeKind)
                    {
                        return;
                    }

                    ExceptionUtils.ExpectedException<ArgumentException>(
                        () => new ResourceType(typeof(object), derivedTypeKind, baseResourceType, "Foo", "NewDerivedType", false),
                        string.Format("A resource type of kind '{0}' cannot derive from a base resource type of kind '{1}'. Inheritance is only supported when resource types are of the same kind.\r\nParameter name: resourceTypeKind",
                            derivedTypeKind.ToString(), baseResourceType.ResourceTypeKind.ToString()),
                        "Cannot derive from a resource type of different kind.");
                });
        }

        [TestMethod, Variation("Verifies the instance type and name of a collection type.")]
        public void EntityCollectionResourceTypeTest()
        {
            ResourceType entityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "foo", "Customer", false);
            ResourceType collectionType = ResourceType.GetEntityCollectionResourceType(entityType);
            Type expectedInstanceType = typeof(IEnumerable<>).MakeGenericType(entityType.InstanceType);
            Assert.AreEqual(expectedInstanceType, collectionType.InstanceType, "Incorrect instance type.");
            Assert.AreEqual("Collection(" + entityType.FullName + ")", collectionType.FullName, "Incorrect fullname.");
            Assert.AreEqual("Collection(" + entityType.FullName + ")", collectionType.Name, "Incorrect name.");
        }

        [TestMethod, Variation("Verifies some basic behavior of resource type.")]
        public void BehaviourTest()
        {
            // its okay to construct types with no namespace. The full name should just return name.
            ResourceType resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, null, "Customer", false);
            Assert.IsTrue(resourceType.FullName == resourceType.Name, "When no namespace is specified, full name must be the same as name");
        }

        [TestMethod, Variation("Ordering of properties must be preserved")]
        public void PropertyOrderValidationTest()
        {
            ResourceType rt = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, null, "baseType", false);
            ResourceType complexType = new ResourceType(typeof(string), ResourceTypeKind.ComplexType, null, "foo", "bar", false);
            ResourceType rt1 = new ResourceType(typeof(byte[]), ResourceTypeKind.EntityType, null, "foo", "derivedType", false);

            List<ResourceProperty> properties = new List<ResourceProperty>();
            ResourceProperty p = new ResourceProperty("p", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(Single)));
            rt.AddProperty(p);
            properties.Add(p);

            p = new ResourceProperty("k1", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(int)));
            rt.AddProperty(p);
            properties.Add(p);

            p = new ResourceProperty("k2", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(double)));
            rt.AddProperty(p);
            properties.Add(p);

            p = new ResourceProperty("e1", ResourcePropertyKind.Primitive | ResourcePropertyKind.ETag, ResourceType.GetPrimitiveResourceType(typeof(DateTime)));
            rt.AddProperty(p);
            properties.Add(p);

            p = new ResourceProperty("e2", ResourcePropertyKind.Primitive | ResourcePropertyKind.ETag, ResourceType.GetPrimitiveResourceType(typeof(string)));
            rt.AddProperty(p);
            properties.Add(p);

            p = new ResourceProperty("complexprop", ResourcePropertyKind.ComplexType, complexType);
            rt.AddProperty(p);
            properties.Add(p);

            p = new ResourceProperty("refProp", ResourcePropertyKind.ResourceReference, rt1);
            rt.AddProperty(p);
            properties.Add(p);

            p = new ResourceProperty("collectionProp", ResourcePropertyKind.ResourceSetReference, rt1);
            rt.AddProperty(p);
            properties.Add(p);

            this.CheckOrdering(rt, properties, false);
            this.CheckOrdering(rt, properties, true);

            // now if the derived type add some more properties, then also ordering must be preserved
            ResourceType derivedType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, rt, "foo", "derived", false);
            p = new ResourceProperty("dp1", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(byte[])));
            derivedType.AddProperty(p);
            properties.Add(p);

            p = new ResourceProperty("detag", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(Int64)));
            derivedType.AddProperty(p);
            properties.Add(p);

            this.CheckOrdering(derivedType, properties, false);

            Assert.IsTrue(derivedType.PropertiesDeclaredOnThisType[0] == properties[properties.Count - 2], "order of properties must be preserved");
            Assert.IsTrue(derivedType.PropertiesDeclaredOnThisType[1] == properties[properties.Count - 1], "order of properties must be preserved");
        }

        [TestMethod, Variation("Validate that lazy loading of properties happen after read only was set.")]
        public void LazyLoadingValidationReadOnlyTest()
        {
            ResourceType customerType, derivedCustomerType, orderType, addressType;
            InitializeTypesForLazyLoading(out customerType, out derivedCustomerType, out orderType, out addressType);

            using (LazyLoadPropertiesResourceType.CreateChangeScope())
            {
                // Setting this to read only should set everything to set readonly, but none of the virtual methods should be called yet.
                derivedCustomerType.SetReadOnly();
                Assert.IsTrue(LazyLoadPropertiesResourceType.LoadPropertiesDeclaredOnThisTypeMethodCalled.Count == 0, "No virtual method should be called");
                Assert.IsTrue(derivedCustomerType.IsReadOnly, "derived customer must be set to readonly");
                Assert.IsTrue(customerType.IsReadOnly, "customer must be set to readonly");
                Assert.IsTrue(!orderType.IsReadOnly, "order must not be set to readonly, since orders property is not loaded at this point");
                Assert.IsTrue(!addressType.IsReadOnly, "address must not be set to readonly, since address property is not loaded at this point");

                // calling properties collection for derived type should invoke the virtual method for itself and its base type, but
                // not for navigation properties
                derivedCustomerType.Properties.First();
                Assert.IsTrue(LazyLoadPropertiesResourceType.LoadPropertiesDeclaredOnThisTypeMethodCalled.Count == 2, "virtual method should be called only for DerivedCustomer and customer");
                Assert.IsTrue(LazyLoadPropertiesResourceType.LoadPropertiesDeclaredOnThisTypeMethodCalled[0] == "Foo.Customer", "virtual method should be called first for Customer");
                Assert.IsTrue(LazyLoadPropertiesResourceType.LoadPropertiesDeclaredOnThisTypeMethodCalled[1] == "Foo.DerivedCustomer", "virtual method should be called first for Customer");
                Assert.IsTrue(orderType.IsReadOnly, "order must be set to readonly, since orders property is now loaded");
                Assert.IsTrue(addressType.IsReadOnly, "address must be set to readonly, since address property is now loaded");

                // Calling base type properties should not invoke the virtual method again, since its been invoked once
                customerType.Properties.First();
                Assert.IsTrue(LazyLoadPropertiesResourceType.LoadPropertiesDeclaredOnThisTypeMethodCalled.Count == 2, "virtual method should be called only for DerivedCustomer and customer");
            }
        }

        [TestMethod, Variation("Validate that lazy loading of properties happen before read only was set.")]
        public void LazyLoadingValidationNonReadOnlyTest()
        {
            ResourceType customerType, derivedCustomerType, orderType, addressType;
            InitializeTypesForLazyLoading(out customerType, out derivedCustomerType, out orderType, out addressType);

            using (LazyLoadPropertiesResourceType.CreateChangeScope())
            {
                // calling properties collection for derived type should invoke the virtual method for itself and its base type, but
                // not for navigation properties
                derivedCustomerType.Properties.First();
                Assert.IsTrue(LazyLoadPropertiesResourceType.LoadPropertiesDeclaredOnThisTypeMethodCalled.Count == 2, "virtual method should be called only for DerivedCustomer and customer");
                Assert.IsTrue(LazyLoadPropertiesResourceType.LoadPropertiesDeclaredOnThisTypeMethodCalled[0] == "Foo.Customer", "virtual method should be called first for Customer");
                Assert.IsTrue(LazyLoadPropertiesResourceType.LoadPropertiesDeclaredOnThisTypeMethodCalled[1] == "Foo.DerivedCustomer", "virtual method should be called first for Customer");
                Assert.IsTrue(!customerType.IsReadOnly, "customer must not be set to readonly");
                Assert.IsTrue(!derivedCustomerType.IsReadOnly, "derivedCustomer must not be set to readonly");
                Assert.IsTrue(!orderType.IsReadOnly, "order must not be set to readonly");
                Assert.IsTrue(!addressType.IsReadOnly, "address must not be set to readonly");

                // Calling base type properties should not invoke the virtual method again, since its been invoked once
                customerType.Properties.First();
                Assert.IsTrue(LazyLoadPropertiesResourceType.LoadPropertiesDeclaredOnThisTypeMethodCalled.Count == 2, "virtual method should be called only for DerivedCustomer and customer");

                derivedCustomerType.SetReadOnly();
                Assert.IsTrue(LazyLoadPropertiesResourceType.LoadPropertiesDeclaredOnThisTypeMethodCalled.Count == 2, "virtual method should be called only for DerivedCustomer and customer");
            }
        }

        #region Named Stream Metadata Tests

        [TestMethod, Variation("Validates NamedStreams collections on ResourceType.")]
        public void AddNamedStreamTest()
        {
            ResourceType type1 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "namespace", "type1", true);
            type1.CanReflectOnInstanceType = false;
            ResourceProperty id = new ResourceProperty("ID", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int)));
            type1.AddProperty(id);
            id.CanReflectOnInstanceTypeProperty = false;

            ResourceType type2 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, type1, "namespace", "type2", false);
            type2.AddProperty(new ResourceProperty("Stream1", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));
            type2.AddProperty(new ResourceProperty("Stream2", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));

            ResourceType type3 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, type2, "namespace", "type3", false);
            type3.AddProperty(new ResourceProperty("Stream3", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));

            Assert.AreEqual(0, type1.GetNamedStreams().Count(), "Wrong count");
            Assert.AreEqual(0, type1.GetNamedStreamsDeclaredOnThisType().Count(), "Wrong count");
            
            Assert.AreEqual(2, type2.GetNamedStreams().Count(), "Wrong count");
            Assert.IsNotNull(type2.GetNamedStreams().Single(s => s.Name == "Stream1"), "Unexpected null");
            Assert.IsNotNull(type2.GetNamedStreams().Single(s => s.Name == "Stream2"), "Unexpected null");
            
            Assert.AreEqual(2, type2.GetNamedStreamsDeclaredOnThisType().Count(), "Wrong count");
            Assert.IsNotNull(type2.GetNamedStreamsDeclaredOnThisType().Single(s => s.Name == "Stream1"), "Unexpected null");
            Assert.IsNotNull(type2.GetNamedStreamsDeclaredOnThisType().Single(s => s.Name == "Stream2"), "Unexpected null");
            
            Assert.AreEqual(3, type3.GetNamedStreams().Count(), "Wrong count");
            Assert.IsNotNull(type3.GetNamedStreams().Single(s => s.Name == "Stream1"), "Unexpected null");
            Assert.IsNotNull(type3.GetNamedStreams().Single(s => s.Name == "Stream2"), "Unexpected null");
            Assert.IsNotNull(type3.GetNamedStreams().Single(s => s.Name == "Stream3"), "Unexpected null");

            Assert.AreEqual(1, type3.GetNamedStreamsDeclaredOnThisType().Count(), "Wrong count");
            Assert.IsNotNull(type3.GetNamedStreamsDeclaredOnThisType().Single(s => s.Name == "Stream3"), "Unexpected null");

            type3.SetReadOnly();

            Assert.AreEqual(0, type1.GetNamedStreams().Count(), "Wrong count");
            Assert.AreEqual(0, type1.GetNamedStreamsDeclaredOnThisType().Count(), "Wrong count");

            Assert.AreEqual(2, type2.GetNamedStreams().Count(), "Wrong count");
            Assert.IsNotNull(type2.GetNamedStreams().Single(s => s.Name == "Stream1"), "Unexpected null");
            Assert.IsNotNull(type2.GetNamedStreams().Single(s => s.Name == "Stream2"), "Unexpected null");

            Assert.AreEqual(2, type2.GetNamedStreamsDeclaredOnThisType().Count(), "Wrong count");
            Assert.IsNotNull(type2.GetNamedStreamsDeclaredOnThisType().Single(s => s.Name == "Stream1"), "Unexpected null");
            Assert.IsNotNull(type2.GetNamedStreamsDeclaredOnThisType().Single(s => s.Name == "Stream2"), "Unexpected null");

            Assert.AreEqual(3, type3.GetNamedStreams().Count(), "Wrong count");
            Assert.IsNotNull(type3.GetNamedStreams().Single(s => s.Name == "Stream1"), "Unexpected null");
            Assert.IsNotNull(type3.GetNamedStreams().Single(s => s.Name == "Stream2"), "Unexpected null");
            Assert.IsNotNull(type3.GetNamedStreams().Single(s => s.Name == "Stream3"), "Unexpected null");

            Assert.AreEqual(1, type3.GetNamedStreamsDeclaredOnThisType().Count(), "Wrong count");
            Assert.IsNotNull(type3.GetNamedStreamsDeclaredOnThisType().Single(s => s.Name == "Stream3"), "Unexpected null");
        }

        [TestMethod, Variation("Validates the correct things are checked when adding named streams.")]
        public void AddNamedStreamValidationTest()
        {
            ResourceProperty idProperty = new ResourceProperty("ID", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int)));
            ResourceProperty name = new ResourceProperty("Name", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string)));
            idProperty.CanReflectOnInstanceTypeProperty = false;
            name.CanReflectOnInstanceTypeProperty = false;

            ResourceType entityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "namespace", "entityType", false);

            // Adding named stream to readonly type
            entityType.AddProperty(idProperty);
            entityType.SetReadOnly();
            ResourceProperty stream1 = new ResourceProperty("Stream1", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => entityType.AddProperty(stream1),
                string.Format("The resource type '{0}' cannot be modified since it is already set to read-only.", entityType.FullName),
                "Adding to a sealed type should fail.");

            // Adding a named stream which will collide with properties declared on this type
            ResourceType type1 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "namespace", "type1", true);
            type1.AddProperty(idProperty);
            ResourceProperty idStream = new ResourceProperty("ID", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => type1.AddProperty(idStream),
                "A property with same name 'ID' already exists in type 'namespace.type1'. Please make sure that there is no property with the same name defined in one of the base types.",
                "Named stream can't have the same name as a property.");

            // Adding a named stream which will collide with properties declared on base type
            type1 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "namespace", "type1", true);
            type1.AddProperty(idProperty);
            ResourceType type2 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, type1, "namespace", "type2", false);
            // We don't detect name collision between named streams and properties until we enumerate properties after the type is set to readonly.
            idStream = new ResourceProperty("ID", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
            type2.AddProperty(idStream);
            type2.Properties.Count();
            type2.SetReadOnly();
            // Detect name collision now.
            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => type2.Properties.Count(),
                "A property with same name 'ID' already exists in type 'namespace.type2'. Please make sure that there is no property with the same name defined in one of the base types.",
                "Named stream can't have the same name as a property on base type.");

            // Adding a named stream which will collide with named streams declared on this type
            type1 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "namespace", "type1", true);
            type1.AddProperty(idProperty);
            ResourceProperty nameStream = new ResourceProperty("Name", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
            type1.AddProperty(nameStream);
            // Adding the same named stream instance
            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => type1.AddProperty(nameStream),
                "A property with same name 'Name' already exists in type 'namespace.type1'. Please make sure that there is no property with the same name defined in one of the base types.",
                "Named stream can't have the same name as another named stream.");
            // Adding a new instance with the same name
            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => type1.AddProperty(new ResourceProperty("Name", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)))),
                "A property with same name 'Name' already exists in type 'namespace.type1'. Please make sure that there is no property with the same name defined in one of the base types.",
                "Named stream can't have the same name as another named stream - new instance.");

            // Adding a named stream which will collide with named streams declared on base type
            type1 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "namespace", "type1", true);
            type1.AddProperty(idProperty);
            type1.AddProperty(nameStream);
            type2 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, type1, "namespace", "type2", false);
            type2.AddProperty(nameStream);  // Adding the same instance
            type2.SetReadOnly();
            // Delay detection until ValidateType(), this is because you can add named streams to the base type that collides with those on the derived type.
            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => type2.Properties.Count(),
                "A property with same name 'Name' already exists in type 'namespace.type2'. Please make sure that there is no property with the same name defined in one of the base types.",
                "Named stream can't have the same name as another named stream - new instance.");
            type2 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, type1, "namespace", "type2", false);
            type2.AddProperty(new ResourceProperty(nameStream.Name, ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));  // Adding a new instance with the same name
            type2.SetReadOnly();
            // Delay detection until ValidaType(), this is because you can add named streams to the base type that collides with those on the derived type.
            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => type2.Properties.Count(),
                "A property with same name 'Name' already exists in type 'namespace.type2'. Please make sure that there is no property with the same name defined in one of the base types.",
                "Named stream can't have the same name as another named stream - new instance.");

            // Adding a property which will collide with named streams declared on this type
            type1 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "namespace", "type1", true);
            type1.AddProperty(idProperty);
            nameStream = new ResourceProperty("Name", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
            type1.AddProperty(nameStream);
            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => type1.AddProperty(name),
                "A property with same name 'Name' already exists in type 'namespace.type1'. Please make sure that there is no property with the same name defined in one of the base types.",
                "Can't add property with the same name as a named stream.");

            // Adding a property which will collide with named streams declared on base type
            type1 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "namespace", "type1", true);
            type1.AddProperty(idProperty);
            type1.AddProperty(nameStream);
            // We don't detect name collision between named streams and properties until we enumerate properties after the type is set to readonly.
            type2 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, type1, "namespace", "type2", false);
            type2.AddProperty(name);
            type2.Properties.Count();
            type2.SetReadOnly();
            // Detect name collision now.
            ExceptionUtils.ExpectedException<InvalidOperationException>(
                () => type2.Properties.Count(),
                "A property with same name 'Name' already exists in type 'namespace.type2'. Please make sure that there is no property with the same name defined in one of the base types.",
                "Can't add property with the same name as a named stream on base type.");
        }

        #endregion Named Stream Metadata Tests

        /// <summary>
        /// Returns all resource types which can be type of a key property.
        /// </summary>
        /// <returns>The list of resource types which can be keys.</returns>
        private static IEnumerable<ResourceType> GetKeyResourceTypes()
        {
            return ResourceTypeUtils.GetPrimitiveResourceTypes().Where(rt => rt.InstanceType != typeof(System.IO.Stream) && !(rt.InstanceType.IsGenericType && rt.InstanceType.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        /// <summary>
        /// Validates order of properties on a resource type.
        /// </summary>
        /// <param name="resourceType">The resource type to check.</param>
        /// <param name="properties">The list of properties in the expected order.</param>
        /// <param name="checkPropertiesDeclaredInThisType">If only properties declared on the specified type should be checked.</param>
        private void CheckOrdering(ResourceType resourceType, List<ResourceProperty> properties, bool checkPropertiesDeclaredInThisType)
        {
            int keyIndex = 0;
            int etagIndex = 0;
            for (int i = 0; i < properties.Count; i++)
            {
                ResourceProperty property = properties[i];
                ResourceProperty p1 = checkPropertiesDeclaredInThisType ? resourceType.PropertiesDeclaredOnThisType[i] : resourceType.Properties[i];
                Assert.IsTrue(p1 == property, "properties ordered must be preserved");
                if ((property.Kind & ResourcePropertyKind.Key) == ResourcePropertyKind.Key)
                {
                    Assert.IsTrue(resourceType.KeyProperties[keyIndex] == property, "order of key properties must also be preserved");
                    ++keyIndex;
                }

                if ((property.Kind & ResourcePropertyKind.ETag) == ResourcePropertyKind.ETag)
                {
                    Assert.IsTrue(resourceType.ETagProperties[etagIndex] == property, "order of key properties must also be preserved");
                    ++etagIndex;
                }
            }
        }

        /// <summary>
        /// Creates types for property lazy loading tests
        /// </summary>
        /// <param name="customerType">The customer base type.</param>
        /// <param name="derivedCustomerType">The customer derived type.</param>
        /// <param name="orderType">The order type.</param>
        /// <param name="addressType">The address type.</param>
        private void InitializeTypesForLazyLoading(out ResourceType customerType, out ResourceType derivedCustomerType, out ResourceType orderType, out ResourceType addressType)
        {
            customerType = new LazyLoadPropertiesResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "Customer", false);
            derivedCustomerType = new LazyLoadPropertiesResourceType(typeof(object), ResourceTypeKind.EntityType, customerType, "Foo", "DerivedCustomer", false);
            orderType = new LazyLoadPropertiesResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "Order", false);
            addressType = new LazyLoadPropertiesResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "Address", false);

            LazyLoadPropertiesResourceType.TypeToPropertyMap = new Dictionary<string, ResourceProperty[]>(StringComparer.Ordinal);
            LazyLoadPropertiesResourceType.TypeToPropertyMap.Add("Foo.Customer", new ResourceProperty[] {
                    CreateNonClrProperty("ID", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))),
                    CreateNonClrProperty("Name", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))),
                    CreateNonClrProperty("Orders", ResourcePropertyKind.ResourceSetReference, orderType),
                    CreateNonClrProperty("Address", ResourcePropertyKind.ResourceReference, addressType) });

            LazyLoadPropertiesResourceType.TypeToPropertyMap.Add("Foo.DerivedCustomer", new ResourceProperty[] {
                    CreateNonClrProperty("DerivedOrders", ResourcePropertyKind.ResourceSetReference, orderType),
                    CreateNonClrProperty("MyAddress", ResourcePropertyKind.ResourceReference, addressType) });

            LazyLoadPropertiesResourceType.TypeToPropertyMap.Add("Foo.Order", new ResourceProperty[] {
                    CreateNonClrProperty("ID", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))),
                    CreateNonClrProperty("DollarAmount", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(decimal))),
                    CreateNonClrProperty("Customer", ResourcePropertyKind.ResourceReference, customerType) });

            LazyLoadPropertiesResourceType.TypeToPropertyMap.Add("Foo.Address", new ResourceProperty[] {
                    CreateNonClrProperty("Street Number", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))),
                    CreateNonClrProperty("Street Name", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))),
                    CreateNonClrProperty("Street Name", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))) });
        }

        /// <summary>
        /// Creates a resource property not backed by a CLR property.
        /// </summary>
        /// <param name="name">The name of the property to create.</param>
        /// <param name="kind">The kind of the property to create.</param>
        /// <param name="propertyType">The type of the property to create.</param>
        /// <returns>The newly created property.</returns>
        private static ResourceProperty CreateNonClrProperty(string name, ResourcePropertyKind kind, ResourceType propertyType)
        {
            ResourceProperty property = new ResourceProperty(name, kind, propertyType);
            property.CanReflectOnInstanceTypeProperty = false;
            return property;
        }

        public class LazyLoadPropertiesResourceType : ResourceType
        {
            public LazyLoadPropertiesResourceType(Type instanceType, ResourceTypeKind resourceTypeKind, ResourceType baseType, string namespaceName, string name, bool isAbstract)
                : base(instanceType, resourceTypeKind, baseType, namespaceName, name, isAbstract)
            {
            }

            internal static Dictionary<string, ResourceProperty[]> TypeToPropertyMap = null;
            internal static List<string> LoadPropertiesDeclaredOnThisTypeMethodCalled = null;

            protected override IEnumerable<ResourceProperty> LoadPropertiesDeclaredOnThisType()
            {
                LoadPropertiesDeclaredOnThisTypeMethodCalled.Add(this.FullName);
                ResourceProperty[] properties = null;
                if (TypeToPropertyMap != null && TypeToPropertyMap.TryGetValue(this.FullName, out properties))
                {
                    return properties;
                }

                return base.LoadPropertiesDeclaredOnThisType();
            }

            internal static IDisposable CreateChangeScope()
            {
                LazyLoadPropertiesResourceType.LoadPropertiesDeclaredOnThisTypeMethodCalled = new List<string>();
                return new MyResourceTypeScope();
            }

            private class MyResourceTypeScope : IDisposable
            {
                public void Dispose()
                {
                    LazyLoadPropertiesResourceType.LoadPropertiesDeclaredOnThisTypeMethodCalled = null;
                }
            }
        }
    }
}
