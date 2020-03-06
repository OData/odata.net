//---------------------------------------------------------------------
// <copyright file="EdmUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.OData.Edm.Tests
{
    /// <summary>
    ///Tests EdmUtils functionalities
    ///</summary>
    [TestClass]
    public class EdmUtilTests
    {
        [TestMethod]
        public void FunctionImportShouldProduceCorrectFullyQualifiedNameAndNotHaveParameters()
        {
            var function = new EdmFunction("d.s", "testFunction", EdmCoreModel.Instance.GetString(true));
            function.AddParameter("param1", EdmCoreModel.Instance.GetString(false));
            var functionImport = new EdmFunctionImport(new EdmEntityContainer("d.s", "container"), "testFunction", function);
            //Assert.Equals("d.s.container/testFunction", EdmUtil.(functionImport));
        }

        [TestMethod]
        public void EntitySetShouldProduceCorrectFullyQualifiedName()
        {
            var entitySet = new EdmEntitySet(new EdmEntityContainer("d.s", "container"), "entitySet", new EdmEntityType("foo", "type"));
            //Assert.Equal("d.s.container/entitySet", EdmUtil.FullyQualifiedName(entitySet));
        }
    }
}
