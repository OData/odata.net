//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsTypeDefinitionDefinitionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
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
            ((IEdmTypeDefinition)typeDefinitionDefinition).UnderlyingType.PrimitiveKind.Should().Be(EdmPrimitiveTypeKind.String);
            ((IEdmSchemaElement)typeDefinitionDefinition).SchemaElementKind.Should().Be(EdmSchemaElementKind.TypeDefinition);
            ((IEdmNamedElement)typeDefinitionDefinition).Name.Should().Be("Length");
            typeDefinitionDefinition.TypeKind.Should().Be(EdmTypeKind.TypeDefinition);
            typeDefinitionDefinition.Element.Should().Be(typeDefinition);
        }
    }
}
