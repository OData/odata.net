//---------------------------------------------------------------------
// <copyright file="EdmTypeDefinitionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class EdmTypeDefinitionTests
    {
        [Fact]
        public void TestModelWithTypeDefinition()
        {
            var model = new EdmModel();

            var addressType = new EdmTypeDefinition("MyNS", "Address", EdmPrimitiveTypeKind.String);
            model.AddElement(addressType);

            var weightType = new EdmTypeDefinition("MyNS", "Weight", EdmPrimitiveTypeKind.Decimal);
            model.AddElement(weightType);

            var personType = new EdmEntityType("MyNS", "Person");

            var addressTypeReference = new EdmTypeDefinitionReference(addressType, false);
            personType.AddStructuralProperty("Address", addressTypeReference);
            addressTypeReference.Definition.Should().Be(addressType);
            addressTypeReference.IsNullable.Should().BeFalse();

            var weightTypeReference = new EdmTypeDefinitionReference(weightType, true);
            personType.AddStructuralProperty("Weight", weightTypeReference);
            weightTypeReference.Definition.Should().Be(weightType);
            weightTypeReference.IsNullable.Should().BeTrue();

            var personId = personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            personType.AddKeys(personId);
        }

        [Fact]
        public void TestEdmTypeDefinitionConstructorWithPrimitiveTypeKind()
        {
            var intAlias = new EdmTypeDefinition("MyNS", "TestInt", EdmPrimitiveTypeKind.Int32);
            intAlias.Namespace.Should().Be("MyNS");
            intAlias.Name.Should().Be("TestInt");
            intAlias.TypeKind.Should().Be(EdmTypeKind.TypeDefinition);
            intAlias.SchemaElementKind.Should().Be(EdmSchemaElementKind.TypeDefinition);
            intAlias.UnderlyingType.PrimitiveKind.Should().Be(EdmPrimitiveTypeKind.Int32);

            var stringAlias = new EdmTypeDefinition("MyNamespace", "TestString", EdmPrimitiveTypeKind.String);
            stringAlias.Namespace.Should().Be("MyNamespace");
            stringAlias.Name.Should().Be("TestString");
            stringAlias.TypeKind.Should().Be(EdmTypeKind.TypeDefinition);
            stringAlias.SchemaElementKind.Should().Be(EdmSchemaElementKind.TypeDefinition);
            stringAlias.UnderlyingType.PrimitiveKind.Should().Be(EdmPrimitiveTypeKind.String);

            var decimalAlias = new EdmTypeDefinition("TestNS", "TestDecimal", EdmPrimitiveTypeKind.Decimal);
            decimalAlias.Namespace.Should().Be("TestNS");
            decimalAlias.Name.Should().Be("TestDecimal");
            decimalAlias.TypeKind.Should().Be(EdmTypeKind.TypeDefinition);
            decimalAlias.SchemaElementKind.Should().Be(EdmSchemaElementKind.TypeDefinition);
            decimalAlias.UnderlyingType.PrimitiveKind.Should().Be(EdmPrimitiveTypeKind.Decimal);

            var booleanAlias = new EdmTypeDefinition("TestNamespace", "TestBoolean", EdmPrimitiveTypeKind.Boolean);
            booleanAlias.Namespace.Should().Be("TestNamespace");
            booleanAlias.Name.Should().Be("TestBoolean");
            booleanAlias.TypeKind.Should().Be(EdmTypeKind.TypeDefinition);
            booleanAlias.SchemaElementKind.Should().Be(EdmSchemaElementKind.TypeDefinition);
            booleanAlias.UnderlyingType.PrimitiveKind.Should().Be(EdmPrimitiveTypeKind.Boolean);
        }

        [Fact]
        public void TestEdmTypeDefinitionConstructorWithPrimitiveType()
        {
            var intAlias = new EdmTypeDefinition("MyNS", "TestInt", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32));
            intAlias.Namespace.Should().Be("MyNS");
            intAlias.Name.Should().Be("TestInt");
            intAlias.TypeKind.Should().Be(EdmTypeKind.TypeDefinition);
            intAlias.SchemaElementKind.Should().Be(EdmSchemaElementKind.TypeDefinition);
            intAlias.UnderlyingType.Should().Be(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32));

            var stringAlias = new EdmTypeDefinition("MyNamespace", "TestString", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String));
            stringAlias.Namespace.Should().Be("MyNamespace");
            stringAlias.Name.Should().Be("TestString");
            stringAlias.TypeKind.Should().Be(EdmTypeKind.TypeDefinition);
            stringAlias.SchemaElementKind.Should().Be(EdmSchemaElementKind.TypeDefinition);
            stringAlias.UnderlyingType.Should().Be(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String));

            var decimalAlias = new EdmTypeDefinition("TestNS", "TestDecimal", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal));
            decimalAlias.Namespace.Should().Be("TestNS");
            decimalAlias.Name.Should().Be("TestDecimal");
            decimalAlias.TypeKind.Should().Be(EdmTypeKind.TypeDefinition);
            decimalAlias.SchemaElementKind.Should().Be(EdmSchemaElementKind.TypeDefinition);
            decimalAlias.UnderlyingType.Should().Be(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal));

            var booleanAlias = new EdmTypeDefinition("TestNamespace", "TestBoolean", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean));
            booleanAlias.Namespace.Should().Be("TestNamespace");
            booleanAlias.Name.Should().Be("TestBoolean");
            booleanAlias.TypeKind.Should().Be(EdmTypeKind.TypeDefinition);
            booleanAlias.SchemaElementKind.Should().Be(EdmSchemaElementKind.TypeDefinition);
            booleanAlias.UnderlyingType.Should().Be(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean));
        }
    }
}
