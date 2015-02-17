//---------------------------------------------------------------------
// <copyright file="ResourceTypeUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion

    /// <summary>
    /// Helper methods for testing Data Services metadata types and their OM
    /// </summary>
    public class ResourceTypeUtils
    {
        /// <summary>
        /// Cached list of all primitive resource types.
        /// </summary>
        private static IEnumerable<ResourceType> primitiveResourceTypes;

        /// <summary>Returns list of all primitive resource types.</summary>
        /// <returns>All primitive resource types.</returns>
        public static IEnumerable<ResourceType> GetPrimitiveResourceTypes()
        {
            if (primitiveResourceTypes == null)
            {
                primitiveResourceTypes = UnitTestsUtil.GetPrimitiveResourceTypes();
            }

            return primitiveResourceTypes;
        }

        /// <summary>
        /// Returns a test instance of one of the provider OM types.
        /// </summary>
        /// <param name="type">The type to get instance of.</param>
        /// <returns>The </returns>
        public static object GetTestInstance(Type type)
        {
            if (type == typeof(ResourceType))
            {
                ResourceProperty p = new ResourceProperty("ID", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(int))) { CanReflectOnInstanceTypeProperty = false };
                ResourceType resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "NoBaseType", false);
                Assert.IsTrue(resourceType.KeyProperties.Count == 0, "It's okay not to have key properties, since we haven't set this type to readonly yet");
                resourceType.AddProperty(p);
                return resourceType;
            }
            else if (type == typeof(ResourceProperty))
            {
                return new ResourceProperty("NonKeyProperty", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int)));
            }
            else if (type == typeof(ServiceOperation))
            {
                ServiceOperationParameter parameter = new ServiceOperationParameter("o", ResourceType.GetPrimitiveResourceType(typeof(string)));
                ServiceOperation operation = new ServiceOperation("Dummy", ServiceOperationResultKind.Void, null, null, "POST", new ServiceOperationParameter[] { parameter });
                return operation;
            }
            else if (type == typeof(ServiceOperationParameter))
            {
                return new ServiceOperationParameter("o", ResourceType.GetPrimitiveResourceType(typeof(string)));
            }
            else if (type == typeof(ResourceSet))
            {
                ResourceProperty p = new ResourceProperty("ID", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(int)));

                ResourceType resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Foo", "NoBaseType", false);
                resourceType.AddProperty(p);
                return new ResourceSet("Customers", resourceType);
            }

            throw new Exception(String.Format("Unexpected type encountered: '{0}'", type.FullName));
        }
    }
}
