//---------------------------------------------------------------------
// <copyright file="SchemaJsonParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.OData.Edm.Csdl.Parsing;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using NuGet.Frameworks;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Parsing
{
    public class SchemaJsonParserTests
    {
        #region EntityContainer
        [Fact]
        public void ParseCsdlEntityContainerBasicWorksAsExpected()
        {
            string json = @"""DemoService"": {
  ""$Kind"": ""EntityContainer""
}";

            CsdlEntityContainer entityContainer = ParseCsdlSchemaElement(json, SchemaJsonParser.ParseCsdlEntityContainer);
            Assert.NotNull(entityContainer);

            Assert.Equal("DemoService", entityContainer.Name);
            Assert.Empty(entityContainer.EntitySets);
            Assert.Empty(entityContainer.Singletons);
            Assert.Empty(entityContainer.OperationImports);
        }

        [Fact]
        public void ParseCsdlEntityContainerWithMembersWorksAsExpected()
        {
            string json = @"""DemoService"": {
  ""$Kind"": ""EntityContainer"",
  ""Products"": {
     ""$Type"": ""self.Product"",
     ""$Collection"": true
    },
   ""Me"": {
     ""$Type"": ""self.User""
    },
   ""LeaveRequestApproval"": {
    ""$Action"": ""self.Approval""
  },
 ""ProductsByRating"": {
    ""$EntitySet"": ""Products"",
    ""$Function"": ""self.ProductsByRating""
  },
 ""@UI.DisplayName"": ""Supplier Directory""
}";

            CsdlEntityContainer entityContainer = ParseCsdlSchemaElement(json, SchemaJsonParser.ParseCsdlEntityContainer);
            Assert.NotNull(entityContainer);

            Assert.Equal("DemoService", entityContainer.Name);

            CsdlEntitySet entitySet = Assert.Single(entityContainer.EntitySets);
            Assert.Equal("Products", entitySet.Name);

            CsdlSingleton singleton = Assert.Single(entityContainer.Singletons);
            Assert.Equal("Me", singleton.Name);

            Assert.Equal(2, entityContainer.OperationImports.Count());
            CsdlActionImport actionImport = Assert.Single(entityContainer.OperationImports.OfType<CsdlActionImport>());
            Assert.Equal("LeaveRequestApproval", actionImport.Name);

            CsdlFunctionImport functionImport = Assert.Single(entityContainer.OperationImports.OfType<CsdlFunctionImport>());
            Assert.Equal("ProductsByRating", functionImport.Name);

            CsdlAnnotation annotation = Assert.Single(entityContainer.VocabularyAnnotations);
            Assert.Equal("UI.DisplayName", annotation.Term);
            Assert.IsType<CsdlConstantExpression>(annotation.Expression);
        }
        #endregion

        #region Structural Type
        [Fact]
        public void ParseCsdlComplexTypeWithMembersWorksAsExpected()
        {
            string json = @"""CountRestrictionsType"": {
    ""$Kind"": ""ComplexType"",
    ""Countable"": {
        ""$Type"": ""Edm.Boolean"",
        ""$DefaultValue"": true,
        ""@Core.Description"": ""Entities can be counted (only valid if targeting an entity set)""
    },
    ""NonCountableProperties"": {
        ""$Collection"": true,
        ""$Type"": ""Edm.PropertyPath"",
        ""@Core.Description"": ""Members of these collection properties cannot be counted""
    },
    ""NonCountableNavigationProperties"": {
        ""$Collection"": true,
        ""$Type"": ""Edm.NavigationPropertyPath"",
        ""@Core.Description"": ""Members of these navigation properties cannot be counted""
    }
}";

            CsdlComplexType complexType = ParseCsdlSchemaElement(json, SchemaJsonParser.ParseCsdlComplexType);
            Assert.NotNull(complexType);

            Assert.Equal("CountRestrictionsType", complexType.Name);
            Assert.False(complexType.IsAbstract);
            Assert.False(complexType.IsOpen);
            Assert.Null(complexType.BaseTypeName);
            Assert.Empty(complexType.NavigationProperties);

            Assert.Equal(3, complexType.StructuralProperties.Count());

            // Countable
            CsdlProperty countable = complexType.StructuralProperties.FirstOrDefault(p => p.Name == "Countable");
            Assert.NotNull(countable);
            CsdlPrimitiveTypeReference primitiveType = Assert.IsType<CsdlPrimitiveTypeReference>(countable.Type);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, primitiveType.Kind);
            Assert.Equal("true", countable.DefaultValue);
            Assert.IsType<CsdlConstantExpression>(Assert.Single(countable.VocabularyAnnotations).Expression);

            // NonCountableProperties
            CsdlProperty nonCountable = complexType.StructuralProperties.FirstOrDefault(p => p.Name == "NonCountableProperties");
            Assert.NotNull(nonCountable);
            CsdlExpressionTypeReference expressionType = Assert.IsType<CsdlExpressionTypeReference>(nonCountable.Type);
            CsdlNamedTypeReference namedType = Assert.IsType<CsdlNamedTypeReference>(Assert.IsType<CsdlCollectionType>(expressionType.TypeExpression).ElementType);
            Assert.Equal("Edm.PropertyPath", namedType.FullName);
            Assert.Null(nonCountable.DefaultValue);
            Assert.IsType<CsdlConstantExpression>(Assert.Single(nonCountable.VocabularyAnnotations).Expression);

            // NonCountableNavigationProperties
            CsdlProperty nonCountableNav = complexType.StructuralProperties.FirstOrDefault(p => p.Name == "NonCountableNavigationProperties");
            Assert.NotNull(nonCountableNav);
            expressionType = Assert.IsType<CsdlExpressionTypeReference>(nonCountableNav.Type);
            namedType = Assert.IsType<CsdlNamedTypeReference>(Assert.IsType<CsdlCollectionType>(expressionType.TypeExpression).ElementType);
            Assert.Equal("Edm.NavigationPropertyPath", namedType.FullName);
            Assert.Null(nonCountableNav.DefaultValue);
            Assert.IsType<CsdlConstantExpression>(Assert.Single(nonCountableNav.VocabularyAnnotations).Expression);
        }

        [Fact]
        public void ParseCsdlEntityTypeWithMembersWorksAsExpected()
        {
            string json = @"""Supplier"": {
  ""$Kind"": ""EntityType"",
  ""$Key"": [
    ""ID""
  ],
  ""ID"": {},
  ""Name"": {
    ""$Nullable"": true
  },
  ""Address"": {
    ""$Type"": ""self.Address""
  },
  ""Concurrency"": {
    ""$Type"": ""Edm.Int32""
  },
  ""Products"": {
    ""$Kind"": ""NavigationProperty"",
    ""$Partner"": ""Supplier"",
    ""$Collection"": true,
    ""$Type"": ""self.Product""
  }
}";

            CsdlEntityType entityType = ParseCsdlSchemaElement(json, SchemaJsonParser.ParseCsdlEntityType);
            Assert.NotNull(entityType);

            Assert.Equal("Supplier", entityType.Name);
            Assert.False(entityType.IsAbstract);
            Assert.False(entityType.IsOpen);
            Assert.Null(entityType.BaseTypeName);
            Assert.False(entityType.HasStream);

            CsdlPropertyReference propertyRef = Assert.Single(entityType.Key.Properties);
            Assert.Equal("ID", propertyRef.PropertyName);

            Assert.Equal(4, entityType.StructuralProperties.Count());
            // ID
            CsdlProperty id = entityType.StructuralProperties.FirstOrDefault(p => p.Name == "ID");
            Assert.NotNull(id);
            CsdlStringTypeReference stringType = Assert.IsType<CsdlStringTypeReference>(id.Type);
            Assert.False(stringType.IsUnbounded);
            Assert.Null(stringType.IsUnicode);
            Assert.Null(stringType.MaxLength);
            Assert.False(stringType.IsNullable);

            // Name
            CsdlProperty name = entityType.StructuralProperties.FirstOrDefault(p => p.Name == "Name");
            Assert.NotNull(name);
            stringType = Assert.IsType<CsdlStringTypeReference>(name.Type);
            Assert.True(stringType.IsNullable);

            // Address
            CsdlProperty address = entityType.StructuralProperties.FirstOrDefault(p => p.Name == "Address");
            Assert.NotNull(address);
            CsdlNamedTypeReference namedType = Assert.IsType<CsdlNamedTypeReference>(address.Type);
            Assert.Equal("self.Address", namedType.FullName);

            // Concurrency
            CsdlProperty concurrency = entityType.StructuralProperties.FirstOrDefault(p => p.Name == "Concurrency");
            Assert.NotNull(concurrency);
            CsdlPrimitiveTypeReference primitiveType = Assert.IsType<CsdlPrimitiveTypeReference>(concurrency.Type);
            Assert.Equal(EdmPrimitiveTypeKind.Int32, primitiveType.Kind);
            Assert.False(primitiveType.IsNullable);

            // Products
            CsdlNavigationProperty products = Assert.Single(entityType.NavigationProperties);
            Assert.Equal("Products", products.Name);
            Assert.Equal("Collection(self.Product)", products.Type);
            Assert.Equal("Supplier", products.PartnerPath.Path);
        }

        #endregion

        #region Enum Type
        [Fact]
        public void ParseCsdlEnumTypeWithMembersWorksAsExpected()
        {
            string json = @"""IsolationLevel"": {
  ""$Kind"": ""EnumType"",
  ""$IsFlags"": true,
  ""Snapshot"": 1,
  ""Snapshot@Core.DisplayName"": ""All data returned for a request"",
 ""@Core.Description"": ""Supported isolation levels""
        }";

            CsdlEnumType enumType = ParseCsdlSchemaElement(json, SchemaJsonParser.ParseCsdlEnumType);
            Assert.NotNull(enumType);

            Assert.Equal("IsolationLevel", enumType.Name);
            Assert.True(enumType.IsFlags);
            CsdlAnnotation annotation = Assert.Single(enumType.VocabularyAnnotations);
            Assert.Equal("Core.Description", annotation.Term);
            CsdlConstantExpression descripExp = Assert.IsType<CsdlConstantExpression>(annotation.Expression);
            Assert.Equal("Supported isolation levels", descripExp.Value);

            CsdlEnumMember enumMember = Assert.Single(enumType.Members);
            Assert.Equal("Snapshot", enumMember.Name);
            Assert.Equal(1, enumMember.Value.Value);

            annotation = Assert.Single(enumMember.VocabularyAnnotations);
            Assert.Equal("Core.DisplayName", annotation.Term);
            CsdlConstantExpression displayNameExp = Assert.IsType<CsdlConstantExpression>(annotation.Expression);
            Assert.Equal("All data returned for a request", displayNameExp.Value);
        }

        #endregion

        #region TypeDefinition
        [Fact]
        public void ParseCsdlTypeDefinitionWithMembersWorksAsExpected()
        {
            string json = @"""FilterExpressionType"": {
  ""$Kind"": ""TypeDefinition"",
  ""$UnderlyingType"": ""Edm.String"",
  ""@Validation.AllowedValues"": [
      {
        ""Value"": ""SingleValue""
      },
      {
         ""Value"": ""MultiValue""
      }
  ]
}";

            CsdlTypeDefinition typeDefinition = ParseCsdlSchemaElement(json, SchemaJsonParser.ParseCsdlTypeDefinition);
            Assert.NotNull(typeDefinition);

            Assert.Equal("FilterExpressionType", typeDefinition.Name);
            Assert.Equal("Edm.String", typeDefinition.UnderlyingTypeName);

            CsdlAnnotation annotation = Assert.Single(typeDefinition.VocabularyAnnotations);
            Assert.Equal("Validation.AllowedValues", annotation.Term);
            CsdlCollectionExpression collectionExp = Assert.IsType<CsdlCollectionExpression>(annotation.Expression);
            Assert.Equal(2, collectionExp.ElementValues.Count());
        }

        #endregion

        #region Term
        [Fact]
        public void ParseCsdlTermWithMembersWorksAsExpected()
        {
            string json =  @"""ConformanceLevel"": {
  ""$Kind"": ""Term"",
  ""$Type"": ""Capabilities.ConformanceLevelType"",
  ""$Nullable"": true,
  ""$AppliesTo"": [
    ""EntityContainer""
  ],
  ""@Core.Description"": ""The conformance level achieved by this service""
  }";

            CsdlTerm term = ParseCsdlSchemaElement(json, SchemaJsonParser.ParseCsdlTermType);
            Assert.NotNull(term);

            Assert.Equal("ConformanceLevel", term.Name);

            Assert.Equal("EntityContainer", term.AppliesTo);
            Assert.Null(term.DefaultValue);
            Assert.True(term.HasVocabularyAnnotations);

            CsdlNamedTypeReference namedType = Assert.IsType<CsdlNamedTypeReference>(term.Type);
            Assert.Equal("Capabilities.ConformanceLevelType", namedType.FullName);
            Assert.True(namedType.IsNullable);

            CsdlAnnotation annotation = Assert.Single(term.VocabularyAnnotations);
            Assert.Equal("Core.Description", annotation.Term);
            Assert.IsType<CsdlConstantExpression>(annotation.Expression);
        }

        #endregion

        #region Operations
        [Fact]
        public void ParseCsdlOperationForFunctionWithMembersWorksAsExpected()
        {
            string json = @"""CreatedEntities"": [
    {
        ""$Kind"": ""Function"",
        ""$Parameter"": [
            {
                ""$Name"": ""before"",
                ""$Type"": ""Edm.DateTimeOffset"",
                ""$Nullable"": true,
                ""$Precision"": 10
            },
            {
                ""$Name"": ""after"",
                ""$Type"": ""Edm.DateTimeOffset"",
                ""$Precision"": 9
            }
        ],
        ""$ReturnType"": {
            ""$Collection"": true,
            ""$Type"": ""Edm.EntityType"",
            ""$Nullable"": true
        },
        ""$IsComposable"": true
    }
]";

            IList<CsdlOperation> operations = ParseCsdlSchemaElement(json, SchemaJsonParser.TryParseCsdlOperationOverload);
            Assert.NotNull(operations);
            CsdlOperation operation = Assert.Single(operations);

            CsdlFunction function = Assert.IsType<CsdlFunction>(operation);
            Assert.Equal("CreatedEntities", function.Name);
            Assert.False(function.IsBound);
            Assert.True(function.IsComposable);
            Assert.Null(function.EntitySetPath);

            // Parameter
            Assert.Equal(2, function.Parameters.Count());
            CsdlOperationParameter parameter = function.Parameters.First(c => c.Name == "before");
            Assert.False(parameter.IsOptional);
            Assert.Null(parameter.DefaultValue);
            CsdlTemporalTypeReference temporalType = Assert.IsType<CsdlTemporalTypeReference>(parameter.Type);
            Assert.Equal(EdmPrimitiveTypeKind.DateTimeOffset, temporalType.Kind);
            Assert.True(temporalType.IsNullable);
            Assert.Equal(10, temporalType.Precision.Value);

            parameter = function.Parameters.First(c => c.Name == "after");
            Assert.False(parameter.IsOptional);
            Assert.Null(parameter.DefaultValue);
            temporalType = Assert.IsType<CsdlTemporalTypeReference>(parameter.Type);
            Assert.Equal(EdmPrimitiveTypeKind.DateTimeOffset, temporalType.Kind);
            Assert.False(temporalType.IsNullable);
            Assert.Equal(9, temporalType.Precision.Value);

            // ReturnType
            Assert.NotNull(function.Return);
            CsdlExpressionTypeReference expTypeRef = Assert.IsType<CsdlExpressionTypeReference>(operation.Return.ReturnType);
            Assert.True(expTypeRef.IsNullable);

            CsdlNamedTypeReference namedType = Assert.IsType<CsdlNamedTypeReference>(Assert.IsType<CsdlCollectionType>(expTypeRef.TypeExpression).ElementType);
            Assert.True(namedType.IsNullable); // derived from collection
            Assert.Equal("Edm.EntityType", namedType.FullName);
        }

        [Fact]
        public void ParseCsdlOperationForActionWithMembersWorksAsExpected()
        {
            string json = @"""Approval"": [
    {
        ""$Kind"": ""Action"",
        ""$IsBound"": true,
        ""$Parameter"": [
            {
                ""$Name"": ""binding"",
                ""$Type"": ""self.Order"",
                ""$Nullable"": true
            }
        ]
    }
]";

            IList<CsdlOperation> operations = ParseCsdlSchemaElement(json, SchemaJsonParser.TryParseCsdlOperationOverload);
            Assert.NotNull(operations);
            CsdlOperation operation = Assert.Single(operations);

            CsdlAction action = Assert.IsType<CsdlAction>(operation);
            Assert.Equal("Approval", action.Name);
            Assert.True(action.IsBound);
            Assert.Null(action.EntitySetPath);

            // Parameter
            CsdlOperationParameter parameter = Assert.Single(action.Parameters);
            Assert.Equal("binding", parameter.Name);
            Assert.False(parameter.IsOptional);
            Assert.Null(parameter.DefaultValue);
            CsdlNamedTypeReference namedType = Assert.IsType<CsdlNamedTypeReference>(parameter.Type);
            Assert.True(namedType.IsNullable);
            Assert.Equal("self.Order", namedType.FullName);

            // ReturnType
            Assert.Null(action.Return);
        }

        [Fact]
        public void ParseCsdlOperationWithActionAndFunctionWorksAsExpected()
        {
            string json = @"""Approval"": [
    {
        ""$Kind"": ""Action"",
        ""$IsBound"": true,
        ""$Parameter"": [
            {
                ""$Name"": ""binding"",
                ""$Type"": ""self.Order"",
                ""$Nullable"": true
            }
        ]
    },
    {
        ""$Kind"": ""Function"",
        ""$Parameter"": [
            {
                ""$Name"": ""Rating"",
                ""$Type"": ""Edm.Decimal"",
                ""$Precision"": 4,
                ""$Scale"": 1
            }
        ],
        ""$ReturnType"": {
            ""$Type"": ""self.Product""
         }
    }
]";

            IList<CsdlOperation> operations = ParseCsdlSchemaElement(json, SchemaJsonParser.TryParseCsdlOperationOverload);
            Assert.NotNull(operations);
            Assert.Equal(2, operations.Count);

            // #1 CsdlAction
            CsdlAction action = Assert.IsType<CsdlAction>(operations.First());
            Assert.Equal("Approval", action.Name);
            Assert.True(action.IsBound);
            Assert.Null(action.EntitySetPath);

            CsdlOperationParameter parameter = Assert.Single(action.Parameters);
            Assert.Equal("binding", parameter.Name);
            Assert.False(parameter.IsOptional);
            Assert.Null(parameter.DefaultValue);
            CsdlNamedTypeReference namedType = Assert.IsType<CsdlNamedTypeReference>(parameter.Type);
            Assert.True(namedType.IsNullable);
            Assert.Equal("self.Order", namedType.FullName);
            Assert.Null(action.Return);

            // #2 CsdlFunction
            CsdlFunction function = Assert.IsType<CsdlFunction>(operations.Last());
            Assert.Equal("Approval", function.Name);
            Assert.False(function.IsBound);
            Assert.Null(function.EntitySetPath);

            parameter = Assert.Single(function.Parameters);
            Assert.Equal("Rating", parameter.Name);
            Assert.False(parameter.IsOptional);
            Assert.Null(parameter.DefaultValue);
            CsdlDecimalTypeReference decimalType = Assert.IsType<CsdlDecimalTypeReference>(parameter.Type);
            Assert.False(decimalType.IsNullable);
            Assert.Equal(4, decimalType.Precision.Value);
            Assert.Equal(1, decimalType.Scale.Value);

            Assert.NotNull(function.Return);
            namedType = Assert.IsType<CsdlNamedTypeReference>(function.Return.ReturnType);
            Assert.False(namedType.IsNullable);
            Assert.Equal("self.Product", namedType.FullName);
        }
        #endregion

        #region OutOfLineAnnotations
        [Fact]
        public void ParseCsdlOutOfLineAnnotationWithMembersWorksAsExpected()
        {
            string json = @"""$Annotations"": {
   ""self.Person"": {
       ""@Core.Description#Tablet"": ""Dummy"",
       ""@UI.DisplayName#MyTablet"": {
           ""$Path"": ""FirstName""
       }
   },
   ""self.MyEntityType"": {
      ""@self.Dummy"": true,
      ""@UI.Threshold#Example73"": {
         ""$Type"": ""Edm.Decimal"",
         ""$Cast"": {
           ""$Path"": ""Average""
           }
      }
   }
}";

            IList<CsdlAnnotations> annotations = ParseCsdlSchemaElement(json, SchemaJsonParser.ParseCsdlOutOfLineAnnotations);
            Assert.NotNull(annotations);
            Assert.Equal(2, annotations.Count);

            // #1
            CsdlAnnotations csdlAnnotations = annotations.First(c => c.Target == "self.Person");
            Assert.Null(csdlAnnotations.Qualifier);
            Assert.Equal(2, csdlAnnotations.Annotations.Count());

            // #1.1
            CsdlAnnotation csdlAnnotation = csdlAnnotations.Annotations.First(c => c.Term == "Core.Description");
            Assert.Equal("Tablet", csdlAnnotation.Qualifier);
            CsdlConstantExpression constExp = Assert.IsType<CsdlConstantExpression>(csdlAnnotation.Expression);
            Assert.Equal("Dummy", constExp.Value);
            Assert.Equal(EdmValueKind.String, constExp.ValueKind);

            // #1.2
            csdlAnnotation = csdlAnnotations.Annotations.First(c => c.Term == "UI.DisplayName");
            Assert.Equal("MyTablet", csdlAnnotation.Qualifier);
            CsdlPathExpression pathExp = Assert.IsType<CsdlPathExpression>(csdlAnnotation.Expression);
            Assert.Equal("FirstName", pathExp.Path);

            // #2
            csdlAnnotations = annotations.First(c => c.Target == "self.MyEntityType");
            Assert.Null(csdlAnnotations.Qualifier);
            Assert.Equal(2, csdlAnnotations.Annotations.Count());

            // #2.1
            csdlAnnotation = csdlAnnotations.Annotations.First(c => c.Term == "self.Dummy");
            Assert.Null(csdlAnnotation.Qualifier);
            constExp = Assert.IsType<CsdlConstantExpression>(csdlAnnotation.Expression);
            Assert.Equal("true", constExp.Value);
            Assert.Equal(EdmValueKind.Boolean, constExp.ValueKind);

            // #2.2
            csdlAnnotation = csdlAnnotations.Annotations.First(c => c.Term == "UI.Threshold");
            Assert.Equal("Example73", csdlAnnotation.Qualifier);
            CsdlCastExpression castExp = Assert.IsType<CsdlCastExpression>(csdlAnnotation.Expression);
            Assert.IsType<CsdlDecimalTypeReference>(castExp.Type);

            pathExp = Assert.IsType<CsdlPathExpression>(castExp.Operand);
            Assert.Equal("Average", pathExp.Path);
        }
        #endregion

        private static T ParseCsdlSchemaElement<T>(string json, Func<string, JsonElement, JsonParserContext, T> parser)
        {
            string wrapper = "{" + json + "}";
            using (JsonDocument document = JsonDocument.Parse(wrapper))
            {
                JsonElement rootElement = document.RootElement;

                Assert.Equal(JsonValueKind.Object, rootElement.ValueKind);
                JsonProperty property = Assert.Single(rootElement.EnumerateObject());
                JsonParserContext context = new JsonParserContext();
                return parser(property.Name, property.Value, context);
            }
        }
    }
}
#endif