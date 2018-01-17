//---------------------------------------------------------------------
// <copyright file="ComplexTypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.UnitTests
{
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ComplexTypeTests
    {
        [TestMethod]
        public void TestOpenType()
        {
            var type1 = new EdmComplexType("NS", "Type1", null, false);
            Assert.IsFalse(type1.IsOpen);

            type1 = new EdmComplexType("NS", "Type1", null, false, false);
            Assert.IsFalse(type1.IsOpen);

            type1 = new EdmComplexType("NS", "Type1", null, false, true);
            Assert.IsTrue(type1.IsOpen);
        }
    }
}
