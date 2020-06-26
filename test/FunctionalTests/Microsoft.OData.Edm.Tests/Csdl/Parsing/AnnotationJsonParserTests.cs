//---------------------------------------------------------------------
// <copyright file="AnnotationJsonParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1
using System.Linq;
using System.Text.Json;
using Microsoft.OData.Edm.Csdl.Parsing;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Parsing
{
    public class AnnotationJsonParserTests
    {
        [Fact]
        public void ParseCsdlAnnotationWorksAsExpected()
        {
            string annotation = "\"@Measures.ISOCurrency\": \"USD\"";

            CsdlAnnotation csdlAnnotation = ParseAnnotation(annotation, out _);

            Assert.NotNull(csdlAnnotation);

            Assert.Equal("Measures.ISOCurrency", csdlAnnotation.Term);
            Assert.Null(csdlAnnotation.Qualifier);
            Assert.NotNull(csdlAnnotation.Expression);
            CsdlConstantExpression constantExp = Assert.IsType<CsdlConstantExpression>(csdlAnnotation.Expression);
            Assert.Equal(EdmValueKind.String, constantExp.ValueKind);
            Assert.Equal("USD", constantExp.Value);
        }

        [Fact]
        public void ParseCsdlAnnotationWithQualifierWorksAsExpected()
        {
            string annotation = @"""@UI.DisplayName#Tablet"": {
  ""$Path"": ""FirstName""
}";

            CsdlAnnotation csdlAnnotation = ParseAnnotation(annotation, out _);

            Assert.NotNull(csdlAnnotation);

            Assert.Equal("UI.DisplayName", csdlAnnotation.Term);
            Assert.Equal("Tablet", csdlAnnotation.Qualifier);
            Assert.NotNull(csdlAnnotation.Expression);
            CsdlPathExpression pathExp = Assert.IsType<CsdlPathExpression>(csdlAnnotation.Expression);
            Assert.Equal("FirstName", pathExp.Path);
        }

        [Fact]
        public void ParseCsdlAnnotationWorksButWithErrors()
        {
            string annotation = @"""@UI.DisplayName#Tablet"": {
  ""$Path"": [""FirstName""]
}";

            JsonParserContext context;
            CsdlAnnotation csdlAnnotation = ParseAnnotation(annotation, out context);

            Assert.NotNull(csdlAnnotation);

            Assert.Equal("UI.DisplayName", csdlAnnotation.Term);
            Assert.Equal("Tablet", csdlAnnotation.Qualifier);
            Assert.NotNull(csdlAnnotation.Expression);
            CsdlPathExpression pathExp = Assert.IsType<CsdlPathExpression>(csdlAnnotation.Expression);
            Assert.Null(pathExp.Path);

            EdmError error = Assert.Single(context.Errors);
            Assert.Equal(EdmErrorCode.UnexpectedValueKind, error.ErrorCode);
            Assert.Equal("$.@UI.DisplayName#Tablet.$Path", error.ErrorLocation.ToString());
            Assert.Equal("An unexpected 'Array' value kind was found when parsing the JSON path '$.@UI.DisplayName#Tablet.$Path'. A 'String' value kind was expected.", error.ErrorMessage);
        }

        #region Parse Expression
        [Theory]
        [InlineData(" null ", EdmValueKind.Null, null)]
        [InlineData(" 42 ", EdmValueKind.Integer, "42")]
        [InlineData(" true ", EdmValueKind.Boolean, "true")]
        [InlineData("false ", EdmValueKind.Boolean, "false")]
        [InlineData("\"abc\" ", EdmValueKind.String, "abc")]
        [InlineData("\"21EC2020-3AEA-1069-A2DD-08002B30309D\" ", EdmValueKind.String, "21EC2020-3AEA-1069-A2DD-08002B30309D")] // Guid as string
        [InlineData("\"Red, Striped\"", EdmValueKind.String, "Red, Striped")] // Enum as string
        [InlineData("\"INF\"", EdmValueKind.String, "INF")] // INF as string
        [InlineData("\"P7D\"", EdmValueKind.String, "P7D")] // Duration as string
        [InlineData("\"2000-01-01T16:00:00.000Z\"", EdmValueKind.String, "2000-01-01T16:00:00.000Z")] // DateTimeOffset as string
        [InlineData("\"2000-01-01\"", EdmValueKind.String, "2000-01-01")] // Date as string
        [InlineData("\"T0RhdGE\"", EdmValueKind.String, "T0RhdGE")] // Binary as string
        [InlineData("3.14", EdmValueKind.Decimal, "3.14")]
        [InlineData("2e-2", EdmValueKind.Decimal, "0.02")]
        [InlineData("1.7976931348623157E+308", EdmValueKind.Floating, "1.7976931348623157E+308")] // double.MaxValue
        public void ParseConstantExpressionWorksAsExpected(string json, EdmValueKind kind, string expected)
        {
            CsdlExpressionBase expressionBase = ParseExpression(json);

            Assert.NotNull(expressionBase);
            CsdlConstantExpression constExp = Assert.IsType<CsdlConstantExpression>(expressionBase);

            Assert.Equal(kind, constExp.ValueKind);
            Assert.Equal(expected, constExp.Value);
        }

        [Fact]
        public void ParseCollectionExpressionWorksAsExpected()
        {
            string json = @" [
  ""Product"",
  ""Supplier"",
  ""Customer""
]";

            CsdlExpressionBase expressionBase = ParseExpression(json);

            Assert.NotNull(expressionBase);
            CsdlCollectionExpression collectionExp = Assert.IsType<CsdlCollectionExpression>(expressionBase);
            Assert.Equal(3, collectionExp.ElementValues.Count());

            Assert.Equal(new[] { "Product", "Supplier", "Customer" },
                collectionExp.ElementValues.Select(e => ((CsdlConstantExpression)e).Value));
        }


        [Fact]
        public void ParseValuePathExpressionWorksAsExpected()
        {
            string json = @" { ""$Path"": ""@vCard.Address#work/FullName""  }";

            CsdlExpressionBase expressionBase = ParseExpression(json);

            Assert.NotNull(expressionBase);
            CsdlPathExpression pathExp = Assert.IsType<CsdlPathExpression>(expressionBase);
            Assert.Equal("@vCard.Address#work/FullName", pathExp.Path);
        }

        [Fact]
        public void ParseApplyExpressionWorksAsExpected()
        {
            string json = @" {
""$Apply"": [
    ""Product: "",
    {
                ""$Path"": ""ProductName""
    },
    ""("",
    {
               ""$Path"": ""Available/Quantity""
    },
    "" "",
    {
                ""$Path"": ""Available /Unit""
    },
    "" available)""
  ],
  ""$Function"": ""odata.concat""
}";

            CsdlExpressionBase expressionBase = ParseExpression(json);

            Assert.NotNull(expressionBase);
            CsdlApplyExpression applyExp = Assert.IsType<CsdlApplyExpression>(expressionBase);

            Assert.NotNull(applyExp.Function);
            Assert.Equal("odata.concat", applyExp.Function);

            Assert.NotNull(applyExp.Arguments);
            Assert.Equal(7, applyExp.Arguments.Count());

            // 0
            CsdlConstantExpression constantArgument = Assert.IsType<CsdlConstantExpression>(applyExp.Arguments.ElementAt(0));
            Assert.Equal("Product: ", constantArgument.Value);

            // 1
            CsdlPathExpression pathArgument = Assert.IsType<CsdlPathExpression>(applyExp.Arguments.ElementAt(1));
            Assert.Equal("ProductName", pathArgument.Path);

            // 2
            constantArgument = Assert.IsType<CsdlConstantExpression>(applyExp.Arguments.ElementAt(2));
            Assert.Equal("(", constantArgument.Value);

            // 3
            pathArgument = Assert.IsType<CsdlPathExpression>(applyExp.Arguments.ElementAt(3));
            Assert.Equal("Available/Quantity", pathArgument.Path);

            // 4
            constantArgument = Assert.IsType<CsdlConstantExpression>(applyExp.Arguments.ElementAt(4));
            Assert.Equal(" ", constantArgument.Value);

            // 5
            pathArgument = Assert.IsType<CsdlPathExpression>(applyExp.Arguments.ElementAt(5));
            Assert.Equal("Available /Unit", pathArgument.Path);

            // 6
            constantArgument = Assert.IsType<CsdlConstantExpression>(applyExp.Arguments.ElementAt(6));
            Assert.Equal(" available)", constantArgument.Value);
        }

        [Fact]
        public void ParseCastExpressionWorksAsExpected()
        {
            string json = @" {
  ""$Cast"": {
    ""$Path"": ""Average""
  },
  ""$Type"": ""Edm.Decimal"",
  ""$Precision"": 42
}";

            CsdlExpressionBase expressionBase = ParseExpression(json);

            Assert.NotNull(expressionBase);
            CsdlCastExpression castExp = Assert.IsType<CsdlCastExpression>(expressionBase);

            Assert.NotNull(castExp.Operand);
            CsdlPathExpression pathExp = Assert.IsType<CsdlPathExpression>(castExp.Operand);
            Assert.Equal("Average", pathExp.Path);

            Assert.NotNull(castExp.Type);
            CsdlDecimalTypeReference decimalType = Assert.IsType<CsdlDecimalTypeReference>(castExp.Type);
            Assert.NotNull(decimalType.Precision);
            Assert.Equal(42, decimalType.Precision.Value);
            Assert.Null(decimalType.Scale);
        }

        [Fact]
        public void ParseIfExpressionWorksAsExpected()
        {
            string json = @" {
  ""$If"": [
    {
       ""$Path"": ""IsFemale""
    },
    ""Female"",
    ""Male""
  ]
}";

            CsdlExpressionBase expressionBase = ParseExpression(json);

            Assert.NotNull(expressionBase);
            CsdlIfExpression ifExp = Assert.IsType<CsdlIfExpression>(expressionBase);

            Assert.NotNull(ifExp.Test);
            CsdlPathExpression pathExp = Assert.IsType<CsdlPathExpression>(ifExp.Test);
            Assert.Equal("IsFemale", pathExp.Path);

            Assert.NotNull(ifExp.IfTrue);
            CsdlConstantExpression trueExp = Assert.IsType<CsdlConstantExpression>(ifExp.IfTrue);
            Assert.Equal(EdmValueKind.String, trueExp.ValueKind);
            Assert.Equal("Female", trueExp.Value);

            Assert.NotNull(ifExp.IfFalse);
            CsdlConstantExpression falseExp = Assert.IsType<CsdlConstantExpression>(ifExp.IfFalse);
            Assert.Equal(EdmValueKind.String, falseExp.ValueKind);
            Assert.Equal("Male", falseExp.Value);
        }

        [Fact]
        public void ParseIsOfExpressionWorksAsExpected()
        {
            string json = @" { ""$IsOf"": {
     ""$Path"": ""FirstName""
  },
   ""$Type"": ""self.PreferredCustomer"",
   ""$Collection"": true
}";

            CsdlExpressionBase expressionBase = ParseExpression(json);

            Assert.NotNull(expressionBase);
            CsdlIsTypeExpression isTypeExp = Assert.IsType<CsdlIsTypeExpression>(expressionBase);

            Assert.NotNull(isTypeExp.Type);
            CsdlExpressionTypeReference expTypeReference = Assert.IsType<CsdlExpressionTypeReference>(isTypeExp.Type);
            CsdlCollectionType collectionType = Assert.IsType<CsdlCollectionType>(expTypeReference.TypeExpression);
            CsdlNamedTypeReference namedType = Assert.IsType<CsdlNamedTypeReference>(collectionType.ElementType);
            Assert.Equal("self.PreferredCustomer", namedType.FullName);

            Assert.NotNull(isTypeExp.Operand);
            Assert.IsType<CsdlPathExpression>(isTypeExp.Operand);
        }

        [Fact]
        public void ParseLabeledElementExpressionWorksAsExpected()
        {
            string json = @" { ""$LabeledElement"": {
     ""$Path"": ""FirstName""
  },
   ""$Name"": ""CustomerFirstName""
}";

            CsdlExpressionBase expressionBase = ParseExpression(json);

            Assert.NotNull(expressionBase);
            CsdlLabeledExpression labeledExp = Assert.IsType<CsdlLabeledExpression>(expressionBase);

            Assert.Equal("CustomerFirstName", labeledExp.Label);
            Assert.NotNull(labeledExp.Element);
            Assert.IsType<CsdlPathExpression>(labeledExp.Element);
        }

        [Fact]
        public void ParseLabeledElementReferenceExpressionWorksAsExpected()
        {
            string json = @" { ""$LabeledElementReference"": ""self.CustomerFirstName"" }";

            CsdlExpressionBase expressionBase = ParseExpression(json);

            Assert.NotNull(expressionBase);
            CsdlLabeledExpressionReferenceExpression labeledRefExp = Assert.IsType<CsdlLabeledExpressionReferenceExpression>(expressionBase);
            Assert.Equal("self.CustomerFirstName", labeledRefExp.Label);
        }

        [Fact]
        public void ParseRecordExpressionWorksAsExpected()
        {
            string json = @" {
  ""@type"": ""org.example.person.Manager"",
  ""@Core.Description"": ""Annotation on record"",
  ""GivenName"": {
    ""$Path"": ""FirstName""
  },
  ""GivenName@Core.Description"": ""Annotation on record member"",
  ""Surname"": {
    ""$Path"": ""LastName""
  },
  ""DirectSupervisor"": {
    ""$Path"": ""Manager""
  },
  ""CostCenter"": ""CostCenter Value""
}";

            CsdlExpressionBase expressionBase = ParseExpression(json, out JsonParserContext context);

            Assert.NotEmpty(context.Errors);
            Assert.Equal(2, context.Errors.Count);
            Assert.Equal("A member at JSON path '$.@Core.Description' is not supported.", context.Errors.First().ErrorMessage);
            Assert.Equal("A member at JSON path '$.GivenName@Core.Description' is not supported.", context.Errors.Last().ErrorMessage);

            Assert.NotNull(expressionBase);
            CsdlRecordExpression recordExp = Assert.IsType<CsdlRecordExpression>(expressionBase);

            Assert.NotNull(recordExp.Type);
            CsdlNamedTypeReference namedType = Assert.IsType<CsdlNamedTypeReference>(recordExp.Type);
            Assert.Equal("org.example.person.Manager", namedType.FullName);

            Assert.NotNull(recordExp.PropertyValues);
            Assert.Equal(4, recordExp.PropertyValues.Count());

            // GivenName
            CsdlPropertyValue propertyValue = recordExp.PropertyValues.FirstOrDefault(c => c.Property == "GivenName");
            Assert.NotNull(propertyValue);
            Assert.IsType<CsdlPathExpression>(propertyValue.Expression);

            // Surname
            propertyValue = recordExp.PropertyValues.FirstOrDefault(c => c.Property == "Surname");
            Assert.NotNull(propertyValue);
            Assert.IsType<CsdlPathExpression>(propertyValue.Expression);

            // DirectSupervisor
            propertyValue = recordExp.PropertyValues.FirstOrDefault(c => c.Property == "DirectSupervisor");
            Assert.NotNull(propertyValue);
            Assert.IsType<CsdlPathExpression>(propertyValue.Expression);

            // DirectSupervisor
            propertyValue = recordExp.PropertyValues.FirstOrDefault(c => c.Property == "CostCenter");
            Assert.NotNull(propertyValue);
            Assert.IsType<CsdlConstantExpression>(propertyValue.Expression);
        }
        #endregion

        private static CsdlAnnotation ParseAnnotation(string annotation, out JsonParserContext context)
        {
            string wrapper = "{" + annotation + "}";
            using (JsonDocument document = JsonDocument.Parse(wrapper))
            {
                JsonElement rootElement = document.RootElement;
                context = new JsonParserContext();

                Assert.Equal(JsonValueKind.Object, rootElement.ValueKind);

                // only one property
                JsonProperty property = Assert.Single(rootElement.EnumerateObject());

                bool ok = AnnotationJsonParser.TryParseCsdlAnnotation(property.Name, property.Value, context, out CsdlAnnotation csdlAnnotation);
                Assert.True(ok);
                return csdlAnnotation;
            }
        }

        private static CsdlExpressionBase ParseExpression(string json)
        {
            return ParseExpression(json, out _);
        }

        private static CsdlExpressionBase ParseExpression(string json, out JsonParserContext context)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement rootElement = document.RootElement;
                context = new JsonParserContext();
                return AnnotationJsonParser.ParseExpression(rootElement, context);
            }
        }
    }
}
#endif