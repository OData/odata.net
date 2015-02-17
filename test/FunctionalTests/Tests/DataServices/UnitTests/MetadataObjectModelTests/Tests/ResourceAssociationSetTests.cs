//---------------------------------------------------------------------
// <copyright file="ResourceAssociationSetTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using Microsoft.OData.Service.Providers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AstoriaTestNS = System.Data.Test.Astoria;
    using Microsoft.Test.ModuleCore;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ResourceAssociationSet class.
    /// </summary>
    [TestClass, TestCase]
    public class ResourceAssociationSetTests
    {
        [TestMethod, Variation( "Verifies that resource association set doesn't allow certain operations.")]
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

            ResourceAssociationSetEnd end1 = new ResourceAssociationSetEnd(customerSet, customerType, null);
            ResourceAssociationSetEnd end2 = new ResourceAssociationSetEnd(orderSet, orderType, null);

            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceAssociationSet), 
                "Value cannot be null or empty.\r\nParameter name: name", 
                null, end1, end2);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceAssociationSet), 
                "Value cannot be null or empty.\r\nParameter name: name", 
                string.Empty, end1, end2);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceAssociationSet), 
                "Value cannot be null.\r\nParameter name: end1", 
                "Customer_Order", null, end2);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceAssociationSet), 
                "Value cannot be null.\r\nParameter name: end2", 
                "Customer_Order", end1, null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ResourceAssociationSet),
                "The ResourceProperty of the ResourceAssociationEnds cannot both be null.", 
                "Customer_Order", end1, end2);

        }
    }
}
