//---------------------------------------------------------------------
// <copyright file="ODataUriResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Metadata;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Metadata
{
    /// <summary>
    /// Unit tests for ODataUriResolver
    /// </summary>
    public class ODataUriResolverTests : ExtensionTestBase
    {
        [Fact]
        public void DefaultEnableCaseInsensitiveShouldbeFalse()
        {
            ODataQueryOptionParser parser2 = new ODataQueryOptionParser(HardCodedTestModel.TestModel, null, null, new Dictionary<string, string>());
            parser2.Resolver.EnableCaseInsensitive.Should().BeFalse();
        }

        [Fact]
        public void DefaultResolverShouldBeInvariant()
        {
            ODataQueryOptionParser parser1 = new ODataQueryOptionParser(HardCodedTestModel.TestModel, null, null, new Dictionary<string, string>());
            ODataQueryOptionParser parser2 = new ODataQueryOptionParser(HardCodedTestModel.TestModel, null, null, new Dictionary<string, string>());
            parser1.Resolver.EnableCaseInsensitive = true;
            parser2.Resolver.EnableCaseInsensitive.Should().BeFalse();
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
            var bin = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            bin.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            bin.Right.ShouldBeEnumNode(new ODataEnumValue("2", enumtypeRef.FullName()));
        }

        private static void VerifyEnumVsStringFilterExpressionReverse(FilterClause filter)
        {
            var enumtypeRef = new EdmEnumTypeReference(UriEdmHelpers.FindEnumTypeFromModel(HardCodedTestModel.TestModel, "Fully.Qualified.Namespace.ColorPattern"), true);
            var bin = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            bin.Right.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            bin.Left.ShouldBeEnumNode(new ODataEnumValue("2", enumtypeRef.FullName()));
        }

        [Fact]
        public void TestEnumAsStringHas()
        {
            this.TestStringAsEnum(
               "MoonSet?$filter=color has TestNS.Color'Blue'",
               "MoonSet?$filter=color has 'Blue'",
               parser => parser.ParseFilter(),
               clause => clause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has),
               Strings.MetadataBinder_IncompatibleOperandsError("TestNS.Color", "Edm.String", "Has"));
        }

        [Fact]
        public void CustomPromotionRuleTest()
        {
            Uri query = new Uri("People?$orderby=FavoriteDate add 3", UriKind.Relative);
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, query) { Resolver = new MiscPromotionResolver() };
            var clause = parser.ParseOrderBy();
            var node = clause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add).And;
            node.TypeReference.IsBinary().Should().BeTrue();
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
            this.TestStringAsEnum(
                "GetColorCmykImport(co=TestNS.Color'Blue')",
                "GetColorCmykImport(co='Blue')",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeOperationImportSegment(GetColorCmykImport),
                Strings.MetadataBinder_CannotConvertToType("Edm.String", "TestNS.Color"));
        }

        [Fact]
        public void TestStringAsEnumInFunctionParameterWithCaseInsensitive()
        {
            this.TestStringAsEnum(
                "GetColorCmykImport(co=TestNS.Color'Blue')",
                "GetColorCmykImport(CO='Blue')",
                parser =>
                {
                    parser.Resolver.EnableCaseInsensitive = true;
                    return parser.ParsePath();
                },
                path => path.LastSegment.ShouldBeOperationImportSegment(GetColorCmykImport),
                Strings.MetadataBinder_CannotConvertToType("Edm.String", "TestNS.Color"));
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
                .And.ShouldHaveParameterCount(1)
                .And.ShouldHaveConstantParameter<object>("co", null);
        }

        [Fact]
        public void TestStringAsEnumInFunctionParameterOfCollectionType()
        {
            // This works even StringAsEnum not enabled.
            var uriParser = new ODataUriParser(
                Model,
                ServiceRoot,
                new Uri("http://host/GetMixedColorImport(co=['Blue', null])"));

            var path = uriParser.ParsePath();
            var node = path.LastSegment
                .ShouldBeOperationImportSegment(GetMixedColor)
                .And.ShouldHaveParameterCount(1)
                .And.Parameters.Single().Value.As<ConstantNode>();
            var values = node.Value.ShouldBeODataCollectionValue().And;

            var items = values.Items.Cast<object>().ToList();
            items.Count.Should().Be(2);
            items[0].ShouldBeODataEnumValue("TestNS.Color", "Blue");
            items[1].Should().BeNull();
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
                    var keyInfo = path.LastSegment.As<KeySegment>().Keys.Single();
                    keyInfo.Key.Should().Be("color");
                    keyInfo.Value.As<ConstantNode>().Value.As<ODataEnumValue>().TypeName.Should().Be("TestNS.Color");
                    keyInfo.Value.As<ConstantNode>().Value.As<ODataEnumValue>().Value.Should().Be("2");
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
                    parser.UrlConventions = ODataUrlConventions.KeyAsSegment; return parser.ParsePath();
                },
                path =>
                {
                    var keyInfo = path.LastSegment.As<KeySegment>().Keys.Single();
                    keyInfo.Key.Should().Be("color");
                    keyInfo.Value.As<ConstantNode>().Value.As<ODataEnumValue>().TypeName.Should().Be("TestNS.Color");
                    keyInfo.Value.As<ConstantNode>().Value.As<ODataEnumValue>().Value.Should().Be("2");
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
                    var keyList = path.LastSegment.As<KeySegment>().Keys.ToList();
                    keyList.Count.Should().Be(2);
                    var keyInfo = keyList[0];
                    keyInfo.Key.Should().Be("color");
                    keyInfo.Value.As<ConstantNode>().Value.As<ODataEnumValue>().TypeName.Should().Be("TestNS.Color");
                    keyInfo.Value.As<ConstantNode>().Value.As<ODataEnumValue>().Value.Should().Be("2");
                    var keyInfo1 = keyList[1];
                    keyInfo1.Key.Should().Be("id");
                    keyInfo1.Value.Should().Be(1);
                },
                Strings.RequestUriProcessor_SyntaxError);
        }
        #endregion


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
