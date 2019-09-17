//---------------------------------------------------------------------
// <copyright file="CsdlTypeDefinitionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Parsing.Ast
{
    public class CsdlTypeDefinitionTests
    {
        [Fact]
        public void TestCsdlTypeDefinitionConstructor()
        {
            var loc = new CsdlLocation(17, 4);
            var type = new CsdlTypeDefinition("Length", "Edm.Int32", loc);

            Assert.Equal(loc, type.Location);
            Assert.Equal("Length", type.Name);
            Assert.Equal("Edm.Int32", type.UnderlyingTypeName);
        }
    }
}
