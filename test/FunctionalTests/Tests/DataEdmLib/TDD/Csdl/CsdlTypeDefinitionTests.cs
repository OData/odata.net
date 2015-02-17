//---------------------------------------------------------------------
// <copyright file="CsdlTypeDefinitionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Edm.TDD.Tests
{
    using FluentAssertions;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Csdl.Parsing.Ast;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CsdlTypeDefinitionTests
    {
        [TestMethod]
        public void TestCsdlTypeDefinitionConstructor()
        {
            var loc = new CsdlLocation(17, 4);
            var type = new CsdlTypeDefinition("Length", "Edm.Int32", loc);

            type.Location.Should().Be(loc);
            type.Name.Should().Be("Length");
            type.UnderlyingTypeName.Should().Be("Edm.Int32");
        }
    }
}
