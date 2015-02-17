//---------------------------------------------------------------------
// <copyright file="ResourceAssociationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Server
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.IO;
    using Microsoft.Spatial;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ResourceAssociationTests
    {
        [TestMethod, Variation("In resource association set end, resource type must be the declaring type")]
        public void ResourceTypeMustBeDeclaringType_1()
        {
            ResourceType customerType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Test", "Customer", false /*isAbstract*/);
            ResourceType employeeType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, customerType, "Test", "Employee", false /*isAbstract*/);
            ResourceSet customerSet = new ResourceSet("CustomerSet", customerType);

            ResourceType orderType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Test", "Employee", false /*isAbstract*/);
            ResourceProperty orderProperty = new ResourceProperty("Orders", ResourcePropertyKind.ResourceSetReference, orderType);
            customerType.AddProperty(orderProperty);

            try
            {
                new ResourceAssociationSetEnd(customerSet, employeeType, orderProperty);
                Assert.Fail("Creating resource association set end should never have succeeded");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(
                    DataServicesResourceUtil.GetString("ResourceAssociationSetEnd_ResourceTypeMustBeTheDeclaringType", employeeType.FullName, orderProperty.Name),
                    e.Message);
            }
        }

        [TestMethod, Variation("In resource association set end, resource type must be the declaring type")]
        public void ResourceTypeMustBeDeclaringType_2()
        {
            ResourceType baseCustomerType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Test", "Employee", true /*isAbstract*/);
            ResourceType customerType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, baseCustomerType, "Test", "Customer", false /*isAbstract*/);
            ResourceSet customerSet = new ResourceSet("CustomerSet", customerType);

            ResourceType orderType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Test", "Employee", false /*isAbstract*/);
            ResourceProperty orderProperty = new ResourceProperty("Orders", ResourcePropertyKind.ResourceSetReference, orderType);
            baseCustomerType.AddProperty(orderProperty);

            try
            {
                new ResourceAssociationSetEnd(customerSet, customerType, orderProperty);
                Assert.Fail("Creating resource association set end should never have succeeded");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(
                    DataServicesResourceUtil.GetString("ResourceAssociationSetEnd_ResourceTypeMustBeTheDeclaringType", customerType.FullName, orderProperty.Name),
                    e.Message);
            }
        }
    }
}
