//---------------------------------------------------------------------
// <copyright file="EdmTypeDefinitionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class EdmTypeDefinitionTests
    {
        [Fact]
        public void EdmModelAddComplexTypeTest()
        {
            var model = new EdmModel();
            var complexTypeA = model.AddComplexType("NS", "ComplexA");
            var complexTypeB = model.AddComplexType("NS", "ComplexB", complexTypeA);
            var complexTypeC = model.AddComplexType("NS", "ComplexC", complexTypeB, false);
            var complexTypeD = model.AddComplexType("NS", "ComplexD", complexTypeC, false, true);
            Assert.Equal(complexTypeA, model.FindDeclaredType("NS.ComplexA"));
            Assert.Equal(complexTypeB, model.FindDeclaredType("NS.ComplexB"));
            Assert.Equal(complexTypeC, model.FindDeclaredType("NS.ComplexC"));
            Assert.Equal(complexTypeD, model.FindDeclaredType("NS.ComplexD"));
        }

        [Fact]
        public void EdmModelAddEntityTypeTest()
        {
            var model = new EdmModel();
            var entityTypeA = model.AddEntityType("NS", "EntityA");
            var entityTypeB = model.AddEntityType("NS", "EntityB", entityTypeA);
            var entityTypeC = model.AddEntityType("NS", "EntityC", entityTypeB, false, true);
            var entityTypeD = model.AddEntityType("NS", "EntityD", entityTypeC, false, true, true);
            Assert.Equal(entityTypeA, model.FindDeclaredType("NS.EntityA"));
            Assert.Equal(entityTypeB, model.FindDeclaredType("NS.EntityB"));
            Assert.Equal(entityTypeC, model.FindDeclaredType("NS.EntityC"));
            Assert.Equal(entityTypeD, model.FindDeclaredType("NS.EntityD"));
        }

        [Fact]
        public void EdmModelAddTermTest()
        {
            var model = new EdmModel();
            var term1 = model.AddTerm("NS", "Term1", EdmPrimitiveTypeKind.String);

            var boolType = EdmCoreModel.Instance.GetBoolean(false);
            var term2 = model.AddTerm("NS", "Term2", boolType);

            var term3 = model.AddTerm("NS", "Term3", boolType, "entityset", "default value");

            Assert.Equal(term1, model.FindDeclaredTerm("NS.Term1"));
            Assert.Equal(term2, model.FindDeclaredTerm("NS.Term2"));
            Assert.Equal(term3, model.FindDeclaredTerm("NS.Term3"));
        }

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
            Assert.Same(addressType, addressTypeReference.Definition);
            Assert.False(addressTypeReference.IsNullable);

            var weightTypeReference = new EdmTypeDefinitionReference(weightType, true);
            personType.AddStructuralProperty("Weight", weightTypeReference);
            Assert.Same(weightType, weightTypeReference.Definition);
            Assert.True(weightTypeReference.IsNullable);

            var personId = personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            personType.AddKeys(personId);
        }

        [Fact]
        public void TestEdmTypeDefinitionConstructorWithPrimitiveTypeKind()
        {
            var intAlias = new EdmTypeDefinition("MyNS", "TestInt", EdmPrimitiveTypeKind.Int32);
            Assert.Equal("MyNS", intAlias.Namespace);
            Assert.Equal("TestInt", intAlias.Name);
            Assert.Equal(EdmTypeKind.TypeDefinition, intAlias.TypeKind);
            Assert.Equal(EdmSchemaElementKind.TypeDefinition, intAlias.SchemaElementKind);
            Assert.Equal(EdmPrimitiveTypeKind.Int32, intAlias.UnderlyingType.PrimitiveKind);

            var stringAlias = new EdmTypeDefinition("MyNamespace", "TestString", EdmPrimitiveTypeKind.String);
            Assert.Equal("MyNamespace", stringAlias.Namespace);
            Assert.Equal("TestString", stringAlias.Name);
            Assert.Equal(EdmTypeKind.TypeDefinition, stringAlias.TypeKind);
            Assert.Equal(EdmSchemaElementKind.TypeDefinition, stringAlias.SchemaElementKind);
            Assert.Equal(EdmPrimitiveTypeKind.String, stringAlias.UnderlyingType.PrimitiveKind);

            var decimalAlias = new EdmTypeDefinition("TestNS", "TestDecimal", EdmPrimitiveTypeKind.Decimal);
            Assert.Equal("TestNS", decimalAlias.Namespace);
            Assert.Equal("TestDecimal", decimalAlias.Name);
            Assert.Equal(EdmTypeKind.TypeDefinition, decimalAlias.TypeKind);
            Assert.Equal(EdmSchemaElementKind.TypeDefinition, decimalAlias.SchemaElementKind);
            Assert.Equal(EdmPrimitiveTypeKind.Decimal, decimalAlias.UnderlyingType.PrimitiveKind);

            var booleanAlias = new EdmTypeDefinition("TestNamespace", "TestBoolean", EdmPrimitiveTypeKind.Boolean);
            Assert.Equal("TestNamespace", booleanAlias.Namespace);
            Assert.Equal("TestBoolean", booleanAlias.Name);
            Assert.Equal(EdmTypeKind.TypeDefinition, booleanAlias.TypeKind);
            Assert.Equal(EdmSchemaElementKind.TypeDefinition, booleanAlias.SchemaElementKind);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, booleanAlias.UnderlyingType.PrimitiveKind);
        }

        [Fact]
        public void TestEdmTypeDefinitionConstructorWithPrimitiveType()
        {
            var intAlias = new EdmTypeDefinition("MyNS", "TestInt", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32));
            Assert.Equal("MyNS", intAlias.Namespace);
            Assert.Equal("TestInt", intAlias.Name);
            Assert.Equal(EdmTypeKind.TypeDefinition, intAlias.TypeKind);
            Assert.Equal(EdmSchemaElementKind.TypeDefinition, intAlias.SchemaElementKind);
            Assert.Equal(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), intAlias.UnderlyingType);

            var stringAlias = new EdmTypeDefinition("MyNamespace", "TestString", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String));
            Assert.Equal("MyNamespace", stringAlias.Namespace);
            Assert.Equal("TestString", stringAlias.Name);
            Assert.Equal(EdmTypeKind.TypeDefinition, stringAlias.TypeKind);
            Assert.Equal(EdmSchemaElementKind.TypeDefinition, stringAlias.SchemaElementKind);
            Assert.Same(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), stringAlias.UnderlyingType);

            var decimalAlias = new EdmTypeDefinition("TestNS", "TestDecimal", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal));
            Assert.Equal("TestNS", decimalAlias.Namespace);
            Assert.Equal("TestDecimal", decimalAlias.Name);
            Assert.Equal(EdmTypeKind.TypeDefinition, decimalAlias.TypeKind);
            Assert.Equal(EdmSchemaElementKind.TypeDefinition, decimalAlias.SchemaElementKind);
            Assert.Same(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), decimalAlias.UnderlyingType);

            var booleanAlias = new EdmTypeDefinition("TestNamespace", "TestBoolean", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean));
            Assert.Equal("TestNamespace", booleanAlias.Namespace);
            Assert.Equal("TestBoolean", booleanAlias.Name);
            Assert.Equal(EdmTypeKind.TypeDefinition, booleanAlias.TypeKind);
            Assert.Equal(EdmSchemaElementKind.TypeDefinition, booleanAlias.SchemaElementKind);
            Assert.Same(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean), booleanAlias.UnderlyingType);
        }
    }
}
