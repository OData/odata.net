//---------------------------------------------------------------------
// <copyright file="EdmUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Edm.TDD.Tests
{
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            EdmUtil.FullyQualifiedName(functionImport).Should().Be("d.s.container/testFunction");
        }

        [TestMethod]
        public void EntitySetShouldProduceCorrectFullyQualifiedName()
        {
            var entitySet = new EdmEntitySet(new EdmEntityContainer("d.s", "container"), "entitySet", new EdmEntityType("foo", "type"));
            EdmUtil.FullyQualifiedName(entitySet).Should().Be("d.s.container/entitySet");
        }
    }
}
