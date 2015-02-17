//---------------------------------------------------------------------
// <copyright file="OperationParameterTests.cs" company="Microsoft">
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
    /// Tests for the ServiceOperationParameter class
    /// </summary>
    [TestClass, TestCase]
    public class OperationParameterTests
    {
        [TestMethod, Variation("Verifies that service operation parameter doesn't allow certain types.")]
        public void ServiceOperationParameterInvalidCasesTest()
        {
            ResourceType complexType = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "foo", "Address", false);
            ResourceType entityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "foo", "Order", false);
            ResourceSet entitySet = new ResourceSet("Set", entityType);
            ResourceType collectionOfPrimitive = ResourceType.GetCollectionResourceType(ResourceType.GetPrimitiveResourceType(typeof(int)));
            ResourceType collectionOfComplex = ResourceType.GetCollectionResourceType(complexType);
            ResourceType collectionType = ResourceType.GetEntityCollectionResourceType(entityType);
            ResourceType streamType = ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream));

            var parameterCases = ResourceTypeUtils.GetPrimitiveResourceTypes().Except(new[] {streamType}).Select(rt => new { Type = rt, Invalid = false })
                .Concat(new ResourceType[] {
                        streamType,
                        complexType,
                        entityType,
                        collectionOfPrimitive,
                        collectionOfComplex,
                        collectionType }.Select(rt => new { Type = rt, Invalid = true }));

            AstoriaTestNS.TestUtil.RunCombinations(parameterCases, (parameterCase) =>
            {
                Exception e = AstoriaTestNS.TestUtil.RunCatching(() => new ServiceOperationParameter("p", parameterCase.Type));
                if (parameterCase.Invalid)
                {
                    ExceptionUtils.IsExpectedException<ArgumentException>(e, String.Format("The service operation parameter 'p' of type '{0}' is not supported.\r\nParameter name: parameterType", parameterCase.Type.FullName));
                }
                else
                {
                    Assert.IsNull(e, "Service op parameter should have succeeded.");
                }
            });
        }

        [TestMethod, Variation("Verifies that service action parameter doesn't allow certain types.")]
        public void ServiceActionParameterInvalidCasesTest()
        {
            ResourceType complexType = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "foo", "Address", false);
            ResourceType entityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "foo", "Order", false);
            ResourceSet entitySet = new ResourceSet("Set", entityType);
            ResourceType collectionOfPrimitive = ResourceType.GetCollectionResourceType(ResourceType.GetPrimitiveResourceType(typeof(int)));
            ResourceType collectionOfComplex = ResourceType.GetCollectionResourceType(complexType);
            ResourceType collectionType = ResourceType.GetEntityCollectionResourceType(entityType);

            var parameterTypes = ResourceTypeUtils.GetPrimitiveResourceTypes()
                .Concat(new ResourceType[] {
                        complexType,
                        entityType,
                        collectionOfPrimitive,
                        collectionOfComplex,
                        collectionType });
            AstoriaTestNS.TestUtil.RunCombinations(parameterTypes, (parameterType) =>
            {
                Exception e = AstoriaTestNS.TestUtil.RunCatching(() => new ServiceActionParameter("p", parameterType));
                if (parameterType != ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)))
                {
                    Assert.IsNull(e, "Service op parameter should have succeeded.");
                }
                else
                {
                    ExceptionUtils.IsExpectedException<ArgumentException>(e, "The service operation parameter 'p' of type 'Edm.Stream' is not supported.\r\nParameter name: parameterType");
                }
            });
        }
    }
}
