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

        [Ignore] // Ignorning because these tests take too long to run in snap and then fail, need to fix, best would be to unit test each case rather than this crazyiness
        [TestMethod, Variation("Action constructor test cases.")]
        public void ServiceActionConstructorTests()
        {
            ResourceProperty id = new ResourceProperty("ID", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(int)));
            ResourceType customerType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "Customer", false);
            customerType.AddProperty(id);
            customerType.SetReadOnly();
            ResourceSet customerSet = new ResourceSet("Customers", customerType);
            ResourceType orderType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "Order", false);
            orderType.AddProperty(id);
            orderType.SetReadOnly();

            ResourceType complexType = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "foo", "Address", false);
            ResourceType collectionOfPrimitive = ResourceType.GetCollectionResourceType(ResourceType.GetPrimitiveResourceType(typeof(int)));
            ResourceType collectionOfComplex = ResourceType.GetCollectionResourceType(complexType);
            ResourceType collectionOfCustomer = ResourceType.GetEntityCollectionResourceType(customerType);
            ResourceType collectionOfOrder = ResourceType.GetEntityCollectionResourceType(orderType);

            var types = ResourceTypeUtils.GetPrimitiveResourceTypes().Concat(new ResourceType[] { null, customerType, orderType, complexType, collectionOfPrimitive, collectionOfComplex, collectionOfCustomer, collectionOfOrder });
            var resultSetsOrPathExpressions = new object[] { null, customerSet, new ResourceSetPathExpression("p1/Foo") };
            var parameters = types.Except(new[] { ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)) }).Select(t => t != null ? new ServiceActionParameter[] { new ServiceActionParameter("p1", t) } : new ServiceActionParameter[0]);
            parameters = parameters.Concat(types.Except(new[] { ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)), null }).ToList().Combinations(2).Select(tt => new ServiceActionParameter[] { new ServiceActionParameter("p1", tt[0]), new ServiceActionParameter("p2", tt[1]) }));
            var operationParameterBindings = new OperationParameterBindingKind[] { OperationParameterBindingKind.Never, OperationParameterBindingKind.Sometimes, OperationParameterBindingKind.Always };

            AstoriaTestNS.TestUtil.RunCombinations(
                types, resultSetsOrPathExpressions, parameters, operationParameterBindings,
                (returnType, resultSetOrPathExpression, paramList, operationParameterBindingKind) =>
            {
                ServiceAction action = null;
                ResourceSet resourceSet = resultSetOrPathExpression as ResourceSet;
                ResourceSetPathExpression pathExpression = resultSetOrPathExpression as ResourceSetPathExpression;
                bool bindable = (operationParameterBindingKind == OperationParameterBindingKind.Always || operationParameterBindingKind == OperationParameterBindingKind.Sometimes);
                Exception e = null;

                try
                {
                    if (pathExpression != null)
                    {
                        bindable = true;
                        action = new ServiceAction("foo", returnType, OperationParameterBindingKind.Sometimes, paramList, pathExpression);
                    }
                    else
                    {
                        action = new ServiceAction("foo", returnType, resourceSet, operationParameterBindingKind, paramList);
                    }
                }
                catch (Exception ex)
                {
                    e = ex;
                }
                if (resourceSet != null && operationParameterBindingKind != OperationParameterBindingKind.Never)
                {
                    ExceptionUtils.IsExpectedException<ArgumentException>(e, "When 'returnType' is an entity type or an entity collection type, 'resultSetPathExpression' and 'resultSet' cannot be both null and the resource type of the result set must be assignable from 'returnType'.");
                }
                else if (returnType != null && (returnType.ResourceTypeKind == ResourceTypeKind.EntityType || returnType.ResourceTypeKind == ResourceTypeKind.EntityCollection) &&
                    (resourceSet == null && pathExpression == null ||
                    resourceSet != null && returnType.ResourceTypeKind == ResourceTypeKind.EntityType && resourceSet.ResourceType != returnType ||
                    resourceSet != null && returnType.ResourceTypeKind == ResourceTypeKind.EntityCollection && resourceSet.ResourceType != ((EntityCollectionResourceType)returnType).ItemType))
                {
                    ExceptionUtils.IsExpectedException<ArgumentException>(e, "When 'returnType' is an entity type or an entity collection type, 'resultSetPathExpression' and 'resultSet' cannot be both null and the resource type of the result set must be assignable from 'returnType'.");
                }
                else if ((returnType == null || returnType.ResourceTypeKind != ResourceTypeKind.EntityCollection && returnType.ResourceTypeKind != ResourceTypeKind.EntityType) && resourceSet != null)
                {
                    ExceptionUtils.IsExpectedException<ArgumentException>(e, "'resultSet' must be null when 'returnType' is null, not an entity type or not an entity collection type.");
                }
                else if ((returnType == null || returnType.ResourceTypeKind != ResourceTypeKind.EntityCollection && returnType.ResourceTypeKind != ResourceTypeKind.EntityType) && pathExpression != null)
                {
                    ExceptionUtils.IsExpectedException<ArgumentException>(e, "'resultSetPathExpression' must be null when 'returnType' is null, not an entity type or not an entity collection type.");
                }
                else if (returnType == ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)))
                {
                    ExceptionUtils.IsExpectedException<ArgumentException>(e, "The resource type 'Edm.Stream' is not a type that can be returned by a function or action. A function or action can only return values of an entity type, an entity collection type, a complex type, a collection type or any primitive type, other than the stream type.\r\nParameter name: returnType");
                }
                else if (paramList.Length > 0 && paramList.Skip(bindable ? 1 : 0).Any(p => p.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityType || p.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityCollection))
                {
                    var param = paramList.Skip(bindable ? 1 : 0).First(p => p.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityType || p.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityCollection);
                    var parameterTypeKind = param.ParameterType.ResourceTypeKind;
                    ExceptionUtils.IsExpectedException<ArgumentException>(e, string.Format("The '{0}' parameter is of resource type kind '{1}' and it is not the binding parameter. Parameter of type kind '{1}' is only supported for the binding parameter.", param.Name, parameterTypeKind));
                }
                else if (pathExpression != null && !bindable)
                {
                    ExceptionUtils.IsExpectedException<ArgumentException>(e, "The binding parameter type must be an entity type or an entity collection type when 'resultSetPathExpression' is not null.");
                }
                else if (bindable && paramList.Length == 0)
                {
                    ExceptionUtils.IsExpectedException<ArgumentException>(e, "Bindable actions or functions must have at least one parameter, where the first parameter is the binding parameter.\r\nParameter name: operationParameterBindingKind");
                }
                else if (pathExpression != null && bindable && paramList.First().ParameterType.ResourceTypeKind != ResourceTypeKind.EntityType && paramList.First().ParameterType.ResourceTypeKind != ResourceTypeKind.EntityCollection)
                {
                    ExceptionUtils.IsExpectedException<ArgumentException>(e, "The binding parameter type must be an entity type or an entity collection type when 'resultSetPathExpression' is not null.");
                }
                else if (paramList.Length > 0 && bindable && paramList[0].ParameterType.ResourceTypeKind != ResourceTypeKind.EntityType && paramList[0].ParameterType.ResourceTypeKind != ResourceTypeKind.EntityCollection)
                {
                    ExceptionUtils.IsExpectedException<ArgumentException>(e, "An action's binding parameter must be of type Entity or EntityCollection.\r\nParameter name: parameters");
                }
                else
                {
                    Assert.IsNull(e, "Received exception but expected none. Exception message: {0}", e == null ? string.Empty : e.Message);
                    Assert.IsNotNull(action, "Action should be constructed.");

                    Assert.AreEqual("foo", action.Name, "unexpected name");
                    Assert.AreEqual(returnType, action.ReturnType, "unexpected return type");
                    Assert.AreEqual(resourceSet, action.ResourceSet, "unexpected result set");
                    Assert.AreEqual(pathExpression, action.ResultSetPathExpression, "unexpected path expression");
                    Assert.IsTrue(!bindable || action.BindingParameter == action.Parameters.First(), "unexpected binding parameter");
                    Assert.IsTrue(action.Method == "POST", "HttpMethod must be POST for ServiceActions.");
                    Assert.IsTrue(action.ResourceSet == null || action.ResultSetPathExpression == null, "'resultSet' and 'resultSetPathExpression' cannot be both set by the constructor.");
                }
            });
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
