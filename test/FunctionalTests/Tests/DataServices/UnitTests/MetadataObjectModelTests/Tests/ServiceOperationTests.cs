//---------------------------------------------------------------------
// <copyright file="ServiceOperationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using System;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AstoriaTestNS = System.Data.Test.Astoria;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ServiceOperation class
    /// </summary>
    [TestClass, TestCase]
    public class ServiceOperationTests
    {
        [TestMethod, Variation("Verifies that service operation doesn't allow certain operations.")]
        public void ServiceOperationConstructorInvalidTests()
        {
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "The 'resultType' parameter must be null when the 'resultKind' parameter value is 'Void', however the 'resultType' parameter cannot be null when the 'resultKind' parameter is of any value other than 'Void'. Please make sure that the 'resultKind' parameter value is set according to the 'resultType' parameter value.",
                "op", ServiceOperationResultKind.Void, ResourceType.GetPrimitiveResourceType(typeof(int)), null, "GET", null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "The 'resultType' parameter must be null when the 'resultKind' parameter value is 'Void', however the 'resultType' parameter cannot be null when the 'resultKind' parameter is of any value other than 'Void'. Please make sure that the 'resultKind' parameter value is set according to the 'resultType' parameter value.",
                "op", ServiceOperationResultKind.DirectValue, null, null, "GET", null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "A parameter with the name 'p1' already exists. Please make sure that every parameter has a unique name.",
                "op", ServiceOperationResultKind.Void, null, null, "GET",
                new ServiceOperationParameter[] { 
                        new ServiceOperationParameter("p1", ResourceType.GetPrimitiveResourceType(typeof(int))),
                        new ServiceOperationParameter("p1", ResourceType.GetPrimitiveResourceType(typeof(string))) });
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "The resource type 'Collection(foo.Customer)' is not a type that can be returned by a service operation. A service operation can only return values of an entity type, a complex type or any primitive type, other than the stream type.",
                "op", ServiceOperationResultKind.QueryWithMultipleResults, ResourceType.GetEntityCollectionResourceType(new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "foo", "Customer", false)), null, "GET", null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "The resource type 'Edm.Stream' is not a type that can be returned by a service operation. A service operation can only return values of an entity type, a complex type or any primitive type, other than the stream type.",
                "op", ServiceOperationResultKind.DirectValue, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)), null, "GET", null);

            ResourceProperty p = new ResourceProperty("ID", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(int)));
            ResourceType customerType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "NoBaseType", false);
            customerType.AddProperty(p);
            customerType.SetReadOnly();
            ResourceSet customerSet = new ResourceSet("Customers", customerType);
            ResourceType orderType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "NoBaseType", false);
            orderType.AddProperty(p);
            orderType.SetReadOnly();

            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "'resultSet' must be null when 'resultType' is null or not an EntityType.",
                "op", ServiceOperationResultKind.Void, null, customerSet, "GET", null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "'resultSet' must be null when 'resultType' is null or not an EntityType.",
                "op", ServiceOperationResultKind.DirectValue, ResourceType.GetPrimitiveResourceType(typeof(int)), customerSet, "GET", null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "When 'resultType' is an entity type, 'resultSet' cannot be null and the resource type of 'resultSet' must be assignable from 'resultType'.",
                "op", ServiceOperationResultKind.QueryWithMultipleResults, customerType, null, "GET", null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "When 'resultType' is an entity type, 'resultSet' cannot be null and the resource type of 'resultSet' must be assignable from 'resultType'.",
                "op", ServiceOperationResultKind.QueryWithMultipleResults, orderType, customerSet, "GET", null);

            ExceptionUtils.ThrowsException<InvalidOperationException>(
                () =>
                {
                    ServiceOperation op = new ServiceOperation("op", ServiceOperationResultKind.Void, null, null, "GET", null);
                    op.MimeType = null;
                },
                "MimeType cannot be set to null");

            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "Value cannot be null or empty.\r\nParameter name: method",
                "op", ServiceOperationResultKind.DirectValue, ResourceType.GetPrimitiveResourceType(typeof(int)), null, null, null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "Value cannot be null or empty.\r\nParameter name: method",
                "op", ServiceOperationResultKind.DirectValue, ResourceType.GetPrimitiveResourceType(typeof(int)), null, string.Empty, null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "Value cannot be null or empty.\r\nParameter name: name",
                null, ServiceOperationResultKind.DirectValue, ResourceType.GetPrimitiveResourceType(typeof(int)), null, "GET", null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "Value cannot be null or empty.\r\nParameter name: name",
                string.Empty, ServiceOperationResultKind.DirectValue, ResourceType.GetPrimitiveResourceType(typeof(int)), null, "GET", null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "An invalid HTTP method 'PUT' was specified for the service operation 'op'. Only the HTTP 'POST' and 'GET' methods are supported for service operations.",
                "op", ServiceOperationResultKind.Void, null, null, "PUT", null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "An invalid HTTP method 'PATCH' was specified for the service operation 'op'. Only the HTTP 'POST' and 'GET' methods are supported for service operations.",
                "op", ServiceOperationResultKind.Void, null, null, "PATCH", null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "An invalid HTTP method 'DELETE' was specified for the service operation 'op'. Only the HTTP 'POST' and 'GET' methods are supported for service operations.",
                "op", ServiceOperationResultKind.Void, null, null, "DELETE", null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "An invalid HTTP method 'HEAD' was specified for the service operation 'op'. Only the HTTP 'POST' and 'GET' methods are supported for service operations.",
                "op", ServiceOperationResultKind.Void, null, null, "HEAD", null);
            ExceptionUtils.CheckInvalidConstructorParameters(
                typeof(ServiceOperation),
                "An invalid HTTP method 'Some Method' was specified for the service operation 'op'. Only the HTTP 'POST' and 'GET' methods are supported for service operations.",
                "op", ServiceOperationResultKind.Void, null, null, "Some Method", null);
        }

        [TestMethod, Variation("Verifies that service operation allows the right types for parameters and return type and fails on others.")]
        public void ServiceOperationTypes()
        {
            ResourceType complexType = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "foo", "Address", false);
            ResourceType entityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "foo", "Order", false);
            ResourceSet entitySet = new ResourceSet("Set", entityType);
            ResourceType collectionOfPrimitive = ResourceType.GetCollectionResourceType(ResourceType.GetPrimitiveResourceType(typeof(int)));
            ResourceType collectionOfComplex = ResourceType.GetCollectionResourceType(complexType);

            var returnTypeDirectCases = ResourceTypeUtils.GetPrimitiveResourceTypes().Select(rt => new { Type = rt, Set = (ResourceSet)null, Invalid = rt.InstanceType == typeof(System.IO.Stream) ? true : false })
                .Concat(new[] {
                        new { Type = complexType, Set = (ResourceSet)null, Invalid = false },
                        new { Type = entityType, Set = entitySet, Invalid = false }
                    })
                .Concat(new ResourceType[] { collectionOfPrimitive, collectionOfComplex }.Select(rt => new { Type = rt, Set = (ResourceSet)null, Invalid = true }));
            AstoriaTestNS.TestUtil.RunCombinations(returnTypeDirectCases, (returnTypeDirectCase) =>
            {
                Exception e = AstoriaTestNS.TestUtil.RunCatching(() =>
                    new ServiceOperation("so", ServiceOperationResultKind.DirectValue, returnTypeDirectCase.Type, returnTypeDirectCase.Set, "GET", null));
                if (returnTypeDirectCase.Invalid)
                {
                    Assert.IsNotNull(e, "Service op should have failed to be constructed.");
                    Assert.IsTrue(e is ArgumentException, "Invalid exception type.");
                }
                else
                {
                    Assert.IsNull(e, "Service op should have succeeded.");
                }
            });
        }
    }
}
