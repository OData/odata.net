//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSerializationVisitorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
#if NETCOREAPP3_1
using System.Text.Json;
using System.Text.Encodings.Web;
#endif
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Serialization
{
    /// <summary>
    /// Unit tests of EdmModelCsdlSerializationVisitor. Aiming for whitebox coverage of these methods.
    /// </summary>
    public class EdmModelCsdlSerializationVisitorTests
    {
        private EdmModel model = new EdmModel();

        private static readonly EdmEntityContainer defaultContainer = new EdmEntityContainer("Default.NameSpace", "Container");
        private static readonly EdmAction defaultCheckoutAction = new EdmAction("Default.NameSpace2", "CheckOut", null);
        private static readonly EdmFunction defaultGetStuffFunction = new EdmFunction("Default.NameSpace2", "GetStuff", EdmCoreModel.Instance.GetString(true));

        public EdmModelCsdlSerializationVisitorTests()
        {
            this.model.SetEdmxVersion(new Version(4, 0));
        }

        #region Complex Type
        [Fact]
        public void VerifyComplexTypeWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "Dimensions");
            complexType.AddStructuralProperty("Height", EdmCoreModel.Instance.GetDecimal(6, 2, true));
            complexType.AddStructuralProperty("Weight", EdmCoreModel.Instance.GetDecimal(6, null, true));
            complexType.AddStructuralProperty("Length", EdmCoreModel.Instance.GetDecimal(null, null, false));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaType(complexType),
                @"<ComplexType Name=""Dimensions"">
  <Property Name=""Height"" Type=""Edm.Decimal"" Precision=""6"" Scale=""2"" />
  <Property Name=""Weight"" Type=""Edm.Decimal"" Precision=""6"" Scale=""Variable"" />
  <Property Name=""Length"" Type=""Edm.Decimal"" Nullable=""false"" />
</ComplexType>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaType(complexType), @"{
  ""Dimensions"": {
    ""$Kind"": ""ComplexType"",
    ""Height"": {
      ""$Type"": ""Edm.Decimal"",
      ""$Nullable"": true,
      ""$Precision"": 6,
      ""$Scale"": 2
    },
    ""Weight"": {
      ""$Type"": ""Edm.Decimal"",
      ""$Nullable"": true,
      ""$Precision"": 6
    },
    ""Length"": {
      ""$Type"": ""Edm.Decimal""
    }
  }
}");
        }

        [Fact]
        public void VerifyComplexTypeWithNavigationPropertiesWrittenCorrectly()
        {
            // Arrange
            EdmEntityType city = new EdmEntityType("NS", "City");
            EdmComplexType complexType = new EdmComplexType("NS", "Address");
            complexType.AddStructuralProperty("Street", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false, 42, false, true))));
            complexType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "City",
                Target = city,
                TargetMultiplicity = EdmMultiplicity.One,
                OnDelete = EdmOnDeleteAction.Cascade,
                ContainsTarget = true
            });

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaType(complexType),
                @"<ComplexType Name=""Address"">
  <Property Name=""Street"" Type=""Collection(Edm.String)"" MaxLength=""42"" Unicode=""false"" />
  <NavigationProperty Name=""City"" Type=""NS.City"" Nullable=""false"" ContainsTarget=""true"">
    <OnDelete Action=""Cascade"" />
  </NavigationProperty>
</ComplexType>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaType(complexType), @"{
  ""Address"": {
    ""$Kind"": ""ComplexType"",
    ""Street"": {
      ""$Collection"": true,
      ""$Nullable"": true,
      ""$MaxLength"": 42,
      ""$Unicode"": false
    },
    ""City"": {
      ""$Kind"": ""NavigationProperty"",
      ""$Type"": ""NS.City"",
      ""$ContainsTarget"": true,
      ""$OnDelete"": ""Cascade""
    }
  }
}");
        }

        [Fact]
        public void VerifyComplexTypeWithAnnotationWrittenCorrectly()
        {
            // Arrange
            EdmComplexType baseType = new EdmComplexType("NS", "Base");
            EdmComplexType complexType = new EdmComplexType("NS", "Sub", baseType, true, true);
            complexType.AddStructuralProperty("Data", EdmCoreModel.Instance.GetInt32(false));
            this.model.AddElement(complexType);
            this.model.AddElement(baseType);
            this.model.SetDescriptionAnnotation(complexType, "Sub's Description");

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaType(complexType),
                @"<ComplexType Name=""Sub"" BaseType=""NS.Base"" Abstract=""true"" OpenType=""true"">
  <Property Name=""Data"" Type=""Edm.Int32"" Nullable=""false"" />
  <Annotation Term=""Org.OData.Core.V1.Description"" String=""Sub's Description"" />
</ComplexType>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaType(complexType), @"{
  ""Sub"": {
    ""$Kind"": ""ComplexType"",
    ""$BaseType"": ""NS.Base"",
    ""$Abstract"": true,
    ""$OpenType"": true,
    ""Data"": {
      ""$Type"": ""Edm.Int32""
    },
    ""@Org.OData.Core.V1.Description"": ""Sub's Description""
  }
}");
        }
        #endregion

        #region Entity Type
        [Fact]
        public void VerifyEntityTypeWrittenCorrectly()
        {
            // Arrange
            EdmEntityType entityType = new EdmEntityType("NS", "Customer");
            entityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            entityType.AddStructuralProperty("GeoPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, 8, false));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaType(entityType),
                @"<EntityType Name=""Customer"">
  <Property Name=""Name"" Type=""Edm.String"" />
  <Property Name=""GeoPoint"" Type=""Edm.GeographyPoint"" Nullable=""false"" SRID=""8"" />
</EntityType>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaType(entityType), @"{
  ""Customer"": {
    ""$Kind"": ""EntityType"",
    ""Name"": {
      ""$Nullable"": true
    },
    ""GeoPoint"": {
      ""$Type"": ""Edm.GeographyPoint"",
      ""$SRID"": 8
    }
  }
}");
        }

        [Fact]
        public void VerifyEntityTypeWithNavigationPropertiesWrittenCorrectly()
        {
            // Arrange
            EdmComplexType address = new EdmComplexType("NS", "Address");
            address.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(false));
            EdmEntityType order = new EdmEntityType("NS", "Order");
            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddStructuralProperty("Locations", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(address, false))));
            customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Orders",
                Target = order,
                TargetMultiplicity = EdmMultiplicity.Many,
                OnDelete = EdmOnDeleteAction.Cascade,
            });

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaType(customer),
                @"<EntityType Name=""Customer"">
  <Property Name=""Locations"" Type=""Collection(NS.Address)"" Nullable=""false"" />
  <NavigationProperty Name=""Orders"" Type=""Collection(NS.Order)"">
    <OnDelete Action=""Cascade"" />
  </NavigationProperty>
</EntityType>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaType(customer), @"{
  ""Customer"": {
    ""$Kind"": ""EntityType"",
    ""Locations"": {
      ""$Collection"": true,
      ""$Type"": ""NS.Address""
    },
    ""Orders"": {
      ""$Kind"": ""NavigationProperty"",
      ""$Collection"": true,
      ""$Type"": ""NS.Order"",
      ""$OnDelete"": ""Cascade""
    }
  }
}");
        }

        [Fact]
        public void VerifyEntityTypeWithAnnotationWrittenCorrectly()
        {
            // Arrange
            EdmEntityType baseType = new EdmEntityType("NS", "Base");
            EdmEntityType subType = new EdmEntityType("NS", "Sub", baseType, true, true);
            subType.AddStructuralProperty("Data", EdmCoreModel.Instance.GetPrimitiveType(false));
            this.model.AddElement(subType);
            this.model.AddElement(baseType);
            this.model.SetLongDescriptionAnnotation(subType, "Sub's Long Description");
            this.model.SetNamespaceAlias("NS", "NsAlias");

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaType(subType),
                @"<EntityType Name=""Sub"" BaseType=""NsAlias.Base"" Abstract=""true"" OpenType=""true"">
  <Property Name=""Data"" Type=""Edm.PrimitiveType"" Nullable=""false"" />
  <Annotation Term=""Org.OData.Core.V1.LongDescription"" String=""Sub's Long Description"" />
</EntityType>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaType(subType), @"{
  ""Sub"": {
    ""$Kind"": ""EntityType"",
    ""$BaseType"": ""NsAlias.Base"",
    ""$Abstract"": true,
    ""$OpenType"": true,
    ""Data"": {
      ""$Type"": ""Edm.PrimitiveType""
    },
    ""@Org.OData.Core.V1.LongDescription"": ""Sub's Long Description""
  }
}");
        }
        #endregion

        #region Enum Type

        [Fact]
        public void VerifyEnumTypeWrittenCorrectly()
        {
            // Arrange
            EdmEnumType enumType = new EdmEnumType("NS", "Color");
            enumType.AddMember("Blue", new EdmEnumMemberValue(0));
            enumType.AddMember("White", new EdmEnumMemberValue(1));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaType(enumType),
                @"<EnumType Name=""Color"">
  <Member Name=""Blue"" Value=""0"" />
  <Member Name=""White"" Value=""1"" />
</EnumType>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaType(enumType), @"{
  ""Color"": {
    ""$Kind"": ""EnumType"",
    ""Blue"": 0,
    ""White"": 1
  }
}");
        }

        [Fact]
        public void VerifyEnumTypeAllWrittenCorrectly()
        {
            // Arrange
            EdmEnumType enumType = new EdmEnumType("NS", "Color", EdmPrimitiveTypeKind.Int16, true);
            enumType.AddMember("Blue", new EdmEnumMemberValue(0));
            enumType.AddMember("White", new EdmEnumMemberValue(1));
            this.model.AddElement(enumType);
            this.model.SetDescriptionAnnotation(enumType, "Color Description");

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaType(enumType),
                @"<EnumType Name=""Color"" UnderlyingType=""Edm.Int16"" IsFlags=""true"">
  <Member Name=""Blue"" Value=""0"" />
  <Member Name=""White"" Value=""1"" />
  <Annotation Term=""Org.OData.Core.V1.Description"" String=""Color Description"" />
</EnumType>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaType(enumType), @"{
  ""Color"": {
    ""$Kind"": ""EnumType"",
    ""$UnderlyingType"": ""Edm.Int16"",
    ""$IsFlags"": true,
    ""Blue"": 0,
    ""White"": 1,
    ""@Org.OData.Core.V1.Description"": ""Color Description""
  }
}");
        }

        [Fact]
        public void VerifyEnumTypeWithEnumMemberAnnotationWrittenCorrectly()
        {
            // Arrange
            EdmEnumType enumType = new EdmEnumType("NS", "Color", EdmPrimitiveTypeKind.Int16, true);
            enumType.AddMember("Blue", new EdmEnumMemberValue(0));
            var member = enumType.AddMember("White", new EdmEnumMemberValue(1));
            this.model.AddElement(enumType);
            this.model.SetDescriptionAnnotation(member, "With overnight");
            this.model.SetDescriptionAnnotation(enumType, "color description");

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaType(enumType),
                @"<EnumType Name=""Color"" UnderlyingType=""Edm.Int16"" IsFlags=""true"">
  <Member Name=""Blue"" Value=""0"" />
  <Member Name=""White"" Value=""1"">
    <Annotation Term=""Org.OData.Core.V1.Description"" String=""With overnight"" />
  </Member>
  <Annotation Term=""Org.OData.Core.V1.Description"" String=""color description"" />
</EnumType>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaType(enumType), @"{
  ""Color"": {
    ""$Kind"": ""EnumType"",
    ""$UnderlyingType"": ""Edm.Int16"",
    ""$IsFlags"": true,
    ""Blue"": 0,
    ""White"": 1,
    ""White@Org.OData.Core.V1.Description"": ""With overnight"",
    ""@Org.OData.Core.V1.Description"": ""color description""
  }
}");
        }
        #endregion

        #region Action

        [Fact]
        public void VerifyPrimitiveCollectionReturnTypeDefinedInChildReturnTypeElement()
        {
            // Arrange
            var action = new EdmAction("Default.Namespace", "Checkout", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(action),
                @"<Action Name=""Checkout""><ReturnType Type=""Collection(Edm.String)"" /></Action>", false);

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(action), @"{
  ""$Kind"": ""Action"",
  ""$ReturnType"": {
    ""$Collection"": true,
    ""$Nullable"": true
  }
}", true, false);
        }

        [Fact]
        public void VerifyNullableIsWrittenInChildReturnTypeElementForCollectionElementType()
        {
            // Arrange
            var action = new EdmAction("Default.Namespace", "Checkout", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(isUnbounded: false, maxLength: 10, isUnicode: false, isNullable: false)));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(action),
                @"<Action Name=""Checkout"">
  <ReturnType Type=""Collection(Edm.String)"" Nullable=""false"" MaxLength=""10"" Unicode=""false"" />
</Action>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(action), @"{
  ""$Kind"": ""Action"",
  ""$ReturnType"": {
    ""$Collection"": true,
    ""$MaxLength"": 10,
    ""$Unicode"": false
  }
}", true, false);
        }

        [Fact]
        public void VerifyPrimitiveReturnTypeDefinedInChildReturnTypeElement()
        {
            // Arrange
            var action = new EdmAction("Default.Namespace", "Checkout", EdmCoreModel.Instance.GetString(false));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(action),
                @"<Action Name=""Checkout""><ReturnType Type=""Edm.String"" Nullable=""false"" /></Action>", false);

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(action), @"{
  ""$Kind"": ""Action"",
  ""$ReturnType"": {}
}", true, false);
        }

        [Fact]
        public void VerifyEntityReturnTypeDefinedInChildReturnTypeElement()
        {
            // Arrange
            var entityType = new EdmEntityType("NS.ds", "EntityType");
            var action = new EdmAction("Default.Namespace", "Checkout", new EdmEntityTypeReference(entityType, false));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(action),
                @"<Action Name=""Checkout""><ReturnType Type=""NS.ds.EntityType"" Nullable=""false"" /></Action>", false);

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(action), @"{
  ""$Kind"": ""Action"",
  ""$ReturnType"": {
    ""$Type"": ""NS.ds.EntityType""
  }
}", true, false);
        }

        [Fact]
        public void VerifyNoReturnTypeElementIsWrittenWhenNoReturnTypeIsProvided()
        {
            // Arrange
            var action = new EdmAction("Default.Namespace", "Checkout", null);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(action),
                @"<Action Name=""Checkout"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(action), @"{
  ""$Kind"": ""Action""
}", true, false);
        }

        [Fact]
        public void VerifyOperationParameterWritten()
        {
            // Arrange
            var action = new EdmAction("Default.Namespace", "Checkout", null);
            action.AddParameter("firstParameter", EdmCoreModel.Instance.GetSingle(true));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(action),
                @"<Action Name=""Checkout""><Parameter Name=""firstParameter"" Type=""Edm.Single"" /></Action>", false);

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(action), @"{
  ""$Kind"": ""Action"",
  ""$Parameter"": [
    {
      ""$Name"": ""firstParameter"",
      ""$Type"": ""Edm.Single"",
      ""$Nullable"": true
    }
  ]
}", true, false);
        }

        [Fact]
        public void VerifyActionWrittenCorrectly()
        {
            // Arrange
            EdmAction action = new EdmAction("NS", "DoStuff", EdmCoreModel.Instance.GetString(true), true, null);
            action.AddParameter("param1", EdmCoreModel.Instance.GetString(true));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(action),
                @"<Action Name=""DoStuff"" IsBound=""true"">
  <Parameter Name=""param1"" Type=""Edm.String"" />
  <ReturnType Type=""Edm.String"" />
</Action>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(action), @"{
  ""$Kind"": ""Action"",
  ""$IsBound"": true,
  ""$Parameter"": [
    {
      ""$Name"": ""param1"",
      ""$Nullable"": true
    }
  ],
  ""$ReturnType"": {
    ""$Nullable"": true
  }
}", true, false);
        }

        [Fact]
        public void VerifyActionWithAnnotationsWrittenCorrectly()
        {
            // Arrange
            EdmAction action = new EdmAction("NS", "DoStuff", EdmCoreModel.Instance.GetString(true), true, null);
            var parameter = action.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            this.model.AddElement(action);
            this.model.SetDescriptionAnnotation(action, "action annotation");
            this.model.SetDescriptionAnnotation(parameter, "parameter annotation");
            this.model.SetDescriptionAnnotation(action.GetReturn(), "return annotation");

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(action),
                @"<Action Name=""DoStuff"" IsBound=""true"">
  <Parameter Name=""param1"" Type=""Edm.String"">
    <Annotation Term=""Org.OData.Core.V1.Description"" String=""parameter annotation"" />
  </Parameter>
  <ReturnType Type=""Edm.String"">
    <Annotation Term=""Org.OData.Core.V1.Description"" String=""return annotation"" />
  </ReturnType>
  <Annotation Term=""Org.OData.Core.V1.Description"" String=""action annotation"" />
</Action>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(action), @"{
  ""$Kind"": ""Action"",
  ""$IsBound"": true,
  ""$Parameter"": [
    {
      ""$Name"": ""param1"",
      ""$Nullable"": true,
      ""@Org.OData.Core.V1.Description"": ""parameter annotation""
    }
  ],
  ""$ReturnType"": {
    ""$Nullable"": true,
    ""@Org.OData.Core.V1.Description"": ""return annotation""
  },
  ""@Org.OData.Core.V1.Description"": ""action annotation""
}", true, false);
        }

        [Fact]
        public void VerifySchemaWithActionsWrittenCorrectly()
        {
            // Arrange
            EdmSchema schema = new EdmSchema("NS");
            EdmAction action = new EdmAction("NS", "DoStuff", EdmCoreModel.Instance.GetString(true), true, null);
            action.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            schema.AddSchemaElement(action);

            action = new EdmAction("NS", "DoStuff", EdmCoreModel.Instance.GetString(true), true, null);
            action.AddParameter("param1", EdmCoreModel.Instance.GetInt32(true));
            schema.AddSchemaElement(action);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEdmSchema(schema, null),
                @"<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Action Name=""DoStuff"" IsBound=""true"">
    <Parameter Name=""param1"" Type=""Edm.String"" />
    <ReturnType Type=""Edm.String"" />
  </Action>
  <Action Name=""DoStuff"" IsBound=""true"">
    <Parameter Name=""param1"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.String"" />
  </Action>
</Schema>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEdmSchema(schema, null), @"{
  ""NS"": {
    ""DoStuff"": [
      {
        ""$Kind"": ""Action"",
        ""$IsBound"": true,
        ""$Parameter"": [
          {
            ""$Name"": ""param1"",
            ""$Nullable"": true
          }
        ],
        ""$ReturnType"": {
          ""$Nullable"": true
        }
      },
      {
        ""$Kind"": ""Action"",
        ""$IsBound"": true,
        ""$Parameter"": [
          {
            ""$Name"": ""param1"",
            ""$Type"": ""Edm.Int32"",
            ""$Nullable"": true
          }
        ],
        ""$ReturnType"": {
          ""$Nullable"": true
        }
      }
    ]
  }
}");
        }
        #endregion

        #region Function
        [Fact]
        public void FunctionShouldWriteOutCorrectly()
        {
            // Arrange
            var function = new EdmFunction("Default.Namespace", "Checkout", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(function),
                @"<Function Name=""Checkout"">
  <ReturnType Type=""Collection(Edm.String)"" />
</Function>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(function), @"{
  ""$Kind"": ""Function"",
  ""$ReturnType"": {
    ""$Collection"": true,
    ""$Nullable"": true
  }
}", true, false);
        }

        [Fact]
        public void VerifyFunctionWrittenCorrectly()
        {
            // Arrange
            EdmFunction function = new EdmFunction("NS", "DoStuff", EdmCoreModel.Instance.GetString(true), true, new EdmPathExpression("Customers", "Orders"), true);
            function.AddParameter("bindingParameter", EdmCoreModel.Instance.GetString(true));
            function.AddParameter("param", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(true))));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(function),
                @"<Function Name=""DoStuff"" IsBound=""true"" EntitySetPath=""Customers/Orders"" IsComposable=""true"">
  <Parameter Name=""bindingParameter"" Type=""Edm.String"" />
  <Parameter Name=""param"" Type=""Collection(Edm.Int32)"" />
  <ReturnType Type=""Edm.String"" />
</Function>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(function), @"{
  ""$Kind"": ""Function"",
  ""$IsBound"": true,
  ""$EntitySetPath"": ""Customers/Orders"",
  ""$IsComposable"": true,
  ""$Parameter"": [
    {
      ""$Name"": ""bindingParameter"",
      ""$Nullable"": true
    },
    {
      ""$Name"": ""param"",
      ""$Collection"": true,
      ""$Type"": ""Edm.Int32"",
      ""$Nullable"": true
    }
  ],
  ""$ReturnType"": {
    ""$Nullable"": true
  }
}", true, false);
        }

        [Fact]
        public void VerifyFunctionWithAnnotationsWrittenCorrectly()
        {
            // Arrange
            EdmFunction function = new EdmFunction("NS", "DoStuff", EdmCoreModel.Instance.GetString(true), true, new EdmPathExpression("Customers", "Orders"), true);
            var parameter = function.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            this.model.AddElement(function);
            this.model.SetDescriptionAnnotation(function, "action annotation");
            this.model.SetDescriptionAnnotation(parameter, "parameter annotation");
            this.model.SetDescriptionAnnotation(function.GetReturn(), "return annotation");

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(function),
                @"<Function Name=""DoStuff"" IsBound=""true"" EntitySetPath=""Customers/Orders"" IsComposable=""true"">
  <Parameter Name=""param1"" Type=""Edm.String"">
    <Annotation Term=""Org.OData.Core.V1.Description"" String=""parameter annotation"" />
  </Parameter>
  <ReturnType Type=""Edm.String"">
    <Annotation Term=""Org.OData.Core.V1.Description"" String=""return annotation"" />
  </ReturnType>
  <Annotation Term=""Org.OData.Core.V1.Description"" String=""action annotation"" />
</Function>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(function), @"{
  ""$Kind"": ""Function"",
  ""$IsBound"": true,
  ""$EntitySetPath"": ""Customers/Orders"",
  ""$IsComposable"": true,
  ""$Parameter"": [
    {
      ""$Name"": ""param1"",
      ""$Nullable"": true,
      ""@Org.OData.Core.V1.Description"": ""parameter annotation""
    }
  ],
  ""$ReturnType"": {
    ""$Nullable"": true,
    ""@Org.OData.Core.V1.Description"": ""return annotation""
  },
  ""@Org.OData.Core.V1.Description"": ""action annotation""
}", true, false);
        }

        [Fact]
        public void VerifySchemaWithFunctionsWrittenCorrectly()
        {
            // Arrange
            EdmSchema schema = new EdmSchema("NS");
            EdmFunction function = new EdmFunction("NS", "GetStuff", EdmCoreModel.Instance.GetString(true), true, null, false);
            function.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            schema.AddSchemaElement(function);

            function = new EdmFunction("NS", "GetStuff", EdmCoreModel.Instance.GetGuid(false), true, null, false);
            function.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            function.AddParameter("param2", EdmCoreModel.Instance.GetInt32(false));
            schema.AddSchemaElement(function);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEdmSchema(schema, null),
                @"<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""GetStuff"" IsBound=""true"">
    <Parameter Name=""param1"" Type=""Edm.String"" />
    <ReturnType Type=""Edm.String"" />
  </Function>
  <Function Name=""GetStuff"" IsBound=""true"">
    <Parameter Name=""param1"" Type=""Edm.String"" />
    <Parameter Name=""param2"" Type=""Edm.Int32"" Nullable=""false"" />
    <ReturnType Type=""Edm.Guid"" Nullable=""false"" />
  </Function>
</Schema>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEdmSchema(schema, null), @"{
  ""NS"": {
    ""GetStuff"": [
      {
        ""$Kind"": ""Function"",
        ""$IsBound"": true,
        ""$Parameter"": [
          {
            ""$Name"": ""param1"",
            ""$Nullable"": true
          }
        ],
        ""$ReturnType"": {
          ""$Nullable"": true
        }
      },
      {
        ""$Kind"": ""Function"",
        ""$IsBound"": true,
        ""$Parameter"": [
          {
            ""$Name"": ""param1"",
            ""$Nullable"": true
          },
          {
            ""$Name"": ""param2"",
            ""$Type"": ""Edm.Int32""
          }
        ],
        ""$ReturnType"": {
          ""$Type"": ""Edm.Guid""
        }
      }
    ]
  }
}");
        }
        #endregion

        #region Term
        [Fact]
        public void VerifyTermWrittenCorrectly()
        {
            // Arrange
            EdmTerm term = new EdmTerm("NS", "MyAnnotation", EdmPrimitiveTypeKind.Int32, "Function,Action,EntitySet");

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(term),
                @"<Term Name=""MyAnnotation"" Type=""Edm.Int32"" AppliesTo=""Function,Action,EntitySet"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(term), @"{
  ""MyAnnotation"": {
    ""$Kind"": ""Term"",
    ""$Type"": ""Edm.Int32"",
    ""$AppliesTo"": [
      ""Function"",
      ""Action"",
      ""EntitySet""
    ],
    ""$Nullable"": true
  }
}");
        }

        [Fact]
        public void VerifyTermWithAnnotationsWrittenCorrectly()
        {
            // Arrange
            EdmTerm term = new EdmTerm("NS", "MyAnnotation", EdmPrimitiveTypeKind.Int32, "Function,Action,EntitySet");
            this.model.AddElement(term);
            this.model.SetDescriptionAnnotation(term, "term description");

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(term),
                @"<Term Name=""MyAnnotation"" Type=""Edm.Int32"" AppliesTo=""Function,Action,EntitySet"">
  <Annotation Term=""Org.OData.Core.V1.Description"" String=""term description"" />
</Term>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(term), @"{
  ""MyAnnotation"": {
    ""$Kind"": ""Term"",
    ""$Type"": ""Edm.Int32"",
    ""$AppliesTo"": [
      ""Function"",
      ""Action"",
      ""EntitySet""
    ],
    ""$Nullable"": true,
    ""@Org.OData.Core.V1.Description"": ""term description""
  }
}");
        }
        #endregion

        #region Type definition
        [Fact]
        public void VerifyTypeDefinitionWrittenCorrectly()
        {
            // Arrange
            EdmTypeDefinition definition = new EdmTypeDefinition("NS", "MyTypeDefinition", EdmPrimitiveTypeKind.PrimitiveType);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(definition),
                @"<TypeDefinition Name=""MyTypeDefinition"" UnderlyingType=""Edm.PrimitiveType"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(definition), @"{
  ""MyTypeDefinition"": {
    ""$Kind"": ""TypeDefinition"",
    ""$UnderlyingType"": ""Edm.PrimitiveType""
  }
}");
        }

        [Fact]
        public void VerifyTypeDefinitionWithMoreInformationsWrittenCorrectly()
        {
            // Arrange
            EdmTypeDefinition definition = new EdmTypeDefinition("NS", "MyTypeDefinition", EdmPrimitiveTypeKind.Int32);
            EdmTerm term = new EdmTerm("Measures", "Unit", EdmPrimitiveTypeKind.String);
            this.model.AddElement(definition);
            this.model.AddElement(term);
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(definition, term, new EdmStringConstant("Centimeters"));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            this.model.SetVocabularyAnnotation(annotation);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(definition),
                @"<TypeDefinition Name=""MyTypeDefinition"" UnderlyingType=""Edm.Int32"">
  <Annotation Term=""Measures.Unit"" String=""Centimeters"" />
</TypeDefinition>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(definition), @"{
  ""MyTypeDefinition"": {
    ""$Kind"": ""TypeDefinition"",
    ""$UnderlyingType"": ""Edm.Int32"",
    ""@Measures.Unit"": ""Centimeters""
  }
}");
        }
        #endregion

        #region Entity Container
        [Fact]
        public void VerifyEntityContainerWrittenCorrectly()
        {
            // Arrange
            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(container),
                @"<EntityContainer Name=""Default"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(container), @"{
  ""Default"": {
    ""$Kind"": ""EntityContainer""
  }
}");
        }

        [Fact]
        public void VerifyEntityContainerWithElementsWrittenCorrectly()
        {
            // Arrange
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntitySet set = new EdmEntitySet(container, "Set", entityType);
            EdmSingleton me = new EdmSingleton(container, "Me", entityType);
            container.AddElement(set);
            container.AddElement(me);

            this.model.AddElement(container);
            this.model.SetDescriptionAnnotation(container, "Desciption for EntitySet container");

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitSchemaElement(container),
                @"<EntityContainer Name=""Default"">
  <EntitySet Name=""Set"" EntityType=""NS.Entity"" />
  <Singleton Name=""Me"" Type=""NS.Entity"" />
  <Annotation Term=""Org.OData.Core.V1.Description"" String=""Desciption for EntitySet container"" />
</EntityContainer>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitSchemaElement(container), @"{
  ""Default"": {
    ""$Kind"": ""EntityContainer"",
    ""Set"": {
      ""$Collection"": true,
      ""$Type"": ""NS.Entity""
    },
    ""Me"": {
      ""$Type"": ""NS.Entity""
    },
    ""@Org.OData.Core.V1.Description"": ""Desciption for EntitySet container""
  }
}");
        }
        #endregion

        #region EntitySet
        [Fact]
        public void VerifyEntitySetWrittenCorrectly()
        {
            // Arrange
            IEdmEntityType entityType = new EdmEntityType("NS", "EntityType");
            IEdmEntityContainer container = new EdmEntityContainer("NS", "Container");
            EdmEntitySet entitySet = new EdmEntitySet(container, "Set", entityType);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new[] { entitySet }),
                @"<EntitySet Name=""Set"" EntityType=""NS.EntityType"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new[] { entitySet }),
                @"{
  ""Set"": {
    ""$Collection"": true,
    ""$Type"": ""NS.EntityType""
  }
}");

            // Act & Assert for non-indent JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new[] { entitySet }),
                @"{""Set"":{""$Collection"":true,""$Type"":""NS.EntityType""}}", false);
        }

        [Fact]
        public void VerifyEntitySetWithAllInformationsWrittenCorrectly()
        {
            // Arrange
            var person = new EdmEntityType("NS", "Person");
            var entityId = person.AddStructuralProperty("UserName", EdmCoreModel.Instance.GetString(false));
            person.AddKeys(entityId);

            var city = new EdmEntityType("NS", "City");
            var cityId = city.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            city.AddKeys(cityId);

            var countryOrRegion = new EdmEntityType("NS", "CountryOrRegion");
            var countryId = countryOrRegion.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            countryOrRegion.AddKeys(countryId);

            var complex = new EdmComplexType("NS", "Address");
            complex.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            var navP = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "City",
                    Target = city,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            var derivedComplex = new EdmComplexType("NS", "WorkAddress", complex);
            var navP2 = derivedComplex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "CountryOrRegion",
                    Target = countryOrRegion,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            person.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(complex, false));
            person.AddStructuralProperty("WorkAddress", new EdmComplexTypeReference(complex, false));
            person.AddStructuralProperty("Addresses", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complex, false))));

            model.AddElement(person);
            model.AddElement(city);
            model.AddElement(countryOrRegion);
            model.AddElement(complex);
            model.AddElement(derivedComplex);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);
            EdmEntitySet people = new EdmEntitySet(entityContainer, "People", person);
            EdmEntitySet cities = new EdmEntitySet(entityContainer, "City", city);
            EdmEntitySet countriesOrRegions = new EdmEntitySet(entityContainer, "CountryOrRegion", countryOrRegion);
            people.AddNavigationTarget(navP, cities, new EdmPathExpression("HomeAddress/City"));
            people.AddNavigationTarget(navP, cities, new EdmPathExpression("Addresses/City"));
            people.AddNavigationTarget(navP2, countriesOrRegions, new EdmPathExpression("WorkAddress/NS.WorkAddress/CountryOrRegion"));
            entityContainer.AddElement(people);
            entityContainer.AddElement(cities);
            entityContainer.AddElement(countriesOrRegions);

            EdmTerm term = new EdmTerm("UI", "ReadOnly", EdmPrimitiveTypeKind.Boolean);
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(people, term, new EdmBooleanConstant(true));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new[] { people }),
                @"<EntitySet Name=""People"" EntityType=""NS.Person"">
  <NavigationPropertyBinding Path=""Addresses/City"" Target=""City"" />
  <NavigationPropertyBinding Path=""HomeAddress/City"" Target=""City"" />
  <NavigationPropertyBinding Path=""WorkAddress/NS.WorkAddress/CountryOrRegion"" Target=""CountryOrRegion"" />
  <Annotation Term=""UI.ReadOnly"" Bool=""true"" />
</EntitySet>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new[] { people }),
                @"{
  ""People"": {
    ""$Collection"": true,
    ""$Type"": ""NS.Person"",
    ""$NavigationPropertyBinding"": {
      ""Addresses/City"": ""City"",
      ""HomeAddress/City"": ""City"",
      ""WorkAddress/NS.WorkAddress/CountryOrRegion"": ""CountryOrRegion""
    },
    ""@UI.ReadOnly"": true
  }
}");
        }


        #endregion

        #region Singleton
        [Fact]
        public void VerifySingletonWrittenCorrectly()
        {
            IEdmEntityType entityType = new EdmEntityType("NS", "EntityType");
            IEdmEntityContainer container = new EdmEntityContainer("NS", "Container");
            EdmSingleton singleton = new EdmSingleton(container, "Me", entityType);

            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new[] { singleton }),
                @"<Singleton Name=""Me"" Type=""NS.EntityType"" />");

            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new[] { singleton }), @"{
  ""Me"": {
    ""$Type"": ""NS.EntityType""
  }
}");

            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new[] { singleton }),
                @"{""Me"":{""$Type"":""NS.EntityType""}}", false);
        }

        [Fact]
        public void VerifySingletonWithAnnotationWrittenCorrectly()
        {
            var entityType = new EdmEntityType("NS", "EntityType");
            var container = new EdmEntityContainer("NS", "Container");
            var singleton = new EdmSingleton(container, "Me", entityType);
            container.AddElement(singleton);
            this.model.AddElement(entityType);
            this.model.AddElement(container);

            var enumType = new EdmEnumType("NS", "Permission", true);
            var read = enumType.AddMember("Read", new EdmEnumMemberValue(0));
            var write = enumType.AddMember("Write", new EdmEnumMemberValue(1));
            var term = new EdmTerm("NS", "MyTerm", new EdmEnumTypeReference(enumType, true));
            this.model.AddElement(term);

            var enumMemberValue = new EdmEnumMemberExpression(read, write);
            var annotation = new EdmVocabularyAnnotation(singleton, term, enumMemberValue);
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            this.model.SetVocabularyAnnotation(annotation);

            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new[] { singleton }),
                @"<Singleton Name=""Me"" Type=""NS.EntityType"">
  <Annotation Term=""NS.MyTerm"">
    <EnumMember>NS.Permission/Read NS.Permission/Write</EnumMember>
  </Annotation>
</Singleton>");

            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new[] { singleton }), @"{
  ""Me"": {
    ""$Type"": ""NS.EntityType"",
    ""@NS.MyTerm"": ""Read,Write""
  }
}");
        }

        #endregion

        #region Action Import

        [Fact]
        public void VerifyActionImportWrittenCorrectly()
        {
            // Arrange
            var actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImport }),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImport }),
                @"{
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut""
  }
}");

            // Act & Assert for non-indent JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImport }),
                @"{""Checkout"":{""$Kind"":""ActionImport"",""$Action"":""Default.NameSpace2.CheckOut""}}", false);
        }

        [Fact]
        public void VerifyTwoIdenticalNamedActionImportsWithNoEntitySetPropertyOnlyWrittenOnce()
        {
            // Arrange
            var actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);
            var actionImport2 = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImport, actionImport2 }),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImport, actionImport2 }),
                @"{
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut""
  }
}");
        }

        [Fact]
        public void VerifyTwoIdenticalNamedActionImportsWithSameEntitySetOnlyWrittenOnce()
        {
            // Arrange
            var actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("Set"));
            var actionImport2 = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("Set"));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImport, actionImport2 }),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""Set"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImport, actionImport2 }),
                @"{
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut"",
    ""$EntitySet"": ""Set""
  }
}");
        }

        [Fact]
        public void VerifyTwoIdenticalNamedActionImportsWithSameEdmPathOnlyWrittenOnce()
        {
            // Arrange
            var actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("path1", "path2"));
            var actionImport2 = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("path1", "path2"));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImport, actionImport2 }),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""path1/path2"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImport, actionImport2 }),
                @"{
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut"",
    ""$EntitySet"": ""path1/path2""
  }
}");
        }

        /// <summary>
        /// From OData Spec, it should be invalid to have overload action import.
        /// Need to check with OData TC.
        /// </summary>
        [Fact]
        public void VerifyIdenticalNamedActionImportsWithDifferentEntitySetPropertiesAreWritten()
        {
            // Arrange
            var actionImportOnSet = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("Set"));
            var actionImportOnSet2 = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("Set2"));
            var actionImportWithNoEntitySet = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);
            var actionImportWithUniqueEdmPath = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("path1", "path2"));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImportOnSet, actionImportOnSet2, actionImportWithNoEntitySet, actionImportWithUniqueEdmPath }),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""Set"" />
<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""Set2"" />
<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" />
<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""path1/path2"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImportOnSet, actionImportOnSet2, actionImportWithNoEntitySet, actionImportWithUniqueEdmPath }),
    @"{
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut"",
    ""$EntitySet"": ""Set""
  },
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut"",
    ""$EntitySet"": ""Set2""
  },
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut""
  },
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut"",
    ""$EntitySet"": ""path1/path2""
  }
}");
        }

        #endregion

        #region Function Import
        [Fact]
        public void VerifyFunctionImportWrittenCorrectly()
        {
            // Arrange
            var functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, null, true);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport }),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" IncludeInServiceDocument=""true"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport }),
                @"{
  ""GetStuff"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$IncludeInServiceDocument"": true
  }
}");

            // Act & Assert for non-indent JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport }),
                @"{""GetStuff"":{""$Kind"":""FunctionImport"",""$Function"":""Default.NameSpace2.GetStuff"",""$IncludeInServiceDocument"":true}}", false);
        }

        [Fact]
        public void VerifyTwoIdenticalNamedFunctionImportsWithoutEntitySetValueOnlyWrittenOnce()
        {
            // Arrange
            var functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, null, true);
            var functionImport2 = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, null, true);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport, functionImport2 }),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" IncludeInServiceDocument=""true"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport, functionImport2 }),
                @"{
  ""GetStuff"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$IncludeInServiceDocument"": true
  }
}");
        }

        [Fact]
        public void VerifyTwoIdenticalNamedFunctionImportsWithSameEntitySetValueOnlyWrittenOnce()
        {
            // Arrange
            var functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Set"), true);
            var functionImport2 = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Set"), true);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport, functionImport2 }),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Set"" IncludeInServiceDocument=""true"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport, functionImport2 }),
                @"{
  ""GetStuff"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$EntitySet"": ""Set"",
    ""$IncludeInServiceDocument"": true
  }
}");
        }

        [Fact]
        public void VerifyTwoIdenticalNamedFunctionImportsWithSameEntitySetPathValueOnlyWrittenOnce()
        {
            // Arrange
            var functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("path1", "path2"), true);
            var functionImport2 = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("path1", "path2"), true);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport, functionImport2 }),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""path1/path2"" IncludeInServiceDocument=""true"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport, functionImport2 }),
                @"{
  ""GetStuff"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$EntitySet"": ""path1/path2"",
    ""$IncludeInServiceDocument"": true
  }
}");
        }

        [Fact]
        public void VerifyIdenticalNamedFunctionImportsWithDifferentEntitySetPropertiesAreWritten()
        {
            // Arrange
            var functionImportOnSet = new EdmFunctionImport(defaultContainer, "Checkout", defaultGetStuffFunction, new EdmPathExpression("Set"), false);
            var functionImportOnSet2 = new EdmFunctionImport(defaultContainer, "Checkout", defaultGetStuffFunction, new EdmPathExpression("Set2"), false);
            var functionmportWithNoEntitySet = new EdmFunctionImport(defaultContainer, "Checkout", defaultGetStuffFunction, null, true);
            var functionImportWithUniqueEdmPath = new EdmFunctionImport(defaultContainer, "Checkout", defaultGetStuffFunction, new EdmPathExpression("path1", "path2"), false);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImportOnSet, functionImportOnSet2, functionmportWithNoEntitySet, functionImportWithUniqueEdmPath }),
                @"<FunctionImport Name=""Checkout"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Set"" />
<FunctionImport Name=""Checkout"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Set2"" />
<FunctionImport Name=""Checkout"" Function=""Default.NameSpace2.GetStuff"" IncludeInServiceDocument=""true"" />
<FunctionImport Name=""Checkout"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""path1/path2"" />");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImportOnSet, functionImportOnSet2, functionmportWithNoEntitySet, functionImportWithUniqueEdmPath }),
                @"{
  ""Checkout"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$EntitySet"": ""Set""
  },
  ""Checkout"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$EntitySet"": ""Set2""
  },
  ""Checkout"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$IncludeInServiceDocument"": true
  },
  ""Checkout"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$EntitySet"": ""path1/path2""
  }
}");
        }

        #endregion

        #region Out of line annotation
        [Fact]
        public void VerifyOutOfLineAnnotationWrittenCorrectly()
        {
            // Arrange
            EdmSchema schema = new EdmSchema("NS");

            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmProperty property = complexType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);

            EdmTerm term = new EdmTerm("UI", "Thumbnail", EdmPrimitiveTypeKind.Binary);
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, new EdmBinaryConstant(new byte[] { 0x4f, 0x44, 0x61, 0x74, 0x61 }));
            schema.AddVocabularyAnnotation(annotation);

            EdmTerm displayName = new EdmTerm("UI", "DisplayName", EdmPrimitiveTypeKind.Int32);
            annotation = new EdmVocabularyAnnotation(complexType, displayName, new EdmIntegerConstant(42));
            schema.AddVocabularyAnnotation(annotation);

            annotation = new EdmVocabularyAnnotation(complexType, displayName, "Tablet", new EdmIntegerConstant(88));
            schema.AddVocabularyAnnotation(annotation);

            annotation = new EdmVocabularyAnnotation(property, displayName, "Tablet", new EdmIntegerConstant(42));
            schema.AddVocabularyAnnotation(annotation);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEdmSchema(schema, null),
                @"<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""NS.ComplexType"">
    <Annotation Term=""UI.Thumbnail"" Binary=""4F44617461"" />
    <Annotation Term=""UI.DisplayName"" Int=""42"" />
    <Annotation Term=""UI.DisplayName"" Qualifier=""Tablet"" Int=""88"" />
  </Annotations>
  <Annotations Target=""NS.ComplexType/Name"">
    <Annotation Term=""UI.DisplayName"" Qualifier=""Tablet"" Int=""42"" />
  </Annotations>
</Schema>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEdmSchema(schema, null), @"{
  ""NS"": {
    ""$Annotations"": {
      ""NS.ComplexType"": {
        ""@UI.Thumbnail"": ""T0RhdGE"",
        ""@UI.DisplayName"": 42,
        ""@UI.DisplayName#Tablet"": 88
      },
      ""NS.ComplexType/Name"": {
        ""@UI.DisplayName#Tablet"": 42
      }
    }
  }
}");
        }

        [Fact]
        public void VerifyOutOfLineAnnotationForEnumMemberWrittenCorrectly()
        {
            // Arrange
            EdmSchema schema = new EdmSchema("NS");

            EdmEnumType enumType = new EdmEnumType("NS", "Color", EdmPrimitiveTypeKind.Int16, true);
            var blue = enumType.AddMember("Blue", new EdmEnumMemberValue(0));

            EdmTerm term = new EdmTerm("UI", "Thumbnail", EdmPrimitiveTypeKind.Binary);
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(enumType, term, new EdmBinaryConstant(new byte[] { 0x4f, 0x44, 0x61, 0x74, 0x61 }));
            schema.AddVocabularyAnnotation(annotation);

            EdmTerm displayName = new EdmTerm("UI", "DisplayName", EdmPrimitiveTypeKind.Int32);
            annotation = new EdmVocabularyAnnotation(blue, displayName, new EdmIntegerConstant(42));
            schema.AddVocabularyAnnotation(annotation);

            annotation = new EdmVocabularyAnnotation(blue, displayName, "Tablet", new EdmIntegerConstant(88));
            schema.AddVocabularyAnnotation(annotation);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitEdmSchema(schema, null),
                @"<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""NS.Color"">
    <Annotation Term=""UI.Thumbnail"" Binary=""4F44617461"" />
  </Annotations>
  <Annotations Target=""NS.Color/Blue"">
    <Annotation Term=""UI.DisplayName"" Int=""42"" />
    <Annotation Term=""UI.DisplayName"" Qualifier=""Tablet"" Int=""88"" />
  </Annotations>
</Schema>");

            // Act & Assert for JSON
            VisitAndVerifyJson(v => v.VisitEdmSchema(schema, null), @"{
  ""NS"": {
    ""$Annotations"": {
      ""NS.Color"": {
        ""@UI.Thumbnail"": ""T0RhdGE""
      },
      ""NS.Color/Blue"": {
        ""@UI.DisplayName"": 42,
        ""@UI.DisplayName#Tablet"": 88
      }
    }
  }
}");
        }
        #endregion

        #region Annotation
        [Fact]
        public void VerifyAnnotationWithBinaryExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("UI", "Thumbnail", EdmPrimitiveTypeKind.Binary);

            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, new EdmBinaryConstant(new byte[] { 0x4f, 0x44, 0x61, 0x74, 0x61 }));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""UI.Thumbnail"" Binary=""4F44617461"" />");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@UI.Thumbnail"": ""T0RhdGE""
}");
        }

        [Fact]
        public void VerifyAnnotationWithBooleanExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("UI", "ReadOnly", EdmPrimitiveTypeKind.Boolean);

            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, new EdmBooleanConstant(true));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""UI.ReadOnly"" Bool=""true"" />");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@UI.ReadOnly"": true
}");
        }

        [Fact]
        public void VerifyAnnotationWithDateExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("vCard", "birthDay", EdmPrimitiveTypeKind.Date);

            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, new EdmDateConstant(new Date(2019, 4, 5)));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""vCard.birthDay"" Date=""2019-04-05"" />");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@vCard.birthDay"": ""2019-04-05""
}");
        }

        [Fact]
        public void VerifyAnnotationWithDateTimeOffsetExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("UI", "LastUpdated", EdmPrimitiveTypeKind.DateTimeOffset);

            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, new EdmDateTimeOffsetConstant(new DateTimeOffset(2019, 4, 5, 13, 53, 41, TimeSpan.Zero)));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""UI.LastUpdated"" DateTimeOffset=""2019-04-05T13:53:41Z"" />");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@UI.LastUpdated"": ""2019-04-05T13:53:41Z""
}");
        }

        [Fact]
        public void VerifyAnnotationWithDecimalExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("UI", "Width", EdmPrimitiveTypeKind.Decimal);

            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, new EdmDecimalConstant(3.1415926m));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""UI.Width"" Decimal=""3.1415926"" />");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@UI.Width"": 3.1415926
}");
        }

        [Fact]
        public void VerifyAnnotationWithDurationExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("Task", "Duration", EdmPrimitiveTypeKind.Duration);

            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, new EdmDurationConstant(new TimeSpan(2, 3, 4)));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""Task.Duration"" Duration=""PT2H3M4S"" />");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@Task.Duration"": ""PT2H3M4S""
}");
        }

        [Fact]
        public void VerifyAnnotationWithEnumMemberExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");

            var enumType = new EdmEnumType("NS", "Permission", true);
            var read = enumType.AddMember("Read", new EdmEnumMemberValue(0));
            var write = enumType.AddMember("Write", new EdmEnumMemberValue(1));
            var term = new EdmTerm("Self", "HasPattern", new EdmEnumTypeReference(enumType, true));
            var annotation = new EdmVocabularyAnnotation(complexType, term, new EdmEnumMemberExpression(read, write));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""Self.HasPattern"">
  <EnumMember>NS.Permission/Read NS.Permission/Write</EnumMember>
</Annotation>");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@Self.HasPattern"": ""Read,Write""
}");
        }

        [Fact]
        public void VerifyAnnotationWithFloatExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("UI", "FloatWidth", EdmPrimitiveTypeKind.Single);

            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, new EdmFloatingConstant(3.14f));

#if NETCOREAPP3_1
            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""UI.FloatWidth"" Float=""3.140000104904175"" />");
#else
            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""UI.FloatWidth"" Float=""3.1400001049041748"" />");
#endif

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@UI.FloatWidth"": 3.140000104904175
}");
        }

        [Fact]
        public void VerifyAnnotationWithGuidExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("UI", "Id", EdmPrimitiveTypeKind.Guid);

            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, new EdmGuidConstant(new Guid("21EC2020-3AEA-1069-A2DD-08002B30309D")));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""UI.Id"" Guid=""21ec2020-3aea-1069-a2dd-08002b30309d"" />");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@UI.Id"": ""21ec2020-3aea-1069-a2dd-08002b30309d""
}");
        }

        [Fact]
        public void VerifyAnnotationWithIntegerExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("An", "Int", EdmPrimitiveTypeKind.Int32);

            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, new EdmIntegerConstant(42));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""An.Int"" Int=""42"" />");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@An.Int"": 42
}");
        }

        [Fact]
        public void VerifyAnnotationWithTimeOfDayExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("UI", "EndTime", EdmPrimitiveTypeKind.TimeOfDay);

            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, new EdmTimeOfDayConstant(new TimeOfDay(0, 21, 45, 04)));

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""UI.EndTime"" TimeOfDay=""00:21:45.0040000"" />");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@UI.EndTime"": ""00:21:45.0040000""
}");
        }

        [Fact]
        public void VerifyAnnotationWithApplyExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");

            EdmTerm term = new EdmTerm("UI", "DisplayName", EdmPrimitiveTypeKind.TimeOfDay);

            EdmFunction function = new EdmFunction("odata", "concat", EdmCoreModel.Instance.GetString(false));
            EdmApplyExpression apply = new EdmApplyExpression(function, new EdmPathExpression("ProductName"),
                new EdmPathExpression("Available/Quantity"),
                new EdmPathExpression("Available/Unit"));
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, apply);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""UI.DisplayName"">
  <Apply Function=""odata.concat"">
    <Path>ProductName</Path>
    <Path>Available/Quantity</Path>
    <Path>Available/Unit</Path>
  </Apply>
</Annotation>");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@UI.DisplayName"": {
    ""$Apply"": [
      {
        ""$Path"": ""ProductName""
      },
      {
        ""$Path"": ""Available/Quantity""
      },
      {
        ""$Path"": ""Available/Unit""
      }
    ],
    ""$Function"": ""odata.concat""
  }
}");
        }

        [Fact]
        public void VerifyAnnotationWithCastExpressionWrittenCorrectly()
        {
            // Arrange
            IEdmTypeReference typeReference = EdmCoreModel.Instance.GetDecimal(3, 4, false);
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("UI", "Threshold", typeReference);

            var operand = new EdmPathExpression("Average");
            var cast = new EdmCastExpression(operand, typeReference);
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, cast);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""UI.Threshold"">
  <Cast Type=""Edm.Decimal"" Nullable=""false"" Precision=""3"" Scale=""4"">
    <Path>Average</Path>
  </Cast>
</Annotation>");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@UI.Threshold"": {
    ""$Cast"": {
      ""$Path"": ""Average""
    },
    ""$Type"": ""Edm.Decimal"",
    ""$Precision"": 3,
    ""$Scale"": 4
  }
}");
        }

        [Fact]
        public void VerifyAnnotationWithCollectionExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("Seo", "SeoTerms",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))));

            var collection = new EdmCollectionExpression(new EdmStringConstant("Product"),
                new EdmStringConstant("Supplier"),
                new EdmStringConstant("Customer"));

            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, collection);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""Seo.SeoTerms"">
  <Collection>
    <String>Product</String>
    <String>Supplier</String>
    <String>Customer</String>
  </Collection>
</Annotation>");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@Seo.SeoTerms"": [
    ""Product"",
    ""Supplier"",
    ""Customer""
  ]
}");
        }

        [Fact]
        public void VerifyAnnotationWithIfThenElseExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");

            EdmTerm term = new EdmTerm("Person", "Gender", EdmPrimitiveTypeKind.TimeOfDay);

            EdmIfExpression ifExpression = new EdmIfExpression(new EdmPathExpression("IsFemale"),
                new EdmStringConstant("Female"),
                new EdmStringConstant("Male"));
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, ifExpression);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""Person.Gender"">
  <If>
    <Path>IsFemale</Path>
    <String>Female</String>
    <String>Male</String>
  </If>
</Annotation>");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@Person.Gender"": {
    ""$If"": [
      {
        ""$Path"": ""IsFemale""
      },
      ""Female"",
      ""Male""
    ]
  }
}");
        }

        [Fact]
        public void VerifyAnnotationWithIsOfThenElseExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");

            EdmTerm term = new EdmTerm("Self", "IsPreferredCustomer", EdmPrimitiveTypeKind.String); // It seems the type here is meanless?

            EdmIsTypeExpression isType = new EdmIsTypeExpression(new EdmPathExpression("Customer"),
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false, 42, true, true))));
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, isType);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""Self.IsPreferredCustomer"">
  <IsType Type=""Collection(Edm.String)"" MaxLength=""42"">
    <Path>Customer</Path>
  </IsType>
</Annotation>");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@Self.IsPreferredCustomer"": {
    ""$IsOf"": {
      ""$Path"": ""Customer""
    },
    ""$Collection"": true,
    ""$Type"": ""Edm.String"",
    ""$Nullable"": true,
    ""$MaxLength"": 42
  }
}");
        }

        [Fact]
        public void VerifyAnnotationWithLabeledElementExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("org.example.display", "DisplayName", EdmCoreModel.Instance.GetPrimitiveType(false));

            EdmLabeledExpression labeled = new EdmLabeledExpression("CustomerFirstName", new EdmPathExpression("FirstName"));
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, labeled);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""org.example.display.DisplayName"">
  <LabeledElement Name=""CustomerFirstName"">
    <Path>FirstName</Path>
  </LabeledElement>
</Annotation>");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@org.example.display.DisplayName"": {
    ""$Name"": ""CustomerFirstName"",
    ""$LabeledElement"": {
      ""$Path"": ""FirstName""
    }
  }
}");
        }

        [Fact]
        public void VerifyAnnotationWithLabeledElementReferenceExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("org.example.display", "DisplayName", EdmCoreModel.Instance.GetPrimitiveType(false));

            EdmLabeledExpression labeled = new EdmLabeledExpression("CustomerFirstName", new EdmPathExpression("FirstName"));
            EdmLabeledExpressionReferenceExpression expression = new EdmLabeledExpressionReferenceExpression(labeled);
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, expression);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""org.example.display.DisplayName"">
  <LabeledElementReference Name=""CustomerFirstName"" />
</Annotation>");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@org.example.display.DisplayName"": {
    ""$LabeledElementReference"": ""CustomerFirstName""
  }
}");
        }

        [Fact]
        public void VerifyAnnotationWithNullExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmTerm term = new EdmTerm("UI", "DisplayName", new EdmUntypedTypeReference(EdmCoreModel.Instance.GetUntypedType()));

            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, EdmNullExpression.Instance);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""UI.DisplayName""><Null /></Annotation>", false);

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{""@UI.DisplayName"":null}", false);
        }

        [Fact]
        public void VerifyAnnotationWithRecordExpressionWrittenCorrectly()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "Address");
            complexType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            complexType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);

            EdmTerm term = new EdmTerm("Person", "Employee", new EdmComplexTypeReference(complexType, false));
            IEdmRecordExpression record = new EdmRecordExpression(new EdmComplexTypeReference(complexType, false),
                    new EdmPropertyConstructor("Street", new EdmStringConstant("148th ave")),
                    new EdmPropertyConstructor("City", new EdmStringConstant("Redmond")));
            var annotation = new EdmVocabularyAnnotation(complexType, term, record);

            // Act & Assert for XML
            VisitAndVerifyXml(v => v.VisitVocabularyAnnotation(annotation),
                @"<Annotation Term=""Person.Employee"">
  <Record Type=""NS.Address"">
    <PropertyValue Property=""Street"" String=""148th ave"" />
    <PropertyValue Property=""City"" String=""Redmond"" />
  </Record>
</Annotation>");

            // Act & Assert for Json
            VisitAndVerifyJson(v => v.VisitVocabularyAnnotation(annotation), @"{
  ""@Person.Employee"": {
    ""$Type"": ""NS.Address"",
    ""Street"": ""148th ave"",
    ""City"": ""Redmond""
  }
}");
        }
        #endregion

        internal void VisitAndVerifyXml(Action<EdmModelCsdlSerializationVisitor> testAction, string expected, bool indent = true)
        {
            XmlWriter xmlWriter;
            MemoryStream memStream;

            Version edmxVersion = model.GetEdmxVersion();
            memStream = new MemoryStream();
            xmlWriter = XmlWriter.Create(memStream, new XmlWriterSettings()
            {
                Indent = indent,
                ConformanceLevel = ConformanceLevel.Auto
            }); ;
            var schemaWriter = new EdmModelCsdlSchemaXmlWriter(model, xmlWriter, edmxVersion);
            var visitor = new EdmModelCsdlSerializationVisitor(model, schemaWriter);

            testAction(visitor);
            xmlWriter.Flush();
            memStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(memStream);
            
            // Remove extra xml header text as its not needed.
            string result = reader.ReadToEnd().Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", string.Empty);
            Assert.Equal(expected, result);
        }

        internal void VisitAndVerifyJson(Action<EdmModelCsdlSerializationVisitor> testAction, string expected, bool indent = true, bool wrapper = true)
        {
#if NETCOREAPP3_1
            Version edmxVersion = this.model.GetEdmxVersion();

            using (MemoryStream memStream = new MemoryStream())
            {
                JsonWriterOptions options = new JsonWriterOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    Indented = indent,
                    SkipValidation = false
                };

                using (Utf8JsonWriter jsonWriter = new Utf8JsonWriter(memStream, options))
                {
                    var csdlSchemaWriter = new EdmModelCsdlSchemaJsonWriter(this.model, jsonWriter, edmxVersion);
                    var visitor = new EdmModelCsdlSerializationVisitor(this.model, csdlSchemaWriter);

                    // Use {} to wrapper the input.
                    if (wrapper)
                    {
                        jsonWriter.WriteStartObject();
                    }

                    testAction(visitor);

                    if (wrapper)
                    {
                        jsonWriter.WriteEndObject();
                    }

                    jsonWriter.Flush();
                }

                memStream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(memStream);

                string result = reader.ReadToEnd();
                Assert.Equal(expected, result);
            }
#endif
        }
    }
}
