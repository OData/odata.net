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

        [Theory]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("a. ", false)]
        [InlineData(".com", false)]
        [InlineData("com.", false)]
        [InlineData(".", false)]
        [InlineData("com", false)]
        [InlineData("a.b.com", true)]
        [InlineData(" a.b.com", true)]
        [InlineData("a.b.com ", true)]
        [InlineData(" a . b . c ", true)]
        [InlineData("a . . c", false)]
        [InlineData("a .. c", false)]
        public void IsQualifiedName_Test(string name, bool expected)
        {
            var actual = EdmUtil.IsQualifiedName(name);
            Assert.Equal(expected, actual);
        }
    }
}
