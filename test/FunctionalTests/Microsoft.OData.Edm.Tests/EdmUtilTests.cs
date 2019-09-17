//---------------------------------------------------------------------
// <copyright file="EdmUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Edm.Tests
{
    /// <summary>
    ///Tests EdmUtils functionalities
    ///</summary>
    public class EdmUtilTests
    {
        [Fact]
        public void FunctionImportShouldProduceCorrectFullyQualifiedNameAndNotHaveParameters()
        {
            var function = new EdmFunction("d.s", "testFunction", EdmCoreModel.Instance.GetString(true));
            function.AddParameter("param1", EdmCoreModel.Instance.GetString(false));
            var functionImport = new EdmFunctionImport(new EdmEntityContainer("d.s", "container"), "testFunction", function);
            Assert.Equal("d.s.container/testFunction", EdmUtil.FullyQualifiedName(functionImport));
        }

        [Fact]
        public void EntitySetShouldProduceCorrectFullyQualifiedName()
        {
            var entitySet = new EdmEntitySet(new EdmEntityContainer("d.s", "container"), "entitySet", new EdmEntityType("foo", "type"));
            Assert.Equal("d.s.container/entitySet", EdmUtil.FullyQualifiedName(entitySet));
        }
    }
}
