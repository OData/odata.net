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
using Microsoft.OData.Edm.Vocabularies;
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
                Strings.BadRequest_KeyMismatch(MoonType2.FullTypeName()));
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

        private void TestPos<TResult>(string originalStr, string caseInsensitiveStr, Func<ODataUriParser, TResult> parse, Action<TResult> verify, string errorMessage)
        {
            TestUriParserExtension(originalStr, caseInsensitiveStr, parse, verify, errorMessage, Model, parser => parser.Resolver = new Pos());
        }

        private void TestStringAsEnum<TResult>(string originalStr, string caseInsensitiveStr, Func<ODataUriParser, TResult> parse, Action<TResult> verify, string errorMessage)
        {
            TestUriParserExtension(originalStr, caseInsensitiveStr, parse, verify, errorMessage, Model, parser => parser.Resolver = new StringAsEnumResolver());
        }

        #region "Resolve type"
        [Theory]
        [InlineData("Fully.Qualified.Namespace.Employee", "Employee", false, false)]
        [InlineData("fully.Qualified.nameSpace.employee", "Employee", true, false)]
        [InlineData("Fully.Qualified.Namespace.Employee", "Employee", false, true)]
        [InlineData("fully.Qualified.nameSpace.employee", "Employee", true, true)]
        public void ResolveType_ReturnsMatchingSchemaType(string input, string expectedType, bool enableCaseInsensitive, bool isImmutable)
        {
            var model = HardCodedTestModel.GetEdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var resolver = new ODataUriResolver { EnableCaseInsensitive = enableCaseInsensitive };

            IEdmSchemaType type = resolver.ResolveType(model, input);

            Assert.Equal(expectedType, type.Name);
        }

        [Theory]
        [InlineData("fully.Qualified.nameSpace.employee", false, false)]
        [InlineData("fully.Qualified.nameSpace.employ", true, false)]
        [InlineData("fully.Qualified.nameSpace.employee", false, true)]
        [InlineData("fully.Qualified.nameSpace.employ", true, true)]
        public void ResolveTypeReturnsNullIfNameDoesNotExist(string input, bool enableCaseInsensitive, bool isImmutable)
        {
            var model = HardCodedTestModel.GetEdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var resolver = new ODataUriResolver { EnableCaseInsensitive = enableCaseInsensitive };

            var result = resolver.ResolveType(model, input);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ResolveType_ThrowsErrors_IfThereAreDuplicates_WithImmutableModel(bool isImmutable)
        {
            var model = new EdmModel();
            var type1 = model.AddEntityType("NS.Models", "Person");
            type1.AddKeys(type1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type2 = model.AddEntityType("NS.Models", "person");
            type2.AddKeys(type1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var resolver = new ODataUriResolver { EnableCaseInsensitive = true };

            var exception = Assert.Throws<ODataException>(() => resolver.ResolveType(model, "ns.models.person"));
            Assert.Equal(Strings.UriParserMetadata_MultipleMatchingTypesFound("ns.models.person"), exception.Message);
        }
        #endregion

        #region "Resolve bound operations"
        [Theory]
        [InlineData("Fully.Qualified.Namespace.HasHat", false, false)]
        [InlineData("fully.Qualified.nameSpace.hashat", true, false)]
        [InlineData("Fully.Qualified.Namespace.HasHat", false, true)]
        [InlineData("fully.Qualified.nameSpace.hashat", true, true)]
        public void ResolveBoundOperations_ReturnsMatchingBoundOperations(string input, bool enableCaseInsensitive, bool isImmutable)
        {
            var model = HardCodedTestModel.GetEdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var bindingType = model.FindType("Fully.Qualified.Namespace.Person");

            var resolver = new ODataUriResolver { EnableCaseInsensitive = enableCaseInsensitive };

            IEnumerable<IEdmOperation> matches = resolver.ResolveBoundOperations(model, input, bindingType);

            Assert.NotEmpty(matches);

            var exepectedMatches = model.FindBoundOperations("Fully.Qualified.Namespace.HasHat", bindingType);
            foreach (var expectedMatch in exepectedMatches )
            {
                Assert.Contains(matches, match => expectedMatch == match);
            }
        }

        [Theory]
        [InlineData("Fully.Qualified.Namespace.DoesNotExist", false, false)]
        [InlineData("fully.Qualified.nameSpace.doestnotexist", true, false)]
        [InlineData("Fully.Qualified.Namespace.DoesNotExist", false, true)]
        [InlineData("fully.Qualified.nameSpace.doesnotexist", true, true)]
        [InlineData("Fully.Qualified.Namespace.getPersonByDate", true, true)]
        public void ResolveBoundOperations_ReturnsEmptyCollectionIfThereAreNoMatches(string input, bool enableCaseInsensitive, bool isImmutable)
        {
            var model = HardCodedTestModel.GetEdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var bindingType = model.FindType("Fully.Qualified.Namespace.Person");

            var resolver = new ODataUriResolver { EnableCaseInsensitive = enableCaseInsensitive };

            IEnumerable<IEdmOperation> matches = resolver.ResolveBoundOperations(model, input, bindingType);
            Assert.Empty(matches);
        }
        #endregion

        #region "Resolve unbound operations"
        [Theory]
        [InlineData("Fully.Qualified.Namespace.GetPersonByDate", false, false)]
        [InlineData("fully.Qualified.nameSpace.getpersonByDate", true, false)]
        [InlineData("Fully.Qualified.Namespace.GetPersonByDate", false, true)]
        [InlineData("fully.Qualified.nameSpace.getpersonByDate", true, true)]
        public void ResolveUnboundOperations_ReturnsMatchingUnboundOperations(string input, bool enableCaseInsensitive, bool isImmutable)
        {
            var model = HardCodedTestModel.GetEdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var resolver = new ODataUriResolver { EnableCaseInsensitive = enableCaseInsensitive };

            IEnumerable<IEdmOperation> matches = resolver.ResolveUnboundOperations(model, input);

            Assert.NotEmpty(matches);

            var exepectedMatches = model.FindOperations("Fully.Qualified.Namespace.GetPersonByDate");
            foreach (var expectedMatch in exepectedMatches)
            {
                Assert.Contains(matches, match => expectedMatch == match);
            }
        }

        [Theory]
        [InlineData("Fully.Qualified.Namespace.DoesNotExist", false, false)]
        [InlineData("fully.Qualified.nameSpace.doestnotexist", true, false)]
        [InlineData("Fully.Qualified.Namespace.DoesNotExist", false, true)]
        [InlineData("fully.Qualified.nameSpace.doesnotexist", true, true)]
        [InlineData("Fully.Qualified.Namespace.hasHat", true, true)]
        public void ResolveUnboundOperations_ReturnsEmptyCollectionIfThereAreNoMatches(string input, bool enableCaseInsensitive, bool isImmutable)
        {
            var model = HardCodedTestModel.GetEdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var resolver = new ODataUriResolver { EnableCaseInsensitive = enableCaseInsensitive };

            IEnumerable<IEdmOperation> matches = resolver.ResolveUnboundOperations(model, input);
            Assert.Empty(matches);
        }
        #endregion

        #region "Resolve term"
        [Theory]
        [InlineData("Fully.Qualified.Namespace.PrimitiveTerm", false, false)]
        [InlineData("fully.Qualified.nameSpace.primitiveTerm", true, false)]
        [InlineData("Fully.Qualified.Namespace.PrimitiveTerm", false, true)]
        [InlineData("fully.Qualified.nameSpace.primitiveTerm", true, true)]
        public void ResolveTerm_ReturnsMatchingTerm(string input, bool enableCaseInsensitive, bool isImmutable)
        {
            var model = HardCodedTestModel.GetEdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var resolver = new ODataUriResolver { EnableCaseInsensitive = enableCaseInsensitive };

            IEdmTerm match = resolver.ResolveTerm(model, input);

            var expectedMatch = model.FindTerm("Fully.Qualified.Namespace.PrimitiveTerm");
            Assert.Equal(expectedMatch, match);
        }

        [Theory]
        [InlineData("Fully.Qualified.Namespace.DoesNotExist", false, false)]
        [InlineData("fully.Qualified.nameSpace.doestnotexist", true, false)]
        [InlineData("Fully.Qualified.Namespace.DoesNotExist", false, true)]
        [InlineData("fully.Qualified.nameSpace.doesnotexist", true, true)]
        public void ResolveTerms_ReturnsNullIfThereIsNoMatch(string input, bool enableCaseInsensitive, bool isImmutable)
        {
            var model = HardCodedTestModel.GetEdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var resolver = new ODataUriResolver { EnableCaseInsensitive = enableCaseInsensitive };

            IEdmTerm match = resolver.ResolveTerm(model, input);
            Assert.Null(match);
        }

        [Fact]
        public void ResolveTerm_ThrowsException_IfThereAreDuplicates()
        {
            var model = new EdmModel();
            model.AddTerm("NS", "SomeTerm", EdmPrimitiveTypeKind.String);
            model.AddTerm("NS", "someTerm", EdmPrimitiveTypeKind.String);
            model.MarkAsImmutable();

            var resolver = new ODataUriResolver { EnableCaseInsensitive = true };

            var exception = Assert.Throws<ODataException>(() => resolver.ResolveTerm(model, "ns.someTerm"));
            Assert.Equal(Strings.UriParserMetadata_MultipleMatchingTypesFound("ns.someTerm"), exception.Message);
        }
        #endregion

        #region "Resolve navigation sources"
        [Theory]
        [InlineData("Shapes", false, false)]
        [InlineData("shapes", true, false)]
        [InlineData("Shapes", false, true)]
        [InlineData("shapes", true, true)]
        public void ResolveNavigationSource_ReturnsMatchingNavigationSource(string input, bool enableCaseInsensitive, bool isImmutable)
        {
            var model = HardCodedTestModel.GetEdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var resolver = new ODataUriResolver { EnableCaseInsensitive = enableCaseInsensitive };

            IEdmNavigationSource result = resolver.ResolveNavigationSource(model, input);

            Assert.Equal("Shapes", result.Name);
        }

        [Theory]
        [InlineData("shapes", false, false)]
        [InlineData("shape", true, false)]
        [InlineData("shapes", false, true)]
        [InlineData("shape", true, true)]
        public void ResolveNavigationSource_ReturnsNull_IfNoMatch(string input, bool enableCaseInsensitive, bool isImmutable)
        {
            var model = HardCodedTestModel.GetEdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var resolver = new ODataUriResolver { EnableCaseInsensitive = enableCaseInsensitive };

            IEdmNavigationSource result = resolver.ResolveNavigationSource(model, input);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ResolveNavigationSource_ThrowsException_IfMultipleMatchesFound(bool isImmutable)
        {
            var model = new EdmModel();
            var type = model.AddEntityType("NS", "Person");
            type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            var container = model.AddEntityContainer("NS", "Container");
            container.AddEntitySet("People", type);
            container.AddSingleton("people", type);

            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var resolver = new ODataUriResolver { EnableCaseInsensitive = true };

            var exception = Assert.Throws<ODataException>(() => resolver.ResolveNavigationSource(model, "peoPle"));
            Assert.Equal(Strings.UriParserMetadata_MultipleMatchingNavigationSourcesFound("peoPle"), exception.Message);
        }
        #endregion

        #region "Resolve Operation Imports"
        [Theory]
        [InlineData("GetMostImporantPerson", false, false)]
        [InlineData("getMostImporantperson", true, false)]
        [InlineData("GetMostImporantPerson", false, true)]
        [InlineData("getMostImporantperson", true, true)]
        public void ResolveOperationImports_ReturnsMatchingOperationImports(string input, bool enableCaseInsensitive, bool isImmutable)
        {
            var model = HardCodedTestModel.GetEdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var resolver = new ODataUriResolver { EnableCaseInsensitive = enableCaseInsensitive };

            IEnumerable<IEdmOperationImport> result = resolver.ResolveOperationImports(model, input);

            Assert.NotEmpty(result);

            var expectedMatches = model.EntityContainer.Elements.OfType<IEdmOperationImport>().Where(op => op.Name == "GetMostImporantPerson").ToList();
            foreach (var expectedMatch in expectedMatches)
            {
                Assert.Contains(result, match => expectedMatch == match);
            }

            Assert.Equal(2, expectedMatches.Count);
        }

        [Theory]
        [InlineData("getMostImporantPerson", false, false)]
        [InlineData("DoesNotExist", true, false)]
        [InlineData("DoestNotExist", false, true)]
        [InlineData("DoesNotExist", true, true)]
        public void ResolveOperationImports_ReturnsEmptyCollection_IfThereAreNoMatches(string input, bool enableCaseInsensitive, bool isImmutable)
        {
            var model = HardCodedTestModel.GetEdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var resolver = new ODataUriResolver { EnableCaseInsensitive = enableCaseInsensitive };

            IEnumerable<IEdmOperationImport> result = resolver.ResolveOperationImports(model, input);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ResolveOperationImportsReturnsEmptyEnumerableForNoEntityContainerInModel(bool isImmutable)
        {
            var model = new EdmModel();
            if (isImmutable)
            {
                model.MarkAsImmutable();
            }

            var operationImportName = "NonExistingOperationImport";
            var odataUriResolver = new ODataUriResolver { EnableCaseInsensitive = true };
            var result = odataUriResolver.ResolveOperationImports(model, operationImportName);
            Assert.Empty(result);
        }
        #endregion
    }

    class Pos : ODataUriResolver
    {
    }
}
