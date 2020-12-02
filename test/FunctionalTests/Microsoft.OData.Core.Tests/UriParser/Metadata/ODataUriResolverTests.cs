//---------------------------------------------------------------------
// <copyright file="ODataUriResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Metadata
{
    /// <summary>
    /// Unit tests for ODataUriResolver
    /// </summary>
    public class ODataUriResolverTests : ExtensionTestBase
    {
        [Fact]
        public void DefaultEnableCaseInsensitiveShouldbeFalse()
        {
            ODataQueryOptionParser parser2 = new ODataQueryOptionParser(HardCodedTestModel.TestModel, null, null, new Dictionary<string, string>()) { Resolver = new ODataUriResolver() };
            Assert.False(parser2.Resolver.EnableCaseInsensitive);
        }

        [Fact]
        public void DefaultResolverShouldBeInvariant()
        {
            // Now different parsers share the same instance of the default resolver.
            ODataQueryOptionParser parser1 = new ODataQueryOptionParser(HardCodedTestModel.TestModel, null, null, new Dictionary<string, string>());
            ODataQueryOptionParser parser2 = new ODataQueryOptionParser(HardCodedTestModel.TestModel, null, null, new Dictionary<string, string>());
            parser1.Resolver.EnableCaseInsensitive = true;
            Assert.True(parser2.Resolver.EnableCaseInsensitive);
        }

        #region Enum vesus string
        [Fact]
        public void StringAsEnumTest()
        {
            Uri normalUri = new Uri("http://host/Pet2Set?$filter=PetColorPattern eq Fully.Qualified.Namespace.ColorPattern'Blue'");
            Uri unqualifiedEnumUri = new Uri("http://host/Pet2Set?$filter=PetColorPattern eq 'Blue'");

            Uri normalUriReverse = new Uri("http://host/Pet2Set?$filter=Fully.Qualified.Namespace.ColorPattern'Blue' eq PetColorPattern");
            Uri unqualifiedEnumUriReverse = new Uri("http://host/Pet2Set?$filter='Blue' eq PetColorPattern");

            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, normalUri);
            VerifyEnumVsStringFilterExpression(uriParser.ParseFilter());

            uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, normalUri)
            {
                Resolver = new StringAsEnumResolver()
            };
            VerifyEnumVsStringFilterExpression(uriParser.ParseFilter());

            uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, unqualifiedEnumUri)
            {
                Resolver = new StringAsEnumResolver()
            };
            VerifyEnumVsStringFilterExpression(uriParser.ParseFilter());

            uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, normalUriReverse);
            VerifyEnumVsStringFilterExpressionReverse(uriParser.ParseFilter());

            uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, unqualifiedEnumUriReverse)
            {
                Resolver = new StringAsEnumResolver()
            };
            VerifyEnumVsStringFilterExpressionReverse(uriParser.ParseFilter());
        }

        private static void VerifyEnumVsStringFilterExpression(FilterClause filter)
        {
            var enumtypeRef = new EdmEnumTypeReference(UriEdmHelpers.FindEnumTypeFromModel(HardCodedTestModel.TestModel, "Fully.Qualified.Namespace.ColorPattern"), true);
            var bin = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            bin.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            bin.Right.ShouldBeEnumNode(enumtypeRef.EnumDefinition(), "2");
        }

        private static void VerifyEnumVsStringFilterExpressionReverse(FilterClause filter)
        {
            var enumtypeRef = new EdmEnumTypeReference(UriEdmHelpers.FindEnumTypeFromModel(HardCodedTestModel.TestModel, "Fully.Qualified.Namespace.ColorPattern"), true);
            var bin = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            bin.Right.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            bin.Left.ShouldBeEnumNode(enumtypeRef.EnumDefinition(), "2");
        }

        [Fact]
        public void TestEnumAsStringHas()
        {
            Uri unqualifiedEnumUri = new Uri("http://host/Pet2Set?$filter=PetColorPattern has 'Blue'");

            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, unqualifiedEnumUri)
            {
                Resolver = new StringAsEnumResolver()
            };
          
            var filter = uriParser.ParseFilter();

            var enumtypeRef = new EdmEnumTypeReference(UriEdmHelpers.FindEnumTypeFromModel(HardCodedTestModel.TestModel, "Fully.Qualified.Namespace.ColorPattern"), true);
            var bin = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has);
            bin.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            bin.Right.ShouldBeStringCompatibleEnumNode(enumtypeRef.EnumDefinition(), "2");
        }

        [Fact]
        public void CustomPromotionRuleTest()
        {
            Uri query = new Uri("People?$orderby=FavoriteDate add 3", UriKind.Relative);
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, query) { Resolver = new MiscPromotionResolver() };
            var clause = parser.ParseOrderBy();
            var node = clause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add);
            Assert.True(node.TypeReference.IsBinary());
            node.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonFavoriteDateProp());
            node.Right.ShouldBeConstantQueryNode("3");
        }

        private class MiscPromotionResolver : ODataUriResolver
        {
            public override void PromoteBinaryOperandTypes(BinaryOperatorKind binaryOperatorKind, ref SingleValueNode leftNode, ref SingleValueNode rightNode, out IEdmTypeReference typeReference)
            {
                typeReference = null;

                if (binaryOperatorKind == BinaryOperatorKind.Add)
                {
                    if (leftNode.TypeReference.IsDateTimeOffset() && rightNode.TypeReference.IsInt32())
                    {
                        var constNode = rightNode as ConstantNode;
                        if (constNode != null)
                        {
                            typeReference = EdmCoreModel.Instance.GetBinary(true);
                            rightNode = new ConstantNode(constNode.Value.ToString());
                            return;
                        }
                    }
                }

                base.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
            }
        }
        #endregion

        #region enum as string in function parameter
        [Fact]
        public void TestStringAsEnumInFunctionParameter()
        {
            var uriParser = new ODataUriParser(
                Model,
                ServiceRoot,
                new Uri("http://host/GetColorCmykImport(co='Blue')"))
            {
                Resolver = new ODataUriResolver()
            };

            var path = uriParser.ParsePath();
            var parameters = path.LastSegment.ShouldBeOperationImportSegment(GetColorCmykImport)
                .ShouldHaveParameterCount(1)
                .Parameters;
            var parameter = Assert.Single(parameters);
            var constantNode = Assert.IsType<ConstantNode>(parameter.Value);
            constantNode.Value.ShouldBeODataEnumValue("TestNS.Color", "Blue");
        }

        [Fact]
        public void TestStringAsEnumInFunctionParameterWithNullValue()
        {
            var uriParser = new ODataUriParser(
                Model,
                ServiceRoot,
                new Uri("http://host/GetColorCmykImport(co=null)"))
            {
                Resolver = new StringAsEnumResolver()
            };

            var path = uriParser.ParsePath();
            path.LastSegment
                .ShouldBeOperationImportSegment(GetColorCmykImport)
                .ShouldHaveParameterCount(1)
                .ShouldHaveConstantParameter<object>("co", null);
        }

        [Fact]
        public void TestStringAsEnumInFunctionParameterOfCollectionType()
        {
            // This works even StringAsEnum not enabled.
            var uriParser = new ODataUriParser(
                Model,
                ServiceRoot,
                new Uri("http://host/GetMixedColorImport(co=['Blue', null])"))
            {
                Resolver = new ODataUriResolver()
            };

            var path = uriParser.ParsePath();
            var parameters = path.LastSegment.ShouldBeOperationImportSegment(GetMixedColor)
                .ShouldHaveParameterCount(1)
                .Parameters;
            var parameter = Assert.Single(parameters);
            var node = Assert.IsType<ConstantNode>(parameter.Value);
            var values = node.Value.ShouldBeODataCollectionValue();

            var items = values.Items.Cast<object>().ToList();
            Assert.Equal(2, items.Count);
            items[0].ShouldBeODataEnumValue("TestNS.Color", "Blue");
            Assert.Null(items[1]);
        }
        #endregion

        #region enum as string in key
        [Fact]
        public void TestStringAsEnumInKey()
        {
            this.TestStringAsEnum(
                "MoonSet(TestNS.Color'Blue')",
                "MoonSet('Blue')",
                parser => parser.ParsePath(),
                path =>
                {
                    var keySegment = Assert.IsType<KeySegment>(path.LastSegment);
                    var keyInfo = Assert.Single(keySegment.Keys);
                    Assert.Equal("color", keyInfo.Key);
                    keyInfo.Value.ShouldBeODataEnumValue("TestNS.Color", "2");
                },
                Strings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void TestStringAsEnumInKeyUsingKeyAsSegment()
        {
            this.TestStringAsEnum(
                "MoonSet/TestNS.Color'Blue'",
                "MoonSet/'Blue'",
                parser =>
                {
                    parser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash; return parser.ParsePath();
                },
                path =>
                {
                    var keySegment = Assert.IsType<KeySegment>(path.LastSegment);
                    var keyInfo = Assert.Single(keySegment.Keys);
                    Assert.Equal("color", keyInfo.Key);
                    keyInfo.Value.ShouldBeODataEnumValue("TestNS.Color", "2");
                },
                Strings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void TestStringAsEnumInNamedKey()
        {
            this.TestStringAsEnum(
                "MoonSet2(id=1,color=TestNS.Color'Blue')",
                "MoonSet2(id=1,color='Blue')",
                parser => parser.ParsePath(),
                path =>
                {
                    var keySegment = Assert.IsType<KeySegment>(path.LastSegment);
                    var keyList = keySegment.Keys.ToList();
                    Assert.Equal(2, keyList.Count);
                    var keyInfo = keyList[0];
                    Assert.Equal("color", keyInfo.Key);
                    keyInfo.Value.ShouldBeODataEnumValue("TestNS.Color", "2");
                    var keyInfo1 = keyList[1];
                    Assert.Equal("id", keyInfo1.Key);
                    Assert.Equal(1, keyInfo1.Value);
                },
                Strings.RequestUriProcessor_SyntaxError);
        }
        #endregion

        [Fact]
        public void UnqualifiedTypeNameShouldNotBeTreatedAsTypeCast()
        {
            var model = new EdmModel();
            var entityType = new EdmEntityType("NS", "Entity");
            entityType.AddKeys(entityType.AddStructuralProperty("IdStr", EdmPrimitiveTypeKind.String, false));
            var container = new EdmEntityContainer("NS", "Container");
            var set = container.AddEntitySet("Set", entityType);
            model.AddElements(new IEdmSchemaElement[] { entityType, container });

            var svcRoot = new Uri("http://host", UriKind.Absolute);
            var parseResult = new ODataUriParser(model, svcRoot, new Uri("http://host/Set/Date", UriKind.Absolute)).ParseUri();
            Assert.True(parseResult.Path.LastSegment is KeySegment);
        }

        [Fact]
        public void CaseInsensitiveEntitySetKeyNamePositional()
        {
            this.TestPos(
               "PetSet(key1=90, key2='stm')",
               "PetSet(90, 'stm')",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("key1", 90), new KeyValuePair<string, object>("key2", "stm")),
               "Segments with multiple key values must specify them in 'name=value' form.");
        }

        [Fact]
        public void ResolveOperationImportsReturnsEmptyEnumerableForNoEntityContainerInModel()
        {
            var operationImportName = "NonExistingOperationImport";
            var odataUriResolver = new ODataUriResolver { EnableCaseInsensitive = true };
            var result = odataUriResolver.ResolveOperationImports(new EdmModel(), operationImportName);
            Assert.Empty(result);
        }

        private void TestPos<TResult>(string originalStr, string caseInsensitiveStr, Func<ODataUriParser, TResult> parse, Action<TResult> verify, string errorMessage)
        {
            TestUriParserExtension(originalStr, caseInsensitiveStr, parse, verify, errorMessage, Model, parser => parser.Resolver = new Pos());
        }

        private void TestStringAsEnum<TResult>(string originalStr, string caseInsensitiveStr, Func<ODataUriParser, TResult> parse, Action<TResult> verify, string errorMessage)
        {
            TestUriParserExtension(originalStr, caseInsensitiveStr, parse, verify, errorMessage, Model, parser => parser.Resolver = new StringAsEnumResolver());
        }
    }

    class Pos : ODataUriResolver
    {
    }
}
