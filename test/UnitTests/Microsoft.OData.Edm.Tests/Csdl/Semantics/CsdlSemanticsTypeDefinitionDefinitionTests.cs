//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsTypeDefinitionDefinitionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Semantics
{
    public class CsdlSemanticsTypeDefinitionDefinitionTests
    {
        [Fact]
        public void CsdlSemanticsTypeDefinitionDefinitionUnitTest()
        {
            CsdlTypeDefinition typeDefinition = new CsdlTypeDefinition("Length", "Edm.String", new CsdlLocation(5, 6));
            CsdlSemanticsTypeDefinitionDefinition typeDefinitionDefinition = new CsdlSemanticsTypeDefinitionDefinition(null, typeDefinition);
            Assert.Equal(EdmPrimitiveTypeKind.String, ((IEdmTypeDefinition)typeDefinitionDefinition).UnderlyingType.PrimitiveKind);
            Assert.Equal(EdmSchemaElementKind.TypeDefinition, ((IEdmSchemaElement)typeDefinitionDefinition).SchemaElementKind);
            Assert.Equal("Length", ((IEdmNamedElement)typeDefinitionDefinition).Name);
            Assert.Equal(EdmTypeKind.TypeDefinition, typeDefinitionDefinition.TypeKind);
            Assert.Equal(typeDefinition, typeDefinitionDefinition.Element);
        }
    }
}
