//---------------------------------------------------------------------
// <copyright file="CodeGenSampleMockTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Design.T4.UnitTests.CodeGen
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using NamespacePrefixWithSingleNamespace;

    [TestClass]
    public class CodeGenSampleMockTests
    {
        [TestMethod]
        public void ReturnSuccessMessageIfMethodIsMockable()
        {
            string message = "Add";
            Mock<MyContainer> mock = new Mock<MyContainer>(null);
            mock.CallBase = true;
            mock.Setup(x => x.AddToItems(It.IsAny<TestType>())).Callback(() => { message = "Saved Successfully"; });
            AddItemsClass addItemsClass = new AddItemsClass();
            string result = addItemsClass.SaveItems(mock.Object);
            mock.Verify();
            Assert.AreEqual(result, message);

        }
    }

    public class AddItemsClass
    {
        public string SaveItems(MyContainer mc)
        {
            TestType testType = new TestType()
            {
                KeyProp = 1,
                ValueProp = "TestType1"
            };
            mc.AddToItems(testType);
            return "Saved Successfully";
        }
    }
}
