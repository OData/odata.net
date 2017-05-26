//---------------------------------------------------------------------
// <copyright file="ExtensionMethodsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.UnitTests
{
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExtensionMethodsTests
    {
        [TestMethod]
        public void TestEntityType()
        {
            var container = new EdmEntityContainer("NS", "C");
            var entityType = new EdmEntityType("NS", "People");
            var entitySet = new EdmEntitySet(container, "Peoples", entityType);
            Assert.AreEqual(entityType, entitySet.EntityType());
            var singleton = new EdmSingleton(container, "Boss", entityType);
            Assert.AreEqual(entityType, singleton.EntityType());
        }
    }
}
