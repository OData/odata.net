//---------------------------------------------------------------------
// <copyright file="EdmNavigationPropertyInfoTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class EdmNavigationPropertyInfoTests
    {
        [Fact]
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

            Assert.Equal(navigationPropertyInfo.ContainsTarget, cloneNavigationPropertyInfo.ContainsTarget);
            Assert.Equal(navigationPropertyInfo.Name, cloneNavigationPropertyInfo.Name);
            Assert.Equal(navigationPropertyInfo.DependentProperties, cloneNavigationPropertyInfo.DependentProperties);
            Assert.Equal(navigationPropertyInfo.OnDelete, cloneNavigationPropertyInfo.OnDelete);
            Assert.Equal(navigationPropertyInfo.Target, cloneNavigationPropertyInfo.Target);
            Assert.Equal(navigationPropertyInfo.PrincipalProperties, cloneNavigationPropertyInfo.PrincipalProperties);
            Assert.Equal(navigationPropertyInfo.TargetMultiplicity, cloneNavigationPropertyInfo.TargetMultiplicity);
        }

    }
}
