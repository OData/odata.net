//---------------------------------------------------------------------
// <copyright file="EdmNavigationPropertyInfoTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Edm.TDD.Tests
{
    using System;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EdmNavigationPropertyInfoTests
    {
        [TestMethod]
        public void TestCloneEdmnavigationPropertyInfo()
        {
            var type = new EdmEntityType("NS", "name");
            var property1 = new EdmStructuralProperty(type, "property1", EdmCoreModel.Instance.GetInt32(false));
            var property2 = new EdmStructuralProperty(type, "property2", EdmCoreModel.Instance.GetInt32(false));
            EdmNavigationPropertyInfo navigationPropertyInfo = new EdmNavigationPropertyInfo
            {
                ContainsTarget = true,
                DependentProperties = new[] {property1}, 
                Name = "navPropInfo", 
                OnDelete = EdmOnDeleteAction.Cascade, 
                PrincipalProperties = new[] {property2}, 
                Target = type, 
                TargetMultiplicity = EdmMultiplicity.Many
            };
            var cloneNavigationPropertyInfo = navigationPropertyInfo.Clone();

            Assert.AreEqual(navigationPropertyInfo.ContainsTarget, cloneNavigationPropertyInfo.ContainsTarget);
            Assert.AreEqual(navigationPropertyInfo.Name, cloneNavigationPropertyInfo.Name);
            Assert.AreEqual(navigationPropertyInfo.DependentProperties, cloneNavigationPropertyInfo.DependentProperties);
            Assert.AreEqual(navigationPropertyInfo.OnDelete, cloneNavigationPropertyInfo.OnDelete);
            Assert.AreEqual(navigationPropertyInfo.Target, cloneNavigationPropertyInfo.Target);
            Assert.AreEqual(navigationPropertyInfo.PrincipalProperties, cloneNavigationPropertyInfo.PrincipalProperties);
            Assert.AreEqual(navigationPropertyInfo.TargetMultiplicity, cloneNavigationPropertyInfo.TargetMultiplicity);
        }

    }
}
