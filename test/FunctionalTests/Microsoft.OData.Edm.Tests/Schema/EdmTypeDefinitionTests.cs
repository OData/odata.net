//---------------------------------------------------------------------
// <copyright file="EdmTypeDefinitionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if NETCOREAPP2_1 || NETCOREAPP3_1
using System.Text.Encodings.Web;
using System.Text.Json;
#endif
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
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

        [Fact]
        public void TestEdmTypeDefinitionFacetsDeserialization()
        {
            // Arrange
            var csdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS.TypeDefinitions"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <TypeDefinition Name=""Money"" UnderlyingType=""Edm.Decimal"" Precision=""16"" Scale=""2"" />
      <TypeDefinition Name=""Password"" UnderlyingType=""Edm.String"" MaxLength=""128"" Unicode=""false"" />
      <TypeDefinition Name=""LocationMarker"" UnderlyingType=""Edm.GeographyPoint"" SRID=""3246"" />
      <TypeDefinition Name=""PlaceMarker"" UnderlyingType=""Edm.GeometryPoint"" SRID=""2463"" />
    </Schema>
    </edmx:DataServices>
</edmx:Edmx>";

            IEdmModel model;

            // Act
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(csdl)))
            using (var reader = XmlReader.Create(memoryStream))
            {
                if (!CsdlReader.TryParse(reader, out model, out IEnumerable<EdmError> errors))
                {
                    Assert.True(false, string.Join("\r\n", errors.Select(d => d.ToString())));
                }
            }

            // Assert
            var moneyTypeDefinition = model.FindType("NS.TypeDefinitions.Money");
            Assert.NotNull(moneyTypeDefinition);
            var moneyFacetedTypeDefinition = Assert.IsAssignableFrom<IEdmFacetedTypeDefinition>(moneyTypeDefinition);
            Assert.Equal(moneyFacetedTypeDefinition.UnderlyingType, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal));
            Assert.Equal(16, moneyFacetedTypeDefinition.Precision);
            Assert.Equal(2, moneyFacetedTypeDefinition.Scale);

            var passwordTypeDefinition = model.FindType("NS.TypeDefinitions.Password");
            Assert.NotNull(passwordTypeDefinition);
            var passwordFacetedTypeDefinition = Assert.IsAssignableFrom<IEdmFacetedTypeDefinition>(passwordTypeDefinition);
            Assert.Equal(passwordFacetedTypeDefinition.UnderlyingType, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String));
            Assert.Equal(128, passwordFacetedTypeDefinition.MaxLength);
            Assert.Equal(false, passwordFacetedTypeDefinition.IsUnicode);

            var locationMarkerTypeDefinition = model.FindType("NS.TypeDefinitions.LocationMarker");
            Assert.NotNull(locationMarkerTypeDefinition);
            var locationMarkerFacetedTypeDefinition = Assert.IsAssignableFrom<IEdmFacetedTypeDefinition>(locationMarkerTypeDefinition);
            Assert.Equal(locationMarkerFacetedTypeDefinition.UnderlyingType, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint));
            Assert.Equal(3246, locationMarkerFacetedTypeDefinition.Srid);

            var placeMarkerTypeDefinition = model.FindType("NS.TypeDefinitions.PlaceMarker");
            Assert.NotNull(placeMarkerTypeDefinition);
            var placeMarkerFacetedTypeDefinition = Assert.IsAssignableFrom<IEdmFacetedTypeDefinition>(placeMarkerTypeDefinition);
            Assert.Equal(placeMarkerFacetedTypeDefinition.UnderlyingType, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint));
            Assert.Equal(2463, placeMarkerFacetedTypeDefinition.Srid);
        }

        [Fact]
        public void TestEdmTypeDefinitionFacetsSerialization()
        {
            // Arrange
            var model = new EdmModel();

            var moneyTypeDefinition = new EdmTypeDefinition(
                namespaceName: "NS.TypeDefinitions",
                name: "Money",
                underlyingType: EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal),
                maxLength: null,
                isUnicode: null,
                precision: 16,
                scale: 2,
                srid: null);
            model.AddElement(moneyTypeDefinition);

            var passwordTypeDefinition = new EdmTypeDefinition(
                namespaceName: "NS.TypeDefinitions",
                name: "Password",
                underlyingType: EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String),
                maxLength: 128,
                isUnicode: false,
                precision: null,
                scale: null,
                srid: null);
            model.AddElement(passwordTypeDefinition);

            var locationMarkerTypeDefinition = new EdmTypeDefinition(
                namespaceName: "NS.TypeDefinitions",
                name: "LocationMarker",
                underlyingType: EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint),
                maxLength: null,
                isUnicode: null,
                precision: null,
                scale: null,
                srid: 3246);
            model.AddElement(locationMarkerTypeDefinition);

            var placeMarkerTypeDefinition = new EdmTypeDefinition(
                namespaceName: "NS.TypeDefinitions",
                name: "PlaceMarker",
                underlyingType: EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint),
                maxLength: null,
                isUnicode: null,
                precision: null,
                scale: null,
                srid: 2463);
            model.AddElement(placeMarkerTypeDefinition);

            string csdl;
            // Act
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream))
                {
                    if (!CsdlWriter.TryWriteCsdl(model, writer, CsdlTarget.OData, out IEnumerable<EdmError> errors))
                    {
                        Assert.True(false, string.Join("\r\n", errors.Select(d => d.ToString())));
                    }
                }

                memoryStream.Position = 0;
                csdl = new StreamReader(memoryStream).ReadToEnd();
            }

            // Assert
            var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
<edmx:DataServices><Schema Namespace=""NS.TypeDefinitions"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
<TypeDefinition Name=""Money"" UnderlyingType=""Edm.Decimal"" Precision=""16"" Scale=""2"" />
<TypeDefinition Name=""Password"" UnderlyingType=""Edm.String"" MaxLength=""128"" Unicode=""false"" />
<TypeDefinition Name=""LocationMarker"" UnderlyingType=""Edm.GeographyPoint"" SRID=""3246"" />
<TypeDefinition Name=""PlaceMarker"" UnderlyingType=""Edm.GeometryPoint"" SRID=""2463"" />
</Schema>
</edmx:DataServices>
</edmx:Edmx>";
            Assert.NotNull(csdl);
            Assert.Equal(expected.Replace("\r\n", ""), csdl);
        }

#if NETCOREAPP2_1 || NETCOREAPP3_1
        [Fact]
        public void TestEdmTypeDefinitionFacetsJsonSerialization()
        {
            // Arrange
            var model = new EdmModel();

            var moneyTypeDefinition = new EdmTypeDefinition(
                namespaceName: "NS.TypeDefinitions",
                name: "Money",
                underlyingType: EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal),
                maxLength: null,
                isUnicode: null,
                precision: 16,
                scale: 2,
                srid: null);
            model.AddElement(moneyTypeDefinition);

            var passwordTypeDefinition = new EdmTypeDefinition(
                namespaceName: "NS.TypeDefinitions",
                name: "Password",
                underlyingType: EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String),
                maxLength: 128,
                isUnicode: true,
                precision: null,
                scale: null,
                srid: null);
            model.AddElement(passwordTypeDefinition);

            var locationMarkerTypeDefinition = new EdmTypeDefinition(
                namespaceName: "NS.TypeDefinitions",
                name: "LocationMarker",
                underlyingType: EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint),
                maxLength: null,
                isUnicode: null,
                precision: null,
                scale: null,
                srid: 3246);
            model.AddElement(locationMarkerTypeDefinition);

            var placeMarkerTypeDefinition = new EdmTypeDefinition(
                namespaceName: "NS.TypeDefinitions",
                name: "PlaceMarker",
                underlyingType: EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint),
                maxLength: null,
                isUnicode: null,
                precision: null,
                scale: null,
                srid: 2463);
            model.AddElement(placeMarkerTypeDefinition);

            string csdlJson;
            // Act
            using (var memoryStream = new MemoryStream())
            {
                JsonWriterOptions options = new JsonWriterOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    Indented = true,
                    SkipValidation = false
                };

                using (Utf8JsonWriter writer = new Utf8JsonWriter(memoryStream, options))
                {
                    if (!CsdlWriter.TryWriteCsdl(model, writer, out IEnumerable<EdmError> errors))
                    {
                        Assert.True(false, string.Join("\r\n", errors.Select(d => d.ToString())));
                    }
                }

                memoryStream.Position = 0;
                csdlJson = new StreamReader(memoryStream).ReadToEnd();
            }

            // Assert
            var expected = @"{
  ""$Version"": ""4.0"",
  ""NS.TypeDefinitions"": {
    ""Money"": {
      ""$Kind"": ""TypeDefinition"",
      ""$UnderlyingType"": ""Edm.Decimal"",
      ""$Precision"": 16,
      ""$Scale"": 2
    },
    ""Password"": {
      ""$Kind"": ""TypeDefinition"",
      ""$UnderlyingType"": ""Edm.String"",
      ""$MaxLength"": 128,
      ""$Unicode"": true
    },
    ""LocationMarker"": {
      ""$Kind"": ""TypeDefinition"",
      ""$UnderlyingType"": ""Edm.GeographyPoint"",
      ""$SRID"": 3246
    },
    ""PlaceMarker"": {
      ""$Kind"": ""TypeDefinition"",
      ""$UnderlyingType"": ""Edm.GeometryPoint"",
      ""$SRID"": 2463
    }
  }
}";
            Assert.NotNull(csdlJson);
            Assert.Equal(expected, csdlJson);
        }
#endif
    }
}
