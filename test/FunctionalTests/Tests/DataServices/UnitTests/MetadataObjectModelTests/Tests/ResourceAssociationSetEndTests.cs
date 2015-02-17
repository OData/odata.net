//---------------------------------------------------------------------
// <copyright file="ResourceAssociationSetEndTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using Microsoft.OData.Service.Providers;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ResourceAssociationSetEnd class.
    /// </summary>
    [TestClass, TestCase]
    public class ResourceAssociationSetEndTests
    {
        [TestMethod, Variation("Verifies that resource association set end doesn't allow certain operations.")]
        public void InvalidCasesTest()
        {
            ResourceProperty id = new ResourceProperty("ID", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(int))) { CanReflectOnInstanceTypeProperty = false };
            ResourceType customerType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "CustomerType", false);
            customerType.AddProperty(id);
            ResourceType orderType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "OrderType", false);
            orderType.AddProperty(id);

            ResourceProperty customerOrders = new ResourceProperty("Orders", ResourcePropertyKind.ResourceSetReference, orderType);
            ResourceProperty orderCustomer = new ResourceProperty("Customer", ResourcePropertyKind.ResourceReference, customerType);
            customerType.AddProperty(customerOrders);
            orderType.AddProperty(orderCustomer);
            customerOrders.CanReflectOnInstanceTypeProperty = false;
            orderCustomer.CanReflectOnInstanceTypeProperty = false;
            customerType.SetReadOnly();
            orderType.SetReadOnly();

            ResourceSet customerSet = new ResourceSet("Customers", customerType);
            ResourceSet orderSet = new ResourceSet("Orders", orderType);

            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceAssociationSetEnd),
                "Value cannot be null.\r\nParameter name: resourceSet",
                null, customerType, customerOrders);

            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceAssociationSetEnd),
                "Value cannot be null.\r\nParameter name: resourceType",
                customerSet, null, customerOrders);

            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceAssociationSetEnd),
                "The resourceProperty parameter must be a navigation property on the resource type specified by the resourceType parameter.",
                customerSet, customerType, id);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceAssociationSetEnd),
                "The resourceProperty parameter must be a navigation property on the resource type specified by the resourceType parameter.",
                customerSet, customerType, orderCustomer);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceAssociationSetEnd),
                "The resourceType parameter must be a type that is assignable to the resource set specified by the resourceSet parameter.",
                customerSet,
                orderType,
                orderCustomer);
        }
    }
}
